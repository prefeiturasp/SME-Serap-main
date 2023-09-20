using Dapper;
using GestaoEscolar.Entities;
using GestaoEscolar.IRepository;
using GestaoEscolar.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoEscolar.Repository
{
    public class SYS_UnidadeAdministrativaRepository : ConnectionReadOnly, ISYS_UnidadeAdministrativaRepository
    {
        public IEnumerable<SYS_UnidadeAdministrativa> LoadSimple(Guid ent_id, IEnumerable<string> uad_id = null)
        {
            var sql = new StringBuilder("SELECT uad.uad_id, uad.uad_nome, uad.uad_codigo, uad.uad_sigla ");
            sql.Append("FROM SYS_UnidadeAdministrativa uad (NOLOCK) ");
            sql.Append("WHERE uad.ent_id = @ent_id AND uad.uad_situacao = @situacao ");
            if (uad_id != null)
                sql.AppendFormat("AND uad_id IN ({0}) ", string.Join(",", uad_id.ToArray()));

            sql.Append("ORDER BY uad.uad_nome");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<SYS_UnidadeAdministrativa>(sql.ToString(), new { ent_id = ent_id, situacao = (byte)1 });

            }
        }
        public SYS_UnidadeAdministrativa GetByUad_Id(Guid uad_id)
        {
            var sql = new StringBuilder("SELECT uad.uad_id, uad.uad_nome, uad.uad_codigo ");
            sql.Append("FROM SYS_UnidadeAdministrativa uad (NOLOCK) ");
            sql.Append("WHERE uad.uad_situacao = @situacao ");
            sql.AppendFormat("AND uad_id = @uad_id ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                return cn.Query<SYS_UnidadeAdministrativa>(sql.ToString(), new { situacao = (byte)1, uad_id = uad_id }).FirstOrDefault();
            }
        }
        public IEnumerable<SYS_UnidadeAdministrativa> LoadSimpleCoordinator(Guid ent_id, IEnumerable<string> uad_id)
		{
			var sql = new StringBuilder();
            sql.AppendLine("SELECT DISTINCT uad.uad_id, uad.uad_nome, uad.uad_codigo");
			sql.AppendLine("FROM ESC_Escola (NOLOCK) e ");
			sql.AppendLine("INNER JOIN SYS_UnidadeAdministrativa (NOLOCK) uad ON e.uad_idSuperiorGestao = uad.uad_id ");
			sql.AppendLine("WHERE uad.uad_situacao = @situacao ");
            sql.AppendLine(string.Format("AND e.uad_id IN ({0})", string.Join(",", uad_id.ToArray())));
			sql.AppendLine("ORDER BY uad.uad_nome");

			using (IDbConnection cn = Connection)
			{
				cn.Open();
				return cn.Query<SYS_UnidadeAdministrativa>(sql.ToString(), new { ent_id = ent_id, situacao = (byte)1 });
			}
		}

        public IEnumerable<SYS_UnidadeAdministrativa> LoadSimpleTeacher(Guid ent_id, Guid pes_id)
        {
            var sql = new StringBuilder("SELECT DISTINCT uad.uad_id, uad.uad_nome, uad.uad_codigo ");
            sql.Append("FROM ACA_Docente a (NOLOCK) ");
            sql.Append("INNER JOIN TUR_TurmaDocente td (NOLOCK) ON a.doc_id = td.doc_id ");
            sql.Append("INNER JOIN TUR_TurmaDisciplina tdis (NOLOCK) ON tdis.tud_id = td.tud_id ");
            sql.Append("INNER JOIN TUR_Turma t (NOLOCK) ON t.tur_id = tdis.tur_id ");
            sql.Append("INNER JOIN ESC_Escola e (NOLOCK) ON e.esc_id = t.esc_id ");
            sql.Append("INNER JOIN SYS_UnidadeAdministrativa uad (NOLOCK) ON uad.uad_id = e.uad_idSuperiorGestao ");
            sql.Append("WHERE a.ent_id = @ent_id AND a.pes_id = @pes_id ");
            sql.Append("AND a.doc_situacao = @situacao AND td.tdt_situacao = @situacao AND t.tur_situacao = @situacao AND e.esc_situacao = @situacao AND uad.uad_situacao = @situacao ");

            sql.Append("ORDER BY uad.uad_nome");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<SYS_UnidadeAdministrativa>(sql.ToString(), new { ent_id = ent_id, pes_id = pes_id, situacao = (byte)1 });
            }
        }
    }
}
