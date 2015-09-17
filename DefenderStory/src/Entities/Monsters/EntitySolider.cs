using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;

namespace DefenderStory.Entities
{
	[EntityRegistry("Solider", 6)]
	public class EntitySolider : EntityLiving
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Dwarf;
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

		public EntitySolider(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			this.Size = new Size(16, 32);
			this.CollisionAIs.Add(new AIKillDefender(this));
			SetAnime(0, 3, 8);
		}

		public override void SetKilledAnime()
		{
			SetGraphic(4);
		}

		public override void SetCrushedAnime()
		{
			SetGraphic(0);
			this.IsCrushed = false;
		}
	}
}
