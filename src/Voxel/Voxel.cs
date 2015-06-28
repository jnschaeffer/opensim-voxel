using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using log4net;
using Nini.Config;
using OpenMetaverse;
using Mono.Addins;
using OpenSim.Framework;
using OpenSim.Framework.Console;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.Framework.Scenes;

[assembly: Addin ("Voxel", OpenSim.VersionInfo.VersionNumber + ".1")]
[assembly: AddinDependency ("OpenSim.Region.Framework", OpenSim.VersionInfo.VersionNumber)]
[assembly: AddinDescription("Collaborative voxel-based building game")]
[assembly: AddinAuthor("Computer User")]
namespace Voxel
{
	[Extension(Path = "/OpenSim/RegionModules", NodeName = "RegionModule", Id="Voxel")]
	public class Voxel : INonSharedRegionModule
	{

		public static readonly ILog Log = LogManager.GetLogger (MethodBase.GetCurrentMethod ().DeclaringType);
		private IConfigSource configSource;
		private Scene scene;
		private int channel;
		private CommandRouter cmdHandler;

		public Voxel ()
		{
			cmdHandler = new CommandRouter ();
		}

		public Type ReplaceableInterface {
			get { return null; }
		}

		public string Name {
			get { return "Voxel"; }
		}

		public void Initialise (IConfigSource config)
		{
			Log.Info ("[Voxel] Module loaded");
			configSource = config;
			IConfig helloConfig = configSource.Configs ["Voxel"];
			if (helloConfig != null) {
				channel = helloConfig.GetInt ("Channel", 10);
				Log.Info (string.Format("[Voxel] Listening on channel {0}", channel));
			} else {
				channel = 10;
			}
		}

		public void Close ()
		{
			Log.Info ("[Voxel] Closing Hello World.");
		}

		public void AddRegion (Scene s)
		{
			scene = s;

			// Initialize command handler before adding event handlers
			UserPrimManager userPrimManager = new UserPrimManager (scene);

			cmdHandler.Add ("create", new CreateHandler (userPrimManager));
			cmdHandler.Add ("undo", new UndoHandler (userPrimManager));

			scene.EventManager.OnChatFromClient += SimChatEventHandler;
			scene.EventManager.OnChatFromWorld += SimChatEventHandler;
		}

		public void RemoveRegion (Scene s)
		{
		}

		public void RegionLoaded (Scene s)
		{
		}

		public void SimChatEventHandler (object sender, OSChatMessage msg)
		{
			UUID msgSender = msg.SenderUUID;
			int msgChannel = msg.Channel;
			string text = msg.Message;

			Log.Info (string.Format ("[Voxel] Received command on channel {0} from {1} {2}: {3}", msgChannel, msg.Sender.FirstName, msg.Sender.LastName, text));
			if (msgChannel == channel && text != "") {
				Log.Info ("[Voxel] Handling command.");
				if (!cmdHandler.Handle (msgSender, text)) {
					Log.Info (string.Format ("[Voxel] Command failed: {0}", text));
				}
			}
		}
	}
}

