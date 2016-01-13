using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;

namespace DefenderStory.Entities
{
	[EntityRegistry("EntityBlackServer", 85)]
	public class EntityBlackServer : EntityLiving
	{

		/// <summary>
		/// AIのモード。
		/// </summary>
		int nowstatus = 0;
		/// <summary>
		/// タイマー。
		/// </summary>
		int tick = 0;

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.BlackServer;
			}
		}

		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Monster;
			}
		}

		public EntityBlackServer(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 32);
			CollisionAIs.Add(new AIKillDefender(this));

		}

		public override void SetKilledAnime()
		{
			AnimeSpeed = 0;
		}

		public override void SetCrushedAnime()
		{
			AnimeSpeed = 0;
			IsCrushed = false;
		}

		public override void onUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。

			switch (nowstatus)
			{
				case 0:
				case 3:
					if (tick > 30)
					{
						nowstatus++;
						tick = -1;
					}
					break;
				case 1:
				case 2:
				case 4:
				case 5:
					if (tick > 8)
					{
						if (nowstatus == 5)
							nowstatus = 0;
						else
							nowstatus++;
						tick = -1;
						if (nowstatus == 2 && !IsDying)
							Parent.Add(new EntitySaba(this.Location, Mpts, Map, Parent)); ;//鯖を出現
					}
					break;
			}

			tick++;
			SetGraphic(nowstatus);
			base.onUpdate(ks);
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			return this;
		}

	}

	[EntityRegistry("EntitySaba", -1)]
	public class EntitySaba : EntityFlying
	{


		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.BlackServer;
			}
		}

		public override Sounds KilledSound => Sounds.Null;

		public override EntityGroup MyGroup => EntityGroup.Monster;

		int mode = 6;
		int tick = 0;
		public EntitySaba(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			CollisionAIs.Add(new AIKillDefender(this));

			Velocity = new Data.Vector(-2.3f, 0);
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

		public override RectangleF Collision
		{
			get
			{
				return new RectangleF(0, 16, 16, 8);
			}
		}

		public override void onUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			if (mode < 9 && tick > 8)
				mode++;
			tick++;
			SetGraphic(mode);

			base.onUpdate(ks);
			var a = CollisionLeft();
			if (!IsDying && a != Data.ObjectHitFlag.NotHit && a != Data.ObjectHitFlag.InWater)
				Kill();
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			return this;
		}

	}


}
