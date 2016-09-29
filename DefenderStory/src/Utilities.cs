using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using DxLibDLL;
using MusicSheet.Sequence;
using NextMidi.Data;
using TakeUpJewel.Entities;
using static DxLibDLL.DX;
using Object = TakeUpJewel.Data.Object;

namespace TakeUpJewel.Util
{
	/// <summary>
	///     ゲームのリソースを取得し管理する静的クラスです。
	/// </summary>
	public static class ResourceUtility
	{
		public const int TextureWidth = 256;
		public const int TextureHeight = 64;

		public static int BgBreakTime { get; set; }

		public static int BgJukeBox { get; set; }


		public static void Init()
		{
			MagicPlayer = new int[72];

			if (LoadDivGraph("Resources\\Graphics\\spplayer_magic.png", 72, 18, 4, 16, 32, out MagicPlayer[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			IcePlayer = new int[72];

			if (LoadDivGraph("Resources\\Graphics\\spplayer_ice.png", 72, 18, 4, 16, 32, out IcePlayer[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			FirePlayer = new int[72];

			if (LoadDivGraph("Resources\\Graphics\\spplayer_fire.png", 72, 18, 4, 16, 32, out FirePlayer[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			BigPlayer = new int[72];

			if (LoadDivGraph("Resources\\Graphics\\spplayer.png", 72, 18, 4, 16, 32, out BigPlayer[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			MiniPlayer = new int[72];

			if (LoadDivGraph("Resources\\Graphics\\spplayermini.png", 72, 18, 4, 16, 16, out MiniPlayer[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			MagicPlayerFemale = new int[72];

			if (LoadDivGraph("Resources\\Graphics\\spfemaleplayer_magic.png", 72, 18, 4, 16, 32, out MagicPlayerFemale[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			IcePlayerFemale = new int[72];

			if (LoadDivGraph("Resources\\Graphics\\spfemaleplayer_ice.png", 72, 18, 4, 16, 32, out IcePlayerFemale[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			FirePlayerFemale = new int[72];

			if (LoadDivGraph("Resources\\Graphics\\spfemaleplayer_fire.png", 72, 18, 4, 16, 32, out FirePlayerFemale[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			BigPlayerFemale = new int[72];

			if (LoadDivGraph("Resources\\Graphics\\spfemaleplayer.png", 72, 18, 4, 16, 32, out BigPlayerFemale[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			Queen = new int[20];

			if (LoadDivGraph("Resources\\Graphics\\spqueen.png", 20, 16, 2, 16, 32, out Queen[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			King = new int[20];

			if (LoadDivGraph("Resources\\Graphics\\spking.png", 20, 16, 2, 16, 32, out King[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			CommonMob = new int[144];

			if (LoadDivGraph("Resources\\Graphics\\commonMob.png", 144, 16, 4, 16, 16, out CommonMob[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			ModokeeGround = new int[8];

			if (LoadDivGraph("Resources\\Graphics\\spModokee.png", 8, 8, 1, 32, 16, out ModokeeGround[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			ModokeeCave = new int[8];

			if (LoadDivGraph("Resources\\Graphics\\spCaveModokee.png", 8, 8, 1, 32, 16, out ModokeeCave[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			Daemon = new int[14];

			if (LoadDivGraph("Resources\\Graphics\\spdaemon.png", 14, 14, 1, 16, 16, out Daemon[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			Archer = new int[14];

			if (LoadDivGraph("Resources\\Graphics\\sparcher.png", 8, 8, 1, 32, 32, out Archer[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			Weapon = new int[29];

			if (LoadDivGraph("Resources\\Graphics\\spweapon.png", 29, 16, 2, 16, 16, out Weapon[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			Dwarf = new int[5];

			if (LoadDivGraph("Resources\\Graphics\\spdwarf.png", 5, 5, 1, 16, 32, out Dwarf[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			Fighter = new int[2];

			if (LoadDivGraph("Resources\\Graphics\\spfighter.png", 2, 2, 1, 16, 32, out Fighter[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			Turbo = new int[6];

			if (LoadDivGraph("Resources\\Graphics\\spturbo.png", 6, 6, 1, 16, 16, out Turbo[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			Boxer = new int[6];

			if (LoadDivGraph("Resources\\Graphics\\spboxer.png", 2, 2, 1, 32, 32, out Boxer[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			RollingRock = new int[2];

			if (LoadDivGraph("Resources\\Graphics\\sprollingrock.png", 2, 2, 1, 32, 32, out RollingRock[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");


			Turcos = new int[12];

			if (LoadDivGraph("Resources\\Graphics\\spTurcos.png", 12, 11, 2, 24, 16, out Turcos[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			TurcosShell = new int[4];

			if (LoadDivGraph("Resources\\Graphics\\spTurcosShell.png", 4, 4, 1, 16, 16, out TurcosShell[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			Spider = new int[1];

			if (LoadDivGraph("Resources\\Graphics\\spSpider.png", 1, 1, 1, 16, 16, out Spider[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			Logo = new int[2];

			if (LoadDivGraph("Resources\\Graphics\\logo.png", 2, 2, 1, 180, 101, out Logo[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			Item = new int[14];

			if (LoadDivGraph("Resources\\Graphics\\spitem.png", 14, 14, 1, 16, 16, out Item[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			Densy = new int[4];

			if (LoadDivGraph("Resources\\Graphics\\spDensy.png", 4, 4, 1, 16, 16, out Densy[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			FolderFly = new int[5];

			if (LoadDivGraph("Resources\\Graphics\\spFolderFly.png", 5, 5, 1, 16, 16, out FolderFly[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			BlackServer = new int[10];

			if (LoadDivGraph("Resources\\Graphics\\spblackserver.png", 10, 10, 1, 16, 32, out BlackServer[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			CameraMan = new int[6];

			if (LoadDivGraph("Resources\\Graphics\\spCameraMan.png", 6, 6, 1, 16, 32, out CameraMan[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			Particle = new int[8];

			if (LoadDivGraph("Resources\\Graphics\\spparticle.png", 8, 8, 1, 8, 8, out Particle[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			StrangeFlower = new int[5];

			if (LoadDivGraph("Resources\\Graphics\\spstrangeflower.png", 5, 5, 1, 48, 48, out StrangeFlower[0]) == -1)
				throw new Exception("キャラの読み込みに失敗しました。");

			if ((TheEnd = LoadGraph("Resources\\Graphics\\theend.png")) == 0)
				throw new Exception("キャラの読み込みに失敗しました。");

			if ((MesBox = LoadGraph("Resources\\Graphics\\uimesbox.png")) == 0)
				throw new Exception("キャラの読み込みに失敗しました。");

			if ((BgJukeBox = LoadGraph("Resources\\Graphics\\bgjukebox.bmp")) == 0)
				throw new Exception("キャラの読み込みに失敗しました。");

			if ((BgBreakTime = LoadGraph("Resources\\Graphics\\bgbreaktime.bmp")) == 0)
				throw new Exception("キャラの読み込みに失敗しました。");

			foreach (var s in Directory.GetFiles(".\\Resources\\Music"))
				MusicList[Path.GetFileName(s)] = Sequencer.LoadSmf(s);
		}

		/// <summary>
		///     指定した画像ファイルを、指定した幅と高さで分割してキャッシュします。
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns>GraphicList 辞書のキーである、テクスチャファイル名のハッシュ。</returns>
		public static int LoadTexture(string texture, int width, int height)
		{
			var xNum = TextureWidth / width;
			var yNum = TextureHeight / height;
			var buf = new int[xNum * yNum];
			var hash = texture.GetHashCode();
			if (GraphicList.ContainsKey(hash) && (GraphicList[hash] != null))
				return hash;

			LoadDivGraph($"Resources\\Graphics\\{texture}", xNum * yNum, xNum, yNum, width, height, out buf[0]);
			GraphicList[hash] = buf;
			if (_texInfoList.ContainsKey(hash) && (_texInfoList[hash] != null))
				return hash;
			_texInfoList[hash] = new TextureLoadingInfo(texture, new Size(width, height));
			return hash;
		}

		public static void ReloadTexture()
		{
			foreach (var kvp in GraphicList)
				foreach (var i in kvp.Value)
					DeleteGraph(i);
			GraphicList.Clear();
			foreach (var p in _texInfoList)
			{
				var i = p.Value;
				LoadTexture(i.Texture, i.Size.Width, i.Size.Height);
			}
		}

		public static int[] GetMpt(string mptname)
		{
			int[] m = new int[64],
				mm = new int[256];
			LoadDivGraph("Resources\\Graphics\\" + mptname + ".png", 64, 16, 4, 16, 16, out m[0]);
			MapChip = m;
			LoadDivGraph("Resources\\Graphics\\" + mptname + ".png", 256, 32, 8, 8, 8, out mm[0]);
			MapChipMini = mm;

			return m;
		}

		private class TextureLoadingInfo
		{
			public TextureLoadingInfo(string texture, Size size)
			{
				Texture = texture;
				Size = size;
			}

			public string Texture { get; }
			public Size Size { get; }
		}

		#region Properties

		/// <summary>
		///     ファイアープレイヤー。
		/// </summary>
		public static int[] FirePlayer { get; private set; }

		/// <summary>
		///     アイスプレイヤー。
		/// </summary>
		public static int[] IcePlayer { get; private set; }

		/// <summary>
		///     リーフプレイヤー。
		/// </summary>
		public static int[] MagicPlayer { get; private set; }

		/// <summary>
		///     スーパープレイヤー。
		/// </summary>
		public static int[] BigPlayer { get; private set; }

		public static int[] Queen { get; private set; }
		public static int[] King { get; private set; }

		/// <summary>
		///     ファイアープレイヤー。
		/// </summary>
		public static int[] FirePlayerFemale { get; private set; }

		/// <summary>
		///     アイスプレイヤー。
		/// </summary>
		public static int[] IcePlayerFemale { get; private set; }

		/// <summary>
		///     リーフプレイヤー。
		/// </summary>
		public static int[] MagicPlayerFemale { get; private set; }

		/// <summary>
		///     スーパープレイヤー。
		/// </summary>
		public static int[] BigPlayerFemale { get; private set; }

		/// <summary>
		///     プレイヤー。
		/// </summary>
		public static int[] MiniPlayer { get; private set; }

		/// <summary>
		///     雑魚キャラ。
		/// </summary>
		public static int[] CommonMob { get; private set; }

		/// <summary>
		///     地上に生息するモドキー。
		/// </summary>
		public static int[] ModokeeGround { get; private set; }

		/// <summary>
		///     地下に生息するモドキー。
		/// </summary>
		public static int[] ModokeeCave { get; private set; }

		/// <summary>
		///     デーモン。
		/// </summary>
		public static int[] Daemon { get; private set; }

		/// <summary>
		///     アーチャー。
		/// </summary>
		public static int[] Archer { get; private set; }

		/// <summary>
		///     武器など。
		/// </summary>
		public static int[] Weapon { get; private set; }

		/// <summary>
		///     ドワーフ。
		/// </summary>
		public static int[] Dwarf { get; private set; }

		/// <summary>
		///     クモ。
		/// </summary>
		public static int[] Spider { get; private set; }

		/// <summary>
		///     ターコス。
		/// </summary>
		public static int[] Turcos { get; private set; }

		/// <summary>
		///     ターコスの甲羅。
		/// </summary>
		public static int[] TurcosShell { get; private set; }

		/// <summary>
		///     アイテム。
		/// </summary>
		public static int[] Item { get; private set; }

		/// <summary>
		///     パーティクル。
		/// </summary>
		public static int[] Particle { get; private set; }

		/// <summary>
		///     マップチップ。
		/// </summary>
		public static int[] MapChip { get; private set; }

		/// <summary>
		///     ボクサー。
		/// </summary>
		public static int[] Boxer { get; private set; }

		/// <summary>
		///     転がる岩。
		/// </summary>
		public static int[] RollingRock { get; private set; }

		/// <summary>
		///     ターボ。
		/// </summary>
		public static int[] Turbo { get; private set; }

		/// <summary>
		///     ファイター。
		/// </summary>
		public static int[] Fighter { get; private set; }

		/// <summary>
		///     デンジー。
		/// </summary>
		public static int[] Densy { get; private set; }

		/// <summary>
		///     フォルダーフライ。
		/// </summary>
		public static int[] FolderFly { get; private set; }

		/// <summary>
		///     ブラックサーバー。
		/// </summary>
		public static int[] BlackServer { get; private set; }

		/// <summary>
		///     カメラマン。
		/// </summary>
		public static int[] CameraMan { get; private set; }

		/// <summary>
		///     マップチップ。
		/// </summary>
		public static int[] Logo { get; private set; }

		/// <summary>
		///     8pxに分割されたマップチップ。ブロック破壊時などに使う。
		/// </summary>
		public static int[] MapChipMini { get; private set; }

		public static int[] StrangeFlower { get; private set; }

		public static int TheEnd { get; private set; }

		public static int MesBox { get; private set; }

		public static Dictionary<string, MidiData> MusicList { get; } = new Dictionary<string, MidiData>();

		public static Dictionary<int, int[]> GraphicList { get; } = new Dictionary<int, int[]>();

		private static readonly Dictionary<int, TextureLoadingInfo> _texInfoList = new Dictionary<int, TextureLoadingInfo>();

		#endregion
	}

	/// <summary>
	///     パーティクルの処理をまとめた静的クラスです。
	/// </summary>
	public static class ParticleUtility
	{
		public static int Tick;

		/// <summary>
		///     泡を出します。
		/// </summary>
		/// <param name="sp">泡を出すエンティティ。</param>
		public static void Bubble(Entity sp)
		{
			if (Tick % 65 == 0)
				sp.Parent.Add(GameEngine.EntityRegister.CreateEntity("Bubble",
					new PointF((int) sp.Location.X + sp.Size.Width / 2 - 4, (int) sp.Location.Y + sp.Size.Height / 2 - 4), sp.Mpts,
					sp.Map, sp.Parent));
			Tick = (Tick + 1) % 3600;
		}

		/// <summary>
		///     水しぶきを上げます。
		/// </summary>
		/// <param name="sp">水しぶきを上げるエンティティ。</param>
		public static void WaterSplash(Entity sp)
		{
			for (var i = 0; i < 13 + GetRand(4); i++)
				sp.Parent.Add(GameEngine.EntityRegister.CreateEntity("WaterSplash",
					new Point((int) sp.Location.X + GetRand(sp.Size.Width), (int) sp.Location.Y + sp.Size.Height / 2), sp.Mpts, sp.Map,
					sp.Parent));
		}

		public static void BrokenBlock(Point pos, EntityList spcolle, Object[] mptobjects)
		{
			for (var i = 0; i < 3 + GetRand(3); i++)
				spcolle.Add(GameEngine.EntityRegister.CreateEntity("BrokenBlock",
					new Point(pos.X + GetRand(8) - 4, pos.Y + GetRand(8) - 4), mptobjects, GameEngine.Chips, spcolle));
		}
	}

	/// <summary>
	///     フォントデータを取得し、文字列描画機能を提供する静的クラスです。
	/// </summary>
	public static class FontUtility
	{
		private static int[] _hFont;
		private static int[] _hFontMini;
		private static readonly Dictionary<char, int> DicFont = new Dictionary<char, int>();
		private static readonly Dictionary<char, int> NativeFont = new Dictionary<char, int>();
		private static bool _isInit;

		public static void Init()
		{
			_hFont = new int[384];
			LoadDivGraph("Resources\\Graphics\\font.png", 384, 16, 24, 10, 10, out _hFont[0]);
			_hFontMini = new int[320];
			LoadDivGraph("Resources\\Graphics\\font_mini.png", 320, 16, 20, 8, 8, out _hFontMini[0]);
			var i = 1; //0番目は、存在しない文字があったときに表示する文字なので、1から始まる
			foreach (var c in File.ReadAllText("Resources\\Document\\char.txt"))
				DicFont[c] = i++; //文字番号を char によって指定できるよう登録する
			ChangeFont("MS Gothic");
			SetFontSize(11);
			//DX.ChangeFontType(DX.DX_FONTTYPE_ANTIALIASING);
			SetFontThickness(1);
			_isInit = true;
		}

		/// <summary>
		///     文字列を描画します。
		/// </summary>
		/// <param name="x">描画する始点 X 座標。</param>
		/// <param name="y">描画する始点 Y 座標。</param>
		/// <param name="txt">描画する文字列。</param>
		/// <param name="color">色。</param>
		public static void DrawString(int x, int y, string txt, int color)
		{
			if (!_isInit) throw new Exception("Font Utility が初期化されていません。");
			int tx = x, ty = y;
			SetDrawBright((color & 0xff0000) >> 16, (color & 0x00ff00) >> 8, color & 0x0000ff);
			for (var i = 0; i < txt.Length; i++)
			{
				int target;
				if (DicFont.ContainsKey(txt[i]))
					//ある文字ならそれを指定
					target = DicFont[txt[i]];
				else if ((txt[i] == '\r') || (txt[i] == '\n'))
				{
					if ((i + 1 < txt.Length) && (txt[i] == '\r') && (txt[i + 1] == '\n'))
						i++;
					tx = x;
					ty += 10;
					continue;
				}
				else if ((txt[i] == ' ') || (txt[i] == '　'))
				{
					//空白文字はないので飛ばす(空白は開く)
					tx += 8;
					continue;
				}
				else
					//それ以外なら、存在しない文字として出力する
					target = 0;

				DrawGraph(tx, ty, target > 0 ? _hFont[target] : GetNativeFont(txt[i]), TRUE);
				//DX.DrawString(_x, _y, "" + txt[i], GetColor(255, 255, 255));

				tx += txt[i] < 128 ? 8 : 10;
			}
			SetDrawBright(255, 255, 255);
			//var p = GetDrawingSize(txt);
			//DX.DrawBox(x, y, x + p.Width, y + p.Height, DX.GetColor(255, 0, 0), 0);
		}

		private static int GetNativeFont(char c)
		{
			if (NativeFont.ContainsKey(c))
				return NativeFont[c];
			int r, g, b;
			GetDrawBright(out r, out g, out b);
			SetDrawBright(255, 255, 255);
			var beforeScreen = GetDrawScreen();
			var fontScreen = MakeScreen(10, 10, TRUE);
			SetDrawScreen(fontScreen);
			DX.DrawString(0, 0, c.ToString(), GetColor(255, 255, 255));
			SetDrawScreen(beforeScreen);
			SetDrawBright(r, g, b);
			return NativeFont[c] = fontScreen;
		}

		public static void DrawString(int x, int y, int xx, int yy, string txt, int color)
		{
			if (!_isInit)
				throw new Exception("Font Utility が初期化されていません。");
			int tx = x, ty = y;
			SetDrawBright((color & 0xff0000) >> 16, (color & 0x00ff00) >> 8, color & 0x0000ff);
			for (var i = 0; i < txt.Length; i++)
			{
				int target;
				if (DicFont.ContainsKey(txt[i])) //ある文字ならそれを指定
					target = DicFont[txt[i]];
				else if ((txt[i] == '\r') || (txt[i] == '\n'))
				{
					if ((i + 1 < txt.Length) && (txt[i] == '\r') && (txt[i + 1] == '\n'))
						i++;
					tx = x;
					ty += 10;
					continue;
				}
				else if ((txt[i] == ' ') || (txt[i] == '　')) //空白文字はないので飛ばす(空白は開く)
				{
					tx += 8;
					continue;
				}
				else
					target = 0; //それ以外なら、存在しない文字として出力する
				if (tx > xx)
				{
					ty += 10;
					tx = x;
				}
				if (ty > yy)
					break;
				if (target > 0)
					DrawGraph(tx, ty, _hFont[target], 1);
				else
					DX.DrawString(tx, ty, "" + txt[i], GetColor(255, 255, 255));
				tx += txt[i] < 128 ? 8 : 10;
			}
			SetDrawBright(255, 255, 255);
			//var p = GetDrawingSize(txt);
			//DX.DrawBox(x, y, x + p.Width, y + p.Height, DX.GetColor(255, 0, 0), 0);
		}

		/// <summary>
		///     中央寄せして文字列を描画します。
		/// </summary>
		/// <param name="y"></param>
		/// <param name="txt"></param>
		/// <param name="color"></param>
		public static void DrawString(int y, string txt, int color)
		{
			foreach (var t in txt.Split('\n'))
				DrawString(GameEngine.ScrSize.Width / 2 - GetDrawingSize(t).Width / 2, y += 10, t, color);
		}

		public static void DrawMiniString(int x, int y, string txt, int color)
		{
			if (!_isInit) throw new Exception("Font Utility が初期化されていません。");
			int tx = x, ty = y;
			SetDrawBright((color & 0xff0000) >> 16, (color & 0x00ff00) >> 8, color & 0x0000ff);
			for (var i = 0; i < txt.Length; i++)
			{
				int target;
				if (DicFont.ContainsKey(txt[i])) //ある文字ならそれを指定
					target = DicFont[txt[i]];
				else if ((txt[i] == '\r') || (txt[i] == '\n'))
				{
					if ((i + 1 < txt.Length) && (txt[i] == '\r') && (txt[i + 1] == '\n'))
						i++;
					tx = x;
					ty += 8;
					continue;
				}
				else if ((txt[i] == ' ') || (txt[i] == '　')) //空白文字はないので飛ばす(空白は開く)
				{
					tx += 6;
					continue;
				}
				else //それ以外なら、存在しない文字として出力する
					target = 0;
				DrawGraph(tx, ty, _hFontMini[target], 1);
				tx += txt[i] < 128 ? 6 : 8;
			}
			SetDrawBright(255, 255, 255);
		}

		public static void DrawMiniString(int y, string txt, int color)
		{
			foreach (var t in txt.Split('\n'))
				DrawMiniString(GameEngine.ScrSize.Width / 2 - GetMiniDrawingSize(t).Width / 2, y += 10, t, color);
		}

		public static void DrawString(int y, string txt, Color color)
		{
			DrawString(y, txt, (color.R << 16) | (color.G << 8) | color.B);
		}

		public static void DrawString(int x, int y, string txt, Color color)
		{
			DrawString(x, y, txt, (color.R << 16) | (color.G << 8) | color.B);
		}

		public static void DrawString(int x, int y, int xx, int yy, string txt, Color color)
		{
			DrawString(x, y, xx, yy, txt, (color.R << 16) | (color.G << 8) | color.B);
		}


		public static void DrawMiniString(int y, string txt, Color color)
		{
			DrawMiniString(y, txt, (color.R << 16) | (color.G << 8) | color.B);
		}

		public static void DrawMiniString(int x, int y, string txt, Color color)
		{
			DrawMiniString(x, y, txt, (color.R << 16) | (color.G << 8) | color.B);
		}

		public static Size GetDrawingSize(string txt)
		{
			if (!_isInit)
				throw new Exception("Font Utility が初期化されていません。");
			int w = 0, h = 0;
			var bw = 0;
			for (var i = 0; i < txt.Length; i++)
			{
				if (!DicFont.ContainsKey(txt[i]))
					switch (txt[i])
					{
						case '\r':
						case '\n':
							if (bw < w)
								bw = w;
							if ((i + 1 < txt.Length) && (txt[i] == '\r') && (txt[i + 1] == '\n'))
								i++;
							w = 0;
							h += 10;
							continue;
						case ' ':
						case '　':
							w += 8;
							continue;
					}

				w += txt[i] < 128 ? 8 : 10;
			}
			if (bw < w)
				bw = w;
			w = bw;
			h += 10;
			return new Size(w, h);
		}

		public static Size GetMiniDrawingSize(string txt)
		{
			if (!_isInit)
				throw new Exception("Font Utility が初期化されていません。");
			int w = 0, h = 0, bw = 0;
			for (var i = 0; i < txt.Length; i++)
			{
				if (!DicFont.ContainsKey(txt[i]))
					switch (txt[i])
					{
						case '\r':
						case '\n':
							if (bw < w)
								bw = w;
							if ((i + 1 < txt.Length) && (txt[i] == '\r') && (txt[i + 1] == '\n'))
								i++;
							w = 0;
							h += 8;
							continue;
						case ' ':
						case '　':
							w += 6;
							continue;
					}
				w += txt[i] < 128 ? 6 : 8;
			}
			if (bw < w)
				bw = w;
			w = bw;
			h += 8;

			return new Size(w, h);
		}
	}

	/// <summary>
	///     効果音データを取得し、再生する機能を提供する静的クラスです。
	/// </summary>
	public static class SoundUtility
	{
		private static List<int> _soundlist;

		/// <summary>
		///     SoundUtility を初期化します。使用前に必ず行ってください。
		/// </summary>
		public static void Init()
		{
			_soundlist = new List<int>();
			string fn;
			var i = 0;
			while (File.Exists(fn = $"Resources\\Sounds\\{i}.wav"))
			{
				int handle;
				if ((handle = LoadSoundMem(fn)) == -1)
					throw new Exception("効果音の読み込みに失敗しました。");
				_soundlist.Add(handle);
				i++;
			}
		}

		/// <summary>
		///     指定されたサウンドを再生します。
		/// </summary>
		/// <param name="snd">再生するサウンド。</param>
		public static void PlaySound(Sounds snd)
		{
			if (snd == Sounds.Null)
				return;
			snd--;
			PlaySound((int) snd);
		}

		public static void PlaySound(int snd)
		{
			if (snd == -1)
				return;
			PlaySoundMem(_soundlist[snd], DX_PLAYTYPE_BACK);
			ChangeVolumeSoundMem(180, _soundlist[snd]);
		}


		/// <summary>
		///     細かいパラメーターを設定し、指定されたサウンドを再生します。
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
			SetFrequencySoundMem(freq, _soundlist[(int) snd]);
			ChangeNextPlayVolumeSoundMem(volume, _soundlist[(int) snd]);
			ChangeNextPlayPanSoundMem(pan, _soundlist[(int) snd]);
			if (PlaySoundMem(_soundlist[(int) snd], DX_PLAYTYPE_BACK) == -1)
				throw new Exception("再生に失敗しました");
			ChangeNextPlayVolumeSoundMem(255, _soundlist[(int) snd]);
			ChangeNextPlayPanSoundMem(0, _soundlist[(int) snd]);
		}
	}


	/// <summary>
	///     効果音の番号を指定します。
	/// </summary>
	public enum Sounds
	{
		/// <summary>
		///     サウンドを再生しない。
		/// </summary>
		Null,

		/// <summary>
		///     0 コインの音。
		/// </summary>
		GetCoin,

		/// <summary>
		///     1 小さなキャラがジャンプする音。
		/// </summary>
		SmallJump,

		/// <summary>
		///     2 大きなキャラがジャンプする音。
		/// </summary>
		BigJump,

		/// <summary>
		///     3 Citringo のジングル。
		/// </summary>
		Citringo,

		/// <summary>
		///     4 プレイヤーが死んだ音。
		/// </summary>
		PlayerMiss,

		/// <summary>
		///     5 踏み潰す音。
		/// </summary>
		Stepped,

		/// <summary>
		///     6 悲鳴。
		/// </summary>
		Killed,

		/// <summary>
		///     7 矢を射つ音。
		/// </summary>
		ShootArrow,

		/// <summary>
		///     8 矢が刺さる音。
		/// </summary>
		StuckArrow,

		/// <summary>
		///     9 着地音。
		/// </summary>
		Land,

		/// <summary>
		///     10 水飛沫。
		/// </summary>
		WaterSplash,

		/// <summary>
		///     11 破壊音。
		/// </summary>
		Destroy,

		/// <summary>
		///     12 泳ぐ音。
		/// </summary>
		Swim,

		/// <summary>
		///     13 主人公がパワーアップする音。
		/// </summary>
		PowerUp,

		/// <summary>
		///     14 主人公がパワーダウンする音。
		/// </summary>
		PowerDown,

		/// <summary>
		///     15 アイテムが出現する音。
		/// </summary>
		ItemSpawn,

		/// <summary>
		///     16 プレイヤーのライフが上がる音。
		/// </summary>
		Player1Up,

		/// <summary>
		///     17 主人公が武器のあるパワーアップをする音。
		/// </summary>
		GetWeapon,

		/// <summary>
		///     18 ファイアーボールが投げられる音。
		/// </summary>
		ShootFire,

		/// <summary>
		///     19 タイトルロゴが光るときの音。
		/// </summary>
		Flash,

		/// <summary>
		///     20 無敵が終了する警告音。
		/// </summary>
		WarningMuteki,

		/// <summary>
		///     21 食べる音1。
		/// </summary>
		Paku1,

		/// <summary>
		///     22 食べる音2。
		/// </summary>
		Paku2,

		/// <summary>
		///     23 何かが動く音。
		/// </summary>
		Poyo,

		/// <summary>
		///     24 カーソルが確定される音。
		/// </summary>
		Pressed,

		/// <summary>
		///     25 カーソルが移動する音。
		/// </summary>
		Selected,

		/// <summary>
		///     26 しゃべる音。
		/// </summary>
		Speaking,

		/// <summary>
		///     27 シャッターを閉める音。
		/// </summary>
		Shutter,

		/// <summary>
		///     28 レーザーが発射される音。
		/// </summary>
		Razer,

		/// <summary>
		///     29 カメラがオートフォーカスする音。
		/// </summary>
		Focus,

		/// <summary>
		///     30 入力を取り消したときの音。
		/// </summary>
		Back,

		/// <summary>
		///     31 回復したときの音。
		/// </summary>
		LifeUp,

		/// <summary>
		///     32 何かに入るときの音。
		/// </summary>
		Into,
		Explode,
		Dumping,
		BalloonBroken
	}

	/// <summary>
	///     開発に便利な機能が用意されています。
	/// </summary>
	public static class DevelopUtility
	{
		/// <summary>
		///     指定した値から一つをランダムに選んで返します。
		/// </summary>
		/// <param name="dat">値の集合。</param>
		/// <returns>値の集合から一つ選ばれたもの。</returns>
		public static T GetRandom<T>(params T[] dat)
		{
			var rnd = new Random();
			return dat[rnd.Next(dat.Length)];
		}

		public static EventScript GetItemDescription(params string[] setumei)
		{
			var sb = new StringBuilder();
			sb.Append(@"[wait:40][enstop][mesbox:down]");
			foreach (var s in setumei)
				sb.Append($"[mes:{s}]");
			sb.Append("[mesend][enstart]");
			return new EventScript(sb.ToString());
		}

		public static float GetLengthTo(this PointF p1, PointF p2)
			=> (float) Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));

		/// <summary>
		///     指定した座標が、マップの範囲から外れているかどうか判定します。
		/// </summary>
		/// <param name="pnt">座標。</param>
		/// <returns>マップの範囲から外れていれば true が返されます。</returns>
		public static bool IsOutOfRange(this Point pnt) => (pnt.X < 0) || (pnt.X > GameEngine.Map.Width * 16 - 1) ||
														   (pnt.Y < 0) || (pnt.Y > GameEngine.Map.Height * 16 - 1);


		/// <summary>
		///     指定した座標が、マップの範囲から外れているかどうか判定します。
		/// </summary>
		/// <param name="pnt">座標。</param>
		/// <returns>マップの範囲から外れていれば true が返されます。</returns>
		public static bool IsOutOfRange(this PointF pnt) => (pnt.X < 0) || (pnt.X > GameEngine.Map.Width * 16 - 1) ||
															(pnt.Y < 0) || (pnt.Y > GameEngine.Map.Height * 16 - 1);

		/// <summary>
		///     矩形同士の当たり判定を計算します。
		/// </summary>
		/// <param name="rect1">矩形。</param>
		/// <param name="rect2">矩形。</param>
		/// <returns>当たっているかどうか。</returns>
		public static bool CheckCollision(this Rectangle rect1, RectangleF rect2)
			=> (rect1.X < rect2.X + rect2.Width) && (rect2.X < rect1.X + rect1.Width) &&
			   (rect1.Y < rect2.Y + rect2.Height) && (rect2.Y < rect1.Y + rect1.Height);

		/// <summary>
		///     矩形同士の当たり判定を計算します。
		/// </summary>
		/// <param name="rect1">矩形。</param>
		/// <param name="rect2">矩形。</param>
		/// <returns>当たっているかどうか。</returns>
		public static bool CheckCollision(this RectangleF rect1, RectangleF rect2)
			=> (rect1.X < rect2.X + rect2.Width) && (rect2.X < rect1.X + rect1.Width) &&
			   (rect1.Y < rect2.Y + rect2.Height) && (rect2.Y < rect1.Y + rect1.Height);

		/// <summary>
		///     点と矩形の当たり判定を計算します。
		/// </summary>
		/// <param name="p">点。</param>
		/// <param name="r">矩形。</param>
		/// <returns>当たっているかどうか。</returns>
		public static bool CheckCollision(this Point p, RectangleF r)
			=> (r.X < p.X) && (p.X < r.X + r.Width) && (r.Y < p.Y) && (p.Y < r.Y + r.Height);

		/// <summary>
		///     点と矩形の当たり判定を計算します。
		/// </summary>
		/// <param name="p">点。</param>
		/// <param name="r">矩形。</param>
		/// <returns>当たっているかどうか。</returns>
		public static bool CheckCollision(this PointF p, RectangleF r)
			=> (r.X < p.X) && (p.X < r.X + r.Width) && (r.Y < p.Y) && (p.Y < r.Y + r.Height);

		/// <summary>
		///     度数法を弧度法に変換します。
		/// </summary>
		/// <param name="deg"></param>
		/// <returns></returns>
		public static double Deg2Rad(double deg) => deg / 180 * Math.PI;

		/// <summary>
		///     弧度法を度数法に変換します。
		/// </summary>
		/// <param name="rad"></param>
		/// <returns></returns>
		public static double Rad2Deg(double rad) => rad * 180 / Math.PI;
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