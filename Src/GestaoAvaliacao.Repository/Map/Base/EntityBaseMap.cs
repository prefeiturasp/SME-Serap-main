using GestaoAvaliacao.Entities.Base;
using System.Data.Entity.ModelConfiguration;

namespace GestaoAvaliacao.Repository.Map.Base
{
    public class EntityBaseMap<T> : EntityTypeConfiguration<T> where T: EntityBase
    {
        public EntityBaseMap()
        {
            HasKey(x => new { Id = x.Id });
            Property(x => x.CreateDate).IsRequired();
            Property(x => x.UpdateDate).IsRequired();
            Property(x => x.State).IsRequired();
            Ignore(x => x.Validate);
        }
    }
}
