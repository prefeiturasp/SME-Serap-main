using GestaoAvaliacao.Entities.Base;
using GestaoAvaliacao.Worker.Database.Contexts.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.Repository.Base
{
    public abstract class BaseWorkerRepository<TEntity>
        where TEntity : EntityBase
    {
        protected readonly IGestaoAvaliacaoWorkerContext _gestaoAvaliacaoWorkerContext;

        public BaseWorkerRepository(IGestaoAvaliacaoWorkerContext gestaoAvaliacaoWorkerContext)
        {
            _gestaoAvaliacaoWorkerContext = gestaoAvaliacaoWorkerContext;
        }

        protected abstract DbSet<TEntity> DbSet { get; }

        protected Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken) 
            => DbSet.FirstOrDefaultAsync(predicate, cancellationToken);

        protected Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
            => DbSet.Where(predicate).ToListAsync(cancellationToken);

        public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            if (entity is null || !entity.Validate.IsValid) return;
            entity.UpdateDate = DateTime.Now;
            _gestaoAvaliacaoWorkerContext.Entry(entity).State = EntityState.Modified;
            await _gestaoAvaliacaoWorkerContext.SaveChangesAsync(cancellationToken);
        }
    }
}