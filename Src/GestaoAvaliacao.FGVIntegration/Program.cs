using System.Windows.Forms;

namespace GestaoAvaliacao.FGVIntegration
{
    static class Program
	{
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new IntegrationForm());
		}
	}
}
