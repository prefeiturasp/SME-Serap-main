using ImportacaoDeQuestionariosSME.Services.ImagensDeAlunos;
using ImportacaoDeQuestionariosSME.Services.ImagensDeAlunos.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImportacaoDeQuestionariosSME.Forms.ImagensDeRedacoes
{
    public partial class ImportacaoDeImagensDeRedacoes : Form
    {
        private readonly IImagemAlunoServices _imagemAlunoServices;

        public ImportacaoDeImagensDeRedacoes()
        {
            _imagemAlunoServices = new ImagemAlunoServices();
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, EventArgs e) => Close();

        private async void btnImportar_Click(object sender, EventArgs e)
        {
            var dto = new ImportacaoDeImagemAlunoDto
            {
                Ano = txtAno.Text,
                CaminhoDaPlanilha = txtArquivo.Text
            };

            MessageBox.Show(txtArquivo.Text);

            await _imagemAlunoServices.ImportarAsync(dto);

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