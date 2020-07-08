using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionario;
using ImportacaoDeQuestionariosSME.Services.FatoresAssociadosQuestionario.Dtos;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ImportacaoDeQuestionariosSME.Forms.FatoresAssociadosQuestionario
{
    public partial class ImportacaoDeFatorAssociadoQuestionario : Form
    {
        private readonly IFatorAssociadoQuestionarioServices _fatorAssociadoQuestionarioServices;

        public ImportacaoDeFatorAssociadoQuestionario()
        {
            _fatorAssociadoQuestionarioServices = new FatorAssociadoQuestionarioServices();
            InitializeComponent();
        }

        private async void btnImportar_Click(object sender, EventArgs e)
        {
            var dto = new ImportacaoDeFatorAssociadoQuestionarioDto
            {
                Edicao = txtAno.Text
            };

            await _fatorAssociadoQuestionarioServices.ImportarAsync(dto);

            if (!dto.IsValid())
            {
                MessageBox.Show(dto.Erros.FirstOrDefault(), "Erro na importação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Importação realizada com sucesso.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ImportacaoDeFatorAssociadoQuestionario_Shown(object sender, EventArgs e)
        {
            txtAno.Text = DateTime.Now.Year.ToString();
            txtAno.Focus();
        }

        private void btnFechar_Click(object sender, EventArgs e) => Close();
    }
}