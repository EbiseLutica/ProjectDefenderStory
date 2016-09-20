using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeUpJewel.Map
{
	public static class MapUtility
	{
		public static void SaveMap(byte[, ,] array, string path)
		{
			var w = array.GetLength(0);
			var h = array.GetLength(1);

			var bw = new System.IO.BinaryWriter(new System.IO.FileStream(path, System.IO.FileMode.Create));
			bw.Write("CITCHIP".ToArray());
			bw.Write(w);
			bw.Write(h);

			for (var z = 0; z < 2; z++)
				for (var y = 0; y < h; y++)
					for (var x = 0; x < w; x++)
						bw.Write(array[x, y, z]);

			bw.Close();

		}

		public static void LoadMap(out byte[, ,] array, string path)
		{
			var br = new System.IO.BinaryReader(new System.IO.FileStream(path, System.IO.FileMode.Open));

			if (new string(br.ReadChars(7)) != "CITCHIP")
			{
				br.Close();
				throw new Exception("指定したファイルは、有効な Defender Story マップファイルではありません。");
			}
			var w = br.ReadInt32();
			var h = br.ReadInt32();

			array = new byte[w, h, 2];

			for (var z = 0; z < 2; z++)
				for (var y = 0; y < h; y++)
					for (var x = 0; x < w; x++)
						array[x, y, z] = br.ReadByte();
			br.Close();

		}
	}

}
