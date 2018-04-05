using AvaliaMais.FolhaRespostas.Data.MongoDB.Context;
using AvaliaMais.FolhaRespostas.Data.MongoDB.Helpers;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva;
using AvaliaMais.FolhaRespostas.Domain.ProcessamentoProva.Interfaces;
using GestaoAvaliacao.Util;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaliaMais.FolhaRespostas.Data.MongoDB.Repository
{
    public class ProcessamentoProvaRepository : IProcessamentoProvaRepository
	{
		private readonly MongoDbContext _db;
		private int _provaId;

		public ProcessamentoProvaRepository(MongoDbContext db)
		{
			_db = db;
		}

		public void AdicionarProcessamentoProva(ProcessamentoProva prova)
		{
			var dres = prova.dres;
			AdicionarDres(dres);

			var escolas = dres.SelectMany(x => x.Escolas).ToList();
			AdicionarEscolas(escolas);

			var turmas = escolas.SelectMany(x => x.Turmas).ToList();
			AdicionarTurmas(turmas);

			var alunos = turmas.SelectMany(x => x.Alunos).ToList();
			AdicionarAlunos(alunos);
		}


        private void AdicionarDres(IEnumerable<DRE> dres)
		{
			if (dres.Count() > 0)
			{
				_db.ProcessamentoDRE.InsertMany(dres);
			}
		}

        private void AdicionarEscolas(IEnumerable<Escola> escolas)
		{
			if (escolas.Count() > 0)
			{
				_db.ProcessamentoEscola.InsertMany(escolas);
			}
		}

        private void AdicionarTurmas(IEnumerable<Turma> turmas)
		{
			if (turmas.Count() > 0)
			{
				_db.ProcessamentoTurma.InsertMany(turmas);
			}
		}

        private void AdicionarAlunos(IEnumerable<Aluno> alunos)
		{
			if (alunos.Count() > 0)
			{
				_db.ProcessamentoAluno.InsertMany(alunos);
			}
		}

		public bool DeletarInativos()
		{
            bool retorno = _db.ProcessamentoDRE.DeleteMany(x => !x.Ativo).IsAcknowledged;
            retorno &= _db.ProcessamentoEscola.DeleteMany(x => !x.Ativo).IsAcknowledged;
			retorno &= _db.ProcessamentoTurma.DeleteMany(x => !x.Ativo).IsAcknowledged;
			retorno &= _db.ProcessamentoAluno.DeleteMany(x => !x.Ativo).IsAcknowledged;

			return retorno;
		}

		public bool InativarDocumentos(int provaId)
		{
			_provaId = provaId;
			var resultado = InativarDocumentoDre();
			resultado &= InativarDocumentoEscola();
			resultado &= InativarDocumentoTurma();
			resultado &= InativarDocumentoAluno();

			return resultado;
		}

		private bool InativarDocumentoDre()
		{
			var builder = Builders<DRE>.Filter;
			var filter = builder.Eq(x => x.ProvaId, _provaId);
			var update = Builders<DRE>.Update.Set(x => x.Ativo, false);
			var result = _db.ProcessamentoDRE.UpdateMany(filter, update);

			return result.IsAcknowledged;
		}

		private bool InativarDocumentoEscola()
		{
			var builder = Builders<Escola>.Filter;
			var filter = builder.Eq(x => x.ProvaId, _provaId);
			var update = Builders<Escola>.Update.Set(x => x.Ativo, false);
			var result = _db.ProcessamentoEscola.UpdateMany(filter, update);

			return result.IsAcknowledged;
		}

		private bool InativarDocumentoTurma()
		{
			var builder = Builders<Turma>.Filter;
			var filter = builder.Eq(x => x.ProvaId, _provaId);
			var update = Builders<Turma>.Update.Set(x => x.Ativo, false);
			var result = _db.ProcessamentoTurma.UpdateMany(filter, update);

			return result.IsAcknowledged;
		}

		private bool InativarDocumentoAluno()
		{
			var builder = Builders<Aluno>.Filter;
			var filter = builder.Eq(x => x.ProvaId, _provaId);
			var update = Builders<Aluno>.Update.Set(x => x.Ativo, false);
			var result = _db.ProcessamentoAluno.UpdateMany(filter, update);

			return result.IsAcknowledged;
		}

		public IEnumerable<DRE> ObterDres(ref Pager pagina, int provaId)
		{
			pagina.SetTotalItens((int)_db.ProcessamentoDRE.Count(x => x.ProvaId == provaId));
			pagina.SetTotalPages(pagina.RecordsCount / pagina.PageSize);

			var resultado = _db.ProcessamentoDRE.Find(x => x.ProvaId == provaId && x.Ativo)
				.Skip(pagina.CurrentPage * pagina.PageSize)
				.Limit(pagina.PageSize)
				.SortBy(x => x.Nome)
				.ToList();

			resultado = resultado ?? new List<DRE>();
			return resultado;
		}

        public IEnumerable<DRE> ObterDresSemPaginacao(int provaId)
        {
            var resultado = _db.ProcessamentoDRE.Find(x => x.ProvaId == provaId && x.Ativo)              
                .SortBy(x => x.Nome)
                .ToList();

            resultado = resultado ?? new List<DRE>();
            return resultado;
        }

        public IEnumerable<DRE> ObterDresGestor(ref Pager pagina, int provaId, Guid[] dresIds)
		{
			var builder = Builders<DRE>.Filter;
			var filter = builder.And(builder.In(x => x.DreId, dresIds),
				builder.Eq(x => x.ProvaId, provaId),
				builder.Eq(x => x.Ativo, true));

			pagina.SetTotalItens((int)_db.ProcessamentoDRE.Count(filter));
			pagina.SetTotalPages(pagina.RecordsCount / pagina.PageSize);

			var resultado = _db.ProcessamentoDRE.Find(filter)
								.Skip(pagina.CurrentPage * pagina.PageSize)
								.Limit(pagina.PageSize)
								.SortBy(x => x.Nome)
								.ToList();
			resultado = resultado ?? new List<DRE>();
			return resultado;
		}

        public IEnumerable<DRE> ObterDresGestorSemPaginacao(int provaId, Guid[] dresIds)
        {
            var builder = Builders<DRE>.Filter;
            var filter = builder.And(builder.In(x => x.DreId, dresIds),
                builder.Eq(x => x.ProvaId, provaId),
                builder.Eq(x => x.Ativo, true));

            var resultado = _db.ProcessamentoDRE.Find(filter)                              
                                .SortBy(x => x.Nome)
                                .ToList();
            resultado = resultado ?? new List<DRE>();
            return resultado;
        }

        public IEnumerable<Escola> ObterEscolasDiretor(ref Pager pagina, int provaId, Guid[] uadsIds)
		{
			var builder = Builders<Escola>.Filter;
			var filter = builder.And(builder.In(x => x.EscolaUad, uadsIds),
							builder.Eq(x => x.ProvaId, provaId),
							builder.Eq(x => x.Ativo, true));

			pagina.SetTotalItens((int)_db.ProcessamentoEscola.Count(filter));
			pagina.SetTotalPages(pagina.RecordsCount / pagina.PageSize);

			var resultado = _db.ProcessamentoEscola.Find(filter)
								.Skip(pagina.CurrentPage * pagina.PageSize)
								.Limit(pagina.PageSize)
								.SortBy(x => x.Nome)
								.ToList();

			resultado = resultado ?? new List<Escola>();
			return resultado;
		}

		public IEnumerable<Escola> ObterEscolas(ref Pager pagina, int provaId, Guid DreId)
		{
			pagina.SetTotalItens((int)_db.ProcessamentoEscola.Count(x => x.DreId == DreId && x.ProvaId == provaId));
			pagina.SetTotalPages(pagina.RecordsCount / pagina.PageSize);

			var resultado = _db.ProcessamentoEscola.Find(x => x.DreId == DreId && x.ProvaId == provaId && x.Ativo)
								.Skip(pagina.CurrentPage * pagina.PageSize)
								.Limit(pagina.PageSize)
								.SortBy(x => x.Nome)
								.ToList();

			resultado = resultado ?? new List<Escola>();
			return resultado;
		}

        public IEnumerable<Escola> ObterEscolasSemPaginacao(int provaId, Guid DreId)
        {
            var resultado = _db.ProcessamentoEscola.Find(x => x.DreId == DreId && x.ProvaId == provaId && x.Ativo)                               
                                .SortBy(x => x.Nome)
                                .ToList();

            resultado = resultado ?? new List<Escola>();
            return resultado;
        }

        public IEnumerable<Escola> ObterEscolasDiretorSemPaginacao(int provaId, Guid[] uadsIds)
        {
            var builder = Builders<Escola>.Filter;
            var filter = builder.And(builder.In(x => x.EscolaUad, uadsIds),
                            builder.Eq(x => x.ProvaId, provaId),
                            builder.Eq(x => x.Ativo, true));

            var resultado = _db.ProcessamentoEscola.Find(filter)                               
                                .SortBy(x => x.Nome)
                                .ToList();

            resultado = resultado ?? new List<Escola>();
            return resultado;
        }

        public IEnumerable<Turma> ObterTurmas(ref Pager pagina, int provaId, int escolaId)
		{
			pagina.SetTotalItens((int)_db.ProcessamentoTurma.Count(x => x.EscolaId == escolaId && x.ProvaId == provaId));
			pagina.SetTotalPages(pagina.RecordsCount / pagina.PageSize);

			var resultado = _db.ProcessamentoTurma.Find(x => x.EscolaId == escolaId && x.ProvaId == provaId && x.Ativo)
								.Skip(pagina.CurrentPage * pagina.PageSize)
								.Limit(pagina.PageSize)
								.SortBy(x => x.Nome)
								.ToList();

			resultado = resultado ?? new List<Turma>();
			return resultado;
		}

		public IEnumerable<Turma> ObterTurmasProfessor(ref Pager pagina, int provaId, int[] turmas)
		{
			var builder = Builders<Turma>.Filter;
			var filter = builder.And(builder.In(x => x.TurmaId, turmas),
							builder.Eq(x => x.ProvaId, provaId),
							builder.Eq(x => x.Ativo, true));

			pagina.SetTotalItens((int)_db.ProcessamentoTurma.Count(filter));
			pagina.SetTotalPages(pagina.RecordsCount / pagina.PageSize);

			var resultado = _db.ProcessamentoTurma.Find(filter)
								.Skip(pagina.CurrentPage * pagina.PageSize)
								.Limit(pagina.PageSize)
								.SortBy(x => x.Nome)
								.ToList();

			resultado = resultado ?? new List<Turma>();
			return resultado;
		}

		public IEnumerable<Aluno> ObterAlunos(ref Pager pagina, int turmaId, int provaId)
		{
			pagina.SetTotalItens((int)_db.ProcessamentoAluno.Count(x => x.TurmaId == turmaId && x.ProvaId == provaId));
			pagina.SetTotalPages(pagina.RecordsCount / pagina.PageSize);

			var resultado = _db.ProcessamentoAluno.Find(x => x.TurmaId == turmaId && x.Ativo && x.ProvaId == provaId)
								.Skip(pagina.CurrentPage * pagina.PageSize)
								.Limit(pagina.PageSize)
								.SortBy(x => x.Numero)
								.ToList();
			resultado = resultado ?? new List<Aluno>();
			return resultado;
		}

		public Quantidade QuantidadeDre(int provaId)
		{
			var resultado = _db.ProcessamentoDRE.Find(x => x.ProvaId == provaId && x.Ativo).ToList();
			return QuantidadeHelper<DRE>.QuantidadeTotal(resultado);
		}

		public Quantidade QuantidadeDreGestor(int provaId, Guid[] dresIds)
		{
			var builder = Builders<DRE>.Filter;
			var filter = builder.And(builder.In(x => x.DreId, dresIds),
				builder.Eq(x => x.ProvaId, provaId),
				builder.Eq(x => x.Ativo, true));
			var resultado = _db.ProcessamentoDRE.Find(filter).ToList();
			return QuantidadeHelper<DRE>.QuantidadeTotal(resultado);
		}

		public Quantidade QuantidadeEscola(int provaId, Guid DreId)
		{
			var resultado = _db.ProcessamentoEscola.Find(x => x.DreId == DreId && x.ProvaId == provaId && x.Ativo).ToList();
			var quantidade = new Quantidade();
			quantidade.Aderidos = resultado.Sum(a => a.AlunoStatus.Aderidos);
			quantidade.Identificados = resultado.Sum(a => a.AlunoStatus.Identificados);
			quantidade.Sucesso = resultado.Sum(a => a.ProcessamentoStatus.Sucesso);
			quantidade.Conferir = resultado.Sum(a => a.ProcessamentoStatus.Conferir);
			quantidade.Ausente = resultado.Sum(a => a.ProcessamentoStatus.Ausente);
			quantidade.Erro = resultado.Sum(a => a.ProcessamentoStatus.Erro);
			quantidade.Pendente = resultado.Sum(a => a.ProcessamentoStatus.Pendente);

			return quantidade;
		}

		public Quantidade QuantidadeEscolaDiretor(int provaId, Guid[] uadsIds)
		{
			var builder = Builders<Escola>.Filter;
			var filter = builder.And(builder.In(x => x.EscolaUad, uadsIds),
							builder.Eq(x => x.ProvaId, provaId),
							builder.Eq(x => x.Ativo, true));
			var resultado = _db.ProcessamentoEscola.Find(filter).ToList();

			var quantidade = new Quantidade();
			quantidade.Aderidos = resultado.Sum(a => a.AlunoStatus.Aderidos);
			quantidade.Identificados = resultado.Sum(a => a.AlunoStatus.Identificados);
			quantidade.Sucesso = resultado.Sum(a => a.ProcessamentoStatus.Sucesso);
			quantidade.Conferir = resultado.Sum(a => a.ProcessamentoStatus.Conferir);
			quantidade.Ausente = resultado.Sum(a => a.ProcessamentoStatus.Ausente);
			quantidade.Erro = resultado.Sum(a => a.ProcessamentoStatus.Erro);
			quantidade.Pendente = resultado.Sum(a => a.ProcessamentoStatus.Pendente);

			return quantidade;
		}

		public Quantidade QuantidadeTurma(int provaId, int escolaId)
		{
			var resultado = _db.ProcessamentoTurma.Find(x => x.EscolaId == escolaId && x.ProvaId == provaId && x.Ativo).ToList();
			var quantidade = new Quantidade();
			quantidade.Aderidos = resultado.Sum(a => a.AlunoStatus.Aderidos);
			quantidade.Identificados = resultado.Sum(a => a.AlunoStatus.Identificados);
			quantidade.Sucesso = resultado.Sum(a => a.ProcessamentoStatus.Sucesso);
			quantidade.Conferir = resultado.Sum(a => a.ProcessamentoStatus.Conferir);
			quantidade.Ausente = resultado.Sum(a => a.ProcessamentoStatus.Ausente);
			quantidade.Erro = resultado.Sum(a => a.ProcessamentoStatus.Erro);
			quantidade.Pendente = resultado.Sum(a => a.ProcessamentoStatus.Pendente);

			return quantidade;
		}

		public Quantidade QuantidadeTurmaProfessor(int provaId, int[] turmas)
		{
			var builder = Builders<Turma>.Filter;
			var filter = builder.And(builder.In(x => x.TurmaId, turmas),
							builder.Eq(x => x.ProvaId, provaId),
							builder.Eq(x => x.Ativo, true));
			var resultado = _db.ProcessamentoTurma.Find(filter).ToList();
			var quantidade = new Quantidade();
			quantidade.Aderidos = resultado.Sum(a => a.AlunoStatus.Aderidos);
			quantidade.Identificados = resultado.Sum(a => a.AlunoStatus.Identificados);
			quantidade.Sucesso = resultado.Sum(a => a.ProcessamentoStatus.Sucesso);
			quantidade.Conferir = resultado.Sum(a => a.ProcessamentoStatus.Conferir);
			quantidade.Ausente = resultado.Sum(a => a.ProcessamentoStatus.Ausente);
			quantidade.Erro = resultado.Sum(a => a.ProcessamentoStatus.Erro);
			quantidade.Pendente = resultado.Sum(a => a.ProcessamentoStatus.Pendente);

			return quantidade;
		}
	
		public IEnumerable<DRE> ObterDres(int provaId)
		{
			var resultado = _db.ProcessamentoDRE.Find(x => x.ProvaId == provaId && x.Ativo)
				.SortBy(x => x.Nome)
				.ToList();

			resultado = resultado ?? new List<DRE>();
			return resultado;
		}

		public IEnumerable<DRE> ObterDresGestor(int provaId, Guid[] dresIds)
		{
			var builder = Builders<DRE>.Filter;
			var filter = builder.And(builder.In(x => x.DreId, dresIds),
				builder.Eq(x => x.ProvaId, provaId),
				builder.Eq(x => x.Ativo, true));

			var resultado = _db.ProcessamentoDRE.Find(filter)
								.SortBy(x => x.Nome)
								.ToList();
			resultado = resultado ?? new List<DRE>();
			return resultado;
		}

		public IEnumerable<Escola> ObterEscolas(int provaId, Guid DreId)
		{
			var resultado = _db.ProcessamentoEscola.Find(x => x.DreId == DreId && x.ProvaId == provaId && x.Ativo)
								.SortBy(x => x.Nome)
								.ToList();

			resultado = resultado ?? new List<Escola>();
			return resultado;
		}

		public IEnumerable<Escola> ObterEscolaDiretor(int provaId, Guid[] uadsIds)
		{
			var builder = Builders<Escola>.Filter;
			var filter = builder.And(builder.In(x => x.EscolaUad, uadsIds),
							builder.Eq(x => x.ProvaId, provaId),
							builder.Eq(x => x.Ativo, true));

			var resultado = _db.ProcessamentoEscola.Find(filter)
								.SortBy(x => x.Nome)
								.ToList();

			resultado = resultado ?? new List<Escola>();
			return resultado;
		}

		public IEnumerable<Turma> ObterTurmas(int provaId, int escolaId)
		{
			var resultado = _db.ProcessamentoTurma.Find(x => x.EscolaId == escolaId && x.ProvaId == provaId && x.Ativo)
								.SortBy(x => x.Nome)
								.ToList();

			resultado = resultado ?? new List<Turma>();
			return resultado;
		}

		public IEnumerable<Turma> ObterTurmasProfessor(int provaId, int[] turmas)
		{
			var builder = Builders<Turma>.Filter;
			var filter = builder.And(builder.In(x => x.TurmaId, turmas),
							builder.Eq(x => x.ProvaId, provaId),
							builder.Eq(x => x.Ativo, true));

			var resultado = _db.ProcessamentoTurma.Find(filter)
								.SortBy(x => x.Nome)
								.ToList();

			resultado = resultado ?? new List<Turma>();
			return resultado;
		}

		public IEnumerable<Aluno> ObterAlunos(int turmaId, int provaId)
		{
			var resultado = _db.ProcessamentoAluno.Find(x => x.TurmaId == turmaId && x.Ativo && x.ProvaId == provaId)

								.SortBy(x => x.Nome)
								.ToList();
			resultado = resultado ?? new List<Aluno>();
			return resultado;
		}
	}
}
