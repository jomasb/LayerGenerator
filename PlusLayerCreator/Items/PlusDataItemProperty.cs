using System.Runtime.Serialization;

namespace PlusLayerCreator.Items
{
	[DataContract]
	public class PlusDataItemProperty
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
		public string Type
		{
			get;
			set;
		}

		[DataMember]
		public string Length
		{
			get;
			set;
		}

		[DataMember]
		public bool IsReadOnly
		{
			get;
			set;
		}

		[DataMember]
		public bool IsRequired
		{
			get;
			set;
		}

		[DataMember]
		public bool IsKey
		{
			get;
			set;
		}

		[DataMember]
		public bool IsFilterProperty
		{
			get;
			set;
		}

		[DataMember]
		public string FilterPropertyType
		{
			get;
			set;
		}
	}	
}
