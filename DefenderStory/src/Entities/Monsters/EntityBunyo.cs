using System.Drawing;
using TakeUpJewel.AI;
using TakeUpJewel.Data;
using TakeUpJewel.Util;

namespace TakeUpJewel.Entities
{
	[EntityRegistry("Bunyo", 1)]
	public class EntityBunyo : EntityLiving
	{

		public override int[] ImageHandle => ResourceUtility.CommonMob;


		public override EntityGroup MyGroup => EntityGroup.Monster;

		public override RectangleF Collision => new RectangleF(2, 2, 12, 14);

		public override void Move()
		{
			base.Move();
		}

		public EntityBunyo(PointF pnt, Object[] obj, byte[,,] chps, EntityList par)
		{
			Mpts = obj;
			Map = chps;
			Parent = par;
			Location = pnt;
			MainAi = new AiWalk(this, 1, 0, 1, 4, 5);
			CollisionAIs.Add(new AiKillDefender(this));
			Size = new Size(16, 16);
		}

		public override void OnUpdate(Status ks)
		{
			base.OnUpdate(ks);
		}

		public override void SetKilledAnime()
		{
			SetGraphic(2);
		}

		public override void SetCrushedAnime()
		{
			SetGraphic(3);
		}
	}
}
