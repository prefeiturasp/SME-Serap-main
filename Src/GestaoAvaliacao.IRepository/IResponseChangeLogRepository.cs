using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoAvaliacao.IRepository
{
    public interface IResponseChangeLogRepository
    {
        List<StudentDTO> GetInfoStudents(List<long> alunos);
        List<DresDTO> GetInfoDresSchools(IEnumerable<string> dres, Guid ent_id);
        List<UsersDTO> GetInfoUsers(IEnumerable<string> usuarios, Guid ent_id);
        List<TeamsDTO> GetInfoTeams(List<long> turmas);
        List<BlockItem> GetBlockItens(List<long> itens, long Test_id);
    }
}
