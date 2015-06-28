using System;
using OpenSim.Region.Framework.Scenes;
using System.Collections.Generic;
using OpenSim.Framework;
using OpenMetaverse;
using OpenSim.Region.Framework.Interfaces;
using log4net;
using System.Reflection;

namespace Voxel
{
	public class PrimManager
	{
		private static readonly UUID defaultTexture = new UUID ("5748decc-f629-461c-9a36-a35a236fe36f");
		private static readonly float primSize = 1.0f;
		private Scene scene;
		private Stack<SceneObjectGroup> sogStack;
		private Stack<Block[]> blocksStack;

		public PrimManager (Scene scene)
		{
			this.scene = scene;
			sogStack = new Stack<SceneObjectGroup> ();
			blocksStack = new Stack<Block[]> ();
		}

		public void AddBlocks (Block[] blocks)
		{
			if (blocks.Length == 0) {
				return;
			}

			Vector3 rootPos = blocks [0].Pos;
			PrimitiveBaseShape prim = PrimitiveBaseShape.CreateBox ();
			Primitive.TextureEntry tex = new Primitive.TextureEntry (defaultTexture);
			tex.DefaultTexture.RGBA = blocks [0].Color;
			prim.Textures = tex;
			prim.Scale = new Vector3 (primSize, primSize, primSize);

			SceneObjectGroup sog = new SceneObjectGroup (UUID.Zero, rootPos, prim);
			sog.RootPart.UpdatePrimFlags (false, false, true, false);

			for (int i = 1; i < blocks.Length; i++) {
				Vector3 pos = blocks [i].Pos;
				prim = PrimitiveBaseShape.CreateBox ();
				tex = new Primitive.TextureEntry (defaultTexture);
				tex.DefaultTexture.RGBA = blocks [i].Color;
				prim.Textures = tex;
				prim.Scale = new Vector3 (primSize, primSize, primSize);

				Quaternion zeroRot = new Quaternion (0, 0, 0, 1);
				SceneObjectPart sop = new SceneObjectPart (UUID.Zero, prim, rootPos, zeroRot, pos - rootPos);
				sog.AddPart (sop);
			}

			scene.AddNewSceneObject (sog, false);
			sogStack.Push (sog);
			blocksStack.Push (blocks);

			IDialogModule dm = scene.RequestModuleInterface<IDialogModule> ();
			dm.SendNotificationToUsersInRegion (UUID.Zero, "World Master", "New block(s) created.");

			Voxel.Log.Info (string.Format ("[Voxel] Created {0} blocks", blocks.Length));
		}

		public void Undo ()
		{
			if (sogStack.Count == 0) {
				return;
			}

			SceneObjectGroup sog = sogStack.Pop ();
			blocksStack.Pop ();

			scene.DeleteSceneObject (sog, false);
		}

		public Block LastBlock ()
		{
			if (blocksStack.Count == 0) {
				return new Block (new Vector3 (128.0f, 128.0f, 25.0f), new Color4 (1.0f, 0.0f, 0.0f, 1.0f));
			} else {
				Block[] blocks = blocksStack.Peek ();
				return blocks [blocks.Length - 1];
			}
		}

		public Vector3 LastPos ()
		{
			if (blocksStack.Count == 0) {
				return new Vector3 (128.0f, 128.0f, 25.0f);
			} else {
				Block[] blocks = blocksStack.Peek ();
				return blocks [blocks.Length - 1].Pos;
			}
		}
	}
}

