using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextMidi.DataElement;
using MusicSheet.Mssf;
using NextMidi.Filing.Midi;
using NextMidi.Data;
using NextMidi.DataElement.MetaData;
using DxLibDLL;
using System.Windows.Forms;
using NextMidi.Time;
using NextMidi.Data.Score;

namespace MusicSheet.Sequence
{

	public enum DrumFlag
	{
		Kick, Snare, HihatClose, HihatOpen, Cymbal, Tom1, Tom2, Tom3
	}

	public class MidiClock
	{
		public int BPM
		{
			get; set;
		}
		public int Resolution
		{
			get; set;
		}

		int btime;
		

		public MidiClock(int bpm, int resolution)
		{
			BPM = bpm;
			Resolution = resolution;

		}

		float _tick;
		int _mili;

		public int TickCount
		{
			get
			{
				int ret;
				if (IsRunning)
					ret = (int)(_tick += TempoMap.GetTickLength(DX.GetNowCount() - btime, BPM, Resolution));
				else
					ret = (int)_tick;
				btime = DX.GetNowCount();
				
				return ret;

			}
			set
			{
				this._tick = value;
			}
		}

		public int MiliSec
		{
			get
			{
				return _mili += 1000 / Fps;
			}
		}

		public bool IsRunning
		{
			get; set;
		}

		public void Start()
		{
			btime = DX.GetNowCount();
			IsRunning = true;
		}

		public void Stop()
		{
			IsRunning = false;

		}

		public void Reset()
		{
			_mili = (int)(_tick = 0);
		}

		public int Fps
		{
			get; set;
		} = 60;

		



	}

	public class Sequencer
	{
		public SoundModule sm;
		public MidiData mfd;
		public MidiClock mc;
		public int nMillisec, nTickCount, btick, bpm;
		public int eot = 0;
		public List<int> drumtracks = new List<int>();
		public string title, copyright, lyrics;
		public int loop = -1;
		public bool IsPlaying { get; private set; }
		public int MaxChannel { get; private set; }

		public List<MetaEvent> metas = new List<MetaEvent>();

		public static MidiData LoadSMF(string filename)
		{
			MidiReader mfr = new MidiReader(new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read), Encoding.Default);
			var m = mfr.ReadFile();
			mfr.Close();
			return m;
		}

		public Sequencer()
		{
			this.Reset();
		
		}

		public Sequencer(MidiData md)
			: this()
		{
			SetMidiData(md);
		}
		TempoMap tm;
		public int MusicTime
		{
			get
			{
				return tm?.ToMilliSeconds(eot) ?? 0;
			}
		}

		public void SetMidiData(MidiData md)
		{
			mfd = md;
			tm = new TempoMap(mfd);
			this.Stop();
			this.Reset();
			eot = 0;
			drumtracks = new List<int>();
			if (mfd != null)
			{
				mc = new MidiClock(120, mfd.Resolution.Resolution);
			}
			int cnt = 0;
			foreach (NextMidi.Data.Track.MidiTrack lst in mfd.Tracks)
			{
				if (lst.Channel.GetValueOrDefault() == 9 && !drumtracks.Contains(cnt))
					drumtracks.Add(cnt);

				if (eot < (lst.TickLength))
					eot = lst.TickLength;
				cnt++;
			}
			
			loaded = true;
		}


		public int CurrentBPM
		{
			get
			{
				var a = metas.FindLast(new Predicate<MetaEvent>((te) => te is TempoEvent && te.Tick <= nTickCount)) as TempoEvent;
				if (a != null)
					return a.Tempo;
				return 120;
			}
		}

		public RhythmEvent CurrentRhythm
		{
			get
			{
				return metas.FindLast(new Predicate<MetaEvent>((te) => te is RhythmEvent && te.Tick <= nTickCount)) as RhythmEvent;
			}
		}

		public string CurrentTitle
		{
			get
			{
				var a = metas.FindLast(new Predicate<MetaEvent>((te) => te is MidiTrackTitle && te.Tick <= nTickCount)) as MidiTrackTitle;
				if (a != null)
					return a.Text;
				return "";
			}
		}

		public string CurrentCopyright
		{
			get
			{
				var a = metas.FindLast(new Predicate<MetaEvent>((te) => te is MidiCopyright && te.Tick <= nTickCount)) as MidiCopyright;
				if (a != null)
					return a.Text;
				return "";
			}
		}

		public string CurrentLyric
		{
			get
			{
				var a = metas.FindLast(new Predicate<MetaEvent>((te) => te is MidiLyric && te.Tick <= nTickCount)) as MidiLyric;
				if (a != null)
					return a.Text;
				return "";
			}
		}
		public SMFPosition Position
		{
			get
			{
				Queue<RhythmEvent> qre = new Queue<RhythmEvent>(metas.FindAll(new Predicate<MetaEvent>((te) => te is RhythmEvent)).Cast<RhythmEvent>());
				SMFPosition last = new SMFPosition();
				last.Measure = 1;
				last.Beat = 1;

				RhythmEvent tmp = new RhythmEvent(4, 4);
				for (int i = 0; i < nTickCount; i++)
				{
					if (last.Tick >= 4f / tmp.Note * mfd.Resolution.Resolution)
					{
						last.Beat++;
						last.Tick = 0;
					}
					if (last.Beat > tmp.Rhythm)
					{
						last.Measure++;
						last.Beat = 1;
					}

					if (qre.Count > 0 && qre.Peek().Tick <= i)
					{
						tmp = qre.Dequeue();
					}

					
					last.Tick++;
				}

				return last;
			}
		}

		public void Reset()
		{
			if (sm != null)
				sm.Panic();
			
			
			sm = new SoundModule();
			
			MaxChannel = 0;
			bpm = 120;
			btick = -1;
			loop = -1;
			title = copyright = lyrics = "";
		}

		bool loaded = false;

		public bool IsLoaded
		{
			get
			{
				return loaded;
			}
		}

		public void Load(string filename)
		{
			Stop();
			Reset();
			SetMidiData(LoadSMF(filename));
		}

		public void Play()
		{
			if (!loaded)
				return;

			metas.Clear();
			var cnt = 0;
			foreach (NextMidi.Data.Track.MidiTrack mt in mfd.Tracks)
			{
				if (mt.Channel != null && mt.Channel != 9 && MaxChannel < mt.Channel)
					MaxChannel = (int)mt.Channel;
				foreach (NextMidi.DataElement.MidiEvent me in mt.GetData())
				{
					if (me is MetaEvent)
						if (me is MidiTrackTitle && cnt == 0 || !(me is MidiTrackTitle))
							metas.Add(me as MetaEvent);
					if (me is ControlEvent && ((ControlEvent)me).Number == 111)
						loop = me.Tick;
				}
				cnt++;
			}
			mc.Reset();
			mc.Start();
			IsPlaying = true;
		}

		public void Resume()
		{
			if (!loaded)
				return;
			mc.Start();
			IsPlaying = true;
		}

		public void Stop()
		{
			if (!loaded)
				return;
			if (mc != null)
				sm.Panic();
			mc.Stop();
			IsPlaying = false;
		}


		public void	PlayLoop()
		{
			//Console.WriteLine("mc:{0}", mc.IsRunning());
			if (!mc.IsRunning)
				return;
			nMillisec = mc.MiliSec;
			nTickCount = mc.TickCount;
			mc.BPM = bpm = CurrentBPM;


			//List<MidiEvent> mes = mfd.Tracks[2].GetData();
			//string debug2 = "";
			for (int j = 1; j < mfd.Tracks.Count; j++)
			{
				//int hoge = mfd.Tracks[j].Channel.GetValueOrDefault();
				//debug2 += hoge + " ";

				NextMidi.Data.Track.MidiTrack mt = mfd.Tracks[j];
				if (nTickCount <= mt.TickLength)
				{

					foreach (MidiEvent me in mt.GetTickData(btick + 1, nTickCount + 1))
					{
						//	try
						//	{
						sm.SendEvent(me, mt.Channel, nTickCount, ref mc);


						if (me is MidiLyric)
						{
							MidiLyric ml = me as MidiLyric;
							lyrics = ml.Text;
						}
					}
					//	}
					//	catch (Exception ex)
					//	{
					//		Console.WriteLine(ex);
					//	}
				}
			}

			

			sm.PlayTones(nTickCount);

			if (nTickCount > eot)
			{
				sm.Panic();
				if (loop == -1)
				{
					this.IsPlaying = false;
					mc.Stop();
				}
				else
				{
					mc.TickCount = loop;
					nTickCount = loop - 1;
				}
			}
			

			btick = nTickCount;
		}
		



	}


	public class SoundModule
	{
		public Dictionary<int, Tone>[] Tones { get; set; }

		Mssf[] insts = new Mssf[128];

		public Tone[] bTone = new Tone[16];

		public Tone nowTone = null;

		public int bms = 0;

		public Tone[] LastTone { get; private set; }

		public Channel[] channels = new Channel[16];

		public int Volume
		{
			get
			{
				return volume;
			}
			set
			{
				if (value < 0 || value > 100)
					throw new ArgumentOutOfRangeException("value");
				volume = value;
			}
		}

		int volume = 100;

		public int loop = -1;
		Queue<Tone> tonequeue = new Queue<Tone>(4);
		public int portamenttick = 0;

		public int[] hPercs = new int[128];

		public SoundModule()
		{
			Tones = new Dictionary<int, Tone>[16];
			LastTone = new Tone[16];
			for (int i = 0; i < 16; i++)
			{
				Tones[i] = new Dictionary<int, Tone>();
				if (i != 10)
					channels[i] = new Channel(0, false, 64, 100, 127, 0, 0, new RPN(0), new RPN(0), new RPN(2));
				else
					channels[i] = new Channel(0, true, 64, 100, 127, 0, 0, new RPN(0), new RPN(0), new RPN(2));

			}


			for (int i = 0; i < 128; i++)
			{
				int a = 0, d = 0, r = 0, pan = 0;
				byte s = 0;
				NoiseOption no;
				short[] wave = null;
				if (System.IO.File.Exists(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\Insts\\" + i + ".mssf"))
				{
					MssfUtility.LoadFileDynamic(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\Insts\\" + i + ".mssf", out wave, out a, out d, out s, out r, out pan, out no);
					insts[i] = new Mssf(wave, a, d, s, r, pan, no);
				}
				else
				{
					insts[i] = Mssf.Empty;
				}


				if (System.IO.File.Exists(string.Format(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\Drums\\{0}.wav", i)))
					hPercs[i] = DX.LoadSoundMem(string.Format(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\Drums\\{0}.wav", i));
				else
					hPercs[i] = 0;

			}
		}

		~SoundModule()
		{
			foreach (int hoge in hPercs)
			{
				DX.DeleteSoundMem(hoge);
			}

		}

		ControlEvent[] rpns = new ControlEvent[4];

		public void Panic()
		{
			foreach (Dictionary<int, Tone> tones in Tones)
				foreach (Tone tone in tones.Values)
				{
					tone.Abort();
				}
		}

		public void SendEvent(MidiEvent me, byte? channel, int miditick, ref MidiClock cmc)
		{
			if (channel == null)
				return;
			Mssf m = insts[channels[channel.GetValueOrDefault()].inst];
			if (m.wave == null && channel.GetValueOrDefault() != 9)

				return;
			
			//ノートオン
			if (miditick != -1)
			{
				if (me is NoteEvent)
				{
					//Tone a;
					NoteEvent ne = (NoteEvent)me;

					if (ne.Note < 120 && ne.Note > 11 && ne.Channel.Value != 9)
					{
						if (ne.Velocity == 0)
						{
							Tones[(int)channel][ne.Note].Stop();
						}
						if (Tones[(int)channel].ContainsKey(ne.Note))
						{
							Tones[(int)channel][ne.Note].Abort();
							Tones[(int)channel].Remove(ne.Note);
						}
						bTone[ne.Channel.Value] = nowTone;
						Tone t = new Tone(pitchnames[ne.Note % 12], (ne.Note - 12) / 12, m.wave, new Envelope(m.a, m.d, m.s, m.r), 255, 0, ne.Velocity, m.noiseoption);
						LastTone[(byte)channel] = t;
						Tones[(int)channel].Add(ne.Note, t);
						Tones[(int)channel][ne.Note].StartPlay(miditick, ne.Gate);
						tonequeue.Enqueue(t);

						nowTone = t;
						portamenttick = 0;
						bms = DX.GetNowCount();
					}
					else
					{
						DX.PlaySoundMem(hPercs[ne.Note], DX.DX_PLAYTYPE_BACK);
						foreach (int handle in hPercs)
						{
							if (handle == 0)
								continue;
							DX.ChangePanSoundMem((channels[9].panpot - 64) * 4, hPercs[ne.Note]);
							DX.ChangeVolumeSoundMem((int)(255 * (channels[9].volume / 127.0) * (channels[9].expression / 127.0) * (ne.Velocity / 127.0) * (Volume / 100f)), hPercs[ne.Note]);
						}
					}
				}
			}
			else
			{
				if (me is NoteOnEvent)
				{
					//Tone a;
					NoteOnEvent ne = (NoteOnEvent)me;
					if (ne.Note < 120 && ne.Note > 11 && ne.Channel.Value != 9)
					{

						if (ne.Velocity == 0)
						{
							Tones[(int)channel][ne.Note].Stop();
						}
						if (Tones[(int)channel].ContainsKey(ne.Note))
						{
							Tones[(int)channel][ne.Note].Abort();
							Tones[(int)channel].Remove(ne.Note);
						}
						Tone t = new Tone(pitchnames[ne.Note % 12], (ne.Note - 12) / 12, m.wave, new Envelope(m.a, m.d, m.s, m.r), 255, 0, ne.Velocity, m.noiseoption);
						LastTone[(byte)channel] = t;
						Tones[(int)channel].Add(ne.Note, t);
						Tones[(int)channel][ne.Note].StartPlay(-1, -1);
						tonequeue.Enqueue(t);
					}
					else
					{
						DX.PlaySoundMem(hPercs[ne.Note], DX.DX_PLAYTYPE_BACK);
						foreach (int handle in hPercs)
						{
							if (handle == 0)
								continue;
							DX.ChangePanSoundMem((channels[9].panpot - 64) * 4, hPercs[ne.Note]);
							DX.ChangeVolumeSoundMem((int)(255 * (channels[9].volume / 127.0) * (channels[9].expression / 127.0) * (ne.Velocity / 127.0)), hPercs[ne.Note]);
						}
					}
				}
				//ノートオフ
				if (me is NoteOffEvent)
				{
					NoteOffEvent noe = (NoteOffEvent)me;
					Tones[(int)channel][noe.Note].Stop();
				}
			}

			if (me is ProgramEvent)
			{
				channels[(int)channel].inst = ((ProgramEvent)me).Value;
			}

			if (me is ControlEvent)
			{
				ControlEvent ce = (ControlEvent)me;
				switch (ce.Number)
				{
					case 10:
						channels[(int)channel].panpot = ce.Value;
						break;
					case 7:
						channels[(int)channel].volume = ce.Value;
						break;
					case 11:
						channels[(int)channel].expression = ce.Value;
						break;
					case 101:
						rpns[0] = ce;
						break;
					case 100:
						rpns[1] = ce;
						break;
					case 6:
						rpns[2] = ce;
						if (rpns[1] == null)
							break;
						switch (rpns[1].Value)
						{
							case 0:
								channels[(int)channel].bendRange = new RPN(rpns[2].Value);
								break;

							case 2:
								channels[(int)channel].noteShift = new RPN((short)(rpns[2].Value - 64));
								break;

						}
						break;
					case 38:
						rpns[3] = ce;
						if (rpns[1] == null || rpns[2] == null)
							break;
						if (rpns[1].Value == 1)
						{
							channels[(int)channel].tweak = new RPN((short)(rpns[2].Value * 128 + rpns[3].Value - 8192));
						}
						break;
					case 111:
						loop = ce.Tick;
						break;
					case 65:
						channels[(int)channel].portament = ce.Value;
						break;
					case 5:
						channels[(int)channel].portamentTime = ce.Value;
						break;
				}
			}

			if (me is PitchEvent)
			{
				byte[] native = me.ToNativeEvent();
				int pitchdata = native[2] * 128 + native[1] - 8192;

				channels[(int)channel].pitchbend = pitchdata;

				//Console.WriteLine("Decimal: " + pe.Value + "Hexa: " + pe.Value.ToString("X2") + "Binary: " + Convert.ToString(2, pe.Value));
			}

			if (cmc != null && me is NextMidi.DataElement.MetaData.MidiEndOfTrack)
			{

				channels[me.Channel.GetValueOrDefault()].end = true;
				bool allend = true;
				foreach (Channel c in channels)
					if (!c.end)
					{
						allend = false;
						break;
					}
				if (allend)
				{
					if (loop == -1)
						cmc.Stop();
					else
						cmc.TickCount = loop;
				}

			}
		}



		public string[] pitchnames = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

		int bmilisec = DX.GetNowCount();

		public void PlayTones(int miditick)
		{
			//(Dictionary<int, Tone> channel in Tones)
			for (int i = 0; i < Tones.Length; i++)
			{
				Dictionary<int, Tone> channel = Tones[i];
				//bool canDelete = false;
				//foreach (KeyValuePair<int, Tone> tone in channel)
				KeyValuePair<int, Tone>[] tones = channel.ToArray();
				for (int j = 0; j < channel.Count; j++)
				{
					KeyValuePair<int, Tone> tone = tones[j];

					tone.Value.PlayLoop(miditick);
					DX.ChangePanSoundMem((channels[i].panpot - 64) * 4, tone.Value.Handle);
					DX.ChangeVolumeSoundMem((int)(tone.Value.OutVolume * (channels[i].volume / 127.0) * (channels[i].expression / 127.0) * (tone.Value.Velocity / 127.0) * (Volume / 100f)), tone.Value.Handle);
					//DX.SetFrequencySoundMem(44100 + (int)(channels[i].pitchbend * ((44100 * (channels[i].bendRange.Data / 12.0)) / 8192.0)), tone.Value.Handle);
					int a = (int)
						(
						((tone.Value.Octave > 6) ? (tone.Value.GetFreq(tone.Value.Pitch, tone.Value.Octave - 2) * 100) : (tone.Value.GetFreq(tone.Value.Pitch, 4) * 100))
						* Math.Pow(2, (channels[i].pitchbend / 8192.0) * (channels[i].bendRange.Data / 12.0)) * Math.Pow(2, (channels[i].tweak.Data / 8192f) * (2 / 12f)) * Math.Pow(2, (channels[i].noteShift.Data / 12f))
						);
					DX.SetFrequencySoundMem(a, tone.Value.Handle);

					if (!tone.Value.Playing)
					{
						if (tone.Value == LastTone[i])
							LastTone[i] = null;
						channel.Remove(tone.Key);
						continue;
					}
					/*
					if (channels[i].portament > 63)
					{
						if (bTone[i] == null)
							continue;
						if (tone.Value.PortamentTick > channels[i].portamentTime)
							continue;
						portafreq = (int)((bTone[i].noteno - tone.Value.noteno) * 682 - ((bTone[i].noteno - tone.Value.noteno) * (8192 / 12.0) / ((float)channels[i].portamentTime)) * ((float)tone.Value.PortamentTick));
						DX.DrawString(0, 300, portafreq.ToString(), DX.GetColor(255, 255, 255));
						DX.SetFrequencySoundMem((int)(44000 * Math.Pow(2, portafreq / 8192.0)), tone.Value.Handle);
				//		Console.WriteLine("{0} {1} {2}", "O" + tone.Value.Octave + tone.Value.Pitch, portafreq, tone.Value.PortamentTick);
						tone.Value.PortamentTick += 1;
					}

					*/


					//Console.Write("");
				}
			}
		}

		/// <summary>
		/// 指定したチャンネルデータベースから、リリース状態以外で出力中の Tone の数を取得します。
		/// </summary>
		/// <param name="ch">チャンネルを表す Dictionary。</param>
		/// <returns>リリース状態でない Playing な Tone の数。</returns>
		public static int GetPlayingTone(Dictionary<int, Tone> ch)
		{
			int i = 0;
			foreach (Tone dat in ch.Values)
			{
				if (dat.Playing && dat.envflag != EnvelopeFlag.Release)
					i++;
			}
			return i;
		}

	}

	public struct RPN
	{
		public short Data;

		public RPN(short data)
		{
			Data = data;
		}

	}

	public struct SMFPosition
	{
		public int Measure;
		public int Beat;
		public int Tick;
	}

	public struct Channel
	{
		public byte inst;
		public bool isDrum;
		public byte panpot;
		public byte volume;
		public byte expression;

		public int pitchbend;

		public bool end;

		public int portamentTime;

		public int portament;

		public RPN tweak;

		public RPN noteShift;

		public RPN bendRange;

		public Channel(byte i, bool id, byte p, byte v, byte e, int pmt, int pm, RPN t, RPN n, RPN b)
		{
			inst = i;
			isDrum = id;
			panpot = p;
			volume = v;
			expression = e;
			end = false;
			portamentTime = pmt;
			portament = pm;
			tweak = t;
			noteShift = n;
			bendRange = b;
			pitchbend = 0;
		}



	}

	public struct Mssf
	{
		public short[] wave;
		public int a, d, r, pan;
		public byte s;
		public static readonly Mssf Empty = new Mssf();
		public NoiseOption noiseoption;
		public Mssf(short[] w, int a, int d, byte s, int r, int pan)
		{
			this.wave = w;
			this.a = a;
			this.d = d;
			this.s = s;
			this.r = r;
			this.pan = pan;
			noiseoption = NoiseOption.None;
		}

		public Mssf(short[] w, int a, int d, byte s, int r, int pan, NoiseOption noise)
		{
			this.wave = w;
			this.a = a;
			this.d = d;
			this.s = s;
			this.r = r;
			this.pan = pan;
			noiseoption = noise;
		}


	}
}
