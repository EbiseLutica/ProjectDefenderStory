using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;
using Codeplex.Data;
using DefenderStory.Data;

namespace DefenderStory.Entities
{
	[EntityRegistry("Turcos_Green", 9)]
	public class EntityTurcos : EntityLiving
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Turcos;
			}
		}

		public override Size Size
		{
			get; protected set;
		}

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Monster;
			}
		}

		public EntityTurcos(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;

			Size = new Size(24, 16);
			MainAI = new AIWalk(this, 1, 0, 1, 3, 4);
			CollisionAIs.Add(new AIKillDefender(this));

		}

		public override RectangleF Collision
		{
			get
			{
				return new RectangleF(2, 2, 20, 14);
			}
		}

		public override void SetKilledAnime()
		{
			
		}

		public override void SetCrushedAnime()
		{
			
		}

		public override void Kill()
		{
			if (IsDying)
				return;
			if (IsCrushed)
			{
				Parent.Add(new EntityTurcosShell(Location, Mpts, Map, Parent));
				this.IsDead = true;
			}
			else
			{
				IsDying = true;
			}
		}

		

	}

	[EntityRegistry("Turcos_Red", 10)]
	public class EntityTurcosRed : EntityTurcos
	{
		public EntityTurcosRed(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
			: base(pnt, obj, chips, par)
		{
			MainAI = new AIWalk(this, 1, 6, 7, 9, 10);
		}

		public override void Kill()
		{
			Parent.Add(new EntityTurcosShellRed(Location, Mpts, Map, Parent));
			this.IsDead = true;
		}

	}

	public class EntityTurcosShell : EntityLiving
	{
		const int speed = 3;
		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.TurcosShell;
			}
		}

		public override Size Size
		{
			get; protected set;
		}

		public override EntityGroup MyGroup
		{
			get
			{
				return (isRunning ? EntityGroup.Monster : EntityGroup.MonsterWeapon);
			}
		}

		protected AIKiller killai;

		public EntityTurcosShell(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			killai = new AIKiller(this);
			Size = new Size(16, 16);
			mutekitime = mutekimax;
			SetGraphic(0);
		}

		public override RectangleF Collision
		{
			get
			{
				return new RectangleF(2, 2, 12, 14);
			}
		}

		public override void CheckCollision()
		{
			if (!IsDying && (Owner == null || isRunning))
				base.CheckCollision();
		}

		public override void SetKilledAnime()
		{
			
		}

		public override void SetCrushedAnime()
		{
			
		}
		public bool isRunning = false;
		public int mutekitime = 0;

		public EntityLiving Owner
		{
			get; set;
		}

		

		public override void Kill()
		{
			if (mutekitime > 0)
				return;
			if (IsDying)
				return;
			if (Owner != null && GameEngine.ks.inlshift)
			{
				return;
			}
			if (!IsCrushed || IsFall)
			{
				if (IsFall && !IsCrushed)
					IsDead = true;
				else if (!IsCrushed)
					IsDying = true;
				SoundUtility.PlaySound(Sounds.Killed);
				return;
			}
			SwitchMode();
		}



		const int mutekimax = 20;
		protected virtual void SwitchMode()
		{
			if (!isRunning)
			{
				isRunning = true;
				mutekitime = mutekimax;
				Owner = null;
				SoundUtility.PlaySound(Sounds.Killed);
				Velocity.X = (Parent.MainEntity.Location.X < Location.X) ? speed : -speed;
				//Location.X += Velocity.X * 8;
				Velocity.Y = 0;
			}
			else
			{
				isRunning = false;
				mutekitime = mutekimax;
				if (CollisionAIs.Contains(killai))
					CollisionAIs.Remove(killai);
				SoundUtility.PlaySound(Sounds.Stepped);
				Velocity.X = 0;
				Velocity.Y = 0;
			}
		}

		public override void onUpdate(Status ks)
		{
			if (IsDying)
			{
				base.onUpdate(ks);
				return;
			}
			if (mutekitime > 0)
				mutekitime--;
			if (isRunning && mutekitime == 0 && CollisionAIs.Count == 0)
				CollisionAIs.Add(killai);
			if (CollisionLeft() == ObjectHitFlag.Hit || Location.X <= 0)
			{
				Velocity.X = speed;
			}
			if (CollisionRight() == ObjectHitFlag.Hit || Location.X >= GameEngine.map.Width * 16 - 1)
			{
				Velocity.X = -speed;
			}

			if (Owner != null && !ks.inlshift)
			{
				SwitchMode();
				Owner = null;
			}
			if (Owner != null && !isRunning)
			{
				foreach (EntityLiving ep in Parent.FindEntitiesByType<EntityLiving>())
				{
					if (ep == this || ep.IsDying || ep.MyGroup == EntityGroup.Defender || ep.MyGroup != EntityGroup.Monster)
						continue;
					if (new Rectangle((int)ep.Location.X, (int)(ep.Location.Y), (int)ep.Size.Width, (int)ep.Size.Height)
						.CheckCollision(new Rectangle((int)Location.X, (int)Location.Y + 8, (int)Size.Width, (int)Size.Height - 8)))
						ep.Kill();
				}
			}
			if (mutekitime == 0 && !isRunning && !ks.inlshift && new RectangleF(Parent.MainEntity.Location, Parent.MainEntity.Size).CheckCollision(new RectangleF(Location, Size)))
				SwitchMode();
			base.onUpdate(ks);
		}

		public override void Move()
		{
			if (IsDying)
				return;
			if (Owner != null && GameEngine.ks.inlshift)
			{
				this.Location = new PointF(Owner.Location.X + (Owner.Direction == Direction.Left ? -this.Size.Width : Owner.Size.Width), Owner.Location.Y + Owner.Size.Height / 2 - this.Size.Height / 2);
			}
			if (Owner == null || isRunning)
				base.Move();
		}

		public override void Dying()
		{
			Velocity.Y = -1f;
		}


	}

	public class EntityTurcosShellRed : EntityTurcosShell
	{
		public EntityTurcosShellRed(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
			: base(pnt, obj, chips, par)
		{
			SetGraphic(1);
		}
    }

}
