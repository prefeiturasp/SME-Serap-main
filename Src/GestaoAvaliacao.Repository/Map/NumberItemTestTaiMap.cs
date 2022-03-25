using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class NumberItemTestTaiMap : EntityBaseMap<NumberItemTestTai>
    {
        public NumberItemTestTaiMap()
        {
            ToTable("NumberItemTestTai");

            Property(p => p.TestId)
               .IsRequired()
               .HasColumnType("bigint");

            Property(p => p.ItemAplicationTaiId)
                .IsRequired()
                .HasColumnType("bigint");

            Property(p => p.Id)
               .IsRequired();
        }
    }
}
