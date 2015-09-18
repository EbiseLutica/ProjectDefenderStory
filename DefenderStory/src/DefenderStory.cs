using DxLibDLL;
using System.Drawing;
using System.Windows.Forms;
using DefenderStory.Util;
using System;

namespace DefenderStory
{
	static class Program
	{
		/// <summary>
		/// メイン画面バッファのハンドル。
		/// </summary>
		public static int hMainScreen;
		[STAThread]
		static void Main(string[] args)
		{

			DX.SetUseGraphAlphaChannel(1);
			DX.ChangeWindowMode(1);

			DX.SetWindowText("Defender Story");
			
			//SetAlwaysRunFlag(1);

			Size scrSize = new Size(640, 480);

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

			DX.DrawGraph(640 / 2 - 246 / 2 , 480 / 2 - 64 / 2, logo, 1);
			DX.ScreenFlip();
			#region スプラッシュ

			/*
			int logo = LoadGraph("Resources\\Graphics\\citringo.png");
			
			SoundUtility.PlaySound(Sounds.Citringo);
			ClearDrawScreen();
			for (int i = 0; i < 120; i++)
			{
				ClearDrawScreen();
				DrawGraph(37, 88, logo, 1);
				DX.ProcessMessage();
				if (DX.ScreenFlip() == -1)
				{
					DxLib_End();
					return;
				}
			}
			

			DeleteGraph(logo);
			*/
			#endregion


			//----モジュールの初期化
			FontUtility.Init();
			SoundUtility.Init();
			ResourceUtility.Init();
			hMainScreen = DX.MakeScreen(320, 240);
			GameEngine.Init();
			//----
			while (true)
			{

				DX.ProcessMessage();
				GameEngine.DoGameLoop();
				CopyToDXScreen();
				if (DX.ScreenFlip() == -1)
				{
					//StopSoundMem(nowHandle);
					//DeleteSoundMem(nowHandle);
					DX.DxLib_End();
					return;
				}
				
				//Console.Write(output + "\t");

				
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
		internal static void CopyToDXScreen()
		{
			DX.SetDrawScreen(DX.DX_SCREEN_BACK);
			DX.DrawExtendGraph(0, 0, 640, 480, hMainScreen, 0);
			DX.SetDrawScreen(hMainScreen);
		}
	}




}