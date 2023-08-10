using GestaoAvaliacao.Entities.Base;

namespace GestaoAvaliacao.Entities
{
    public class ReportStudies : EntityBase
    {
        public virtual string Name { get; set; }

        public virtual int TypeGroup { get; set; }

        public virtual string Addressee { get; set; }

        public virtual string Link { get; set; }

    }
}
