using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class AnswerSheetBatchMap : EntityBaseMap<AnswerSheetBatch>
    {
        public AnswerSheetBatchMap()
        {
            ToTable("AnswerSheetBatch");

            Property(p => p.Description)
                .IsRequired()
                .HasColumnType("varchar(MAX)");

            HasRequired(p => p.Test)
                .WithMany()
                .HasForeignKey(p => p.Test_Id);
        }
    }
}
