//#define DEMO_MODE

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Codeplex.Data;
using MusicSheet.Sequence;
using TakeUpJewel.Data;
using TakeUpJewel.Entities;
using TakeUpJewel.Map;
using TakeUpJewel.Util;
using static DxLibDLL.DX;
using Object = TakeUpJewel.Data.Object;


namespace TakeUpJewel
{

	/// <summary>
	/// ゲームループのモードを指定します。
	/// </summary>
	public enum GameMode
	{
		/// <summary>
		/// タイトル画面モード。
		/// </summary>
		Title,
		/// <summary>
		/// ストーリーモード。
		/// </summary>
		Story,
		/// <summary>
		/// アクションゲームモード。
		/// </summary>
		Action,
		/// <summary>
		/// エンディングモード。
		/// </summary>
		Ending,
		/// <summary>
		/// デバッグモード。
		/// </summary>
		Debug,
		/// <summary>
		/// 休憩モード。
		/// </summary>
		Breaktime,
		/// <summary>
		/// あらすじ。
		/// </summary>
		Prolog
	}

	/// <summary>
	/// タイトル画面のモードを指定します。
	/// </summary>
	public enum TitleMode
	{
		/// <summary>
		/// オープニングムービー。
		/// </summary>
		Opening,
		/// <summary>
		/// ゲームロゴが上がる。
		/// </summary>
		GameLogoRising,
		/// <summary>
		/// ゲームロゴが光る。
		/// </summary>
		GameLogoLightning,
		/// <summary>
		/// メインタイトル画面。
		/// </summary>
		MainTitle,
		/// <summary>
		/// 設定画面。
		/// </summary>
		Setting,
		/// <summary>
		/// ヘルプ画面。
		/// </summary>
		Help,
		/// <summary>
		/// BGM 視聴モード。
		/// </summary>
		JukeBox
	}

	/// <summary>
	/// エンディングのモードを指定します。
	/// </summary>
	public enum EndingMode
	{
		Message,
		Credit,
		TheEnd
	}

	/// <summary>
	/// ゲームモードを制御する静的クラスです。
	/// </summary>
	public static class GameEngine
	{
		#region フィールド
		static int _bsec, _level = 1, _area;
		public static Size Map;
		public static byte[,,] Chips;
		static EntityList _entitylist;
		static string _mptname;
		public static Object[] Mptobjects;
		static int _f, _fps;
		static bool _binz;
		public static Point Camera = new Point(0, 0);
		public static int Time;
		static bool _isDyingflag, _goalflag;
		public static int Tick;
		public static Sequencer Seq;
		public static Status Ks;
		public static Size ScrSize;
		static int[] _hndlMpt;
		static int _hndlBg;
		static int _hndlFg;

		public static AreaInfo Areainfo;
		public static LevelData Data;
		public static EntityRegister EntityRegister = new EntityRegister();
		#endregion
		/// <summary>
		/// 現在所持しているコインの枚数を取得または設定します。
		/// </summary>
		public static int Coin { get; set; }
		/// <summary>
		/// 現在残っているライフ数を取得または設定します。
		/// </summary>
		public static int Life { get; set; }
		/// <summary>
		/// 現時点で立てた中間フラグの指す座標を取得または設定します。
		/// </summary>
		public static PointF Middle { get; set; }
		/// <summary>
		/// ゴール中であるかどうかを取得または設定します。
		/// </summary>
		public static bool IsGoal { get; set; }
		/// <summary>
		/// ゲームオーバーであるかどうかを取得または設定します。
		/// </summary>
		public static bool IsGameOver { get; set; }
		/// <summary>
		/// 次のステージを取得または設定します。
		/// </summary>
		public static int NextStage { get; set; }
		/// <summary>
		/// デバッグモードであるかどうかを取得または設定します。
		/// </summary>
		public static bool IsDebugMode;
		/// <summary>
		/// 初期化したかどうかを取得または設定します。
		/// </summary>
		public static bool IsInit { get; set; }
		static int _bcoin;
		/// <summary>
		/// 開始レベルを定義します。
		/// </summary>
		public const int StartLevel = 1;

		public const string HelpType = "kb";

		/// <summary>
		/// 現在のゲームモード。
		/// </summary>
		public static GameMode Gamemode;

		/// <summary>
		/// ゲームエンジンを初期化します。
		/// </summary>
		public static void Init(int w, int h)
		{
			Init(1, new Size(w, h));
		}

		/// <summary>
		/// 画面サイズを指定して、ゲームエンジンを初期化します。
		/// </summary>
		/// <param name="s"></param>
		public static void Init(Size s)
		{
			Init(1, s);

		}

		/// <summary>
		/// 開始レベルを指定して、ゲームエンジンを初期化します。
		/// </summary>
		/// <param name="level"></param>
		public static void Init(int level)
		{
			Init(level, new Size(320, 240));

		}

		/// <summary>
		/// 開始レベルおよび画面サイズを指定して、ゲームエンジンを初期化します。
		/// </summary>
		/// <param name="level"></param>
		/// <param name="s"></param>
		public static void Init(int level, Size s)
		{
			#region 変数初期化
			_bsec = DateTime.Now.Second;
			level = StartLevel;
			Map = new Size(20, 15);
			MakeScreen(s.Width, s.Height);
			_entitylist = new EntityList();
			Coin = 0;
			Life = 5;
			Time = 300;
			IsGoal = false;
			NextStage = 0;
			ScrSize = s;
			IsInit = true;
			Gamemode = GameMode.Title;
			HMainScreen = MakeScreen(s.Width, s.Height);

			_splashtime = 0;
			_endmode = EndingMode.Message;
			_height = 0;
			_gametick = 0;
			_titlemode = TitleMode.Opening;
			_storyflag = false;
			_storyline = 0;
			_serif = "";
			_storyimg = "";
			_storywaiting = false;
			_nowserif = false;
			_serifptr = 0;
			_bgpath = "";


			#endregion

			Load(level);
		}

		public static string TestAreaPath = "";
		public static bool IsTestPlay;
		public static bool IsCheatMode;
		public static bool TestPlayFinished;

		public static uint BackColor { get; set; }

		/// <summary>
		/// テストプレイモードを開始します．
		/// </summary>
		/// <param name="testAreaPath">テストプレイ対象のファイルがあるパス．</param>
		/// <param name="cheat">true にすると，チートモードになります．</param>
		/// <param name="pf">プレイヤーの初期状態を指定します．</param>
		public static IEnumerable<int> TestPlay(string testAreaPath, bool cheat, string mptname, PlayerForm pf, Size s, uint c)
		{
			IsTestPlay = true;
			TestAreaPath = testAreaPath;
			TestPlayFinished = false;
			IsCheatMode = cheat;
			_bsec = DateTime.Now.Second;
			_entitylist = new EntityList();
			Coin = 0;
			Time = 300;
			_level = _area = 1;
			IsGoal = false;
			ScrSize = s;
			IsInit = true;
			Gamemode = GameMode.Action;
			BackColor = c;

			HMainScreen = MakeScreen(s.Width, s.Height);

			_splashtime = 0;
			_height = 0;
			_gametick = 0;
			_mptname = mptname;
			//--- ↑↑↑初期化　↓↓↓読み込み
			Seq = new Sequencer();
			Load(TestAreaPath, pf);

			while (true)
			{
				if (TestPlayFinished)
					break;
				yield return DoGameLoop();
			}

		}

		/// <summary>
		/// レベル番号とエリア番号を指定して、マップを読み込みます。
		/// </summary>
		/// <param name="level"></param>
		/// <param name="area"></param>
		public static void Load(int level, int area)
		{
			if (level == -1)
			{
				Gamemode = GameMode.Ending;
				BgmPlay("omedeto.mid");
				return;
			}
			Load(level, area, PlayerForm.Big);
		}

		/// <summary>
		/// レベル番号を指定して、マップを読み込みます。
		/// </summary>
		/// <param name="level"></param>
		public static void Load(int level)
		{
			if (level == -1)
			{
				Gamemode = GameMode.Ending;
				BgmPlay("omedeto.mid");
				return;
			}
			Load(level, PlayerForm.Big);
		}

		/// <summary>
		/// レベル番号とエリア番号とプレイヤー状態を指定して、マップを読み込みます。
		/// </summary>
		/// <param name="level"></param>
		/// <param name="area"></param>
		/// <param name="pf"></param>
		public static void Load(int level, int area, PlayerForm pf)
		{
			Load($"Resources\\Levels\\Level {level}\\Area {area}", pf);
		}

		public static void Load(string path, PlayerForm pf)
		{
			if (Data != null)
				Time = Data.Time;
#if DEMO_MODE
			time = 70;              // 1分ぐらいタイマー
			pf = PlayerForm.Big;    // デモ版では強制的にでかくする
#endif

			//areainfo = GetJsonData<AreaInfo>("Resources\\Levels\\Level " + level + "\\Area " + area + "\\area.json");
			if (!IsTestPlay)
			{
				var o = DynamicJson.Parse(File.ReadAllText(Path.Combine(path, "area.json")));

				Areainfo = new AreaInfo
				{
					Mpt = o.Mpt,
					Music = o.Music,
					Bg = o.BG,
					ScrollSpeed = (int)o.ScrollSpeed
				};

				if (o.IsDefined("FG"))
					Areainfo.Fg = o.FG;
				if (o.IsDefined("FGScrollSpeed"))
					Areainfo.FgScrollSpeed = (int)o.FGScrollSpeed;
				else
					Areainfo.FgScrollSpeed = Areainfo.ScrollSpeed;

				_mptname = Areainfo.Mpt;
			}

			_hndlMpt = ResourceUtility.GetMpt(_mptname);
			var hndlMptsoft = LoadSoftImage("Resources\\Graphics\\" + _mptname + "_hj.png");



			Mptobjects = new Object[64];

			_isTimeUp = false;

			int r, g, b, a;
			var hits = new Color[5];
			for (var i = 0; i < 5; i++)
			{
				GetPixelSoftImage(hndlMptsoft, i, 64, out r, out g, out b, out a);
				hits[i] = Color.FromArgb(r, g, b, a);
			}

			var hitlist = hits.ToList();
			var mask = new byte[16, 16];
			for (var iy = 0; iy < 4; iy++)
			{
				for (var ix = 0; ix < 16; ix++)
				{
					for (var y = 0; y < 16; y++)
						for (var x = 0; x < 16; x++)
						{
							GetPixelSoftImage(hndlMptsoft, x + ix * 16, y + iy * 16, out r, out g, out b, out a);
							mask[x, y] = (byte)hitlist.IndexOf(Color.FromArgb(r, g, b, a));
						}
					Mptobjects[iy * 16 + ix] = new Object(_hndlMpt[iy * 16 + ix], (byte[,])mask.Clone());
				}
			}

			DeleteSoftImage(hndlMptsoft);

			_entitylist.Clear();

			MapUtility.LoadMap(out Chips, Path.Combine(path, "map.citmap"));
			Map = new Size(Chips.GetLength(0), Chips.GetLength(1));

			if (!IsTestPlay)
			{
				_hndlBg = LoadGraph("Resources\\Graphics\\" + Areainfo.Bg);

				_hndlFg = Areainfo.Fg != "" ? LoadGraph("Resources\\Graphics\\" + Areainfo.Fg) : 0;
			}

			dynamic spdata = DynamicJson.Parse(File.ReadAllText(Path.Combine(path, "spdata.json")));

			foreach (var obj in spdata)
			{
				int posx = (int)obj.PosX, posy = (int)obj.PosY, spid = (int)obj.EntityID;
				try
				{
					if (obj.IsDefined("Tag"))
						obj.EntityData.Tag = obj.Tag;
					if (EntityRegister.GetDataById(spid) != null)
						_entitylist.Add(EntityRegister.CreateEntity(spid, new PointF(posx, posy), Mptobjects, Chips, _entitylist, obj.EntityData), spid == 0);
				}
				catch (InvalidOperationException ioe)
				{
					Console.WriteLine("[ERROR]" + ioe.Message);
				}
			}

			if (!Middle.IsEmpty)
			{
				_entitylist.MainEntity.Location.X = Middle.X;
				_entitylist.MainEntity.Location.Y = Middle.Y;
				_entitylist.MainEntity.Location = Middle;

			}

			if (_entitylist.MainEntity is EntityPlayer)
			{
				var p = (EntityPlayer)_entitylist.MainEntity;
				p.Form = pf;
			}

			Camera = new Point((int)(-_entitylist.MainEntity.Location.X + 160), (int)(-_entitylist.MainEntity.Location.Y + 160));

			if (!IsTestPlay)
				BgmPlay(Areainfo.Music);
			_isDyingflag = false;
			_goalflag = false;
			Tick = 0;
			Ks = new Status(_binz, Camera, Map);
		}

		/// <summary>
		/// ファイル名を指定して、BGM を再生します。
		/// </summary>
		/// <param name="name"></param>
		public static void BgmPlay(string name)
		{
			if (Seq == null)
				Seq = new Sequencer();
			Seq.Stop();
			Seq.Reset();
			Seq.SetMidiData(ResourceUtility.MusicList[name]);
			Seq.Play();
		}

		/// <summary>
		/// BGM を停止します。
		/// </summary>
		public static void BgmStop()
		{
			Seq.Stop();
		}

		/// <summary>
		/// 指定した時間で、 BGM をフェードアウトします。
		/// </summary>
		/// <param name="time">時間(単位はミリ秒)。</param>
		public static void BgmStop(int time)
		{
			Task.Factory.StartNew(() =>
			{
				var bvol = Seq.Sm.Volume;
				for (var i = 0; i < time; i++)
				{
					Seq.Sm.Volume = bvol - (int)(bvol * (i / (float)time));
					Thread.Sleep(1);
				}
				BgmStop();
			});
		}

		/// <summary>
		/// レベル番号とプレイヤー状態を指定して、マップを読み込みます。
		/// </summary>
		/// <param name="level"></param>
		/// <param name="pf"></param>
		public static void Load(int level, PlayerForm pf)
		{
			if (!IsInit)
				throw new Exception("ゲームエンジンが初期化されていません。");
			if (level == -1)
			{
				Gamemode = GameMode.Ending;
				BgmPlay("omedeto.mid");
				return;
			}
			_level = level;
			Data = GetJsonData<LevelData>("Resources\\Levels\\Level " + level + "\\lvldat.json");
			_area = Data.FirstArea;
			Time = Data.Time;
			_storytext = File.Exists("Resources\\Levels\\Level " + level + "\\story.txt") ? File.ReadAllText("Resources\\Levels\\Level " + level + "\\story.txt") : "";
			Load(level, _area, pf);
			Seq.Stop();
			_splashtime = 120;
		}

		/// <summary>
		/// Json 文字列を .NET オブジェクトに変換し、指定した型にキャストして返します。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="json"></param>
		/// <returns></returns>
		public static T GetJsonData<T>(string json)
		{
			var jsonstring = File.ReadAllText(json);
			var s = new DataContractJsonSerializer(typeof(T));
			var jsonBytes = Encoding.Unicode.GetBytes(jsonstring);
			var sr = new MemoryStream(jsonBytes);
			var obj = (T)s.ReadObject(sr);

			return obj;
		}

		static int _guiCursor;

		static bool _binf3, _inf3;
		static bool _binup, _bindown, _binesc;
		static int _splashtime;
		static EndingMode _endmode;
		static string _credit;
		static int _height;
		static int _graphhandle;
		static int _graphhandle2;
		static int _gametick;
		private static TitleMode _titlemode;
		static bool _storyflag;
		static string _storytext;
		static string[] _storylines;
		static int _storyline;
		static string _serif = "";
		static string _storyimg = "";
		static bool _storywaiting;
		static bool _nowserif;
		static string[] _serifs;
		static int _serifptr;
		static string _bgpath = "";
		static string _bgpath2;
		static GameMode _bgamemode;
		static readonly Dictionary<string, string> Facealias = new Dictionary<string, string>();
		static readonly Regex Regpreprc = new Regex(@"\[(.+)?\:(.+)\]");
		public static string GameVersion
		{ get; set; } = "2.0.0-alpha";
		public static string Copyright
		{ get; set; } = "(C)2016 ＣCitringo";
		static int _helppage;
		static int _timer = -1;
		static MatchCollection _m;
		static bool _isTimeUp;
		public static PlayerGender CurrentGender { get; set; }
	

	

	/// <summary>
		/// メイン画面バッファのハンドル。
		/// </summary>
		public static int HMainScreen;

		public static Action Finallyprocess;



		/// <summary>
		/// ゲームのループ処理をします。
		/// </summary>
		public static int DoGameLoop()
		{
			if (!IsInit)
				throw new Exception("ゲームエンジンが初期化されていません。");
			var h = GetDrawScreen();
			SetDrawScreen(HMainScreen);
			Ks.Update(_binz, Camera, Map);
			var inesc = CheckHitKey(KEY_INPUT_ESCAPE) == 1;
			if (!inesc)
				inesc = (GetJoypadInputState(DX_INPUT_PAD1) & PAD_INPUT_10) == PAD_INPUT_10;
			ClearDrawScreen();  //消し去る

			Seq.PlayLoop();
			//----FPS 測定
			_f++;

			if (_bsec != DateTime.Now.Second)
			{
				_fps = _f;
				_f = 0;
				_bsec = DateTime.Now.Second;
			}
			switch (Gamemode)
			{
				case GameMode.Action:
					#region ActionMode
					//デバッグモードの切り替え
					_binf3 = _inf3;
					_inf3 = CheckHitKey(KEY_INPUT_F3) == 1;
					if (_inf3 && !_binf3)
					{
						IsDebugMode = !IsDebugMode;
					}

					#region スプラッシュ
					if (_splashtime > 0 && !IsTestPlay)
					{
						ClearDrawScreen();
						var ahi = $"レベル {_level}";
						FontUtility.DrawString(ScrSize.Width / 2 - ahi.Length * 5, ScrSize.Height / 2 - 5, ahi, 0xffffff);

						_splashtime--;
						if (_splashtime == 0)
						{
							BgmPlay(Areainfo.Music);
							_isDyingflag = false;
							_goalflag = false;
							Tick = 0;

						}

						break;
					}
					#endregion

					if (IsCheatMode && _entitylist.MainEntity is EntityPlayer)
					{
						((EntityPlayer)_entitylist.MainEntity).MutekiTime = 10;
					}


					#region 自動制御
					//ゴールしたら自動制御
					if (IsGoal)
					{
						Ks.Indown = false;
						Ks.Inleft = false;
						Ks.Inlshift = false;
						Ks.Inright = true;
						Ks.Inup = false;
						Ks.Inz = false;
						Ks.Inz1 = false;
						if (Time > 0)
						{
							Time--;
							if (Time % 10 == 0)
							{
								SoundUtility.PlaySound(Sounds.GetCoin);
								Coin++;
							}
						}
						if (!_goalflag)
						{
							BgmPlay("jingle_gameclear.mid");
							_goalflag = true;
						}
					}

					//Level 2-1 での制御
					if (_level == 2 && _area == 1)
					{
						Ks.Indown = false;
						Ks.Inleft = false;
						Ks.Inlshift = false;
						Ks.Inright = true;
						Ks.Inup = false;
						Ks.Inz = false;
						Ks.Inz1 = false;
						if (_entitylist.MainEntity.Location.X >= 300)
						{
							_level = 2;
							_area = 2;
							Load(2, 2, (_entitylist.MainEntity as EntityPlayer)?.Form ?? PlayerForm.Big);

							break;
						}
					}
					#endregion

					//ゴールしたあと画面遷移するときにいろいろリセットする
					if (IsGoal && !Seq.IsPlaying && Time == 0)
					{
						if (!IsTestPlay)
						{
							IsGoal = false;
							Middle = Point.Empty;

#if DEMO_MODE
							Environment.Exit(0);
#else
							_level = NextStage;
							Gamemode = GameMode.Story;
							Load(_level, (_entitylist.MainEntity as EntityPlayer)?.Form ?? PlayerForm.Big);
#endif
						}
						else
							TestPlayFinished = true;

						break;

					}

					//ESC キーでブレイクタイム
					if (inesc && !_binesc)
					{
						SetBreakTime(() => {
							Coin = 0;
							_bcoin = 0;
							Life = 5;
							BgmStop();
						}, GameMode.Action);
						break;
					}

					//主人公が死んでしばらくしたら振り出しに戻して始める処理をする
					if (_entitylist.MainEntity.IsDead && !Seq.IsPlaying)
					{
						if (!IsTestPlay)
						{
							if (Life == 0)
							{
								BgmPlay("IcyHeart.mid");
								_guiCursor = 0;
								IsGameOver = true;

								break;
							}
							Load(_level, _area);
						}
						else
						{
							TestPlayFinished = true;
							Coin = 0;
							_bcoin = 0;
							Life = 5;

						}

						break;
					}

					#region 主人公処理
					//主人公が死んだ瞬間に音楽を切り替える
					if (((EntityLiving)_entitylist.MainEntity).IsDying && !_isDyingflag)
					{
						BgmPlay("zannnenn.mid");
						Life--;
						//バイブレーションを鳴らす
						StartJoypadVibration(DX_INPUT_PAD1, 500, 1500);
						_isDyingflag = true;
					}

					//主人公が落下死したらパーティクルを出す
					if (Tick % 2 == 0 && ((EntityLiving)_entitylist.MainEntity).IsDying && ((EntityLiving)_entitylist.MainEntity).IsFall)
					{
						for (var i = 0; i < 8; i++)
							_entitylist.Add(EntityRegister.CreateEntity("Star", new Point((int)_entitylist.MainEntity.Location.X + GetRand(32) - 16, Ks.Map.Height * 16 - 1), Mptobjects, Chips, _entitylist));
					}
					#endregion


					#region カメラ処理
					if (!((EntityLiving)_entitylist.MainEntity).IsDying)
					{

						if (_entitylist.MainEntity.Location.X + Ks.Camera.X > ScrSize.Width / 2 && _entitylist.MainEntity.Velocity.X > 0 && Ks.Camera.X > -Ks.Map.Width * 16 + ScrSize.Width)
						{
							//ks.camera.Offset(-(int)entitylist.MainEntity.Velocity.X, 0);
							Ks.Camera = new Point(-(int)_entitylist.MainEntity.Location.X + ScrSize.Width / 2, Ks.Camera.Y);
						}

						if (Ks.Map.Width * 16 - _entitylist.MainEntity.Location.X > ScrSize.Width / 2 && _entitylist.MainEntity.Velocity.X < 0 && Ks.Camera.X < 0)
						{
							//ks.camera.Offset(-(int)entitylist.MainEntity.Velocity.X, 0);
							Ks.Camera = new Point(-(int)_entitylist.MainEntity.Location.X + ScrSize.Width / 2, Ks.Camera.Y);
						}

						if (_entitylist.MainEntity.Location.Y + Ks.Camera.Y > ScrSize.Height / 2 && _entitylist.MainEntity.Velocity.Y > 0 && Ks.Camera.Y > -Ks.Map.Height * 16 + ScrSize.Height)
						{
							//ks.camera.Offset(0, -(int)entitylist.MainEntity.Velocity.Y);
							Ks.Camera = new Point(Ks.Camera.X, -(int)_entitylist.MainEntity.Location.Y + ScrSize.Height / 2);
						}

						if (Ks.Map.Height * 16 - _entitylist.MainEntity.Location.Y > ScrSize.Height / 2 && _entitylist.MainEntity.Velocity.Y < 0 && Ks.Camera.Y < 0)
						{
							//ks.camera.Offset(0, -(int)entitylist.MainEntity.Velocity.Y);
							//ks.camera.Offset(ks.camera.X, -(int)entitylist.MainEntity.Location.Y + scrSize.Height / 2);
							Ks.Camera = new Point(Ks.Camera.X, -(int)_entitylist.MainEntity.Location.Y + ScrSize.Height / 2);
						}

						if (Ks.Camera.X > 0)
							Ks.Camera = new Point(0, Ks.Camera.Y);

						if (Ks.Camera.Y > 0)
							Ks.Camera = new Point(Ks.Camera.X, 0);

						if (Ks.Camera.X < -Ks.Map.Width * 16 + ScrSize.Width)
							Ks.Camera = new Point(-Ks.Map.Width * 16 + ScrSize.Width, Ks.Camera.Y);

						if (Ks.Camera.Y < -Ks.Map.Height * 16 + ScrSize.Height)
							Ks.Camera = new Point(Ks.Camera.X, -Ks.Map.Height * 16 + ScrSize.Height);
					}
					#endregion

					#region 描画
					// +--------------+
					// |Background    |
					// | +--------------+
					// | |Mpt Back      |
					// | | +--------------+
					// +-| |Entities      |
					//   | | +--------------+
					//   +-| |Mpt Forward   |
					//     | | +--------------+
					//     +-| |Foreground    |
					//       | |              |
					//       +-|              |
					//         |              |
					//         +--------------+

					#region Background
					var bgx = 0;
					if (!IsTestPlay)
					{
						bgx = (int)-(-Ks.Camera.X * (Areainfo.ScrollSpeed / 10.0) % (ScrSize.Width + 1));
						DrawExtendGraph(bgx, 0, bgx + ScrSize.Width, ScrSize.Height, _hndlBg, 0);
						DrawExtendGraph(bgx + ScrSize.Width, 0, bgx + ScrSize.Width * 2, ScrSize.Height + 1, _hndlBg, 1);    //スクロールするから2枚使ってループできるようにしている
					}
					else
					{
						DrawFillBox(0, 0, ScrSize.Width, ScrSize.Height, BackColor);
					}
					#endregion

					#region Mpt Back
					for (var y = 0; y < Map.Height * 16; y += 16)
					{
						if (y + Camera.Y < -16 || y + Camera.Y > ScrSize.Height)
							continue;
						for (var x = 0; x < Map.Width * 16; x += 16)
						{
							if (x + Camera.X < -16 || x + Camera.X > ScrSize.Width)
								continue;
							DrawGraph(x + Camera.X, y + Camera.Y, _hndlMpt[Chips[x / 16, y / 16, 1]], 1);
						}
					}
					#endregion

					#region Entities
					_entitylist.Draw(ref Ks, ref Chips);
					if (((EntityLiving)_entitylist.MainEntity).IsDying && IsGoal)
						((EntityLiving)_entitylist.MainEntity).IsDying = false;
					#endregion

					#region Mpt Forward
					for (var y = 0; y < Map.Height * 16; y += 16)
						for (var x = 0; x < Map.Width * 16; x += 16)
							DrawGraph(x + Camera.X, y + Camera.Y, _hndlMpt[Chips[x / 16, y / 16, 0]], 1);
					#endregion

					#region Foreground
					var fgx = 0;
					if (!IsTestPlay)
					{
						fgx = (int)-(-Ks.Camera.X * (Areainfo.FgScrollSpeed / 10.0) % (ScrSize.Width + 1));
						if (_hndlFg != 0)
						{
							DrawExtendGraph(fgx, 0, fgx + ScrSize.Width, ScrSize.Height, _hndlFg, 1);
							DrawExtendGraph(fgx + ScrSize.Width, 0, fgx + ScrSize.Width * 2, ScrSize.Height, _hndlFg, 1);    //スクロールするから2枚使ってループできるようにしている
						}
					}
					#endregion

					#region Time up
					if (_isTimeUp && ((EntityLiving)_entitylist.MainEntity).IsDying)
					{
						var mes = "TIME UP";
						var x = ScrSize.Width / 2 - FontUtility.GetDrawingSize(mes).Width / 2;
						FontUtility.DrawString(x + 1, 100, mes, Color.Black);
						FontUtility.DrawString(x, 99, mes, Color.White);
					}
					#endregion


					#endregion




					//コイン50枚毎に1UPの処理する
					if (_bcoin != Coin && Coin % 50 == 0)
					{
						Life++;
						SoundUtility.PlaySound(Sounds.Player1Up);
					}

					#region HUD 表示
					//テキストを表示
					//通常表示とデバッグモード表示
					var buf = $"①{Coin} Alen×{Life} {new string('♥', (_entitylist.MainEntity as EntityPlayer)?.Life ?? 0)}\nレベル{_level} ⌚{Time}";
					if (IsDebugMode)
					{
						_entitylist.DebugDraw(ref Ks, ref Chips);
						buf =
							$"MS{Seq.NTickCount}/{Seq.Eot} P({(int) _entitylist.MainEntity.Location.X},{(int) _entitylist.MainEntity.Location.Y}) {_fps}\n" + 
							$"S{_entitylist.Count} MS({Map.Width},{Map.Height}) CP({Camera.X},{Camera.Y})\n" + 
							$"V({(int) _entitylist.MainEntity.Velocity.X},{(int) _entitylist.MainEntity.Velocity.Y}) AS{_entitylist.Count(sp => !(Math.Abs(_entitylist.MainEntity.Location.X - sp.Location.X) > ScrSize.Width))} SS({ScrSize.Width},{ScrSize.Height})\n" +
							$"EntityPlayer: {(((EntityLiving) _entitylist.MainEntity).IsDying ? "DEAD" : "ALIVE")}\n" + 
							$"Lift: {new Func<IEnumerable<EntityTurcosShell>, string>(sps => { return sps.Aggregate("", (current, tsb) => current + (tsb.Owner != null ? "T" : "F")); })(_entitylist.OfType<EntityTurcosShell>())}\nIRun: {new Func<IEnumerable<EntityTurcosShell>, string>(sps => { var retval = ""; foreach (var tsb in sps) retval += tsb.IsRunning ? "T" : "F"; return retval; })(_entitylist.OfType<EntityTurcosShell>())}\n" + 
							$"INPUT: {new Func<string>(() => { var lastdata = ""; var joydata = GetJoypadInputState(DX_INPUT_KEY_PAD1); if ((joydata & PAD_INPUT_1) == PAD_INPUT_1) lastdata += "1 "; if ((joydata & PAD_INPUT_2) == PAD_INPUT_2) lastdata += "2 "; if ((joydata & PAD_INPUT_3) == PAD_INPUT_3) lastdata += "3 "; if ((joydata & PAD_INPUT_4) == PAD_INPUT_4) lastdata += "4 "; if ((joydata & PAD_INPUT_5) == PAD_INPUT_5) lastdata += "5 "; if ((joydata & PAD_INPUT_6) == PAD_INPUT_6) lastdata += "6 "; if ((joydata & PAD_INPUT_7) == PAD_INPUT_7) lastdata += "7 "; if ((joydata & PAD_INPUT_8) == PAD_INPUT_8) lastdata += "8 "; if ((joydata & PAD_INPUT_9) == PAD_INPUT_9) lastdata += "9 "; if ((joydata & PAD_INPUT_10) == PAD_INPUT_10) lastdata += "10 "; if ((joydata & PAD_INPUT_11) == PAD_INPUT_11) lastdata += "11 "; if ((joydata & PAD_INPUT_12) == PAD_INPUT_12) lastdata += "12 "; if ((joydata & PAD_INPUT_13) == PAD_INPUT_13) lastdata += "13 "; if ((joydata & PAD_INPUT_14) == PAD_INPUT_14) lastdata += "14 "; if ((joydata & PAD_INPUT_15) == PAD_INPUT_15) lastdata += "15 "; if ((joydata & PAD_INPUT_16) == PAD_INPUT_16) lastdata += "16 "; if ((joydata & PAD_INPUT_17) == PAD_INPUT_17) lastdata += "17 "; if ((joydata & PAD_INPUT_18) == PAD_INPUT_18) lastdata += "18 "; if ((joydata & PAD_INPUT_19) == PAD_INPUT_19) lastdata += "19 "; if ((joydata & PAD_INPUT_20) == PAD_INPUT_20) lastdata += "20 "; if ((joydata & PAD_INPUT_21) == PAD_INPUT_21) lastdata += "21 "; if ((joydata & PAD_INPUT_22) == PAD_INPUT_22) lastdata += "22 "; if ((joydata & PAD_INPUT_23) == PAD_INPUT_23) lastdata += "23 "; if ((joydata & PAD_INPUT_24) == PAD_INPUT_24) lastdata += "24 "; if ((joydata & PAD_INPUT_25) == PAD_INPUT_25) lastdata += "25 "; if ((joydata & PAD_INPUT_26) == PAD_INPUT_26) lastdata += "26 "; if ((joydata & PAD_INPUT_27) == PAD_INPUT_27) lastdata += "27 "; if ((joydata & PAD_INPUT_28) == PAD_INPUT_28) lastdata += "28 "; return lastdata; })()}\n" + 
							$"POV: {GetJoypadPOVState(DX_INPUT_PAD1, 0)}\n" +
							$"bgx: {bgx} fgx: {fgx}";
					}

					FontUtility.DrawString(1, 1, buf, 0);
					FontUtility.DrawString(0, 0, buf, 0xffffff);

					#endregion

					//タイムをデクリメント、なくなったら殺す
					if (!IsGoal && Tick % 60 == 0 && !((EntityLiving)_entitylist.MainEntity).IsDying)
					{
						if (Time == 0)
						{
							((EntityLiving)_entitylist.MainEntity).Kill();
							_isTimeUp = true;
						}
						else
							Time--;
					}



					#region ゲームオーバーダイアログ
					if (IsGameOver)
					{
						DrawBox(64, 64, 256, 176, 0x7f000000, 1);
						FontUtility.DrawString(124, 96, "ゲームオーバー", 0xffffff);
						FontUtility.DrawString(124, 106, "  つづける", 0xffffff);
						FontUtility.DrawString(124, 116, "  あきらめる", 0xffffff);
						FontUtility.DrawString(124, 106 + 10 * _guiCursor, ">", 0xffffff);

						if (Ks.Inup && !_binup)
						{
							_guiCursor ^= 1;
						}
						if (Ks.Indown && !_bindown)
						{
							_guiCursor ^= 1;
						}
						if (Ks.Inz)
						{
							switch (_guiCursor)
							{
								case 0:
									IsGameOver = false;
									Life = 5;
									Middle = Point.Empty;
									Load(_level);
									break;
								case 1:
									Ks.Inz1 = false;
									_titlemode = TitleMode.Opening;
									Gamemode = GameMode.Title;
									break;
							}
						}
					}
					#endregion

					_bcoin = Coin;
					#endregion
					break;
				case GameMode.Story:
					#region StoryMode
					//ESC キーでブレイクタイム
					if (inesc && !_binesc)
					{
						SetBreakTime(() =>
						{
							_storyflag = false;
							if (_graphhandle != 0)
								DeleteGraph(_graphhandle);
							_graphhandle = 0;
							if (_graphhandle2 != 0)
								DeleteGraph(_graphhandle2);
							_graphhandle2 = 0;
						}, Gamemode);

						break;
					}
					if (!_storyflag) //初期化
					{
						if (_storytext == "")
						{
							Gamemode = GameMode.Action;
							break;
						}
						_storyflag = true;
						_storyline = 0;
						_serif = "";
						_storylines = _storytext.Split('\r', '\n');
						Facealias.Clear();
					}



					if (Regpreprc.IsMatch(_storylines[_storyline]))   //プリプロセッサ
					{
						_m = Regpreprc.Matches(_storylines[_storyline]);

						switch (_m[0].Groups[1].Value)
						{
							case "img":
								if (_graphhandle != 0)
								{
									DeleteGraph(_graphhandle);
									_graphhandle = 0;
								}
								if (_m[0].Groups[2].Value == "white")
								{
									_bgpath = "white";
									_graphhandle = MakeScreen(ScrSize.Width, ScrSize.Height);
									FillGraph(_graphhandle, 255, 255, 255);
									break;
								}
								_graphhandle = LoadGraph(_bgpath = "Resources\\Graphics\\" + _m[0].Groups[2].Value);
								break;
							case "bgm":
								if (_m[0].Groups[2].Value == "stop")
								{
									BgmStop();
									break;
								}
								if (Seq.IsPlaying)
									BgmStop();
								BgmPlay(_m[0].Groups[2].Value);
								break;
							case "face":
								var val = _m[0].Groups[2].Value.Split(',');
								Facealias.Add(val[0], val[1]);
								break;
							case "se":
								SoundUtility.PlaySound((Sounds)Enum.Parse(typeof(Sounds), _m[0].Groups[2].Value));
								break;
							case "wait":
								if (_timer == -1)
								{
									_timer = int.Parse(_m[0].Groups[2].Value);
								}
								if (_timer > 0)
								{
									_timer--;
									if (_timer == 0)
										_timer = -1;
								}
								break;

						}
					}
					if (!_storywaiting && !_nowserif && !Regpreprc.IsMatch(_storylines[_storyline]) && _storylines[_storyline].IndexOf(',') > -1)
					{
						_nowserif = true;
						_serifs = _storylines[_storyline].Split(',');
						_serifptr = 0;
						_serif = "";
						if (_graphhandle2 != 0)
						{
							DeleteGraph(_graphhandle2);
							_graphhandle2 = 0;
						}
						_graphhandle2 = Facealias.ContainsKey(_serifs[0]) ? LoadGraph(_bgpath2 = "Resources\\Graphics\\" + Facealias[_serifs[0]] + "\\" + _serifs[2] + ".png") : LoadGraph(_bgpath2 = "Resources\\Graphics\\" + _serifs[0] + "\\" + _serifs[2] + ".png");
						if (_serifs.Length != 3)
						{
							throw new Exception("セリフの記述が不適切です。");
						}
					}

					if (!_storywaiting && _nowserif && Tick % (Ks.Inz ? 1 : 4) == 0)
					{
						for (var i = 0; i < (Ks.Inlshift ? 7 : 1); i++)
						{
							_serif += _serifs[1][_serifptr];
							SoundUtility.PlaySound(Sounds.Saying);
							if (_serifs[1][_serifptr] == '。')
							{
								_storywaiting = true;
							}
							_serifptr++;
							if (_serifs[1].Length <= _serifptr)
							{
								_storywaiting = true;
								_nowserif = false;
								break;
							}
						}
					}

					if (_graphhandle != 0)
					{
						DrawGraph(0, 0, _graphhandle, 0);
					}

					if (_timer == -1)
					{

						if (_serifs != null)
						{
							DrawBox(0, ScrSize.Height - 64, 8 + FontUtility.GetDrawingSize(_serifs[0]).Width, ScrSize.Height - 64 - 16, GetColor(64, 64, 64), 1);
							FontUtility.DrawString(4, ScrSize.Height - 64 - 13, _serifs[0], Color.White);
							DrawBox(0, ScrSize.Height - 64, ScrSize.Width, ScrSize.Height, GetColor(64, 64, 64), 1);
							FontUtility.DrawString(8, ScrSize.Height - 56, ScrSize.Width - 8, ScrSize.Height - 8, _serif, Color.White);
						}

						if (_storywaiting)
						{
							FontUtility.DrawMiniString(ScrSize.Width - 12, ScrSize.Height - 12, Tick % 16 < 8 ? "▼" : "▽", Color.White);
						}


						DrawExtendGraph(90, 16, 230, 156, _graphhandle2, 0);

					}

					if (_storywaiting)
					{
						if (Ks.Inz1 || Ks.Inlshift)
						{
							_storywaiting = false;
						}
					}

					if (!_nowserif && !_storywaiting && !(_timer > 0))
					{
						_serifs = null;
						_serif = "";
						_graphhandle2 = 0;
						_storyline++;
					}
					if (_storylines.Length <= _storyline)
					{
						_storyflag = false;
						DeleteGraph(_graphhandle);
						_graphhandle = 0;
						DeleteGraph(_graphhandle2);
						_graphhandle2 = 0;
						Gamemode = GameMode.Action;
						if (Seq.IsPlaying)
							BgmStop(500);
					}
					#endregion
					break;
				case GameMode.Title:
					#region TitleMode
					if (inesc && !_binesc && _titlemode == TitleMode.MainTitle)
					{
						if (HelpType == "kb")
							Environment.Exit(0);
						break;
					}
					switch (_titlemode)
					{
						case TitleMode.Opening:
							// TODO: オープニングムービーが完成したらここを実装する。
							_titlemode = TitleMode.GameLogoRising;
							break;
						case TitleMode.GameLogoRising:
							#region
							if (_gametick > 360 || Ks.Inz1)
							{
								_titlemode = TitleMode.GameLogoLightning;
								_gametick = 0;
								SoundUtility.PlaySound(Sounds.Flash);
								Ks.Inz1 = false;
								break;
							}

							DrawGraph(ScrSize.Width / 2 - 180 / 2, (int)(ScrSize.Height - Math.Min(220, (float)_gametick / 300 * 220)), ResourceUtility.Logo[0], 1);

							_gametick++;
							break;
						#endregion
						case TitleMode.GameLogoLightning:
							#region
							if (_gametick < 60)
							{
								DrawGraph(ScrSize.Width / 2 - 180 / 2, ScrSize.Height - 220, ResourceUtility.Logo[Convert.ToInt32(_gametick % 8 > 3)], 1);
							}
							else
								DrawGraph(ScrSize.Width / 2 - 180 / 2, ScrSize.Height - 220, ResourceUtility.Logo[0], 1);
							if (_gametick > 120 || Ks.Inz1)
							{
								_gametick = 0;
								_titlemode = TitleMode.MainTitle;
								_graphhandle = LoadGraph(@"Resources\Graphics\story_1.bmp");
								BgmPlay("hometownv2.mid");
								_guiCursor = 0;
								Ks.Inz1 = false;
							}
							_gametick++;
							break;
						#endregion
						case TitleMode.MainTitle:
							#region 

							#region 描画
							DrawGraph(0, 0, _graphhandle, 0);
							DrawGraph(ScrSize.Width / 2 - 180 / 2, ScrSize.Height - 220, ResourceUtility.Logo[0], 1);
							var uistr = $@"{(_guiCursor == 0 ? ">" : " ")}はじめから

{(_guiCursor == 1 ? ">" : " ")}設定

{(_guiCursor == 2 ? ">" : " ")}ヘルプ

{(_guiCursor == 3 ? ">" : " ")}おわる";
							FontUtility.DrawString(ScrSize.Width / 2 - 40, ScrSize.Height / 2 + 16, uistr, Color.Black);
							FontUtility.DrawMiniString(0, ScrSize.Height - 8, Copyright, Color.Black);
							FontUtility.DrawMiniString(ScrSize.Width - GameVersion.Length * 6, ScrSize.Height - 8, GameVersion, Color.Black);

							#endregion

							#region 判定
							if (_binup != Ks.Inup && Ks.Inup)
							{
								_guiCursor--;
								if (_guiCursor < 0)
									_guiCursor = 3;
								SoundUtility.PlaySound(Sounds.Selected);
							}

							if (_bindown != Ks.Indown && Ks.Indown)
							{
								_guiCursor++;
								if (_guiCursor > 3)
									_guiCursor = 0;
								SoundUtility.PlaySound(Sounds.Selected);
							}
							if (Ks.Inz1)
							{
								switch (_guiCursor)
								{
									case 0:
#if DEMO_MODE
										gamemode = GameMode.Action;
										BGMStop();
#else
										Gamemode = GameMode.Prolog;
										BgmStop();
										BgmPlay("nothing.mid");
#endif
										Ks.Inz1 = false;
										DeleteGraph(_graphhandle);
										_graphhandle = 0;
										_gametick = 0;
										break;
									case 1:
										_titlemode = TitleMode.Setting;
										break;
									case 2:
										_titlemode = TitleMode.Help;
										break;
									case 3:
										if (HelpType == "kb")
											Environment.Exit(0);
										break;
								}
								SoundUtility.PlaySound(Sounds.Pressed);
							}
							break;
						#endregion
						#endregion
						case TitleMode.Setting:
							FontUtility.DrawString(32, $@"残念だが、工事中だぜ。
{(HelpType == "kb" ? "Esc キー" : "[START] ボタン")} を押してタイトルへ戻る", Color.White);
							if (inesc && !_binesc)
								_titlemode = TitleMode.MainTitle;
							break;
						case TitleMode.Help:
							if (_graphhandle2 == 0)
							{
								_graphhandle2 = LoadGraph($@"Resources\Graphics\{HelpType}actgame.png");
							}
							if (_helppage == 1 && Ks.Inlshift)
							{
								DeleteGraph(_graphhandle2);
								_graphhandle2 = LoadGraph($@"Resources\Graphics\{HelpType}actgame.png");
								_helppage--;
							}
							if (Ks.Inz1)
							{
								Ks.Inz1 = false;
								_helppage++;
								DeleteGraph(_graphhandle2);
								_graphhandle2 = LoadGraph($@"Resources\Graphics\{HelpType}story.png");
								if (_helppage == 2)
								{
									_helppage = 0;
									DeleteGraph(_graphhandle2);
									_graphhandle2 = 0;
									_titlemode = TitleMode.MainTitle;
								}
							}
							DrawGraph(0, 0, _graphhandle2, 0);
							if (inesc && !_binesc)
							{
								_helppage = 0;
								DeleteGraph(_graphhandle2);
								_graphhandle2 = 0;
								_titlemode = TitleMode.MainTitle;
							}
							break;
					}
					#endregion
					break;
				case GameMode.Ending:
					#region EndingMode
					//ESC キーでブレイクタイム
					if (inesc && !_binesc)
					{
						Gamemode = GameMode.Breaktime;
						_bgamemode = GameMode.Ending;
						_guiCursor = 0;

						break;
					}
					switch (_endmode)
					{
						case EndingMode.Message:
							FontUtility.DrawString(48, "物語はここで途切れている...", 0xffffff);
							if (Ks.Inz || !Seq.IsPlaying)
							{
								_endmode = EndingMode.Credit;
								Camera.Y = 0;
								_credit = File.ReadAllText("Resources\\Document\\staffrole.txt");
								BgmPlay("c011_piano.mid");
								_height = FontUtility.GetDrawingSize(_credit).Height + 10;
								_graphhandle = MakeScreen(ScrSize.Width, _height);
								SetDrawScreen(_graphhandle);
								FontUtility.DrawString(Camera.Y, _credit, 0xffffff);
								SetDrawScreen(HMainScreen);
								_gametick = GetNowCount();
							}
							break;
						case EndingMode.Credit:
							Camera.Y = ScrSize.Height - (int)((float)(GetNowCount() - _gametick) / Seq.MusicTime * (_height + ScrSize.Height - 10));
							DrawGraph(0, Camera.Y, _graphhandle, 0);
							//FontUtility.DrawMiniString(0, scrSize.Height - 8, $"{camera.Y} / {-height}", 0xffffff);
							if (Camera.Y < -_height)
							{
								_endmode = EndingMode.TheEnd;
								DeleteGraph(_graphhandle);
							}
							break;
						case EndingMode.TheEnd:
							DrawGraph(ScrSize.Width / 2 - 128 / 2, ScrSize.Height / 2 - 32 / 2 - 32, ResourceUtility.TheEnd, 0);
							//DX.DrawGraph(0, 0, ResourceUtility.TheEnd, 0);
							//DX.DrawBox(scrSize.Width / 2 - 128 / 2, scrSize.Height / 2 - 32 / 2 - 32, scrSize.Width / 2 - 128 / 2 + 128, scrSize.Height / 2 - 32 / 2 - 32 + 32, DX.GetColor(255, 0, 0), 0);
							FontUtility.DrawMiniString(180, $"どれか{(HelpType == "kb" ? "キー" : "ボタン")}を押してください", 0xffffff);
							if (Ks.AnyKeyPressed)
							{

								Init(ScrSize.Width, ScrSize.Height);
								Ks.Inz1 = false;
							}
							break;
					}

					#endregion
					break;
				case GameMode.Debug:
					#region Debug
					//ESC キーでブレイクタイム
					if (inesc && !_binesc)
					{
						Gamemode = GameMode.Breaktime;
						_bgamemode = GameMode.Debug;
						_guiCursor = 0;

						break;
					}
					FontUtility.DrawString(0, "あか", Color.Red);
					FontUtility.DrawString(10, "だいだい", Color.Orange);
					FontUtility.DrawString(20, "きいろ", Color.Yellow);
					FontUtility.DrawString(30, "きみどり", Color.Lime);
					FontUtility.DrawString(40, "みどり", Color.Green);
					FontUtility.DrawString(50, "あお", Color.Blue);
					FontUtility.DrawString(60, "むらさき", Color.Purple);
					#endregion
					break;
				case GameMode.Prolog:
					#region PrologMode
					//ESC キーでブレイクタイム
					if (inesc && !_binesc)
					{
						Gamemode = GameMode.Breaktime;
						_bgamemode = GameMode.Prolog;
						_guiCursor = 0;

						break;
					}
					if (_graphhandle == 0)
					{
						var story = File.ReadAllText("Resources\\Document\\prolog.txt");
						_graphhandle = MakeScreen(ScrSize.Width, _height = FontUtility.GetDrawingSize(story).Height + 32);
						SetDrawScreen(_graphhandle);
						FontUtility.DrawString(0, story, Color.White);
						SetDrawScreen(HMainScreen);
						Ks.Camera.Y = ScrSize.Height;

					}
					if (Tick % 3 == 0)
						Ks.Camera.Y--;
					DrawGraph(0, Ks.Camera.Y, _graphhandle, 0);
					DrawBox(0, ScrSize.Height - 10, ScrSize.Width, ScrSize.Height, 0, 1);
					if (_gametick > 60)
					{
						FontUtility.DrawString(ScrSize.Width - 100, $"{(HelpType == "kb" ? "Z キー" : "×ボタン")}でスキップ", Color.White);
						if (Ks.Inz1)
						{
							Ks.Inz1 = false;
							Gamemode = GameMode.Story;
							BgmStop();
							DeleteGraph(_graphhandle);
							_graphhandle = 0;
						}
					}
					_gametick++;
					#endregion
					break;
				case GameMode.Breaktime:
					#region BreakTime
					FontUtility.DrawString(48, "おひるね", Color.White);
					FontUtility.DrawString(80, $"{(_guiCursor == 0 ? ">" : " ")}つづける", _guiCursor == 0 ? Color.Yellow : Color.White);
					FontUtility.DrawString(90, $"{(_guiCursor == 1 ? ">" : " ")}おわる　", _guiCursor == 1 ? Color.Yellow : Color.White);
					if ((Ks.Inup && !_binup) || (Ks.Indown && !_bindown))
						_guiCursor ^= 1;

					if (Ks.Inz1)
					{
						if (_guiCursor == 0)
						{
							Gamemode = _bgamemode;
						}
						else
						{
							_guiCursor = 0;
							Finallyprocess?.Invoke();
							if (!IsTestPlay)
								Init(ScrSize.Width, ScrSize.Height);
							else
								TestPlayFinished = true;
						}
						Ks.Inz1 = false;
					}
					#endregion
					break;
			}


			_binz = Ks.Inz;
			Tick = (Tick + 1) % 3600;
			Camera = Ks.Camera;
			Map = Ks.Map;
			_binup = Ks.Inup;
			_bindown = Ks.Indown;
			_binesc = inesc;

			SetDrawScreen(h);
			return HMainScreen;
		}

		private static void SetBreakTime(Action a, GameMode g)
		{
			_bgamemode = g;
			Finallyprocess = a;
			Gamemode = GameMode.Breaktime;
			_guiCursor = 0;
		}

		public static void Reload()
		{
			var h = GetDrawScreen();
			HMainScreen = MakeScreen(ScrSize.Width, ScrSize.Height);
			SetDrawScreen(HMainScreen);
			ResourceUtility.Init();
			FontUtility.Init();
			switch (Gamemode)
			{
				case GameMode.Title:
					switch (_titlemode)
					{
						case TitleMode.MainTitle:
							_graphhandle = LoadGraph(@"Resources\Graphics\story_1.bmp");
							break;
						case TitleMode.Help:
							if (_helppage == 0)
								_graphhandle2 = LoadGraph($@"Resources\Graphics\{HelpType}actgame.png");
							if (_helppage == 1)
								_graphhandle2 = LoadGraph($@"Resources\Graphics\{HelpType}story.png");
							break;
					}
					break;
				case GameMode.Story:
					if (_bgpath == "white")
					{
						_graphhandle = MakeScreen(ScrSize.Width, ScrSize.Height);
						FillGraph(_graphhandle, 255, 255, 255);
					}
					else
						_graphhandle = LoadGraph(_bgpath);
					_graphhandle2 = LoadGraph(_bgpath2);
					break;
				case GameMode.Ending:
					switch (_endmode)
					{
						case EndingMode.Credit:

							_graphhandle = MakeScreen(ScrSize.Width, _height);
							SetDrawScreen(_graphhandle);
							FontUtility.DrawString(Camera.Y, _credit, 0xffffff);
							SetDrawScreen(HMainScreen);
							break;
					}
					break;
				case GameMode.Prolog:
					var story = File.ReadAllText(@"Resources\Document\prolog.txt");
					_graphhandle = MakeScreen(ScrSize.Width, _height = FontUtility.GetDrawingSize(story).Height + 32);
					SetDrawScreen(_graphhandle);
					FontUtility.DrawString(0, story, Color.White);
					SetDrawScreen(HMainScreen);
					Ks.Camera.Y = ScrSize.Height;

					break;
			}

			_hndlBg = LoadGraph(@"Resources\Graphics\" + Areainfo.Bg);
			_hndlMpt = ResourceUtility.GetMpt(_mptname);
			var hndlMptsoft = LoadSoftImage($@"Resources\Graphics\{_mptname}_hj.png");
			_hndlFg = Areainfo.Fg != "" ? LoadGraph($@"Resources\Graphics\{Areainfo.Fg}") : 0;



			Mptobjects = new Object[64];

			int r, g, b, a;
			var hits = new Color[5];
			for (var i = 0; i < 5; i++)
			{
				GetPixelSoftImage(hndlMptsoft, i, 64, out r, out g, out b, out a);
				hits[i] = Color.FromArgb(r, g, b, a);
			}

			var hitlist = hits.ToList();
			var mask = new byte[16, 16];
			for (var iy = 0; iy < 4; iy++)
			{
				for (var ix = 0; ix < 16; ix++)
				{
					for (var y = 0; y < 16; y++)
						for (var x = 0; x < 16; x++)
						{
							GetPixelSoftImage(hndlMptsoft, x + ix * 16, y + iy * 16, out r, out g, out b, out a);
							mask[x, y] = (byte)hitlist.IndexOf(Color.FromArgb(r, g, b, a));
						}
					Mptobjects[iy * 16 + ix] = new Object(_hndlMpt[iy * 16 + ix], (byte[,])mask.Clone());
				}
			}

			DeleteSoftImage(hndlMptsoft);
			SetDrawScreen(h);
		}

	}

	/// <summary>
	/// ゲームエンジンが使用する様々なステータス変数の集合体を表します。
	/// </summary>
	public struct Status
	{
		public bool Inup, Indown, Inleft, Inright, Inlshift, Inz, Inz1;
		public Point Camera;
		public Size Map;
		public Status(bool binz, Point c, Size m)
		{
			Inup = CheckHitKey(KEY_INPUT_UP) == 1;
			Indown = CheckHitKey(KEY_INPUT_DOWN) == 1;
			Inleft = CheckHitKey(KEY_INPUT_LEFT) == 1;
			Inright = CheckHitKey(KEY_INPUT_RIGHT) == 1;
			Inlshift = CheckHitKey(KEY_INPUT_LSHIFT) == 1;
			Inz = CheckHitKey(KEY_INPUT_Z) == 1;

			var joy = GetJoypadInputState(DX_INPUT_PAD1);
			if (!Inup) Inup = (joy & PAD_INPUT_UP) == PAD_INPUT_UP;
			if (!Indown) Indown = (joy & PAD_INPUT_DOWN) == PAD_INPUT_DOWN;
			if (!Inleft) Inleft = (joy & PAD_INPUT_LEFT) == PAD_INPUT_LEFT;
			if (!Inright) Inright = (joy & PAD_INPUT_RIGHT) == PAD_INPUT_RIGHT;
			if (!Inlshift) Inlshift = (joy & PAD_INPUT_1) == PAD_INPUT_1;
			if (!Inz) Inz = (joy & PAD_INPUT_2) == PAD_INPUT_2;

			Inz1 = Inz;

			if (Inz && binz)
				Inz1 = false;
			Camera = c;
			Map = m;
		}

		void Set(ref bool a)
		{
			if (!a)
				a = true;
		}

		public void Update(bool binz, Point c, Size m)
		{
			if (GetWindowActiveFlag() == 0)
			{
				Inup = false;
				Indown = false;
				Inleft = false;
				Inright = false;
				Inz = false;
				Camera = c;
				Inz1 = Inz;
				Map = m;
				return;
			}
			Inup = CheckHitKey(KEY_INPUT_UP) == 1;
			Indown = CheckHitKey(KEY_INPUT_DOWN) == 1;
			Inleft = CheckHitKey(KEY_INPUT_LEFT) == 1;
			Inright = CheckHitKey(KEY_INPUT_RIGHT) == 1;
			Inlshift = CheckHitKey(KEY_INPUT_LSHIFT) == 1;
			Inz = CheckHitKey(KEY_INPUT_Z) == 1;
			var joy = GetJoypadInputState(DX_INPUT_PAD1);

			if (!Inup) Inup = (joy & PAD_INPUT_UP) == PAD_INPUT_UP;
			if (!Indown) Indown = (joy & PAD_INPUT_DOWN) == PAD_INPUT_DOWN;
			if (!Inleft) Inleft = (joy & PAD_INPUT_LEFT) == PAD_INPUT_LEFT;
			if (!Inright) Inright = (joy & PAD_INPUT_RIGHT) == PAD_INPUT_RIGHT;
			if (!Inlshift) Inlshift = (joy & PAD_INPUT_1) == PAD_INPUT_1;
			if (!Inz) Inz = (joy & PAD_INPUT_2) == PAD_INPUT_2;

			var pov = GetJoypadPOVState(DX_INPUT_PAD1, 0);
			switch (pov)
			{
				case 0:
					Set(ref Inup);
					break;
				case 4500:
					Set(ref Inup);
					Set(ref Inright);
					break;
				case 9000:
					Set(ref Inright);
					break;
				case 13500:
					Set(ref Inright);
					Set(ref Indown);
					break;
				case 18000:
					Set(ref Indown);
					break;
				case 22500:
					Set(ref Indown);
					Set(ref Inleft);
					break;
				case 27000:
					Set(ref Inleft);
					break;
				case 31500:
					Set(ref Inleft);
					Set(ref Inup);
					break;
			}


			if (Inz)
				Inz1 = Inz;

			if (Inz && binz)
				Inz1 = false;
			Camera = c;
			Map = m;
		}

		public bool AnyKeyPressed => Inup || Indown || Inleft || Inright || Inz || Inlshift;

	}

	public class EventScript
	{
		private static string[] ParseArgs(string cs)
		{
			var list = new List<string>();
			var buffer = "";
			var quotFlag = false;
			
			for (var i = 0; i < cs.Length; i++)
			{
				var c = cs[i];
				var cm1 = i > 0 ? cs[i - 1] : '\0';
				var cp1 = i < cs.Length - 1 ? cs[i + 1] : '\0';
				switch (c)
				{
					case '\\':
						switch (cp1)
						{
							case 'n':
								buffer = buffer + '\n';
								break;
							case '"':
								buffer = buffer + '"';
								break;
							case '\\':
								buffer = buffer + '\\';
								break;
							default:
								throw new ArgumentException($@"Invalid escape sequence \{cp1}");
						}
						i++;
						break;
					case '"':
						quotFlag = !quotFlag;
						break;
					default:
						if ((c == ' ') && !quotFlag)
						{
							list.Add(buffer);
							buffer = "";
						}
						else
						{
							buffer = buffer + c;
						}
						break;
				}
			}
			list.Add(buffer);
			return list.ToArray();
		}
	}

	public enum PlayerGender
	{
		/// <summary>
		/// 男主人公。
		/// </summary>
		Male,
		/// <summary>
		/// 女主人公。
		/// </summary>
		Female,
		/// <summary>
		/// その他。
		/// </summary>
		Other
	}

}
