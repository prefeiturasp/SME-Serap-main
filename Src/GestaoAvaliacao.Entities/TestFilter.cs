
using GestaoAvaliacao.Util;
using System;
namespace GestaoAvaliacao.Entities
{
    public class TestFilter
    {
        public TestFilter()
        {
        }

        public virtual int? TestId { get; set; }

        public virtual int? TestType { get; set; }

        public virtual long? DisciplineId { get; set; }

        public virtual int? FrequencyApplication { get; set; }

        public virtual string CreationDateStart { get; set; }

        public virtual string CreationDateEnd { get; set; }

        public virtual bool Pendente { get; set; }

        public virtual bool Cadastrada { get; set; }

        public virtual bool Andamento { get; set; }

        public virtual bool Aplicada { get; set; }

        public virtual Guid ent_id { get; set; }

        public virtual Guid pes_id { get; set; }

        public virtual Guid usuId { get; set; }

        public virtual Guid gru_id { get; set; }

        public virtual EnumSYS_Visao vis_id { get; set; }

        public virtual bool? global { get; set; }

        public virtual string dre_id { get; set; }

        public virtual int esc_id { get; set; }

        public int ttn_id { get; set; }

        public string tne_id_ordem { get; set; }
        public DateTime? ApplicationStartDate { get; set; }
        public DateTime? ApplicationEndDate { get; set; }
        public DateTime? CorrectionEndDate { get; set; }
        public DateTime? CorrectionStartDate { get; set; }
        public bool? visibleTest { get; set; }
        public bool? visibleMultidiscipline { get; set; }
        public bool getGroup { get; set; }
        public long? TestGroupId { get; set; }
        public long? TestSubGroupId { get; set; }
        public short ordenacao { get; set; }

    }
}
