using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TakeUpJewel.Util;
using static DxLibDLL.DX;

namespace TakeUpJewel
{
	internal static class Program
	{
		private static Size _scrSize;

		[STAThread]
		private static void Main(string[] args)
		{
			bool inf11 = false, binf11 = false, isfullscreen = false;
			SetUseGraphAlphaChannel(1);
			ChangeWindowMode(1);
			_scrSize = new Size(640, 480);

			SetWindowText("Take Up Jewel");

			SetAlwaysRunFlag(1);

			SetWindowVisibleFlag(0);
			if (DxLib_Init() == -1)
			{
				ShowError("DirectX の初期化に失敗しました。");
				return;
			}

			if (SetGraphMode(_scrSize.Width, _scrSize.Height, 32, 60) == -1)
			{
				ShowError("サイズの変更に失敗しました。");
				return;
			}

			SetWindowVisibleFlag(1);

			if (SetDrawScreen(DX_SCREEN_BACK) == -1)
			{
				ShowError("裏画面の指定に失敗しました。");
				return;
			}

			var logo = LoadGraph("Resources\\Graphics\\citringo.png");
			ClearDrawScreen();
			DrawGraph(_scrSize.Width / 2 - 246 / 2, _scrSize.Height / 2 - 64 / 2, logo, 1);
			ScreenFlip();

			//----モジュールの初期化
			FontUtility.Init();
			SoundUtility.Init();
			ResourceUtility.Init();

			GameEngine.Init(320, 240);
			//----
			while (true)
			{
				inf11 = CheckHitKey(KEY_INPUT_F11) == TRUE;


				ProcessMessage();
				if (inf11 && !binf11)
				{
					if (isfullscreen)
					{
						isfullscreen = false;
						_scrSize = new Size(640, 480);
						SetGraphMode(_scrSize.Width, _scrSize.Height, 32, 60);
						ChangeWindowMode(1);
						GameEngine.Reload();
					}
					else
					{
						isfullscreen = true;
						_scrSize = Screen.PrimaryScreen.Bounds.Size;
						ChangeWindowMode(0);
						SetGraphMode(_scrSize.Width, _scrSize.Height, 32, 60);
						GameEngine.Reload();
					}
					ScreenFlip();
				}
				int hScreen;
				CopyToDxScreen(hScreen = GameEngine.DoGameLoop());

				if (CheckHitKey(KEY_INPUT_F2) == TRUE)
				{
					if (!Directory.Exists("ScreenShot"))
						Directory.CreateDirectory("ScreenShot");
					SaveDrawScreenToPNG(0, 0, _scrSize.Width, _scrSize.Height, $@"ScreenShot\{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.png");
				}
				if (ScreenFlip() == -1)
				{
					//StopSoundMem(nowHandle);
					//DeleteSoundMem(nowHandle);
					DxLib_End();
					return;
				}

				//Console.Write(output + "\t");
				binf11 = inf11;
			}
		}


		/// <summary>
		///     エラーを表示します。
		/// </summary>
		/// <param name="message">エラーメッセージ。</param>
		/// <returns>ダイアログの結果。</returns>
		public static DialogResult ShowError(string message)
		{
			DxLib_End();
			return MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1);
		}


		/// <summary>
		///     画面バッファを拡大して DxLib のスクリーンにコピーします。
		/// </summary>
		internal static void CopyToDxScreen(int handle)
		{
			SetDrawScreen(DX_SCREEN_BACK);
			ClearDrawScreen();
			if ((float) _scrSize.Width / _scrSize.Height != 320f / 240)
			{
				var a = _scrSize.Height / 240;
				var width = 320 * a;
				DrawExtendGraph((_scrSize.Width - width) / 2, (_scrSize.Height - 240 * a) / 2,
					_scrSize.Width - (_scrSize.Width - width) / 2, _scrSize.Height - (_scrSize.Height - 240 * a) / 2, handle, 0);
			}
			else
			{
				DrawExtendGraph(0, 0, _scrSize.Width, _scrSize.Height, handle, 0);
			}
//			DX.SetDrawScreen(handle);
		}
	}
}