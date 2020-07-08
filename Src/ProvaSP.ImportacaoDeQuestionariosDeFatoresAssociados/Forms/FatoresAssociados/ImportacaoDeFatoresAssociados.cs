using ImportacaoDeQuestionariosSME.Services.FatoresAssociados;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociados.Dtos;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ImportacaoDeQuestionariosSME.Forms.FatoresAssociados
{
    public partial class ImportacaoDeFatoresAssociados : Form
    {
        private readonly IFatorAssociadoServices _fatorAssociadoServices;

        public ImportacaoDeFatoresAssociados()
        {
            _fatorAssociadoServices = new FatorAssociadoServices();
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, EventArgs e) => Close();

        private async void btnImportar_Click(object sender, EventArgs e)
        {
            var dto = new ImportacaoDeFatoresAssociadosDto
            {
                Edicao = txtAno.Text,
                CaminhoDaPlanilha = txtArquivo.Text,
            };

            await _fatorAssociadoServices.ImportarAsync(dto);

            if (!dto.IsValid())
            {
                MessageBox.Show(dto.Erros.FirstOrDefault(), "Erro na importação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Importação realizada com sucesso.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}