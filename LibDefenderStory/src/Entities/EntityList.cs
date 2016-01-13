
using DefenderStory.AI;
using DxLibDLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DefenderStory.Entities
{
	/// <summary>
	/// エンティティのコレクションを表します。
	/// </summary>
	public class EntityList : EntityCollection
	{

		/// <summary>
		/// EntityList の新しいインスタンスを初期化します。
		/// </summary>
		public EntityList()
		{
			Items = new List<Entity>();

		}

		/// <summary>
		/// タグを使って Entity を探します。
		/// </summary>
		/// <param name="tag">タグ文字列。</param>
		/// <returns>Entity が存在すればそれらのコレクション、なければ空のコレクションが返ります。</returns>
		public IEnumerable<Entity> FindEntitiesByTag(string tag)
		{
			return this.Where(new Func<Entity, bool>(sp => tag == sp.Tag));
		}

		/// <summary>
		/// 型を指定して Entity を探します。
		/// </summary>
		/// <typeparam name="T">探す対象のデータ型。</typeparam>
		/// <returns></returns>
		public IEnumerable<Entity> FindEntitiesByType<T>()
		{
			return this.Where(new Func<Entity, bool>(sp => sp is T));
		}

		public override void Add(Entity item)
		{
			this.Add(item, false);
		}

		public void Add(Entity item, bool isMain)
		{
			Items.Add(item);
			if (item is EntityLiving)
			{
				if (((EntityLiving)item).MainAI != null)
					((EntityLiving)item).MainAI.onInit();
				foreach (AIBase ai in ((EntityLiving)item).CollisionAIs)
				{
					ai.onInit();
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
			for (int i = 0; i < Count; i++)
			{
				Entity item = this[i];
				if (item.IsDead)    //死んだら消す
				{
					this.Remove(item);
					//continue;
				}
				if (i >= Count)
					break;
				item = this[i];
				if (Math.Abs(MainEntity.Location.X - item.Location.X) > w / 2)
					continue;
				item.onUpdate(ks);    //更新処理をする
				if (item is EntityLiving)
				{
					if (((EntityLiving)item).MainAI != null && ((EntityLiving)item).MainAI.Use)
						((EntityLiving)item).MainAI.onUpdate();
					foreach (AIBase ai in ((EntityLiving)item).CollisionAIs)
					{
						if (ai.Use)
							ai.onUpdate();
					}	
				}
				if (item is EntityVisible)  //描画可能な Entity は描画する
				{
					((EntityVisible)item).onDraw(new PointF(ks.camera.X + item.Location.X, ks.camera.Y + item.Location.Y), ks);
				}
			}
		}

		public void DebugDraw(ref Status ks, ref byte[,,] chips)
		{
			foreach (EntityVisible item in FindEntitiesByType<EntityVisible>())
			{
				((EntityVisible)item).onDebugDraw(new PointF(ks.camera.X + item.Location.X, ks.camera.Y + item.Location.Y), ks);
			}

		}

	}
}