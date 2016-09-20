using System.Drawing;
using TakeUpJewel.AI;
using TakeUpJewel.Data;
using TakeUpJewel.Util;

namespace TakeUpJewel.Entities
{
	[EntityRegistry("Densy", 88)]
	public class EntityDensy : EntityFlying
	{

		public override int[] ImageHandle => ResourceUtility.Densy;


		public override EntityGroup MyGroup => EntityGroup.Monster;

		public EntityDensy(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			MainAi = new AiFlySearch(this, 2, 0, 3, 0, 3);
			CollisionAIs.Add(new AiKillDefender(this));
		}

		public override void SetKilledAnime()
		{
			AnimeSpeed = 0;
		}

		public override void SetCrushedAnime()
		{
			IsCrushed = false;
			AnimeSpeed = 0;
		}

		public override void OnUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			
			base.OnUpdate(ks);
		}



		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			return this;
		}

	}
}
