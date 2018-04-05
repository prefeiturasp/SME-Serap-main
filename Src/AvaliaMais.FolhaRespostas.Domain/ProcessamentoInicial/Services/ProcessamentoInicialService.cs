using AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial.Interfaces;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using System;
using System.Collections.Generic;

namespace AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial.Services
{
    public class ProcessamentoInicialService : IProcessamentoInicialService
	{
		private readonly IProcessamentoInicialRepository _processametoInicialRepository;

		public ProcessamentoInicialService(IProcessamentoInicialRepository processametoInicialRepository)
		{
			_processametoInicialRepository = processametoInicialRepository;
		}

		public IEnumerable<Processamento> ObterProcessamentoProva(int provaId)
		{
			return _processametoInicialRepository.ObterProcessamentoProva(provaId);
		}

		public IEnumerable<Processamento> ObterProcessamentoProvaAdesaoTotal(int provaId)
		{
			return _processametoInicialRepository.ObterProcessamentoProvaAdesaoTotal(provaId);
		}

		public IEnumerable<Aluno> ObterAlunosPorTurmaProva(string TurmaIds, int ProvaId)
		{
			return _processametoInicialRepository.ObterAlunosPorTurmaProva(TurmaIds, ProvaId);
		}

		public IEnumerable<int> ObterTurmasDoUsuario(Guid userId)
		{
			return _processametoInicialRepository.ObterTurmasDoUsuario(userId);
		}
	}
}
