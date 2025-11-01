# 🐳 Docker Quick Start

Começar a usar Ferritine com Docker em 3 minutos!

## Pré-requisitos

- Docker instalado
- Docker Compose instalado

## Início Rápido

```bash
# 1. Clonar e entrar no diretório
git clone https://github.com/ferritine/ferritine.git
cd ferritine

# 2. Copiar configuração
cp .env.example .env

# 3. Iniciar tudo
chmod +x docker-manage.sh
./docker-manage.sh start

# Pronto! 🎉
```

## Comandos Essenciais

```bash
# Controle
./docker-manage.sh start     # Iniciar
./docker-manage.sh stop      # Parar  
./docker-manage.sh status    # Status
./docker-manage.sh logs      # Ver logs

# Banco de Dados
./docker-manage.sh init-db   # Inicializar
./docker-manage.sh seed-db   # Popular dados
./docker-manage.sh stats-db  # Estatísticas

# Debug
./docker-manage.sh exec      # Shell no container
./docker-manage.sh db        # PostgreSQL CLI

# Ajuda
./docker-manage.sh help      # Todos os comandos
```

## Acessos

- **PostgreSQL:** `localhost:5432`
- **PgAdmin:** `http://localhost:5050` (com `start-admin`)

## Documentação Completa

📖 Ver [docs/DOCKER_GUIDE.md](docs/DOCKER_GUIDE.md)

---

**Desenvolvido com** 🐳 **e** ❤️

