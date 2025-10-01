-- Initialize PostgreSQL database and user for production compose
-- Note: POSTGRES_USER/POSTGRES_PASSWORD/POSTGRES_DB are already set via env
-- This script is idempotent and only creates missing objects

DO
$$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'vkord') THEN
        PERFORM dblink_exec('dbname=' || current_database(), ''); -- ensure dblink exists if needed
        EXECUTE 'CREATE DATABASE vkord OWNER ' || quote_ident(COALESCE(current_setting('POSTGRES_USER', true), 'vkord_user'));
    END IF;
END
$$;

-- Grant privileges if user exists
DO
$$
DECLARE
    v_user text := COALESCE(current_setting('POSTGRES_USER', true), 'vkord_user');
BEGIN
    IF EXISTS (SELECT FROM pg_roles WHERE rolname = v_user) THEN
        EXECUTE 'GRANT ALL PRIVILEGES ON DATABASE vkord TO ' || quote_ident(v_user);
    END IF;
END
$$;


