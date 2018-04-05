using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class ParameterCategoryMap : EntityBaseMap<ParameterCategory>
    {
        public ParameterCategoryMap()
        {
            ToTable("ParameterCategory");

            Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("varchar");
        }
    }
}
