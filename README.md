## Sobre o projeto

Esta **API** foi desenvolvida com o objetivo de oferecer uma solução estruturada e confiável para o controle de faturamento de barbearias. 
Utilizando o ecossistema **.NET 8** e seguindo os princípios do **Domain-Driven Design (DDD)**, a aplicação permite registrar e gerenciar 
detalhadamente cada movimentação financeira, com informações como serviço realizado, data e hora, valor, forma de pagamento e responsável 
pelo atendimento.

Todo o armazenamento de dados é realizado de forma persistente em um banco de dados **MySQL**, garantindo a integridade e a segurança das 
informações registradas. A API também oferece a geração de relatórios em **PDF** e **Excel** com o total de faturamento semanal, permitindo análises 
rápidas e precisas sobre a saúde financeira do negócio.

A arquitetura baseia-se no modelo **REST**, utilizando os métodos HTTP padrão para uma comunicação eficiente e padronizada. Para facilitar o 
desenvolvimento e a integração, o projeto conta com o **Swagger**, que gera uma interface interativa para explorar, documentar e testar todos 
os endpoints da API de forma simplificada.

<div align="center">
  <img src="images/heroimage.png" alt="hero-image" width="450"/>
</div>

### Features
- **Domain-Driven Design (DDD)**: Arquitetura organizada em camadas, com foco na clareza do negócio e facilidade de manutenção.

- **Testes de Unidade**: Testes abrangentes com Shouldly para garantir a funcionalidade, qualidade e confiabilidade em cada entrega.

- **Business Intelligence & Reports**: Relatórios semanais exportados em PDF e Excel com totais consolidados para uma gestão financeira mais inteligente.

- **API RESTful com Swagger**: Endpoints padronizados e documentação interativa para uma integração simples e eficiente.
  
### Construído com
![badge-dot-net]
![badge-windows]
![badge-visual-studio]
![badge-mysql]
![badge-swagger]

---

# Autenticação e Uso da API

A API utiliza autenticação baseada em **Bearer Token (JWT)**. A maioria das rotas requer que o token gerado no Login seja enviado no cabeçalho (*Header*) da requisição:

```http
Authorization: Bearer <SEU_TOKEN_JWT>
```

Credenciais Padrão (Admin Seed)
Para facilitar os testes iniciais, a aplicação já possui um usuário Administrador padrão semeado no banco de dados:

E-mail: admin@barberboss.com

Senha: BarberBoss7539!

Fluxo de Autenticação e Exemplos de Requisição
1. Registrar um Novo Usuário
Endpoint público para criação de novas contas.

POST /api/Users
```json
{
  "name": "Barbeiro Silva",
  "email": "barbeiro@barberboss.com",
  "password": "Password@123"
}
```

2. Autenticação (Login)
Endpoint público para autenticação de usuários cadastrados ou do Admin padrão. Retorna o Token JWT.

POST /api/Login

Requisição:
```json
{
  "email": "admin@barberboss.com",
  "password": "Admin@123"
}
```
Resposta de Sucesso (200 OK):

```json
{
  "name": "Admin BarberBoss",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

3. Utilizando o Token no Swagger
Execute a aplicação e acesse a página do Swagger.

Clique no botão Authorize (localizado no canto superior direito).

No campo de texto, insira o token recebido no Login com o prefixo Bearer :

```plaintext
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```
Clique em Authorize e feche o modal. Suas requisições no Swagger agora estarão autenticadas.

4. Atualizar Usuário por ID (Exemplo de Rota Protegida - Admin)
Endpoint restrito a administradores para atualização de cadastro de usuários.

PUT /api/Users/{id}

Header: Authorization: Bearer <TOKEN_ADMIN>

Requisição:

```json
{
  "name": "Barbeiro Silva Atualizado",
  "email": "barbeiro.novoemail@barberboss.com",
  "role": "Barber",
  "isActive": true
}
```

# Getting Started

Para obter uma cópia local funcionando, siga estes passos simples.

### Requisitos

* Visual Studio versão 2022+ ou Visual Studio Code
* Windows 10+ ou Linux/MacOS com [.NET SDK][dot-net-sdk] instalado
* MySql Server

### Instalação

1. Clone o repositório:
    ```sh
    git clone https://github.com/LeonardoHMG/BarberBoss.git
    ```

2. Preencha as informações no arquivo `appsettings.Development.json` (Connection String e Chave JWT).
3. Execute as Migrations do Entity Framework Core se necessário para criar o banco de dados:
   ```bash
   dotnet ef database update
   ```
5. Execute a API e aproveite o seu teste :)


<!-- Links -->
[dot-net-sdk]: https://dotnet.microsoft.com/en-us/download/dotnet/8.0

<!-- Images -->
[hero-image]: images/heroimage.png

<!-- Badges -->
[badge-dot-net]: https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=fff&style=for-the-badge
[badge-windows]: https://img.shields.io/badge/Windows-0078D4?logo=windows&logoColor=fff&style=for-the-badge
[badge-visual-studio]: https://img.shields.io/badge/Visual%20Studio-5C2D91?logo=visualstudio&logoColor=fff&style=for-the-badge
[badge-mysql]: https://img.shields.io/badge/MySQL-4479A1?logo=mysql&logoColor=fff&style=for-the-badge
[badge-swagger]: https://img.shields.io/badge/Swagger-85EA2D?logo=swagger&logoColor=000&style=for-the-badge
