using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class NumberItemTestTaiMap : EntityBaseMap<NumberItemTestTai>
    {
        public NumberItemTestTaiMap()
        {
            ToTable("NumberItemTestTai");

            HasRequired(t => t.Test)
                .WithMany()
                .HasForeignKey(t => t.TestId);

            HasRequired(t => t.ItemAplicationTai)
                .WithMany()
                .HasForeignKey(t => t.ItemAplicationTaiId);

        }
    }
}
