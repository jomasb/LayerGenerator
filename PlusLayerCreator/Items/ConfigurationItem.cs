using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace PlusLayerCreator.Items
{
	[DataContract]
	public class ConfigurationItem
	{
		[DataMember]
		public string Name
		{
			get;
			set;
		}

        [DataMember]
		public int Order
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
	    public string Server
	    {
	        get;
	        set;
	    }

	    [DataMember]
	    public string RepRplRead
	    {
	        get;
	        set;
	    }

	    [DataMember]
	    public string RepRplWrite
        {
	        get;
	        set;
	    }

	    [DataMember]
	    public string TransactionCodeRead
	    {
	        get;
	        set;
	    }

	    [DataMember]
	    public string TransactionCodeWrite
	    {
	        get;
	        set;
	    }

	    [DataMember]
	    public string TableCountProperty
	    {
	        get;
	        set;
	    }

        [DataMember]
		public ObservableCollection<ConfigurationProperty> Properties
		{
			get;
			set;
		}
	}
}
