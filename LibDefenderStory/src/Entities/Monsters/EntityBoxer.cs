using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;
using DefenderStory.Data;

namespace DefenderStory.Entities
{
	[EntityRegistry("Boxer", 16)]
	public class EntityBoxer : EntityLiving
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Boxer;
			}
		}
		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Monster;
			}
		}

		public EntityBoxer(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 32);
			CollisionAIs.Add(new AIKillDefender(this));
			SetAnime(0, 1, 8);
		}

		public override void SetKilledAnime()
		{
			SetGraphic(0);
		}

		public override void SetCrushedAnime()
		{
			SetGraphic(0);
			IsCrushed = false;
		}

		public override void onUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			base.onUpdate(ks);
			if (IsOnLand)
			{
				Velocity.Y = -1f;
			}
			if (IsDying)
				Velocity = Vector.Zero;
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			return this;
		}

	}

	[EntityRegistry("Fighter", 14)]
	public class EntityFighter : EntityLiving
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Fighter;
			}
		}


		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Monster;
			}
		}

		public EntityFighter(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 32);
			CollisionAIs.Add(new AIKillDefender(this));
			SetAnime(0, 1, 8);
		}

		public override void SetKilledAnime()
		{
			SetGraphic(0);
		}

		public override void SetCrushedAnime()
		{
			SetGraphic(0);
			IsCrushed = false;
		}

		public override void onUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			base.onUpdate(ks);
			if (IsOnLand)
			{
				Velocity.Y = -1f;
			}
			Velocity.X = -1f;
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
