using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Entities.Enumerator;
using System;

namespace GestaoAvaliacao.Entities
{
    public class RequestRevoke : EntityBase
    {
        public Guid UsuId { get; set; }

        public virtual Test Test { get; set; }
        public long Test_Id { get; set; }

        public EnumSituation Situation { get; set; }
        public string Justification { get; set; }

        public virtual BlockItem BlockItem { get; set; }
        public long BlockItem_Id { get; set; }

    }
}
