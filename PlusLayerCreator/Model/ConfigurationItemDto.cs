using System.Collections.Generic;
using System.Runtime.Serialization;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Model
{
    [DataContract]
    public class ConfigurationItemDto
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
        public string Translation
		{
			get; set;
		}

		[DataMember]
        public string Parent
		{
			get; set;
		}

		[DataMember]
        public bool IsPreFilterItem
		{
			get; set;
		}

		[DataMember]
        public bool IsDetailComboBoxItem
		{
			get; set;
		}

		[DataMember]
        public bool CanEdit
		{
			get; set;
		}

		[DataMember]
        public bool CanEditMultiple
		{
			get; set;
		}

		[DataMember]
        public bool CanDelete
		{
			get; set;
		}

		[DataMember]
        public bool CanClone
		{
			get; set;
		}

		[DataMember]
        public bool CanSort
		{
			get; set;
		}

		[DataMember]
        public string Requirement
		{
			get; set;
		}

		[DataMember]
        public string Sys
		{
			get; set;
		}

		[DataMember]
        public string Server
		{
			get; set;
		}

		[DataMember]
        public string Table
		{
			get; set;
		}

		[DataMember]
        public string TableDescription
		{
			get; set;
		}

		[DataMember]
        public string ReqRplRead
		{
			get; set;
		}

		[DataMember]
        public string ReqRplWrite
		{
			get; set;
		}

		[DataMember]
        public string TransactionCodeRead
		{
			get; set;
		}

		[DataMember]
        public string TransactionCodeWrite
		{
			get; set;
		}

		[DataMember]
        public IList<ConfigurationPropertyDto> Properties
		{
			get; set;
		}
	}
}