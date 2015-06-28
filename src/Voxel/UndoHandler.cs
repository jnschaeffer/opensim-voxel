using System;

namespace Voxel
{
	public class UndoHandler : ICommandHandler
	{
		private UserPrimManager userPrimManager;

		public UndoHandler (UserPrimManager u)
		{
			userPrimManager = u;
		}

		public bool TryExecute (Command cmd)
		{
			if (cmd.Args.Length > 1) {
				return false;
			}

			userPrimManager.GetPrimManager (cmd.User).Undo ();

			return true;
		}
	}
}

