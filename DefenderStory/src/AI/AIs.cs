using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefenderStory.Entities;
using DefenderStory.Data;
using DefenderStory.Util;
using System.Drawing;
using Codeplex.Data;
using DxLibDLL;

namespace DefenderStory.AI
{
	public class AIWalk : AIBase
	{

		public override bool Use
		{
			get
			{
				//生きている間動作する。
				return !HostEntity.IsDying;
			}
		}

		int LeftAnimeStartIndex;
		int RightAnimeStartIndex;
		int LeftAnimeEndIndex;
		int RightAnimeEndIndex;

		public AIWalk(EntityLiving baseentity, int spd, int lAnmStart, int lAnmEnd, int rAnmStart, int rAnmEnd)
		{
			HostEntity = baseentity;
			speed = spd;
			LeftAnimeStartIndex = lAnmStart;
			RightAnimeStartIndex = rAnmStart;
			LeftAnimeEndIndex = lAnmEnd;
			RightAnimeEndIndex = rAnmEnd;
		}

		private int speed = 1;

		public override void onUpdate()
		{

			if (HostEntity.CollisionLeft() == ObjectHitFlag.Hit || HostEntity.Location.X <= 0)
			{
				HostEntity.Velocity.X = speed;
				HostEntity.SetAnime(RightAnimeStartIndex, RightAnimeEndIndex, 8);
			}
			if (HostEntity.CollisionRight() == ObjectHitFlag.Hit || HostEntity.Location.X >= GameEngine.map.Width * 16 - 1)
			{
				HostEntity.Velocity.X = -speed;
				HostEntity.SetAnime(LeftAnimeStartIndex, LeftAnimeEndIndex, 8);
			}
		}

		public override void onInit()
		{
			if (HostEntity.Parent.MainEntity.Location.X < HostEntity.Location.X)
			{
				HostEntity.Velocity.X = -speed;
				HostEntity.SetAnime(LeftAnimeStartIndex, LeftAnimeEndIndex, 8);
			}
			else
			{
				HostEntity.Velocity.X = speed;
				HostEntity.SetAnime(RightAnimeStartIndex, RightAnimeEndIndex, 8);
			}
		}
	}

	public class AIFlySine : AIBase
	{

		public override bool Use
		{
			get
			{
				//生きている間動作する。
				return !HostEntity.IsDying;
			}
		}

		int LeftAnimeStartIndex;
		int RightAnimeStartIndex;
		int LeftAnimeEndIndex;
		int RightAnimeEndIndex;

		public AIFlySine(EntityLiving baseentity, int spd, int lAnmStart, int lAnmEnd, int rAnmStart, int rAnmEnd)
		{
			HostEntity = baseentity;
			speed = spd;
			LeftAnimeStartIndex = lAnmStart;
			RightAnimeStartIndex = rAnmStart;
			LeftAnimeEndIndex = lAnmEnd;
			RightAnimeEndIndex = rAnmEnd;
		}

		private int speed = 1;

		private int deg = 0;

		public override void onUpdate()
		{

			if (HostEntity.CollisionLeft() == ObjectHitFlag.Hit || HostEntity.Location.X <= 0)
			{
				HostEntity.Velocity.X = speed;
				HostEntity.SetAnime(RightAnimeStartIndex, RightAnimeEndIndex, 8);
			}
			if (HostEntity.CollisionRight() == ObjectHitFlag.Hit || HostEntity.Location.X >= GameEngine.map.Width * 16 - 1)
			{
				HostEntity.Velocity.X = -speed;
				HostEntity.SetAnime(LeftAnimeStartIndex, LeftAnimeEndIndex, 8);
			}
			HostEntity.Velocity.Y = (float)Math.Sin(deg / 180.0 * Math.PI) * (HostEntity.Direction == Direction.Left ? -1 : 1);
			deg = (deg + 5) % 360;
		}

		public override void onInit()
		{
			if (HostEntity.Parent.MainEntity.Location.X < HostEntity.Location.X)
			{
				HostEntity.Velocity.X = -speed;
				HostEntity.SetAnime(LeftAnimeStartIndex, LeftAnimeEndIndex, 8);
			}
			else
			{	
				HostEntity.Velocity.X = speed;
				HostEntity.SetAnime(RightAnimeStartIndex, RightAnimeEndIndex, 8);
			}
		}
	}

	public class AIFlySearch : AIBase
	{

		public override bool Use
		{
			get
			{
				//生きている間動作する。
				return !HostEntity.IsDying;
			}
		}

		int LeftAnimeStartIndex;
		int RightAnimeStartIndex;
		int LeftAnimeEndIndex;
		int RightAnimeEndIndex;

		public AIFlySearch(EntityLiving baseentity, int spd, int lAnmStart, int lAnmEnd, int rAnmStart, int rAnmEnd)
		{
			HostEntity = baseentity;
			speed = spd;
			LeftAnimeStartIndex = lAnmStart;
			RightAnimeStartIndex = rAnmStart;
			LeftAnimeEndIndex = lAnmEnd;
			RightAnimeEndIndex = rAnmEnd;
		}

		private int speed = 1;

		private int deg = 0;

		public override void onUpdate()
		{
			float atan = (float)Math.Atan2(HostEntity.Parent.MainEntity.Location.Y - HostEntity.Location.Y, HostEntity.Parent.MainEntity.Location.X - HostEntity.Location.X);
			HostEntity.Velocity.X = (float)Math.Cos(atan);
			HostEntity.Velocity.Y = (float)Math.Sin(atan);

			if (HostEntity.Direction == Direction.Left)
				HostEntity.SetAnime(LeftAnimeStartIndex, LeftAnimeEndIndex, 8);
			else
				HostEntity.SetAnime(RightAnimeStartIndex, RightAnimeEndIndex, 8);


		}
	}


	public class AIKillDefender : AIBase
	{
		public override bool Use
		{
			get
			{
				return !HostEntity.IsDying;
			}
		}

		public AIKillDefender(EntityLiving el)
		{
			this.HostEntity = el;
		}

		public override void onUpdate()
		{
			foreach (EntityPlayer ep in HostEntity.Parent.FindEntitiesByType<EntityPlayer>())
			{
				if (ep.IsDying)
					continue;
				if (new Rectangle((int)ep.Location.X, (int)(ep.Location.Y), (int)ep.Size.Width, (int)ep.Size.Height)
					.CheckCollision(new Rectangle((int)this.HostEntity.Location.X, (int)this.HostEntity.Location.Y + 8 , (int)this.HostEntity.Size.Width, (int)this.HostEntity.Size.Height - 8)))
					ep.Kill();

			}


		}
	}

	public class AIKillMonster : AIBase
	{
		public override bool Use
		{
			get
			{
				return !HostEntity.IsDying;
			}
		}

		public AIKillMonster(EntityLiving el)
		{
			this.HostEntity = el;
		}

		public override void onUpdate()
		{
			foreach (EntityLiving el in new List<Entity>(HostEntity.Parent.FindEntitiesByType<EntityLiving>()))
			{
				if (el.MyGroup != EntityGroup.Monster)
					continue;
				if (el.IsDying)
					continue;
				if (new Rectangle((int)HostEntity.Location.X, (int)HostEntity.Location.Y + (int)HostEntity.Size.Height - 1, (int)HostEntity.Size.Width, 1).CheckCollision(new Rectangle((int)el.Location.X, (int)el.Location.Y, (int)el.Size.Width, (int)el.Size.Height / 4)))
				{
					if (GameEngine.ks.inz)
						HostEntity.Velocity.Y = -3;
					else
						HostEntity.Velocity.Y = -1f;
					el.Kill(false, true);
					SoundUtility.PlaySound(Sounds.Stepped);
				}
			}


		}
	}

	public class AIKiller : AIBase
	{
		public override bool Use
		{
			get
			{
				return !HostEntity.IsDying;
			}
		}

		public AIKiller(EntityLiving el)
		{
			this.HostEntity = el;
		}

		public override void onUpdate()
		{
			foreach (EntityLiving ep in HostEntity.Parent.FindEntitiesByType<EntityLiving>())
			{
				if (ep == HostEntity ||  ep.IsDying || (ep.MyGroup != EntityGroup.Defender && ep.MyGroup != EntityGroup.Monster))
					continue;
				if (new Rectangle((int)ep.Location.X, (int)(ep.Location.Y), (int)ep.Size.Width, (int)ep.Size.Height)
					.CheckCollision(new Rectangle((int)this.HostEntity.Location.X, (int)this.HostEntity.Location.Y + 8, (int)this.HostEntity.Size.Width, (int)this.HostEntity.Size.Height - 8)))
					ep.Kill();
			}
		}
	}

	public class AIArch : AIBase
	{
		public override bool Use
		{
			get
			{
				return !HostEntity.IsDying;
			}
		}

		int nowstatus = 0;

		int tick = 0;
		public override void onUpdate()
		{
			if (nowstatus != 2 && nowstatus != 0 && tick == 15 || tick == 30)
			{
				if (nowstatus == 2)
				{
					SoundUtility.PlaySound(Sounds.ShootArrow);

					// TODO: Arrow を実装したら、ここのコメントアウトを外す
					int speed = (HostEntity.Direction == Direction.Right ? (DX.GetRand(4) + 1) : (-DX.GetRand(4) - 1));
                    HostEntity.Parent.Add(GameEngine.EntityRegister.CreateEntity("Arrow", new PointF(HostEntity.Location.X + (HostEntity.Direction == Direction.Left ? 0 : HostEntity.Size.Width), HostEntity.Location.Y + HostEntity.Size.Height / 2), GameEngine.mptobjects, GameEngine.chips, HostEntity.Parent, DynamicJson.Parse("{\"Speed\": " + speed + " }")));
				}
				tick = -1;
				nowstatus = (nowstatus + 1) % 4;
			}
			if (HostEntity.Parent.MainEntity.Location.X < HostEntity.Location.X)

			HostEntity.SetGraphic(nowstatus + (HostEntity.Direction == Direction.Right ? 4 : 0));
			tick++;
        }

		public AIArch(EntityLiving el)
		{
			HostEntity = el;
		}


	}

}
