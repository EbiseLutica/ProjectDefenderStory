using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;

namespace DefenderStory.Entities
{
	[EntityRegistry("FolderFly", 86)]
	public class EntityFolderFly : EntityFlying
	{
		private int tick = 0;
		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.FolderFly;
			}
		}

		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Monster;
			}
		}

		public EntityFolderFly(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			MainAI = new AIFlySine(this, 1, 0, 1, 0, 1);
			CollisionAIs.Add(new AIKillDefender(this));
		}

		public override void SetKilledAnime()
		{
			AnimeSpeed = 0;
		}

		public override void SetCrushedAnime()
		{
			AnimeSpeed = 0;
			IsCrushed = false;
		}

		public override void onUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			if (tick > 140 && !IsDying)
			{
				tick = 0;
				SoundUtility.PlaySound(Sounds.ShootArrow);
				var r = Math.Atan2(Location.Y - Parent.MainEntity.Location.Y, Location.X - Parent.MainEntity.Location.X);
				float x = -(float)Math.Cos(r) * 2.2f,
					y = -(float)Math.Sin(r) * 2.2f;

				Parent.Add(new EntityDocument(Location, Mpts, Map, Parent) { Velocity = new Data.Vector(x, y) });
			}
			tick++;

			base.onUpdate(ks);
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			return this;
		}

	}

	[EntityRegistry("Document", -1)]
	public class EntityDocument : EntityFlying
	{
		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.FolderFly;
			}
		}



		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.MonsterWeapon;
			}
		}

		public EntityDocument(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			CollisionAIs.Add(new AIKillDefender(this));
			SetAnime(2, 4, 4);
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

		public override void CheckCollision()
		{
			
		}

		public override void onUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。

			if (Location.X < -Size.Width || Location.Y < -Size.Height || Location.X > ks.map.Width * 16 || Location.Y > ks.map.Height * 16)
				Kill();

			base.onUpdate(ks);
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			return this;
		}

	}

}
