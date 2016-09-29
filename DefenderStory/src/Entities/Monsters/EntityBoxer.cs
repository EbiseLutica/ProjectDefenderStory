using System.Drawing;
using TakeUpJewel.AI;
using TakeUpJewel.Data;
using TakeUpJewel.Util;

namespace TakeUpJewel.Entities
{
	[EntityRegistry("Boxer", 16)]
	public class EntityBoxer : EntityLiving
	{
		public EntityBoxer(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 32);
			CollisionAIs.Add(new AiKillDefender(this));
			SetAnime(0, 1, 8);
		}

		public override int[] ImageHandle => ResourceUtility.Boxer;


		public override EntityGroup MyGroup => EntityGroup.Enemy;

		public override void SetKilledAnime()
		{
			SetGraphic(0);
		}

		public override void SetCrushedAnime()
		{
			SetGraphic(0);
			IsCrushed = false;
		}

		public override void OnUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			base.OnUpdate(ks);
			if (IsOnLand)
				Velocity.Y = -1f;
			if (IsDying)
				Velocity = Vector.Zero;
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			return this;
		}
	}

	[EntityRegistry("Fighter", 14)]
	public class EntityFighter : EntityLiving
	{
		public EntityFighter(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 32);
			CollisionAIs.Add(new AiKillDefender(this));
			SetAnime(0, 1, 8);
		}

		public override int[] ImageHandle => ResourceUtility.Fighter;


		public override EntityGroup MyGroup => EntityGroup.Enemy;

		public override void SetKilledAnime()
		{
			SetGraphic(0);
		}

		public override void SetCrushedAnime()
		{
			SetGraphic(0);
			IsCrushed = false;
		}

		public override void OnUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			base.OnUpdate(ks);
			if (IsOnLand)
				Velocity.Y = -1f;
			Velocity.X = -1f;
			if (IsDying)
				Velocity = Vector.Zero;
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			return this;
		}
	}
}