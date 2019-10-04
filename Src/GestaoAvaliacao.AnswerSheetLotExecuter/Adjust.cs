using Castle.Windsor;
using GestaoAvaliacao.MappingDependence;
using GestaoAvaliacao.Services;
using System;
using System.Windows.Forms;

namespace GestaoAvaliacao.AnswerSheetLotExecuter
{
    public partial class Adjust : Form
    {
        private readonly IWindsorContainer container;

        public Adjust()
        {
            container = new WindsorContainer()
                .Install(new BusinessInstaller() { LifestylePerWebRequest = false })
                .Install(new RepositoriesInstaller() { LifestylePerWebRequest = false })
                .Install(new StorageInstaller() { LifestylePerWebRequest = false })
                .Install(new PDFConverterInstaller() { LifestylePerWebRequest = false })
                .Install(new ServiceContainerInstaller());

            InitializeComponent();
        }

        private async void ButtonUpdateAnswers_Click(object sender, System.EventArgs e)
        {
            try
            {
                var service = container.Resolve<StudentCorrection>();

                var testId = (long)numericUpDownTestId.Value;
                var teamId = numericUpDownTeamId.Value > 0 ? (long?)numericUpDownTeamId.Value : null;
                var itemIdOld = (long)numericUpDownItemIdOld.Value;
                var itemIdNew = numericUpDownItemIdNew.Value > 0 ? (long?)numericUpDownItemIdNew.Value : null;
                var alternativeIdOld = (long)numericUpDownAlternativeIdOld.Value;
                var alternativeIdNew = numericUpDownAlternativeIdNew.Value > 0 ? (long?)numericUpDownAlternativeIdNew.Value : null;

                await service.UpdateAnswersTest(testId, teamId, itemIdOld, itemIdNew, alternativeIdOld, alternativeIdNew, checkBoxAnswerCorret.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Concat("Erro ao realizar os ajustes.\n", ex.Message));
            }            
        }
    }
}
