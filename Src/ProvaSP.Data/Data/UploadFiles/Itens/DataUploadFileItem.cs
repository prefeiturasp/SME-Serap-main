using Dapper;
using ProvaSP.Data.Funcionalidades;
using ProvaSP.Model.Entidades.UploadFiles.Itens;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ProvaSP.Data.Data.UploadFiles.Itens
{
    public class DataUploadFileItem : IDataUploadFileItem
    {
        public Task<long> GetNextIdAsync()
        {
            var query = @"SELECT
                            ISNULL(MAX(Id), 0) + 1
                        FROM
                            UploadFileItem (NOLOCK)";

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.QuerySingleAsync<long>(query);
            }
        }

        public async Task<bool> AnyAsync(long uploadFileBatchId)
        {
            var query = @"SELECT TOP 1 1 FROM UploadFileItem (NOLOCK) WHERE UploadFileBatchId = @uploadFileBatchId;";

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                var result = await conn.QuerySingleAsync<short?>(query);
                return result is null ? false : true;
            }
        }

        public Task AddAsync(IEnumerable<UploadFileItem> entities)
        {
            if (!entities?.Any() ?? true) return Task.FromResult(false);
            var itemCommands = entities
                .Select(x => GetCommandAddItem(x))
                .ToList();

            var command = $@"INSERT INTO UploadFileItem (Id, CreatedDate, FileName, OriginPath, Situation,UploadFileBatchId)
                             VALUES {string.Join(",", itemCommands)}";

            using (var conn = new SqlConnection(StringsConexao.ProvaSP))
            {
                return conn.ExecuteAsync(command);
            }
        }

        private static string GetCommandAddItem(UploadFileItem entity)
            => $@"({entity.Id}, {entity.CreatedDate}, {entity.FileName}, {entity.OriginPath}, {(short)entity.Situation}, {entity.UploadFileBatchId})";
    }
}