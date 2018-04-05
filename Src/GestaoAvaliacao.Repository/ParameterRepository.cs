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
    public class ParameterRepository : ConnectionReadOnly, IParameterRepository
    {
        #region CRUD

        public void Update(List<Parameter> parameters)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                foreach (Parameter entity in parameters)
                {
                    Parameter parameter = gestaoAvaliacaoContext.Parameter.FirstOrDefault(p => p.Id == entity.Id);

                    parameter.Value = string.IsNullOrEmpty(entity.Value) ? " " : entity.Value;

                    parameter.Obligatory = entity.Obligatory;

                    parameter.Versioning = entity.Versioning;

                    parameter.UpdateDate = DateTime.Now;

                    gestaoAvaliacaoContext.Entry(parameter).State = System.Data.Entity.EntityState.Modified;

                    gestaoAvaliacaoContext.SaveChanges();
                }
            }
        }

        #endregion

        #region Read

        public IEnumerable<Parameter> GetParamsByPage(long PageId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.Parameter.AsNoTracking().Include("ParameterPage").Include("ParameterCategory").Include("ParameterType").
                        Where(p => p.ParameterPage.Id == PageId && p.State != (byte)EnumState.excluido).ToList();
                }
            }
        }

        public Parameter GetByKey(string key, Guid EntityId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                    return GestaoAvaliacaoContext.Parameter.AsNoTracking()
                        .FirstOrDefault(i => i.Key == key && i.State == (Byte)EnumState.ativo && i.EntityId == EntityId);
            }
        }

        public Parameter GetByKey(string key)
        {
            StringBuilder sql = new StringBuilder("SELECT [Id], [Key], [Value], [Description],[StartDate],[EndDate],[CreateDate],[UpdateDate],[State],[EntityId]");
            sql.Append(",[Obligatory],[Versioning],[ParameterCategory_Id],[ParameterPage_Id],[ParameterType_Id] ");
            sql.Append("FROM [dbo].[Parameter] ");
            sql.Append("WHERE [Key] = @key");
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<Parameter>(sql.ToString(), new { key = key }).FirstOrDefault();
            }
        }

        public IEnumerable<Parameter> GetParametersImage(Guid EntityId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                    return
                        GestaoAvaliacaoContext.Parameter.AsNoTracking().Where(
                            i => (i.Key == "JPEG"
                            || i.Key == "GIF"
                            || i.Key == "PNG"
                            || i.Key == "BMP"
                            || i.Key == "IMAGE_GIF_COMPRESSION"
                            || i.Key == "IMAGE_MAX_SIZE_FILE"
                            || i.Key == "IMAGE_QUALITY"
                            || i.Key == "IMAGE_MAX_RESOLUTION_HEIGHT"
                            || i.Key == "IMAGE_MAX_RESOLUTION_WIDTH")
                            && i.State == (Byte)EnumState.ativo
                            && i.EntityId == EntityId).ToList();
            }
        }

        public IEnumerable<Parameter> GetAll()
        {
            StringBuilder sql = new StringBuilder("SELECT [Id], [Key], [Value], [Description],[StartDate],[EndDate],[CreateDate],[UpdateDate],[State],[EntityId]");
            sql.Append(",[Obligatory],[Versioning],[ParameterCategory_Id],[ParameterPage_Id],[ParameterType_Id] ");
            sql.Append("FROM [dbo].[Parameter]");
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var parameters = cn.Query<Parameter>(sql.ToString());
                return parameters;
            }

        }

        public Parameter GetParamByKey(string key, Guid EntityId)
        {
            #region Consulta

            var sql = new StringBuilder(@"SELECT [Id]
                                                  ,[Key]
                                                  ,[Value]
                                                  ,[Description]
                                                  ,[StartDate]
                                                  ,[EndDate]
                                                  ,[CreateDate]
                                                  ,[UpdateDate]
                                                  ,[State]
                                                  ,[EntityId]
                                                  ,[Obligatory]
                                                  ,[Versioning]
                                                  ,[ParameterCategory_Id]
                                                  ,[ParameterPage_Id]
                                                  ,[ParameterType_Id]");
            sql.AppendLine("FROM [Parameter] WITH (NOLOCK)");
            sql.AppendLine("WHERE [Key] = @key AND [State] <> @state AND [EntityId] = @entityId");

            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<Parameter>(sql.ToString(),
                    new
                    {
                        key = key,
                        state = (byte)EnumState.excluido,
                        entityId = EntityId
                    }).FirstOrDefault();
            }
        }

        #endregion

    }
}
