using ImportacaoDeQuestionariosSME.Services.Constructos;
using ImportacaoDeQuestionariosSME.Services.Constructos.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportacaoDeQuestionariosSME.Forms.Constructos
{
    public partial class ImportacaoDeConstructos : Form
    {
        private readonly IConstructoServices _constructoServices;

        public ImportacaoDeConstructos()
        {
            _constructoServices = new ConstructoServices();
            InitializeComponent();
        }

        private async void btnImportar_Click(object sender, EventArgs e)
        {
            var dto = new ImportacaoDeConstructosDto
            {
                Edicao = txtAno.Text
            };

            await _constructoServices.ImportarAsync(dto);

            if (!dto.IsValid())
            {
                MessageBox.Show(dto.Erros.FirstOrDefault(), "Erro na importação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Importação realizada com sucesso.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnFechar_Click(object sender, EventArgs e) => Close();
    }
}