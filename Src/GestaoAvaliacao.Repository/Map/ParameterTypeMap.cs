using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class ParameterTypeMap : EntityBaseMap<ParameterType>
    {
        public ParameterTypeMap()
        {
            ToTable("ParameterType");

            Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("varchar");
        }
    }
}
