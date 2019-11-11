using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Repository
{
    public class ItemAudioRepository : ConnectionReadOnly, IItemAudioRepository
    {
        #region Read

        public IEnumerable<ItemAudio> GetAudiosByItemId(long itemId)
        {
            var sql = @"SELECT IAI.Id AS ItemFileId, IAI.Item_Id, F.Id AS FileId, F.[Path], F.Name
                        FROM ItemAudio IAI WITH (NOLOCK)
                        INNER JOIN [File] F WITH (NOLOCK) ON F.Id = IAI.File_Id
                        WHERE IAI.Item_Id = @itemId AND IAI.State = @state";

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var retorno = cn.Query<ItemAudio>(sql.ToString(), new
                {
                    itemId = itemId,
                    state = (byte)EnumState.ativo
                });

                return retorno;
            }
        }

        public IEnumerable<ItemAudio> GetAudiosByLstItemId(List<long> itemId)
        {
            StringBuilder sql = new StringBuilder(@"SELECT IAI.Id AS ItemFileId, IAI.Item_Id, F.Id AS FileId, F.[Path]
                        FROM ItemAudio IAI WITH (NOLOCK)
                        INNER JOIN [File] F WITH (NOLOCK) ON F.Id = IAI.File_Id
                        WHERE IAI.State = @state ");

            sql.AppendFormat("AND IAI.Item_Id IN ({0})", string.Join(",", itemId));

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var retorno = cn.Query<ItemAudio>(sql.ToString(), new
                {
                    state = (byte)EnumState.ativo
                });

                return retorno;
            }
        }

        #endregion
    }
}
