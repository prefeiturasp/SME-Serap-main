using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using GestaoAvaliacao.Entities.DTO;
using GestaoAvaliacao.IBusiness;
using Microsoft.AspNet.SignalR;

namespace GestaoAvaliacao.Hubs.Tests
{
    public class StudentTestSessionHub : Hub
    {
        private readonly IStudentTestSessionBusiness _studentTestSessionBusiness;

        public StudentTestSessionHub(IStudentTestSessionBusiness studentTestSessionBusiness)
        {
            _studentTestSessionBusiness = studentTestSessionBusiness;
        }

        public async Task StartSession(long aluId, long turId, long testId, Guid usuId)
        {
            var dto = new StartStudentTestSessionDto
            {
                AluId = aluId,
                TurId = turId,
                TestId = testId,
                UsuId = usuId,
                ConnectionId = Context.ConnectionId
            };

            await _studentTestSessionBusiness.StartSessionAsync(dto);
            if(!dto.IsValid)
            {
                Clients.Caller.reportStartSessionError(dto.Errors);
                return;
            }

            Clients.Caller.reportStartSessionSuccess();
        }

        public async Task EndSession(long aluId, long turId, long testId)
        {
            var dto = new EndStudentTestSessionDto
            {
                AluId = aluId,
                TurId = turId,
                TestId = testId
            };

            await _studentTestSessionBusiness.EndSessionAsync(dto);
            if (!dto.IsValid)
            {
                Clients.Caller.reportEndSessionError(dto.Errors);
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
        }
    }
}