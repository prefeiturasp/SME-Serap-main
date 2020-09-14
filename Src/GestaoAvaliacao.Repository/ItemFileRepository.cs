using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GestaoAvaliacao.Repository
{
    public class ItemFileRepository : ConnectionReadOnly, IItemFileRepository
    {
        #region Read

        public IEnumerable<ItemFile> GetVideosByItemId(long itemId)
        {
            var sql = @"SELECT IFI.Id AS ItemFileId, IFI.Item_Id, F.Id AS FileId, F.[Path], F.Name, F.ContentType AS FileType, FT.Id AS ThumbnailId, 
                        FT.[Path] AS ThumbnailPath, FT.Name AS ThumbnailName, FC.Id AS ConvertedFileId, FC.Name AS ConvertedFileName, FC.[Path] AS ConvertedFilePath,
                        FC.ContentType AS ConvertedFileType
                        FROM ItemFile IFI WITH (NOLOCK)
                        INNER JOIN [File] F WITH (NOLOCK) ON F.Id = IFI.File_Id
                        INNER JOIN [File] FT WITH (NOLOCK) ON FT.Id = IFI.Thumbnail_Id
                        LEFT JOIN [File] FC (NOLOCK) ON IFI.ConvertedFile_Id = FC.Id
                        WHERE IFI.Item_Id = @itemId AND IFI.State = @state";

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var retorno = cn.Query<ItemFile>(sql.ToString(), new
                {
                    itemId = itemId,
                    state = (byte)EnumState.ativo
                });

                return retorno;
            }
        }

        public IEnumerable<ItemFile> GetVideosByLstItemId(List<long> itemId)
        {
            StringBuilder sql = new StringBuilder(@"SELECT IFI.Id AS ItemFileId, IFI.Item_Id, F.Id AS FileId, F.[Path], F.Name, F.ContentType AS FileType, FT.Id AS ThumbnailId, 
                        FT.[Path] AS ThumbnailPath, FT.Name AS ThumbnailName, FC.Id AS ConvertedFileId, FC.Name AS ConvertedFileName, FC.[Path] AS ConvertedFilePath,
                        FC.ContentType AS ConvertedFileType
                        FROM ItemFile IFI WITH (NOLOCK)
                        INNER JOIN [File] F WITH (NOLOCK) ON F.Id = IFI.File_Id
                        INNER JOIN [File] FT WITH (NOLOCK) ON FT.Id = IFI.Thumbnail_Id
                        LEFT JOIN [File] FC (NOLOCK) ON IFI.ConvertedFile_Id = FC.Id
                        WHERE IFI.State = @state ");

            sql.AppendFormat("AND IFI.Item_Id IN ({0})", string.Join(",", itemId));

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var retorno = cn.Query<ItemFile>(sql.ToString(), new
                {
                    state = (byte)EnumState.ativo
                });

                return retorno;
            }
        }

        #endregion Read
    }
}