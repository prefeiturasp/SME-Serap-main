using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using EntityFile = GestaoAvaliacao.Entities.File;

namespace AvaliaMais.FolhaRespostas.Application.Interfaces
{
    public interface IExportacaoAppService
	{
		EntityFile ExportReport(IEnumerable<DRE> dres, IEnumerable<Escola> escolas, IEnumerable<Turma> turmas, 
			IEnumerable<Aluno> alunos, Quantidade totalRede, string separator, string virtualDirectory, string physicalDirectory, SYS_Usuario usuario);
		IEnumerable<DRE> ObterDres(int provaId);
		IEnumerable<DRE> ObterDresGestor(int provaId, Guid usuarioId, Guid grupoId);

		IEnumerable<Escola> ObterEscolas(int provaId, Guid DreId);
		IEnumerable<Escola> ObterEscolasDiretor(int provaId, Guid usuario, Guid grupo);

		IEnumerable<Turma> ObterTurmas(int provaId, int escolaId);
		IEnumerable<Turma> ObterTurmasProfessor(int provaId, Guid usuarioId);

		IEnumerable<Aluno> ObterAlunos(int turmaId, int provaId);

		Quantidade ObterQuantidadeDres(int provaId);
		Quantidade ObterQuantidadeDresGestor(int provaId, Guid usuarioId, Guid grupoId);
	}
}
