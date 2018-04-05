using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class BookletMap : EntityBaseMap<Booklet>
    {
        public BookletMap()
        {
            ToTable("Booklet");

            HasOptional(p => p.Test)
                .WithMany()
                .HasForeignKey(p => p.Test_Id);
        }
    }
}
