using System.Drawing;
using TakeUpJewel.Data;
using TakeUpJewel.Util;
using static DxLibDLL.DX;
using static TakeUpJewel.Util.ResourceUtility;

namespace TakeUpJewel.Entities
{
	/// <summary>
	///     MainEntity が衝突した瞬間 スクリプトが発動するEntity。
	/// </summary>
	[EntityRegistry(nameof(EntityTrigger), 91)]
	public class EntityTrigger : Entity
	{
		private bool _bTriggered;
		private bool _executed;
		private string _myScript;
		private ScriptRepeatingOption _option;
		private bool _triggered;

		public EntityTrigger(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
		}

		public override EntityGroup MyGroup => EntityGroup.System;


		/// <summary>
		///     Tick 毎に呼ばれる Entity の処理イベントです。
		/// </summary>
		/// <param name="ks"></param>
		public override void OnUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			var ep = Parent.MainEntity;
			base.OnUpdate(ks);
			// プレイヤーと自分の当たり判定があったとき、スクリプト実行。
			// ReSharper disable once AssignmentInConditionalExpression
			if (_triggered = new Rectangle((int) ep.Location.X, (int) ep.Location.Y, ep.Size.Width, ep.Size.Height)
				.CheckCollision(new Rectangle((int) Location.X, (int) Location.Y, Size.Width,
					Size.Height)))
			{
				// 実行済みなら実行しない
				if ((_option == ScriptRepeatingOption.NoRepeat) && _executed)
					return;
				if ((_option == ScriptRepeatingOption.RepeatWhenMainEntityLeaveAndReenter) && _bTriggered)
					return;

				EventRuntime.AddScript(new EventScript(_myScript));
				_executed = true;
			}
		}

		/// <summary>
		///     Entity 生成時にメタデータが渡されると、このメソッドが呼ばれます。
		/// </summary>
		/// <param name="jsonobj"></param>
		/// <returns></returns>
		public override Entity SetEntityData(dynamic jsonobj)
		{
			//TODO: メタデータに埋め込まれたスクリプトを取得する
			if (jsonobj.Script())
				_myScript = jsonobj.Script;

			base.SetEntityData((object) jsonobj);
			return this;
		}
	}

	public enum ScriptRepeatingOption
	{
		/// <summary>
		///     1度きり実行する。
		/// </summary>
		NoRepeat,

		/// <summary>
		///     衝突中はスクリプトを常時実行(ランタイムが処理中でなければ)
		/// </summary>
		RepeatWhileMainEntityOnMe,

		/// <summary>
		///     一旦離れて再び入ったら再度実行
		/// </summary>
		RepeatWhenMainEntityLeaveAndReenter
	}

	/// <summary>
	///     プレイヤーが話しかけられる Entity です。
	/// </summary>
	[EntityRegistry(nameof(EntityTalkable), 89)]
	public class EntityTalkable : EntitySprite
	{
		private bool _canExecuteScript;
		private string _myScript;

		public EntityTalkable(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
			: base(pnt, obj, chips, par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
		}

		public override EntityGroup MyGroup => EntityGroup.System;

		/// <summary>
		///     死んでいるアニメーションを設定します。
		/// </summary>
		public override void SetKilledAnime()
		{
		}

		/// <summary>
		///     踏みつけられたアニメーションを設定します。
		/// </summary>
		public override void SetCrushedAnime()
		{
		}

		/// <summary>
		///     Tick 毎に呼ばれる Entity の処理イベントです。
		/// </summary>
		/// <param name="ks"></param>
		public override void OnUpdate(Status ks)
		{
			//TODO: ここにこの Entity が行う処理を記述してください。
			foreach (EntityPlayer ep in Parent.FindEntitiesByType<EntityPlayer>())
			{
				if (ep.IsDying)
					continue;


				// プレイヤーと自分の当たり判定があり、上キーが押されたとき、スクリプト実行
				if ((_canExecuteScript = new Rectangle((int) ep.Location.X, (int) ep.Location.Y, ep.Size.Width, ep.Size.Height)
						.CheckCollision(new Rectangle((int) Location.X, (int) Location.Y, Size.Width,
							Size.Height))) && ks.Inup)
					try
					{
						EventRuntime.AddScript(new EventScript(_myScript));
					}
					catch (EventScript.EventScriptException ex)
					{
						EventRuntime.AddScript(new EventScript($@"[enstop]
[mesbox:down]
[mes:""エラー！\n{ex.Message.Replace(@"\", @"\\").Replace(@"""", @"\""")}""]
[mesend]
[enstart]"));
					}
			}
			base.OnUpdate(ks);
		}

		public override void OnDraw(PointF p, Status ks)
		{
			base.OnDraw(p, ks);
			if (_canExecuteScript)
				DrawGraphF(p.X + Size.Width / 2f - 4, p.Y - 8, Particle[7], TRUE);
		}

		/// <summary>
		///     Entity 生成時にメタデータが渡されると、このメソッドが呼ばれます。
		/// </summary>
		/// <param name="jsonobj"></param>
		/// <returns></returns>
		public override Entity SetEntityData(dynamic jsonobj)
		{
			//TODO: メタデータに埋め込まれたスクリプトを取得する
			if (jsonobj.Script())
				_myScript = jsonobj.Script;
			base.SetEntityData((object) jsonobj);
			return this;
		}
	}

	[EntityRegistry(nameof(EntitySprite), 90)]
	public class EntitySprite : EntityLiving
	{
		private int hash;

		public EntitySprite(PointF pnt, Object[] obj, byte[,,] chips, EntityList par)
		{
			Location = pnt;
			Mpts = obj;
			Map = chips;
			Parent = par;
		}


		public override int[] ImageHandle => GraphicList[hash];


		public override EntityGroup MyGroup => EntityGroup.System;

		public override RectangleF Collision => new RectangleF(default(PointF), Size);

		/// <summary>
		///     死んでいるアニメーションを設定します。
		/// </summary>
		public override void SetKilledAnime()
		{
		}

		/// <summary>
		///     踏みつけられたアニメーションを設定します。
		/// </summary>
		public override void SetCrushedAnime()
		{
		}

		/// <summary>
		///     EntitySprite は死にません。
		/// </summary>
		public override void Kill()
		{
		}

		/// <summary>
		///     EntitySprite は死にません。
		/// </summary>
		public override void Kill(bool isfall, bool iscrushed)
		{
		}


		/// <summary>
		///     Entity 生成時にメタデータが渡されると、このメソッドが呼ばれます。
		/// </summary>
		/// <param name="jsonobj"></param>
		/// <returns></returns>
		public override Entity SetEntityData(dynamic jsonobj)
		{
			string textureFile = jsonobj.TextureFile;
			var width = (int) jsonobj.Width;
			var height = (int) jsonobj.Height;
			var startIndex = (int) jsonobj.StartIndex;
			var endIndex = (int) jsonobj.EndIndex;
			var speed = (int) jsonobj.Speed;
			hash = LoadTexture(textureFile, width, height);

			if (jsonobj.UseAnime)
				SetAnime(startIndex, endIndex, speed);
			else
				SetGraphic(startIndex);

			Size = new Size(width, height);

			base.SetEntityData((object) jsonobj);
			return this;
		}
	}
}