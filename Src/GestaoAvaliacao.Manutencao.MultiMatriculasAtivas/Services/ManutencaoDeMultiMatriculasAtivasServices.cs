using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.Matriculas;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.StudentCorrections;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Data.TestsSectionStatusCorrectionBusiness;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Domain.Alunos;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Domain.Matriculas;
using GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Dtos;
using GestaoAvaliacao.MongoEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Manutencao.MultiMatriculasAtivas.Services
{
    internal class ManutencaoDeMultiMatriculasAtivasServices : IManutencaoDeMultiMatriculasAtivasServices
    {
        private readonly IMatriculaRepository _matriculaRepository;
        private readonly IAjusteStudentCorrectionRepository _ajusteStudentCorrectionRepository;
        private readonly IAjusteTestSectionStatusCorrectionRepository _ajusteTestSectionStatusCorrectionRepository;
        private readonly IAjusteCorrectionResultsRepository _ajusteCorrectionResultsRepository;
        private readonly DateTime _dataDeReferenciaDaMatriculaCorreta;
        private readonly ICollection<long> _alunosQueNaoPossuemMatriculasAtivasNaDataDeReferencia;
        private readonly ICollection<Tuple<Aluno, string>> _excecoes;
        private readonly HashSet<ProvaPorTurmaParaAjusteDaCorrecaoDto> _provasPorTurmaParaAjusteDaCorrecao;

        public ManutencaoDeMultiMatriculasAtivasServices(DateTime dataDeReferenciaDaMatriculaCorreta)
        {
            _alunosQueNaoPossuemMatriculasAtivasNaDataDeReferencia = new List<long>();
            _excecoes = new List<Tuple<Aluno, string>>();
            _dataDeReferenciaDaMatriculaCorreta = dataDeReferenciaDaMatriculaCorreta;
            _matriculaRepository = new MatriculaRepository();
            _ajusteStudentCorrectionRepository = new AjusteStudentCorrectionRepository();
            _ajusteTestSectionStatusCorrectionRepository = new AjusteTestSectionStatusCorrectionRepository();
            _ajusteCorrectionResultsRepository = new AjusteCorrectionResultsRepository();
            _provasPorTurmaParaAjusteDaCorrecao = new HashSet<ProvaPorTurmaParaAjusteDaCorrecaoDto>();
        }

        public async Task AjustarProvasEDesabilitarMatriculasIncorretas(int ano, IEnumerable<Aluno> alunos, Action barraDeProgressoReport)
        {
            if (!alunos?.Any() ?? true) return;

            foreach (var aluno in alunos)
            {
                await AjustarProvasEDesabilitarMatriculasIncorretasDoAluno(ano, aluno, barraDeProgressoReport);
            }

            await AjustarStatusDeCorrecaoDasProvasParaAsTurmas();
            await RemoverCorrectionResults();
        }

        private async Task RemoverCorrectionResults()
        {
            if (!_provasPorTurmaParaAjusteDaCorrecao.Any()) return;
            foreach (var provaPorTurma in _provasPorTurmaParaAjusteDaCorrecao)
                await _ajusteCorrectionResultsRepository.DeleteCorrectionResults(provaPorTurma.TestId, provaPorTurma.TurmaId, provaPorTurma.EntidadeId);
        }

        private async Task AjustarProvasEDesabilitarMatriculasIncorretasDoAluno(int ano, Aluno aluno, Action barraDeProgressoReport)
        {
            try
            {
                var matriculasAtivas = await _matriculaRepository.GetMatriculasAtivasDoAluno(aluno.AlunoId, ano);
                if (!matriculasAtivas?.Any() ?? true) return;

                if (!matriculasAtivas.Any(x => x.DataDeAlteracao.Date == _dataDeReferenciaDaMatriculaCorreta.Date))
                {
                    _alunosQueNaoPossuemMatriculasAtivasNaDataDeReferencia.Add(aluno.AlunoId);
                    return;
                }

                var matriculaCriadaNaIntegracaoDaDataDeReferencia = matriculasAtivas.First(x => x.DataDeAlteracao.Date == _dataDeReferenciaDaMatriculaCorreta.Date);
                var matriculasQueSeraoAjustadas = matriculasAtivas.Where(x => x.DataDeAlteracao.Date != _dataDeReferenciaDaMatriculaCorreta.Date);

                foreach (var matriculaQueSeraAjustada in matriculasQueSeraoAjustadas)
                {
                    await AjustarProvasRealizadasNoPeiodoPeloAluno(aluno, matriculaQueSeraAjustada, matriculaCriadaNaIntegracaoDaDataDeReferencia);
                    await _matriculaRepository.DeleteMatricula(matriculaQueSeraAjustada);
                }

                await _matriculaRepository.UpdateNumeroDeChamadaAsync(matriculaCriadaNaIntegracaoDaDataDeReferencia, matriculasQueSeraoAjustadas.First().NumeroDeChamada);
            }
            catch (Exception ex)
            {
                _excecoes.Add(new Tuple<Aluno, string>(aluno, ex.InnerException?.Message ?? ex.Message));
            }

            barraDeProgressoReport();
        }

        private async Task<bool> AjustarProvasRealizadasNoPeiodoPeloAluno(Aluno aluno, Matricula matriculaQueSeraAjustada, Matricula matriculaCriadaNaIntegracaoDaDataDeReferencia)
        {
            var provasRealizadas = await _ajusteStudentCorrectionRepository.GetCorrections(aluno.AlunoId, matriculaQueSeraAjustada.TurmaId, matriculaQueSeraAjustada.DataDaMatricula);
            if (!provasRealizadas?.Any() ?? true) return false;

            var provasAjustasParaATurmaCorreta = provasRealizadas
                .Select(x =>
                {
                    var correction = new StudentCorrection
                    (
                        x.Test_Id,
                        matriculaCriadaNaIntegracaoDaDataDeReferencia.TurmaId,
                        x.alu_id,
                        aluno.EntidadeId,
                        matriculaCriadaNaIntegracaoDaDataDeReferencia.DreId,
                        matriculaCriadaNaIntegracaoDaDataDeReferencia.EscolaId
                    );

                    correction.Answers.AddRange(x.Answers);
                    correction.Automatic = x.Automatic;
                    correction.Grade = x.Grade;
                    correction.Hits = x.Hits;
                    correction.NumberAnswers = x.NumberAnswers;
                    correction.OrdemUltimaResposta = x.OrdemUltimaResposta;
                    correction.provaFinalizada = x.provaFinalizada;

                    return correction;
                })
                .ToList();

            await _ajusteStudentCorrectionRepository.InsertManyAsync(provasAjustasParaATurmaCorreta);
            foreach (var provaRealizada in provasRealizadas)
                await _ajusteStudentCorrectionRepository.Delete(provaRealizada);

            AddProvaPorTurma(provasAjustasParaATurmaCorreta
                .Select(x => new ProvaPorTurmaParaAjusteDaCorrecaoDto
                {
                    TestId = x.Test_Id,
                    TurmaId = x.tur_id,
                    EntidadeId = aluno.EntidadeId
                }).ToList());

            return true;
        }

        private void AddProvaPorTurma(IEnumerable<ProvaPorTurmaParaAjusteDaCorrecaoDto> provasPorTurma)
        {
            foreach (var provaPorTurma in provasPorTurma)
                _provasPorTurmaParaAjusteDaCorrecao.Add(provaPorTurma);
        }

        private async Task AjustarStatusDeCorrecaoDasProvasParaAsTurmas()
        {
            if (!_provasPorTurmaParaAjusteDaCorrecao.Any()) return;
            foreach (var provaPorTurma in _provasPorTurmaParaAjusteDaCorrecao)
                await _ajusteTestSectionStatusCorrectionRepository.UpdateStatusAsync(provaPorTurma.TestId, provaPorTurma.TurmaId, Util.EnumStatusCorrection.PartialSuccess);
        }
    }
}