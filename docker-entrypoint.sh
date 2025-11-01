#!/bin/bash
set -e

# Script de entrada para container Docker
# Aguarda PostgreSQL estar pronto e inicializa o banco

echo "🐳 Ferritine Docker Entrypoint"

# Aguardar PostgreSQL estar pronto
if [ -n "$DB_HOST" ]; then
    echo "⏳ Aguardando PostgreSQL em $DB_HOST:$DB_PORT..."

    max_attempts=30
    attempt=0

    until PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U "$DB_USER" -d "$DB_NAME" -c '\q' 2>/dev/null || [ $attempt -eq $max_attempts ]; do
        attempt=$((attempt + 1))
        echo "   Tentativa $attempt/$max_attempts..."
        sleep 2
    done

    if [ $attempt -eq $max_attempts ]; then
        echo "❌ Erro: PostgreSQL não respondeu após $max_attempts tentativas"
        exit 1
    fi

    echo "✅ PostgreSQL está pronto!"

    # Verificar se banco precisa ser inicializado
    echo "🔍 Verificando estado do banco de dados..."

    table_count=$(PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U "$DB_USER" -d "$DB_NAME" -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'public';" 2>/dev/null | tr -d ' ')

    if [ "$table_count" = "0" ]; then
        echo "📦 Banco de dados vazio. Inicializando..."
        python scripts/init_database.py init
        echo "🌱 Populando com dados iniciais..."
        python scripts/init_database.py seed
        echo "✅ Banco de dados inicializado com sucesso!"
    else
        echo "✅ Banco de dados já inicializado ($table_count tabelas encontradas)"
    fi
fi

# Executar comando passado como argumento
echo "🚀 Iniciando aplicação..."
echo "   Comando: $@"
exec "$@"

