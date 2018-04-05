using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class AnswerSheetLotMap : EntityBaseMap<AnswerSheetLot>
	{
		public AnswerSheetLotMap()
		{
			ToTable("AnswerSheetLot");

            Property(p => p.ExecutionOwner)
                .IsOptional()
                .HasMaxLength(1000)
                .HasColumnType("varchar");

            HasOptional(p => p.Test)
				.WithMany()
				.HasForeignKey(p => p.Test_Id);

            HasOptional(p => p.Parent)
                .WithMany()
                .HasForeignKey(p => p.Parent_Id);

            Ignore(p => p.TestId);
            Ignore(p => p.ParentId);
        } 
	}
}
