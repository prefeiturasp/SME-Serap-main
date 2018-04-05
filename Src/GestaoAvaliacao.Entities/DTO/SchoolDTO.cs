using System;

namespace GestaoAvaliacao.Entities.DTO
{
    [Serializable]
    public class SchoolDTO
    {
        public Guid dre_id { get; set; }
        public int esc_id { get; set; }
    }
}
