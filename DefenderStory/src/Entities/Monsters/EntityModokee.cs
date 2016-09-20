using System.Drawing;
using TakeUpJewel.AI;
using TakeUpJewel.Data;
using TakeUpJewel.Util;

namespace TakeUpJewel.Entities
{
	[EntityRegistry("Modokee_Ground", 2)]
	public class EntityModokee : EntityFlying
	{

		public override int[] ImageHandle => ResourceUtility.ModokeeGround;


		public override EntityGroup MyGroup => EntityGroup.Monster;

		public EntityModokee(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(32, 16);
			MainAi = new AiFlySine(this, 1, 0, 1, 4, 5);
			CollisionAIs.Add(new AiKillDefender(this));
		}

		public override void SetKilledAnime()
		{
			if (Direction == Direction.Left)
				SetGraphic(2);
			else
				SetGraphic(6);
		}

		public override void SetCrushedAnime()
		{
			if (Direction == Direction.Left)
				SetGraphic(3);
			else
				SetGraphic(7);
		}
	}

}
