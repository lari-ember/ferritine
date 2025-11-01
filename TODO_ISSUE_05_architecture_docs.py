# TODO: Issue #5 - Criar documentação técnica de arquitetura
# Labels: docs, phase-0: fundamentals, priority: medium, complexity: beginner
# Milestone: Milestone 0: Fundamentos e Infraestrutura

"""
Documentar a arquitetura do sistema detalhadamente.

Tarefas:
- [ ] Criar docs/architecture.md
- [ ] Documentar diagrama de componentes
- [ ] Explicar cada camada (apresentação, lógica, dados, hardware)
- [ ] Documentar fluxo de dados
- [ ] Adicionar diagramas (usar Mermaid ou PlantUML)
- [ ] Documentar padrões de design utilizados

Critérios de Aceitação:
- Documento architecture.md completo
- Diagramas claros e informativos
- Cada camada bem explicada
- Padrões de design documentados
"""

# TODO: Criar arquivo docs/architecture.md com o seguinte conteúdo:

ARCHITECTURE_DOC = """
# 🏗️ Arquitetura do Sistema Ferritine

## 📋 Visão Geral

O Ferritine é um sistema híbrido físico-digital que combina:
- **Simulação computacional** de uma cidade com agentes inteligentes
- **Maquete física** com componentes eletrônicos controlados
- **Interface web** para visualização e controle
- **Sistema IoT** para comunicação com hardware

---

## 🎯 Arquitetura em Camadas

```
┌─────────────────────────────────────────────────────────────┐
│                    CAMADA DE APRESENTAÇÃO                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Web UI     │  │ Visualização │  │   Dashboard  │      │
│  │  (React)     │  │  2D/3D       │  │   Estatísticas│      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            ↓ HTTP/WebSocket
┌─────────────────────────────────────────────────────────────┐
│                      CAMADA DE API                           │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   REST API   │  │  WebSocket   │  │    MQTT      │      │
│  │   (FastAPI)  │  │   Server     │  │    Broker    │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    CAMADA DE LÓGICA                          │
│  ┌──────────────────────────────────────────────────┐       │
│  │           Motor de Simulação                      │       │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐         │       │
│  │  │ Agentes  │ │  Mundo   │ │ Economia │         │       │
│  │  └──────────┘ └──────────┘ └──────────┘         │       │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐         │       │
│  │  │Transporte│ │ Política │ │Construção│         │       │
│  │  └──────────┘ └──────────┘ └──────────┘         │       │
│  └──────────────────────────────────────────────────┘       │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    CAMADA DE DADOS                           │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   SQLite     │  │    Logs      │  │ Configuração │      │
│  │  (SQLAlchemy)│  │  (Arquivos)  │  │    (YAML)    │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            ↓ Serial/MQTT
┌─────────────────────────────────────────────────────────────┐
│                    CAMADA DE HARDWARE                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Arduino    │  │    ESP32     │  │   Sensores   │      │
│  │  (Controle)  │  │   (WiFi)     │  │   Atuadores  │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│                                                              │
│  ┌────────────────────────────────────────────────┐        │
│  │           Maquete Física                        │        │
│  │  LEDs, Servos, Sensores, Trilhos, Trens        │        │
│  └────────────────────────────────────────────────┘        │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔧 Componentes Principais

### 1. Backend (`backend/`)

#### 1.1 Simulação (`backend/simulation/`)
- **`world.py`**: Representa o mundo/cidade (grid 2D, edifícios, terreno)
- **`agent.py`**: Agentes inteligentes com IA e rotinas
- **`economy.py`**: Sistema econômico (salários, preços, comércio)
- **`transport.py`**: Sistema de transporte (trens, ônibus, carros)
- **`politics.py`**: Sistema político (eleições, leis)
- **`construction.py`**: Sistema de construção de edifícios

#### 1.2 API (`backend/api/`)
- **`rest.py`**: API REST com FastAPI
- **`websocket.py`**: WebSocket para updates em tempo real
- **`mqtt_bridge.py`**: Bridge entre simulação e MQTT

#### 1.3 Banco de Dados (`backend/database/`)
- **`models.py`**: Modelos SQLAlchemy
- **`queries.py`**: Queries comuns e otimizadas
- **`migrations/`**: Migrations do Alembic

#### 1.4 Utilitários (`backend/utils/`)
- **`logger.py`**: Sistema de logging centralizado
- **`config_loader.py`**: Carregamento de configurações YAML
- **`helpers.py`**: Funções auxiliares

---

### 2. Frontend (`frontend/`)

#### 2.1 Web UI (`frontend/web/`)
- **React** para interface moderna
- **Dashboard** com estatísticas em tempo real
- **Controles** para simulação
- **WebSocket** para updates em tempo real

---

### 3. Visualização (`visualization/`)

#### 3.1 2D (`visualization/pygame_view.py`)
- Pygame para visualização 2D
- Grid com edifícios, agentes, veículos
- Informações de hover
- Controles de câmera

#### 3.2 3D (Futuro) (`visualization/threejs_view/`)
- Three.js para visualização 3D
- Modelos 3D dos edifícios
- Iluminação dinâmica (dia/noite)

---

### 4. Hardware (`hardware/`)

#### 4.1 Arduino (`hardware/arduino/`)
- **`led_controller/`**: Controle de LEDs (dia/noite)
- **`servo_controller/`**: Controle de servos (desvios de trilho)
- **`sensor_reader/`**: Leitura de sensores (reed switch)

#### 4.2 ESP32 (`hardware/esp32/`)
- **`wifi_bridge/`**: Bridge WiFi/MQTT
- **`ota_updates/`**: Updates over-the-air

---

## 🔄 Fluxo de Dados

### Fluxo 1: Simulação → Visualização
```
[Motor de Simulação] 
    ↓ step()
[Atualiza Estado dos Agentes/Mundo]
    ↓ snapshot()
[Serializa Estado]
    ↓ WebSocket
[Frontend/Visualização]
    ↓ render()
[Exibe na Tela]
```

### Fluxo 2: Simulação → Hardware
```
[Motor de Simulação]
    ↓ evento (trem em posição X)
[Sistema de Eventos]
    ↓ publish MQTT
[MQTT Broker]
    ↓ subscribe
[Arduino/ESP32]
    ↓ comando
[Atualiza Hardware (LED, Servo)]
```

### Fluxo 3: Hardware → Simulação
```
[Sensor Físico (Reed Switch)]
    ↓ detecta trem
[Arduino]
    ↓ envia Serial/MQTT
[Python Bridge]
    ↓ evento
[Motor de Simulação]
    ↓ atualiza posição
[Sincroniza Trem Virtual]
```

### Fluxo 4: Usuário → Simulação
```
[Usuário clica no Dashboard]
    ↓ HTTP POST
[API REST]
    ↓ comando
[Motor de Simulação]
    ↓ executa ação
[Atualiza Estado]
    ↓ WebSocket
[Atualiza Dashboard]
```

---

## 🎨 Padrões de Design

### 1. **Singleton**
- **Onde**: Sistema de configuração, logger
- **Por quê**: Garantir única instância global

```python
# config_loader.py
_config_instance = None

def get_config():
    global _config_instance
    if _config_instance is None:
        _config_instance = Config()
    return _config_instance
```

### 2. **Observer**
- **Onde**: Sistema de eventos da simulação
- **Por quê**: Componentes observam mudanças sem acoplamento

```python
# event_system.py
class EventBus:
    def __init__(self):
        self.listeners = {}
    
    def subscribe(self, event_type, callback):
        if event_type not in self.listeners:
            self.listeners[event_type] = []
        self.listeners[event_type].append(callback)
    
    def publish(self, event_type, data):
        for callback in self.listeners.get(event_type, []):
            callback(data)
```

### 3. **State Machine**
- **Onde**: Comportamento dos agentes
- **Por quê**: Estados bem definidos (dormindo, trabalhando, viajando)

```python
# agent.py
class AgentState(Enum):
    SLEEPING = "sleeping"
    WORKING = "working"
    TRAVELING = "traveling"
    IDLE = "idle"

class Agent:
    def __init__(self):
        self.state = AgentState.IDLE
    
    def transition_to(self, new_state):
        # Validar transição
        self.state = new_state
```

### 4. **Repository**
- **Onde**: Acesso ao banco de dados
- **Por quê**: Abstrai lógica de persistência

```python
# repositories.py
class AgentRepository:
    def __init__(self, session):
        self.session = session
    
    def get_by_id(self, agent_id):
        return self.session.query(Agent).filter_by(id=agent_id).first()
    
    def get_all(self):
        return self.session.query(Agent).all()
```

### 5. **Factory**
- **Onde**: Criação de agentes, edifícios
- **Por quê**: Encapsula lógica de criação

```python
# factories.py
class AgentFactory:
    @staticmethod
    def create_citizen(name, home, work):
        return Agent(
            name=name,
            home=home,
            work=work,
            money=1000.0,
            state=AgentState.IDLE
        )
```

---

## 📦 Tecnologias Utilizadas

### Backend
- **Python 3.10+**: Linguagem principal
- **SQLAlchemy**: ORM para banco de dados
- **FastAPI**: API REST/WebSocket
- **Paho MQTT**: Cliente MQTT
- **PySerial**: Comunicação serial

### Frontend
- **React**: UI framework
- **Recharts**: Gráficos e estatísticas
- **Socket.io**: WebSocket client

### Visualização
- **Pygame**: Visualização 2D
- **Three.js** (futuro): Visualização 3D

### Hardware
- **Arduino**: Microcontrolador
- **ESP32**: WiFi e IoT
- **MQTT Broker** (Mosquitto): Mensageria

---

## 🔐 Segurança

### Autenticação (Futuro)
- JWT tokens para API
- Rate limiting
- CORS configurado

### Dados
- Backup automático do banco
- Logs de auditoria
- Validação de entrada

---

## 📈 Escalabilidade

### Horizontal
- Múltiplas instâncias do backend com load balancer
- MQTT broker distribuído

### Vertical
- Otimização de queries SQL
- Cache com Redis (futuro)
- Processamento assíncrono

---

## 🧪 Testabilidade

### Testes Unitários
- Cada módulo testado isoladamente
- Mocks para dependências externas

### Testes de Integração
- Testes end-to-end
- Simulação completa

### Testes de Hardware
- Ambiente de teste com hardware virtual
- Simulação de sensores

---

## 📚 Referências

- [SQLAlchemy Documentation](https://docs.sqlalchemy.org/)
- [FastAPI Documentation](https://fastapi.tiangolo.com/)
- [MQTT Specification](https://mqtt.org/)
- [Pygame Documentation](https://www.pygame.org/docs/)

---

**Última atualização**: 2025-10-29
"""

# TODO: Criar diagramas Mermaid
MERMAID_DIAGRAMS = """
## Diagrama de Componentes (Mermaid)

```mermaid
graph TB
    subgraph Frontend
        A[Web UI<br/>React]
        B[Visualização 2D<br/>Pygame]
    end
    
    subgraph Backend
        C[API REST<br/>FastAPI]
        D[WebSocket Server]
        E[Motor de Simulação]
        F[Sistema Econômico]
        G[Sistema de Transporte]
    end
    
    subgraph Data
        H[(SQLite<br/>Database)]
        I[Logs]
        J[Config YAML]
    end
    
    subgraph Hardware
        K[Arduino]
        L[ESP32]
        M[MQTT Broker]
    end
    
    A --> C
    A --> D
    B --> E
    C --> E
    D --> E
    E --> F
    E --> G
    E --> H
    E --> I
    E --> J
    E --> M
    M --> K
    M --> L
```

## Diagrama de Sequência (Mermaid)

```mermaid
sequenceDiagram
    participant U as Usuário
    participant W as Web UI
    participant A as API
    participant S as Simulação
    participant D as Database
    participant H as Hardware
    
    U->>W: Clica "Iniciar Simulação"
    W->>A: POST /api/simulation/start
    A->>S: start_simulation()
    S->>D: load_state()
    D-->>S: estado inicial
    S->>S: step() loop
    S->>H: MQTT publish (LEDs)
    H-->>H: atualiza LEDs
    S->>A: WebSocket update
    A-->>W: estado atualizado
    W-->>U: renderiza visualização
```
"""

# TODO: Salvar documentação
# with open('docs/architecture.md', 'w', encoding='utf-8') as f:
#     f.write(ARCHITECTURE_DOC)
#     f.write('\n\n')
#     f.write(MERMAID_DIAGRAMS)

print("TODO: Criar docs/architecture.md com documentação completa da arquitetura")
print("TODO: Adicionar diagramas Mermaid ou PlantUML")
print("TODO: Revisar e expandir conforme o projeto evolui")

