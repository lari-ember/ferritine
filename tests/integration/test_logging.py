"""
Teste do sistema de logging centralizado do Ferritine.

Valida que:
- Logger é criado sem erros
- Diferentes níveis de log funcionam
- Arquivos de log são criados
- Formatação é aplicada corretamente
"""

import pytest
from pathlib import Path
from backend.utils.logger import get_logger, FerritineLogger
from backend.simulation.models.agente import Agente
from backend.simulation.models.cidade import Cidade
import time


@pytest.fixture
def logger():
    """Fixture que fornece um logger para os testes."""
    return get_logger("test_module")


def test_logger_creation():
    """Testa criação básica de logger."""
    logger = get_logger("test_module")
    assert logger is not None
    assert isinstance(logger, FerritineLogger)


def test_log_levels(logger):
    """Testa diferentes níveis de log."""
    # Não lança exceções
    logger.debug("Mensagem DEBUG - não aparece no console")
    logger.info("Mensagem INFO - aparece no console")
    logger.warning("Mensagem WARNING - aparece no console")
    logger.error("Mensagem ERROR - aparece no console e em errors.log")
    logger.critical("Mensagem CRITICAL - aparece em todos os logs")


def test_log_formatting(logger):
    """Testa formatação de mensagens."""
    # Não lança exceções com diferentes formatações
    logger.info("Teste com argumento: nome=%s, valor=%d", "teste", 42)
    logger.debug("Teste com múltiplos args: %s, %s, %s", "a", "b", "c")


def test_log_files():
    """Testa criação de arquivos de log."""
    log_dir = Path("data/logs")

    # Aguardar um pouco para garantir que os logs foram escritos
# Esperar até que os arquivos de log existam com tentativas curtas
    ferritine_log = log_dir / "ferritine.log"
    errors_log = log_dir / "errors.log"
    timeout = 1.0  # segundos totais de espera
    interval = 0.05  # intervalo entre tentativas
    end = time.time() + timeout
    while time.time() < end:
        if log_dir.exists() and ferritine_log.exists() and errors_log.exists():
            break
        time.sleep(interval)
    # Verificar arquivos
    assert log_dir.exists(), f"Diretório {log_dir} não existe"


    assert ferritine_log.exists(), f"Arquivo {ferritine_log} não criado"
    assert errors_log.exists(), f"Arquivo {errors_log} não criado"


def test_with_models():
    """Testa logging com os modelos de simulação."""
    cidade = Cidade()

    # Adicionar agentes
    for i in range(3):
        agente = Agente(f"Agente{i}", "Casa", f"Trabalho{i}")
        cidade.add_agente(agente)

    # Executar simulação
    for hora in [7, 12, 20]:
        cidade.step(hora)
        snapshot = cidade.snapshot()
        assert snapshot is not None


def test_log_content():
    """Verifica conteúdo do arquivo de log."""
    log_file = Path("data/logs/ferritine.log")

    assert log_file.exists(), f"Arquivo {log_file} não existe"

    with open(log_file, "r", encoding="utf-8") as f:
        content = f.read()

    # Verifica que houve logs escritos
    assert len(content) > 0, "Arquivo de log vazio"

