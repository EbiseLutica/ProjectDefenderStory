﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DxLibDLL;

namespace MusicSheet.Mssf
{
	public class Tone : ITone, ICloneable
	{
		public static List<string> Pitchnames =
			new[] {"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"}.ToList();

		private bool _beforenew;

		private float _outvolume;

		public int Bmilisec;

		public EnvelopeFlag Envflag = EnvelopeFlag.None;

		public int Gate;


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

		public bool IsNew = true;
		public bool IsStopping;

		public int Noteno;
		public int NowMidiTick;
		public int ReleasedVolume;
		public int StartedMidiTick;

		public Tone(string pitch, int octave, short[] wave, Envelope env, int vol, int pan)
		{
			if (DX.DxLib_IsInit() == 0)
				throw new Exception("DXLib が初期化されていません。");
			Pitch = pitch;
			Octave = octave;
			Wave = wave;
			Freq = GetFreq(pitch, octave);

			Handle = SetWave(wave, octave, pan);
			Envelope = env;

			Volume = vol;

			Playing = false;
			Velocity = 100;
		}

		public Tone(string pitch, int octave, short[] wave, Envelope env, int vol, int pan, int vel)
		{
			if (DX.DxLib_IsInit() == 0)
				throw new Exception("DXLib が初期化されていません。");
			Pitch = pitch;
			Octave = octave;
			Wave = wave;
			Freq = GetFreq(pitch, octave);
			Handle = SetWave(wave, octave, pan);
			Envelope = env;

			Volume = vol;

			Velocity = vel;

			Playing = false;
		}

		public Tone(string pitch, int octave, short[] wave, Envelope env, int vol, int pan, int vel, NoiseOption noiseflag)
		{
			if (DX.DxLib_IsInit() == 0)
				throw new Exception("DXLib が初期化されていません。");


			Pitch = pitch;
			Octave = octave;
			Wave = wave;
			Freq = GetFreq(pitch, octave);
			Handle = SetWave(wave, octave, pan, noiseflag);
			Envelope = env;

			Volume = vol;

			Velocity = vel;

			Playing = false;
		}

		public short[] Wave { get; set; }

		public int Volume { get; set; }

		public Envelope Envelope { get; set; }
		public int Velocity { get; set; }

		public int PortamentTick { get; set; }

		public object Clone()
		{
			var t = (Tone) MemberwiseClone();
			t.Wave = (short[]) Wave.Clone();
			t.Envelope = new Envelope(
				Envelope.AttackTime,
				Envelope.DecayTime,
				Envelope.SustainLevel,
				Envelope.ReleaseTime
			);

			return t;
		}

		public string Pitch { get; set; }
		public int Octave { get; set; }

		public float Freq { get; set; }

		public int OutVolume => (int) _outvolume;

		public int Tick { get; set; }

		public bool Playing { get; set; }

		public int Handle { get; set; }

		public void StartPlay(int miditick, int gate)
		{
			_outvolume = 0;
			Playing = true;
			DX.ChangeVolumeSoundMem(0, Handle);
			DX.SetFrequencySoundMem((int) (GetFreq(Pitch, 4) * 100), Handle);
			if (Octave > 6)
				DX.SetFrequencySoundMem((int) (GetFreq(Pitch, Octave - 2) * 100), Handle);
			DX.PlaySoundMem(Handle, DX.DX_PLAYTYPE_LOOP);
			//DX.ChangeVolumeSoundMem(0, Handle);
			Envflag = EnvelopeFlag.Attack;
			Tick = (int) (Envelope.AttackTime / 1.2);
			Bmilisec = DX.GetNowCount();
			StartedMidiTick = miditick;
			Gate = gate;
			//Console.WriteLine("[DEBUG]音源再生開始");
		}

		public void PlayLoop(int miditick)
		{
			if (!Playing)
				return;
			if (IsNew && _beforenew)
				IsNew = false;
			if (!_beforenew)
				_beforenew = true;
			//if (DX.GetNowCount() - bmilisec <= 0)
			//	return;

			#region ふるいの

/*
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
														_outvolume = (int)((tick) * ((float)Volume / (Envelope.AttackTime - 1)));
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
														_outvolume = (int)(Volume - (tick + 1) * ((float)(Volume - Envelope.SustainLevel * (Volume / 255.0)) / Envelope.DecayTime));
														break;
													case EnvelopeFlag.Release:
														if (tick >= Envelope.ReleaseTime)
														{
															Abort();
															return;
														}
														_outvolume = (int)(releasedVolume - (tick + 1) * ((float)releasedVolume / Envelope.ReleaseTime));
														break;
												}
												*/

			#endregion

			switch (Envflag)
			{
				case EnvelopeFlag.Attack:
					if (Envelope.AttackTime == 0)
					{
						_outvolume = 255;
						Envflag = EnvelopeFlag.Decay;
						break;
					}
					var zoubun = 255f / Envelope.AttackTime;
					_outvolume = _outvolume + zoubun * Tick;
					if (_outvolume >= 255)
					{
						_outvolume = 255;
						Envflag = EnvelopeFlag.Decay;
					}
					break;
				case EnvelopeFlag.Decay:
					if (Envelope.DecayTime == 0)
					{
						_outvolume = Envelope.SustainLevel;
						Envflag = EnvelopeFlag.Sustain;
						break;
					}
					var genbun = (255f - Envelope.SustainLevel) / Envelope.DecayTime;
					_outvolume = _outvolume - genbun * Tick;
					if (_outvolume <= Envelope.SustainLevel)
					{
						_outvolume = Envelope.SustainLevel;
						Envflag = EnvelopeFlag.Sustain;
					}
					break;
				case EnvelopeFlag.Release:
					if (Envelope.ReleaseTime == 0)
					{
						Abort();
						break;
					}
					var genbun2 = (float) ReleasedVolume / Envelope.ReleaseTime;
					_outvolume = _outvolume - genbun2 * Tick;
					if (_outvolume <= 0)
					{
						_outvolume = 0;
						Abort();
					}
					break;
			}

			NowMidiTick = miditick;
			if ((NowMidiTick - StartedMidiTick > Gate) && (Gate != -1) && (Envflag != EnvelopeFlag.Release))
				Stop();
			Tick = DX.GetNowCount() - Bmilisec;

			Bmilisec = DX.GetNowCount();
			//Console.WriteLine("[DEBUG]音源ループ: {0}, {1}, {2}", outVolume, tick, envflag);
		}

		public void Stop()
		{
			ReleasedVolume = OutVolume;
			Envflag = EnvelopeFlag.Release;
			Tick = 0;
			IsStopping = true;
			//Console.WriteLine("[DEBUG]音源再生終了リクエスト受信");
		}

		public void Abort()
		{
			Tick = 0;
			Envflag = EnvelopeFlag.None;
			DX.StopSoundMem(Handle);
			DX.DeleteSoundMem(Handle);
			Playing = false;

			//Console.WriteLine("[DEBUG]音源再生終了");
		}

		/// <summary>
		///     一周期分の波形データ、周波数、パンポットを指定して、波形データを作成します。
		/// </summary>
		/// <param name="wave">32個の波形データ。</param>
		/// <param name="hz">周波数。</param>
		/// <param name="pan">-100 ～ +100 の範囲で、パンポット。</param>
		/// <returns>生成されたサウンドバッファーのハンドル。</returns>
		public static int SetWave(short[] wave, int oct, int pan)
		{
			return SetWave(wave, oct, pan, NoiseOption.None);
		}

		public static int SetWave(short[] wave, int oct, int pan, NoiseOption no)
		{
			var hz = GetRelativeFreq(Math.Min(oct, 6));
			var length = 0;
			//string debug = "";
			var sheed = 0x8000;
			/*for (int i = 0; i < 44100; i++)
				if ((decimal)Math.PI * 2 / (decimal)44100 * i * (hz + (hz / 440m)) > (decimal)Math.PI * 2)
				{
					//Console.Write(Math.PI * 2 / 44100 * i * hz * 180 / Math.PI);
					length = i;
					break;
				}
			 */
			length = (int) (44100 / hz + 0.5);
			//length++;
			//length *= 10;
			//		Hz = 1 / s
			//×s	s × Hz = 1
			//÷Hz	s = 1 / Hz
			//if (hz >= 1046)
			//	length = 11025;
			float t = 0;

			if (no != NoiseOption.None)
				length = 11025;

			var hSSnd = DX.MakeSoftSound1Ch16Bit44KHz(length);

			//int[] sikis = new int[length];
			var y = new ushort[length];

			var noise = new ushort[length];
			if (no != NoiseOption.None)
				noise = SetNoise(hz, no == NoiseOption.Short ? true : false, ref sheed, length);
			for (var i = 0; i < length; i++)
			{
				//t = wave[(int)((Math.PI * 2 / 44100 * i * hz * 180 / Math.PI) % 360 / (359 / 31.0))]+32768; // divided by 10 means volume control
				var siki = (int) Math.Round(2 / 44100.0 * i * hz * 180 % 360 / (360 / 31.0));
				t = wave[siki] + 32768;
				//var siki = Math.PI * 2 / 44100 * i * hz;
				//t = (float)Math.Sin(siki) * 32768 + 32767;
				//	t = (float)(Math.Sin(Math.PI * 2 / 44100 * i * hz) * 32768 + 32768);
				//sikis[i] = siki;
				//t = wave[(int)(i * (32.0 / length))] + 32768;

				t = (noise[i] + t) / 2;

				/*if (i > 0)
				y[i] = (ushort)(0.9f * (ushort)y[i - 1] + 0.1f*t);
				else*/
				y[i] = (ushort) t;
				//debug += (int)((Math.PI * 2 / 44100 * i * hz * 180 / Math.PI) % 360 / (359 / 31.0)) + " ";
				/*
				if (i > 1)
					y[i] = (ushort)((y[i - 2] + y[i - 1] + t) / 3);
				if (i > 0)
					y[i] = (ushort)((y[i - 1] + t) / 3);
				else
					y[i] = (ushort)t;
				*/
				DX.WriteSoftSoundData(hSSnd, i, (ushort) t, (ushort) t);
			}

			var retval = DX.LoadSoundMemFromSoftSound(hSSnd);
			DX.DeleteSoftSound(hSSnd);

			return retval;
		}

		public static ushort[] SetNoise(float hz, bool isShortFreq, ref int sheed, int length)
		{
			var output = 0;


			var data = new ushort[length];
			float t = 0;
			//int hSSnd = DX.MakeSoftSound1Ch16Bit44KHz(length);
			var kaisu = 1;
			for (var i = 0; i < length; i++)
			{
				if (i > 8000 / hz * kaisu)
				{
					sheed >>= 1;
					sheed |= ((sheed ^ (sheed >> (isShortFreq ? 6 : 1))) & 1) << 15;
					output = sheed & 1;
					kaisu++;
				}
				t = output * 65535; // divided by 10 means volume control
				data[i] = (ushort) t;
			}


			return data;
		}


		public static float GetRelativeFreq(int oct)
		{
			return GetFreqS("A", oct);
		}

		public float GetFreq(string pitch, int oct)
		{
			var hoge = oct * 12 + Pitchnames.IndexOf(pitch) + 12;
			Noteno = hoge;
			var hage = GetFreq(hoge);

			return hage;
		}

		public static float GetFreqS(string pitch, int oct)
		{
			var hoge = oct * 12 + Pitchnames.IndexOf(pitch) + 12;
			var hage = GetFreqS(hoge);

			return hage;
		}

		public float GetFreq(int noteno)
		{
			Noteno = noteno;
			return (float) (441 * Math.Pow(2, (noteno - 69) / 12.0));
		}

		public static float GetFreqS(int noteno)
		{
			return (float) (441 * Math.Pow(2, (noteno - 69) / 12.0));
		}
	}

	public enum NoiseOption
	{
		None,
		Long,
		Short
	}

	public enum EnvelopeFlag
	{
		None,
		Attack,
		Decay,
		Sustain,
		Release
	}

	public class Envelope
	{
		public Envelope(int a, int d, byte s, int r)
		{
			AttackTime = a;
			DecayTime = d;
			SustainLevel = s;
			ReleaseTime = r;
		}

		public int AttackTime { get; set; }
		public int DecayTime { get; set; }
		public byte SustainLevel { get; set; }
		public int ReleaseTime { get; set; }
	}

	public interface ITone
	{
		string Pitch { get; set; }
		int Octave { get; set; }
		float Freq { get; set; }
		int OutVolume { get; }

		int Tick { get; set; }
		bool Playing { get; set; }
		int Handle { get; set; }

		void StartPlay(int miditick, int gate);
		void PlayLoop(int miditick);
		void Stop();
		void Abort();
	}

	public static class MssfUtility
	{
		public static void LoadFileDynamic(string path, out short[] wave, out int a, out int d, out byte s, out int r, out int pan)
		{
			NoiseOption tmp;
			LoadFileDynamic(path, out wave, out a, out d, out s, out r, out pan, out tmp);
		}

		public static void LoadFileDynamic(string path, out short[] wave, out int a, out int d, out byte s, out int r, out int pan, out NoiseOption noiseoption)
		{
			BinaryReader br = null;
			try
			{
				br = new BinaryReader(new FileStream(path, FileMode.Open));
			}
			catch (FileNotFoundException)
			{
				throw new Exception("ERR:0004");
			}
			var head = br.ReadChars(8);
			if (new string(head) != "MSSF_VER")
				throw new Exception("ERR:0002");
			var ver = 0;
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

		public static void SaveFileVer1(string path, short[] wave, int a, int d, byte s, int r, int pan)
		{
			BinaryWriter bw = null;
			try
			{
				bw = new BinaryWriter(new FileStream(path, FileMode.Create));
			}
			catch (UnauthorizedAccessException)
			{
				throw new Exception("ERR:0003");
			}
			bw.Write(new[] {'M', 'S', 'S', 'F', '_', 'V', 'E', 'R', '0', '0', '1'}, 0, 11); //ヘッダー
			foreach (var wav in wave)
				bw.Write(wav); //波形データ
			bw.Write(a); //アタックタイム
			bw.Write(d); //ディケイタイム
			bw.Write(s); //サスティンレベル
			bw.Write(r); //リリースタイム
			bw.Write(pan); //パンポット

			bw.Close(); //ストリームを閉じる
		}

		public static void SaveFileVer2(string path, short[] wave, int a, int d, byte s, int r, int pan, NoiseOption noiseoption)
		{
			BinaryWriter bw = null;
			try
			{
				bw = new BinaryWriter(new FileStream(path, FileMode.Create));
			}
			catch (UnauthorizedAccessException)
			{
				throw new Exception("ERR:0003");
			}
			bw.Write(new[] {'M', 'S', 'S', 'F', '_', 'V', 'E', 'R', '0', '0', '2'}, 0, 11); //ヘッダー
			foreach (var wav in wave)
				bw.Write(wav); //波形データ
			bw.Write(a); //アタックタイム
			bw.Write(d); //ディケイタイム
			bw.Write(s); //サスティンレベル
			bw.Write(r); //リリースタイム
			bw.Write(pan); //パンポット
			bw.Write((byte) noiseoption);
			bw.Close(); //ストリームを閉じる
		}

		public static void LoadFileVer1(string path, out short[] wave, out int a, out int d, out byte s, out int r, out int pan)
		{
			wave = new short[32];
			BinaryReader br = null;
			try
			{
				br = new BinaryReader(new FileStream(path, FileMode.Open));
			}
			catch (FileNotFoundException)
			{
				throw new Exception("ERR:0004");
			}
			var head = br.ReadChars(11);
			if (new string(head) != "MSSF_VER001")
				if (new string(head).Substring(0, 8) == "MSSF_VER")
					throw new Exception("ERR:0001"); //指定した Music Sheet Sound File のバージョンが異なる。
				else
					throw new Exception("ERR:0002"); //そのファイルは Music Sheet Sound File ではない。
			for (var i = 0; i < 32; i++)
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
			BinaryReader br = null;
			try
			{
				br = new BinaryReader(new FileStream(path, FileMode.Open));
			}
			catch (FileNotFoundException)
			{
				throw new Exception("ERR:0004");
			}
			var head = br.ReadChars(11);
			if (new string(head) != "MSSF_VER002")
				if (new string(head).Substring(0, 8) == "MSSF_VER")
					throw new Exception("ERR:0001"); //指定した Music Sheet Sound File のバージョンが異なる。
				else
					throw new Exception("ERR:0002"); //そのファイルは Music Sheet Sound File ではない。
			for (var i = 0; i < 32; i++)
				wave[i] = br.ReadInt16();

			a = br.ReadInt32();
			d = br.ReadInt32();
			s = br.ReadByte();
			r = br.ReadInt32();

			pan = br.ReadInt32();
			int tmp = br.ReadByte();
			if (tmp > 2)
				throw new Exception("ERR:0006");

			noiseoption = (NoiseOption) tmp;
			br.Close();
		}
	}
}