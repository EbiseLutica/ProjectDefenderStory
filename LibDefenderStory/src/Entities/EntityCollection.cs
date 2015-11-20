
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DefenderStory.Entities
{
	/// <summary>
	/// Entity のコレクションを表します。
	/// </summary>
	public abstract class EntityCollection : ICollection<Entity>
	{

		public List<Entity> Items { get; protected set; }



		public EntityCollection()
		{
			Items = new List<Entity>();

		}

		public virtual void Add(Entity item)
		{
			if (Contains(item) == false)
			{
				Items.Add(item);
			}

		}

		public Entity this[int i]
		{
			get
			{
				return Items[i];
			}
			set
			{
				Items[i] = value;
			}
		}


		public bool IsReadOnly { get { return false; } }
		public void Clear() { Items.Clear(); }
		public int Count { get { return Items.Count; } }
		public bool Contains(Entity item) { return Items.Contains(item); }
		public void CopyTo(Entity[] array, int arrayIndex) { Items.CopyTo(array, arrayIndex); }
		public bool Remove(Entity item) { return Items.Remove(item); }
		public IEnumerator<Entity> GetEnumerator()
		{
			return Items.GetEnumerator();

		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Items.GetEnumerator();
		}



	}


}