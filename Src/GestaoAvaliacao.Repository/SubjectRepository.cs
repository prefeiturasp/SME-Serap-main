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
using System.Threading.Tasks;

namespace GestaoAvaliacao.Repository
{
    public class SubjectRepository : ConnectionReadOnly, ISubjectRepository
    {
        #region Read

        public Subject Get(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT Id, Description " +
                            "FROM Subject " +
                            "WHERE Id = @id ";

                var subject = cn.Query<Subject>(sql, new { id = id }).FirstOrDefault();

                return subject;
            }

        }

        public List<Discipline> SearchSubjects(string assunto, string subassunto, Guid EntityId, ref Pager pager)
        {
            using (IDbConnection cn = Connection)
            {
                List<Discipline> lstDiscipline = new List<Discipline>();

                cn.Open();
                var sqlDisciplina = @"WITH Disciplinas AS " +
                            "( " +
                               "SELECT Id, DIS.Description, TNE.tne_nome, " +
                               "ROW_NUMBER() OVER (ORDER BY Description) AS RowNumber " +
                               "FROM Discipline AS DIS " +
                               "INNER JOIN dbo.SGP_ACA_TipoNivelEnsino AS TNE " +
                               "ON TNE.tne_id = DIS.TypeLevelEducationId " +
                               "WHERE State != @state " +
                               "AND EntityId = @entityid " +
                               "AND EXISTS (SELECT SD.Discipline_Id FROM dbo.SubjectDiscipline AS SD " +
                               "INNER JOIN dbo.Subject AS SUB " +
                               "ON SUB.Id = SD.Subject_Id WHERE Discipline_Id = DIS.Id " +
                               "AND SUB.State != @state) " +
                            ") " +
                           "SELECT Id, Description, tne_nome " +
                           "FROM Disciplinas " +
                           "WHERE RowNumber > ( @pageSize * @page ) " +
                           "AND RowNumber <= ( ( @page + 1 ) * @pageSize ) " +
                           "ORDER BY RowNumber";

                var sqlAssunto = @"; WITH Assunto AS (
	                                    SELECT SUB.Id, SUB.Description, SD.Discipline_Id, K.Description AS KnowledgeArea_Desc
	                                    FROM Subject AS SUB 
	                                    INNER JOIN dbo.SubjectDiscipline AS SD 
	                                    ON SD.Subject_Id = SUB.Id 
	                                    INNER JOIN SubjectKnowledgeArea AS SKA WITH(NOLOCK)
		                                    ON SUB.Id = SKA.Subject_Id
	                                    INNER JOIN KnowledgeArea AS K WITH(NOLOCK)
		                                    ON SKA.KnowledgeArea_Id = K.Id
	                                    WHERE SUB.State != @state 
	                                    AND SUB.EntityId = @entityid 
	                                    AND (@assunto IS NULL OR SUB.Description LIKE '%' + @assunto + '%')
                                    )

                                    SELECT A.Id, A.Description, A.Discipline_Id,
                                     STUFF(( SELECT  ', ' + A2.KnowledgeArea_Desc
                                                    FROM Assunto AS A2 WITH(NOLOCK)
                                                    WHERE   
					                                    A.Id = A2.Id
				                                    GROUP BY A2.KnowledgeArea_Desc
                                                    ORDER BY A2.KnowledgeArea_Desc
                                                    FOR
                                                    XML PATH('')
                                                    ), 1, 1, '') AS KnowledgeArea_Description
                                    FROM Assunto A
                                    GROUP BY A.Id, A.Description, A.Discipline_Id 
                                    ORDER BY A.Description";

                var sqlSubAssunto = @"SELECT Id, Description, Subject_Id " +
                               "FROM SubSubject " +
                               "WHERE State != @state " +
                               "AND @subassunto IS NULL OR Description LIKE '%' + @subassunto + '%'";

                var disciplines = cn.Query<Discipline>(sqlDisciplina, new { state = (Byte)EnumState.excluido, entityid = EntityId, pageSize = pager.PageSize, page = pager.CurrentPage });
                var assuntos = cn.Query<Subject>(sqlAssunto, new { state = (Byte)EnumState.excluido, entityid = EntityId, assunto = assunto });
                var subassuntos = cn.Query<SubSubject>(sqlSubAssunto, new { state = (Byte)EnumState.excluido, subassunto = subassunto });

                lstDiscipline.AddRange(disciplines);

                foreach (Discipline discipline in lstDiscipline)
                {
                    discipline.Subjects.AddRange(assuntos.ToList().FindAll(p => p.Discipline_Id == discipline.Id));

                    foreach (Subject subject in discipline.Subjects)
                    {
                        subject.SubSubjects.AddRange(subassuntos.ToList().FindAll(p => subject.Discipline_Id == discipline.Id && p.Subject_Id == subject.Id));
                    }

                    discipline.Subjects.RemoveAll(p => p.SubSubjects.Count == 0);
                }

                lstDiscipline.RemoveAll(p => p.Subjects.Count == 0);

                pager.SetTotalPages((int)Math.Ceiling(lstDiscipline.Count() / (double)pager.PageSize));
                pager.SetTotalItens(lstDiscipline.Count());

                return lstDiscipline;
            }
        }

        public Subject GetSubject(long id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT Id, Description, State 
                            FROM Subject AS SUB WITH(NOLOCK)
                            WHERE Id = @id 
	                            AND state != @state 
                            SELECT Id, Description 
                            FROM SubSubject
                            WHERE Subject_Id = @id
	                            AND state != @state 
                            SELECT DIS.Id, DIS.Description 
                            FROM SubjectDiscipline AS SD WITH(NOLOCK)
                            INNER JOIN Discipline AS DIS WITH(NOLOCK)
	                            ON DIS.Id = SD.Discipline_Id
                            WHERE Subject_Id = @id
                            SELECT KA.Id, KA.Description 
                            FROM SubjectKnowledgeArea AS SKA WITH(NOLOCK)
                            INNER JOIN KnowledgeArea AS KA WITH(NOLOCK)
	                            ON KA.Id = SKA.KnowledgeArea_Id
                            WHERE Subject_Id = @id ";

                var multi = cn.QueryMultiple(sql, new { id = id, state = (Byte)EnumState.excluido });

                var listSubject = multi.Read<Subject>();
                var listSubSubject = multi.Read<SubSubject>();
                var listSubjectDiscipline = multi.Read<Discipline>();
                var listSubjectKnowledgeArea = multi.Read<KnowledgeArea>();

                var subject = listSubject.FirstOrDefault();

                foreach (var subsubject in listSubSubject)
                {
                    subject.SubSubjects.Add(subsubject);
                }

                foreach (var subjectDiscipline in listSubjectDiscipline)
                {
                    subject.Disciplines.Add(subjectDiscipline);
                }

                foreach (var subjectKnowledgeArea in listSubjectKnowledgeArea)
                {
                    subject.KnowledgeAreas.Add(subjectKnowledgeArea);
                }

                return subject;
            }
        }

        public Subject LoadSubjectBySubsubject(long idSubsubject)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT SUB.Id, SUB.Description, SUBS.Id AS Subsubject_Id, SUBS.Description AS Subsubject_Description
                            FROM SubSubject AS SUBS WITH(NOLOCK) 
                            INNER JOIN Subject AS SUB WITH(NOLOCK)
                                ON SUB.Id = SUBS.Subject_Id
                            WHERE SUBS.Id = @id ";

                var subject = cn.Query<Subject>(sql, new { id = idSubsubject }).FirstOrDefault();

                return subject;
            }

        }

        public List<AJX_Select2> LoadAllSubjects(string description, Guid EntityId)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT Id, Description " +
                           "FROM Subject " +
                           "WHERE EntityId = @entityid " +
                           "AND State = @state " +
                           "AND (@Description IS NULL OR Description LIKE '%' + @Description + '%') ";

                var lstsubjects = cn.Query<Subject>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, Description = description });

                List<AJX_Select2> lstAJX_Select2 = new List<AJX_Select2>();

                foreach (Subject subject in lstsubjects)
                {
                    AJX_Select2 AJX_Select2 = new AJX_Select2();

                    AJX_Select2.id = subject.Id.ToString();
                    AJX_Select2.text = subject.Description;

                    lstAJX_Select2.Add(AJX_Select2);
                }

                return lstAJX_Select2;
            }
        }

        public List<AJX_Select2> ObterAssuntosPorDisciplinaId(Guid EntityId, long DisciplinaId)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"SELECT Id, Description
                                FROM Subject s
                            inner join SubjectDiscipline sd on s.id = sd.Subject_Id
                                WHERE EntityId = @entityid
                                AND State = @state
                                and sd.Discipline_Id = @disciplinaId";

                var lstsubjects = cn.Query<Subject>(sql, new { state = (Byte)EnumState.ativo, entityid = EntityId, disciplinaId = DisciplinaId });

                List<AJX_Select2> lstAJX_Select2 = new List<AJX_Select2>();

                foreach (Subject subject in lstsubjects)
                {
                    AJX_Select2 AJX_Select2 = new AJX_Select2();

                    AJX_Select2.id = subject.Id.ToString();
                    AJX_Select2.text = subject.Description;

                    lstAJX_Select2.Add(AJX_Select2);
                }

                return lstAJX_Select2;
            }
        }

        public List<AJX_Select2> LoadSubsubjectBySubject(string description, string subjects, Guid EntityId)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine("SELECT SS.Id, SS.Description ");
                sql.AppendLine("FROM SubSubject AS SS WITH(NOLOCK) ");
                sql.AppendLine(string.Format("WHERE @subjects IS NOT NULL AND SS.Subject_Id IN ({0}) ", subjects));
                sql.AppendLine("AND SS.State = @state ");
                sql.AppendLine("AND (@Description IS NULL OR SS.Description LIKE '%' + @Description + '%') ");
                sql.AppendLine("GROUP BY SS.Id, SS.Description ");
                sql.AppendLine("ORDER BY SS.Description ");

                var lstSubSubject = cn.Query<SubSubject>(sql.ToString(), new { state = (Byte)EnumState.ativo, entityid = EntityId, subjects = subjects, Description = description });

                List<AJX_Select2> lstAJX_Select2 = new List<AJX_Select2>();

                foreach (SubSubject subsubject in lstSubSubject)
                {
                    AJX_Select2 AJX_Select2 = new AJX_Select2();

                    AJX_Select2.id = subsubject.Id.ToString();
                    AJX_Select2.text = subsubject.Description;

                    lstAJX_Select2.Add(AJX_Select2);
                }

                return lstAJX_Select2;
            }
        }

        public bool ExistsDescription(string description, Guid ent_id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT COUNT(Id) " +
                           "FROM Subject " +
                           "WHERE Description COLLATE Latin1_General_CI_AS = @Description " +
                           "AND EntityId = @entityid " +
                           "AND State != @state ";

                var count = (int)cn.ExecuteScalar(sql, new { Description = description, entityid = ent_id, state = (Byte)EnumState.excluido });

                return count > 0;
            }
        }

        public bool ExistsItemRelated(Subject subject, Guid ent_id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT COUNT(I.Id) 
                           FROM Item AS I WITH(NOLOCK)
                           INNER JOIN SubSubject AS SS WITH(NOLOCK)
                                ON I.SubSubject_Id = SS.Id
                           INNER JOIN Subject AS S WITH(NOLOCK)
                                ON SS.Subject_Id = S.Id
                           WHERE S.Id = @Id
                           AND S.EntityId = @entityid
                           AND I.State != @state";

                var count = (int)cn.ExecuteScalar(sql, new { Id = subject.Id, entityid = ent_id, state = (Byte)EnumState.excluido });

                return count > 0;
            }
        }

        public bool IsDeletedSubSubjectBeenUsed(Subject entity)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    var _entity = GestaoAvaliacaoContext.Subject.Include("SubSubjects").Where(x => x.Id == entity.Id && x.SubSubjects.Any(m => m.State == (Byte)EnumState.ativo)).FirstOrDefault();
                    foreach (var existingChild in _entity.SubSubjects.ToList())
                    {
                        if (!entity.SubSubjects.Any(c => c.Id == existingChild.Id))
                        {
                            return GestaoAvaliacaoContext.Item.FirstOrDefault(x => x.SubSubject.Id == existingChild.Id) != null;
                        }
                    }
                    return false;
                }
            }
        }

        #endregion

        #region CRUD

        public Subject Save(Subject entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                Subject _entity = new Subject
                {
                    Description = entity.Description,
                    EntityId = entity.EntityId,
                    State = entity.State
                };

                foreach (var subsubject in entity.SubSubjects)
                {
                    subsubject.State = (Byte)EnumState.ativo;
                    subsubject.CreateDate = dateNow;
                    subsubject.UpdateDate = dateNow;
                }

                _entity.SubSubjects = new List<SubSubject>();
                _entity.SubSubjects.AddRange(entity.SubSubjects);

                List<KnowledgeArea> lstKnowledgeArea = new List<KnowledgeArea>();

                foreach (var knowledgeAreas in entity.KnowledgeAreas)
                {
                    KnowledgeArea knowledgeArea = new KnowledgeArea();
                    knowledgeArea = gestaoAvaliacaoContext.KnowledgeArea.FirstOrDefault(s => s.Id == knowledgeAreas.Id);
                    lstKnowledgeArea.Add(knowledgeArea);
                }

                List<Discipline> lstDiscipline = new List<Discipline>();

                foreach (var disciplines in entity.Disciplines)
                {
                    Discipline discipline = new Discipline();
                    discipline = gestaoAvaliacaoContext.Discipline.FirstOrDefault(s => s.Id == disciplines.Id);
                    lstDiscipline.Add(discipline);
                }

                _entity.KnowledgeAreas.AddRange(lstKnowledgeArea);
                _entity.Disciplines.AddRange(lstDiscipline);

                gestaoAvaliacaoContext.Subject.Add(_entity);
                gestaoAvaliacaoContext.SaveChanges();

                return _entity;
            }
        }

        public Subject Update(Subject entity)
        {

            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                var _entity = gestaoAvaliacaoContext.Subject.Include("SubSubjects").Where(x => x.Id == entity.Id && x.SubSubjects.Any(m => m.State == (Byte)EnumState.ativo)).FirstOrDefault();

                gestaoAvaliacaoContext.Entry(_entity).CurrentValues.SetValues(entity);

                _entity.UpdateDate = dateNow;

                foreach (var existingChild in _entity.SubSubjects.ToList())
                {
                    if (!entity.SubSubjects.Any(c => c.Id == existingChild.Id))
                    {
                        existingChild.State = (Byte)EnumState.excluido;
                        existingChild.UpdateDate = dateNow;
                        gestaoAvaliacaoContext.Entry(existingChild);
                    }
                }
                List<SubSubject> newSubSubject = new List<SubSubject>();

                foreach (var childModel in entity.SubSubjects)
                {
                    var existingChild = _entity.SubSubjects.SingleOrDefault(c => c.Id == childModel.Id);

                    if (existingChild != null)
                    {
                        existingChild.UpdateDate = dateNow;
                        gestaoAvaliacaoContext.Entry(existingChild).CurrentValues.SetValues(childModel);
                    }
                    else
                    {
                        var newChild = new SubSubject
                        {
                            Description = childModel.Description,
                            State = (Byte)EnumState.ativo,
                            CreateDate = dateNow,
                            UpdateDate = dateNow
                        };
                        newSubSubject.Add(newChild);
                    }
                }
                _entity.SubSubjects.AddRange(newSubSubject);
                gestaoAvaliacaoContext.SaveChanges();

                return _entity;
            }

        }

        public void Delete(Subject entity)
        {

            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;
                Subject _entity = gestaoAvaliacaoContext.Subject.FirstOrDefault(x => x.Id == entity.Id);

                _entity.State = Convert.ToByte(EnumState.excluido);
                _entity.UpdateDate = dateNow;

                foreach (SubSubject item in _entity.SubSubjects)
                {
                    item.State = Convert.ToByte(EnumState.excluido);
                    item.UpdateDate = dateNow;
                }

                gestaoAvaliacaoContext.Entry(_entity).State = System.Data.Entity.EntityState.Modified;
                gestaoAvaliacaoContext.SaveChanges();
            }
        }

        #endregion
    }
}
