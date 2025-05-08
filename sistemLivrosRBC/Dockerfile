# Usando a imagem oficial do PostgreSQL
FROM postgres:latest

# Definindo as variáveis de ambiente para o PostgreSQL
ENV POSTGRES_USER=postgres
ENV POSTGRES_PASSWORD=root
ENV POSTGRES_DB=sistemarbc

# Copiar o arquivo SQL para dentro do container
COPY ./query-inicial.sql /docker-entrypoint-initdb.d/

# Configuração personalizada do PostgreSQL (sem o arquivo bancodedados.cnf)
RUN echo "listen_addresses = '*'" >> /usr/share/postgresql/postgresql.conf.sample && \
    echo "max_connections = 100" >> /usr/share/postgresql/postgresql.conf.sample && \
    echo "shared_buffers = 128MB" >> /usr/share/postgresql/postgresql.conf.sample && \
    echo "work_mem = 4MB" >> /usr/share/postgresql/postgresql.conf.sample && \
    echo "maintenance_work_mem = 64MB" >> /usr/share/postgresql/postgresql.conf.sample

# Expondo a porta padrão do PostgreSQL
EXPOSE 5432
