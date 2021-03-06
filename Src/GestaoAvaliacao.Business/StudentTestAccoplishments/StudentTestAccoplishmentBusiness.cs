using FluentValidation;
using GestaoAvaliacao.Dtos.StudentTestAccoplishments;
using GestaoAvaliacao.Entities.DTO;
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
        private readonly IStudentCorrectionBusiness _studentCorrectionBusiness;
        private readonly IValidator<StartStudentTestSessionDto> _startStudentTestSessionValidator;
        private readonly IValidator<EndStudentTestSessionDto> _endStudentTestSessionValidator;
        private readonly IValidator<EndStudentTestAccoplishmentDto> _endStudentTestAccoplishmentValidator;

        public StudentTestAccoplishmentBusiness(IStudentTestAccoplishmentRepository studentTestAccoplishmentRepository,
            ITestRepository testRepository, IStudentCorrectionBusiness studentCorrectionBusiness,
            IValidator<StartStudentTestSessionDto> startStudentTestSessionValidator, IValidator<EndStudentTestSessionDto> endtudentTestSessionValidator,
            IValidator<EndStudentTestAccoplishmentDto> endStudentTestAccoplishmentValidator)
        {
            _studentTestAccoplishmentRepository = studentTestAccoplishmentRepository;
            _testRepository = testRepository;
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

        public async Task<StudentTestTimeResultDto> GetStudenteResultAsync(List<ElectronicTestDTO> electronicTests)
        {
            var resultado = new StudentTestTimeResultDto
            {
                Ano = DateTime.Now.Year
            };
            var listaDeAnos = new List<int>();
            var listaDeProvasDoAnoCorrente = new List<StudentTestTimeListaDto>();
            var listaDeProvasDosAnosAnteriores = new List<StudentTestTimeListaDto>();

            var temposDeDuracao = await _studentTestAccoplishmentRepository.GetAsyncByAluId(electronicTests.FirstOrDefault().alu_id);
            foreach (var electronicTest in electronicTests)
            {
                var tempoDeDuracaoDaProva = temposDeDuracao.FirstOrDefault(o => o.TestId == electronicTest.Id);
                var studentCorrection = await _studentCorrectionBusiness.GetStudentCorrectionByTestAluId(electronicTest.Id, electronicTest.alu_id, electronicTest.tur_id);
                if (studentCorrection != null && !studentCorrection.provaFinalizada.HasValue)
                    studentCorrection.provaFinalizada = false;

                if ((studentCorrection == null && electronicTest.quantDiasRestantes > 0) ||
                    (studentCorrection != null && !studentCorrection.provaFinalizada.Value && electronicTest.quantDiasRestantes > 0))
                    continue;

                var dataDeFinalizacaoDaProva = "";
                if (studentCorrection != null && !studentCorrection.provaFinalizada.HasValue)
                    studentCorrection.provaFinalizada = false;

                if (electronicTest.ApplicationEndDate.Year >= resultado.Ano)
                {
                    if ((studentCorrection.provaFinalizada.Value) && studentCorrection.UpdateDate <= new DateTime(2000, 01, 01))
                        dataDeFinalizacaoDaProva = electronicTest.ApplicationEndDate.ToShortDateString();
                    else if ((studentCorrection.provaFinalizada.Value) && studentCorrection.UpdateDate > new DateTime(2000, 01, 01))
                        dataDeFinalizacaoDaProva = studentCorrection.UpdateDate.ToShortTimeString();
                    else
                        dataDeFinalizacaoDaProva = "(sem informação).";

                    resultado.ProvasDoAnoCorrente = true;
                    listaDeProvasDoAnoCorrente.Add(new StudentTestTimeListaDto
                    {
                        Id = electronicTest.Id,
                        TurId = electronicTest.tur_id,
                        EscId = electronicTest.esc_id,
                        DreId = electronicTest.dre_id,
                        DataDeFinalizacao = dataDeFinalizacaoDaProva,
                        NomeDaProva = electronicTest.Description,
                        Periodo = electronicTest.FrequencyApplicationText,
                        QuantidadeDeItens = electronicTest.NumberItem ?? 0,
                        TempoDeProva = tempoDeDuracaoDaProva?.TempoDeDuracao ?? "(sem informação)",
                        Ano = electronicTest.ApplicationEndDate.Year
                    });

                    if (!listaDeAnos.Any(s => s == electronicTest.ApplicationEndDate.Year))
                        listaDeAnos.Add(electronicTest.ApplicationEndDate.Year);
                }
                else
                {
                    if (studentCorrection == null ||
                        (studentCorrection.UpdateDate <= new DateTime(2000, 01, 01)
                        && electronicTest.ApplicationEndDate > new DateTime(2000, 01, 01))
                    )
                        dataDeFinalizacaoDaProva = electronicTest.ApplicationEndDate.ToShortDateString();
                    else if (studentCorrection != null && studentCorrection.UpdateDate > new DateTime(2000, 01, 01))
                        dataDeFinalizacaoDaProva = studentCorrection.UpdateDate.ToShortTimeString();
                    else
                        dataDeFinalizacaoDaProva = "(sem informação).";

                    listaDeProvasDosAnosAnteriores.Add(new StudentTestTimeListaDto
                    {
                        Id = electronicTest.Id,
                        TurId = electronicTest.tur_id,
                        EscId = electronicTest.esc_id,
                        DreId = electronicTest.dre_id,
                        DataDeFinalizacao = dataDeFinalizacaoDaProva,
                        NomeDaProva = electronicTest.Description,
                        Periodo = electronicTest.FrequencyApplicationText,
                        QuantidadeDeItens = electronicTest.NumberItem ?? 0,
                        TempoDeProva = tempoDeDuracaoDaProva?.TempoDeDuracao ?? "(sem informação) ",
                        Ano = electronicTest.ApplicationEndDate.Year
                    });

                    if (!listaDeAnos.Any(s => s == electronicTest.ApplicationEndDate.Year))
                        listaDeAnos.Add(electronicTest.ApplicationEndDate.Year);
                }
            }
            resultado.ListaDeAnos = listaDeAnos.OrderByDescending(o => o).ToList();
            resultado.ListaProvasDoAnoCorrente = listaDeProvasDoAnoCorrente;
            resultado.ListaProvasDosAnosAnteriores = listaDeProvasDosAnosAnteriores;

            return resultado;
        }
    }
}