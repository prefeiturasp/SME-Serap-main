using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using GestaoAvaliacao.Util.Videos;

namespace GestaoAvaliacao.MappingDependence
{
    public class UtilIntaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyContaining<VideoConverter>()
                                .BasedOn(typeof(IVideoConverter))
                                .WithService.AllInterfaces()
                                .SetLifestyle(true));
        }
    }
}