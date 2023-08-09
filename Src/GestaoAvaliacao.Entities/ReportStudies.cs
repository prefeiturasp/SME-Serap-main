using GestaoAvaliacao.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
