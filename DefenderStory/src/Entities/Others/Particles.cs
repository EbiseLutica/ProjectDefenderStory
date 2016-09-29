using System.Drawing;
using DxLibDLL;
using TakeUpJewel.Data;
using TakeUpJewel.Util;

namespace TakeUpJewel.Entities
{
	public class EntityParticleBase : EntityLiving
	{
		public EntityParticleBase(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(8, 8);
		}

		public override int[] ImageHandle => ResourceUtility.Particle;


		public override EntityGroup MyGroup => EntityGroup.Particle;

		public override int DyingMax => 0;

		public override void CheckCollision()
		{
		}


		public override void SetKilledAnime()
		{
		}

		public override void SetCrushedAnime()
		{
		}

		public override void Kill()
		{
			IsDead = true;
		}

		public override void OnUpdate(Status ks)
		{
			if ((Location.X < -Size.Width) || (Location.X > GameEngine.Map.Width * 16) || (Location.Y < -Size.Height) ||
				(Location.Y > GameEngine.Map.Height * 16))
				Kill(true, false);
			base.OnUpdate(ks);
		}
	}

	[EntityRegistry("Star", -1)]
	public class EntityStar : EntityParticleBase
	{
		public EntityStar(PointF p, Object[] mpts, byte[,,] mp, EntityList par)
			: base(p, mpts, mp, par)
		{
			Velocity.X = DX.GetRand(4) - 2;
			Velocity.Y = -DX.GetRand(5) - 5;
			SetGraphic(1);
		}

		public override void OnUpdate(Status ks)
		{
			base.OnUpdate(ks);
		}
	}

	[EntityRegistry("WaterSplash", -1)]
	public class EntityWaterSplash : EntityParticleBase
	{
		public EntityWaterSplash(PointF p, Object[] mpts, byte[,,] mp, EntityList par)
			: base(p, mpts, mp, par)
		{
			Velocity.X = DX.GetRand(2) - 1;
			Velocity.Y = -4;
			SetGraphic(0);
		}

		public override void OnUpdate(Status ks)
		{
			Velocity.Y += 0.3f;
			base.OnUpdate(ks);
		}
	}

	[EntityRegistry("Bubble", -1)]
	public class EntityBubble : EntityParticleBase
	{
		public EntityBubble(PointF p, Object[] mpts, byte[,,] mp, EntityList par)
			: base(p, mpts, mp, par)
		{
			Velocity.Y = -1;
			SetGraphic(2);
		}

		public override void OnUpdate(Status ks)
		{
			var judge = new PointF(Location.X + Size.Width / 2, Location.Y + Size.Height / 2);
			if (Mpts[Map[(int) judge.X / 16, (int) judge.Y / 16, 0]].CheckHit((int) judge.X % 16, (int) judge.Y % 16) !=
				ObjectHitFlag.InWater)
				Kill();
			base.OnUpdate(ks);
		}
	}

	[EntityRegistry("BrokenBlock", -1)]
	public class EntityBrokenBlock : EntityParticleBase
	{
		public EntityBrokenBlock(PointF p, Object[] mpts, byte[,,] mp, EntityList par)
			: base(p, mpts, mp, par)
		{
			Velocity.X = DX.GetRand(4) - 2;
			Velocity.Y = -DX.GetRand(5) - 5;
			SetGraphic(DevelopUtility.GetRandom(18, 19, 50, 51));
		}

		public override int[] ImageHandle => ResourceUtility.MapChipMini;

		public override void OnUpdate(Status ks)
		{
			Velocity.Y += 0.3f;
			base.OnUpdate(ks);
		}
	}
}