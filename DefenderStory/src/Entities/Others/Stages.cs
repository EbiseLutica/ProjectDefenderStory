using System.Drawing;
using TakeUpJewel.Data;

namespace TakeUpJewel.Entities
{
	[EntityRegistry("InfinitySpawner", 5)]
	public class EntityInfinitySpawner : Entity
	{
		private dynamic _obj;

		private int _tick;

		public EntityInfinitySpawner(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
		}


		public override EntityGroup MyGroup => EntityGroup.Stage;

		public override void OnUpdate(Status ks)
		{
			if (((Parent.MainEntity.Location.X + 8 < Location.X - 32) ||
				 (Location.X + 48 < Parent.MainEntity.Location.X + 8)) &&
				(_tick > 120))
			{
				if (_obj.IsDefined("Tag"))
					_obj.EntityData.Tag = _obj.Tag;
				int spid = (int) _obj.EntityID, posx = (int) _obj.PosX, posy = (int) _obj.PosY;
				if (GameEngine.EntityRegister.GetDataById(spid) != null)
					Parent.Add(
						GameEngine.EntityRegister.CreateEntity(spid, new PointF(posx, posy), Mpts, Map, Parent, _obj.EntityData),
						spid == 0);
				_tick = 0;
			}
			_tick++;
			base.OnUpdate(ks);
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			if (jsonobj.IsDefined("TargetEntity"))
			{
				_obj = jsonobj.TargetEntity;
				_obj.PosX = Location.X;
				_obj.PosY = Location.Y;
			}
			return this;
		}
	}

	[EntityRegistry("MiddleFlag", 83)]
	public class EntityMiddleFlag : Entity
	{
		private bool _flagged;
		private dynamic _obj = null;

		public EntityMiddleFlag(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
		}


		public override EntityGroup MyGroup => EntityGroup.Stage;

		public override void OnUpdate(Status ks)
		{
			if ((Parent.MainEntity.Location.X > Location.X + 8) && !_flagged)
			{
				GameEngine.Middle = new PointF(Location.X + 8, Location.Y + 8);
				_flagged = true;
			}
			base.OnUpdate(ks);
		}
	}

	[EntityRegistry("Goal", 18)]
	public class EntityGoal : Entity
	{
		private int _next = 1;
		private dynamic _obj = null;

		public EntityGoal(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
		}


		public override EntityGroup MyGroup => EntityGroup.Stage;

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			_next = (int) jsonobj.NextStage;
			return this;
		}

		public override void OnUpdate(Status ks)
		{
			if (Parent.MainEntity.Location.X > Location.X + 8)
			{
				GameEngine.NextStage = _next;
				GameEngine.IsGoal = true;
			}
			base.OnUpdate(ks);
		}
	}
}