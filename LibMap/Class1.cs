using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefenderStory.Map
{
	public static class MapUtility
	{
		public static void SaveMap(byte[, ,] array, string path)
		{
			int w = array.GetLength(0);
			int h = array.GetLength(1);

			System.IO.BinaryWriter bw = new System.IO.BinaryWriter(new System.IO.FileStream(path, System.IO.FileMode.Create));
			bw.Write("CITCHIP".ToArray());
			bw.Write(w);
			bw.Write(h);

			for (int z = 0; z < 2; z++)
				for (int y = 0; y < h; y++)
					for (int x = 0; x < w; x++)
						bw.Write(array[x, y, z]);

			bw.Close();

		}

		public static void LoadMap(out byte[, ,] array, string path)
		{
			System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.FileStream(path, System.IO.FileMode.Open));

			if (new string(br.ReadChars(7)) != "CITCHIP")
			{
				br.Close();
				throw new Exception("指定したファイルは、有効な Defender Story マップファイルではありません。");
			}
			int w = br.ReadInt32();
			int h = br.ReadInt32();

			array = new byte[w, h, 2];

			for (int z = 0; z < 2; z++)
				for (int y = 0; y < h; y++)
					for (int x = 0; x < w; x++)
						array[x, y, z] = br.ReadByte();
			br.Close();

		}
	}

}
