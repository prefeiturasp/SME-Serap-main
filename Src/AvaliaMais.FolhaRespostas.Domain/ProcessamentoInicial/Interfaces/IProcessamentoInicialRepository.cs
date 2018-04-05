using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using System;
using System.Collections.Generic;


namespace AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial.Interfaces
{
    public interface IProcessamentoInicialRepository
	{
		IEnumerable<Processamento> ObterProcessamentoProva(int provaId);
        IEnumerable<Processamento> ObterProcessamentoProvaAdesaoTotal(int provaId);

        IEnumerable<Aluno> ObterAlunosPorTurmaProva(string TurmaIds, int ProvaId);
		IEnumerable<int> ObterTurmasDoUsuario(Guid userId);
	}
}
