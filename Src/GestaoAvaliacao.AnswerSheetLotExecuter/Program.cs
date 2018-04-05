using System.Windows.Forms;

namespace GestaoAvaliacao.AnswerSheetLotExecuter
{
    static class Program
	{
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new CallServices());
		}
	}
}
