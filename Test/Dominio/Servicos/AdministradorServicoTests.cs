using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Dominio.Servicos
{
    [TestClass]
    public class AdministradorServicoTests
    {
        private DbContexto CriarContextoDeTeste()
        {
            var options = new DbContextOptionsBuilder<DbContexto>()
                             .UseInMemoryDatabase(Guid.NewGuid().ToString())
                             .Options;

            return new DbContexto(options);

        }

        [TestMethod]
        public void DeveSalvarAdministrador()
        {
            // Arrange
            var contexto = CriarContextoDeTeste();
            var administradorServico = new AdministradorServico(contexto);

            var adm = new Administrador
            {
                Id = 1,
                Email = "teste@teste.com",
                Senha = "teste",
                Perfil = "Adm"
            };

            // Act
            administradorServico.Incluir(adm);

            // Assert
            var admSalvo = contexto.Administradores.FirstOrDefault(a => a.Id == 1);

            Assert.IsNotNull(admSalvo);
            Assert.AreEqual("teste@teste.com", admSalvo?.Email);
            Assert.AreEqual("Adm", admSalvo?.Perfil);

        }

        [TestMethod]
        public void DeveBuscarAdministradorPorId()
        {
            // Arrange
            var contexto = CriarContextoDeTeste();
            var administradorServico = new AdministradorServico(contexto);

            var adm = new Administrador
            {
                Id = 1,
                Email = "teste@teste.com",
                Senha = "teste",
                Perfil = "Adm"
            };
            
            administradorServico.Incluir(adm);

            // Act
            var admSalvo = administradorServico.BuscaPorId(adm.Id);

            // Assert
            Assert.IsNotNull(admSalvo);
            Assert.AreEqual(1, admSalvo?.Id);

        }

        [TestMethod]
        public void DeveRealizarLoginComCredenciaisValidas()
        {
            // Arrange
            var contexto = CriarContextoDeTeste();
            var servico = new AdministradorServico(contexto);

            var adm = new Administrador
            {
                Id = 1,
                Email = "login@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            };

            servico.Incluir(adm);

            var loginDTO = new LoginDTO
            {
                Email = "login@teste.com",
                Senha = "123456"
            };

            // Act
            var resultado = servico.Login(loginDTO);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual("login@teste.com", resultado?.Email);
        }

        [TestMethod]
        public void DeveRetornarTodosAdministradoresSemPaginacao()
        {
            // Arrange
            var contexto = CriarContextoDeTeste();
            var servico = new AdministradorServico(contexto);

            for (int i = 1; i <= 3; i++)
            {
                servico.Incluir(new Administrador
                {
                    Id = i,
                    Email = $"adm{i}@teste.com",
                    Senha = "123",
                    Perfil = "Adm"
                });
            }

            // Act
            var resultado = servico.Todos(null);

            // Assert
            Assert.AreEqual(3, resultado.Count);
        }

        [TestMethod]
        public void DeveRetornarAdministradoresComPaginacao()
        {
            // Arrange
            var contexto = CriarContextoDeTeste();
            var servico = new AdministradorServico(contexto);

            for (int i = 1; i <= 15; i++)
            {
                servico.Incluir(new Administrador
                {
                    Id = i,
                    Email = $"adm{i}@teste.com",
                    Senha = "123",
                    Perfil = "Adm"
                });
            }

            // Act
            var resultadoPagina1 = servico.Todos(1);
            var resultadoPagina2 = servico.Todos(2);

            // Assert
            Assert.AreEqual(10, resultadoPagina1.Count);
            Assert.AreEqual(5, resultadoPagina2.Count);
        }
    }
}