# Guia do Sistema de Configuração

## Visão Geral

O sistema de configuração do Ferritine fornece um gerenciamento centralizado de todas as configurações do projeto através de arquivos YAML estruturados. Este sistema permite:

- ✅ Configuração simples via arquivos YAML
- ✅ Validação automática de valores
- ✅ Tipos estruturados e seguros
- ✅ Carregamento/salvamento automático
- ✅ Singleton para acesso global
- ✅ Logging de todas as operações

## Estrutura de Arquivos

```
ferritine/
├── data/
│   └── config.yaml              # Arquivo de configuração principal
├── backend/
│   └── utils/
│       └── config_loader.py     # Módulo de configuração
```

## Uso Básico

### Acessar configurações

```python
from backend.utils.config_loader import get_config

config = get_config()

# Acessar valores específicos
speed = config.simulation.speed
salary = config.economy.base_salary
window_width = config.visualization.window_width
```

### Modificar configurações em tempo de execução

```python
from backend.utils.config_loader import get_config

config = get_config()

# Modificar valores
config.simulation.speed = 2.0
config.economy.base_salary = 1500.0

# Salvar as mudanças no arquivo
config.save()
```

### Recarregar configurações do arquivo

```python
from backend.utils.config_loader import get_config

config = get_config()

# Recarrega todas as configurações do arquivo YAML
config.reload()
```

### Validar configurações

```python
from backend.utils.config_loader import get_config

config = get_config()

# Valida todas as configurações
if config.validate():
    print("Todas as configurações são válidas!")
else:
    print("Alguns valores estão inválidos!")
```

### Obter resumo das configurações

```python
from backend.utils.config_loader import get_config

config = get_config()

# Imprime um resumo formatado de todas as configurações
print(config.get_summary())
```

## Arquivo de Configuração (config.yaml)

O arquivo `data/config.yaml` contém todas as configurações agrupadas por seção:

### Seção: Simulation

```yaml
simulation:
  speed: 1.0                    # Velocidade da simulação (0.1-10.0)
  start_time: 6                 # Hora inicial (0-23)
  tick_rate: 60                 # Ticks por segundo (> 0)
  auto_save_interval: 300       # Segundos entre auto-saves (> 0)
```

**Parâmetros:**
- `speed`: Multiplicador da velocidade da simulação
  - `0.5` = 2x mais lento
  - `1.0` = tempo real
  - `2.0` = 2x mais rápido
  - Range válido: 0.1 a 10.0

- `start_time`: Hora do dia em que a simulação começa (0-23, onde 0 = meia-noite)

- `tick_rate`: Quantas vezes por segundo o simulador atualiza o estado

- `auto_save_interval`: Intervalo em segundos para salvamento automático do estado

### Seção: Database

```yaml
database:
  path: "data/db/city.db"      # Caminho do arquivo do banco de dados
  echo_sql: false              # Mostrar queries SQL no console
  pool_size: 10                # Número de conexões simultâneas
```

**Parâmetros:**
- `path`: Caminho para o arquivo SQLite (será criado automaticamente)

- `echo_sql`: Ativa/desativa logging de todas as queries SQL (útil para debug)

- `pool_size`: Número máximo de conexões abertas simultaneamente com o BD

### Seção: IoT

```yaml
iot:
  serial_port: "/dev/ttyUSB0"      # Porta serial do Arduino/hardware
  baud_rate: 115200               # Velocidade de comunicação
  mqtt_broker: "localhost"         # Endereço do broker MQTT
  mqtt_port: 1883                 # Porta do broker MQTT
  mqtt_topic_prefix: "ferritine"   # Prefixo dos tópicos
```

**Parâmetros:**
- `serial_port`: Porta de conexão com o hardware
  - Linux: `/dev/ttyUSB0`, `/dev/ttyACM0`
  - Windows: `COM3`, `COM4`
  - macOS: `/dev/tty.usbserial-*`

- `baud_rate`: Velocidade de transmissão em bauds (típico: 9600, 115200)

- `mqtt_broker`: IP ou hostname do servidor MQTT

- `mqtt_port`: Porta do MQTT (padrão: 1883)

- `mqtt_topic_prefix`: Prefixo usado para todos os tópicos MQTT

### Seção: Economy

```yaml
economy:
  base_salary: 1000.0          # Salário base dos agentes
  rent_multiplier: 0.3         # Aluguel como % do salário
  food_cost_per_day: 20.0      # Custo diário de alimento
  transport_cost: 5.0          # Custo de transporte
```

**Parâmetros:**
- `base_salary`: Salário mensal/periódico base (> 0)

- `rent_multiplier`: Quanto do salário é gasto com aluguel (0.0-1.0)
  - `0.3` = 30% do salário

- `food_cost_per_day`: Custo diário de alimentação por agente (>= 0)

- `transport_cost`: Custo por deslocamento de transporte (>= 0)

### Seção: Visualization

```yaml
visualization:
  window_width: 1280           # Largura da janela
  window_height: 720          # Altura da janela
  fps: 60                      # Taxa de frames por segundo
  grid_size: 32                # Tamanho de cada célula em pixels
  show_debug: false            # Ativar modo debug
```

**Parâmetros:**
- `window_width`: Resolução horizontal (> 0)

- `window_height`: Resolução vertical (> 0)

- `fps`: Atualização de frames por segundo (> 0)

- `grid_size`: Tamanho em pixels de cada célula do grid (> 0)

- `show_debug`: Habilita visualização de informações de debug

## Classes de Configuração

### SimulationConfig

```python
@dataclass
class SimulationConfig:
    speed: float = 1.0
    start_time: int = 0
    tick_rate: int = 60
    auto_save_interval: int = 300
```

### DatabaseConfig

```python
@dataclass
class DatabaseConfig:
    path: str = "data/db/city.db"
    echo_sql: bool = False
    pool_size: int = 10
```

### IoTConfig

```python
@dataclass
class IoTConfig:
    serial_port: str = "/dev/ttyUSB0"
    baud_rate: int = 115200
    mqtt_broker: str = "localhost"
    mqtt_port: int = 1883
    mqtt_topic_prefix: str = "ferritine"
```

### EconomyConfig

```python
@dataclass
class EconomyConfig:
    base_salary: float = 1000.0
    rent_multiplier: float = 0.3
    food_cost_per_day: float = 20.0
    transport_cost: float = 5.0
```

### VisualizationConfig

```python
@dataclass
class VisualizationConfig:
    window_width: int = 1280
    window_height: int = 720
    fps: int = 60
    grid_size: int = 32
    show_debug: bool = False
```

## Validação

O sistema valida automaticamente todas as configurações ao carregar. As validações incluem:

### SimulationConfig
- `speed`: deve estar entre 0.1 e 10.0
- `start_time`: deve estar entre 0 e 23
- `tick_rate`: deve ser positivo
- `auto_save_interval`: deve ser positivo

### DatabaseConfig
- `path`: não pode estar vazio
- `pool_size`: deve ser positivo

### IoTConfig
- `serial_port`: não pode estar vazio
- `baud_rate`: deve ser positivo
- `mqtt_broker`: não pode estar vazio
- `mqtt_port`: deve estar entre 0 e 65535

### EconomyConfig
- `base_salary`: deve ser positivo
- `rent_multiplier`: deve estar entre 0 e 1
- `food_cost_per_day`: não pode ser negativo
- `transport_cost`: não pode ser negativo

### VisualizationConfig
- `window_width`: deve ser positivo
- `window_height`: deve ser positivo
- `fps`: deve ser positivo
- `grid_size`: deve ser positivo

## Logging

Todas as operações de configuração são registradas via sistema de logging:

```
[2025-01-11 14:30:45] [INFO] [config_loader] Carregando configurações de: data/config.yaml
[2025-01-11 14:30:45] [DEBUG] [config_loader] Configurações de simulação carregadas
[2025-01-11 14:30:45] [DEBUG] [config_loader] Configurações de banco de dados carregadas
[2025-01-11 14:30:45] [INFO] [config_loader] Configurações carregadas e validadas com sucesso
```

## Exemplos Práticos

### Exemplo 1: Usar configuração em um módulo

```python
# app/models/agente.py
from backend.utils.config_loader import get_config
from backend.utils.logger import get_logger

logger = get_logger(__name__)
config = get_config()

class Agente:
    def __init__(self, nome, casa, trabalho):
        self.nome = nome
        self.casa = casa
        self.trabalho = trabalho
        self.saldo = config.economy.base_salary
        logger.debug(f"Agente {nome} criado com salário: ${self.saldo}")

    def pagar_despesas(self, horas):
        aluguel = config.economy.base_salary * config.economy.rent_multiplier
        comida = config.economy.food_cost_per_day * (horas / 24)
        self.saldo -= aluguel + comida
        logger.debug(f"{self.nome} pagou ${aluguel + comida:.2f} em despesas")
```

### Exemplo 2: Configurar simulação em tempo de execução

```python
# main.py
from backend.utils.config_loader import get_config
from backend.simulation import Simulador

config = get_config()

# Iniciar simulação com configurações padrão
sim = Simulador(
    speed=config.simulation.speed,
    start_time=config.simulation.start_time,
    tick_rate=config.simulation.tick_rate
)

# Usuário quer 2x mais rápido
config.simulation.speed = 2.0
config.save()

# Recarregar para ver mudanças
config.reload()
print(f"Velocidade da simulação: {config.simulation.speed}x")
```

### Exemplo 3: Modo de configuração para diferentes ambientes

```python
# Criar config.yaml diferente para desenvolvimento/produção

# config-dev.yaml
simulation:
  speed: 5.0              # Mais rápido para testes
  tick_rate: 120          # Mais precisão
  auto_save_interval: 60  # Salvar frequentemente

# config-prod.yaml
simulation:
  speed: 1.0              # Tempo real
  tick_rate: 60           # Normal
  auto_save_interval: 300 # 5 minutos

# Carregar configuração apropriada
import os
from backend.utils.config_loader import get_config, reset_config

env = os.getenv('FERRITINE_ENV', 'dev')
reset_config()
config = get_config(f"data/config-{env}.yaml")
```

## Testes

Para testar com diferentes configurações:

```python
# tests/test_config.py
import pytest
from backend.utils.config_loader import Config, reset_config
from pathlib import Path

def test_config_defaults():
    """Testa valores padrão."""
    config = Config()
    assert config.simulation.speed == 1.0
    assert config.database.pool_size == 10

def test_config_validation():
    """Testa validação."""
    config = Config()
    assert config.validate() == True
    
    # Valor inválido
    config.simulation.speed = 15.0  # Fora do range
    assert config.validate() == False

def test_config_save_load(tmp_path):
    """Testa salvamento e carregamento."""
    config_file = tmp_path / "test_config.yaml"
    config1 = Config(str(config_file))
    
    config1.simulation.speed = 2.5
    config1.save()
    
    # Novo objeto carrega o arquivo
    config2 = Config(str(config_file))
    assert config2.simulation.speed == 2.5
```

## Troubleshooting

### Erro: "Arquivo de configuração não encontrado"

O arquivo será criado automaticamente com valores padrão na primeira execução.

### Erro: "Configurações não validadas"

Verifique os valores no arquivo `data/config.yaml` contra as regras de validação acima.

### Erro: "ModuleNotFoundError: No module named 'yaml'"

Execute: `pip install -r requirements.txt`

### Alterações não aparecem

Use `config.reload()` para recarregar as mudanças do arquivo.

## Próximos Passos

1. Integrar configuração em todos os módulos principais
2. Adicionar suporte a múltiplos perfis de configuração
3. Implementar interface CLI para editar configurações
4. Adicionar validação de tipos mais robusta (Pydantic)
5. Suporte a variáveis de ambiente para override

## API Completa

```python
from backend.utils.config_loader import (
    get_config,              # Obter instância singleton
    reset_config,            # Resetar singleton (testes)
    Config,                  # Classe principal
    SimulationConfig,        # Classe de configuração
    DatabaseConfig,
    IoTConfig,
    EconomyConfig,
    VisualizationConfig
)

# Métodos disponíveis
config = get_config()
config.load()                    # Carregar do arquivo
config.save()                    # Salvar para arquivo
config.validate()                # Validar valores
config.reload()                  # Recarregar do arquivo
config.get_summary()             # Obter resumo formatado

# Atributos de configuração
config.simulation                # SimulationConfig
config.database                  # DatabaseConfig
config.iot                       # IoTConfig
config.economy                   # EconomyConfig
config.visualization             # VisualizationConfig
```

