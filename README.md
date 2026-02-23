# CRM Clientes — Rommanel

Sistema de gerenciamento de clientes (PF e PJ) construído com
Clean Architecture, DDD, CQRS, Event Sourcing e Angular como
parte de um desafio de processo seletivo da Capitani Group e 
Rommanel, [conforme documentação](https://github.com/alexandredorea/Challenge.CRM.Rommanel/blob/master/docs/202602-desafio-tecnico-rommanel.pdf).

---

## Pré-requisitos

| Ferramenta     | Versão mínima | Necessário para    |
|----------------|---------------|--------------------|
| Docker Desktop | 4.x           | Ambiente completo  |
| .NET SDK       | 10.x          | Dev backend local  |
| Node.js        | 22.x          | Dev frontend local |
| Angular CLI    | 21.x          | Dev frontend local |


---

## Subindo o ambiente completo (Docker)

```bash
# 1. Clone o repositório
git clone https://github.com/alexandredorea/Challenge.CRM.Rommanel.git
cd Challenge.CRM.Rommanel

# 2. Configure as variáveis de ambiente
cp .env.example .env

# 3. Execute o Compose a partir da pasta "solution items"
cd "solution items"
docker compose --env-file ../.env up -d

# 4. Aguarde todos os serviços ficarem healthy (~60s na primeira execução)
docker compose ps

# 5. Caso precise acompanhe os logs
docker compose logs -f api

#6. Aplique as migrações no banco de dados
dotnet ef database update --project src/Challenge.CRM.Rommanel.Infrastructure --startup-project src/Challenge.CRM.Rommanel.Api
```

| Serviço    | URL                           | Credenciais             |
|------------|-------------------------------|-------------------------|
| Frontend   | http://localhost:4200         | admin.crm / Admin@2026  |
| API        | http://localhost:5000         | (requer JWT Keycloak)   |
| Scalar     | http://localhost:5000/docs    | (requer JWT Keycloak)   |
| Keycloak   | http://localhost:8080         | admin / admin@2026      |
| PostgreSQL | localhost:5432                | crm / crm@2026          |

---

## Problemas encontrados (comuns)

| Problema                | Causa | Solução |
|-------------------------|----------------------------------|-----------------------------------------------------------|
| Keycloak demora a subir | Aguarda PostgreSQL ficar healthy | Esperar ~60 seg. na primeira execução                     |
| `realm not found`       | realm-export.json não importado  | Verificar path `../keycloak/realm-export.json` no compose |
| Frontend tela branca    | Keycloak não disponível          | Aguardar Keycloak ficar healthy antes de abrir o browser  |
| API retorna 401         | Token expirado ou inválido       | Fazer logout/login no frontend                            |
| Migration não aplicada  | Banco não estava no ar           | Reiniciar o container da API após o banco subir           |
| `ng: command not found` | CLI não instalado                | `npm install -g @angular/cli`                             |
| Porta 4200 ocupada      | Outro processo na porta          | `ng serve --port 4300`                                    |

---

## Desenvolvimento local (sem Docker)

### Backend

```bash
# 1. Sobe apenas PostgreSQL e Keycloak via Docker
cd "solution items"
docker compose --env-file ../.env up postgres keycloak -d

# 2. Roda a API localmente
cd backend/src/Challenge.CRM.Rommanel.Api
dotnet restore
dotnet run
# API disponível em http://localhost:5000/docs
```

### Frontend

```bash
# 1. Instala dependências
cd frontend/src
npm install

# 2. Roda com proxy para a API local
ng serve --proxy-config proxy.conf.json
# Frontend disponível em http://localhost:4200
```

### Frontend sem backend (apenas visual)

Para visualizar o layout sem Keycloak ou API no ar,
comente temporariamente o `APP_INITIALIZER` em `app.config.ts`
e libere o `authGuard` retornando `true`:

```bash
ng serve
```

---

## Executando os testes (caso deseje)

```bash
# Testes de domínio — sem Docker, são rápidos
dotnet test backend/tests/Challenge.CRM.Rommanel.Domain.Tests

# Testes de integração — requer Docker (Testcontainers sobe PostgreSQL)
dotnet test backend/tests/Challenge.CRM.Rommanel.Integration.Tests

# Todos os testes com cobertura
dotnet test backend/ --collect:"XPlat Code Coverage"

# Para gerar um relatório HTML (caso necessário)
reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coverage-report" \
  -reporttypes:Html

# Abrir relatório
start coverage-report/index.html   # Windows
open  coverage-report/index.html   # Linux/macOS
```

---

## Endpoints disponíveis da API

| Método | Rota                                      | Descrição                     |
|--------|-------------------------------------------|-------------------------------|
| GET    | /api/v1/customers                         | Lista paginada com filtro     |
| GET    | /api/v1/customers/{id}                    | Busca por Id                  |
| GET    | /api/v1/customers/{id}/events             | Histórico de eventos          |
| POST   | /api/v1/customers                         | Cria cliente (PF ou PJ)       |
| PATCH  | /api/v1/customers/{id}/address            | Atualiza endereço             |
| PATCH  | /api/v1/customers/{id}/email              | Atualiza e-mail               |
| PATCH  | /api/v1/customers/{id}/telephone          | Atualiza telefone             |
| DELETE | /api/v1/customers/{id}                    | Desativa cliente              |
| GET    | /api/v1/addresses/{postalCode}            | Consulta CEP via ViaCEP       |

---

### Camadas

| Projeto          | Responsabilidade                                   |
|------------------|----------------------------------------------------|
| `Domain`         | Entidades ricas, Value Objects, Eventos, Exceções  |
| `Application`    | Commands, Queries, Handlers, Validators, Behaviors |
| `Infrastructure` | EF Core, Event Store, ViaCEP + Polly, Repositórios |
| `API`            | Controllers, Middlewares, Scalar, Program.cs       |
| `Frontend`       | Angular 21, Material, Signals, Keycloak PKCE       |

---

### Decisões de arquitetura

Estas são decisões relevantes e complementares ao que foi disposto nos
[documentos de decisão arquitetural](https://github.com/alexandredorea/Challenge.CRM.Rommanel/tree/master/docs).
Ademais foi um dos requisitos solicitados para este [desafio](https://github.com/alexandredorea/Challenge.CRM.Rommanel/blob/master/docs/202602-desafio-tecnico-rommanel.pdf).

**Event Sourcing com JSONB**
- Os eventos de domínio são persistidos na tabela `customer_events`
como JSONB no PostgreSQL. Isso permite "reidratação" sem schema fixo
e os eventos evoluem sem novas migrations.

**Validação em duas camadas**
- `FluentValidation` (Application): formato, obrigatoriedade e tamanho
— falha rápido antes de tocar o domínio.
- `DomainException` (Domain): regras de negócio — CPF/CNPJ corretos,
DDD válido, Inscrição Estadual apenas para PJ.

**Value Objects com `OwnsOne`**
- Todos os VOs são mapeados via `OwnsOne` no EF Core — mesma tabela
que o agregado, sem joins desnecessários.

**CorrelationId de ponta a ponta**
- Gerado no Angular, propagado via header `X-Correlation-ID` até o
Event Store — rastreia uma operação completa nos logs do Serilog.

**Keycloak com PKCE**
- O frontend usa `pkceMethod: 'S256'` — padrão de segurança
recomendado para SPAs, sem necessidade de client secret.


## Comandos Docker úteis

```bash
# Subir tudo
docker compose --env-file ../.env up -d

# Parar sem remover volumes
docker compose stop

# Parar e remover containers (preserva volumes)
docker compose down

# Parar e remover TUDO incluindo volumes (reseta banco)
docker compose down -v

# Rebuild após mudanças no código
docker compose build api
docker compose up -d api

# Ver logs em tempo real
docker compose logs -f api
docker compose logs -f frontend

# Acessar o banco diretamente (ou use uma IDE como DBeaver)
docker exec -it crm-postgres psql -U crm -d crm_clientes
```