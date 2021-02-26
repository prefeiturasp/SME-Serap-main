using FluentValidation;
using GestaoAvaliacao.Dtos.StudentTestAccoplishments;
using GestaoAvaliacao.Entities.DTO.StudentTestAccoplishments;
using GestaoAvaliacao.Entities.StudentTestAccoplishments;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business.StudentTestAccoplishments
{
    public class StudentTestAccoplishmentBusiness : IStudentTestAccoplishmentBusiness
    {
        private readonly IStudentTestAccoplishmentRepository _studentTestAccoplishmentRepository;
        private readonly ITestRepository _testRepository;
        private readonly ITestBusiness _testBusiness;
        private readonly IStudentCorrectionBusiness _studentCorrectionBusiness;
        private readonly IValidator<StartStudentTestSessionDto> _startStudentTestSessionValidator;
        private readonly IValidator<EndStudentTestSessionDto> _endStudentTestSessionValidator;
        private readonly IValidator<EndStudentTestAccoplishmentDto> _endStudentTestAccoplishmentValidator;

        public StudentTestAccoplishmentBusiness(IStudentTestAccoplishmentRepository studentTestAccoplishmentRepository,
            ITestRepository testRepository, ITestBusiness testBusiness, IStudentCorrectionBusiness studentCorrectionBusiness,
            IValidator<StartStudentTestSessionDto> startStudentTestSessionValidator, IValidator<EndStudentTestSessionDto> endtudentTestSessionValidator,
            IValidator<EndStudentTestAccoplishmentDto> endStudentTestAccoplishmentValidator)
        {
            _studentTestAccoplishmentRepository = studentTestAccoplishmentRepository;
            _testRepository = testRepository;
            _testBusiness = testBusiness;
            _studentCorrectionBusiness = studentCorrectionBusiness;
            _startStudentTestSessionValidator = startStudentTestSessionValidator;
            _endStudentTestSessionValidator = endtudentTestSessionValidator;
            _endStudentTestAccoplishmentValidator = endStudentTestAccoplishmentValidator;
        }

        public async Task StartSessionAsync(StartStudentTestSessionDto dto)
        {
            if (dto is null)
            {
                dto = new StartStudentTestSessionDto();
                dto.AddError("Os dados para início da sessão na prova não foram informados.");
                return;
            }

            var validationResult = _startStudentTestSessionValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                dto.AddError(validationResult);
                return;
            }

            try
            {
                var studentTestAccoplishment = await GetStudentTestAccoplishmentAsync(dto);
                if (!dto.IsValid) return;

                var session = studentTestAccoplishment.CreateSession(Guid.Parse(dto.ConnectionId));
                if (!session.Validate.IsValid)
                {
                    dto.AddError(session.Validate.Message);
                    return;
                }

                session.Start();
                await _studentTestAccoplishmentRepository.AddSessionAsync(session);
            }
            catch (Exception ex)
            {
                dto.AddError(ex);
            }
        }

        public async Task EndSessionAsync(EndStudentTestSessionDto dto)
        {
            if (dto is null)
            {
                dto = new EndStudentTestSessionDto();
                dto.AddError("Os dados para finalizar da sessão na prova não foram informados.");
                return;
            }

            var validationResult = _endStudentTestSessionValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                dto.AddError(validationResult);
                return;
            }

            try
            {
                var session = await _studentTestAccoplishmentRepository.GetSessionAsync(Guid.Parse(dto.ConnectionId));
                if (session is null)
                {
                    dto.AddError("A sessão do usuário não foi localizada.");
                    return;
                }

                if (session.Done()) return;
                session.End(dto.ByLostConnection);
                await _studentTestAccoplishmentRepository.UpdateSessionAsync(session);
            }
            catch (Exception ex)
            {
                dto.AddError(ex);
            }
        }

        public async Task EndStudentTestAccoplishmentAsync(EndStudentTestAccoplishmentDto dto)
        {
            if (dto is null)
            {
                dto = new EndStudentTestAccoplishmentDto();
                dto.AddError("Os dados para finalizar da sessão na prova não foram informados.");
                return;
            }

            var validationResult = _endStudentTestAccoplishmentValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                dto.AddError(validationResult);
                return;
            }

            try
            {
                var studentTestAccoplishment = await _studentTestAccoplishmentRepository.GetAsync(dto.AluId, dto.TurId, dto.TestId);
                if (studentTestAccoplishment is null)
                {
                    dto.AddError("O registro de execução de prova do aluno não foi localizado.");
                    return;
                }

                var endDate = DateTime.Now;
                var session = studentTestAccoplishment.Sessions.FirstOrDefault(x => x.ConnectionId == Guid.Parse(dto.ConnectionId));
                session?.End(false, endDate);
                studentTestAccoplishment.End(endDate);

                await _studentTestAccoplishmentRepository.UpdateSessionAsync(session);
                await _studentTestAccoplishmentRepository.UpdateAsync(studentTestAccoplishment);
            }
            catch (Exception ex)
            {
                dto.AddError(ex);
            }
        }

        private async Task<StudentTestAccoplishment> GetStudentTestAccoplishmentAsync(StartStudentTestSessionDto dto)
        {
            var studentTestAccoplishment = await _studentTestAccoplishmentRepository.GetAsync(dto.AluId, dto.TurId, dto.TestId);
            if (studentTestAccoplishment != null) return studentTestAccoplishment;

            var test = _testRepository.GetTestBy_Id(dto.TestId);
            if (test is null)
            {
                dto.AddError("A prova em andamento não foi localizada.");
                return null;
            }

            studentTestAccoplishment = new StudentTestAccoplishment(dto.AluId, dto.TurId, test);
            if (!studentTestAccoplishment.Validate.IsValid)
            {
                dto.AddError(studentTestAccoplishment.Validate.Message);
                return null;
            }

            await _studentTestAccoplishmentRepository.AddAsync(studentTestAccoplishment);
            return studentTestAccoplishment;
        }

        public async Task<StudentTestTimeResultDto> GetStudenteResultAsync(Guid pesId)
        {
            var resultado = new StudentTestTimeResultDto();
            resultado.Ano = DateTime.Now.Year;
            var listaDeProvasDoAnoCorrente = new List<StudentTestTimeListaDto>();
            var listaDeProvasDosAnosAnteriores = new List<StudentTestTimeListaDto>();
            var electronicTests = await _testBusiness.SearchEletronicTestsByPesId(pesId);
            if (electronicTests is null || electronicTests.Count() == 0)
                return resultado;

            var temposDeDuracao = await _studentTestAccoplishmentRepository.GetAsyncByAluId(electronicTests.FirstOrDefault().alu_id);
            foreach (var electronicTest in electronicTests)
            {
                var tempoDeDuracaoDaProva = temposDeDuracao.FirstOrDefault(o => o.TestId == electronicTest.Id);
                var studentCorrection = await _studentCorrectionBusiness.GetStudentCorrectionByTestAluId(electronicTest.Id, electronicTest.alu_id, electronicTest.tur_id);
                var dataDeFinalizacaoDaProva = (studentCorrection?.provaFinalizada ?? false) ?
                    (tempoDeDuracaoDaProva?.DataDeFinalizacaoDaProva ?? studentCorrection.UpdateDate.ToShortDateString()) :
                    "Prova em andamento.";
                if (electronicTest.ApplicationEndDate.Year >= DateTime.Now.Year)
                {

                    listaDeProvasDoAnoCorrente.Add(new StudentTestTimeListaDto
                    {
                        DataDeFinalizacao = dataDeFinalizacaoDaProva,
                        NomeDaProva = electronicTest.Description,
                        Periodo = electronicTest.FrequencyApplicationText,
                        QuantidadeDeItens = electronicTest.NumberItem ?? 0,
                        TempoDeProva = tempoDeDuracaoDaProva.TempoDeDuracao
                    });
                }
                else
                {
                    listaDeProvasDosAnosAnteriores.Add(new StudentTestTimeListaDto
                    {
                        DataDeFinalizacao = dataDeFinalizacaoDaProva,
                        NomeDaProva = electronicTest.Description,
                        Periodo = electronicTest.FrequencyApplicationText,
                        QuantidadeDeItens = electronicTest.NumberItem ?? 0,
                        TempoDeProva = tempoDeDuracaoDaProva.TempoDeDuracao
                    });
                }
            }
            resultado.ListaProvasDoAnoCorrente = listaDeProvasDoAnoCorrente;
            resultado.ListaProvasDoAnoCorrente = listaDeProvasDosAnosAnteriores;

            return resultado;
        }
    }
}