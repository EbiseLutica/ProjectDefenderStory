using System;
using System.Drawing;
using System.Globalization;
using Codeplex.Data;
using DxLibDLL;
using TakeUpJewel.AI;
using TakeUpJewel.Data;
using TakeUpJewel.Util;
using static TakeUpJewel.Util.DevelopUtility;
using Object = TakeUpJewel.Data.Object;

namespace TakeUpJewel.Entities
{
	public enum QueenBossBehaviorOption
	{
		Waiting1,
		ThrowWeaponToLeft,
		MoveToLeft,
		Waiting2,
		ThrowWeaponToRight,
		MoveToRight
	}

	public enum KingBossBehaviorOption
	{
		JumpToLeft,
		Waiting1,
		ThrowChocolateToRight,
		JumpToRight,
		Waiting2,
		ThrowChocolateToLeft
	}

	[EntityRegistry("QueenBoss", 92)]
	public class EntityQueenBoss : EntityLiving
	{
		public const int MaxLife = 20;

		private readonly float _left;
		private string _endScript;

		private PointF _firstLoc;
		private bool _isBattling;

		private QueenBossBehaviorOption _nowBehavior;

		private int _startKyori = 256;

		private string _startScript;
		private float _top;
		protected float InternalGravity;


		public override int DyingMax => 0;

		/// <summary>
		///     無敵時間。0のときは無敵ではないが、0以上の時は無敵である。
		/// </summary>
		public int MutekiTime;

		protected int Tick;

		public EntityQueenBoss(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			CollisionAIs.Add(new AiKillDefender(this));
			InternalGravity = 0.1f;
			IsOnLand = true;
			_firstLoc = pnt;
			Tick = 50;
			_left = _firstLoc.X - 200;
			Size = new Size(12, 30);
		}

		public int Life { get; set; } = MaxLife;

		public override int[] ImageHandle => ResourceUtility.Queen;


		public override RectangleF Collision => new RectangleF(new PointF(2, 2), Size);

		public override EntityGroup MyGroup => EntityGroup.Enemy;

		public override float Gravity => InternalGravity;

		public override void OnUpdate(Status ks)
		{
			base.OnUpdate(ks);
			//FontUtility.DrawMiniString((int)Location.X - 9 + GameEngine.camera.X, (int)Location.Y + GameEngine.camera.Y, "" + AnimeSpeed, 0xffffff);


			if (!_isBattling)
			{
				// ReSharper disable once AssignmentInConditionalExpression
				if (_isBattling = Location.GetLengthTo(Parent.MainEntity.Location) < _startKyori)
					try
					{
						EventRuntime.AddScript(new EventScript(_startScript));
					}
					catch (EventScript.EventScriptException ex)
					{
						EventRuntime.AddScript(new EventScript($@"[enstop]
[mesbox:down]
[mes:""エラー！\n{ex.Message.Replace(@"\", @"\\").Replace(@"""", @"\""")}""]
[mesend]
[enstart]"));
					}

				return;
			}
			switch (_nowBehavior)
			{
				case QueenBossBehaviorOption.Waiting1:
					if (Tick == 0)
					{
						Tick = 60;
						_nowBehavior = QueenBossBehaviorOption.ThrowWeaponToLeft;
						SetGraphic(16);
					}
					break;
				case QueenBossBehaviorOption.ThrowWeaponToLeft:
					if (Tick == 30)
					{
						SoundUtility.PlaySound(Sounds.ShootArrow);
						SetGraphic(17);
						Parent.Add(
							new EntityCircusBall(Location, Mpts, Map, Parent).SetEntityData(
								DynamicJson.Parse(DynamicJson.Serialize(new {SpeedX = -2.0f}))));
					}
					if (Tick == 0)
					{
						_nowBehavior = QueenBossBehaviorOption.MoveToLeft;
						SetAnime(0, 3, 8);
						Velocity.X = -1.6f;
					}
					break;
				case QueenBossBehaviorOption.MoveToLeft:
					if ((Location.X <= _left - 16) || (Location.X <= 0) || (CollisionLeft() == ObjectHitFlag.Hit))
					{
						Tick = 60;
						_nowBehavior = QueenBossBehaviorOption.Waiting2;
						SetGraphic(6);
						Velocity.X = 0;
					}
					break;
				case QueenBossBehaviorOption.Waiting2:
					if (Tick == 0)
					{
						Tick = 60;
						_nowBehavior = QueenBossBehaviorOption.ThrowWeaponToRight;
						SetGraphic(18);
					}
					break;
				case QueenBossBehaviorOption.ThrowWeaponToRight:
					if (Tick == 30)
					{
						SoundUtility.PlaySound(Sounds.ShootArrow);
						Parent.Add(
							new EntityCircusBall(Location, Mpts, Map, Parent).SetEntityData(
								DynamicJson.Parse(DynamicJson.Serialize(new {SpeedX = 2.0f}))));
						SetGraphic(19);
					}
					if (Tick == 0)
					{
						_nowBehavior = QueenBossBehaviorOption.MoveToRight;
						SetAnime(6, 9, 8);
						Velocity.X = 1.6f;
					}
					break;
				case QueenBossBehaviorOption.MoveToRight:
					if (((Location.X >= _firstLoc.X) && (Location.X >= GameEngine.Map.Width - 1)) ||
						(CollisionRight() == ObjectHitFlag.Hit))
					{
						Tick = 60;
						_nowBehavior = QueenBossBehaviorOption.Waiting1;
						SetGraphic(6);
						Velocity.X = 0;
					}
					break;
			}

			Tick--;

			if (MutekiTime <= 0) return;
			MutekiTime--;
		}

		public override void OnDraw(PointF p, Status ks)
		{
			if (MutekiTime > 0)
			{
				if (MutekiTime % 8 < 4)
					base.OnDraw(p, ks);
			}
			else
				base.OnDraw(p, ks);
		}

		public override void OnDebugDraw(PointF p, Status ks)
		{
			FontUtility.DrawMiniString((int) p.X, (int) p.Y - 10,
				Location.GetLengthTo(Parent.MainEntity.Location).ToString(CultureInfo.CurrentCulture), Color.Red);
			base.OnDebugDraw(p, ks);
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
				return;
			}


			if (MutekiTime > 0)
				return;

			if (Life < 1)
			{
				try
				{
					EventRuntime.AddScript(new EventScript(_endScript));
				}
				catch (EventScript.EventScriptException ex)
				{
					EventRuntime.AddScript(new EventScript($@"[enstop]
[mesbox:down]
[mes:""エラー！\n{ex.Message.Replace(@"\", @"\\").Replace(@"""", @"\""")}""]
[mesend]
[enstart]"));
				}


				base.Kill();

				return;
			}

			SoundUtility.PlaySound(Sounds.PowerDown);

			MutekiTime = 180;

			Life -= IsCrushed ? MaxLife / 3 : 1;
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			_startKyori = (int) jsonobj.StartKyori;
			_startScript = jsonobj.StartScript;
			_endScript = jsonobj.EndScript;
			return this;
		}


		public override void SetKilledAnime()
		{
			SetGraphic(5);
		}

		public override void SetCrushedAnime()
		{
			SetGraphic(5);

			IsCrushed = false;
		}
	}

	[EntityRegistry("KingBoss", 93)]
	public class EntityKingBoss : EntityLiving
	{
		public const int MaxLife = 40;

		private readonly float _left;
		private readonly float _top;
		private readonly float _waza1;
		private readonly float _waza2;
		private readonly float _waza3;
		private string _endScript;

		private PointF _firstLoc;
		private bool _isBattling;

		private KingBossBehaviorOption _nowBehavior;

		private int _startKyori = 256;

		private string _startScript;
		protected float InternalGravity;

		public override int DyingMax => 0;

		/// <summary>
		///     無敵時間。0のときは無敵ではないが、0以上の時は無敵である。
		/// </summary>
		public int MutekiTime;

		protected int Tick;

		public EntityKingBoss(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			CollisionAIs.Add(new AiKillDefender(this));
			InternalGravity = 0.1f;
			IsOnLand = true;
			_firstLoc = pnt;
			Tick = 50;
			_left = _firstLoc.X - 192;
			_top = _firstLoc.Y - 128;
			Size = new Size(12, 30);

			var a = (_firstLoc.X - _left) / 5;
			_waza1 = _left + a;
			_waza2 = _waza1 + a;
			_waza3 = _waza2 + a;
		}

		public int Life { get; set; } = MaxLife;

		public override int[] ImageHandle => ResourceUtility.King;


		public override RectangleF Collision => new RectangleF(new PointF(2, 2), Size);

		public override EntityGroup MyGroup => EntityGroup.Enemy;

		public override float Gravity => InternalGravity;

		public override void OnUpdate(Status ks)
		{
			base.OnUpdate(ks);
			//FontUtility.DrawMiniString((int)Location.X - 9 + GameEngine.camera.X, (int)Location.Y + GameEngine.camera.Y, "" + AnimeSpeed, 0xffffff);


			if (!_isBattling)
			{
				// ReSharper disable once AssignmentInConditionalExpression
				if (_isBattling = Location.GetLengthTo(Parent.MainEntity.Location) < _startKyori)
				{
					try
					{
						EventRuntime.AddScript(new EventScript(_startScript));
					}
					catch (EventScript.EventScriptException ex)
					{
						EventRuntime.AddScript(new EventScript($@"[enstop]
[mesbox:down]
[mes:""エラー！\n{ex.Message.Replace(@"\", @"\\").Replace(@"""", @"\""")}""]
[mesend]
[enstart]"));
					}

					Velocity.X = -1.6f;
					InternalGravity = -0.04f;
					SetAnime(0, 3, 5);
				}

				return;
			}

			switch (_nowBehavior)
			{
				case KingBossBehaviorOption.JumpToLeft:
					if ((Location.Y <= _top) || (Location.Y <= 0) || (CollisionTop() == ObjectHitFlag.Hit))
					{
						Velocity.Y = InternalGravity = 0;
						Location.Y = _top;
					}
					if (Tick % 30 == 0)
					{
						SoundUtility.PlaySound(Sounds.ShootArrow);
						var r = Math.Atan2(Location.Y - Parent.MainEntity.Location.Y, Location.X - Parent.MainEntity.Location.X);
						float x = -(float) Math.Cos(r) * 3.4f,
							y = -(float) Math.Sin(r) * 3.4f;

						Parent.Add(new EntityPlayingCard(Location, Mpts, Map, Parent) {Velocity = new Vector(x, y)});
					}
					if ((Location.X <= _left) || (Location.X <= 0) || (CollisionLeft() == ObjectHitFlag.Hit))
					{
						InternalGravity = 0.1f;
						_nowBehavior = KingBossBehaviorOption.Waiting1;
						Tick = -1;
					}
					break;
				case KingBossBehaviorOption.Waiting1:
					if ((Tick < 0) && (CollisionBottom() == ObjectHitFlag.Hit))
						Tick = 120;

					if (Tick == 100)
						SetGraphic(0);
					if (Tick == 80)
						SetGraphic(6);
					if (Tick == 0)
					{
						_nowBehavior = KingBossBehaviorOption.ThrowChocolateToRight;
						Tick = 120;
						Parent.Add(GameEngine.EntityRegister.CreateEntity("Bunyo", new PointF(Location.X, Location.Y - 16), Mpts, Map,
							Parent));

						SoundUtility.PlaySound(Sounds.ItemSpawn);
					}

					break;
				case KingBossBehaviorOption.ThrowChocolateToRight:
					if (Tick <= 0)
					{
						_nowBehavior = KingBossBehaviorOption.JumpToRight;

						Velocity.X = 1.6f;
						InternalGravity = -0.04f;
						SetAnime(6, 9, 5);
					}
					break;
				case KingBossBehaviorOption.JumpToRight:

					if ((Location.Y <= _top) || (Location.Y <= 0) || (CollisionTop() == ObjectHitFlag.Hit))
						Velocity.Y = InternalGravity = 0;
					if (Tick % 30 == 0)
					{
						SoundUtility.PlaySound(Sounds.ShootArrow);
						var r = Math.Atan2(Location.Y - Parent.MainEntity.Location.Y, Location.X - Parent.MainEntity.Location.X);
						float x = -(float) Math.Cos(r) * 3.4f,
							y = -(float) Math.Sin(r) * 3.4f;

						Parent.Add(new EntityPlayingCard(Location, Mpts, Map, Parent) {Velocity = new Vector(x, y)});
					}
					if (((Location.X >= _firstLoc.X) && (Location.X >= GameEngine.Map.Width - 1)) ||
						(CollisionRight() == ObjectHitFlag.Hit))
					{
						InternalGravity = 0.1f;
						Velocity.X = 0;
						_nowBehavior = KingBossBehaviorOption.Waiting2;
						Tick = -1;
					}
					break;
				case KingBossBehaviorOption.Waiting2:
					if ((Tick < 0) && (CollisionBottom() == ObjectHitFlag.Hit))
						Tick = 120;

					if (Tick == 100)
						SetGraphic(6);
					if (Tick == 80)
						SetGraphic(0);
					if (Tick == 0)
					{
						_nowBehavior = KingBossBehaviorOption.ThrowChocolateToLeft;
						Parent.Add(GameEngine.EntityRegister.CreateEntity("SoulChocolate", new PointF(Location.X, Location.Y - 16), Mpts,
							Map, Parent));
						Tick = 120;

						SoundUtility.PlaySound(Sounds.ItemSpawn);
					}

					break;
				case KingBossBehaviorOption.ThrowChocolateToLeft:
					if (Tick <= 0)
					{
						_nowBehavior = KingBossBehaviorOption.JumpToLeft;


						Velocity.X = -1.6f;
						InternalGravity = -0.04f;
						SetAnime(0, 3, 5);
					}
					break;
			}

			Tick--;

			if (MutekiTime <= 0) return;
			MutekiTime--;
		}

		public override void OnDraw(PointF p, Status ks)
		{
			if (MutekiTime > 0)
			{
				if (MutekiTime % 8 < 4)
					base.OnDraw(p, ks);
			}
			else
				base.OnDraw(p, ks);
		}

		public override void OnDebugDraw(PointF p, Status ks)
		{
			base.OnDebugDraw(p, ks);
			FontUtility.DrawMiniString((int) p.X, (int) (p.Y - 16), Location.GetLengthTo(Parent.MainEntity.Location).ToString(),
				Color.Red);
			FontUtility.DrawMiniString((int) p.X, (int) p.Y - 8, _nowBehavior.ToString(), Color.White);
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
				return;
			}


			if (MutekiTime > 0)
				return;

			if (Life < 1)
			{
				try
				{
					EventRuntime.AddScript(new EventScript(_endScript));
				}
				catch (EventScript.EventScriptException ex)
				{
					EventRuntime.AddScript(new EventScript($@"[enstop]
[mesbox:down]
[mes:""エラー！\n{ex.Message.Replace(@"\", @"\\").Replace(@"""", @"\""")}""]
[mesend]
[enstart]"));
				}
				base.Kill();


				return;
			}
			MutekiTime = 240;

			Life -= IsCrushed ? MaxLife / 4 : 1;
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			_startKyori = (int) jsonobj.StartKyori;
			_startScript = jsonobj.StartScript;
			_endScript = jsonobj.EndScript;
			return this;
		}


		public override void SetKilledAnime()
		{
			SetGraphic(5);
		}

		public override void SetCrushedAnime()
		{
			SetGraphic(5);

			IsCrushed = false;
		}
	}

	public class EntityPlayingCard : EntityDocument
	{
		public int Life = 90;

		public EntityPlayingCard(PointF pnt, Object[] obj, byte[,,] chips, EntityList par) : base(pnt, obj, chips, par)
		{
			if (DX.GetRand(2) == 0)
				SetAnime(21, 24, 8);
			else
				SetAnime(25, 28, 8);
		}

		public override int[] ImageHandle => ResourceUtility.Weapon;

		public override void OnUpdate(Status ks)
		{
			if (Life-- < 0) Kill();

			base.OnUpdate(ks);
		}
	}

	public class EntityExplosion : EntityFlying
	{
		public override EntityGroup MyGroup { get; }
		public override int[] ImageHandle { get; }

		public override void SetKilledAnime()
		{
		}

		public override void SetCrushedAnime()
		{
		}
	}

	public class EntityCircusBall : EntityLiving
	{
		public int Life = 3;

		public EntityCircusBall(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Velocity.Y = -3.0f;
			Size = new Size(16, 16);
			MainAi = new AiKillDefender(this);
			SetGraphic(GetRandom(17, 18, 19));
		}

		public override EntityGroup MyGroup => EntityGroup.Enemy;
		public override int[] ImageHandle => ResourceUtility.Weapon;

		public override void SetKilledAnime()
		{
		}

		public override void SetCrushedAnime()
		{
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			if (jsonobj.IsDefined("SpeedX"))
				Velocity.X = (float) jsonobj.SpeedX;
			return this;
		}

		public override void OnUpdate(Status ks)
		{
			if (CollisionBottom() == ObjectHitFlag.Hit)
			{
				Life--;
				Location.Y += Velocity.Y = -4.3f;
				SoundUtility.PlaySound(Sounds.Dumping);
			}

			if ((CollisionLeft() == ObjectHitFlag.Hit) || (CollisionRight() == ObjectHitFlag.Hit))
			{
				Life--;
				Location.X += Velocity.X *= -1;
				SoundUtility.PlaySound(Sounds.Dumping);
			}

			if (Life < 0)
				Kill();
			base.OnUpdate(ks);
		}

		public override void Kill()
		{
			SoundUtility.PlaySound(Sounds.BalloonBroken);
			IsDead = true;
		}
	}
}