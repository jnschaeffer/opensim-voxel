using System;
using OpenMetaverse;
using System.Drawing;
using log4net;
using System.Reflection;

namespace Voxel
{
	public class CreateHandler : ICommandHandler
	{
		private enum Direction
		{
			Up,
			// z-axis
			Down,
			West,
			// x-axis
			East,
			North,
			// y-axis
			South,
			U,
			D,
			W,
			E,
			N,
			S
		}

		private UserPrimManager userPrimManager;

		public CreateHandler (UserPrimManager u)
		{
			userPrimManager = u;
		}

		private bool tryParseVector (string vecStr, out Vector3 pos)
		{
			char[] comma = {','};
			string[] coords = vecStr.Split (comma);
			float x, y, z;

			if (coords.Length != 3) {
				pos = new Vector3(0, 0, 0);
				return false;
			}

			if (!(float.TryParse(coords[0], out x) && float.TryParse(coords[1], out y) && float.TryParse(coords[2], out z))) {
				pos = new Vector3(0, 0, 0);
				return false;
			}

			pos = new Vector3(x, y, z);

			return true;
		}

		private Vector3 makeRelativePosition(Direction dir, Vector3 lastPos)
		{
			Vector3 delta;

			switch (dir) {
			case Direction.U:
			case Direction.Up:
				delta = new Vector3 (0.0f, 0.0f, 1.0f);
				break;
			case Direction.D:
			case Direction.Down:
				delta = new Vector3 (0.0f, 0.0f, -1.0f);
				break;
			case Direction.W:
			case Direction.West:
				delta = new Vector3 (1.0f, 0.0f, 0.0f);
				break;
			case Direction.E:
			case Direction.East:
				delta = new Vector3 (-1.0f, 0.0f, 0.0f);
				break;
			case Direction.N:
			case Direction.North:
				delta = new Vector3 (0.0f, 1.0f, 0.0f);
				break;
			case Direction.S:
			case Direction.South:
				delta = new Vector3 (0.0f, -1.0f, 0.0f);
				break;
			default:
				delta = new Vector3 (0, 0, 0);
				break;
			}

			return lastPos + delta;
		}

		private bool tryParsePosition(string posStr, Vector3 lastPos, out Vector3 pos)
		{
			Direction dir;

			if (Enum.TryParse<CreateHandler.Direction> (posStr, true, out dir)) {
				pos = makeRelativePosition (dir, lastPos);
				return true;
			} else if (tryParseVector (posStr, out pos)) {
				return true;
			} else {
				pos = new Vector3 (0, 0, 0);
				return false;
			}
		}

		private bool tryParseHTMLColor(string colorStr, out Color4 color)
		{

			Color htmlCol;
			bool valid = true;

			try {
				htmlCol = ColorTranslator.FromHtml (string.Format ("#{0}", colorStr));
				valid = true;
			} catch {
				htmlCol = Color.Empty;
				valid = false;
			}

			color = new Color4 ();
			color.R = htmlCol.R/255.0f;
			color.G = htmlCol.G/255.0f;
			color.B = htmlCol.B/255.0f;
			color.A = 1.0f;

			return valid;
		}

		private bool tryParseColor(string colorStr, out Color4 color)
		{
			KnownColor k;

			if (Enum.TryParse<KnownColor> (colorStr, true, out k)) {
				Color c = Color.FromKnownColor (k);

				color = new Color4 ();
				color.R = c.R/255.0f;
				color.G = c.G/255.0f;
				color.B = c.B/255.0f;
				color.A = 1.0f;

				Voxel.Log.Info (string.Format ("[Voxel] Mapping color {0} as ({1},{2},{3},{4})->({5},{6},{7},{8})",
				                          colorStr, c.R, c.G, c.B, c.A, color.R, color.G, color.B, color.A));

				return true;
			} else if (tryParseHTMLColor (colorStr, out color)) {
				return true;
			} else {
				return false;
			}
		}

		private bool tryParseBlock(string blockStr, Block lastBlock, out Block block)
		{
			char[] semicolon = {';'};
			string[] sections = blockStr.Split (semicolon);

			Vector3 pos;
			Color4 color;

			switch (sections.Length) {
			case 1:
				color = lastBlock.Color;
				if (!tryParsePosition(sections[0], lastBlock.Pos, out pos)) {
					block = null;
				} else {
					block = new Block(pos, color);
				}
				break;
			case 2:
				if (!(tryParsePosition(sections[0], lastBlock.Pos, out pos) && tryParseColor(sections[1], out color))) {
					block = null;
				} else {
					block = new Block(pos, color);
				}
				break;
			default:
				block = null;
				break;
			}

			return block != null;
		}

		public bool TryExecute(Command cmd)
		{
			Voxel.Log.Info ("[Voxel] Executing command.");
			string[] args = cmd.Args;

			if (args.Length < 2) {
				return false;
			}

			Block[] blocks = new Block[args.Length-1];
			PrimManager pm = userPrimManager.GetPrimManager (cmd.User);

			Block lastBlock = pm.LastBlock ();

			for (int i = 1; i < args.Length; i++) {
				Block block;
				if (!tryParseBlock (args [i], lastBlock, out block)) {
					return false;
				}
				blocks [i - 1] = block;
				lastBlock = block;
			}

			pm.AddBlocks (blocks);

			return true;
		}
	}
}

