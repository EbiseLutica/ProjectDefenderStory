using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;
using System.Collections.Generic;

namespace DefenderStory.Entities
{
	[EntityRegistry("Densy", 88)]
	public class EntityDensy : EntityFlying
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Densy;
			}
		}

		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Monster;
			}
		}

		public EntityDensy(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			this.MainAI = new AIFlySearch(this, 2, 0, 3, 0, 3);
			this.CollisionAIs.Add(new AIKillDefender(this));
		}

		public override void SetKilledAnime()
		{
			AnimeSpeed = 0;
		}

		public override void SetCrushedAnime()
		{
			IsCrushed = false;
			AnimeSpeed = 0;
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
