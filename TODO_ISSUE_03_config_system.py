# TODO: Issue #3 - Implementar sistema de configuração com YAML
# Labels: feat, phase-0: fundamentals, priority: high, complexity: intermediate
# Milestone: Milestone 0: Fundamentos e Infraestrutura

"""
Criar sistema de configuração centralizado usando arquivos YAML.

Tarefas:
- [ ] Criar data/config.yaml com configurações padrão
- [ ] Implementar backend/utils/config_loader.py
- [ ] Adicionar validação de configurações
- [ ] Criar exemplo de arquivo de configuração (config.example.yaml)
- [ ] Documentar todas as opções de configuração
- [ ] Atualizar código para usar configurações centralizadas

Configurações a incluir:
- Parâmetros de simulação (velocidade, tempo inicial)
- Configurações de banco de dados
- Configurações de IoT (porta serial, broker MQTT)
- Parâmetros econômicos (salários, preços)
- Configurações de visualização
"""

import yaml
from pathlib import Path
from typing import Any, Dict, Optional
from dataclasses import dataclass

# TODO: Criar estrutura de configuração
@dataclass
class SimulationConfig:
    """Configurações da simulação."""
    speed: float = 1.0  # Velocidade de simulação (1.0 = tempo real)
    start_time: int = 0  # Hora inicial (0-23)
    tick_rate: int = 60  # Ticks por segundo
    auto_save_interval: int = 300  # Segundos entre auto-saves
    
    # TODO: Adicionar mais parâmetros


@dataclass
class DatabaseConfig:
    """Configurações do banco de dados."""
    path: str = "data/db/city.db"
    echo_sql: bool = False  # Mostrar SQL no console
    pool_size: int = 10
    
    # TODO: Adicionar mais parâmetros


@dataclass
class IoTConfig:
    """Configurações de IoT e hardware."""
    serial_port: str = "/dev/ttyUSB0"
    baud_rate: int = 115200
    mqtt_broker: str = "localhost"
    mqtt_port: int = 1883
    mqtt_topic_prefix: str = "ferritine"
    
    # TODO: Adicionar mais parâmetros


@dataclass
class EconomyConfig:
    """Parâmetros econômicos."""
    base_salary: float = 1000.0
    rent_multiplier: float = 0.3
    food_cost_per_day: float = 20.0
    transport_cost: float = 5.0
    
    # TODO: Adicionar mais parâmetros


@dataclass
class VisualizationConfig:
    """Configurações de visualização."""
    window_width: int = 1280
    window_height: int = 720
    fps: int = 60
    grid_size: int = 32  # Pixels por célula
    show_debug: bool = False
    
    # TODO: Adicionar mais parâmetros


# TODO: Criar classe principal de configuração
class Config:
    """Gerenciador de configuração centralizado."""
    
    def __init__(self, config_path: str = "data/config.yaml"):
        self.config_path = Path(config_path)
        
        # TODO: Carregar configurações
        self.simulation = SimulationConfig()
        self.database = DatabaseConfig()
        self.iot = IoTConfig()
        self.economy = EconomyConfig()
        self.visualization = VisualizationConfig()
        
        if self.config_path.exists():
            self.load()
        else:
            # TODO: Criar arquivo de configuração padrão
            self.save()
    
    def load(self):
        """Carrega configurações do arquivo YAML."""
        # TODO: Implementar carregamento
        with open(self.config_path, 'r', encoding='utf-8') as f:
            data = yaml.safe_load(f)
        
        # TODO: Validar e aplicar configurações
        if 'simulation' in data:
            # Atualizar self.simulation com valores do YAML
            pass
        
        # TODO: Fazer o mesmo para outras seções
        pass
    
    def save(self):
        """Salva configurações no arquivo YAML."""
        # TODO: Implementar salvamento
        self.config_path.parent.mkdir(parents=True, exist_ok=True)
        
        data = {
            'simulation': {
                'speed': self.simulation.speed,
                'start_time': self.simulation.start_time,
                'tick_rate': self.simulation.tick_rate,
                'auto_save_interval': self.simulation.auto_save_interval,
            },
            'database': {
                'path': self.database.path,
                'echo_sql': self.database.echo_sql,
                'pool_size': self.database.pool_size,
            },
            'iot': {
                'serial_port': self.iot.serial_port,
                'baud_rate': self.iot.baud_rate,
                'mqtt_broker': self.iot.mqtt_broker,
                'mqtt_port': self.iot.mqtt_port,
                'mqtt_topic_prefix': self.iot.mqtt_topic_prefix,
            },
            'economy': {
                'base_salary': self.economy.base_salary,
                'rent_multiplier': self.economy.rent_multiplier,
                'food_cost_per_day': self.economy.food_cost_per_day,
                'transport_cost': self.economy.transport_cost,
            },
            'visualization': {
                'window_width': self.visualization.window_width,
                'window_height': self.visualization.window_height,
                'fps': self.visualization.fps,
                'grid_size': self.visualization.grid_size,
                'show_debug': self.visualization.show_debug,
            }
        }
        
        with open(self.config_path, 'w', encoding='utf-8') as f:
            yaml.dump(data, f, default_flow_style=False, allow_unicode=True)
    
    def validate(self) -> bool:
        """Valida configurações."""
        # TODO: Implementar validação
        # - Verificar tipos
        # - Verificar ranges válidos
        # - Verificar dependências
        return True
    
    def reload(self):
        """Recarrega configurações do arquivo."""
        self.load()


# TODO: Criar singleton global
_config_instance: Optional[Config] = None

def get_config() -> Config:
    """Retorna instância singleton de configuração."""
    global _config_instance
    if _config_instance is None:
        _config_instance = Config()
    return _config_instance


# TODO: Criar arquivo de exemplo config.example.yaml
EXAMPLE_CONFIG = """
# Configuração do Ferritine
# Copie este arquivo para data/config.yaml e ajuste conforme necessário

simulation:
  speed: 1.0              # Velocidade da simulação (1.0 = tempo real)
  start_time: 6           # Hora inicial (0-23)
  tick_rate: 60           # Ticks por segundo
  auto_save_interval: 300 # Auto-save a cada 5 minutos

database:
  path: "data/db/city.db"
  echo_sql: false         # Mostrar queries SQL no console
  pool_size: 10

iot:
  serial_port: "/dev/ttyUSB0"  # Porta serial do Arduino
  baud_rate: 115200
  mqtt_broker: "localhost"
  mqtt_port: 1883
  mqtt_topic_prefix: "ferritine"

economy:
  base_salary: 1000.0
  rent_multiplier: 0.3    # Aluguel = 30% do salário
  food_cost_per_day: 20.0
  transport_cost: 5.0

visualization:
  window_width: 1280
  window_height: 720
  fps: 60
  grid_size: 32           # Pixels por célula do grid
  show_debug: false
"""

# TODO: Uso no código
"""
# Em qualquer módulo:
from backend.utils.config_loader import get_config

config = get_config()

# Acessar configurações
print(config.simulation.speed)
print(config.economy.base_salary)

# Modificar e salvar
config.simulation.speed = 2.0
config.save()

# Recarregar do arquivo
config.reload()
"""

# TODO: Adicionar ao requirements.txt
# PyYAML>=6.0

