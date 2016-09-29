using System.Drawing;
using TakeUpJewel.Data;

namespace TakeUpJewel.Entities
{
	//[EntityRegistry("", 127)]
	public class EntityTemplate : EntityLiving
	{
		public EntityTemplate(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
		}

		public override int[] ImageHandle => null;


		public override EntityGroup MyGroup => EntityGroup.Other;

		/// <summary>
		///     死んでいるアニメーションを設定します。
		/// </summary>
		public override void SetKilledAnime()
		{
		}

		/// <summary>
		///     踏みつけられたアニメーションを設定します。
		/// </summary>
		public override void SetCrushedAnime()
		{
		}

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