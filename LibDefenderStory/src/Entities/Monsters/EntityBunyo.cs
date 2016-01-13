using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DefenderStory.Util;
using DefenderStory.Data;
using DefenderStory.AI;

namespace DefenderStory.Entities
{
	[EntityRegistry("Bunyo", 1)]
	public class EntityBunyo : EntityLiving
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.CommonMob;
			}
		}

		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Monster;
			}
		}

		public override RectangleF Collision
		{
			get
			{
				return new RectangleF(2, 2, 12, 14);
			}
		}

		public override void Move()
		{
			base.Move();
		}

		public EntityBunyo(PointF pnt, Data.Object[] obj, byte[,,] chps, EntityList par)
		{
			Mpts = obj;
			Map = chps;
			Parent = par;
			Location = pnt;
			MainAI = new AIWalk(this, 1, 0, 1, 4, 5);
			CollisionAIs.Add(new AIKillDefender(this));
			Size = new Size(16, 16);
		}

		public override void onUpdate(Status ks)
		{
			base.onUpdate(ks);
		}

		public override void SetKilledAnime()
		{
			SetGraphic(2);
		}

		public override void SetCrushedAnime()
		{
			SetGraphic(3);
		}
	}
}
