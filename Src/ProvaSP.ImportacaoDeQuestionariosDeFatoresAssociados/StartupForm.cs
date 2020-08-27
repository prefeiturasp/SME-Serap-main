using ImportacaoDeQuestionariosSME.Forms.Constructos;
using ImportacaoDeQuestionariosSME.Forms.FatoresAssociados;
using ImportacaoDeQuestionariosSME.Forms.FatoresAssociadosQuestionario;
using ImportacaoDeQuestionariosSME.Forms.FatoresAssociadosQuestionarioResposta;
using ImportacaoDeQuestionariosSME.Forms.ImagensDeRedacoes;
using System;
using System.Windows.Forms;

namespace ImportacaoDeQuestionariosSME
{
    public partial class StartupForm : Form
    {
        public StartupForm()
        {
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, EventArgs e) => Close();

        private void btnFatoresAssociadosQuestionariosRespostas_Click(object sender, EventArgs e)
        {
            var form = new ImportacaoDeQuestionariosDeFatoresAssociadosQuestionarios();
            form.ShowDialog();
        }

        private void btnFatoresAssociadosQuestionarios_Click(object sender, EventArgs e)
        {
            var form = new ImportacaoDeFatorAssociadoQuestionario();
            form.ShowDialog();
        }

        private void btnFatoresAssociados_Click(object sender, EventArgs e)
        {
            var form = new ImportacaoDeFatoresAssociados();
            form.ShowDialog();
        }

        private void btnConstructos_Click(object sender, EventArgs e)
        {
            var form = new ImportacaoDeConstructos();
            form.ShowDialog();
        }

        private void btnImagensDasRedacoes_Click(object sender, EventArgs e)
        {
            var form = new ImportacaoDeImagensDeRedacoes();
            form.ShowDialog();
        }

        private void btnFatoresAssociadosQuestionariosFamiliaRespostas_Click(object sender, EventArgs e)
        {
            var form = new ImportacaoDeQuestionariosDeFatoresAssociadosQuestionariosFamilia();
            form.ShowDialog();
        }
    }
}