using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Integracao.Servicos
{
    [TestClass]
    public class VeiculoServicoIntegracaoTests
    {
        [TestInitialize]
        public void LimparBaseDeDados()
        {
            var contexto = CriarContextoComBancoReal();
            contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE Veiculos;");
            contexto.SaveChanges();
        }

        private DbContexto CriarContextoComBancoReal()
        {
            var options = new DbContextOptionsBuilder<DbContexto>().Options;
            return new DbContexto(options);
        }

        private VeiculoServico CriarServico()
        {
            var contexto = CriarContextoComBancoReal();
            return new VeiculoServico(contexto);
        }

        [TestMethod]
        public void DeveIncluirVeiculo()
        {
            var servico = CriarServico();

            var veiculo = new Veiculo
            {
                Nome = "Civic",
                Marca = "Honda",
                Ano = 2020
            };

            servico.Incluir(veiculo);

            var buscado = servico.BuscaPorId(veiculo.Id);

            Assert.IsNotNull(buscado);
            Assert.AreEqual("Civic", buscado.Nome);
        }

        [TestMethod]
        public void DeveBuscarVeiculoPorId()
        {
            var servico = CriarServico();

            var veiculo = new Veiculo
            {
                Nome = "Corolla",
                Marca = "Toyota",
                Ano = 2021
            };

            servico.Incluir(veiculo);

            var resultado = servico.BuscaPorId(veiculo.Id);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("Corolla", resultado.Nome);
        }

        [TestMethod]
        public void DeveAtualizarVeiculo()
        {
            var servico = CriarServico();

            var veiculo = new Veiculo
            {
                Nome = "Gol",
                Marca = "Volkswagen",
                Ano = 2018
            };

            servico.Incluir(veiculo);

            veiculo.Nome = "Gol G6";
            veiculo.Ano = 2019;

            servico.Atualizar(veiculo);

            var atualizado = servico.BuscaPorId(veiculo.Id);

            Assert.AreEqual("Gol G6", atualizado?.Nome);
            Assert.AreEqual(2019, atualizado?.Ano);
        }

        [TestMethod]
        public void DeveApagarVeiculo()
        {
            var servico = CriarServico();

            var veiculo = new Veiculo
            {
                Nome = "Fiesta",
                Marca = "Ford",
                Ano = 2017
            };

            servico.Incluir(veiculo);
            servico.Apagar(veiculo);

            var resultado = servico.BuscaPorId(veiculo.Id);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void DeveRetornarTodosSemPaginacao()
        {
            var servico = CriarServico();

            for (int i = 1; i <= 3; i++)
            {
                servico.Incluir(new Veiculo
                {
                    Nome = $"Carro{i}",
                    Marca = "Genérica",
                    Ano = 2020 + i
                });
            }

            var resultado = servico.Todos(null);

            Assert.IsTrue(resultado.Count >= 3);
        }

        [TestMethod]
        public void DeveRetornarVeiculosComPaginacao()
        {
            var servico = CriarServico();

            for (int i = 1; i <= 15; i++)
            {
                servico.Incluir(new Veiculo
                {
                    Nome = $"Veiculo{i}",
                    Marca = "MarcaX",
                    Ano = 2000 + i
                });
            }

            var pagina1 = servico.Todos(1);
            var pagina2 = servico.Todos(2);

            Assert.AreEqual(10, pagina1.Count);
            Assert.AreEqual(5, pagina2.Count);
        }

        [TestMethod]
        public void DeveFiltrarPorNome()
        {
            var servico = CriarServico();

            servico.Incluir(new Veiculo { Nome = "Fiat Uno", Marca = "Fiat", Ano = 2015 });
            servico.Incluir(new Veiculo { Nome = "Fiat Palio", Marca = "Fiat", Ano = 2016 });
            servico.Incluir(new Veiculo { Nome = "Civic", Marca = "Honda", Ano = 2020 });

            var resultado = servico.Todos(null, nome: "fiat");

            Assert.IsTrue(resultado.Any(v => v.Nome.Contains("Fiat")));
            Assert.IsFalse(resultado.Any(v => v.Nome == "Civic"));
        }
    }
}
