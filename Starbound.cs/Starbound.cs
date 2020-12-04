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
		public override string StartPath => "win64/starbound_server.exe";
		public string FullName => "Starbound Dedicated Server";
		public bool AllowsEmbedConsole = true; // TODO ScoWalt is this right? // Does this server support output redirect?
		public int PortIncrements = 1; // This tells WindowsGSM how many ports should skip after installation
		public object QueryMethod = null;

		public string Port = "21025"; // Default port
		public string QueryPort = "21025"; // Default Query Port
		public string Defaultmap = "empty"; // Default map name (arbitrary for Starbound)
		public string Maxplayers = "8"; // Default maxplayers
		public string Additional = ""; // Additional server start parameter

		// Standard functions
		public async void CreateServerCFG() { /* Starbound creates its own default server config on first launch */ }
		public async Task<Process> Start()
		{
			var p = new Process
			{
				StartInfo =
				{
					WindowStyle = ProcessWindowStyle.Minimized,
					UseShellExecute = false,
					WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID),
					FileName = ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath),
					Arguments = ""
				},
				EnableRaisingEvents = true
			}

			try
			{
				p.Start();
				return p;
			}
			catch (Exception)
			{
				base.Error = e.message;
				return null;
			}
		}
		public async Task Stop(Process p)
		{
			// Starbound Dedicated Server doesn't appear to have a proper shutdown
			await Task.Run(() => { p.Kill(); });
		}
	}
}
