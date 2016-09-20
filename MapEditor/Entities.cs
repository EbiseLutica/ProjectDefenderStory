using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
	public class Sign : IEntityData
	{
		[Description("入力するテキスト．")]
		public string MessageText { get; set; }
	}

	[Serializable]
	public class Talkable : IEntityData
	{
		
	}
	
}
