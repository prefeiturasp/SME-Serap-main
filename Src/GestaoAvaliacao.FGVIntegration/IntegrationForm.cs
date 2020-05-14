using Castle.Windsor;
using GestaoAvaliacao.FGVIntegration.Business;
using GestaoAvaliacao.FGVIntegration.Logging;
using System;
using System.Windows.Forms;

namespace GestaoAvaliacao.FGVIntegration
{
    public partial class IntegrationForm : Form
    {
        private readonly IWindsorContainer container;
        private readonly ILogging logging;

        public IntegrationForm()
        {
            container = new WindsorContainer()
                .Install(new FGVEnsinoMedioInstaller());
            this.logging = container.Resolve<ILogging>();

            InitializeComponent();
        }

        private async void btSincEnsinoMedio_Click(object sender, EventArgs e)
        {
            try
            {
                var integracao = container.Resolve<IIntegracaoBusiness>();

                //string[] codigoEscolas = new string[] { /*"094609", "017442",*/"018210"/*,"094668","017272","093181","093637","016519",*/ };
                bool sucesso = await integracao.RealizarIntegracaoEnsinoMedio();

                MessageBox.Show("Integração finalizada. Verifique os logs em 'Log/logger.txt' para verificar possíveis problemas.", "Sucesso");
            }
            catch (Exception ex)
            {
                logging.Logger.Error("Ocorreu erro durante a integração.", ex);
                MessageBox.Show($"Ocorreu erro durante a integração. Verifique os logs em 'Log/logger.txt' para detalhes", "Erro");
            }
        }

    }
}