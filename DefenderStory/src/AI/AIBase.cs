using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefenderStory.Entities;

namespace DefenderStory.AI
{
	public abstract class AIBase
	{
		protected EntityLiving HostEntity;

		/// <summary>
		/// 派生クラスで、AI を動作させるかどうかのフラグを取得します。
		/// </summary>
		public abstract bool Use
		{
			get;
		}

		public abstract void onUpdate();

		public virtual void onInit() { }

	}
}
