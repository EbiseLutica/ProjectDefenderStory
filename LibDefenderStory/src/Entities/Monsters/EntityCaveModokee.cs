using DefenderStory.AI;
using DefenderStory.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DefenderStory.Entities
{
	[EntityRegistry("Modokee_Cave", 8)]
	public class EntityCaveModokee : EntityModokee
	{
		public EntityCaveModokee(PointF pnt, Data.Object[] objs, byte[,,] chips, EntityList par)
			: base(pnt, objs, chips, par)
		{
			this.MainAI = new AIFlySearch(this, 1, 0, 1, 4, 5);
		}

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Modokee_Cave;
			}
		}

	}
}
