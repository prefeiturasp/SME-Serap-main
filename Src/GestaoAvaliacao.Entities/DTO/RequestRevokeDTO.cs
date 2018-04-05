using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Entities.Enumerator;
using System;

namespace GestaoAvaliacao.Entities.DTO
{
    public class RequestRevokeDTO : EntityBase
    {
        public Guid UsuId { get; set; }

        public string Justification { get; set; }

        public EnumSituation Situation { get; set; }

        public string pes_nome { get; set; }

        public string usu_email { get; set; }
    }
}
