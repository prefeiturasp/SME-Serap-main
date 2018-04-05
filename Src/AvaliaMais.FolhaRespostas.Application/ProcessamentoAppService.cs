using AvaliaMais.FolhaRespostas.Application.Interfaces;
using AvaliaMais.FolhaRespostas.Application.ViewModels;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoInicial.Interfaces;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva.Interfaces;
using GestaoAvaliacao.Util;
using MSTech.CoreSSO.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AvaliaMais.FolhaRespostas.Application
{
    public class ProcessamentoAppService : IProcessamentoAppService
    {
        private readonly IProcessamentoProvaService _processamentoProvaService;
        private readonly IProcessamentoInicialService _processamentoInicialService;
        
        public ProcessamentoAppService(IProcessamentoProvaService processamentoProvaService,
             IProcessamentoInicialService processamentoInicialService)
        {
            _processamentoProvaService = processamentoProvaService;
            _processamentoInicialService = processamentoInicialService;
        }

        public DREViewModel ObterDres(ref Pager pagina, int provaId)
        {
            var dres = _processamentoProvaService.ObterDres(ref pagina, provaId);
            var quantidade = _processamentoProvaService.QuantidadeDre(provaId);

            dres = dres.Select(x => new
            DRE
            {
                _id = x._id,
                DreId = x.DreId,
                Nome = x.Nome,
                AlunoStatus = x.AlunoStatus,
                ProcessamentoStatus = x.ProcessamentoStatus,
                ProvaId = x.ProvaId
            }).ToList();

            return new DREViewModel { Success = true, lista = dres, QuantidadeTotal = quantidade };
        }
        
        public DREViewModel ObterDresGestor(ref Pager pagina, int provaId, SYS_Usuario usuario, SYS_Grupo grupo)
        {
            var unidadesAdm = ListarUnidadesAdmUsuarioGrupo(usuario, grupo);
            if (unidadesAdm.Count() > 0)
            {
                var dres = _processamentoProvaService.ObterDresGestor(ref pagina, provaId, unidadesAdm);
                var quantidade = _processamentoProvaService.QuantidadeDreGestor(provaId, unidadesAdm);
                dres = dres.Select(x => new
                DRE
                {
                    _id = x._id,
                    DreId = x.DreId,
                    Nome = x.Nome,
                    AlunoStatus = x.AlunoStatus,
                    ProcessamentoStatus = x.ProcessamentoStatus,
                    ProvaId = x.ProvaId
                }).ToList();

                return new DREViewModel { Success = true, lista = dres, QuantidadeTotal = quantidade };
            }
            return new DREViewModel();
        }
       
        public EscolaViewModel ObterEscolas(ref Pager pagina, int provaId, Guid DreId)
        {
            var escolas = _processamentoProvaService.ObterEscola(ref pagina, provaId, DreId);
            var quantidade = _processamentoProvaService.QuantidadeEscola(provaId, DreId);

            escolas = escolas.Select(x => new
            Escola
            {
                _id = x._id,
                EscolaId = x.EscolaId,
                EscolaUad = x.EscolaUad,
                Nome = x.Nome,
                AlunoStatus = x.AlunoStatus,
                ProcessamentoStatus = x.ProcessamentoStatus,
                DreId = x.DreId
            }).ToList();

            return new EscolaViewModel { Success = true, lista = escolas, QuantidadeTotal = quantidade };
        }



        public EscolaViewModel ObterEscolasDiretor(ref Pager pagina, int provaId, SYS_Usuario usuario, SYS_Grupo grupo)
        {
            var unidadesAdm = ListarUnidadesAdmUsuarioGrupo(usuario, grupo);

            if (unidadesAdm.Count() > 0)
            {
                var escolas = _processamentoProvaService.ObterEscolaDiretor(ref pagina, provaId, unidadesAdm);
                var quantidade = _processamentoProvaService.QuantidadeEscolaDiretor(provaId, unidadesAdm);
                escolas = escolas.Select(x => new
                Escola
                {
                    _id = x._id,
                    EscolaId = x.EscolaId,
                    Nome = x.Nome,
                    AlunoStatus = x.AlunoStatus,
                    ProcessamentoStatus = x.ProcessamentoStatus,
                    DreId = x.DreId
                }).ToList();
                return new EscolaViewModel { Success = true, lista = escolas, QuantidadeTotal = quantidade };

            }
            return new EscolaViewModel();
        }        

        public TurmaViewModel ObterTurmas(ref Pager pagina, int provaId, int escolaId)
        {
            var turmas = _processamentoProvaService.ObterTurmas(ref pagina, provaId, escolaId);
            var quantidade = _processamentoProvaService.QuantidadeTurma(provaId, escolaId);
            turmas = turmas.Select(x => new
            Turma
            {
                _id = x._id,
                TurmaId = x.TurmaId,
                Nome = x.Nome,
                AlunoStatus = x.AlunoStatus,
                ProcessamentoStatus = x.ProcessamentoStatus,
                EscolaId = x.EscolaId
            }).ToList();

            return new TurmaViewModel { Success = true, lista = turmas, QuantidadeTotal = quantidade };
        }

        public TurmaViewModel ObterTurmasProfessor(ref Pager pagina, int provaId, Guid usuarioId)
        {
            var turmasProfessor = ListarTurmasProfessor(usuarioId);
            if (turmasProfessor.Count() > 0)
            {
                var turmas = _processamentoProvaService.ObterTurmasProfessor(ref pagina, provaId, turmasProfessor);
                var quantidade = _processamentoProvaService.QuantidadeTurmaProfessor(provaId, turmasProfessor);

                var turmasFiltradas = turmas.Select(x => new
                Turma
                {
                    _id = x._id,
                    TurmaId = x.TurmaId,
                    Nome = x.Nome,
                    AlunoStatus = x.AlunoStatus,
                    ProcessamentoStatus = x.ProcessamentoStatus,
                    EscolaId = x.EscolaId
                });

                return new TurmaViewModel { Success = true, lista = turmasFiltradas, QuantidadeTotal = quantidade };
            }
            return new TurmaViewModel();
        }

        public IEnumerable<Aluno> ObterAlunos(ref Pager pagina, int turmaId, int provaId)
        {
            return _processamentoProvaService.ObterAlunos(ref pagina, turmaId, provaId);
        }

        // Bounded Context - Core
        private Guid[] ListarUnidadesAdmUsuarioGrupo(SYS_Usuario usuario, SYS_Grupo grupo)
        {
            var dt = MSTech.CoreSSO.BLL.SYS_UsuarioGrupoUABO.GetSelect(usuario.usu_id, grupo.gru_id);
            return dt.AsEnumerable().Select(x => x.Field<Guid>("uad_id")).Cast<Guid>().ToArray();
        }

        private int[] ListarTurmasProfessor(Guid usuarioId)
        {
            return _processamentoInicialService.ObterTurmasDoUsuario(usuarioId).ToArray();
        }
    }
}
