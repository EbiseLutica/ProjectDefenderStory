using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;
using DxLibDLL;

namespace DefenderStory.Entities
{
	[EntityRegistry("CameraMan", 87)]
	public class EntityCameraMan : EntityLiving
	{

		int tick = 0;
		bool isTension = false;

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.CameraMan;
			}
		}

		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Monster;
			}
		}

		public EntityCameraMan(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 32);
			CollisionAIs.Add(new AIKillDefender(this));
			Jump();
		}

		public override void SetKilledAnime()
		{
			
		}

		public override void SetCrushedAnime()
		{
			IsCrushed = false;
			AnimeSpeed = 0;
		}

		/// <summary>
		/// アニメ用タイマー。
		/// </summary>
		int timer = 0;

		int panime = 0;
		bool isjumping = false;
		public void Jump()
		{
			/*if (!IsJumping)
			{
				if (timer > 8)
				{
					timer = -1;
					panime++;
					if (panime == 3)
					{
						IsJumping = true;
						Velocity.Y = -3.4f;
					}
				}
				timer++;
			}
			else
			{
				if (CollisionBottom() == Data.ObjectHitFlag.Hit)
				{
					IsJumping = false;
					panime = 0;
					timer = 0;
				}
			}*/
			if (CollisionBottom() == Data.ObjectHitFlag.Hit)
			{
				//Velocity.Y = -3.4f;
				if (isjumping)
				{
					isjumping = false;
					timer = -1;
					panime = 0;
				}
				timer++;
				if (timer > 8)
				{
					timer = 0;
					if (panime < 3)
						panime++;
					if (panime == 3)
					{
						isjumping = true;
						Velocity.Y = -3.4f;
					}
                }
			}
			SetGraphic(panime);
		}

		public override void onUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			if (!IsDying)
			{
				if (!isTension)
				{
					if (Math.Abs(Location.X - Parent.MainEntity.Location.X) < 64 && Location.Y <= Parent.MainEntity.Location.Y && Parent.MainEntity.Location.Y <= Location.Y + Size.Height)
					{
						isTension = true;
						tick = 0;
						Velocity.Y = 0.2f;
					}
				}
				else
				{
					if (tick > 180)
						tick = -1;
					tick++;
					if (tick == 0)
					{

						//TODO: 効果音発生
						
						if (Math.Abs(Location.X - Parent.MainEntity.Location.X) >= 64 && Location.Y > Parent.MainEntity.Location.Y || Parent.MainEntity.Location.Y > Location.Y + Size.Height)
						{

							isTension = false;
							tick = 0;
							goto nuke;
						}
						//SoundUtility.PlaySound(Sounds.Focus);
						SoundUtility.PlaySound(Sounds.Shutter);
					}

					if (tick > 30 && tick < 60 && tick % 4 == 0)
					{
						Parent.Add(new EntityCameraRazer(Location, Mpts, Map, Parent));
						SoundUtility.PlaySound(Sounds.Razer);
					}

					if (tick == 60)
					{
						if (Math.Abs(Location.X - Parent.MainEntity.Location.X) >= 64 && Location.Y > Parent.MainEntity.Location.Y || Parent.MainEntity.Location.Y > Location.Y + Size.Height)
						{

							isTension = false;
							tick = 0;
							goto nuke;
						}
					}
					

					SetGraphic(0);

				}
			}
			nuke:
			if (!isTension)
				Jump();
			base.onUpdate(ks);
		}

		public override void onDraw(PointF p, Status ks)
		{
			base.onDraw(p, ks);
			if (tick > 0 && tick < 8 && isTension)
			{
				DX.DrawGraphF(p.X - 10, p.Y - 2, ImageHandle[4], 1);

			}
		}

		public override void onDebugDraw(PointF p, Status ks)
		{
			base.onDebugDraw(p, ks);
			FontUtility.DrawMiniString((int)p.X + 4, (int)p.Y - 12, $"{panime} {timer}", 0xffffff);
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			return this;
		}

	}

	[EntityRegistry("CameraRazer", -1)]
	public class EntityCameraRazer : EntityFlying
	{

	

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.CameraMan;
			}
		}



		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.MonsterWeapon;
			}
		}

		public EntityCameraRazer(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 32);
			//CollisionAIs.Add(new AIKillDefender(this));
			SetGraphic(5);
			Velocity.X = -4f;
		}

		public override void SetKilledAnime()
		{

		}

		public override void SetCrushedAnime()
		{

		}

		public override void CheckCollision()
		{
		
		}

		public override void Kill()
		{
			IsDead = true;
		}

		public override void onUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			if (Location.X < -Size.Width || Location.Y < -Size.Height || Location.X > ks.map.Width * 16 || Location.Y > ks.map.Height * 16)
				Kill();
			foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
			{
				if (ep.IsDying)
					continue;
				if (new Rectangle((int)ep.Location.X, (int)(ep.Location.Y), (int)ep.Size.Width, (int)ep.Size.Height)
					.CheckCollision(new RectangleF(Location.X, Location.Y + 4, 16, 2)))
				{
					ep.Kill();
				}

			}
			base.onUpdate(ks);
		}
		

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			return this;
		}

	}

}
