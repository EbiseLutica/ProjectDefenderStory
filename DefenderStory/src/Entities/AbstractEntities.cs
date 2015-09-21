using DxLibDLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DefenderStory.Util;
using DefenderStory.Data;
using Codeplex.Data;
using DefenderStory.AI;

namespace DefenderStory.Entities
{

	/// <summary>
	/// エンティティが属するグループを指定します。この値は、エンティティの制御で、他のエンティティに敵対するか、味方するか、無視するかなどを判定するために利用するのが目的です。
	/// </summary>
	public enum EntityGroup
	{
		/// <summary>
		/// 主人公に味方する生き物であることを表します。
		/// </summary>
		Defender,
		/// <summary>
		/// 主人公に敵対する生き物であることを表します。
		/// </summary>
		Monster,
		/// <summary>
		/// 制御のためのエンティティ (スクリプト実行など...) であることを表します。
		/// </summary>
		System,
		/// <summary>
		/// ステージの仕掛け (背景や絵画や仕掛けなど...) であることを表します。
		/// </summary>
		DefenderWeapon,
		Stage,
		/// <summary>
		/// ディフェンダー側の武器(アイスやリーフなど)であることを表します。
		/// </summary>
		/// <summary>
		/// 敵側の武器(矢など)であることを表します。
		/// </summary>
		MonsterWeapon,
		/// <summary>
		/// パーティクルを表します。
		/// </summary>
		Particle,
		/// <summary>
		/// その他を表します。
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
		/// 自分の座標。
		/// </summary>
		public PointF Location;

		/// <summary>
		/// 自分の速度。
		/// </summary>
		public Vector Velocity;

		/// <summary>
		/// 自分の所属しているグループを取得します。
		/// </summary>
		public abstract EntityGroup MyGroup
		{ get; }

		public Direction Direction;

		/// <summary>
		/// マップ上から削除されるフラグ。
		/// </summary>
		public bool IsDead;

		/// <summary>
		/// 自分の大きさ。
		/// </summary>
		public Size Size
		{ get; protected set; }

		/// <summary>
		/// 前フレームでの場所。
		/// </summary>
		public PointF bLocation;

		/// <summary>
		/// 前フレームでの速度。
		/// </summary>
		public Vector bVelocity;

		/// <summary>
		/// 自分につけられたタグ。
		/// </summary>
		public String Tag;

		/// <summary>
		/// 地面についているかどうか。
		/// </summary>
		public bool IsOnLand;

		/// <summary>
		/// 落下によって死んだかどうか。
		/// </summary>
		public bool IsFall;

		/// <summary>
		/// 現在のマップチップ。
		/// </summary>
		public Data.Object[] Mpts;

		/// <summary>
		/// この Entity に、エンティティデータを設定します。
		/// </summary>
		/// <param name="jsonobj"></param>
		/// <returns></returns>
		public virtual Entity SetEntityData(dynamic jsonobj)
		{
			if (jsonobj == null)
				return this;
			if (jsonobj.IsDefined("Tag"))
			{
				Tag = jsonobj.Tag;
			}
			return this;
		}

		/// <summary>
		/// 現在のマップデータ。
		/// </summary>
		public byte[,,] Map;

		/// <summary>
		/// 自分の親である EntityList。
		/// </summary>
		public EntityList Parent;

		/// <summary>
		/// 踏み潰されたかどうか。
		/// </summary>
		public bool IsCrushed;

		/// <summary>
		/// この Entity を殺します。
		/// </summary>
		public virtual void Kill()
		{
			this.IsDead = true;
		}

		/// <summary>
		/// オプションを指定して、この Entity を殺します。
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
		/// Entity の場所を更新します。
		/// </summary>
		public virtual void Move()
		{
			this.Location += this.Velocity;
		}

		/// <summary>
		/// フレーム毎に呼ばれ、Entity のアップデート処理をします。
		/// </summary>
		public virtual void onUpdate(Status ks)
		{
			Move();

			Backup();
		}

		/// <summary>
		/// 変数をバックアップし、次フレームに持ち越す処理をします。
		/// </summary>
		public virtual void Backup()
		{
			bLocation = Location;
			bVelocity = Velocity;
		}

	}

	public abstract class EntityVisible : Entity
	{
		public abstract void onDraw(PointF p, Status ks);

		public virtual void onDebugDraw(PointF p, Status ks)
		{
		}

	}

	public abstract class EntityGraphical : EntityVisible
	{
		/// <summary>
		/// この Entity が使用する画像ハンドルを取得します。
		/// </summary>
		public abstract int[] ImageHandle { get; }
		/// <summary>
		/// アニメーションの始点。
		/// </summary>
		public int AnimeStartIndex;
		/// <summary>
		/// アニメーションの終点。
		/// </summary>
		public int AnimeEndIndex;
		/// <summary>
		/// アニメーションの速さ(単位は Tick)。
		/// </summary>
		public int AnimeSpeed;

		/// <summary>
		/// 現在の画像のインデックス。
		/// </summary>
		protected int ptranime;

		/// <summary>
		/// 現在のループ回数。
		/// </summary>
		protected int looptimes;

		/// <summary>
		/// 最大ループ回数。
		/// </summary>
		public int LoopTimes;

		public void SetAnime(int startindex, int endindex, int speed)
		{
			if (AnimeStartIndex == startindex && AnimeEndIndex == endindex)
				return;
			ptranime = AnimeStartIndex = startindex;
			AnimeEndIndex = endindex;
			AnimeSpeed = speed;
			LoopTimes = -1;

		}

		public void SetGraphic(int index)
		{
			ptranime = AnimeStartIndex = AnimeEndIndex = index;
			AnimeSpeed = 0;
		}

		public override void onUpdate(Status ks)
		{
			base.onUpdate(ks);
			ControlAnime();
		}

		public override void onDraw(PointF p, Status ks)
		{
			DX.DrawGraph((int)p.X, (int)p.Y, ImageHandle[ptranime], 1);
		}

		/// <summary>
		/// アニメーションの制御を行います。
		/// </summary>
		public virtual void ControlAnime()
		{
			if (AnimeSpeed > 0 && GameEngine.tick % AnimeSpeed == 0)
			{
				if (ptranime < AnimeStartIndex) // 現在位置が始点より小さければ始点に戻す。
					ptranime = AnimeStartIndex;

				if (ImageHandle == null)    // そもそも ImageHandle が指定されていなかったらなにもせず抜ける。
					return;

				ptranime++;
				if (ptranime > AnimeEndIndex)   // アニメが最後まで終わったら、ループなどの設定に従って制御する。
				{

					looptimes++;
					if (looptimes >= LoopTimes && LoopTimes != -1)
					{
						AnimeSpeed = 0;
						ptranime--;
					}
					else
						ptranime = AnimeStartIndex;
				}
			}
			if (AnimeSpeed == 0)
				ptranime = AnimeStartIndex;
		}
	}

	public abstract class EntityLiving : EntityGraphical
	{
		/// <summary>
		/// この Entity の重力加速度を取得。基本的に変更せず、葉っぱなど空気抵抗があるものに対応させるときに、派生クラスでオーバーライドするべきです。
		/// </summary>
		public virtual float Gravity
		{
			get
			{
				return 0.1f;
			}
		}

		/// <summary>
		/// 死んでいる途中かどうか。
		/// </summary>
		public bool IsDying;

		/// <summary>
		/// ジャンプしているかどうか。
		/// </summary>
		public bool IsJumping;


		public virtual void UpdateGravity()
		{
			Velocity.Y += Gravity;
			if (IsInWater)
				Velocity.Y -= 0.03f;
		}

		public override void onUpdate(Status ks)
		{
			UpdateGravity();
			CheckCollision();
			IsInWater = GetIsInWater();
			if (IsDying)
				Dying();
			if (IsInWater && !bIsInWater)
				onIntoWater();
			if (!IsInWater && bIsInWater)
				onOutOfWater();
			
			base.onUpdate(ks);
		}

		/// <summary>
		/// 死んでいる間のタイマー。
		/// </summary>
		public int DyingTick;

		const int addition = 7;

		/// <summary>
		/// この Entity の当たり判定。
		/// </summary>
		public virtual RectangleF Collision
		{
			get
			{
				return new RectangleF(new PointF(0, 0), this.Size);
			}
		}

		public override void Backup()
		{
			bIsInWater = IsInWater;
			base.Backup();
		}

		public bool bIsInWater;

		public bool IsInWater;

		/// <summary>
		/// 水中にいるかどうかを取得します。
		/// </summary>
		public virtual bool GetIsInWater()
		{
			int x = (int)this.Location.X + this.Size.Width / 2;
			int y = (int)this.Location.Y + this.Size.Height / 4;


			if (new Point(x, y).IsOutOfRange())
				return false;

			return Mpts[Map[x / 16, y / 16, 0]].CheckHit(x % 16, y % 16) == ObjectHitFlag.InWater;
		}

		/// <summary>
		/// 水中に入った時に呼ばれます。
		/// </summary>
		public virtual void onIntoWater()
		{
			SoundUtility.PlaySound(Sounds.WaterSplash);
		}

		/// <summary>
		/// 水中から出た時に呼ばれます。
		/// </summary>
		public virtual void onOutOfWater()
		{
			SoundUtility.PlaySound(Sounds.WaterSplash);
		}


		public virtual void Dying()
		{
			if (DyingTick == 0)
				IsDead = true;
			DyingTick--;
		}

		public AIBase MainAI = null;

		public List<AIBase> CollisionAIs = new List<AIBase>();

		/// <summary>
		/// 当たり判定を計算します。
		/// </summary>
		public virtual void CheckCollision()
		{
			if (Location.X < 0)
				Location.X = 0;
			if (Location.X > GameEngine.map.Width * 16 - Size.Width)
				Location.X = GameEngine.map.Width * 16 - Size.Width;
			if (Location.Y < 0)
				Location.Y = 0;
			if (Location.Y > GameEngine.map.Height * 16 - Size.Height)
				Kill(true, false);
			CollisionTop();
			CollisionBottom();
			CollisionLeft();
			CollisionRight();
		}

		/// <summary>
		/// 上の当たり判定を計算します。
		/// </summary>
		public virtual ObjectHitFlag CollisionTop()
		{
			int x, y;
			ObjectHitFlag retval = ObjectHitFlag.NotHit;

			for (x = (int)(Location.X + Collision.Left) + (int)Collision.Width / 4; x < (int)(Location.X + Collision.Right); x += (int)Collision.Width / 2)
			{
				y = (int)(Location.Y + Collision.Y);
				var pnt = new Point(x, y);
				if (pnt.IsOutOfRange())
					continue;
				var hit = Mpts[Map[x / 16, y / 16, 0]].CheckHit(x % 16, y % 16);
				switch (hit)
				{
					case ObjectHitFlag.Hit:
						if (Mpts[Map[x / 16, y / 16, 0]].CheckHit(x % 16, (y + 1) % 16) == ObjectHitFlag.Hit)
							Location.Y++;
						Velocity.Y = 0;
						if (this is EntityPlayer && ((EntityPlayer)this).Form != PlayerForm.Mini)
						{
							if (IsJumping && Map[x / 16, y / 16, 0] == 9) //ブロック破壊
							{
								SoundUtility.PlaySound(Sounds.Destroy);
								Map[x / 16, y / 16, 0] = 0;
								ParticleUtility.BrokenBlock(new Point(x, y), Parent, Mpts);
							}
						}
						break;
					case ObjectHitFlag.Damage:
						this.Kill();
						break;
					case ObjectHitFlag.Death:
						this.Kill();
						break;
				}
				if (y > 0)
					if (Mpts[Map[x / 16, (y - 1) / 16, 0]].CheckHit(x % 16, (y - 1) % 16) == ObjectHitFlag.Hit)
						retval = ObjectHitFlag.Hit;
				if (GameEngine.isDebugMode)
					DX.DrawPixel(GameEngine.ks.camera.X + x, GameEngine.ks.camera.Y + y, DX.GetColor(255, 255, 255));
			}
			foreach (IScaffold sc in Parent.FindEntitiesByType<IScaffold>())
			{
				if (new Rectangle((int)(Location.X + Collision.Left), (int)(Location.Y + Collision.Y), (int)Collision.Width, 1).CheckCollision(
					new Rectangle((int)(sc.Location.X + sc.Collision.Left), (int)(sc.Location.Y + sc.Collision.Y), (int)sc.Collision.Width, (int)sc.Collision.Height)))
				{
					Location.Y++;
					Velocity.Y = 0;
				}
			}
			return retval;
		}

		/// <summary>
		/// 下の当たり判定を計算します。
		/// </summary>
		public virtual ObjectHitFlag CollisionBottom()
		{
			int x, y;
			ObjectHitFlag retval = ObjectHitFlag.NotHit;
			IsOnLand = false;
			for (x = (int)(Location.X + Collision.Left) + (int)Collision.Width / 4; x < (int)(Location.X + Collision.Right); x += (int)Collision.Width / 2)
			{
				y = (int)(Location.Y + Collision.Y + Collision.Height);
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
						break;
					case ObjectHitFlag.Death:
						Kill();
						break;
				}
				if (y < GameEngine.map.Height * 16 - 1)
					if (Mpts[Map[x / 16, (y + 1) / 16, 0]].CheckHit(x % 16, (y + 1) % 16) == ObjectHitFlag.Hit)
						retval = ObjectHitFlag.Hit;
				if (GameEngine.isDebugMode)
					DX.DrawPixel(GameEngine.ks.camera.X + x, GameEngine.ks.camera.Y + y, DX.GetColor(255, 255, 255));
			}
			foreach (IScaffold sc in Parent.FindEntitiesByType<IScaffold>())
			{
				if (new Rectangle((int)(Location.X + Collision.Left), (int)(Location.Y + Collision.Y + Collision.Height), (int)Collision.Width, 1).CheckCollision(
					new Rectangle((int)(sc.Location.X + sc.Collision.Left), (int)(sc.Location.Y + sc.Collision.Y), (int)sc.Collision.Width, (int)sc.Collision.Height)))
				{
					Location.Y--;
					IsOnLand = true;
					Velocity.Y = 0;
					retval = ObjectHitFlag.Hit;
				}
			}
			return retval;
		}

		public abstract void SetKilledAnime();
		public abstract void SetCrushedAnime();


		/// <summary>
		/// 左の当たり判定を計算します。
		/// </summary>
		public virtual ObjectHitFlag CollisionLeft()
		{
			int x, y;
			ObjectHitFlag retval = ObjectHitFlag.NotHit;

			for (y = (int)(Location.Y + Collision.Top) + Size.Height / 6; y < (int)(Location.Y + Collision.Bottom); y += (int)Collision.Height / 3)
			{
				x = (int)(Location.X + Collision.X);
				var pnt = new Point(x, y);
				if (pnt.IsOutOfRange())
					continue;
				var hit = Mpts[Map[x / 16, y / 16, 0]].CheckHit(x % 16, y % 16);
				switch (hit)
				{
					case ObjectHitFlag.Hit:
						//if (Mpts[Map[(x + 1) / 16, y / 16, 0]].CheckHit((x + 1) % 16, y % 16) == ObjectHitFlag.Hit)
						if (Mpts[Map[x / 16, (y - 1) / 16, 0]].CheckHit(x % 16, (y - 1) % 16) == ObjectHitFlag.NotHit)
							Location.Y -= (this is EntityPlayer && ((EntityPlayer)this).Form == PlayerForm.Big) ? 2 : 1;
						else
							Location.X++;
						if (this is EntityTurcosShell && ((EntityTurcosShell)this).isRunning)
						{
							if (Map[x / 16, y / 16, 0] == 9) //ブロック破壊
							{
								SoundUtility.PlaySound(Sounds.Destroy);
								Map[x / 16, y / 16, 0] = 0;
								ParticleUtility.BrokenBlock(new Point(x, y), Parent, Mpts);
							}
						}
						Velocity.X = 0;
						retval = ObjectHitFlag.Hit;
						break;
					case ObjectHitFlag.Damage:
						Kill();
						break;
					case ObjectHitFlag.Death:
						Kill();
						break;
				}
				if (x > 0)
					if (Mpts[Map[(x - 1) / 16, y / 16, 0]].CheckHit((x - 1) % 16, y % 16) == ObjectHitFlag.Hit)
						retval = ObjectHitFlag.Hit;
				if (GameEngine.isDebugMode)
					DX.DrawPixel(GameEngine.ks.camera.X + x, GameEngine.ks.camera.Y + y, DX.GetColor(255, 255, 255));
			}
			foreach (IScaffold sc in Parent.FindEntitiesByType<IScaffold>())
			{
				if (new Rectangle((int)(Location.X + Collision.Left), (int)(Location.Y + Collision.Y), (int)1, (int)(Collision.Height)).CheckCollision(
					new Rectangle((int)(sc.Location.X + sc.Collision.Left), (int)(sc.Location.Y + sc.Collision.Y), (int)sc.Collision.Width, (int)sc.Collision.Height)))
				{
					Location.X++;
					Velocity.X = 0;
					retval = ObjectHitFlag.Hit;
				}
			}
			return retval;
		}

		/// <summary>
		/// 右の当たり判定を計算します。
		/// </summary>
		public virtual ObjectHitFlag CollisionRight()
		{
			int x, y;
			ObjectHitFlag retval = ObjectHitFlag.NotHit;

			for (y = (int)(Location.Y + Collision.Top) + Size.Height / 6; y < (int)(Location.Y + Collision.Bottom); y += (int)Collision.Height / 3)
			{
				x = (int)(Location.X + Collision.X + Collision.Width);
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
							Location.Y -= (this is EntityPlayer && ((EntityPlayer)this).Form == PlayerForm.Big) ? 2 : 1;
						else
							Location.X--;
						if (this is EntityTurcosShell && ((EntityTurcosShell)this).isRunning)
						{
							if (Map[x / 16, y / 16, 0] == 9) //ブロック破壊
							{
								SoundUtility.PlaySound(Sounds.Destroy);
								Map[x / 16, y / 16, 0] = 0;
								ParticleUtility.BrokenBlock(new Point(x, y), Parent, Mpts);
							}
						}
						Velocity.X = 0;
						retval = ObjectHitFlag.Hit;
						break;
					case ObjectHitFlag.Damage:
						Kill();
						break;
					case ObjectHitFlag.Death:
						Kill();
						break;
				}
				if (x < GameEngine.map.Width * 16 - 1)
					if (Mpts[Map[(x + 1) / 16, y / 16, 0]].CheckHit((x + 1) % 16, y % 16) == ObjectHitFlag.Hit)
						retval = ObjectHitFlag.Hit;
				if (GameEngine.isDebugMode)
					DX.DrawPixel(GameEngine.ks.camera.X + x, GameEngine.ks.camera.Y + y, DX.GetColor(255, 255, 255));
			}
			foreach (IScaffold sc in Parent.FindEntitiesByType<IScaffold>())
			{
				if (new Rectangle((int)(Location.X + Collision.Left), (int)(Location.Y + Collision.Y), (int)1, (int)(Collision.Height)).CheckCollision(
					new Rectangle((int)(sc.Location.X + sc.Collision.Left), (int)(sc.Location.Y + sc.Collision.Y), (int)sc.Collision.Width, (int)sc.Collision.Height)))
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
			Velocity = Vector.Zero;
		}

		public override void onDraw(PointF p, Status ks)
		{
			if (!IsDying || IsCrushed)
				base.onDraw(p, ks);
			else if (!IsFall)
				DX.DrawRotaGraph2F(p.X + Size.Width / 2, p.Y + Size.Height, Size.Width / 2, Size.Height, 1, DevelopUtility.Deg2Rad(Math.Max(-90, -((DyingMax - DyingTick) * (90.0 / 28.0)))), ImageHandle[ptranime], 1);
		}

		public virtual int DyingMax
		{
			get
			{
				return 42;
			}
		}

	}

	public abstract class EntityProjectile : EntityGraphical
	{
		/// <summary>
		/// この Entity の重力加速度を取得。基本的に変更せず、葉っぱなど空気抵抗があるものに対応させるときに、派生クラスでオーバーライドするべきです。
		/// </summary>
		public virtual float Gravity
		{
			get
			{
				return 0.1f;
			}
		}

		public override void Backup()
		{
			bIsInWater = IsInWater;
			base.Backup();
		}

		public bool bIsInWater;

		public bool IsInWater;

		/// <summary>
		/// 水中にいるかどうかを取得します。
		/// </summary>
		public virtual bool GetIsInWater()
		{
			int x = (int)this.Location.X + this.Size.Width / 2;
			int y = (int)this.Location.Y + this.Size.Height / 4;


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
		/// 水中に入った時に呼ばれます。
		/// </summary>
		public virtual void onIntoWater()
		{
			SoundUtility.PlaySound(Sounds.WaterSplash);
		}

		/// <summary>
		/// 水中から出た時に呼ばれます。
		/// </summary>
		public virtual void onOutOfWater()
		{
			SoundUtility.PlaySound(Sounds.WaterSplash);
		}
		PointF bcol;
		public virtual PointF Collision
		{
			get
			{
				return bcol = new PointF((float)Math.Cos(rad) * 4, (float)Math.Sin(rad) * 4);
            }
		}
		
		/// <summary>
		/// 移動を停止してから存在している時間を取得します。
		/// </summary>
		/// 
		public virtual int AliveTime
		{
			get
			{
				return 150;
			}
		}

		protected int alive = 0;

		public bool IsStucked = false;

		public override void onUpdate(Status ks)
		{
			IsInWater = GetIsInWater();
			UpdateGravity();

			int x = (int)(Collision.X + Location.X);
			int y = (int)(Collision.Y + Location.Y);
			if (Mpts[Map[x / 16, y / 16, 0]].CheckHit(x % 16, y % 16) == ObjectHitFlag.Hit)
			{
				Velocity = Vector.Zero;
				if (!IsStucked)
				{
					onStucked();
				}
			}
			if (IsStucked)
			{
				alive--;
				if (alive <= 0)
					IsDead = true;
			}
			if (IsInWater && !bIsInWater)
				onIntoWater();
			if (!IsInWater && bIsInWater)
				onOutOfWater();
			base.onUpdate(ks);

		}



		public virtual void onStucked()
		{
			IsStucked = true;
			alive = AliveTime;
		}

		double rad = 0;
		public override void onDraw(PointF p, Status ks)
		{
			DX.DrawRotaGraph2F(p.X, p.Y, 8, 8, 1, ((Velocity.X != 0 && Velocity.Y != 0) ? (rad = Math.Atan2(Velocity.Y, Velocity.X)) + Math.PI : rad + Math.PI), ImageHandle[ptranime], 1);
		}

		public override void onDebugDraw(PointF p, Status ks)
		{
			FontUtility.DrawMiniString((int)p.X, (int)p.Y - 8, "" + DevelopUtility.Rad2Deg(rad), 0xffffff);
			DX.DrawCircle((int)(p.X + Collision.X), (int)(p.Y + Collision.Y), 4, DX.GetColor(255, 0, 0), 1);
			base.onDebugDraw(p, ks);
		}


	}

	public interface IScaffold
	{
		PointF Location
		{
			get;
		}

		RectangleF Collision
		{
			get;
		}

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
