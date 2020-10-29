using FluentValidation;
using GestaoAvaliacao.Entities.DTO.StudentTestAccoplishments;
using GestaoAvaliacao.Entities.StudentTestAccoplishments;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business.StudentTestAccoplishments
{
    public class StudentTestAccoplishmentBusiness : IStudentTestAccoplishmentBusiness
    {
        private readonly IStudentTestAccoplishmentRepository _studentTestAccoplishmentRepository;
        private readonly ITestRepository _testRepository;
        private readonly IValidator<StartStudentTestSessionDto> _startStudentTestSessionValidator;
        private readonly IValidator<EndStudentTestSessionDto> _endStudentTestSessionValidator;
        private readonly IValidator<EndStudentTestAccoplishmentDto> _endStudentTestAccoplishmentValidator;

        public StudentTestAccoplishmentBusiness(IStudentTestAccoplishmentRepository studentTestAccoplishmentRepository, ITestRepository testRepository,
            IValidator<StartStudentTestSessionDto> startStudentTestSessionValidator, IValidator<EndStudentTestSessionDto> endtudentTestSessionValidator,
            IValidator<EndStudentTestAccoplishmentDto> endStudentTestAccoplishmentValidator)
        {
            _studentTestAccoplishmentRepository = studentTestAccoplishmentRepository;
            _testRepository = testRepository;
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
    }
}