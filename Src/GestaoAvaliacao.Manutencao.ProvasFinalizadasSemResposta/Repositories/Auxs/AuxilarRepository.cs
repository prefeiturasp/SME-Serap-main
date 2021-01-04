using Dapper;
using GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Repositories.Abstractions;
using GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Repositories.Auxs.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.ProvasFinalizadasSemResposta.Repositories.Auxs
{
    internal class AuxilarRepository : BaseSqlRepository, IAuxilarRepository
    {
        public async Task<IEnumerable<TestDto>> GetTestsById(IEnumerable<long> ids)
        {
            var idsConcat = string.Join(",", ids);
            var query = $@"SELECT Id, Description FROM Test (NOLOCK) WHERE Id IN ({idsConcat})";

            using (var conn = Connection)
            {
                return await conn.QueryAsync<TestDto>(query);
            }
        }

        public async Task<IEnumerable<StudentDto>> GetStudentsById(IEnumerable<long> ids)
        {
            var idsConcat = string.Join(",", ids);
            var query = $@"SELECT alu_Id AS Id, alu_matricula AS EolCode, alu_nome AS Name FROM SGP_ACA_Aluno (NOLOCK) WHERE alu_Id IN ({idsConcat})";

            using (var conn = Connection)
            {
                return await conn.QueryAsync<StudentDto>(query);
            }
        }

        public async Task<IEnumerable<SchoolBySectionDto>> GetSchoolsBySectionAsync(IEnumerable<long> ids)
        {
            var idsConcat = string.Join(",", ids);
            var query = $@"SELECT DISTINCT tur.tur_id AS SectionId, esc.esc_Id AS SchoolId, esc_nome AS Name, uad_nome AS DreName 
                        FROM 
                            SGP_TUR_Turma tur (NOLOCK) 
                        INNER JOIN
                            SGP_ESC_Escola esc (NOLOCK)
                            ON tur.esc_id = esc.esc_id
                        INNER JOIN
                            SGP_SYS_UnidadeAdministrativa uad (NOLOCK)
                            ON esc.uad_idSuperiorGestao = uad.uad_id
                        WHERE tur.tur_id IN ({idsConcat})";

            using (var conn = Connection)
            {
                return await conn.QueryAsync<SchoolBySectionDto>(query);
            }
        }
    }
}