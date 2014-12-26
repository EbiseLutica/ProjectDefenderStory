using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;
using System.Drawing;
using System.Windows.Forms;

using NextMidi;
using NextMidi.MidiPort;
using NextMidi.MidiPort.Input;

namespace MusicSheetSoundEditor
{
	
	public class Program
	{

		public static short[] WAV_SQUARE = {
													 -32768, -32768, -32768, -32768, -32768, -32768, -32768, -32768, 
													 -32768, -32768, -32768, -32768, -32768, -32768, -32768, -32768, 
													  32767,  32767,  32767,  32767,  32767,  32767,  32767,  32767, 
													  32767,  32767,  32767,  32767,  32767,  32767,  32767,  32767
												 };

		public static short[] WAV_TRIANGLE = {
														-4096,  -8192, -12288, -16384, -20479, -24575, -28671, -32767,
													   -32767, -28671, -24575, -20479, -16384, -12288,  -8192,  -4096,
														 4096,   8192,  12288,  16384,  20479,  24575,  28671,  32767, 
														32767,  28671,  24575,  20479,  16384,  12288,   8192,   4096
													   
												   };


		public static short[] WAV_12Pulse = {
													 -32768, -32768, -32768, -32768,  32767,  32767,  32767,  32767,
													  32767,  32767,  32767,  32767,  32767,  32767,  32767,  32767,
													  32767,  32767,  32767,  32767,  32767,  32767,  32767,  32767, 
													  32767,  32767,  32767,  32767,  32767,  32767,  32767,  32767
												 };


		public static short[] WAV_25Pulse = {
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
			if (DX.DxLib_Init() == -1)
			{
				DX.DxLib_End();
				Console.WriteLine("[DEBUG]DirectX の初期化に失敗しました。");
				return;
			}
			int f = 0;
			int fps = 1;
			DX.SetDrawScreen(DX.DX_SCREEN_BACK);
			//int nextFps = 60;
			int bsec = DateTime.Now.Second;
			int bmsec = DateTime.Now.Millisecond;

			//int nowHandle = 0;
			//short[] wave = new short[32];
			short[] wave = WAV_SQUARE;

			int pan = 0;

			//DX.PlaySoundMem(nowHandle = SetWave(wave, GetFreq("F", 4)), DX.DX_PLAYTYPE_LOOP);

			// editMode 0...Wave 1...Play
			int editMode = 0;

			NoiseOption noiseoption = NoiseOption.None;

			DXButton[] buttonsForEditMode1 = new DXButton[14];
			int octave = 4;
			
			
			string[] pitches = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

			Envelope myenv = new Envelope(0, 0, 255, 0);
			Tone nowTone = null;

			buttonsForEditMode1[0] = new DXButton(new Rectangle(2, 18, 32, 32), "←", Color.Black, Color.White);
			buttonsForEditMode1[0].ClickedAction = new Action<int, DXButton>((mbtn, dxb) =>
			{
				if (mbtn == DX.MOUSE_INPUT_LEFT)
				{
					octave -= 1;
					if (octave < 0)
						octave = 0;
				}
			});

			buttonsForEditMode1[1] = new DXButton(new Rectangle(66, 18, 32, 32), "→", Color.Black, Color.White);
			buttonsForEditMode1[1].ClickedAction = new Action<int, DXButton>((mbtn, dxb) =>
			{
				if (mbtn == DX.MOUSE_INPUT_LEFT)
				{
					octave += 1;
					if (octave > 8)
						octave = 8;
				}
			});

			buttonsForEditMode1[2] = new DXButton(new Rectangle(30, 98, 20, 128), "C", Color.White, Color.Black);
			buttonsForEditMode1[3] = new DXButton(new Rectangle(50, 98, 20, 96), "C#", Color.Black, Color.White);
			buttonsForEditMode1[4] = new DXButton(new Rectangle(70, 98, 20, 128), "D", Color.White, Color.Black);
			buttonsForEditMode1[5] = new DXButton(new Rectangle(90, 98, 20, 96), "D#", Color.Black, Color.White);
			buttonsForEditMode1[6] = new DXButton(new Rectangle(110, 98, 20, 128), "E", Color.White, Color.Black);
			buttonsForEditMode1[7] = new DXButton(new Rectangle(130, 98, 20, 128), "F", Color.White, Color.Black);
			buttonsForEditMode1[8] = new DXButton(new Rectangle(150, 98, 20, 96), "F#", Color.Black, Color.White);
			buttonsForEditMode1[9] = new DXButton(new Rectangle(170, 98, 20, 128), "G", Color.White, Color.Black);
			buttonsForEditMode1[10] = new DXButton(new Rectangle(190, 98, 20, 96), "G#", Color.Black, Color.White);
			buttonsForEditMode1[11] = new DXButton(new Rectangle(210, 98, 20, 128), "A", Color.White, Color.Black);
			buttonsForEditMode1[12] = new DXButton(new Rectangle(230, 98, 20, 96), "A#", Color.Black, Color.White);
			buttonsForEditMode1[13] = new DXButton(new Rectangle(250, 98, 20, 128), "B", Color.White, Color.Black);

			DXButton[] buttonsForEditMode2 = new DXButton[13];

			buttonsForEditMode2[0] = new DXButton(new Rectangle(24, 16, 32, 32), "A↑", Color.Black, Color.White);
			buttonsForEditMode2[1] = new DXButton(new Rectangle(64, 16, 32, 32), "D↑", Color.Black, Color.White);
			buttonsForEditMode2[2] = new DXButton(new Rectangle(104, 16, 32, 32), "S↑", Color.Black, Color.White);
			buttonsForEditMode2[3] = new DXButton(new Rectangle(144, 16, 32, 32), "R↑", Color.Black, Color.White);

			buttonsForEditMode2[4] = new DXButton(new Rectangle(24, 208, 32, 32), "A↓", Color.Black, Color.White);
			buttonsForEditMode2[5] = new DXButton(new Rectangle(64, 208, 32, 32), "D↓", Color.Black, Color.White);
			buttonsForEditMode2[6] = new DXButton(new Rectangle(104, 208, 32, 32), "S↓", Color.Black, Color.White);
			buttonsForEditMode2[7] = new DXButton(new Rectangle(144, 208, 32, 32), "R↓", Color.Black, Color.White);

			buttonsForEditMode2[8] = new DXButton(new Rectangle(190, 162, 48, 32), "Save", Color.Black, Color.White);
			buttonsForEditMode2[9] = new DXButton(new Rectangle(190, 208, 48, 32), "Load", Color.Black, Color.White);
			buttonsForEditMode2[10] = new DXButton(new Rectangle(184, 16, 16, 16), "", Color.White, Color.Black);
			buttonsForEditMode2[11] = new DXButton(new Rectangle(184, 40, 16, 16), "", Color.White, Color.Black);
			buttonsForEditMode2[12] = new DXButton(new Rectangle(184, 64, 16, 16), "", Color.White, Color.Black);


			


			buttonsForEditMode2[0].MouseDownAction2 = new Action<int, DXButton>((mbtn, dxb) =>
			{
				myenv.AttackTime++;
			});

			buttonsForEditMode2[1].MouseDownAction2 = new Action<int, DXButton>((mbtn, dxb) =>
			{
				myenv.DecayTime++;
			});

			buttonsForEditMode2[2].MouseDownAction2 = new Action<int, DXButton>((mbtn, dxb) =>
			{
				myenv.SustainLevel++;
				if (myenv.SustainLevel > 255)
					myenv.SustainLevel = 255;
			});

			buttonsForEditMode2[3].MouseDownAction2 = new Action<int, DXButton>((mbtn, dxb) =>
			{
				myenv.ReleaseTime++;
			});

			buttonsForEditMode2[4].MouseDownAction2 = new Action<int, DXButton>((mbtn, dxb) =>
			{
				myenv.AttackTime--;
				if (myenv.AttackTime < 0)
					myenv.AttackTime = 0;
			});

			buttonsForEditMode2[5].MouseDownAction2 = new Action<int, DXButton>((mbtn, dxb) =>
			{
				myenv.DecayTime--;
				if (myenv.DecayTime < 0)
					myenv.DecayTime = 0;
			});

			buttonsForEditMode2[6].MouseDownAction2 = new Action<int, DXButton>((mbtn, dxb) =>
			{
				myenv.SustainLevel--;
				if (myenv.SustainLevel < 0)
					myenv.SustainLevel = 0;
			});

			buttonsForEditMode2[7].MouseDownAction2 = new Action<int, DXButton>((mbtn, dxb) =>
			{
				myenv.ReleaseTime--;
				if (myenv.ReleaseTime < 0)
					myenv.ReleaseTime = 0;
			});

			buttonsForEditMode2[8].ClickedAction = new Action<int, DXButton>((mbtn, dxb) =>
			{
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.Filter = "Music Sheet サウンドファイル (*.mssf)|*.mssf|すべてのファイル (*.*)|*.*";
				if (sfd.ShowDialog() == DialogResult.Cancel)
					return;
				SaveFileVer2(sfd.FileName, wave, myenv.AttackTime, myenv.DecayTime, myenv.SustainLevel, myenv.ReleaseTime, pan, noiseoption);


			});

			buttonsForEditMode2[9].ClickedAction = new Action<int, DXButton>((mbtn, dxb) =>
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.Filter = "Music Sheet サウンドファイル (*.mssf)|*.mssf|すべてのファイル (*.*)|*.*";
				if (ofd.ShowDialog() == DialogResult.Cancel)
					return;
				int a,d,r;
				byte s;

				LoadFileDynamic(ofd.FileName, out wave, out a, out d, out s, out r, out pan, out noiseoption);

				myenv.AttackTime = a;
				myenv.DecayTime = d;
				myenv.SustainLevel = s;
				myenv.ReleaseTime = r;

			});

			buttonsForEditMode2[10].ClickedAction = new Action<int, DXButton>((mbtn, dxb) =>
				{
					noiseoption = NoiseOption.None;
				});

			buttonsForEditMode2[11].ClickedAction = new Action<int, DXButton>((mbtn, dxb) =>
			{
				noiseoption = NoiseOption.Long;
			});

			buttonsForEditMode2[12].ClickedAction = new Action<int, DXButton>((mbtn, dxb) =>
			{
				noiseoption = NoiseOption.Short;
			});

			Action<int, DXButton> down = new Action<int, DXButton>((mbtn, dbt) =>
			{
				
				//DX.PlaySoundMem(nowHandle = SetWave(wave, GetFreq(dbt.Text, octave)), DX.DX_PLAYTYPE_LOOP);
				if (nowTone != null && nowTone.Playing)
				{
					nowTone.Abort();
					nowTone = null;
				}
				nowTone = new Tone(dbt.Text, octave, wave, myenv, 255, pan, 100, noiseoption);
				nowTone.StartPlay(-1,-1);
			});

			Action<int, DXButton> up = new Action<int, DXButton>((mbtn, dbt) =>
			{
				//DX.StopSoundMem(nowHandle);
				//DX.DeleteSoundMem(nowHandle);
				nowTone.Stop();
			});


			for (int i = 2; i <= 13; i++)
			{
				buttonsForEditMode1[i].MouseDownAction = down;
				buttonsForEditMode1[i].MouseUpAction = up;
				
			}





			//int sheed = 0x8000;

			

			//bool isShortFreq = false;

			

			while (true)
			{

				DX.ProcessMessage();
				DX.ClearDrawScreen();

				int key1 = DX.CheckHitKey(DX.KEY_INPUT_1);
				int key2 = DX.CheckHitKey(DX.KEY_INPUT_2);
				int key3 = DX.CheckHitKey(DX.KEY_INPUT_3);
				int mouseIn = DX.GetMouseInput();
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

						int mouseGridX = mouseX / 10;
						int mouseGridY = (mouseY - 16) / 15;

						for (int x = 0; x < 32; x++)
							DX.DrawBox(x * 10, 136, x * 10 + 10, 136 + (int)(wave[x] / 4095.875 * 15), DX.GetColor(0, 172, 224), 1);
						for (int y = 0; y < 240; y += 15)
						{
							for (int x = 0; x < 320; x += 10)
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
						foreach (DXButton dxb in buttonsForEditMode1)
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
							string hoge = "";
							if (((int)nowTone.envflag) >= 1)
								hoge += "A";
							if (((int)nowTone.envflag) >= 2)
								hoge += "D";
							if (((int)nowTone.envflag) >= 3)
								hoge += "S";
							if (((int)nowTone.envflag) >= 4)
								hoge += "R";
							DX.DrawString(210, 64, hoge, DX.GetColor(255,255,255));
							for (int i = 0; i < 32; i++)
								DX.DrawLine(i + 202, (int)(wave[i] / 4095.875 * 1.2 + 51), i + 202, 51, 0xffffff);
						}

						break;
					case 2:
						foreach (DXButton dxb in buttonsForEditMode2)
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

				for (int i = 0; i < 16; i++)
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
			int output = 0;

			
			

			int bcnt = -1;

			Int16[] data = new Int16[length];
			float t = 0;
			int hSSnd = DX.MakeSoftSound1Ch16Bit44KHz(length);

			for (int i = 0; i < length; i++)
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

			int retval = DX.LoadSoundMemFromSoftSound(hSSnd);
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
			int length = 0;
			for (int i = 0; i < 44100; i++)
				if (Math.PI * 2 / 44100 * i * hz * 180 / Math.PI >= 360)
				{
					Console.Write(Math.PI * 2 / 44100 * i * hz * 180 / Math.PI);
					length = i;
					break;
				}

			Int16[] data = new Int16[length];
			float t = 0;
			int hSSnd = DX.MakeSoftSound1Ch16Bit44KHz(length);
			for (int i = 0; i < length; i++)
			{
				t = wave[(int)((Math.PI * 2 / 44100 * i * hz * 180 / Math.PI) % 360 / (360 / 32.0))]; // divided by 10 means volume control
				data[i] = (short)t;
				DX.WriteSoftSoundData(hSSnd, i, (short)t, 0);
			}

			int retval = DX.LoadSoundMemFromSoftSound(hSSnd);
			DX.DeleteSoftSound(hSSnd);
			
			return retval;
		}

		public static void LoadFileDynamic(string path, out short[] wave, out int a, out int d, out byte s, out int r, out int pan)
		{
			NoiseOption tmp;
			LoadFileDynamic(path, out wave, out a, out d, out s, out r, out pan, out tmp);
		}

		public static void LoadFileDynamic(string path, out short[] wave, out int a, out int d, out byte s, out int r, out int pan, out NoiseOption noiseoption)
		{
			System.IO.BinaryReader br = null;
			try
			{
				br = new System.IO.BinaryReader(new System.IO.FileStream(path, System.IO.FileMode.Open));
			}
			catch (System.IO.FileNotFoundException)
			{
				throw new Exception("ERR:0004");
			}
			char[] head = br.ReadChars(8);
			if (new string(head) != "MSSF_VER")
			{
				throw new Exception("ERR:0002");
			}
			int ver = 0;
			try
			{
				ver = int.Parse(new string(br.ReadChars(3)));
			}
			catch (Exception)
			{
				throw new Exception("ERR:0002");
			}
			br.Close();
			switch (ver)
			{
				case 1:
					LoadFileVer1(path, out wave, out a, out d, out s, out r, out pan);
					noiseoption = NoiseOption.None;
					break;
				case 2:
					LoadFileVer2(path, out wave, out a, out d, out s, out r, out pan, out noiseoption);
					break;
				default:
					throw new Exception("ERR:0005");

			}
		}

		public static void SaveFileVer1(string path, short[] wave, Int32 a, Int32 d, byte s, Int32 r, Int32 pan)
		{
			System.IO.BinaryWriter bw = null;
			try
			{
				bw = new System.IO.BinaryWriter(new System.IO.FileStream(path, System.IO.FileMode.Create));
			}
			catch (UnauthorizedAccessException)
			{
				throw new Exception("ERR:0003");
			}
			bw.Write(new[] { 'M', 'S', 'S', 'F', '_', 'V', 'E', 'R', '0', '0', '1' }, 0, 11);	//ヘッダー
			foreach (short wav in wave)
				bw.Write(wav);							//波形データ
			bw.Write(a);								//アタックタイム
			bw.Write(d);								//ディケイタイム
			bw.Write(s);								//サスティンレベル
			bw.Write(r);								//リリースタイム
			bw.Write(pan);								//パンポット
			
			bw.Close();									//ストリームを閉じる
		}

		public static void SaveFileVer2(string path, short[] wave, Int32 a, Int32 d, byte s, Int32 r, Int32 pan, NoiseOption noiseoption)
		{
			System.IO.BinaryWriter bw = null;
			try
			{
				bw = new System.IO.BinaryWriter(new System.IO.FileStream(path, System.IO.FileMode.Create));
			}
			catch (UnauthorizedAccessException)
			{
				throw new Exception("ERR:0003");
			}
			bw.Write(new[] { 'M', 'S', 'S', 'F', '_', 'V', 'E', 'R', '0', '0', '2' }, 0, 11);	//ヘッダー
			foreach (short wav in wave)
				bw.Write(wav);							//波形データ
			bw.Write(a);								//アタックタイム
			bw.Write(d);								//ディケイタイム
			bw.Write(s);								//サスティンレベル
			bw.Write(r);								//リリースタイム
			bw.Write(pan);								//パンポット
			bw.Write((byte)noiseoption);
			bw.Close();									//ストリームを閉じる
		}

		public static void LoadFileVer1(string path, out short[] wave, out int a, out int d, out byte s, out int r, out int pan)
		{
			wave = new short[32];
			System.IO.BinaryReader br = null;
			try
			{
				br = new System.IO.BinaryReader(new System.IO.FileStream(path, System.IO.FileMode.Open));
			}
			catch (System.IO.FileNotFoundException)
			{
				throw new Exception("ERR:0004");
			}
			char[] head = br.ReadChars(11);
			if (new string(head) != "MSSF_VER001")
			{
				if (new string(head).Substring(0, 8) == "MSSF_VER")
				{
					throw new Exception("ERR:0001");	//指定した Music Sheet Sound File のバージョンが異なる。
				}
				else
				{
					throw new Exception("ERR:0002");	//そのファイルは Music Sheet Sound File ではない。
				}
			}
			for (int i = 0; i < 32; i++)
				wave[i] = br.ReadInt16();

			a = br.ReadInt32();
			d = br.ReadInt32();
			s = br.ReadByte();
			r = br.ReadInt32();

			pan = br.ReadInt32();
			br.Close();
		}

		public static void LoadFileVer2(string path, out short[] wave, out int a, out int d, out byte s, out int r, out int pan, out NoiseOption noiseoption)
		{
			wave = new short[32];
			System.IO.BinaryReader br = null;
			try
			{
				br = new System.IO.BinaryReader(new System.IO.FileStream(path, System.IO.FileMode.Open));
			}
			catch (System.IO.FileNotFoundException)
			{
				throw new Exception("ERR:0004");
			}
			char[] head = br.ReadChars(11);
			if (new string(head) != "MSSF_VER002")
			{
				if (new string(head).Substring(0, 8) == "MSSF_VER")
				{
					throw new Exception("ERR:0001");	//指定した Music Sheet Sound File のバージョンが異なる。
				}
				else
				{
					throw new Exception("ERR:0002");	//そのファイルは Music Sheet Sound File ではない。
				}
			}
			for (int i = 0; i < 32; i++)
				wave[i] = br.ReadInt16();

			a = br.ReadInt32();
			d = br.ReadInt32();
			s = br.ReadByte();
			r = br.ReadInt32();

			pan = br.ReadInt32();
			int tmp = br.ReadByte();
			if (tmp > 2)
				throw new Exception("ERR:0006");

			noiseoption = (NoiseOption)tmp;
			br.Close();
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

	public static class ColorPallete
	{
		public static Color Red = Color.FromArgb(255, 0, 0);
		public static Color Orange = Color.FromArgb(255, 128, 0);
		public static Color Yellow = Color.FromArgb(255, 255, 0);
		public static Color LimeGreen = Color.FromArgb(0, 255, 0);
		public static Color Green = Color.FromArgb(0, 128, 64);
		public static Color Cyan = Color.FromArgb(0, 255, 255);
		public static Color Blue = Color.FromArgb(0, 0, 255);
		public static Color Purple = Color.FromArgb(128, 0, 255);
		public static Color Pink = Color.FromArgb(255, 0, 128);
		public static Color DarkRed = Color.FromArgb(164, 32, 0);
		public static Color White = Color.FromArgb(255, 255, 255);
		public static Color CreamSoda = Color.FromArgb(192, 255, 144);
		public static Color SkyBlue = Color.FromArgb(64, 172, 255);
		public static Color Windows98 = Color.FromArgb(0, 128, 128);
		public static Color LightPink = Color.FromArgb(255, 142, 142);
		public static Color LightOrange = Color.FromArgb(255, 192, 128);

		public static Color GetColorByIndex(int index)
		{
			switch (index)
			{
				case 0:
					return Red;
				case 1:
					return Orange;
				case 2:
					return Yellow;
				case 3:
					return LimeGreen;
				case 4:
					return Green;
				case 5:
					return Cyan;
				case 6:
					return Blue;
				case 7:
					return Purple;
				case 8:
					return Pink;
				case 9:
					return DarkRed;
				case 10:
					return White;
				case 11:
					return CreamSoda;
				case 12:
					return SkyBlue;
				case 13:
					return Windows98;
				case 14:
					return LightPink;
				case 15:
					return LightOrange;
				default:
					return Color.Black;

			}
		}

		

	}

	public class DXButton
	{
		/// <summary>
		/// 位置と大きさです。
		/// </summary>
		public Rectangle PointAndSize { get; set; }

		public Color BackColor { get; set; }
		public Color ForeColor { get; set; }

		/// <summary>
		/// 表示するテキストです。
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// クリックされた時に実行するメソッドです。
		/// </summary>
		public Action<int, DXButton> ClickedAction { get; set; }

		/// <summary>
		/// マウスが押された時に実行するメソッドです。
		/// </summary>
		public Action<int, DXButton> MouseDownAction { get; set; }

		/// <summary>
		/// マウスが押されている間ずっと実行されるメソッドです。
		/// </summary>
		public Action<int, DXButton> MouseDownAction2 { get; set; }

		/// <summary>
		/// マウスが離された時に実行するメソッドです。
		/// </summary>
		public Action<int, DXButton> MouseUpAction { get; set; }

		Queue<Tuple<Action<int, DXButton>, int>> taskqueue = new Queue<Tuple<Action<int, DXButton>, int>>();

		/// <summary>
		/// 四角形領域およびテキストを指定し、 DXButton の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="pointsize">位置と大きさ。</param>
		/// <param name="text">表示するテキスト。</param>
		public DXButton(Rectangle pointsize, string text, Color bc, Color fc)
		{
			PointAndSize = pointsize;
			Text = text;
			BackColor = bc;
			ForeColor = fc;
			
		}

		int bmbtn = 0;

		bool isMouseDown = false;
		
		

		/// <summary>
		/// 描画とともに、イベントを処理します。
		/// </summary>
		public void Draw()
		{
			if (DX.DxLib_IsInit() == 0)
			{
				throw new Exception("DXLib が初期化されていません。");
			}

			
			DX.DrawBox(PointAndSize.X + Convert.ToByte(isMouseDown), PointAndSize.Y + Convert.ToByte(isMouseDown), PointAndSize.X + PointAndSize.Width - Convert.ToByte(isMouseDown), PointAndSize.Y + PointAndSize.Height - Convert.ToByte(isMouseDown), DX.GetColor(Math.Abs(BackColor.R - 255 * Convert.ToByte(isMouseDown)), Math.Abs(BackColor.G - 255 * Convert.ToByte(isMouseDown)), Math.Abs(BackColor.B - 255 * Convert.ToByte(isMouseDown))), 1);
			DX.DrawBox(PointAndSize.X + Convert.ToByte(isMouseDown), PointAndSize.Y + Convert.ToByte(isMouseDown), PointAndSize.X + PointAndSize.Width - Convert.ToByte(isMouseDown), PointAndSize.Y + PointAndSize.Height - Convert.ToByte(isMouseDown), DX.GetColor(Math.Abs(ForeColor.R - 255 * Convert.ToByte(isMouseDown)), Math.Abs(ForeColor.G - 255 * Convert.ToByte(isMouseDown)), Math.Abs(ForeColor.B - 255 * Convert.ToByte(isMouseDown))), 0);
			DX.DrawString(PointAndSize.X + PointAndSize.Width / 2 - DX.GetDrawStringWidth(this.Text, this.Text.Length) / 2, PointAndSize.Y + PointAndSize.Height / 2 - DX.GetFontSize() / 2, this.Text, DX.GetColor(Math.Abs(ForeColor.R - 255 * Convert.ToByte(isMouseDown)), Math.Abs(ForeColor.G - 255 * Convert.ToByte(isMouseDown)), Math.Abs(ForeColor.B - 255 * Convert.ToByte(isMouseDown))));

			int mx, my, mbtn;


			DX.GetMousePoint(out mx, out my);
			mbtn = DX.GetMouseInput();
			bool contain = PointAndSize.Contains(mx, my);

			
			if (bmbtn > 0 && mbtn == 0 && contain)
			{
				if (ClickedAction != null)
					taskqueue.Enqueue(Tuple.Create<Action<int, DXButton>, int>(ClickedAction, bmbtn));
					
				//Console.WriteLine("debug_isclicked");
			}
			if (bmbtn == 0 && mbtn > 0 && !isMouseDown && contain)
			{

				isMouseDown = true;
				if (MouseDownAction != null)
					taskqueue.Enqueue(Tuple.Create<Action<int, DXButton>, int>(MouseDownAction, mbtn));
			}

			if (mbtn > 0 && contain && isMouseDown)
			{
				if (MouseDownAction2 != null)
					taskqueue.Enqueue(Tuple.Create<Action<int, DXButton>, int>(MouseDownAction2, mbtn));
			}

			//DX.DrawString(0, 0, isMouseDown.ToString() + " " + mbtn + " " + Convert.ToByte(isMouseDown), DX.GetColor(255, 255, 255));

			if (bmbtn > 0 && mbtn == 0 && isMouseDown)
			{
				isMouseDown = false;
				if (MouseUpAction != null)
					taskqueue.Enqueue(Tuple.Create<Action<int, DXButton>, int>(MouseUpAction, bmbtn));
				//Console.WriteLine("debug_ismouseup");
			}

			if (taskqueue.Count > 0)
			{
				Tuple<Action<int, DXButton>, int> temp = taskqueue.Dequeue();
				temp.Item1.Invoke(temp.Item2, this);
			}

			bmbtn = mbtn;
		}

	}

	public class DXTrackBar
	{
		/// <summary>
		/// 位置と大きさです。
		/// </summary>
		Rectangle PointAndSize { get; set; }

		/// <summary>
		/// 背景色です。
		/// </summary>
		public Color BackColor { get; set; }
		/// <summary>
		/// 前景色です。
		/// </summary>
		public Color ForeColor { get; set; }

		/// <summary>
		/// 値です。
		/// </summary>
		public int Value { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsValueChanging { get; private set; }

		/// <summary>
		/// 値の変更が終わった瞬間のイベントです。
		/// </summary>
		public Action<int, DXTrackBar> ValueChangedAction { get; set; }

		Queue<Tuple<Action<int, DXTrackBar>, int>> taskqueue = new Queue<Tuple<Action<int, DXTrackBar>, int>>();

		/// <summary>
		/// 最大値です。
		/// </summary>
		public int MaxValue { get; set; }

		/// <summary>
		/// 矩形領域、色、最大値を指定し、 DXLib で動作するトラックバーの新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="pointsize">位置と大きさ。</param>
		/// <param name="text">表示するテキスト。</param>
		/// <param name="bc">背景色。</param>
		/// <param name="fc">前景色。</param>
		/// <param name="mv">最大値。</param>
		public DXTrackBar(Rectangle pointsize, Color bc, Color fc, int mv)
		{
			PointAndSize = pointsize;
			BackColor = bc;
			ForeColor = fc;
			MaxValue = mv;
		}

		int bmbtn = 0;

		//bool isMouseDown = false;



		/// <summary>
		/// 描画とともに、イベントを処理します。
		/// </summary>
		public void Draw()
		{
			if (DX.DxLib_IsInit() == 0)
			{
				throw new Exception("DXLib が初期化されていません。");
			}


			

			int mx, my, mbtn;

			


			DX.GetMousePoint(out mx, out my);
			mbtn = DX.GetMouseInput();
			bool contain = PointAndSize.Contains(mx, my);

			if (mbtn == 1 && contain)
			{
				this.IsValueChanging = true;
				Value = (mx- PointAndSize.X) * (MaxValue / (PointAndSize.Width - PointAndSize.X));
			}

			if (bmbtn > 0 && mbtn == 0 && contain)
			{
				if (ValueChangedAction != null)
					taskqueue.Enqueue(Tuple.Create<Action<int, DXTrackBar>, int>(ValueChangedAction, this.Value));
				this.IsValueChanging = false;
				//Console.WriteLine("debug_isclicked");
			}

			int dx = PointAndSize.X + Value * (PointAndSize.Width - PointAndSize.X) / MaxValue;
			DX.DrawBox(PointAndSize.X, PointAndSize.Y, PointAndSize.X + PointAndSize.Width, PointAndSize.Y + PointAndSize.Height, BackColor.ToArgb(), 1);
			DX.DrawBox(PointAndSize.X, PointAndSize.Y, PointAndSize.X + PointAndSize.Width, PointAndSize.Y + PointAndSize.Height, ForeColor.ToArgb(), 0);
			DX.DrawBox(PointAndSize.X, PointAndSize.Y, dx, PointAndSize.Y + PointAndSize.Height, ForeColor.ToArgb(), 1);

			//DX.DrawString(0, 0, isMouseDown.ToString() + " " + mbtn + " " + Convert.ToByte(isMouseDown), DX.GetColor(255, 255, 255));

			if (taskqueue.Count > 0)
			{
				Tuple<Action<int, DXTrackBar>, int> temp = taskqueue.Dequeue();
				temp.Item1.Invoke(temp.Item2, this);
			}

			bmbtn = mbtn;
		}

	}

	public interface ITone
	{
		string Pitch { get; set; }
		int Octave { get; set; }
		int Freq { get; set; }
		int OutVolume { get; set; }
		int tick { get; set; }
		bool Playing { get; set; }
		int Handle { get; set; }

		void StartPlay(int miditick, int gate);
		void PlayLoop(int miditick);
		void Stop();
		void Abort();


	}

	public class Tone : ITone
	{
		public string Pitch { get; set; }
		public int Octave { get; set; }

		public short[] Wave { get; set; }

		public int Volume { get; set; }

		public int Freq { get; set; }

		public EnvelopeFlag envflag = EnvelopeFlag.None;
		
		public Envelope Envelope { get; set; }

		public int OutVolume {get;set;}

		public int tick { get; set; }

		public bool Playing { get; set; }

		public int Handle { get; set; }
		public bool isStopping = false;
		public int Velocity { get; set; }

		public int noteno = 0;

		public int bmilisec = 0;
		public int startedMidiTick = 0;
		public int nowMidiTick = 0;
		public int releasedVolume = 0;

		

		public Tone(string pitch, int octave, short[] wave, Envelope env, int vol, int pan)
		{
			if (DX.DxLib_IsInit() == 0) 
			{
				throw new Exception("DXLib が初期化されていません。");
			}
			Pitch = pitch;
			Octave = octave;
			Wave = wave;

			Handle = SetWave(wave, Freq = GetFreq(pitch, octave), pan);
			Envelope = env;

			Volume = vol;

			Playing = false;
			Velocity = 100;
		}

		public Tone(string pitch, int octave, short[] wave, Envelope env, int vol, int pan, int vel)
		{
			if (DX.DxLib_IsInit() == 0)
			{
				throw new Exception("DXLib が初期化されていません。");
			}
			Pitch = pitch;
			Octave = octave;
			Wave = wave;

			Handle = SetWave(wave, Freq = GetFreq(pitch, octave), pan);
			Envelope = env;

			Volume = vol;

			Velocity = vel;

			Playing = false;
		}

		public Tone(string pitch, int octave, short[] wave, Envelope env, int vol, int pan, int vel, NoiseOption noiseflag)
		{
			if (DX.DxLib_IsInit() == 0)
			{
				throw new Exception("DXLib が初期化されていません。");
			}

			

			Pitch = pitch;
			Octave = octave;
			Wave = wave;

			Handle = SetWave(wave, Freq = GetFreq(pitch, octave), pan, noiseflag);
			Envelope = env;

			Volume = vol;

			Velocity = vel;

			Playing = false;

		}

		public int gate;

		public void StartPlay(int miditick, int gate)
		{
			OutVolume = Volume;
			Playing = true;
			DX.PlaySoundMem(Handle, DX.DX_PLAYTYPE_LOOP);
			DX.ChangeVolumeSoundMem(0, Handle);
			this.envflag = EnvelopeFlag.Attack;
			tick = 0;
			bmilisec = DX.GetNowCount();
			startedMidiTick = miditick;
			this.gate = gate;
			//Console.WriteLine("[DEBUG]音源再生開始");
		}

		bool beforenew = false;

		public void PlayLoop(int miditick)
		{
			if (!this.Playing)
				return;
			if (isNew && beforenew)
				isNew = false;
			if (!beforenew)
				beforenew = true;
			//if (DX.GetNowCount() - bmilisec <= 0)
			//	return;
				
			switch (envflag)
			{
				case EnvelopeFlag.Attack:
					if (Envelope.AttackTime <= 1)
					{
						tick = -1;
						envflag = EnvelopeFlag.Decay;
						break;
					}
					if (tick >= Envelope.AttackTime)
					{
						tick = -1;
						envflag = EnvelopeFlag.Decay;
						break;
					}
					OutVolume = (int)((tick)*((float)Volume / (Envelope.AttackTime - 1)));
					break;
				case EnvelopeFlag.Decay:
					if (Envelope.DecayTime == 0)
					{
						tick = -1;
						envflag = EnvelopeFlag.Sustain;
						break;
					}
					if (tick >= Envelope.DecayTime)
					{
						tick = -1;
						envflag = EnvelopeFlag.Sustain;
						break;
					}
					OutVolume = (int)(Volume - (tick + 1) * ((float)(Volume - Envelope.SustainLevel * (Volume / 255.0)) / Envelope.DecayTime));
					break;
				case EnvelopeFlag.Release:
					if (tick >= Envelope.ReleaseTime)
					{
						Abort();
						return;
					}
					OutVolume = (int)(releasedVolume - (tick + 1) * ((float)releasedVolume / Envelope.ReleaseTime));
					break;
			}
			
			
			nowMidiTick = miditick;
			if (nowMidiTick - startedMidiTick > gate && gate != -1 && envflag != EnvelopeFlag.Release)
				this.Stop();
			tick += DX.GetNowCount() - bmilisec;

			bmilisec = DX.GetNowCount();
			//Console.WriteLine("[DEBUG]音源ループ: {0}, {1}, {2}", outVolume, tick, envflag);
		}

		public void Stop()
		{
			releasedVolume = OutVolume;
			envflag = EnvelopeFlag.Release;
			tick = 0;
			isStopping = true;
			//Console.WriteLine("[DEBUG]音源再生終了リクエスト受信");
		}

		public void Abort()
		{
			tick = 0;
			envflag = EnvelopeFlag.None;
			DX.StopSoundMem(Handle);
			DX.DeleteSoundMem(Handle);
			Playing = false;
			
			//Console.WriteLine("[DEBUG]音源再生終了");
		}

		/// <summary>
		/// 一周期分の波形データ、周波数、パンポットを指定して、波形データを作成します。
		/// </summary>
		/// <param name="wave">32個の波形データ。</param>
		/// <param name="hz">周波数。</param>
		/// <param name="pan">-100 ～ +100 の範囲で、パンポット。</param>
		/// <returns>生成されたサウンドバッファーのハンドル。</returns>
		public static int SetWave(short[] wave, int hz, int pan)
		{
			return SetWave(wave, hz, pan, NoiseOption.None);
		}

		public static int SetWave(short[] wave, int hz, int pan, NoiseOption no)
		{
	
			int length = 0;
			//string debug = "";
			int sheed = 0x8000;
			/*for (int i = 0; i < 44100; i++)
				if ((decimal)Math.PI * 2 / (decimal)44100 * i * (hz + (hz / 440m)) > (decimal)Math.PI * 2)
				{
					//Console.Write(Math.PI * 2 / 44100 * i * hz * 180 / Math.PI);
					length = i;
					break;
				}
			 */
			length = (int)Math.Round(44100m / hz);
			//length *= 64;
			//		Hz = 1 / s
			//×s	s × Hz = 1
			//÷Hz	s = 1 / Hz
			//if (hz >= 1046)
			//	length = 11025;
			float t = 0;

			if (no != NoiseOption.None)
				length = 11025;

			int hSSnd = DX.MakeSoftSound1Ch16Bit44KHz(length);

			//int[] sikis = new int[length];
			ushort[] y = new ushort[length];

			ushort[] noise = new ushort[length];
			if (no != NoiseOption.None)
			{
				noise = SetNoise(hz, ((no == NoiseOption.Short) ? true : false), ref sheed, length);
			}

			for (int i = 0; i < length; i++)
			{
				//t = wave[(int)((Math.PI * 2 / 44100 * i * hz * 180 / Math.PI) % 360 / (359 / 31.0))]+32768; // divided by 10 means volume control
				int siki = (int)Math.Round(3.141592 * 2 / 44100 * i * hz * 180 / 3.141592 % 360 / (359 / 31.0));
					t = (float)(wave[siki] + 32768);
				//	t = (float)(Math.Sin(Math.PI * 2 / 44100 * i * hz) * 32768 + 32768);
				//sikis[i] = siki;
				//t = wave[(int)(i * (32.0 / length))] + 32768;
				t = (noise[i] + t) / 2;

				
				/*if (i > 0)
				y[i] = (ushort)(0.9f * (ushort)y[i - 1] + 0.1f*t);
				else*/
				y[i] = (ushort)t;
					//debug += (int)((Math.PI * 2 / 44100 * i * hz * 180 / Math.PI) % 360 / (359 / 31.0)) + " ";
				/*
				if (i > 1)
					y[i] = (ushort)((y[i - 2] + y[i - 1] + t) / 3);
				if (i > 0)
					y[i] = (ushort)((y[i - 1] + t) / 3);
				else
					y[i] = (ushort)t;
				*/
				DX.WriteSoftSoundData(hSSnd, i, (ushort)t, (ushort)t);
			}

			int retval = DX.LoadSoundMemFromSoftSound(hSSnd);
			DX.DeleteSoftSound(hSSnd);

			return retval;
		}

		public static ushort[] SetNoise(int hz, bool isShortFreq, ref int sheed, int length)
		{
			int output = 0;




			int bcnt = -1;

			ushort[] data = new ushort[length];
			float t = 0;
			//int hSSnd = DX.MakeSoftSound1Ch16Bit44KHz(length);
			int kaisu = 1;
			for (int i = 0; i < length; i++)
			{
				if (i > 8000 / hz * kaisu)
				{
					sheed >>= 1;
					sheed |= ((sheed ^ (sheed >> (isShortFreq ? 6 : 1))) & 1) << 15;
					output = sheed & 1;
					kaisu++;
				}
				t = output * 65535; // divided by 10 means volume control
				data[i] = (ushort)t;
				
			}



			return data;
		}


		/*
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
					case 0:
						freq = freq / 2 / 2 / 2 / 2;
						return freq / 100;
					default:
						throw new ArgumentException("オクターブが以上です。");

				}
			return freq / 100;
		}
		*/

		public bool isNew = true;

		public int GetFreq(string pitch, int oct)
		{
			int hoge = oct * 12 + pitchnames.IndexOf(pitch) + 12;
			this.noteno = hoge;
			int hage = GetFreq(hoge);
			
			return hage;
		}

		public static List<string> pitchnames = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" }.ToList();

		public int GetFreq(int noteno)
		{
			this.noteno = noteno;
			return (int)(440 * Math.Pow(2, (noteno - 69) / 12.0));
		}

		public static int GetFreqS(int noteno)
		{
			return (int)(440 * Math.Pow(2, (noteno - 69) / 12.0));
		}
		
	

	}

	public enum NoiseOption
	{
		None, Long, Short
	}

	public enum EnvelopeFlag
	{
		None, Attack, Decay, Sustain, Release
	}

	public class Envelope
	{
		public int AttackTime { get; set; }
		public int DecayTime { get; set; }
		public byte SustainLevel { get; set; }
		public int ReleaseTime { get; set; }


		public Envelope(int a, int d, byte s, int r)
		{
			AttackTime = a;
			DecayTime = d;
			SustainLevel = s;
			ReleaseTime = r;
		}

		

	}


}
