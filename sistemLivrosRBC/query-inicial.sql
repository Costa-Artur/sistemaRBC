-- Alterando a tabela Livros para ter livros diferentes
CREATE TABLE Livros(
                       LivroID SERIAL PRIMARY KEY,
                       Titulo VARCHAR(255) NOT NULL,
                       Genero VARCHAR(255) NOT NULL,
                       Tema VARCHAR(255) NOT NULL,
                       NumeroDePaginas INT NOT NULL,
                       AnoDeLancamento INT NOT NULL,
                       GeneroProtagonista VARCHAR(100) NOT NULL,
                       IdiomaOriginal VARCHAR(100) NOT NULL,
                       ComplexidadeDaLinguagem VARCHAR(100) NOT NULL,
                       Populariedade VARCHAR(100) NOT NULL,
                       PublicoAlvo VARCHAR(100) NOT NULL,
                       Narrador VARCHAR(100) NOT NULL
);

-- Inserindo livros
INSERT INTO Livros(Titulo, Genero, Tema, NumeroDePaginas, AnoDeLancamento, GeneroProtagonista, IdiomaOriginal,
                   ComplexidadeDaLinguagem, Populariedade, PublicoAlvo, Narrador)
VALUES
    ('Moby Dick', 'Aventura', 'Aventura no Mar', 635, 1851, 'M', 'Inglês', 'Alta', 'Popular', 'Adulto', 'Narrador Onisciente'),
    ('O Grande Gatsby', 'Romance', 'Sociedade e Amor', 218, 1925, 'M', 'Inglês', 'Moderada', 'Muito Popular', 'Adulto', 'Primeira Pessoa'),
    ('A Hora da Estrela', 'Drama', 'Sofrimento e Existência', 120, 1986, 'F', 'Português', 'Alta', 'Popular', 'Adulto', 'Narrador Onisciente'),
    ('O Senhor dos Anéis', 'Fantasia', 'Batalha contra o Mal', 1178, 1954, 'M', 'Inglês', 'Moderada', 'Muito Popular', 'Jovem Adulto', 'Narrador Onisciente'),
    ('A Culpa é das Estrelas', 'Romance', 'Câncer e Amor', 313, 2012, 'F', 'Inglês', 'Simples', 'Muito Popular', 'Jovem Adulto', 'Primeira Pessoa'),
    ('Harry Potter e a Pedra Filosofal', 'Fantasia', 'Magia e Aventura', 309, 1997, 'M', 'Inglês', 'Moderada', 'Muito Popular', 'Jovem Adulto', 'Narrador Onisciente');
