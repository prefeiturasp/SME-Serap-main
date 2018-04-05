using GestaoAvaliacao.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class TestSectionStatusCorrection : EntityBase
    {
        public virtual Test Test { get; set; }
        public long Test_Id { get; set; }
        public long tur_id { get; set; }
        public Util.EnumStatusCorrection StatusCorrection { get; set; }
        [NotMapped]
        public Guid idDRE { get; set; }
        [NotMapped]
        public string DRE { get; set; }
        [NotMapped]
        public int esc_id { get; set; }
        [NotMapped]
        public string esc_nome { get; set; }
        [NotMapped]
        public string tur_codigo { get; set; }
    }
}
