using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.Util;
using DefenderStory.AI;

namespace DefenderStory.Entities
{
	[EntityRegistry("Daemon", 3)]
	public class EntityDaemon : EntityLiving
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Daemon;
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

		public EntityDaemon(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			MainAI = new AIWalk(this, 1, 0, 3, 7, 10);
			CollisionAIs.Add(new AIKillDefender(this));
		}

		public override void SetKilledAnime()
		{
			SetAnime(5, 6, 15);
			LoopTimes = 0;
		}

		public override void SetCrushedAnime()
		{
			SetGraphic(4);
		}
	}
}
