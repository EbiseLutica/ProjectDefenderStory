using System.Drawing;
using DxLibDLL;
using TakeUpJewel.AI;
using TakeUpJewel.Data;
using TakeUpJewel.Util;

namespace TakeUpJewel.Entities
{
	[EntityRegistry("Spider", 11)]
	public class EntitySpider : EntityFlying
	{
		protected float Accel = 0.1f;

		public EntitySpider(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = new PointF(pnt.X, pnt.Y + 64);
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			CollisionAIs.Add(new AiKillDefender(this));
		}

		public override int[] ImageHandle => ResourceUtility.Spider;


		public override EntityGroup MyGroup => EntityGroup.Enemy;

		public override void CheckCollision()
		{
			if (!IsDying)
				base.CheckCollision();
		}

		public override void SetKilledAnime()
		{
		}

		public override void SetCrushedAnime()
		{
		}

		public override void OnUpdate(Status ks)
		{
			if (!IsDying)
			{
				Velocity.Y += Accel;
				if (Velocity.Y >= 0.8f)
					Accel = -0.01f;
				else if (Velocity.Y <= -0.8f)
					Accel = 0.01f;
			}
			base.OnUpdate(ks);
		}

		public override void Kill()
		{
			base.Kill();
			Velocity.Y = 0;
		}
	}

	[EntityRegistry("SpiderString", 12)]
	public class EntityString : EntityVisible
	{
		private string _targetTag = "";

		public EntityString(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
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
			if (jsonobj.IsDefined("TargetEntityTag"))
				_targetTag = jsonobj.TargetEntityTag;
			return this;
		}

		public override void OnDraw(PointF p, Status ks)
		{
			foreach (var te in Parent.FindEntitiesByTag(_targetTag))
			{
				if (te is EntityLiving && ((EntityLiving) te).IsDying)
					continue;
				var pp = new PointF(ks.Camera.X + te.Location.X, ks.Camera.Y + te.Location.Y);
				DX.DrawLine((int) p.X + 8, (int) p.Y + 8, (int) pp.X + 8, (int) pp.Y + 8, DX.GetColor(255, 255, 255));
			}
		}
	}
}