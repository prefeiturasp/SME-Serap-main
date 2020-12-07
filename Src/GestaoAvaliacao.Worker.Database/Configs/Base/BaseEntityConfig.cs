using GestaoAvaliacao.Worker.Domain.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoAvaliacao.Worker.Database.Configs.Base
{
    public abstract class BaseEntityConfig<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : EntityWorkerBase
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.ToTable(GetTableName());
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreateDate).IsRequired();
            builder.Property(x => x.UpdateDate).IsRequired();
            builder.Property(x => x.State).IsRequired();
            builder.Ignore(x => x.Errors);
            builder.Ignore(x => x.IsValid);
            OnConfiguring(builder);
        }

        protected abstract void OnConfiguring(EntityTypeBuilder<TEntity> builder);

        protected abstract string GetTableName();
    }
}