using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Codeplex.Data;
using TakeUpJewel.AI;
using TakeUpJewel.Data;
using TakeUpJewel.Util;
using static TakeUpJewel.Util.DevelopUtility;

namespace TakeUpJewel.Entities
{
	[EntityRegistry("ItemSpawner", 22)]
	public class EntityItemSpawner : Entity
	{
		private Items _item;

		public EntityItemSpawner(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
		}

		public override EntityGroup MyGroup => EntityGroup.Stage;


		public override void OnUpdate(Status ks)
		{
			var lst = new List<Entity>(Parent.FindEntitiesByType<EntityPlayer>());
			foreach (var entity in lst)
			{
				var ep = (EntityPlayer) entity;
				if (ep.IsDying)
					continue;
				if (
					new RectangleF(ep.Location.X, ep.Location.Y + 1, ep.Size.Width, ep.Size.Height - 1).CheckCollision(
						new RectangleF(Location.X + 2, Location.Y + 4, 12, 12)) && ep.Velocity.Y < 0)
					OpenItem(ep);
			}
			foreach (var entity in new List<Entity>(Parent.FindEntitiesByType<EntityTurcosShell>()))
			{
				var m = (EntityTurcosShell) entity;
				if (m.IsRunning &&
					new RectangleF(Location.X - 4, Location.Y + 8, 24, 8).CheckCollision(new RectangleF(m.Location, m.Size)))
				{
					try
					{
						OpenItem((EntityPlayer) Parent.First(s => s is EntityPlayer));
					}
					catch
					{
						// 握りつぶす
					}
					break;
				}
			}
		}

		public override Entity SetEntityData(dynamic jsonobj)
		{
			if (jsonobj.IsDefined("EntityType"))
				_item = (Items) jsonobj.EntityType;
			return base.SetEntityData((object) jsonobj);
		}

		private void OpenItem(EntityPlayer player)
		{
			switch (_item)
			{
				case Items.Coin:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("Coin", Location, Mpts, Map, Parent,
						DynamicJson.Parse(@"{""WorkingType"": 1}")));
					break;
				case Items.SoulChocolate:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("SoulChocolate", new PointF(Location.X, Location.Y - 16), Mpts,
						Map, Parent));
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.Grimoire:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("Grimoire", new PointF(Location.X, Location.Y - 16), Mpts, Map,
						Parent));
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.FireWands:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("FireWands", new PointF(Location.X, Location.Y - 16), Mpts, Map,
						Parent));
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.PepperOrPillow:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("FireWands", new PointF(Location.X, Location.Y - 16), Mpts, Map,
							Parent)
					);
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.IcyPendant:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("IcyPendant", new PointF(Location.X, Location.Y - 6), Mpts, Map,
							Parent)
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
					Parent.Add(GameEngine.EntityRegister.CreateEntity("Grimoire", new PointF(Location.X, Location.Y - 16), Mpts, Map,
							Parent)
					);
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
				case Items.Feather:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("Feather", new PointF(Location.X, Location.Y - 16), Mpts, Map,
						Parent));
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;

				case Items.FeatherOrCoin:
					if (player.MutekiTime != 0)
					{
						Parent.Add((player.MutekiTime > 0) && player.IsItemMuteki
								? GameEngine.EntityRegister.CreateEntity("Feather", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent)
								: GameEngine.EntityRegister.CreateEntity("Coin", new PointF(Location.X, Location.Y - 16), Mpts, Map, Parent,
									DynamicJson.Parse(@"{""WorkingType"": 1}"))
						);
						SoundUtility.PlaySound(Sounds.ItemSpawn);
					}
					else
						Parent.Add(GameEngine.EntityRegister.CreateEntity("Coin", Location, Mpts, Map, Parent,
							DynamicJson.Parse(@"{""WorkingType"": 1}")));
					break;
				case Items.PoisonMushroom:
					Parent.Add(GameEngine.EntityRegister.CreateEntity("PoisonMushroom", new PointF(Location.X, Location.Y - 16), Mpts,
						Map, Parent));
					SoundUtility.PlaySound(Sounds.ItemSpawn);
					break;
			}
			//PlaySound(Sounds.ItemSpawn);
			Map[(int) (Location.X / 16), (int) (Location.Y / 16), 0] = 10;
			Kill(); //役目が終わったので殺す
		}
	}

	/// <summary>
	///     ゲームのアイテムを指定します。
	/// </summary>
	public enum Items
	{
		/// <summary>
		///     ライフアップするチョコ。
		/// </summary>
		SoulChocolate,

		/// <summary>
		///     ファイアーステッキ。
		/// </summary>
		FireWands,

		/// <summary>
		///     アイシーペンダント。
		/// </summary>
		IcyPendant,

		/// <summary>
		///     魔導書。
		/// </summary>
		Grimoire,

		/// <summary>
		///     毒キノコ。
		/// </summary>
		PoisonMushroom,

		/// <summary>
		///     羽。
		/// </summary>
		Feather,

		/// <summary>
		///     唐辛子、プレイヤーが小さければ枕。
		/// </summary>
		PepperOrPillow,

		/// <summary>
		///     唐辛子、プレイヤーが小さければ枕。
		/// </summary>
		IceOrPillow,

		/// <summary>
		///     唐辛子、プレイヤーが小さければ枕。
		/// </summary>
		LeafOrPillow,

		/// <summary>
		///     唐辛子、羽状態でなければコイン。
		/// </summary>
		FeatherOrCoin,

		/// <summary>
		///     コイン。
		/// </summary>
		Coin
	}

	/// <summary>
	///     コインの挙動タイプを指定します。
	/// </summary>
	public enum WorkingType
	{
		/// <summary>
		///     メインキャラが触れると取得できるタイプ。
		/// </summary>
		Normal,

		/// <summary>
		///     ブロックから出てくるタイプ。
		/// </summary>
		FromBlock
	}

	[EntityRegistry("SoulChocolate", 32)]
	public class EntityChocolate : EntityFlying
	{
		public EntityChocolate(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			Velocity = new Vector(0, -2.4f);
			SetGraphic(5);
		}

		public static bool Nikaime { get; set; }
		public override int[] ImageHandle => ResourceUtility.Item;


		public override EntityGroup MyGroup => EntityGroup.Stage;

		public override RectangleF Collision => new RectangleF(0, 0, 16, 16);

		public override void SetKilledAnime()
		{
		}

		public override void SetCrushedAnime()
		{
		}

		public override void OnUpdate(Status ks)
		{
			if (!Nikaime)
			{
				EventRuntime.AddScript(GetItemDescription(@"ソウルチョコレート\n命の　すべてが　詰まった　フシギな　チョコレート。", "プレイヤーの　体力が　1だけ　回復する。"));
				Nikaime = true;
			}
			if (Velocity.Y < 0)
				Velocity.Y += 0.1f;
			else if (MainAi == null)
				MainAi = new AiFlySine(this, 0, 5, 5, 5, 5);
			foreach (var entity in Parent.FindEntitiesByType<EntityPlayer>())
			{
				var ep = (EntityPlayer) entity;
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.Life++;
					SoundUtility.PlaySound(Sounds.LifeUp);
					IsDead = true;
				}
			}
			base.OnUpdate(ks);
		}
	}

	[EntityRegistry("PoisonMushroom", 36)]
	public class EntityPoisonMushroom : EntityLiving
	{
		private const float Spdmax = 2;

		public EntityPoisonMushroom(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			MainAi = new AiWalk(this, -1, 4, 4, 4, 4);
		}

		public override int[] ImageHandle => ResourceUtility.Item;

		public override EntityGroup MyGroup => EntityGroup.Stage;

		public override RectangleF Collision => new RectangleF(2, 2, 12, 14);

		public override void SetKilledAnime()
		{
		}

		public override void SetCrushedAnime()
		{
		}

		public override void OnUpdate(Status ks)
		{
			foreach (var entity in Parent.FindEntitiesByType<EntityPlayer>())
			{
				var ep = (EntityPlayer) entity;
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.Kill();
					IsDead = true;
				}
			}
			if ((Parent.MainEntity.Location.X < Location.X) && (Velocity.X > -Spdmax))
				Velocity.X -= 0.2f;
			if ((Parent.MainEntity.Location.X > Location.X) && (Velocity.X < Spdmax))
				Velocity.X += 0.2f;

			base.OnUpdate(ks);
		}
	}

	[EntityRegistry("FireWands", 33)]
	public class EntityPepper : EntityLiving
	{
		public EntityPepper(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			SetAnime(0, 3, 8);
			Velocity = new Vector(0, -3.0f);
		}

		public override int[] ImageHandle => ResourceUtility.Item;


		public static bool Nikaime { get; set; }
		public override EntityGroup MyGroup => EntityGroup.Stage;

		public override void SetKilledAnime()
		{
		}

		public override void SetCrushedAnime()
		{
		}

		public override void OnUpdate(Status ks)
		{
			if (!Nikaime)
			{
				EventRuntime.AddScript(GetItemDescription(@"ほのおのつえ\nなくならない　燃料が　詰められた　つえ。",
					"ファイアボールを　撃てるようになる。　ファイアボールは　向いている　方向に　発射でき、　上下キーで　向きを　調節可能。"));
				Nikaime = true;
			}


			foreach (var entity in Parent.FindEntitiesByType<EntityPlayer>())
			{
				var ep = (EntityPlayer) entity;
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.PowerUp(PlayerForm.Fire);
					IsDead = true;
				}
			}
			base.OnUpdate(ks);
		}
	}

	[EntityRegistry("IcyPendant", -1)]
	public class EntityIcyPendant : EntityLiving
	{
		public EntityIcyPendant(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			SetGraphic(4);
		}

		public static bool Nikaime { get; set; }
		public override int[] ImageHandle => ResourceUtility.Item;


		public override EntityGroup MyGroup => EntityGroup.Stage;

		public override void SetKilledAnime()
		{
		}

		public override void SetCrushedAnime()
		{
		}

		public override void OnUpdate(Status ks)
		{
			if (!Nikaime)
			{
				EventRuntime.AddScript(GetItemDescription(@"アイシーペンダント\n触ると　つめたい　アイシークリスタルで　つくられた　ペンダント。",
					"氷ブロックを　発射できるようになる。", "氷ブロックは　床を滑りながら　敵を倒し　滑り続けると　小さくなり　最終的には　消える。"));
				Nikaime = true;
			}

			foreach (var entity in Parent.FindEntitiesByType<EntityPlayer>())
			{
				var ep = (EntityPlayer) entity;
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.PowerUp(PlayerForm.Ice);
					IsDead = true;
				}
			}
			base.OnUpdate(ks);
		}
	}

	[EntityRegistry("Grimoire", 34)]
	public class EntityGrimoire : EntityLiving
	{
		public EntityGrimoire(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
			Size = new Size(16, 16);
			SetAnime(10, 13, 8);
			Velocity = new Vector(0, -2.0f);
		}

		public override int[] ImageHandle => ResourceUtility.Item;

		public static bool Nikaime { get; set; }

		public override float Gravity => 0.05f;

		public override EntityGroup MyGroup => EntityGroup.Stage;

		public override void SetKilledAnime()
		{
		}

		public override void SetCrushedAnime()
		{
		}

		public override void OnUpdate(Status ks)
		{
			if (!Nikaime)
			{
				EventRuntime.AddScript(GetItemDescription("グリモワール(魔導書)\n魔法の　すべてが　記されている。",
					"魔法弾が撃てるようになる。", "魔法弾は　射程距離が　短いが　最も近くの　モンスターを　たおす。"));
				Nikaime = true;
			}
			Velocity.X *= 0.98f;
			foreach (var entity in Parent.FindEntitiesByType<EntityPlayer>())
			{
				var ep = (EntityPlayer) entity;
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.PowerUp(PlayerForm.Magic);
					IsDead = true;
				}
			}
			base.OnUpdate(ks);
		}
	}

	[EntityRegistry("Feather", 37)]
	public class EntityFeather : EntityLiving
	{
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

		public override int[] ImageHandle => ResourceUtility.Item;


		public override EntityGroup MyGroup => EntityGroup.Stage;

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
			foreach (var entity in Parent.FindEntitiesByType<EntityPlayer>())
			{
				var ep = (EntityPlayer) entity;
				if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
				{
					ep.SetMuteki();
					IsDead = true;
				}
			}
			base.OnUpdate(ks);
		}
	}

	[EntityRegistry("Coin", 28)]
	public class EntityCoin : EntityGraphical
	{
		private bool _animating;
		private WorkingType _worktype = WorkingType.Normal;

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

		public override int[] ImageHandle => ResourceUtility.Item;


		public override EntityGroup MyGroup => EntityGroup.Stage;
		public static bool Nikaime { get; set; }

		public override Entity SetEntityData(dynamic jsonobj)
		{
			if (jsonobj.IsDefined("WorkingType"))
				_worktype = (WorkingType) jsonobj.WorkingType;
			base.SetEntityData((object) jsonobj);
			if (_worktype == WorkingType.FromBlock)
				AnimeSpeed = 3;
			return this;
		}

		public override void OnUpdate(Status ks)
		{
			if (!_animating)
				switch (_worktype)
				{
					case WorkingType.Normal:
						Velocity.Y = 0;
						foreach (var entity in Parent.FindEntitiesByType<EntityPlayer>())
						{
							var ep = (EntityPlayer) entity;
							if (!ep.IsDying && new RectangleF(ep.Location, ep.Size).CheckCollision(new RectangleF(Location, Size)))
								_animating = true;
						}
						break;
					case WorkingType.FromBlock:
						_animating = true;
						SoundUtility.PlaySound(Sounds.GetCoin);
						GameEngine.Coin++;
						if (!Nikaime)
						{
							EventRuntime.AddScript(GetItemDescription("トランジスタコイン\nトランジスタ王国の　通貨。",
								"50枚　集めると　残機が　1だけ　増える。"));
							Nikaime = true;
						}
						break;
				}

			if (_animating)
				switch (_worktype)
				{
					case WorkingType.Normal:
						IsDead = true;
						Parent.Add(GameEngine.EntityRegister.CreateEntity("Coin", Location, Mpts, Map, Parent,
							DynamicJson.Parse(@"{""WorkingType"": 1}")));
						break;
					case WorkingType.FromBlock:
						Velocity.Y *= 0.8f;
						if (Velocity.Y > -0.1f)
							Kill();
						break;
				}

			base.OnUpdate(ks);
		}
	}
}