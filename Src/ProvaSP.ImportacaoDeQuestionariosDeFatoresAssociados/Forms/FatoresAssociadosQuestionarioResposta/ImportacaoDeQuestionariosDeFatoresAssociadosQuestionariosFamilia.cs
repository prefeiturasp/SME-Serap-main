using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Dtos;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionarioRespostaFamilia.Factory;
using ImportacaoDeQuestionariosSME.Utils;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ImportacaoDeQuestionariosSME.Forms.FatoresAssociadosQuestionarioResposta
{
    public partial class ImportacaoDeQuestionariosDeFatoresAssociadosQuestionariosFamilia : Form
    {
        private readonly IFatoresAssociadosQuestionarioRespostaFamiliaServicesFactory _fatoresAssociadosQuestionarioRespostaFamiliaServicesFactory;
        private DataTable _dtRespostas;

        public ImportacaoDeQuestionariosDeFatoresAssociadosQuestionariosFamilia()
        {
            _fatoresAssociadosQuestionarioRespostaFamiliaServicesFactory = new FatoresAssociadosQuestionarioRespostaFamiliaServicesFactory();
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

            if (string.IsNullOrWhiteSpace(dialog.FileName)) 
                return;

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
                    MessageBox.Show("Não existem regitros na planilha para importação.");
                    return;
                }
            }

            var service = _fatoresAssociadosQuestionarioRespostaFamiliaServicesFactory.Create(rdbDRE.Checked, rdbEscola.Checked, rdbSME.Checked, _dtRespostas);
            await service.ImportarAsync(dto);

            if (!dto.IsValid())
            {
                MessageBox.Show(dto.Erros.FirstOrDefault(), "Erro na importação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Importação realizada com sucesso.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
