using System.Runtime.Serialization;

namespace PlusLayerCreator.Model
{
    [DataContract]
    public class DirectHopItemDto
    {
        
        private string _controllerHandle;
        private string _dialogName;
        private string _localizationKey;
        private string _product;
        private int _order;

        [DataMember]
        public string ControllerHandle
        {
	        get; set;
        }

		[DataMember]
        public string DialogName
		{
			get; set;
		}

		[DataMember]
        public string LocalizationKey
		{
			get; set;
		}

		[DataMember]
        public string Product
		{
			get; set;
		}

		[DataMember]
        public int Order
		{
			get; set;
		}
	}
}