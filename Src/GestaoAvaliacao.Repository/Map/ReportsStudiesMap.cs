using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository.Map
{
    public class ReportsStudiesMap : EntityBaseMap<ReportStudies>
    {
        public ReportsStudiesMap()
        {
            ToTable("ReportsStudies");

            Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnType("varchar");

            Property(p => p.TypeGroup)
                .IsRequired();

            Property(p => p.Addressee)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnType("varchar");

            Property(p => p.Link)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnType("varchar");

        }
    }
}
