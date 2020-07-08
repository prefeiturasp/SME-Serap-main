using ImportacaoDeQuestionariosSME.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImportacaoDeQuestionariosSME.Data.Repositories.Abstractions
{
    public abstract class BaseInsertRepository<TEntity> : BaseRepository
        where TEntity : Entity
    {
        protected const int MaxInsertData = 1000;

        public BaseInsertRepository()
            : base()
        {
        }

        protected abstract string GetQueryInsert();

        protected abstract string GetValuesQueryForEntity(TEntity entity);

        protected IList<string> GetSqlsInPages(IEnumerable<TEntity> entities)
        {
            var query = GetQueryInsert();

            var sqlsToExecute = new List<string>();
            var numberOfPages = (int)Math.Ceiling((double)entities.Count() / MaxInsertData);

            for (int i = 0; i < numberOfPages; i++)
            {
                var entityToInsert = entities
                    .Skip(i * MaxInsertData)
                    .Take(MaxInsertData);

                var valuesToInsert = entityToInsert
                    .Select(e => GetValuesQueryForEntity(e));

                sqlsToExecute.Add(query + string.Join(",", valuesToInsert));
            }

            return sqlsToExecute;
        }
    }
}