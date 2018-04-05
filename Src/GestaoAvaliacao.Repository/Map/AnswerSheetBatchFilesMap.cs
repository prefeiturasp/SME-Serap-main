using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class AnswerSheetBatchFilesMap : EntityBaseMap<AnswerSheetBatchFiles>
    {
        public AnswerSheetBatchFilesMap()
        {
            ToTable("AnswerSheetBatchFiles");

            HasRequired(p => p.File)
                .WithMany()
                .HasForeignKey(p => p.File_Id);

            HasOptional(p => p.AnswerSheetBatch)
                .WithMany(p => p.AnswerSheetBatchFiles)
                .HasForeignKey(p => p.AnswerSheetBatch_Id);

            HasOptional(p => p.AnswerSheetBatchQueue)
                .WithMany(p => p.AnswerSheetBatchFiles)
                .HasForeignKey(p => p.AnswerSheetBatchQueue_Id);
        }
    }
}
