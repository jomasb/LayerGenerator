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
		public ObservableCollection<PlusDataItemProperty> Properties
		{
			get;
			set;
		}
	}
}
