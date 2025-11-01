## Banco de Dados - PostgreSQL

Este documento descreve a configuração e uso do banco de dados PostgreSQL no projeto Ferritine.

### Estrutura

O sistema de banco de dados foi implementado com:
- **PostgreSQL** como banco principal (com suporte a SQLite para desenvolvimento/testes)
- **SQLAlchemy 2.0+** como ORM
- **Alembic** para migrations
- Modelos complexos com suporte a JSON, UUIDs e relacionamentos

### Modelos Principais

#### Agent (Agente)
Modelo completo de agente com:
- **Identificação**: UUID, nome gerado aleatoriamente
- **Skills**: JSON com habilidades
- **Rotina**: Relacionamento com tabela de rotinas
- **Dados pessoais**: Data de nascimento (idade calculada), gênero
- **Localização**: Polimórfica (building, vehicle, street)
- **Saúde**: Status de saúde (saudável, doente, crítico, morto)
- **Energia**: -100 a 100 (negativo para deficiências)
- **Humor**: Sistema complexo em JSON (similar a The Sims 4)
- **Economia**: Carteira com DECIMAL(13,2), pode ser negativa até -100k
- **Inventário**: JSON com itens
- **Status**: idle, moving, working, sleeping, sick, etc
- **Objetivos**: JSON com metas (curto/médio/longo prazo, sonhos)
- **Personalidade**: Adaptativa, afetada por genética e eventos
- **Histórico**: Eventos importantes, traumas, família
- **Genética**: Sistema complexo herdado dos pais
- **Soft delete**: Mantém dados históricos

#### Building (Edifício)
- Tipos: residential, commercial, industrial, public
- Localização (x, y) com dimensões
- Capacidade e ocupação
- Custos de aluguel e operação

#### Profession (Profissão)
- Nome, descrição, setor
- Salário base
- Skills necessárias

#### Routine (Rotina)
- Programação por hora do dia
- Associada a agentes

#### NamePool (Pool de Nomes)
- Nomes (primeiro, meio, sobrenome)
- Raridade (0.0 a 1.0)
- Gênero específico ou neutro
- Origem cultural

### Configuração

#### 1. Instalar Dependências

```bash
pip install -r requirements.txt
```

#### 2. Configurar Variáveis de Ambiente

Copie o arquivo `.env.example` para `.env`:

```bash
cp .env.example .env
```

Edite o `.env` com suas configurações:

```env
# PostgreSQL (produção)
DB_HOST=localhost
DB_PORT=5432
DB_NAME=ferritine
DB_USER=ferritine_user
DB_PASSWORD=ferritine_pass

# SQLite (desenvolvimento/testes)
SQLITE_PATH=data/db/ferritine.db
```

#### 3. Criar Banco PostgreSQL

```bash
# Criar usuário e banco
sudo -u postgres psql

CREATE USER ferritine_user WITH PASSWORD 'ferritine_pass';
CREATE DATABASE ferritine OWNER ferritine_user;
GRANT ALL PRIVILEGES ON DATABASE ferritine TO ferritine_user;
\q
```

#### 4. Inicializar Banco de Dados

```bash
# Usando PostgreSQL
python scripts/init_database.py init

# Ou usando SQLite (desenvolvimento)
python scripts/init_database.py --sqlite init
```

#### 5. Popular com Dados Iniciais

```bash
# Adiciona profissões, nomes e dados de exemplo
python scripts/init_database.py seed

# Ou com SQLite
python scripts/init_database.py --sqlite seed
```

### Migrations com Alembic

#### Criar Nova Migration

```bash
# Gerar migration automática baseada nos modelos
alembic revision --autogenerate -m "Descrição da mudança"
```

#### Aplicar Migrations

```bash
# Aplicar todas as migrations pendentes
alembic upgrade head

# Aplicar migration específica
alembic upgrade <revision>
```

#### Reverter Migrations

```bash
# Reverter última migration
alembic downgrade -1

# Reverter para revision específica
alembic downgrade <revision>
```

#### Histórico de Migrations

```bash
# Ver histórico
alembic history

# Ver migration atual
alembic current
```

### Uso do Banco de Dados

#### Exemplo Básico

```python
from backend.database import session_scope, DatabaseQueries
from backend.database.models import Agent, CreatedBy, Gender
from datetime import datetime
from decimal import Decimal

# Usando context manager
with session_scope() as session:
    queries = DatabaseQueries(session)
    
    # Criar agente
    agent = queries.agents.create(
        name="João Silva",
        created_by=CreatedBy.IA,
        birth_date=datetime(1995, 5, 15),
        gender=Gender.MALE,
        wallet=Decimal("1000.00"),
        version="0.1.0"
    )
    
    # Buscar agente
    found = queries.agents.get_by_id(agent.id)
    print(f"Agente: {found.name}, Idade: {found.age}")
    
    # Listar todos os agentes
    all_agents = queries.agents.get_all()
    
    # Buscar por status
    working = queries.agents.get_by_status(AgentStatus.WORKING)
    
    # Estatísticas
    stats = queries.agents.get_statistics()
    print(f"Total de agentes: {stats['total']}")
```

#### Queries Disponíveis

**Agentes:**
- `get_by_id(agent_id)` - Buscar por ID
- `get_all(include_deleted=False)` - Listar todos
- `get_by_name(name)` - Buscar por nome
- `get_at_location(type, id)` - Agentes em localização
- `get_by_profession(profession_id)` - Por profissão
- `get_by_status(status)` - Por status
- `get_wealthy_agents(min_amount)` - Agentes ricos
- `get_poor_agents(max_amount)` - Agentes pobres
- `soft_delete(agent_id)` - Deletar (soft)
- `get_statistics()` - Estatísticas gerais

**Edifícios:**
- `get_by_id(building_id)`
- `get_by_type(building_type)`
- `get_at_position(x, y)`
- `get_with_vacancy(building_type)`

**Profissões:**
- `get_by_id(profession_id)`
- `get_by_name(name)`
- `get_by_sector(sector)`

**Nomes:**
- `get_random_name(name_type, gender)`

### Scripts de Gerenciamento

```bash
# Inicializar banco
python scripts/init_database.py init

# Popular com dados seed
python scripts/init_database.py seed

# Ver estatísticas
python scripts/init_database.py stats

# Remover todas as tabelas (CUIDADO!)
python scripts/init_database.py drop
```

### Testes

```bash
# Rodar testes do banco de dados
pytest tests/unit/test_database.py -v

# Com cobertura
pytest tests/unit/test_database.py --cov=backend.database
```

### Estrutura de Arquivos

```
backend/database/
├── __init__.py          # Exports principais
├── models.py            # Modelos SQLAlchemy
├── connection.py        # Gerenciamento de conexão
└── queries.py           # Queries e operações CRUD

migrations/              # Migrations do Alembic
├── versions/           # Arquivos de migration
├── env.py             # Configuração do Alembic
└── README

scripts/
└── init_database.py    # Script de inicialização

tests/unit/
└── test_database.py    # Testes dos modelos
```

### Características Especiais

#### Sistema de Genética
```python
genetics = {
    "hair_color": "brown",
    "eye_color": "blue",
    "height_factor": 1.05,
    "parent1_id": str(uuid4()),
    "parent2_id": str(uuid4()),
    "traits": ["intelligent", "athletic"]
}
agent.genetics = genetics
```

#### Sistema de Humor
```python
mood_data = {
    "happiness": 0.8,
    "energy": 0.6,
    "stress": 0.3,
    "modifiers": [
        {"type": "work_success", "value": 0.2, "duration": 24},
        {"type": "tired", "value": -0.1, "duration": 6}
    ]
}
agent.mood_data = mood_data
```

#### Histórico de Eventos
```python
history = [
    {
        "event": "birth",
        "date": "2020-01-01",
        "description": "Nascido na cidade"
    },
    {
        "event": "education",
        "date": "2025-01-01",
        "description": "Formado em Engenharia"
    }
]
agent.history = history
```

### Otimizações

- **Índices**: Criados automaticamente em campos chave (id, name, status)
- **UUID**: Uso de UUID v4 para IDs únicos e distribuídos
- **JSONB**: PostgreSQL usa JSONB para melhor performance em campos JSON
- **Soft Delete**: Agentes não são removidos fisicamente
- **Pool de Conexões**: Configurado para PostgreSQL (10 conexões + 20 overflow)

### Troubleshooting

#### Erro de Conexão PostgreSQL
```bash
# Verificar se PostgreSQL está rodando
sudo systemctl status postgresql

# Iniciar PostgreSQL
sudo systemctl start postgresql
```

#### Usar SQLite Temporariamente
```python
from backend.database import DatabaseManager

db_manager = DatabaseManager(use_sqlite=True)
db_manager.init_database()
```

#### Resetar Banco de Dados
```bash
python scripts/init_database.py drop
python scripts/init_database.py init
python scripts/init_database.py seed
```

