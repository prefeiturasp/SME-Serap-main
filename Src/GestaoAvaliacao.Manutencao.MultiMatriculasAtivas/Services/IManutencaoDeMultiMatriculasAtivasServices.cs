using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Domain.Alunos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Services
{
    internal interface IManutencaoDeMultiMatriculasAtivasServices
    {
        Task AjustarProvasEDesabilitarMatriculasIncorretas(int ano, IEnumerable<Aluno> alunos, Action barraDeProgressoReport);
    }
}