using System;
using System.Collections.Generic;
using OpenMetaverse;

namespace Voxel
{
	public class CommandRouter
	{
		private Dictionary<string, ICommandHandler> cmdDictionary;

		public CommandRouter ()
		{
			cmdDictionary = new Dictionary<string, ICommandHandler> ();
		}

		public void Add(string name, ICommandHandler cmd)
		{
			cmdDictionary.Add (name, cmd);
		}

		public bool Handle(UUID user, string cmdStr)
		{
			Voxel.Log.Info ("[Voxel] Routing command.");
			char[] splitTokens = {' '};
			string[] args = cmdStr.Split(splitTokens);

			ICommandHandler cmd;
			if (!cmdDictionary.TryGetValue(args[0], out cmd)) {
				Voxel.Log.Info(string.Format("[Voxel] Command '{0}' not found.", args[0]));
				return false;
			}

			return cmd.TryExecute(new Command(user, args));
		}
	}
}

