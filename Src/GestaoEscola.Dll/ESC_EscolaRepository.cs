using Dapper;
using GestaoAvaliacao.Business.DTO;
using GestaoEscolar.Entities;
using GestaoEscolar.Entities.Projections;
using GestaoEscolar.IRepository;
using GestaoEscolar.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoEscolar.Repository
{
    public class ESC_EscolaRepository : ConnectionReadOnly, IESC_EscolaRepository
    {
        public IEnumerable<ESC_Escola> LoadSimple(Guid ent_id, Guid uad_id, IEnumerable<string> esc_id = null)
        {
            var sql = new StringBuilder("SELECT esc_id, esc_nome, uad_idSuperiorGestao ");
            sql.Append("FROM ESC_Escola AS ESC WITH (NOLOCK) ");
            sql.Append("INNER JOIN Synonym_AdministrativeUnitType AS AUT WITH(NOLOCK) ");
            sql.Append("ON AUT.AdministrativeUnitTypeId = ESC.tua_id ");
            sql.Append("WHERE ent_id = @ent_id ");
            sql.Append("AND esc_situacao = @state ");
            sql.Append("AND AUT.State = @state ");

            if (uad_id != null && uad_id != Guid.Empty)
                sql.Append("AND uad_idSuperiorGestao = @uad_idSuperiorGestao ");

            if (esc_id != null)
                sql.AppendFormat("AND uad_id IN ({0}) ", string.Join(",", esc_id));

            sql.Append("ORDER BY esc_nome");


            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<ESC_Escola>(sql.ToString(), new { ent_id = ent_id, uad_idSuperiorGestao = uad_id, state = (byte)1 });

            }
        }

        public ESC_Escola GetWithAdministrativeUnity(Guid ent_id, long esc_id)
        {

            string sql = @"SELECT e.esc_id, e.esc_nome,
                           uad.uad_id, uad.uad_nome, uad.uad_sigla 
                           FROM ESC_Escola e WITH (NOLOCK) 
                           INNER JOIN SYS_UnidadeAdministrativa uad (NOLOCK) ON e.uad_idSuperiorGestao = uad.uad_id 
                           WHERE e.ent_id = @ent_id 
                           AND e.esc_situacao = @state 
                           AND e.esc_id = @esc_id ";

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<ESC_Escola, SYS_UnidadeAdministrativa, ESC_Escola>
                    (
                        sql.ToString(),
                        (e, uad) =>
                        {
                            e.SYS_UnidadeAdministrativa = uad;
                            return e;
                        },
                        new { ent_id = ent_id, esc_id = esc_id, state = (byte)1 },
                        splitOn: "uad_id"
                    ).FirstOrDefault();

            }
        }

        public IEnumerable<ESC_Escola> LoadSimpleTeacher(Guid ent_id, Guid pes_id, Guid uad_id)
        {
            var sql = new StringBuilder("SELECT DISTINCT e.esc_id, e.esc_nome, e.uad_id ");
            sql.AppendLine("FROM ACA_Docente (NOLOCK) d ");
            sql.AppendLine("INNER JOIN TUR_TurmaDocente (NOLOCK) tdoc ON d.doc_id = tdoc.doc_id AND tdoc.tdt_situacao = @state ");
            sql.AppendLine("INNER JOIN TUR_TurmaDisciplina (NOLOCK) td ON td.tud_id = tdoc.tud_id AND td.tud_situacao = @state ");
            sql.AppendLine("INNER JOIN TUR_Turma t (NOLOCK) ON t.tur_id = td.tur_id AND t.tur_situacao = @state ");
            sql.AppendLine("INNER JOIN ESC_Escola e (NOLOCK) ON e.esc_id = t.esc_id AND e.esc_situacao = @state ");
            sql.AppendLine("INNER JOIN Synonym_AdministrativeUnitType AS AUT WITH(NOLOCK) ");
            sql.AppendLine("ON AUT.AdministrativeUnitTypeId = e.tua_id ");
            sql.AppendLine("WHERE d.doc_situacao = @state ");
            sql.AppendLine("AND AUT.State = @state ");
            sql.AppendLine("AND e.uad_idSuperiorGestao = @uad_id  AND d.ent_id = @ent_id AND pes_id = @pes_id ");
            sql.Append("ORDER BY e.esc_nome ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<ESC_Escola>(sql.ToString(), new { ent_id = ent_id, pes_id = pes_id, uad_id = uad_id, state = (byte)1 });
            }
        }


        public int GetTotalSchool(Guid ent_id, IEnumerable<string> uad_id = null)
        {
            var sql = new StringBuilder("SELECT COUNT(e.esc_id) ");
            sql.Append("FROM ESC_ESCOLA e ");
            sql.Append("WHERE e.esc_situacao = @state AND e.ent_id = @ent_id ");

            #region Gestor
            if (uad_id != null)
            {
                sql.AppendFormat("AND uad_idSuperiorGestao IN ({0}) ", string.Join(",", uad_id));
            }
            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.ExecuteScalar<int>(sql.ToString(), new { ent_id = ent_id, uad_id = uad_id, state = (byte)1 });
            }
        }



        public ESC_Escola Get(int esc_id)
        {
            var sql = new StringBuilder("SELECT esc_id, ent_id, uad_id, esc_codigo, esc_nome, esc_situacao, esc_dataCriacao, esc_dataAlteracao, uad_idSuperiorGestao ");
            sql.Append("FROM ESC_Escola ");
            sql.Append("WHERE esc_id = @esc_id ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<ESC_Escola>(sql.ToString(), new { esc_id = esc_id }).FirstOrDefault();
            }

        }

        /// <summary>
        /// Busca o nome da DRE e da escola
        /// </summary>
        /// <param name="esc_id">Id da escola</param>
        /// <returns>Projection com o nome da DRE e da escola</returns>
        public SchoolAndDRENamesProjection GetSchoolAndDRENames(int esc_id)
        {
            var sql = new StringBuilder("SELECT esc.esc_nome, uad.uad_nome ")
                                        .AppendLine("FROM ESC_Escola esc ")
                                        .AppendLine("INNER JOIN SYS_UnidadeAdministrativa uad ON uad.uad_id = esc.uad_idSuperiorGestao")
                                        .AppendLine("WHERE esc.esc_id = @esc_id ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<SchoolAndDRENamesProjection>(sql.ToString(), new { esc_id = esc_id }).FirstOrDefault();
            }
        }


        public IEnumerable<EscolaDto> LoadAllSchoollsActiveDto()
        {

            var sql = new StringBuilder("SELECT esc_codigo as EscCodigo , esc_nome as EscNome  ")
                                        .AppendLine("FROM ESC_Escola  ")
                                        .AppendLine("WHERE esc_situacao = 1");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<EscolaDto>(sql.ToString()).ToList();
            }
        }

        public IEnumerable<EscolaDto> ListarEscolasPorcodigoDre(string uad_codigo)
        {
            var sql = @"SELECT esc.esc_codigo as EscCodigo , esc.esc_nome as EscNome
								FROM ESC_Escola esc
								INNER JOIN SYS_UnidadeAdministrativa uad ON uad.uad_id = esc.uad_idSuperiorGestao
								where esc.esc_situacao = 1
								and uad.uad_codigo = @uad_codigo";

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<EscolaDto>(sql.ToString(), new { uad_codigo = uad_codigo }).ToList();
            }
        }
    }
}
