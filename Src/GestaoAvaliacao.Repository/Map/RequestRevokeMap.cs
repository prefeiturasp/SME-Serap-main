using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class RequestRevokeMap : EntityBaseMap<RequestRevoke>
    {
        public RequestRevokeMap()
        {
            ToTable("RequestRevoke");

            Property(p => p.Justification)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnType("varchar");

            HasRequired(p => p.Test)
                .WithMany()
                .HasForeignKey(p => p.Test_Id);

            HasRequired(p => p.BlockItem)
                .WithMany(p => p.RequestRevokes)
                .HasForeignKey(p => p.BlockItem_Id);
        }
    }
}
