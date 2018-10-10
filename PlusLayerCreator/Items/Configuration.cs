using System.Collections.ObjectModel;
using System.Linq;

namespace PlusLayerCreator.Items
{
    public class Configuration : ItemBase
    {
        private ObservableCollection<ConfigurationItem> _dataLayout;
        private ObservableCollection<DirectHopItem> _directHops;
        private string _controllerHandle;
        private string _dialogTranslationEnglish;
        private string _dialogTranslationGerman;
        private string _dialogName;
        private string _product;
        private bool _isCreateUiFilter;
        private bool _isCreateUi;
        private bool _isCreateRepository;
        private bool _isCreateRepositoryDtoFactory;
        private bool _isCreateDataItemFactory;
        private bool _isCreateDataItem;
        private bool _isUseBusinessServiceWithoutBo;
        private bool _isCreateBusinessService;
        private bool _isCreateGateway;
        private bool _isCreateDtoFactory;
        private bool _isCreateDto;
        private string _outputPath;
        private string _inputPath;

        public string InputPath
        {
            get => _inputPath;
            set => SetProperty(ref _inputPath, value);
        }

        public string OutputPath
        {
            get => _outputPath;
            set => SetProperty(ref _outputPath, value);
        }

        public bool IsCreateDto
        {
            get => _isCreateDto;
            set => SetProperty(ref _isCreateDto, value);
        }

        
        public bool IsCreateDtoFactory
        {
            get => _isCreateDtoFactory;
            set => SetProperty(ref _isCreateDtoFactory, value);
        }

        
        public bool IsCreateGateway
        {
            get => _isCreateGateway;
            set => SetProperty(ref _isCreateGateway, value);
        }

        
        public bool IsCreateBusinessService
        {
            get => _isCreateBusinessService;
            set => SetProperty(ref _isCreateBusinessService, value);
        }

        
        public bool IsUseBusinessServiceWithoutBo
        {
            get => _isUseBusinessServiceWithoutBo;
            set => SetProperty(ref _isUseBusinessServiceWithoutBo, value);
        }

        
        public bool IsCreateDataItem
        {
            get => _isCreateDataItem;
            set => SetProperty(ref _isCreateDataItem, value);
        }

        
        public bool IsCreateDataItemFactory
        {
            get => _isCreateDataItemFactory;
            set => SetProperty(ref _isCreateDataItemFactory, value);
        }

        
        public bool IsCreateRepositoryDtoFactory
        {
            get => _isCreateRepositoryDtoFactory;
            set => SetProperty(ref _isCreateRepositoryDtoFactory, value);
        }

        
        public bool IsCreateRepository
        {
            get => _isCreateRepository;
            set => SetProperty(ref _isCreateRepository, value);
        }

        
        public bool IsCreateUi
        {
            get => _isCreateUi;
            set => SetProperty(ref _isCreateUi, value);
        }

        
        public bool IsCreateUiFilter
        {
            get => _isCreateUiFilter;
            set => SetProperty(ref _isCreateUiFilter, value);
        }

        
        public string Product
        {
            get => _product;
            set => SetProperty(ref _product, value);
        }

        
        public string DialogName
        {
            get => _dialogName;
            set => SetProperty(ref _dialogName, value);
        }

        
        public string DialogTranslationGerman
        {
            get => _dialogTranslationGerman;
            set => SetProperty(ref _dialogTranslationGerman, value);
        }

        
        public string DialogTranslationEnglish
        {
            get => _dialogTranslationEnglish;
            set => SetProperty(ref _dialogTranslationEnglish, value);
        }

        
        public string ControllerHandle
        {
            get => _controllerHandle;
            set => SetProperty(ref _controllerHandle, value);
        }

        
        public ObservableCollection<ConfigurationItem> DataLayout
        {
            get => _dataLayout;
            set => SetProperty(ref _dataLayout, value);
        }

        
        public ObservableCollection<DirectHopItem> DirectHops
        {
            get => _directHops;
            set => SetProperty(ref _directHops, value);
        }

        public ConfigurationItem GetMasterItem()
        {
            return DataLayout.OrderBy(x => x.Order).FirstOrDefault(t => !t.IsPreFilterItem);
        }
    }
}