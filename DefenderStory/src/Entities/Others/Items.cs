using System;
using DefenderStory.Entities;
using System.Drawing;
using DefenderStory.AI;
using DefenderStory.Util;
using System.Collections.Generic;
using System.Linq;
using DxLibDLL;
using DefenderStory.Data;
using Codeplex.Data;

namespace DefenderStory.Entities
{
	[EntityRegistry("ItemSpawner", 22)]
	public class EntityItemSpawner : Entity
	{
		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Stage;
			}
		}


		public override void onUpdate(Status ks)
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
				if (m.isRunning && new RectangleF(Location.X - 4, Location.Y + 8, 24, 8).CheckCollision(new RectangleF(m.Location, m.Size)))
				{
					try
					{
						OpenItem((EntityPlayer)Parent.First(new Func<Entity, bool>((s) => s is EntityPlayer)));
					}
					catch (Exception) { }
					break;
				}
			}
		}

		public EntityItemSpawner(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
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
				this.item = (Items)jsonobj.EntityType;
			return base.SetEntityData((object)jsonobj);
		}

		Items item;

		void OpenItem(EntityPlayer player)
		{
			switch (item)
			{
				case Items.Coin:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("Coin", Location, Mpts, Map, Parent, DynamicJson.Parse(@"{""WorkingType"": 1}")));
					break;
				case Items.Pillow:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("Pillow", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent));
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.Leaf:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("Leaf", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent));
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.Pepper:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("Pepper", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent));
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.PepperOrPillow:
					Parent.Add((player.Form == PlayerForm.Mini) ?
						GameEngine.EntityRegister.CreateEntity("Pillow", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent) :
						GameEngine.EntityRegister.CreateEntity("Pepper", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent)
						);
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.IceOrPillow:
					Parent.Add((player.Form == PlayerForm.Mini) ?
						GameEngine.EntityRegister.CreateEntity("Pillow", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent) :
						GameEngine.EntityRegister.CreateEntity("Ice", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent)
						);
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.LeafOrPillow:
					Parent.Add((player.Form == PlayerForm.Mini) ?
						GameEngine.EntityRegister.CreateEntity("Pillow", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent) :
						GameEngine.EntityRegister.CreateEntity("Leaf", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent)
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
						Parent.Add((player.MutekiTime > 0 && player.IsItemMuteki) ?
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
		/// 枕。
		/// </summary>
		Pillow,
		/// <summary>
		/// 唐辛子。
		/// </summary>
		Pepper,
		/// <summary>
		/// アイス。
		/// </summary>
		Ice,
		/// <summary>
		/// リーフ。
		/// </summary>
		Leaf,
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

	[EntityRegistry("Pillow", 32)]
	public class EntityPillow : EntityLiving
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Item;
			}
		}

		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Stage;
			}
		}

		public override RectangleF Collision
		{
			get
			{
				return new RectangleF(2, 2, 12, 14);
			}
		}

		public EntityPillow(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			this.MainAI = new AIWalk(this, -1, 3, 3, 3, 3);
		}

		public override void SetKilledAnime()
		{
			
		}

		public override void SetCrushedAnime()
		{
			
		}

		public override void onUpdate(Status ks)
		{
			foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.PowerUp(PlayerForm.Big);
					this.IsDead = true;
				}
			base.onUpdate(ks);
		}

	}

	[EntityRegistry("PoisonMushroom", 36)]
	public class EntityPoisonMushroom : EntityLiving
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Item;
			}
		}


		const float spdmax = 2;

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Stage;
			}
		}

		public override RectangleF Collision
		{
			get
			{
				return new RectangleF(2, 2, 12, 14);
			}
		}

		public EntityPoisonMushroom(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			this.MainAI = new AIWalk(this, -1, 4, 4, 4, 4);
		}

		public override void SetKilledAnime()
		{

		}

		public override void SetCrushedAnime()
		{

		}

		public override void onUpdate(Status ks)
		{
			foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.Kill();
					this.IsDead = true;
				}
			if (Parent.MainEntity.Location.X < this.Location.X && Velocity.X > -spdmax)
				Velocity.X -= 0.2f;
			if (Parent.MainEntity.Location.X > this.Location.X && Velocity.X < spdmax)
				Velocity.X += 0.2f;

			base.onUpdate(ks);
		}

	}

	[EntityRegistry("Pepper", 33)]
	public class EntityPepper : EntityLiving
	{

		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Item;
			}
		}

		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Stage;
			}
		}

		public EntityPepper(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			SetGraphic(0);
		}

		public override void SetKilledAnime()
		{

		}

		public override void SetCrushedAnime()
		{

		}

		public override void onUpdate(Status ks)
		{
			foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.PowerUp(PlayerForm.Fire);
					this.IsDead = true;
				}
			base.onUpdate(ks);
		}

	}

	[EntityRegistry("Leaf", 34)]
	public class EntityLeaf : EntityLiving
	{
		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Item;
			}
		}

		public override float Gravity
		{
			get
			{
				return 0.01f;
			}
		}

		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Stage;
			}
		}

		public EntityLeaf(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			SetGraphic(2);
			Velocity = new Vector(0, -0.9f);
		}

		public override void SetKilledAnime()
		{

		}

		public override void SetCrushedAnime()
		{

		}

		public override void onUpdate(Status ks)
		{
			foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.PowerUp(PlayerForm.Leaf);
					this.IsDead = true;
				}
			base.onUpdate(ks);
		}

	}

	[EntityRegistry("Feather", 37)]
	public class EntityFeather : EntityLiving
	{
		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Item;
			}
		}


		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Stage;
			}
		}

		public EntityFeather(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			SetGraphic(5);
			Velocity = new Vector(0, -0.9f);
			MainAI = new AIWalk(this, 1, 5, 5, 5, 5);
		}

		public override void SetKilledAnime()
		{

		}

		public override void SetCrushedAnime()
		{

		}

		public override void onUpdate(Status ks)
		{
			if (IsOnLand)
				this.Velocity.Y = -Velocity.Y;
			foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.SetMuteki();
					this.IsDead = true;
				}
			base.onUpdate(ks);
		}

	}

	[EntityRegistry("Coin", 28)]
	public class EntityCoin : EntityGraphical
	{
		public override int[] ImageHandle
		{
			get
			{
				return ResourceUtility.Item;
			}
		}

		

		public override EntityGroup MyGroup
		{
			get
			{
				return EntityGroup.Stage;
			}
		}

		public EntityCoin(PointF pnt, Data.Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			SetGraphic(6);
			Velocity.Y = -12;
		}
		WorkingType worktype = WorkingType.Normal;
		public override Entity SetEntityData(dynamic jsonobj)
		{
			if (jsonobj.IsDefined("WorkingType"))
                this.worktype = (WorkingType)jsonobj.WorkingType;
			base.SetEntityData((object)jsonobj);
			return this;
		}


		bool Animating = false;

		public override void onUpdate(Status ks)
		{
			if (!Animating)
				switch (worktype)
				{
					case WorkingType.Normal:
						Velocity.Y = 0;
						foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
							if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
							{
								Animating = true;
							}
						break;
					case WorkingType.FromBlock:
						Animating = true;
						SoundUtility.PlaySound(Sounds.GetCoin);
						GameEngine.Coin++;
						break;
				}

			if (Animating)
			{
				switch (worktype)
				{
					case WorkingType.Normal:
						this.IsDead = true;
						Parent.Add(GameEngine.EntityRegister.CreateEntity("Coin", Location, Mpts, Map, Parent, DynamicJson.Parse(@"{""WorkingType"": 1}")));
						break;
					case WorkingType.FromBlock:
						Velocity.Y *= 0.8f;
						if (Velocity.Y > -0.1f)
							this.Kill();
						break;
				}
			}

			base.onUpdate(ks);
		}

	}

}