# TODO: Issue #2 - Configurar sistema de logging
# Labels: feat, phase-0: fundamentals, priority: high, complexity: beginner
# Milestone: Milestone 0: Fundamentos e Infraestrutura

"""
Implementar sistema de logging profissional para debug e monitoramento.

Tarefas:
- [ ] Criar backend/utils/logger.py
- [ ] Configurar diferentes níveis de log (DEBUG, INFO, WARNING, ERROR)
- [ ] Implementar rotação de logs
- [ ] Adicionar logs em módulos críticos (simulação, economia, transporte)
- [ ] Criar arquivo de configuração para logs
- [ ] Documentar como usar o sistema de logging

Critérios de Aceitação:
- Sistema de logging funcional
- Logs salvos em data/logs/
- Diferentes níveis de log configuráveis
- Rotação automática de arquivos de log
"""

import logging
from logging.handlers import RotatingFileHandler
import os
from pathlib import Path

# TODO: Criar classe Logger centralizada
class FerritineLogger:
    """
    Sistema de logging centralizado para o projeto Ferritine.
    
    Exemplo de uso:
        from backend.utils.logger import get_logger
        
        logger = get_logger(__name__)
        logger.info("Simulação iniciada")
        logger.debug("Agente criado: {}", agent_id)
        logger.error("Erro ao processar: {}", error)
    """
    
    def __init__(self, name: str, log_dir: str = "data/logs"):
        # TODO: Implementar inicialização
        self.name = name
        self.log_dir = Path(log_dir)
        self.log_dir.mkdir(parents=True, exist_ok=True)
        
        # TODO: Configurar handlers
        # - Console handler (INFO+)
        # - File handler (DEBUG+) com rotação
        # - Error file handler (ERROR+)
        
        # TODO: Configurar formatação
        # Formato: [2025-10-29 14:30:45] [INFO] [module] Mensagem
        pass
    
    # TODO: Implementar métodos
    def debug(self, message: str, *args):
        pass
    
    def info(self, message: str, *args):
        pass
    
    def warning(self, message: str, *args):
        pass
    
    def error(self, message: str, *args):
        pass
    
    def critical(self, message: str, *args):
        pass


# TODO: Criar função factory
def get_logger(name: str) -> FerritineLogger:
    """
    Retorna um logger configurado para o módulo especificado.
    
    Args:
        name: Nome do módulo (use __name__)
    
    Returns:
        Logger configurado
    """
    return FerritineLogger(name)


# TODO: Criar configuração de logging
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

# TODO: Adicionar logs em módulos existentes
# Exemplo para app/models/agente.py:
"""
from backend.utils.logger import get_logger

logger = get_logger(__name__)

class Agente:
    def __init__(self, nome, casa, trabalho):
        logger.debug(f"Criando agente: {nome}")
        # ... código existente ...
    
    def step(self, hora):
        logger.debug(f"Agente {self.nome} processando hora {hora}")
        # ... código existente ...
"""

# TODO: Criar documentação em docs/LOGGING_GUIDE.md

