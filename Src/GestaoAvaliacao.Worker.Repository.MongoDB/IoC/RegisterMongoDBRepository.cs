using GestaoAvaliacao.Worker.IoC.Contracts;
using GestaoAvaliacao.Worker.Repository.MongoDB.Contracts;
using GestaoAvaliacao.Worker.Repository.MongoDB.Tests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoAvaliacao.Worker.Repository.MongoDB.IoC
{
    public class RegisterMongoDBRepository : IIoCRegisterBootstrap
    {
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IStudentCorrectionMongoDBRepository, StudentCorrectionMongoDBRepository>();
            services.AddTransient<ITestTemplateMongoDBRepository, TestTemplateMongoDBRepository>();
            services.AddTransient<ISectionTestStatsMongoDBRepository, SectionTestStatsMongoDBRepository>();
            services.AddTransient<ICorrectionResultsMongoDBRepository, CorrectionResultsMongoDBRepository>();
        }
    }
}