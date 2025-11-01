"""
Sistema de configuração centralizado para o projeto Ferritine.

Fornece gerenciamento de configurações via arquivos YAML com validação
e tipos estruturados.

Exemplo de uso:
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

import yaml
from pathlib import Path
from typing import Any, Dict, Optional
from dataclasses import dataclass, asdict
from backend.utils.logger import get_logger

logger = get_logger(__name__)


@dataclass
class SimulationConfig:
    """Configurações da simulação."""
    speed: float = 1.0  # Velocidade de simulação (1.0 = tempo real)
    start_time: int = 0  # Hora inicial (0-23)
    tick_rate: int = 60  # Ticks por segundo
    auto_save_interval: int = 300  # Segundos entre auto-saves


@dataclass
class DatabaseConfig:
    """Configurações do banco de dados."""
    path: str = "data/db/city.db"
    echo_sql: bool = False  # Mostrar SQL no console
    pool_size: int = 10


@dataclass
class IoTConfig:
    """Configurações de IoT e hardware."""
    serial_port: str = "/dev/ttyUSB0"
    baud_rate: int = 115200
    mqtt_broker: str = "localhost"
    mqtt_port: int = 1883
    mqtt_topic_prefix: str = "ferritine"


@dataclass
class EconomyConfig:
    """Parâmetros econômicos."""
    base_salary: float = 1000.0
    rent_multiplier: float = 0.3
    food_cost_per_day: float = 20.0
    transport_cost: float = 5.0


@dataclass
class VisualizationConfig:
    """Configurações de visualização."""
    window_width: int = 1280
    window_height: int = 720
    fps: int = 60
    grid_size: int = 32  # Pixels por célula
    show_debug: bool = False


class Config:
    """Gerenciador de configuração centralizado."""

    def __init__(self, config_path: str = "data/config.yaml"):
        """
        Inicializa o gerenciador de configurações.

        Args:
            config_path: Caminho para o arquivo de configuração YAML
        """
        self.config_path = Path(config_path)

        # Inicializa com valores padrão
        self.simulation = SimulationConfig()
        self.database = DatabaseConfig()
        self.iot = IoTConfig()
        self.economy = EconomyConfig()
        self.visualization = VisualizationConfig()

        # Carrega configurações se o arquivo existir
        if self.config_path.exists():
            logger.info(f"Carregando configurações de: {self.config_path}")
            self.load()
        else:
            logger.info(f"Arquivo de configuração não encontrado: {self.config_path}")
            logger.info("Criando arquivo de configuração com valores padrão...")
            self.save()

    def load(self):
        """
        Carrega configurações do arquivo YAML.

        Valida e aplica as configurações ao sistema.
        """
        try:
            with open(self.config_path, 'r', encoding='utf-8') as f:
                data = yaml.safe_load(f)

            if data is None:
                logger.warning("Arquivo de configuração vazio, usando valores padrão")
                return

            # Carregar cada seção de configuração
            if 'simulation' in data:
                self._load_section(self.simulation, data['simulation'])
                logger.debug("Configurações de simulação carregadas")

            if 'database' in data:
                self._load_section(self.database, data['database'])
                logger.debug("Configurações de banco de dados carregadas")

            if 'iot' in data:
                self._load_section(self.iot, data['iot'])
                logger.debug("Configurações de IoT carregadas")

            if 'economy' in data:
                self._load_section(self.economy, data['economy'])
                logger.debug("Configurações econômicas carregadas")

            if 'visualization' in data:
                self._load_section(self.visualization, data['visualization'])
                logger.debug("Configurações de visualização carregadas")

            if self.validate():
                logger.info("Configurações carregadas e validadas com sucesso")
            else:
                logger.warning("Algumas configurações não passaram na validação")

        except Exception as e:
            logger.error(f"Erro ao carregar configurações: {e}")
            raise

    @staticmethod
    def _load_section(config_obj: Any, data: Dict[str, Any]) -> None:
        """
        Carrega dados em um objeto de configuração (dataclass).

        Args:
            config_obj: Objeto dataclass para atualizar
            data: Dicionário com dados YAML
        """
        for field_name, field_value in data.items():
            if hasattr(config_obj, field_name):
                setattr(config_obj, field_name, field_value)

    def save(self):
        """Salva configurações no arquivo YAML."""
        try:
            self.config_path.parent.mkdir(parents=True, exist_ok=True)

            data = {
                'simulation': asdict(self.simulation),
                'database': asdict(self.database),
                'iot': asdict(self.iot),
                'economy': asdict(self.economy),
                'visualization': asdict(self.visualization),
            }

            with open(self.config_path, 'w', encoding='utf-8') as f:
                yaml.dump(data, f, default_flow_style=False, allow_unicode=True,
                         sort_keys=False)

            logger.info(f"Configurações salvas em: {self.config_path}")

        except Exception as e:
            logger.error(f"Erro ao salvar configurações: {e}")
            raise

    def validate(self) -> bool:
        """
        Valida todas as configurações.

        Verifica:
        - Tipos corretos
        - Ranges válidos
        - Valores obrigatórios

        Returns:
            True se todas as configurações são válidas
        """
        try:
            # Validar SimulationConfig
            if not (0.1 <= self.simulation.speed <= 10.0):
                logger.warning("simulation.speed fora do range (0.1-10.0)")
                return False

            if not (0 <= self.simulation.start_time <= 23):
                logger.warning("simulation.start_time deve estar entre 0-23")
                return False

            if self.simulation.tick_rate <= 0:
                logger.warning("simulation.tick_rate deve ser positivo")
                return False

            if self.simulation.auto_save_interval <= 0:
                logger.warning("simulation.auto_save_interval deve ser positivo")
                return False

            # Validar DatabaseConfig
            if not self.database.path:
                logger.warning("database.path não pode estar vazio")
                return False

            if self.database.pool_size <= 0:
                logger.warning("database.pool_size deve ser positivo")
                return False

            # Validar IoTConfig
            if not self.iot.serial_port:
                logger.warning("iot.serial_port não pode estar vazio")
                return False

            if self.iot.baud_rate <= 0:
                logger.warning("iot.baud_rate deve ser positivo")
                return False

            if not self.iot.mqtt_broker:
                logger.warning("iot.mqtt_broker não pode estar vazio")
                return False

            if not (0 <= self.iot.mqtt_port <= 65535):
                logger.warning("iot.mqtt_port deve estar entre 0-65535")
                return False

            # Validar EconomyConfig
            if self.economy.base_salary <= 0:
                logger.warning("economy.base_salary deve ser positivo")
                return False

            if not (0 <= self.economy.rent_multiplier <= 1):
                logger.warning("economy.rent_multiplier deve estar entre 0-1")
                return False

            if self.economy.food_cost_per_day < 0:
                logger.warning("economy.food_cost_per_day não pode ser negativo")
                return False

            if self.economy.transport_cost < 0:
                logger.warning("economy.transport_cost não pode ser negativo")
                return False

            # Validar VisualizationConfig
            if self.visualization.window_width <= 0:
                logger.warning("visualization.window_width deve ser positivo")
                return False

            if self.visualization.window_height <= 0:
                logger.warning("visualization.window_height deve ser positivo")
                return False

            if self.visualization.fps <= 0:
                logger.warning("visualization.fps deve ser positivo")
                return False

            if self.visualization.grid_size <= 0:
                logger.warning("visualization.grid_size deve ser positivo")
                return False

            logger.debug("Todas as configurações validadas com sucesso")
            return True

        except Exception as e:
            logger.error(f"Erro ao validar configurações: {e}")
            return False

    def reload(self):
        """Recarrega configurações do arquivo."""
        logger.info("Recarregando configurações...")
        self.load()

    def get_summary(self) -> str:
        """
        Retorna um resumo de todas as configurações carregadas.

        Returns:
            String formatada com resumo das configurações
        """
        summary = "=== Resumo de Configurações ===\n"
        summary += f"Arquivo: {self.config_path}\n\n"

        summary += "Simulação:\n"
        summary += f"  - Velocidade: {self.simulation.speed}x\n"
        summary += f"  - Hora inicial: {self.simulation.start_time}:00\n"
        summary += f"  - Taxa de atualização: {self.simulation.tick_rate} ticks/s\n"
        summary += f"  - Auto-save: a cada {self.simulation.auto_save_interval}s\n\n"

        summary += "Banco de Dados:\n"
        summary += f"  - Caminho: {self.database.path}\n"
        summary += f"  - Echo SQL: {self.database.echo_sql}\n"
        summary += f"  - Pool size: {self.database.pool_size}\n\n"

        summary += "IoT:\n"
        summary += f"  - Porta serial: {self.iot.serial_port}\n"
        summary += f"  - Baud rate: {self.iot.baud_rate}\n"
        summary += f"  - MQTT Broker: {self.iot.mqtt_broker}:{self.iot.mqtt_port}\n"
        summary += f"  - Tópico MQTT: {self.iot.mqtt_topic_prefix}\n\n"

        summary += "Economia:\n"
        summary += f"  - Salário base: ${self.economy.base_salary:.2f}\n"
        summary += f"  - Multiplicador de aluguel: {self.economy.rent_multiplier:.1%}\n"
        summary += f"  - Custo de alimento/dia: ${self.economy.food_cost_per_day:.2f}\n"
        summary += f"  - Custo de transporte: ${self.economy.transport_cost:.2f}\n\n"

        summary += "Visualização:\n"
        summary += f"  - Resolução: {self.visualization.window_width}x{self.visualization.window_height}\n"
        summary += f"  - FPS: {self.visualization.fps}\n"
        summary += f"  - Tamanho da célula: {self.visualization.grid_size}px\n"
        summary += f"  - Debug: {'Ativado' if self.visualization.show_debug else 'Desativado'}\n"

        return summary


# Singleton global
_config_instance: Optional[Config] = None


def get_config() -> Config:
    """
    Retorna instância singleton de configuração.

    Garante que apenas uma instância de Config seja criada durante
    a execução do programa.

    Returns:
        Instância singleton de Config
    """
    global _config_instance
    if _config_instance is None:
        _config_instance = Config()
    return _config_instance


def reset_config():
    """
    Reseta a instância singleton de configuração.

    Útil para testes que precisam recarregar configurações.
    """
    global _config_instance
    _config_instance = None

