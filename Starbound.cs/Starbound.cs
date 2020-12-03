using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;

namespace WindowsGSM.Plugins
{
	public class Starbound : SteamCMDAgent
	{
		public Plugin Plugin = new Plugin
		{
			name = "WindowsGSM.Starbound",
			author = "ScoWalt",
			description = "🧩 WindowsGSM plugin for Starbound Dedicated Server"
			version = "0.1",
			url = "https://github.com/scowalt/WindowsGSM.Starbound",
			color = "#221868"
		}

		public Starbound(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
		private readonly ServerConfig _serverData;

		// Properties for SteamCMD installer
		public override bool loginAnonymous => true; // Starbound Dedicated Server does NOT allow anonymous login. See https://developer.valvesoftware.com/wiki/Dedicated_Servers_List
		public override string AppId => "533830"; // https://steamdb.info/app/533830/

		// Standard variables

	}
}
