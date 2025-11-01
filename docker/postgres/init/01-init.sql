-- Script de inicialização do PostgreSQL
-- Executado automaticamente quando o container é criado

-- Criar extensões úteis
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Configurar timezone
SET timezone = 'America/Sao_Paulo';

-- Mensagem de inicialização
DO $$
BEGIN
    RAISE NOTICE '✅ Banco de dados Ferritine inicializado com sucesso!';
    RAISE NOTICE '   Database: %', current_database();
    RAISE NOTICE '   User: %', current_user;
    RAISE NOTICE '   Timezone: %', current_setting('TIMEZONE');
END $$;

