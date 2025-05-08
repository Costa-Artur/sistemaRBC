using System.Windows;
using Npgsql;

namespace sistemLivrosRBC;


public class SistemaRbc
{
    private string connString = "Host=127.0.0.1;Username=postgres;Password=root;Database=db";
    private double p_gen = 1, p_tema = 0.7, p_comp_ling = 0.8, p_prot_gen = 0.5, p_idioma_original = 0.6;
    
    public void InserirCaso(string titulo, string genero, string tema, int numPag, int ano, string generoProt, string idioma, string complexidade, string popularidade, string publico, string narrador)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connString))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand(@"INSERT INTO Livros(Titulo, Genero, Tema, NumeroDePaginas, AnoDeLancamento, GeneroProtagonista, IdiomaOriginal, ComplexidadeDaLinguagem, Populariedade, PublicoAlvo, Narrador)
                                                     VALUES (@titulo, @genero, @tema, @numPag, @ano, @generoProt, @idioma, @complexidade, @popularidade, @publico, @narrador)", connection))
                {
                    cmd.Parameters.AddWithValue("titulo", titulo);
                    cmd.Parameters.AddWithValue("genero", genero);
                    cmd.Parameters.AddWithValue("tema", tema);
                    cmd.Parameters.AddWithValue("numPag", numPag);
                    cmd.Parameters.AddWithValue("ano", ano);
                    cmd.Parameters.AddWithValue("generoProt", generoProt);
                    cmd.Parameters.AddWithValue("idioma", idioma);
                    cmd.Parameters.AddWithValue("complexidade", complexidade);
                    cmd.Parameters.AddWithValue("popularidade", popularidade);
                    cmd.Parameters.AddWithValue("publico", publico);
                    cmd.Parameters.AddWithValue("narrador", narrador);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Livro adicionado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao inserir no banco de dados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
    
  public void RecomendarLivros(string genero, string tema, int numPag, int ano, string generoProt, string idioma, string complexidade, string popularidade, string publico, string narrador,
                                  float pAnoLanc, float pNumPag, float pPop, float pPubAlvo, float pNarracao)
{
    try
    {
        List<Tuple<double, string>> scores = new List<Tuple<double, string>>();

        using (var connection = new NpgsqlConnection(connString))
        {
            connection.Open();

            using (var cmd = new NpgsqlCommand("SELECT * FROM Livros", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    double score = 0;

                    // Calculando o score para cada livro
                    score += pAnoLanc * (reader["AnoDeLancamento"].ToString() == ano.ToString() ? 1 : 0);
                    score += pNumPag * (numPag >= Convert.ToInt32(reader["NumeroDePaginas"]) * 0.9 && numPag <= Convert.ToInt32(reader["NumeroDePaginas"]) * 1.1 ? 1 : 0);
                    score += pPop * (reader["Populariedade"].ToString().ToLower() == popularidade.ToLower() ? 1 : 0);
                    score += pPubAlvo * (reader["PublicoAlvo"].ToString().ToLower() == publico.ToLower() ? 1 : 0);
                    score += pNarracao * (reader["Narrador"].ToString().ToLower() == narrador.ToLower() ? 1 : 0);
                    score += (reader["Genero"].ToString().ToLower() == genero.ToLower() ? 1 : 0);
                    score += (reader["Tema"].ToString().ToLower() == tema.ToLower() ? 1 : 0);

                    scores.Add(new Tuple<double, string>(score, reader["Titulo"].ToString()));
                }
            }
        }

        // Ordenar os livros por score
        var rankedBooks = scores.OrderByDescending(x => x.Item1).ToList();
        
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Erro ao buscar livros: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}

}