using ImportacaoDeQuestionariosSME.Services.FatoresAssociados;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.DRE.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioResposta.Factory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ImportacaoDeQuestionariosSME.Forms.FatoresAssociadosQuestionarioResposta
{
    public partial class ImportacaoDeQuestionariosDeFatoresAssociadosQuestionarios : Form
    {
        private IEnumerable<QuestaoConstructoDto> _questaoConstructoDtos;
        private readonly IFatorAssociadoServices _fatorAssociadoServices;
        private readonly IImportacaoDeQuestionariosDeFatoresAssociadosFactory _importacaoDeQuestionariosDeFatoresAssociadosFactory;

        public ImportacaoDeQuestionariosDeFatoresAssociadosQuestionarios()
        {
            _fatorAssociadoServices = new FatorAssociadoServices();
            _importacaoDeQuestionariosDeFatoresAssociadosFactory = new ImportacaoDeQuestionariosDeFatoresAssociadosFactory();
            InitializeComponent();
        }


        private void btnFechar_Click(object sender, EventArgs e) => Close();

        private void ImportacaoDeQuestionariosDeFatoresAssociados_Shown(object sender, EventArgs e)
        {
            txtAno.Text = DateTime.Now.Year.ToString();
            cmbQuestionario.SelectedIndex = 0;

            txtAno.Focus();
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
            if (_questaoConstructoDtos is null || !_questaoConstructoDtos.Any())
            {
                MessageBox.Show("Importe a tabela de fatores associados antes.", "Erro na importação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var service = _importacaoDeQuestionariosDeFatoresAssociadosFactory.Create(rdbDRE.Checked, rdbEscola.Checked, rdbSME.Checked, _questaoConstructoDtos);
            if (service is null)
            {
                MessageBox.Show("Não foi possível criar o serviço para a importação.", "Erro na importação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var questionario = cmbQuestionario.SelectedItem.ToString();
            var questionarioId = int.Parse(questionario.Substring(0, questionario.IndexOf(" -")));

            var dto = new ImportacaoDeQuestionariosDeFatoresAssociadosDto
            {
                Edicao = txtAno.Text,
                CaminhoDaPlanilha = txtArquivo.Text,
                QuestionarioId = questionarioId
            };

            await service.ImportarAsync(dto);

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
            if(string.IsNullOrWhiteSpace(txtArquivoFatoresAssociados.Text))
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
            catch(Exception ex)
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
    }
}