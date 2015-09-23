using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefenderStory.Entities;
using DefenderStory.Util;
using System.Drawing;
using DefenderStory.Data;
using DefenderStory.AI;
using Codeplex.Data;

namespace DefenderStory.Entities
{

	public enum PlayerForm
	{
		Mini,
		Big,
		Fire,
		Ice,
		Leaf,
	}

	[EntityRegistry("Player", 0)]
    public class EntityPlayer : EntityLiving
	{
		/// <summary>
		/// 現在のパワーアップ状態を取得します。
		/// </summary>
		public PlayerForm Form
		{
			get; set;
		}

		/// <summary>
		/// 無敵時間。0のときは無敵ではないが、0以上の時は無敵である。
		/// </summary>
		public int MutekiTime = 0;

		/// <summary>
		/// パワーアップ時間。0より大きいと、Big : Mini 間のパワーアップアニメを再生する。
		/// </summary>
		public int PowerupTime = 0;

		/// <summary>
		/// アイテムによって無敵になったかどうか。
		/// </summary>
		public bool IsItemMuteki = false;
		private float spdAddition;
		private float spdlimit;
		private float spddivition;
		private bool lshifted;

		public override int[] ImageHandle
		{
			get
			{
				if (PowerupTime > 0)
				{
					if (GameEngine.tick % 8 < 4)
					{
						return ResourceUtility.MiniPlayer;
					}
					else
					{

						return ResourceUtility.BigPlayer;
					}
				}
				switch (Form)
				{
					default:
						return ResourceUtility.MiniPlayer;
					case PlayerForm.Big:
						return ResourceUtility.BigPlayer;
					case PlayerForm.Ice:
						return ResourceUtility.IcePlayer;
					case PlayerForm.Leaf:
						return ResourceUtility.LeafPlayer;
					case PlayerForm.Fire:
						return ResourceUtility.FirePlayer;
				}
			}
		}
		

		

		public override RectangleF Collision
		{
			get
			{
				return new RectangleF(new PointF(2, 2), Size);
			}
		}

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Defender;
			}
		}

		public override void onUpdate(Status ks)
		{
			base.onUpdate(ks);
			if (Form == PlayerForm.Mini && Size.Height != 14)
				Size = new Size(12, 14);
			if (Form != PlayerForm.Mini && Size.Height != 30)
				Size = new Size(12, 30);
			if (!IsDying)
				InputControl(ks);
			AnimeControl();
			if (!IsDying)
				ProcessMuteki();
			//FontUtility.DrawMiniString((int)Location.X - 9 + GameEngine.camera.X, (int)Location.Y + GameEngine.camera.Y, "" + AnimeSpeed, 0xffffff);
		}

		public override void Move()
		{
			if (PowerupTime == 0)
				base.Move();
		}

		public override void UpdateGravity()
		{
			if (PowerupTime == 0)
				base.UpdateGravity();
		}

		private void ProcessMuteki()
		{
			if (PowerupTime > 0)
				PowerupTime--;
			if (MutekiTime > 0)
			{
				MutekiTime--;
				if (MutekiTime == 90 && IsItemMuteki)
				{
					SoundUtility.PlaySound(Sounds.WarningMuteki);
				}
				if (MutekiTime == 0 && IsItemMuteki)
				{
					IsItemMuteki = false;
					GameEngine.BGMStop();
					GameEngine.BGMPlay(GameEngine.areainfo.Music);
				}
			}
		}

		private void AnimeControl()
		{
			switch (Direction)
			{
				case Direction.Left:
					if (IsOnLand)
						SetAnime(0, 3, 8);
					else
						SetGraphic(4);
					if (IsInWater)
						SetAnime(18, 21, 8);
					break;
				case Direction.Right:
					if (IsOnLand)
						SetAnime(6, 9, 8);
					else
						SetGraphic(10);
					if (IsInWater)
						SetAnime(22, 25, 8);
					break;
			}
			if (Velocity.X < 0.1f && Velocity.X > -0.1f)
				AnimeSpeed = 0;
			else
				AnimeSpeed = (int)(10 - spdlimit + (spdlimit - Math.Abs(Velocity.X)) * 10);
		}

		public void InputControl(Status ks)
		{
			if (ks.inleft)
			{
				Direction = Direction.Left;
				/*
				if (Velocity.X > 0)
				{
					Velocity.X *= spddivition;
					if (Velocity.X < 0.4f && Velocity.X > -0.4f)
						Velocity.X = 0;
				}
				else
				{*/

				Velocity.X -= spdAddition;
				if (Velocity.X < -spdlimit)
					Velocity.X = -spdlimit;
				//}
			}
			else if (ks.inright)
			{
				Direction = Direction.Right;
				/*if (Velocity.X < 0)
				{
					Velocity.X *= spddivition;
					if (Velocity.X < 0.4f && Velocity.X > -0.4f)
						Velocity.X = 0;
				}
				else
				{*/
				Velocity.X += spdAddition;
				if (Velocity.X > spdlimit)
					Velocity.X = spdlimit;
				//}
			}
			else
			{
				Velocity.X *= spddivition;
				if (Velocity.X < 0.1f && Velocity.X > -0.1f)
					Velocity.X = 0;
			}



			if (ks.inz1)
			{

				if (Form != PlayerForm.Mini && !IsOnLand && ks.inleft && CollisionLeft() == ObjectHitFlag.Hit)
				{
					Velocity.X = 3f;
					Velocity.Y = -4f;
					SoundUtility.PlaySound(Sounds.Destroy);
				}
				else if (Form != PlayerForm.Mini && !IsOnLand && ks.inright && CollisionRight() == ObjectHitFlag.Hit)
				{
					Velocity.X = -3f;
					Velocity.Y = -4f;
					SoundUtility.PlaySound(Sounds.Destroy);
				}
				else
				{
					if (IsInWater)
					{

						SoundUtility.PlaySound(Sounds.Swim);
						Velocity.Y = -2f;
						IsJumping = false;
						IsOnLand = true;
					}
					else if (IsOnLand)
					{
						if (!IsJumping)
						{
							if (Form == PlayerForm.Mini)
								SoundUtility.PlaySound(Sounds.SmallJump);
							else
								SoundUtility.PlaySound(Sounds.BigJump);
						}
						Velocity.Y = -3.6f - Math.Abs(Velocity.X) / 6.5f;
						Move();
					}
					IsJumping = true;
				}
				ks.inz1 = false;
			}
			//if (!ks.inz && IsJumping)
			//	Velocity.Y += 0.1f;

			if (!ks.inz && IsJumping)
			{
				Velocity.Y += 0.1f;
			}

			//FontUtility.DrawMiniString((int)Location.X + ks.camera.X, (int)Location.Y + ks.camera.Y - 10, Velocity.Y.ToString(), 0xffffff);
			if (ks.inlshift)
			{
				spdAddition = 0.4f;
				spddivition = 0.9f;
				spdlimit = 2.7f;
				if (!lshifted)
				{
					switch (Form)
					{
						case PlayerForm.Fire:
							SoundUtility.PlaySound(Sounds.ShootFire);
							Parent.Add(new EntityFireWeapon(Location, Mpts, Map, Parent).SetEntityData(DynamicJson.Parse(@"{""Speed"": " + (Direction == Direction.Right ? EntityFireWeapon.SPEED_X : -EntityFireWeapon.SPEED_X) + "}")));
							break;
					}
				}

				foreach (EntityLiving e in Parent.FindEntitiesByType<EntityLiving>())
				{
					if (e.IsDying)
						continue;
					if (this.MutekiTime > 0 && this.IsItemMuteki && (e.MyGroup == EntityGroup.Monster || e.MyGroup == EntityGroup.MonsterWeapon) && new RectangleF(e.Location.X, e.Location.Y, e.Size.Width, e.Size.Height).CheckCollision(new RectangleF(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height)))
					{
						e.Kill();
					}

				}

				foreach (EntityTurcosShell e in Parent.FindEntitiesByType<EntityTurcosShell>())
				{
					if (e.isRunning)
						continue;
					if (e.mutekitime > 0)
						continue;
					if (new RectangleF(Location, Size).CheckCollision(new RectangleF(e.Location, e.Size)))
					{
						e.Owner = this;
					}
				}
				lshifted = true;
			}
			else
			{
				lshifted = false;
				spdAddition = 0.2f;
				spddivition = 0.9f;
				spdlimit = 1.4f;
			}

		}

		public override void onOutOfWater()
		{
			Velocity.Y = -3.2f;
			base.onOutOfWater();
		}

		public override Sounds KilledSound => Sounds.PlayerMiss;

		public EntityPlayer(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			CollisionAIs.Add(new AIKillMonster(this));

			IsOnLand = true;
			Form = PlayerForm.Mini;
			Size = new Size(12, 14);
		}

		/// <summary>
		/// この EntityPlayer を殺害します。
		/// </summary>
		public override void Kill()
		{
			if (IsDying)
				return;
			if (IsFall)
			{
				base.Kill();
				//SoundUtility.PlaySound(Sounds.PlayerMiss);
				return;
			}
			if (MutekiTime > 0)
				return;
			if (Form == PlayerForm.Mini)
			{
				SetGraphic(5);
				base.Kill();
				//SoundUtility.PlaySound(Sounds.PlayerMiss);
			}
			else if (Form == PlayerForm.Big)
			{
				Form = PlayerForm.Mini;
				SoundUtility.PlaySound(Sounds.PowerDown);
				MutekiTime = 240;
				this.Location.Y += 16;
				this.Size = new Size(12, 14);
			}
			else
			{
				Form = PlayerForm.Big;
				SoundUtility.PlaySound(Sounds.PowerDown);
				MutekiTime = 240;
			}
			this.Velocity = Vector.Zero;
		}

		public override void onDraw(PointF p, Status ks)
		{
			if (MutekiTime > 0 && PowerupTime == 0)
			{
				if (MutekiTime % 8 < 4)
					base.onDraw(p, ks);
			}
			else
				base.onDraw(new PointF(p.X, p.Y + ((GameEngine.tick % 8 < 4 && PowerupTime > 0) ? 16 : 0)), ks);
		}

		internal void PowerUp(PlayerForm f)
		{
			switch (f)
			{
				case PlayerForm.Big:
					SoundUtility.PlaySound(Sounds.PowerUp);
					if (Form != PlayerForm.Mini)
						break;
					PowerupTime = 80;
					MutekiTime = 120;
					Location.Y -= 16;
					Size = new Size(12, 30);
					break;
				case PlayerForm.Fire:
				case PlayerForm.Leaf:
				case PlayerForm.Ice:
					if (Form == PlayerForm.Mini)
						Location.Y -= 16;
					SoundUtility.PlaySound(Sounds.GetWeapon);
					Size = new Size(12, 30);
					break;
			}
			Form = f;
		}

		public override void SetKilledAnime()
		{
			SetGraphic(5);
		}

		public override void SetCrushedAnime()
		{
			SetGraphic(5);
		}

		internal void SetMuteki()
		{
			IsItemMuteki = true;
			MutekiTime = 600;
			
			SoundUtility.PlaySound(Sounds.PowerUp);
			GameEngine.BGMPlay("muteki.mid");
		}
	}


}
