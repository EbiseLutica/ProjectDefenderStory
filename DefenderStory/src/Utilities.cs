
using DxLibDLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using DefenderStory.Entities;
using MusicSheet.Sequence;
using NextMidi.Data;

namespace DefenderStory.Util
{
	/// <summary>
	/// ゲームのリソースを取得し管理する静的クラスです。
	/// </summary>
	public static class ResourceUtility
	{
		#region Properties
		/// <summary>
		/// ファイアープレイヤー。
		/// </summary>
		public static int[] FirePlayer { get; private set; }
		/// <summary>
		/// アイスプレイヤー。
		/// </summary>
		public static int[] IcePlayer { get; private set; }
		/// <summary>
		/// リーフプレイヤー。
		/// </summary>
		public static int[] LeafPlayer { get; private set; }
		/// <summary>
		/// スーパープレイヤー。
		/// </summary>
		public static int[] BigPlayer { get; private set; }
		/// <summary>
		/// プレイヤー。
		/// </summary>
		public static int[] MiniPlayer { get; private set; }
		/// <summary>
		/// 雑魚キャラ。
		/// </summary>
		public static int[] CommonMob { get; private set; }
		/// <summary>
		/// 地上に生息するモドキー。
		/// </summary>
		public static int[] Modokee_Ground { get; private set; }
		/// <summary>
		/// 地下に生息するモドキー。
		/// </summary>
		public static int[] Modokee_Cave { get; private set; }
		/// <summary>
		/// デーモン。
		/// </summary>
		public static int[] Daemon { get; private set; }
		/// <summary>
		/// アーチャー。
		/// </summary>
		public static int[] Archer { get; private set; }
		/// <summary>
		/// 武器など。
		/// </summary>
		public static int[] Weapon { get; private set; }
		/// <summary>
		/// ドワーフ。
		/// </summary>
		public static int[] Dwarf { get; private set; }
		/// <summary>
		/// クモ。
		/// </summary>
		public static int[] Spider { get; private set; }
		/// <summary>
		/// ターコス。
		/// </summary>
		public static int[] Turcos { get; private set; }
		/// <summary>
		/// ターコスの甲羅。
		/// </summary>
		public static int[] TurcosShell { get; private set; }
		/// ウッディー(没キャラ)。
		/// </summary>
		public static int[] Woody { get; private set; }
		/// <summary>
		/// アイテム。
		/// </summary>
		public static int[] Item { get; private set; }
		/// <summary>
		/// パーティクル。
		/// </summary>
		public static int[] Particle { get; private set; }
		/// <summary>
		/// マップチップ。
		/// </summary>
		public static int[] MapChip { get; private set; }
		/// <summary>
		/// ボクサー。
		/// </summary>
		public static int[] Boxer { get; private set; }
		/// <summary>
		/// 転がる岩。
		/// </summary>
		public static int[] RollingRock { get; private set; }
		/// <summary>
		/// ターボ。
		/// </summary>
		public static int[] Turbo { get; private set; }
		/// <summary>
		/// ファイター。
		/// </summary>
		public static int[] Fighter { get; private set; }

		/// <summary>
		/// マップチップ。
		/// </summary>
		public static int[] Logo { get; private set; }
		/// <summary>
		/// 8pxに分割されたマップチップ。ブロック破壊時などに使う。
		/// </summary>
		public static int[] MapChipMini { get; private set; }

		public static int[] StrangeFlower { get; private set; }

		public static int TheEnd { get; private set; }

		private static Dictionary<string, MidiData> musicList = new Dictionary<string, MidiData>();
		public static Dictionary<string, MidiData> MusicList
		{
			get
			{
				return musicList;
			}
		}

		#endregion
		public static void Init()
		{

			LeafPlayer = new int[72];

			if (DX.LoadDivGraph("Resources\\Graphics\\spplayer_leaf.png", 72, 18, 4, 16, 32, out LeafPlayer[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			IcePlayer = new int[72];

			if (DX.LoadDivGraph("Resources\\Graphics\\spplayer_ice.png", 72, 18, 4, 16, 32, out IcePlayer[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			FirePlayer = new int[72];

			if (DX.LoadDivGraph("Resources\\Graphics\\spplayer_fire.png", 72, 18, 4, 16, 32, out FirePlayer[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}


			BigPlayer = new int[72];

			if (DX.LoadDivGraph("Resources\\Graphics\\spplayer.png", 72, 18, 4, 16, 32, out BigPlayer[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			MiniPlayer = new int[72];

			if (DX.LoadDivGraph("Resources\\Graphics\\spplayermini.png", 72, 18, 4, 16, 16, out MiniPlayer[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			CommonMob = new int[144];

			if (DX.LoadDivGraph("Resources\\Graphics\\commonMob.png", 144, 16, 4, 16, 16, out CommonMob[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}

			Modokee_Ground = new int[8];

			if (DX.LoadDivGraph("Resources\\Graphics\\spModokee.png", 8, 8, 1, 32, 16, out Modokee_Ground[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}

			Modokee_Cave = new int[8];

			if (DX.LoadDivGraph("Resources\\Graphics\\spCaveModokee.png", 8, 8, 1, 32, 16, out Modokee_Cave[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}

			Daemon = new int[14];

			if (DX.LoadDivGraph("Resources\\Graphics\\spdaemon.png", 14, 14, 1, 16, 16, out Daemon[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}

			Archer = new int[14];

			if (DX.LoadDivGraph("Resources\\Graphics\\sparcher.png", 8, 8, 1, 32, 32, out Archer[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}

			Weapon = new int[14];

			if (DX.LoadDivGraph("Resources\\Graphics\\spweapon.png", 14, 14, 1, 16, 16, out Weapon[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}

			Dwarf = new int[5];

			if (DX.LoadDivGraph("Resources\\Graphics\\spdwarf.png", 5, 5, 1, 16, 32, out Dwarf[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}

			Fighter = new int[2];

			if (DX.LoadDivGraph("Resources\\Graphics\\spfighter.png", 2, 2, 1, 16, 32, out Fighter[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}

			Turbo = new int[6];

			if (DX.LoadDivGraph("Resources\\Graphics\\spturbo.png", 6, 6, 1, 16, 16, out Turbo[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}

			Boxer = new int[6];

			if (DX.LoadDivGraph("Resources\\Graphics\\spboxer.png", 2, 2, 1, 32, 32, out Boxer[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}

			RollingRock = new int[2];

			if (DX.LoadDivGraph("Resources\\Graphics\\sprollingrock.png", 2, 2, 1, 32, 32, out RollingRock[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}


			Turcos = new int[12];

			if (DX.LoadDivGraph("Resources\\Graphics\\spTurcos.png", 12, 11, 2, 24, 16, out Turcos[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}

			TurcosShell = new int[4];

			if (DX.LoadDivGraph("Resources\\Graphics\\spTurcosShell.png", 4, 4, 1, 16, 16, out TurcosShell[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}

			Spider = new int[1];

			if (DX.LoadDivGraph("Resources\\Graphics\\spSpider.png", 1, 1, 1, 16, 16, out Spider[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}

			Logo = new int[2];

			if (DX.LoadDivGraph("Resources\\Graphics\\logo.png", 2, 2, 1, 180, 101, out Logo[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");

			}

			Item = new int[10];

			if (DX.LoadDivGraph("Resources\\Graphics\\spitem.png", 10, 10, 1, 16, 16, out Item[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			Particle = new int[7];

			if (DX.LoadDivGraph("Resources\\Graphics\\spparticle.png", 7, 7, 1, 8, 8, out Particle[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			StrangeFlower = new int[5];

			if (DX.LoadDivGraph("Resources\\Graphics\\spstrangeflower.png", 5, 5, 1, 48, 48, out StrangeFlower[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			if ((TheEnd = DX.LoadGraph("Resources\\Graphics\\theend.png")) == 0)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			foreach (var s in Directory.GetFiles(".\\Resources\\Music"))
			{
				MusicList[Path.GetFileName(s)] = Sequencer.LoadSMF(s);
			}
		}

	

		public static int[] GetMpt(string mptname)
		{
			int[] m = new int[64],
				mm = new int[256];
			DX.LoadDivGraph("Resources\\Graphics\\" + mptname + ".png", 64, 16, 4, 16, 16, out m[0]);
			MapChip = m;
			DX.LoadDivGraph("Resources\\Graphics\\" + mptname + ".png", 256, 32, 8, 8, 8, out mm[0]);
			MapChipMini = mm;

			return m;
		}

		

	}

	/// <summary>
	/// パーティクルの処理をまとめた静的クラスです。
	/// </summary>
	public static class ParticleUtility
	{
		public static int tick = 0;

		/// <summary>
		/// 泡を出します。
		/// </summary>
		/// <param name="sp">泡を出すエンティティ。</param>
		public static void Bubble(Entity sp)
		{
			if (tick % 65 == 0)
			{
				sp.Parent.Add(GameEngine.EntityRegister.CreateEntity("Bubble", new PointF((int)sp.Location.X + sp.Size.Width / 2 - 4, (int)sp.Location.Y + sp.Size.Height / 2 - 4), sp.Mpts, sp.Map, sp.Parent));
			}
			tick = (tick + 1) % 3600;
		}

		/// <summary>
		/// 水しぶきを上げます。
		/// </summary>
		/// <param name="sp">水しぶきを上げるエンティティ。</param>
		public static void WaterSplash(Entity sp)
		{
			for (int i = 0; i < 13 + DX.GetRand(4); i++)
			{
				sp.Parent.Add(GameEngine.EntityRegister.CreateEntity("WaterSplash", new Point((int)sp.Location.X + DX.GetRand(sp.Size.Width), (int)sp.Location.Y + sp.Size.Height / 2), sp.Mpts, sp.Map, sp.Parent));
			}
		}

		public static void BrokenBlock(Point pos, EntityList spcolle, Data.Object[] mptobjects)
		{
			for (int i = 0; i < 3 + DX.GetRand(3); i++)
			{
				spcolle.Add(GameEngine.EntityRegister.CreateEntity("BrokenBlock", new Point(pos.X + DX.GetRand(8) - 4, pos.Y + DX.GetRand(8) - 4), mptobjects, GameEngine.chips, spcolle));
			}
		}
	
	}
	/// <summary>
	/// フォントデータを取得し、文字列描画機能を提供する静的クラスです。
	/// </summary>
	public static class FontUtility
	{
		static int[] hFont;
		static int[] hFont_mini;
		static Dictionary<char, int> dicFont = new Dictionary<char, int>();
		static bool isInit = false;
		public static void Init()
		{
			hFont = new int[384];
			DX.LoadDivGraph("Resources\\Graphics\\font.png", 384, 16, 24, 10, 10, out hFont[0]);
			hFont_mini = new int[320];
			DX.LoadDivGraph("Resources\\Graphics\\font_mini.png", 320, 16, 20, 8, 8, out hFont_mini[0]);
			int i = 1;  //0番目は、存在しない文字があったときに表示する文字なので、1から始まる
			foreach (char c in File.ReadAllText("Resources\\Document\\char.txt"))
			{
				dicFont[c] = i++;   //文字番号を char によって指定できるよう登録する
			}
			DX.ChangeFont("MS Gothic");
			DX.SetFontSize(11);
			//DX.ChangeFontType(DX.DX_FONTTYPE_ANTIALIASING);
			DX.SetFontThickness(1);
			isInit = true;
		}

		/// <summary>
		/// 文字列を描画します。
		/// </summary>
		/// <param name="x">描画する始点 X 座標。</param>
		/// <param name="y">描画する始点 Y 座標。</param>
		/// <param name="txt">描画する文字列。</param>
		/// <param name="color">色。</param>
		public static void DrawString(int x, int y, string txt, int color)
		{
			if (!isInit) throw new Exception("Font Utility が初期化されていません。");
			int _x = x, _y = y;
			DX.SetDrawBright((color & 0xff0000) >> 16, (color & 0x00ff00) >> 8, color & 0x0000ff);
			for (int i = 0; i < txt.Length; i++)
			{
				int target;
				if (dicFont.ContainsKey(txt[i]))    //ある文字ならそれを指定
					target = dicFont[txt[i]];
				else if (txt[i] == '\n')
				{
					_x = x;
					_y += 10;
					continue;
				}
				else if (txt[i] == ' ')             //空白文字はないので飛ばす(空白は開く)
				{
					_x += 8;
					continue;
				}
				else
					target = 0;                     //それ以外なら、存在しない文字として出力する

				if (target > 0)
					DX.DrawGraph(_x, _y, hFont[target], 1);
				else
					DX.DrawString(_x, _y, "" + txt[i], DX.GetColor(255, 255, 255)); 
				_x += ((txt[i] < 128) ? 8 : 10);

			}
			DX.SetDrawBright(255, 255, 255);
			//var p = GetDrawingSize(txt);
			//DX.DrawBox(x, y, x + p.Width, y + p.Height, DX.GetColor(255, 0, 0), 0);

		}

		public static void DrawString(int x, int y, int xx, int yy, string txt, int color)
		{
			if (!isInit)
				throw new Exception("Font Utility が初期化されていません。");
			int _x = x, _y = y;
			DX.SetDrawBright((color & 0xff0000) >> 16, (color & 0x00ff00) >> 8, color & 0x0000ff);
			for (int i = 0; i < txt.Length; i++)
			{
				int target;
				if (dicFont.ContainsKey(txt[i]))    //ある文字ならそれを指定
					target = dicFont[txt[i]];
				else if (txt[i] == '\n')
				{
					_x = x;
					_y += 10;
					continue;
				}
				else if (txt[i] == ' ')             //空白文字はないので飛ばす(空白は開く)
				{
					_x += 8;
					continue;
				}
				else
					target = 0;                     //それ以外なら、存在しない文字として出力する
				if (_x > xx)
				{
					_y += 10;
					_x = x;
				}
				if (_y > yy)
				{
					break;
				}
				if (target > 0)
					DX.DrawGraph(_x, _y, hFont[target], 1);
				else
					DX.DrawString(_x, _y, "" + txt[i], DX.GetColor(255, 255, 255));
				_x += ((txt[i] < 128) ? 8 : 10);

			}
			DX.SetDrawBright(255, 255, 255);
			//var p = GetDrawingSize(txt);
			//DX.DrawBox(x, y, x + p.Width, y + p.Height, DX.GetColor(255, 0, 0), 0);
		}

		/// <summary>
		/// 中央寄せして文字列を描画します。
		/// </summary>
		/// <param name="y"></param>
		/// <param name="txt"></param>
		/// <param name="color"></param>
		public static void DrawString(int y, string txt, int color)
		{
			foreach (var t in txt.Split('\n'))
			{
				DrawString(GameEngine.scrSize.Width / 2 - GetDrawingSize(t).Width / 2, y += 10, t, color);
			}
		}

		public static void DrawMiniString(int x, int y, string txt, int color)
		{
			if (!isInit) throw new Exception("Font Utility が初期化されていません。");
			int _x = x, _y = y;
			DX.SetDrawBright((color & 0xff0000) >> 16, (color & 0x00ff00) >> 8, color & 0x0000ff);
			for (int i = 0; i < txt.Length; i++)
			{
				int target;
				if (dicFont.ContainsKey(txt[i]))    //ある文字ならそれを指定
					target = dicFont[txt[i]];
				else if (txt[i] == '\n')
				{
					_x = x;
					_y += 8;
					continue;
				}
				else if (txt[i] == ' ')             //空白文字はないので飛ばす(空白は開く)
				{
					_x += 6;
					continue;
				}
				else                                //それ以外なら、存在しない文字として出力する
					target = 0;
				DX.DrawGraph(_x, _y, hFont_mini[target], 1);
				_x += ((txt[i] < 128) ? 6 : 8);
			}
			DX.SetDrawBright(255, 255, 255);
		}

		public static void DrawMiniString(int y, string txt, int color)
		{
			foreach (var t in txt.Split('\n'))
			{
				DrawMiniString(GameEngine.scrSize.Width / 2 - GetMiniDrawingSize(t).Width / 2, y += 10, t, color);
			}
		}

		public static void DrawString(int y, string txt, Color color)
		{
			DrawString(y, txt, color.R << 16 | color.G << 8 | color.B);
		}

		public static void DrawString(int x, int y, string txt, Color color)
		{
			DrawString(x, y, txt, color.R << 16 | color.G << 8 | color.B);
		}

		public static void DrawString(int x, int y, int xx, int yy, string txt, Color color)
		{
			DrawString(x, y, xx, yy, txt, color.R << 16 | color.G << 8 | color.B);
		}


		public static void DrawMiniString(int y, string txt, Color color)
		{
			DrawMiniString(y, txt, color.R << 16 | color.G << 8 | color.B);
		}

		public static void DrawMiniString(int x, int y, string txt, Color color)
		{
			DrawMiniString(x, y, txt, color.R << 16 | color.G << 8 | color.B);
		}

		public static Size GetDrawingSize(string txt)
		{
			if (!isInit)
				throw new Exception("Font Utility が初期化されていません。");
			int w = 0, h = 0;
			int bw = 0;
			for (int i = 0; i < txt.Length; i++)
			{
				int target;
				if (dicFont.ContainsKey(txt[i]))    //ある文字ならそれを指定
					target = dicFont[txt[i]];
				else if (txt[i] == '\n')
				{
					if (bw < w)
						bw = w;
					w = 0;
					h += 10;
					continue;
				}
				else if (txt[i] == ' ')             //空白文字はないので開ける
				{
					w += 8;
					continue;
				}

				w += ((txt[i] < 128) ? 8 : 10);
			}
			if (bw < w)
				bw = w;
			w = bw;
			h += 10;
			return new Size(w, h);
		}

		public static Size GetMiniDrawingSize(string txt)
		{
			if (!isInit)
				throw new Exception("Font Utility が初期化されていません。");
			int w = 0, h = 0, bw = 0;
			for (int i = 0; i < txt.Length; i++)
			{
				int target;
				if (dicFont.ContainsKey(txt[i]))    //ある文字ならそれを指定
					target = dicFont[txt[i]];
				else if (txt[i] == '\n')
				{
					if (bw < w)
						bw = w;
					w = 0;
					h += 8;
					continue;
				}
				else if (txt[i] == ' ')             //空白文字はないので飛ばす(空白は開く)
				{
					w += 6;
					continue;
				}
				w += ((txt[i] < 128) ? 6 : 8);
			}
			if (bw < w)
				bw = w;
			w = bw;
			h += 8;

			return new Size(w, h);
		}

	}

	/// <summary>
	/// 効果音データを取得し、再生する機能を提供する静的クラスです。
	/// </summary>
	public static class SoundUtility
	{
		static List<int> soundlist;

		/// <summary>
		/// SoundUtility を初期化します。使用前に必ず行ってください。
		/// </summary>
		public static void Init()
		{
			soundlist = new List<int>();
			string fn = "";
			int i = 0;
			while (File.Exists(fn = string.Format("Resources\\Sounds\\{0}.wav", i)))
			{
				int handle;
				if ((handle = DX.LoadSoundMem(fn)) == -1)
				{
					throw new Exception("効果音の読み込みに失敗しました。");
				}
				soundlist.Add(handle);
				i++;
			}
		}

		/// <summary>
		/// 指定されたサウンドを再生します。
		/// </summary>
		/// <param name="snd">再生するサウンド。</param>
		public static void PlaySound(Sounds snd)
		{
			if (snd == Sounds.Null)
				return;
			snd--;
			DX.PlaySoundMem(soundlist[(int)snd], DX.DX_PLAYTYPE_BACK);
		}

		public static void PlaySound(int snd)
		{
			if (snd == 0)
				return;
			snd--;
			DX.PlaySoundMem(soundlist[snd], DX.DX_PLAYTYPE_BACK);
		}


		/// <summary>
		/// 細かいパラメーターを設定し、指定されたサウンドを再生します。
		/// </summary>
		/// <param name="snd">再生するサウンド。</param>
		/// <param name="freq">周波数(100 ~ 100000)。</param>
		/// <param name="volume">音量(0 ~ 255)。</param>
		/// <param name="pan">パンポット(-255 ~ 255)。</param>
		public static void PlaySound(Sounds snd, int freq, int volume, int pan)
		{
			if (snd == Sounds.Null)
				return;
			snd--;
			DX.SetFrequencySoundMem(freq, soundlist[(int)snd]);
			DX.ChangeNextPlayVolumeSoundMem(volume, soundlist[(int)snd]);
			DX.ChangeNextPlayPanSoundMem(pan, soundlist[(int)snd]);
			if (DX.PlaySoundMem(soundlist[(int)snd], DX.DX_PLAYTYPE_BACK) == -1)
			{
				throw new Exception("再生に失敗しました");
			}
			DX.ChangeNextPlayVolumeSoundMem(255, soundlist[(int)snd]);
			DX.ChangeNextPlayPanSoundMem(0, soundlist[(int)snd]);

		}


	}

	/// <summary>
	/// 効果音の番号を指定します。
	/// </summary>
	public enum Sounds
	{
		/// <summary>
		/// サウンドを再生しない。
		/// </summary>
		Null,
		/// <summary>
		/// 0 コインの音。
		/// </summary>
		GetCoin,
		/// <summary>
		/// 1 小さなキャラがジャンプする音。
		/// </summary>
		SmallJump,
		/// <summary>
		/// 2 大きなキャラがジャンプする音。
		/// </summary>
		BigJump,
		/// <summary>
		/// 3 Citringo のジングル。
		/// </summary>
		Citringo,
		/// <summary>
		/// 4 プレイヤーが死んだ音。
		/// </summary>
		PlayerMiss,
		/// <summary>
		/// 5 踏み潰す音。
		/// </summary>
		Stepped,
		/// <summary>
		/// 6 悲鳴。
		/// </summary>
		Killed,
		/// <summary>
		/// 7 矢を射つ音。
		/// </summary>
		ShootArrow,
		/// <summary>
		/// 8 矢が刺さる音。
		/// </summary>
		StuckArrow,
		/// <summary>
		/// 9 着地音。
		/// </summary>
		Land,
		/// <summary>
		/// 10 水飛沫。
		/// </summary>
		WaterSplash,
		/// <summary>
		/// 11 破壊音。
		/// </summary>
		Destroy,
		/// <summary>
		/// 12 泳ぐ音。
		/// </summary>
		Swim,
		/// <summary>
		/// 13 主人公がパワーアップする音。
		/// </summary>
		PowerUp,
		/// <summary>
		/// 14 主人公がパワーダウンする音。
		/// </summary>
		PowerDown,
		/// <summary>
		/// 15 アイテムが出現する音。
		/// </summary>
		ItemSpawn,
		/// <summary>
		/// 16 プレイヤーのライフが上がる音。
		/// </summary>
		Player1Up,
		/// <summary>
		/// 17 主人公が武器のあるパワーアップをする音。
		/// </summary>
		GetWeapon,
		/// <summary>
		/// 18 ファイアーボールが投げられる音。
		/// </summary>
		ShootFire,
		/// <summary>
		/// 19 タイトルロゴが光るときの音。
		/// </summary>
		Flash,
		/// <summary>
		/// 20 無敵が終了する警告音。
		/// </summary>
		WarningMuteki,
		/// <summary>
		/// 21 食べる音1。
		/// </summary>
		Paku1,
		/// <summary>
		/// 22 食べる音2。
		/// </summary>
		Paku2,
		/// <summary>
		/// 23 何かが動く音。
		/// </summary>
		Poyo,
		/// <summary>
		/// 24 カーソルが確定される音。
		/// </summary>
		Pressed,
		/// <summary>
		/// 25 カーソルが移動する音。
		/// </summary>
		Selected,
		/// <summary>
		/// 26 しゃべる音。 
		/// </summary>
		Saying
	}

	/// <summary>
	/// 開発に便利な機能が用意されています。
	/// </summary>
	public static class DevelopUtility
	{
		/// <summary>
		/// 指定した値から一つをランダムに選んで返します。
		/// </summary>
		/// <param name="dat">値の集合。</param>
		/// <returns>値の集合から一つ選ばれたもの。</returns>
		public static T GetRandom<T>(params T[] dat)
		{
			Random rnd = new Random();
			return dat[rnd.Next(dat.Length)];
		}

		/// <summary>
		/// 指定した座標が、マップの範囲から外れているかどうか判定します。
		/// </summary>
		/// <param name="pnt">座標。</param>
		/// <returns>マップの範囲から外れていれば true が返されます。</returns>
		public static bool IsOutOfRange(this Point pnt)
		{
			return pnt.X < 0 || pnt.X > GameEngine.map.Width * 16 - 1||
				pnt.Y < 0 || pnt.Y > GameEngine.map.Height * 16 - 1;
		}

		/// <summary>
		/// 指定した座標が、マップの範囲から外れているかどうか判定します。
		/// </summary>
		/// <param name="pnt">座標。</param>
		/// <returns>マップの範囲から外れていれば true が返されます。</returns>
		public static bool IsOutOfRange(this PointF pnt)
		{
			return pnt.X < 0 || pnt.X > GameEngine.map.Width * 16 - 1 ||
				pnt.Y < 0 || pnt.Y > GameEngine.map.Height * 16 - 1;
		}

		/// <summary>
		/// 矩形同士の当たり判定を計算します。
		/// </summary>
		/// <param name="rect1">矩形。</param>
		/// <param name="rect2">矩形。</param>
		/// <returns>当たっているかどうか。</returns>
		public static bool CheckCollision(this Rectangle rect1, RectangleF rect2)
		{
			return (rect1.X < rect2.X + rect2.Width) && (rect2.X < rect1.X + rect1.Width) &&
				   (rect1.Y < rect2.Y + rect2.Height) && (rect2.Y < rect1.Y + rect1.Height);
		}

		/// <summary>
		/// 矩形同士の当たり判定を計算します。
		/// </summary>
		/// <param name="rect1">矩形。</param>
		/// <param name="rect2">矩形。</param>
		/// <returns>当たっているかどうか。</returns>
		public static bool CheckCollision(this RectangleF rect1, RectangleF rect2)
		{
			return (rect1.X < rect2.X + rect2.Width) && (rect2.X < rect1.X + rect1.Width) &&
				   (rect1.Y < rect2.Y + rect2.Height) && (rect2.Y < rect1.Y + rect1.Height);
		}

		/// <summary>
		/// 点と矩形の当たり判定を計算します。
		/// </summary>
		/// <param name="p">点。</param>
		/// <param name="r">矩形。</param>
		/// <returns>当たっているかどうか。</returns>
		public static bool CheckCollision(this Point p, RectangleF r)
		{
			return (r.X < p.X) && (p.X < r.X + r.Width) && (r.Y < p.Y) && (p.Y < r.Y + r.Height);
		}

		/// <summary>
		/// 点と矩形の当たり判定を計算します。
		/// </summary>
		/// <param name="p">点。</param>
		/// <param name="r">矩形。</param>
		/// <returns>当たっているかどうか。</returns>
		public static bool CheckCollision(this PointF p, RectangleF r)
		{
			return (r.X < p.X) && (p.X < r.X + r.Width) && (r.Y < p.Y) && (p.Y < r.Y + r.Height);
		}

		/// <summary>
		/// 度数法を弧度法に変換します。
		/// </summary>
		/// <param name="deg"></param>
		/// <returns></returns>
		public static double Deg2Rad(double deg)
		{
			return deg / 180 * Math.PI;
		}

		/// <summary>
		/// 弧度法を度数法に変換します。
		/// </summary>
		/// <param name="rad"></param>
		/// <returns></returns>
		public static double Rad2Deg(double rad)
		{
			return rad * 180 / Math.PI;
		}


	}

	public static class EventUtility
	{
		/*public static int ContinuousBonus(int bbonus)
		{
			int bonus = bonus;
			switch(bbonus)
			{
				case 0:

			}
		}*/
	}

}
