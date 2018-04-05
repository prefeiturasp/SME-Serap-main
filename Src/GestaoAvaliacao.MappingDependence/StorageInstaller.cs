using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using GestaoAvaliacao.FileServer;
using GestaoAvaliacao.IFileServer;

namespace GestaoAvaliacao.MappingDependence
{
    public class StorageInstaller : IWindsorInstaller
	{
		public bool LifestylePerWebRequest { get; set; }

		public StorageInstaller()
		{
			this.LifestylePerWebRequest = true;
		}

		/// <summary>
		/// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer"/>.
		/// </summary>
		/// <param name="container">The container.</param><param name="store">The configuration store.</param>
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			switch (GetKey())
			{
				case "Local":
					container.Register(Component.For<IStorage>().ImplementedBy<LocalStorage>().SetLifestyle(LifestylePerWebRequest));
					break;
			}
		}

		private string GetKey()
		{
			string storageType = System.Configuration.ConfigurationManager.AppSettings["StorageType"];

			return string.IsNullOrEmpty(storageType) ? string.Empty : storageType;
		}
	}
}
