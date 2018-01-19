using System.Runtime.Serialization;

namespace PlusLayerCreator.Items
{
	[DataContract]
	public enum TemplateMode
	{
		ReadOnly,
		Edit,
		EditMulti
	}
}
