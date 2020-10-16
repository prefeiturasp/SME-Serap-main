using Dapper;
using ProvaSP.Data.Data.Abstractions;
using ProvaSP.Model.Entidades.UploadFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProvaSP.Data.Data.UploadFiles
{
    public class DataUploadFileBatch : BaseData, IDataUploadFileBatch
    {
        private const string BaseSelect = @"SELECT
                            Id,
                            CreatedDate,
                            BeginDate,
                            UploadFileBatchType,
                            Edicao,
                            AreaDeConhecimento,
                            CicloDeAprendizagem,
                            Situation,
                            FileCount,
                            UsuId
                        FROM
                            UploadFileBatch (NOLOCK)";

        public async Task<long?> AddAsync(UploadFileBatch entity)
        {
            if (entity is null || !entity.Valid) return null;

            var command = @"INSERT INTO UploadFileBatch
                            (CreatedDate, UploadFileBatchType, Edicao, AreaDeConhecimento, CicloDeAprendizagem, Situation, FileCount, UsuId)
                            VALUES
                            (@createdDate, @uploadFileBatchType, @edicao, @areaDeConhecimento, @cicloDeAprendizagem, @situation, @fileCount, @usuId);

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
                usuId = entity.UsuId
            });
        }

        public async Task UpdateAsync(UploadFileBatch entity)
        {
            if (entity is null || !entity.Valid) return;

            var command = @"UPDATE UploadFileBatch
                            SET
                                UpdateDate = @updateDate,
                                Situation = @situation
                            WHERE
                                Id = @id;";

            await GetSqlConnection().ExecuteAsync(command, new
            {
                id = entity.Id,
                updateDate = entity.UpdateDate,
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
            var query = $@"{BaseSelect}
                        WHERE
                            Id = @id;";

            return GetSqlConnection().QuerySingleAsync<UploadFileBatch>(query, new
            {
                id
            });
        }

        public Task<IEnumerable<UploadFileBatch>> GetActiveBatchesAsync(Guid usuId, UploadFileBatchType uploadFileBatchType)
        {
            var query = $@"{BaseSelect}
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
    }
}