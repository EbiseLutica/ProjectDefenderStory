using System.Drawing;
using TakeUpJewel.AI;
using TakeUpJewel.Data;
using TakeUpJewel.Util;

namespace TakeUpJewel.Entities
{
	[EntityRegistry("Archer", 4)]
	public class EntityArcher : EntityLiving
	{
		public EntityArcher(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(32, 32);
			MainAi = new AiArch(this);
			CollisionAIs.Add(new AiKillDefender(this));
		}

		public override int[] ImageHandle => ResourceUtility.Archer;


		public override EntityGroup MyGroup => EntityGroup.Enemy;

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
		public EntityArrow(PointF pnt, Object[] obj, byte[,,] chps, EntityList par)
		{
			Mpts = obj;
			Map = chps;
			Parent = par;
			Location = pnt;
			SetGraphic(0);
			Velocity.X = -2;
		}

		public override int[] ImageHandle => ResourceUtility.Weapon;

		public override EntityGroup MyGroup => EntityGroup.MonsterWeapon;

		public override void OnStucked()
		{
			SoundUtility.PlaySound(Sounds.StuckArrow);
			base.OnStucked();
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			base.SetEntityData((object) jsonobj);
			if (jsonobj.IsDefined("Speed"))
				Velocity.X = (float) jsonobj.Speed;
			return this;
		}

		public override void OnUpdate(Status ks)
		{
			if (!IsStucked)
				foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
				{
					if (ep.IsDying)
						continue;
					if (new Rectangle((int) ep.Location.X, (int) ep.Location.Y, ep.Size.Width, ep.Size.Height)
						.CheckCollision(new Rectangle((int) Location.X, (int) Location.Y, Size.Width, Size.Height)))
						ep.Kill();
				}
			base.OnUpdate(ks);
		}
	}
}