using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using System.Text.RegularExpressions;

namespace WindowsGSM.Plugins
{
	public class Starbound : SteamCMDAgent
	{
		public Plugin Plugin = new Plugin
		{
			name = "WindowsGSM.Starbound",
			author = "ScoWalt",
			description = "🧩 WindowsGSM plugin for Starbound Dedicated Server",
			version = "0.1",
			url = "https://github.com/scowalt/WindowsGSM.Starbound",
			color = "#221868"
		};

		public Starbound(ServerConfig serverData) : base(serverData) => base.serverData = _serverData = serverData;
		private readonly ServerConfig _serverData;

		// Properties for SteamCMD installer
		public override bool loginAnonymous => false; // Starbound Dedicated Server does NOT allow anonymous login. See https://developer.valvesoftware.com/wiki/Dedicated_Servers_List
		public override string AppId => "533830"; // https://steamdb.info/app/533830/

		// Default server config file for Starbound
		private static string defaultConfig => @"{
  ""allowAdminCommands"" : true,
  ""allowAdminCommandsFromAnyone"" : false,
  ""allowAnonymousConnections"" : true,
  ""allowAssetsMismatch"" : true,
  ""anonymousConnectionsAreAdmin"" : false,
  ""bannedIPs"" : [],
  ""bannedUuids"" : [],
  ""checkAssetsDigest"" : false,
  ""clearPlayerFiles"" : false,
  ""clearUniverseFiles"" : false,
  ""clientIPJoinable"" : false,
  ""clientP2PJoinable"" : true,
  ""configurationVersion"" : {
    ""basic"" : 2,
    ""server"" : 4
  },
  ""crafting"" : {
    ""filterHaveMaterials"" : false
  },
  ""gameServerBind"" : ""*"",
  ""gameServerPort"" : 21025,
  ""interactiveHighlight"" : true,
  ""inventory"" : {
    ""pickupToActionBar"" : true
  },
  ""maxPlayers"" : 8,
  ""maxTeamSize"" : 4,
  ""monochromeLighting"" : false,
  ""playerBackupFileCount"" : 3,
  ""queryServerBind"" : ""*"",
  ""queryServerPort"" : 21025,
  ""rconServerBind"" : ""*"",
  ""rconServerPassword"" : """",
  ""rconServerPort"" : 21026,
  ""rconServerTimeout"" : 1000,
  ""runQueryServer"" : false,
  ""runRconServer"" : false,
  ""safeScripts"" : true,
  ""scriptInstructionLimit"" : 10000000,
  ""scriptInstructionMeasureInterval"" : 10000,
  ""scriptProfilingEnabled"" : false,
  ""scriptRecursionLimit"" : 100,
  ""serverFidelity"" : ""automatic"",
  ""serverName"" : ""A Starbound Server"",
  ""serverOverrideAssetsDigest"" : null,
  ""serverUsers"" : {
  },
  ""tutorialMessages"" : true
}";

		// Standard variables
		public override string StartPath => "win64/starbound_server.exe";
		public string FullName => "Starbound Dedicated Server";
		public bool AllowsEmbedConsole = true; // Does this server support output redirect?
		public int PortIncrements = 1; // This tells WindowsGSM how many ports should skip after installation
		public object QueryMethod = null;

		public string Port = "21025"; // Default port
		public string QueryPort = "21025"; // Default Query Port
		public string Defaultmap = ""; // Default map name (arbitrary for Starbound)
		public string Maxplayers = "8"; // Default maxplayers
		public string Additional = ""; // Additional server start parameter

		// Standard functions
		public async void CreateServerCFG()
		{
			// Make sure the "storage" directory exists (which contains the server config
			Directory.CreateDirectory(ServerPath.GetServersServerFiles(_serverData.ServerID, "storage"));

			// If no config exists, write the default config to the correct location
			var configFilePath = ServerPath.GetServersServerFiles(_serverData.ServerID, "storage", "starbound_server.config");
			if (!File.Exists(configFilePath))
            {
				File.WriteAllText(configFilePath, defaultConfig);
            }

			// Update the config file on disk with information from WindowsGSM
			var configFileContents = File.ReadAllText(configFilePath);
			configFileContents = Regex.Replace(configFileContents, "\"gameServerPort\" : [0-9]*,", $@"""gameServerPort"" : {Port},");
			configFileContents = Regex.Replace(configFileContents, "\"queryServerPort\" : [0-9]*,", $@"""queryServerPort"" : {QueryPort},");
			configFileContents = Regex.Replace(configFileContents, "\"maxPlayers\" : [0-9]*,", $@"""maxPlayers"" : {Maxplayers},");
			File.WriteAllText(configFilePath, configFileContents);
		}

		public async Task<Process> Start()
		{
			var p = new Process
			{
				StartInfo =
				{
					WindowStyle = ProcessWindowStyle.Minimized,
					UseShellExecute = false,
					WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID, "win64"),
					FileName = ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath),
					Arguments = ""
				},
				EnableRaisingEvents = true
			};

			try
			{
				p.Start();
				return p;
			}
			catch (System.Exception e)
			{
				base.Error = e.Message;
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
