using GestaoAvaliacao.Worker.IoC;
using GestaoAvaliacao.Worker.StudentTestsSent.Processing;
using GestaoAvaliacao.Worker.StudentTestsSent.Services.CorrectionResult;
using GestaoAvaliacao.Worker.StudentTestsSent.Services.Grades;
using GestaoAvaliacao.Worker.StudentTestsSent.Workers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace GestaoAvaliacao.Worker.StudentTestsSent
{
    public class Program
    {
        private static readonly IIoCStartup _ioCStartup = new IoCStartup();

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<StudentTestSentWorker>();
                    _ioCStartup.Register(services, hostContext.Configuration);
                    services.AddTransient<IGenerateCorrectionResultsServices, GenerateCorrectionResultsServices>();
                    services.AddTransient<IProcessGradesServices, ProcessGradesServices>();
                    services.AddTransient<IStudentTestSentProcessingChain, StudentTestSentProcessingChain>();
                    services.AddMediatR(Assembly.GetExecutingAssembly());
                });
    }
}