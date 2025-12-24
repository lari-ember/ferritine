# Sistema de Logging do Ferritine

## Visão Geral

O Ferritine utiliza um sistema de logging centralizado e profissional baseado no módulo `logging` padrão do Python. O sistema fornece múltiplos níveis de log, rotação automática de arquivos e saída formatada em console e arquivos.

## Configuração Automática

O sistema de logging é automaticamente configurado na primeira vez que um logger é obtido. Não é necessária nenhuma configuração manual.

```python
from backend.utils.logger import get_logger

# Primeira chamada configura o sistema globalmente
logger = get_logger(__name__)
logger.info("Sistema iniciado")
```

## Uso Básico

### Importar e obter um logger

```python
from backend.utils.logger import get_logger

# Use __name__ para que o logger identifique o módulo corretamente
logger = get_logger(__name__)
```

### Registrar mensagens em diferentes níveis

```python
# DEBUG - Informações detalhadas de diagnóstico
logger.debug("Variável x = %d", x)

# INFO - Confirmação de eventos
logger.info("Simulação iniciada com %d agentes", num_agentes)

# WARNING - Indicação de possíveis problemas
logger.warning("Agente %s não encontrado", agent_id)

# ERROR - Erro que não impede a execução
logger.error("Falha ao carregar configuração: %s", error_msg)

# CRITICAL - Erro que pode parar a execução
logger.critical("Banco de dados inacessível, interrompendo")
```

## Estrutura de Logs

### Níveis de Log e Destinos

| Nível | Console | Arquivo | Arquivo Erros |
|-------|---------|---------|---------------|
| DEBUG | ❌ | ✅ | ❌ |
| INFO | ✅ | ✅ | ❌ |
| WARNING | ✅ | ✅ | ❌ |
| ERROR | ✅ | ✅ | ✅ |
| CRITICAL | ✅ | ✅ | ✅ |

### Formatos

#### Console (simples)
```
[2025-10-31 14:30:45] [INFO] [ferritine.simulation.models.agente] Simulação iniciada
```

#### Arquivos (detalhado)
```
[2025-10-31 14:30:45] [DEBUG] [ferritine.simulation.models.agente:42] Criando agente: João
```

## Arquivos de Log

Os logs são salvos em `data/logs/`:

- **ferritine.log** - Log geral com todos os níveis (DEBUG+)
  - Tamanho máximo: 10MB
  - Backup automático: 5 arquivos rotacionados

- **errors.log** - Log apenas de erros (ERROR+)
  - Tamanho máximo: 10MB
  - Backup automático: 5 arquivos rotacionados

Quando um arquivo atinge 10MB, é automaticamente rotacionado:
- `ferritine.log` → `ferritine.log.1`
- `ferritine.log.1` → `ferritine.log.2`
- etc., mantendo no máximo 5 backups

## Exemplo Prático: Sistema de Simulação

### Módulo agente.py

```python
from backend.utils.logger import get_logger

logger = get_logger(__name__)

class Agente:
    def __init__(self, nome: str, casa: str, trabalho: str):
        self.nome = nome
        self.casa = casa
        self.trabalho = trabalho
        self.local = casa
        logger.debug("Criando agente: %s (casa=%s, trabalho=%s)", nome, casa, trabalho)
    
    def step(self, hora: int):
        if 7 <= hora < 17:
            self.local = self.trabalho
        else:
            self.local = self.casa
        logger.debug("Agente %s movido para %s (hora=%d)", self.nome, self.local, hora)
```

### Módulo cidade.py

```python
from backend.utils.logger import get_logger

logger = get_logger(__name__)

class Cidade:
    def __init__(self, agentes=None):
        self.agentes = agentes or []
        logger.info("Cidade criada com %d agentes", len(self.agentes))
    
    def add_agente(self, agente):
        self.agentes.append(agente)
        logger.debug("Agente %s adicionado à cidade", agente.nome)
    
    def step(self, hora: int):
        logger.debug("Processando step para hora %d com %d agentes", hora, len(self.agentes))
        for agente in self.agentes:
            agente.step(hora)
```

### Script de simulação

```python
from backend.utils.logger import get_logger
from backend.simulation.models.agente import Agente
from backend.simulation.models.cidade import Cidade

logger = get_logger(__name__)

try:
    logger.info("Iniciando simulação")
    
    cidade = Cidade()
    logger.info("Cidade criada")
    
    # Criar agentes
    for i in range(10):
        agente = Agente(f"Agente{i}", "Casa", f"Trabalho{i}")
        cidade.add_agente(agente)
    
    logger.info("Agentes criados: %d", len(cidade.agentes))
    
    # Simular 24 horas
    for hora in range(24):
        cidade.step(hora)
    
    logger.info("Simulação concluída com sucesso")
    
except Exception as e:
    logger.error("Erro durante simulação: %s", str(e), exc_info=True)
    logger.critical("Simulação falhou")
```

## Boas Práticas

### 1. Use formatação com placeholders

❌ Evite concatenação:
```python
logger.info("Agente " + nome + " criado")
```

✅ Use formatação:
```python
logger.info("Agente %s criado", nome)
```

### 2. Escolha o nível correto

- **DEBUG**: Informações durante desenvolvimento
- **INFO**: Marcos importantes da execução
- **WARNING**: Situações incomuns mas esperadas
- **ERROR**: Erros que permitem continuação
- **CRITICAL**: Falhas que podem parar o sistema

### 3. Inclua contexto suficiente

❌ Vago:
```python
logger.error("Erro")
```

✅ Informativo:
```python
logger.error("Falha ao processar agente %s na hora %d: %s", agent_id, hora, str(e))
```

### 4. Use exc_info para exceções

```python
try:
    # código
except Exception as e:
    logger.error("Erro processando: %s", str(e), exc_info=True)
```

## Verificando os Logs

### Em tempo de execução (Console)

```
[2025-10-31 14:30:45] [INFO] [ferritine.main] Simulação iniciada
[2025-10-31 14:30:46] [INFO] [ferritine.simulation.models.cidade] Cidade criada com 10 agentes
```

### Após execução (Arquivos)

```bash
# Ver últimas 50 linhas do log geral
tail -50 data/logs/ferritine.log

# Ver logs de erro
cat data/logs/errors.log

# Buscar por um agente específico
grep "Agente5" data/logs/ferritine.log
```

## Configuração Avançada

### Usando dictConfig (opcional)

Para integração com frameworks, a configuração pode ser aplicada com:

```python
import logging.config
from backend.utils.logger import LOGGING_CONFIG

logging.config.dictConfig(LOGGING_CONFIG)
```

### Mudar diretório de logs

```python
from backend.utils.logger import get_logger

logger = get_logger("meu_modulo", log_dir="/caminho/customizado/logs")
```

## Solução de Problemas

### Logs não aparecem no console

- Verifique se você está usando `logger.info()` ou superior
- DEBUG e mensagens muito verbosas não aparecem no console, apenas em arquivo

### Arquivo de log não é criado

- Certifique-se que `data/logs/` existe ou possui permissão de criação
- O diretório é criado automaticamente se não existir

### Muitos arquivos rotacionados

- Os últimos 5 backups são mantidos
- Arquivos antigos são sobrescritos
- Isso é comportamento esperado

## Estrutura de Módulos

O sistema de logging é implementado em:

```
backend/
└── utils/
    └── logger.py          # Sistema centralizado de logging
```

É utilizado por:

```
backend/simulation/models/
├── agente.py             # Logging de criação e movimento de agentes
└── cidade.py             # Logging de operações da cidade
```

## Adicionando Logs a Novos Módulos

Para adicionar logging a qualquer módulo:

1. Importe o logger:
```python
from backend.utils.logger import get_logger

logger = get_logger(__name__)
```

2. Registre eventos importantes:
```python
logger.debug("Detalhes de diagnóstico")
logger.info("Marco importante")
logger.error("Erro tratado")
```

3. Verifique os logs em `data/logs/`

## Próximas Melhorias

- [ ] Integração com sistema de configuração para controlar níveis dinamicamente
- [ ] Logs estruturados em JSON para análise
- [ ] Dashboard de visualização de logs em tempo real
- [ ] Filtros e alertas automáticos para padrões de erro

