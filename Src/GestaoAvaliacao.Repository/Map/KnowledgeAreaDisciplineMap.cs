using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Repository.Map.Base;

namespace GestaoAvaliacao.Repository.Map
{
    class KnowledgeAreaDisciplineMap : EntityBaseMap<KnowledgeAreaDiscipline>
    {
        public KnowledgeAreaDisciplineMap()
        {
            ToTable("KnowledgeAreaDiscipline");
        }
    }
}
