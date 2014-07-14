using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DxLibDLL;

namespace MusicSheetSEEditor
{
	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
			DX.ChangeWindowMode(1);
			DX.SetFontSize(14);
			DX.SetFontThickness(1);
			DX.SetWindowText("MSPercEditor");
			DX.SetAlwaysRunFlag(1);
			
			if (DX.DxLib_Init() == -1)
			{
				ShowError("DirectX の初期化に失敗しました。");
				return;
			}

			if (DX.SetDrawScreen(DX.DX_SCREEN_BACK) == -1)
			{
				ShowError("設定に失敗しました。");
				return;
			}
			DX.SetGraphMode(480, 360, 32,60);
			Form1 f1 = new Form1();
			f1.Show();
			DX.SetUserChildWindow(f1.Handle);
			while (true)
			{
				DX.ProcessMessage();						//しないと固まる
				DX.ClearDrawScreen();						//クリアしておかないとScreenFlipでどうなるかわからない
				if (DX.ScreenFlip() == -1)					//ウィンドウを消すとtrueになるようなので終了処理をする
				{
					DX.DxLib_End();							
					//ShowError("描画更新に失敗しました。");
					return;
				}
			}

		}
		/// <summary>
		///	DXLibを終了してエラーメッセージを表示します。
		/// </summary>
		/// <param name="text"></param>
		public static void ShowError(string text)
		{
			DX.DxLib_End();
			MessageBox.Show(string.Format(@"実行中にエラーが発生しました。
エラー内容: {0}", text), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
		}
	}
}
