using DxLibDLL;
using System.Drawing;
using System.Windows.Forms;
using DefenderStory.Util;
using System;

namespace DefenderStory
{
	static class Program
	{
		static Size scrSize;
		[STAThread]
        static void Main(string[] args)
		{
			bool inf11 = false, binf11 = false, isfullscreen = true;
			DX.SetUseGraphAlphaChannel(1);
			DX.ChangeWindowMode(0);

			scrSize = new Size(1366, 768);
			
			DX.SetWindowText("Defender Story");
			
			DX.SetAlwaysRunFlag(1);

			DX.SetWindowVisibleFlag(0);
			if (DX.DxLib_Init() == -1)
			{
				ShowError("DirectX の初期化に失敗しました。");
				return;
			}

			if (DX.SetGraphMode(scrSize.Width, scrSize.Height, 32, 60) == -1)
			{
				ShowError("サイズの変更に失敗しました。");
				return;
			}

			DX.SetWindowVisibleFlag(1);

			if (DX.SetDrawScreen(DX.DX_SCREEN_BACK) == -1)
			{
				ShowError("裏画面の指定に失敗しました。");
				return;
			}

			int logo = DX.LoadGraph("Resources\\Graphics\\citringo.png");

			DX.DrawGraph(scrSize.Width / 2 - 246 / 2 , scrSize.Height / 2 - 64 / 2, logo, 1);
			DX.ScreenFlip();

			//----モジュールの初期化
			FontUtility.Init();
			SoundUtility.Init();
			ResourceUtility.Init();
			
			GameEngine.Init(320, 240);
			//----
			while (true)
			{
				inf11 = DX.CheckHitKey(DX.KEY_INPUT_F11) == 1;
				DX.ProcessMessage();
				if (inf11 && !binf11)
				{
					if (isfullscreen)
					{
						isfullscreen = false;
						scrSize = new Size(640, 480);
						DX.SetGraphMode(scrSize.Width, scrSize.Height, 32, 60);
						DX.ChangeWindowMode(1);
						GameEngine.Reload();
					}
					else
					{
						isfullscreen = true;
						scrSize = Screen.PrimaryScreen.Bounds.Size;
						DX.ChangeWindowMode(0);
						DX.SetGraphMode(scrSize.Width, scrSize.Height, 32, 60);
						GameEngine.Reload();
					}
					DX.ScreenFlip();
				}
				CopyToDXScreen(GameEngine.DoGameLoop());
				if (DX.ScreenFlip() == -1)
				{
					//StopSoundMem(nowHandle);
					//DeleteSoundMem(nowHandle);
					DX.DxLib_End();
					return;
				}

				//Console.Write(output + "\t");
				binf11 = inf11;
				
			}



		}

		
		/// <summary>
		/// エラーを表示します。
		/// </summary>
		/// <param name="message">エラーメッセージ。</param>
		/// <returns>ダイアログの結果。</returns>
		public static DialogResult ShowError(string message)
		{
			DX.DxLib_End();
			return MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1);
		}



		/// <summary>
		/// 画面バッファを拡大して DxLib のスクリーンにコピーします。
		/// </summary>
		internal static void CopyToDXScreen(int handle)
		{
			DX.SetDrawScreen(DX.DX_SCREEN_BACK);
			DX.ClearDrawScreen();
			if ((float)scrSize.Width / scrSize.Height != 320f / 240)
			{
				int a = scrSize.Height / 240;
				int width = (int)(320 * a);
				DX.DrawExtendGraph((scrSize.Width - width) / 2, (scrSize.Height - 240 * a) / 2, scrSize.Width - (scrSize.Width - width) / 2, scrSize.Height - (scrSize.Height - 240 * a) / 2, handle, 0);
			}
			else
			{
				DX.DrawExtendGraph(0, 0, scrSize.Width, scrSize.Height, handle, 0);
			}
//			DX.SetDrawScreen(handle);
		}
	}




}