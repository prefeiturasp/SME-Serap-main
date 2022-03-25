using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class NumberItemsAplicationTaiMap : EntityBaseMap<NumberItemsAplicationTai>
    {
        public NumberItemsAplicationTaiMap()
        {
            ToTable("NumberItemsAplicationTai");

            Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("varchar");

            Property(p => p.Value)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("varchar");

            Property(p => p.Id)
               .IsRequired();
        }
    }
}
