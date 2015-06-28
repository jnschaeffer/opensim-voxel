using System;
using OpenMetaverse;

namespace Voxel
{
	public class Command
	{
		public UUID User {
			get;
			private set;
		}

		public string[] Args {
			get;
			private set;
		}

		public Command (UUID u, string[] a)
		{
			User = u;
			Args = a;
		}
	}
}

