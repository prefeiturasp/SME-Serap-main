using Dapper;
using GestaoEscolar.Entities;
using GestaoEscolar.Entities.DTO;
using GestaoEscolar.IRepository;
using GestaoEscolar.Repository.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoEscolar.Repository
{
    public class TUR_TurmaRepository : ConnectionReadOnly, ITUR_TurmaRepository
    {
        public IEnumerable<TUR_Turma> Load(int esc_id, int ttn_id, Guid? pes_id = null, Guid? ent_id = null)
        {
            var sql = new StringBuilder("SELECT tur.tur_id, tur.tur_codigo ");
            sql.Append("FROM TUR_Turma tur ");

            if (pes_id.HasValue && ent_id.HasValue)
            {
                sql.Append("INNER JOIN (SELECT DISTINCT tud.tur_id ");
                sql.Append("FROM TUR_TurmaDisciplina tud ");
                sql.Append("INNER JOIN TUR_TurmaDocente tdt ON tdt.tud_id = tud.tud_id ");
                sql.Append("INNER JOIN ACA_Docente doc ON doc.doc_id = tdt.doc_id ");
                sql.Append("WHERE tud.tud_situacao = @state AND tdt.tdt_situacao = @state AND doc.doc_situacao = @state ");
                sql.Append("AND doc.pes_id = @pes_id AND doc.ent_id = @ent_id) tud ON tur.tur_id = tud.tur_id ");
            }
            sql.Append("WHERE tur.tur_situacao = @state AND tur.esc_id = @esc_id ");

            if (ttn_id > 0)
                sql.Append("AND tur.ttn_id = @ttn_id ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<TUR_Turma>(sql.ToString(), new { esc_id = esc_id, state = (byte)1, ttn_id = ttn_id, pes_id = pes_id, ent_id = ent_id });
            }
        }

        public int GetTotalSection(Guid ent_id, IEnumerable<string> uad_id = null)
        {
            var sql = new StringBuilder("SELECT ISNULL(COUNT(t.tur_id), 0) ");
            sql.Append("FROM ESC_Escola e ");
            sql.Append("INNER JOIN TUR_Turma t ON e.esc_id = t.esc_id ");
            sql.Append("WHERE e.esc_situacao = @state AND t.tur_situacao = @state AND e.ent_id = @ent_id ");

            #region Gestor
            if (uad_id != null)
            {
                sql.AppendFormat("AND e.uad_id = {0} ", string.Join(",", uad_id));
            }
            #endregion

            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.ExecuteScalar<int>(sql.ToString(), new { ent_id = ent_id, uad_id = uad_id, state = (byte)1 });
            }
        }

        public TUR_Turma Get(long tur_id)
        {
            var sql = new StringBuilder("SELECT tur_id, esc_id, tur_codigo, tur_descricao, cal_id, ttn_id, tur_situacao, tur_dataCriacao, tur_dataAlteracao, tur_tipo ");
            sql.Append("FROM TUR_Turma WITH (NOLOCK) ");
            sql.Append("WHERE tur_id = @tur_id ");


            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<TUR_Turma>(sql.ToString(), new { tur_id = tur_id }).FirstOrDefault();
            }
        }

        public TUR_Turma GetWithTurno(long tur_id)
        {
            var sql = new StringBuilder("SELECT tur.tur_id, tur.esc_id, tur.tur_codigo, tur.tur_descricao, tur.cal_id, tur.tur_situacao, tur.tur_dataCriacao, ");
            sql.Append("tur.tur_dataAlteracao, tur.tur_tipo, ttn.ttn_id, ttn.ttn_nome ");
            sql.Append("FROM TUR_Turma tur WITH (NOLOCK) ");
            sql.Append("INNER JOIN ACA_TipoTurno WITH (NOLOCK) ttn ON tur.ttn_id = ttn.ttn_id ");
            sql.Append("WHERE tur_id = @tur_id ");


            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var result = cn.Query<TUR_Turma, ACA_TipoTurno, TUR_Turma>(sql.ToString(),
                    (t, ttn) =>
                    {
                        t.ACA_TipoTurno = ttn;
                        return t;
                    }
                        , new { tur_id = tur_id }, splitOn: "ttn_id");

                return result.Any() ? result.FirstOrDefault() : null;
            }
        }
        public TUR_TurmaDTO GetWithTurnoAndModality(long tur_id)
        {

            string sql = @"SELECT tur.tur_id, tur.esc_id, tur.tur_codigo, tur.tur_descricao, tur.cal_id, tur.tur_situacao, 
                           tur.tur_dataCriacao, tur.tur_dataAlteracao, tur.tur_tipo, 
                           ttn.ttn_id, ttn.ttn_nome,
                           tme.tme_id, tme.tme_nome 
                           FROM TUR_Turma tur WITH (NOLOCK) 
                           INNER JOIN ACA_TipoTurno WITH (NOLOCK) ttn ON tur.ttn_id = ttn.ttn_id 
                           INNER JOIN TUR_TurmaCurriculo WITH (NOLOCK) tcr ON tur.tur_id = tcr.tur_id
                           INNER JOIN ACA_Curso WITH (NOLOCK) cur ON tcr.cur_id = cur.cur_id
                           INNER JOIN ACA_TipoModalidadeEnsino WITH (NOLOCK) tme ON cur.tme_id = tme.tme_id
                           WHERE tur.tur_id = @tur_id ";

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                var result = cn.Query<TUR_TurmaDTO, ACA_TipoTurno, ACA_TipoModalidadeEnsino, TUR_TurmaDTO>(sql.ToString(),
                    (t, ttn, tme) =>
                    {
                        t.ACA_TipoTurno = ttn;
                        t.ACA_TipoModalidadeEnsino = tme;
                        return t;
                    },
                    new { tur_id = tur_id }, splitOn: "ttn_id, tme_id");

                return result.Any() ? result.FirstOrDefault() : null;
            }
        }

        public bool ValidateTeacherSection(long tur_id, Guid pes_id)
        {
            var sql = new StringBuilder("SELECT TD.tur_id FROM [TUR_TurmaDocente] T WITH (NOLOCK) ");
            sql.Append("INNER JOIN [TUR_TurmaDisciplina] TD WITH (NOLOCK) ON TD.tud_id = T.tud_id ");
            sql.Append("INNER JOIN [ACA_Docente] D WITH (NOLOCK) ON D.doc_id = T.doc_id ");
            sql.Append("WHERE D.pes_id = @pes_id AND TD.tur_id = @tur_id ");

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                long turId = cn.Query<long>(sql.ToString(), new { tur_id = tur_id, pes_id = pes_id }).FirstOrDefault();

                return turId > 0;
            }
        }

        public IEnumerable<TUR_Turma> LoadByGrade(int esc_id, int ttn_id, IEnumerable<int> years, Guid? pes_id = null, Guid? ent_id = null)
        {
            var sql = new StringBuilder("SELECT DISTINCT tur.tur_id, tur.tur_codigo ");
            sql.Append("FROM TUR_Turma tur ");

            if (pes_id.HasValue && ent_id.HasValue)
            {
                sql.Append("INNER JOIN (SELECT DISTINCT tud.tur_id ");
                sql.Append("FROM TUR_TurmaDisciplina tud ");
                sql.Append("INNER JOIN TUR_TurmaDocente tdt ON tdt.tud_id = tud.tud_id ");
                sql.Append("INNER JOIN ACA_Docente doc ON doc.doc_id = tdt.doc_id ");
                sql.Append("WHERE tud.tud_situacao = @state AND tdt.tdt_situacao = @state AND doc.doc_situacao = @state ");
                sql.Append("AND doc.pes_id = @pes_id AND doc.ent_id = @ent_id) tud ON tur.tur_id = tud.tur_id ");
            }
            sql.Append("INNER JOIN TUR_TurmaCurriculo tc WITH (NOLOCK) ON tur.tur_id = tc.tur_id AND tc.tcr_situacao <> 3 ");
            sql.AppendFormat("INNER JOIN ACA_CurriculoPeriodo crp WITH (NOLOCK) ON tc.cur_id = Crp.cur_id AND tc.crr_id = Crp.crr_id AND tc.crp_id = Crp.crp_id AND crp.crp_situacao <> 3 AND crp.tcp_id  IN ({0}) ", string.Join(",", years));
            sql.Append("WHERE tur.tur_situacao = @state AND tur.esc_id = @esc_id ");

            if (ttn_id > 0)
                sql.Append("AND tur.ttn_id = @ttn_id ");



            using (IDbConnection cn = Connection)
            {
                cn.Open();

                return cn.Query<TUR_Turma>(sql.ToString(), new { esc_id = esc_id, state = (byte)1, ttn_id = ttn_id, pes_id = pes_id, ent_id = ent_id });
            }
        }
    }
}
