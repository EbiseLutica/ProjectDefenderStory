using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;
using System.Drawing;

namespace DefenderStory
{
	class Program
	{
		static void Main(string[] args)
		{
			DX.SetGraphMode(320, 256, 32);
			DX.SetDrawScreen(DX.DX_SCREEN_BACK);
			DX.ChangeWindowMode(1);
			DX.SetWaitVSyncFlag(0);
			DX.SetFontSize(14);
			DX.SetFontThickness(1);
			DX.SetWindowText("MSSoundEditor");
			DX.SetAlwaysRunFlag(1);
			if (DX.DxLib_Init() == -1)
			{
				DX.DxLib_End();
				Console.WriteLine("[DEBUG]DirectX の初期化に失敗しました。");
				return;
			}
			int f = 0;
			int fps = 1;

			//int nextFps = 60;
			int bsec = DateTime.Now.Second;
			int bmsec = DateTime.Now.Millisecond;

			int playerchip_handle = DX.LoadSoftImage("Resources\\Graphics\\player_chip.bmp");
			
			
			
			DX.CreateGraphFromRectSoftImage(playerchip_handle, 0, 0, 16, 32);


			

			while (true)
			{
				DX.ClearDrawScreen();



				if (DX.ScreenFlip() == -1)
				{
					//DX.StopSoundMem(nowHandle);
					//DX.DeleteSoundMem(nowHandle);
					DX.DxLib_End();
					return;
				}





				//Console.Write(output + "\t");

				for (int i = 0; i < 16; i++)
				{
					bmsec = DateTime.Now.Millisecond;
					if (DX.WaitTimer(1) == -1)
					{
						DX.DxLib_End();
						return;
					}
				}
			}



		}

		public class Splite
		{
			Point Point { get; set; }
			Size Size { get; set; }
			int[] ImageHandle { get; set; }
			bool UseAnime { get; set; }
			int AnimeSpeed { get; set; }
			int nowImageNumber = 0;

			int bms = DateTime.Now.Millisecond;

			public Splite(Point p, Size s, int[] handle, bool useanime, int speed)
			{
				Point = p;
				Size = s;
				ImageHandle = handle;
				UseAnime = useanime;
				AnimeSpeed = speed;

			}

			public void Draw()
			{
				if (DateTime.Now.Millisecond - bms >= AnimeSpeed * 15)
				{
					AnimeSpeed = (AnimeSpeed + 1) % ImageHandle.Length;
					bms = DateTime.Now.Millisecond;
				}
				DX.DrawGraph(Point.X, Point.Y, ImageHandle[nowImageNumber], 0);
				
			}

		}

	}
}
