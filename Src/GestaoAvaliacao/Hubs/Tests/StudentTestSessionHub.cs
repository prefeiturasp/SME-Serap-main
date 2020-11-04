using GestaoAvaliacao.Entities.DTO.StudentTestAccoplishments;
using GestaoAvaliacao.IBusiness;
using GestaoAvaliacao.WebProject.Facade;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Hubs.Tests
{
    public class StudentTestSessionHub : Hub
    {
        private readonly IStudentTestAccoplishmentBusiness _studentTestSessionBusiness;
        private const string errorLogType = "StudentTestSession";

        public StudentTestSessionHub(IStudentTestAccoplishmentBusiness studentTestSessionBusiness)
        {
            _studentTestSessionBusiness = studentTestSessionBusiness;
        }

        public async Task StartSession(long aluId, long turId, long testId)
        {
            var dto = new StartStudentTestSessionDto
            {
                AluId = aluId,
                TurId = turId,
                TestId = testId,
                ConnectionId = Context.ConnectionId
            };

            await _studentTestSessionBusiness.StartSessionAsync(dto);
            if (!dto.IsValid)
            {
                Clients.Caller.reportStartSessionError(dto.Errors);
                LogErrors(dto.Errors);
            }
            else
            {
                Clients.Caller.reportStartSessionSuccess();
            }
        }

        public async Task EndSession()
        {
            var dto = new EndStudentTestSessionDto
            {
                ConnectionId = Context.ConnectionId,
            };

            await _studentTestSessionBusiness.EndSessionAsync(dto);
            if (!dto.IsValid)
            {
                Clients.Caller.reportEndSessionError(dto.Errors);
                LogErrors(dto.Errors);
            }
            else
            {
                Clients.Caller.reportEndSessionSuccess();
            }
        }

        public async Task EndTest(long aluId, long turId, long testId)
        {
            var dto = new EndStudentTestAccoplishmentDto
            {
                AluId = aluId,
                TurId = turId,
                TestId = testId,
                ConnectionId = Context.ConnectionId
            };

            await _studentTestSessionBusiness.EndStudentTestAccoplishmentAsync(dto);
            if (!dto.IsValid)
            {
                Clients.Caller.reportEndSessionError(dto.Errors);
                LogErrors(dto.Errors);
            }
            else
            {
                Clients.Caller.reportEndSessionSuccess();
            }
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            var dto = new EndStudentTestSessionDto
            {
                ConnectionId = Context.ConnectionId,
                ByLostConnection = true
            };

            await Task.WhenAll(_studentTestSessionBusiness.EndSessionAsync(dto), base.OnDisconnected(stopCalled));
            if (!dto.IsValid)
            {
                Clients.Caller.reportEndSessionError(dto.Errors);
                LogErrors(dto.Errors);
            }
        }

        private void LogErrors(IEnumerable<string> errorMessages)
        {
            foreach (var errorMessage in errorMessages)
                LogFacade.SaveBasicError(errorMessage, errorLogType);
        }
    }
}