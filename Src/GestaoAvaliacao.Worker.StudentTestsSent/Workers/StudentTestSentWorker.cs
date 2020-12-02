using GestaoAvaliacao.Worker.Repository.Contracts;
using GestaoAvaliacao.Worker.StudentTestsSent.Requests.Commands;
using MediatR;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GestaoAvaliacao.Worker.StudentTestsSent.Workers
{
    public class StudentTestSentWorker : IHostedService
    {
        private readonly IMediator _mediator;

        public StudentTestSentWorker(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken) => _mediator.Send(new ProcessStudentTestSentCommand());

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("WORKER FINISHED.");
        }
    }
}