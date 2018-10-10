using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PlusLayerCreator.Items;
using PlusLayerCreator.Model;

namespace PlusLayerCreator
{
	public static class ItemFactory
	{
		#region Items

		public static Configuration GetConfigurationFromDto(ConfigurationDto dto)
		{
			Configuration item = new Configuration()
			{
				InputPath = dto.InputPath,
				OutputPath = dto.OutputPath,
				IsCreateDto = dto.IsCreateDto,
				IsCreateDtoFactory = dto.IsCreateDtoFactory,
				IsCreateGateway = dto.IsCreateGateway,
				IsCreateBusinessService = dto.IsCreateBusinessService,
				IsUseBusinessServiceWithoutBo = dto.IsUseBusinessServiceWithoutBo,
				IsCreateDataItem = dto.IsCreateDataItem,
				IsCreateDataItemFactory = dto.IsCreateDataItemFactory,
				IsCreateRepositoryDtoFactory = dto.IsCreateRepositoryDtoFactory,
				IsCreateRepository = dto.IsCreateRepository,
				IsCreateUi = dto.IsCreateUi,
				IsCreateUiFilter = dto.IsCreateUiFilter,
				Product = dto.Product,
				DialogName = dto.DialogName,
				DialogTranslationGerman = dto.DialogTranslationGerman,
				DialogTranslationEnglish = dto.DialogTranslationEnglish,
				ControllerHandle = dto.ControllerHandle,
			};

			item.DataLayout = new ObservableCollection<ConfigurationItem>();
			foreach (ConfigurationItemDto configurationItem in dto.DataLayout.OrderBy(t => t.Order))
			{
				item.DataLayout.Add(GetConfigurationItemFromDto(configurationItem));
			}

			item.DirectHops = new ObservableCollection<DirectHopItem>();
			foreach (DirectHopItemDto directHop in dto.DirectHops.OrderBy(t => t.Order))
			{
				item.DirectHops.Add(GetDirectHopItemFromDto(directHop));
			}

			return item;
		}

		public static ConfigurationItem GetConfigurationItemFromDto(ConfigurationItemDto dto)
		{
			ConfigurationItem item = new ConfigurationItem()
			{
				Order = dto.Order,
				Name = dto.Name,
				Translation = dto.Translation,
				Parent = dto.Parent,
				IsPreFilterItem = dto.IsPreFilterItem,
				IsDetailComboBoxItem = dto.IsDetailComboBoxItem,
				CanEdit = dto.CanEdit,
				CanEditMultiple = dto.CanEditMultiple,
				CanDelete = dto.CanDelete,
				CanClone = dto.CanClone,
				CanSort = dto.CanSort,
				Requirement = dto.Requirement,
				Sys = dto.Sys,
				Server = dto.Server,
				Table = dto.Table,
				TableDescription = dto.TableDescription,
				ReqRplRead = dto.ReqRplRead,
				ReqRplWrite = dto.ReqRplWrite,
				TransactionCodeRead = dto.TransactionCodeRead,
				TransactionCodeWrite = dto.TransactionCodeWrite
			};

			item.Properties = new ObservableCollection<ConfigurationProperty>();
			foreach (ConfigurationPropertyDto configurationPropertyDto in dto.Properties.OrderBy(t => t.Order))
			{
				item.Properties.Add(GetConfigurationPropertyFromDto(configurationPropertyDto));
			}

			return item;
		}

		public static ConfigurationProperty GetConfigurationPropertyFromDto(ConfigurationPropertyDto dto)
		{
			return new ConfigurationProperty()
			{
				Order = dto.Order,
				Name = dto.Name,
				TranslationDe = dto.TranslationDe,
				TranslationEn = dto.TranslationEn,
				Type = dto.Type,
				ComboBoxItem = dto.ComboBoxItem,
				FilterPropertyType = dto.FilterPropertyType,
				IsFilterProperty = dto.IsFilterProperty,
				IsKey = dto.IsKey,
				IsVisibleInGrid = dto.IsVisibleInGrid,
				IsRequired = dto.IsRequired,
				IsReadOnly = dto.IsReadOnly,
				Length = dto.Length,
				ShouldLazyLoad = dto.ShouldLazyLoad,
				MessageField = dto.MessageField,
				MessageDataType = dto.MessageDataType
			};
		}

		public static DirectHopItem GetDirectHopItemFromDto(DirectHopItemDto dto)
		{
			return new DirectHopItem()
			{
				ControllerHandle = dto.ControllerHandle,
				DialogName = dto.DialogName,
				LocalizationKey = dto.LocalizationKey,
				Order = dto.Order,
				Product = dto.Product
			};
		}

		#endregion Items

		#region Dtos

		public static ConfigurationDto GetConfigurationFromItem(Configuration item)
		{
			ConfigurationDto dto = new ConfigurationDto()
			{
				InputPath = item.InputPath,
				OutputPath = item.OutputPath,
				IsCreateDto = item.IsCreateDto,
				IsCreateDtoFactory = item.IsCreateDtoFactory,
				IsCreateGateway = item.IsCreateGateway,
				IsCreateBusinessService = item.IsCreateBusinessService,
				IsUseBusinessServiceWithoutBo = item.IsUseBusinessServiceWithoutBo,
				IsCreateDataItem = item.IsCreateDataItem,
				IsCreateDataItemFactory = item.IsCreateDataItemFactory,
				IsCreateRepositoryDtoFactory = item.IsCreateRepositoryDtoFactory,
				IsCreateRepository = item.IsCreateRepository,
				IsCreateUi = item.IsCreateUi,
				IsCreateUiFilter = item.IsCreateUiFilter,
				Product = item.Product,
				DialogName = item.DialogName,
				DialogTranslationGerman = item.DialogTranslationGerman,
				DialogTranslationEnglish = item.DialogTranslationEnglish,
				ControllerHandle = item.ControllerHandle,
			};

			dto.DataLayout = new List<ConfigurationItemDto>();
			foreach (ConfigurationItem configurationItem in item.DataLayout.OrderBy(t => t.Order))
			{
				dto.DataLayout.Add(GetConfigurationItemFromItem(configurationItem));
			}

			dto.DirectHops = new List<DirectHopItemDto>();
			foreach (DirectHopItem directHop in item.DirectHops.OrderBy(t => t.Order))
			{
				dto.DirectHops.Add(GetDirectHopItemFromItem(directHop));
			}

			return dto;
		}
	
		public static ConfigurationItemDto GetConfigurationItemFromItem(ConfigurationItem item)
		{
			ConfigurationItemDto dto = new ConfigurationItemDto()
			{
				Order = item.Order,
				Name = item.Name,
				Translation = item.Translation,
				Parent = item.Parent,
				IsPreFilterItem = item.IsPreFilterItem,
				IsDetailComboBoxItem = item.IsDetailComboBoxItem,
				CanEdit = item.CanEdit,
				CanEditMultiple = item.CanEditMultiple,
				CanDelete = item.CanDelete,
				CanClone = item.CanClone,
				CanSort = item.CanSort,
				Requirement = item.Requirement,
				Sys = item.Sys,
				Server = item.Server,
				Table = item.Table,
				TableDescription = item.TableDescription,
				ReqRplRead = item.ReqRplRead,
				ReqRplWrite = item.ReqRplWrite,
				TransactionCodeRead = item.TransactionCodeRead,
				TransactionCodeWrite = item.TransactionCodeWrite
			};

			dto.Properties = new List<ConfigurationPropertyDto>();
			foreach (ConfigurationProperty configurationProperty in item.Properties.OrderBy(t => t.Order))
			{
				dto.Properties.Add(GetConfigurationPropertyFromItem(configurationProperty));
			}

			return dto;
		}

		public static ConfigurationPropertyDto GetConfigurationPropertyFromItem(ConfigurationProperty item)
		{
			return new ConfigurationPropertyDto()
			{
				Order = item.Order,
				Name = item.Name,
				TranslationDe = item.TranslationDe,
				TranslationEn = item.TranslationEn,
				Type = item.Type,
				ComboBoxItem = item.ComboBoxItem,
				FilterPropertyType = item.FilterPropertyType,
				IsFilterProperty = item.IsFilterProperty,
				IsKey = item.IsKey,
				IsVisibleInGrid = item.IsVisibleInGrid,
				IsRequired = item.IsRequired,
				IsReadOnly = item.IsReadOnly,
				Length = item.Length,
				ShouldLazyLoad = item.ShouldLazyLoad,
				MessageField = item.MessageField,
				MessageDataType = item.MessageDataType
			};
		}

		public static DirectHopItemDto GetDirectHopItemFromItem(DirectHopItem item)
		{
			return new DirectHopItemDto()
			{
				ControllerHandle = item.ControllerHandle,
				DialogName = item.DialogName,
				LocalizationKey = item.LocalizationKey,
				Order = item.Order,
				Product = item.Product
			};
		}

		#endregion Dtos
	}
}
