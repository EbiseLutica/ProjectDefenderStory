using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DxLibDLL;
using MusicSheet.Sequence;
using MusicSheet.Mssf;
using System.Threading;


namespace MusicSheetMidiSequencer
{
	public partial class MainWindow : Form
	{
		Sequencer _seq;
		SoundModule _sndmodule;
		bool _midiInIsOn = false;
		Random _rand = new Random(1145141919);
		public MainWindow()
			: this(null) { }

		public MainWindow(string path)
		{
			InitializeComponent();
			DX.ChangeWindowMode(1);
			DX.SetWaitVSyncFlag(0);
			DX.SetWindowVisibleFlag(0);
			DX.SetAlwaysRunFlag(1);
			if (DX.DxLib_Init() == -1)
			{
				DX.DxLib_End();
				Console.WriteLine("[DEBUG]DirectX の初期化に失敗しました。");
				Application.Exit();
			}
			_seq = new Sequencer();
			_sndmodule = new SoundModule();
			//MessageBox.Show(System.IO.Path.GetDirectoryName(Application.ExecutablePath));
			backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
			//backgroundWorker1.RunWorkerAsync();
			timer1.Start();
			if (path != null)
			{
				Load(path);
				Play();
			}
				
		}

		void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			_closerequest = false;
		}



		public bool PlayRequest;
		public string File = "";

		/// <summary>
		/// 再生リクエストが送られたかどうかを判定し、ある場合は指定した文字列変数にファイル名を代入します。
		/// </summary>
		/// <param name="data">代入する String 変数。</param>
		/// <returns>リクエストが送られたかどうか。</returns>
		public bool GetPlayRequest(ref string data)
		{
			if (PlayRequest)
				data = File;
			var p = PlayRequest;
			PlayRequest = false;
			return p;
		}
		bool _isPausing;
		private void button3_Click(object sender, EventArgs e)
		{
			Stop();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Pause();
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			if (_seq.IsPlaying)
				return;
			Play();
		}



		private void バージョン情報ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show(
@"Music Sheet Player ver 1.0.0
Music Sheet v1.1.0
(C)2014-2015 Citringo"
			);
		}

		private void 公式サイトToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://citringo.net");
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			if (DX.CheckHitKey(DX.KEY_INPUT_LSHIFT) == 1)
				contextMenuStrip2.Show(MousePosition.X, MousePosition.Y);
			else
				contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
		}

		private void MainWindow_DragEnter(object sender, DragEventArgs e)
		{
			if (_midiInIsOn)
				return;
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				//ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
				e.Effect = DragDropEffects.Copy;
			else
				//ファイル以外は受け付けない
				e.Effect = DragDropEffects.None;
		}

		private void MainWindow_DragDrop(object sender, DragEventArgs e)
		{
			if (_midiInIsOn)
				return;
			//コントロール内にドロップされたとき実行される
			//ドロップされたすべてのファイル名を取得する
			var fileName =
				(string[])e.Data.GetData(DataFormats.FileDrop, false);
			//ListBoxに追加する
			if (fileName.Length > 1)
				MessageBox.Show("複数のファイルがドロップされました。\r\n先頭のファイルのみ再生を開始します");
			File = fileName[0];
			WorkerCancel();
			Load(File);
			Play();
			
		}
		bool _locked;

		private new void Load(string file)
		{
			try
			{
				_seq.Load(file);
			}
			catch (InvalidOperationException)
			{
				MessageBox.Show("SMF の読み込み中にエラーが発生しました！", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
			catch (NullReferenceException)
			{
				MessageBox.Show("SMF の読み込み中にエラーが発生しました！\n不正な SMF ファイルです。", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}


		private void Play()
		{
			if (_midiInIsOn)
				return;
			
			WorkerCancel();
			_bugged = false;
			if (_isPausing)
				_seq.Resume();
			else
			{
				_seq.Reset();
				_seq.Play();
			}
			backgroundWorker1.RunWorkerAsync();
			trackBar1.Value = 0;
			trackBar1.Minimum = 0;
			trackBar1.Maximum = _seq.Eot;
			trackBar1.LargeChange = _seq.Eot / 10;
		}

		private void Pause()
		{
			if (_midiInIsOn)
				return;
			WorkerCancel();
			_isPausing = true;
			_seq.Stop();
			backgroundWorker1.RunWorkerAsync();
		}

		private void Stop()
		{
			if (_midiInIsOn)
				return;
			WorkerCancel();
			_isPausing = false;
			_seq.Stop();
			backgroundWorker1.RunWorkerAsync();
		}


		private void mssf波形一覧ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new WaveList().Show();
		}

		SeqInfo _info = SeqInfo.Title;

		private void label4_Click(object sender, EventArgs e)
		{
			var i = (int)_info;
			i--;
			if (i < 0)
				i = 2;
			_info = (SeqInfo)i;
			switch (_info)
			{
				case SeqInfo.Title:
					label1.Text = _seq.Title;
					break;
				case SeqInfo.Copyright:
					label1.Text = _seq.Copyright;
					break;
				case SeqInfo.Lyric:
					label1.Text = _seq.Lyrics;
					break;
			}
		}

		private void label4_MouseEnter(object sender, EventArgs e)
		{
			((Label)sender).ForeColor = Color.White;
		}

		private void label4_MouseLeave(object sender, EventArgs e)
		{
			((Label)sender).ForeColor = Color.Black;
		}

		private void label3_Click(object sender, EventArgs e)
		{
			var i = (int)_info;
			i++;
			if (i > 2)
				i = 0;

		}

		private void label4_MouseDown(object sender, MouseEventArgs e)
		{
			((Label)sender).Font = new Font(((Label)sender).Font.FontFamily, 7);
		}

		private void label4_MouseUp(object sender, MouseEventArgs e)
		{
			((Label)sender).Font = new Font(((Label)sender).Font.FontFamily, 9);
			if (!_seq.IsLoaded)
			{
				label1.Text = "No Standard Midi File";
				return;
			}
			var i = (int)_info;
			if (sender == label4)
			{
				i--;
				if (i < 0)
					i = 2;
			}
			else
			{
				i++;
				if (i > 2)
					i = 0;
			}
			_info = (SeqInfo)i;
		}

		bool _closerequest;

		private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			timer1.Stop();
			WorkerCancel();
			DX.DxLib_End();

		}

		private void WorkerCancel()
		{
			backgroundWorker1.CancelAsync();
			while (backgroundWorker1.IsBusy)
				Application.DoEvents();
		}

		bool _bugged;

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (_locked)
				return;
			_locked = true;
			if (!_midiInIsOn)
			{
				var pos = _seq.Position;
				label2.Text = string.Format("{0} : {1} : {2}", pos.Measure, pos.Beat, pos.Tick);
				if (!_seq.IsLoaded)
					label1.Text = "No Standard Midi File";
				else
					switch (_info)
					{
						case SeqInfo.Title:
							label1.Text = _seq.CurrentTitle;
							break;
						case SeqInfo.Copyright:
							label1.Text = _seq.CurrentCopyright;
							break;
						case SeqInfo.Lyric:
							label1.Text = _seq.CurrentLyric;
							break;
					}

				trackBar1.Value = Math.Min(_seq.Eot, _seq.NTickCount);
			}

			pictureBox2.Refresh();



			if (_seq.IsLoaded && _bug)
			{
				if (_rand.Next(34) == 0 && !_bugged)
				{
					_bugged = true;
					for (var i = 0; i < _rand.Next(5) + 4; i++)
						switch (_rand.Next(4))
						{
							case 0:
								_seq.Sm.Channels[_rand.Next(_seq.MaxChannel)].Volume = (byte)(_rand.Next(64) + 64);
								break;
							case 1:
								_seq.Sm.Channels[_rand.Next(_seq.MaxChannel)].Pitchbend = _rand.Next(16384) - 8192;
								break;
							case 2:
								_seq.Sm.Channels[_rand.Next(_seq.MaxChannel)].Expression = (byte)(_rand.Next(64) + 64);
								break;
							case 3:
								_seq.Sm.Channels[_rand.Next(_seq.MaxChannel)].Inst = (byte)_rand.Next(128);
								break;
						}
				}
				if (_rand.Next(32) == 0)
				{
					_bugged = false;
				}
			}
			_locked = false;
		}


		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			WorkerCancel();
			_seq.Sm.Panic();
			_seq.Btick = trackBar1.Value;
			_seq.Mc.TickCount = trackBar1.Value;
			backgroundWorker1.RunWorkerAsync();
		}
		bool _workerlocked;
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{

			while (true)
			{
				
				if (backgroundWorker1.CancellationPending)
				{
					e.Cancel = true;
					_closerequest = false;
					break;
				}
				_workerlocked = true;
				try
				{
					_seq.PlayLoop();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
				_workerlocked = false;
				DX.WaitVSync(1);
			}
		}

		bool _bug;
		private void netcitringomusicSheetةڼةڼةڼةڼةڼToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!_bug)
			{
				MessageBox.Show("重大な障害が発生したため、このプログラムをしゅうｒｙ└(՞ةڼ◔)」ｷｴｴｴｴｴﾇﾍﾞﾁﾞｮﾝﾇｿﾞﾁﾞｮﾝﾍﾞﾙﾒｯﾃｨｽﾓｹﾞﾛﾝﾎﾞｮwwwwww", "v(՞ةڼ◔)vﾋｨｨｨwwwｲﾋｨｨｨｨｨｨwwwwwwww", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				Text = "(՞ةڼ◔՞ةڼ◔՞ةڼ◔՞ةڼ◔՞ةڼ◔՞ةڼ◔՞ةڼ◔՞ةڼ◔՞ةڼ◔)";
				ReplaceToKichigai(Controls);
				_bug = true;
				_bugged = false;
			}
			else
			{
				MessageBox.Show("異常なプログラム書き換えを検出したため、修復を行います。", "バグじゃなくてハグをください 誰がハゲじゃハゲ", MessageBoxButtons.OK, MessageBoxIcon.Information);
				Application.Restart();
			}
		}

		/// <summary>
		/// 指定した ControlCollection に含まれる要素がキチガイになります。
		/// </summary>
		/// <param name="cs">キチらせる ControlCollection。</param>
		void ReplaceToKichigai(Control.ControlCollection cs)
		{
			foreach (Control c in cs)
			{
				if (c is Panel)
					ReplaceToKichigai((c as Panel).Controls);
				c.Text = "(՞ةڼ◔՞ةڼ◔՞ةڼ◔՞ةڼ◔՞ةڼ◔՞ةڼ◔՞ةڼ◔՞ةڼ◔՞ةڼ◔)";
			}
		}

		private void pictureBox2_Paint(object sender, PaintEventArgs e)
		{
			var x = 64;
			var y = 16;
			var g = e.Graphics;

			g.DrawString("Volume", new Font(FontFamily.GenericMonospace, 7), Brushes.Black, 2, 24);
			g.DrawString("Expression", new Font(FontFamily.GenericMonospace, 7), Brushes.Black, 2, 36);
			g.DrawString("Envelope", new Font(FontFamily.GenericMonospace, 7), Brushes.Black, 2, 48);
			g.DrawString("Output", new Font(FontFamily.GenericMonospace, 7), Brushes.Black, 2, 66);
			g.DrawString("Panpot", new Font(FontFamily.GenericMonospace, 7), Brushes.Black, 2, 94);
			g.DrawString("PitchBend", new Font(FontFamily.GenericMonospace, 7), Brushes.Black, 2, 104);
			g.DrawString("P.Change", new Font(FontFamily.GenericMonospace, 7), Brushes.Black, 2, 116);
			g.DrawString("Frequency", new Font(FontFamily.GenericMonospace, 7), Brushes.Black, 2, 126);
			var cnt = 0;
			foreach (var ch in _seq.Sm.Channels)
			{
				DrawChannels(x, y, ch, g, cnt);
				x += 34;
				cnt++;
			}
		}

		private void DrawChannels(int x, int y, Channel ch, Graphics g, int chcnt)
		{
			//g.DrawRectangle(Pens.Black, x, y, 32, 126);

			g.DrawString("V", new Font(FontFamily.GenericMonospace, 7), Brushes.Black, x + 1, y + 2);
			g.DrawRectangle(Pens.Black, x + 2, y + 12, 6, 64);
			g.DrawString("E", new Font(FontFamily.GenericMonospace, 7), Brushes.Black, x + 7, y + 2);
			g.DrawRectangle(Pens.Black, x + 8, y + 12, 6, 64);
			g.DrawString("N", new Font(FontFamily.GenericMonospace, 7), Brushes.Black, x + 13, y + 2);
			g.DrawRectangle(Pens.Black, x + 14, y + 12, 6, 64);
			g.DrawString("O", new Font(FontFamily.GenericMonospace, 7), Brushes.Black, x + 19, y + 2);
			g.DrawRectangle(Pens.Black, x + 20, y + 12, 10, 64);

			g.DrawRectangle(Pens.Black, x + 2, y + 78, 28, 8);
			g.DrawRectangle(Pens.Black, x + 2, y + 88, 28, 8);

			g.DrawString(string.Format("{0, 5}", ch.Inst), new Font(FontFamily.GenericMonospace, 7), Brushes.Black, x, y + 98);

			g.FillRectangle(Brushes.Lime, x + 3, y + 13 + (64 - 64 * (ch.Volume / 127f)), 5, 64 * (ch.Volume / 127f) - 1);
			g.FillRectangle(Brushes.Lime, x + 9, y + 13 + (64 - 64 * (ch.Expression / 127f)), 5, 64 * (ch.Expression / 127f) - 1);

			g.FillRectangle(Brushes.Lime, GetRectBy4Point(x + 16, y + 79, (int)(x + 3 + ch.Panpot * (26 / 127f)), y + 85));
			g.FillRectangle(Brushes.Lime, GetRectBy4Point(x + 16, y + 89, (int)(x + 16 + (ch.Pitchbend * (13 / 8192f))), y + 95));

			for (var i = y + 14; i < y + 14 + 62; i += 2)
			{
				g.DrawLine(Pens.Black, x + 3, i, x + 7, i);
				g.DrawLine(Pens.Black, x + 9, i, x + 13, i);
			}

			Tone lt;
			if (_seq.Sm.LastTone[chcnt] != null)
			{
				lt = (Tone)_seq.Sm.LastTone[chcnt].Clone();

				var a = (lt.OutVolume * (ch.Volume / 127f) * (ch.Expression / 127f) * (lt.Velocity / 127f)) / 255f;

				g.FillRectangle(Brushes.Lime, x + 15, y + 13 + (64 - 64 * (lt.OutVolume / 255f)), 5, 64 * (lt.OutVolume / 255f) - 1);

				g.FillRectangle(Brushes.Lime, x + 21, y + 13 + (64 - 64 * a), 9, 64 * a - 1);

				g.DrawString(string.Format("{0, 5}", (int)(lt.Freq * Math.Pow(2, (ch.Pitchbend / 8192.0) * (ch.BendRange.Data / 12.0)) * Math.Pow(2, (ch.Tweak.Data / 8192f) * (2 / 12f)) * Math.Pow(2, ch.NoteShift.Data / 12f))), new Font(FontFamily.GenericMonospace, 7), Brushes.Black, x + 1, y + 108);
			}

			for (var i = y + 14; i < y + 14 + 62; i += 2)
			{
				g.DrawLine(Pens.Black, x + 15, i, x + 19, i);
				g.DrawLine(Pens.Black, x + 21, i, x + 30, i);
			}

			//(int)(tone.Value.OutVolume * (channels[i].volume / 127.0) * (channels[i].expression / 127.0) * (tone.Value.Velocity / 127.0))
		}

		static Rectangle GetRectBy4Point(int x0, int y0, int x1, int y1)
		{
			var last = new Rectangle();

			int x, y, w, h;
			w = x1 - x0; x = x0;
			if (w < 0)
			{
				w = -w + 1;
				x = x0 - w + 1;
			}
			else w++;

			h = y1 - y0; y = y0;
			if (h < 0)
			{
				h = -h + 1;
				y = y0 - h + 1;
			}
			else h++;

			last.X = x; last.Y = y; last.Width = w; last.Height = h;
			return last;
		}

	}

	enum SeqInfo
	{
		Title, Lyric, Copyright
	}

}
