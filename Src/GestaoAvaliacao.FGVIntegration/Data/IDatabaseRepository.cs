using GestaoAvaliacao.FGVIntegration.Models;
using System.Collections.Generic;

namespace GestaoAvaliacao.FGVIntegration.Data
{
    public interface IDatabaseRepository
    {

        ICollection<Pessoa> BuscarPessoas(ICollection<string> pCodigosRF);

        Pessoa BuscarPessoa(string pCodigoRF);

        /// <summary>
        /// Busca as escolas de ensino médio com a informação do diretor.
        /// </summary>
        ICollection<Escola> BuscarEscolas(ICollection<string> pCodigoEscolas);

        /// <summary>
        /// Busca as turmas das escolas de ensino médio. 
        /// Se for informado o parâmetro pCodigoEscolas, serão consideradas apenas aquelas escolas de ensino médio.
        /// </summary>
        ICollection<Turma> BuscarTurmas(ICollection<Escola> pCodigoEscolas);

        /// <summary>
        /// Busca os professores das escolas de ensino médio. 
        /// Se for informado o parâmetro pCodigoEscolas, serão consideradas apenas aquelas escolas de ensino médio.
        /// </summary>
        ICollection<Professor> BuscarProfessores(ICollection<Escola> pCodigoEscolas);

        /// <summary>
        /// Busca as turmas dos professores das escolas de ensino médio. 
        /// Se for informado o parâmetro pCodigoEscolas, serão consideradas apenas aquelas escolas de ensino médio.
        /// </summary>
        ICollection<ProfessorTurma> BuscarProfessoresTurmas(ICollection<Escola> pCodigoEscolas);

        /// <summary>
        /// Busca os alunos das escolas de ensino médio. 
        /// Se for informado o parâmetro pCodigoEscolas, serão consideradas apenas aquelas escolas de ensino médio.
        /// </summary>
        ICollection<Aluno> BuscarAlunos(ICollection<Escola> pCodigoEscolas);
    }
}