using GestaoAvaliacao.FGVIntegration.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.FGVIntegration.FGVEnsinoMedio
{
    public interface IDataRepository
    {
        ICollection<Escola> BuscarEscolas();
        ICollection<Coordenador> BuscarCoordenadores();
        ICollection<Turma> BuscarTurmas();
        ICollection<Professor> BuscarProfessores();
        ICollection<ProfessorTurma> BuscarProfessoresTurmas();
        ICollection<Aluno> BuscarAlunos();
    }
}