using Castle.MicroKernel.Registration;
using GestaoAvaliacao.IFileServer;
using System.Collections;

namespace GestaoAvaliacao.MappingDependence
{
    public static class ComponentRegistrationExtender
	{
		public static ComponentRegistration<IStorage> SetLifestyle(this ComponentRegistration<IStorage> reg, bool LifestylePerWebRequest)
		{
			if (LifestylePerWebRequest)
				return reg.LifestylePerWebRequest();
			else
				return reg.LifestyleSingleton();
		}
	}

	public static class BasedOnDescriptorExtender
	{
		public static BasedOnDescriptor SetLifestyle(this BasedOnDescriptor reg, bool LifestylePerWebRequest)
		{
			if (LifestylePerWebRequest)
				return reg.LifestylePerWebRequest();
			else
				return reg.LifestyleSingleton();
		}
	}
}
