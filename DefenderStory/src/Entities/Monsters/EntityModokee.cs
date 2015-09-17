using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.Util;
using DefenderStory.AI;

namespace DefenderStory.Entities
{
	[EntityRegistry("Modokee_Ground", 2)]
	public class EntityModokee : EntityFlying
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Modokee_Ground;
			}
		}

		public override Size Size
		{
			get; protected set;
		}

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Monster;
			}
		}

		public EntityModokee(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			this.Size = new Size(32, 16);
			this.MainAI = new AIFlySine(this, 1, 0, 1, 4, 5);
			this.CollisionAIs.Add(new AIKillDefender(this));
		}

		public override void SetKilledAnime()
		{
			if (Direction == Direction.Left)
				SetGraphic(2);
			else
				SetGraphic(6);
		}

		public override void SetCrushedAnime()
		{
			if (Direction == Direction.Left)
				SetGraphic(3);
			else
				SetGraphic(7);
		}
	}

}
