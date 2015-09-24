using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;
using DefenderStory.Data;

namespace DefenderStory.Entities
{
	[EntityRegistry("Turbo", 17)]
	public class EntityTurbo : EntityLiving
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Turbo;
			}
		}

		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Monster;
			}
		}

		public EntityTurbo(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			CollisionAIs.Add(new AIKillDefender(this));
			SetAnime(0, 1, 8);
		}

		public override void SetKilledAnime()
		{
			SetGraphic(2);	
		}

		public override void SetCrushedAnime()
		{
			SetGraphic(2);
			IsCrushed = false;
		}

		public override void onUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			base.onUpdate(ks);
			Velocity.X = -2f;
			if (IsDying)
				Velocity = Vector.Zero;
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			return this;
		}

	}

	[EntityRegistry("RollingRock", 15)]
	public class EntityRollingRock : EntityLiving
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Turbo;
			}
		}



		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Monster;
			}
		}

		public EntityRollingRock(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			CollisionAIs.Add(new AIKillDefender(this));
			SetAnime(0, 1, 8);
		}

		public override void SetKilledAnime()
		{
			
		}

		public override void SetCrushedAnime()
		{
			IsCrushed = false;
		}

		public override void Kill()
		{
			base.Kill();
			IsDead = true;
			Parent.Add(new EntityTurbo(Location, Mpts, Map, Parent) { Velocity = new Data.Vector(2, -3) });
			Parent.Add(new EntityTurbo(Location, Mpts, Map, Parent) { Velocity = new Data.Vector(-2, -3) });
		}

		public override void onUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			base.onUpdate(ks);
			Velocity.X = -2f;
			if (IsDying)
				Velocity = Vector.Zero;
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			return this;
		}

	}
}
