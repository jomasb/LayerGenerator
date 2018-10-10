using System.Runtime.Serialization;

namespace PlusLayerCreator.Model
{
    [DataContract]
    public class ConfigurationPropertyDto
    {
        [DataMember]
        public string Name
        {
	        get; set;
        }

		[DataMember]
        public int Order
		{
			get; set;
		}

		[DataMember]
        public string TranslationDe
		{
			get; set;
		}
		[DataMember]
        public string TranslationEn
		{
			get; set;
		}

		[DataMember]
        public string Type
		{
			get; set;
		}

		[DataMember]
        public string ComboBoxItem
		{
			get; set;
		}

		[DataMember]
        public string Length
		{
			get; set;
		}

		[DataMember]
        public bool IsReadOnly
		{
			get; set;
		}

		[DataMember]
        public bool IsRequired
		{
			get; set;
		}

		[DataMember]
        public bool IsKey
		{
			get; set;
		}

		[DataMember]
        public bool IsVisibleInGrid
		{
			get; set;
		}

		[DataMember]
        public bool IsFilterProperty
		{
			get; set;
		}

		[DataMember]
        public string FilterPropertyType
		{
			get; set;
		}

		[DataMember]
        public bool ShouldLazyLoad
		{
			get; set;
		}

		[DataMember]
        public string MessageField
		{
			get; set;
		}

		[DataMember]
        public string MessageDataType
		{
			get; set;
		}
	}
}