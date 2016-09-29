using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TakeUpJewel.Data;
using TakeUpJewel.Util;
using static DxLibDLL.DX;
using Object = TakeUpJewel.Data.Object;

namespace TakeUpJewel.Entities
{
	public class EntityFireWeapon : EntityLiving
	{
		public const int SpeedX = 4;

		private int _defspeed;
		public int Life = 20;

		public EntityFireWeapon(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(8, 8);
			SetAnime(3, 6, 8);
			var vec = new Vector();
			if (GameEngine.Ks.Inup)
				vec.Y = -1;
			if (GameEngine.Ks.Indown)
				vec.Y = 1;
			if (GameEngine.Ks.Inleft)
				vec.X = -1;
			else if (GameEngine.Ks.Inright)
				vec.X = 1;

			vec *= 4f;

			Velocity = vec;
		}

		public override int[] ImageHandle => ResourceUtility.Particle;


		public override EntityGroup MyGroup => EntityGroup.DefenderWeapon;

		public override Sounds KilledSound => Sounds.Null;

		public override void SetKilledAnime()
		{
		}

		public override void SetCrushedAnime()
		{
		}

		public override void OnUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			UpdateBehavior(ks);
			if ((int) Velocity.X == 0)
				Velocity.X = _defspeed * 4f;

			base.OnUpdate(ks);
		}

		private void UpdateBehavior(Status ks)
		{
			if (Location.X < 0)
				Kill();

			if (Location.X > ks.Map.Width * 16 - 16)
				Kill();

			if (Location.Y < 0)
			{
				Velocity.Y = 0;
				Location.Y = 0;
			}

			if (Location.Y > ks.Map.Height * 16)
				Kill(true, false);

			foreach (EntityLiving e in new List<Entity>(Parent.FindEntitiesByType<EntityLiving>()))
				if (!e.IsDying && (e.MyGroup == EntityGroup.Enemy) &&
					new RectangleF(Location, Size).CheckCollision(new RectangleF(e.Location, e.Size)))
				{
					e.Kill();
					Kill();
				}
			if (Life < 1)
				Kill();
			if (CollisionBottom() == ObjectHitFlag.Hit)
			{
				Velocity.Y = -2.5f;
				Life--;
			}
			if ((CollisionLeft() == ObjectHitFlag.Hit) || (CollisionRight() == ObjectHitFlag.Hit))
				Kill();
		}

		public override void Dying()
		{
			IsDead = true;
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			if (jsonobj.IsDefined("SpeedX"))
				_defspeed = Math.Sign((float) jsonobj.SpeedX);
			return this;
		}
	}

	public class EntityIceWeapon : EntityLiving
	{
		public const int SpeedX = 4;

		private readonly int _defspeed = 0;
		public int Life = 100;

		public EntityIceWeapon(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			SetGraphic(14);
		}

		public override int[] ImageHandle => ResourceUtility.Weapon;


		public override EntityGroup MyGroup => EntityGroup.DefenderWeapon;

		public override Sounds KilledSound => Sounds.Null;

		public override void SetKilledAnime()
		{
		}

		public override void SetCrushedAnime()
		{
		}

		public override void OnUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			UpdateBehavior(ks);
			if ((int) Velocity.X == 0)
				Velocity.X = _defspeed * 4f;

			base.OnUpdate(ks);
		}

		private void UpdateBehavior(Status ks)
		{
			if (Location.X < 0)
				Kill();

			if (Location.X > ks.Map.Width * 16 - 16)
				Kill();

			if (Location.Y < 0)
			{
				Velocity.Y = 0;
				Location.Y = 0;
			}

			if (Location.Y > ks.Map.Height * 16)
				Kill(true, false);

			foreach (EntityLiving e in new List<Entity>(Parent.FindEntitiesByType<EntityLiving>()))
				if (!e.IsDying && (e.MyGroup == EntityGroup.Enemy) &&
					new RectangleF(Location, Size).CheckCollision(new RectangleF(e.Location, e.Size)))
					e.Kill();
			if (Life < 1)
				Kill();

			if (Life == 70)
			{
				SetGraphic(15);
				Velocity.X *= 0.5f;
			}
			if (Life == 35)
			{
				SetGraphic(16);
				Velocity.X *= 0.5f;
			}

			if ((CollisionLeft() == ObjectHitFlag.Hit) || (CollisionRight() == ObjectHitFlag.Hit))
				Velocity.X *= -1;
			Life--;
		}

		public override void Dying()
		{
			IsDead = true;
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			if (jsonobj.IsDefined("SpeedX"))
				Velocity.X = (float) jsonobj.SpeedX;
			return this;
		}
	}

	public class EntityLeafWeapon : EntityFlying
	{
		public int Life = 40;

		public EntityLeafWeapon(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(8, 8);
			SetGraphic(2);
			Entity ent = null;
			var min = float.MaxValue;

			if (Parent.Count(en => en is EntityLeafWeapon) > 3)
				Kill();

			foreach (var e in from entity in Parent
				where entity.MyGroup == EntityGroup.Enemy
				select entity)
				if (Math.Abs(Location.X - e.Location.X) < min)
				{
					min = Math.Abs(Location.X - e.Location.X);
					ent = e;
				}

			var r = ent != null
				? Math.Atan2(Location.Y - ent.Location.Y, Location.X - ent.Location.X)
				: DevelopUtility.Deg2Rad(GetRand(360));

			float x = -(float) Math.Cos(r) * 5,
				y = -(float) Math.Sin(r) * 5;

			Velocity = new Vector(x, y);
		}

		public override int[] ImageHandle => ResourceUtility.Weapon;


		public override EntityGroup MyGroup => EntityGroup.DefenderWeapon;

		public override Sounds KilledSound => Sounds.Null;


		public override ObjectHitFlag CollisionBottom() => ObjectHitFlag.NotHit;
		public override ObjectHitFlag CollisionTop() => ObjectHitFlag.NotHit;
		public override ObjectHitFlag CollisionLeft() => ObjectHitFlag.NotHit;
		public override ObjectHitFlag CollisionRight() => ObjectHitFlag.NotHit;

		public sealed override void Kill()
		{
			base.Kill();
		}

		public override void SetKilledAnime()
		{
		}

		public override void SetCrushedAnime()
		{
		}

		public override void OnUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			UpdateBehavior(ks);
			base.OnUpdate(ks);
		}

		private void UpdateBehavior(Status ks)
		{
			if (Location.X < 0)
				Kill();

			if (Location.X > ks.Map.Width * 16 - 16)
				Kill();

			if (Location.Y < 0)
				Kill();

			if (Location.Y > ks.Map.Height * 16)
				Kill(true, false);

			foreach (EntityLiving e in new List<Entity>(Parent.FindEntitiesByType<EntityLiving>()))
				if (!e.IsDying && (e.MyGroup == EntityGroup.Enemy) &&
					new RectangleF(Location, Size).CheckCollision(new RectangleF(e.Location, e.Size)))
				{
					e.Kill();
					Kill();
				}
			if (Life < 1)
				Kill();
			if (Math.Abs(Location.X - Parent.MainEntity.Location.X) > GameEngine.ScrSize.Width / 1.7)
				Kill();
			if (CollisionBottom() == ObjectHitFlag.Hit)
				Kill();
			if ((CollisionLeft() == ObjectHitFlag.Hit) || (CollisionRight() == ObjectHitFlag.Hit))
				Kill();
			Life--;
		}

		public override void Dying()
		{
			IsDead = true;
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			if (jsonobj.IsDefined("SpeedX"))
				Velocity.X = (float) jsonobj.SpeedX;
			if (jsonobj.IsDefined("SpeedY"))
				Velocity.Y = (float) jsonobj.SpeedY;
			return this;
		}
	}
}