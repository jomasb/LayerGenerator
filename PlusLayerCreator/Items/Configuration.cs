using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace PlusLayerCreator.Items
{
    [DataContract]
    public class Configuration : ItemBase
    {
        private IList<ConfigurationItem> _dataLayout;
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

        [DataMember]
        public string InputPath
        {
            get => _inputPath;
            set => SetProperty(ref _inputPath, value);
        }

        [DataMember]
        public string OutputPath
        {
            get => _outputPath;
            set => SetProperty(ref _outputPath, value);
        }

        [DataMember]
        public bool IsCreateDto
        {
            get => _isCreateDto;
            set => SetProperty(ref _isCreateDto, value);
        }

        [DataMember]
        public bool IsCreateDtoFactory
        {
            get => _isCreateDtoFactory;
            set => SetProperty(ref _isCreateDtoFactory, value);
        }

        [DataMember]
        public bool IsCreateGateway
        {
            get => _isCreateGateway;
            set => SetProperty(ref _isCreateGateway, value);
        }

        [DataMember]
        public bool IsCreateBusinessService
        {
            get => _isCreateBusinessService;
            set => SetProperty(ref _isCreateBusinessService, value);
        }

        [DataMember]
        public bool IsUseBusinessServiceWithoutBo
        {
            get => _isUseBusinessServiceWithoutBo;
            set => SetProperty(ref _isUseBusinessServiceWithoutBo, value);
        }

        [DataMember]
        public bool IsCreateDataItem
        {
            get => _isCreateDataItem;
            set => SetProperty(ref _isCreateDataItem, value);
        }

        [DataMember]
        public bool IsCreateDataItemFactory
        {
            get => _isCreateDataItemFactory;
            set => SetProperty(ref _isCreateDataItemFactory, value);
        }

        [DataMember]
        public bool IsCreateRepositoryDtoFactory
        {
            get => _isCreateRepositoryDtoFactory;
            set => SetProperty(ref _isCreateRepositoryDtoFactory, value);
        }

        [DataMember]
        public bool IsCreateRepository
        {
            get => _isCreateRepository;
            set => SetProperty(ref _isCreateRepository, value);
        }

        [DataMember]
        public bool IsCreateUi
        {
            get => _isCreateUi;
            set => SetProperty(ref _isCreateUi, value);
        }

        [DataMember]
        public bool IsCreateUiFilter
        {
            get => _isCreateUiFilter;
            set => SetProperty(ref _isCreateUiFilter, value);
        }

        [DataMember]
        public string Product
        {
            get => _product;
            set => SetProperty(ref _product, value);
        }

        [DataMember]
        public string DialogName
        {
            get => _dialogName;
            set => SetProperty(ref _dialogName, value);
        }

        [DataMember]
        public string DialogTranslationGerman
        {
            get => _dialogTranslationGerman;
            set => SetProperty(ref _dialogTranslationGerman, value);
        }

        [DataMember]
        public string DialogTranslationEnglish
        {
            get => _dialogTranslationEnglish;
            set => SetProperty(ref _dialogTranslationEnglish, value);
        }

        [DataMember]
        public string ControllerHandle
        {
            get => _controllerHandle;
            set => SetProperty(ref _controllerHandle, value);
        }

        [DataMember]
        public IList<ConfigurationItem> DataLayout
        {
            get => _dataLayout;
            set => SetProperty(ref _dataLayout, value);
        }

        public ConfigurationItem GetMasterItem()
        {
            return DataLayout.FirstOrDefault(t => string.IsNullOrEmpty(t.Parent) && !t.IsPreFilterItem);
        }
    }
}