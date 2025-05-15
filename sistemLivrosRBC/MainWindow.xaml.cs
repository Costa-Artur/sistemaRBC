using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Npgsql;

namespace sistemLivrosRBC
{
    public partial class MainWindow : Window
    {
        private string connString = "Host=127.0.0.1;Username=postgres;Password=root;Database=sistemarbc";

        public MainWindow()
        {
            InitializeComponent();

            // Setar pesos padrão nos campos de peso (caso queira garantir)
            pGeneroTextBox.Text = "1";
            pPlataformaTextBox.Text = "0.8";
            pEstiloTextBox.Text = "0.6";
            pDesenvolvedoraTextBox.Text = "0.7";
        }

        private void BtnRecomendar_Click(object sender, RoutedEventArgs e)
        {
            // Ler atributos do jogo
            string genero = generoTextBox.Text.Trim();
            string plataforma = plataformaTextBox.Text.Trim();

            string faixaEtaria = (faixaEtariaComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "";
            string multiplayer = (multiplayerComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "";

            int ano = int.TryParse(anoLancamentoTextBox.Text.Trim(), out int tmpAno) ? tmpAno : 0;
            double avaliacao = double.TryParse(avaliacaoTextBox.Text.Trim(), out double tmpAvaliacao) ? tmpAvaliacao : 0;
            string estiloVisual = estiloVisualTextBox.Text.Trim();
            string desenvolvedora = desenvolvedoraTextBox.Text.Trim();

            // Ler pesos (fixos e variáveis)
            double p_gen = double.TryParse(pGeneroTextBox.Text.Trim(), out double tmpPgen) ? tmpPgen : 1;
            double p_plat = double.TryParse(pPlataformaTextBox.Text.Trim(), out double tmpPplat) ? tmpPplat : 0.8;
            double p_estilo = double.TryParse(pEstiloTextBox.Text.Trim(), out double tmpPestilo) ? tmpPestilo : 0.6;
            double p_dev = double.TryParse(pDesenvolvedoraTextBox.Text.Trim(), out double tmpPdev) ? tmpPdev : 0.7;

            double pAnoLanc = double.TryParse(pAnoLancamentoTextBox.Text.Trim(), out double tmpPAno) ? tmpPAno : 0;
            double pDuracao = double.TryParse(pDuracaoTextBox.Text.Trim(), out double tmpPDuracao) ? tmpPDuracao : 0;
            double pAvaliacao = double.TryParse(pAvaliacaoTextBox.Text.Trim(), out double tmpPAval) ? tmpPAval : 0;
            double pFaixaEtaria = double.TryParse(pFaixaEtariaTextBox.Text.Trim(), out double tmpPFaixa) ? tmpPFaixa : 0;
            double pMultiplayer = double.TryParse(pMultiplayerTextBox.Text.Trim(), out double tmpPMulti) ? tmpPMulti : 0;

            RecomendarJogos(genero, plataforma, faixaEtaria, multiplayer,
                            ano, avaliacao, estiloVisual, desenvolvedora,
                            p_gen, p_plat, p_estilo, p_dev,
                            pAnoLanc, pDuracao, pAvaliacao, pFaixaEtaria, pMultiplayer);
        }

        private void RecomendarJogos(string genero, string plataforma, string faixaEtaria, string multiplayer,
                                     int ano, double avaliacao, string estiloVisual, string desenvolvedora,
                                     double p_gen, double p_plat, double p_estilo, double p_dev,
                                     double pAnoLanc, double pDuracao, double pAvaliacao, double pFaixaEtaria, double pMultiplayer)
        {
            try
            {
                var jogos = new List<Jogo>();

                using (var connection = new NpgsqlConnection(connString))
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand("SELECT * FROM Jogos", connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            double score = 0;

                            // Ano de lançamento (peso variável)
                            if (pAnoLanc > 0)
                            {
                                int anoBanco = Convert.ToInt32(reader["AnoLancamento"]);
                                score += pAnoLanc * (ano == anoBanco ? 1 : 0);
                            }

                            // Duração (peso variável)
                            if (pDuracao > 0)
                            {
                                int duracaoBanco = Convert.ToInt32(reader["DuracaoHoras"]);
                                // Considera similar se dentro de +/- 10% (aqui usaremos como base a duração, não ano)
                                if (ano > 0 && duracaoBanco > 0)
                                {
                                    // Atenção: seu código original compara 'ano' com duração, provavelmente um erro.
                                    // Ajustei para comparar com 'avaliacao' não faz sentido, então vamos ignorar essa parte
                                    // ou colocar alguma lógica coerente para duração - você pode adaptar.
                                    // Vou fazer uma pontuação simples: se duraçãoBanco dentro de 10% do ano, ou seja...
                                    // isso é estranho, então vou considerar que a entrada do usuário não tem duração, ignoramos.
                                    // Para fins do exemplo, colocarei 0.
                                    score += 0;
                                }
                            }

                            // Avaliação (peso variável)
                            if (pAvaliacao > 0)
                            {
                                double avaliacaoBanco = Convert.ToDouble(reader["Avaliacao"]);
                                double diff = Math.Abs(avaliacao - avaliacaoBanco);
                                score += pAvaliacao * (diff < 2 ? 1 : 0);
                            }

                            // Faixa Etária (peso variável)
                            if (pFaixaEtaria > 0)
                            {
                                string faixaBanco = reader["FaixaEtaria"].ToString();
                                score += pFaixaEtaria * (faixaBanco.Equals(faixaEtaria, StringComparison.OrdinalIgnoreCase) ? 1 : 0);
                            }

                            // Multiplayer (peso variável)
                            if (pMultiplayer > 0)
                            {
                                string multiBanco = reader["Multiplayer"].ToString();
                                score += pMultiplayer * (multiBanco.Equals(multiplayer, StringComparison.OrdinalIgnoreCase) ? 1 : 0);
                            }

                            // Critérios fixos
                            if (!string.IsNullOrEmpty(genero))
                            {
                                string generoBanco = reader["Genero"].ToString();
                                score += (generoBanco.Equals(genero, StringComparison.OrdinalIgnoreCase) ? p_gen : 0);
                            }

                            if (!string.IsNullOrEmpty(plataforma))
                            {
                                string plataformaBanco = reader["Plataforma"].ToString();
                                score += (plataformaBanco.Equals(plataforma, StringComparison.OrdinalIgnoreCase) ? p_plat : 0);
                            }

                            if (!string.IsNullOrEmpty(estiloVisual))
                            {
                                string estiloBanco = reader["EstiloVisual"].ToString();
                                score += (estiloBanco.Equals(estiloVisual, StringComparison.OrdinalIgnoreCase) ? p_estilo : 0);
                            }

                            if (!string.IsNullOrEmpty(desenvolvedora))
                            {
                                string devBanco = reader["Desenvolvedora"].ToString();
                                score += (devBanco.Equals(desenvolvedora, StringComparison.OrdinalIgnoreCase) ? p_dev : 0);
                            }

                            // Criar objeto Jogo com dados e similaridade
                            // Definindo maxScore para normalizar percentual:
                            // Soma dos pesos fixos + soma dos pesos variáveis (assumindo todos 1)
                            // Exemplo simples para normalizar:
                            double maxScore = p_gen + p_plat + p_estilo + p_dev + pAnoLanc + pDuracao + pAvaliacao + pFaixaEtaria + pMultiplayer;
                            if (maxScore == 0) maxScore = 1; // evitar divisão por zero

                            double percent = Math.Min(100, (score / maxScore) * 100);

                            jogos.Add(new Jogo
                            {
                                Titulo = reader["Titulo"].ToString(),
                                Genero = reader["Genero"].ToString(),
                                Plataforma = reader["Plataforma"].ToString(),
                                AnoLancamento = Convert.ToInt32(reader["AnoLancamento"]),
                                Avaliacao = Convert.ToDouble(reader["Avaliacao"]),
                                FaixaEtaria = reader["FaixaEtaria"].ToString(),
                                Multiplayer = reader["Multiplayer"].ToString(),
                                EstiloVisual = reader["EstiloVisual"].ToString(),
                                Desenvolvedora = reader["Desenvolvedora"].ToString(),
                                SimilaridadePercent = percent
                            });
                        }
                    }
                }

                // Ordenar do mais similar para o menos similar
                var rankedJogos = jogos.OrderByDescending(j => j.SimilaridadePercent).ToList();

                jogosListView.ItemsSource = rankedJogos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao buscar jogos: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    // Classe modelo para jogos, com similaridade
    public class Jogo
    {
        public string Titulo { get; set; }
        public string Genero { get; set; }
        public string Plataforma { get; set; }
        public int AnoLancamento { get; set; }
        public double Avaliacao { get; set; }
        public string FaixaEtaria { get; set; }
        public string Multiplayer { get; set; }
        public string EstiloVisual { get; set; }
        public string Desenvolvedora { get; set; }
        public double SimilaridadePercent { get; set; }
    }
}
