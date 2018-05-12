using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PlusLayerCreator.Items
{
    [DataContract]
    public class Configuration
    {
        [DataMember] public string InputPath { get; set; }

        [DataMember] public string OutputPath { get; set; }

        [DataMember] public bool IsCreateDto { get; set; }

        [DataMember] public bool IsCreateDtoFactory { get; set; }

        [DataMember] public bool IsCreateGateway { get; set; }

        [DataMember] public bool IsCreateBusinessService { get; set; }

        [DataMember] public bool IsUseBusinessServiceWithoutBo { get; set; }

        [DataMember] public bool IsCreateDataItem { get; set; }

        [DataMember] public bool IsCreateDataItemFactory { get; set; }

        [DataMember] public bool IsCreateRepositoryDtoFactory { get; set; }

        [DataMember] public bool IsCreateRepository { get; set; }

        [DataMember] public bool IsCreateUi { get; set; }

        [DataMember] public bool IsCreateUiFilter { get; set; }

        [DataMember] public string Product { get; set; }

        [DataMember] public string DialogName { get; set; }

        [DataMember] public string DialogTranslationGerman { get; set; }

        [DataMember] public string DialogTranslationEnglish { get; set; }

        [DataMember] public string ControllerHandle { get; set; }

        [DataMember] public IList<ConfigurationItem> DataLayout { get; set; }
    }
}