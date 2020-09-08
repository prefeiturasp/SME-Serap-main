using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Domain.Matriculas
{
    internal interface IMatriculaRepository
    {
        Task<IEnumerable<Matricula>> GetMatriculasAtivasDoAluno(long alunoId, int ano);
        Task DeleteMatricula(Matricula matricula);
        Task UpdateNumeroDeChamadaAsync(Matricula matricula, int numeroDeChamada);
    }
}