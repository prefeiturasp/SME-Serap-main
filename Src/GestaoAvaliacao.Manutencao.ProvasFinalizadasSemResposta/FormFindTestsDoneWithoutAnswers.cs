using GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta
{
    public partial class FormFindTestsDoneWithoutAnswers : Form
    {
        private readonly IFindTestsDoneWithoutAnswersServices _findTestsDoneWithoutAnswersServices;

        public FormFindTestsDoneWithoutAnswers()
        {
            _findTestsDoneWithoutAnswersServices = new FindTestsDoneWithoutAnswersServices();
            InitializeComponent();
        }

        private async void btnFind_Click(object sender, EventArgs e)
        {
            EnableComponents(false);

            try
            {
                var results = await _findTestsDoneWithoutAnswersServices.FindStudentsWithNoAnswersAsync(dtpUpdateDateStart.Value);
                if (!results?.Any() ?? true)
                {
                    MessageBox.Show("Nenhuma prova sem resposta encontrada");
                    EnableComponents(true);
                }

                dtgResults.DataSource = results;
                dtgResults.Refresh();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.InnerException?.Message ?? ex.Message);
            }

            EnableComponents(true);
        }

        private void EnableComponents(bool enable)
        {
            btnFind.Enabled = enable;
            dtpUpdateDateStart.Enabled = enable;
        }
    }
}
