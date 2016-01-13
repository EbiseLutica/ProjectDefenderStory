using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;

namespace DefenderStory.Entities
{
	[EntityRegistry("InfinitySpawner", 5)]
	public class EntityInfinitySpawner : Entity
	{
		private dynamic obj = null;
		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Stage;
			}
		}

		public EntityInfinitySpawner(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
		}

		public override void onUpdate(Status ks)
		{
			if ((Parent.MainEntity.Location.X + 8 < this.Location.X - 32 ||
				this.Location.X + 48 < Parent.MainEntity.Location.X + 8) &&
				tick > 120)
			{
				if (obj.IsDefined("Tag"))
					obj.EntityData.Tag = obj.Tag;
				int spid = (int)obj.EntityID, posx = (int)obj.PosX, posy = (int)obj.PosY;
				if (GameEngine.EntityRegister.GetDataById(spid) != null)
					Parent.Add(GameEngine.EntityRegister.CreateEntity(spid, new PointF(posx, posy), Mpts, Map, Parent, obj.EntityData), spid == 0);
				tick = 0;
			}
			tick++;
			base.onUpdate(ks);
		}

		private int tick = 0;

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			if (jsonobj.IsDefined("TargetEntity"))
			{
				obj = jsonobj.TargetEntity;
				obj.PosX = Location.X;
				obj.PosY = Location.Y;
			}
			return this;
		}

	}

	[EntityRegistry("MiddleFlag", 83)]
	public class EntityMiddleFlag : Entity
	{
		private dynamic obj = null;
		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Stage;
			}
		}

		public EntityMiddleFlag(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
		}
		private bool flagged = false;
		public override void onUpdate(Status ks)
		{
			if (Parent.MainEntity.Location.X > this.Location.X + 8 && !flagged)
			{
				GameEngine.Middle = new PointF(this.Location.X + 8, this.Location.Y + 8);
				flagged = true;
			}
			base.onUpdate(ks);
		}

	}

	[EntityRegistry("Goal", 18)]
	public class EntityGoal : Entity
	{
		private dynamic obj = null;
		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Stage;
			}
		}

		public EntityGoal(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			next = (int)jsonobj.NextStage;
			return this;
		}

		private int next = 1;
		public override void onUpdate(Status ks)
		{
			if (Parent.MainEntity.Location.X > this.Location.X + 8)
			{
				GameEngine.NextStage = this.next;
				GameEngine.IsGoal = true;
			}
			base.onUpdate(ks);
		}

	}

}
