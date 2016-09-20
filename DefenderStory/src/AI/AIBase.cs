using TakeUpJewel.Entities;

namespace TakeUpJewel.AI
{
	public abstract class AiBase
	{
		protected EntityLiving HostEntity;

		/// <summary>
		/// 派生クラスで、AI を動作させるかどうかのフラグを取得します。
		/// </summary>
		public abstract bool Use
		{
			get;
		}

		public abstract void OnUpdate();

		public virtual void OnInit() { }

	}
}
