using System.Windows.Forms;

namespace SetupInstaller
{
	public static class Program
	{
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new SetupWebSocket());
		}
	}
}
