using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Repository
{
    public class AnswerSheetBatchQueueRepository : ConnectionReadOnly, IAnswerSheetBatchQueueRepository
    {
        #region Read
        public IEnumerable<AnswerSheetBatchQueueResult> Search(AnswerSheetBatchQueueFilter filter, ref Pager pager)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"WITH NumberedAnswerSheetBatchQueue AS " +
                            "( " +
                               "SELECT A.[Id],A.[File_Id],A.[AnswerSheetBatch_Id],A.[SupAdmUnit_Id],A.[School_Id],A.[CountFiles], " +
                               "A.[Situation],A.[Description],A.[CreatedBy_Id],A.[CreateDate],A.[UpdateDate],A.[State],A.[EntityId], " +
                               "U.[usu_login], " +
                               "ROW_NUMBER() OVER (ORDER BY A.CreateDate DESC) AS RowNumber " +
                               GetFieldsEscolaDRE() +
                               "FROM AnswerSheetBatchQueue A WITH (NOLOCK) " +
                                GetInnerPermissao(filter) +
                               "     LEFT JOIN Synonym_Core_SYS_Usuario U WITH (NOLOCK) ON A.[CreatedBy_Id] = U.[usu_id] " +
                                GetInnerEscolaDRE(filter, true) +
                               "WHERE State  = @state " +
                               GetAndVisaoIndividual(filter) +
                               GetAndFilters(filter) +
                               GetAndFiltersUadSchool(filter) +
                            ") " +
                            ", Quantidades AS " +
                            "( " +
                                "SELECT COUNT(DISTINCT ISNULL(BF.Student_Id, BF.Id)) as quantidade, BF.Situation, ASBQ.Id " +
                                    "FROM [AnswerSheetBatchFiles] BF WITH (NOLOCK) " +
                                    "LEFT JOIN [AnswerSheetBatch] B WITH (NOLOCK) ON B.[Id] = BF.[AnswerSheetBatch_Id] AND B.[State] = 1 " +
                                    "LEFT JOIN AnswerSheetBatchQueue ASBQ WITH (NOLOCK) ON ASBQ.Id = BF.AnswerSheetBatchQueue_Id " +
                                 "WHERE BF.[State] = @state " +
                                "GROUP BY BF.Situation, ASBQ.Id " +
                            ") " +

                      "SELECT " +
                          (filter.top.HasValue ? string.Format("TOP {0} ", filter.top) : "") +
                          "A.[Id],A.[File_Id],A.[AnswerSheetBatch_Id],A.[SupAdmUnit_Id],A.[School_Id],A.[CountFiles], " +
                          "A.[Situation],A.[Description],A.[CreatedBy_Id],A.[CreateDate],A.[UpdateDate],A.[State],A.[EntityId], " +
                          "F.[Id] AS File_Id, F.[OriginalName] AS FileName, F.[Path] AS FilePath, " +
                          "quantPending.quantidade AS Pending, " +
                          "quantSuccess.quantidade AS Success, " +
                          "quantError.quantidade AS Error, " +
                          "quantWarning.quantidade AS Warning, " +
                          "quantPendingIdentification.quantidade AS PendingIdentification, " +
                          "quantNotIdentified.quantidade AS NotIdentified, " +
                          "quantAbsent.quantidade AS [Absent], " +
                          "A.[usu_login] AS UserName, A.SchoolName, A.SupAdmUnitName " +
                      "FROM NumberedAnswerSheetBatchQueue A WITH (NOLOCK) " +
                      "INNER JOIN [File] F WITH (NOLOCK) ON A.File_Id = F.Id " +
                      "LEFT JOIN Quantidades as quantPending " +
                          "ON quantPending.Id = A.[Id] " +
                          "AND quantPending.Situation = @SituationPending " +
                      "LEFT JOIN Quantidades as quantSuccess " +
                          "ON quantSuccess.Id = A.[Id] " +
                          "AND quantSuccess.Situation = @SituationSuccess " +
                      "LEFT JOIN Quantidades as quantError " +
                          "ON quantError.Id = A.[Id] " +
                          "AND quantError.Situation = @SituationError " +
                      "LEFT JOIN Quantidades as quantWarning " +
                          "ON quantWarning.Id = A.[Id] " +
                          "AND quantWarning.Situation = @SituationWarning " +
                      "LEFT JOIN Quantidades as quantPendingIdentification " +
                          "ON quantPendingIdentification.Id = A.[Id] " +
                          "AND quantPendingIdentification.Situation = @SituationPendingIdentification " +
                      "LEFT JOIN Quantidades as quantNotIdentified " +
                          "ON quantNotIdentified.Id = A.[Id] " +
                          "AND quantNotIdentified.Situation = @SituationNotIdentified " +
                      "LEFT JOIN Quantidades as quantAbsent " +
                          "ON quantAbsent.Id = A.[Id] " +
                          "AND quantAbsent.Situation = @SituationAbsent " +
                      "WHERE A.RowNumber > ( @pageSize * @page ) " +
                      "AND A.RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
                      "ORDER BY A.RowNumber";

                var answerSheetBatchQueue = cn.Query<AnswerSheetBatchQueueResult>(sql,
                    new
                    {
                        state = (Byte)EnumState.ativo,
                        pageSize = pager.PageSize,
                        page = pager.CurrentPage,
                        Situation = filter.Situation,
                        UserId = filter.UserId,
                        SupAdmUnitId = filter.SupAdmUnitId,
                        SchoolId = filter.SchoolId,
                        SituationPending = (byte)EnumBatchSituation.Pending,
                        SituationSuccess = (byte)EnumBatchSituation.Success,
                        SituationError = (byte)EnumBatchSituation.Error,
                        SituationWarning = (byte)EnumBatchSituation.Warning,
                        SituationPendingIdentification = (byte)EnumBatchSituation.PendingIdentification,
                        SituationNotIdentified = (byte)EnumBatchSituation.NotIdentified,
                        SituationAbsent = (byte)EnumBatchSituation.Absent,
                        StartDate = filter.StartDate,
                        EndDate = filter.EndDate
                    });

                var count = 0;
                if (!filter.top.HasValue)
                {
                    var countSql = @"SELECT COUNT(A.id) " +
                                    "FROM AnswerSheetBatchQueue A WITH (NOLOCK)" +
                                    GetInnerPermissao(filter) +
                                    "WHERE State = @state " +
                                    GetAndVisaoIndividual(filter) +
                                    GetAndFilters(filter) +
                                    GetAndFiltersUadSchool(filter);

                    count = (int)cn.ExecuteScalar(countSql,
                        new
                        {
                            state = (Byte)EnumState.ativo,
                            Situation = filter.Situation,
                            UserId = filter.UserId,
                            SupAdmUnitId = filter.SupAdmUnitId,
                            SchoolId = filter.SchoolId,
                            StartDate = filter.StartDate,
                            EndDate = filter.EndDate
                        });
                }
                else
                {
                    count = (int)filter.top.Value;
                }

                pager.SetTotalPages((int)Math.Ceiling(answerSheetBatchQueue.Count() / (double)pager.PageSize));
                pager.SetTotalItens(count);

                return answerSheetBatchQueue;
            }
        }
        public IEnumerable<AnswerSheetBatchQueueResult> GetTop(AnswerSheetBatchQueueFilter filter)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"SELECT ";
                if (filter.top > 0)
                    sql += string.Format("TOP {0} ", filter.top);

                sql += "A.[Id],A.[File_Id],A.[AnswerSheetBatch_Id],A.[SupAdmUnit_Id],A.[School_Id],A.[CountFiles], " +
                       "A.[Situation],A.[Description],A.[CreatedBy_Id],A.[CreateDate],A.[UpdateDate],A.[State],A.[EntityId], " +
                       "F.[Id] AS File_Id, F.[OriginalName] AS FileName, F.[Path] AS FilePath" +
                       GetFieldsEscolaDRE() +
                       "FROM AnswerSheetBatchQueue A WITH (NOLOCK) " +
                       GetInnerPermissao(filter) +
                       "INNER JOIN [File] F WITH (NOLOCK) ON A.File_Id = F.Id " +
                       GetInnerEscolaDRE(filter, false) +
                       "WHERE A.State = @state " +
                       GetAndVisaoIndividual(filter) +
                       "AND A.Situation = 1 " +
                       "ORDER BY A.CreateDate DESC";

                var answerSheetBatchQueue = cn.Query<AnswerSheetBatchQueueResult>(sql,
                    new
                    {
                        state = (Byte)EnumState.ativo,
                        top = filter.top,
                        UserId = filter.UserId
                    });

                return answerSheetBatchQueue;
            }
        }

        public IEnumerable<AnswerSheetBatchQueue> GetAnswerSheetBatchBySituation(EnumBatchQueueSituation Situation, int rows)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"SELECT ";
                if (rows > 0)
                    sql += string.Format("TOP {0} ", rows);

                sql += "A.[Id],A.[File_Id],A.[AnswerSheetBatch_Id],A.[SupAdmUnit_Id],A.[School_Id],A.[CountFiles], " +
                       "A.[Situation],A.[Description],A.[CreatedBy_Id],A.[CreateDate],A.[UpdateDate],A.[State],A.[EntityId] " +
                       "FROM AnswerSheetBatchQueue A WITH (NOLOCK) " +
                       "WHERE A.State = @state " +
                       "AND A.Situation = @Situation " +
                       "ORDER BY A.CreateDate ASC";

                var answerSheetBatchQueue = cn.Query<AnswerSheetBatchQueue>(sql,
                    new
                    {
                        state = (Byte)EnumState.ativo,
                        Situation = Situation
                    });

                return answerSheetBatchQueue;
            }
        }

        private string GetAndFilters(AnswerSheetBatchQueueFilter filter)
        {
            StringBuilder sqlFilter = new StringBuilder();
            if (filter.Situation != null)
            {
                sqlFilter.Append("AND Situation = @Situation ");
            }

            if (filter.StartDate == null && filter.EndDate != null)
            {
                sqlFilter.AppendLine("AND CAST(A.[UpdateDate] AS DATE) <= CAST(@EndDate AS DATE) ");
            }
            else if (filter.StartDate != null && filter.EndDate == null)
            {
                sqlFilter.AppendLine("AND CAST(A.[UpdateDate] AS DATE) >= CAST(@StartDate AS DATE) ");
            }
            else if (filter.StartDate != null && filter.EndDate != null)
            {
                sqlFilter.AppendLine("AND CAST(A.[UpdateDate] AS DATE) >= CAST(@StartDate AS DATE) AND CAST(A.[UpdateDate] AS DATE) <= CAST(@EndDate AS DATE) ");
            }

            return sqlFilter.ToString();
        }

        private string GetAndFiltersUadSchool(AnswerSheetBatchQueueFilter filter)
        {
            StringBuilder sqlFilter = new StringBuilder();
            if (filter.SupAdmUnitId != null)
            {
                sqlFilter.Append("AND [SupAdmUnit_Id] = @SupAdmUnitId ");
            }
            if (filter.SchoolId != null)
            {
                sqlFilter.Append("AND [School_Id] = @SchoolId ");
            }
            return sqlFilter.ToString();
        }

        private string GetInnerPermissao(AnswerSheetBatchQueueFilter filter)
        {
            StringBuilder sqlInner = new StringBuilder();
            if ((filter.VisionId == (int)EnumSYS_Visao.UnidadeAdministrativa) || (filter.VisionId == (int)EnumSYS_Visao.Gestao))
            {
                sqlInner.Append("INNER JOIN [SGP_ESC_Escola] E WITH (NOLOCK) ON A.[School_Id] = E.[esc_id] AND E.[esc_situacao] = 1 ");
                if ((filter.VisionId == (int)EnumSYS_Visao.UnidadeAdministrativa) && (filter.uads.Count() > 0))
                    sqlInner.Append(string.Format("AND e.uad_id IN ({0}) ", string.Join(",", filter.uads)));
                else if ((filter.VisionId == (int)EnumSYS_Visao.Gestao) && (filter.uads.Count() > 0))
                    sqlInner.Append(string.Format("AND e.uad_idSuperiorGestao IN ({0}) ", string.Join(",", filter.uads)));
            }
            return sqlInner.ToString();
        }
        private string GetAndVisaoIndividual(AnswerSheetBatchQueueFilter filter)
        {
            if (filter.VisionId == (int)EnumSYS_Visao.Individual)
                return "AND A.[CreatedBy_Id] = @UserId ";
            return string.Empty;
        }
        /// <summary>
        /// Inclue INNER de escola e Unidade Administrativa
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="InclueEscola">Inclue INNER da escola quando a função GetInnerPermissao não for utilizada na mesma consulta</param>
        /// <returns></returns>
        private string GetInnerEscolaDRE(AnswerSheetBatchQueueFilter filter, bool InclueEscola)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("LEFT JOIN [SGP_ESC_Escola] E2 WITH (NOLOCK) ON A.[School_Id] = E2.[esc_id] AND E2.[esc_situacao] = 1 ");
            sql.Append("LEFT JOIN [SGP_SYS_UnidadeAdministrativa] UAD WITH (NOLOCK) ON UAD.[uad_id] = E2.[uad_idSuperiorGestao] AND UAD.[uad_situacao] = 1 ");
            return sql.ToString();
        }

        private string GetFieldsEscolaDRE()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(", E2.[esc_nome] AS SchoolName ");
            sql.Append(", CASE WHEN UAD.[uad_sigla] IS NOT NULL THEN UAD.[uad_sigla] + ' - ' + UAD.[uad_nome] ELSE UAD.[uad_nome] END AS SupAdmUnitName ");
            return sql.ToString();
        }
        #endregion

        #region Write
        public AnswerSheetBatchQueue Save(AnswerSheetBatchQueue entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {

                GestaoAvaliacaoContext.AnswerSheetBatchQueue.Add(entity);
                GestaoAvaliacaoContext.SaveChanges();
                return entity;
            }

        }

        public AnswerSheetBatchQueue Update(long Id, AnswerSheetBatchQueue entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                AnswerSheetBatchQueue answerSheetBatchQueue = GestaoAvaliacaoContext.AnswerSheetBatchQueue.FirstOrDefault(a => a.Id == entity.Id);
                if (answerSheetBatchQueue != null)
                {
                    if (!answerSheetBatchQueue.AnswerSheetBatch_Id.Equals(entity.AnswerSheetBatch_Id))
                        answerSheetBatchQueue.AnswerSheetBatch_Id = entity.AnswerSheetBatch_Id;

                    if (entity.School_Id != null && !answerSheetBatchQueue.School_Id.Equals(entity.School_Id))
                        answerSheetBatchQueue.School_Id = entity.School_Id;

                    if (entity.SupAdmUnit_Id != null && !answerSheetBatchQueue.SupAdmUnit_Id.Equals(entity.SupAdmUnit_Id))
                        answerSheetBatchQueue.SupAdmUnit_Id = entity.SupAdmUnit_Id;

                    if (!answerSheetBatchQueue.Situation.Equals(entity.Situation))
                        answerSheetBatchQueue.Situation = entity.Situation;

                    if (entity.Description != null && (string.IsNullOrEmpty(answerSheetBatchQueue.Description) || (answerSheetBatchQueue.Description != null && !answerSheetBatchQueue.Description.Equals(entity.Description))))
                        answerSheetBatchQueue.Description = entity.Description;

                    if (entity.CountFiles != null && !answerSheetBatchQueue.CountFiles.Equals(entity.CountFiles))
                        answerSheetBatchQueue.CountFiles = entity.CountFiles;

                    if (!answerSheetBatchQueue.File_Id.Equals(entity.File_Id))
                        answerSheetBatchQueue.File_Id = entity.File_Id;

                    answerSheetBatchQueue.UpdateDate = DateTime.Now;

                    GestaoAvaliacaoContext.Entry(answerSheetBatchQueue).State = System.Data.Entity.EntityState.Modified;
                    GestaoAvaliacaoContext.SaveChanges();
                }
                return answerSheetBatchQueue;
            }
        }

        public void UpdateZipProcessing(AnswerSheetBatchQueue entity)
        {
            var sql = new StringBuilder("UPDATE AnswerSheetBatchQueue SET UpdateDate = @updateDate, Situation = @situation, Description = @description");
            sql.AppendLine("WHERE Situation = @erro AND State <> @state");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                cn.Execute(sql.ToString(),
                    new
                    {
                        updateDate = DateTime.Now,
                        situation = entity.Situation,
                        description = entity.Description,
                        erro = EnumBatchQueueSituation.Processing,
                        state = EnumState.excluido
                    });
            }
        }

        public void Delete(AnswerSheetBatchQueue entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;
                AnswerSheetBatchQueue _entity = gestaoAvaliacaoContext.AnswerSheetBatchQueue.FirstOrDefault(x => x.Id == entity.Id);
                _entity.AnswerSheetBatchFiles = gestaoAvaliacaoContext.AnswerSheetBatchFiles.Where(p => p.AnswerSheetBatchQueue_Id == entity.Id).ToList();

                _entity.State = Convert.ToByte(EnumState.excluido);
                _entity.UpdateDate = dateNow;

                foreach (AnswerSheetBatchFiles item in _entity.AnswerSheetBatchFiles)
                {
                    item.State = Convert.ToByte(EnumState.excluido);
                    item.UpdateDate = dateNow;
                }

                gestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                gestaoAvaliacaoContext.SaveChanges();
            }
        }

        public IEnumerable<AnswerSheetBatchFiles> SelectFilesError(AnswerSheetBatchQueue entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                return gestaoAvaliacaoContext.AnswerSheetBatchFiles.Where(p => p.AnswerSheetBatchQueue_Id == entity.Id && (p.Situation == EnumBatchSituation.Error || p.Situation == EnumBatchSituation.NotIdentified)).ToList();
            }
        }

        #endregion
    }
}