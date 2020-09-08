using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Domain.Alunos
{
    internal interface IAlunoRepository
    {
        Task<IEnumerable<Aluno>> GetAlunosComMaisDeUmaMatriculaAtiva(int ano);
    }
}