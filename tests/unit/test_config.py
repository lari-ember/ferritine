"""
Testes para o sistema de configuração.

Valida:
- Carregamento de configurações
- Validação de valores
- Salvamento e carregamento
- Singleton pattern
"""

import pytest
import tempfile
from pathlib import Path
from backend.utils.config_loader import (
    Config, get_config, reset_config,
    SimulationConfig, DatabaseConfig, IoTConfig,
    EconomyConfig, VisualizationConfig
)


class TestConfigDefaults:
    """Testa valores padrão das configurações."""

    def test_simulation_defaults(self):
        """Testa valores padrão de SimulationConfig."""
        config = SimulationConfig()
        assert config.speed == 1.0
        assert config.start_time == 0
        assert config.tick_rate == 60
        assert config.auto_save_interval == 300

    def test_database_defaults(self):
        """Testa valores padrão de DatabaseConfig."""
        config = DatabaseConfig()
        assert config.path == "data/db/city.db"
        assert config.echo_sql is False
        assert config.pool_size == 10

    def test_iot_defaults(self):
        """Testa valores padrão de IoTConfig."""
        config = IoTConfig()
        assert config.serial_port == "/dev/ttyUSB0"
        assert config.baud_rate == 115200
        assert config.mqtt_broker == "localhost"
        assert config.mqtt_port == 1883
        assert config.mqtt_topic_prefix == "ferritine"

    def test_economy_defaults(self):
        """Testa valores padrão de EconomyConfig."""
        config = EconomyConfig()
        assert config.base_salary == 1000.0
        assert config.rent_multiplier == 0.3
        assert config.food_cost_per_day == 20.0
        assert config.transport_cost == 5.0

    def test_visualization_defaults(self):
        """Testa valores padrão de VisualizationConfig."""
        config = VisualizationConfig()
        assert config.window_width == 1280
        assert config.window_height == 720
        assert config.fps == 60
        assert config.grid_size == 32
        assert config.show_debug is False


class TestConfigValidation:
    """Testa validação de configurações."""

    def test_valid_config(self):
        """Testa que configuração padrão é válida."""
        config = Config()
        assert config.validate() is True

    def test_simulation_speed_range(self):
        """Testa validação de speed (0.1-10.0)."""
        config = Config()
        
        # Válido
        config.simulation.speed = 0.1
        assert config.validate() is True
        
        config.simulation.speed = 10.0
        assert config.validate() is True
        
        # Inválido - muito baixo
        config.simulation.speed = 0.05
        assert config.validate() is False
        
        # Inválido - muito alto
        config.simulation.speed = 15.0
        assert config.validate() is False

    def test_start_time_range(self):
        """Testa validação de start_time (0-23)."""
        config = Config()
        
        # Válido
        config.simulation.start_time = 0
        assert config.validate() is True
        
        config.simulation.start_time = 23
        assert config.validate() is True
        
        # Inválido
        config.simulation.start_time = -1
        assert config.validate() is False
        
        config.simulation.start_time = 24
        assert config.validate() is False

    def test_positive_values(self):
        """Testa validação de valores que devem ser positivos."""
        config = Config()
        
        # tick_rate
        config.simulation.tick_rate = 0
        assert config.validate() is False
        
        config.simulation.tick_rate = 60
        assert config.validate() is True
        
        # base_salary
        config.economy.base_salary = 0
        assert config.validate() is False
        
        config.economy.base_salary = 1000.0
        assert config.validate() is True

    def test_rent_multiplier_range(self):
        """Testa validação de rent_multiplier (0-1)."""
        config = Config()
        
        # Válido
        config.economy.rent_multiplier = 0.0
        assert config.validate() is True
        
        config.economy.rent_multiplier = 0.5
        assert config.validate() is True
        
        config.economy.rent_multiplier = 1.0
        assert config.validate() is True
        
        # Inválido
        config.economy.rent_multiplier = 1.5
        assert config.validate() is False
        
        config.economy.rent_multiplier = -0.1
        assert config.validate() is False

    def test_mqtt_port_range(self):
        """Testa validação de mqtt_port (0-65535)."""
        config = Config()
        
        # Válido
        config.iot.mqtt_port = 1883
        assert config.validate() is True
        
        config.iot.mqtt_port = 0
        assert config.validate() is True
        
        config.iot.mqtt_port = 65535
        assert config.validate() is True
        
        # Inválido
        config.iot.mqtt_port = -1
        assert config.validate() is False
        
        config.iot.mqtt_port = 65536
        assert config.validate() is False


class TestConfigSaveLoad:
    """Testa salvamento e carregamento de configurações."""

    def test_save_creates_file(self):
        """Testa que save cria o arquivo YAML."""
        with tempfile.TemporaryDirectory() as tmpdir:
            config_file = Path(tmpdir) / "test_config.yaml"
            
            config = Config(str(config_file))
            # Garantir que o arquivo foi criado explicitamente
            config.save()
            assert config_file.exists()
            assert config_file.stat().st_size > 0

    def test_load_modifies_config(self):
        """Testa que load carrega valores do arquivo."""
        with tempfile.TemporaryDirectory() as tmpdir:
            config_file = Path(tmpdir) / "test_config.yaml"
            
            # Criar e salvar com valores modificados
            config1 = Config(str(config_file))
            config1.simulation.speed = 2.5
            config1.economy.base_salary = 1500.0
            config1.visualization.fps = 120
            config1.save()
            
            # Criar novo objeto que carrega o arquivo
            config2 = Config(str(config_file))
            
            # Verificar que os valores foram carregados
            assert config2.simulation.speed == 2.5
            assert config2.economy.base_salary == 1500.0
            assert config2.visualization.fps == 120

    def test_reload_updates_values(self):
        """Testa que reload atualiza valores do arquivo."""
        with tempfile.TemporaryDirectory() as tmpdir:
            config_file = Path(tmpdir) / "test_config.yaml"
            config = Config(str(config_file))
            
            # Modificar valor em arquivo
            config.simulation.speed = 2.0
            config.save()
            
            # Alterar na memória
            config.simulation.speed = 5.0
            assert config.simulation.speed == 5.0
            
            # Recarregar do arquivo
            config.reload()
            assert config.simulation.speed == 2.0


class TestConfigSingleton:
    """Testa padrão singleton."""

    def test_singleton_returns_same_instance(self):
        """Testa que get_config retorna a mesma instância."""
        reset_config()
        
        config1 = get_config()
        config2 = get_config()
        
        assert config1 is config2

    def test_reset_singleton(self):
        """Testa que reset_config cria nova instância."""
        config1 = get_config()
        reset_config()
        config2 = get_config()
        
        assert config1 is not config2


class TestConfigSummary:
    """Testa méto do get_summary."""

    def test_summary_contains_values(self):
        """Testa que summary contém todos os valores."""
        config = Config()
        summary = config.get_summary()
        
        # Verificar que contém seções principais
        assert "Simulação" in summary
        assert "Banco de Dados" in summary
        assert "IoT" in summary
        assert "Economia" in summary
        assert "Visualização" in summary
        
        # Verificar que contém valores
        assert "1.0x" in summary
        assert "1000.00" in summary
        assert "1280x720" in summary


class TestConfigIntegration:
    """Testes de integração."""

    def test_full_workflow(self):
        """Testa fluxo completo: criar, salvar, carregar, validar."""
        with tempfile.TemporaryDirectory() as tmpdir:
            config_file = Path(tmpdir) / "integration_test.yaml"
            
            # 1. Criar configuração
            config = Config(str(config_file))
            assert config is not None
            
            # 2. Validar
            assert config.validate() is True
            
            # 3. Modificar
            config.simulation.speed = 3.0
            config.economy.base_salary = 2000.0
            
            # 4. Salvar
            config.save()
            assert config_file.exists()
            
            # 5. Carregar em nova instância
            config2 = Config(str(config_file))
            
            # 6. Verificar valores
            assert config2.simulation.speed == 3.0
            assert config2.economy.base_salary == 2000.0
            assert config2.validate() is True


if __name__ == "__main__":
    pytest.main([__file__, "-v"])

