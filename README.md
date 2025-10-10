# 🚗 API de Veículos Minimal ASP.NET Core

Este projeto é uma API REST desenvolvida com ASP.NET Core utilizando o modelo de **Minimal APIs**, com foco em simplicidade, aprendizado e organização de código. Ele inclui autenticação via JWT, controle de acesso por perfil de usuário, testes unitários e testes de integração com MSTest, utilizando banco de dados MySQL para persistência.

---

## 📚 Objetivo

Criar uma API de veículos que permita:

- ✅ Cadastrar veículos  
- ✅ Listar todos os veículos  
- ✅ Buscar veículo por ID  
- ✅ Atualizar dados de um veículo  
- ✅ Remover veículo  
- ✅ Autenticação via JWT  
- ✅ Controle de acesso por perfil (administrador e editor)  
- ✅ Testes unitários  
- ✅ Testes de integração com MSTest  
- ✅ Banco de dados MySQL para persistência

---

## 🔐 Autenticação com JWT

A API utiliza **JSON Web Tokens (JWT)** para autenticação. A chave de assinatura está definida internamente no `appsettings.json` e não precisa ser informada ao usuário final.

### 🔑 Fluxo básico:

1. O usuário envia credenciais para o endpoint de login  
2. A API valida e retorna um token JWT  
3. O token deve ser enviado no cabeçalho `Authorization` como `Bearer <token>`  
4. Rotas protegidas exigem token válido para acesso

---

## 👮‍♂️ Perfis e Controle de Acesso

A API possui controle de acesso baseado em **perfis de usuário**, com permissões distintas para administradores e editores.

### 🔹 Permissões de Administradores:

- ✅ **Veículos**: cadastrar, listar, atualizar, editar e excluir  
- ✅ **Administradores**: fazer login, cadastrar novos administradores, buscar administradores  
- ✅ **Editores**: podem ser cadastrados por administradores  
- ✅ A verificação de permissões é feita via `Claims` no token JWT

### 🔹 Permissões de Editores:

- ✅ **Veículos**: listar e cadastrar  
- ❌ Não podem atualizar ou excluir veículos  
- ❌ Não podem gerenciar usuários

---

## 🧑‍💻 Acesso Inicial ao Sistema

Ao iniciar o projeto, um administrador padrão já está cadastrado automaticamente no banco de dados. Ele pode ser usado para realizar o primeiro login e cadastrar outros administradores ou editores.

### 🔐 Credenciais padrão:

- **Email:** `administrador@teste.com`  
- **Senha:** `123456`

---

## 🗄️ Banco de Dados MySQL

A API utiliza **MySQL** como banco de dados. Para que o projeto funcione corretamente, é necessário:

1. Ter o MySQL instalado e em execução  
2. Criar manualmente o banco com o nome definido na connection string  
3. Garantir que o usuário e senha definidos no `appsettings.json` tenham permissão de acesso

### 🔧 Connection string usada no `appsettings.json`:

```json
"ConnectionStrings": {
  "MySql": "Server=localhost;Database=minimal_api;Uid=root;Pwd=1234"
}
```

> ⚠️ O banco `minimal_api` **deve ser criado manualmente** antes de executar o projeto.  
> As tabelas serão geradas automaticamente com base nas entidades definidas no código (`Administrador`, `Veiculo`, etc.) ao iniciar a aplicação.

---

## 🧪 Testes Automatizados

O projeto inclui dois tipos de testes:

### 🔹 Testes Unitários
- Validação de regras de negócio  
- Testes isolados de métodos e serviços  
- Escritos com **MSTest**

### 🔹 Testes de Integração
- Utilizam `WebApplicationFactory` para simular o ambiente real da API  
- Validam endpoints e comportamento HTTP  
- Utilizam **banco de dados em memória** para simular persistência sem dependências externas

---

## 🧱 Estrutura do Projeto

```
/API-VEICULOS-MINIMAL-ASPNET/
├── api/
│   ├── Backup/
│   ├── bin/
│   ├── obj/
│   ├── Dominio/
│   │   ├── DTOs/
│   │   ├── Entidades/
│   │   ├── Enums/
│   │   ├── ModelViews/
│   │   ├── Services/
│   │   └── Validacoes/
│   ├── Infraestrutura/
│   ├── Properties/
│   ├── appsettings.Development.json
│   ├── appsettings.json
│   ├── minimal-api.csproj
│   └── Program.cs
├── Test/
│   ├── Integracao/
│   │   ├── Servicos/
│   │   │   ├── AdministradorServicoIntegracaoTests.cs
│   │   │   └── VeiculoServicoIntegracaoTests.cs
│   ├── Requests/
│   │   ├── AdministradorRequestTests.cs
│   │   └── VeiculoRequestTests.cs
│   └── obj/
├── .gitignore
├── minimal-api.sln
```

---

## 🚀 Como Executar Localmente

1. **Clone o repositório:**

```bash
git clone https://github.com/MarceloCarneiro100/api-veiculos-minimal-aspnet.git
cd api-veiculos-minimal-aspnet
```

2. **Crie o banco MySQL conforme a connection string**

```sql
CREATE DATABASE minimal_api;
```

3. **Execute o projeto:**

```bash
dotnet run --project api/minimal-api.csproj
```

4. A API estará disponível em:  
   `http://localhost:5183/`

---

## 🧪 Como Executar os Testes

```bash
dotnet test
```

---

## 📄 Documentação da API

A API possui documentação interativa via Swagger, disponível em:

```
http://localhost:5183/swagger
```

Acesse localmente após iniciar o projeto para visualizar todos os endpoints e testar requisições diretamente pelo navegador.

---

## 🛠 Tecnologias Utilizadas

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- ASP.NET Core Minimal API  
- C#  
- JWT (JSON Web Tokens)  
- MySQL  
- MSTest  
- WebApplicationFactory  
- Banco de dados em memória (InMemory)

---


## 📄 Projeto

Este projeto foi originalmente desenvolvido em um curso da plataforma [DIO](https://www.dio.me/) e posteriormente modificado e aprimorado em algumas partes por mim , [Marcelo Carneiro](https://github.com/MarceloCarneiro100).

## Créditos

- Plataforma original: [DIO](https://www.dio.me/).
