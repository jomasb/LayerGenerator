using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace PlusLayerCreator.Items
{
	[DataContract]
	public class PlusDataItem
	{
		[DataMember]
		public string Name
		{
			get;
			set;
		}

		[DataMember]
		public string Translation
		{
			get;
			set;
		}

		[DataMember]
		public string Parent
		{
			get;
			set;
		}

		[DataMember]
		public bool IsPreFilterItem { get; set; }

		[DataMember]
		public bool CanRead { get; set; }

		[DataMember]
		public bool CanEdit { get; set; }

		[DataMember]
		public bool CanEditMultiple { get; set; }

		[DataMember]
		public bool CanDelete { get; set; }

		[DataMember]
		public bool CanClone { get; set; }

		[DataMember]
		public bool CanSort { get; set; }

		[DataMember]
		public ObservableCollection<PlusDataItemProperty> Properties
		{
			get;
			set;
		}
	}
}
