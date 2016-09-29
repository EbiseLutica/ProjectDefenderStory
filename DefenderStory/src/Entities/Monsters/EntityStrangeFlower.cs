using System.Collections.Generic;
using System.Drawing;
using TakeUpJewel.Data;
using TakeUpJewel.Util;

namespace TakeUpJewel.Entities
{
	[EntityRegistry("StrangeFlower", 13)]
	public class EntityStrangeFlower : EntityLiving, IScaffold
	{
		private int _nowstatus;
		private int _tick;

		public EntityStrangeFlower(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(48, 48);
		}

		public override int[] ImageHandle => ResourceUtility.StrangeFlower;


		public override EntityGroup MyGroup => EntityGroup.Stage;

		public override RectangleF Collision => new RectangleF(16, 16, 16, 32);

		PointF IScaffold.Location => Location;

		public override void SetKilledAnime()
		{
		}

		public override void SetCrushedAnime()
		{
		}

		public override void OnUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			UpdateStatus();
			//int x = (int)(ks.camera.X + Location.X);
			//int y = (int)(ks.camera.Y + Location.Y);

			//DxLibDLL.DX.DrawBox(x, y, x + Size.Width, y + Size.Height, DxLibDLL.DX.GetColor(255, 0, 0), 1);
			base.OnUpdate(ks);
		}

		public void UpdateStatus()
		{
			switch (_nowstatus)
			{
				case 0: // 構え
					SetGraphic(0);
					foreach (EntityLiving sp in new List<Entity>(Parent.FindEntitiesByType<EntityLiving>()))
					{
						if ((sp.MyGroup != EntityGroup.Friend) && (sp.MyGroup != EntityGroup.Enemy))
							continue;

						if (sp.IsDying)
							continue;
						if (new Rectangle((int) (sp.Location.X + sp.Collision.Left), (int) (sp.Location.Y + sp.Collision.Bottom),
							(int) sp.Collision.Width, 3).CheckCollision(
							new Rectangle((int) (Location.X + Collision.Left), (int) (Location.Y + Collision.Y), (int) Collision.Width,
								(int) Collision.Height)))
						{
							_nowstatus = 1;
							_tick = -1;
							sp.Velocity = Vector.Zero;
						}
					}

					if (_tick > 240)
					{
						_tick = -1;
						_nowstatus++;

						SoundUtility.PlaySound(Sounds.Paku1);
					}
					break;
				case 1: // 閉じようとする
					SetGraphic(1);
					foreach (EntityLiving sp in new List<Entity>(Parent.FindEntitiesByType<EntityLiving>()))
					{
						if ((sp.MyGroup != EntityGroup.Friend) && (sp.MyGroup != EntityGroup.Enemy))
							continue;

						if (sp.IsDying)
							continue;
						if (new Rectangle((int) (sp.Location.X + sp.Collision.Left), (int) (sp.Location.Y + sp.Collision.Bottom),
							(int) sp.Collision.Width, 3).CheckCollision(
							new Rectangle((int) (Location.X + Collision.Left), (int) (Location.Y + Collision.Y), (int) Collision.Width,
								(int) Collision.Height)))
							sp.Velocity = Vector.Zero;
					}
					if (_tick > 30)
					{
						_tick = -1;
						_nowstatus++;
						SoundUtility.PlaySound(Sounds.Paku2);
						foreach (EntityLiving sp in new List<Entity>(Parent.FindEntitiesByType<EntityLiving>()))
						{
							if ((sp.MyGroup != EntityGroup.Friend) && (sp.MyGroup != EntityGroup.Enemy))
								continue;

							if (sp.IsDying)
								continue;
							if ((sp != this) &&
								new RectangleF(sp.Location.X, sp.Location.Y, sp.Size.Width, sp.Size.Height).CheckCollision(
									new RectangleF(Location.X, Location.Y, Size.Width, Size.Height)))
							{
								sp.Kill();
								_nowstatus = 2;
								_tick = -1;
							}
						}
					}
					break;
				case 2: // 閉じる
					SetGraphic(2);
					if (_tick > 120)
					{
						_tick = -1;
						_nowstatus++;
					}
					break;
				case 3: // むずむず
					if ((_tick == 0) || (_tick == 30))
					{
						SoundUtility.PlaySound(Sounds.Poyo);
						SetGraphic(3);
					}
					else if ((_tick == 15) || (_tick == 60))
					{
						SoundUtility.PlaySound(Sounds.Poyo);
						SetGraphic(4);
					}
					if (_tick > 70)
					{
						_nowstatus++;
						_tick = -1;
					}

					break;
				case 4: // 開けようとする
					SetGraphic(1);
					if (_tick > 30)
					{
						_tick = -1;
						_nowstatus = 0;
					}
					break;
			}
			_tick++;
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			return this;
		}
	}
}