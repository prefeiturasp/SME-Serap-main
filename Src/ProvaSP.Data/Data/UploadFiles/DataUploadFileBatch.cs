using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades.UploadFiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ProvaSP.Data.Data.UploadFiles
{
    public class DataUploadFileBatch : IDataUploadFileBatch
    {
        private IDbConnection _dbConnection;

        private const string BaseSelect = @"SELECT
                            Id,
                            CreatedDate,
                            BeginDate,
                            UploadFileBatchType,
                            Edicao,
                            AreaDeConhecimento,
                            UploadFileBatchCicloDeAprendizagem,
                            Situacao,
                            UsuId
                        FROM
                            UploadFileBatch (NOLOCK)";

        public async Task<long?> AddAsync(UploadFileBatch entity)
        {
            if (entity is null || !entity.Valid) return null;

            var command = @"INSERT INTO UploadFileBatch
                            (CreatedDate, Edicao, AreaDeConhecimento, CicloDeAprendizagem, Situation)
                            VALUES
                            (@createdDate, @edicao, @areaDeConhecimento, @cicloDeAprendizagem, @situation);

                            SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]; ";

            return await GetSqlConnection().QuerySingleAsync<long>(command, new
            {
                createdDate = entity.CreatedDate,
                edicao = entity.Edicao,
                areaDeConhecimento = (short)entity.AreaDeConhecimento,
                cicloDeAprendizagem = (short)entity.CicloDeAprendizagem,
                situation = entity.Situation
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

        public Task<UploadFileBatch> GetActiveBatchAsync(UploadFileBatchType uploadFileBatchType)
        {
            var query = $@"{BaseSelect}
                        WHERE
                            UploadFileBatchType = @uploadFileBatchType
                            AND Situation IN (@situationNotStarted, @situationInProgress);";

            return GetSqlConnection().QueryFirstAsync<UploadFileBatch>(query, new
            {
                uploadFileBatchType = (short)uploadFileBatchType,
                situationNotStarted = UploadFileBatchSituation.NotStarted,
                situationInProgress = UploadFileBatchSituation.InProgress
            });
        }

        public async Task<bool> AnyBatchActiveAsync(UploadFileBatchType uploadFileBatchType)
        {
            var query = @"SELECT TOP 1 1
                        FROM
                            UploadFileBatch (NOLOCK)
                        WHERE
                            UploadFileBatchType = @uploadFileBatchType
                            Situation IN (@situationNotStarted, @situationInProgress);";

            var result = await GetSqlConnection().ExecuteScalarAsync<short?>(query, new
            {
                uploadFileBatchType = (short)uploadFileBatchType,
                situationNotStarted = UploadFileBatchSituation.NotStarted,
                situationInProgress = UploadFileBatchSituation.InProgress
            });

            return result is null ? false : true;
        }

        private IDbConnection GetSqlConnection()
        {
            _dbConnection = _dbConnection ?? new SqlConnection(StringsConexao.ProvaSP);
            if (_dbConnection.State != ConnectionState.Open) _dbConnection.Open();

            return _dbConnection;
        }
    }
}