using ImportacaoDeQuestionariosSME.Data.Repositories.Alunos;
using ImportacaoDeQuestionariosSME.Data.Repositories.Alunos.Dtos;
using ImportacaoDeQuestionariosSME.Domain.Alunos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociados;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Factory;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.SME;
using ImportacaoDeQuestionariosSME.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportacaoDeQuestionariosSME.Forms.FatoresAssociadosQuestionarioResposta
{
    public partial class ImportacaoDeQuestionariosDeFatoresAssociadosQuestionariosFamilia : Form
    {
        private readonly IFatorAssociadoServices _fatorAssociadoServices;
        private readonly IFatoresAssociadosQuestionarioRespostaFamiliaServicesFactory _fatoresAssociadosQuestionarioRespostaFamiliaServicesFactory;
        private IEnumerable<QuestaoConstructoDto> _questaoConstructoDtos;
        private DataTable _dtRespostas;

        public ImportacaoDeQuestionariosDeFatoresAssociadosQuestionariosFamilia()
        {
            _fatoresAssociadosQuestionarioRespostaFamiliaServicesFactory = new FatoresAssociadosQuestionarioRespostaFamiliaServicesFactory();
            _fatorAssociadoServices = new FatorAssociadoServices();
            InitializeComponent();
        }

        private void ImportacaoDeQuestionariosDeFatoresAssociadosQuestionariosFamilia_Shown(object sender, EventArgs e)
        {
            txtAno.Text = "2019";
        }

        private void btnLocalizarTabela_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.ShowDialog();

            if (string.IsNullOrWhiteSpace(dialog.FileName)) return;
            if (!dialog.FileName.EndsWith(".csv"))
            {
                MessageBox.Show("Selecione um arquivo tipo CSV.", "Formato inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            txtArquivo.Text = dialog.FileName;
        }

        private async void btnImportar_Click(object sender, EventArgs e)
        {
            var dto = new ImportacaoDeQuestionariosDeFatoresAssociadosFamiliaDto
            {
                Edicao = txtAno.Text,
                CaminhoDaPlanilhaQuesitonarios = txtArquivoQuestionario.Text
            };

            if (_dtRespostas is null || _dtRespostas.Rows.Count <= 0)
            {
                _dtRespostas = CsvManager.GetCsvFile(txtArquivo.Text);
                if (_dtRespostas.Rows.Count <= 0)
                {
                    MessageBox.Show("Não existem regitros na planilha para exportação.");
                    return;
                }
            }

            var service = _fatoresAssociadosQuestionarioRespostaFamiliaServicesFactory.Create(rdbDRE.Checked, rdbEscola.Checked, rdbSME.Checked, _dtRespostas);
            await service.ImportarAsync(dto, _questaoConstructoDtos);

            if (!dto.IsValid())
            {
                MessageBox.Show(dto.Erros.FirstOrDefault(), "Erro na importação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Importação realizada com sucesso.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLocalizarTabelaFatoresAssociados_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.ShowDialog();

            if (string.IsNullOrWhiteSpace(dialog.FileName)) return;
            if (!dialog.FileName.EndsWith(".csv"))
            {
                MessageBox.Show("Selecione um arquivo tipo CSV.", "Formato inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            txtArquivoFatoresAssociados.Text = dialog.FileName;
        }

        private void btnImportarFatoresAssociados_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtArquivoFatoresAssociados.Text))
            {
                MessageBox.Show("Informe o caminho da planilha.", "Erro na importação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnImportarFatoresAssociados.PerformClick();
                return;
            }

            try
            {
                var dtFatoresAssociados = _fatorAssociadoServices.GetTabelaDeFatoresAssociadosAjustada(txtArquivoFatoresAssociados.Text);
                _questaoConstructoDtos = FilterTableFatoresAssociados(dtFatoresAssociados);
                MessageBox.Show("Importação realizada com sucesso.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Erro na importação", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IEnumerable<QuestaoConstructoDto> FilterTableFatoresAssociados(DataTable dtFatoresAssociados)
        {
            var questaoConstructoDtos = new List<QuestaoConstructoDto>();

            foreach (DataRow row in dtFatoresAssociados.Rows)
            {
                for (var indiceQuestao = 1; indiceQuestao <= 21; indiceQuestao++)
                {
                    int.TryParse(row[$"num_questao_{indiceQuestao}"].ToString(), out var questao);
                    if (questao <= 0) break;
                    var constructoDescricao = row["ID"].ToString();

                    if (questaoConstructoDtos.Any(x => x.Questao == questao && x.ConstructoDescricao == constructoDescricao)) continue;

                    questaoConstructoDtos.Add(new QuestaoConstructoDto
                    {
                        ConstructoDescricao = constructoDescricao,
                        Questao = questao
                    });
                }
            }

            return questaoConstructoDtos;
        }

        private void btnLocalizarTabelaQuestionario_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.ShowDialog();

            if (string.IsNullOrWhiteSpace(dialog.FileName)) return;
            if (!dialog.FileName.EndsWith(".csv"))
            {
                MessageBox.Show("Selecione um arquivo tipo CSV.", "Formato inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            txtArquivoQuestionario.Text = dialog.FileName;
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnProcessarAnosEscolares_Click(object sender, EventArgs e)
        {
            var dtRespostas = CsvManager.GetCsvFile(txtArquivo.Text);
            if (dtRespostas.Rows.Count <= 0)
            {
                MessageBox.Show("Não existem regitros na planilha para exportação.");
                return;
            }

            var dtMatriculaAnoEscolar = CsvManager.GetCsvFile(txtArquivoAnoEscolar.Text);
            if (dtMatriculaAnoEscolar.Rows.Count <= 0)
            {
                MessageBox.Show("Não existem regitros na planilha para exportação.");
                return;
            }

            var matriculasAnoEscolar = dtMatriculaAnoEscolar
                .AsEnumerable()
                .ToDictionary
                (row => int.Parse(row["Matricula"].ToString()),
                 row => int.Parse(row["AnoEscolar"].ToString())
                );

            dtRespostas.Columns.Add("AnoEscolar", typeof(int));

            foreach (DataRow row in dtRespostas.Rows)
            {
                matriculasAnoEscolar.TryGetValue(int.Parse(row["CL_ALU_CODIGO"].ToString()), out var anoEscolar);
                if(anoEscolar <= 0)
                {

                }
                row["AnoEscolar"] = anoEscolar;
            }

            dtRespostas.ToCSV(txtArquivo.Text);
        }

        private void btnLocalizarTabelaAnoEscolar_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.ShowDialog();

            if (string.IsNullOrWhiteSpace(dialog.FileName)) return;
            if (!dialog.FileName.EndsWith(".csv"))
            {
                MessageBox.Show("Selecione um arquivo tipo CSV.", "Formato inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            txtArquivoAnoEscolar.Text = dialog.FileName;
        }
    }
}
