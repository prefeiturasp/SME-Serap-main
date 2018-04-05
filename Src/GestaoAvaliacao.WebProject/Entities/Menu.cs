using System.Collections.Generic;

namespace GestaoAvaliacao.WebProject.Entities
{
    public class Menu
	{
		public string Id { get; set; }
		public int Ordem { get; set; }
		public string Url { get; set; }
		public List<Menu> Itens { get; set; }
		public string Icon { get; set; }
	}
}
