using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;

namespace DefenderStory.Entities
{
	[EntityRegistry("", 127)]
	public class EntityTemplate : EntityLiving
	{

		public override int[] ImageHandle
		{
			get
			{
				throw new NotImplementedException();
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
				throw new NotImplementedException();
			}
		}

		public EntityTemplate(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
		}

		public override void SetKilledAnime()
		{
			throw new NotImplementedException();
		}

		public override void SetCrushedAnime()
		{
			throw new NotImplementedException();
		}
	}
}
