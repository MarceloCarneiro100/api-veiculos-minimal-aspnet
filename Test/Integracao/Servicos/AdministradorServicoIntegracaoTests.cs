using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;
using Microsoft.EntityFrameworkCore;

namespace Test.Integracao.Servicos
{
    [TestClass]
    public class AdministradorServicoIntegracaoTests
    {
        [TestInitialize]
        public void LimparBaseDeDados()
        {
            var contexto = CriarContextoComBancoReal();
            contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores;");
            contexto.SaveChanges();
        }

        private DbContexto CriarContextoComBancoReal()
        {
            var options = new DbContextOptionsBuilder<DbContexto>().Options;
            return new DbContexto(options);
        }

        private AdministradorServico CriarServico()
        {
            var contexto = CriarContextoComBancoReal();
            return new AdministradorServico(contexto);
        }

        [TestMethod]
        public void DeveIncluirAdministrador()
        {
            var servico = CriarServico();

            var adm = new Administrador
            {
                Email = "adm@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            };

            var resultado = servico.Incluir(adm);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("adm@teste.com", resultado.Email);
        }

        [TestMethod]
        public void DeveBuscarAdministradorPorId()
        {
            var servico = CriarServico();

            var adm = new Administrador
            {
                Email = "buscar@teste.com",
                Senha = "abc123",
                Perfil = "Adm"
            };

            var salvo = servico.Incluir(adm);
            var buscado = servico.BuscaPorId(salvo.Id);

            Assert.IsNotNull(buscado);
            Assert.AreEqual(salvo.Id, buscado.Id);
        }

        [TestMethod]
        public void DeveRealizarLoginComCredenciaisValidas()
        {
            var servico = CriarServico();

            var adm = new Administrador
            {
                Email = "login@teste.com",
                Senha = "senha123",
                Perfil = "Adm"
            };

            servico.Incluir(adm);

            var loginDTO = new LoginDTO
            {
                Email = "login@teste.com",
                Senha = "senha123"
            };

            var resultado = servico.Login(loginDTO);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("login@teste.com", resultado.Email);
        }

        [TestMethod]
        public void DeveRetornarTodosSemPaginacao()
        {
            var servico = CriarServico();

            for (int i = 1; i <= 3; i++)
            {
                servico.Incluir(new Administrador
                {
                    Email = $"adm{i}@teste.com",
                    Senha = "123",
                    Perfil = "Adm"
                });
            }

            var resultado = servico.Todos(null);

            Assert.IsTrue(resultado.Count >= 3);
        }

        [TestMethod]
        public void DeveRetornarAdministradoresComPaginacao()
        {
            var servico = CriarServico();

            for (int i = 1; i <= 15; i++)
            {
                servico.Incluir(new Administrador
                {
                    Email = $"adm{i}@teste.com",
                    Senha = "123",
                    Perfil = "Adm"
                });
            }

            var pagina1 = servico.Todos(1);
            var pagina2 = servico.Todos(2);

            Assert.AreEqual(10, pagina1.Count);
            Assert.AreEqual(5, pagina2.Count);
        }
    }
}
