namespace DefenderStory.Data
{
	/// <summary>
	/// mpt の最小単位を表します。
	/// </summary>
	public class Object
	{
		public int ImageHandle { get; set; }
		/// <summary>
		/// mpt の当たり判定をビットマップで指定します。
		/// ～記法～
		/// 0...当たらない
		/// 1...当たる
		/// 2...当たるとダメージ
		/// 3...当たると即死
		/// 4...当たると水中
		/// </summary>
		public byte[,] HitMask { get; set; }

		/// <summary>
		/// 座標を指定して、この mpt の当たり判定を取得します。
		/// </summary>
		/// <param name="x">mpt 内の X 座標。</param>
		/// <param name="y">mpt 内の Y 座標。</param>
		/// <returns></returns>
		public ObjectHitFlag CheckHit(int x, int y)
		{
			return (ObjectHitFlag)HitMask[x, y];
		}

		/// <summary>
		/// テクスチャのハンドルと当たり判定座標を指定して、 Object の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="mask"></param>
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
