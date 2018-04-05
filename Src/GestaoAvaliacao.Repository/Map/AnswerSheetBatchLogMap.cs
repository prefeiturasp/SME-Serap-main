using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class AnswerSheetBatchLogMap : EntityBaseMap<AnswerSheetBatchLog>
    {
        public AnswerSheetBatchLogMap()
        {
            ToTable("AnswerSheetBatchLog");

            Property(p => p.Description)
                .IsOptional()
                .HasColumnType("varchar(MAX)");

            HasRequired(p => p.AnswerSheetBatchFile)
                .WithMany()
                .HasForeignKey(p => p.AnswerSheetBatchFile_Id);

            HasOptional(p => p.File)
                .WithMany()
                .HasForeignKey(p => p.File_Id);
        }
    }
}
