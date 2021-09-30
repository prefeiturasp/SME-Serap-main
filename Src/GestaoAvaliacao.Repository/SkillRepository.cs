using Dapper;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.Enumerator;
using GestaoAvaliacao.IRepository;
using GestaoAvaliacao.Repository.Context;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GestaoAvaliacao.Repository
{
    public class SkillRepository : ConnectionReadOnly, ISkillRepository
    {
        #region CRUD

        public Skill Save(Skill entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;
                entity.CreateDate = dateNow;
                entity.UpdateDate = dateNow;
                entity.State = Convert.ToByte(EnumState.ativo);
                entity.EvaluationMatrix = gestaoAvaliacaoContext.EvaluationMatrix.FirstOrDefault(s => s.Id == entity.EvaluationMatrix.Id);
                entity.ModelSkillLevel = gestaoAvaliacaoContext.ModelSkillLevel.FirstOrDefault(s => s.Id == entity.ModelSkillLevel.Id);

                gestaoAvaliacaoContext.Skill.Add(entity);
                gestaoAvaliacaoContext.SaveChanges();
            }
            return entity;
        }

        public void Update(Skill entity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                Skill skill = gestaoAvaliacaoContext.Skill.FirstOrDefault(s => s.Id == entity.Id);

                if (!string.IsNullOrEmpty(entity.Description))
                    skill.Description = entity.Description;

                if (!string.IsNullOrEmpty(entity.Code))
                    skill.Code = entity.Code;

                if (entity.CognitiveCompetence.Id > 0)
                {
                    skill.CognitiveCompetence_Id = entity.CognitiveCompetence.Id;
                }
                else
                {
                    skill.CognitiveCompetence_Id = null;
                }

                skill.UpdateDate = DateTime.Now;

                gestaoAvaliacaoContext.Entry(skill).State = System.Data.Entity.EntityState.Modified;

                gestaoAvaliacaoContext.SaveChanges();
            }
        }

        public Skill Get(long id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return gestaoAvaliacaoContext.Skill.Include("Parent").Include("EvaluationMatrix").Include("CognitiveCompetence").AsNoTracking().FirstOrDefault(bt => bt.Id == id);
                }
            }
        }

        public IEnumerable<Skill> Load(ref Pager pager)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return pager.Paginate(gestaoAvaliacaoContext.Skill.AsNoTracking().Where(x => x.State == (Byte)EnumState.ativo).OrderBy(x => x.Description).AsQueryable());
                }
            }
        }

        public void Delete(Skill entity)
        {
            using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                Skill skill = GestaoAvaliacaoContext.Skill.FirstOrDefault(a => a.Id == entity.Id);

                IEnumerable<Skill> listSkill = GestaoAvaliacaoContext.Skill.Where(a => a.Parent.Id == entity.Id && a.State == (Byte)EnumState.ativo);

                foreach (var s in listSkill)
                {
                    s.State = Convert.ToByte(EnumState.excluido);
                    s.UpdateDate = DateTime.Now;
                }

                skill.State = Convert.ToByte(EnumState.excluido);
                skill.UpdateDate = DateTime.Now;

                GestaoAvaliacaoContext.Entry(skill).State = System.Data.Entity.EntityState.Modified;
                GestaoAvaliacaoContext.SaveChanges();
            }
        }

        #endregion

        #region Custom methods

        public IEnumerable<Skill> Search(String search, ref Pager pager)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return pager.Paginate(gestaoAvaliacaoContext.Skill.AsNoTracking().Where(a => a.Description.Contains(search) && a.State == (Byte)EnumState.ativo).OrderBy(x => x.Description));
                }
            }
        }

        public IEnumerable<Skill> GetByMatrix(long idMatrix)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query =
                        ctx.Skill.AsNoTracking().Include("EvaluationMatrix").Include("ModelSkillLevel").Include("Parent").Where(
                            s =>
                            s.State == (byte)EnumState.ativo && s.EvaluationMatrix.Id == idMatrix &&
                            s.EvaluationMatrix.State == (byte)EnumState.ativo &&
                            s.ModelSkillLevel.State == (byte)EnumState.ativo).ToList();

                    return query;
                }
            }
        }

        public IEnumerable<Skill> GetByDiscipline(long disciplineId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query =
                        ctx.Skill.AsNoTracking().Include("EvaluationMatrix").Include("ModelSkillLevel").Include("Parent").Where(
                            s =>
                            s.State == (byte)EnumState.ativo &&
                            (s.EvaluationMatrix.Discipline.Id == disciplineId || disciplineId == 0) &&
                            s.EvaluationMatrix.State == (byte)EnumState.ativo &&
                            s.ModelSkillLevel.State == (byte)EnumState.ativo).ToList();

                    return query;
                }
            }
        }

        public IEnumerable<Skill> GetByCognitiveCompetence(long idCognitiveCompetence)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query =
                        ctx.Skill.AsNoTracking().Include("CognitiveCompetence").Where(
                            s =>
                            s.State == (byte)EnumState.ativo && s.CognitiveCompetence.Id == idCognitiveCompetence &&
                            s.EvaluationMatrix.State == (byte)EnumState.ativo &&
                            s.ModelSkillLevel.State == (byte)EnumState.ativo).ToList();

                    return query;
                }
            }
        }

        public IEnumerable<Skill> GetByParent(long idSkill)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext ctx = new GestaoAvaliacaoContext())
                {
                    var query =
                        ctx.Skill.AsNoTracking().Include("EvaluationMatrix").Include("ModelSkillLevel").Where(
                            s =>
                            s.State == (byte)EnumState.ativo && s.Parent.Id == idSkill &&
                            s.EvaluationMatrix.State == (byte)EnumState.ativo &&
                            s.ModelSkillLevel.State == (byte)EnumState.ativo).ToList();

                    return query;
                }
            }
        }

        public IEnumerable<Skill> GetComboByDiscipline(long idDiscipline)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var sql = @"SELECT S.Id, S.[Description], S.Code
                            FROM EvaluationMatrix AS E WITH(NOLOCK) 
                            INNER JOIN Skill AS S WITH(NOLOCK) 
                            ON S.EvaluationMatrix_Id = E.Id
                            WHERE e.Discipline_Id = @idDiscipline 
                            AND ModelSkillLevel_Id = 2
                            AND S.State = @state
                            AND e.State = @state";

                var skill = cn.Query<Skill>(sql, new
                {
                    idDiscipline = idDiscipline,
                    state = (Byte)EnumState.ativo
                });

                return skill;
            }
        }

        public IEnumerable<Skill> SearchByMatrix(long evaluationMatrixId, long modelSkillLevelId, long? parentId, ref Pager pager)
        {
            var sql = new StringBuilder("SELECT s.Id, s.Code, s.Description, s.LastLevel, msl.Id, msl.Description, msl.Level, s1.Id, cc.Id, cc.Description ");
            sql.Append("FROM Skill s (NOLOCK) ");
            sql.Append("INNER JOIN ModelSkillLevel msl (NOLOCK) ON msl.Id = s.ModelSkillLevel_Id ");
            sql.Append("LEFT JOIN Skill s1 (NOLOCK) ON s.Parent_Id = s1.Id ");
            sql.Append("LEFT JOIN CognitiveCompetence cc (NOLOCK) ON cc.Id = s.CognitiveCompetence_Id ");
            sql.Append("WHERE s.EvaluationMatrix_Id = @evaluationMatrixId AND s.ModelSkillLevel_Id = @modelSkillLevelId AND s.State = @state ");

            if (parentId.HasValue)
                sql.Append("AND s.Parent_Id = @parentId ");

            sql.Append("ORDER BY s.CreateDate DESC ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<Skill, ModelSkillLevel, Skill, CognitiveCompetence, Skill>(sql.ToString(),
                    (s, msl, s1, cc) =>
                    {
                        s.ModelSkillLevel = msl;

                        if (s1 != null)
                            s.Parent = s1;

                        if (cc != null)
                            s.CognitiveCompetence = cc;
                        return s;
                    }, new { evaluationMatrixId = evaluationMatrixId, modelSkillLevelId = modelSkillLevelId, parentId = parentId, state = (byte)EnumState.ativo });

            }
        }

        public IEnumerable<Skill> LoadByMatrix(long evaluationMatrixId, long modelSkillLevelId, long? parentId, ref Pager pager)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    if (parentId == null)
                        return pager.Paginate(gestaoAvaliacaoContext.Skill.Include("Parent").Include("ModelSkillLevel").AsNoTracking().Where(x => x.EvaluationMatrix.Id == evaluationMatrixId && x.ModelSkillLevel.Id == modelSkillLevelId && x.State == (Byte)EnumState.ativo).OrderBy(x => x.CreateDate).AsQueryable());
                    else
                        return pager.Paginate(gestaoAvaliacaoContext.Skill.Include("Parent").Include("ModelSkillLevel").AsNoTracking().Where(x => x.EvaluationMatrix.Id == evaluationMatrixId && x.ModelSkillLevel.Id == modelSkillLevelId && x.Parent.Id == parentId && x.State == (Byte)EnumState.ativo).OrderBy(x => x.CreateDate).AsQueryable());


                }
            }
        }

        public void SaveRange(List<Skill> listEntity)
        {
            using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
            {
                DateTime dateNow = DateTime.Now;

                foreach (Skill skill in listEntity)
                {
                    skill.UpdateDate = dateNow;
                    skill.State = Convert.ToByte(EnumState.ativo);
                    skill.EvaluationMatrix = gestaoAvaliacaoContext.EvaluationMatrix.Find(skill.EvaluationMatrix.Id);
                    skill.ModelSkillLevel = gestaoAvaliacaoContext.ModelSkillLevel.Find(skill.ModelSkillLevel.Id);
                    skill.Parent = gestaoAvaliacaoContext.Skill.Find(skill.Parent.Id);

                    if (skill.CognitiveCompetence != null && skill.CognitiveCompetence.Id > 0)
                    {
                        skill.CognitiveCompetence_Id = skill.CognitiveCompetence.Id;
                        skill.CognitiveCompetence = null;
                    }
                }

                gestaoAvaliacaoContext.Skill.AddRange(listEntity);
                gestaoAvaliacaoContext.SaveChanges();
            }
        }

        public object GetParent(long id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext gestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {

                    var aux = (
                        from s in gestaoAvaliacaoContext.Skill
                        where s.Id == id
                        select new
                        {
                            Id = s.Id,
                            Description = s.Description,
                            Code = s.Code,
                            LastLevel = s.LastLevel,
                            ModelSkillLevel = new
                            {
                                Id = s.ModelSkillLevel.Id,
                                Level = s.ModelSkillLevel.Level,
                                Description = s.ModelSkillLevel.Description,
                                LastLevel = s.ModelSkillLevel.LastLevel
                            },
                            EvaluationMatrix = s.EvaluationMatrix.Id,
                            Parent = new
                            {
                                Id = s.Parent == null ? 0 : s.Parent.Id,
                                Level = s.Parent == null ? 0 : s.Parent.ModelSkillLevel.Id
                            }
                        }
                    );

                    return aux.FirstOrDefault();
                }
            }
        }

        public bool ExistsCode(string code, long modelSkillLevelId, long evaluationMatrixId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.Skill.AsNoTracking().FirstOrDefault(
                        a => a.Code.Equals(code) && a.ModelSkillLevel.Id == modelSkillLevelId && a.EvaluationMatrix.Id == evaluationMatrixId && a.State == (Byte)EnumState.ativo) != null;
                }
            }
        }

        public bool ExistsCodeAlter(string code, long modelSkillLevelId, long evaluationMatrixId, long id)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.Skill.AsNoTracking().FirstOrDefault(
                        a => a.Code.Equals(code) && a.ModelSkillLevel.Id == modelSkillLevelId && a.EvaluationMatrix.Id == evaluationMatrixId && a.Id != id && a.State == (Byte)EnumState.ativo) != null;
                }
            }
        }

        public bool ExistsItemSkill(long skillId, long evaluationMatrixId)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    return GestaoAvaliacaoContext.ItemSkill.AsNoTracking().FirstOrDefault(
                        a => a.Item.EvaluationMatrix.Id == evaluationMatrixId && a.Skill.Id == skillId && a.State == (Byte)EnumState.ativo) != null;
                }
            }
        }

        public IEnumerable<ItemReportItemSkill> GetBySkillReport(int id, int skill, Guid EntityId, long TypeLevelEducation)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    SqlParameter disciplina = new SqlParameter();
                    disciplina.SqlDbType = SqlDbType.VarChar;
                    disciplina.ParameterName = "@disciplina";

                    if (id == 0)
                        disciplina.Value = DBNull.Value;
                    else
                    {
                        disciplina.Value = id;
                    }

                    SqlParameter Skill = new SqlParameter("@Skill", skill);
                    Skill.SqlDbType = SqlDbType.Int;

                    SqlParameter entityId = new SqlParameter("@EntityId", EntityId);
                    entityId.SqlDbType = SqlDbType.UniqueIdentifier;

                    SqlParameter typeLevelEducation = new SqlParameter();
                    typeLevelEducation.SqlDbType = SqlDbType.Int;
                    typeLevelEducation.ParameterName = "@typeLevelEducation";

                    if (TypeLevelEducation == 0)
                        typeLevelEducation.Value = DBNull.Value;
                    else
                    {
                        typeLevelEducation.Value = TypeLevelEducation;
                    }

                    var myEntities =
                        GestaoAvaliacaoContext.Database.SqlQuery<ItemReportItemSkill>(
                            "EXEC MS_Item_ReportItemSkill @disciplina, @Skill, @EntityId, @typeLevelEducation", disciplina, Skill, entityId, typeLevelEducation).ToList<ItemReportItemSkill>();
                    return myEntities;
                }
            }
        }
        public IEnumerable<ItemReportItemSkill> GetBySkillReportOneLevel(int id, int matrizId, Guid EntityId, long TypeLevelEducation)
        {
            var transactionOptions = new System.Transactions.TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
            };

            using (new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (GestaoAvaliacaoContext GestaoAvaliacaoContext = new GestaoAvaliacaoContext())
                {
                    SqlParameter disciplina = new SqlParameter();
                    disciplina.SqlDbType = SqlDbType.VarChar;
                    disciplina.ParameterName = "@disciplina";

                    if (id == 0)
                        disciplina.Value = DBNull.Value;
                    else
                    {
                        disciplina.Value = id;
                    }

                    SqlParameter matriz = new SqlParameter("@matriz", matrizId);
                    matriz.SqlDbType = SqlDbType.Int;

                    SqlParameter entityId = new SqlParameter("@EntityId", EntityId);
                    entityId.SqlDbType = SqlDbType.UniqueIdentifier;

                    SqlParameter typeLevelEducation = new SqlParameter();
                    typeLevelEducation.SqlDbType = SqlDbType.Int;
                    typeLevelEducation.ParameterName = "@typeLevelEducation";

                    if (TypeLevelEducation == 0)
                        typeLevelEducation.Value = DBNull.Value;
                    else
                    {
                        typeLevelEducation.Value = TypeLevelEducation;
                    }

                    var myEntities =
                        GestaoAvaliacaoContext.Database.SqlQuery<ItemReportItemSkill>(
                            "EXEC MS_Item_ReportItemSkillOneLevel @disciplina, @matriz, @EntityId, @typeLevelEducation", disciplina, matriz, entityId, typeLevelEducation).ToList<ItemReportItemSkill>();
                    return myEntities;
                }
            }
        }
        #endregion
    }
}
