using Dapper;
using ProvaSP.Data.Data.Abstractions;
using ProvaSP.Model.Entidades.UploadFiles;
using ProvaSP.Model.Entidades.UploadFiles.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProvaSP.Data.Data.UploadFiles
{
    public class DataUploadFileBatch : BaseData, IDataUploadFileBatch
    {
        private const string BaseColumnsSelect = @"Id,
                            CreatedDate,
                            BeginDate,
                            UploadFileBatchType,
                            Edicao,
                            AreaDeConhecimento,
                            CicloDeAprendizagem,
                            Situation,
                            FileCount,
                            FileErrorCount,
                            UsuId,
                            UsuName";

        private const string BaseConditionsSelect = @"WHERE
                    UploadFileBatchType = @uploadFileBatchType
                    AND Edicao = @edicao
                    AND (AreaDeConhecimento = @areaDeConhecimento OR @areaDeConhecimento IS NULL)
                    AND (CicloDeAprendizagem = @cicloDeAprendizagem OR @cicloDeAprendizagem IS NULL) ";

        public async Task<long?> AddAsync(UploadFileBatch entity)
        {
            if (entity is null || !entity.Valid) return null;

            var command = @"INSERT INTO UploadFileBatch
                            (CreatedDate, UploadFileBatchType, Edicao, AreaDeConhecimento, CicloDeAprendizagem, Situation, FileCount, UsuId, UsuName)
                            VALUES
                            (@createdDate, @uploadFileBatchType, @edicao, @areaDeConhecimento, @cicloDeAprendizagem, @situation, @fileCount, @usuId, @usuName);

                            SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]; ";

            return await GetSqlConnection().QuerySingleAsync<long>(command, new
            {
                createdDate = entity.CreatedDate,
                uploadFileBatchType = (short)entity.UploadFileBatchType,
                edicao = entity.Edicao,
                areaDeConhecimento = (short)entity.AreaDeConhecimento,
                cicloDeAprendizagem = (short)entity.CicloDeAprendizagem,
                situation = entity.Situation,
                fileCount = entity.FileCount,
                usuId = entity.UsuId,
                usuName = entity.UsuName
            });
        }

        public async Task UpdateAsync(UploadFileBatch entity)
        {
            if (entity is null || !entity.Valid) return;

            var command = @"UPDATE UploadFileBatch
                            SET
                                BeginDate = @beginDate,
                                UpdateDate = @updateDate,
                                FileCount = @fileCount,
                                FileErrorCount = @fileErrorCount,
                                Situation = @situation
                            WHERE
                                Id = @id;";

            await GetSqlConnection().ExecuteAsync(command, new
            {
                id = entity.Id,
                beginDate = entity.BeginDate,
                updateDate = entity.UpdateDate,
                fileCount = entity.FileCount,
                fileErrorCount = entity.FileErrorCount,
                situation = entity.Situation,
            });
        }

        public async Task UpdateAsync(IEnumerable<UploadFileBatch> entities)
        {
            if (!entities?.Any() ?? true) return;

            await Task.WhenAll(entities
                .Select(entity => UpdateAsync(entity)));
        }

        public Task<UploadFileBatch> GetAsync(long id)
        {
            var query = $@"SELECT
                        {BaseColumnsSelect}
                        FROM
                            UploadFileBatch (NOLOCK)
                        WHERE
                            Id = @id;";

            return GetSqlConnection().QuerySingleAsync<UploadFileBatch>(query, new
            {
                id
            });
        }

        public Task<IEnumerable<UploadFileBatch>> GetActiveBatchesAsync(Guid usuId, UploadFileBatchType uploadFileBatchType)
        {
            var query = $@"SELECT
                        {BaseColumnsSelect}
                        FROM
                            UploadFileBatch (NOLOCK)
                        WHERE
                            UsuId = @usuId
                            AND UploadFileBatchType = @uploadFileBatchType
                            AND Situation IN (@situationNotStarted, @situationInProgress);";

            return GetSqlConnection().QueryAsync<UploadFileBatch>(query, new
            {
                usuId,
                uploadFileBatchType = (short)uploadFileBatchType,
                situationNotStarted = (short)UploadFileBatchSituation.NotStarted,
                situationInProgress = (short)UploadFileBatchSituation.InProgress
            });
        }

        public async Task<bool> AnyBatchActiveAsync(UploadFileBatchType uploadFileBatchType)
        {
            var query = @"SELECT TOP 1 1
                        FROM
                            UploadFileBatch (NOLOCK)
                        WHERE
                            UploadFileBatchType = @uploadFileBatchType
                            AND Situation IN (@situationNotStarted, @situationInProgress);";

            var result = await GetSqlConnection().ExecuteScalarAsync<short?>(query, new
            {
                uploadFileBatchType = (short)uploadFileBatchType,
                situationNotStarted = UploadFileBatchSituation.NotStarted,
                situationInProgress = UploadFileBatchSituation.InProgress
            });

            return result is null ? false : true;
        }

        public async Task<UploadFileBatchPaginated> GetAsync(UploadFileBatchFilter filter)
        {
            var query = $@"SELECT 
                        TOP (@rows) {BaseColumnsSelect}
                        FROM    
                        (SELECT
                            ROW_NUMBER() OVER (ORDER BY CreatedDate DESC) AS RowNum,
                            {BaseColumnsSelect}
                            FROM
                            UploadFileBatch (NOLOCK)
                            {BaseConditionsSelect}
                        ) AS RowConstrainedResult
                        WHERE RowNum > (@skip);";

            var entitiesTask = GetSqlConnection().QueryAsync<UploadFileBatch>(query, GetSearchParameters(filter));
            var maxPageTask = GetMaxPageAsync(filter);
            await Task.WhenAll(entitiesTask, maxPageTask);

            return new UploadFileBatchPaginated
            {
                Entities = entitiesTask.Result,
                MaxPage = maxPageTask.Result,
                Page = filter.Page
            };
        }

        private Task<int?> GetMaxPageAsync(UploadFileBatchFilter filter)
        {
            var query = $@"SELECT TOP 1 (COUNT(*) OVER() + @rows - 1)/@rows 
                        FROM UploadFileBatch (NOLOCK)
                        {BaseConditionsSelect}
                        ORDER BY CreatedDate DESC;";

            return GetSqlConnection().QuerySingleOrDefaultAsync<int?>(query, GetSearchParameters(filter));
        }

        private object GetSearchParameters(UploadFileBatchFilter filter)
            => new
            {
                uploadFileBatchType = filter.UploadFileBatchType,
                edicao = filter.Edicao,
                areaDeConhecimento = (short?)filter.AreaDeConhecimento,
                cicloDeAprendizagem = (short?)filter.CicloDeAprendizagem,
                skip = (filter.Page - 1) * UploadFileBatchFilter.RowsPerPage,
                rows = UploadFileBatchFilter.RowsPerPage
            };
    }
}