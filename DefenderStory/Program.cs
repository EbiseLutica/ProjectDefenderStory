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
			DX.SetGraphMode(320, 240, 32);
			DX.SetDrawScreen(DX.DX_SCREEN_BACK);
			DX.SetUseGraphAlphaChannel(1);
			DX.ChangeWindowMode(1);
			DX.SetWaitVSyncFlag(0);
			DX.SetFontSize(14);
			DX.SetFontThickness(1);
			DX.SetWindowText("Defender Story");
			DX.SetAlwaysRunFlag(1);
			
			if (DX.DxLib_Init() == -1)
			{
				DX.DxLib_End();
				Console.WriteLine("[DEBUG]DirectX の初期化に失敗しました。");
				return;
			}
			//int f = 0;
			//int fps = 1;

			//int nextFps = 60;
			int bsec = DateTime.Now.Second;
			int bmsec = DateTime.Now.Millisecond;



			//int hndl_bigplayer_walkleft = DX.CreateGraphFromRectSoftImage(hndl_playerchip, 0, 0, 16, 32);

			int[] hndl_bigplayer_datas = new int[11];

			if (DX.LoadDivGraph("Resources\\Graphics\\player_chip.png", 11, 11, 1, 16, 32, out hndl_bigplayer_datas[0]) == -1)
			{
				throw new Exception("プレイヤーキャラの読み込みに失敗しました。");
			}


			int[] hndl_bigplayer_walkleft = {
												hndl_bigplayer_datas[0],
												hndl_bigplayer_datas[1],
												hndl_bigplayer_datas[2],
												hndl_bigplayer_datas[3]
											};

			int[] hndl_bigplayer_jumpleft = {
												hndl_bigplayer_datas[4]
											};

			int[] hndl_bigplayer_dead = {
											hndl_bigplayer_datas[5]
										};

			int[] hndl_bigplayer_walkright = {
												hndl_bigplayer_datas[6],
												hndl_bigplayer_datas[7],
												hndl_bigplayer_datas[8],
												hndl_bigplayer_datas[9]
											 };

			int[] hndl_bigplayer_jumpright = {
												 hndl_bigplayer_datas[10]
											 };

			float spdAddition = 0.2f, spddivition = 0.8f, spdx = 0, spdy = 0, spdlimit = 2, nowX = 152, nowY = 114, bspdx = 0;



			Sprite me = new Sprite(new Point((int)nowX, (int)nowY), new Size(16, 32), hndl_bigplayer_walkright, true, 8, 0);

			int jimeny = 180;
			bool jumping = false;
			bool jumped = false;
			int jumpcnt = 0;
			float jumpy = 0;
			int f = 0, fps = 0;

			while (true)
			{
				DX.ClearDrawScreen();	//消し去る

				DX.DrawBox(0, 0, 320, 240, Color.SkyBlue.ToArgb(), 1);

				DX.DrawBox(0, jimeny, 320, 240, Color.SaddleBrown.ToArgb(), 1);

				int inup = DX.CheckHitKey(DX.KEY_INPUT_UP);
				int indown = DX.CheckHitKey(DX.KEY_INPUT_DOWN);
				int inleft = DX.CheckHitKey(DX.KEY_INPUT_LEFT);
				int inright = DX.CheckHitKey(DX.KEY_INPUT_RIGHT);
				int inlshift = DX.CheckHitKey(DX.KEY_INPUT_LSHIFT);
				int inz = DX.CheckHitKey(DX.KEY_INPUT_Z);

				/*
				if (inup == 1)
				{
					spdy -= spdAddition;
					if (spdy < -spdlimit)
						spdy = -spdlimit;

				}
				else if (indown == 1)
				{
					spdy += spdAddition;
					if (spdy > spdlimit)
						spdy = spdlimit;
				}
				else
				{
					spdy *= spddivition;
					if (spdy < 0.1f && spdy > -0.1f)
						spdy = 0;
				}
				*/
				if (inleft == 1)
				{
					spdx -= spdAddition;
					if (spdx < -spdlimit)
						spdx = -spdlimit;
				}
				else if (inright == 1)
				{
					spdx += spdAddition;
					if (spdx > spdlimit)
						spdx = spdlimit;
				}
				else
				{
					spdx *= spddivition;
					if (spdx < 0.1f && spdx > -0.1f)
						spdx = 0;
				}

				if (inz == 1 && !jumped)
				{
					jumping = true;
					jumped = true;
				}

				if (inlshift == 1)
				{
					spdAddition = 0.4f;
					spddivition = 0.95f;
					spdlimit = 4;
				}
				else
				{
					spdAddition = 0.2f;
					spddivition = 0.8f;
					spdlimit = 2;
				}

				if (spdx < 0)
				{
					if (jumping)
					{
						me.ImageHandle = hndl_bigplayer_jumpleft;
						me.nowImageNumber = 0;
						me.UseAnime = false;
					}
					else
					{
						me.ImageHandle = hndl_bigplayer_walkleft;
						me.UseAnime = true;
					}
				}

				if (spdx > 0)
				{
					if (jumping)
					{
						me.ImageHandle = hndl_bigplayer_jumpright;
						me.nowImageNumber = 0;
						me.UseAnime = false;
					}
					else
					{
						me.ImageHandle = hndl_bigplayer_walkright;
						me.UseAnime = true;
					}
				}
				if (spdx == 0 && bspdx != 0)
				{
					me.UseAnime = false;
					me.nowImageNumber = 0;
				}

				

				if (!jumping)
				{
					spdy += 0.2f;
					if (spdy > spdlimit)
						spdy = spdlimit;
					jumpy = nowY;
				}
				else
				{

					nowY = jumpy - (float)Math.Sin(jumpcnt / 180.0 * Math.PI) * 64;
					jumpcnt += 4;
					if (jumpcnt > 180)
					{
						jumpcnt = 0;

						jumping = false;
					}
				}

				nowX += spdx; nowY += spdy;

				if (nowX < -16)
					nowX = 320;

				if (nowX > 320)
					nowX = -16;



				if (nowY + me.Size.Height > jimeny)
				{
					nowY = jimeny - me.Size.Height;
					spdy = 0;
					jumped = false;
				}

				

				


				
				me.Point = new Point((int)nowX, (int)nowY);



				me.Draw();	//自機を描画

				f++;
				if (bsec != DateTime.Now.Second)
				{
					fps = f;
					f = 0;
					bsec = DateTime.Now.Second;
				}

				DX.DrawString(0, 0, string.Format("X: {0} Y: {1} jmp: {2} fps: {3}", nowX, nowY, jumping, fps), DX.GetColor(255, 255, 255));

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

				bspdx = spdx;

			}



		}

	}

	//スプライトのスペル間違えてたので修正

	public class Sprite
	{
		public Point Point { get; set; }
		public Size Size { get; set; }
		public int[] ImageHandle { get; set; }
		public bool UseAnime { get; set; }
		public int LoopTimes { get; set; }
		public int AnimeSpeed { get; set; }

		public int nowImageNumber = 0;

		public int looptimes = 0;

		int bms = DX.GetNowCount();

		public Sprite(Point p, Size s, int[] handle, bool useanime, int speed, int loop)
		{
			Point = p;
			Size = s;
			ImageHandle = handle;
			UseAnime = useanime;
			AnimeSpeed = speed;
			LoopTimes = loop;

		}

		public void Draw()
		{
			if (UseAnime && DX.GetNowCount() - bms >= AnimeSpeed * 15)
			{
				nowImageNumber++;
				if (nowImageNumber >= ImageHandle.Length)
				{

					looptimes++;
					if (looptimes >= LoopTimes && LoopTimes != 0)
						UseAnime = false;
					else
						nowImageNumber = 0;
				}
				bms = DX.GetNowCount();
			}
			DX.DrawGraph(Point.X, Point.Y, ImageHandle[nowImageNumber], 1);

		}

	}

}