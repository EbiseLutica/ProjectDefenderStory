using System.Drawing;
using TakeUpJewel.AI;
using TakeUpJewel.Data;
using TakeUpJewel.Util;

namespace TakeUpJewel.Entities
{
	[EntityRegistry("EntityBlackServer", 85)]
	public class EntityBlackServer : EntityLiving
	{
		/// <summary>
		///     AIのモード。
		/// </summary>
		private int _nowstatus;

		/// <summary>
		///     タイマー。
		/// </summary>
		private int _tick;

		public EntityBlackServer(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 32);
			CollisionAIs.Add(new AiKillDefender(this));
		}

		public override int[] ImageHandle => ResourceUtility.BlackServer;


		public override EntityGroup MyGroup => EntityGroup.Enemy;

		public override void SetKilledAnime()
		{
			AnimeSpeed = 0;
		}

		public override void SetCrushedAnime()
		{
			AnimeSpeed = 0;
			IsCrushed = false;
		}

		public override void OnUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。

			switch (_nowstatus)
			{
				case 0:
				case 3:
					if (_tick > 30)
					{
						_nowstatus++;
						_tick = -1;
					}
					break;
				case 1:
				case 2:
				case 4:
				case 5:
					if (_tick > 8)
					{
						if (_nowstatus == 5)
							_nowstatus = 0;
						else
							_nowstatus++;
						_tick = -1;
						if ((_nowstatus == 2) && !IsDying)
							Parent.Add(new EntitySaba(Location, Mpts, Map, Parent));
						; //鯖を出現
					}
					break;
			}

			_tick++;
			SetGraphic(_nowstatus);
			base.OnUpdate(ks);
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			return this;
		}
	}

	[EntityRegistry("EntitySaba", -1)]
	public class EntitySaba : EntityFlying
	{
		private int _mode = 6;
		private int _tick;

		public EntitySaba(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			CollisionAIs.Add(new AiKillDefender(this));

			Velocity = new Vector(-2.3f, 0);
		}


		public override int[] ImageHandle => ResourceUtility.BlackServer;

		public override Sounds KilledSound => Sounds.Null;

		public override EntityGroup MyGroup => EntityGroup.Enemy;

		public override RectangleF Collision => new RectangleF(0, 16, 16, 8);

		public override void SetKilledAnime()
		{
			AnimeSpeed = 0;
		}

		public override void SetCrushedAnime()
		{
			IsCrushed = false;
			AnimeSpeed = 0;
		}

		public override void OnUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			if ((_mode < 9) && (_tick > 8))
				_mode++;
			_tick++;
			SetGraphic(_mode);

			base.OnUpdate(ks);
			var a = CollisionLeft();
			if (!IsDying && (a != ObjectHitFlag.NotHit) && (a != ObjectHitFlag.InWater))
				Kill();
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			return this;
		}
	}
}