using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class ParameterPageMap : EntityBaseMap<ParameterPage>
    {
        public ParameterPageMap()
        {
            ToTable("ParameterPage");

            Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("varchar");

            Property(p => p.pageVersioning)
                .IsRequired();

            Property(p => p.pageObligatory)
                .IsRequired();
        }
    }
}
