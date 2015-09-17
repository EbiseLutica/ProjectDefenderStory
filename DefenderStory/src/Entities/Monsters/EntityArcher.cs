using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;

namespace DefenderStory.Entities
{
	[EntityRegistry("Archer", 4)]
	public class EntityArcher : EntityLiving
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Archer;
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

		public EntityArcher(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(32, 32);
			MainAI = new AIArch(this);
			CollisionAIs.Add(new AIKillDefender(this));
		}

		public override void SetKilledAnime()
		{
			SetGraphic(Direction == Direction.Left ? 0 : 4);
		}

		public override void SetCrushedAnime()
		{
			SetGraphic(Direction == Direction.Left ? 0 : 4);
			IsCrushed = false;
		}
	}

	[EntityRegistry("Arrow", 67)]
	public class EntityArrow : EntityProjectile
	{
		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Weapon;
			}
		}

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.MonsterWeapon;
			}
		}

		public override Size Size
		{
			get; protected set;
		}

		public EntityArrow(PointF pnt, Data.Object[] obj, byte[,,] chps, EntityList par)
		{
			Mpts = obj;
			Map = chps;
			Parent = par;
			Location = pnt;
			SetGraphic(0);
			Velocity.X = -2;
		}

		public override void onStucked()
		{
			SoundUtility.PlaySound(Sounds.StuckArrow);
			base.onStucked();
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object)jsonobj);
			if (jsonobj.IsDefined("Speed"))
				this.Velocity.X = (float)jsonobj.Speed;
			return this;
		}

		public override void onUpdate(Status ks)
		{
			if (!IsStucked)
			{
				foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
				{
					if (ep.IsDying)
						continue;
					if (new Rectangle((int)ep.Location.X, (int)(ep.Location.Y), (int)ep.Size.Width, (int)ep.Size.Height)
						.CheckCollision(new Rectangle((int)Location.X, (int)Location.Y, (int)Size.Width, (int)Size.Height)))
						ep.Kill();
				}
			}
			base.onUpdate(ks);
		}

	}

}
