using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Dominio.Servicos
{
    [TestClass]
    public class VeiculoServicoTests
    {
        private DbContexto CriarContextoDeTeste()
        {
            var options = new DbContextOptionsBuilder<DbContexto>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DbContexto(options);
        }

        [TestMethod]
        public void DeveIncluirVeiculoCorretamente()
        {
            var contexto = CriarContextoDeTeste();
            var servico = new VeiculoServico(contexto);

            var veiculo = new Veiculo { Nome = "Uno", Marca = "Fiat", Ano = 2020 };
            servico.Incluir(veiculo);

            var resultado = contexto.Veiculos.FirstOrDefault();
            Assert.IsNotNull(resultado);
            Assert.AreEqual("Uno", resultado?.Nome);
        }

        [TestMethod]
        public void DeveBuscarVeiculoPorId()
        {
            var contexto = CriarContextoDeTeste();
            var servico = new VeiculoServico(contexto);

            var veiculo = new Veiculo { Id = 1, Nome = "Civic", Marca = "Honda", Ano = 2022 };
            servico.Incluir(veiculo);

            var resultado = servico.BuscaPorId(1);
            Assert.IsNotNull(resultado);
            Assert.AreEqual("Civic", resultado?.Nome);
        }

        [TestMethod]
        public void DeveAtualizarVeiculoCorretamente()
        {
            var contexto = CriarContextoDeTeste();
            var servico = new VeiculoServico(contexto);

            var veiculo = new Veiculo { Id = 1, Nome = "Gol", Marca = "VW", Ano = 2018 };
            servico.Incluir(veiculo);

            veiculo.Nome = "Gol G6";
            servico.Atualizar(veiculo);

            var resultado = servico.BuscaPorId(1);
            Assert.AreEqual("Gol G6", resultado?.Nome);
        }

        [TestMethod]
        public void DeveApagarVeiculoCorretamente()
        {
            var contexto = CriarContextoDeTeste();
            var servico = new VeiculoServico(contexto);

            var veiculo = new Veiculo { Id = 1, Nome = "Palio", Marca = "Fiat", Ano = 2015 };
            servico.Incluir(veiculo);

            servico.Apagar(veiculo);

            var resultado = servico.BuscaPorId(1);
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void DeveListarVeiculosComFiltroENome()
        {
            var contexto = CriarContextoDeTeste();
            var servico = new VeiculoServico(contexto);

            servico.Incluir(new Veiculo { Nome = "Civic", Marca = "Honda", Ano = 2020 });
            servico.Incluir(new Veiculo { Nome = "Uno", Marca = "Fiat", Ano = 2019 });
            servico.Incluir(new Veiculo { Nome = "Celta", Marca = "Chevrolet", Ano = 2018 });

            var resultado = servico.Todos(1, nome: "uno");

            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual("Uno", resultado[0].Nome);
        }
    }
}