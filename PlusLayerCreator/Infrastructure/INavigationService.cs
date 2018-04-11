using Prism.Regions;

namespace PlusLayerCreator.Infrastructure
{
    public interface INavigationService
    {
        void Navigate(string regionName, string viewName);
        void Navigate(string regionName, string viewName, NavigationParameters parameters);
        void Navigate(string regionName, string viewName, string parameterName, object parameter);
        void Unload(string regionName);
    }


    public interface IDialog
    {
        bool IsClosed { get; set; }
    }
}