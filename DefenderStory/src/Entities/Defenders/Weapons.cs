using System.Collections.Generic;
using System.Drawing;
using TakeUpJewel.Data;
using TakeUpJewel.Util;

namespace TakeUpJewel.Entities
{
	public class EntityFireWeapon : EntityLiving
	{

		public override int[] ImageHandle => ResourceUtility.Particle;


		public override EntityGroup MyGroup => EntityGroup.DefenderWeapon;
		public const int SpeedX = 4;
		public int Life = 20;
		public EntityFireWeapon(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(8, 8);
			SetAnime(3, 6, 8);
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
			{
				Kill();
			}

			if (Location.X > ks.Map.Width * 16 - 16)
			{
				Kill();
			}

			if (Location.Y < 0)
			{
				Velocity.Y = 0;
				Location.Y = 0;
			}

			if (Location.Y > ks.Map.Height * 16)
			{
				Kill(true, false);
			}

			foreach (EntityLiving e in new List<Entity>(Parent.FindEntitiesByType<EntityLiving>()))
			{
				if (!e.IsDying && e.MyGroup == EntityGroup.Monster && new RectangleF(Location, Size).CheckCollision(new RectangleF(e.Location, e.Size)))
				{
					e.Kill();
					Kill();
				}
			}
			if (Life < 1)
				Kill();
			if (CollisionBottom() == ObjectHitFlag.Hit)
			{
				Velocity.Y = -2.5f;
				Life--;
			}
			if (CollisionLeft() == ObjectHitFlag.Hit || CollisionRight() == ObjectHitFlag.Hit)
			{
				Kill();
			}

		}

		public override void Dying()
		{
			IsDead = true;
		}

		public override Sounds KilledSound => Sounds.Null;

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			if (jsonobj.IsDefined("Speed"))
				Velocity.X = (float)jsonobj.Speed;
			return this;
		}

	}

	public class EntityLeafWeapon : EntityFlying
	{

		public override int[] ImageHandle => ResourceUtility.Weapon;


		public override EntityGroup MyGroup => EntityGroup.DefenderWeapon;
		public const int SpeedX = 4;
		public int Life = 20;
		public EntityLeafWeapon(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(8, 8);
			SetGraphic(2);
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
			{
				Kill();
			}

			if (Location.X > ks.Map.Width * 16 - 16)
			{
				Kill();
			}

			if (Location.Y < 0)
			{
				Kill();
			}

			if (Location.Y > ks.Map.Height * 16)
			{
				Kill(true, false);
			}

			foreach (EntityLiving e in new List<Entity>(Parent.FindEntitiesByType<EntityLiving>()))
			{
				if (!e.IsDying && e.MyGroup == EntityGroup.Monster && new RectangleF(Location, Size).CheckCollision(new RectangleF(e.Location, e.Size)))
				{
					e.Kill();
					Kill();
				}
			}
			if (Life < 1)
				Kill();
			if (CollisionBottom() == ObjectHitFlag.Hit)
			{
				Kill();
			}
			if (CollisionLeft() == ObjectHitFlag.Hit || CollisionRight() == ObjectHitFlag.Hit)
			{
				Kill();
			}

		}

		public override void Dying()
		{
			IsDead = true;
		}

		public override Sounds KilledSound => Sounds.Null;

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			if (jsonobj.IsDefined("SpeedX"))
				Velocity.X = (float)jsonobj.SpeedX;
			if (jsonobj.IsDefined("SpeedY"))
				Velocity.Y = (float)jsonobj.SpeedY;
			return this;
		}

	}
}
