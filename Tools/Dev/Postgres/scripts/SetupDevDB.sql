CREATE ROLE gateway WITH
  LOGIN
  INHERIT
  NOCREATEDB
  NOCREATEROLE
  PASSWORD 'passw0rd';

CREATE DATABASE gateway WITH 
  OWNER = gateway
  ENCODING = 'UTF8'
  LC_COLLATE = 'en_US.UTF-8'
  LC_CTYPE = 'en_US.UTF-8'
  TABLESPACE = pg_default
  CONNECTION LIMIT = -1
  TEMPLATE template0;

\c gateway
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";