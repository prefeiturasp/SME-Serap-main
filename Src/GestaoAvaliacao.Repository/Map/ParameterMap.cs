using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class ParameterMap : EntityBaseMap<Parameter>
	{
		public ParameterMap()
		{
			ToTable("Parameter");

			HasRequired(p => p.ParameterPage);
			HasRequired(p => p.ParameterCategory);
			HasRequired(p => p.ParameterType);

			Property(p => p.Key)
				.IsRequired()
				.HasMaxLength(100)
				.HasColumnType("varchar");

		  Property(p => p.Value)
				.IsRequired()
				.HasMaxLength(8000)
				.HasColumnType("varchar");

		  Property(p => p.Description)
				.IsOptional()
				.HasMaxLength(200)
				.HasColumnType("varchar");

			Property(p => p.StartDate)
			  .IsRequired();

			Property(p => p.EndDate)
			  .IsOptional();

			HasRequired(p => p.ParameterPage)
				.WithMany()
				.HasForeignKey(p => p.ParameterPage_Id);
		}

	}
}
