using System;
using System.Drawing;
using TakeUpJewel.AI;
using TakeUpJewel.Data;
using TakeUpJewel.Util;
using Object = TakeUpJewel.Data.Object;

namespace TakeUpJewel.Entities
{
	[EntityRegistry("FolderFly", 86)]
	public class EntityFolderFly : EntityFlying
	{
		private int _tick;

		public EntityFolderFly(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			MainAi = new AiFlySine(this, 1, 0, 1, 0, 1);
			CollisionAIs.Add(new AiKillDefender(this));
		}

		public override int[] ImageHandle => ResourceUtility.FolderFly;


		public override EntityGroup MyGroup => EntityGroup.Enemy;

		public override void SetKilledAnime()
		{
			AnimeSpeed = 0;
		}

		public override void SetCrushedAnime()
		{
			AnimeSpeed = 0;
			IsCrushed = false;
		}

		public override void OnUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			if ((_tick > 140) && !IsDying)
			{
				_tick = 0;
				SoundUtility.PlaySound(Sounds.ShootArrow);
				var r = Math.Atan2(Location.Y - Parent.MainEntity.Location.Y, Location.X - Parent.MainEntity.Location.X);
				float x = -(float) Math.Cos(r) * 2.2f,
					y = -(float) Math.Sin(r) * 2.2f;

				Parent.Add(new EntityDocument(Location, Mpts, Map, Parent) {Velocity = new Vector(x, y)});
			}
			_tick++;

			base.OnUpdate(ks);
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			return this;
		}
	}

	[EntityRegistry("Document", -1)]
	public class EntityDocument : EntityFlying
	{
		public EntityDocument(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			CollisionAIs.Add(new AiKillDefender(this));
			SetAnime(2, 4, 4);
		}

		public override int[] ImageHandle => ResourceUtility.FolderFly;


		public override EntityGroup MyGroup => EntityGroup.MonsterWeapon;

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

		public override void CheckCollision()
		{
		}

		public override void OnUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。

			if ((Location.X < -Size.Width) || (Location.Y < -Size.Height) || (Location.X > ks.Map.Width * 16) ||
				(Location.Y > ks.Map.Height * 16))
				Kill();

			base.OnUpdate(ks);
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			return this;
		}
	}
}