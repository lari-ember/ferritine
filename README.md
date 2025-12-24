# 🏙️ Ferritine

<div align="center">

[![Python Version](https://img.shields.io/badge/python-3.8%2B-blue.svg)](https://www.python.org/downloads/release/python-380/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Docker](https://img.shields.io/badge/docker-ready-blue.svg)](https://www.docker.com/)
[![PostgreSQL](https://img.shields.io/badge/postgresql-15-blue.svg)](https://www.postgresql.org/)
[![Tests](https://github.com/ferritine/ferritine/workflows/Tests/badge.svg)](https://github.com/ferritine/ferritine/actions/workflows/tests.yml)
[![Release](https://github.com/ferritine/ferritine/workflows/Release%20Drafter/badge.svg)](https://github.com/ferritine/ferritine/actions/workflows/release-drafter.yml)
[![Code Coverage](https://img.shields.io/badge/coverage-check%20codecov-brightgreen.svg)](https://codecov.io)

</div>

**Ferritine** é um projeto de maquete híbrida físico-digital que combina agentes inteligentes, simulação temporal e integração com hardware para criar um ambiente urbano interativo e dinâmico.

O projeto simula uma cidade com agentes que possuem rotinas diárias realistas, movendo-se entre casa e trabalho de acordo com horários programados. É ideal para pesquisa em sistemas multiagentes, simulação urbana e IoT.

## ✨ Recursos

- 🤖 **Simulação de Agentes**: Agentes autônomos com rotinas diárias (casa ↔ trabalho)
- 🗄️ **Banco de Dados PostgreSQL**: Persistência completa com 8 modelos e 30+ campos no Agent
- 🐳 **Docker Ready**: Containerização completa com PostgreSQL, auto-inicialização e PgAdmin
- 🏗️ **Arquitetura Modular**: Código organizado e fácil de estender
- ⏰ **Simulação Temporal**: Sistema de tempo discreto (horas do dia)
- 🧪 **Testes Automatizados**: 16+ testes de banco de dados (100% passing)
- 📊 **Snapshots de Estado**: Visualização do estado da cidade a qualquer momento
- 🔄 **Versionamento Semântico**: Sistema automatizado de releases e changelogs
- 🚀 **CI/CD Completo**: Workflows GitHub Actions para testes, releases e qualidade
- 📝 **Release Drafter**: Geração automática de changelogs organizados por categoria
- 🏷️ **Sistema de Labels**: Organização de PRs e issues com labels semânticos

## 📋 Pré-requisitos

### Opção 1: Docker (Recomendado) 🐳

- **Docker** 20.10+ ([Instalar Docker](https://docs.docker.com/get-docker/))
- **Docker Compose** 2.0+ (incluído no Docker Desktop)
- **git** (para clonar o repositório)

📖 **Quick Start Docker**: Ver [DOCKER_GUIDE_QUICKSTART.md](docs/guides/DOCKER_GUIDE_QUICKSTART.md)

### Opção 2: Instalação Local

- **Python 3.8 ou superior** ([Download](https://www.python.org/downloads/))
- **PostgreSQL 15+** (ou SQLite para desenvolvimento)
- **pip** (gerenciador de pacotes do Python)
- **git** (para clonar o repositório)
- **Sistema operacional**: Linux, macOS ou Windows com WSL

### Dependências do Projeto

As dependências são gerenciadas através do `requirements.txt`:
- `pytest>=8.4.2` - Framework de testes
- Outras dependências serão listadas conforme o projeto evolui

## 🚀 Instalação e Uso

> 📖 **Guia Completo**: Para instruções detalhadas, consulte o [Guia de Início Rápido](docs/guides/QUICKSTART.md)

### Opção A: Usando Docker 🐳 (Recomendado)

```bash
# 1. Clone o repositório
git clone https://github.com/ferritine/ferritine.git
cd ferritine

# 2. Copie a configuração
cp .env.example .env

# 3. Inicie tudo com um comando
chmod +x docker-manage.sh
./docker-manage.sh start

# Pronto! PostgreSQL + Aplicação rodando
```

**Comandos Docker Úteis:**
```bash
./docker-manage.sh status      # Ver status
./docker-manage.sh logs        # Ver logs
./docker-manage.sh exec        # Shell no container
./docker-manage.sh db          # PostgreSQL CLI
./docker-manage.sh stop        # Parar tudo
```

📖 **Guia Docker**: [DOCKER_GUIDE_QUICKSTART.md](docs/guides/DOCKER_GUIDE_QUICKSTART.md) | [docs/guides/DOCKER_GUIDE.md](docs/guides/DOCKER_GUIDE.md)

---

### Opção B: Instalação Local

### 1. Clone o Repositório
```bash
git clone https://github.com/ferritine/ferritine.git
cd ferritine
```

### 2. Configure o Ambiente Virtual
```bash
# Criar ambiente virtual
python -m venv .venv

# Ativar ambiente virtual
# No Linux/macOS:
source .venv/bin/activate
# No Windows (PowerShell):
# .venv\Scripts\Activate.ps1
```

### 3. Instale as Dependências
```bash
pip install -r requirements.txt

# Opcional: Instale em modo desenvolvimento (recomendado para contribuidores)
pip install -e .
```

### 4. Execute o Programa
```bash
python main.py
```

### 5. Execute os Testes
```bash
# Executar todos os testes
pytest

# Executar com verbose
pytest -v

# Executar com cobertura
pytest --cov=app
```

## 📁 Estrutura do Projeto

Após a **Issue #1 - Reorganização da Estrutura** (fase concluída), o projeto segue uma arquitetura modular e escalável:

```
ferritine/
├── .github/                              # Configurações do GitHub
│   ├── ISSUE_TEMPLATE/                   # Templates para issues
│   │   ├── bug_report.md                 # Template para reportar bugs
│   │   └── feature_request.md            # Template para solicitar funcionalidades
│   ├── scripts/                          # Scripts de automação
│   │   ├── bump_version.py               # Script para atualizar versão
│   │   └── generate_changelog.sh         # Script para gerar changelog
│   └── workflows/                        # GitHub Actions workflows
│       ├── python-app.yml                # CI/CD para testes
│       ├── release.yml                   # Workflow de release automatizado
│       └── tests.yml                     # Testes automatizados
│
├── backend/                              # Lógica do backend
│   ├── __init__.py
│   ├── simulation/                       # Motor de simulação (antes: app/)
│   │   ├── __init__.py
│   │   └── models/                       # Modelos de domínio
│   │       ├── __init__.py
│   │       ├── agente.py                 # Classe Agente (habitantes)
│   │       └── cidade.py                 # Classe Cidade (mundo)
│   ├── database/                         # Modelos e queries do banco (futuro)
│   │   └── __init__.py
│   ├── api/                              # API REST/WebSocket (futuro)
│   │   └── __init__.py
│   └── utils/                            # Utilitários e configurações
│       ├── __init__.py
│       └── config.py                     # Configurações centralizadas
│
├── frontend/                             # Interface web (futuro)
│   └── __init__.py
│
├── visualization/                        # Visualização 2D/3D (futuro)
│   └── __init__.py
│
├── hardware/                             # Código para Arduino/ESP32 (futuro)
│   └── __init__.py
│
├── data/                                 # Banco de dados, logs, configs
│   ├── logs/                             # Arquivos de log
│   ├── db/                               # Banco de dados SQLite
│   └── config/                           # Arquivos de configuração
│
├── tests/                                # Testes automatizados
│   ├── __init__.py
│   ├── unit/                             # Testes unitários
│   │   ├── __init__.py
│   │   └── simulation/                   # Testes do motor de simulação
│   │       ├── __init__.py
│   │       └── test_sim.py               # Testes das classes Agente e Cidade
│   └── integration/                      # Testes de integração (futuro)
│       └── __init__.py
│
├── docs/                                 # Documentação do projeto
│   ├── README.md                         # Documentação detalhada
│   ├── MIGRATION_REPORT.md               # Relatório de migrações
│   └── *.md                              # Outros documentos
│
├── scripts/                              # Scripts locais
│   ├── bump_version.py                   # Script local de versionamento
│   └── create_github_structure.py        # Script de setup do GitHub
│
├── main.py                               # Ponto de entrada da aplicação
├── requirements.txt                      # Dependências organizadas por categoria
├── setup.py                              # Configuração de instalação
├── VERSION                               # Versão atual (semantic versioning)
├── pytest.ini                            # Configuração do pytest
├── README.md                             # Documentação principal
├── QUICKSTART.md                         # Guia rápido de início
├── LICENSE                               # Licença do projeto (MIT)
└── .env.example                          # Exemplo de variáveis de ambiente
```

### ✨ Destaques da Nova Arquitetura

**Modularidade**:
- `backend/` contém toda a lógica do servidor
- `frontend/`, `visualization/`, `hardware/` prontos para expansão
- `data/` centraliza logs, banco de dados e configurações
- `tests/` segue a mesma estrutura do código

**Escalabilidade**:
- Estrutura preparada para múltiplos subsistemas
- Importações relativas dentro de módulos
- Configuração centralizada em `backend/utils/config.py`

**Manutenibilidade**:
- Todos os pacotes possuem `__init__.py` adequados
- Documentação de string em cada módulo
- Testes co-localizados com a estrutura

### Descrição dos Componentes

**Core Simulation**:
- **`backend/simulation/models/agente.py`**: Classe `Agente` que representa habitantes com rotinas diárias
- **`backend/simulation/models/cidade.py`**: Classe `Cidade` que gerencia agentes e executa simulações

**Configuration & Utilities**:
- **`backend/utils/config.py`**: Gerenciamento centralizado de configurações
  - Carrega variáveis de ambiente
  - Define paths para dados, logs e config
  - Suporta múltiplos ambientes (development, production, testing)

**Testing**:
- **`tests/unit/simulation/test_sim.py`**: Testes unitários para agentes e cidade
- Estrutura pronta para expandir com testes de integração

**Database & Persistence** ✨ **NOVO**:
- **`backend/database/models.py`**: Modelos completos de banco de dados (PostgreSQL/SQLite)
  - Agent (Agente) com 30+ campos (genética, personalidade, humor, objetivos)
  - Building, Profession, Routine, Vehicle, Event, EconomicStat, NamePool
- **`backend/database/connection.py`**: Gerenciamento de conexões e sessões
- **`backend/database/queries.py`**: 40+ queries otimizadas para CRUD e estatísticas
- **`scripts/init_database.py`**: CLI para gerenciar banco (init, seed, stats, drop)
- **`migrations/`**: Migrations com Alembic para versionamento do schema
- **`examples/database_demo.py`**: Demonstração completa do sistema de banco

**Entry Points**:
- **`main.py`**: Script de demonstração que executa uma simulação de 24 horas
- **`setup.py`**: Configuração para instalação do pacote
- **`VERSION`**: Arquivo que armazena a versão atual usando versionamento semântico (MAJOR.MINOR.PATCH)
- **`.github/workflows/`**: Workflows de CI/CD automatizados (testes, releases, changelog)
- **`.github/release-drafter.yml`**: Configuração para geração automática de changelogs
- **`docs/`**: Toda a documentação do projeto organizada em um único lugar ([ver índice](docs/README.md))

## 💡 Exemplos de Uso

### Executando a Demo

Ao executar `python main.py`, você verá uma simulação de 24 horas:

```
00h -> {'Ana': 'CasaA', 'Beto': 'CasaB', 'Clara': 'CasaC'}
01h -> {'Ana': 'CasaA', 'Beto': 'CasaB', 'Clara': 'CasaC'}
...
07h -> {'Ana': 'Fábrica', 'Beto': 'Loja', 'Clara': 'Escola'}
...
17h -> {'Ana': 'CasaA', 'Beto': 'CasaB', 'Clara': 'CasaC'}
```

### Criando Seus Próprios Agentes

```python
from backend.simulation.models.agente import Agente
from backend.simulation.models.cidade import Cidade

# Criar uma cidade
cidade = Cidade()

# Adicionar agentes com diferentes rotinas
cidade.add_agente(Agente("João", "CasaA", "Hospital"))
cidade.add_agente(Agente("Maria", "CasaB", "Universidade"))

# Simular um dia
for hora in range(24):
    cidade.step(hora)
    print(f"{hora:02d}h -> {cidade.snapshot()}")
```

### Personalizando Horários

Você pode estender a classe `Agente` para customizar horários:

```python
from backend.simulation.models.agente import Agente

class AgentePersonalizado(Agente):
    def step(self, hora: int):
        # Trabalha das 9h às 18h
        if 9 <= hora < 18:
            self.local = self.trabalho
        else:
            self.local = self.casa
```

### Usando a Configuração Centralizada

```python
from backend.utils.config import Config, get_config

# Obter configuração do ambiente atual
config = get_config()

# Acessar configurações
print(f"Project root: {config.PROJECT_ROOT}")
print(f"Log file: {config.LOG_FILE}")
```

### Usando o Banco de Dados ✨ **NOVO**

```python
from backend.database import session_scope, DatabaseQueries
from backend.database.models import Agent, CreatedBy, Gender
from datetime import datetime
from decimal import Decimal

# Criar um agente com dados complexos
with session_scope() as session:
    queries = DatabaseQueries(session)
    
    agent = queries.agents.create(
        name="Dr. Ana Silva",
        created_by=CreatedBy.IA,
        birth_date=datetime(1990, 3, 15),
        gender=Gender.FEMALE,
        wallet=Decimal("15000.00"),
        energy_level=85,
        version="0.1.0",
        skills={"programming": 95, "leadership": 80},
        personality={"openness": 0.8, "conscientiousness": 0.9},
        genetics={"hair_color": "brown", "intelligence_factor": 1.1}
    )
    
    print(f"Agente criado: {agent.name}, {agent.age} anos")
    print(f"Carteira: R$ {agent.wallet}")

# Ver exemplo completo
# python examples/database_demo.py
```

**Quick Start do Banco:**
```bash
# Inicializar banco SQLite (desenvolvimento)
python scripts/init_database.py --sqlite init
python scripts/init_database.py --sqlite seed

# Ou PostgreSQL (produção)
python scripts/init_database.py init
python scripts/init_database.py seed
```

📖 **Documentação Completa**: [Database Guide](docs/database/DATABASE_GUIDE.md)
print(f"Database URL: {config.DATABASE_URL}")
print(f"Agent work hours: {config.AGENT_WORK_START_HOUR}h - {config.AGENT_WORK_END_HOUR}h")

# Garantir que os diretórios necessários existem
config.ensure_directories()
```

## 📝 TODOs e Issues Prioritárias

O projeto possui **arquivos TODO detalhados** para as issues prioritárias da **Fase 0** (Infraestrutura Crítica).

### 🎯 Issues com TODOs Prontos

Criamos arquivos com código, templates e instruções completas para implementar:

1. **Issue #1**: Estrutura do projeto (`TODO_ISSUE_01_project_structure.py`)
2. **Issue #2**: Sistema de logging (`TODO_ISSUE_02_logging_system.py`)
3. **Issue #3**: Configuração YAML (`TODO_ISSUE_03_config_system.py`)
4. **Issue #4**: Banco de dados SQLite (`TODO_ISSUE_04_database.py`)
5. **Issue #5**: Documentação de arquitetura (`TODO_ISSUE_05_architecture_docs.py`)

### 📚 Guia de Uso

Consulte [README_TODOS.md](README_TODOS.md) para:
- Descrição de cada issue
- Ordem recomendada de implementação
- Como usar os arquivos TODO
- Dicas e exemplos

### 💡 Para Começar

```bash
# Ver TODOs disponíveis
ls TODO_ISSUE_*.py

# Ler guia completo
cat README_TODOS.md

# Exemplo: Revisar Issue #1
cat TODO_ISSUE_01_project_structure.py
```

Cada arquivo TODO contém:
- ✅ Código pronto para adaptar
- ✅ Estruturas de dados completas
- ✅ Exemplos de uso
- ✅ Instruções passo a passo

## 🤝 Contribuição

Contribuições são muito bem-vindas! Este projeto segue boas práticas de desenvolvimento e code review.

### Como Contribuir

1. **Fork o Repositório**
   ```bash
   # Clique em "Fork" no GitHub e depois clone seu fork
   git clone https://github.com/seu-usuario/ferritine.git
   cd ferritine
   ```

2. **Crie uma Branch para sua Feature**
   ```bash
   git checkout -b feature/minha-nova-feature
   # ou
   git checkout -b fix/correcao-de-bug
   ```

3. **Faça suas Alterações**
   - Escreva código limpo e bem documentado
   - Siga o padrão PEP 8 (Python)
   - Adicione testes para novas funcionalidades
   - Adicione docstrings em português para funções e classes

4. **Execute os Testes**
   ```bash
   pytest -v
   ```

5. **Commit suas Mudanças**
   ```bash
   git add .
   git commit -m "feat: adiciona nova funcionalidade X"
   ```
   
   **Padrão de Commits** (Conventional Commits):
   - `feat:` - Nova funcionalidade
   - `fix:` - Correção de bug
   - `docs:` - Mudanças na documentação
   - `test:` - Adição ou modificação de testes
   - `refactor:` - Refatoração de código
   - `chore:` - Tarefas de manutenção

6. **Push para seu Fork**
   ```bash
   git push origin feature/minha-nova-feature
   ```

7. **Abra um Pull Request**
   - Vá para o repositório original no GitHub
   - Clique em "New Pull Request"
   - Descreva suas mudanças detalhadamente
   - **Adicione labels apropriados** ao PR:
     - `feature` - para novas funcionalidades
     - `bug` - para correções de bugs
     - `documentation` - para melhorias na documentação
     - `chore` - para tarefas de manutenção
     - `test` - para melhorias em testes
   - Os labels ajudam o Release Drafter a organizar o changelog automaticamente!

### 🏷️ Sistema de Labels

Este projeto usa labels para organizar PRs e issues. Quando criar um PR, adicione labels apropriados:

**Tipo de Mudança:**
- `feature`, `enhancement` - Nova funcionalidade
- `bug`, `fix` - Correção de bug
- `documentation`, `docs` - Documentação
- `chore`, `maintenance`, `refactor` - Manutenção
- `test` - Testes
- `performance` - Melhorias de performance
- `dependencies` - Atualização de dependências

**Impacto na Versão:**
- `major`, `breaking` - Mudança breaking (v2.0.0)
- `minor` - Nova funcionalidade (v1.1.0)
- `patch` - Correção de bug (v1.0.1)

**Prioridade:**
- `priority:critical` - Urgente
- `priority:high` - Alta
- `priority:medium` - Média
- `priority:low` - Baixa

Ver arquivo [`.github/labels.yml`](.github/labels.yml) para a lista completa de labels.

### 🚀 Fluxo de Release Automatizado

Este projeto usa **Release Drafter** para gerar changelogs automaticamente:

1. **Você cria um PR** e adiciona labels apropriados
2. **Merge do PR** → Release Drafter atualiza o draft automaticamente
3. **Quando pronto** → Você publica o release (manualmente ou com tag)
4. **Changelog gerado** → Organizado por categoria baseado nos labels!

📖 **Leia o guia completo**: [docs/guides/WORKFLOWS_GUIDE.md](docs/guides/WORKFLOWS_GUIDE.md)

### Reportando Bugs

Use o [template de bug report](.github/ISSUE_TEMPLATE/bug_report.md) para reportar bugs. Inclua:
- Descrição clara do problema
- Passos para reproduzir
- Comportamento esperado vs. atual
- Informações do ambiente (SO, versão do Python, etc.)

### Solicitando Funcionalidades

Use o [template de feature request](.github/ISSUE_TEMPLATE/feature_request.md) para sugerir novas funcionalidades.

### Diretrizes de Código

- **Estilo**: Siga PEP 8
- **Documentação**: Todas as classes e funções públicas devem ter docstrings
- **Testes**: Novas funcionalidades devem incluir testes
- **Idioma**: Código e comentários em português
- **Type Hints**: Use type hints quando possível

📖 **Leia mais**: [Guia Completo de Contribuição](docs/guides/CONTRIBUTING.md)

## 🔖 Versionamento

Este projeto usa [Versionamento Semântico (SemVer)](https://semver.org/lang/pt-BR/):

- **MAJOR** (X.0.0): Mudanças incompatíveis na API
- **MINOR** (0.X.0): Novas funcionalidades compatíveis com versões anteriores
- **PATCH** (0.0.X): Correções de bugs compatíveis com versões anteriores

A versão atual está no arquivo `VERSION` na raiz do projeto.

### 🤖 Releases Automatizados (Recomendado)

O projeto possui **3 formas de criar releases**, sendo a forma **2** a mais recomendada:

#### **Forma 1: Automático ao Mergear PRs**
- Quando PRs são merged para `main`, o workflow `release.yml` pode criar releases automaticamente
- Útil para projetos que querem release a cada merge
- Requer configuração no workflow

#### **Forma 2: Release Drafter + Tag (RECOMENDADO) 🌟**
Este é o fluxo mais moderno e recomendado:

1. **Desenvolva normalmente** - Crie PRs e adicione labels apropriados
2. **Merge PRs** - O Release Drafter atualiza o draft automaticamente
3. **Revise o draft** - Vá em "Releases" e veja o changelog gerado
4. **Crie uma tag quando pronto**:
   ```bash
   git tag v0.2.0 -m "Release v0.2.0"
   git push origin v0.2.0
   ```
5. **Release publicado automaticamente!** 🎉

**Vantagens:**
- ✅ Changelog organizado por categorias
- ✅ Controle sobre quando publicar
- ✅ Revisão antes de publicar
- ✅ Versionamento sugerido automaticamente

#### **Forma 3: Manual via GitHub Actions**
1. Acesse a aba **Actions** no GitHub
2. Selecione o workflow **"Release (bump & create)"**
3. Clique em **"Run workflow"**
4. Escolha o nível de incremento (patch/minor/major)
5. Confirme a execução

O workflow irá:
- ✅ Incrementar a versão no arquivo `VERSION`
- ✅ Fazer commit e push da nova versão
- ✅ Criar uma tag Git (`vX.Y.Z`)
- ✅ Gerar changelog automaticamente
- ✅ Criar uma Release no GitHub com o changelog

### 📚 Documentação Completa de Workflows

Para entender todos os workflows, como funcionam os labels, e o fluxo completo de desenvolvimento:

📖 **Leia**: [docs/guides/WORKFLOWS_GUIDE.md](docs/guides/WORKFLOWS_GUIDE.md)

### Atualizando a Versão Localmente (para desenvolvimento)

Use o script de bump de versão:

```bash
# Incrementar patch version (0.1.0 -> 0.1.1)
python .github/scripts/bump_version.py --level patch

# Incrementar minor version (0.1.0 -> 0.2.0)
python .github/scripts/bump_version.py --level minor

# Incrementar major version (0.1.0 -> 1.0.0)
python .github/scripts/bump_version.py --level major
```

## 📝 Changelog

O changelog é gerado automaticamente durante o processo de release, baseado nos commits entre as tags. Para melhores resultados, use [Conventional Commits](https://www.conventionalcommits.org/pt-br/) nas mensagens de commit.

📖 **Ver histórico completo**: [CHANGELOG.md](docs/CHANGELOG.md)

## 📚 Documentação

Toda a documentação do projeto está organizada na pasta `docs/`. Consulte o [📖 índice da documentação](docs/README.md) para navegação completa.

### 🚀 Guias de Início Rápido

- ⚡ [START_HERE](docs/guides/START_HERE.md) - Backend para Unity em 3 comandos
- 📖 [Guia de Início Rápido](docs/guides/QUICKSTART.md) - Comece a usar em 5 minutos
- 🐳 [Docker Guide](docs/guides/DOCKER_GUIDE.md) - Usando Docker
- 🐛 [Troubleshooting](docs/guides/TROUBLESHOOTING.md) - Resolução de problemas

### 👥 Contribuição e Desenvolvimento

- 🤝 [Guia de Contribuição](docs/guides/CONTRIBUTING.md) - Como contribuir para o projeto
- 🔧 [Guia de Workflows](docs/guides/WORKFLOWS_GUIDE.md) - CI/CD e automações
- 📋 [Quick Issue Guide](docs/guides/QUICK_ISSUE_GUIDE.md) - Criando issues

### 🏗️ Arquitetura e Design

- 📊 [Game Design Document](docs/architecture/GDD_FERRITINE.md) - Conceito e design completo
- 🏗️ [Documentação Técnica da Maquete](docs/architecture/MAQUETE_TECH_DOCS.md) - Specs técnicas
- 📋 [Issues, Milestones & Tags](docs/architecture/ISSUES_MILESTONES_TAGS.md) - Planejamento

### 💾 Banco de Dados

- 💾 [Database Guide](docs/database/DATABASE_GUIDE.md) - Guia completo do banco de dados
- 🏢 [Building Model](docs/database/DATABASE_BUILDING_USAGE.md) - Modelo de edifícios

### 🎮 Unity

- 🎮 [Unity Integration Guide](docs/unity/UNITY_INTEGRATION_GUIDE.md) - Integração completa
- 🎨 [Agent Animations](docs/unity/AGENT_ANIMATION_IMPLEMENTATION.md) - Sistema de animações
- 🔌 [API Endpoints](docs/unity/API_ENDPOINTS.md) - Endpoints da API

### 🔨 Desenvolvimento

- 📝 [Implementation Summary](docs/development/IMPLEMENTATION_SUMMARY.md) - Resumo das implementações
- 🚀 [Next Steps](docs/development/NEXT_STEPS.md) - Próximos passos
- 📋 [README TODOs](docs/development/README_TODOS.md) - TODOs prioritários

### 📝 Histórico

- 📝 [Changelog](docs/CHANGELOG.md) - Histórico de mudanças

---

📂 **[Ver índice completo da documentação →](docs/README.md)**

## 📄 Licença
Este projeto está licenciado sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 🗺️ Roadmap

O projeto está organizado em **fases bem definidas** com issues e milestones detalhados:

### 📋 Planejamento Completo

Consulte os seguintes documentos para ver o plano completo de desenvolvimento:

- **[ISSUES_MILESTONES_TAGS.md](ISSUES_MILESTONES_TAGS.md)** - 55 issues detalhadas organizadas em milestones
- **[docs/guides/QUICK_ISSUE_GUIDE.md](docs/guides/QUICK_ISSUE_GUIDE.md)** - Guia rápido para criar issues no GitHub
- **[gdd_ferritine.md](gdd_ferritine.md)** - Game Design Document completo (3600+ linhas)

### Fases do Projeto

- **Fase 0**: Fundamentos e Infraestrutura (Mês 1-2)
- **Fase 1**: Simulação Digital Básica (Mês 3-4)
  - 1.1: Mundo Estático
  - 1.2: Agentes Simples
  - 1.3: Economia Básica
  - 1.4: Transporte Ferroviário Virtual
- **Fase 2**: Hardware Básico (Mês 5-7)
  - 2.1: Circuito de Iluminação
  - 2.2: Sensor de Trem
  - 2.3: Controle de Desvio
- **Fase 3**: Maquete Física Inicial (Mês 8-12)
  - 3.1: Base e Topografia
  - 3.2: Trilhos e Primeiro Trem
  - 3.3: Primeiros Edifícios
  - 3.4: Integração Física-Digital
- **Fase 4**: Expansão e Refinamento (Ano 2+)

### Funcionalidades Planejadas

- [ ] Interface gráfica 2D/3D para visualização da simulação
- [ ] Integração com hardware (Arduino/ESP32/Raspberry Pi)
- [ ] Múltiplos tipos de agentes com IA comportamental
- [ ] Sistema econômico completo (oferta, demanda, inflação)
- [ ] Sistema político (eleições, políticas públicas)
- [ ] Eventos aleatórios e emergentes
- [ ] Persistência de estado (salvar/carregar simulações)
- [ ] API REST para controle remoto
- [ ] Dashboard web em tempo real
- [ ] Realidade Aumentada (AR) via mobile
- [ ] Sistema de transporte multimodal (trens, ônibus, carros)
- [ ] Geração de notícias com IA

## 🙏 Agradecimentos

Este projeto foi desenvolvido como parte de uma maquete híbrida físico-digital, combinando simulação computacional com elementos físicos para criar uma experiência educacional e interativa.

## 📞 Contato

Se você tiver dúvidas, sugestões ou quiser contribuir, sinta-se à vontade para:

- Abrir uma [issue](https://github.com/ferritine/ferritine/issues)
- Enviar um [pull request](https://github.com/ferritine/ferritine/pulls)
- Entrar em contato através do GitHub

---

<div align="center">

**Feito com ❤️ para simulação urbana e IoT**

[![Stars](https://img.shields.io/github/stars/ferritine/ferritine?style=social)](https://github.com/ferritine/ferritine/stargazers)
[![Forks](https://img.shields.io/github/forks/ferritine/ferritine?style=social)](https://github.com/ferritine/ferritine/network/members)

</div>

