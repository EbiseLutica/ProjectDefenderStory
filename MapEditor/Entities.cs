using System;
using System.ComponentModel;
using System.Drawing.Design;
using TakeUpJewel.Entities;

namespace MapEditor
{
	[Serializable]
	public class Entity
	{
		[Category("配置")]
		[Description("Entity の X 座標．")]
		public int PosX { get; set; }

		[Category("配置")]
		[Description("Entity の Y 座標．")]
		public int PosY { get; set; }

		[Category("配置")]
		[ReadOnly(true)]
		[Description("Entity の種類を表す ID．")]
		public int EntityId { get; set; }

		[Category("配置")]
		[Description("Entity の描画順。この値を基に昇順に描画します(つまり、小さい数字ほど奥に見え、大きい数字ほど手前に見えます。)")]
		public int ZIndex { get; set; }

		[Category("その他")]
		[Description("他の Entity がこの Entity と連携するためのタグ．")]
		public string Tag { get; set; }

		[Category("その他")]
		[Description("この Entity 固有の設定．")]
		public IEntityData EntityData { get; set; }
	}

	[Serializable]
	public abstract class EntityBoss : IEntityData
	{
		[Description("ボスイベントが始まる X 座標．")]
		public double StartX { get; set; }
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public interface IEntityData
	{
	}

	[Serializable]
	public class Player : IEntityData
	{
		[Description("ドアから出現するかどうかのフラグ．")]
		public bool SpawnFromDoor { get; set; }

		[Description("出現対象のドアに関連付けられたタグ．")]
		public string TargetDoorTag { get; set; }
	}

	[Serializable]
	public class InfinitySpawner : IEntityData
	{
		[Description("出現させる対象のエンティティ．")]
		public Entity TargetEntity { get; set; } = new Entity
		{
			PosX = 0,
			PosY = 0,
			EntityId = 0,
			EntityData = null,
			Tag = ""
		};
	}

	[Serializable]
	public class Woody : EntityBoss
	{
	}

	[Serializable]
	public class SpiderString : IEntityData
	{
		[Description("糸をつける対象 Entity と関連付けられたタグ．")]
		public string TargetEntityTag { get; set; }
	}

	[Serializable]
	public class Goal : IEntityData
	{
		[Description("移動先 Level．")]
		public int NextStage { get; set; }
	}

	[Serializable]
	public class ItemSpawner : IEntityData
	{
		[Description("出現するアイテム．")]
		public Items EntityType { get; set; }
	}

	[Serializable]
	public class GoblinGirl : EntityBoss
	{
	}

	[Serializable]
	public class Coin : IEntityData
	{
		[Description("動作モードを指定するが，通常は初期値のままでよい．")]
		public WorkingType WorkingType { get; set; }
	}

	[Serializable]
	public class HalfGoal : IEntityData
	{
		[Description("移動先 Area．")]
		public int NextArea { get; set; }
	}

	[Serializable]
	public class DevilTsubasa : EntityBoss
	{
	}

	[Serializable]
	public class NormalTsubasa : EntityBoss
	{
	}

	[Serializable]
	public class Yuan : EntityBoss
	{
	}

	[Serializable]
	public class Sprite : IEntityData
	{
		public string TextureFile { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public int StartIndex { get; set; }

		public int EndIndex { get; set; }

		public int Speed { get; set; }

		public bool UseAnime { get; set; }
	}

	[Serializable]
	public class Talkable : Sprite
	{
		[Editor(
			 "System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			 typeof(UITypeEditor))]
		[Description("この EntityTalkable が話しかけられたときに実行されるイベントスクリプト。")]
		public string Script { get; set; }
	}

	[Serializable]
	public class Boss : IEntityData
	{
		[Editor(
			 "System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			 typeof(UITypeEditor))]
		[Description("ボスバトル前のイベントスクリプト。")]
		public string StartScript { get; set; }

		[Editor(
			 "System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
			 typeof(UITypeEditor))]
		[Description("ボスバトル後のイベントスクリプト。")]
		public string EndScript { get; set; }

		[Description("ボスバトル開始に必要なメインEntityとの距離。")]
		public int StartKyori { get; set; } = 256;
	}
}