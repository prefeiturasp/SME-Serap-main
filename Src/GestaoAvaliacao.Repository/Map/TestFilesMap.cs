using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    public class TestFilesMap : EntityBaseMap<TestFiles>
    {
        public TestFilesMap()
        {
            ToTable("TestFiles");

            Property(p => p.UserId).IsOptional();

            HasRequired(p => p.Test)
                .WithMany()
                .HasForeignKey(p => p.Test_Id);

            HasRequired(p => p.File)
                .WithMany(p => p.TestFiles)
                .HasForeignKey(p => p.File_Id);
        }
    }
}
