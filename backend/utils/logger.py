"""
Sistema de logging centralizado para o projeto Ferritine.

Fornece um logger configurado com múltiplos handlers:
- Console: INFO e acima
- Arquivo: DEBUG e acima (com rotação)
- Arquivo de Erros: ERROR e acima (com rotação)

Exemplo de uso:
    from backend.utils.logger import get_logger

    logger = get_logger(__name__)
    logger.debug("Criando agente: %s", agent_id)
    logger.info("Simulação iniciada")
    logger.warning("Possível problema detectado")
    logger.error("Erro ao processar: %s", error)
    logger.critical("Falha crítica do sistema")
"""

import logging
from logging.handlers import RotatingFileHandler
from pathlib import Path

# Flag para garantir que a configuração global seja feita apenas uma vez
_configured = False


class FerritineLogger:
    """
    Wrapper para logging centralizado do Ferritine.

    Delega para um logger filho que herda configuração do logger pai 'ferritine'.
    Garante que todos os módulos compartilhem a mesma configuração.
    """

    def __init__(self, name: str, log_dir: str = "data/logs"):
        """
        Inicializa o logger para um módulo específico.

        Args:
            name: Nome do módulo (tipicamente __name__)
            log_dir: Diretório para armazenar logs (padrão: data/logs)
        """
        self.name = name
        self.log_dir = Path(log_dir)
        self.log_dir.mkdir(parents=True, exist_ok=True)

        # Configura logger global na primeira inicialização
        self._ensure_configured()

        # Obtém logger filho que propaga para 'ferritine'
        self._logger = logging.getLogger(f"ferritine.{name}")
        self._logger.setLevel(logging.DEBUG)

    def _ensure_configured(self) -> None:
        """Configura o logger raiz 'ferritine' na primeira vez."""
        global _configured
        if _configured:
            return

        # Logger raiz 'ferritine'
        base_logger = logging.getLogger("ferritine")
        base_logger.setLevel(logging.DEBUG)
        base_logger.propagate = False

        # Formatadores
        fmt_standard = logging.Formatter(
            "[%(asctime)s] [%(levelname)s] [%(name)s] %(message)s",
            datefmt="%Y-%m-%d %H:%M:%S"
        )
        fmt_detailed = logging.Formatter(
            "[%(asctime)s] [%(levelname)s] [%(name)s:%(lineno)d] %(message)s",
            datefmt="%Y-%m-%d %H:%M:%S"
        )

        # Handler: Console (INFO+)
        console_handler = logging.StreamHandler()
        console_handler.setLevel(logging.INFO)
        console_handler.setFormatter(fmt_standard)
        base_logger.addHandler(console_handler)

        # Handler: Arquivo geral (DEBUG+) com rotação
        file_path = self.log_dir / "ferritine.log"
        file_handler = RotatingFileHandler(
            filename=str(file_path),
            maxBytes=10 * 1024 * 1024,  # 10MB
            backupCount=5,
            encoding="utf-8"
        )
        file_handler.setLevel(logging.DEBUG)
        file_handler.setFormatter(fmt_detailed)
        base_logger.addHandler(file_handler)

        # Handler: Arquivo de erros (ERROR+) com rotação
        error_path = self.log_dir / "errors.log"
        error_handler = RotatingFileHandler(
            filename=str(error_path),
            maxBytes=10 * 1024 * 1024,  # 10MB
            backupCount=5,
            encoding="utf-8"
        )
        error_handler.setLevel(logging.ERROR)
        error_handler.setFormatter(fmt_detailed)
        base_logger.addHandler(error_handler)

        _configured = True

    def debug(self, message: str, *args, **kwargs) -> None:
        """Log de nível DEBUG."""
        self._logger.debug(message, *args, **kwargs)

    def info(self, message: str, *args, **kwargs) -> None:
        """Log de nível INFO."""
        self._logger.info(message, *args, **kwargs)

    def warning(self, message: str, *args, **kwargs) -> None:
        """Log de nível WARNING."""
        self._logger.warning(message, *args, **kwargs)

    def error(self, message: str, *args, **kwargs) -> None:
        """Log de nível ERROR."""
        self._logger.error(message, *args, **kwargs)

    def critical(self, message: str, *args, **kwargs) -> None:
        """Log de nível CRITICAL."""
        self._logger.critical(message, *args, **kwargs)


def get_logger(name: str) -> FerritineLogger:
    """
    Factory function para obter um logger configurado.

    Args:
        name: Nome do módulo (use __name__)

    Returns:
        Instância de FerritineLogger configurada

    Exemplo:
        from backend.utils.logger import get_logger
        logger = get_logger(__name__)
        logger.info("Olá, mundo!")
    """
    return FerritineLogger(name)


# Configuração compatível com logging.config.dictConfig
# Mantida para referência e possível integração futura
LOGGING_CONFIG = {
    'version': 1,
    'disable_existing_loggers': False,
    'formatters': {
        'standard': {
            'format': '[%(asctime)s] [%(levelname)s] [%(name)s] %(message)s',
            'datefmt': '%Y-%m-%d %H:%M:%S'
        },
        'detailed': {
            'format': '[%(asctime)s] [%(levelname)s] [%(name)s:%(lineno)d] %(message)s',
            'datefmt': '%Y-%m-%d %H:%M:%S'
        }
    },
    'handlers': {
        'console': {
            'level': 'INFO',
            'class': 'logging.StreamHandler',
            'formatter': 'standard'
        },
        'file': {
            'level': 'DEBUG',
            'class': 'logging.handlers.RotatingFileHandler',
            'filename': 'data/logs/ferritine.log',
            'maxBytes': 10485760,  # 10MB
            'backupCount': 5,
            'formatter': 'detailed'
        },
        'error_file': {
            'level': 'ERROR',
            'class': 'logging.handlers.RotatingFileHandler',
            'filename': 'data/logs/errors.log',
            'maxBytes': 10485760,  # 10MB
            'backupCount': 5,
            'formatter': 'detailed'
        }
    },
    'loggers': {
        'ferritine': {
            'level': 'DEBUG',
            'handlers': ['console', 'file', 'error_file'],
            'propagate': False
        }
    }
}

