using System.Collections;
using System.Collections.Generic;

namespace TakeUpJewel.Entities
{
	/// <summary>
	///     Entity のコレクションを表します。
	/// </summary>
	public abstract class EntityCollection : ICollection<Entity>
	{
		protected EntityCollection()
		{
			Items = new List<Entity>();
		}

		public List<Entity> Items { get; protected set; }

		public Entity this[int i]
		{
			get { return Items[i]; }
			set { Items[i] = value; }
		}

		public virtual void Add(Entity item)
		{
			if (Contains(item) == false)
				Items.Add(item);
		}


		public bool IsReadOnly => false;

		public void Clear()
		{
			Items.Clear();
		}

		public int Count => Items.Count;

		public bool Contains(Entity item)
		{
			return Items.Contains(item);
		}

		public void CopyTo(Entity[] array, int arrayIndex)
		{
			Items.CopyTo(array, arrayIndex);
		}

		public bool Remove(Entity item)
		{
			return Items.Remove(item);
		}

		public IEnumerator<Entity> GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Items.GetEnumerator();
		}
	}
}