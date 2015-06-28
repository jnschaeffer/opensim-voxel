using System;
using OpenMetaverse;

namespace Voxel
{
	public class Block
	{
		public Vector3 Pos {
			get;
			private set;
		}
		public Color4 Color {
			get;
			private set;
		}

		public Block (Vector3 pos, Color4 color)
		{
			Pos = pos;
			Color = color;
		}
	}
}

