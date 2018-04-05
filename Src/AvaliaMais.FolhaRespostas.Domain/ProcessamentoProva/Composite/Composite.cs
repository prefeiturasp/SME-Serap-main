using System.Collections.Generic;

namespace AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva
{
    public class Composite : Component
	{
		public List<Component> _unidades = new List<Component>();

		public override void Add(Component component)
		{
			_unidades.Add(component);
		}

		public override void Remove(Component component)
		{
			_unidades.Remove(component);
		}

		public bool PossuiFilhos()
		{
			return _unidades.Count > 0;
		}
	}
}
