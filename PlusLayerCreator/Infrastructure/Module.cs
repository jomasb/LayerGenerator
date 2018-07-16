using Microsoft.Practices.Unity;
using PlusLayerCreator.Configure;
using PlusLayerCreator.Detail;
using Prism.Modularity;
using Prism.Regions;

namespace PlusLayerCreator.Infrastructure
{
    public class Module : IModule
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
            _container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());

            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion,
                () => _container.Resolve<ConfigureView>());
            _regionManager.RegisterViewWithRegion(RegionNames.DetailRegion,
                () => _container.Resolve<EmptyView>());

            _container.RegisterType<object, EmptyView>(ViewNames.EmptyView);
            _container.RegisterType<object, DataItemDetailView>(ViewNames.DataItemDetailView);
            _container.RegisterType<object, DirectHopDetailView>(ViewNames.DirectHopDetailView);
            _container.RegisterType<object, DataItemPropertyDetailView>(ViewNames.DataItemPropertyDetailView);
        }
    }
}