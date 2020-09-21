using GestaoAvaliacao.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoAvaliacao.Entities
{
    public class Alternative : EntityBase
    {
        public virtual Item Item { get; set; }

        public virtual string Description { get; set; }

        public virtual Boolean Correct { get; set; }

        public virtual int Order { get; set; }

        public virtual string Justificative { get; set; }

        public virtual string Numeration { get; set; }

        public virtual decimal? TCTBiserialCoefficient { get; set; }

        public virtual decimal? TCTDificulty { get; set; }

        public virtual decimal? TCTDiscrimination { get; set; }

        [NotMapped]
        public int ItemOrder { get; set; }

        [NotMapped]
        public int Item_Id { get; set; }

        [NotMapped]
        public bool Selected { get; set; }

        [NotMapped]
        public string NumerationSem { get; set; }

        [NotMapped]
        public bool Changed { get; set; }
    }
}
