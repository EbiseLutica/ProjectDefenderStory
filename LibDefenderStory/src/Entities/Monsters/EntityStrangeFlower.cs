using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;
using DefenderStory.Data;
using System.Collections.Generic;

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
			Size = new Size(48, 48);
		}

		public override RectangleF Collision
		{
			get
			{
				return new RectangleF(16, 16, 16, 32);
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
			//int x = (int)(ks.camera.X + Location.X);
			//int y = (int)(ks.camera.Y + Location.Y);

			//DxLibDLL.DX.DrawBox(x, y, x + Size.Width, y + Size.Height, DxLibDLL.DX.GetColor(255, 0, 0), 1);
			base.onUpdate(ks);
		}

		

		int nowstatus, tick;

		public void UpdateStatus()
		{
			switch (nowstatus)
			{
				case 0: // 構え
					SetGraphic(0);
					foreach (EntityLiving sp in new List<Entity>(Parent.FindEntitiesByType<EntityLiving>()))
					{
						if (sp.MyGroup != EntityGroup.Defender && sp.MyGroup != EntityGroup.Monster)
							continue;

						if (sp.IsDying)
							continue;
						if (new Rectangle((int)(sp.Location.X + sp.Collision.Left), (int)(sp.Location.Y + sp.Collision.Bottom), (int)sp.Collision.Width, 3).CheckCollision(
							new Rectangle((int)(Location.X + Collision.Left), (int)(Location.Y + Collision.Y), (int)Collision.Width, (int)Collision.Height)))
						{
							nowstatus = 1;
							tick = -1;
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
					foreach (EntityLiving sp in new List<Entity>(Parent.FindEntitiesByType<EntityLiving>()))
					{
						if (sp.MyGroup != EntityGroup.Defender && sp.MyGroup != EntityGroup.Monster)
							continue;

						if (sp.IsDying)
							continue;
						if (new Rectangle((int)(sp.Location.X + sp.Collision.Left), (int)(sp.Location.Y + sp.Collision.Bottom), (int)sp.Collision.Width, 3).CheckCollision(
							new Rectangle((int)(Location.X + Collision.Left), (int)(Location.Y + Collision.Y), (int)Collision.Width, (int)Collision.Height)))
						{
							sp.Velocity = Vector.Zero;
						}
					}
					if (tick > 30)
					{
						tick = -1;
						nowstatus++;
						SoundUtility.PlaySound(Sounds.Paku2);
						foreach (EntityLiving sp in new List<Entity>(Parent.FindEntitiesByType<EntityLiving>()))
						{
							if (sp.MyGroup != EntityGroup.Defender && sp.MyGroup != EntityGroup.Monster)
								continue;

							if (sp.IsDying)
								continue;
							if (sp != this && new RectangleF(sp.Location.X, sp.Location.Y, sp.Size.Width, sp.Size.Height).CheckCollision(new RectangleF(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height)))
							{
								sp.Kill();
								nowstatus = 2;
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
			tick++;
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			return this;
		}

	}
}
