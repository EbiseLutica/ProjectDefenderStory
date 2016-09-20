using System;
using System.Collections.Generic;
using System.Drawing;
using Codeplex.Data;
using DxLibDLL;
using TakeUpJewel.Data;
using TakeUpJewel.Entities;
using TakeUpJewel.Util;

namespace TakeUpJewel.AI
{
	public class AiWalk : AiBase
	{

		public override bool Use => !HostEntity.IsDying;

		readonly int _leftAnimeStartIndex;
		readonly int _rightAnimeStartIndex;
		readonly int _leftAnimeEndIndex;
		readonly int _rightAnimeEndIndex;

		public AiWalk(EntityLiving baseentity, int spd, int lAnmStart, int lAnmEnd, int rAnmStart, int rAnmEnd)
		{
			HostEntity = baseentity;
			_speed = spd;
			_leftAnimeStartIndex = lAnmStart;
			_rightAnimeStartIndex = rAnmStart;
			_leftAnimeEndIndex = lAnmEnd;
			_rightAnimeEndIndex = rAnmEnd;
		}

		private readonly int _speed = 1;

		public override void OnUpdate()
		{

			if (HostEntity.CollisionLeft() == ObjectHitFlag.Hit || HostEntity.Location.X <= 0)
			{
				HostEntity.Velocity.X = _speed;
				HostEntity.SetAnime(_rightAnimeStartIndex, _rightAnimeEndIndex, 8);
			}
			if (HostEntity.CollisionRight() == ObjectHitFlag.Hit || HostEntity.Location.X >= GameEngine.Map.Width * 16 - 1)
			{
				HostEntity.Velocity.X = -_speed;
				HostEntity.SetAnime(_leftAnimeStartIndex, _leftAnimeEndIndex, 8);
			}
		}

		public override void OnInit()
		{
			if (HostEntity.Parent.MainEntity.Location.X < HostEntity.Location.X)
			{
				HostEntity.Velocity.X = -_speed;
				HostEntity.SetAnime(_leftAnimeStartIndex, _leftAnimeEndIndex, 8);
			}
			else
			{
				HostEntity.Velocity.X = _speed;
				HostEntity.SetAnime(_rightAnimeStartIndex, _rightAnimeEndIndex, 8);
			}
		}
	}

	public class AiFlySine : AiBase
	{

		public override bool Use => !HostEntity.IsDying;

		readonly int _leftAnimeStartIndex;
		readonly int _rightAnimeStartIndex;
		readonly int _leftAnimeEndIndex;
		readonly int _rightAnimeEndIndex;

		public AiFlySine(EntityLiving baseentity, int spd, int lAnmStart, int lAnmEnd, int rAnmStart, int rAnmEnd)
		{
			HostEntity = baseentity;
			_speed = spd;
			_leftAnimeStartIndex = lAnmStart;
			_rightAnimeStartIndex = rAnmStart;
			_leftAnimeEndIndex = lAnmEnd;
			_rightAnimeEndIndex = rAnmEnd;
		}

		private readonly int _speed = 1;

		private int _deg;

		public override void OnUpdate()
		{

			if (HostEntity.CollisionLeft() == ObjectHitFlag.Hit || HostEntity.Location.X <= 0)
			{
				HostEntity.Velocity.X = _speed;
				HostEntity.SetAnime(_rightAnimeStartIndex, _rightAnimeEndIndex, 8);
			}
			if (HostEntity.CollisionRight() == ObjectHitFlag.Hit || HostEntity.Location.X >= GameEngine.Map.Width * 16 - 1)
			{
				HostEntity.Velocity.X = -_speed;
				HostEntity.SetAnime(_leftAnimeStartIndex, _leftAnimeEndIndex, 8);
			}
			HostEntity.Velocity.Y = (float)Math.Sin(_deg / 180.0 * Math.PI) * (HostEntity.Direction == Direction.Left ? -1 : 1);
			_deg = (_deg + 5) % 360;
		}

		public override void OnInit()
		{
			if (HostEntity.Parent.MainEntity.Location.X < HostEntity.Location.X)
			{
				HostEntity.Velocity.X = -_speed;
				HostEntity.SetAnime(_leftAnimeStartIndex, _leftAnimeEndIndex, 8);
			}
			else
			{	
				HostEntity.Velocity.X = _speed;
				HostEntity.SetAnime(_rightAnimeStartIndex, _rightAnimeEndIndex, 8);
			}
		}
	}

	public class AiFlySearch : AiBase
	{

		public override bool Use => !HostEntity.IsDying;

		readonly int _leftAnimeStartIndex;
		readonly int _rightAnimeStartIndex;
		readonly int _leftAnimeEndIndex;
		readonly int _rightAnimeEndIndex;

		public AiFlySearch(EntityLiving baseentity, int spd, int lAnmStart, int lAnmEnd, int rAnmStart, int rAnmEnd)
		{
			HostEntity = baseentity;
			_speed = spd;
			_leftAnimeStartIndex = lAnmStart;
			_rightAnimeStartIndex = rAnmStart;
			_leftAnimeEndIndex = lAnmEnd;
			_rightAnimeEndIndex = rAnmEnd;
		}

		private int _speed = 1;

		private int _deg = 0;

		private int _tick = 60;

		public override void OnUpdate()
		{
			_tick++;

			if (_tick > 60)
			{
				var atan = (float)Math.Atan2(HostEntity.Parent.MainEntity.Location.Y - HostEntity.Location.Y, HostEntity.Parent.MainEntity.Location.X - HostEntity.Location.X);
				HostEntity.Velocity.X = (float)Math.Cos(atan);
				HostEntity.Velocity.Y = (float)Math.Sin(atan);
				_tick = 0;
			}
			if (HostEntity.Direction == Direction.Left)
				HostEntity.SetAnime(_leftAnimeStartIndex, _leftAnimeEndIndex, 8);
			else
				HostEntity.SetAnime(_rightAnimeStartIndex, _rightAnimeEndIndex, 8);


		}
	}


	public class AiKillDefender : AiBase
	{
		public override bool Use => !HostEntity.IsDying;

		public AiKillDefender(EntityLiving el)
		{
			HostEntity = el;
		}

		public override void OnUpdate()
		{
			foreach (EntityPlayer ep in HostEntity.Parent.FindEntitiesByType<EntityPlayer>())
			{
				if (ep.IsDying)
					continue;
				if (new Rectangle((int)ep.Location.X, (int)ep.Location.Y, ep.Size.Width, ep.Size.Height)
					.CheckCollision(new Rectangle((int)HostEntity.Location.X, (int)HostEntity.Location.Y + 8 , HostEntity.Size.Width, HostEntity.Size.Height - 8)))
					ep.Kill();

			}


		}
	}

	public class AiKillMonster : AiBase
	{
		public override bool Use => !HostEntity.IsDying;

		public AiKillMonster(EntityLiving el)
		{
			HostEntity = el;
		}

		public override void OnUpdate()
		{
			foreach (EntityLiving el in new List<Entity>(HostEntity.Parent.FindEntitiesByType<EntityLiving>()))
			{
				if (el.MyGroup != EntityGroup.Monster)
					continue;
				if (el.IsDying)
					continue;
				if (new Rectangle((int)HostEntity.Location.X, (int)HostEntity.Location.Y + HostEntity.Size.Height - 1, HostEntity.Size.Width, 1).CheckCollision(new Rectangle((int)el.Location.X, (int)el.Location.Y, el.Size.Width, el.Size.Height / 4)))
				{
					if (GameEngine.Ks.Inz)
						HostEntity.Velocity.Y = -3;
					else
						HostEntity.Velocity.Y = -1f;
					el.Kill(false, true);
					SoundUtility.PlaySound(Sounds.Stepped);
				}
			}


		}
	}

	public class AiKiller : AiBase
	{
		public override bool Use => !HostEntity.IsDying;

		public AiKiller(EntityLiving el)
		{
			HostEntity = el;
		}

		public override void OnUpdate()
		{
			foreach (EntityLiving ep in new List<Entity>(HostEntity.Parent.FindEntitiesByType<EntityLiving>()))
			{
				if (ep == HostEntity ||  ep.IsDying || (ep.MyGroup != EntityGroup.Defender && ep.MyGroup != EntityGroup.Monster))
					continue;
				if (HostEntity is EntityTurcosShell && ((EntityTurcosShell)HostEntity).Mutekitime > 0 && ep.MyGroup == EntityGroup.Defender)
					continue;
				if (new Rectangle((int)ep.Location.X, (int)ep.Location.Y, ep.Size.Width, ep.Size.Height)
					.CheckCollision(new Rectangle((int)HostEntity.Location.X, (int)HostEntity.Location.Y + 8, HostEntity.Size.Width, HostEntity.Size.Height - 8)))
				{
					if (ep.IsDying)
						continue;
					ep.Kill();
				}
			}
		}
	}

	public class AiArch : AiBase
	{
		public override bool Use => !HostEntity.IsDying;

		int _nowstatus;

		int _tick;
		public override void OnUpdate()
		{
			if (_nowstatus != 2 && _nowstatus != 0 && _tick == 15 || _tick == 30)
			{
				if (_nowstatus == 2)
				{
					SoundUtility.PlaySound(Sounds.ShootArrow);

					// TODO: Arrow を実装したら、ここのコメントアウトを外す
					var speed = HostEntity.Direction == Direction.Right ? DX.GetRand(4) + 1 : -DX.GetRand(4) - 1;
                    HostEntity.Parent.Add(GameEngine.EntityRegister.CreateEntity("Arrow", new PointF(HostEntity.Location.X + (HostEntity.Direction == Direction.Left ? 0 : HostEntity.Size.Width), HostEntity.Location.Y + HostEntity.Size.Height / 2), GameEngine.Mptobjects, GameEngine.Chips, HostEntity.Parent, DynamicJson.Parse("{\"Speed\": " + speed + " }")));
				}
				_tick = -1;
				_nowstatus = (_nowstatus + 1) % 4;
			}
			if (HostEntity.Parent.MainEntity.Location.X < HostEntity.Location.X)

			HostEntity.SetGraphic(_nowstatus + (HostEntity.Direction == Direction.Right ? 4 : 0));
			_tick++;
        }

		public AiArch(EntityLiving el)
		{
			HostEntity = el;
		}


	}

}
