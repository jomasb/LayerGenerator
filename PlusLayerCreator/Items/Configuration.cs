using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PlusLayerCreator.Items
{
	[DataContract]
	public class Configuration
	{
		[DataMember]
		public TemplateMode Template
		{
			get;
			set;
		}

		[DataMember]
		public string InputPath
		{
			get;
			set;
		}

		[DataMember]
		public string OutputPath
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCreateDto
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCreateDtoFactory
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCreateGateway
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCreateBusinessService
		{
			get;
			set;
		}

		[DataMember]
		public bool IsUseBusinessServiceWithoutBO
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCreateDataItem
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCreateDataItemFactory
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCreateRepositoryDtoFactory
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCreateRepository
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCreateUi
		{
			get;
			set;
		}

		[DataMember]
		public bool IsCreateUiFilter
		{
			get;
			set;
		}

		[DataMember]
		public string Product
		{
			get;
			set;
		}

		[DataMember]
		public string DialogName
		{
			get;
			set;
		}

		[DataMember]
		public string DialogTranslationDE
		{
			get;
			set;
		}

		[DataMember]
		public string DialogTranslationEN
		{
			get;
			set;
		}

		[DataMember]
		public IList<PlusDataItem> DataLayout
		{
			get;
			set;
		}
	}
}
