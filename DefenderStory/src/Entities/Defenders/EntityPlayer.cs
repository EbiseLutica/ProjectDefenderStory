using System;
using System.Drawing;
using Codeplex.Data;
using TakeUpJewel.AI;
using TakeUpJewel.Data;
using TakeUpJewel.Util;
using Object = TakeUpJewel.Data.Object;

namespace TakeUpJewel.Entities
{
	public enum PlayerForm
	{
		Big,
		Fire,
		Ice,
		Magic
	}

	[EntityRegistry("Player", 0)]
	public class EntityPlayer : EntityLiving
	{
		private int _flowtimer;
		private bool _lshifted;
		private float _spdAddition;
		private float _spddivition;
		private float _spdlimit;

		/// <summary>
		///     アイテムによって無敵になったかどうか。
		/// </summary>
		public bool IsItemMuteki;

		/// <summary>
		///     無敵時間。0のときは無敵ではないが、0以上の時は無敵である。
		/// </summary>
		public int MutekiTime;

		/// <summary>
		///     パワーアップ時間。0より大きいと、Big : Mini 間のパワーアップアニメを再生する。
		/// </summary>
		public int PowerupTime;

		public EntityPlayer(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			CollisionAIs.Add(new AiKillMonster(this));

			IsOnLand = true;
			Form = PlayerForm.Big;
			Size = new Size(12, 14);
		}

		public int Life { get; set; } = 5;

		/// <summary>
		///     現在のパワーアップ状態を取得します。
		/// </summary>
		public PlayerForm Form { get; set; }

		public override int[] ImageHandle
		{
			get
			{
				switch (Form)
				{
					default:
						return GameEngine.CurrentGender == PlayerGender.Male ? ResourceUtility.BigPlayer : ResourceUtility.BigPlayerFemale;
					case PlayerForm.Ice:
						return GameEngine.CurrentGender == PlayerGender.Male ? ResourceUtility.IcePlayer : ResourceUtility.IcePlayerFemale;
					case PlayerForm.Magic:
						return GameEngine.CurrentGender == PlayerGender.Male
							? ResourceUtility.MagicPlayer
							: ResourceUtility.MagicPlayerFemale;
					case PlayerForm.Fire:
						return GameEngine.CurrentGender == PlayerGender.Male
							? ResourceUtility.FirePlayer
							: ResourceUtility.FirePlayerFemale;
				}
			}
		}


		public override RectangleF Collision => new RectangleF(new PointF(2, 2), Size);

		public override EntityGroup MyGroup => EntityGroup.Friend;

		public override Sounds KilledSound => Sounds.PlayerMiss;

		public override void OnUpdate(Status ks)
		{
			base.OnUpdate(ks);
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
				if ((MutekiTime == 90) && IsItemMuteki)
					SoundUtility.PlaySound(Sounds.WarningMuteki);
				if ((MutekiTime == 0) && IsItemMuteki)
				{
					IsItemMuteki = false;
					GameEngine.BgmStop();
					GameEngine.BgmPlay(GameEngine.Areainfo.Music);
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
			if ((Velocity.X < 0.1f) && (Velocity.X > -0.1f))
				AnimeSpeed = 0;
			else
				AnimeSpeed = (int) (10 - _spdlimit + (_spdlimit - Math.Abs(Velocity.X)) * 10);
		}

		public void InputControl(Status ks)
		{
			if (ks.Inleft)
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

				Velocity.X -= _spdAddition;
				if (Velocity.X < -_spdlimit)
					Velocity.X = -_spdlimit;
				//}
			}
			else if (ks.Inright)
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
				Velocity.X += _spdAddition;
				if (Velocity.X > _spdlimit)
					Velocity.X = _spdlimit;
				//}
			}
			else
			{
				Velocity.X *= _spddivition;
				if ((Velocity.X < 0.1f) && (Velocity.X > -0.1f))
					Velocity.X = 0;
			}


			if (ks.Inz1)
			{
				if (!IsOnLand && ks.Inleft && (CollisionLeft() == ObjectHitFlag.Hit))
				{
					Velocity.X = 3f;
					Velocity.Y = -4f;
					SoundUtility.PlaySound(Sounds.Destroy);
				}
				else if (!IsOnLand && ks.Inright && (CollisionRight() == ObjectHitFlag.Hit))
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
					else if (IsOnLand || (_flowtimer < 10))
					{
						if (!IsJumping)
							SoundUtility.PlaySound(Sounds.BigJump);
						Velocity.Y = -3.6f - Math.Abs(Velocity.X) / 6.5f;
						Move();
					}
					IsJumping = true;
				}
				ks.Inz1 = false;
			}
			if (IsOnLand)
				_flowtimer = 0;
			else
				_flowtimer++;
			//if (!ks.inz && IsJumping)
			//	Velocity.Y += 0.1f;

			if (!ks.Inz && IsJumping)
				Velocity.Y += 0.1f;

			//FontUtility.DrawMiniString((int)Location.X + ks.camera.X, (int)Location.Y + ks.camera.Y - 10, Velocity.Y.ToString(), 0xffffff);
			if (ks.Inlshift)
			{
				_spdAddition = 0.4f;
				_spddivition = 0.9f;
				_spdlimit = 2.7f;
				if (!_lshifted)
					switch (Form)
					{
						case PlayerForm.Fire:
							SoundUtility.PlaySound(Sounds.ShootFire);
							Parent.Add(
								new EntityFireWeapon(Location, Mpts, Map, Parent).SetEntityData(
									DynamicJson.Parse(@"{""SpeedX"": " +
													  (Direction == Direction.Right ? EntityFireWeapon.SpeedX : -EntityFireWeapon.SpeedX) + "}")));
							break;
						case PlayerForm.Ice:
							SoundUtility.PlaySound(Sounds.ShootFire);
							Parent.Add(
								new EntityIceWeapon(Location, Mpts, Map, Parent).SetEntityData(
									DynamicJson.Parse(@"{""SpeedX"": " +
													  (Direction == Direction.Right ? EntityIceWeapon.SpeedX : -EntityIceWeapon.SpeedX) + "}")));
							break;
						case PlayerForm.Magic:
							SoundUtility.PlaySound(Sounds.ShootFire);
							Parent.Add(new EntityLeafWeapon(Location, Mpts, Map, Parent));
							break;
					}

				foreach (EntityLiving entity in Parent.FindEntitiesByType<EntityLiving>())
				{
					var e = entity;
					if (e.IsDying)
						continue;
					if ((MutekiTime > 0) && IsItemMuteki &&
						((e.MyGroup == EntityGroup.Enemy) || (e.MyGroup == EntityGroup.MonsterWeapon)) &&
						new RectangleF(e.Location.X, e.Location.Y, e.Size.Width, e.Size.Height).CheckCollision(new RectangleF(Location.X,
							Location.Y, Size.Width, Size.Height)))
						e.Kill();
				}

				foreach (EntityTurcosShell e in Parent.FindEntitiesByType<EntityTurcosShell>())
				{
					if (e.IsRunning)
						continue;
					//if (e.mutekitime > 0)
					//	continue;
					if (new RectangleF(Location, Size).CheckCollision(new RectangleF(e.Location, e.Size)))
						e.Owner = this;
				}
				_lshifted = true;
			}
			else
			{
				_lshifted = false;
				_spdAddition = 0.2f;
				_spddivition = 0.9f;
				_spdlimit = 1.4f;
			}
		}

		public override void OnOutOfWater()
		{
			Velocity.Y = -3.2f;
			base.OnOutOfWater();
		}

		/// <summary>
		///     この EntityPlayer を殺害します。
		/// </summary>
		public override void Kill()
		{
			if (IsDying)
				return;
			if (IsFall)
			{
				base.Kill();
				Velocity = Vector.Zero;
				Life = 0;
				SoundUtility.PlaySound(Sounds.PlayerMiss);
				return;
			}
			if (GameEngine.Time == 0)
			{
				SetGraphic(5);
				base.Kill();
				return;
			}
			if (MutekiTime > 0)
				return;
			SoundUtility.PlaySound(Sounds.PowerDown);
			MutekiTime = 240;
			Life--;

			if (Form != PlayerForm.Big)
				Form = PlayerForm.Big;

			if (Life < 1)
			{
				SetGraphic(5);
				base.Kill();
				//SoundUtility.PlaySound(Sounds.PlayerMiss);
			}

			Velocity *= 0;
		}

		public override void OnDraw(PointF p, Status ks)
		{
			if ((MutekiTime > 0) && (PowerupTime == 0))
			{
				if (MutekiTime % 8 < 4)
					base.OnDraw(p, ks);
			}
			else
				base.OnDraw(new PointF(p.X, p.Y + ((GameEngine.Tick % 8 < 4) && (PowerupTime > 0) ? 16 : 0)), ks);
		}

		internal void PowerUp(PlayerForm f)
		{
			switch (f)
			{
				case PlayerForm.Fire:
				case PlayerForm.Magic:
				case PlayerForm.Ice:
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
			GameEngine.BgmPlay("muteki.mid");
		}
	}
}