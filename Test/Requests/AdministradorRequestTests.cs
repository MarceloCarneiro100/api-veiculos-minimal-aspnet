using System.Net.Http.Json;
using MinimalApi.Dominio.ModelViews;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Infraestrutura.Db;
using System.Text.Json;
using System.Net;
using MinimalApi.Dominio.Entidades;

namespace Test.Requests
{
    [TestClass]
    public class AdministradorRequestTests
    {
        private HttpClient _client = null!;

        private void LimparBaseDeDados(DbContexto contexto)
        {
            contexto.Administradores.RemoveRange(contexto.Administradores);
            contexto.SaveChanges();
        }

        [TestInitialize]
        public void Setup()
        {
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Development")
                        .UseContentRoot(Path.GetFullPath("../../../../api"));

                });

            _client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var contexto = scope.ServiceProvider.GetRequiredService<DbContexto>();

            LimparBaseDeDados(contexto);

            contexto.Administradores.Add(new Administrador
            {
                Id = -1,
                Email = "administrador@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            });

            contexto.SaveChanges();
        }

        [TestMethod]
        public async Task DeveRealizarLoginComSucesso()
        {
            var loginDTO = new
            {
                Email = "administrador@teste.com",
                Senha = "123456"
            };

            var response = await _client.PostAsJsonAsync("/administradores/login", loginDTO);

            Assert.IsTrue(response.IsSuccessStatusCode);

            var resultado = await response.Content.ReadFromJsonAsync<AdministradorLogado>();
            Assert.IsNotNull(resultado);
            Assert.AreEqual("administrador@teste.com", resultado.Email);
            Assert.IsFalse(string.IsNullOrEmpty(resultado.Token));
        }

        [TestMethod]
        public async Task NaoDeveRealizarLoginComSenhaIncorreta()
        {
            var loginDTO = new
            {
                Email = "administrador@teste.com",
                Senha = "senhaErrada"
            };

            var response = await _client.PostAsJsonAsync("/administradores/login", loginDTO);

            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task NaoDeveRealizarLoginComEmailInexistente()
        {
            var loginDTO = new
            {
                Email = "naoexiste@teste.com",
                Senha = "123456"
            };

            var response = await _client.PostAsJsonAsync("/administradores/login", loginDTO);

            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task NaoDeveRealizarLoginComCamposVazios()
        {
            var loginDTO = new
            {
                Email = "",
                Senha = ""
            };

            var response = await _client.PostAsJsonAsync("/administradores/login", loginDTO);

            // Espera Unauthorized, pois a API trata campos vazios como credenciais inválidas
            Assert.AreEqual(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task DeveListarAdministradoresComSucesso()
        {
            // Autentica e injeta o token
            var loginDTO = new { Email = "administrador@teste.com", Senha = "123456" };
            var loginResponse = await _client.PostAsJsonAsync("/administradores/login", loginDTO);
            var resultado = await loginResponse.Content.ReadFromJsonAsync<AdministradorLogado>();
            var token = resultado?.Token;

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Faz a requisição
            var response = await _client.GetAsync("/administradores");

            Assert.IsTrue(response.IsSuccessStatusCode);

            var lista = await response.Content.ReadFromJsonAsync<List<AdministradorModelView>>();
            Assert.IsNotNull(lista);
            Assert.IsTrue(lista.Count > 0);
        }

        private async Task<string> GerarTokenAdministradorAsync()
        {
            var loginDTO = new
            {
                Email = "administrador@teste.com",
                Senha = "123456"
            };

            var loginResponse = await _client.PostAsJsonAsync("/administradores/login", loginDTO);

            Assert.IsTrue(loginResponse.IsSuccessStatusCode, "Login falhou. Verifique se o usuário existe e tem perfil 'Adm'.");

            var resultado = await loginResponse.Content.ReadFromJsonAsync<AdministradorLogado>();
            Assert.IsNotNull(resultado?.Token, "Token não foi retornado.");

            return resultado.Token;
        }


        [TestMethod]
        public async Task DeveCadastrarAdministradorComSucesso()
        {
            var token = await GerarTokenAdministradorAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var novoAdm = new
            {
                Email = "novo@teste.com",
                Senha = "abc123",
                Perfil = 1
            };

            var response = await _client.PostAsJsonAsync("/administradores", novoAdm);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Conteúdo da resposta:");
            Console.WriteLine(string.IsNullOrWhiteSpace(responseContent) ? "[vazio]" : responseContent);

            if (string.IsNullOrWhiteSpace(responseContent))
            {
                Assert.Fail("Resposta veio vazia. Não foi possível desserializar.");
                return;
            }

            var criado = JsonSerializer.Deserialize<AdministradorModelView>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(criado);
            Assert.AreEqual("novo@teste.com", criado.Email);
            Assert.AreEqual("Editor", criado.Perfil);
        }

        [TestMethod]
        public async Task DeveBuscarAdministradorPorIdComSucesso()
        {
            var token = await GerarTokenAdministradorAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/administradores/-1");

            Assert.IsTrue(response.IsSuccessStatusCode);

            var administrador = await response.Content.ReadFromJsonAsync<AdministradorModelView>();
            Assert.IsNotNull(administrador);
            Assert.AreEqual(-1, administrador.Id);
            Assert.AreEqual("administrador@teste.com", administrador.Email);
            Assert.AreEqual("Adm", administrador.Perfil);
        }

        [TestMethod]
        public async Task DeveBuscarAdministradoresPaginadosComSucesso()
        {
            var token = await GerarTokenAdministradorAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/administradores?pagina=1&quantidade=5");

            Assert.IsTrue(response.IsSuccessStatusCode);

            var lista = await response.Content.ReadFromJsonAsync<List<AdministradorModelView>>();
            Assert.IsNotNull(lista);
            Assert.IsTrue(lista.Count <= 5); // Verifica se veio no máximo 5
        }
    }
}
