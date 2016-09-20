using System.Drawing;
using TakeUpJewel.AI;
using TakeUpJewel.Data;
using TakeUpJewel.Util;

namespace TakeUpJewel.Entities
{
	[EntityRegistry("Solider", 6)]
	public class EntitySolider : EntityLiving
	{

		public override int[] ImageHandle => ResourceUtility.Dwarf;


		public override EntityGroup MyGroup => EntityGroup.Monster;

		public EntitySolider(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 32);
			CollisionAIs.Add(new AiKillDefender(this));
			SetAnime(0, 3, 8);
		}

		public override void SetKilledAnime()
		{
			SetGraphic(4);
		}

		public override void SetCrushedAnime()
		{
			SetGraphic(0);
			IsCrushed = false;
		}
	}
}
