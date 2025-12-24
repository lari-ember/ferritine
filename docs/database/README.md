# 💾 Documentação do Banco de Dados

Documentação completa sobre o sistema de banco de dados do Ferritine.

## 📚 Guias Principais

### Guia Completo

- **[DATABASE_GUIDE.md](DATABASE_GUIDE.md)** - 📖 Guia completo do banco de dados
  - Configuração e instalação
  - Modelos de dados
  - Queries e operações
  - Boas práticas

### Modelos Específicos

- **[DATABASE_BUILDING_USAGE.md](DATABASE_BUILDING_USAGE.md)** - Uso do modelo Building
- **[BUILDING_MODEL_SUMMARY.md](BUILDING_MODEL_SUMMARY.md)** - Resumo do modelo Building

### Implementação e Status

- **[ISSUE_04_DATABASE_COMPLETE.md](ISSUE_04_DATABASE_COMPLETE.md)** - Issue #4 (Database) completa
- **[ISSUE_04_SUMMARY.md](ISSUE_04_SUMMARY.md)** - Resumo da implementação
- **[TODO_ISSUE_04_STATUS.md](TODO_ISSUE_04_STATUS.md)** - Status atual do desenvolvimento

## 🗄️ Modelos de Dados

O Ferritine utiliza os seguintes modelos principais:

- **Agent** - Agentes inteligentes com 30+ campos
- **Building** - Edifícios e construções
- **Profession** - Profissões dos agentes
- **Routine** - Rotinas diárias
- **Vehicle** - Veículos
- **Event** - Eventos da simulação
- **EconomicStat** - Estatísticas econômicas
- **NamePool** - Pool de nomes

## 🚀 Quick Start

```bash
# Inicializar banco SQLite (desenvolvimento)
python scripts/init_database.py --sqlite init
python scripts/init_database.py --sqlite seed

# Ou PostgreSQL (produção)
python scripts/init_database.py init
python scripts/init_database.py seed
```

## 💻 Exemplo de Uso

```python
from backend.database import session_scope, DatabaseQueries

with session_scope() as session:
    queries = DatabaseQueries(session)
    
    # Criar um agente
    agent = queries.agents.create(
        name="Dr. Ana Silva",
        wallet=15000.00,
        energy_level=85
    )
```

## 📖 Mais Informações

Para informações detalhadas, consulte o [DATABASE_GUIDE.md](DATABASE_GUIDE.md).

---

[⬅️ Voltar ao índice principal](../README.md)
