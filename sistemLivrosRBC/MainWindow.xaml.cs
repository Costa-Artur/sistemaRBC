using System.Windows;
using System.Windows.Controls;
using Npgsql;

//Criar a imagem
//docker build -t meu-postgres-db .

//Rodar o container
//docker run --name postgres-container -d -p 5432:5432 meu-postgres-db

namespace sistemLivrosRBC
{
    public partial class MainWindow : Window
    {
        
        private string connString = "Host=127.0.0.1;Username=postgres;Password=root;Database=sistemarbc";
        
  // pesos fixos
        private double p_gen = 1;
        private double p_prot_gen = 0.5;
        private double p_tema = 0.7;
        private double p_comp_ling = 0.8;

        // pesos variáveis
        private double p_ano_lanc = 0;
        private double p_num_pag = 0;
        private double p_pop = 0;
        private double p_pub_alvo = 0;
        private double p_narracao = 0;
        private double p_idioma_original = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnRecomendar_Click(object sender, RoutedEventArgs e)
{
    // Verifique se os campos de texto têm valores válidos
    string genero = string.IsNullOrEmpty(generoTextBox.Text) ? "" : generoTextBox.Text;
    string tema = string.IsNullOrEmpty(temaTextBox.Text) ? "" : temaTextBox.Text;
    int numPag = string.IsNullOrEmpty(numPagTextBox.Text) ? 0 : Convert.ToInt32(numPagTextBox.Text);
    int ano = string.IsNullOrEmpty(anoLancamentoTextBox.Text) ? 0 : Convert.ToInt32(anoLancamentoTextBox.Text);
    
    // Verifique se o ComboBox tem um item selecionado
    string generoProt = generoProtComboBox.SelectedItem == null ? "" : generoProtComboBox.SelectedItem.ToString();
    string idioma = string.IsNullOrEmpty(idiomaTextBox.Text) ? "" : idiomaTextBox.Text;
    string complexidade = complexidadeComboBox.SelectedItem == null ? "" : complexidadeComboBox.SelectedItem.ToString();
    string popularidade = string.IsNullOrEmpty(popTextBox.Text) ? "" : popTextBox.Text;
    string publico = publicoComboBox.SelectedItem == null ? "" : publicoComboBox.SelectedItem.ToString();
    string narrador = narradorComboBox.SelectedItem == null ? "" : narradorComboBox.SelectedItem.ToString();

    // Pesos dos critérios (verifique se o valor é vazio e, caso contrário, defina como null)
    float? pAnoLanc = string.IsNullOrEmpty(pAnoLancamentoTextBox.Text) ? (float?)null : Convert.ToSingle(pAnoLancamentoTextBox.Text);
    float? pNumPag = string.IsNullOrEmpty(pNumPagTextBox.Text) ? (float?)null : Convert.ToSingle(pNumPagTextBox.Text);
    float? pPop = string.IsNullOrEmpty(pPopTextBox.Text) ? (float?)null : Convert.ToSingle(pPopTextBox.Text);
    float? pPubAlvo = string.IsNullOrEmpty(pPubAlvoTextBox.Text) ? (float?)null : Convert.ToSingle(pPubAlvoTextBox.Text);
    float? pNarracao = string.IsNullOrEmpty(pNarracaoTextBox.Text) ? (float?)null : Convert.ToSingle(pNarracaoTextBox.Text);

    // Chama a função RecomendarLivros com os valores
    RecomendarLivros(genero, tema, numPag, ano, generoProt, idioma, complexidade, popularidade, publico, narrador,
                      pAnoLanc, pNumPag, pPop, pPubAlvo, pNarracao);
}


        
        
        
public void RecomendarLivros(string genero, string tema, int numPag, int ano, string generoProt, string idioma, string complexidade, string popularidade, string publico, string narrador,
                                  float? pAnoLanc, float? pNumPag, float? pPop, float? pPubAlvo, float? pNarracao)
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
                    if (pAnoLanc.HasValue)
                        score += pAnoLanc.Value * (ano == Convert.ToInt32(reader["AnoDeLancamento"]) ? 1 : 0);
                    if (pNumPag.HasValue)
                        score += pNumPag.Value * (numPag >= Convert.ToInt32(reader["NumeroDePaginas"]) * 0.9 && numPag <= Convert.ToInt32(reader["NumeroDePaginas"]) * 1.1 ? 1 : 0);
                    if (pPop.HasValue)
                        score += pPop.Value * (reader["Populariedade"].ToString().ToLower() == popularidade.ToLower() ? 1 : 0);
                    if (pPubAlvo.HasValue)
                        score += pPubAlvo.Value * (reader["PublicoAlvo"].ToString().ToLower() == publico.ToLower() ? 1 : 0);
                    if (pNarracao.HasValue)
                        score += pNarracao.Value * (reader["Narrador"].ToString().ToLower() == narrador.ToLower() ? 1 : 0);

                    // Outros critérios que sempre serão considerados
                    score += (genero.ToLower() == reader["Genero"].ToString().ToLower() ? 1 : 0);
                    score += (tema.ToLower() == reader["Tema"].ToString().ToLower() ? 1 : 0);
                    score += (generoProt.ToLower() == reader["GeneroProtagonista"].ToString().ToLower() ? 1 : 0);
                    score += (idioma.ToLower() == reader["IdiomaOriginal"].ToString().ToLower() ? 1 : 0);
                    score += (complexidade.ToLower() == reader["ComplexidadeDaLinguagem"].ToString().ToLower() ? 1 : 0);

                    scores.Add(new Tuple<double, string>(score, reader["Titulo"].ToString()));
                }
            }
        }

        // Ordenar os livros por score
        var rankedBooks = scores.OrderByDescending(x => x.Item1).ToList();

        // Exibindo os resultados na ListBox
        livrosListBox.Items.Clear();
        foreach (var item in rankedBooks)
        {
            livrosListBox.Items.Add($"{item.Item2} - Score: {item.Item1:F2}");
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Erro ao buscar livros: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}

        
    }
}
