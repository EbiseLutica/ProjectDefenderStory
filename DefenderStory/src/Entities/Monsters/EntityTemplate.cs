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
				return null;
			}
		}

		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Other;
			}
		}

		public EntityTemplate(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
		}

		public override void SetKilledAnime()
		{
			
		}

		public override void SetCrushedAnime()
		{
			
		}

		public override void onUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			base.onUpdate(ks);
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			return this;
		}

	}
}
