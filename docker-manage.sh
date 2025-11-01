#!/bin/bash

# 🐳 Ferritine Docker Manager
# Script completo para gerenciar containers Docker

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Funções auxiliares
print_header() {
    echo -e "${CYAN}╔════════════════════════════════════════════════╗${NC}"
    echo -e "${CYAN}║       🐳 FERRITINE DOCKER MANAGER 🐳          ║${NC}"
    echo -e "${CYAN}╚════════════════════════════════════════════════╝${NC}"
    echo ""
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

print_step() {
    echo -e "${PURPLE}➜ $1${NC}"
}

# Verificar se Docker está instalado
check_docker() {
    if ! command -v docker &> /dev/null; then
        print_error "Docker não está instalado!"
        echo "Instale o Docker: https://docs.docker.com/get-docker/"
        exit 1
    fi

    if ! command -v docker-compose &> /dev/null && ! docker compose version &> /dev/null; then
        print_error "Docker Compose não está instalado!"
        echo "Instale o Docker Compose: https://docs.docker.com/compose/install/"
        exit 1
    fi
}

# Detectar comando docker-compose
get_compose_cmd() {
    if docker compose version &> /dev/null; then
        echo "docker compose"
    else
        echo "docker-compose"
    fi
}

# Iniciar containers
start() {
    print_step "Iniciando containers..."

    COMPOSE_CMD=$(get_compose_cmd)

    # Verificar se .env existe
    if [ ! -f .env ]; then
        print_warning "Arquivo .env não encontrado. Criando a partir de .env.example..."
        cp .env.example .env
        print_info "Edite o arquivo .env se necessário"
    fi

    # Iniciar containers
    $COMPOSE_CMD up -d

    print_success "Containers iniciados!"
    echo ""
    print_info "Aguardando PostgreSQL inicializar..."
    sleep 5

    # Mostrar status
    status
}

# Iniciar com PgAdmin
start_with_admin() {
    print_step "Iniciando containers com PgAdmin..."

    COMPOSE_CMD=$(get_compose_cmd)

    if [ ! -f .env ]; then
        print_warning "Arquivo .env não encontrado. Criando a partir de .env.example..."
        cp .env.example .env
    fi

    $COMPOSE_CMD --profile admin up -d

    print_success "Containers iniciados com PgAdmin!"
    echo ""
    print_info "PgAdmin disponível em: http://localhost:5050"
    print_info "Email padrão: admin@ferritine.local"
    print_info "Senha padrão: admin"
    echo ""

    status
}

# Parar containers
stop() {
    print_step "Parando containers..."

    COMPOSE_CMD=$(get_compose_cmd)
    $COMPOSE_CMD stop

    print_success "Containers parados!"
}

# Reiniciar containers
restart() {
    print_step "Reiniciando containers..."
    stop
    sleep 2
    start
}

# Ver logs
logs() {
    COMPOSE_CMD=$(get_compose_cmd)

    if [ -n "$1" ]; then
        print_step "Logs do serviço: $1"
        $COMPOSE_CMD logs -f "$1"
    else
        print_step "Logs de todos os serviços"
        $COMPOSE_CMD logs -f
    fi
}

# Status dos containers
status() {
    print_step "Status dos containers:"
    echo ""

    COMPOSE_CMD=$(get_compose_cmd)
    $COMPOSE_CMD ps

    echo ""
    print_info "Informações de conexão:"
    echo "  PostgreSQL: localhost:5432"
    echo "  Database: ferritine"
    echo "  User: ferritine_user"
    echo "  Password: (ver .env)"
}

# Executar comando no container da aplicação
exec_app() {
    COMPOSE_CMD=$(get_compose_cmd)

    if [ -z "$1" ]; then
        print_step "Abrindo shell no container da aplicação..."
        $COMPOSE_CMD exec app /bin/bash
    else
        print_step "Executando comando: $@"
        $COMPOSE_CMD exec app "$@"
    fi
}

# Executar comando no PostgreSQL
exec_db() {
    COMPOSE_CMD=$(get_compose_cmd)

    if [ -z "$1" ]; then
        print_step "Abrindo psql no PostgreSQL..."
        $COMPOSE_CMD exec postgres psql -U ferritine_user -d ferritine
    else
        print_step "Executando SQL: $@"
        $COMPOSE_CMD exec postgres psql -U ferritine_user -d ferritine -c "$@"
    fi
}

# Inicializar banco de dados
init_db() {
    print_step "Inicializando banco de dados..."

    COMPOSE_CMD=$(get_compose_cmd)
    $COMPOSE_CMD exec app python scripts/init_database.py init

    print_success "Banco de dados inicializado!"
}

# Popular banco com dados
seed_db() {
    print_step "Populando banco de dados..."

    COMPOSE_CMD=$(get_compose_cmd)
    $COMPOSE_CMD exec app python scripts/init_database.py seed

    print_success "Dados inseridos com sucesso!"
}

# Ver estatísticas do banco
stats_db() {
    print_step "Estatísticas do banco de dados:"
    echo ""

    COMPOSE_CMD=$(get_compose_cmd)
    $COMPOSE_CMD exec app python scripts/init_database.py stats
}

# Executar testes
run_tests() {
    print_step "Executando testes..."

    COMPOSE_CMD=$(get_compose_cmd)
    $COMPOSE_CMD exec app pytest tests/ -v
}

# Build da imagem
build() {
    print_step "Construindo imagem Docker..."

    COMPOSE_CMD=$(get_compose_cmd)
    $COMPOSE_CMD build --no-cache

    print_success "Imagem construída!"
}

# Reconstruir e reiniciar
rebuild() {
    print_step "Reconstruindo e reiniciando..."
    stop
    build
    start
}

# Limpar containers
clean() {
    print_warning "Isso irá PARAR e REMOVER os containers!"
    read -p "Continuar? (y/N): " -n 1 -r
    echo

    if [[ $REPLY =~ ^[Yy]$ ]]; then
        print_step "Removendo containers..."

        COMPOSE_CMD=$(get_compose_cmd)
        $COMPOSE_CMD down

        print_success "Containers removidos!"
    else
        print_info "Operação cancelada"
    fi
}

# Limpar tudo (incluindo volumes)
clean_all() {
    print_error "ATENÇÃO: Isso irá REMOVER containers, volumes E DADOS!"
    print_warning "Todos os dados do banco serão PERDIDOS!"
    read -p "Tem certeza? Digite 'CONFIRMAR': " -r
    echo

    if [[ $REPLY == "CONFIRMAR" ]]; then
        print_step "Removendo containers, volumes e dados..."

        COMPOSE_CMD=$(get_compose_cmd)
        $COMPOSE_CMD down -v

        # Limpar volumes órfãos
        docker volume prune -f

        print_success "Tudo removido!"
    else
        print_info "Operação cancelada"
    fi
}

# Limpar imagens
clean_images() {
    print_warning "Isso irá remover as imagens Docker do Ferritine!"
    read -p "Continuar? (y/N): " -n 1 -r
    echo

    if [[ $REPLY =~ ^[Yy]$ ]]; then
        print_step "Removendo imagens..."

        docker rmi ferritine-app:latest 2>/dev/null || true
        docker image prune -f

        print_success "Imagens removidas!"
    else
        print_info "Operação cancelada"
    fi
}

# Backup do banco
backup() {
    TIMESTAMP=$(date +%Y%m%d_%H%M%S)
    BACKUP_FILE="backups/ferritine_backup_$TIMESTAMP.sql"

    print_step "Criando backup do banco de dados..."

    mkdir -p backups

    COMPOSE_CMD=$(get_compose_cmd)
    $COMPOSE_CMD exec -T postgres pg_dump -U ferritine_user ferritine > "$BACKUP_FILE"

    print_success "Backup criado: $BACKUP_FILE"
}

# Restaurar backup
restore() {
    if [ -z "$1" ]; then
        print_error "Especifique o arquivo de backup"
        echo "Uso: $0 restore <arquivo.sql>"
        exit 1
    fi

    if [ ! -f "$1" ]; then
        print_error "Arquivo não encontrado: $1"
        exit 1
    fi

    print_warning "Isso irá SOBRESCREVER o banco de dados atual!"
    read -p "Continuar? (y/N): " -n 1 -r
    echo

    if [[ $REPLY =~ ^[Yy]$ ]]; then
        print_step "Restaurando backup: $1"

        COMPOSE_CMD=$(get_compose_cmd)
        $COMPOSE_CMD exec -T postgres psql -U ferritine_user ferritine < "$1"

        print_success "Backup restaurado!"
    else
        print_info "Operação cancelada"
    fi
}

# Menu de ajuda
show_help() {
    print_header
    echo "Uso: $0 <comando> [opções]"
    echo ""
    echo "COMANDOS DE CONTROLE:"
    echo "  start           - Iniciar containers"
    echo "  start-admin     - Iniciar com PgAdmin"
    echo "  stop            - Parar containers"
    echo "  restart         - Reiniciar containers"
    echo "  status          - Ver status dos containers"
    echo ""
    echo "COMANDOS DE DESENVOLVIMENTO:"
    echo "  logs [serviço]  - Ver logs (app, postgres, pgadmin)"
    echo "  exec            - Abrir shell no container da app"
    echo "  exec <cmd>      - Executar comando no container"
    echo "  db              - Abrir psql no PostgreSQL"
    echo "  db <sql>        - Executar SQL no PostgreSQL"
    echo "  test            - Executar testes"
    echo ""
    echo "COMANDOS DE BANCO DE DADOS:"
    echo "  init-db         - Inicializar banco de dados"
    echo "  seed-db         - Popular com dados iniciais"
    echo "  stats-db        - Ver estatísticas"
    echo "  backup          - Criar backup do banco"
    echo "  restore <file>  - Restaurar backup"
    echo ""
    echo "COMANDOS DE BUILD:"
    echo "  build           - Construir imagem Docker"
    echo "  rebuild         - Reconstruir e reiniciar"
    echo ""
    echo "COMANDOS DE LIMPEZA:"
    echo "  clean           - Remover containers"
    echo "  clean-all       - Remover containers E volumes (CUIDADO!)"
    echo "  clean-images    - Remover imagens Docker"
    echo ""
    echo "EXEMPLOS:"
    echo "  $0 start                    # Iniciar tudo"
    echo "  $0 logs app                 # Ver logs da aplicação"
    echo "  $0 exec python main.py      # Executar main.py"
    echo "  $0 db 'SELECT COUNT(*) FROM agents;'  # Query SQL"
    echo "  $0 backup                   # Fazer backup"
    echo ""
}

# Main
main() {
    check_docker

    case "$1" in
        start)
            print_header
            start
            ;;
        start-admin)
            print_header
            start_with_admin
            ;;
        stop)
            print_header
            stop
            ;;
        restart)
            print_header
            restart
            ;;
        logs)
            shift
            logs "$@"
            ;;
        status)
            print_header
            status
            ;;
        exec)
            shift
            exec_app "$@"
            ;;
        db)
            shift
            exec_db "$@"
            ;;
        init-db)
            print_header
            init_db
            ;;
        seed-db)
            print_header
            seed_db
            ;;
        stats-db)
            print_header
            stats_db
            ;;
        test)
            print_header
            run_tests
            ;;
        build)
            print_header
            build
            ;;
        rebuild)
            print_header
            rebuild
            ;;
        clean)
            print_header
            clean
            ;;
        clean-all)
            print_header
            clean_all
            ;;
        clean-images)
            print_header
            clean_images
            ;;
        backup)
            print_header
            backup
            ;;
        restore)
            print_header
            restore "$2"
            ;;
        help|--help|-h)
            show_help
            ;;
        *)
            show_help
            exit 1
            ;;
    esac
}

# Executar
main "$@"

