using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;
using DefenderStory.Data;

namespace DefenderStory.Entities
{
	[EntityRegistry("StrangeFlower", 13)]
	public class EntityStrangeFlower : EntityLiving, IScaffold
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.StrangeFlower;
			}
		}

		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Stage;
			}
		}

		public EntityStrangeFlower(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(64, 64);
		}

		public override RectangleF Collision
		{
			get
			{
				return new RectangleF(0, 32, 64, 32);
			}
		}

		PointF IScaffold.Location
		{
			get
			{
				return Location;
			}
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
			UpdateStatus();

			base.onUpdate(ks);
		}

		

		int nowstatus, tick;

		public void UpdateStatus()
		{
			switch (nowstatus)
			{
				case 0: // 構え
					SetGraphic(0);
					foreach (EntityLiving sp in Parent.FindEntitiesByType<EntityLiving>())
					{
						if (sp.MyGroup != EntityGroup.Defender && sp.MyGroup != EntityGroup.Monster)
							continue;

						if (sp.IsDying)
							continue;
						if (sp != this && new RectangleF(sp.Location.X, sp.Location.Y, sp.Size.Width, sp.Size.Height).CheckCollision(new RectangleF(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height)))
						{
							sp.Velocity = Vector.Zero;
						}
					}
					if (tick > 240)
					{
						tick = -1;
						nowstatus++;
						
						SoundUtility.PlaySound(Sounds.Paku1);
					}
					break;
				case 1: // 閉じようとする
					SetGraphic(1);
					if (tick > 30)
					{
						tick = -1;
						nowstatus++;
						SoundUtility.PlaySound(Sounds.Paku2);
						foreach (EntityLiving sp in Parent.FindEntitiesByType<EntityLiving>())
						{
							if (sp.MyGroup != EntityGroup.Defender && sp.MyGroup != EntityGroup.Monster)
								continue;

							if (sp.IsDying)
								continue;
							if (sp != this && new RectangleF(sp.Location.X, sp.Location.Y, sp.Size.Width, sp.Size.Height).CheckCollision(new RectangleF(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height)))
							{
								sp.IsDying = true;
								tick = -1;
							}
						}
					}
					break;
				case 2: // 閉じる
					SetGraphic(2);
					if (tick > 120)
					{
						tick = -1;
						nowstatus++;
					}
					break;
				case 3: // むずむず
					if (tick == 0 || tick == 30)
					{
						SoundUtility.PlaySound(Sounds.Poyo);
						SetGraphic(3);
					}
					else if (tick == 15 || tick == 60)
					{
						SoundUtility.PlaySound(Sounds.Poyo);
						SetGraphic(4);
					}
					if (tick > 70)
					{
						nowstatus++;
						tick = -1;
					}

					break;
				case 4: // 開けようとする
					SetGraphic(1);
					if (tick > 30)
					{
						tick = -1;
						nowstatus = 0;
					}
					break;
			}
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			return this;
		}

	}
}
