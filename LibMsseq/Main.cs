using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DxLibDLL;
using MusicSheet.Mssf;
using NextMidi.Data;
using NextMidi.Data.Score;
using NextMidi.DataElement;
using NextMidi.DataElement.MetaData;
using NextMidi.Filing.Midi;

namespace MusicSheet.Sequence
{
	public enum DrumFlag
	{
		Kick,
		Snare,
		HihatClose,
		HihatOpen,
		Cymbal,
		Tom1,
		Tom2,
		Tom3
	}

	public class MidiClock
	{
		private int _btime;
		private int _mili;

		private float _tick;


		public MidiClock(int bpm, int resolution)
		{
			Bpm = bpm;
			Resolution = resolution;
		}

		public int Bpm { get; set; }

		public int Resolution { get; set; }

		public int TickCount
		{
			get
			{
				int ret, ntime = DX.GetNowCount();
				if (IsRunning || (ntime > _btime))
					ret = (int) (_tick += TempoMapBase.GetTickLength(ntime - _btime, Bpm, Resolution));
				else
					ret = (int) _tick;
				_btime = DX.GetNowCount();
				return ret;
			}
			set { _tick = value; }
		}

		public int MiliSec
		{
			get { return _mili += 1000 / Fps; }
		}

		public bool IsRunning { get; set; }

		public int Fps { get; set; } = 60;

		public void Start()
		{
			_btime = DX.GetNowCount();
			IsRunning = true;
		}

		public void Stop()
		{
			IsRunning = false;
		}

		public void Reset()
		{
			_mili = (int) (_tick = 0);
		}
	}

	public class Sequencer
	{
		private TempoMap _tm;
		public List<int> Drumtracks = new List<int>();
		public int Eot;
		public int Loop = -1;
		public MidiClock Mc;

		public List<MetaEvent> Metas = new List<MetaEvent>();
		public MidiData Mfd;
		public int NMillisec, NTickCount, Btick, Bpm;
		public SoundModule Sm;
		public string Title, Copyright, Lyrics;

		public Sequencer()
		{
			Sm = new SoundModule();
			Reset();
		}

		public Sequencer(MidiData md)
			: this()
		{
			SetMidiData(md);
		}

		public bool IsPlaying { get; private set; }
		public int MaxChannel { get; private set; }
		public int MusicTime => _tm?.ToMilliSeconds(Eot) ?? 0;


		public int CurrentBpm
		{
			get
			{
				var a = Metas.FindLast(te => te is TempoEvent && (te.Tick <= NTickCount)) as TempoEvent;
				if (a != null)
					return a.Tempo;
				return 120;
			}
		}

		public RhythmEvent CurrentRhythm
		{
			get { return Metas.FindLast(te => te is RhythmEvent && (te.Tick <= NTickCount)) as RhythmEvent; }
		}

		public string CurrentTitle
		{
			get
			{
				var a = Metas.FindLast(te => te is MidiTrackTitle && (te.Tick <= NTickCount)) as MidiTrackTitle;
				if (a != null)
					return a.Text;
				return "";
			}
		}

		public string CurrentCopyright
		{
			get
			{
				var a = Metas.FindLast(te => te is MidiCopyright && (te.Tick <= NTickCount)) as MidiCopyright;
				if (a != null)
					return a.Text;
				return "";
			}
		}

		public string CurrentLyric
		{
			get
			{
				var a = Metas.FindLast(te => te is MidiLyric && (te.Tick <= NTickCount)) as MidiLyric;
				if (a != null)
					return a.Text;
				return "";
			}
		}

		public SmfPosition Position
		{
			get
			{
				var qre = new Queue<RhythmEvent>(Metas.FindAll(te => te is RhythmEvent).Cast<RhythmEvent>());
				var last = new SmfPosition
				{
					Measure = 1,
					Beat = 1
				};

				var tmp = new RhythmEvent(4, 4);
				for (var i = 0; i < NTickCount; i++)
				{
					if (last.Tick >= 4f / tmp.Note * Mfd.Resolution.Resolution)
					{
						last.Beat++;
						last.Tick = 0;
					}
					if (last.Beat > tmp.Rhythm)
					{
						last.Measure++;
						last.Beat = 1;
					}

					if ((qre.Count > 0) && (qre.Peek().Tick <= i))
						tmp = qre.Dequeue();


					last.Tick++;
				}

				return last;
			}
		}

		public bool IsLoaded { get; private set; }

		public static MidiData LoadSmf(string filename)
		{
			var mfr = new MidiReader(new FileStream(filename, FileMode.Open, FileAccess.Read), Encoding.Default);
			var m = mfr.ReadFile();
			mfr.Close();
			return m;
		}

		public void SetMidiData(MidiData md)
		{
			Mfd = md;
			_tm = new TempoMap(Mfd);
			Stop();
			Reset();
			Eot = 0;
			Drumtracks = new List<int>();
			if (Mfd != null)
				Mc = new MidiClock(120, Mfd.Resolution.Resolution);
			var cnt = 0;
			if (Mfd != null)
				foreach (var lst in Mfd.Tracks)
				{
					if ((lst.Channel.GetValueOrDefault() == 9) && !Drumtracks.Contains(cnt))
						Drumtracks.Add(cnt);

					if (Eot < lst.TickLength)
						Eot = lst.TickLength;
					cnt++;
				}

			IsLoaded = true;
		}

		public void Reset()
		{
			Sm?.Panic();


			Sm?.Reset();

			MaxChannel = 0;
			Bpm = 120;
			Btick = -1;
			Loop = -1;
			Title = Copyright = Lyrics = "";
		}

		public void Load(string filename)
		{
			Stop();
			Reset();
			SetMidiData(LoadSmf(filename));
		}

		public void Play()
		{
			if (!IsLoaded)
				return;
			if ((Mc == null) || (Mfd == null))
				return;
			Metas.Clear();
			var cnt = 0;
			foreach (var mt in Mfd.Tracks)
			{
				if ((mt.Channel != null) && (mt.Channel != 9) && (MaxChannel < mt.Channel))
					MaxChannel = (int) mt.Channel;
				foreach (var me in mt.GetData())
				{
					if (me is MetaEvent)
						if ((me is MidiTrackTitle && (cnt == 0)) || !(me is MidiTrackTitle))
							Metas.Add(me as MetaEvent);
					var e = me as ControlEvent;
					if ((e != null) && (e.Number == 111))
						Loop = e.Tick;
				}
				cnt++;
			}
			Mc.Reset();
			Mc.Start();
			IsPlaying = true;
		}

		public void Resume()
		{
			if (!IsLoaded)
				return;
			Mc.Start();
			IsPlaying = true;
		}

		public void Stop()
		{
			if (!IsLoaded)
				return;
			if (Mc != null)
				Sm.Panic();
			Mc?.Stop();
			IsPlaying = false;
		}


		public void PlayLoop()
		{
			//Console.WriteLine("mc:{0}", mc.IsRunning());
			if ((Mc == null) || !Mc.IsRunning)
				return;
			NMillisec = Mc.MiliSec;
			NTickCount = Mc.TickCount;
			Mc.Bpm = Bpm = CurrentBpm;


			//List<MidiEvent> mes = mfd.Tracks[2].GetData();
			//string debug2 = "";
			for (var j = 1; j < Mfd.Tracks.Count; j++)
			{
				//int hoge = mfd.Tracks[j].Channel.GetValueOrDefault();
				//debug2 += hoge + " ";

				var mt = Mfd.Tracks[j];
				if (NTickCount <= mt.TickLength)
					foreach (var me in mt.GetTickData(Btick + 1, NTickCount + 1))
					{
						//	try
						//	{
						Sm.SendEvent(me, mt.Channel, NTickCount, ref Mc);


						if (me is MidiLyric)
						{
							var ml = me as MidiLyric;
							Lyrics = ml.Text;
						}
					}
			}


			Sm.PlayTones(NTickCount);

			if (NTickCount > Eot)
			{
				Sm.Panic();
				if (Loop == -1)
				{
					IsPlaying = false;
					Mc.Stop();
				}
				else
				{
					Mc.TickCount = Loop;
					NTickCount = Loop - 1;
				}
			}


			Btick = NTickCount;
		}
	}


	public class SoundModule
	{
		private readonly Mssf[] _insts;
		private readonly Queue<Tone> _tonequeue;

		private ControlEvent[] _rpns;

		private int _volume;

		public int Bms;

		public Tone[] BTone;

		public Channel[] Channels;

		public int[] HPercs;

		public int Loop;

		public Tone NowTone;


		public string[] Pitchnames;
		public int Portamenttick;

		public SoundModule()
		{
			Pitchnames = new[] {"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};

			HPercs = new int[128];
			_tonequeue = new Queue<Tone>(4);
			_insts = new Mssf[128];
			Reset();
			for (var i = 0; i < 128; i++)
			{
				if (File.Exists($@"{Path.GetDirectoryName(Application.ExecutablePath)}\Insts\{i}.mssf"))
				{
					short[] wave;
					NoiseOption no;
					byte s;
					int d;
					int a;
					int r;
					int pan;
					MssfUtility.LoadFileDynamic($@"{Path.GetDirectoryName(Application.ExecutablePath)}\Insts\{i}.mssf", out wave, out a,
						out d, out s, out r, out pan, out no);
					_insts[i] = new Mssf(wave, a, d, s, r, pan, no);
				}
				else
				{
					_insts[i] = Mssf.Empty;
				}


				if (File.Exists($@"{Path.GetDirectoryName(Application.ExecutablePath)}\Drums\{i}.wav"))
					HPercs[i] = DX.LoadSoundMem($@"{Path.GetDirectoryName(Application.ExecutablePath)}\Drums\{i}.wav");
				else
					HPercs[i] = 0;
			}
		}

		public Dictionary<int, Tone>[] Tones { get; set; }

		public Tone[] LastTone { get; private set; }

		public int Volume
		{
			get { return _volume; }
			set
			{
				if ((value < 0) || (value > 100))
					throw new ArgumentOutOfRangeException(nameof(value));
				_volume = value;
			}
		}

		public void Reset()
		{
			Loop = -1;
			_volume = 100;
			Channels = new Channel[16];
			_tonequeue.Clear();
			_rpns = new ControlEvent[4];

			BTone = new Tone[16];
			Panic();
			Tones = new Dictionary<int, Tone>[16];
			LastTone = new Tone[16];
			for (var i = 0; i < 16; i++)
			{
				Tones[i] = new Dictionary<int, Tone>();
				if (i != 10)
					Channels[i] = new Channel(0, false, 64, 100, 127, 0, 0, new Rpn(0), new Rpn(0), new Rpn(2));
				else
					Channels[i] = new Channel(0, true, 64, 100, 127, 0, 0, new Rpn(0), new Rpn(0), new Rpn(2));
			}
		}

		~SoundModule()
		{
			foreach (var hoge in HPercs)
				DX.DeleteSoundMem(hoge);
		}

		public void Panic()
		{
			if (Tones != null)
				foreach (var tones in Tones)
					foreach (var tone in tones.Values)
						tone?.Abort();
		}

		public void SendEvent(MidiEvent me, byte? channel, int miditick, ref MidiClock cmc)
		{
			if (channel == null)
				return;
			var m = _insts[Channels[channel.GetValueOrDefault()].Inst];
			if ((m.Wave == null) && (channel.GetValueOrDefault() != 9))

				return;

			//ノートオン
			if (miditick != -1)
			{
				var @event = me as NoteEvent;
				if (@event != null)
				{
					//Tone a;
					var ne = @event;

					if ((ne.Note < 120) && (ne.Note > 11) && (ne.Channel.Value != 9))
					{
						if (ne.Velocity == 0)
							Tones[(int) channel][ne.Note].Stop();
						if (Tones[(int) channel].ContainsKey(ne.Note))
						{
							Tones[(int) channel][ne.Note].Abort();
							Tones[(int) channel].Remove(ne.Note);
						}
						BTone[ne.Channel.Value] = NowTone;
						var t = new Tone(Pitchnames[ne.Note % 12], (ne.Note - 12) / 12, m.Wave, new Envelope(m.A, m.D, m.S, m.R), 255, 0,
							ne.Velocity, m.Noiseoption);
						LastTone[(byte) channel] = t;
						Tones[(int) channel].Add(ne.Note, t);
						Tones[(int) channel][ne.Note].StartPlay(miditick, ne.Gate);
						_tonequeue.Enqueue(t);

						NowTone = t;
						Portamenttick = 0;
						Bms = DX.GetNowCount();
					}
					else
					{
						DX.PlaySoundMem(HPercs[ne.Note], DX.DX_PLAYTYPE_BACK);
						foreach (var handle in HPercs)
						{
							if (handle == 0)
								continue;
							DX.ChangePanSoundMem((Channels[9].Panpot - 64) * 4, HPercs[ne.Note]);
							DX.ChangeVolumeSoundMem(
								(int)
								(255 * (Channels[9].Volume / 127.0) * (Channels[9].Expression / 127.0) * (ne.Velocity / 127.0) * (Volume / 100f)),
								HPercs[ne.Note]);
						}
					}
				}
			}
			else
			{
				var onEvent = me as NoteOnEvent;
				if (onEvent != null)
				{
					//Tone a;
					var ne = onEvent;
					if ((ne.Note < 120) && (ne.Note > 11) && (ne.Channel.Value != 9))
					{
						if (ne.Velocity == 0)
							Tones[(int) channel][ne.Note].Stop();
						if (Tones[(int) channel].ContainsKey(ne.Note))
						{
							Tones[(int) channel][ne.Note].Abort();
							Tones[(int) channel].Remove(ne.Note);
						}
						var t = new Tone(Pitchnames[ne.Note % 12], (ne.Note - 12) / 12, m.Wave, new Envelope(m.A, m.D, m.S, m.R), 255, 0,
							ne.Velocity, m.Noiseoption);
						LastTone[(byte) channel] = t;
						Tones[(int) channel].Add(ne.Note, t);
						Tones[(int) channel][ne.Note].StartPlay(-1, -1);
						_tonequeue.Enqueue(t);
					}
					else
					{
						DX.PlaySoundMem(HPercs[ne.Note], DX.DX_PLAYTYPE_BACK);
						foreach (var handle in HPercs)
						{
							if (handle == 0)
								continue;
							DX.ChangePanSoundMem((Channels[9].Panpot - 64) * 4, HPercs[ne.Note]);
							DX.ChangeVolumeSoundMem(
								(int) (255 * (Channels[9].Volume / 127.0) * (Channels[9].Expression / 127.0) * (ne.Velocity / 127.0)),
								HPercs[ne.Note]);
						}
					}
				}
				//ノートオフ
				var @event = me as NoteOffEvent;
				if (@event != null)
				{
					var noe = @event;
					Tones[(int) channel][noe.Note].Stop();
				}
			}

			var programEvent = me as ProgramEvent;
			if (programEvent != null)
				Channels[(int) channel].Inst = programEvent.Value;

			var controlEvent = me as ControlEvent;
			if (controlEvent != null)
			{
				var ce = controlEvent;
				switch (ce.Number)
				{
					case 10:
						Channels[(int) channel].Panpot = ce.Value;
						break;
					case 7:
						Channels[(int) channel].Volume = ce.Value;
						break;
					case 11:
						Channels[(int) channel].Expression = ce.Value;
						break;
					case 101:
						_rpns[0] = ce;
						break;
					case 100:
						_rpns[1] = ce;
						break;
					case 6:
						_rpns[2] = ce;
						if (_rpns[1] == null)
							break;
						switch (_rpns[1].Value)
						{
							case 0:
								Channels[(int) channel].BendRange = new Rpn(_rpns[2].Value);
								break;

							case 2:
								Channels[(int) channel].NoteShift = new Rpn((short) (_rpns[2].Value - 64));
								break;
						}
						break;
					case 38:
						_rpns[3] = ce;
						if ((_rpns[1] == null) || (_rpns[2] == null))
							break;
						if (_rpns[1].Value == 1)
							Channels[(int) channel].Tweak = new Rpn((short) (_rpns[2].Value * 128 + _rpns[3].Value - 8192));
						break;
					case 111:
						Loop = ce.Tick;
						break;
					case 65:
						Channels[(int) channel].Portament = ce.Value;
						break;
					case 5:
						Channels[(int) channel].PortamentTime = ce.Value;
						break;
				}
			}

			if (me is PitchEvent)
			{
				var native = me.ToNativeEvent();
				var pitchdata = native[2] * 128 + native[1] - 8192;

				Channels[(int) channel].Pitchbend = pitchdata;

				//Console.WriteLine("Decimal: " + pe.Value + "Hexa: " + pe.Value.ToString("X2") + "Binary: " + Convert.ToString(2, pe.Value));
			}

			if ((cmc != null) && me is MidiEndOfTrack)
			{
				Channels[me.Channel.GetValueOrDefault()].End = true;
				var allend = true;
				foreach (var c in Channels)
					if (!c.End)
					{
						allend = false;
						break;
					}
				if (allend)
					if (Loop == -1)
						cmc.Stop();
					else
						cmc.TickCount = Loop;
			}
		}

		public void PlayTones(int miditick)
		{
			//(Dictionary<int, Tone> channel in Tones)
			for (var i = 0; i < Tones.Length; i++)
			{
				var channel = Tones[i];
				//bool canDelete = false;
				//foreach (KeyValuePair<int, Tone> tone in channel)
				var tones = channel.ToArray();
				for (var j = 0; j < channel.Count; j++)
				{
					var tone = tones[j];

					tone.Value.PlayLoop(miditick);
					DX.ChangePanSoundMem((Channels[i].Panpot - 64) * 4, tone.Value.Handle);
					DX.ChangeVolumeSoundMem(
						(int)
						(tone.Value.OutVolume * (Channels[i].Volume / 127.0) * (Channels[i].Expression / 127.0) *
						 (tone.Value.Velocity / 127.0) * (Volume / 100f)), tone.Value.Handle);
					//DX.SetFrequencySoundMem(44100 + (int)(channels[i].pitchbend * ((44100 * (channels[i].bendRange.Data / 12.0)) / 8192.0)), tone.Value.Handle);
					var a = (int)
					(
						(tone.Value.Octave > 6
							? tone.Value.GetFreq(tone.Value.Pitch, tone.Value.Octave - 2) * 100
							: tone.Value.GetFreq(tone.Value.Pitch, 4) * 100)
						* Math.Pow(2, Channels[i].Pitchbend / 8192.0 * (Channels[i].BendRange.Data / 12.0)) *
						Math.Pow(2, Channels[i].Tweak.Data / 8192f * (2 / 12f)) * Math.Pow(2, Channels[i].NoteShift.Data / 12f)
					);
					DX.SetFrequencySoundMem(a, tone.Value.Handle);

					if (!tone.Value.Playing)
					{
						if (tone.Value == LastTone[i])
							LastTone[i] = null;
						channel.Remove(tone.Key);
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
		///     指定したチャンネルデータベースから、リリース状態以外で出力中の Tone の数を取得します。
		/// </summary>
		/// <param name="ch">チャンネルを表す Dictionary。</param>
		/// <returns>リリース状態でない Playing な Tone の数。</returns>
		public static int GetPlayingTone(Dictionary<int, Tone> ch)
			=> ch.Values.Count(dat => dat.Playing && (dat.Envflag != EnvelopeFlag.Release));
	}

	public struct Rpn
	{
		public short Data;

		public Rpn(short data)
		{
			Data = data;
		}
	}

	public struct SmfPosition
	{
		public int Measure;
		public int Beat;
		public int Tick;
	}

	public struct Channel
	{
		public byte Inst;
		public bool IsDrum;
		public byte Panpot;
		public byte Volume;
		public byte Expression;

		public int Pitchbend;

		public bool End;

		public int PortamentTime;

		public int Portament;

		public Rpn Tweak;

		public Rpn NoteShift;

		public Rpn BendRange;

		public Channel(byte i, bool id, byte p, byte v, byte e, int pmt, int pm, Rpn t, Rpn n, Rpn b)
		{
			Inst = i;
			IsDrum = id;
			Panpot = p;
			Volume = v;
			Expression = e;
			End = false;
			PortamentTime = pmt;
			Portament = pm;
			Tweak = t;
			NoteShift = n;
			BendRange = b;
			Pitchbend = 0;
		}
	}

	public struct Mssf
	{
		public short[] Wave;
		public int A, D, R, Pan;
		public byte S;
		public static readonly Mssf Empty = new Mssf();
		public NoiseOption Noiseoption;

		public Mssf(short[] w, int a, int d, byte s, int r, int pan)
		{
			Wave = w;
			A = a;
			D = d;
			S = s;
			R = r;
			Pan = pan;
			Noiseoption = NoiseOption.None;
		}

		public Mssf(short[] w, int a, int d, byte s, int r, int pan, NoiseOption noise)
		{
			Wave = w;
			A = a;
			D = d;
			S = s;
			R = r;
			Pan = pan;
			Noiseoption = noise;
		}
	}
}