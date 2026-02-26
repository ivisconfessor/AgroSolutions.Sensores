# AgroSolutions.Sensores

Microserviço de **ingestão de dados de sensores** (simulados) do AgroSolutions. Expõe API para receber umidade do solo, temperatura e precipitação por talhão, persiste em MongoDB e publica eventos no RabbitMQ.

## Estrutura

- **Api**: Minimal APIs (POST/GET leituras)
- **Aplicacao**: Camada de aplicação
- **Dominio**: Entidade `LeituraSensor`
- **Infra**: MongoDB (MongoDB.Driver) + RabbitMQ (RabbitMQ.Client)
- **Tests**: Projeto de testes

## MongoDB (Mongo Atlas)

### Pacote recomendado

- **MongoDB.Driver** (oficial) – já referenciado no projeto.

### Collections sugeridas

Crie no Mongo Atlas um database (ex.: `agrosolutions_sensores`) e a collection abaixo. A API cria documentos na primeira escrita.

| Collection          | Uso |
|---------------------|-----|
| **leituras_sensores** | Uma leitura por documento: talhão, data/hora, umidade (%), temperatura (°C), precipitação (mm). |

### Índice recomendado (Mongo Atlas)

Para consultas por talhão e período (séries temporais), crie um índice na collection `leituras_sensores`:

- **Campos**: `id_talhao` (asc), `data_leitura` (desc)
- **Nome sugerido**: `ix_leituras_id_talhao_data`

No Atlas: Database → Collections → `leituras_sensores` → Indexes → Create Index:
- Field: `id_talhao`, Order: 1
- Field: `data_leitura`, Order: -1

### Exemplo de documento (leituras_sensores)

```json
{
  "_id": "<ObjectId>",
  "id_talhao": "<UUID do talhão>",
  "data_leitura": "<ISODate>",
  "umidade_solo": 65.5,
  "temperatura": 24.2,
  "precipitacao": 0.0
}
```

## RabbitMQ

- **Fila**: `agrosolutions.sensores.leituras` (configurável em `RabbitMQ:FilaLeituras`)
- Ao ingerir uma leitura, a API publica uma mensagem JSON nessa fila para outros microsserviços (ex.: Análise/Alertas).

## Configuração

Em `appsettings.json` ou User Secrets (recomendado em dev):

```json
{
  "MongoDb": {
    "ConnectionString": "<sua connection string do Mongo Atlas>",
    "DatabaseName": "agrosolutions_sensores"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest",
    "FilaLeituras": "agrosolutions.sensores.leituras"
  }
}
```

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| POST   | /leituras | Inserir leitura (idTalhao, umidadeSolo, temperatura, precipitacao; dataLeitura opcional) |
| GET    | /leituras?idTalhao={guid}&de=&ate=&limite= | Listar leituras por talhão (filtros opcionais) |
| GET    | /leituras/{id} | Obter leitura por id |

## Executar

```bash
dotnet run --project src/AgroSolutions.Sensores.Api
```

Swagger: `https://localhost:7052/swagger` (ou porta configurada).
