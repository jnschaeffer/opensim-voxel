using System;

namespace Voxel
{
	public interface ICommandHandler
	{
		bool TryExecute(Command cmd);
	}
}

