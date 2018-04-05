using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class EvaluationMatrixMap : EntityBaseMap<EvaluationMatrix>
    {
        public EvaluationMatrixMap()
        {
            ToTable("EvaluationMatrix");

            Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnType("varchar");           
            
        }
    }
}
