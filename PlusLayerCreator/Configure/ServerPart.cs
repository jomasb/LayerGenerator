using System.IO;
using PlusLayerCreator.Items;

namespace PlusLayerCreator.Configure
{
	public class ServerPart
	{
		private readonly Configuration _configuration;
		private readonly string _serverPart;
		private readonly string _dlePart;
		private readonly string _messagesPart;

		public ServerPart(Configuration configuration)
		{
			_configuration = configuration;
			_serverPart = File.ReadAllText(configuration.InputPath + @"Server\Server.tpl");
			_dlePart = File.ReadAllText(configuration.InputPath + @"Server\DLE.tpl");
			_messagesPart = File.ReadAllText(configuration.InputPath + @"Server\Messages.tpl");
		}

		public void Create()
		{
			string serverContent = string.Empty;
			string dleContent = string.Empty;
			string messagesContent = string.Empty;

			foreach (ConfigurationItem item in _configuration.DataLayout)
			{
				if (!string.IsNullOrEmpty(item.Server))
				{
					serverContent = _serverPart.DoReplacesServer(item);
					dleContent = _dlePart.DoReplacesServer(item);
					messagesContent = _messagesPart.DoReplacesServer(item);
				}

				if (!string.IsNullOrEmpty(item.Server))
				{
					Helpers.CreateFileFromString(serverContent, _configuration.OutputPath + @"Server\" + item.Server + "01");
					Helpers.CreateFileFromString(dleContent, _configuration.OutputPath + @"Server\" + item.Server);
					Helpers.CreateFileFromString(messagesContent, _configuration.OutputPath + @"Server\ME" + item.Server);
				}
			}
		}
	}
}
