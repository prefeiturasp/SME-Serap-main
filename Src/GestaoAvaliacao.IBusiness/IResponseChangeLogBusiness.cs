using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.MongoEntities;
using GestaoAvaliacao.MongoEntities.DTO;
using GestaoAvaliacao.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IBusiness
{
    public interface IResponseChangeLogBusiness
    {
        Task<ResponseChangeLog> Save(Answer answer, long alu_id, long test_id, long tur_id, Guid ent_id, Guid usuId, bool manual, IEnumerable<StudentCorrectionAnswerGrid> studentsAnswers, long AbsenceReason_IdAnterior, long AbsenceReason_IdAtual);
        Task SaveAsync(IEnumerable<Answer> answers, long alu_id, long test_id, long tur_id, Guid ent_id, Guid usuId, bool manual, IEnumerable<StudentCorrectionAnswerGrid> studentsAnswers, long AbsenceReason_IdAnterior, long AbsenceReason_IdAtual);
        List<ResponseChangeLogDTO> GetResponseChangeLog(long test_id, Guid ent_id, Guid? uad_id, long? esc_id, long? tur_id, DateTime? DateStartChange, DateTime? DateEndChange, ref Pager pager);
        List<StudentDTO> GetInfoStudents(List<long> alunos);
        List<DresDTO> GetInfoDresSchools(List<string> dres, Guid ent_id);
        List<UsersDTO> GetInfoUsers(List<string> usuarios, Guid ent_id);
        List<TeamsDTO> GetInfoTeams(List<long> turmas);
        List<BlockItem> GetBlockItens(List<long> itens, long test_id);
    }
}
