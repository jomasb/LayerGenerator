using Microsoft.Practices.Unity;
using PlusLayerCreator.Configure;
using Prism.Modularity;
using Prism.Regions;

namespace PlusLayerCreator.Infrastructure
{
	public class Module  : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

		public Module(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;

        }

        public void Initialize()
        {
			_regionManager.RegisterViewWithRegion(RegionNames.MainRegion,
				() => _container.Resolve<ConfigureView>());

        }
    }
}
