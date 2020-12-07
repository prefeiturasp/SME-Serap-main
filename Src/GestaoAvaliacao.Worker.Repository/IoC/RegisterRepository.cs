using GestaoAvaliacao.Worker.IoC.Contracts;
using GestaoAvaliacao.Worker.Repository.Contracts;
using GestaoAvaliacao.Worker.Repository.Parameters;
using GestaoAvaliacao.Worker.Repository.Tests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoAvaliacao.Worker.Repository.IoC
{
    public class RegisterRepository : IIoCRegisterBootstrap
    {
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IStudentTestSentRepository, StudentTestSentRepository>();
            services.AddTransient<IParameterRepository, ParameterRepository>();
            services.AddTransient<IStudentCorrectionAuxiliarRepository, StudentCorrectionAuxiliarRepository>();
            services.AddTransient<ITestSectionStatusCorrectionRepository, TestSectionStatusCorrectionRepository>();
        }
    }
}