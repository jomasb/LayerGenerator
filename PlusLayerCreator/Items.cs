using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlusLayerCreator
{
	public class PlusDataItem
	{
		public ObservableCollection<PlusDataItemProperty> Properties
		{
			get;
			set;
		}
	}

	public class PlusDataItemProperty
	{
		public string Name
		{
			get;
			set;
		}
		public string Type
		{
			get;
			set;
		}
		public string Length
		{
			get;
			set;
		}
		public bool IsReadOnly
		{
			get;
			set;
		}
		public bool IsRequired
		{
			get;
			set;
		}
		public bool IsKey
		{
			get;
			set;
		}
		public bool IsFilterProperty
		{
			get;
			set;
		}
		public string FilterPropertyType
		{
			get;
			set;
		}
	}

	public class Configuration
	{
		public bool IsCreateDto
		{
			get;
			set;
		}
		public bool IsCreateDtoFactory
		{
			get;
			set;
		}
		public bool IsCreateGateway
		{
			get;
			set;
		}
		public bool IsCreateBusinessService
		{
			get;
			set;
		}
		public bool IsUseBusinessServiceWithoutBO
		{
			get;
			set;
		}
		public bool IsCreateDataItem
		{
			get;
			set;
		}
		public bool IsCreateDataItemFactory
		{
			get;
			set;
		}
		public bool IsCreateRepositoryDtoFactory
		{
			get;
			set;
		}
		public bool IsCreateRepository
		{
			get;
			set;
		}
		public bool IsCreateUi
		{
			get;
			set;
		}
		public bool IsCreateUiFilter
		{
			get;
			set;
		}
		public string Product
		{
			get;
			set;
		}
		public string Item
		{
			get;
			set;
		}
		public string DialogName
		{
			get;
			set;
		}
		public IList<PlusDataItemProperty> DataLayout
		{
			get;
			set;
		}
	}
}
