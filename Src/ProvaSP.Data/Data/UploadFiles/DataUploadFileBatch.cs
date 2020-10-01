using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades.UploadFiles;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ProvaSP.Data.Data.UploadFiles
{
    public class DataUploadFileBatch : IDataUploadFileBatch
    {
        public async Task<long?> AddAsync(UploadFileBatch entity)
        {
            if (entity is null || !entity.Valid) return null;

            var command = @"INSERT INTO UploadFileBatch
                            (CreatedDate, Edicao, AreaDeConhecimento, CicloDeAprendizagem, Situation)
                            VALUES
                            (@createdDate, @edicao, @areaDeConhecimento, @cicloDeAprendizagem, @situation);

                            SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]; ";

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return await conn.QuerySingleAsync<long>(command, new
                {
                    createdDate = entity.CreatedDate,
                    edicao = entity.Edicao,
                    areaDeConhecimento = (short)entity.AreaDeConhecimento,
                    cicloDeAprendizagem = (short)entity.CicloDeAprendizagem,
                    situation = entity.Situation
                });
            }
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

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                await conn.ExecuteAsync(command, new
                {
                    id = entity.Id,
                    updateDate = entity.UpdateDate,
                    situation = entity.Situation,
                });
            }
        }

        public Task<UploadFileBatch> GetAsync(long id)
        {
            var query = @"SELECT
                            Id,
                            CreatedDate,
                            BeginDate,
                            Edicao,
                            AreaDeConhecimento,
                            UploadFileBatchCicloDeAprendizagem,
                            Situacao,
                            UsuId
                        FROM
                            UploadFileBatch (NOLOCK)
                        WHERE
                            Id = @id;";

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.QuerySingleAsync<UploadFileBatch>(query, new
                {
                    id
                });
            }
        }

        public Task<IEnumerable<UploadFileBatch>> GetAsync(Guid usuId)
        {
            var query = @"SELECT
                            Id,
                            CreatedDate,
                            BeginDate,
                            Edicao,
                            AreaDeConhecimento,
                            UploadFileBatchCicloDeAprendizagem,
                            Situacao,
                            UsuId
                        FROM
                            UploadFileBatch (NOLOCK)
                        WHERE
                            UsuId = @usuId;";

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.QueryAsync<UploadFileBatch>(query, new
                {
                    usuId
                });
            }
        }

        public async Task<bool> AnyBatchActiveAsync()
        {
            var query = @"SELECT TOP 1 1
                        FROM
                            UploadFileBatch (NOLOCK)
                        WHERE
                            Situation IN (@situationNotStarted, @situationInProgress);";

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                var result = await conn.ExecuteScalarAsync<short?>(query, new
                {
                    situationNotStarted = UploadFileBatchSituation.NotStarted,
                    situationInProgress = UploadFileBatchSituation.InProgress
                });

                return result is null ? false : true;
            }
        }
    }
}