namespace DefenderStory.Data
{
	/// <summary>
	/// mpt の最小単位を表します。
	/// </summary>
	public class Object
	{
		public int ImageHandle { get; set; }
		/// <summary>
		/// オブジェクトの当たり判定をビットマップで指定します。
		/// ～記法～
		/// 0...当たらない
		/// 1...当たる
		/// 2...当たるとダメージ
		/// 3...当たると即死
		/// 4...当たると水中
		/// </summary>
		public byte[,] HitMask { get; set; }

		public ObjectHitFlag CheckHit(int x, int y)
		{
			return (ObjectHitFlag)HitMask[x, y];
		}

		public Object(int handle, byte[,] mask)
		{
			ImageHandle = handle;
			HitMask = mask;
		}


	}


	/// <summary>
	/// オブジェクトの当たり判定フラグを指定します。
	/// </summary>
	public enum ObjectHitFlag
	{
		/// <summary>
		/// 何もなし。
		/// </summary>
		NotHit,
		/// <summary>
		/// 物体。
		/// </summary>
		Hit,
		/// <summary>
		/// ダメージを与えるもの。
		/// </summary>
		Damage,
		/// <summary>
		/// 即殺するもの。
		/// </summary>
		Death,
		/// <summary>
		/// 水中。
		/// </summary>
		InWater
	}
}
