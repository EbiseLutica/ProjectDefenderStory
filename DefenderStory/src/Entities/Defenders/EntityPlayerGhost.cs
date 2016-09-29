using System.Drawing;
using TakeUpJewel.Data;
using TakeUpJewel.Entities;

namespace TakeUpJewel.src.Entities.Defenders
{
	public class EntityPlayerGhost : EntityGraphical
	{
		public EntityPlayerGhost(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
		}

		public override int[] ImageHandle => null;


		public override EntityGroup MyGroup => EntityGroup.Particle;


		/// <summary>
		///     Tick 毎に呼ばれる Entity の処理イベントです。
		/// </summary>
		/// <param name="ks"></param>
		public override void OnUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			base.OnUpdate(ks);
		}

		/// <summary>
		///     Entity 生成時にメタデータが渡されると、このメソッドが呼ばれます。
		/// </summary>
		/// <param name="jsonobj"></param>
		/// <returns></returns>
		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			return this;
		}
	}
}