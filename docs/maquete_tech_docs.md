# Maquete Viva - Documentação Técnica Completa
## Sistema de Documentação, Versionamento e Arquitetura

---

## 📚 ÍNDICE DA DOCUMENTAÇÃO TÉCNICA

1. [Sistema de Versionamento Semântico](#versionamento)
2. [Estrutura de Documentação Git](#git-docs)
3. [Manifesto de Design](#manifesto)
4. [Arquitetura de Software Detalhada](#arquitetura)
5. [Guia de Banco de Dados](#database)
6. [Engine 3D: Unity vs Godot](#engine-3d)
7. [GitHub Actions e Automações](#actions)
8. [Sistema de Issues e Projects](#issues)
9. [Releases e Changelog](#releases)
10. [Guias de Contribuição](#contributing)

---

## 🔢 SISTEMA DE VERSIONAMENTO SEMÂNTICO {#versionamento}

### Convenção Semantic Versioning (SemVer)

**Formato**: `MAJOR.MINOR.PATCH-STAGE.BUILD`

**Exemplo**: `v1.2.3-alpha.5`

### Estrutura de Versão

```
v MAJOR . MINOR . PATCH - STAGE . BUILD
│   │      │       │       │       │
│   │      │       │       │       └─ Build number (opcional)
│   │      │       │       └────────── pre-release stage
│   │      │       └────────────────── Bug fixes
│   │      └────────────────────────── New features (backward compatible)
│   └───────────────────────────────── Breaking changes
└───────────────────────────────────── Version prefix
```

### Regras de Incremento

#### MAJOR (v1.0.0 → v2.0.0)
**Quando usar**:
- Mudanças que quebram compatibilidade
- Refatoração completa de sistema
- Remoção de features importantes

**Exemplos**:
- Migração de Pygame para Unity/Godot
- Mudança de SQLite para PostgreSQL (sem migração automática)
- Redesign completo da API

#### MINOR (v1.0.0 → v1.1.0)
**Quando usar**:
- Novas features que não quebram código existente
- Adição de novos sistemas
- Expansões de funcionalidade

**Exemplos**:
- Adicionar sistema político
- Implementar AR (Realidade Aumentada)
- Novo tipo de veículo (ônibus)

#### PATCH (v1.0.0 → v1.0.1)
**Quando usar**:
- Correções de bugs
- Melhorias de performance
- Ajustes de balanceamento
- Correções de documentação

**Exemplos**:
- Corrigir sensor que não detecta trem
- Otimizar loop de agentes (2x mais rápido)
- Consertar bug onde agentes não comem

### Estágios de Desenvolvimento

#### Pre-Alpha (`v0.0.1-prealpha`)
**Características**:
- Protótipos iniciais
- Features incompletas
- Código experimental
- Nada é estável

**Duração Esperada**: Meses 1-3 (Fase 0-1 do GDD)

#### Alpha (`v0.1.0-alpha`)
**Características**:
- Features principais implementadas
- Muitos bugs esperados
- API pode mudar drasticamente
- Apenas para desenvolvedores

**Critérios**:
- ✅ Simulação básica funciona (50+ agentes)
- ✅ Economia simples operacional
- ✅ Arduino + Python comunicando
- ⚠️ Interface rudimentar

**Duração Esperada**: Meses 4-8 (Fase 2 do GDD)

#### Beta (`v0.5.0-beta`)
**Características**:
- Todas features principais completas
- Poucos bugs críticos
- API relativamente estável
- Testadores externos podem usar

**Critérios**:
- ✅ Maquete física funcional (1m²)
- ✅ 100+ agentes estáveis
- ✅ Dashboard web funcionando
- ✅ Sistema de sensores integrado
- ⚠️ Falta polimento

**Duração Esperada**: Meses 9-18 (Fase 3-4 do GDD)

#### Release Candidate (`v1.0.0-rc.1`)
**Características**:
- Potencialmente pronto para produção
- Testes finais
- Apenas bugs menores
- Documentação quase completa

**Critérios**:
- ✅ Sem bugs críticos conhecidos
- ✅ Performance aceitável
- ✅ Documentação 90% completa
- ✅ Testado por pelo menos 3 pessoas

**Duração Esperada**: Mês 19-20

#### Stable Release (`v1.0.0`)
**Características**:
- Pronto para uso público
- Testado extensivamente
- Documentação completa
- Suporte garantido

**Critérios**:
- ✅ Todos critérios de RC atendidos
- ✅ Pelo menos 1 mês sem bugs críticos
- ✅ README, tutoriais e guias completos

### Exemplos de Histórico de Versões

```
v0.0.1-prealpha    - 2025-01-15 - Primeiro commit, estrutura básica
v0.0.5-prealpha    - 2025-02-01 - Classe Agente implementada
v0.1.0-alpha       - 2025-03-10 - Simulação com 10 agentes funcionando
v0.2.0-alpha       - 2025-04-20 - Economia básica + Arduino integrado
v0.3.0-alpha       - 2025-06-05 - Maquete física iniciada
v0.5.0-beta        - 2025-09-15 - Maquete 1m² completa + 100 agentes
v0.6.0-beta        - 2025-11-20 - Dashboard web funcional
v0.8.0-beta        - 2026-02-10 - Sistema político + AR básico
v1.0.0-rc.1        - 2026-04-15 - Release Candidate 1
v1.0.0-rc.2        - 2026-05-01 - RC2 (correções finais)
v1.0.0             - 2026-06-01 - 🎉 LANÇAMENTO OFICIAL!
v1.1.0             - 2026-08-15 - Nova feature: Aeroporto
v1.1.1             - 2026-08-22 - Hotfix: Bug no pathfinding
v1.2.0             - 2026-10-30 - Expansão: Sistema educacional
v2.0.0             - 2027-03-20 - Migração para Unity 3D
```

---

## 📁 ESTRUTURA DE DOCUMENTAÇÃO GIT {#git-docs}

### Árvore de Documentação Completa

```
maquete_viva/
│
├── README.md                          # Visão geral, quickstart
├── CHANGELOG.md                       # Histórico de versões
├── CONTRIBUTING.md                    # Como contribuir
├── CODE_OF_CONDUCT.md                 # Código de conduta
├── LICENSE                            # Licença (MIT, GPL, etc)
│
├── docs/
│   ├── index.md                       # Índice da documentação
│   │
│   ├── getting-started/
│   │   ├── README.md                  # Introdução
│   │   ├── installation.md            # Guia de instalação
│   │   ├── quickstart.md              # Primeiro uso
│   │   ├── hardware-setup.md          # Configurar Arduino/sensores
│   │   └── troubleshooting.md         # Problemas comuns
│   │
│   ├── architecture/
│   │   ├── README.md                  # Visão geral arquitetura
│   │   ├── design-manifesto.md        # Princípios de design
│   │   ├── software-architecture.md   # Diagrams + patterns
│   │   ├── database-schema.md         # Estrutura BD completa
│   │   ├── api-reference.md           # Endpoints REST/WebSocket
│   │   └── data-flow.md               # Fluxo de dados
│   │
│   ├── simulation/
│   │   ├── agents.md                  # Sistema de agentes
│   │   ├── economy.md                 # Modelo econômico
│   │   ├── transport.md               # Logística de transporte
│   │   ├── politics.md                # Sistema político
│   │   └── events.md                  # Eventos e narrativa
│   │
│   ├── hardware/
│   │   ├── electronics-basics.md      # Eletrônica para iniciantes
│   │   ├── arduino-guide.md           # Programação Arduino
│   │   ├── esp32-guide.md             # IoT com ESP32
│   │   ├── sensors.md                 # Tipos de sensores
│   │   ├── actuators.md               # Servos, motores, LEDs
│   │   ├── dcc-control.md             # Sistema DCC para trens
│   │   └── circuits/                  # Diagramas Fritzing
│   │       ├── led-circuit.png
│   │       ├── sensor-circuit.png
│   │       └── switch-circuit.png
│   │
│   ├── physical-build/
│   │   ├── materials.md               # Lista de materiais
│   │   ├── tools.md                   # Ferramentas necessárias
│   │   ├── base-construction.md       # Construir base
│   │   ├── terrain.md                 # Relevo e topografia
│   │   ├── buildings.md               # Construir prédios
│   │   ├── tracks.md                  # Instalar trilhos
│   │   ├── weathering.md              # Técnicas de envelhecimento
│   │   └── lighting.md                # Sistema de iluminação
│   │
│   ├── software-guide/
│   │   ├── python-setup.md            # Ambiente Python
│   │   ├── running-simulation.md      # Executar simulação
│   │   ├── configuration.md           # Arquivo config.yaml
│   │   ├── database-setup.md          # Configurar SQLite/PostgreSQL
│   │   ├── mqtt-setup.md              # Broker MQTT
│   │   └── web-dashboard.md           # Dashboard web
│   │
│   ├── 3d-visualization/
│   │   ├── engine-comparison.md       # Unity vs Godot vs Blender
│   │   ├── unity-setup.md             # Projeto Unity
│   │   ├── godot-setup.md             # Projeto Godot
│   │   ├── blender-export.md          # Exportar modelos Blender
│   │   ├── ar-implementation.md       # AR com ARKit/ARCore
│   │   └── assets/                    # Modelos 3D, texturas
│   │
│   ├── tutorials/
│   │   ├── first-agent.md             # Criar primeiro agente
│   │   ├── first-building.md          # Construir primeiro prédio
│   │   ├── first-train.md             # Trem funcionando
│   │   ├── add-sensor.md              # Adicionar sensor
│   │   ├── custom-event.md            # Criar evento customizado
│   │   └── ai-training.md             # Treinar modelo ML
│   │
│   ├── api/
│   │   ├── rest-api.md                # Documentação REST
│   │   ├── websocket-api.md           # Documentação WebSocket
│   │   ├── mqtt-topics.md             # Tópicos MQTT
│   │   └── examples/                  # Exemplos de uso
│   │       ├── get-agents.py
│   │       ├── control-train.py
│   │       └── subscribe-events.js
│   │
│   ├── development/
│   │   ├── setup-dev-environment.md   # Ambiente desenvolvimento
│   │   ├── coding-standards.md        # Padrões de código
│   │   ├── git-workflow.md            # Fluxo Git (branching)
│   │   ├── testing.md                 # Testes unitários
│   │   ├── ci-cd.md                   # GitHub Actions
│   │   └── debugging.md               # Técnicas de debug
│   │
│   ├── deployment/
│   │   ├── requirements.md            # Requisitos sistema
│   │   ├── installation.md            # Instalação produção
│   │   ├── docker.md                  # Containerização
│   │   ├── raspberry-pi.md            # Deploy em RaspberryPi
│   │   └── backup.md                  # Backup do banco de dados
│   │
│   ├── faq.md                         # Perguntas frequentes
│   ├── glossary.md                    # Glossário técnico
│   ├── resources.md                   # Links úteis
│   └── roadmap.md                     # Roadmap futuro
│
├── .github/
│   ├── ISSUE_TEMPLATE/
│   │   ├── bug_report.md              # Template bug
│   │   ├── feature_request.md         # Template feature
│   │   └── question.md                # Template pergunta
│   │
│   ├── PULL_REQUEST_TEMPLATE.md       # Template PR
│   │
│   ├── workflows/
│   │   ├── tests.yml                  # Testes automáticos
│   │   ├── lint.yml                   # Linting código
│   │   ├── docs.yml                   # Build documentação
│   │   ├── release.yml                # Criar release
│   │   └── version-bump.yml           # Atualizar versão
│   │
│   └── dependabot.yml                 # Atualizações automáticas
│
└── examples/
    ├── minimal-simulation.py          # Exemplo mínimo
    ├── custom-agent.py                # Agente customizado
    ├── arduino-basic/                 # Sketch Arduino básico
    └── web-client/                    # Cliente web exemplo
```

---

## 🎨 MANIFESTO DE DESIGN {#manifesto}

### Princípios Fundamentais

#### 1. **Simplicidade Progressiva**
> "Fácil de começar, infinito para dominar"

**Na Prática**:
- Interface básica para iniciantes (botões grandes, tutorial guiado)
- Menus avançados escondidos, mas acessíveis
- Shortcuts para usuários experientes
- Tooltips informativos em todos os elementos

**Exemplo**:
```
Iniciante vê:  [▶️ Iniciar Simulação] [⏸️ Pausar] [⚙️ Configurações]
Avançado vê:   +100 atalhos de teclado, console de comandos, scripts Lua
```

#### 2. **Físico e Digital São Um Só**
> "A maquete física e a simulação são faces da mesma moeda"

**Na Prática**:
- Sincronização em tempo real (latência <100ms)
- Mudanças físicas refletem no digital (sensor detecta → simulação atualiza)
- Mudanças digitais podem afetar físico (simulação decide → servo move desvio)
- AR como ponte visual entre mundos

#### 3. **Falha é Feature**
> "Acidentes, bugs e imperfeições criam narrativas"

**Na Prática**:
- Bugs viram histórias (agente bugado vira lenda local)
- Falhas de hardware são eventos narrativos (trem descarrilha → crise)
- Sistema de "memória" registra tudo (historiadores futuros leem logs)
- Modo "realista" vs "sandbox" (erros são opcionais)

#### 4. **Modularidade Radical**
> "Tudo pode ser desligado, trocado ou expandido"

**Na Prática**:
- Sistemas independentes (economia funciona sem transporte)
- Plugins/mods fáceis de criar
- Configuração extensiva (YAML, JSON)
- Arquitetura baseada em eventos (desacoplamento)

#### 5. **Acessibilidade Sem Concessões**
> "Complexidade profunda com entrada gentil"

**Na Prática**:
- Tutoriais interativos (não só texto)
- Simulador sem hardware (modo "virtual" puro)
- Documentação em níveis (iniciante → expert)
- Código bem comentado, arquitetura clara

#### 6. **Performance é Respeitabilidade**
> "Cada frame importa, cada segundo do usuário é sagrado"

**Na Prática**:
- Simulação roda em 60 FPS mesmo com 500 agentes
- Otimização constante (profiling regular)
- Loading screens informativos (nunca silêncio)
- Degradação graciosa (se lento, avisa e sugere opções)

#### 7. **Estética é Funcional**
> "Beleza serve à clareza"

**Na Prática**:
- Cores indicam estado (vermelho = problema, verde = ok)
- Animações têm propósito (não apenas "bonito")
- UI segue hierarquia visual clara
- Consistência em todo sistema

#### 8. **Dados São Tesouros**
> "Cada número conta uma história"

**Na Prática**:
- Logs detalhados de tudo
- Estatísticas exportáveis (CSV, JSON)
- Visualizações de dados (gráficos, heatmaps)
- Histórico completo (time-travel debugging possível)

#### 9. **Comunidade é Core**
> "O projeto cresce com quem usa"

**Na Prática**:
- GitHub Discussions ativo
- Aceitar PRs com guidelines claras
- Créditos visíveis para contribuidores
- Roadmap influenciado por comunidade

#### 10. **Diversão Acima de Tudo**
> "Se não for divertido, não vale a pena"

**Na Prática**:
- Easter eggs escondidos
- Eventos absurdos ocasionais (invasão de patos?)
- Humor sutil em logs/erros
- Celebração de conquistas (achievements com animações)

### Anti-Padrões a Evitar

❌ **Feature Creep Descontrolado**
- ✅ Fazer: Planejar releases com escopo fechado
- ❌ Evitar: "Só mais uma funcionalidade..."

❌ **Over-Engineering Prematuro**
- ✅ Fazer: Código simples que funciona
- ❌ Evitar: "Vou criar um sistema ultra-flexível que..."

❌ **Documentação Como Afterthought**
- ✅ Fazer: Documentar enquanto desenvolve
- ❌ Evitar: "Documento depois que funcionar"

❌ **UI Confusa por "Profissionalismo"**
- ✅ Fazer: Botões claros, labels óbvios
- ❌ Evitar: Ícones obscuros sem tooltip

❌ **Otimização Prematura**
- ✅ Fazer: Fazer funcionar, depois otimizar
- ❌ Evitar: "Vou usar esse algoritmo complexo caso..."

---

## 🏗️ ARQUITETURA DE SOFTWARE DETALHADA {#arquitetura}

### Visão Geral: Clean Architecture Adaptada

```
┌─────────────────────────────────────────────────────────┐
│                   PRESENTATION LAYER                    │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ Web         │  │ Mobile       │  │ AR           │  │
│  │ Dashboard   │  │ App          │  │ Viewer       │  │
│  └──────┬──────┘  └──────┬───────┘  └──────┬───────┘  │
└─────────┼─────────────────┼──────────────────┼──────────┘
          │                 │                  │
          └────────────┬────┴──────────────────┘
                       │
        ┌──────────────▼──────────────────┐
        │      APPLICATION LAYER          │
        │  ┌──────────────────────────┐  │
        │  │  Controllers             │  │
        │  │  - CityController        │  │
        │  │  - AgentController       │  │
        │  │  - TransportController   │  │
        │  └────────┬─────────────────┘  │
        └───────────┼─────────────────────┘
                    │
        ┌───────────▼──────────────────┐
        │      DOMAIN LAYER            │
        │  ┌───────────────────────┐  │
        │  │  Business Logic       │  │
        │  │  - AgentBehavior      │  │
        │  │  - EconomySimulator   │  │
        │  │  - EventEngine        │  │
        │  │  - PathfindingService │  │
        │  └───────┬───────────────┘  │
        │          │                   │
        │  ┌───────▼───────────────┐  │
        │  │  Domain Models        │  │
        │  │  - Agent              │  │
        │  │  - Building           │  │
        │  │  - Vehicle            │  │
        │  │  - City               │  │
        │  └───────┬───────────────┘  │
        └──────────┼───────────────────┘
                   │
        ┌──────────▼─────────────────┐
        │   INFRASTRUCTURE LAYER     │
        │  ┌──────────────────────┐  │
        │  │  Data Access         │  │
        │  │  - Repository        │  │
        │  │  - ORM (SQLAlchemy)  │  │
        │  └──────────────────────┘  │
        │  ┌──────────────────────┐  │
        │  │  External Services   │  │
        │  │  - MQTT Client       │  │
        │  │  - Serial Comm       │  │
        │  │  - File Storage      │  │
        │  └──────────────────────┘  │
        └────────────────────────────┘
                   │
        ┌──────────▼──────────────┐
        │   DATABASE & HARDWARE   │
        │  ┌──────────────────┐   │
        │  │ SQLite/Postgres  │   │
        │  └──────────────────┘   │
        │  ┌──────────────────┐   │
        │  │ Arduino/ESP32    │   │
        │  └──────────────────┘   │
        └─────────────────────────┘
```

### Padrões de Design Utilizados

#### 1. **Repository Pattern**
**Propósito**: Abstrai acesso ao banco de dados

```python
# backend/infrastructure/repositories/agent_repository.py

from typing import List, Optional
from backend.domain.models.agent import Agent

class AgentRepository:
    """
    Responsável por toda interação com BD relacionada a Agentes
    """
    
    def __init__(self, db_session):
        self.session = db_session
    
    def get_by_id(self, agent_id: int) -> Optional[Agent]:
        """Busca agente por ID"""
        pass
    
    def get_all(self) -> List[Agent]:
        """Retorna todos agentes"""
        pass
    
    def get_by_location(self, location_id: int) -> List[Agent]:
        """Agentes em determinada localização"""
        pass
    
    def save(self, agent: Agent) -> Agent:
        """Salva ou atualiza agente"""
        pass
    
    def delete(self, agent_id: int) -> bool:
        """Remove agente"""
        pass
    
    def count(self) -> int:
        """Conta total de agentes"""
        pass
```

#### 2. **Service Layer Pattern**
**Propósito**: Orquestra lógica de negócio complexa

```python
# backend/application/services/transport_service.py

class TransportService:
    """
    Orquestra sistema de transporte
    """
    
    def __init__(self, vehicle_repo, route_repo, event_bus):
        self.vehicle_repo = vehicle_repo
        self.route_repo = route_repo
        self.event_bus = event_bus
    
    def schedule_train(self, train_id: int, route_id: int):
        """
        Agenda trem em rota
        - Valida disponibilidade
        - Calcula horários
        - Reserva recurso
        - Emite eventos
        """
        train = self.vehicle_repo.get_by_id(train_id)
        route = self.route_repo.get_by_id(route_id)
        
        if not self._is_available(train, route):
            raise ConflictException("Trem indisponível")
        
        schedule = self._calculate_schedule(train, route)
        train.assign_route(route, schedule)
        
        self.vehicle_repo.save(train)
        self.event_bus.emit("train_scheduled", {
            "train_id": train_id,
            "route_id": route_id,
            "departure": schedule.departure
        })
        
        return schedule
```

#### 3. **Observer Pattern (Event Bus)**
**Propósito**: Comunicação desacoplada entre sistemas

```python
# backend/infrastructure/event_bus.py

class EventBus:
    """Sistema de eventos global"""
    
    _instance = None
    
    def __new__(cls):
        if cls._instance is None:
            cls._instance = super().__new__(cls)
            cls._instance._listeners = {}
        return cls._instance
    
    def subscribe(self, event_type: str, handler: Callable):
        if event_type not in self._listeners:
            self._listeners[event_type] = []
        self._listeners[event_type].append(handler)
    
    def emit(self, event_type: str, data: dict):
        if event_type in self._listeners:
            for handler in self._listeners[event_type]:
                handler(data)

# Uso em diferentes partes do sistema:

# Sistema de transporte emite:
event_bus.emit("train_arrived", {"train_id": 1, "station_id": 5})

# Sistema de economia escuta:
def on_train_arrival(data):
    calculate_passenger_revenue(data["train_id"])
    
event_bus.subscribe("train_arrived", on_train_arrival)

# Sistema de UI escuta:
def on_train_arrival_ui(data):
    show_notification(f"Trem {data['train_id']} chegou!")
    
event_bus.subscribe("train_arrived", on_train_arrival_ui)
```

#### 4. **Factory Pattern**
**Propósito**: Criação complexa de objetos

```python
# backend/domain/factories/agent_factory.py

class AgentFactory:
    """
    Cria agentes com atributos procedurais realistas
    """
    
    @staticmethod
    def create_random_citizen(city_context: City) -> Agent:
        """
        Gera cidadão com atributos aleatórios mas coerentes
        """
        age = random.randint(18, 70)
        name = NameGenerator.generate(city_context.culture)
        
        # Atributos correlacionados com idade
        if age < 30:
            energy = random.randint(70, 100)
            health = random.randint(80, 100)
        else:
            energy = random.randint(50, 80)
            health = random.randint(60, 90)
        
        # Profissão baseada em educação e idade
        education = random.randint(0, 100)
        if education > 80 and age > 25:
            job = JobFactory.create_professional_job()
        else:
            job = JobFactory.create_manual_job()
        
        return Agent(
            name=name,
            age=age,
            education=education,
            job=job,
            health=health,
            energy=energy,
            # ... mais atributos
        )
```

#### 5. **Strategy Pattern**
**Propósito**: Comportamentos intercambiáveis

```python
# backend/domain/strategies/pathfinding.py

from abc import ABC, abstractmethod

class PathfindingStrategy(ABC):
    @abstractmethod
    def find_path(self, start, end, graph):
        pass

class AStarPathfinding(PathfindingStrategy):
    def find_path(self, start, end, graph):
        # Implementação A*
        pass

class DijkstraPathfinding(PathfindingStrategy):
    def find_path(self, start, end, graph):
        # Implementação Dijkstra
        pass

class SimplePathfinding(PathfindingStrategy):
    def find_path(self, start, end, graph):
        # Caminho mais simples (menos computação)
        pass

# Uso:
class Agent:
    def __init__(self, pathfinding_strategy: PathfindingStrategy):
        self.pathfinding = pathfinding_strategy
    
    def move_to(self, destination):
        path = self.pathfinding.find_path(self.location, destination, world.graph)
        self.follow_path(path)

# Configurável:
config = load_config()
if config['performance_mode']:
    strategy = SimplePathfinding()
else:
    strategy = AStarPathfinding()

agent = Agent(pathfinding_strategy=strategy)
```

### Estrutura de Módulos

```python
# backend/main.py - Ponto de entrada

from backend.infrastructure.database import Database
from backend.infrastructure.event_bus import EventBus
from backend.application.simulation_controller import SimulationController
from backend.presentation.web_server import WebServer
from backend.infrastructure.iot.mqtt_client import MQTTClient

def main():
    # 1. Inicializar infraestrutura
    db = Database()
    event_bus = EventBus()
    mqtt_client = MQTTClient()
    
    # 2. Criar controlador principal
    simulation = SimulationController(db, event_bus, mqtt_client)
    
    # 3. Iniciar servidor web
    web_server = WebServer(simulation)
    web_server.start()
    
    # 4. Loop principal
    simulation.run()

if __name__ == "__main__":
    main()
```

---

## 🗄️ GUIA DE BANCO DE DADOS {#database}

### Comparação: SQLite vs PostgreSQL

| Aspecto | SQLite | PostgreSQL |
|---------|--------|------------|
| **Uso Recomendado** | Desenvolvimento, single-user | Produção, multi-user |
| **Setup** | Zero configuração | Requer instalação servidor |
| **Concorrência** | Limitada (file locking) | Excelente (MVCC) |
| **Performance** | Ótima para leitura | Melhor para escrita concorrente |
| **Tamanho Máximo** | ~140 TB (prático: <100GB) | Ilimitado |
| **Backup** | Copiar arquivo .db | Ferramentas nativas (pg_dump) |
| **Custo** | Gratuito, incluso | Gratuito, precisa hosting |

**Recomendação para o Projeto**:
- **Fase 0-3**: SQLite (simplicidade)
- **Fase 4+**: Migrar para PostgreSQL (escalabilidade)

### Schema Completo Normalizado

#### Modelo Entidade-Relacionamento (ER Diagram)

```
┌───────────────┐
│    AGENTS     │
├───────────────┤
│ id (PK)       │◄─────┐
│ name          │      │
│ age           │      │
│ gender        │      │  ┌──────────────────┐
│ home_id (FK)  ├──────┼──┤   BUILDINGS      │
│ job_id (FK)   │      │  ├──────────────────┤
│ workplace_id  │──────┘  │ id (PK)          │
│ salary        │         │ name             │
│ money         │         │ type             │
│ health        │         │ x, y             │
│ energy        │         │ owner_id (FK)    ├───┐
│ happiness     │         │ value            │   │
│ ...           │         │ capacity         │   │
└───────┬───────┘         └──────────────────┘   │
        │                                         │
        │  ┌──────────────────────┐              │
        └──┤  FAMILY_RELATIONS    │              │
           ├──────────────────────┤              │
           │ id (PK)              │              │
           │ agent_id (FK)        │              │
           │ related_agent_id(FK) │              │
           │ relation_type        │              │
           └──────────────────────┘              │
                                                  │
┌──────────────────┐                            │
│    VEHICLES      │                            │
├──────────────────┤                            │
│ id (PK)          │                            │
│ type             │                            │
│ model            │        ┌───────────────┐  │
│ current_pos      │        │    ROUTES     │  │
│ speed            │◄───────┤───────────────┤  │
│ capacity         │        │ id (PK)       │  │
│ cargo            │        │ vehicle_id(FK)│  │
│ condition        │        │ start_st (FK) ├──┘
└──────────────────┘        │ end_st (FK)   │
                            │ departure     │
                            │ arrival       │
                            └───────────────┘

┌──────────────────┐        ┌─────────────────┐
│     EVENTS       │        │  ECONOMY_STATS  │
├──────────────────┤        ├─────────────────┤
│ id (PK)          │        │ id (PK)         │
│ timestamp        │        │ date            │
│ type             │        │ gdp             │
│ description      │        │ unemployment    │
│ impact_happiness │        │ inflation       │
│ impact_economy   │        │ avg_happiness   │
│ related_agent_id │        │ population      │
│ related_bldg_id  │        └─────────────────┘
└──────────────────┘
```

### DDL - Data Definition Language

#### SQLite Schema

```sql
-- =====================================================
-- MAQUETE VIVA - SQLite Database Schema v1.0.0
-- =====================================================

PRAGMA foreign_keys = ON;

-- =====================================================
-- AGENTS TABLE
-- =====================================================
CREATE TABLE agents (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    age INTEGER NOT NULL CHECK(age >= 0 AND age <= 120),
    gender TEXT CHECK(gender IN ('M', 'F', 'NB', 'O')),
    
    -- Location & Work
    home_id INTEGER,
    current_location_id INTEGER,
    job_id INTEGER,
    workplace_id INTEGER,
    
    -- Financial
    salary REAL DEFAULT 0 CHECK(salary >= 0),
    money REAL DEFAULT 0,
    
    -- Physical/Mental Attributes
    health INTEGER DEFAULT 100 CHECK(health >= 0 AND health <= 100),
    energy INTEGER DEFAULT 100 CHECK(energy >= 0 AND energy <= 100),
    happiness INTEGER DEFAULT 70 CHECK(happiness >= 0 AND happiness <= 100),
    hunger INTEGER DEFAULT 0 CHECK(hunger >= 0 AND hunger <= 100),
    
    -- Skills
    knowledge INTEGER DEFAULT 50 CHECK(knowledge >= 0 AND knowledge <= 100),
    strength INTEGER DEFAULT 50 CHECK(strength >= 0 AND strength <= 100),
    attention INTEGER DEFAULT 70 CHECK(attention >= 0 AND attention <= 100),
    
    -- Personality Traits
    laziness INTEGER DEFAULT 20 CHECK(laziness >= 0 AND laziness <= 100),
    ambition INTEGER DEFAULT 50 CHECK(ambition >= 0 AND ambition <= 100),
    
    -- Relationships
    is_married BOOLEAN DEFAULT 0,
    
    -- State
    current_state TEXT DEFAULT 'at_home',
    current_activity TEXT,
    
    -- Metadata
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (home_id) REFERENCES buildings(id) ON DELETE SET NULL,
    FOREIGN KEY (current_location_id) REFERENCES buildings(id) ON DELETE SET NULL,
    FOREIGN KEY (workplace_id) REFERENCES buildings(id) ON DELETE SET NULL
);

-- Indexes para performance
CREATE INDEX idx_agents_location ON agents(current_location_id);
CREATE INDEX idx_agents_workplace ON agents(workplace_id);
CREATE INDEX idx_agents_state ON agents(current_state);

-- =====================================================
-- BUILDINGS TABLE
-- =====================================================
CREATE TABLE buildings (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    type TEXT NOT NULL CHECK(type IN (
        'residential', 'commercial', 'industrial', 
        'public', 'infrastructure', 'recreational'
    )),
    subtype TEXT, -- house, apartment, factory, station, etc
    
    -- Location
    x INTEGER NOT NULL,
    y INTEGER NOT NULL,
    z INTEGER DEFAULT 0, -- Altura (para viadutos, etc)
    
    -- Ownership
    owner_id INTEGER,
    
    -- Construction
    construction_progress INTEGER DEFAULT 100 CHECK(
        construction_progress >= 0 AND construction_progress <= 100
    ),
    construction_start_date TIMESTAMP,
    construction_end_date TIMESTAMP,
    
    -- Condition
    condition_state INTEGER DEFAULT 100 CHECK(
        condition_state >= 0 AND condition_state <= 100
    ),
    last_maintenance TIMESTAMP,
    
    -- Economics
    value REAL DEFAULT 0,
    rent REAL DEFAULT 0,
    
    -- Capacity
    capacity INTEGER DEFAULT 1, -- Quantas pessoas/unidades suporta
    current_occupancy INTEGER DEFAULT 0,
    
    -- Metadata
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (owner_id) REFERENCES agents(id) ON DELETE SET NULL
);

CREATE INDEX idx_buildings_type ON buildings(type);
CREATE INDEX idx_buildings_location ON buildings(x, y);
CREATE INDEX idx_buildings_owner ON buildings(owner_id);

-- =====================================================
-- VEHICLES TABLE
-- =====================================================
CREATE TABLE vehicles (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    type TEXT NOT NULL CHECK(type IN ('train', 'bus', 'car', 'truck')),
    model TEXT NOT NULL,
    
    -- Current State
    current_station_id INTEGER,
    current_route_id INTEGER,
    current_position REAL DEFAULT 0, -- 0-1 (progresso na rota)
    speed REAL DEFAULT 0,
    
    -- Capacity
    passenger_capacity INTEGER DEFAULT 0,
    cargo_capacity REAL DEFAULT 0, -- toneladas
    current_passengers INTEGER DEFAULT 0,
    current_cargo REAL DEFAULT 0,
    cargo_type TEXT,
    
    -- Condition
    condition_state INTEGER DEFAULT 100 CHECK(
        condition_state >= 0 AND condition_state <= 100
    ),
    fuel_level REAL DEFAULT 100 CHECK(
        fuel_level >= 0 AND fuel_level <= 100
    ),
    kilometers_driven REAL DEFAULT 0,
    last_maintenance_km REAL DEFAULT 0,
    
    -- Status
    is_active BOOLEAN DEFAULT 1,
    is_in_maintenance BOOLEAN DEFAULT 0,
    
    -- Metadata
    purchase_date TIMESTAMP,
    purchase_value REAL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (current_station_id) REFERENCES buildings(id) ON DELETE SET NULL,
    FOREIGN KEY (current_route_id) REFERENCES routes(id) ON DELETE SET NULL
);

CREATE INDEX idx_vehicles_type ON vehicles(type);
CREATE INDEX idx_vehicles_active ON vehicles(is_active);
CREATE INDEX idx_vehicles_station ON vehicles(current_station_id);

-- =====================================================
-- ROUTES TABLE
-- =====================================================
CREATE TABLE routes (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    vehicle_type TEXT NOT NULL CHECK(vehicle_type IN ('train', 'bus')),
    
    -- Stations
    start_station_id INTEGER NOT NULL,
    end_station_id INTEGER NOT NULL,
    
    -- Schedule
    departure_time TIME,
    arrival_time TIME,
    frequency_minutes INTEGER DEFAULT 60, -- A cada X minutos
    
    -- Pricing
    passenger_fare REAL DEFAULT 5.0,
    cargo_rate_per_ton REAL DEFAULT 10.0,
    
    -- Status
    is_active BOOLEAN DEFAULT 1,
    
    -- Metadata
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (start_station_id) REFERENCES buildings(id) ON DELETE CASCADE,
    FOREIGN KEY (end_station_id) REFERENCES buildings(id) ON DELETE CASCADE
);

CREATE INDEX idx_routes_stations ON routes(start_station_id, end_station_id);
CREATE INDEX idx_routes_active ON routes(is_active);

-- =====================================================
-- ROUTE_WAYPOINTS TABLE (rota completa, não só início/fim)
-- =====================================================
CREATE TABLE route_waypoints (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    route_id INTEGER NOT NULL,
    sequence INTEGER NOT NULL, -- Ordem (1, 2, 3...)
    station_id INTEGER NOT NULL,
    stop_duration INTEGER DEFAULT 2, -- Minutos de parada
    
    FOREIGN KEY (route_id) REFERENCES routes(id) ON DELETE CASCADE,
    FOREIGN KEY (station_id) REFERENCES buildings(id) ON DELETE CASCADE,
    
    UNIQUE(route_id, sequence)
);

CREATE INDEX idx_waypoints_route ON route_waypoints(route_id);

-- =====================================================
-- EVENTS TABLE
-- =====================================================
CREATE TABLE events (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Event Type
    type TEXT NOT NULL CHECK(type IN (
        'construction', 'accident', 'election', 'disaster', 
        'birth', 'death', 'marriage', 'divorce',
        'job_change', 'business_open', 'business_close',
        'crime', 'celebration', 'protest', 'other'
    )),
    
    -- Description
    title TEXT NOT NULL,
    description TEXT,
    
    -- Impact
    impact_happiness INTEGER DEFAULT 0,
    impact_economy REAL DEFAULT 0,
    severity TEXT CHECK(severity IN ('low', 'medium', 'high', 'critical')),
    
    -- Related Entities
    related_agent_id INTEGER,
    related_building_id INTEGER,
    related_vehicle_id INTEGER,
    
    -- Metadata
    is_public BOOLEAN DEFAULT 1, -- Aparece em notícias?
    is_resolved BOOLEAN DEFAULT 1,
    
    FOREIGN KEY (related_agent_id) REFERENCES agents(id) ON DELETE SET NULL,
    FOREIGN KEY (related_building_id) REFERENCES buildings(id) ON DELETE SET NULL,
    FOREIGN KEY (related_vehicle_id) REFERENCES vehicles(id) ON DELETE SET NULL
);

CREATE INDEX idx_events_timestamp ON events(timestamp);
CREATE INDEX idx_events_type ON events(type);
CREATE INDEX idx_events_public ON events(is_public);

-- =====================================================
-- FAMILY_RELATIONS TABLE
-- =====================================================
CREATE TABLE family_relations (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    agent_id INTEGER NOT NULL,
    related_agent_id INTEGER NOT NULL,
    relation_type TEXT NOT NULL CHECK(relation_type IN (
        'parent', 'child', 'spouse', 'sibling', 
        'grandparent', 'grandchild', 'uncle_aunt', 'nephew_niece'
    )),
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (agent_id) REFERENCES agents(id) ON DELETE CASCADE,
    FOREIGN KEY (related_agent_id) REFERENCES agents(id) ON DELETE CASCADE,
    
    UNIQUE(agent_id, related_agent_id, relation_type)
);

CREATE INDEX idx_family_agent ON family_relations(agent_id);

-- =====================================================
-- ECONOMY_STATS TABLE
-- =====================================================
CREATE TABLE economy_stats (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    date DATE NOT NULL UNIQUE,
    
    -- Macroeconomics
    gdp REAL DEFAULT 0,
    unemployment_rate REAL DEFAULT 0,
    inflation_rate REAL DEFAULT 0,
    
    -- Population
    population INTEGER DEFAULT 0,
    births INTEGER DEFAULT 0,
    deaths INTEGER DEFAULT 0,
    immigrants INTEGER DEFAULT 0,
    emigrants INTEGER DEFAULT 0,
    
    -- Happiness
    average_happiness REAL DEFAULT 70,
    min_happiness REAL DEFAULT 0,
    max_happiness REAL DEFAULT 100,
    
    -- Money Supply
    total_money_supply REAL DEFAULT 0,
    government_balance REAL DEFAULT 0,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_economy_date ON economy_stats(date);

-- =====================================================
-- TRANSACTIONS TABLE (registro de transações econômicas)
-- =====================================================
CREATE TABLE transactions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Transaction Type
    type TEXT NOT NULL CHECK(type IN (
        'salary', 'purchase', 'sale', 'tax', 
        'rent', 'maintenance', 'transport', 'other'
    )),
    
    -- Parties
    from_agent_id INTEGER,
    to_agent_id INTEGER,
    from_building_id INTEGER,
    to_building_id INTEGER,
    
    -- Amount
    amount REAL NOT NULL,
    currency TEXT DEFAULT 'BRL',
    
    -- Description
    description TEXT,
    
    FOREIGN KEY (from_agent_id) REFERENCES agents(id) ON DELETE SET NULL,
    FOREIGN KEY (to_agent_id) REFERENCES agents(id) ON DELETE SET NULL
);

CREATE INDEX idx_transactions_timestamp ON transactions(timestamp);
CREATE INDEX idx_transactions_type ON transactions(type);

-- =====================================================
-- JOBS TABLE
-- =====================================================
CREATE TABLE jobs (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL,
    category TEXT CHECK(category IN (
        'manual', 'clerical', 'professional', 
        'managerial', 'executive', 'creative'
    )),
    
    -- Requirements
    min_education INTEGER DEFAULT 0,
    min_knowledge INTEGER DEFAULT 0,
    min_strength INTEGER DEFAULT 0,
    
    -- Compensation
    base_salary REAL NOT NULL,
    
    -- Employer
    employer_building_id INTEGER,
    
    -- Status
    is_available BOOLEAN DEFAULT 1,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (employer_building_id) REFERENCES buildings(id) ON DELETE CASCADE
);

CREATE INDEX idx_jobs_available ON jobs(is_available);
CREATE INDEX idx_jobs_employer ON jobs(employer_building_id);

-- =====================================================
-- SENSORS TABLE (hardware físico)
-- =====================================================
CREATE TABLE sensors (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    sensor_type TEXT NOT NULL CHECK(sensor_type IN (
        'reed_switch', 'ir_sensor', 'ultrasonic', 
        'temperature', 'light', 'button'
    )),
    
    -- Hardware
    arduino_id TEXT, -- Qual Arduino/ESP32
    pin INTEGER,
    
    -- Location
    location_description TEXT,
    related_building_id INTEGER,
    track_section TEXT,
    
    -- Calibration
    calibration_value REAL,
    last_reading REAL,
    last_reading_time TIMESTAMP,
    
    -- Status
    is_active BOOLEAN DEFAULT 1,
    is_faulty BOOLEAN DEFAULT 0,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (related_building_id) REFERENCES buildings(id) ON DELETE SET NULL
);

CREATE INDEX idx_sensors_active ON sensors(is_active);

-- =====================================================
-- SENSOR_READINGS TABLE (histórico de leituras)
-- =====================================================
CREATE TABLE sensor_readings (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    sensor_id INTEGER NOT NULL,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    value REAL NOT NULL,
    
    FOREIGN KEY (sensor_id) REFERENCES sensors(id) ON DELETE CASCADE
);

CREATE INDEX idx_readings_sensor ON sensor_readings(sensor_id);
CREATE INDEX idx_readings_timestamp ON sensor_readings(timestamp);

-- =====================================================
-- TRIGGERS (lógica automática)
-- =====================================================

-- Atualizar updated_at automaticamente
CREATE TRIGGER update_agents_timestamp 
AFTER UPDATE ON agents
FOR EACH ROW
BEGIN
    UPDATE agents SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
END;

CREATE TRIGGER update_buildings_timestamp 
AFTER UPDATE ON buildings
FOR EACH ROW
BEGIN
    UPDATE buildings SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
END;

CREATE TRIGGER update_vehicles_timestamp 
AFTER UPDATE ON vehicles
FOR EACH ROW
BEGIN
    UPDATE vehicles SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
END;

-- Validar ocupação de prédio
CREATE TRIGGER validate_building_occupancy
BEFORE UPDATE OF current_occupancy ON buildings
FOR EACH ROW
WHEN NEW.current_occupancy > NEW.capacity
BEGIN
    SELECT RAISE(ABORT, 'Occupancy cannot exceed capacity');
END;

-- =====================================================
-- VIEWS (consultas pré-definidas)
-- =====================================================

-- Estatísticas de agentes por emprego
CREATE VIEW agent_employment_stats AS
SELECT 
    j.title AS job_title,
    j.category AS job_category,
    COUNT(a.id) AS num_employees,
    AVG(a.salary) AS avg_salary,
    AVG(a.happiness) AS avg_happiness
FROM agents a
LEFT JOIN jobs j ON a.job_id = j.id
GROUP BY j.id;

-- Ocupação de prédios
CREATE VIEW building_occupancy_stats AS
SELECT 
    type,
    subtype,
    COUNT(*) AS total_buildings,
    SUM(capacity) AS total_capacity,
    SUM(current_occupancy) AS total_occupied,
    ROUND(AVG(CAST(current_occupancy AS REAL) / capacity * 100), 2) AS avg_occupancy_pct
FROM buildings
WHERE capacity > 0
GROUP BY type, subtype;

-- Performance de veículos
CREATE VIEW vehicle_performance AS
SELECT 
    v.id,
    v.type,
    v.model,
    v.condition_state,
    v.kilometers_driven,
    COUNT(DISTINCT r.id) AS routes_assigned,
    AVG(v.current_passengers) AS avg_passengers,
    v.kilometers_driven - v.last_maintenance_km AS km_since_maintenance
FROM vehicles v
LEFT JOIN routes r ON r.id = v.current_route_id
GROUP BY v.id;

-- =====================================================
-- INITIAL DATA (dados iniciais)
-- =====================================================

-- Inserir primeira cidade (edifício especial)
INSERT INTO buildings (name, type, subtype, x, y, capacity) VALUES
('Praça Central', 'public', 'plaza', 50, 50, 100),
('Estação Ferroviária Principal', 'infrastructure', 'train_station', 45, 50, 300),
('Prefeitura', 'public', 'government', 50, 55, 50);

-- Inserir empregos iniciais
INSERT INTO jobs (title, category, base_salary, employer_building_id) VALUES
('Prefeito', 'executive', 8000, 3),
('Maquinista', 'manual', 3500, 2),
('Atendente de Estação', 'clerical', 2000, 2);
```

### PostgreSQL Schema (Diferenças Principais)

```sql
-- =====================================================
-- PostgreSQL Specific Features
-- =====================================================

-- Usar SERIAL ao invés de AUTOINCREMENT
CREATE TABLE agents (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    age SMALLINT NOT NULL CHECK(age >= 0 AND age <= 120),
    -- ... resto igual
);

-- Usar tipos mais específicos
money_column NUMERIC(10, 2), -- Ao invés de REAL
timestamp_column TIMESTAMPTZ, -- Timestamp with timezone

-- Índices parciais (mais eficientes)
CREATE INDEX idx_active_vehicles ON vehicles(id) WHERE is_active = TRUE;

-- Índices GiST para queries espaciais (localização)
CREATE EXTENSION IF NOT EXISTS postgis;

ALTER TABLE buildings ADD COLUMN geom geometry(POINT, 4326);
CREATE INDEX idx_buildings_geom ON buildings USING GIST(geom);

-- Full-text search
ALTER TABLE events ADD COLUMN search_vector tsvector;
CREATE INDEX idx_events_search ON events USING GIN(search_vector);

-- Particionamento (para tabelas gigantes)
CREATE TABLE sensor_readings (
    -- ... columns
) PARTITION BY RANGE (timestamp);

CREATE TABLE sensor_readings_2025_01 PARTITION OF sensor_readings
    FOR VALUES FROM ('2025-01-01') TO ('2025-02-01');
```

---

## 🎮 ENGINE 3D: UNITY VS GODOT {#engine-3d}

### Comparação Detalhada

| Critério | Unity | Godot | Recomendação |
|----------|-------|-------|--------------|
| **Curva de Aprendizado** | Média-Alta | Baixa-Média | Godot: mais fácil |
| **Linguagem** | C# | GDScript (Python-like) | Godot: familiar |
| **Performance 3D** | Excelente | Boa | Unity: melhor |
| **2D Nativo** | Não (adaptado) | Sim | Godot: superior 2D |
| **Tamanho Engine** | ~5GB | ~50MB | Godot: leve |
| **Asset Store** | Gigante | Crescente | Unity: mais assets |
| **Licença

    # Conectar à API Python
    api_client.connect_to_backend("ws://localhost:8765")
    api_client.message_received.connect(_on_backend_message)
    
    # Inicializar cidade
    city_manager.initialize()
    
    # Configurar câmera
    camera.setup_isometric()
    
    print("Maquete Viva iniciada!")

func _process(delta):
    if simulation_running:
        # Atualizar visualização baseado no backend
        city_manager.update_visualization(delta * time_scale)

func _on_backend_message(data: Dictionary):
    """Recebe atualizações do Python backend"""
    match data.type:
        "agent_update":
            city_manager.update_agent(data.agent_id, data.position, data.state)
        "vehicle_update":
            city_manager.update_vehicle(data.vehicle_id, data.position, data.speed)
        "event":
            hud.show_event_notification(data.event_title, data.event_description)
        "stats_update":
            hud.update_stats(data.stats)

func start_simulation():
    simulation_running = true
    api_client.send_command("start_simulation")

func pause_simulation():
    simulation_running = false
    api_client.send_command("pause_simulation")

func set_time_scale(scale: float):
    time_scale = scale
    api_client.send_command("set_time_scale", {"scale": scale})
```

#### 4. Cliente WebSocket (GDScript)

```gdscript
# scripts/api_client.gd
extends Node

signal message_received(data: Dictionary)
signal connected_to_backend
signal connection_failed

var socket = WebSocketPeer.new()
var connected: bool = false

func connect_to_backend(url: String):
    var error = socket.connect_to_url(url)
    if error != OK:
        print("Falha ao conectar: ", error)
        connection_failed.emit()
    else:
        print("Conectando ao backend Python...")

func _process(_delta):
    socket.poll()
    var state = socket.get_ready_state()
    
    if state == WebSocketPeer.STATE_OPEN:
        if not connected:
            connected = true
            connected_to_backend.emit()
            print("Conectado ao backend!")
        
        # Receber mensagens
        while socket.get_available_packet_count():
            var packet = socket.get_packet()
            var json_string = packet.get_string_from_utf8()
            var data = JSON.parse_string(json_string)
            if data:
                message_received.emit(data)
    
    elif state == WebSocketPeer.STATE_CLOSED:
        if connected:
            print("Conexão perdida com backend")
            connected = false

func send_command(command: String, params: Dictionary = {}):
    if not connected:
        print("Não conectado ao backend")
        return
    
    var message = {
        "command": command,
        "params": params,
        "timestamp": Time.get_unix_time_from_system()
    }
    
    var json_string = JSON.stringify(message)
    socket.send_text(json_string)

func request_data(data_type: String):
    send_command("request_data", {"type": data_type})
```

#### 5. Gerenciador de Cidade (GDScript)

```gdscript
# scripts/city_manager.gd
extends Node3D

# Prefabs
@export var agent_prefab: PackedScene
@export var building_prefab: PackedScene
@export var vehicle_prefab: PackedScene

# Containers
@onready var agents_container = $Agents
@onready var buildings_container = $Buildings
@onready var vehicles_container = $Vehicles

# Dicionários de instâncias
var agents: Dictionary = {}
var buildings: Dictionary = {}
var vehicles: Dictionary = {}

func initialize():
    # Carregar estado inicial do backend
    get_parent().get_node("APIClient").request_data("initial_state")

func create_agent(agent_data: Dictionary):
    var agent_instance = agent_prefab.instantiate()
    agent_instance.setup(agent_data)
    agents_container.add_child(agent_instance)
    agents[agent_data.id] = agent_instance

func update_agent(agent_id: int, position: Vector3, state: String):
    if agent_id in agents:
        agents[agent_id].update_position(position)
        agents[agent_id].update_state(state)

func create_building(building_data: Dictionary):
    var building_instance = building_prefab.instantiate()
    building_instance.setup(building_data)
    buildings_container.add_child(building_instance)
    buildings[building_data.id] = building_instance

func create_vehicle(vehicle_data: Dictionary):
    var vehicle_instance = vehicle_prefab.instantiate()
    vehicle_instance.setup(vehicle_data)
    vehicles_container.add_child(vehicle_instance)
    vehicles[vehicle_data.id] = vehicle_instance

func update_vehicle(vehicle_id: int, position: Vector3, speed: float):
    if vehicle_id in vehicles:
        vehicles[vehicle_id].move_to(position, speed)

func update_visualization(delta: float):
    # Atualizar animações suaves
    for agent in agents.values():
        agent.update_animation(delta)
    
    for vehicle in vehicles.values():
        vehicle.update_movement(delta)
```

#### 6. Script de Agente (GDScript)

```gdscript
# scripts/agent.gd
extends Node3D

# Propriedades
var agent_id: int
var agent_name: String
var current_state: String = "idle"

# Movimento
var target_position: Vector3
var move_speed: float = 2.0

# Visualização
@onready var mesh = $MeshInstance3D
@onready var animation_player = $AnimationPlayer
@onready var label = $Label3D

func setup(data: Dictionary):
    agent_id = data.id
    agent_name = data.name
    position = Vector3(data.x, 0, data.y)
    label.text = agent_name

func update_position(new_position: Vector3):
    target_position = new_position

func update_state(new_state: String):
    if current_state != new_state:
        current_state = new_state
        _play_state_animation()

func _play_state_animation():
    match current_state:
        "walking":
            animation_player.play("walk")
        "working":
            animation_player.play("work")
        "sleeping":
            animation_player.play("idle")
        _:
            animation_player.play("idle")

func update_animation(delta: float):
    # Movimento suave até target
    if position.distance_to(target_position) > 0.1:
        var direction = (target_position - position).normalized()
        position += direction * move_speed * delta
        look_at(target_position, Vector3.UP)
```

---

## ⚙️ GITHUB ACTIONS E AUTOMAÇÕES {#actions}

### Estrutura de Workflows

#### 1. Testes Automáticos (.github/workflows/tests.yml)

```yaml
name: Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  test-python:
    runs-on: ubuntu-latest
    
    strategy:
      matrix:
        python-version: [3.9, 3.10, 3.11]
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Set up Python ${{ matrix.python-version }}
      uses: actions/setup-python@v4
      with:
        python-version: ${{ matrix.python-version }}
    
    - name: Cache dependencies
      uses: actions/cache@v3
      with:
        path: ~/.cache/pip
        key: ${{ runner.os }}-pip-${{ hashFiles('**/requirements.txt') }}
    
    - name: Install dependencies
      run: |
        python -m pip install --upgrade pip
        pip install -r requirements.txt
        pip install pytest pytest-cov
    
    - name: Run tests with coverage
      run: |
        pytest --cov=backend --cov-report=xml --cov-report=html
    
    - name: Upload coverage to Codecov
      uses: codecov/codecov-action@v3
      with:
        file: ./coverage.xml
        flags: unittests
        name: codecov-umbrella
    
    - name: Archive coverage report
      uses: actions/upload-artifact@v3
      with:
        name: coverage-report
        path: htmlcov/

  test-hardware:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Validate Arduino sketches
      uses: arduino/compile-sketches@v1
      with:
        fqbn: arduino:avr:uno
        sketch-paths: |
          - hardware/arduino/train_sensor
          - hardware/arduino/switch_control
```

#### 2. Linting e Formatação (.github/workflows/lint.yml)

```yaml
name: Linting

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  lint-python:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Set up Python
      uses: actions/setup-python@v4
      with:
        python-version: '3.11'
    
    - name: Install linters
      run: |
        pip install flake8 black isort mypy
    
    - name: Run flake8
      run: flake8 backend/ --max-line-length=100 --exclude=__pycache__
    
    - name: Check black formatting
      run: black --check backend/
    
    - name: Check import sorting
      run: isort --check-only backend/
    
    - name: Run mypy type checking
      run: mypy backend/ --ignore-missing-imports
  
  lint-docs:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Markdown linting
      uses: DavidAnson/markdownlint-cli2-action@v11
      with:
        globs: '**/*.md'
```

#### 3. Build de Documentação (.github/workflows/docs.yml)

```yaml
name: Documentation

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build-docs:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Set up Python
      uses: actions/setup-python@v4
      with:
        python-version: '3.11'
    
    - name: Install dependencies
      run: |
        pip install mkdocs mkdocs-material
    
    - name: Build documentation
      run: mkdocs build
    
    - name: Deploy to GitHub Pages
      if: github.event_name == 'push' && github.ref == 'refs/heads/main'
      uses: peaceiris/actions-gh-pages@v3
      with:
        github_token: ${{ secrets.GITHUB_TOKEN }}
        publish_dir: ./site
```

#### 4. Release Automático (.github/workflows/release.yml)

```yaml
name: Release

on:
  push:
    tags:
      - 'v*.*.*'

jobs:
  create-release:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
      with:
        fetch-depth: 0
    
    - name: Generate changelog
      id: changelog
      uses: metcalfc/changelog-generator@v4.0.1
      with:
        myToken: ${{ secrets.GITHUB_TOKEN }}
    
    - name: Create Release
      uses: actions/create-release@v1
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
      with:
        tag_name: ${{ github.ref }}
        release_name: Release ${{ github.ref }}
        body: |
          ${{ steps.changelog.outputs.changelog }}
          
          ## Instalação
          
          ```bash
          pip install maquete-viva==${{ github.ref }}
          ```
        draft: false
        prerelease: ${{ contains(github.ref, 'alpha') || contains(github.ref, 'beta') || contains(github.ref, 'rc') }}
    
    - name: Build Python package
      run: |
        pip install build
        python -m build
    
    - name: Publish to PyPI
      if: startsWith(github.ref, 'refs/tags/v') && !contains(github.ref, 'alpha') && !contains(github.ref, 'beta')
      uses: pypa/gh-action-pypi-publish@release/v1
      with:
        password: ${{ secrets.PYPI_API_TOKEN }}
```

#### 5. Atualização Automática de Versão (.github/workflows/version-bump.yml)

```yaml
name: Version Bump

on:
  workflow_dispatch:
    inputs:
      bump_type:
        description: 'Type of version bump'
        required: true
        type: choice
        options:
          - patch
          - minor
          - major

jobs:
  bump-version:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
      with:
        token: ${{ secrets.GITHUB_TOKEN }}
    
    - name: Set up Python
      uses: actions/setup-python@v4
      with:
        python-version: '3.11'
    
    - name: Install bump2version
      run: pip install bump2version
    
    - name: Configure git
      run: |
        git config user.name "GitHub Actions Bot"
        git config user.email "actions@github.com"
    
    - name: Bump version
      run: |
        bump2version ${{ github.event.inputs.bump_type }}
    
    - name: Push changes
      run: |
        git push
        git push --tags
```

---

## 📋 SISTEMA DE ISSUES E PROJECTS {#issues}

### Templates de Issues

#### Bug Report (.github/ISSUE_TEMPLATE/bug_report.md)

```markdown
---
name: Bug Report
about: Reportar um problema
title: '[BUG] '
labels: bug
assignees: ''
---

## 🐛 Descrição do Bug

Descreva claramente o problema encontrado.

## 📝 Passos para Reproduzir

1. Vá para '...'
2. Clique em '...'
3. Execute '...'
4. Veja o erro

## ✅ Comportamento Esperado

O que deveria acontecer?

## ❌ Comportamento Atual

O que está acontecendo?

## 📸 Screenshots

Se aplicável, adicione screenshots.

## 🖥️ Ambiente

- **OS**: [ex: Ubuntu 22.04, Windows 11]
- **Python Version**: [ex: 3.11.2]
- **Versão do Projeto**: [ex: v0.5.0-beta]
- **Hardware**: [Arduino Uno, ESP32, etc]

## 📊 Logs

```
Cole logs relevantes aqui
```

## 🔍 Contexto Adicional

Qualquer outra informação útil.

## 🏷️ Possível Solução

Se tiver ideia de como corrigir, descreva aqui.
```

#### Feature Request (.github/ISSUE_TEMPLATE/feature_request.md)

```markdown
---
name: Feature Request
about: Sugerir nova funcionalidade
title: '[FEATURE] '
labels: enhancement
assignees: ''
---

## 🚀 Descrição da Feature

Descreva claramente a funcionalidade desejada.

## 💡 Motivação

Por que essa feature é útil? Que problema resolve?

## 📐 Proposta de Implementação

Como você imagina que isso funcionaria?

### Interface

Como o usuário interagiria com isso?

### Exemplo de Uso

```python
# Código exemplo se aplicável
agent.new_feature(param=value)
```

## 🎨 Mockups/Designs

Se tiver desenhos ou mockups, adicione aqui.

## 📊 Impacto

- **Complexidade**: [Baixa/Média/Alta]
- **Prioridade**: [Baixa/Média/Alta/Crítica]
- **Sistemas Afetados**: [Economia, Transporte, UI, etc]

## 🔗 Features Relacionadas

Issues ou PRs relacionados: #123, #456

## ✅ Critérios de Aceitação

- [ ] Requisito 1
- [ ] Requisito 2
- [ ] Requisito 3

## 🤔 Alternativas Consideradas

Outras formas de implementar ou resolver o problema.
```

#### Question (.github/ISSUE_TEMPLATE/question.md)

```markdown
---
name: Question
about: Fazer uma pergunta
title: '[QUESTION] '
labels: question
assignees: ''
---

## ❓ Pergunta

Faça sua pergunta de forma clara.

## 🔍 Contexto

Dê contexto sobre o que você está tentando fazer.

## 📚 O que você já tentou

- Procurei na documentação em...
- Tentei fazer X mas...

## 💻 Código Relevante

```python
# Cole código se aplicável
```

## 🎯 Resultado Esperado

O que você espera que aconteça/resposta esperada.
```

### Pull Request Template (.github/PULL_REQUEST_TEMPLATE.md)

```markdown
## 📝 Descrição

Descreva as mudanças deste PR.

## 🎯 Motivação e Contexto

Por que essa mudança é necessária? Que problema resolve?

Closes #(issue)

## 🧪 Tipo de Mudança

- [ ] 🐛 Bug fix (não quebra funcionalidade existente)
- [ ] ✨ Nova feature (não quebra funcionalidade existente)
- [ ] 💥 Breaking change (fix ou feature que causa mudança incompatível)
- [ ] 📝 Documentação
- [ ] 🎨 Refatoração (sem mudança funcional)
- [ ] ⚡ Melhoria de performance
- [ ] ✅ Adição/atualização de testes

## ✅ Checklist

- [ ] Meu código segue o style guide do projeto
- [ ] Revisei meu próprio código
- [ ] Comentei código complexo
- [ ] Atualizei a documentação
- [ ] Minhas mudanças não geram novos warnings
- [ ] Adicionei testes que provam que o fix/feature funciona
- [ ] Testes unitários novos e existentes passam localmente
- [ ] Mudanças dependentes foram mergeadas e publicadas

## 🧪 Como Testar

Passos para testar as mudanças:

1. Passo 1
2. Passo 2
3. Passo 3

## 📸 Screenshots (se aplicável)

Antes | Depois
------|-------
![antes](url) | ![depois](url)

## 📊 Performance (se aplicável)

Antes | Depois
------|-------
10s | 5s

## 🔗 Issues Relacionadas

- Closes #123
- Related to #456
```

### GitHub Projects - Estrutura Sugerida

#### Project: Maquete Viva Roadmap

**Colunas**:
1. **📋 Backlog** - Todas as issues não iniciadas
2. **🎯 Próxima Sprint** - Para trabalhar em breve (2-4 semanas)
3. **🏗️ In Progress** - Sendo trabalhadas agora
4. **👀 Review** - Aguardando code review
5. **🧪 Testing** - Em testes
6. **✅ Done** - Completas

**Labels Sugeridas**:
- **Tipo**: `bug`, `enhancement`, `documentation`, `question`
- **Prioridade**: `P0-critical`, `P1-high`, `P2-medium`, `P3-low`
- **Sistema**: `simulation`, `hardware`, `ui`, `database`, `3d-engine`
- **Status**: `needs-triage`, `wontfix`, `duplicate`, `help-wanted`, `good-first-issue`
- **Fase**: `phase-0`, `phase-1`, `phase-2`, `phase-3`, `phase-4`

---

## 📦 RELEASES E CHANGELOG {#releases}

### Estrutura de CHANGELOG.md

```markdown
# Changelog

Todas as mudanças notáveis deste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Funcionalidades adicionadas mas ainda não lançadas

### Changed
- Mudanças em funcionalidades existentes

### Deprecated
- Funcionalidades que serão removidas em breve

### Removed
- Funcionalidades removidas

### Fixed
- Bugs corrigidos

### Security
- Correções de vulnerabilidades

## [1.0.0] - 2026-06-01

### Added
- 🎉 Lançamento oficial da versão 1.0
- Sistema completo de simulação com 500+ agentes
- Dashboard web responsivo
- Integração hardware com Arduino/ESP32
- Sistema econômico dinâmico
- Sistema político com eleições
- 15 tipos de edifícios
- 3 tipos de veículos (trem, ônibus, carro)
- AR básico para visualização

### Changed
- Performance melhorada em 200% com spatial hashing
- UI redesenhada com base em feedback beta

### Fixed
- Corrigido bug crítico onde agentes não comiam (#234)
- Corrigido vazamento de memória em loop principal (#245)
- Sensores agora detectam 99.9% dos trens (#256)

## [0.8.0-beta] - 2026-02-10

### Added
- Sistema político implementado
- Eleições a cada 4 anos (tempo simulado)
- AR básico funcionando (ARCore/ARKit)
- 200 agentes simultâneos

### Changed
- Migrado de Pygame para Godot 4.x
- Refatoração completa da arquitetura (Clean Architecture)

### Fixed
- Diversos bugs de pathfinding
- Sincronização física-digital melhorada

## [0.5.0-beta] - 2025-09-15

### Added
- Maquete física 1m² completa
- 100 agentes funcionando
- Sistema de iluminação LED
- 10 prédios físicos construídos
- Dashboard web funcional

### Changed
- Economia rebalanceada (inflação mais realista)

### Fixed
- Bug onde trens desapareciam ao trocar de rota (#156)

## [0.3.0-alpha] - 2025-06-05

### Added
- Primeiros 5 prédios físicos
- Base MDF instalada
- Trilhos HO funcionando
- Primeiro trem físico sincronizado com simulação

## [0.1.0-alpha] - 2025-03-10

### Added
- Primeira versão funcional da simulação
- 10 agentes com rotinas básicas
- Economia simples (oferta/demanda)
- Integração Arduino + Python
- Sensor reed switch detectando trem

## [0.0.1-prealpha] - 2025-01-15

### Added
- Primeiro commit
- Estrutura básica do projeto
- README inicial
- Classe Agent esboçada

[Unreleased]: https://github.com/usuario/maquete-viva/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/usuario/maquete-viva/compare/v0.8.0-beta...v1.0.0
[0.8.0-beta]: https://github.com/usuario/maquete-viva/compare/v0.5.0-beta...v0.8.0-beta
[0.5.0-beta]: https://github.com/usuario/maquete-viva/compare/v0.3.0-alpha...v0.5.0-beta
[0.3.0-alpha]: https://github.com/usuario/maquete-viva/compare/v0.1.0-alpha...v0.3.0-alpha
[0.1.0-alpha]: https://github.com/usuario/maquete-viva/compare/v0.0.1-prealpha...v0.1.0-alpha
[0.0.1-prealpha]: https://github.com/usuario/maquete-viva/releases/tag/v0.0.1-prealpha
```

### Script de Release Automático

```python
# scripts/release.py
"""
Script para criar release automaticamente
Uso: python scripts/release.py patch|minor|major
"""

import sys
import subprocess
from datetime import date

def get_current_version():
    """Lê versão atual do arquivo VERSION"""
    with open('VERSION', 'r') as f:
        return f.read().strip()

def bump_version(current: str, bump_type: str) -> str:
    """Incrementa versão"""
    major, minor, patch = map(int, current.lstrip('v').split('-')[0].split('.'))
    
    if bump_type == 'major':
        return f"v{major + 1}.0.0"
    elif bump_type == 'minor':
        return f"v{major}.{minor + 1}.0"
    elif bump_type == 'patch':
        return f"v{major}.{minor}.{patch + 1}"
    else:
        raise ValueError(f"Bump type inválido: {bump_type}")

def update_changelog(version: str):
    """Atualiza CHANGELOG.md"""
    today = date.today().isoformat()
    
    with open('CHANGELOG.md', 'r') as f:
        content = f.read()
    
    # Substituir [Unreleased] por nova versão
    content = content.replace(
        '## [Unreleased]',
        f'## [Unreleased]\n\n## [{version}] - {today}'
    )
    
    with open('CHANGELOG.md', 'w') as f:
        f.write(content)

def create_git_tag(version: str):
    """Cria tag no git"""
    subprocess.run(['git', 'add', '.'])
    subprocess.run(['git', 'commit', '-m', f'chore: release {version}'])
    subprocess.run(['git', 'tag', '-a', version, '-m', f'Release {version}'])
    subprocess.run(['git', 'push'])
    subprocess.run(['git', 'push', '--tags'])

def main():
    if len(sys.argv) != 2:
        print("Uso: python scripts/release.py [patch|minor|major]")
        sys.exit(1)
    
    bump_type = sys.argv[1]
    current_version = get_current_version()
    new_version = bump_version(current_version, bump_type)
    
    print(f"Versão atual: {current_version}")
    print(f"Nova versão: {new_version}")
    
    confirm = input("Confirmar release? (y/n): ")
    if confirm.lower() != 'y':
        print("Release cancelada")
        sys.exit(0)
    
    # Atualizar arquivos
    with open('VERSION', 'w') as f:
        f.write(new_version)
    
    update_changelog(new_version)
    create_git_tag(new_version)
    
    print(f"✅ Release {new_version} criada com sucesso!")

if __name__ == '__main__':
    main()
```

---

## 👥 GUIAS DE CONTRIBUIÇÃO {#contributing}

### CONTRIBUTING.md

```markdown
# Guia de Contribuição

Obrigado por considerar contribuir para Maquete Viva! 🚂

## Código de Conduta

Este projeto adere ao [Código de Conduta](CODE_OF_CONDUCT.md). Ao participar, você concorda em seguir suas diretrizes.

## Como Posso Contribuir?

### 🐛 Reportando Bugs

1. Verifique se o bug já não foi reportado em [Issues](https://github.com/usuario/maquete-viva/issues)
2. Use o template de bug report
3. Seja claro e específico
4. Inclua steps para reproduzir
5. Adicione screenshots se possível

### ✨ Sugerindo Features

1. Verifique se a feature já não foi sugerida
2. Use o template de feature request
3. Explique claramente o problema que resolve
4. Descreva a solução proposta

### 💻 Pull Requests

#### Setup Ambiente Desenvolvimento

```bash
# Clone o repositório
git clone https://github.com/usuario/maquete-viva.git
cd maquete-viva

# Crie virtual environment
python -m venv venv
source venv/bin/activate  # Linux/Mac
venv\Scripts\activate     # Windows

# Instale dependências
pip install -r requirements.txt
pip install -r requirements-dev.txt

# Configure pre-commit hooks
pre-commit install
```

#### Fluxo de Trabalho

1. **Fork** o repositório
2. **Clone** seu fork
3. **Crie branch** da `develop`:
   ```bash
   git checkout -b feature/minha-feature develop
   ```
4. **Faça commits** claros:
   ```bash
   git commit -m "feat: adiciona sistema de clima"
   ```
5. **Push** para seu fork
6. **Abra Pull Request** para `develop`

#### Convenção de Commits

Seguimos [Conventional Commits](https://www.conventionalcommits.org/):

```
<tipo>(<escopo>): <descrição>

[corpo opcional]

[rodapé opcional]
```

**Tipos**:
- `feat`: Nova funcionalidade
- `fix`: Correção de bug
- `docs`: Documentação
- `style`: Formatação (não muda lógica)
- `refactor`: Refatoração
- `test`: Testes
- `chore`: Tarefas de manutenção

**Exemplos**:
```
feat(economy): adiciona sistema de impostos
fix(agent): corrige bug de pathfinding
docs(readme): atualiza instruções de instalação
```

#### Padrões de Código

**Python**:
- Seguir [PEP 8](https://pep8.org/)
- Use type hints
- Docstrings em formato Google
- Max line length: 100 caracteres

```python
def calculate_tax(income: float, rate: float = 0.15) -> float:
    """
    Calcula imposto baseado em renda.
    
    Args:# Maquete Viva - Documentação Técnica Completa
## Sistema de Documentação, Versionamento e Arquitetura

---

## 📚 ÍNDICE DA DOCUMENTAÇÃO TÉCNICA

1. [Sistema de Versionamento Semântico](#versionamento)
2. [Estrutura de Documentação Git](#git-docs)
3. [Manifesto de Design](#manifesto)
4. [Arquitetura de Software Detalhada](#arquitetura)
5. [Guia de Banco de Dados](#database)
6. [Engine 3D: Unity vs Godot](#engine-3d)
7. [GitHub Actions e Automações](#actions)
8. [Sistema de Issues e Projects](#issues)
9. [Releases e Changelog](#releases)
10. [Guias de Contribuição](#contributing)

---

## 🔢 SISTEMA DE VERSIONAMENTO SEMÂNTICO {#versionamento}

### Convenção Semantic Versioning (SemVer)

**Formato**: `MAJOR.MINOR.PATCH-STAGE.BUILD`

**Exemplo**: `v1.2.3-alpha.5`

### Estrutura de Versão

```
v MAJOR . MINOR . PATCH - STAGE . BUILD
│   │      │       │       │       │
│   │      │       │       │       └─ Build number (opcional)
│   │      │       │       └────────── pre-release stage
│   │      │       └────────────────── Bug fixes
│   │      └────────────────────────── New features (backward compatible)
│   └───────────────────────────────── Breaking changes
└───────────────────────────────────── Version prefix
```

### Regras de Incremento

#### MAJOR (v1.0.0 → v2.0.0)
**Quando usar**:
- Mudanças que quebram compatibilidade
- Refatoração completa de sistema
- Remoção de features importantes

**Exemplos**:
- Migração de Pygame para Unity/Godot
- Mudança de SQLite para PostgreSQL (sem migração automática)
- Redesign completo da API

#### MINOR (v1.0.0 → v1.1.0)
**Quando usar**:
- Novas features que não quebram código existente
- Adição de novos sistemas
- Expansões de funcionalidade

**Exemplos**:
- Adicionar sistema político
- Implementar AR (Realidade Aumentada)
- Novo tipo de veículo (ônibus)

#### PATCH (v1.0.0 → v1.0.1)
**Quando usar**:
- Correções de bugs
- Melhorias de performance
- Ajustes de balanceamento
- Correções de documentação

**Exemplos**:
- Corrigir sensor que não detecta trem
- Otimizar loop de agentes (2x mais rápido)
- Consertar bug onde agentes não comem

### Estágios de Desenvolvimento

#### Pre-Alpha (`v0.0.1-prealpha`)
**Características**:
- Protótipos iniciais
- Features incompletas
- Código experimental
- Nada é estável

**Duração Esperada**: Meses 1-3 (Fase 0-1 do GDD)

#### Alpha (`v0.1.0-alpha`)
**Características**:
- Features principais implementadas
- Muitos bugs esperados
- API pode mudar drasticamente
- Apenas para desenvolvedores

**Critérios**:
- ✅ Simulação básica funciona (50+ agentes)
- ✅ Economia simples operacional
- ✅ Arduino + Python comunicando
- ⚠️ Interface rudimentar

**Duração Esperada**: Meses 4-8 (Fase 2 do GDD)

#### Beta (`v0.5.0-beta`)
**Características**:
- Todas features principais completas
- Poucos bugs críticos
- API relativamente estável
- Testadores externos podem usar

**Critérios**:
- ✅ Maquete física funcional (1m²)
- ✅ 100+ agentes estáveis
- ✅ Dashboard web funcionando
- ✅ Sistema de sensores integrado
- ⚠️ Falta polimento

**Duração Esperada**: Meses 9-18 (Fase 3-4 do GDD)

#### Release Candidate (`v1.0.0-rc.1`)
**Características**:
- Potencialmente pronto para produção
- Testes finais
- Apenas bugs menores
- Documentação quase completa

**Critérios**:
- ✅ Sem bugs críticos conhecidos
- ✅ Performance aceitável
- ✅ Documentação 90% completa
- ✅ Testado por pelo menos 3 pessoas

**Duração Esperada**: Mês 19-20

#### Stable Release (`v1.0.0`)
**Características**:
- Pronto para uso público
- Testado extensivamente
- Documentação completa
- Suporte garantido

**Critérios**:
- ✅ Todos critérios de RC atendidos
- ✅ Pelo menos 1 mês sem bugs críticos
- ✅ README, tutoriais e guias completos

### Exemplos de Histórico de Versões

```
v0.0.1-prealpha    - 2025-01-15 - Primeiro commit, estrutura básica
v0.0.5-prealpha    - 2025-02-01 - Classe Agente implementada
v0.1.0-alpha       - 2025-03-10 - Simulação com 10 agentes funcionando
v0.2.0-alpha       - 2025-04-20 - Economia básica + Arduino integrado
v0.3.0-alpha       - 2025-06-05 - Maquete física iniciada
v0.5.0-beta        - 2025-09-15 - Maquete 1m² completa + 100 agentes
v0.6.0-beta        - 2025-11-20 - Dashboard web funcional
v0.8.0-beta        - 2026-02-10 - Sistema político + AR básico
v1.0.0-rc.1        - 2026-04-15 - Release Candidate 1
v1.0.0-rc.2        - 2026-05-01 - RC2 (correções finais)
v1.0.0             - 2026-06-01 - 🎉 LANÇAMENTO OFICIAL!
v1.1.0             - 2026-08-15 - Nova feature: Aeroporto
v1.1.1             - 2026-08-22 - Hotfix: Bug no pathfinding
v1.2.0             - 2026-10-30 - Expansão: Sistema educacional
v2.0.0             - 2027-03-20 - Migração para Unity 3D
```

---

## 📁 ESTRUTURA DE DOCUMENTAÇÃO GIT {#git-docs}

### Árvore de Documentação Completa

```
maquete_viva/
│
├── README.md                          # Visão geral, quickstart
├── CHANGELOG.md                       # Histórico de versões
├── CONTRIBUTING.md                    # Como contribuir
├── CODE_OF_CONDUCT.md                 # Código de conduta
├── LICENSE                            # Licença (MIT, GPL, etc)
│
├── docs/
│   ├── index.md                       # Índice da documentação
│   │
│   ├── getting-started/
│   │   ├── README.md                  # Introdução
│   │   ├── installation.md            # Guia de instalação
│   │   ├── quickstart.md              # Primeiro uso
│   │   ├── hardware-setup.md          # Configurar Arduino/sensores
│   │   └── troubleshooting.md         # Problemas comuns
│   │
│   ├── architecture/
│   │   ├── README.md                  # Visão geral arquitetura
│   │   ├── design-manifesto.md        # Princípios de design
│   │   ├── software-architecture.md   # Diagrams + patterns
│   │   ├── database-schema.md         # Estrutura BD completa
│   │   ├── api-reference.md           # Endpoints REST/WebSocket
│   │   └── data-flow.md               # Fluxo de dados
│   │
│   ├── simulation/
│   │   ├── agents.md                  # Sistema de agentes
│   │   ├── economy.md                 # Modelo econômico
│   │   ├── transport.md               # Logística de transporte
│   │   ├── politics.md                # Sistema político
│   │   └── events.md                  # Eventos e narrativa
│   │
│   ├── hardware/
│   │   ├── electronics-basics.md      # Eletrônica para iniciantes
│   │   ├── arduino-guide.md           # Programação Arduino
│   │   ├── esp32-guide.md             # IoT com ESP32
│   │   ├── sensors.md                 # Tipos de sensores
│   │   ├── actuators.md               # Servos, motores, LEDs
│   │   ├── dcc-control.md             # Sistema DCC para trens
│   │   └── circuits/                  # Diagramas Fritzing
│   │       ├── led-circuit.png
│   │       ├── sensor-circuit.png
│   │       └── switch-circuit.png
│   │
│   ├── physical-build/
│   │   ├── materials.md               # Lista de materiais
│   │   ├── tools.md                   # Ferramentas necessárias
│   │   ├── base-construction.md       # Construir base
│   │   ├── terrain.md                 # Relevo e topografia
│   │   ├── buildings.md               # Construir prédios
│   │   ├── tracks.md                  # Instalar trilhos
│   │   ├── weathering.md              # Técnicas de envelhecimento
│   │   └── lighting.md                # Sistema de iluminação
│   │
│   ├── software-guide/
│   │   ├── python-setup.md            # Ambiente Python
│   │   ├── running-simulation.md      # Executar simulação
│   │   ├── configuration.md           # Arquivo config.yaml
│   │   ├── database-setup.md          # Configurar SQLite/PostgreSQL
│   │   ├── mqtt-setup.md              # Broker MQTT
│   │   └── web-dashboard.md           # Dashboard web
│   │
│   ├── 3d-visualization/
│   │   ├── engine-comparison.md       # Unity vs Godot vs Blender
│   │   ├── unity-setup.md             # Projeto Unity
│   │   ├── godot-setup.md             # Projeto Godot
│   │   ├── blender-export.md          # Exportar modelos Blender
│   │   ├── ar-implementation.md       # AR com ARKit/ARCore
│   │   └── assets/                    # Modelos 3D, texturas
│   │
│   ├── tutorials/
│   │   ├── first-agent.md             # Criar primeiro agente
│   │   ├── first-building.md          # Construir primeiro prédio
│   │   ├── first-train.md             # Trem funcionando
│   │   ├── add-sensor.md              # Adicionar sensor
│   │   ├── custom-event.md            # Criar evento customizado
│   │   └── ai-training.md             # Treinar modelo ML
│   │
│   ├── api/
│   │   ├── rest-api.md                # Documentação REST
│   │   ├── websocket-api.md           # Documentação WebSocket
│   │   ├── mqtt-topics.md             # Tópicos MQTT
│   │   └── examples/                  # Exemplos de uso
│   │       ├── get-agents.py
│   │       ├── control-train.py
│   │       └── subscribe-events.js
│   │
│   ├── development/
│   │   ├── setup-dev-environment.md   # Ambiente desenvolvimento
│   │   ├── coding-standards.md        # Padrões de código
│   │   ├── git-workflow.md            # Fluxo Git (branching)
│   │   ├── testing.md                 # Testes unitários
│   │   ├── ci-cd.md                   # GitHub Actions
│   │   └── debugging.md               # Técnicas de debug
│   │
│   ├── deployment/
│   │   ├── requirements.md            # Requisitos sistema
│   │   ├── installation.md            # Instalação produção
│   │   ├── docker.md                  # Containerização
│   │   ├── raspberry-pi.md            # Deploy em RaspberryPi
│   │   └── backup.md                  # Backup do banco de dados
│   │
│   ├── faq.md                         # Perguntas frequentes
│   ├── glossary.md                    # Glossário técnico
│   ├── resources.md                   # Links úteis
│   └── roadmap.md                     # Roadmap futuro
│
├── .github/
│   ├── ISSUE_TEMPLATE/
│   │   ├── bug_report.md              # Template bug
│   │   ├── feature_request.md         # Template feature
│   │   └── question.md                # Template pergunta
│   │
│   ├── PULL_REQUEST_TEMPLATE.md       # Template PR
│   │
│   ├── workflows/
│   │   ├── tests.yml                  # Testes automáticos
│   │   ├── lint.yml                   # Linting código
│   │   ├── docs.yml                   # Build documentação
│   │   ├── release.yml                # Criar release
│   │   └── version-bump.yml           # Atualizar versão
│   │
│   └── dependabot.yml                 # Atualizações automáticas
│
└── examples/
    ├── minimal-simulation.py          # Exemplo mínimo
    ├── custom-agent.py                # Agente customizado
    ├── arduino-basic/                 # Sketch Arduino básico
    └── web-client/                    # Cliente web exemplo
```

---

## 🎨 MANIFESTO DE DESIGN {#manifesto}

### Princípios Fundamentais

#### 1. **Simplicidade Progressiva**
> "Fácil de começar, infinito para dominar"

**Na Prática**:
- Interface básica para iniciantes (botões grandes, tutorial guiado)
- Menus avançados escondidos, mas acessíveis
- Shortcuts para usuários experientes
- Tooltips informativos em todos os elementos

**Exemplo**:
```
Iniciante vê:  [▶️ Iniciar Simulação] [⏸️ Pausar] [⚙️ Configurações]
Avançado vê:   +100 atalhos de teclado, console de comandos, scripts Lua
```

#### 2. **Físico e Digital São Um Só**
> "A maquete física e a simulação são faces da mesma moeda"

**Na Prática**:
- Sincronização em tempo real (latência <100ms)
- Mudanças físicas refletem no digital (sensor detecta → simulação atualiza)
- Mudanças digitais podem afetar físico (simulação decide → servo move desvio)
- AR como ponte visual entre mundos

#### 3. **Falha é Feature**
> "Acidentes, bugs e imperfeições criam narrativas"

**Na Prática**:
- Bugs viram histórias (agente bugado vira lenda local)
- Falhas de hardware são eventos narrativos (trem descarrilha → crise)
- Sistema de "memória" registra tudo (historiadores futuros leem logs)
- Modo "realista" vs "sandbox" (erros são opcionais)

#### 4. **Modularidade Radical**
> "Tudo pode ser desligado, trocado ou expandido"

**Na Prática**:
- Sistemas independentes (economia funciona sem transporte)
- Plugins/mods fáceis de criar
- Configuração extensiva (YAML, JSON)
- Arquitetura baseada em eventos (desacoplamento)

#### 5. **Acessibilidade Sem Concessões**
> "Complexidade profunda com entrada gentil"

**Na Prática**:
- Tutoriais interativos (não só texto)
- Simulador sem hardware (modo "virtual" puro)
- Documentação em níveis (iniciante → expert)
- Código bem comentado, arquitetura clara

#### 6. **Performance é Respeitabilidade**
> "Cada frame importa, cada segundo do usuário é sagrado"

**Na Prática**:
- Simulação roda em 60 FPS mesmo com 500 agentes
- Otimização constante (profiling regular)
- Loading screens informativos (nunca silêncio)
- Degradação graciosa (se lento, avisa e sugere opções)

#### 7. **Estética é Funcional**
> "Beleza serve à clareza"

**Na Prática**:
- Cores indicam estado (vermelho = problema, verde = ok)
- Animações têm propósito (não apenas "bonito")
- UI segue hierarquia visual clara
- Consistência em todo sistema

#### 8. **Dados São Tesouros**
> "Cada número conta uma história"

**Na Prática**:
- Logs detalhados de tudo
- Estatísticas exportáveis (CSV, JSON)
- Visualizações de dados (gráficos, heatmaps)
- Histórico completo (time-travel debugging possível)

#### 9. **Comunidade é Core**
> "O projeto cresce com quem usa"

**Na Prática**:
- GitHub Discussions ativo
- Aceitar PRs com guidelines claras
- Créditos visíveis para contribuidores
- Roadmap influenciado por comunidade

#### 10. **Diversão Acima de Tudo**
> "Se não for divertido, não vale a pena"

**Na Prática**:
- Easter eggs escondidos
- Eventos absurdos ocasionais (invasão de patos?)
- Humor sutil em logs/erros
- Celebração de conquistas (achievements com animações)

### Anti-Padrões a Evitar

❌ **Feature Creep Descontrolado**
- ✅ Fazer: Planejar releases com escopo fechado
- ❌ Evitar: "Só mais uma funcionalidade..."

❌ **Over-Engineering Prematuro**
- ✅ Fazer: Código simples que funciona
- ❌ Evitar: "Vou criar um sistema ultra-flexível que..."

❌ **Documentação Como Afterthought**
- ✅ Fazer: Documentar enquanto desenvolve
- ❌ Evitar: "Documento depois que funcionar"

❌ **UI Confusa por "Profissionalismo"**
- ✅ Fazer: Botões claros, labels óbvios
- ❌ Evitar: Ícones obscuros sem tooltip

❌ **Otimização Prematura**
- ✅ Fazer: Fazer funcionar, depois otimizar
- ❌ Evitar: "Vou usar esse algoritmo complexo caso..."

---

## 🏗️ ARQUITETURA DE SOFTWARE DETALHADA {#arquitetura}

### Visão Geral: Clean Architecture Adaptada

```
┌─────────────────────────────────────────────────────────┐
│                   PRESENTATION LAYER                    │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ Web         │  │ Mobile       │  │ AR           │  │
│  │ Dashboard   │  │ App          │  │ Viewer       │  │
│  └──────┬──────┘  └──────┬───────┘  └──────┬───────┘  │
└─────────┼─────────────────┼──────────────────┼──────────┘
          │                 │                  │
          └────────────┬────┴──────────────────┘
                       │
        ┌──────────────▼──────────────────┐
        │      APPLICATION LAYER          │
        │  ┌──────────────────────────┐  │
        │  │  Controllers             │  │
        │  │  - CityController        │  │
        │  │  - AgentController       │  │
        │  │  - TransportController   │  │
        │  └────────┬─────────────────┘  │
        └───────────┼─────────────────────┘
                    │
        ┌───────────▼──────────────────┐
        │      DOMAIN LAYER            │
        │  ┌───────────────────────┐  │
        │  │  Business Logic       │  │
        │  │  - AgentBehavior      │  │
        │  │  - EconomySimulator   │  │
        │  │  - EventEngine        │  │
        │  │  - PathfindingService │  │
        │  └───────┬───────────────┘  │
        │          │                   │
        │  ┌───────▼───────────────┐  │
        │  │  Domain Models        │  │
        │  │  - Agent              │  │
        │  │  - Building           │  │
        │  │  - Vehicle            │  │
        │  │  - City               │  │
        │  └───────┬───────────────┘  │
        └──────────┼───────────────────┘
                   │
        ┌──────────▼─────────────────┐
        │   INFRASTRUCTURE LAYER     │
        │  ┌──────────────────────┐  │
        │  │  Data Access         │  │
        │  │  - Repository        │  │
        │  │  - ORM (SQLAlchemy)  │  │
        │  └──────────────────────┘  │
        │  ┌──────────────────────┐  │
        │  │  External Services   │  │
        │  │  - MQTT Client       │  │
        │  │  - Serial Comm       │  │
        │  │  - File Storage      │  │
        │  └──────────────────────┘  │
        └────────────────────────────┘
                   │
        ┌──────────▼──────────────┐
        │   DATABASE & HARDWARE   │
        │  ┌──────────────────┐   │
        │  │ SQLite/Postgres  │   │
        │  └──────────────────┘   │
        │  ┌──────────────────┐   │
        │  │ Arduino/ESP32    │   │
        │  └──────────────────┘   │
        └─────────────────────────┘
```

### Padrões de Design Utilizados

#### 1. **Repository Pattern**
**Propósito**: Abstrai acesso ao banco de dados

```python
# backend/infrastructure/repositories/agent_repository.py

from typing import List, Optional
from backend.domain.models.agent import Agent

class AgentRepository:
    """
    Responsável por toda interação com BD relacionada a Agentes
    """
    
    def __init__(self, db_session):
        self.session = db_session
    
    def get_by_id(self, agent_id: int) -> Optional[Agent]:
        """Busca agente por ID"""
        pass
    
    def get_all(self) -> List[Agent]:
        """Retorna todos agentes"""
        pass
    
    def get_by_location(self, location_id: int) -> List[Agent]:
        """Agentes em determinada localização"""
        pass
    
    def save(self, agent: Agent) -> Agent:
        """Salva ou atualiza agente"""
        pass
    
    def delete(self, agent_id: int) -> bool:
        """Remove agente"""
        pass
    
    def count(self) -> int:
        """Conta total de agentes"""
        pass
```

#### 2. **Service Layer Pattern**
**Propósito**: Orquestra lógica de negócio complexa

```python
# backend/application/services/transport_service.py

class TransportService:
    """
    Orquestra sistema de transporte
    """
    
    def __init__(self, vehicle_repo, route_repo, event_bus):
        self.vehicle_repo = vehicle_repo
        self.route_repo = route_repo
        self.event_bus = event_bus
    
    def schedule_train(self, train_id: int, route_id: int):
        """
        Agenda trem em rota
        - Valida disponibilidade
        - Calcula horários
        - Reserva recurso
        - Emite eventos
        """
        train = self.vehicle_repo.get_by_id(train_id)
        route = self.route_repo.get_by_id(route_id)
        
        if not self._is_available(train, route):
            raise ConflictException("Trem indisponível")
        
        schedule = self._calculate_schedule(train, route)
        train.assign_route(route, schedule)
        
        self.vehicle_repo.save(train)
        self.event_bus.emit("train_scheduled", {
            "train_id": train_id,
            "route_id": route_id,
            "departure": schedule.departure
        })
        
        return schedule
```

#### 3. **Observer Pattern (Event Bus)**
**Propósito**: Comunicação desacoplada entre sistemas

```python
# backend/infrastructure/event_bus.py

class EventBus:
    """Sistema de eventos global"""
    
    _instance = None
    
    def __new__(cls):
        if cls._instance is None:
            cls._instance = super().__new__(cls)
            cls._instance._listeners = {}
        return cls._instance
    
    def subscribe(self, event_type: str, handler: Callable):
        if event_type not in self._listeners:
            self._listeners[event_type] = []
        self._listeners[event_type].append(handler)
    
    def emit(self, event_type: str, data: dict):
        if event_type in self._listeners:
            for handler in self._listeners[event_type]:
                handler(data)

# Uso em diferentes partes do sistema:

# Sistema de transporte emite:
event_bus.emit("train_arrived", {"train_id": 1, "station_id": 5})

# Sistema de economia escuta:
def on_train_arrival(data):
    calculate_passenger_revenue(data["train_id"])
    
event_bus.subscribe("train_arrived", on_train_arrival)

# Sistema de UI escuta:
def on_train_arrival_ui(data):
    show_notification(f"Trem {data['train_id']} chegou!")
    
event_bus.subscribe("train_arrived", on_train_arrival_ui)
```

#### 4. **Factory Pattern**
**Propósito**: Criação complexa de objetos

```python
# backend/domain/factories/agent_factory.py

class AgentFactory:
    """
    Cria agentes com atributos procedurais realistas
    """
    
    @staticmethod
    def create_random_citizen(city_context: City) -> Agent:
        """
        Gera cidadão com atributos aleatórios mas coerentes
        """
        age = random.randint(18, 70)
        name = NameGenerator.generate(city_context.culture)
        
        # Atributos correlacionados com idade
        if age < 30:
            energy = random.randint(70, 100)
            health = random.randint(80, 100)
        else:
            energy = random.randint(50, 80)
            health = random.randint(60, 90)
        
        # Profissão baseada em educação e idade
        education = random.randint(0, 100)
        if education > 80 and age > 25:
            job = JobFactory.create_professional_job()
        else:
            job = JobFactory.create_manual_job()
        
        return Agent(
            name=name,
            age=age,
            education=education,
            job=job,
            health=health,
            energy=energy,
            # ... mais atributos
        )
```

#### 5. **Strategy Pattern**
**Propósito**: Comportamentos intercambiáveis

```python
# backend/domain/strategies/pathfinding.py

from abc import ABC, abstractmethod

class PathfindingStrategy(ABC):
    @abstractmethod
    def find_path(self, start, end, graph):
        pass

class AStarPathfinding(PathfindingStrategy):
    def find_path(self, start, end, graph):
        # Implementação A*
        pass

class DijkstraPathfinding(PathfindingStrategy):
    def find_path(self, start, end, graph):
        # Implementação Dijkstra
        pass

class SimplePathfinding(PathfindingStrategy):
    def find_path(self, start, end, graph):
        # Caminho mais simples (menos computação)
        pass

# Uso:
class Agent:
    def __init__(self, pathfinding_strategy: PathfindingStrategy):
        self.pathfinding = pathfinding_strategy
    
    def move_to(self, destination):
        path = self.pathfinding.find_path(self.location, destination, world.graph)
        self.follow_path(path)

# Configurável:
config = load_config()
if config['performance_mode']:
    strategy = SimplePathfinding()
else:
    strategy = AStarPathfinding()

agent = Agent(pathfinding_strategy=strategy)
```

### Estrutura de Módulos

```python
# backend/main.py - Ponto de entrada

from backend.infrastructure.database import Database
from backend.infrastructure.event_bus import EventBus
from backend.application.simulation_controller import SimulationController
from backend.presentation.web_server import WebServer
from backend.infrastructure.iot.mqtt_client import MQTTClient

def main():
    # 1. Inicializar infraestrutura
    db = Database()
    event_bus = EventBus()
    mqtt_client = MQTTClient()
    
    # 2. Criar controlador principal
    simulation = SimulationController(db, event_bus, mqtt_client)
    
    # 3. Iniciar servidor web
    web_server = WebServer(simulation)
    web_server.start()
    
    # 4. Loop principal
    simulation.run()

if __name__ == "__main__":
    main()
```

---

## 🗄️ GUIA DE BANCO DE DADOS {#database}

### Comparação: SQLite vs PostgreSQL

| Aspecto | SQLite | PostgreSQL |
|---------|--------|------------|
| **Uso Recomendado** | Desenvolvimento, single-user | Produção, multi-user |
| **Setup** | Zero configuração | Requer instalação servidor |
| **Concorrência** | Limitada (file locking) | Excelente (MVCC) |
| **Performance** | Ótima para leitura | Melhor para escrita concorrente |
| **Tamanho Máximo** | ~140 TB (prático: <100GB) | Ilimitado |
| **Backup** | Copiar arquivo .db | Ferramentas nativas (pg_dump) |
| **Custo** | Gratuito, incluso | Gratuito, precisa hosting |

**Recomendação para o Projeto**:
- **Fase 0-3**: SQLite (simplicidade)
- **Fase 4+**: Migrar para PostgreSQL (escalabilidade)

### Schema Completo Normalizado

#### Modelo Entidade-Relacionamento (ER Diagram)

```
┌───────────────┐
│    AGENTS     │
├───────────────┤
│ id (PK)       │◄─────┐
│ name          │      │
│ age           │      │
│ gender        │      │  ┌──────────────────┐
│ home_id (FK)  ├──────┼──┤   BUILDINGS      │
│ job_id (FK)   │      │  ├──────────────────┤
│ workplace_id  │──────┘  │ id (PK)          │
│ salary        │         │ name             │
│ money         │         │ type             │
│ health        │         │ x, y             │
│ energy        │         │ owner_id (FK)    ├───┐
│ happiness     │         │ value            │   │
│ ...           │         │ capacity         │   │
└───────┬───────┘         └──────────────────┘   │
        │                                         │
        │  ┌──────────────────────┐              │
        └──┤  FAMILY_RELATIONS    │              │
           ├──────────────────────┤              │
           │ id (PK)              │              │
           │ agent_id (FK)        │              │
           │ related_agent_id(FK) │              │
           │ relation_type        │              │
           └──────────────────────┘              │
                                                  │
┌──────────────────┐                            │
│    VEHICLES      │                            │
├──────────────────┤                            │
│ id (PK)          │                            │
│ type             │                            │
│ model            │        ┌───────────────┐  │
│ current_pos      │        │    ROUTES     │  │
│ speed            │◄───────┤───────────────┤  │
│ capacity         │        │ id (PK)       │  │
│ cargo            │        │ vehicle_id(FK)│  │
│ condition        │        │ start_st (FK) ├──┘
└──────────────────┘        │ end_st (FK)   │
                            │ departure     │
                            │ arrival       │
                            └───────────────┘

┌──────────────────┐        ┌─────────────────┐
│     EVENTS       │        │  ECONOMY_STATS  │
├──────────────────┤        ├─────────────────┤
│ id (PK)          │        │ id (PK)         │
│ timestamp        │        │ date            │
│ type             │        │ gdp             │
│ description      │        │ unemployment    │
│ impact_happiness │        │ inflation       │
│ impact_economy   │        │ avg_happiness   │
│ related_agent_id │        │ population      │
│ related_bldg_id  │        └─────────────────┘
└──────────────────┘
```

### DDL - Data Definition Language

#### SQLite Schema

```sql
-- =====================================================
-- MAQUETE VIVA - SQLite Database Schema v1.0.0
-- =====================================================

PRAGMA foreign_keys = ON;

-- =====================================================
-- AGENTS TABLE
-- =====================================================
CREATE TABLE agents (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    age INTEGER NOT NULL CHECK(age >= 0 AND age <= 120),
    gender TEXT CHECK(gender IN ('M', 'F', 'NB', 'O')),
    
    -- Location & Work
    home_id INTEGER,
    current_location_id INTEGER,
    job_id INTEGER,
    workplace_id INTEGER,
    
    -- Financial
    salary REAL DEFAULT 0 CHECK(salary >= 0),
    money REAL DEFAULT 0,
    
    -- Physical/Mental Attributes
    health INTEGER DEFAULT 100 CHECK(health >= 0 AND health <= 100),
    energy INTEGER DEFAULT 100 CHECK(energy >= 0 AND energy <= 100),
    happiness INTEGER DEFAULT 70 CHECK(happiness >= 0 AND happiness <= 100),
    hunger INTEGER DEFAULT 0 CHECK(hunger >= 0 AND hunger <= 100),
    
    -- Skills
    knowledge INTEGER DEFAULT 50 CHECK(knowledge >= 0 AND knowledge <= 100),
    strength INTEGER DEFAULT 50 CHECK(strength >= 0 AND strength <= 100),
    attention INTEGER DEFAULT 70 CHECK(attention >= 0 AND attention <= 100),
    
    -- Personality Traits
    laziness INTEGER DEFAULT 20 CHECK(laziness >= 0 AND laziness <= 100),
    ambition INTEGER DEFAULT 50 CHECK(ambition >= 0 AND ambition <= 100),
    
    -- Relationships
    is_married BOOLEAN DEFAULT 0,
    
    -- State
    current_state TEXT DEFAULT 'at_home',
    current_activity TEXT,
    
    -- Metadata
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (home_id) REFERENCES buildings(id) ON DELETE SET NULL,
    FOREIGN KEY (current_location_id) REFERENCES buildings(id) ON DELETE SET NULL,
    FOREIGN KEY (workplace_id) REFERENCES buildings(id) ON DELETE SET NULL
);

-- Indexes para performance
CREATE INDEX idx_agents_location ON agents(current_location_id);
CREATE INDEX idx_agents_workplace ON agents(workplace_id);
CREATE INDEX idx_agents_state ON agents(current_state);

-- =====================================================
-- BUILDINGS TABLE
-- =====================================================
CREATE TABLE buildings (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    type TEXT NOT NULL CHECK(type IN (
        'residential', 'commercial', 'industrial', 
        'public', 'infrastructure', 'recreational'
    )),
    subtype TEXT, -- house, apartment, factory, station, etc
    
    -- Location
    x INTEGER NOT NULL,
    y INTEGER NOT NULL,
    z INTEGER DEFAULT 0, -- Altura (para viadutos, etc)
    
    -- Ownership
    owner_id INTEGER,
    
    -- Construction
    construction_progress INTEGER DEFAULT 100 CHECK(
        construction_progress >= 0 AND construction_progress <= 100
    ),
    construction_start_date TIMESTAMP,
    construction_end_date TIMESTAMP,
    
    -- Condition
    condition_state INTEGER DEFAULT 100 CHECK(
        condition_state >= 0 AND condition_state <= 100
    ),
    last_maintenance TIMESTAMP,
    
    -- Economics
    value REAL DEFAULT 0,
    rent REAL DEFAULT 0,
    
    -- Capacity
    capacity INTEGER DEFAULT 1, -- Quantas pessoas/unidades suporta
    current_occupancy INTEGER DEFAULT 0,
    
    -- Metadata
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (owner_id) REFERENCES agents(id) ON DELETE SET NULL
);

CREATE INDEX idx_buildings_type ON buildings(type);
CREATE INDEX idx_buildings_location ON buildings(x, y);
CREATE INDEX idx_buildings_owner ON buildings(owner_id);

-- =====================================================
-- VEHICLES TABLE
-- =====================================================
CREATE TABLE vehicles (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    type TEXT NOT NULL CHECK(type IN ('train', 'bus', 'car', 'truck')),
    model TEXT NOT NULL,
    
    -- Current State
    current_station_id INTEGER,
    current_route_id INTEGER,
    current_position REAL DEFAULT 0, -- 0-1 (progresso na rota)
    speed REAL DEFAULT 0,
    
    -- Capacity
    passenger_capacity INTEGER DEFAULT 0,
    cargo_capacity REAL DEFAULT 0, -- toneladas
    current_passengers INTEGER DEFAULT 0,
    current_cargo REAL DEFAULT 0,
    cargo_type TEXT,
    
    -- Condition
    condition_state INTEGER DEFAULT 100 CHECK(
        condition_state >= 0 AND condition_state <= 100
    ),
    fuel_level REAL DEFAULT 100 CHECK(
        fuel_level >= 0 AND fuel_level <= 100
    ),
    kilometers_driven REAL DEFAULT 0,
    last_maintenance_km REAL DEFAULT 0,
    
    -- Status
    is_active BOOLEAN DEFAULT 1,
    is_in_maintenance BOOLEAN DEFAULT 0,
    
    -- Metadata
    purchase_date TIMESTAMP,
    purchase_value REAL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (current_station_id) REFERENCES buildings(id) ON DELETE SET NULL,
    FOREIGN KEY (current_route_id) REFERENCES routes(id) ON DELETE SET NULL
);

CREATE INDEX idx_vehicles_type ON vehicles(type);
CREATE INDEX idx_vehicles_active ON vehicles(is_active);
CREATE INDEX idx_vehicles_station ON vehicles(current_station_id);

-- =====================================================
-- ROUTES TABLE
-- =====================================================
CREATE TABLE routes (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    vehicle_type TEXT NOT NULL CHECK(vehicle_type IN ('train', 'bus')),
    
    -- Stations
    start_station_id INTEGER NOT NULL,
    end_station_id INTEGER NOT NULL,
    
    -- Schedule
    departure_time TIME,
    arrival_time TIME,
    frequency_minutes INTEGER DEFAULT 60, -- A cada X minutos
    
    -- Pricing
    passenger_fare REAL DEFAULT 5.0,
    cargo_rate_per_ton REAL DEFAULT 10.0,
    
    -- Status
    is_active BOOLEAN DEFAULT 1,
    
    -- Metadata
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (start_station_id) REFERENCES buildings(id) ON DELETE CASCADE,
    FOREIGN KEY (end_station_id) REFERENCES buildings(id) ON DELETE CASCADE
);

CREATE INDEX idx_routes_stations ON routes(start_station_id, end_station_id);
CREATE INDEX idx_routes_active ON routes(is_active);

-- =====================================================
-- ROUTE_WAYPOINTS TABLE (rota completa, não só início/fim)
-- =====================================================
CREATE TABLE route_waypoints (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    route_id INTEGER NOT NULL,
    sequence INTEGER NOT NULL, -- Ordem (1, 2, 3...)
    station_id INTEGER NOT NULL,
    stop_duration INTEGER DEFAULT 2, -- Minutos de parada
    
    FOREIGN KEY (route_id) REFERENCES routes(id) ON DELETE CASCADE,
    FOREIGN KEY (station_id) REFERENCES buildings(id) ON DELETE CASCADE,
    
    UNIQUE(route_id, sequence)
);

CREATE INDEX idx_waypoints_route ON route_waypoints(route_id);

-- =====================================================
-- EVENTS TABLE
-- =====================================================
CREATE TABLE events (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Event Type
    type TEXT NOT NULL CHECK(type IN (
        'construction', 'accident', 'election', 'disaster', 
        'birth', 'death', 'marriage', 'divorce',
        'job_change', 'business_open', 'business_close',
        'crime', 'celebration', 'protest', 'other'
    )),
    
    -- Description
    title TEXT NOT NULL,
    description TEXT,
    
    -- Impact
    impact_happiness INTEGER DEFAULT 0,
    impact_economy REAL DEFAULT 0,
    severity TEXT CHECK(severity IN ('low', 'medium', 'high', 'critical')),
    
    -- Related Entities
    related_agent_id INTEGER,
    related_building_id INTEGER,
    related_vehicle_id INTEGER,
    
    -- Metadata
    is_public BOOLEAN DEFAULT 1, -- Aparece em notícias?
    is_resolved BOOLEAN DEFAULT 1,
    
    FOREIGN KEY (related_agent_id) REFERENCES agents(id) ON DELETE SET NULL,
    FOREIGN KEY (related_building_id) REFERENCES buildings(id) ON DELETE SET NULL,
    FOREIGN KEY (related_vehicle_id) REFERENCES vehicles(id) ON DELETE SET NULL
);

CREATE INDEX idx_events_timestamp ON events(timestamp);
CREATE INDEX idx_events_type ON events(type);
CREATE INDEX idx_events_public ON events(is_public);

-- =====================================================
-- FAMILY_RELATIONS TABLE
-- =====================================================
CREATE TABLE family_relations (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    agent_id INTEGER NOT NULL,
    related_agent_id INTEGER NOT NULL,
    relation_type TEXT NOT NULL CHECK(relation_type IN (
        'parent', 'child', 'spouse', 'sibling', 
        'grandparent', 'grandchild', 'uncle_aunt', 'nephew_niece'
    )),
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (agent_id) REFERENCES agents(id) ON DELETE CASCADE,
    FOREIGN KEY (related_agent_id) REFERENCES agents(id) ON DELETE CASCADE,
    
    UNIQUE(agent_id, related_agent_id, relation_type)
);

CREATE INDEX idx_family_agent ON family_relations(agent_id);

-- =====================================================
-- ECONOMY_STATS TABLE
-- =====================================================
CREATE TABLE economy_stats (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    date DATE NOT NULL UNIQUE,
    
    -- Macroeconomics
    gdp REAL DEFAULT 0,
    unemployment_rate REAL DEFAULT 0,
    inflation_rate REAL DEFAULT 0,
    
    -- Population
    population INTEGER DEFAULT 0,
    births INTEGER DEFAULT 0,
    deaths INTEGER DEFAULT 0,
    immigrants INTEGER DEFAULT 0,
    emigrants INTEGER DEFAULT 0,
    
    -- Happiness
    average_happiness REAL DEFAULT 70,
    min_happiness REAL DEFAULT 0,
    max_happiness REAL DEFAULT 100,
    
    -- Money Supply
    total_money_supply REAL DEFAULT 0,
    government_balance REAL DEFAULT 0,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_economy_date ON economy_stats(date);

-- =====================================================
-- TRANSACTIONS TABLE (registro de transações econômicas)
-- =====================================================
CREATE TABLE transactions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    -- Transaction Type
    type TEXT NOT NULL CHECK(type IN (
        'salary', 'purchase', 'sale', 'tax', 
        'rent', 'maintenance', 'transport', 'other'
    )),
    
    -- Parties
    from_agent_id INTEGER,
    to_agent_id INTEGER,
    from_building_id INTEGER,
    to_building_id INTEGER,
    
    -- Amount
    amount REAL NOT NULL,
    currency TEXT DEFAULT 'BRL',
    
    -- Description
    description TEXT,
    
    FOREIGN KEY (from_agent_id) REFERENCES agents(id) ON DELETE SET NULL,
    FOREIGN KEY (to_agent_id) REFERENCES agents(id) ON DELETE SET NULL
);

CREATE INDEX idx_transactions_timestamp ON transactions(timestamp);
CREATE INDEX idx_transactions_type ON transactions(type);

-- =====================================================
-- JOBS TABLE
-- =====================================================
CREATE TABLE jobs (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL,
    category TEXT CHECK(category IN (
        'manual', 'clerical', 'professional', 
        'managerial', 'executive', 'creative'
    )),
    
    -- Requirements
    min_education INTEGER DEFAULT 0,
    min_knowledge INTEGER DEFAULT 0,
    min_strength INTEGER DEFAULT 0,
    
    -- Compensation
    base_salary REAL NOT NULL,
    
    -- Employer
    employer_building_id INTEGER,
    
    -- Status
    is_available BOOLEAN DEFAULT 1,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (employer_building_id) REFERENCES buildings(id) ON DELETE CASCADE
);

CREATE INDEX idx_jobs_available ON jobs(is_available);
CREATE INDEX idx_jobs_employer ON jobs(employer_building_id);

-- =====================================================
-- SENSORS TABLE (hardware físico)
-- =====================================================
CREATE TABLE sensors (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    sensor_type TEXT NOT NULL CHECK(sensor_type IN (
        'reed_switch', 'ir_sensor', 'ultrasonic', 
        'temperature', 'light', 'button'
    )),
    
    -- Hardware
    arduino_id TEXT, -- Qual Arduino/ESP32
    pin INTEGER,
    
    -- Location
    location_description TEXT,
    related_building_id INTEGER,
    track_section TEXT,
    
    -- Calibration
    calibration_value REAL,
    last_reading REAL,
    last_reading_time TIMESTAMP,
    
    -- Status
    is_active BOOLEAN DEFAULT 1,
    is_faulty BOOLEAN DEFAULT 0,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (related_building_id) REFERENCES buildings(id) ON DELETE SET NULL
);

CREATE INDEX idx_sensors_active ON sensors(is_active);

-- =====================================================
-- SENSOR_READINGS TABLE (histórico de leituras)
-- =====================================================
CREATE TABLE sensor_readings (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    sensor_id INTEGER NOT NULL,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    value REAL NOT NULL,
    
    FOREIGN KEY (sensor_id) REFERENCES sensors(id) ON DELETE CASCADE
);

CREATE INDEX idx_readings_sensor ON sensor_readings(sensor_id);
CREATE INDEX idx_readings_timestamp ON sensor_readings(timestamp);

-- =====================================================
-- TRIGGERS (lógica automática)
-- =====================================================

-- Atualizar updated_at automaticamente
CREATE TRIGGER update_agents_timestamp 
AFTER UPDATE ON agents
FOR EACH ROW
BEGIN
    UPDATE agents SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
END;

CREATE TRIGGER update_buildings_timestamp 
AFTER UPDATE ON buildings
FOR EACH ROW
BEGIN
    UPDATE buildings SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
END;

CREATE TRIGGER update_vehicles_timestamp 
AFTER UPDATE ON vehicles
FOR EACH ROW
BEGIN
    UPDATE vehicles SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
END;

-- Validar ocupação de prédio
CREATE TRIGGER validate_building_occupancy
BEFORE UPDATE OF current_occupancy ON buildings
FOR EACH ROW
WHEN NEW.current_occupancy > NEW.capacity
BEGIN
    SELECT RAISE(ABORT, 'Occupancy cannot exceed capacity');
END;

-- =====================================================
-- VIEWS (consultas pré-definidas)
-- =====================================================

-- Estatísticas de agentes por emprego
CREATE VIEW agent_employment_stats AS
SELECT 
    j.title AS job_title,
    j.category AS job_category,
    COUNT(a.id) AS num_employees,
    AVG(a.salary) AS avg_salary,
    AVG(a.happiness) AS avg_happiness
FROM agents a
LEFT JOIN jobs j ON a.job_id = j.id
GROUP BY j.id;

-- Ocupação de prédios
CREATE VIEW building_occupancy_stats AS
SELECT 
    type,
    subtype,
    COUNT(*) AS total_buildings,
    SUM(capacity) AS total_capacity,
    SUM(current_occupancy) AS total_occupied,
    ROUND(AVG(CAST(current_occupancy AS REAL) / capacity * 100), 2) AS avg_occupancy_pct
FROM buildings
WHERE capacity > 0
GROUP BY type, subtype;

-- Performance de veículos
CREATE VIEW vehicle_performance AS
SELECT 
    v.id,
    v.type,
    v.model,
    v.condition_state,
    v.kilometers_driven,
    COUNT(DISTINCT r.id) AS routes_assigned,
    AVG(v.current_passengers) AS avg_passengers,
    v.kilometers_driven - v.last_maintenance_km AS km_since_maintenance
FROM vehicles v
LEFT JOIN routes r ON r.id = v.current_route_id
GROUP BY v.id;

-- =====================================================
-- INITIAL DATA (dados iniciais)
-- =====================================================

-- Inserir primeira cidade (edifício especial)
INSERT INTO buildings (name, type, subtype, x, y, capacity) VALUES
('Praça Central', 'public', 'plaza', 50, 50, 100),
('Estação Ferroviária Principal', 'infrastructure', 'train_station', 45, 50, 300),
('Prefeitura', 'public', 'government', 50, 55, 50);

-- Inserir empregos iniciais
INSERT INTO jobs (title, category, base_salary, employer_building_id) VALUES
('Prefeito', 'executive', 8000, 3),
('Maquinista', 'manual', 3500, 2),
('Atendente de Estação', 'clerical', 2000, 2);
```

### PostgreSQL Schema (Diferenças Principais)

```sql
-- =====================================================
-- PostgreSQL Specific Features
-- =====================================================

-- Usar SERIAL ao invés de AUTOINCREMENT
CREATE TABLE agents (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    age SMALLINT NOT NULL CHECK(age >= 0 AND age <= 120),
    -- ... resto igual
);

-- Usar tipos mais específicos
money_column NUMERIC(10, 2), -- Ao invés de REAL
timestamp_column TIMESTAMPTZ, -- Timestamp with timezone

-- Índices parciais (mais eficientes)
CREATE INDEX idx_active_vehicles ON vehicles(id) WHERE is_active = TRUE;

-- Índices GiST para queries espaciais (localização)
CREATE EXTENSION IF NOT EXISTS postgis;

ALTER TABLE buildings ADD COLUMN geom geometry(POINT, 4326);
CREATE INDEX idx_buildings_geom ON buildings USING GIST(geom);

-- Full-text search
ALTER TABLE events ADD COLUMN search_vector tsvector;
CREATE INDEX idx_events_search ON events USING GIN(search_vector);

-- Particionamento (para tabelas gigantes)
CREATE TABLE sensor_readings (
    -- ... columns
) PARTITION BY RANGE (timestamp);

CREATE TABLE sensor_readings_2025_01 PARTITION OF sensor_readings
    FOR VALUES FROM ('2025-01-01') TO ('2025-02-01');
```

---

## 🎮 ENGINE 3D: UNITY VS GODOT {#engine-3d}

### Comparação Detalhada

| Critério | Unity | Godot | Recomendação |
|----------|-------|-------|--------------|
| **Curva de Aprendizado** | Média-Alta | Baixa-Média | Godot: mais fácil |
| **Linguagem** | C# | GDScript (Python-like) | Godot: familiar |
| **Performance 3D** | Excelente | Boa | Unity: melhor |
| **2D Nativo** | Não (adaptado) | Sim | Godot: superior 2D |
| **Tamanho Engine** | ~5GB | ~50MB | Godot: leve |
| **Asset Store** | Gigante | Crescente | Unity: mais assets |
| **Licença** | Gratuita até $100k/ano | MIT (100% grátis) | Godot: open source |
| **Export** | PC, Mobile, Web | PC, Mobile, Web | Empate |
| **AR Support** | ARFoundation (nativo) | Requer plugins | Unity: melhor AR |
| **Scripting Visual** | Bolt (pago) | VisualScript (grátis) | Godot |
| **Community** | Enorme | Crescente rápido | Unity maior |
| **Documentação** | Excelente | Boa | Unity mais completa |

### Recomendação para Maquete Viva

**🏆 GODOT 4.x - Escolha Principal**

**Razões**:
1. **Python-like GDScript**: Você já conhece Python, transição suave
2. **Open Source**: MIT License, sem preocupações comerciais
3. **Leveza**: 50MB vs 5GB, perfeito para projeto pessoal
4. **2D/Isométrico Nativo**: Se começar 2D, Godot é superior
5. **Integração Python**: Pode chamar Python scripts diretamente
6. **Blender Friendly**: Import de .blend direto

**Quando usar Unity**:
- Se AR for prioridade absoluta (ARFoundation é excelente)
- Se precisar de assets da Asset Store específicos
- Se performance 3D ultra-realista for crítica

### Arquitetura: Godot + Python Backend

```
┌─────────────────────────────────────────┐
│         GODOT ENGINE (Frontend)         │
│  ┌───────────────────────────────────┐  │
│  │   3D Viewport (Cidade)            │  │
│  │   - Renderização isométrica       │  │
│  │   - Agentes como Node3D           │  │
│  │   - Buildings como MeshInstance   │  │
│  └───────────────────────────────────┘  │
│  ┌───────────────────────────────────┐  │
│  │   UI Layer (CanvasLayer)          │  │
│  │   - Stats panel                   │  │
│  │   - Control buttons               │  │
│  │   - Event notifications           │  │
│  └───────────────────────────────────┘  │
└──────────────┬──────────────────────────┘
               │ WebSocket/HTTP
               │
┌──────────────▼──────────────────────────┐
│      PYTHON BACKEND (Simulação)         │
│  - Lógica de agentes                    │
│  - Economia                             │
│  - Eventos                              │
│  - Banco de dados                       │
└─────────────────────────────────────────┘
```

### Setup Godot 4.x

#### 1. Instalar Godot

```bash
# Linux
wget https://downloads.tuxfamily.org/godotengine/4.2/Godot_v4.2-stable_linux.x86_64.zip
unzip Godot_v4.2-stable_linux.x86_64.zip
chmod +x Godot_v4.2-stable_linux.x86_64
./Godot_v4.2-stable_linux.x86_64

# Windows: Download .exe de godotengine.org
# Mac: Download .dmg
```

#### 2. Estrutura do Projeto Godot

```
maquete_viva_godot/
│
├── project.godot               # Arquivo de configuração
│
├── scenes/
│   ├── main.tscn              # Cena principal
│   ├── city/
│   │   ├── city.tscn          # Cena da cidade
│   │   ├── agent.tscn         # Prefab de agente
│   │   ├── building.tscn      # Prefab de prédio
│   │   └── vehicle.tscn       # Prefab de veículo
│   │
│   └── ui/
│       ├── hud.tscn           # HUD principal
│       ├── stats_panel.tscn   # Painel de estatísticas
│       └── event_popup.tscn   # Popup de eventos
│
├── scripts/
│   ├── main.gd                # Script principal
│   ├── city_manager.gd        # Gerencia cidade
│   ├── agent.gd               # Comportamento agente
│   ├── building.gd            # Lógica de prédio
│   ├── vehicle.gd             # Lógica de veículo
│   ├── api_client.gd          # Comunicação com Python
│   └── camera_controller.gd   # Controle de câmera
│
├── assets/
│   ├── models/
│   │   ├── buildings/
│   │   ├── vehicles/
│   │   └── agents/
│   │
│   ├── textures/
│   │   ├── terrain/
│   │   ├── buildings/
│   │   └── ui/
│   │
│   ├── audio/
│   │   ├── music/
│   │   └── sfx/
│   │
│   └── fonts/
│
└── addons/                    # Plugins de terceiros
    └── websocket_client/
```

#### 3. Script Principal (GDScript)

```gdscript
# scripts/main.gd
extends Node3D

# Referências
@onready var city_manager = $CityManager
@onready var api_client = $APIClient
@onready var camera = $Camera3D
@onready var hud = $CanvasLayer/HUD

# Estado
var simulation_running: bool = false
var time_scale: float = 1.0

func _ready():
    # Conectar à API Python
    api_client.connect_to_backend("ws://localhost:8765")
    api_client.message_received.connect(_on_backend_message)
    
# Iniciar simulação
```