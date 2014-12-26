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
			int wavegraph = 0;
			Form1 f1 = new Form1();
			f1.Show();
			DX.SetUserChildWindow(f1.Handle);
			int hnowmem = 0;
			while (true)
			{
				DX.ProcessMessage();						//しないと固まる
				DX.ClearDrawScreen();						//クリアしておかないとScreenFlipでどうなるかわからない
				if (f1.wavesetrequest)
				{
					if (wavegraph != 0)
						DX.DeleteGraph(wavegraph);
					if (hnowmem != 0)
						DX.DeleteSoundMem(hnowmem);
					int hSSnd = DX.MakeSoftSound1Ch16Bit44KHz(f1.wavedata.Length);
					int hGraph = DX.MakeSoftImage(480, 360);

					for (int i = 0; i < f1.wavedata.Length; i++)
					{
						DX.WriteSoftSoundData(hSSnd, i, f1.wavedata[i], f1.wavedata[i]);
						DX.DrawLineSoftImage(hGraph, i/2, 180, i/2, (int)(f1.wavedata[i] / (65535 / 180.0) + 90), 255, 255, 255, 255);
					}
					wavegraph = DX.CreateGraphFromSoftImage(hGraph);
					hnowmem = DX.LoadSoundMemFromSoftSound(hSSnd);
					DX.DeleteSoftSound(hSSnd);
					DX.DeleteSoftImage(hGraph);
					f1.wavesetrequest = false;
					DX.PlaySoundMem(hnowmem, DX.DX_PLAYTYPE_NORMAL);
				}

				//波形描画

				if (wavegraph != 0)
					DX.DrawGraph(0, 0, wavegraph, 0);

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
	public enum EnvelopeFlag
	{
		None, Attack, Decay, Sustain, Release
	}
}
