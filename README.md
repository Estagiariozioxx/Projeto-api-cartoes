# Gateway de Pagamento API

Esta é uma aplicação ASP.NET Core API que simula um gateway de pagamento. A API oferece funcionalidades para calcular parcelas de pagamento, validar cartões de crédito e gerenciar transações financeiras.

## Requisitos Técnicos

1. **Middleware de Log (Serilog)**
   - Logs armazenados em arquivos locais e no banco de dados MySQL.
   - Captura de erros no bloco `catch` dos tratamentos de exceções.

2. **Estrutura do Código**
   - Organização em classes de serviço, ViewModels e entidades.

3. **Injeção de Dependência**
   - Uso de injeção de dependência para melhor gestão dos serviços.

4. **Documentação com Swagger**
   - Todos os endpoints documentados e acessíveis via Swagger.

## Endpoints

### Pagamentos

- **POST /pagamentos/calcular-parcelas**
  - Calcula o valor das parcelas com base no valor total, taxa de juros e quantidade de parcelas.
  - **Requisição:**
    ```json
    {
      "ValorTotal": 100.00,
      "TaxaJuros": 0.05,
      "QuantidadeParcelas": 3
    }
    ```
  - **Resposta:**
    ```json
    [
      {"parcela": 1, "valor": 34.00},
      {"parcela": 2, "valor": 33.00},
      {"parcela": 3, "valor": 33.00}
    ]
    ```

- **POST /pagamentos**
  - Inicia o processo de pagamento.
  - **Requisição:**
    ```json
    {
      "valor": 100.00,
      "numeroCartao": "1111255516996632",
      "cvv": 123,
      "parcelas": 3
    }
    ```
  - **Resposta:**
    - `201 Created` ou `400 Bad Request`

- **GET /pagamentos/{id}/situacao**
  - Consulta a situação de um pagamento pelo ID.
  - **Resposta:**
    - Situação do pagamento.

- **PUT /pagamentos/{id}/confirmar**
  - Confirma um pagamento pelo ID.
  - **Resposta:**
    - `200 OK` ou `400 Bad Request`

- **PUT /pagamentos/{id}/cancelar**
  - Cancela um pagamento pelo ID.
  - **Resposta:**
    - `200 OK` ou `400 Bad Request`

### Cartões

- **GET /cartoes/{cartao}/obter-bandeira**
  - Retorna a bandeira do cartão.
  - **Resposta:**
    - `200 OK` + Bandeira ou `404 Not Found`

- **GET /cartoes/{cartao}/valido**
  - Verifica se o cartão é válido.
  - **Resposta:**
    - `true` ou `false`

## Tabelas do Banco de Dados

### Cartao

```sql
CREATE TABLE `Cartao` (
  `Numero` varchar(16) NOT NULL,
  `Validade` datetime DEFAULT NULL,
  PRIMARY KEY (`Numero`)
);
```

### Transacao
```sql
CREATE TABLE `Transacao` (
  `TransacaoId` int NOT NULL AUTO_INCREMENT,
  `Valor` decimal(10,2) NOT NULL,
  `Cartao` varchar(16) NOT NULL,
  `CVV` varchar(3) NOT NULL,
  `Parcelas` int NOT NULL DEFAULT '1',
  `Situacao` smallint NOT NULL,
  PRIMARY KEY (`TransacaoId`)
);
```

## Como Executar

1. - Clone este repositório

2. - Configure a string de conexão no appsettings.json.

3. - Execute o projeto no Visual Studio ou pela CLI.

