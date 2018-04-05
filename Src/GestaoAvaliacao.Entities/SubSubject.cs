using GestaoAvaliacao.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Entities
{
    public class SubSubject : EntityBase
    {
        public virtual string Description { get; set; }

        public virtual Subject Subject { get; set; }

        [NotMapped]
        public long Subject_Id { get; set; }
    }
}
