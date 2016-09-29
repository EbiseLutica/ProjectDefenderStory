using System;
using System.Collections.Generic;
using System.Drawing;
using DxLibDLL;
using TakeUpJewel.AI;
using TakeUpJewel.Data;
using TakeUpJewel.Util;
using Object = TakeUpJewel.Data.Object;

namespace TakeUpJewel.Entities
{
	/// <summary>
	///     エンティティが属するグループを指定します。この値は、エンティティの制御で、他のエンティティに敵対するか、味方するか、無視するかなどを判定するために利用するのが目的です。
	/// </summary>
	public enum EntityGroup
	{
		/// <summary>
		///     主人公に味方する生き物であることを表します。
		/// </summary>
		Friend,

		/// <summary>
		///     主人公に敵対する生き物であることを表します。
		/// </summary>
		Enemy,

		/// <summary>
		///     制御のためのエンティティ (スクリプト実行など...) であることを表します。
		/// </summary>
		System,

		/// <summary>
		///     ステージの仕掛け (背景や絵画や仕掛けなど...) であることを表します。
		/// </summary>
		Stage,

		/// <summary>
		///     味方側の武器であることを表します。
		/// </summary>
		DefenderWeapon,

		/// <summary>
		///     敵側の武器であることを表します。
		/// </summary>
		MonsterWeapon,

		/// <summary>
		///     パーティクルを表します。
		/// </summary>
		Particle,

		/// <summary>
		///     その他を表します。
		/// </summary>
		Other
	}

	public enum Direction
	{
		Left,
		Right
	}

	public abstract class Entity
	{
		/// <summary>
		///     前フレームでの場所。
		/// </summary>
		public PointF BLocation;

		/// <summary>
		///     前フレームでの速度。
		/// </summary>
		public Vector BVelocity;

		public Direction Direction;

		/// <summary>
		///     踏み潰されたかどうか。
		/// </summary>
		public bool IsCrushed;

		/// <summary>
		///     マップ上から削除されるフラグ。
		/// </summary>
		public bool IsDead;

		/// <summary>
		///     落下によって死んだかどうか。
		/// </summary>
		public bool IsFall;

		/// <summary>
		///     地面についているかどうか。
		/// </summary>
		public bool IsOnLand;

		/// <summary>
		///     自分の座標。
		/// </summary>
		public PointF Location;

		/// <summary>
		///     現在のマップデータ。
		/// </summary>
		public byte[,,] Map;

		/// <summary>
		///     現在のマップチップ。
		/// </summary>
		public Object[] Mpts;

		/// <summary>
		///     自分の親である EntityList。
		/// </summary>
		public EntityList Parent;

		/// <summary>
		///     自分につけられたタグ。
		/// </summary>
		public string Tag;

		/// <summary>
		///     自分の速度。
		/// </summary>
		public Vector Velocity;

		/// <summary>
		///     描画優先順位。
		/// </summary>
		public int ZIndex;

		/// <summary>
		///     自分の所属しているグループを取得します。
		/// </summary>
		public abstract EntityGroup MyGroup { get; }

		/// <summary>
		///     自分の大きさ。
		/// </summary>
		public Size Size { get; protected set; }

		/// <summary>
		///     この Entity に、エンティティデータを設定します。
		/// </summary>
		/// <param name="jsonobj"></param>
		/// <returns></returns>
		public virtual Entity SetEntityData(dynamic jsonobj)
		{
			if (jsonobj == null)
				return this;
			if (jsonobj.ZIndex())
				ZIndex = (int) jsonobj.ZIndex;
			if (jsonobj.IsDefined("Tag"))
				Tag = jsonobj.Tag;
			return this;
		}

		public virtual void OnReload()
		{
		}

		/// <summary>
		///     この Entity を殺します。
		/// </summary>
		public virtual void Kill()
		{
			IsDead = true;
		}

		/// <summary>
		///     オプションを指定して、この Entity を殺します。
		/// </summary>
		/// <param name="isfall">落下によって死んだかどうか。</param>
		/// <param name="iscrushed">踏み潰されたかどうか。</param>
		public virtual void Kill(bool isfall, bool iscrushed)
		{
			IsFall = isfall;
			IsCrushed = iscrushed;
			Kill();
		}

		/// <summary>
		///     Entity の場所を更新します。
		/// </summary>
		public virtual void Move()
		{
			Location += Velocity;
		}

		/// <summary>
		///     フレーム毎に呼ばれ、Entity のアップデート処理をします。
		/// </summary>
		public virtual void OnUpdate(Status ks)
		{
			Move();

			Backup();
		}

		/// <summary>
		///     変数をバックアップし、次フレームに持ち越す処理をします。
		/// </summary>
		public virtual void Backup()
		{
			BLocation = Location;
			BVelocity = Velocity;
		}
	}

	public abstract class EntityVisible : Entity
	{
		public abstract void OnDraw(PointF p, Status ks);

		public virtual void OnDebugDraw(PointF p, Status ks)
		{
		}
	}

	public abstract class EntityGraphical : EntityVisible
	{
		/// <summary>
		///     アニメーションの終点。
		/// </summary>
		public int AnimeEndIndex;

		/// <summary>
		///     アニメーションの速さ(単位は Tick)。
		/// </summary>
		public int AnimeSpeed;

		/// <summary>
		///     アニメーションの始点。
		/// </summary>
		public int AnimeStartIndex;

		/// <summary>
		///     現在のループ回数。
		/// </summary>
		protected int Looptimes;

		/// <summary>
		///     最大ループ回数。
		/// </summary>
		public int LoopTimes;

		/// <summary>
		///     現在の画像のインデックス。
		/// </summary>
		protected int Ptranime;

		/// <summary>
		///     この Entity が使用する画像ハンドルを取得します。
		/// </summary>
		public abstract int[] ImageHandle { get; }

		public void SetAnime(int startindex, int endindex, int speed)
		{
			if ((AnimeStartIndex == startindex) && (AnimeEndIndex == endindex))
				return;
			Ptranime = AnimeStartIndex = startindex;
			AnimeEndIndex = endindex;
			AnimeSpeed = speed;
			LoopTimes = -1;
		}

		public void SetGraphic(int index)
		{
			Ptranime = AnimeStartIndex = AnimeEndIndex = index;
			AnimeSpeed = 0;
		}

		///
		public override void OnUpdate(Status ks)
		{
			base.OnUpdate(ks);
			ControlAnime();
		}

		public override void OnDraw(PointF p, Status ks)
		{
			DX.DrawGraph((int) p.X, (int) p.Y, ImageHandle[Ptranime], 1);
		}

		/// <summary>
		///     アニメーションの制御を行います。
		/// </summary>
		public virtual void ControlAnime()
		{
			if ((AnimeSpeed > 0) && (GameEngine.Tick % AnimeSpeed == 0))
			{
				if (Ptranime < AnimeStartIndex) // 現在位置が始点より小さければ始点に戻す。
					Ptranime = AnimeStartIndex;

				if (ImageHandle == null) // そもそも ImageHandle が指定されていなかったらなにもせず抜ける。
					return;

				Ptranime++;
				if (Ptranime > AnimeEndIndex) // アニメが最後まで終わったら、ループなどの設定に従って制御する。
				{
					Looptimes++;
					if ((Looptimes >= LoopTimes) && (LoopTimes != -1))
					{
						AnimeSpeed = 0;
						Ptranime--;
					}
					else
						Ptranime = AnimeStartIndex;
				}
			}
			if (AnimeSpeed == 0)
				Ptranime = AnimeStartIndex;
		}
	}

	public abstract class EntityLiving : EntityGraphical
	{
		private const int Addition = 7;

		public bool BIsInWater;

		public List<AiBase> CollisionAIs = new List<AiBase>();

		/// <summary>
		///     死んでいる間のタイマー。
		/// </summary>
		public int DyingTick;

		/// <summary>
		///     死んでいる途中かどうか。
		/// </summary>
		public bool IsDying;

		public bool IsInWater;

		/// <summary>
		///     ジャンプしているかどうか。
		/// </summary>
		public bool IsJumping;

		public AiBase MainAi = null;

		/// <summary>
		///     この Entity の重力加速度を取得。基本的に変更せず、葉っぱなど空気抵抗があるものに対応させるときに、派生クラスでオーバーライドするべきです。
		/// </summary>
		public virtual float Gravity => 0.1f;

		/// <summary>
		///     この Entity の当たり判定。
		/// </summary>
		public virtual RectangleF Collision => new RectangleF(new PointF(0, 0), Size);

		public virtual Sounds KilledSound => Sounds.Killed;

		public virtual int DyingMax => 42;


		public virtual void UpdateGravity()
		{
			Velocity.Y += Gravity;
			if (IsInWater)
				Velocity.Y -= 0.03f;
		}

		public override void OnUpdate(Status ks)
		{
			UpdateGravity();
			CheckCollision();
			IsInWater = GetIsInWater();
			if (IsDying)
				Dying();
			if (IsInWater && !BIsInWater)
				OnIntoWater();
			if (!IsInWater && BIsInWater)
				OnOutOfWater();

			base.OnUpdate(ks);
		}

		public override void Backup()
		{
			BIsInWater = IsInWater;
			base.Backup();
		}

		/// <summary>
		///     水中にいるかどうかを取得します。
		/// </summary>
		public virtual bool GetIsInWater()
		{
			var x = (int) Location.X + Size.Width / 2;
			var y = (int) Location.Y + Size.Height / 4;


			if (new Point(x, y).IsOutOfRange())
				return false;

			return Mpts[Map[x / 16, y / 16, 0]].CheckHit(x % 16, y % 16) == ObjectHitFlag.InWater;
		}

		/// <summary>
		///     水中に入った時に呼ばれます。
		/// </summary>
		public virtual void OnIntoWater()
		{
			SoundUtility.PlaySound(Sounds.WaterSplash);
		}

		/// <summary>
		///     水中から出た時に呼ばれます。
		/// </summary>
		public virtual void OnOutOfWater()
		{
			SoundUtility.PlaySound(Sounds.WaterSplash);
		}


		public virtual void Dying()
		{
			if (DyingTick == 0)
				IsDead = true;
			DyingTick--;
		}

		/// <summary>
		///     当たり判定を計算します。
		/// </summary>
		public virtual void CheckCollision()
		{
			if (Location.X < 0)
				Location.X = 0;
			if (Location.X > GameEngine.Map.Width * 16 - Size.Width)
				Location.X = GameEngine.Map.Width * 16 - Size.Width;
			if (Location.Y < 0)
				Location.Y = 0;
			if (Location.Y > GameEngine.Map.Height * 16)
				Kill(true, false);
			CollisionTop();
			CollisionBottom();
			CollisionLeft();
			CollisionRight();
		}

		/// <summary>
		///     上の当たり判定を計算します。
		/// </summary>
		public virtual ObjectHitFlag CollisionTop()
		{
			int x, y;
			var retval = ObjectHitFlag.NotHit;

			for (x = (int) (Location.X + Collision.Left) + (int) Collision.Width / 4;
				x < (int) (Location.X + Collision.Right);
				x += (int) Collision.Width / 2)
			{
				y = (int) (Location.Y + Collision.Y);
				var pnt = new Point(x, y);
				if (pnt.IsOutOfRange())
					continue;
				var hit = Mpts[Map[x / 16, y / 16, 0]].CheckHit(x % 16, y % 16);
				switch (hit)
				{
					case ObjectHitFlag.Hit:
						//if (Mpts[Map[x / 16, y / 16, 0]].CheckHit(x % 16, (y + 1) % 16) == ObjectHitFlag.Hit)
						Location.Y++;
						Velocity.Y = 0;
						if (this is EntityPlayer)
							if (IsJumping && (Map[x / 16, y / 16, 0] == 9)) //ブロック破壊
							{
								SoundUtility.PlaySound(Sounds.Destroy);
								Map[x / 16, y / 16, 0] = 0;
								ParticleUtility.BrokenBlock(new Point(x, y), Parent, Mpts);
							}
						break;
					case ObjectHitFlag.Damage:
						Kill();
						goto case ObjectHitFlag.Hit;
					case ObjectHitFlag.Death:
						Kill();
						goto case ObjectHitFlag.Hit;
				}
				if (y > 0)
					if (Mpts[Map[x / 16, (y - 1) / 16, 0]].CheckHit(x % 16, (y - 1) % 16) == ObjectHitFlag.Hit)
						retval = ObjectHitFlag.Hit;
				if (GameEngine.IsDebugMode)
					DX.DrawPixel(GameEngine.Ks.Camera.X + x, GameEngine.Ks.Camera.Y + y, DX.GetColor(255, 255, 255));
			}
			foreach (IScaffold sc in Parent.FindEntitiesByType<IScaffold>())
			{
				if (sc == this)
					continue;
				if (new Rectangle((int) (Location.X + Collision.Left), (int) (Location.Y + Collision.Y), (int) Collision.Width, 1)
					.CheckCollision(
						new Rectangle((int) (sc.Location.X + sc.Collision.Left), (int) (sc.Location.Y + sc.Collision.Y),
							(int) sc.Collision.Width, (int) sc.Collision.Height)))
				{
					Location.Y++;
					Velocity.Y = 0;
				}
			}
			return retval;
		}

		/// <summary>
		///     下の当たり判定を計算します。
		/// </summary>
		public virtual ObjectHitFlag CollisionBottom()
		{
			int x, y;
			var retval = ObjectHitFlag.NotHit;
			IsOnLand = false;
			for (x = (int) (Location.X + Collision.Left) + (int) Collision.Width / 4;
				x < (int) (Location.X + Collision.Right);
				x += (int) Collision.Width / 2)
			{
				y = (int) (Location.Y + Collision.Y + Collision.Height);
				var pnt = new Point(x, y);
				if (pnt.IsOutOfRange())
					continue;
				var hit = Mpts[Map[x / 16, y / 16, 0]].CheckHit(x % 16, y % 16);

				switch (hit)
				{
					case ObjectHitFlag.NotHit:
						//IsOnLand = false;
						break;
					case ObjectHitFlag.Hit:
						if (Mpts[Map[x / 16, (y - 1) / 16, 0]].CheckHit(x % 16, (y - 1) % 16) == ObjectHitFlag.Hit)
							Location.Y--;
						IsOnLand = true;
						IsJumping = false;
						retval = ObjectHitFlag.Hit;
						if (Velocity.Y > 0)
							Velocity.Y = 0;
						break;
					case ObjectHitFlag.Damage:
						Kill();
						goto case ObjectHitFlag.Hit;
					case ObjectHitFlag.Death:
						Kill();
						goto case ObjectHitFlag.Hit;
				}
				if (y < GameEngine.Map.Height * 16 - 1)
					if (Mpts[Map[x / 16, (y + 1) / 16, 0]].CheckHit(x % 16, (y + 1) % 16) == ObjectHitFlag.Hit)
						retval = ObjectHitFlag.Hit;
				if (GameEngine.IsDebugMode)
					DX.DrawPixel(GameEngine.Ks.Camera.X + x, GameEngine.Ks.Camera.Y + y, DX.GetColor(255, 255, 255));
			}
			foreach (IScaffold sc in Parent.FindEntitiesByType<IScaffold>())
			{
				if (sc == this)
					continue;
				if (new Rectangle((int) (Location.X + Collision.Left), (int) (Location.Y + Collision.Bottom), (int) Collision.Width,
					1).CheckCollision(
					new Rectangle((int) (sc.Location.X + sc.Collision.Left), (int) (sc.Location.Y + sc.Collision.Y),
						(int) sc.Collision.Width, (int) sc.Collision.Height)))
				{
					Location.Y--;
					IsOnLand = true;
					if (Velocity.Y > 0)
						Velocity.Y = 0;
					retval = ObjectHitFlag.Hit;
				}
			}
			return retval;
		}

		public abstract void SetKilledAnime();
		public abstract void SetCrushedAnime();


		/// <summary>
		///     左の当たり判定を計算します。
		/// </summary>
		public virtual ObjectHitFlag CollisionLeft()
		{
			int x, y;
			var retval = ObjectHitFlag.NotHit;

			for (y = (int) (Location.Y + Collision.Top) + Size.Height / 6;
				y < (int) (Location.Y + Collision.Bottom);
				y += (int) Collision.Height / 3)
			{
				x = (int) (Location.X + Collision.X);
				var pnt = new Point(x, y);
				if (pnt.IsOutOfRange())
					continue;
				var hit = Mpts[Map[x / 16, y / 16, 0]].CheckHit(x % 16, y % 16);
				switch (hit)
				{
					case ObjectHitFlag.Hit:
						//if (Mpts[Map[(x + 1) / 16, y / 16, 0]].CheckHit((x + 1) % 16, y % 16) == ObjectHitFlag.Hit)
						if (Mpts[Map[x / 16, (y - 1) / 16, 0]].CheckHit(x % 16, (y - 1) % 16) == ObjectHitFlag.NotHit)
							Location.Y -= this is EntityPlayer && (((EntityPlayer) this).Form == PlayerForm.Big) ? 2 : 1;
						else
							Location.X++;
						if (this is EntityTurcosShell && ((EntityTurcosShell) this).IsRunning)
							if (Map[x / 16, y / 16, 0] == 9) //ブロック破壊
							{
								SoundUtility.PlaySound(Sounds.Destroy);
								Map[x / 16, y / 16, 0] = 0;
								ParticleUtility.BrokenBlock(new Point(x, y), Parent, Mpts);
							}
						Velocity.X = 0;
						retval = ObjectHitFlag.Hit;
						break;
					case ObjectHitFlag.Damage:
						Kill();
						goto case ObjectHitFlag.Hit;
					case ObjectHitFlag.Death:
						Kill();
						goto case ObjectHitFlag.Hit;
				}
				if (x > 0)
					if (Mpts[Map[(x - 1) / 16, y / 16, 0]].CheckHit((x - 1) % 16, y % 16) == ObjectHitFlag.Hit)
						retval = ObjectHitFlag.Hit;
				if (GameEngine.IsDebugMode)
					DX.DrawPixel(GameEngine.Ks.Camera.X + x, GameEngine.Ks.Camera.Y + y, DX.GetColor(255, 255, 255));
			}
			foreach (IScaffold sc in Parent.FindEntitiesByType<IScaffold>())
			{
				if (sc == this)
					continue;
				if (new Rectangle((int) (Location.X + Collision.Left), (int) (Location.Y + Collision.Y), 1, (int) Collision.Height)
					.CheckCollision(
						new Rectangle((int) (sc.Location.X + sc.Collision.Left), (int) (sc.Location.Y + sc.Collision.Y),
							(int) sc.Collision.Width, (int) sc.Collision.Height)))
				{
					Location.X++;
					Velocity.X = 0;
					retval = ObjectHitFlag.Hit;
				}
			}
			return retval;
		}

		/// <summary>
		///     右の当たり判定を計算します。
		/// </summary>
		public virtual ObjectHitFlag CollisionRight()
		{
			int x, y;
			var retval = ObjectHitFlag.NotHit;

			for (y = (int) (Location.Y + Collision.Top) + Size.Height / 6;
				y < (int) (Location.Y + Collision.Bottom);
				y += (int) Collision.Height / 3)
			{
				x = (int) (Location.X + Collision.X + Collision.Width);
				var pnt = new Point(x, y);
				if (pnt.IsOutOfRange())
					continue;
				var hit = Mpts[Map[x / 16, y / 16, 0]].CheckHit(x % 16, y % 16);
				switch (hit)
				{
					case ObjectHitFlag.NotHit:
						break;
					case ObjectHitFlag.Hit:
						if (Mpts[Map[x / 16, (y - 1) / 16, 0]].CheckHit(x % 16, (y - 1) % 16) == ObjectHitFlag.NotHit)
							Location.Y -= this is EntityPlayer && (((EntityPlayer) this).Form == PlayerForm.Big) ? 2 : 1;
						else
							Location.X--;
						if (this is EntityTurcosShell && ((EntityTurcosShell) this).IsRunning)
							if (Map[x / 16, y / 16, 0] == 9) //ブロック破壊
							{
								SoundUtility.PlaySound(Sounds.Destroy);
								Map[x / 16, y / 16, 0] = 0;
								ParticleUtility.BrokenBlock(new Point(x, y), Parent, Mpts);
							}
						Velocity.X = 0;
						retval = ObjectHitFlag.Hit;
						break;
					case ObjectHitFlag.Damage:
						Kill();
						goto case ObjectHitFlag.Hit;
					case ObjectHitFlag.Death:
						Kill();
						goto case ObjectHitFlag.Hit;
				}
				if (x < GameEngine.Map.Width * 16 - 1)
					if (Mpts[Map[(x + 1) / 16, y / 16, 0]].CheckHit((x + 1) % 16, y % 16) == ObjectHitFlag.Hit)
						retval = ObjectHitFlag.Hit;
				if (GameEngine.IsDebugMode)
					DX.DrawPixel(GameEngine.Ks.Camera.X + x, GameEngine.Ks.Camera.Y + y, DX.GetColor(255, 255, 255));
			}
			foreach (IScaffold sc in Parent.FindEntitiesByType<IScaffold>())
			{
				if (sc == this)
					continue;
				if (new Rectangle((int) (Location.X + Collision.Left), (int) (Location.Y + Collision.Y), 1, (int) Collision.Height)
					.CheckCollision(
						new Rectangle((int) (sc.Location.X + sc.Collision.Left), (int) (sc.Location.Y + sc.Collision.Y),
							(int) sc.Collision.Width, (int) sc.Collision.Height)))
				{
					Location.X--;
					Velocity.X = 0;
					retval = ObjectHitFlag.Hit;
				}
			}
			return retval;
		}

		public override void Kill()
		{
			IsDying = true;
			DyingTick = DyingMax;
			if (IsDying)
				SetKilledAnime();
			if (IsCrushed)
				SetCrushedAnime();
			if (!IsCrushed && !IsFall)
				SoundUtility.PlaySound(KilledSound);
			Velocity = IsCrushed ? Vector.Zero : new Vector(6.0f, -4.0f);
		}

		public override void OnDraw(PointF p, Status ks)
		{
			if (!IsDying || IsCrushed)
				base.OnDraw(p, ks);
			else if (!IsFall)
				DX.DrawRotaGraph2F(p.X + Size.Width / 2, p.Y + Size.Height, Size.Width / 2, Size.Height, 1,
					DevelopUtility.Deg2Rad(Math.Max(-90, -((DyingMax - DyingTick) * (90.0 / 28.0)))), ImageHandle[Ptranime], 1);
		}
	}

	public abstract class EntityProjectile : EntityGraphical
	{
		private PointF _bcol;

		private double _rad;

		protected int Alive;

		public bool BIsInWater;

		public bool IsInWater;

		public bool IsStucked;

		/// <summary>
		///     この Entity の重力加速度を取得。基本的に変更せず、葉っぱなど空気抵抗があるものに対応させるときに、派生クラスでオーバーライドするべきです。
		/// </summary>
		public virtual float Gravity => 0.1f;

		public virtual PointF Collision
		{
			get { return _bcol = new PointF((float) Math.Cos(_rad) * 4, (float) Math.Sin(_rad) * 4); }
		}

		/// <summary>
		///     移動を停止してから存在している時間を取得します。
		/// </summary>
		public virtual int AliveTime => 150;

		public override void Backup()
		{
			BIsInWater = IsInWater;
			base.Backup();
		}

		/// <summary>
		///     水中にいるかどうかを取得します。
		/// </summary>
		public virtual bool GetIsInWater()
		{
			var x = (int) Location.X + Size.Width / 2;
			var y = (int) Location.Y + Size.Height / 4;


			if (new Point(x, y).IsOutOfRange())
				return false;

			return Mpts[Map[x / 16, y / 16, 0]].CheckHit(x % 16, y % 16) == ObjectHitFlag.InWater;
		}

		public virtual void UpdateGravity()
		{
			Velocity.Y += Gravity;
			if (IsInWater)
				Velocity.Y -= 0.03f;
		}

		/// <summary>
		///     水中に入った時に呼ばれます。
		/// </summary>
		public virtual void OnIntoWater()
		{
			SoundUtility.PlaySound(Sounds.WaterSplash);
		}

		/// <summary>
		///     水中から出た時に呼ばれます。
		/// </summary>
		public virtual void OnOutOfWater()
		{
			SoundUtility.PlaySound(Sounds.WaterSplash);
		}

		public override void OnUpdate(Status ks)
		{
			IsInWater = GetIsInWater();
			UpdateGravity();

			var x = (int) (Collision.X + Location.X);
			var y = (int) (Collision.Y + Location.Y);
			if (Mpts[Map[x / 16, y / 16, 0]].CheckHit(x % 16, y % 16) == ObjectHitFlag.Hit)
			{
				Velocity = Vector.Zero;
				if (!IsStucked)
					OnStucked();
			}
			if (IsStucked)
			{
				Alive--;
				if (Alive <= 0)
					IsDead = true;
			}
			if (IsInWater && !BIsInWater)
				OnIntoWater();
			if (!IsInWater && BIsInWater)
				OnOutOfWater();
			base.OnUpdate(ks);
		}


		public virtual void OnStucked()
		{
			IsStucked = true;
			Alive = AliveTime;
		}

		public override void OnDraw(PointF p, Status ks)
		{
			DX.DrawRotaGraph2F(p.X, p.Y, 8, 8, 1,
				(Velocity.X != 0) && (Velocity.Y != 0) ? (_rad = Math.Atan2(Velocity.Y, Velocity.X)) + Math.PI : _rad + Math.PI,
				ImageHandle[Ptranime], 1);
		}

		public override void OnDebugDraw(PointF p, Status ks)
		{
			FontUtility.DrawMiniString((int) p.X, (int) p.Y - 8, "" + DevelopUtility.Rad2Deg(_rad), 0xffffff);
			DX.DrawCircle((int) (p.X + Collision.X), (int) (p.Y + Collision.Y), 4, DX.GetColor(255, 0, 0), 1);
			base.OnDebugDraw(p, ks);
		}
	}

	public interface IScaffold
	{
		PointF Location { get; }

		RectangleF Collision { get; }
	}

	public abstract class EntityFlying : EntityLiving
	{
		public override void UpdateGravity()
		{
			if (IsDying)
				base.UpdateGravity();
		}
	}
}