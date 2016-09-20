using System;
using System.Collections.Generic;
using System.Linq;
using DxLibDLL;
using System.Drawing;
using System.Windows.Forms;
using MusicSheet.Mssf;
using TakeUpJewel.GUI;


namespace MusicSheetSoundEditor
{
	
	public class Program
	{

		public static short[] WavSquare = {
													 -32768, -32768, -32768, -32768, -32768, -32768, -32768, -32768, 
													 -32768, -32768, -32768, -32768, -32768, -32768, -32768, -32768, 
													  32767,  32767,  32767,  32767,  32767,  32767,  32767,  32767, 
													  32767,  32767,  32767,  32767,  32767,  32767,  32767,  32767
												 };

		public static short[] WavTriangle = {
														-4096,  -8192, -12288, -16384, -20479, -24575, -28671, -32767,
													   -32767, -28671, -24575, -20479, -16384, -12288,  -8192,  -4096,
														 4096,   8192,  12288,  16384,  20479,  24575,  28671,  32767, 
														32767,  28671,  24575,  20479,  16384,  12288,   8192,   4096
													   
												   };


		public static short[] Wav12Pulse = {
													 -32768, -32768, -32768, -32768,  32767,  32767,  32767,  32767,
													  32767,  32767,  32767,  32767,  32767,  32767,  32767,  32767,
													  32767,  32767,  32767,  32767,  32767,  32767,  32767,  32767, 
													  32767,  32767,  32767,  32767,  32767,  32767,  32767,  32767
												 };


		public static short[] Wav25Pulse = {
													 -32768, -32768, -32768, -32768, -32768, -32768, -32768, -32768,
													  32767,  32767,  32767,  32767,  32767,  32767,  32767,  32767,
													  32767,  32767,  32767,  32767,  32767,  32767,  32767,  32767, 
													  32767,  32767,  32767,  32767,  32767,  32767,  32767,  32767
												 };


		[STAThread]
		static void Main(string[] args)
		{
			//波形編集モード: 1マスは 10*15


			DX.SetGraphMode(320, 256, 32);
			
			DX.ChangeWindowMode(1);
			DX.SetWaitVSyncFlag(0);
			DX.SetFontSize(14);
			DX.SetFontThickness(1);
			DX.SetWindowText("MSSF Editor");
			DX.SetAlwaysRunFlag(1);
			DX.SetWindowVisibleFlag(0);
			if (DX.DxLib_Init() == -1)
			{
				DX.DxLib_End();
				Console.WriteLine("[DEBUG]DirectX の初期化に失敗しました。");
				return;
			}
			var f = 0;
			var fps = 1;
			DX.SetDrawScreen(DX.DX_SCREEN_BACK);
			//int nextFps = 60;
			var bsec = DateTime.Now.Second;
			var bmsec = DateTime.Now.Millisecond;

			//int nowHandle = 0;
			//short[] wave = new short[32];
			var wave = WavSquare;

			var pan = 0;

			//DX.PlaySoundMem(nowHandle = SetWave(wave, GetFreq("F", 4)), DX.DX_PLAYTYPE_LOOP);

			// editMode 0...Wave 1...Play
			var editMode = 0;

			var noiseoption = NoiseOption.None;

			var buttonsForEditMode1 = new DxButton[14];
			var octave = 4;
			
			
			string[] pitches = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

			var myenv = new Envelope(0, 0, 255, 0);
			Tone nowTone = null;

			buttonsForEditMode1[0] = new DxButton(new Rectangle(2, 18, 32, 32), "←", Color.Black, Color.White);
			buttonsForEditMode1[0].ClickedAction = (mbtn, dxb) =>
			{
				if (mbtn == DX.MOUSE_INPUT_LEFT)
				{
					octave -= 1;
					if (octave < 0)
						octave = 0;
				}
			};

			buttonsForEditMode1[1] = new DxButton(new Rectangle(66, 18, 32, 32), "→", Color.Black, Color.White);
			buttonsForEditMode1[1].ClickedAction = (mbtn, dxb) =>
			{
				if (mbtn == DX.MOUSE_INPUT_LEFT)
				{
					octave += 1;
					if (octave > 8)
						octave = 8;
				}
			};

			buttonsForEditMode1[2] = new DxButton(new Rectangle(30, 98, 20, 128), "C", Color.White, Color.Black);
			buttonsForEditMode1[3] = new DxButton(new Rectangle(50, 98, 20, 96), "C#", Color.Black, Color.White);
			buttonsForEditMode1[4] = new DxButton(new Rectangle(70, 98, 20, 128), "D", Color.White, Color.Black);
			buttonsForEditMode1[5] = new DxButton(new Rectangle(90, 98, 20, 96), "D#", Color.Black, Color.White);
			buttonsForEditMode1[6] = new DxButton(new Rectangle(110, 98, 20, 128), "E", Color.White, Color.Black);
			buttonsForEditMode1[7] = new DxButton(new Rectangle(130, 98, 20, 128), "F", Color.White, Color.Black);
			buttonsForEditMode1[8] = new DxButton(new Rectangle(150, 98, 20, 96), "F#", Color.Black, Color.White);
			buttonsForEditMode1[9] = new DxButton(new Rectangle(170, 98, 20, 128), "G", Color.White, Color.Black);
			buttonsForEditMode1[10] = new DxButton(new Rectangle(190, 98, 20, 96), "G#", Color.Black, Color.White);
			buttonsForEditMode1[11] = new DxButton(new Rectangle(210, 98, 20, 128), "A", Color.White, Color.Black);
			buttonsForEditMode1[12] = new DxButton(new Rectangle(230, 98, 20, 96), "A#", Color.Black, Color.White);
			buttonsForEditMode1[13] = new DxButton(new Rectangle(250, 98, 20, 128), "B", Color.White, Color.Black);

			var buttonsForEditMode2 = new DxButton[13];

			buttonsForEditMode2[0] = new DxButton(new Rectangle(24, 16, 32, 32), "A↑", Color.Black, Color.White);
			buttonsForEditMode2[1] = new DxButton(new Rectangle(64, 16, 32, 32), "D↑", Color.Black, Color.White);
			buttonsForEditMode2[2] = new DxButton(new Rectangle(104, 16, 32, 32), "S↑", Color.Black, Color.White);
			buttonsForEditMode2[3] = new DxButton(new Rectangle(144, 16, 32, 32), "R↑", Color.Black, Color.White);

			buttonsForEditMode2[4] = new DxButton(new Rectangle(24, 208, 32, 32), "A↓", Color.Black, Color.White);
			buttonsForEditMode2[5] = new DxButton(new Rectangle(64, 208, 32, 32), "D↓", Color.Black, Color.White);
			buttonsForEditMode2[6] = new DxButton(new Rectangle(104, 208, 32, 32), "S↓", Color.Black, Color.White);
			buttonsForEditMode2[7] = new DxButton(new Rectangle(144, 208, 32, 32), "R↓", Color.Black, Color.White);

			buttonsForEditMode2[8] = new DxButton(new Rectangle(190, 162, 48, 32), "Save", Color.Black, Color.White);
			buttonsForEditMode2[9] = new DxButton(new Rectangle(190, 208, 48, 32), "Load", Color.Black, Color.White);
			buttonsForEditMode2[10] = new DxButton(new Rectangle(184, 16, 16, 16), "", Color.White, Color.Black);
			buttonsForEditMode2[11] = new DxButton(new Rectangle(184, 40, 16, 16), "", Color.White, Color.Black);
			buttonsForEditMode2[12] = new DxButton(new Rectangle(184, 64, 16, 16), "", Color.White, Color.Black);


			


			buttonsForEditMode2[0].MouseDownAction2 = (mbtn, dxb) =>
			{
				myenv.AttackTime++;
			};

			buttonsForEditMode2[1].MouseDownAction2 = (mbtn, dxb) =>
			{
				myenv.DecayTime++;
			};

			buttonsForEditMode2[2].MouseDownAction2 = (mbtn, dxb) =>
			{
				myenv.SustainLevel++;
				if (myenv.SustainLevel > 255)
					myenv.SustainLevel = 255;
			};

			buttonsForEditMode2[3].MouseDownAction2 = (mbtn, dxb) =>
			{
				myenv.ReleaseTime++;
			};

			buttonsForEditMode2[4].MouseDownAction2 = (mbtn, dxb) =>
			{
				myenv.AttackTime--;
				if (myenv.AttackTime < 0)
					myenv.AttackTime = 0;
			};

			buttonsForEditMode2[5].MouseDownAction2 = (mbtn, dxb) =>
			{
				myenv.DecayTime--;
				if (myenv.DecayTime < 0)
					myenv.DecayTime = 0;
			};

			buttonsForEditMode2[6].MouseDownAction2 = (mbtn, dxb) =>
			{
				myenv.SustainLevel--;
				if (myenv.SustainLevel < 0)
					myenv.SustainLevel = 0;
			};

			buttonsForEditMode2[7].MouseDownAction2 = (mbtn, dxb) =>
			{
				myenv.ReleaseTime--;
				if (myenv.ReleaseTime < 0)
					myenv.ReleaseTime = 0;
			};

			buttonsForEditMode2[8].ClickedAction = (mbtn, dxb) =>
			{
				var sfd = new SaveFileDialog();
				sfd.Filter = "Music Sheet サウンドファイル (*.mssf)|*.mssf|すべてのファイル (*.*)|*.*";
				if (sfd.ShowDialog() == DialogResult.Cancel)
					return;
				MssfUtility.SaveFileVer2(sfd.FileName, wave, myenv.AttackTime, myenv.DecayTime, myenv.SustainLevel, myenv.ReleaseTime, pan, noiseoption);


			};

			buttonsForEditMode2[9].ClickedAction = (mbtn, dxb) =>
			{
				var ofd = new OpenFileDialog();
				ofd.Filter = "Music Sheet サウンドファイル (*.mssf)|*.mssf|すべてのファイル (*.*)|*.*";
				if (ofd.ShowDialog() == DialogResult.Cancel)
					return;
				int a,d,r;
				byte s;

				MssfUtility.LoadFileDynamic(ofd.FileName, out wave, out a, out d, out s, out r, out pan, out noiseoption);

				myenv.AttackTime = a;
				myenv.DecayTime = d;
				myenv.SustainLevel = s;
				myenv.ReleaseTime = r;

			};

			buttonsForEditMode2[10].ClickedAction = (mbtn, dxb) =>
			{
				noiseoption = NoiseOption.None;
			};

			buttonsForEditMode2[11].ClickedAction = (mbtn, dxb) =>
			{
				noiseoption = NoiseOption.Long;
			};

			buttonsForEditMode2[12].ClickedAction = (mbtn, dxb) =>
			{
				noiseoption = NoiseOption.Short;
			};

			var down = new Action<int, DxButton>((mbtn, dbt) =>
			{
				
				//DX.PlaySoundMem(nowHandle = SetWave(wave, GetFreq(dbt.Text, octave)), DX.DX_PLAYTYPE_LOOP);
				if (nowTone != null && nowTone.Playing)
				{
					nowTone.Abort();
					nowTone = null;
				}
				nowTone = new Tone(dbt.Text, octave, wave, myenv, 255, pan, 100, noiseoption);
				nowTone.StartPlay(-1, -1);
			});

			var up = new Action<int, DxButton>((mbtn, dbt) =>
			{
				//DX.StopSoundMem(nowHandle);
				//DX.DeleteSoundMem(nowHandle);
				nowTone.Stop();
			});


			for (var i = 2; i <= 13; i++)
			{
				buttonsForEditMode1[i].MouseDownAction = down;
				buttonsForEditMode1[i].MouseUpAction = up;
				
			}





			//int sheed = 0x8000;

			var ico = new Icon("mssfedit.ico");
			DX.SetWindowIconHandle(ico.Handle);

			//bool isShortFreq = false;
			DX.SetWindowVisibleFlag(1);
			

			while (true)
			{

				DX.ProcessMessage();
				DX.ClearDrawScreen();

				var key1 = DX.CheckHitKey(DX.KEY_INPUT_1);
				var key2 = DX.CheckHitKey(DX.KEY_INPUT_2);
				var key3 = DX.CheckHitKey(DX.KEY_INPUT_3);
				var mouseIn = DX.GetMouseInput();
				int mouseX, mouseY;

				DX.GetMousePoint(out mouseX, out mouseY);

				if (key1 == 1)
					editMode = 0;

				if (key2 == 1)
					editMode = 1;

				if (key3 == 1)
					editMode = 2;

				switch (editMode)
				{
					case 0:

						var mouseGridX = mouseX / 10;
						var mouseGridY = (mouseY - 16) / 15;

						for (var x = 0; x < 32; x++)
							DX.DrawBox(x * 10, 136, x * 10 + 10, 136 + (int)(wave[x] / 4095.875 * 15), DX.GetColor(0, 172, 224), 1);
						for (var y = 0; y < 240; y += 15)
						{
							for (var x = 0; x < 320; x += 10)
							{
								DX.DrawBox(x, y + 16, x + 11, y + 32, DX.GetColor(64, 64, 64), 0);
							}
						}

						if (mouseY < 16)
							mouseGridY = 0;
						else
						{
							DX.DrawCircle(mouseGridX * 10 + 5, mouseGridY * 15 + 15, 4, DX.GetColor(255, 255, 255), 1);
							if (mouseGridX < 32 && mouseGridX >= 0 && mouseGridY < 17)
								if (mouseIn == DX.MOUSE_INPUT_LEFT)
								{
									wave[mouseGridX] = (short)((mouseGridY - 8) * 4095.875);
								}
								else if (mouseIn == DX.MOUSE_INPUT_RIGHT)
								{
									wave[mouseGridX] = 0;
								}
						}
						DX.DrawString(0, 0, string.Format("MouseX: {0} MouseY: {1} MGridX: {2} MGridY: {3}", mouseX, mouseY, mouseGridX, mouseGridY), DX.GetColor(255, 255, 255));

						break;
					case 1:
						foreach (var dxb in buttonsForEditMode1)
						{
							if (dxb != null)
								dxb.Draw();
						}
						DX.DrawString(48, 24, octave.ToString(), DX.GetColor(255, 255, 255));

						DX.DrawString(110, 24, "テスター", DX.GetColor(255, 255, 255));

						if (nowTone != null && nowTone.Playing)
						{
							DX.DrawString(174, 84, nowTone.Freq.ToString(), DX.GetColor(255, 255, 255));
							DX.DrawString(174, 8, "V OV " + nowTone.OutVolume, DX.GetColor(255, 255, 255));
							DX.DrawBox(174, 83, 178, 83 - nowTone.Volume / 4, DX.GetColor(255, 255, 255), 1);
							DX.DrawBox(188, 83, 199, 83 - nowTone.OutVolume / 4, DX.GetColor(255, 255, 255), 1);
							var hoge = "";
							if (((int)nowTone.Envflag) >= 1)
								hoge += "A";
							if (((int)nowTone.Envflag) >= 2)
								hoge += "D";
							if (((int)nowTone.Envflag) >= 3)
								hoge += "S";
							if (((int)nowTone.Envflag) >= 4)
								hoge += "R";
							DX.DrawString(210, 64, hoge, DX.GetColor(255,255,255));
							for (var i = 0; i < 32; i++)
								DX.DrawLine(i + 202, (int)(wave[i] / 4095.875 * 1.2 + 51), i + 202, 51, 0xffffff);
						}

						break;
					case 2:
						foreach (var dxb in buttonsForEditMode2)
						{
							if (dxb != null)
								dxb.Draw();
						}

						DX.DrawString(24, 105, myenv.AttackTime.ToString(), DX.GetColor(255, 255, 255));

						DX.DrawString(64, 105, myenv.DecayTime.ToString(), DX.GetColor(255, 255, 255));

						DX.DrawString(104, 105, myenv.SustainLevel.ToString(), DX.GetColor(255, 255, 255));

						DX.DrawString(144, 105, myenv.ReleaseTime.ToString(), DX.GetColor(255, 255, 255));

						
						buttonsForEditMode2[10].Text = "";
						buttonsForEditMode2[11].Text = "";
						buttonsForEditMode2[12].Text = "";

						buttonsForEditMode2[10 + (int)noiseoption].Text = "×";

						DX.DrawString(buttonsForEditMode2[10].PointAndSize.X + 24, buttonsForEditMode2[10].PointAndSize.Y, "ノイズ無し", 0xffffff);
						DX.DrawString(buttonsForEditMode2[11].PointAndSize.X + 24, buttonsForEditMode2[11].PointAndSize.Y, "長周期ノイズ", 0xffffff);
						DX.DrawString(buttonsForEditMode2[12].PointAndSize.X + 24, buttonsForEditMode2[12].PointAndSize.Y, "短周期ノイズ", 0xffffff);
						break;
				}


				//DX.DrawString(0, 0, "KeyBoard [1]:波形編集 [2]:再生等", DX.GetColor(255, 255, 255));

				f++;
				if (bsec != DateTime.Now.Second)
				{
					fps = f;
					f = 1;
					bsec = DateTime.Now.Second;
				}

				DX.DrawBox(296, 0, 320, 16, 0, 1);
				DX.DrawString(299, 0, fps.ToString(), DX.GetColor(255, 255, 255));

				if (DX.ScreenFlip() == -1)
				{
					//DX.StopSoundMem(nowHandle);
					//DX.DeleteSoundMem(nowHandle);
					DX.DxLib_End();
					return;
				}


				


				//Console.Write(output + "\t");

				for (var i = 0; i < 16; i++)
				{
					if (DateTime.Now.Millisecond - bmsec > 1)
						i += DateTime.Now.Millisecond - bmsec - 1;
					while (DateTime.Now.Millisecond - bmsec == 0) { }
					if (nowTone != null && nowTone.Playing)
					{
						nowTone.PlayLoop(-1);
						DX.ChangeVolumeSoundMem(nowTone.OutVolume, nowTone.Handle);
					}
					bmsec = DateTime.Now.Millisecond;
					if (DX.WaitTimer(1) == -1)
					{
						DX.DxLib_End();
						return;
					}
				}



			}
		}



		public static int SetNoise(int hz, bool isShortFreq, ref int sheed, int length)
		{
			var output = 0;

			
			

			var bcnt = -1;

			var data = new Int16[length];
			float t = 0;
			var hSSnd = DX.MakeSoftSound1Ch16Bit44KHz(length);

			for (var i = 0; i < length; i++)
			{
				if ((int)(Math.PI * 2 / 44100 * i * hz * 180 / Math.PI) != bcnt)
				{
					sheed >>= 1;
					sheed |= ((sheed ^ (sheed >> (isShortFreq ? 6 : 1))) & 1) << 15;
					output = sheed & 1;
				}
				t = output * 32767; // divided by 10 means volume control
				data[i] = (short)t;
				DX.WriteSoftSoundData(hSSnd, i, (short)t, 0);
				bcnt = (int)(Math.PI * 2 / 44100 * i * hz * 180 / Math.PI);
			}

			var retval = DX.LoadSoundMemFromSoftSound(hSSnd);
			DX.DeleteSoftSound(hSSnd);

			return retval;
		}


		static int GetPlusMinus(int flag)
		{
			return (flag == 1 ? 1 : -1);
		}

		static int GetPlusMinus(bool flag)
		{
			return (flag ? 1 : -1);
		}



		

		static int SetWave(short[] wave, int hz)
		{
			var length = 0;
			for (var i = 0; i < 44100; i++)
				if (Math.PI * 2 / 44100 * i * hz * 180 / Math.PI >= 360)
				{
					Console.Write(Math.PI * 2 / 44100 * i * hz * 180 / Math.PI);
					length = i;
					break;
				}

			var data = new Int16[length];
			float t = 0;
			var hSSnd = DX.MakeSoftSound1Ch16Bit44KHz(length);
			for (var i = 0; i < length; i++)
			{
				t = wave[(int)((Math.PI * 2 / 44100 * i * hz * 180 / Math.PI) % 360 / (360 / 32.0))]; // divided by 10 means volume control
				data[i] = (short)t;
				DX.WriteSoftSoundData(hSSnd, i, (short)t, 0);
			}

			var retval = DX.LoadSoundMemFromSoftSound(hSSnd);
			DX.DeleteSoftSound(hSSnd);
			
			return retval;
		}

		

		public static int GetFreq(string pitch, int oct)
		{
			int freq;
			switch (pitch.ToUpper())
			{
				case "A":
					freq = 44000;
					break;
				case "A#":
					freq = 46616;

					break;
				case "B":
					freq = 49388;

					break;
				case "C":
					freq = 26163;

					break;
				case "C#":
					freq = 27718;

					break;
				case "D":
					freq = 29366;

					break;
				case "D#":
					freq = 31113;

					break;
				case "E":
					freq = 32963;

					break;
				case "F":
					freq = 34923;

					break;
				case "F#":
					freq = 36999;

					break;
				case "G":
					freq = 39200;

					break;
				case "G#":
					freq = 41530;

					break;
				default:
					throw new ArgumentException("音階名が異常です。");
					
			}
			

			if (oct >= 4)
				freq = freq * ((int)Math.Pow(2, oct - 4));
			else
				switch (oct)
				{
					case 3:
						freq = freq / 2;
						return freq / 100;
					case 2:
						freq = freq / 2 / 2;
						return freq / 100;
					case 1:
						freq = freq / 2 / 2 / 2;
						return freq / 100;
					default:
						throw new ArgumentException("オクターブが以上です。");
					   
				}
			return freq / 100;
		}
			
	}

	

}
