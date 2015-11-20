using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefenderStory
{
	public abstract class PluginBase
	{
		public abstract string Name { get; }
		public abstract string Version { get; }
		public abstract string Description { get; }

		//public virtual PreInit

	}
}