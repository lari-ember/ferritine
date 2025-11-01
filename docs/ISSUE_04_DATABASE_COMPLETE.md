# Issue #4: Configurar Banco de Dados PostgreSQL - CONCLUÍDO ✓

**Labels**: feat, phase-0: fundamentals, priority: critical, area: database, complexity: intermediate  
**Milestone**: Milestone 0: Fundamentos e Infraestrutura  
**Data de Conclusão**: 2025-11-01

## Resumo

Implementação completa do sistema de persistência de dados usando PostgreSQL e SQLAlchemy, com suporte a SQLite para desenvolvimento e testes.

## Tarefas Concluídas

- [x] Instalar SQLAlchemy, Alembic e psycopg2-binary
- [x] Criar `backend/database/models.py` com todos os modelos
- [x] Implementar modelo completo de Agent (Agente) com todos os campos especificados
- [x] Criar modelos auxiliares (Building, Profession, Routine, NamePool, etc)
- [x] Implementar `backend/database/connection.py` para gerenciamento de conexões
- [x] Implementar `backend/database/queries.py` com queries e operações CRUD
- [x] Configurar Alembic para migrations
- [x] Criar sistema compatível com PostgreSQL e SQLite
- [x] Implementar testes unitários completos
- [x] Criar script de inicialização e seed de dados
- [x] Documentar tudo no DATABASE_GUIDE.md

## Estrutura do Modelo Agent

O modelo Agent foi implementado com TODOS os campos especificados:

### Identificação e Metadados
- ✅ `id` - UUID (compatível com PostgreSQL e SQLite)
- ✅ `name` - String gerado aleatoriamente (suporte via NamePool)
- ✅ `created_at` - DateTime
- ✅ `created_by` - CHAR (I=IA, A=Admin, B=Birth)
- ✅ `version` - String da versão do programa

### Dados Pessoais
- ✅ `birth_date` - DateTime
- ✅ `gender` - Enum (M, F, NB, O) com suporte a autodeterminação
- ✅ Propriedade `age` calculada automaticamente

### Skills e Profissão
- ✅ `skills` - JSON com habilidades
- ✅ `profession_id` - FK para tabela professions
- ✅ `routine_id` - FK para tabela routines

### Localização
- ✅ `home_building_id` - FK para buildings
- ✅ `work_building_id` - FK para buildings
- ✅ `current_location_type` - String polimórfica
- ✅ `current_location_id` - UUID polimórfico
- ✅ `last_seen_at` - Timestamp

### Saúde e Energia
- ✅ `health_status` - CHAR (H=Healthy, S=Sick, C=Critical, D=Dead)
- ✅ `energy_level` - Integer (-100 a 100, aceita negativos para deficiências)
- ✅ Constraint para validar range de energia

### Humor e Personalidade
- ✅ `mood_data` - JSON complexo (similar a The Sims 4)
- ✅ `personality` - JSON adaptativa (genética + eventos)

### Economia
- ✅ `wallet` - DECIMAL(13,2) pode ser negativo até -100k
- ✅ `inventory` - JSON com itens

### Status e Movimento
- ✅ `current_status` - Enum (idle, moving, working, sleeping, sick, eating, socializing)
- ✅ `destination_type` - String polimórfica
- ✅ `destination_id` - UUID polimórfico

### Objetivos e História
- ✅ `goals` - JSON (curto/médio/longo prazo, sonhos)
- ✅ `history` - JSON com eventos importantes, traumas, família
- ✅ `genetics` - JSON com sistema complexo herdado dos pais

### Controle
- ✅ `is_deleted` - Boolean (soft delete)
- ✅ Propriedade `full_biography` para gerar biografia

## Modelos Auxiliares Implementados

### 1. Building (Edifício)
- Tipos: residential, commercial, industrial, public
- Posição (x, y) com dimensões
- Capacidade e ocupação
- Custos (aluguel e operação)
- Relacionamentos com agentes (residentes e trabalhadores)

### 2. Profession (Profissão)
- Nome, descrição, setor
- Salário base
- Skills necessárias (JSON)
- Relacionamento com agentes

### 3. Routine (Rotina)
- Programação por hora do dia (JSON)
- Relacionamento com agentes
- Sistema para ser aprofundado posteriormente

### 4. NamePool (Pool de Nomes)
- Nomes (primeiro, meio, sobrenome)
- Raridade (0.0 a 1.0)
- Gênero específico ou neutro
- Origem cultural
- Sistema de seleção aleatória ponderada

### 5. Vehicle (Veículo)
- Tipos (train, bus, car, etc)
- Localização e movimento
- Capacidade de passageiros e carga

### 6. Event (Evento)
- Registro de eventos da simulação
- Relacionamentos com agentes, edifícios e veículos
- Dados adicionais em JSON

### 7. EconomicStat (Estatística Econômica)
- Estatísticas agregadas por período
- Dinheiro em circulação
- Transações por setor

## Funcionalidades Implementadas

### Sistema de Queries
Implementação completa de queries para:
- Agentes (por ID, nome, localização, profissão, status, saúde, carteira)
- Edifícios (por tipo, posição, vagas disponíveis)
- Veículos (por tipo, disponibilidade)
- Eventos (recentes, por tipo, por agente)
- Profissões (por nome, setor)
- Nomes (seleção aleatória ponderada)
- Estatísticas agregadas

### Compatibilidade Multi-Banco
- Tipo GUID customizado que funciona em PostgreSQL (UUID nativo) e SQLite (CHAR36)
- JSON ao invés de JSONB para compatibilidade
- Sistema de conexão configurável

### Migrations com Alembic
- Configuração completa do Alembic
- Suporte a migrations automáticas
- Integração com variáveis de ambiente

### Scripts de Gerenciamento
`scripts/init_database.py` com comandos:
- `init` - Inicializa banco de dados
- `seed` - Popula com dados iniciais (profissões, nomes, exemplos)
- `stats` - Mostra estatísticas
- `drop` - Remove todas as tabelas
- Suporte a `--sqlite` para desenvolvimento

### Testes Completos
16 testes unitários implementados:
- Criação de modelos
- Propriedades calculadas (idade)
- Skills e genética em JSON
- Limites de carteira e energia
- Queries diversas
- Soft delete
- Estatísticas agregadas

## Arquivos Criados

```
backend/database/
├── __init__.py          # Exports e imports centralizados
├── models.py            # 430 linhas - Todos os modelos
├── connection.py        # 192 linhas - Gerenciamento de conexão
└── queries.py           # 300+ linhas - Queries CRUD

migrations/              # Configuração do Alembic
├── versions/
├── env.py              # Configurado para usar nossos modelos
└── README

scripts/
└── init_database.py    # 300+ linhas - Script de inicialização

tests/unit/
└── test_database.py    # 400+ linhas - 16 testes

docs/
└── DATABASE_GUIDE.md   # Documentação completa

.env.example            # Atualizado com variáveis de DB
requirements.txt        # Atualizado com dependências
alembic.ini            # Configuração do Alembic
```

## Testes Executados

```bash
$ pytest tests/unit/test_database.py -v
====== 16 passed, 1 warning in 0.26s ======

✓ TestAgentModel::test_create_agent
✓ TestAgentModel::test_agent_age_property
✓ TestAgentModel::test_agent_with_skills
✓ TestAgentModel::test_agent_with_genetics
✓ TestAgentModel::test_agent_wallet_limits
✓ TestAgentModel::test_agent_energy_level
✓ TestBuildingModel::test_create_building
✓ TestProfessionModel::test_create_profession
✓ TestAgentQueries::test_get_by_id
✓ TestAgentQueries::test_get_by_name
✓ TestAgentQueries::test_get_by_status
✓ TestAgentQueries::test_soft_delete
✓ TestAgentQueries::test_get_statistics
✓ TestBuildingQueries::test_get_by_type
✓ TestBuildingQueries::test_get_with_vacancy
✓ TestNamePoolQueries::test_get_random_name
```

## Uso Básico

### Inicializar Banco
```bash
# PostgreSQL
python scripts/init_database.py init

# SQLite (desenvolvimento)
python scripts/init_database.py --sqlite init
```

### Popular com Dados
```bash
python scripts/init_database.py seed
```

### Usar no Código
```python
from backend.database import session_scope, DatabaseQueries
from backend.database.models import Agent, CreatedBy, Gender
from datetime import datetime
from decimal import Decimal

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
    
    # Buscar
    found = queries.agents.get_by_id(agent.id)
    print(f"Agente: {found.name}, Idade: {found.age}")
```

## Características Especiais

### 1. Sistema de Genética
```python
genetics = {
    "hair_color": "brown",
    "eye_color": "blue",
    "height_factor": 1.05,
    "parent1_id": str(uuid4()),
    "parent2_id": str(uuid4()),
    "traits": ["intelligent", "athletic"]
}
```

### 2. Sistema de Humor (The Sims 4 style)
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
```

### 3. Histórico Biográfico
```python
history = [
    {"event": "birth", "date": "2020-01-01", "description": "Nascido na cidade"},
    {"event": "education", "date": "2025-01-01", "description": "Formado"}
]
```

## Otimizações

- **Índices**: Criados em campos chave (id, name, status, timestamps)
- **UUID**: Sistema GUID compatível com múltiplos bancos
- **Soft Delete**: Agentes mantidos para histórico
- **Pool de Conexões**: Configurado para PostgreSQL (10+20)
- **Lazy Loading**: Relacionamentos otimizados
- **Constraints**: Validação de energia level (-100 a 100)

## Próximos Passos

1. **Integrar com Simulação**
   - Conectar modelos de Agent e Building com a simulação existente
   - Persistir estado da simulação

2. **Expandir Sistema de Rotinas**
   - Implementar engine de rotinas por hora
   - Sistema de eventos baseados em tempo

3. **Sistema de Relacionamentos**
   - Família (pais, filhos, cônjuge)
   - Amizades e inimizades
   - Relacionamentos românticos

4. **IA de Personalidade**
   - Sistema de decisão baseado em personalidade
   - Evolução de traits por experiências
   - Traumas e memórias

5. **Economia Complexa**
   - Sistema de transações
   - Mercado de trabalho
   - Inflação e economia dinâmica

## Documentação

Toda a documentação está em `docs/DATABASE_GUIDE.md`:
- Guia de instalação e configuração
- Exemplos de uso
- Referência de queries
- Troubleshooting
- Estrutura de arquivos

## Conclusão

✅ **Issue #4 COMPLETA**

Implementação robusta e completa do sistema de banco de dados com:
- Modelo Agent com TODOS os 30+ campos especificados
- 7 modelos auxiliares completos
- Sistema de queries abrangente
- Compatibilidade PostgreSQL + SQLite
- 16 testes passando
- Documentação completa
- Scripts de gerenciamento
- Migrations configuradas

O sistema está pronto para ser integrado com a simulação e expandido conforme necessário!

