# KubeFood

Este é um repositório de estudo e teste que simula uma plataforma de delivery de comida, construída com uma arquitetura de microsserviços.

## Visão Geral

O KubeFood é uma plataforma projetada para gerenciar o ciclo de vida de um pedido, desde a criação de produtos em um catálogo até a simulação da entrega. O sistema é dividido em vários serviços, cada um com sua responsabilidade específica.

## Funcionalidades Principais

O projeto é composto pelos seguintes serviços:

*   **Serviço de Catálogo (`Catalog.API`):**
    *   Responsável por criar e gerenciar os itens do cardápio.

*   **Serviço de Estoque (`Inventory.API`):**
    *   Gerencia a quantidade de cada item disponível no estoque.

*   **Serviço de Pedidos (`Order.API`):**
    *   Permite a criação de ordens de compra para uma lista de itens.
    *   Realiza a validação para garantir que os itens solicitados existem no catálogo.
    *   Verifica se há quantidade suficiente em estoque para atender ao pedido.

*   **Worker de Simulação (`Delivery.Worker`):**
    *   Atua como um consumidor de fila de mensagens.
    *   Após a criação de um pedido, este worker simula os passos subsequentes do processo, como:
        *   Pagamento
        *   Preparação do pedido
        *   Envio para entrega

Este projeto serve como um exemplo prático de como diferentes serviços podem interagir de forma assíncrona para construir um sistema coeso e resiliente.

### Observabilidade e Mensageria

- **RabbitMQ (`rabbitmq`):** Atua como o barramento de eventos, permitindo a comunicação assíncrona e o desacoplamento entre os microsserviços.
- **Jaeger (`jaeger`):** Utilizado para o rastreamento de requisições, permitindo monitorar o fluxo de uma operação através de múltiplos serviços.
- **Seq (`seq`):** Coleta e centraliza os logs de todos os serviços da aplicação, facilitando a depuração e o monitoramento.

## Tecnologias Utilizadas

- **Backend:** .NET 8
- **Bancos de Dados:**
  - MongoDB
  - MySQL
  - SQL Server
- **Mensageria:** RabbitMQ
- **Observabilidade:**
  - OpenTelemetry
  - Jaeger (Rastreamento)
  - Serilog e Seq (Logging)
- **Containerização:** Docker e Docker Compose

## Como Executar o Projeto

Para executar o KubeFood em seu ambiente local, você precisa ter o Docker e o Docker Compose instalados.

1. **Clone o repositório:**
   ```bash
   git clone <url-do-repositorio>
   cd KubeFood
   ```

2. **Inicie os contêineres:**
   Execute o comando a seguir na raiz do projeto para construir as imagens e iniciar todos os serviços definidos no arquivo `docker-compose.yml`.

   ```bash
   docker-compose up -d
   ```

3. **Acessando os serviços:**
   - **APIs:** As APIs estarão disponíveis nas portas definidas no `docker-compose.yml` (ex: `http://localhost:3000` para a Catalog.API).
   - **Jaeger UI:** `http://localhost:16686`
   - **Seq UI:** `http://localhost:5341`
   - **RabbitMQ Management:** `http://localhost:15672` (usuário: `guest`, senha: `guest`)