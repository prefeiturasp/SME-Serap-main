using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    class TestTypeItemLevelMap : EntityBaseMap<TestTypeItemLevel>
    {
        public TestTypeItemLevelMap()
       {
           ToTable("TestTypeItemLevel");

           HasRequired(p => p.ItemLevel);
           HasRequired(p => p.TestType);
       }
    }
}
