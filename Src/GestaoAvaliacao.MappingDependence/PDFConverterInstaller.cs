using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using GestaoAvaliacao.IPDFConverter;
using GestaoAvaliacao.PDFConverter;

namespace GestaoAvaliacao.MappingDependence
{
    public class PDFConverterInstaller : IWindsorInstaller
	{
		public bool LifestylePerWebRequest { get; set; }

		public PDFConverterInstaller()
		{
			this.LifestylePerWebRequest = true;
		}
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Classes.FromAssemblyContaining<HTMLToPDF>()
								.BasedOn(typeof(IHTMLToPDF))
								.WithService.AllInterfaces()
								.SetLifestyle(LifestylePerWebRequest));

			container.Register(Classes.FromAssemblyContaining<PDFMerger>()
								.BasedOn(typeof(IPDFMerger))
								.WithService.AllInterfaces()
								.SetLifestyle(LifestylePerWebRequest));
		}
	}


}
