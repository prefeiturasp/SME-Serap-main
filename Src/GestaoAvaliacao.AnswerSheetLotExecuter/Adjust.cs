using Castle.Windsor;
using GestaoAvaliacao.MappingDependence;
using GestaoAvaliacao.Services;
using MSTech.Security.Cryptography;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace GestaoAvaliacao.AnswerSheetLotExecuter
{
    public partial class Adjust : Form
    {
        private const string PREFIXO_URL_API_EOL = @"http://hom-smeintegracaoapi.sme.prefeitura.sp.gov.br/";
        private readonly IWindsorContainer container;

        public Adjust()
        {
            container = new WindsorContainer()
                .Install(new BusinessInstaller() { LifestylePerWebRequest = false })
                .Install(new RepositoriesInstaller() { LifestylePerWebRequest = false })
                .Install(new StorageInstaller() { LifestylePerWebRequest = false })
                .Install(new PDFConverterInstaller() { LifestylePerWebRequest = false })
                .Install(new ServiceContainerInstaller());

            InitializeComponent();
        }

        private async void ButtonUpdateAnswers_Click(object sender, System.EventArgs e)
        {
            try
            {
                var service = container.Resolve<StudentCorrection>();

                var testId = (long)numericUpDownTestId.Value;
                var teamId = numericUpDownTeamId.Value > 0 ? (long?)numericUpDownTeamId.Value : null;
                var itemIdOld = (long)numericUpDownItemIdOld.Value;
                var itemIdNew = numericUpDownItemIdNew.Value > 0 ? (long?)numericUpDownItemIdNew.Value : null;
                var alternativeIdOld = (long)numericUpDownAlternativeIdOld.Value;
                var alternativeIdNew = numericUpDownAlternativeIdNew.Value > 0 ? (long?)numericUpDownAlternativeIdNew.Value : null;

                await service.UpdateAnswersTest(testId, teamId, itemIdOld, itemIdNew, alternativeIdOld, alternativeIdNew, checkBoxAnswerCorret.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Concat("Erro ao realizar os ajustes.\n", ex.Message));
            }
        }

        private async void ButtonIncludeNewCorrectionResult_Click(object sender, EventArgs e)
        {
            try
            {
                var service = container.Resolve<StudentCorrection>();

                var testId = (long)numericUpDownTempCorrectionResultTestId.Value;
                var teamId = numericUpDownTempCorrectionResultTeamId.Value > 0 ? (long?)numericUpDownTempCorrectionResultTeamId.Value : null;

                await service.IncludeTestNewCorrectionResult(testId, teamId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Concat("Erro ao realizar os ajustes.\n", ex.Message));
            }
        }

        private void Adjust_Load(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(@"User ID=sa;Initial Catalog=GestaoPedagogica;Data Source=.\sqlexpress;Password=S@12345"))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT tce_id, tce_nome FROM ESC_TipoClassificacaoEscola ORDER BY tce_nome", conn);
                using (var dr = command.ExecuteReader())
                {
                    while (dr.Read())
                        comboBoxTipoEscolaImpAluno.Items.Add(new ItemTipoEscola() { Id = dr.GetInt32(0), Descricao = dr.GetString(1) });
                }
                comboBoxTipoEscolaImpAluno.SelectedIndex = 0;
            }
        }

        private async void ButtonImpAlunos_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show(string.Concat("Deseja realmente importar alunos do ", ((ItemTipoEscola)comboBoxTipoEscolaImpAluno.SelectedItem).Descricao, "?"), "Atenção", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            progressBarImportAlunos.Style = ProgressBarStyle.Marquee;
            progressBarImportAlunos.MarqueeAnimationSpeed = 30;

            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.AppendLine("SELECT e.esc_codigo");
            sqlQuery.AppendLine("    FROM ESC_Escola e");
            sqlQuery.AppendLine("        INNER JOIN ESC_EscolaClassificacao ec");
            sqlQuery.AppendLine("            ON e.esc_id = ec.esc_id");
            sqlQuery.AppendLine("        INNER JOIN ESC_TipoClassificacaoEscola tce");
            sqlQuery.AppendLine("            ON ec.tce_id = tce.tce_id");
            sqlQuery.AppendLine("WHERE tce.tce_id = @tce_id and");
            sqlQuery.AppendLine("      e.esc_situacao = 1");
            sqlQuery.AppendLine("ORDER BY e.esc_id");

            using (var conGestaoPedagogica = new SqlConnection(Decrypt(ConfigurationManager.ConnectionStrings["GestaoPedagogica"].ConnectionString)))
            {
                conGestaoPedagogica.Open();
                var commandGestaoPedagogica = new SqlCommand(sqlQuery.ToString(), conGestaoPedagogica);
                commandGestaoPedagogica.Parameters.AddWithValue("@tce_id", ((ItemTipoEscola)comboBoxTipoEscolaImpAluno.SelectedItem).Id);

                using (var drEscolas = commandGestaoPedagogica.ExecuteReader())
                {
                    while (drEscolas.Read())
                    {
                        var escolaCodigo = drEscolas.GetString(0);
                        var httpClient = new HttpClient();
                        var urlTurmas = $"api/escolas/{escolaCodigo}/turmas/anos_letivos/{numericUpDownImpAlunoAnoLetivo.Value}";
                        HttpResponseMessage response = await httpClient.GetAsync(string.Concat(PREFIXO_URL_API_EOL, urlTurmas));
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonTurmasString = await response.Content.ReadAsStringAsync();
                            var turmas = JsonConvert.DeserializeObject<IList<Turma>>(jsonTurmasString);
                            var dadosCompletosTurmas = new List<DadosCompletosTurma>();

                            sqlQuery = new StringBuilder();
                            sqlQuery.AppendLine("SELECT Id,");
                            sqlQuery.AppendLine("	   [Description],");
                            sqlQuery.AppendLine("	   'False'");
                            sqlQuery.AppendLine("	FROM Test");
                            sqlQuery.AppendLine("WHERE TestType_Id = 21 and");
                            sqlQuery.AppendLine("	  ApplicationEndDate > GETDATE()");

                            using (var conGestaoAvaliacao = new SqlConnection(Decrypt(ConfigurationManager.ConnectionStrings["GestaoAvaliacao"].ConnectionString)))
                            {
                                conGestaoAvaliacao.Open();                                

                                var provas = new List<Prova>();
                                using (var command = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao))
                                {
                                    using (var dr = command.ExecuteReader())
                                    {
                                        while (dr.Read())
                                        {
                                            provas.Add(new Prova()
                                            {
                                                Id = dr.GetInt64(0),
                                                Descricao = dr.GetString(1)
                                            });
                                        }
                                    }
                                }

                                sqlQuery.Clear();
                                sqlQuery.AppendLine("SELECT DISTINCT [cd_escola]");
                                sqlQuery.AppendLine("               ,[cd_turma_escola]");
                                sqlQuery.AppendLine("               ,[dc_turma_escola]");
                                sqlQuery.AppendLine("               ,[an_letivo]");
                                sqlQuery.AppendLine("               ,[dc_tipo_periodicidade]");
                                sqlQuery.AppendLine("               ,[dc_tipo_turno]");
                                sqlQuery.AppendLine("               ,[st_turma_escola]");
                                sqlQuery.AppendLine("               ,[cd_tipo_turma]");
                                sqlQuery.AppendLine("               ,[dt_inicio_turma]");
                                sqlQuery.AppendLine("               ,[dt_fim_turma]");
                                sqlQuery.AppendLine("               ,[nr_ordem_serie]");
                                sqlQuery.AppendLine("               ,[cd_modalidade_ensino]");
                                sqlQuery.AppendLine("               ,[cd_etapa_ensino]");
                                sqlQuery.AppendLine("               ,[nr_ordem_etapa]");
                                sqlQuery.AppendLine("               ,[dc_serie_ensino]");
                                sqlQuery.AppendLine("               ,[sg_modalidade_ensino]");
                                sqlQuery.AppendLine("               ,[sg_tp_escola]");
                                sqlQuery.AppendLine("FROM TurmasEjaEol");

                                var commandGestaoAvaliacao = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao);
                                using (var drGestaoAvaliacao = commandGestaoAvaliacao.ExecuteReader())
                                {
                                    while (drGestaoAvaliacao.Read())
                                    {
                                        var codigoTurma = (long)drGestaoAvaliacao.GetDouble(drGestaoAvaliacao.GetOrdinal("cd_turma_escola"));
                                        var nomeTurma = drGestaoAvaliacao.GetString(drGestaoAvaliacao.GetOrdinal("dc_turma_escola"));
                                        var tipoTurma = drGestaoAvaliacao.GetDouble(drGestaoAvaliacao.GetOrdinal("cd_tipo_turma")).ToString();
                                        var situacao = drGestaoAvaliacao.GetString(drGestaoAvaliacao.GetOrdinal("st_turma_escola"));
                                        var dataInicioTurma = Convert.ToDateTime(drGestaoAvaliacao.GetString(drGestaoAvaliacao.GetOrdinal("dt_inicio_turma")));
                                        var dataFimTurma = Convert.ToDateTime(drGestaoAvaliacao.GetString(drGestaoAvaliacao.GetOrdinal("dt_fim_turma")));

                                        if (drGestaoAvaliacao.GetString(drGestaoAvaliacao.GetOrdinal("sg_tp_escola")).Equals("NULL"))
                                        {
                                            turmas.Add(new Turma()
                                            {
                                                codigoTurma = codigoTurma,
                                                nomeTurma = nomeTurma,
                                                TipoTurma = tipoTurma,
                                                Situacao = situacao,
                                                dataInicioTurma = dataInicioTurma,
                                                dataFimTurma = dataFimTurma
                                            });
                                        }

                                        if (!dadosCompletosTurmas.Any(x => x.CodigoTurma.Equals(codigoTurma)))
                                        {
                                            dadosCompletosTurmas.Add(new DadosCompletosTurma()
                                            {
                                                CodigoEscola = (long)drGestaoAvaliacao.GetDouble(drGestaoAvaliacao.GetOrdinal("cd_escola")),
                                                CodigoTurma = codigoTurma,
                                                DescricaoTurma = nomeTurma,
                                                AnoLetivo = (int)drGestaoAvaliacao.GetDouble(drGestaoAvaliacao.GetOrdinal("an_letivo")),
                                                DescricaoTipoPeridiocidade = drGestaoAvaliacao.GetString(drGestaoAvaliacao.GetOrdinal("dc_tipo_periodicidade")),
                                                DescricaoTipoTurno = drGestaoAvaliacao.GetString(drGestaoAvaliacao.GetOrdinal("dc_tipo_turno")),
                                                SituacaoTurma = situacao,
                                                TipoTurma = tipoTurma,
                                                DataInicioTurma = dataInicioTurma,
                                                DataFimTurma = dataFimTurma,
                                                NumeroOrdemSerie = (int)drGestaoAvaliacao.GetDouble(drGestaoAvaliacao.GetOrdinal("nr_ordem_serie")),
                                                CodigoModalidadeEnsino = (int)drGestaoAvaliacao.GetDouble(drGestaoAvaliacao.GetOrdinal("cd_modalidade_ensino")),
                                                CodigoEtapaEnsino = (int)drGestaoAvaliacao.GetDouble(drGestaoAvaliacao.GetOrdinal("cd_etapa_ensino")),
                                                NumeroOrdemEtapa = (int)drGestaoAvaliacao.GetDouble(drGestaoAvaliacao.GetOrdinal("nr_ordem_etapa")),
                                                DescricaoSerieEnsino = drGestaoAvaliacao.GetString(drGestaoAvaliacao.GetOrdinal("dc_serie_ensino")),
                                                SiglaModalidadeEnsino = drGestaoAvaliacao.GetString(drGestaoAvaliacao.GetOrdinal("sg_modalidade_ensino")),
                                                TipoEscola = drGestaoAvaliacao.GetString(drGestaoAvaliacao.GetOrdinal("sg_modalidade_ensino"))
                                            });
                                        }
                                    }
                                }

                                long? tur_id = null;
                                int? cur_id = null;
                                int? tcp_id = null;
                                string descricaoModalidadeEnsino = "CIEJA";
                                var idCalendarioAnual = 0;

                                foreach (var turma in turmas)
                                {
                                    var transactionGestaoAvaliacao = conGestaoAvaliacao.BeginTransaction();

                                    var dadosCompletosTurma = dadosCompletosTurmas.SingleOrDefault(x => x.CodigoTurma.Equals(turma.codigoTurma));

                                    if (dadosCompletosTurma.CodigoTurma > 0)
                                    {
                                        sqlQuery.Clear();
                                        sqlQuery.AppendLine("SELECT TOP 1 t.tur_id");
                                        sqlQuery.AppendLine("	FROM SGP_TUR_Turma t");
                                        sqlQuery.AppendLine("		INNER JOIN SGP_ESC_Escola e");
                                        sqlQuery.AppendLine("			ON t.esc_id = e.esc_id");
                                        sqlQuery.AppendLine("		INNER JOIN SGP_ACA_TipoTurno tt");
                                        sqlQuery.AppendLine("			ON t.ttn_id = tt.ttn_id");
                                        sqlQuery.AppendLine("WHERE e.esc_codigo = @esc_codigo AND");
                                        sqlQuery.AppendLine("	   t.tur_tipo = @tur_tipo AND");
                                        sqlQuery.AppendLine("	   t.cal_id = @cal_id AND");
                                        sqlQuery.AppendLine("	   tt.ttn_nome = @ttn_nome AND");
                                        sqlQuery.AppendLine("	   t.tur_codigo = @tur_codigo");

                                        idCalendarioAnual = ObterIdCalendario(dadosCompletosTurma.DescricaoTipoPeridiocidade, conGestaoAvaliacao, transactionGestaoAvaliacao);

                                        using (commandGestaoAvaliacao = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao, transactionGestaoAvaliacao))
                                        {
                                            commandGestaoAvaliacao.Parameters.AddWithValue("@esc_codigo", dadosCompletosTurma.CodigoEscola.ToString().PadLeft(6, '0'));
                                            commandGestaoAvaliacao.Parameters.AddWithValue("@tur_tipo", dadosCompletosTurma.TipoTurma);
                                            commandGestaoAvaliacao.Parameters.AddWithValue("@cal_id", idCalendarioAnual);
                                            commandGestaoAvaliacao.Parameters.AddWithValue("@ttn_nome", dadosCompletosTurma.DescricaoTipoTurno.Trim());
                                            commandGestaoAvaliacao.Parameters.AddWithValue("@tur_codigo", $"EJA-{dadosCompletosTurma.DescricaoTurma}");
                                            tur_id = (long?)commandGestaoAvaliacao.ExecuteScalar();
                                        }
                                    }

                                    var conGestaoAvaliacao_SGP = new SqlConnection(Decrypt(ConfigurationManager.ConnectionStrings["GestaoAvaliacao_SGP"].ConnectionString));
                                    conGestaoAvaliacao_SGP.Open();
                                    var transactionGestaoAvaliacaoSgp = conGestaoAvaliacao_SGP.BeginTransaction();

                                    try
                                    {
                                        if (dadosCompletosTurma.CodigoTurma > 0)
                                        {
                                            if (!tur_id.HasValue)
                                            {
                                                sqlQuery.Clear();
                                                sqlQuery.AppendLine("DECLARE @tur_id BIGINT = (SELECT TOP 1 tur_id FROM TUR_Turma ORDER BY tur_id DESC) + 1");
                                                sqlQuery.AppendLine("INSERT INTO TUR_Turma");
                                                sqlQuery.AppendLine("SELECT DISTINCT @tur_id,");
                                                sqlQuery.AppendLine("	             esc_id,");
                                                sqlQuery.AppendLine("	             @tur_codigo,");
                                                sqlQuery.AppendLine("	             NULL,");
                                                sqlQuery.AppendLine("	             @cal_id,");
                                                sqlQuery.AppendLine("	             tt.ttn_id,");
                                                sqlQuery.AppendLine("	             1,");
                                                sqlQuery.AppendLine("	             GETDATE(),");
                                                sqlQuery.AppendLine("	             GETDATE(),");
                                                sqlQuery.AppendLine("	             1");
                                                sqlQuery.AppendLine("	FROM ESC_Escola e,");
                                                sqlQuery.AppendLine("		 ACA_TipoTurno tt");
                                                sqlQuery.AppendLine("WHERE e.esc_codigo = @esc_codigo AND");
                                                sqlQuery.AppendLine("	   tt.ttn_nome = @ttn_nome");
                                                sqlQuery.AppendLine("SELECT @tur_id");

                                                using (var commandGestaoAvaliacaoSgp = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao_SGP, transactionGestaoAvaliacaoSgp))
                                                {
                                                    commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tur_codigo", string.Concat("EJA-", dadosCompletosTurma.DescricaoTurma));
                                                    commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@cal_id", idCalendarioAnual);
                                                    commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@esc_codigo", dadosCompletosTurma.CodigoEscola.ToString().PadLeft(6, '0'));
                                                    commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@ttn_nome", dadosCompletosTurma.DescricaoTipoTurno.Trim());
                                                    tur_id = (long?)commandGestaoAvaliacaoSgp.ExecuteScalar();
                                                }
                                            }
                                            else
                                            {
                                                using (var commandGestaoAvaliacaoSgp = new SqlCommand("UPDATE TUR_Turma SET tur_situacao = 1 WHERE tur_id = @tur_id", conGestaoAvaliacao_SGP, transactionGestaoAvaliacaoSgp))
                                                {
                                                    commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tur_id", tur_id.Value);
                                                    commandGestaoAvaliacaoSgp.ExecuteNonQuery();
                                                }
                                            }

                                            if (dadosCompletosTurma.CodigoEtapaEnsino.Equals(3) && dadosCompletosTurma.DescricaoSerieEnsino.Contains("MODULAR"))
                                                descricaoModalidadeEnsino = "EJA Modular";
                                            if (dadosCompletosTurma.CodigoEtapaEnsino.Equals(3) && !dadosCompletosTurma.DescricaoSerieEnsino.Contains("MODULAR"))
                                                descricaoModalidadeEnsino = "EJA Regular";
                                            else if (dadosCompletosTurma.CodigoEtapaEnsino.Equals(11))
                                                descricaoModalidadeEnsino = "EJA Especial";

                                            sqlQuery.Clear();
                                            sqlQuery.AppendLine("SELECT TOP 1 c.cur_id");
                                            sqlQuery.AppendLine("	FROM ACA_Curso c");
                                            sqlQuery.AppendLine("		INNER JOIN ACA_TipoModalidadeEnsino tme");
                                            sqlQuery.AppendLine("			ON c.tme_id = tme.tme_id");
                                            sqlQuery.AppendLine("WHERE c.cur_nome LIKE '%EJA%' AND");
                                            sqlQuery.AppendLine("	   c.cur_nome LIKE @ano AND");
                                            sqlQuery.AppendLine("	   c.cur_codigo = @cur_codigo AND");
                                            sqlQuery.AppendLine("	   tme.tme_nome = @tme_nome");

                                            using (var commandGestaoAvaliacaoSgp = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao_SGP, transactionGestaoAvaliacaoSgp))
                                            {
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@ano", string.Concat("%", numericUpDownImpAlunoAnoLetivo.Value, "%"));
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@cur_codigo", dadosCompletosTurma.CodigoEtapaEnsino);
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tme_nome", descricaoModalidadeEnsino);
                                                cur_id = (int?)commandGestaoAvaliacaoSgp.ExecuteScalar();
                                            }

                                            if (!cur_id.HasValue)
                                            {
                                                sqlQuery.Clear();
                                                sqlQuery.AppendLine("DECLARE @cur_id INT = (SELECT TOP 1 cur_id FROM ACA_Curso ORDER BY cur_id DESC) + 1");
                                                sqlQuery.AppendLine("INSERT INTO ACA_Curso");
                                                sqlQuery.AppendLine("SELECT @cur_id,");
                                                sqlQuery.AppendLine("	    '6CF424DC-8EC3-E011-9B36-00155D033206',");
                                                sqlQuery.AppendLine("	    tne.tne_id,");
                                                sqlQuery.AppendLine("	    tme.tme_id,");
                                                sqlQuery.AppendLine("	    @cur_codigo,");
                                                sqlQuery.AppendLine("	    @cur_nome,");
                                                sqlQuery.AppendLine("	    @cur_nome_abreviado,");
                                                sqlQuery.AppendLine("	    1,");
                                                sqlQuery.AppendLine("	    GETDATE(),");
                                                sqlQuery.AppendLine("	    GETDATE()");
                                                sqlQuery.AppendLine("	FROM ACA_TipoNivelEnsino tne, ");
                                                sqlQuery.AppendLine("	     ACA_TipoModalidadeEnsino tme");
                                                sqlQuery.AppendLine("WHERE tne.tne_ordem = @tne_ordem AND");
                                                sqlQuery.AppendLine("      tme.tme_nome = @tme_nome");
                                                sqlQuery.AppendLine("SELECT @cur_id");

                                                using (var commandGestaoAvaliacaoSgp = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao_SGP, transactionGestaoAvaliacaoSgp))
                                                {
                                                    commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@cur_codigo", dadosCompletosTurma.CodigoEtapaEnsino);
                                                    commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@cur_nome", string.Concat("2019 - ", dadosCompletosTurma.NumeroOrdemEtapa.Equals(7) ? "EJA Modular" : "EJA CIEJA"));
                                                    commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@cur_nome_abreviado", string.Concat("2019 - ", dadosCompletosTurma.NumeroOrdemEtapa.Equals(7) ? "EJA MOD" : "CIEJA"));
                                                    commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tne_ordem", dadosCompletosTurma.NumeroOrdemSerie);
                                                    commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tme_nome", descricaoModalidadeEnsino);
                                                    cur_id = (int?)commandGestaoAvaliacaoSgp.ExecuteScalar();
                                                }
                                            }

                                            sqlQuery.Clear();
                                            sqlQuery.AppendLine("SELECT COUNT(0)");
                                            sqlQuery.AppendLine("	FROM TUR_TurmaTipoCurriculoPeriodo ttcp");
                                            sqlQuery.AppendLine("		 INNER JOIN ESC_Escola e");
                                            sqlQuery.AppendLine("			ON ttcp.esc_id = e.esc_id");
                                            sqlQuery.AppendLine("INNER JOIN ACA_TipoModalidadeEnsino tme");
                                            sqlQuery.AppendLine("   ON ttcp.tme_id = tme.tme_id");
                                            sqlQuery.AppendLine("INNER JOIN ACA_TipoNivelEnsino tne");
                                            sqlQuery.AppendLine("   ON ttcp.tne_id = tne.tne_id");
                                            sqlQuery.AppendLine("WHERE ttcp.tur_id = @tur_id AND");
                                            sqlQuery.AppendLine("	   ttcp.cur_id = @cur_id AND");
                                            sqlQuery.AppendLine("	   tme.tme_nome = @tme_nome AND");
                                            sqlQuery.AppendLine("	   tne.tne_ordem = @tne_ordem AND");
                                            sqlQuery.AppendLine("	   ttcp.crp_ordem = @crp_ordem AND");
                                            sqlQuery.AppendLine("	   e.esc_codigo = @esc_codigo");

                                            using (var commandGestaoAvaliacaoSgp = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao_SGP, transactionGestaoAvaliacaoSgp))
                                            {
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tur_id", tur_id.Value);
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@cur_id", cur_id.Value);
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tme_nome", descricaoModalidadeEnsino);
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tne_ordem", dadosCompletosTurma.NumeroOrdemEtapa);
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@crp_ordem", dadosCompletosTurma.NumeroOrdemSerie);
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@esc_codigo", dadosCompletosTurma.CodigoEscola.ToString().PadLeft(6, '0'));

                                                if ((int)commandGestaoAvaliacaoSgp.ExecuteScalar() < 1)
                                                {
                                                    sqlQuery.Clear();
                                                    sqlQuery.AppendLine("INSERT INTO TUR_TurmaTipoCurriculoPeriodo");
                                                    sqlQuery.AppendLine("SELECT @tur_id,");
                                                    sqlQuery.AppendLine("	    @cur_id,");
                                                    sqlQuery.AppendLine("	    tme.tme_id,");
                                                    sqlQuery.AppendLine("	    tne.tne_id,");
                                                    sqlQuery.AppendLine("	    @crp_ordem,");
                                                    sqlQuery.AppendLine("	    1,");
                                                    sqlQuery.AppendLine("	    e.esc_id");
                                                    sqlQuery.AppendLine("   FROM ESC_Escola e,");
                                                    sqlQuery.AppendLine("		 ACA_TipoModalidadeEnsino tme,");
                                                    sqlQuery.AppendLine("		 ACA_TipoNivelEnsino tne");
                                                    sqlQuery.AppendLine("WHERE tme.tme_nome = @tme_nome AND");
                                                    sqlQuery.AppendLine("	   tne.tne_ordem = @tne_ordem AND");
                                                    sqlQuery.AppendLine("	   e.esc_codigo = @esc_codigo");

                                                    using (var command = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao_SGP, transactionGestaoAvaliacaoSgp))
                                                    {
                                                        command.Parameters.AddWithValue("@tur_id", tur_id.Value);
                                                        command.Parameters.AddWithValue("@cur_id", cur_id.Value);
                                                        command.Parameters.AddWithValue("@tme_nome", descricaoModalidadeEnsino);
                                                        command.Parameters.AddWithValue("@tne_ordem", dadosCompletosTurma.NumeroOrdemEtapa);
                                                        command.Parameters.AddWithValue("@crp_ordem", dadosCompletosTurma.NumeroOrdemSerie);
                                                        command.Parameters.AddWithValue("@esc_codigo", dadosCompletosTurma.CodigoEscola.ToString().PadLeft(6, '0'));
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }

                                            sqlQuery.Clear();
                                            sqlQuery.AppendLine("SELECT TOP 1 tpcp.tcp_id");
                                            sqlQuery.AppendLine("	FROM ACA_TipoCurriculoPeriodo tpcp");
                                            sqlQuery.AppendLine("		INNER JOIN ACA_TipoNivelEnsino tne");
                                            sqlQuery.AppendLine("			ON tpcp.tne_id = tne.tne_id");
                                            sqlQuery.AppendLine("		INNER JOIN ACA_TipoModalidadeEnsino tme");
                                            sqlQuery.AppendLine("			ON tpcp.tme_id = tme.tme_id");
                                            sqlQuery.AppendLine("WHERE tne.tne_ordem = @tne_ordem AND");
                                            sqlQuery.AppendLine("	   tme.tme_nome = @tme_nome AND");
                                            sqlQuery.AppendLine("	   tpcp.tcp_ordem = @tcp_ordem AND");
                                            sqlQuery.AppendLine("	   tpcp.tcp_descricao = @tcp_descricao");

                                            using (var commandGestaoAvaliacaoSgp = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao_SGP, transactionGestaoAvaliacaoSgp))
                                            {
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tne_ordem", dadosCompletosTurma.NumeroOrdemEtapa);
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tme_nome", descricaoModalidadeEnsino);
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tcp_ordem", dadosCompletosTurma.NumeroOrdemSerie);
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tcp_descricao", dadosCompletosTurma.DescricaoSerieEnsino);

                                                tcp_id = (int?)commandGestaoAvaliacaoSgp.ExecuteScalar();

                                                if (!tcp_id.HasValue)
                                                {
                                                    sqlQuery.Clear();
                                                    sqlQuery.AppendLine("DECLARE @tcp_id INT = (SELECT TOP 1 tcp_id FROM ACA_TipoCurriculoPeriodo ORDER BY tcp_id DESC) + 1");
                                                    sqlQuery.AppendLine("INSERT INTO ACA_TipoCurriculoPeriodo");
                                                    sqlQuery.AppendLine("SELECT @tcp_id,");
                                                    sqlQuery.AppendLine("	    tne.tne_id,");
                                                    sqlQuery.AppendLine("	    tme.tme_id,");
                                                    sqlQuery.AppendLine("	    @tcp_descricao,");
                                                    sqlQuery.AppendLine("	    @tcp_ordem,");
                                                    sqlQuery.AppendLine("	    1,");
                                                    sqlQuery.AppendLine("	    GETDATE(),");
                                                    sqlQuery.AppendLine("	    GETDATE()");
                                                    sqlQuery.AppendLine("	FROM ACA_TipoNivelEnsino tne,");
                                                    sqlQuery.AppendLine("		 ACA_TipoModalidadeEnsino tme");
                                                    sqlQuery.AppendLine("WHERE tne.tne_nome like '%EJA%' AND");
                                                    sqlQuery.AppendLine("      tne.tne_ordem = @tne_ordem AND");
                                                    sqlQuery.AppendLine("	   tme.tme_nome = @tme_nome");
                                                    sqlQuery.AppendLine("SELECT @tcp_id");

                                                    using (var command = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao_SGP, transactionGestaoAvaliacaoSgp))
                                                    {
                                                        command.Parameters.AddWithValue("@tne_ordem", dadosCompletosTurma.NumeroOrdemEtapa);
                                                        command.Parameters.AddWithValue("@tme_nome", descricaoModalidadeEnsino);
                                                        command.Parameters.AddWithValue("@tcp_descricao", dadosCompletosTurma.DescricaoSerieEnsino);
                                                        command.Parameters.AddWithValue("@tcp_ordem", dadosCompletosTurma.NumeroOrdemSerie);
                                                        tcp_id = (int?)command.ExecuteScalar();
                                                    }
                                                }
                                            }

                                            sqlQuery.Clear();
                                            sqlQuery.AppendLine("INSERT INTO TestCurriculumGrade");
                                            sqlQuery.AppendLine("	SELECT DISTINCT @tcp_id,");
                                            sqlQuery.AppendLine("					GETDATE(),");
                                            sqlQuery.AppendLine("					GETDATE(),");
                                            sqlQuery.AppendLine("					1,");
                                            sqlQuery.AppendLine("					@Test_Id");                                            
                                            sqlQuery.AppendLine("	WHERE NOT EXISTS (SELECT Id");
                                            sqlQuery.AppendLine("						FROM TestCurriculumGrade");
                                            sqlQuery.AppendLine("					  WHERE TypeCurriculumGradeId = @tcp_id AND");
                                            sqlQuery.AppendLine("						    Test_Id = @Test_Id)");

                                            if (dadosCompletosTurma.DescricaoSerieEnsino.Equals("EJA BASICA II") ||
                                                dadosCompletosTurma.DescricaoSerieEnsino.Equals("2ª EJA MODULAR") ||
                                                dadosCompletosTurma.DescricaoSerieEnsino.Equals("M II"))
                                            {
                                                foreach (var prova in provas.Where(p => p.Descricao.StartsWith("BÁSICA2") || p.Descricao.StartsWith("BASICA2")))
                                                {
                                                    using (var command = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao, transactionGestaoAvaliacao))
                                                    {
                                                        command.Parameters.AddWithValue("@tcp_id", tcp_id);
                                                        command.Parameters.AddWithValue("@Test_Id", prova.Id);
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                            else if (dadosCompletosTurma.DescricaoSerieEnsino.Equals("EJA FINAL II") ||
                                                     dadosCompletosTurma.DescricaoSerieEnsino.Equals("4ª EJA MODULAR") ||
                                                     dadosCompletosTurma.DescricaoSerieEnsino.Equals("M IV"))
                                            {
                                                foreach (var prova in provas.Where(p => !p.Descricao.StartsWith("BÁSICA2") && !p.Descricao.StartsWith("BASICA2")))
                                                {
                                                    using (var command = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao, transactionGestaoAvaliacao))
                                                    {
                                                        command.Parameters.AddWithValue("@tcp_id", tcp_id);
                                                        command.Parameters.AddWithValue("@Test_Id", prova.Id);
                                                        command.ExecuteNonQuery();
                                                    }
                                                }
                                            }

                                            sqlQuery.Clear();
                                            sqlQuery.AppendLine("INSERT INTO TestTypeCourse");
                                            sqlQuery.AppendLine("	SELECT DISTINCT @cur_id,");
                                            sqlQuery.AppendLine("					GETDATE(),");
                                            sqlQuery.AppendLine("					GETDATE(),");
                                            sqlQuery.AppendLine("					1,");
                                            sqlQuery.AppendLine("					21,");
                                            sqlQuery.AppendLine("					tme.tme_id");
                                            sqlQuery.AppendLine("	FROM GestaoAvaliacao_SGP.dbo.ACA_TipoModalidadeEnsino tme (NOLOCK)");
                                            sqlQuery.AppendLine("	WHERE tme.tme_nome = @tme_nome AND");
                                            sqlQuery.AppendLine("	NOT EXISTS (SELECT Id");
                                            sqlQuery.AppendLine("				    FROM TestTypeCourse");
                                            sqlQuery.AppendLine("				WHERE CourseId = @cur_id AND");
                                            sqlQuery.AppendLine("					  TestType_Id = 21 AND");
                                            sqlQuery.AppendLine("					  ModalityId = tme.tme_id)");

                                            using (var command = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao, transactionGestaoAvaliacao))
                                            {
                                                command.Parameters.AddWithValue("@cur_id", cur_id);
                                                command.Parameters.AddWithValue("@tme_nome", descricaoModalidadeEnsino);
                                                command.ExecuteNonQuery();
                                            }

                                            sqlQuery.Clear();
                                            sqlQuery.AppendLine("INSERT INTO ACA_Curriculo");
                                            sqlQuery.AppendLine("	SELECT DISTINCT @cur_id,");
                                            sqlQuery.AppendLine("				    1,");
                                            sqlQuery.AppendLine("				    NULL,");
                                            sqlQuery.AppendLine("				    1,");
                                            sqlQuery.AppendLine("				    GETDATE(),");
                                            sqlQuery.AppendLine("				    GETDATE()");                                            
                                            sqlQuery.AppendLine("	WHERE NOT EXISTS (SELECT cur_id");
                                            sqlQuery.AppendLine("					  FROM ACA_Curriculo");
                                            sqlQuery.AppendLine("					  WHERE cur_id = @cur_id AND");
                                            sqlQuery.AppendLine("							crr_id = 1)");                                            
                                            sqlQuery.AppendLine("	INSERT INTO ACA_CurriculoPeriodo");
                                            sqlQuery.AppendLine("	SELECT @cur_id,");
                                            sqlQuery.AppendLine("		   1,");
                                            sqlQuery.AppendLine("		   @tcp_ordem,");
                                            sqlQuery.AppendLine("		   @tcp_ordem,");
                                            sqlQuery.AppendLine("		   @tcp_descricao,");
                                            sqlQuery.AppendLine("		   1,");
                                            sqlQuery.AppendLine("		   GETDATE(),");
                                            sqlQuery.AppendLine("		   GETDATE(),");
                                            sqlQuery.AppendLine("		   @tcp_id");                                            
                                            sqlQuery.AppendLine("	WHERE NOT EXISTS (SELECT cur_id");
                                            sqlQuery.AppendLine("						FROM ACA_CurriculoPeriodo");
                                            sqlQuery.AppendLine("					  WHERE cur_id = @cur_id AND");
                                            sqlQuery.AppendLine("						    crr_id = 1 AND");
                                            sqlQuery.AppendLine("							crp_id = @tcp_ordem AND");
                                            sqlQuery.AppendLine("							crp_ordem = @tcp_ordem AND");
                                            sqlQuery.AppendLine("							tcp_id = @tcp_id)");

                                            using (var command = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao_SGP, transactionGestaoAvaliacaoSgp))
                                            {
                                                command.Parameters.AddWithValue("@cur_id", cur_id);
                                                command.Parameters.AddWithValue("@tcp_ordem", dadosCompletosTurma.NumeroOrdemSerie);
                                                command.Parameters.AddWithValue("@tcp_descricao", dadosCompletosTurma.DescricaoSerieEnsino);
                                                command.Parameters.AddWithValue("@tcp_id", tcp_id.HasValue ? tcp_id.Value : 0);
                                                command.ExecuteNonQuery();
                                            }

                                            sqlQuery.Clear();
                                            sqlQuery.AppendLine("INSERT INTO TUR_TurmaCurriculo");
                                            sqlQuery.AppendLine("SELECT DISTINCT tur_id,");
                                            sqlQuery.AppendLine("	             @cur_id,");
                                            sqlQuery.AppendLine("	             1,");
                                            sqlQuery.AppendLine("	             @crp_id,");
                                            sqlQuery.AppendLine("	             1,");
                                            sqlQuery.AppendLine("	             GETDATE(),");
                                            sqlQuery.AppendLine("	             GETDATE(),");
                                            sqlQuery.AppendLine("	             @tcp_id");
                                            sqlQuery.AppendLine("FROM TUR_Turma t,");
                                            sqlQuery.AppendLine("	  ESC_Escola e");
                                            sqlQuery.AppendLine("WHERE t.tur_codigo = @tur_codigo AND");
                                            sqlQuery.AppendLine("      e.esc_codigo = @esc_codigo AND");
                                            sqlQuery.AppendLine("	   t.esc_id = e.esc_id AND");
                                            sqlQuery.AppendLine("	   NOT EXISTS (SELECT tur_id");
                                            sqlQuery.AppendLine("				   FROM TUR_TurmaCurriculo");
                                            sqlQuery.AppendLine("				   WHERE tur_id = t.tur_id AND");
                                            sqlQuery.AppendLine("						 cur_id = @cur_id AND");
                                            sqlQuery.AppendLine("						 crr_id = 1)");

                                            using (var commandGestaoAvaliacaoSgp = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao_SGP, transactionGestaoAvaliacaoSgp))
                                            {
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@cur_id", cur_id);
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@crp_id", dadosCompletosTurma.NumeroOrdemSerie);
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tur_codigo", string.Concat("EJA-", dadosCompletosTurma.DescricaoTurma));
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@esc_codigo", dadosCompletosTurma.CodigoEscola.ToString().PadLeft(6, '0'));
                                                commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tcp_id", tcp_id.Value);
                                                commandGestaoAvaliacaoSgp.ExecuteNonQuery();
                                            }
                                        }

                                        response = await httpClient.GetAsync(string.Concat(PREFIXO_URL_API_EOL, $"api/turmas/{turma.codigoTurma}/alunos/anosLetivos/{numericUpDownImpAlunoAnoLetivo.Value}"));
                                        if (response.IsSuccessStatusCode)
                                        {
                                            var jsonAlunosString = await response.Content.ReadAsStringAsync();
                                            var alunos = JsonConvert.DeserializeObject<Aluno[]>(jsonAlunosString)
                                                .Where(a => !a.situacaoMatricula.Equals("Desistente", StringComparison.InvariantCultureIgnoreCase) &&
                                                            !a.situacaoMatricula.Equals("Reclassificado Saída", StringComparison.InvariantCultureIgnoreCase) &&
                                                            !a.situacaoMatricula.Equals("Vínculo Indevido", StringComparison.InvariantCultureIgnoreCase) &&
                                                            !a.situacaoMatricula.Equals("Remanejado Saída", StringComparison.InvariantCultureIgnoreCase) &&
                                                            !a.situacaoMatricula.Equals("Transferido", StringComparison.InvariantCultureIgnoreCase) &&
                                                            !a.situacaoMatricula.Equals("Não Compareceu", StringComparison.InvariantCultureIgnoreCase) &&
                                                            !a.situacaoMatricula.Equals("Deslocamento", StringComparison.InvariantCultureIgnoreCase))
                                                .Distinct();

                                            if (alunos.Any())
                                            {
                                                using (var connCoreSSO = new SqlConnection(Decrypt(ConfigurationManager.ConnectionStrings["CoreSSO"].ConnectionString)))
                                                {
                                                    connCoreSSO.Open();
                                                    sqlQuery.Clear();
                                                    sqlQuery.AppendLine("SELECT u.usu_login,");
                                                    sqlQuery.AppendLine("       u.pes_id,");
                                                    sqlQuery.AppendLine("       u.usu_id,");
                                                    sqlQuery.AppendLine("	    g.gru_id");
                                                    sqlQuery.AppendLine("    FROM SYS_Usuario u");
                                                    sqlQuery.AppendLine("        LEFT JOIN SYS_UsuarioGrupo ug");
                                                    sqlQuery.AppendLine("            ON u.usu_id = ug.usu_id");
                                                    sqlQuery.AppendLine("        LEFT JOIN SYS_Grupo g");
                                                    sqlQuery.AppendLine("            ON ug.gru_id = g.gru_id");
                                                    sqlQuery.AppendLine("WHERE g.sis_id = 204 AND");
                                                    sqlQuery.AppendLine("      g.gru_situacao = 1 AND");
                                                    sqlQuery.AppendLine("      g.gru_nome = 'Aluno' AND");
                                                    sqlQuery.AppendLine($"     u.usu_login IN ({string.Join(", ", alunos.Select(x => string.Concat("'RA", x.codigoAluno, "'")))})");

                                                    var alunosInclusao = new List<Aluno>();
                                                    var alunosSemGrupo = new List<string>();

                                                    var commandCoreSSO = new SqlCommand(sqlQuery.ToString(), connCoreSSO);
                                                    using (var drUsuarios = commandCoreSSO.ExecuteReader())
                                                    {
                                                        var dataTableAlunos = new DataTable();
                                                        dataTableAlunos.Load(drUsuarios);

                                                        alunosInclusao.AddRange(alunos
                                                            .Where(a => !dataTableAlunos.Select($"usu_login = 'RA{a.codigoAluno}'").Any())
                                                            .ToList());

                                                        alunosSemGrupo.AddRange(dataTableAlunos
                                                            .Select($"gru_id IS NULL")
                                                            .Select(r => r["usu_id"].ToString()));
                                                    }

                                                    if (alunosInclusao.Any() || alunosSemGrupo.Any())
                                                    {
                                                        using (var transaction = connCoreSSO.BeginTransaction())
                                                        {
                                                            var query = @"INSERT INTO PES_Pessoa (pes_id, pes_nome, pes_dataNascimento, pes_situacao, pes_dataCriacao, pes_dataAlteracao, pes_integridade)
                                                                          VALUES (@pes_id, @pes_nome, @pes_dataNascimento, 1, GETDATE(), GETDATE(), 0);
                                                                          INSERT INTO SYS_Usuario (usu_id, usu_login, usu_senha, usu_criptografia, usu_situacao, usu_dataCriacao, usu_dataAlteracao, pes_id, usu_integridade, ent_id, usu_integracaoAD, usu_dataAlteracaoSenha)
                                                                          VALUES(@usu_id, @login, @senha, 3, 1, GETDATE(), GETDATE(), @pes_id, 0, '6CF424DC-8EC3-E011-9B36-00155D033206', 0, GETDATE());
                                                                          INSERT INTO SYS_UsuarioGrupo (usu_id, gru_id, usg_situacao) VALUES(@usu_id, (SELECT TOP 1 gru_id FROM SYS_Grupo WHERE sis_id = 204 AND gru_situacao = 1 AND gru_nome = 'Aluno'), 1);";

                                                            try
                                                            {
                                                                Debug.WriteLine("Inclusão alunos:");

                                                                sqlQuery.Clear();
                                                                sqlQuery.AppendLine("DECLARE @alu_id BIGINT = (SELECT TOP 1 alu_id FROM ACA_Aluno ORDER BY alu_id DESC) + 1");
                                                                sqlQuery.AppendLine("INSERT INTO ACA_Aluno");
                                                                sqlQuery.AppendLine("SELECT DISTINCT @alu_id,");
                                                                sqlQuery.AppendLine("	             @alu_nome,");
                                                                sqlQuery.AppendLine("	             '6CF424DC-8EC3-E011-9B36-00155D033206',");
                                                                sqlQuery.AppendLine("	             @alu_matricula,");
                                                                sqlQuery.AppendLine("	             GETDATE(),");
                                                                sqlQuery.AppendLine("	             GETDATE(),");
                                                                sqlQuery.AppendLine("	             1,");
                                                                sqlQuery.AppendLine("	             NULL,");
                                                                sqlQuery.AppendLine("	             NULL,");
                                                                sqlQuery.AppendLine("	             @pes_id");
                                                                sqlQuery.AppendLine("INSERT INTO MTR_MatriculaTurma");
                                                                sqlQuery.AppendLine("SELECT TOP 1 @alu_id,");
                                                                sqlQuery.AppendLine("             1,");
                                                                sqlQuery.AppendLine("	          e.esc_id,");
                                                                sqlQuery.AppendLine("	          t.tur_id,");
                                                                sqlQuery.AppendLine("	          @cur_id,");
                                                                sqlQuery.AppendLine("	          1,");
                                                                sqlQuery.AppendLine("	          tc.crp_id,");
                                                                sqlQuery.AppendLine("	          1,");
                                                                sqlQuery.AppendLine("	          GETDATE(),");
                                                                sqlQuery.AppendLine("	          GETDATE(),");
                                                                sqlQuery.AppendLine("	          @mtu_numeroChamada,");
                                                                sqlQuery.AppendLine("	          @mtu_dataMtricula,");
                                                                sqlQuery.AppendLine("	          NULL,");
                                                                sqlQuery.AppendLine("	          tc.tcp_id");
                                                                sqlQuery.AppendLine("FROM ESC_Escola e,");
                                                                sqlQuery.AppendLine("     TUR_Turma t,");
                                                                sqlQuery.AppendLine("     TUR_TurmaCurriculo tc");
                                                                sqlQuery.AppendLine("WHERE e.esc_codigo = @esc_codigo AND");
                                                                sqlQuery.AppendLine("      t.tur_codigo = @tur_codigo AND");
                                                                sqlQuery.AppendLine("      tc.cur_id = @cur_id AND");
                                                                sqlQuery.AppendLine("      t.esc_id = e.esc_id AND");
                                                                sqlQuery.AppendLine("      t.tur_id = tc.tur_id AND");
                                                                sqlQuery.AppendLine("      t.cal_id = @cal_id");
                                                                sqlQuery.AppendLine("ORDER BY t.tur_id");

                                                                foreach (var alunoInclusao in alunosInclusao)
                                                                {
                                                                    var pes_id = Guid.NewGuid();
                                                                    var usu_id = Guid.NewGuid();
                                                                    var login = string.Concat("RA", alunoInclusao.codigoAluno);
                                                                    var senha = CriptografarSenhaSHA512(alunoInclusao.codigoAluno.ToString().PadLeft(4, '0').Substring(alunoInclusao.codigoAluno.ToString().PadLeft(4, '0').Length - 4, 4));

                                                                    Debug.WriteLine(login);

                                                                    commandCoreSSO = new SqlCommand(query.ToString(), connCoreSSO, transaction);
                                                                    commandCoreSSO.Parameters.AddWithValue("@pes_id", pes_id);
                                                                    commandCoreSSO.Parameters.AddWithValue("@pes_nome", alunoInclusao.nomeAluno);
                                                                    commandCoreSSO.Parameters.AddWithValue("@pes_dataNascimento", alunoInclusao.dataNascimento);
                                                                    commandCoreSSO.Parameters.AddWithValue("@usu_id", usu_id);
                                                                    commandCoreSSO.Parameters.AddWithValue("@login", login);
                                                                    commandCoreSSO.Parameters.AddWithValue("@senha", senha);
                                                                    commandCoreSSO.ExecuteNonQuery();

                                                                    var sqlQueryMatricula = new StringBuilder();
                                                                    sqlQueryMatricula.AppendLine("SELECT cd_aluno,");
                                                                    sqlQueryMatricula.AppendLine("       dt_status_matricula,");
                                                                    sqlQueryMatricula.AppendLine("       st_matricula");
                                                                    sqlQueryMatricula.AppendLine("    FROM MatriculasEjaEol");
                                                                    sqlQueryMatricula.AppendLine("WHERE cd_aluno = @cd_aluno");

                                                                    var dadosMatricula = new List<Matricula>();
                                                                    var dataMatricula = alunoInclusao.dataSituacao;
                                                                    using (var command = new SqlCommand(sqlQueryMatricula.ToString(), conGestaoAvaliacao, transactionGestaoAvaliacao))
                                                                    {
                                                                        command.Parameters.AddWithValue("@cd_aluno", alunoInclusao.codigoAluno);
                                                                        using (var dr = command.ExecuteReader())
                                                                        {
                                                                            while (dr.Read())
                                                                            {
                                                                                dadosMatricula.Add(new Matricula()
                                                                                {
                                                                                    cd_aluno = dr.GetDouble(0),    
                                                                                    dt_status_matricula = Convert.ToDateTime(dr.GetString(1)),
                                                                                    st_matricula = dr.GetDouble(2)
                                                                                });
                                                                            }
                                                                        }

                                                                        if (dadosMatricula.Any())
                                                                        {
                                                                            if (dadosMatricula.Count == 1)
                                                                                dataMatricula = dadosMatricula.Single().dt_status_matricula;
                                                                            else if (dadosMatricula.Count > 1 && dadosMatricula.Any(x => x.st_matricula == 1))
                                                                            {
                                                                                dataMatricula = dadosMatricula.Where(x => x.st_matricula == 1)
                                                                                    .OrderBy(x => x.dt_status_matricula)
                                                                                    .First().dt_status_matricula;
                                                                            }
                                                                            else
                                                                            {
                                                                                dataMatricula = dadosMatricula
                                                                                    .OrderBy(x => x.dt_status_matricula)
                                                                                    .First().dt_status_matricula;
                                                                            }
                                                                        }
                                                                    }

                                                                    using (var commandGestaoAvaliacaoSgp = new SqlCommand(sqlQuery.ToString(), conGestaoAvaliacao_SGP, transactionGestaoAvaliacaoSgp))
                                                                    {
                                                                        commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@alu_nome", alunoInclusao.nomeAluno);
                                                                        commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@alu_matricula", alunoInclusao.codigoAluno);
                                                                        commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@pes_id", pes_id);
                                                                        commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@cur_id", cur_id);                                                                        
                                                                        commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@mtu_numeroChamada", alunoInclusao.numeroChamada);
                                                                        commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@mtu_dataMtricula", dataMatricula);
                                                                        commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@esc_codigo", dadosCompletosTurma.CodigoEscola.ToString().PadLeft(6, '0'));
                                                                        commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@tur_codigo", string.Concat("EJA-", dadosCompletosTurma.DescricaoTurma));
                                                                        commandGestaoAvaliacaoSgp.Parameters.AddWithValue("@cal_id", idCalendarioAnual);                                                                        
                                                                        commandGestaoAvaliacaoSgp.ExecuteNonQuery();
                                                                    }
                                                                }

                                                                Debug.WriteLine("Inclusão de grupos:");
                                                                foreach (var item in alunosSemGrupo)
                                                                {
                                                                    Debug.WriteLine(item);
                                                                    commandCoreSSO = new SqlCommand($"INSERT INTO SYS_UsuarioGrupo (usu_id, gru_id, usg_situacao) VALUES({item}, (SELECT TOP 1 gru_id FROM SYS_Grupo WHERE sis_id = 204 AND gru_situacao = 1 AND gru_nome = 'Aluno'), 1);", connCoreSSO, transaction);
                                                                    commandCoreSSO.ExecuteNonQuery();
                                                                }
                                                                transaction.Commit();
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                transaction.Rollback();
                                                                MessageBox.Show(string.Concat("Erro: ", ex.Message));
                                                                throw;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        transactionGestaoAvaliacaoSgp.Commit();
                                        transactionGestaoAvaliacao.Commit();
                                        conGestaoAvaliacao_SGP.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        transactionGestaoAvaliacaoSgp.Rollback();
                                        transactionGestaoAvaliacao.Rollback();
                                        MessageBox.Show(string.Concat("Erro: ", ex.Message));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            progressBarImportAlunos.Style = ProgressBarStyle.Continuous;
            progressBarImportAlunos.MarqueeAnimationSpeed = 0;
            MessageBox.Show("Processo finalizado!", "Finalizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string Decrypt(string value)
        {
            var cripto = new MSTech.Security.Cryptography.SymmetricAlgorithm(MSTech.Security.Cryptography.SymmetricAlgorithm.Tipo.TripleDES);
            return cripto.Decrypt(value);
        }

        public string CriptografarSenhaSHA512(string senha)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                return Convert.ToBase64String(sha512.ComputeHash(Encoding.Unicode.GetBytes(senha))).TrimStart('/');
            }
        }

        private int ObterIdCalendario(string descricaoTipoPeridiocidade, SqlConnection conexaoGestaoAvaliacao, SqlTransaction transaction)
        {
            var sqlQuery = new StringBuilder();
            sqlQuery.AppendLine("SELECT TOP 1 cal_id");
            sqlQuery.AppendLine("    FROM SGP_ACA_CalendarioAnual");
            sqlQuery.AppendLine("WHERE cal_ano = @cal_ano AND");
            sqlQuery.AppendLine("      cal_descricao LIKE '%EJA%' AND");

            if (descricaoTipoPeridiocidade.Contains("SEMESTRAL") && descricaoTipoPeridiocidade.Contains("1º SEMESTRE"))
                sqlQuery.AppendLine("cal_descricao LIKE '%1° Semestre%'");
            else if (descricaoTipoPeridiocidade.Contains("SEMESTRAL") && descricaoTipoPeridiocidade.Contains("2º SEMESTRE"))
                sqlQuery.AppendLine("cal_descricao LIKE '%2° Semestre%'");
            else
            {
                sqlQuery.AppendLine("cal_descricao NOT LIKE '%1° Semestre%' AND");
                sqlQuery.AppendLine("cal_descricao NOT LIKE '%2° Semestre%'");
            }

            using (var command = new SqlCommand(sqlQuery.ToString(), conexaoGestaoAvaliacao, transaction))
            {
                command.Parameters.AddWithValue("@cal_ano", numericUpDownImpAlunoAnoLetivo.Value);
                return (int)command.ExecuteScalar();
            }
        }


        public struct ItemTipoEscola
        {
            public int Id { get; set; }
            public string Descricao { get; set; }
        }

        public struct Turma
        {
            public long codigoTurma { get; set; }
            public string nomeTurma { get; set; }
            public string TipoTurma { get; set; }
            public string Situacao { get; set; }
            public DateTime dataInicioTurma { get; set; }
            public DateTime dataFimTurma { get; set; }
        }

        public struct Aluno
        {
            public int codigoAluno { get; set; }
            public string nomeAluno { get; set; }
            public DateTime dataNascimento { get; set; }
            public string nomeSocialAluno { get; set; }
            public int codigoSituacaoMatricula { get; set; }
            public string situacaoMatricula { get; set; }
            public DateTime dataSituacao { get; set; }
            public int numeroChamada { get; set; }
            public bool possuiDeficiencia { get; set; }
        }

        public struct DadosCompletosTurma
        {
            public long CodigoEscola { get; set; }
            public long CodigoTurma { get; set; }
            public string DescricaoTurma { get; set; }
            public int AnoLetivo { get; set; }
            public string DescricaoTipoPeridiocidade { get; set; }
            public string DescricaoTipoTurno { get; set; }
            public string SituacaoTurma { get; set; }
            public string TipoTurma { get; set; }
            public DateTime DataInicioTurma { get; set; }
            public DateTime DataFimTurma { get; set; }
            public int NumeroOrdemSerie { get; set; }
            public int CodigoModalidadeEnsino { get; set; }
            public int CodigoEtapaEnsino { get; set; }
            public int NumeroOrdemEtapa { get; set; }
            public string DescricaoSerieEnsino { get; set; }
            public string SiglaModalidadeEnsino { get; set; }
            public string TipoEscola { get; set; }

        }

        public struct Prova
        {
            public long Id { get; set; }
            public string Descricao { get; set; }
        }

        public struct Matricula
        {
            public double cd_aluno { get; set; }
            public DateTime dt_status_matricula { get; set; }
            public double st_matricula { get; set; }
        }
    }
}
