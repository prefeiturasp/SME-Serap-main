using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository.Map
{
    class SubSubjectMap : EntityBaseMap<SubSubject>
    {
        public SubSubjectMap()
        {
            ToTable("SubSubject");

            Property(p => p.Description)
               .IsRequired()
               .HasMaxLength(200)
               .HasColumnType("varchar");
        }
    }
}
