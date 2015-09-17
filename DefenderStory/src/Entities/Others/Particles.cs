using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;
using DxLibDLL;

namespace DefenderStory.Entities
{
	public class EntityParticleBase : EntityLiving
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Particle;
			}
		}

		public override Size Size
		{
			get; protected set;
		}

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Particle;
			}
		}

		public EntityParticleBase(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(8, 8);
		}

		public override void CheckCollision()
		{
			
		}



		public override void SetKilledAnime()
		{

		}

		public override void SetCrushedAnime()
		{

		}

		public override int DyingMax
		{
			get
			{
				return 0;
			}
		}

		public override void Kill()
		{
			IsDead = true;
		}

		public override void onUpdate(Status ks)
		{
			if (Location.X < -Size.Width || Location.X > GameEngine.map.Width * 16 || Location.Y < -Size.Height || Location.Y > GameEngine.map.Height * 16)
				Kill(true, false);
			base.onUpdate(ks);
		}

	}

	[EntityRegistry("Star", -1)]
	public class EntityStar : EntityParticleBase
	{
		public EntityStar(PointF p, Data.Object[] mpts, byte[,,] mp, EntityList par)
			: base(p, mpts, mp, par)
		{
			Velocity.X = DX.GetRand(4) - 2;
			Velocity.Y = -DX.GetRand(5) - 5;
			SetGraphic(1);
		}

		public override void onUpdate(Status ks)
		{
			base.onUpdate(ks);
		}

	}
	[EntityRegistry("WaterSplash", -1)]
	public class EntityWaterSplash : EntityParticleBase
	{
		public EntityWaterSplash(PointF p, Data.Object[] mpts, byte[,,] mp, EntityList par)
			: base(p, mpts, mp, par)
		{
			Velocity.X = DX.GetRand(2) - 1;
			Velocity.Y = -4;
			SetGraphic(0);
		}

		public override void onUpdate(Status ks)
		{
			Velocity.Y += 0.3f;
			base.onUpdate(ks);
		}

	}
	[EntityRegistry("Bubble", -1)]
	public class EntityBubble : EntityParticleBase
	{
		public EntityBubble(PointF p, Data.Object[] mpts, byte[,,] mp, EntityList par)
			: base(p, mpts, mp, par)
		{
			Velocity.Y = -1;
			SetGraphic(2);
		}

		public override void onUpdate(Status ks)
		{
			var judge = new PointF(Location.X + Size.Width / 2, Location.Y + Size.Height / 2);
			if (Mpts[Map[(int)judge.X / 16, (int)judge.Y / 16, 0]].CheckHit((int)judge.X % 16, (int)judge.Y % 16) != Data.ObjectHitFlag.InWater)
			{
				this.Kill();
			}
			base.onUpdate(ks);
		}
	}
	[EntityRegistry("BrokenBlock", -1)]
	public class EntityBrokenBlock : EntityParticleBase
	{
		public EntityBrokenBlock(PointF p, Data.Object[] mpts, byte[,,] mp, EntityList par)
			: base(p, mpts, mp, par)
		{
			Velocity.X = DX.GetRand(4) - 2;
			Velocity.Y = -DX.GetRand(5) - 5;
			SetGraphic(DevelopUtility.GetRandom(18, 19, 50, 51));
		}

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.MapChipMini;
			}
		}

		public override void onUpdate(Status ks)
		{
			Velocity.Y += 0.3f;
			base.onUpdate(ks);
		}
	}

}
