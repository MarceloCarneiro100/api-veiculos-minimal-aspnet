using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Validacoes;

namespace Test.Dominio.Entidades
{
    [TestClass]
    public class VeiculoTests
    {
         [TestMethod]
        public void CriarVeiculo_ComDadosValidos_DeveRetornarVeiculoCriado()
        {
            var veiculo = new Veiculo
            {
                Marca = "Fiat",
                Nome = "Uno",
                Ano = 2020
            };

            Assert.AreEqual("Fiat", veiculo.Marca);
            Assert.AreEqual("Uno", veiculo.Nome);
            Assert.AreEqual(2020, veiculo.Ano);
        }

        [TestMethod]
        public void ValidaDTO_ComMultiplosErros_DeveRetornarTodasMensagens()
        {
            var dto = new VeiculoDTO
            {
                Nome = "", 
                Marca = "", 
                Ano = 1940
            };

            var resultado = VeiculoValidador.ValidaDTO(dto);

            Assert.AreEqual(3, resultado.Mensagens.Count);
            Assert.IsTrue(resultado.Mensagens.Contains("O nome não pode estar em branco."));
            Assert.IsTrue(resultado.Mensagens.Contains("A marca não pode estar em branco."));
            Assert.IsTrue(resultado.Mensagens.Contains("Veículo muito antigo. Somente são aceitos veículos com ano igual ou superior a 1950."));
        }
    }
}