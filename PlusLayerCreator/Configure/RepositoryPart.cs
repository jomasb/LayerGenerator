using System.IO;
using System.Linq;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Configure
{
	public class RepositoryPart
	{
		private Configuration _configuration;
		private string _createRepositoryDtoTemplateUpperPart;
		private string _createRepositoryDtoTemplateLowerPart;
		private string _createDataItemTemplateUpperPart;
		private string _createDataItemTemplateLowerPart;

		public RepositoryPart(Configuration configuration)
		{
			_configuration = configuration;
			_createDataItemTemplateUpperPart = File.ReadAllText(configuration.InputPath + @"Repository\CreateDataItemUpperPart.txt");
			_createDataItemTemplateLowerPart = File.ReadAllText(configuration.InputPath + @"Repository\CreateDataItemLowerPart.txt");
			_createRepositoryDtoTemplateUpperPart = File.ReadAllText(configuration.InputPath + @"Repository\CreateDtoUpperPart.txt");
			_createRepositoryDtoTemplateLowerPart = File.ReadAllText(configuration.InputPath + @"Repository\CreateDtoLowerPart.txt");
		}

		#region Repository

		public void CreateRepository()
		{
			string repositoryServiceMemberContent = string.Empty;
			string repositoryServiceParameterContent = string.Empty;
			string repositoryServiceConstructorContent = string.Empty;

			string interfaceContent = string.Empty;
			string repositoryContent = string.Empty;

			foreach (ConfigurationItem dataItem in _configuration.DataLayout)
			{
				repositoryServiceMemberContent += "private readonly I" + _configuration.Product + dataItem.Name + "Service _" + Helpers.ToPascalCase(_configuration.Product + dataItem.Name + "Service;\r\n");
				if (repositoryServiceParameterContent != string.Empty)
				{
					repositoryServiceParameterContent = repositoryServiceParameterContent + ", ";
				}
				repositoryServiceParameterContent += "I" + _configuration.Product + dataItem.Name + "Service " + Helpers.ToPascalCase(_configuration.Product) + dataItem.Name + "Service";
				repositoryServiceConstructorContent += "_" + Helpers.ToPascalCase(_configuration.Product) + dataItem.Name + "Service = " + Helpers.ToPascalCase(_configuration.Product) + dataItem.Name + "Service;\r\n";

				interfaceContent += "#region " + dataItem.Name + "\r\n\r\n";
				repositoryContent += "#region " + dataItem.Name + "\r\n\r\n";

				string identifier = GetIdentifier(dataItem);
				string readOnly = GetReadOnly(dataItem);

				if (dataItem.CanRead)
				{
					if (string.IsNullOrEmpty(dataItem.Parent))
					{
						if (_configuration.DataLayout.Any(t => t.Parent == dataItem.Name))
						{
							//parent
							interfaceContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\GetParentPart.txt"), dataItem.Name) + "\r\n\r\n";

							string childDataItem = _configuration.DataLayout.FirstOrDefault(t => t.Parent == dataItem.Name).Name;
							string getChild = childDataItem +
											  "s = new PlusLazyLoadingAsyncObservableCollection<" + _configuration.Product + childDataItem + "DataItem>(" +
											  "() => Get" + childDataItem + "sAsync(callContext, " + Helpers.ToPascalCase(dataItem.Name) + "DataItem))" +
											  "{ AutoAcceptAfterSuccessfullyLoading = true, AutoOverwriteParent = true };";

							string content = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\GetParentPart.txt"), dataItem.Name) +
											 "\r\n\r\n";
							content = Helpers.ReplaceSpecialContent(content, new[] { getChild });
							repositoryContent += content;
						}
						else
						{
							//single
							interfaceContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\GetPart.txt"), dataItem.Name) + "\r\n\r\n";
							repositoryContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\GetPart.txt"), dataItem.Name) + "\r\n\r\n";
						}
					}
					else
					{
						//child
						string transformParent = "_" + Helpers.ToPascalCase(_configuration.Product) + "DtoFactory.Create" + dataItem.Parent + "FromDataItem";
						string content = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\GetChildPart.txt"), dataItem.Name) +
										 "\r\n\r\n";
						content = Helpers.ReplaceSpecialContent(content, new[] { _configuration.Product + dataItem.Parent });
						interfaceContent += content;

						content = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\GetChildPart.txt"), dataItem.Name) +
								  "\r\n\r\n";
						content = Helpers.ReplaceSpecialContent(content, new[] { _configuration.Product + dataItem.Parent, transformParent });
						repositoryContent += content;
					}
				}

				if (dataItem.CanClone)
				{
					interfaceContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\ClonePart.txt"), dataItem.Name) + "\r\n\r\n";
					repositoryContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\ClonePart.txt"), dataItem.Name) + "\r\n\r\n";
				}

				if (dataItem.CanDelete)
				{
					interfaceContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\DeletePart.txt"), dataItem.Name) + "\r\n\r\n";
					repositoryContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\DeletePart.txt"), dataItem.Name) + "\r\n\r\n";
				}

				if (dataItem.CanSort)
				{
					interfaceContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\SortPart.txt"), dataItem.Name) + "\r\n\r\n";
					repositoryContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\SortPart.txt"), dataItem.Name) + "\r\n\r\n";
				}
				if (dataItem.CanEdit)
				{
					interfaceContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\AddPart.txt"), dataItem.Name) + "\r\n\r\n";
					repositoryContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\AddPart.txt"), dataItem.Name) + "\r\n\r\n";

					if (!dataItem.CanEditMultiple)
					{
						if (string.IsNullOrEmpty(dataItem.Parent))
						{
							interfaceContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\SavePart.txt"), dataItem.Name) + "\r\n\r\n";
							string content = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\SavePart.txt"), dataItem.Name) + "\r\n\r\n";
							content = Helpers.ReplaceSpecialContent(content, new[] { identifier, readOnly });
							repositoryContent += content;
						}
					}
					if (dataItem.CanEditMultiple)
					{
						if (string.IsNullOrEmpty(dataItem.Parent))
						{
							interfaceContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\Contracts\SaveMultiPart.txt"), dataItem.Name) + "\r\n\r\n";
							string content = Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\SaveMultiPart.txt"), dataItem.Name) + "\r\n\r\n";
							content = Helpers.ReplaceSpecialContent(content, new[] { identifier, readOnly });
							repositoryContent += content;
						}
					}
				}

				interfaceContent += "#endregion " + dataItem.Name + "\r\n\r\n";
				repositoryContent += "#endregion " + dataItem.Name + "\r\n\r\n";
			}

			string dtoLayer = _configuration.IsUseBusinessServiceWithoutBo ? "BusinessServiceLocal" : "Gateway";

			Helpers.CreateFile(_configuration.InputPath + @"Repository\Contracts\IRepositoryTemplate.cs", _configuration.OutputPath + @"Repository\Contracts\I" + _configuration.Product + _configuration.DialogName + "Repository.cs", new[] { interfaceContent });
			Helpers.CreateFile(_configuration.InputPath + @"Repository\RepositoryTemplate.cs", _configuration.OutputPath + @"Repository\" + _configuration.Product + _configuration.DialogName + "Repository.cs", new[] { repositoryServiceMemberContent, repositoryServiceParameterContent, repositoryServiceConstructorContent, repositoryContent, dtoLayer });
		}

		private string GetIdentifier(ConfigurationItem dataItem)
		{
			string identifier = string.Empty;
			foreach (ConfigurationProperty plusDataObject in dataItem.Properties.Where(t => t.IsKey))
			{
				if (identifier != string.Empty)
				{
					identifier = " && " + identifier;
				}

				identifier += "x." + plusDataObject.Name + ".Equals(dto." + plusDataObject.Name + ")";
			}

			return identifier;
		}

		private string GetReadOnly(ConfigurationItem dataItem)
		{
			string readOnly = string.Empty;
			foreach (ConfigurationProperty plusDataObject in dataItem.Properties.Where(t => t.IsKey))
			{
				if (plusDataObject.IsReadOnly)
				{
					readOnly +=
						Helpers.ToPascalCase(dataItem.Name) + "Dto." + plusDataObject.Name + " = " + Helpers.ToPascalCase(dataItem.Name) + "." +
						plusDataObject.Name + ";\r\n";
				}
			}

			return readOnly;
		}

		public void CreateDataItem()
		{
			foreach (ConfigurationItem dataItem in _configuration.DataLayout)
			{
				string dataItemContent = string.Empty;

				if (string.IsNullOrEmpty(dataItem.Parent))
				{
					foreach (ConfigurationItem preFilterDataItem in _configuration.DataLayout.Where(t => t.IsPreFilterItem && t.Properties.Any(z => z.Name == "Id")))
					{
						var pp = preFilterDataItem.Properties.FirstOrDefault(t => t.Name == "Id");
						if (pp != null)
						{
							dataItemContent += "public " + pp.Type + " " + pp.Name + "\r\n" +
							                   "    {get\r\n" +
							                   "    {\r\n" +
											   "        return Get<" + pp.Type + ">();\r\n" +
							                   "    }\r\n" +
							                   "    set\r\n" +
							                   "    {\r\n" +
											   "        Set<" + pp.Type + ">(value);\r\n" +
							                   "    }}\r\n\r\n";
						}
					}
				}

				foreach (ConfigurationProperty plusDataObject in dataItem.Properties)
				{
					if (plusDataObject.IsRequired || plusDataObject.Length != string.Empty)
					{
						dataItemContent += "[";
						if (plusDataObject.IsRequired)
						{
							dataItemContent += "Required";
						}

						if (plusDataObject.IsRequired || plusDataObject.Length != string.Empty)
						{
							dataItemContent += ", ";
						}

						if (!string.IsNullOrEmpty(plusDataObject.Length))
						{
							if (plusDataObject.Type == "int")
							{
								dataItemContent += "NumericRange(0, " + Helpers.GetMaxValue(plusDataObject.Length) + ")";
							}
							if (plusDataObject.Type == "string")
							{
								dataItemContent += "MaxLength(" + plusDataObject.Length + ")";
							}

						}
						dataItemContent += "]\r\n";
					}
					dataItemContent += "public " + plusDataObject.Type + " " + plusDataObject.Name + "\r\n" +
									   "    {get\r\n" +
									   "    {\r\n" +
									   "        return Get<" + plusDataObject.Type + ">();\r\n" +
									   "    }\r\n" +
									   "    set\r\n" +
									   "    {\r\n" +
									   "        Set<" + plusDataObject.Type + ">(value);\r\n" +
									   "    }}\r\n\r\n";

					foreach (ConfigurationItem childDataItem in _configuration.DataLayout.Where(t => t.Parent == dataItem.Name))
					{
						dataItemContent += Helpers.DoReplaces(File.ReadAllText(_configuration.InputPath + @"Repository\DataItems\DataItemCollectionPart.txt"), childDataItem.Name) + "\r\n";
					}
				}

				Helpers.CreateFile(_configuration.InputPath + @"Repository\DataItems\DataItemTemplate.cs",
					_configuration.OutputPath + @"Repository\DataItems\" + _configuration.Product + dataItem.Name + "DataItem.cs", new[] { dataItemContent }, dataItem.Name);
			}
		}

		public void CreateRepositoryDtoFactory()
		{
			string factoryContent = string.Empty;
			foreach (ConfigurationItem dataItem in _configuration.DataLayout)
			{
				factoryContent += Helpers.DoReplaces(_createRepositoryDtoTemplateUpperPart + "\r\n", dataItem.Name);
				foreach (ConfigurationProperty plusDataObject in dataItem.Properties)
				{
					factoryContent += plusDataObject.Name + " = dataItem." + plusDataObject.Name + ",\r\n";
				}
				factoryContent += Helpers.DoReplaces(_createRepositoryDtoTemplateLowerPart, dataItem.Name) + "\r\n";
			}

			string dtoLayer = _configuration.IsUseBusinessServiceWithoutBo ? "BusinessServiceLocal" : "Gateway";

			Helpers.CreateFile(_configuration.InputPath + @"Repository\DtoFactoryTemplate.cs", _configuration.OutputPath + @"Repository\" + _configuration.Product + "DtoFactory.cs",
				new[] { factoryContent, dtoLayer });
		}

		public void CreateDataItemFactory()
		{
			string factoryContent = string.Empty;
			foreach (ConfigurationItem dataItem in _configuration.DataLayout)
			{
				factoryContent += Helpers.DoReplaces(_createDataItemTemplateUpperPart + "\r\n", dataItem.Name);
				foreach (ConfigurationProperty plusDataObject in dataItem.Properties)
				{
					factoryContent += plusDataObject.Name + " = dto." + plusDataObject.Name + ",\r\n";
				}
				factoryContent += Helpers.DoReplaces(_createDataItemTemplateLowerPart, dataItem.Name) + "\r\n";
			}

			string dtoLayer = _configuration.IsUseBusinessServiceWithoutBo ? "BusinessServiceLocal" : "Gateway";

			Helpers.CreateFile(_configuration.InputPath + @"Repository\DataItemFactoryTemplate.cs",
				_configuration.OutputPath + @"Repository\" + _configuration.Product + "DataItemFactory.cs", new[] { factoryContent, dtoLayer });
		}

		#endregion Repository

	}
}
