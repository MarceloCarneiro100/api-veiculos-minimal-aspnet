using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Infraestrutura.Db;

[TestClass]
public class VeiculoRequestTests
{
    private HttpClient _client = null!;
    private string _token = null!;

    [TestInitialize]
    public async Task Setup()
    {
        var factory = new WebApplicationFactory<Program>();
        _client = factory.CreateClient();

        await LimparBancoAsync(factory);
        await CriarAdministradorDiretoAsync(factory);
        _token = await AutenticarAsync("admin@email.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    }

    private async Task LimparBancoAsync(WebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var contexto = scope.ServiceProvider.GetRequiredService<DbContexto>();
        contexto.Veiculos.RemoveRange(contexto.Veiculos);
        contexto.Administradores.RemoveRange(contexto.Administradores);
        await contexto.SaveChangesAsync();
    }

    private async Task CriarAdministradorDiretoAsync(WebApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var contexto = scope.ServiceProvider.GetRequiredService<DbContexto>();

        var adm = new Administrador
        {
            Email = "admin@email.com",
            Senha = "123456",
            Perfil = "Adm"
        };

        contexto.Administradores.Add(adm);
        await contexto.SaveChangesAsync();
    }

    private async Task<string> AutenticarAsync(string email, string senha)
    {
        var login = new { Email = email, Senha = senha };
        var response = await _client.PostAsJsonAsync("/administradores/login", login);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("token").GetString()!;
    }

    private async Task<int> CriarVeiculoAsync()
    {
        var veiculo = new VeiculoDTO
        {
            Nome = "Civic",
            Marca = "Honda",
            Ano = 2020
        };

        var response = await _client.PostAsJsonAsync("/veiculos", veiculo);
        response.EnsureSuccessStatusCode();

        var criado = await response.Content.ReadFromJsonAsync<Veiculo>();
        return criado!.Id;
    }

    [TestMethod]
    public async Task DeveAtualizarVeiculoComSucesso()
    {
        var id = await CriarVeiculoAsync();

        var atualizado = new VeiculoDTO
        {
            Nome = "Corolla",
            Marca = "Toyota",
            Ano = 2021
        };

        var response = await _client.PutAsJsonAsync($"/veiculos/{id}", atualizado);
        response.EnsureSuccessStatusCode();

        var resultado = await response.Content.ReadFromJsonAsync<Veiculo>();
        Assert.IsNotNull(resultado);
        Assert.AreEqual("Corolla", resultado.Nome);
        Assert.AreEqual("Toyota", resultado.Marca);
        Assert.AreEqual(2021, resultado.Ano);
    }

    [TestMethod]
    public async Task DeveExcluirVeiculoComSucesso()
    {
        var id = await CriarVeiculoAsync();

        var delete = await _client.DeleteAsync($"/veiculos/{id}");
        delete.EnsureSuccessStatusCode();

        var busca = await _client.GetAsync($"/veiculos/{id}");
        Assert.AreEqual(HttpStatusCode.NotFound, busca.StatusCode);
    }

    [TestMethod]
    public async Task DeveRetornarNotFoundParaVeiculoInexistente()
    {
        var response = await _client.GetAsync("/veiculos/99999");
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task NaoDeveCadastrarVeiculoComAnoMuitoAntigo()
    {
        var veiculo = new VeiculoDTO
        {
            Nome = "Fusca",
            Marca = "VW",
            Ano = 1940 // inválido: menor que 1950
        };

        var response = await _client.PostAsJsonAsync("/veiculos", veiculo);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode,
            $"Esperado BadRequest para ano {veiculo.Ano}, mas veio {response.StatusCode}");
    }


    [TestMethod]
    public async Task EditorDeveConseguirListarVeiculos()
    {
        await CriarEditorDiretoAsync();
        var tokenEditor = await AutenticarAsync("editor@email.com", "123456");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenEditor);

        var response = await _client.GetAsync("/veiculos");
        response.EnsureSuccessStatusCode();
    }

    private async Task CriarEditorDiretoAsync()
    {
        using var scope = new WebApplicationFactory<Program>().Services.CreateScope();
        var contexto = scope.ServiceProvider.GetRequiredService<DbContexto>();

        var editor = new Administrador
        {
            Email = "editor@email.com",
            Senha = "123456",
            Perfil = "Editor"
        };

        contexto.Administradores.Add(editor);
        await contexto.SaveChangesAsync();
    }
}
