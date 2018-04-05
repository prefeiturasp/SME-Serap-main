using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SetupInstaller
{
	public partial class ReadWebConfig : Form
	{
		public ReadWebConfig()
		{
			InitializeComponent();

			XmlDocument doc = new XmlDocument();
			doc.Load(@"C:\Projetos\GestaoAvaliacao\GestaoAvaliacao\Development\Dev\Src\GestaoAvaliacao.WebSocket\bin\Debug\GestaoAvaliacao.WebSocket.exe.config");
			XmlNode xnodes = doc.SelectSingleNode("/configuration/appSettings");

			int y = 10;
			foreach (XmlNode item in xnodes.ChildNodes)
			{
				if ("WebSocketUrl".Equals(item.Attributes[0].Value) || "MongoDB_Connection".Equals(item.Attributes[0].Value)
					|| "MongoDB_Database".Equals(item.Attributes[0].Value))
				{
					Label lbl = new Label() { Text = item.Attributes[0].Value, Width = 200 };
					TextBox txt = new TextBox() { Text = item.Attributes[1].Value, Width = 300 };
					y += 50;

					flowPanel.Controls.Add(lbl);
					flowPanel.Controls.Add(txt);
				}
			}
		}
	}
}
