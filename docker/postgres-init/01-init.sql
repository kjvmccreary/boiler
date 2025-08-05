-- Enable required PostgreSQL extensions for Boiler
\c boiler_dev;
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Create additional databases for testing
CREATE DATABASE boiler_test;
CREATE DATABASE boiler_integration;

-- Log successful initialization
SELECT 'Boiler PostgreSQL initialization completed successfully!' as status;
