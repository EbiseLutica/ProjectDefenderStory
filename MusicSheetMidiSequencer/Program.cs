using System;
using System.Drawing;
using System.Windows.Forms;
using DxLibDLL;
using TakeUpJewel.GUI;

namespace MusicSheetMidiSequencer
{
	internal class Program
	{
		private static readonly DxLabel _textviewer = null;

		private static string _file = "";
		private static bool _playRequest;

		[STAThread]
		private static void Main(string[] args)
		{
			//Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;

			Application.EnableVisualStyles();

			//if (MessageBox.Show("Windows Form で表示しますか？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
			//{
			if (args.Length == 0)
				Application.Run(new MainWindow());
			else
				Application.Run(new MainWindow(args[0]));
			//}

			/*DX.ChangeWindowMode(1);
			//DX.SetEnableXAudioFlag(1);
			DX.SetWaitVSyncFlag(0);
			DX.SetWindowVisibleFlag(0);
			if (DX.DxLib_Init() == -1)
			{
				ShowError("DirectX の初期化に失敗しました。");
				return;
			}
			
			DX.SetWindowText("MSMidiSequencer");
			//DX.SetGraphMode((isFullScreen ? 1366 : 640), (isFullScreen ? 768 : 480), 32);

			//DX.ChangeFontType(DX.DX_FONTTYPE_ANTIALIASING_4X4);

			Sequencer seq = new Sequencer();

			List<Tuple<DXLabel, int>> drumorbs = new List<Tuple<DXLabel, int>>();

			SoundModule sm = new SoundModule();
			DX.SetDrawScreen(DX.DX_SCREEN_BACK);


			
			DX.SetAlwaysRunFlag(1);

			DX.SetFontSize(10);
			DX.SetFontThickness(1);

			int bmilisec = DX.GetNowCount();
			int baseofdrumspeed = 1920;
			int f = 0, fps = 0;
			seq.Reset();

			float tps = 0;

			int sx, sy, cbd;
			DX.GetScreenState(out sx, out sy, out cbd);

			string btitle, bcopy, blyric;
			btitle = bcopy = blyric = "";

			//mc.SetMIDIInSyncMode(CMIDIClock.SLAVEMIDITIMINGCLOCK);

			List<DXOrb> orbs = new List<DXOrb>();
			Point mainMonitor = new Point(0, 0);
			//MainWindow mw = new MainWindow();
			//if (!isFullScreen)
			//Show();
			/*Move += (object sender, EventArgs e) =>
			{
				if (seq.mc != null)
					seq.mc.Stop();
			};*/
			/*
			int fmhandle = 0;
			bool isPausing = false;

			bool loaded = false;
			int xplus = 384;
			int btick = 0;
			int btick2 = 0;
			int mode = 0;
			#region GUIリセット

			textviewer = new DXLabel(256, 24, "", Color.Black, Color.White);

			DXButton Mode1Btn = new DXButton(new Rectangle(16, 96, 128, 48), "ﾃｷｽﾄ ﾓﾆﾀｰ", Color.Black, Color.White)
				{
					ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) => mode = 0)
				};

			DXButton Mode2Btn = new DXButton(new Rectangle(16, 160, 128, 48), "MIDIｱﾆﾒ", Color.Black, Color.White)
				{
					ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) => mode = 1)
				};

			DXButton Mode3Btn = new DXButton(new Rectangle(16, 224, 128, 48), "楽曲ﾘｽﾄ", Color.Black, Color.White)
				{
					ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) => mode = 2)
				};

			DXButton Mode4Btn = new DXButton(new Rectangle(16, 288, 128, 48), "ドラムフロー", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) => mode = 3)
			};

			DXTrackBar SeekBar = new DXTrackBar(new Rectangle(16, 400, 448, 16), Color.Black, Color.White, 100)
			{
				ValueChangedAction = new Action<int, DXTrackBar>((value, seekbar) =>
				{
					seq.sm.Panic();
					seq.btick = value;
					seq.mc.SetTickCount(value);
				})
			};
			int[] wav = new int[1000];
			DXButton Play = new DXButton(new Rectangle(16, 420, 48, 48), "[>", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((hoge, fuga) =>
					{
						if (!loaded)
						{
							//fmhandle = CreateFM(out wav);
							//DX.PlaySoundMem(fmhandle, DX.DX_PLAYTYPE_LOOP);
							return;
						}

						if (!isPausing)
						{
							drumorbs.Clear();
							foreach (int num in seq.drumtracks)
								for (int i = 0; i < baseofdrumspeed; i++)
								{
									if (i >= seq.mfd.Tracks[num].TickLength)
										break;
									foreach (MidiEvent evnt in seq.mfd.Tracks[num].GetTickData(i, i))
									{
										if (!(evnt is NoteEvent))
											continue;
										NoteEvent ne = (NoteEvent)evnt;

										switch (ne.Note)
										{
											case 36:
												drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
												break;
											case 38:
												drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 16, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
												break;
											case 40:
												drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 32, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
												break;
											case 42:
												drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 32, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
												break;
											case 46:
												drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 48, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
												break;
											case 49:
												drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 64, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
												break;
											case 50:
												drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 80, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
												break;
											case 48:
												drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 80, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
												break;
											case 47:
												drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 96, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
												break;
											case 45:
												drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 96, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
												break;
											case 43:
												drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 112, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
												break;
											case 41:
												drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 112, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
												break;
										}
									}
								}
							seq.Reset();
							seq.Play();
						}
						else
							seq.Resume();
						isPausing = false;
					})
			};

			DXButton Pause = new DXButton(new Rectangle(80, 420, 48, 48), "||", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((hoge, fuga) =>
					{
						if (!loaded)
							return;
						seq.Stop();
						seq.sm.Panic();
						isPausing = true;
					})
			};

			DXButton Stop = new DXButton(new Rectangle(144, 420, 128, 48), "[]", Color.Black, Color.White)
			{

				ClickedAction = new Action<int, DXButton>((hoge, fuga) =>
					{
						if (!loaded)
							return;
						seq.sm.Panic();
						seq.Stop();
					})
			};

			

			DXButton Gakkyoku1 = new DXButton(new Rectangle(mainMonitor.X + 300, mainMonitor.Y + 26, 128, 24), "散歩", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) =>
				{
					playRequest = true;
					file = ".\\Resource\\散歩.mid";
				})
			};
			DXButton Gakkyoku2 = new DXButton(new Rectangle(mainMonitor.X + 300, mainMonitor.Y + 50, 128, 24), "流れる水の働き", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) =>
				{
					playRequest = true;
					file = ".\\Resource\\流れる水の働き.mid";
				})
			};
			DXButton Gakkyoku3 = new DXButton(new Rectangle(mainMonitor.X + 300, mainMonitor.Y + 74, 128, 24), "入道雲は天高く", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) =>
				{
					playRequest = true;
					file = ".\\Resource\\nyudou.mid";
				})
			};
			DXButton Gakkyoku4 = new DXButton(new Rectangle(mainMonitor.X + 300, mainMonitor.Y + 98, 128, 24), "おもちゃｴﾚｸﾄﾛﾆｸｽ", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) =>
				{
					playRequest = true;
					file = ".\\Resource\\おもちゃエレクトロニクス_ms.mid";
				})
			};
			DXButton Gakkyoku5 = new DXButton(new Rectangle(mainMonitor.X + 300, mainMonitor.Y + 122, 128, 24), "雨の日の散歩", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) =>
				{
					playRequest = true;
					file = ".\\Resource\\雨の日の散歩.mid";
				})
			};
			DXButton Gakkyoku6 = new DXButton(new Rectangle(mainMonitor.X + 300, mainMonitor.Y + 146, 128, 24), "何もない", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) =>
				{
					playRequest = true;
					file = ".\\Resource\\nothing.mid";
				})
			};
			DXButton Gakkyoku7 = new DXButton(new Rectangle(mainMonitor.X + 300, mainMonitor.Y + 170, 128, 24), "駅", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) =>
				{
					playRequest = true;
					file = ".\\Resource\\trainstation.mid";
				})
			};
			DXButton Gakkyoku8 = new DXButton(new Rectangle(mainMonitor.X + 300, mainMonitor.Y + 194, 128, 24), "発車", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) =>
				{
					playRequest = true;
					file = ".\\Resource\\trainstart.mid";
				})
			};
			DXButton Gakkyoku9 = new DXButton(new Rectangle(mainMonitor.X + 300, mainMonitor.Y + 218, 128, 24), "乗車中", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) =>
				{
					playRequest = true;
					file = ".\\Resource\\trainnow.mid";
				})
			};
			DXButton Gakkyoku10 = new DXButton(new Rectangle(mainMonitor.X + 300, mainMonitor.Y + 242, 128, 24), "春が来る", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) =>
				{
					playRequest = true;
					file = ".\\Resource\\comespring.mid";
				})
			};
			DXButton Gakkyoku11 = new DXButton(new Rectangle(mainMonitor.X + 300, mainMonitor.Y + 266, 128, 24), "ｱｷﾗﾒﾅｲ", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) =>
				{
					playRequest = true;
					file = ".\\Resource\\アキラメナイ.mid";
				})
			};

			DXButton Gakkyoku12 = new DXButton(new Rectangle(mainMonitor.X + 300, mainMonitor.Y + 290, 128, 24), "戦闘！", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) =>
				{
					playRequest = true;
					file = ".\\Resource\\戦闘！.mid";
				})
			};
			DXButton Gakkyoku13 = new DXButton(new Rectangle(mainMonitor.X + 300, mainMonitor.Y + 314, 128, 24), "ｽｹｱﾘｰ･ﾛｰﾄﾞ", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) =>
				{
					playRequest = true;
					file = ".\\Resource\\スケアリー・ロード.mid";
				})
			};
			DXButton Gakkyoku14 = new DXButton(new Rectangle(mainMonitor.X + 300, mainMonitor.Y + 338, 128, 24), "野生の心", Color.Black, Color.White)
			{
				ClickedAction = new Action<int, DXButton>((mbtn, dxbtn) =>
				{
					playRequest = true;
					file = ".\\Resource\\野生の心.mid";
				})
			};


			#endregion
			//mc.SetTickCount(110000);

			int pianoRoll = DX.MakeScreen(sx - mainMonitor.X, sy - mainMonitor.Y);
			//string filename = "";
			var bairitu = 1;
			/*IntPtr hwnd = DX.GetMainWindowHandle();
			var aa = ((System.Windows.Forms.Control)new System.Windows.Forms.Form());
			var a = typeof(System.Windows.Forms.Control);
			var ff = a.GetField("window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var puke = ff.GetValue(aa);
			var handle = typeof(System.Windows.Forms.NativeWindow).GetField("handle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var hb = handle.GetValue(puke);

			handle.SetValue(puke, hwnd);
			Form dxform = (Form)aa;

			dxform.AllowDrop = true;

			dxform.DragDrop += dxform_DragDrop;  

			dxform.DragEnter += dxform_DragEnter;


			dxform.MouseWheel += dxform_MouseWheel;
			
			dxform.Focus();

			dxform.Show();
			*/
			/*
			Icon ico = new Icon("msmidplayer.ico");
			DX.SetWindowIconHandle(ico.Handle);

			DX.SetWindowVisibleFlag(1);

			int bmilisec2 = DX.GetNowCount();
			while (true)
			{

				int inEsc = DX.CheckHitKey(DX.KEY_INPUT_ESCAPE);
				int inP = DX.CheckHitKey(DX.KEY_INPUT_P);

				if (inP == 1)
				{
					DX.SaveDrawScreenToPNG(0, 0, sx, sy, "スクリーンショット" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".png");
				}
				//if (!seq.mc.IsRunning() && seq.IsPlaying)
				//	seq.mc.Start();
				//seq.mc.fps = fps;
				if (playRequest)
				{
					playRequest = false;
					foreach (Dictionary<int, Tone> t in seq.sm.Tones)
						foreach (Tone to in t.Values)
						{
							to.Abort();
						}
					sm = new SoundModule();
					seq.mc.Stop();
					try
					{
						drumorbs.Clear();
						seq = new Sequencer();
						seq.Reset();
						seq.Load(file);
						seq.Play();
						DX.DeleteGraph(pianoRoll);
						pianoRoll = DX.MakeScreen(sx - mainMonitor.X, sy - mainMonitor.Y);
						SeekBar.MaxValue = seq.eot;
						loaded = true;

						foreach (int num in seq.drumtracks)
							for (int i = 0; i < baseofdrumspeed; i++)
							{
								if (i >= seq.mfd.Tracks[num].TickLength)
									break;
								foreach (MidiEvent evnt in seq.mfd.Tracks[num].GetTickData(i, i))
								{
									if (!(evnt is NoteEvent))
										continue;
									NoteEvent ne = (NoteEvent)evnt;

									switch (ne.Note)
									{
										case 36:
											drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
											break;
										case 38:
											drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 16, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
											break;
										case 40:
											drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 32, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
											break;
										case 42:
											drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 32, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
											break;
										case 46:
											drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 48, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
											break;
										case 49:
											drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 64, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
											break;
										case 50:
											drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 80, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
											break;
										case 48:
											drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 80, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
											break;
										case 47:
											drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 96, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
											break;
										case 45:
											drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 96, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
											break;
										case 43:
											drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 112, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
											break;
										case 41:
											drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 112, mainMonitor.Y, "●", Color.Transparent, Color.White), i - baseofdrumspeed));
											break;
									}
								}
							}

					}
					catch (Exception ex)
					{
						MessageBox.Show("エラーが発生しました。例外名: " + ex.GetType().FullName + "\r\nメッセージ: " + ex.Message + "\r\nスタックトレース: " + ex.StackTrace, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
						seq.Stop();
						loaded = false;
					}
					continue;
				}

				if (inEsc == 1)
				{
					DX.DxLib_End();
					seq.mc.Stop();
					return;
				}

				DX.ProcessMessage();
				DX.ClearDrawScreen();

				DX.DrawString(0, 0, string.Format("{0}ms {1}tick {2}tps {3}fps", seq.nMillisec, seq.nTickCount, tps, fps), DX.GetColor(255, 255, 255));

				/*
				DX.DrawString(32, 64, seq.title, DX.GetColor(255, 255, 255));
				DX.DrawString(32, 96, seq.copyright, DX.GetColor(255, 255, 255));
				DX.DrawString(32, 128, seq.lyrics, DX.GetColor(255, 255, 255));
				DX.DrawString(32, 164, seq.sm.loop.ToString(), DX.GetColor(255, 255, 255));
				DX.DrawString(32, 192, seq.eot.ToString(), DX.GetColor(255, 255, 255));
				DX.DrawString(32, 216, seq.bpm.ToString(), DX.GetColor(255, 255, 255));
				DX.DrawString(32, 248, seq.IsPlaying.ToString(), DX.GetColor(255, 255, 255));
				*/
			//Console.WriteLine(nTickCount);
			//Meta Data 解析
			/*

			seq.PlayLoop();
			if (!SeekBar.IsValueChanging)
				SeekBar.Value = seq.mc.GetTickCount();

			//textBox1.Text = debug2;

			//End

			string debug = "[Playing]; [位相]; [音量]; [出力音量]; [音名]; [ピッチベンド]; [ベンド幅] \n";
			int c = 1;

			int idx = 0;

			DX.SetDrawScreen(pianoRoll);
				DX.DrawGraph(-1, 0, pianoRoll, 0);
				DX.DrawLine(sx - mainMonitor.X - 1, 0, sx - mainMonitor.X - 1, sy - mainMonitor.Y, 0);
			int barheight = (isFullScreen ? 6 : 3);

			
			
			foreach (int num in seq.drumtracks)
				if (seq.nTickCount + baseofdrumspeed * bairitu < seq.mfd.Tracks[num].TickLength)
				{
					foreach (MidiEvent evnt in seq.mfd.Tracks[num].GetTickData(btick + baseofdrumspeed * bairitu, seq.nTickCount + baseofdrumspeed * bairitu))
					{
						if (!(evnt is NoteEvent))
							continue;
						NoteEvent ne = (NoteEvent)evnt;

						switch (ne.Note)
						{
							case 36:
								drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus, mainMonitor.Y, "●", Color.Transparent, Color.White), seq.nTickCount));
								break;
							case 38:
								drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 16, mainMonitor.Y , "●", Color.Transparent, Color.White), seq.nTickCount));
								break;
							case 40:
								drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 32, mainMonitor.Y , "●", Color.Transparent, Color.White), seq.nTickCount));
								break;
							case 42:
								drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 32, mainMonitor.Y , "●", Color.Transparent, Color.White), seq.nTickCount));
								break;
							case 46:
								drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 48, mainMonitor.Y , "●", Color.Transparent, Color.White), seq.nTickCount));
								break;
							case 49:
								drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 64, mainMonitor.Y , "●", Color.Transparent, Color.White), seq.nTickCount));
								break;
							case 50:
								drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 80, mainMonitor.Y , "●", Color.Transparent, Color.White), seq.nTickCount));
								break;
							case 48:
								drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 80, mainMonitor.Y , "●", Color.Transparent, Color.White), seq.nTickCount));
								break;
							case 47:
								drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 96, mainMonitor.Y , "●", Color.Transparent, Color.White), seq.nTickCount));
								break;
							case 45:
								drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 96, mainMonitor.Y , "●", Color.Transparent, Color.White), seq.nTickCount));
								break;
							case 43:
								drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 112, mainMonitor.Y , "●", Color.Transparent, Color.White), seq.nTickCount));
								break;
							case 41:
								drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 112, mainMonitor.Y , "●", Color.Transparent, Color.White), seq.nTickCount));
								break;
						}
					}
				}
			if (seq.sm.loop != -1 && seq.eot > 0 && seq.nTickCount + baseofdrumspeed * bairitu >= seq.eot)
			{
				foreach (int num in seq.drumtracks)
					foreach (MidiEvent evnt in seq.mfd.Tracks[num].GetTickData((btick + baseofdrumspeed * bairitu) % seq.eot + seq.sm.loop, (seq.nTickCount + baseofdrumspeed * bairitu) % seq.eot + seq.sm.loop))
				{
					if (!(evnt is NoteEvent))
						continue;
					NoteEvent ne = (NoteEvent)evnt;

					switch (ne.Note)
					{
						case 36:
							drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus, mainMonitor.Y, "●", Color.Transparent, Color.White), (seq.nTickCount + baseofdrumspeed * bairitu) % seq.eot + seq.sm.loop));
							break;
						case 38:
							drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 16, mainMonitor.Y, "●", Color.Transparent, Color.White), (seq.nTickCount + baseofdrumspeed * bairitu) % seq.eot + seq.sm.loop));
							break;
						case 40:
							drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 32, mainMonitor.Y, "●", Color.Transparent, Color.White), (seq.nTickCount + baseofdrumspeed * bairitu) % seq.eot + seq.sm.loop));
							break;
						case 42:
							drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 32, mainMonitor.Y, "●", Color.Transparent, Color.White), (seq.nTickCount + baseofdrumspeed * bairitu) % seq.eot + seq.sm.loop));
							break;
						case 46:
							drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 48, mainMonitor.Y, "●", Color.Transparent, Color.White), (seq.nTickCount + baseofdrumspeed * bairitu) % seq.eot + seq.sm.loop));
							break;
						case 49:
							drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 64, mainMonitor.Y, "●", Color.Transparent, Color.White), (seq.nTickCount + baseofdrumspeed * bairitu) % seq.eot + seq.sm.loop));
							break;
						case 50:
							drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 80, mainMonitor.Y, "●", Color.Transparent, Color.White), (seq.nTickCount + baseofdrumspeed * bairitu) % seq.eot + seq.sm.loop));
							break;
						case 48:
							drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 80, mainMonitor.Y, "●", Color.Transparent, Color.White), (seq.nTickCount + baseofdrumspeed * bairitu) % seq.eot + seq.sm.loop));
							break;
						case 47:
							drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 96, mainMonitor.Y, "●", Color.Transparent, Color.White), (seq.nTickCount + baseofdrumspeed * bairitu) % seq.eot + seq.sm.loop));
							break;
						case 45:
							drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 96, mainMonitor.Y, "●", Color.Transparent, Color.White), (seq.nTickCount + baseofdrumspeed * bairitu) % seq.eot + seq.sm.loop));
							break;
						case 43:
							drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 112, mainMonitor.Y, "●", Color.Transparent, Color.White), (seq.nTickCount + baseofdrumspeed * bairitu) % seq.eot + seq.sm.loop));
							break;
						case 41:
							drumorbs.Add(Tuple.Create(new DXLabel(mainMonitor.X + xplus + 112, mainMonitor.Y, "●", Color.Transparent, Color.White), (seq.nTickCount + baseofdrumspeed * bairitu) % seq.eot + seq.sm.loop));
							break;
					}
				}
			}

				foreach (Dictionary<int, Tone> channel in seq.sm.Tones)
				{

					debug += c + "Channel ---------------\n";
					foreach (KeyValuePair<int, Tone> item in channel)
					{
						debug += string.Format("item{0}: {1}; {2}; {3}; {4}; {5}; {6}; {7}; {8}; {9}; {10}; {11}; {12};\n", idx, item.Value.Envelope.AttackTime, item.Value.Envelope.DecayTime, item.Value.Envelope.SustainLevel, item.Value.Envelope.ReleaseTime, seq.sm.channels[c - 1].inst, item.Value.Playing, (seq.sm.channels[c - 1].panpot - 64) * 4, item.Value.Volume, item.Value.OutVolume, item.Value.Pitch, seq.sm.channels[c - 1].pitchbend, seq.sm.channels[c - 1].bendRange.Data);
						idx++;
							if (!item.Value.isStopping)
								DX.DrawLine(sx - mainMonitor.X - 1, (int)(-(item.Value.noteno - 127) * barheight - seq.sm.channels[c - 1].pitchbend / (227 * (12 / (float)seq.sm.channels[c - 1].bendRange.Data))), sx - mainMonitor.X - 1, (int)(-(item.Value.noteno - 127) * barheight - seq.sm.channels[c - 1].pitchbend / (227 * (12 / (float)seq.sm.channels[c - 1].bendRange.Data))) + barheight, (uint)ColorPallete.GetColorByIndex(c - 1).ToArgb());
							if (item.Value.isNew)
								orbs.Add(new DXOrb(new Point(sx - mainMonitor.X - barheight, (int)(-(item.Value.noteno - 127) * barheight - seq.sm.channels[c - 1].pitchbend / (227 * (12 / (float)seq.sm.channels[c - 1].bendRange.Data)))), new SizeF(barheight, barheight * 2), ColorPallete.GetColorByIndex(c - 1), 1.2f));
					}
					c++;
				}
				
			DX.SetDrawScreen(DX.DX_SCREEN_BACK);

			for (int i = 0; i < drumorbs.Count; i++)
			{
				Tuple<DXLabel, int> item = drumorbs[i];
				item.Item1.Location = new PointF(item.Item1.Location.X, mainMonitor.Y + (seq.nTickCount - item.Item2) / (float)baseofdrumspeed * bairitu * (isFullScreen ? 736 : 448));
				if (item.Item1.Location.Y >= (isFullScreen ? 736 : 448))
				{
					drumorbs.Remove(item);
					continue;
				}

			}

			switch (mode)
			{
				case 0:
					textviewer.Text = debug;
					textviewer.Draw();
					for (int i = 0; i < wav.Length; i++)
						DX.DrawPixel(i >> 1, 256 + (wav[i] >> 9), 0xffffff);
					break;
				case 1:
					DX.DrawGraph(mainMonitor.X, mainMonitor.Y, pianoRoll, 0);
					Queue<DXOrb> tmp = new Queue<DXOrb>();
					foreach (DXOrb orb in orbs)
					{
						if (orb.location.IsEmpty)
							tmp.Enqueue(orb);
						orb.Update();
					}
					while (tmp.Count > 0)
						orbs.Remove(tmp.Dequeue());
					break;
				case 2:
					Gakkyoku1.Draw();
					Gakkyoku2.Draw();
					Gakkyoku3.Draw();
					Gakkyoku4.Draw();
					Gakkyoku5.Draw();
					Gakkyoku6.Draw();
					Gakkyoku7.Draw();
					Gakkyoku8.Draw();
					Gakkyoku9.Draw();
					Gakkyoku10.Draw();
					Gakkyoku11.Draw();
					Gakkyoku12.Draw();
					Gakkyoku13.Draw();
					Gakkyoku14.Draw();

					break;
				case 3:
					foreach(Tuple<DXLabel, int> item in drumorbs)
						item.Item1.Draw();
					break;
			}

			

			Mode1Btn.Draw();
			Mode2Btn.Draw();
			Mode3Btn.Draw();
			Mode4Btn.Draw();
			Play.Draw();
			Pause.Draw();
			Stop.Draw();
			SeekBar.Draw();

			int mx, my;
			DX.GetMousePoint(out mx, out my);
			DX.DrawTriangle(mx, my, mx + 8, my + 24, mx + 24, my + 16, 0xff00ff, 1);

			if (bcopy != seq.copyright)
				DX.SetWindowText(seq.copyright);

			if (btitle != seq.title)
				DX.SetWindowText(seq.title);

			if (blyric != seq.lyrics)
				DX.SetWindowText(seq.lyrics);


			btitle = seq.title;
			bcopy = seq.copyright;
			blyric = seq.lyrics;
			btick2 += seq.nTickCount - btick;

			if (DX.GetNowCount() - bmilisec >= 1000)
			{
				tps = btick2;
				btick2 = 0;
				bmilisec = DX.GetNowCount();
				fps = f;
				f = 0;
			}
			
			//tps = seq.nTickCount - btick;

			//bmilisec = seq.nMillisec;
			f++;
			if (DX.ScreenFlip() == -1)
			{
				DX.DxLib_End();
				seq.mc.Stop();
				return;
			}



			btick = seq.nTickCount;
			DX.WaitTimer(1);


		}
			 */
		}

		private static void dxform_MouseWheel(object sender, MouseEventArgs e)
		{
			Console.WriteLine("debug");
			_textviewer.Location = new PointF(_textviewer.Location.X,
				_textviewer.Location.Y + e.Delta * SystemInformation.MouseWheelScrollLines / 120);
		}

		private static void dxform_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				//ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
				e.Effect = DragDropEffects.Copy;
			else
				//ファイル以外は受け付けない
				e.Effect = DragDropEffects.None;
		}

		private static void dxform_DragDrop(object sender, DragEventArgs e)
		{
			//コントロール内にドロップされたとき実行される
			//ドロップされたすべてのファイル名を取得する
			var fileName =
				(string[]) e.Data.GetData(DataFormats.FileDrop, false);
			//ListBoxに追加する
			if (fileName.Length > 1)
				MessageBox.Show("複数のファイルがドロップされました。\r\n先頭のファイルのみ再生を開始します");
			_file = fileName[0];
			_playRequest = true;
		}


		public static int GetBpm(byte b1, byte b2, byte b3)
		{
			return 60 * 1000000 / ((b1 << 16) + (b2 << 8) + b3);
		}

		/// <summary>
		///     DXLibを終了してエラーメッセージを表示します。
		/// </summary>
		/// <param name="text"></param>
		public static void ShowError(string text)
		{
			DX.DxLib_End();
			MessageBox.Show(string.Format(@"実行中にエラーが発生しました。
エラー内容: {0}", text), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
		}

		/*
		static int CreateFM(out int[] wav)
		{

			int rethndl = 0;
			
			int imax = 44100;
			int hSSnd = DX.MakeSoftSound1Ch16Bit44KHz(imax);
			int hz = 440;
			wav = new int[imax];
			for (int i = 0; i < imax; i++)
			{
				Func<int, double> siki = new Func<int, double>((hzparam) =>
					{
						return Math.PI * 2 * hzparam / 44100 * i;
					});
				int fm = (int)(Math.Sin(siki(220) + 4 * Math.Sin(siki(440))) * 16384 + 16384);
				wav[i] = fm;
				DX.WriteSoftSoundData(hSSnd, i, (ushort)fm, (ushort)fm);
				
			}
			if (imax < 1000)
			Console.WriteLine("wave: {0}", string.Join(", ", wav));
			rethndl = DX.LoadSoundMemFromSoftSound(hSSnd);

			DX.DeleteSoftSound(hSSnd);

			return rethndl;

		}
		
		static int GetFMWave(int time, int algorithm, int feedback, int lastwave, FMWaveData module1, FMWaveData module2, FMWaveData module3, FMWaveData module4)
		{
			int[] waves = new int[]
			{
				(int)(Math.Sin(Math.PI * 2 * module1.freq / 44100 * time) * module1.amplitude),
				(int)(Math.Sin(Math.PI * 2 * module2.freq / 44100 * time) * module2.amplitude),
				(int)(Math.Sin(Math.PI * 2 * module3.freq / 44100 * time) * module3.amplitude),
				(int)(Math.Sin(Math.PI * 2 * module4.freq / 44100 * time) * module4.amplitude)
			};
			switch(algorithm)
			{
				case 1:
					
					break;
				case 2:

					break;
				case 3:

					break;
				case 4:

					break;
				case 5:

					break;
				case 6:

					break;
				case 7:

					break;
				case 8:

					break;

			}
			
		}*/
	}
}