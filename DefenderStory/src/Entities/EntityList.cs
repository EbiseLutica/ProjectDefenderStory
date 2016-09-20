
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DxLibDLL;

namespace TakeUpJewel.Entities
{
	/// <summary>
	/// エンティティのコレクションを表します。
	/// </summary>
	public class EntityList : EntityCollection
	{
		/// <summary>
		/// タグを使って Entity を探します。
		/// </summary>
		/// <param name="tag">タグ文字列。</param>
		/// <returns>Entity が存在すればそれらのコレクション、なければ空のコレクションが返ります。</returns>
		public IEnumerable<Entity> FindEntitiesByTag(string tag)
		{
			return this.Where(sp => tag == sp.Tag);
		}

		/// <summary>
		/// 型を指定して Entity を探します。
		/// </summary>
		/// <typeparam name="T">探す対象のデータ型。</typeparam>
		/// <returns></returns>
		public IEnumerable<Entity> FindEntitiesByType<T>()
		{
			return this.Where(sp => sp is T);
		}

		public override void Add(Entity item)
		{
			Add(item, false);
		}

		public void Add(Entity item, bool isMain)
		{
			Items.Add(item);
			var living = item as EntityLiving;
			if (living != null)
			{
				living.MainAi?.OnInit();
				foreach (var ai in living.CollisionAIs)
				{
					ai.OnInit();
				}
			}
			if (isMain)
				MainEntity = item;
		}

		public Entity MainEntity
		{
			get; private set;
		}

		public void Draw(ref Status ks, ref byte[,,] chips)
		{
			int w, h;
			DX.GetWindowSize(out w, out h);
			//foreach だと削除・追加時にしぬので、forでやる
			for (var i = 0; i < Count; i++)
			{
				var item = this[i];
				if (item.IsDead)    //死んだら消す
				{
					Remove(item);
					//continue;
				}
				if (i >= Count)
					break;
				item = this[i];
				if (Math.Abs(MainEntity.Location.X - item.Location.X) > w / 2)
					continue;
				item.OnUpdate(ks);    //更新処理をする
				if (item is EntityLiving)
				{
					if (((EntityLiving)item).MainAi != null && ((EntityLiving)item).MainAi.Use)
						((EntityLiving)item).MainAi.OnUpdate();
					foreach (var ai in ((EntityLiving)item).CollisionAIs)
					{
						if (ai.Use)
							ai.OnUpdate();
					}	
				}
				if (item is EntityVisible)  //描画可能な Entity は描画する
				{
					((EntityVisible)item).OnDraw(new PointF(ks.Camera.X + item.Location.X, ks.Camera.Y + item.Location.Y), ks);
				}
			}
		}

		public void DebugDraw(ref Status ks, ref byte[,,] chips)
		{
			foreach (EntityVisible item in FindEntitiesByType<EntityVisible>())
			{
				item.OnDebugDraw(new PointF(ks.Camera.X + item.Location.X, ks.Camera.Y + item.Location.Y), ks);
			}

		}

	}
}