using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;
using DxLibDLL;

namespace DefenderStory.Entities
{
	[EntityRegistry("Spider", 11)]
	public class EntitySpider : EntityFlying
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Spider;
			}
		}

		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Monster;
			}
		}

		protected float accel = 0.1f;

		public EntitySpider(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = new PointF(pnt.X, pnt.Y + 64);
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			CollisionAIs.Add(new AIKillDefender(this));
		}

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

		public override void onUpdate(Status ks)
		{
			if (!IsDying)
			{
				Velocity.Y += accel;
				if (Velocity.Y >= 0.8f)
					accel = -0.01f;
				else if (Velocity.Y <= -0.8f)
					accel = 0.01f;
			}
			base.onUpdate(ks);
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
		private string TargetTag = "";
		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Stage;
			}
		}

		public EntityString(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			if (jsonobj.IsDefined("TargetEntityTag"))
			{
				TargetTag = jsonobj.TargetEntityTag;
			}
			return this;
		}

		public override void onDraw(PointF p, Status ks)
		{
			foreach (var te in Parent.FindEntitiesByTag(TargetTag))
			{
				if (te is EntityLiving && ((EntityLiving)te).IsDying)
					continue;
				var pp = new PointF(ks.camera.X + te.Location.X, ks.camera.Y + te.Location.Y);
				DX.DrawLine((int)p.X + 8, (int)p.Y + 8, (int)pp.X + 8, (int)pp.Y + 8, DX.GetColor(255, 255, 255));
			}
		}
	}
}
