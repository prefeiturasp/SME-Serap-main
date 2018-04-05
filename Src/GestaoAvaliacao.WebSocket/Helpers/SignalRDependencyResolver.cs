using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Microsoft.AspNet.SignalR;

namespace GestaoAvaliacao.WebSocket.Helpers
{
	public class SignalRDependencyResolver : Microsoft.AspNet.SignalR.DefaultDependencyResolver
	{
		private readonly IWindsorContainer _container;

		public SignalRDependencyResolver(IWindsorContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}

			_container = container;

			// perform the lazy registrations
			foreach (var c in _lazyRegistrations)
				_container.Register(c);

			_lazyRegistrations.Clear();
		}

		public override object GetService(Type serviceType)
		{
			if (_container.Kernel.HasComponent(serviceType))
				return _container.Resolve(serviceType);
			return base.GetService(serviceType);
		}

		public override IEnumerable<object> GetServices(Type serviceType)
		{
			IEnumerable<object> objects;
			if (_container.Kernel.HasComponent(serviceType))
				objects = _container.ResolveAll(serviceType).Cast<object>();
			else
				objects = new object[] { };

			var originalContainerServices = base.GetServices(serviceType);
			if (originalContainerServices != null)
				return objects.Concat(originalContainerServices);

			return objects;
		}

		public override void Register(Type serviceType, Func<object> activator)
		{
			if (_container != null)
				// cannot unregister components in windsor, so we use a trick
				_container.Register(Component.For(serviceType).UsingFactoryMethod(activator, true).OverridesExistingRegistration());
			else
				// lazy registration for when the container is up
				_lazyRegistrations.Add(Component.For(serviceType).UsingFactoryMethod(activator));
		}

		// a form of lazy initialization is actually needed because the DefaultDependencyResolver starts initializing itself immediately
		// while we now want to store everything inside CastleWindsor, so the actual registration step have to be postponed until the 
		// container is available
		private readonly List<ComponentRegistration<object>> _lazyRegistrations = new List<ComponentRegistration<object>>();
	}

	public static class WindsorTrickyExtensions
	{
		/// <summary>
		/// Overrideses the existing registration:
		/// to overide an existiong component registration you need to do two things:
		/// 1- give it a name (we use a guid to guarantee uniqueness).
		/// 2- set it as default.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="componentRegistration">The component registration.</param>
		/// <returns></returns>
		public static ComponentRegistration<T> OverridesExistingRegistration<T>(this ComponentRegistration<T> componentRegistration) where T : class
		{
			return componentRegistration
				.Named(Guid.NewGuid().ToString())
				.IsDefault();
		}
	}
}