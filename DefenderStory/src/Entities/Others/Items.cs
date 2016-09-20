using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Codeplex.Data;
using TakeUpJewel.AI;
using TakeUpJewel.Data;
using TakeUpJewel.Util;

namespace TakeUpJewel.Entities
{
	[EntityRegistry("ItemSpawner", 22)]
	public class EntityItemSpawner : Entity
	{
		public override EntityGroup MyGroup => EntityGroup.Stage;


		public override void OnUpdate(Status ks)
		{
			var lst = new List<Entity>(Parent.FindEntitiesByType<EntityPlayer>());
			foreach (EntityPlayer ep in lst)
			{
				if (ep.IsDying)
					continue;
				if (new RectangleF(ep.Location.X, ep.Location.Y, ep.Size.Width, ep.Size.Height).CheckCollision(new RectangleF(Location.X + 7, Location.Y + 8, 2, 10)))
				{
					OpenItem(ep);
				}
			}
			foreach (EntityTurcosShell m in new List<Entity>(Parent.FindEntitiesByType<EntityTurcosShell>()))
			{
				if (m.IsRunning && new RectangleF(Location.X - 4, Location.Y + 8, 24, 8).CheckCollision(new RectangleF(m.Location, m.Size)))
				{
					try
					{
						OpenItem((EntityPlayer)Parent.First(s => s is EntityPlayer));
					}
					catch
					{

					}
					break;
				}
			}
		}

		public EntityItemSpawner(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			if (jsonobj.IsDefined("EntityType"))
				_item = (Items)jsonobj.EntityType;
			return base.SetEntityData((object)jsonobj);
		}

		Items _item;

		void OpenItem(EntityPlayer player)
		{
			switch (_item)
			{
				case Items.Coin:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("Coin", Location, Mpts, Map, Parent, DynamicJson.Parse(@"{""WorkingType"": 1}")));
					break;
				case Items.SoulChocolate:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("SoulChocolate", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent));
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.Grimoire:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("Grimoire", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent));
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.FireWands:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("FireWands", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent));
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.PepperOrPillow:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("FireWands", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent)
						);
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.IceOrPillow:
					//Parent.Add((player.Form == PlayerForm.Mini) ?
					//	GameEngine.EntityRegister.CreateEntity("SoulChocolate", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent) :
					//	GameEngine.EntityRegister.CreateEntity("IcyPendant", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent)
					//	);
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.LeafOrPillow:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("Grimoire", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent)
						);
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.Feather:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("Feather", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent));
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;

				case Items.FeatherOrCoin:
					if (player.MutekiTime != 0)
					{
						Parent.Add(player.MutekiTime > 0 && player.IsItemMuteki ?
						GameEngine.EntityRegister.CreateEntity("Feather", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent) :
						GameEngine.EntityRegister.CreateEntity("Coin", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent, DynamicJson.Parse(@"{""WorkingType"": 1}"))
						);
						SoundUtility.PlaySound(Sounds.ItemSpawn);
					}
					else
						Parent.Add(GameEngine.EntityRegister.CreateEntity("Coin", Location, Mpts, Map, Parent, DynamicJson.Parse(@"{""WorkingType"": 1}")));
					break;
				case Items.PoisonMushroom:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("PoisonMushroom", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent));
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
			}
			//PlaySound(Sounds.ItemSpawn);
			Map[(int)(Location.X / 16), (int)(Location.Y / 16), 0] = 10;
			Kill(); //役目が終わったので殺す

		}

	}

	/// <summary>
	/// ゲームのアイテムを指定します。
	/// </summary>
	public enum Items
	{
		/// <summary>
		///ライフアップするチョコ。
		/// </summary>
		SoulChocolate,
		/// <summary>
		/// ファイアーステッキ。
		/// </summary>
		FireWands,
		/// <summary>
		/// アイシーペンダント。
		/// </summary>
		IcyPendant,
		/// <summary>
		/// 魔導書。
		/// </summary>
		Grimoire,
		/// <summary>
		/// 毒キノコ。
		/// </summary>
		PoisonMushroom,
		/// <summary>
		/// 羽。
		/// </summary>
		Feather,
		/// <summary>
		/// 唐辛子、プレイヤーが小さければ枕。
		/// </summary>
		PepperOrPillow,
		/// <summary>
		/// 唐辛子、プレイヤーが小さければ枕。
		/// </summary>
		IceOrPillow,
		/// <summary>
		/// 唐辛子、プレイヤーが小さければ枕。
		/// </summary>
		LeafOrPillow,
		/// <summary>
		/// 唐辛子、羽状態でなければコイン。
		/// </summary>
		FeatherOrCoin,
		/// <summary>
		/// コイン。
		/// </summary>
		Coin
	}

	/// <summary>
	/// コインの挙動タイプを指定します。
	/// </summary>
	public enum WorkingType
	{
		/// <summary>
		/// メインキャラが触れると取得できるタイプ。
		/// </summary>
		Normal,
		/// <summary>
		/// ブロックから出てくるタイプ。
		/// </summary>
		FromBlock
	}

	[EntityRegistry("SoulChocolate", 32)]
	public class EntityChocolate : EntityLiving
	{

		public override int[] ImageHandle => ResourceUtility.Item;


		public override EntityGroup MyGroup => EntityGroup.Stage;

		public override RectangleF Collision => new RectangleF(0, 0, 16, 16);

		public EntityChocolate(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			MainAi = new AiWalk(this, -1, 5, 5, 5, 5);
		}

		public override void SetKilledAnime()
		{
			
		}

		public override void SetCrushedAnime()
		{
			
		}

		public override void OnUpdate(Status ks)
		{
			foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.Life++;
					
					IsDead = true;
				}
			base.OnUpdate(ks);
		}

	}

	[EntityRegistry("PoisonMushroom", 36)]
	public class EntityPoisonMushroom : EntityLiving
	{

		public override int[] ImageHandle => ResourceUtility.Item;


		const float Spdmax = 2;

		public override EntityGroup MyGroup => EntityGroup.Stage;

		public override RectangleF Collision => new RectangleF(2, 2, 12, 14);

		public EntityPoisonMushroom(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			MainAi = new AiWalk(this, -1, 4, 4, 4, 4);
		}

		public override void SetKilledAnime()
		{

		}

		public override void SetCrushedAnime()
		{

		}

		public override void OnUpdate(Status ks)
		{
			foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.Kill();
					IsDead = true;
				}
			if (Parent.MainEntity.Location.X < Location.X && Velocity.X > -Spdmax)
				Velocity.X -= 0.2f;
			if (Parent.MainEntity.Location.X > Location.X && Velocity.X < Spdmax)
				Velocity.X += 0.2f;

			base.OnUpdate(ks);
		}

	}

	[EntityRegistry("FireWands", 33)]
	public class EntityPepper : EntityLiving
	{

		public override int[] ImageHandle => ResourceUtility.Item;


		public override EntityGroup MyGroup => EntityGroup.Stage;

		public EntityPepper(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			SetAnime(0, 3, 8);
		}

		public override void SetKilledAnime()
		{

		}

		public override void SetCrushedAnime()
		{

		}

		public override void OnUpdate(Status ks)
		{
			foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.PowerUp(PlayerForm.Fire);
					IsDead = true;
				}
			base.OnUpdate(ks);
		}

	}

	[EntityRegistry("IcyPendant", -1)]
	public class EntityIcyPendant : EntityLiving
	{

		public override int[] ImageHandle => ResourceUtility.Item;


		public override EntityGroup MyGroup => EntityGroup.Stage;

		public EntityIcyPendant(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			SetGraphic(4);
		}

		public override void SetKilledAnime()
		{

		}

		public override void SetCrushedAnime()
		{

		}

		public override void OnUpdate(Status ks)
		{
			foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.PowerUp(PlayerForm.Ice);
					IsDead = true;
				}
			base.OnUpdate(ks);
		}

	}

	[EntityRegistry("Grimoire", 34)]
	public class EntityGrimoire : EntityLiving
	{
		public override int[] ImageHandle => ResourceUtility.Item;
		


		public override EntityGroup MyGroup => EntityGroup.Stage;

		public EntityGrimoire(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			SetAnime(10, 13, 8);
			Velocity = new Vector(0, -0.9f);
		}

		public override void SetKilledAnime()
		{

		}

		public override void SetCrushedAnime()
		{

		}

		public override void OnUpdate(Status ks)
		{
			foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.PowerUp(PlayerForm.Magic);
					IsDead = true;
				}
			base.OnUpdate(ks);
		}

	}

	[EntityRegistry("Feather", 37)]
	public class EntityFeather : EntityLiving
	{
		public override int[] ImageHandle => ResourceUtility.Item;


		public override EntityGroup MyGroup => EntityGroup.Stage;

		public EntityFeather(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			SetGraphic(5);
			Velocity = new Vector(0, -0.9f);
			MainAi = new AiWalk(this, 1, 5, 5, 5, 5);
		}

		public override void SetKilledAnime()
		{

		}

		public override void SetCrushedAnime()
		{

		}

		public override void OnUpdate(Status ks)
		{
			if (IsOnLand)
				Velocity.Y = -Velocity.Y;
			foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.SetMuteki();
					IsDead = true;
				}
			base.OnUpdate(ks);
		}

	}

	[EntityRegistry("Coin", 28)]
	public class EntityCoin : EntityGraphical
	{
		public override int[] ImageHandle => ResourceUtility.Item;


		public override EntityGroup MyGroup => EntityGroup.Stage;

		public EntityCoin(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			SetAnime(6, 9, 8);
			
			Velocity.Y = -12;
		}
		WorkingType _worktype = WorkingType.Normal;
		public override Entity SetEntityData(dynamic jsonobj)
		{
			if (jsonobj.IsDefined("WorkingType"))
                _worktype = (WorkingType)jsonobj.WorkingType;
			base.SetEntityData((object)jsonobj);
			if (_worktype == WorkingType.FromBlock)
				AnimeSpeed = 3;
			return this;
		}


		bool _animating;

		public override void OnUpdate(Status ks)
		{
			if (!_animating)
				switch (_worktype)
				{
					case WorkingType.Normal:
						Velocity.Y = 0;
						foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
							if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
							{
								_animating = true;
							}
						break;
					case WorkingType.FromBlock:
						_animating = true;
						SoundUtility.PlaySound(Sounds.GetCoin);
						GameEngine.Coin++;
						break;
				}

			if (_animating)
			{
				switch (_worktype)
				{
					case WorkingType.Normal:
						IsDead = true;
						Parent.Add(GameEngine.EntityRegister.CreateEntity("Coin", Location, Mpts, Map, Parent, DynamicJson.Parse(@"{""WorkingType"": 1}")));
						break;
					case WorkingType.FromBlock:
						Velocity.Y *= 0.8f;
						if (Velocity.Y > -0.1f)
							Kill();
						break;
				}
			}

			base.OnUpdate(ks);
		}

	}

}