using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    class TestPermissionMap : EntityBaseMap<TestPermission>
    {
        public TestPermissionMap()
        {
            ToTable("TestPermission");

            Property(p => p.gru_id)
                .IsRequired();
            
            Property(p => p.AllowAnswer)
                .IsRequired();

            Property(p => p.ShowResult)
                .IsRequired();

            Property(p => p.TestHide)
                .IsRequired();

            HasRequired(p => p.Test)
                .WithMany()
                .HasForeignKey(p => p.Test_Id);
        }
    }
}
