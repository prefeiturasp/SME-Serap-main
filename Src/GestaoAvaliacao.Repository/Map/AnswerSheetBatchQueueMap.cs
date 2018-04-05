using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class AnswerSheetBatchQueueMap : EntityBaseMap<AnswerSheetBatchQueue>
    {
        public AnswerSheetBatchQueueMap()
        {
            ToTable("AnswerSheetBatchQueue");

            HasRequired(p => p.File)
                .WithMany()
                .HasForeignKey(p => p.File_Id);

            Property(p => p.Description)
                .IsOptional()
                .HasColumnType("varchar(MAX)");

            HasOptional(p => p.AnswerSheetBatch)
                .WithMany()
                .HasForeignKey(p => p.AnswerSheetBatch_Id);
        }
    }
}
