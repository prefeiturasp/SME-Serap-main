using AvaliaMais.FolhaRespostas.Application.ViewModels;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;

namespace AvaliaMais.FolhaRespostas.Application.Interfaces
{
    public interface IProcessamentoAppService
	{
		DREViewModel ObterDres(ref Pager pagina, int provaId);        
        DREViewModel ObterDresGestor(ref Pager pagina, int provaId, SYS_Usuario usuario, SYS_Grupo grupo);      
        EscolaViewModel ObterEscolas(ref Pager pagina, int provaId, Guid DreId);        
        EscolaViewModel ObterEscolasDiretor(ref Pager pagina, int provaId, SYS_Usuario usuario,SYS_Grupo grupo); 
        TurmaViewModel ObterTurmas(ref Pager pagina, int provaId, int escolaId);
		TurmaViewModel ObterTurmasProfessor(ref Pager pagina, int provaId, Guid usuarioId);
		IEnumerable<Aluno> ObterAlunos(ref Pager pagina, int turmaId, int provaId);
	}
}
