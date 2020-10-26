using FluentValidation;
using GestaoAvaliacao.Entities;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.IRepository;
using System;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Business
{
    public class StudentTestSessionBusiness : IStudentTestSessionBusiness
    {
        private readonly IStudentTestSessionRepository _studentTestSessionRepository;
        private readonly ITestRepository _testRepository;
        private readonly IValidator<StartStudentTestSessionDto> _startStudentTestSessionValidator;
        private readonly IValidator<EndStudentTestSessionDto> _endStudentTestSessionValidator;

        public StudentTestSessionBusiness(IStudentTestSessionRepository studentTestSessionRepository, ITestRepository testRepository,
            IValidator<StartStudentTestSessionDto> startStudentTestSessionValidator, IValidator<EndStudentTestSessionDto> endtudentTestSessionValidator)
        {
            _studentTestSessionRepository = studentTestSessionRepository;
            _testRepository = testRepository;
            _startStudentTestSessionValidator = startStudentTestSessionValidator;
            _endStudentTestSessionValidator = endtudentTestSessionValidator;
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
                var test = _testRepository.GetTestBy_Id(dto.TestId);
                if (test is null)
                {
                    dto.AddError("A prova em andamento não foi localizada.");
                    return;
                }

                var entity = new StudentTestSession(dto.AluId, dto.TurId, test, dto.UsuId, Guid.Parse(dto.ConnectionId));
                if (!entity.Validate.IsValid)
                {
                    dto.AddError(entity.Validate.Message);
                    return;
                }

                entity.Start();
                await _studentTestSessionRepository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                dto.AddError(ex);
            }
        }

        public async Task EndSessionAsync(EndStudentTestSessionDto dto)
        {
            if(dto is null)
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
                var entity = dto.ByLostConnection
                    ? await _studentTestSessionRepository.GetAsync(Guid.Parse(dto.ConnectionId))
                    : await _studentTestSessionRepository.GetAsync(dto.AluId, dto.TurId, dto.TestId);
                if(entity is null)
                {
                    dto.AddError("A sessão do usuário não foi localizada.");
                    return;
                }

                entity.End(dto.ByLostConnection);
                await _studentTestSessionRepository.UpdateAsync(entity);
            }
            catch(Exception ex)
            {
                dto.AddError(ex);
            }
        }
    }
}