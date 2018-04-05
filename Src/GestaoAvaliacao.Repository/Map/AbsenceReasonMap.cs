using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class AbsenceReasonMap : EntityBaseMap<AbsenceReason>
    {

       public AbsenceReasonMap()
       {
           ToTable("AbsenceReason");

           Property(p => p.Description)
               .IsRequired()
               .HasMaxLength(500)
               .HasColumnType("varchar");

           Property(p => p.AllowRetry)
             .IsRequired();            
           
       }
    }
}
