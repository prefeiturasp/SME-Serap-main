using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository
{
    public class ReportStudiesRepository : ConnectionReadOnly, IReportStudiesRepository
    {
        public bool Save(ReportStudies entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;
                entity.CreateDate = dateNow;
                entity.UpdateDate = dateNow;
                entity.State = Convert.ToByte(Entities.Enumerator.EnumState.ativo);

                gestaoAvaliacaoContext.ReportStudies.Add(entity);
                gestaoAvaliacaoContext.SaveChanges();
            }
            return true;
            
        }

        public IEnumerable<ReportStudies> ListAll()
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                return gestaoAvaliacaoContext.ReportStudies
                    .Where(r => r.State == Convert.ToByte(Entities.Enumerator.EnumState.ativo))
                    .ToList();
            }
        }

        public IEnumerable<ReportStudies> ListPaginated(ref Pager pager)
        {
            using (IDbConnection cn = Connection)
            {
                var sql = new StringBuilder(@"
            WITH PaginatedReports AS (
                SELECT
                    rs.Id,
                    rs.Name,
                    rs.TypeGroup,
                    rs.Addressee,
                    rs.CreateDate,
                    rs.UpdateDate,
                    rs.[State],
                    rs.[Link],
                    ROW_NUMBER() OVER (ORDER BY rs.CreateDate DESC) AS RowNumber
                FROM ReportsStudies rs
                where rs.[State] = @state
            )
            SELECT
                Id,
                Name,
                TypeGroup,
                Addressee,
                CreateDate,
                UpdateDate,
                Link,
                [State]
            FROM PaginatedReports
            WHERE RowNumber > (@pageSize * @page)
            AND RowNumber <= ((@page + 1) * @pageSize)
            SELECT COUNT(rs.Id) AS TotalItems
            FROM ReportsStudies rs
            where rs.[State] = @state
        ");

                cn.Open();

                var query = cn.QueryMultiple(sql.ToString(),
                    new { pageSize = pager.PageSize, page = pager.CurrentPage, state = (int)EnumState.ativo });

                var result = query.Read<ReportStudies>();
                var totalItems = query.Read<int>().FirstOrDefault();

                pager.SetTotalItens(totalItems);
                pager.SetTotalPages((int)Math.Ceiling(totalItems / (double)pager.PageSize));

                return result;
            }
        }

        public IEnumerable<ReportStudies> ListWithFilter(string searchFilter)
        {
            using (IDbConnection cn = Connection)
            {
                string whereClause = "";

                if (!string.IsNullOrEmpty(searchFilter))
                {
                    int id = 0;
                    if (int.TryParse(searchFilter, out id))
                    {
                        whereClause = $" AND rs.Id = {id} and rs.[State] = 1 ";
                    }
                    else
                    {
                        whereClause = $" AND (rs.Name LIKE '%{searchFilter}%' OR rs.Addressee LIKE '%{searchFilter}%') and rs.[State] = 1 ";
                    }
                }

                var sql = new StringBuilder($@"
            WITH FilteredReports AS (
                SELECT
                    rs.Id,
                    rs.Name,
                    rs.TypeGroup,
                    rs.Addressee,
                    rs.CreateDate,
                    rs.UpdateDate,
                    rs.[State],
                    rs.[Link],
                    ROW_NUMBER() OVER (ORDER BY rs.CreateDate DESC) AS RowNumber
                FROM ReportsStudies rs
                WHERE 1=1
                {whereClause}
            )
            SELECT
                Id,
                Name,
                TypeGroup,
                Addressee,
                CreateDate,
                UpdateDate,
                Link,
                [State]
            FROM FilteredReports
        ");

                cn.Open();

                var result = cn.Query<ReportStudies>(sql.ToString());
                return result;
            }
        }

        public bool DeleteById(long id)
        {
            var sql = new StringBuilder("UPDATE ReportsStudies SET [State] = 3 WHERE Id = @id");
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                cn.Execute(sql.ToString(),
                    new
                    {
                        id = id,
                    });
            }
            return true;
        }

        public void Delete(long id)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                var entity = gestaoAvaliacaoContext.ReportStudies.Find(id);
                if (entity != null)
                {
                    entity.State = 3;
                    entity.UpdateDate = DateTime.Now;

                    gestaoAvaliacaoContext.SaveChanges();
                }
            }
        }
    }
}
