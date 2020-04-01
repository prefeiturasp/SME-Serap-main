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

namespace GestaoAvaliacao.Repository
{
    public class KnowledgeAreaRepository : ConnectionReadOnly, IKnowledgeAreaRepository
    {
        public KnowledgeArea Get(long id)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                using (IDbConnection cn = Connection)
                {
                    cn.Open();

                    var sql = @"SELECT Id, Description " +
                               "FROM KnowledgeArea " +
                               "WHERE Id = @id " +
                               "AND state != @state " +
                               "SELECT Discipline_Id " +
                               "FROM KnowledgeAreaDiscipline " +
                               "WHERE KnowledgeArea_Id = @id " +
                               "AND state != @state ";

                    var multi = cn.QueryMultiple(sql, new { id = id, state = (Byte)EnumState.excluido });

                    var listKnowledgeArea = multi.Read<KnowledgeArea>();
                    var listKnowledgeAreaDiscipline = multi.Read<KnowledgeAreaDiscipline>();

                    var knowledgeArea = listKnowledgeArea.FirstOrDefault();

                    foreach (var knowledgeAreaDiscipline in listKnowledgeAreaDiscipline)
                    {
                        knowledgeAreaDiscipline.Discipline = gestaoAvaliacaoContext.Discipline.FirstOrDefault(s => s.Id == knowledgeAreaDiscipline.Discipline_Id);
                        knowledgeArea.KnowledgeAreaDisciplines.Add(knowledgeAreaDiscipline);
                    }

                    return knowledgeArea;
                }
            }
        }

        public IEnumerable<KnowledgeArea> Load(Guid EntityId, ref Pager pager)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"WITH NumberedKnowledgeArea AS " +
                            "( " +
                               "SELECT Id, Description, " +
                               "ROW_NUMBER() OVER (ORDER BY Description) AS RowNumber " +
                               "FROM KnowledgeArea " +
                               "WHERE State  = @state " +
                               "AND EntityId = @entityid " +
                            ") " +
                           "SELECT Id, Description " +
                           "FROM NumberedKnowledgeArea " +
                           "WHERE RowNumber > ( @pageSize * @page ) " +
                           "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
                           "ORDER BY RowNumber";

                var countSql = @"SELECT COUNT(id) " +
                                "FROM KnowledgeArea " +
                                "WHERE State = @state " +
                                "AND EntityId = @entityid";

                var knowledgeArea = cn.Query<KnowledgeArea>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, pageSize = pager.PageSize, page = pager.CurrentPage });
                var count = (int)cn.ExecuteScalar(countSql, new { state = (Byte)EnumState.ativo, entityid = EntityId });

                pager.SetTotalPages((int)Math.Ceiling(knowledgeArea.Count() / (double)pager.PageSize));
                pager.SetTotalItens(count);

                return knowledgeArea;
            }
        }

        public IEnumerable<KnowledgeArea> Search(Guid EntityId, String search, ref Pager pager)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"WITH NumberedKnowledgeArea AS " +
                            "( " +
                               "SELECT Id, Description, " +
                               "ROW_NUMBER() OVER (ORDER BY Description) AS RowNumber " +
                               "FROM KnowledgeArea " +
                               "WHERE State  = @state " +
                               "AND EntityId = @entityid " +
                               "AND (@search IS NULL OR Description Like '%' + @search + '%') " +
                            ") " +
                           "SELECT Id, Description " +
                           "FROM NumberedKnowledgeArea " +
                           "WHERE RowNumber > ( @pageSize * @page ) " +
                           "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
                           "ORDER BY RowNumber";

                var countSql = @"SELECT COUNT(id) " +
                                "FROM KnowledgeArea " +
                                "WHERE State = @state " +
                                "AND (@search IS NULL OR Description Like '%' + @search + '%') " +
                                "AND EntityId = @entityid";

                var knowledgeArea = cn.Query<KnowledgeArea>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, pageSize = pager.PageSize, page = pager.CurrentPage, search = search });
                var count = (int)cn.ExecuteScalar(countSql, new { state = (Byte)EnumState.ativo, entityid = EntityId, search = search });

                pager.SetTotalPages((int)Math.Ceiling(knowledgeArea.Count() / (double)pager.PageSize));
                pager.SetTotalItens(count);

                return knowledgeArea;
            }
        }

        public bool ExistsModelDescription(long id, string description, Guid ent_id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT COUNT(Id) " +
                           "FROM KnowledgeArea " +
                           "WHERE Description COLLATE Latin1_General_CI_AS = @Description " +
                           "AND EntityId = @entityid " +
                           "AND Id <> @id " +
                           "AND State != @state ";

                var count = (int)cn.ExecuteScalar(sql, new { id = id, Description = description, entityid = ent_id, state = (Byte)EnumState.excluido });

                return count > 0;
            }
        }

        public bool ExistsSubject(long Id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT COUNT(KnowledgeArea_Id)
                           FROM SubjectKnowledgeArea AS SKA WITH (NOLOCK)
                           INNER JOIN Subject AS S WITH (NOLOCK)
                           ON S.Id = SKA.Subject_Id
                           WHERE KnowledgeArea_Id = @id
                           AND State != @state ";

                var count = cn.Query<int>(sql, new { id = Id, state = (Byte)EnumState.excluido }).FirstOrDefault();

                return count > 0;
            }
        }

        public List<AJX_Select2> LoadAllKnowledgeAreaActive(string description, Guid EntityId)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT Id, Description " +
                           "FROM KnowledgeArea " +
                           "WHERE EntityId = @entityid " +
                           "AND State = @state " +
                           "AND (@Description IS NULL OR Description LIKE '%' + @Description + '%') ";

                var lstKnowledgeArea = cn.Query<KnowledgeArea>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, Description = description });

                List<AJX_Select2> lstAJX_Select2 = new List<AJX_Select2>();

                foreach (KnowledgeArea knowledgeArea in lstKnowledgeArea)
                {
                    AJX_Select2 AJX_Select2 = new AJX_Select2();

                    AJX_Select2.id = knowledgeArea.Id.ToString();
                    AJX_Select2.text = knowledgeArea.Description;

                    lstAJX_Select2.Add(AJX_Select2);
                }

                return lstAJX_Select2;
            }
        }

        #region CRUD

        public KnowledgeArea Save(KnowledgeArea entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                KnowledgeArea _entity = new KnowledgeArea
                {
                    Description = entity.Description,
                    EntityId = entity.EntityId,
                    State = entity.State
                };

                foreach (var knowledgeAreaDisciplines in entity.KnowledgeAreaDisciplines)
                {
                    knowledgeAreaDisciplines.State = (Byte)EnumState.ativo;
                    knowledgeAreaDisciplines.CreateDate = dateNow;
                    knowledgeAreaDisciplines.UpdateDate = dateNow;
                    knowledgeAreaDisciplines.Discipline = gestaoAvaliacaoContext.Discipline.FirstOrDefault(s => s.Id == knowledgeAreaDisciplines.Id);
                }

                _entity.KnowledgeAreaDisciplines = new List<KnowledgeAreaDiscipline>();
                _entity.KnowledgeAreaDisciplines.AddRange(entity.KnowledgeAreaDisciplines);

                gestaoAvaliacaoContext.KnowledgeArea.Add(_entity);
                gestaoAvaliacaoContext.SaveChanges();

                return _entity;
            }
        }

        public KnowledgeArea Update(KnowledgeArea entity)
        {

            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                var _entity = gestaoAvaliacaoContext.KnowledgeArea.Include("KnowledgeAreaDisciplines").Where(x => x.Id == entity.Id && x.KnowledgeAreaDisciplines.Any(m => m.State == (Byte)EnumState.ativo)).FirstOrDefault();

                gestaoAvaliacaoContext.Entry(_entity).CurrentValues.SetValues(entity);

                _entity.UpdateDate = dateNow;

                foreach (var existingChild in _entity.KnowledgeAreaDisciplines.ToList())
                {
                    if (!entity.KnowledgeAreaDisciplines.Any(c => c.Discipline_Id == existingChild.Discipline_Id))
                    {
                        existingChild.State = (Byte)EnumState.excluido;
                        existingChild.UpdateDate = dateNow;
                        gestaoAvaliacaoContext.Entry(existingChild);
                    }
                }
                List<KnowledgeAreaDiscipline> newKnowledgeAreaDiscipline = new List<KnowledgeAreaDiscipline>();

                foreach (var childModel in entity.KnowledgeAreaDisciplines)
                {
                    var existingChild = _entity.KnowledgeAreaDisciplines.SingleOrDefault(c => c.Discipline_Id == childModel.Discipline_Id);

                    if (existingChild != null)
                    {
                        existingChild.UpdateDate = dateNow;
                        gestaoAvaliacaoContext.Entry(existingChild).CurrentValues.SetValues(childModel);
                    }
                    else
                    {
                        var newChild = new KnowledgeAreaDiscipline
                        {
                            State = (Byte)EnumState.ativo,
                            CreateDate = dateNow,
                            UpdateDate = dateNow,
                            Discipline = gestaoAvaliacaoContext.Discipline.FirstOrDefault(s => s.Id == childModel.Discipline_Id)
                        };
                        newKnowledgeAreaDiscipline.Add(newChild);
                    }
                }
                _entity.KnowledgeAreaDisciplines.AddRange(newKnowledgeAreaDiscipline);
                gestaoAvaliacaoContext.SaveChanges();

                return _entity;
            }

        }

        public void Delete(KnowledgeArea entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                KnowledgeArea knowledgeArea = GestaoAvaliacaoContext.KnowledgeArea.FirstOrDefault(a => a.Id == entity.Id);

                knowledgeArea.State = Convert.ToByte(EnumState.excluido);
                knowledgeArea.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(knowledgeArea).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        #endregion
    }
}
