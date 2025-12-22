# 🐳 Guia Docker - Ferritine

Documentação completa para usar o Ferritine com Docker e PostgreSQL.

## 📋 Pré-requisitos

- **Docker** 20.10+ ([Instalar Docker](https://docs.docker.com/get-docker/))
- **Docker Compose** 2.0+ (geralmente incluído com Docker Desktop)
- **Git** (para clonar o repositório)

### Verificar Instalação

```bash
docker --version
docker compose version  # ou docker-compose --version
```

## 🚀 Quick Start

### 1. Preparar Ambiente

```bash
# Clonar repositório (se ainda não fez)
git clone https://github.com/ferritine/ferritine.git
cd ferritine

# Copiar variáveis de ambiente
cp .env.example .env

# (Opcional) Editar configurações
nano .env
```

### 2. Dar Permissão aos Scripts

```bash
chmod +x docker-manage.sh
chmod +x docker-entrypoint.sh
```

### 3. Iniciar Tudo

```bash
# Opção 1: Usando o script de gerenciamento (RECOMENDADO)
./docker-manage.sh start

# Opção 2: Usando docker-compose diretamente
docker compose up -d
```

### 4. Verificar Status

```bash
./docker-manage.sh status
```

**Pronto!** A aplicação está rodando com PostgreSQL.

## 🎮 Comandos Principais

### Script de Gerenciamento (`docker-manage.sh`)

O script `docker-manage.sh` simplifica todas as operações:

```bash
# CONTROLE BÁSICO
./docker-manage.sh start           # Iniciar containers
./docker-manage.sh stop            # Parar containers
./docker-manage.sh restart         # Reiniciar containers
./docker-manage.sh status          # Ver status

# COM PGADMIN (Interface Visual)
./docker-manage.sh start-admin     # Iniciar com PgAdmin
# Acessar: http://localhost:5050
# Login: admin@ferritine.local / admin

# LOGS E DEBUG
./docker-manage.sh logs            # Todos os logs
./docker-manage.sh logs app        # Logs da aplicação
./docker-manage.sh logs postgres   # Logs do PostgreSQL

# EXECUTAR COMANDOS
./docker-manage.sh exec            # Shell no container
./docker-manage.sh exec python main.py  # Executar script
./docker-manage.sh db              # PostgreSQL CLI (psql)
./docker-manage.sh test            # Executar testes

# BANCO DE DADOS
./docker-manage.sh init-db         # Inicializar banco
./docker-manage.sh seed-db         # Popular dados
./docker-manage.sh stats-db        # Ver estatísticas
./docker-manage.sh backup          # Backup do banco
./docker-manage.sh restore <file>  # Restaurar backup

# BUILD E RECONSTRUÇÃO
./docker-manage.sh build           # Construir imagem
./docker-manage.sh rebuild         # Reconstruir tudo

# LIMPEZA
./docker-manage.sh clean           # Remover containers
./docker-manage.sh clean-all       # Remover tudo (CUIDADO!)
./docker-manage.sh clean-images    # Remover imagens

# AJUDA
./docker-manage.sh help            # Ver todos os comandos
```

## 📦 Componentes do Sistema

### Containers

1. **ferritine-postgres**
   - PostgreSQL 15 Alpine
   - Porta: 5432
   - Volume: `ferritine-postgres-data`
   - Healthcheck automático

2. **ferritine-app**
   - Aplicação Python
   - Depende do PostgreSQL
   - Auto-inicializa banco se vazio

3. **ferritine-pgadmin** (opcional)
   - Interface web para PostgreSQL
   - Porta: 5050
   - Ativar com `start-admin`

### Volumes

- `ferritine-postgres-data` - Dados do PostgreSQL (persistente)
- `ferritine-pgadmin-data` - Dados do PgAdmin (persistente)

### Network

- `ferritine-network` - Rede bridge interna

## 🔧 Configuração

### Variáveis de Ambiente (.env)

```env
# PostgreSQL
DB_HOST=postgres              # Nome do serviço no Docker
DB_PORT=5432
DB_NAME=ferritine
DB_USER=ferritine_user
DB_PASSWORD=ferritine_pass    # MUDE EM PRODUÇÃO!

# PgAdmin
PGADMIN_EMAIL=admin@ferritine.local
PGADMIN_PASSWORD=admin        # MUDE EM PRODUÇÃO!
PGADMIN_PORT=5050

# Aplicação
LOG_LEVEL=INFO
```

**⚠️ Produção:** Sempre mude as senhas padrão!

## 📊 Uso do PgAdmin

### Iniciar com PgAdmin

```bash
./docker-manage.sh start-admin
```

### Acessar PgAdmin

1. Abrir: http://localhost:5050
2. Login:
   - Email: `admin@ferritine.local`
   - Senha: `admin`

### Conectar ao PostgreSQL

No PgAdmin, adicionar servidor:

- **Name:** Ferritine
- **Host:** `postgres` (nome do container)
- **Port:** `5432`
- **Database:** `ferritine`
- **Username:** `ferritine_user`
- **Password:** `ferritine_pass`

## 💻 Exemplos de Uso

### Executar Simulação

```bash
# No container
./docker-manage.sh exec python main.py

# Ver logs em tempo real
./docker-manage.sh logs app
```

### Executar Demonstração do Banco

```bash
./docker-manage.sh exec python examples/database_demo.py
```

### Rodar Testes

```bash
# Todos os testes
./docker-manage.sh test

# Testes específicos
./docker-manage.sh exec pytest tests/unit/test_database.py -v
```

### Acessar PostgreSQL

```bash
# Abrir psql
./docker-manage.sh db

# Executar query
./docker-manage.sh db "SELECT COUNT(*) FROM agents;"

# Listar tabelas
./docker-manage.sh db "\dt"
```

### Shell Interativo

```bash
# Bash no container
./docker-manage.sh exec

# Depois dentro:
python
>>> from backend.database import session_scope
>>> with session_scope() as session:
...     # Seu código aqui
```

## 🔄 Workflows Comuns

### Desenvolvimento Local

```bash
# 1. Iniciar ambiente
./docker-manage.sh start

# 2. Desenvolver (editar código localmente)
# Os volumes montados refletem mudanças

# 3. Testar
./docker-manage.sh test

# 4. Ver logs
./docker-manage.sh logs app

# 5. Parar quando terminar
./docker-manage.sh stop
```

### Reconstruir Após Mudanças

```bash
# Se mudou dependências (requirements.txt)
./docker-manage.sh rebuild

# Ou manualmente:
./docker-manage.sh stop
./docker-manage.sh build
./docker-manage.sh start
```

### Resetar Banco de Dados

```bash
# Opção 1: Limpar e recriar
./docker-manage.sh clean-all
./docker-manage.sh start
./docker-manage.sh seed-db

# Opção 2: Manter containers, só recriar schema
./docker-manage.sh exec python scripts/init_database.py drop
./docker-manage.sh init-db
./docker-manage.sh seed-db
```

### Backup e Restore

```bash
# Criar backup
./docker-manage.sh backup
# Salvo em: backups/ferritine_backup_TIMESTAMP.sql

# Restaurar backup
./docker-manage.sh restore backups/ferritine_backup_20251101_120000.sql
```

## 🐛 Debug e Troubleshooting

### Ver Logs Detalhados

```bash
# Todos os serviços
./docker-manage.sh logs

# Serviço específico com follow
docker compose logs -f app
docker compose logs -f postgres
```

### Container Não Inicia

```bash
# Ver status e erros
./docker-manage.sh status
docker compose ps

# Ver logs de erro
./docker-manage.sh logs postgres
./docker-manage.sh logs app
```

### PostgreSQL Não Conecta

```bash
# Verificar se está saudável
docker compose ps

# Testar conexão manual
docker compose exec postgres pg_isready -U ferritine_user

# Ver logs do PostgreSQL
./docker-manage.sh logs postgres
```

### Aplicação Não Encontra Banco

```bash
# Verificar variáveis de ambiente
docker compose exec app env | grep DB_

# Testar conexão
docker compose exec app python -c "
from backend.database import get_engine
engine = get_engine()
print('Conectado:', engine)
"
```

### Limpar Tudo e Recomeçar

```bash
# Parar tudo
./docker-manage.sh stop

# Remover tudo (containers + volumes + dados)
./docker-manage.sh clean-all

# Remover imagens
./docker-manage.sh clean-images

# Reconstruir do zero
./docker-manage.sh build
./docker-manage.sh start
```

## 🔒 Segurança

### Produção

**SEMPRE mude as senhas padrão em produção:**

```env
# .env (produção)
DB_PASSWORD=<senha-forte-aleatória>
PGADMIN_PASSWORD=<senha-forte-aleatória>
```

### Não Expor Portas

Para não expor PostgreSQL externamente:

```yaml
# docker-compose.yml
services:
  postgres:
    # Remover ou comentar:
    # ports:
    #   - "5432:5432"
```

### Usar Secrets (Produção)

Considere usar Docker Secrets ou variáveis de ambiente seguras.

## 📈 Performance

### Configurar Resources

```yaml
# docker-compose.yml
services:
  postgres:
    deploy:
      resources:
        limits:
          cpus: '2'
          memory: 2G
        reservations:
          cpus: '1'
          memory: 1G
```

### Otimizar PostgreSQL

Editar `docker/postgres/init/01-init.sql`:

```sql
-- Configurações de performance
ALTER SYSTEM SET shared_buffers = '256MB';
ALTER SYSTEM SET effective_cache_size = '1GB';
ALTER SYSTEM SET maintenance_work_mem = '128MB';
```

## 🌍 Ambientes

### Development

```bash
# .env
LOG_LEVEL=DEBUG
DB_ECHO=True
```

### Production

```bash
# .env
LOG_LEVEL=WARNING
DB_ECHO=False
# + Senhas fortes
```

## 📚 Referências

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)
- [PostgreSQL Docker](https://hub.docker.com/_/postgres)
- [PgAdmin Docker](https://www.pgadmin.org/docs/pgadmin4/latest/container_deployment.html)

## ❓ FAQ

**Q: Os dados são preservados quando paro os containers?**  
A: Sim, os dados ficam nos volumes Docker (`ferritine-postgres-data`).

**Q: Como acessar o banco de fora do Docker?**  
A: Use `localhost:5432` com as credenciais do `.env`.

**Q: Posso usar SQLite ao invés de PostgreSQL?**  
A: Sim, para desenvolvimento local sem Docker, use SQLite (ver DATABASE_GUIDE.md).

**Q: Como fazer backup automático?**  
A: Configure um cron job:
```bash
# Crontab
0 2 * * * cd /path/to/ferritine && ./docker-manage.sh backup
```

**Q: Como atualizar a imagem?**  
A: `./docker-manage.sh rebuild`

---

**🎉 Pronto para usar Ferritine com Docker!**

Para mais informações:
- `./docker-manage.sh help` - Ver todos os comandos
- `docs/DATABASE_GUIDE.md` - Guia do banco de dados
- `docs/QUICKSTART_DATABASE.md` - Quick start do banco

