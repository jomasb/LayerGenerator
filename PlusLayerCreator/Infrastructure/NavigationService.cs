using System;
using Prism.Regions;

namespace PlusLayerCreator.Infrastructure
{
    public class NavigationService : INavigationService
    {
        // Navigation Pipeline
        // https://blogs.msdn.microsoft.com/kashiffl/2010/10/04/prism-v4-region-navigation-pipeline/


        private readonly IRegionManager _regionManager;

        public NavigationService(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }


        public void Navigate(string regionName, string viewName)
        {
            _regionManager.RequestNavigate(
                regionName,
                new Uri(viewName, UriKind.Relative));

            //Navigate(regionName, viewName, null);
        }

        public void Navigate(string regionName, string viewName, NavigationParameters parameters)
        {
            _regionManager.RequestNavigate(
                regionName,
                new Uri(viewName, UriKind.Relative),
                parameters);
        }

        public void Navigate(string regionName, string viewName, string parameterName, object parameter)
        {
            var parameters = new NavigationParameters();
            parameters.Add(parameterName, parameter);
            Navigate(regionName, viewName, parameters);
        }

        public void Unload(string regionName)
        {
            var region = _regionManager.Regions[regionName];
            var views = region.Views;

            // Unload the views
            foreach (var view in views) region.Remove(view);
        }
    }
}