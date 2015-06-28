using System;
using System.Collections.Generic;
using OpenMetaverse;
using OpenSim.Region.Framework.Scenes;

namespace Voxel
{
	public class UserPrimManager
	{
		private Dictionary<UUID, PrimManager> lookup;
		private Scene scene;

		public UserPrimManager (Scene s)
		{
			scene = s;
			lookup = new Dictionary<UUID, PrimManager> ();
		}

		public PrimManager GetPrimManager(UUID user)
		{
			PrimManager pm;

			if (!lookup.TryGetValue (user, out pm)) {
				pm = new PrimManager (scene);
				lookup.Add (user, pm);
			}

			return pm;
		}
	}
}

