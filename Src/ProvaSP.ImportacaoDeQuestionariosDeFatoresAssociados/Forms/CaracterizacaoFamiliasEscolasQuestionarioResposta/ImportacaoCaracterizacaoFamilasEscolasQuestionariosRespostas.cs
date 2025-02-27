using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ImportacaoDeQuestionariosSME.Domain.Enums;
using ImportacaoDeQuestionariosSME.Services.CaracterizacaoFamiliasEscolasQuestionarioResposta.Factories;
using ImportacaoDeQuestionariosSME.Services.CaracterizacaoFamiliasEscolasQuestionarioResposta.Dtos;

namespace ImportacaoDeQuestionariosSME.Forms.CaracterizacaoFamiliasEscolasQuestionarioResposta
{
    public partial class ImportacaoCaracterizacaoFamilasEscolasQuestionariosRespostas : Form
    {
        private readonly ICaracterizacaoFamiliasEscolasQuestionarioRespostaFactory _caracterizacaoFamiliasEscolasQuestionarioRespostaFactory;

        public ImportacaoCaracterizacaoFamilasEscolasQuestionariosRespostas()
        {
            InitializeComponent();
            _caracterizacaoFamiliasEscolasQuestionarioRespostaFactory = new CaracterizacaoFamiliasEscolasQuestionarioRespostaFactory();
        }

        private void ImportacaoCaracterizacaoFamilasEscolasQuestionariosRespostas_Load(object sender, EventArgs e)
        {
            if (cmbEdicao.Items.Count > 0)
                cmbEdicao.SelectedIndex = 0;

            PreencherFatoresAssociados();
        }

        private void btnFechar_Click(object sender, EventArgs e) => Close();

        private void PreencherFatoresAssociados()
        {
            cmbFatorAssociadoQuestionario.Items.Clear();

            var questionarios = new Dictionary<int, string>
            {
                { 10, "Estudante" },
                { 11, "Família" }
            };

            cmbFatorAssociadoQuestionario.DataSource = new BindingSource(questionarios, null);
            cmbFatorAssociadoQuestionario.DisplayMember = "Value";
            cmbFatorAssociadoQuestionario.ValueMember = "Key";
            cmbFatorAssociadoQuestionario.SelectedIndex = 0;
        }

        private void btnLocalizarArquivo_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Abrir arquivo CSV";
            dialog.Filter = "CSV Files (*.csv)|*.csv";
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

        private TipoQuestionarioEnum obterTipoQuestionario()
        {
            if (rdbSME.Checked)
                return TipoQuestionarioEnum.TipoQuestionarioSme;

            if (rdbDRE.Checked)
                return TipoQuestionarioEnum.TipoQuestionarioDre;

            return rdbEscola.Checked ? TipoQuestionarioEnum.TipoQuestionarioEscola : TipoQuestionarioEnum.Nenhum;
        }

        private async void btnImportar_Click(object sender, EventArgs e)
        {
            var service = _caracterizacaoFamiliasEscolasQuestionarioRespostaFactory.Create(obterTipoQuestionario());
            if (service is null)
            {
                MessageBox.Show("Não foi possível criar o serviço para a importação.", "Erro na importação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var fatorAssociadoQuestionarioId = ((KeyValuePair<int, string>)cmbFatorAssociadoQuestionario.SelectedItem).Key;

            var dto = new CaracterizacaoFamiliasEscolasQuestionarioRespostaDto(cmbEdicao.Text,
                fatorAssociadoQuestionarioId, obterTipoQuestionario(), txtArquivo.Text);

            await service.ImportarAsync(dto);

            if (!dto.IsValid())
            {
                MessageBox.Show(dto.Erros.FirstOrDefault(), "Erro na importação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Importação realizada com sucesso.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void rdbSME_CheckedChanged(object sender, EventArgs e)
        {
            txtArquivo.Text = string.Empty;
        }

        private void rdbDRE_CheckedChanged(object sender, EventArgs e)
        {
            txtArquivo.Text = string.Empty;
        }

        private void rdbEscola_CheckedChanged(object sender, EventArgs e)
        {
            txtArquivo.Text = string.Empty;
        }
    }
}
