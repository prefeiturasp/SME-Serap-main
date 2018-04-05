using MSTech.CoreSSO.Entities;
using System.Collections.Generic;

namespace GestaoAvaliacao.WebProject.Entities
{
    public class UsuarioLogado
    {
        public string Nome { get; set; }

        public bool IsAdmin { get; internal set; }

        public SYS_Usuario Usuario { get; internal set; }

        public SYS_Grupo Grupo { get; internal set; }

        public IList<SYS_Sistema> Sistemas { get; internal set; }

        public long alu_id { get; set; }
    }
}
