using DxLibDLL;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using DefenderStory.Map;
using static DxLibDLL.DX;
using MusicSheet.Sequence;
using System.Collections.Generic;
using System.Windows.Forms;
using Codeplex.Data;
using DefenderStory.Data;
using DefenderStory.Util;
using DefenderStory.Entities;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.Reflection;

namespace DefenderStory
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
		static int bsec, bmsec, level = 1, area;
		public static Size map;
		public static byte[,,] chips;
		static EntityList entitylist;
		static string mptname;
		public static Data.Object[] mptobjects;
		static int f = 0, fps = 0;
		static bool binz = false;
		public static Point camera = new Point(0, 0);
		static int btime = DX.GetNowCount();
		public static int time = 0;
		static bool IsDyingflag, goalflag;
		public static int tick;
		public static Sequencer seq;
		public static Status ks;
		public static Size scrSize;
		static int[] hndl_mpt;
		static int hndl_bg;
		public static AreaInfo areainfo;
		public static LevelData data;
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
		public static bool isDebugMode = false;
		/// <summary>
		/// 初期化したかどうかを取得または設定します。
		/// </summary>
		public static bool IsInit { get; set; }
		static int bcoin = 0;
		/// <summary>
		/// 開始レベルを定義します。
		/// </summary>
		public const int START_LEVEL = 2;
		/// <summary>
		/// 現在のゲームモード。
		/// </summary>
		public static GameMode gamemode;

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
			bsec = DateTime.Now.Second;
			bmsec = DateTime.Now.Millisecond;
			level = START_LEVEL;
			map = new Size(20, 15);
			hPauseScreen = DX.MakeScreen(s.Width, s.Height);
			entitylist = new EntityList();
			Coin = 0;
			Life = 5;
			time = 300;
			IsGoal = false;
			NextStage = 0;
			scrSize = s;
			IsInit = true;
			gamemode = GameMode.Title;
			hMainScreen = DX.MakeScreen(s.Width, s.Height);
			#endregion

			Load(level);
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
				gamemode = GameMode.Ending;
				BGMPlay("omedeto.mid");
				return;
			}
			Load(level, area, PlayerForm.Mini);
		}

		/// <summary>
		/// レベル番号を指定して、マップを読み込みます。
		/// </summary>
		/// <param name="level"></param>
		public static void Load(int level)
		{
			if (level == -1)
			{
				gamemode = GameMode.Ending;
				BGMPlay("omedeto.mid");
				return;
			}
			Load(level, PlayerForm.Mini);
		}

		/// <summary>
		/// レベル番号とエリア番号とプレイヤー状態を指定して、マップを読み込みます。
		/// </summary>
		/// <param name="level"></param>
		/// <param name="area"></param>
		/// <param name="pf"></param>
		public static void Load(int level, int area, PlayerForm pf)
		{
			if (level == -1)
			{
				gamemode = GameMode.Ending;
				BGMPlay("omedeto.mid");
				return;
			}
			if (data != null)
				time = data.Time;

			areainfo = GetJsonData<AreaInfo>("Resources\\Levels\\Level " + level + "\\Area " + area + "\\area.json");

			mptname = areainfo.Mpt;

			hndl_mpt = ResourceUtility.GetMpt(mptname);
			int hndl_mptsoft = DX.LoadSoftImage("Resources\\Graphics\\" + mptname + "_hj.png");

			

			mptobjects = new Data.Object[64];

			int r, g, b, a;
			Color[] hits = new Color[5];
			for (int i = 0; i < 5; i++)
			{
				DX.GetPixelSoftImage(hndl_mptsoft, i, 64, out r, out g, out b, out a);
				hits[i] = Color.FromArgb(r, g, b, a);
			}

			List<Color> hitlist = hits.ToList();
			byte[,] mask = new byte[16, 16];
			for (int iy = 0; iy < 4; iy++)
			{
				for (int ix = 0; ix < 16; ix++)
				{
					for (int y = 0; y < 16; y++)
						for (int x = 0; x < 16; x++)
						{
							DX.GetPixelSoftImage(hndl_mptsoft, x + ix * 16, y + iy * 16, out r, out g, out b, out a);
							mask[x, y] = (byte)hitlist.IndexOf(Color.FromArgb(r, g, b, a));
						}
					mptobjects[iy * 16 + ix] = new Data.Object(hndl_mpt[iy * 16 + ix], (byte[,])mask.Clone());
				}
			}

			DX.DeleteSoftImage(hndl_mptsoft);

			entitylist.Clear();

			MapUtility.LoadMap(out chips, "Resources\\Levels\\Level " + level + "\\Area " + area + "\\map.citmap");
			map = new Size(chips.GetLength(0), chips.GetLength(1));

			hndl_bg = DX.LoadGraph("Resources\\Graphics\\" + areainfo.BG);

			dynamic spdata = DynamicJson.Parse(File.ReadAllText("Resources\\Levels\\Level " + level + "\\Area " + area + "\\spdata.json"));

			foreach (dynamic obj in spdata)
			{
				int posx = (int)obj.PosX, posy = (int)obj.PosY, spid = (int)obj.EntityID;
				try
				{
					if (obj.IsDefined("Tag"))
						obj.EntityData.Tag = obj.Tag;
					if (EntityRegister.GetDataById(spid) != null)
						entitylist.Add(EntityRegister.CreateEntity(spid, new PointF(posx, posy), mptobjects, chips, entitylist, obj.EntityData), spid == 0);
				}
				catch (InvalidOperationException ioe)
				{
					Console.WriteLine("[ERROR]" + ioe.Message);
				}
			}

			if (!GameEngine.Middle.IsEmpty)
			{
				entitylist.MainEntity.Location.X = GameEngine.Middle.X;
				entitylist.MainEntity.Location.Y = GameEngine.Middle.Y;
				entitylist.MainEntity.Location = GameEngine.Middle;

			}

			if (entitylist.MainEntity is EntityPlayer)
			{
				var p = (EntityPlayer)entitylist.MainEntity;
				p.Form = pf;
			}

			camera = new Point((int)(-entitylist.MainEntity.Location.X + 160), (int)(-entitylist.MainEntity.Location.Y + 160));
			BGMPlay(areainfo.Music);
			IsDyingflag = false;
			goalflag = false;
			tick = 0;
			ks = new Status(binz, camera, map);
		}

		/// <summary>
		/// ファイル名を指定して、BGM を再生します。
		/// </summary>
		/// <param name="name"></param>
		public static void BGMPlay(string name)
		{
			if (seq == null)
				seq = new Sequencer();
			seq.Stop();
			seq.Reset();
			seq.SetMidiData(ResourceUtility.MusicList[name]);
			seq.Play();
		}

		/// <summary>
		/// BGM を停止します。
		/// </summary>
		public static void BGMStop()
		{
			seq.Stop();
		}

		/// <summary>
		/// 指定した時間で、 BGM をフェードアウトします。
		/// </summary>
		/// <param name="time">時間(単位はミリ秒)。</param>
		public static void BGMStop(int time)
		{
			Task.Factory.StartNew(new Action(() =>
			{
				int bvol = seq.sm.Volume;
				for (int i = 0; i < time; i++)
				{
					seq.sm.Volume = bvol - (int)(bvol * (i / (float)time));
					Thread.Sleep(1);
				}
				BGMStop();
			}));
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
				gamemode = GameMode.Ending;
				BGMPlay("omedeto.mid");
				return;
			}
			GameEngine.level = level;
			data = GetJsonData<LevelData>("Resources\\Levels\\Level " + level + "\\lvldat.json");
			area = data.FirstArea;
			time = data.Time;
			storytext = File.ReadAllText("Resources\\Levels\\Level " + level + "\\story.txt");
			Load(level, area, pf);
			seq.Stop();
			splashtime = 120;
		}

		/// <summary>
		/// Json 文字列を .NET オブジェクトに変換し、指定した型にキャストして返します。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="json"></param>
		/// <returns></returns>
		public static T GetJsonData<T>(string json)
		{
			string jsonstring = File.ReadAllText(json);
			var s = new DataContractJsonSerializer(typeof(T));
			var jsonBytes = Encoding.Unicode.GetBytes(jsonstring);
			var sr = new MemoryStream(jsonBytes);
			var obj = (T)s.ReadObject(sr);

			return obj;
		}

		static int guiCursor = 0;

		/// <summary>
		/// ポーズしたときにプレイ中の画面をぼかして表示するのに使うハンドル
		/// </summary>
		static int hPauseScreen;
		static bool binf3, inf3;
		static bool binup, bindown;
		static int binesc;
		static int splashtime = 0;
		static EndingMode endmode;
		static string credit;
		static int height = 0;
		static int graphhandle;
		static int graphhandle2;
		static int gametick = 0;
		private static TitleMode titlemode;
		static bool storyflag = false;
		static string storytext;
		static string[] storylines;
		static int storyline = 0;
		static string serif = "";
		static string storyimg = "";
		static bool storywaiting = false;
		static bool nowserif = false;
		static string[] serifs;
		static int serifptr = 0;
		static string bgpath = "";
		static string bgpath2;
        static GameMode bgamemode;
		static Dictionary<string, string> facealias = new Dictionary<string, string>();
		static Regex regpreprc = new Regex(@"\[(.+)?\:(.+)\]");
		public static string GameVersion
		{ get; set; } = "1.0.0-a";
		public static string Copyright
		{ get; set; } = "(C)2014-2015 ＣCitringo";
		static int helppage = 0;
		static MatchCollection m;
		/// <summary>
		/// メイン画面バッファのハンドル。
		/// </summary>
		public static int hMainScreen;

		public static Action finallyprocess;

		/// <summary>
		/// ゲームのループ処理をします。
		/// </summary>
		public static int DoGameLoop()
		{
			if (!IsInit)
				throw new Exception("ゲームエンジンが初期化されていません。");
			int h = DX.GetDrawScreen();
			DX.SetDrawScreen(hMainScreen);
			ks.Update(binz, camera, map);
			int inesc = DX.CheckHitKey(DX.KEY_INPUT_ESCAPE);
			DX.ClearDrawScreen();  //消し去る
			seq.PlayLoop();
			//----FPS 測定
			f++;
			
			if (bsec != DateTime.Now.Second)
			{
				fps = f;
				f = 0;
				bsec = DateTime.Now.Second;
			}
			switch (gamemode)
			{
				case GameMode.Action:
					#region ActionMode
					//デバッグモードの切り替え
					binf3 = inf3;
					inf3 = DX.CheckHitKey(DX.KEY_INPUT_F3) == 1;
					if (inf3 && !binf3)
					{
						isDebugMode = !isDebugMode;
					}

					#region スプラッシュ
					if (splashtime > 0)
					{
						DX.ClearDrawScreen();
						string ahi = string.Format("Level {0}", level);
						FontUtility.DrawString(160 - ahi.Length * 5, 96, ahi, 0xffffff);

						splashtime--;
						if (splashtime == 0)
						{
							BGMPlay(areainfo.Music);
							IsDyingflag = false;
							goalflag = false;
							tick = 0;

						}
						
						break;
					}
					#endregion

					#region 自動制御
					//ゴールしたら自動制御
					if (GameEngine.IsGoal)
					{
						ks.indown = false;
						ks.inleft = false;
						ks.inlshift = false;
						ks.inright = true;
						ks.inup = false;
						ks.inz = false;
						ks.inz1 = false;
						if (time > 0)
						{
							time--;
							if (time % 10 == 0)
							{
								SoundUtility.PlaySound(Sounds.GetCoin);
								Coin++;
							}
						}
						if (!goalflag)
						{
							BGMPlay("jingle_gameclear.mid");
							goalflag = true;
						}
					}

					//Level 2-1 での制御
					if (GameEngine.level == 2 && GameEngine.area == 1)
					{
						ks.indown = false;
						ks.inleft = false;
						ks.inlshift = false;
						ks.inright = true;
						ks.inup = false;
						ks.inz = false;
						ks.inz1 = false;
						if (entitylist.MainEntity.Location.X >= 300)
						{
							level = 2;
							area = 2;
							Load(2, 2, ((entitylist.MainEntity is EntityPlayer) ? ((EntityPlayer)entitylist.MainEntity).Form : PlayerForm.Mini));
							
							break;
						}
					}
					#endregion

					//ゴールしたあと画面遷移するときにいろいろリセットする
					if (GameEngine.IsGoal && !seq.IsPlaying && time == 0)
					{
						GameEngine.IsGoal = false;
						GameEngine.Middle = Point.Empty;
						level = GameEngine.NextStage;
						gamemode = GameMode.Story;
						Load(level, ((entitylist.MainEntity is EntityPlayer) ? ((EntityPlayer)entitylist.MainEntity).Form : PlayerForm.Mini));
						
						break;

					}

					//ESC キーでブレイクタイム
					if (inesc == 1 && binesc != 1)
					{
						SetBreakTime(null, GameMode.Action);
						break;
					}

					//主人公が死んでしばらくしたら振り出しに戻して始める処理をする
					if (entitylist.MainEntity.IsDead && !seq.IsPlaying)
					{
						if (Life == 0)
						{
							BGMPlay("nahha.mid");
							guiCursor = 0;
							IsGameOver = true;
							
							break;
						}
						Load(level, area);
						
						break;
					}

					#region 主人公処理
					//主人公が死んだ瞬間に音楽を切り替える
					if (((EntityLiving)(entitylist.MainEntity)).IsDying && !IsDyingflag)
					{
						BGMPlay("zannnenn.mid");
						Life--;
						//バイブレーションを鳴らす
						DX.StartJoypadVibration(DX.DX_INPUT_PAD1, 500, 1500);
						IsDyingflag = true;
					}

					//主人公が落下死したらパーティクルを出す
					if (tick % 2 == 0 && ((EntityLiving)(entitylist.MainEntity)).IsDying && ((EntityLiving)(entitylist.MainEntity)).IsFall)
					{
						for (int i = 0; i < 5; i++)
							entitylist.Add(EntityRegister.CreateEntity("Star", new Point((int)entitylist.MainEntity.Location.X + DX.GetRand(32) - 16, ks.map.Height * 16 - 1), mptobjects, chips, entitylist));
					}
					#endregion


					#region カメラ処理
					if (!((EntityLiving)entitylist.MainEntity).IsDying)
					{
						if (entitylist.MainEntity.Location.X + ks.camera.X > scrSize.Width / 2 && entitylist.MainEntity.Velocity.X > 0 && ks.camera.X > -ks.map.Width * 16 + scrSize.Width)
						{
							//ks.camera.Offset(-(int)entitylist.MainEntity.Velocity.X, 0);
							ks.camera = new Point(-(int)entitylist.MainEntity.Location.X + scrSize.Width / 2, ks.camera.Y);
						}

						if (ks.map.Width * 16 - entitylist.MainEntity.Location.X > scrSize.Width / 2 && entitylist.MainEntity.Velocity.X < 0 && ks.camera.X < 0)
						{
							//ks.camera.Offset(-(int)entitylist.MainEntity.Velocity.X, 0);
							ks.camera = new Point(-(int)entitylist.MainEntity.Location.X + scrSize.Width / 2, ks.camera.Y);
						}

						if (entitylist.MainEntity.Location.Y + ks.camera.Y > scrSize.Height / 2 && entitylist.MainEntity.Velocity.Y > 0 && ks.camera.Y > -ks.map.Height * 16 + scrSize.Height)
						{
							ks.camera.Offset(0, -(int)entitylist.MainEntity.Velocity.Y);
							//ks.camera = new Point(ks.camera.X , -(int)entitylist.MainEntity.Location.Y + scrSize.Height / 2);
						}

						if (ks.map.Height * 16 - entitylist.MainEntity.Location.Y > scrSize.Height / 2 && entitylist.MainEntity.Velocity.Y < 0 && ks.camera.Y < 0)
						{
							ks.camera.Offset(0, -(int)entitylist.MainEntity.Velocity.Y);
							//ks.camera.Offset(ks.camera.X, -(int)entitylist.MainEntity.Location.Y + scrSize.Height / 2);

						}

						if (ks.camera.X > 0)
							ks.camera = new Point(0, ks.camera.Y);

						if (ks.camera.Y > 0)
							ks.camera = new Point(ks.camera.X, 0);

						if (ks.camera.X < -ks.map.Width * 16 + scrSize.Width)
							ks.camera = new Point(-ks.map.Width * 16 + scrSize.Width, ks.camera.Y);

						if (ks.camera.Y < -ks.map.Height * 16 + scrSize.Height)
							ks.camera = new Point(ks.camera.X, -ks.map.Height * 16 + scrSize.Height);
					}
					#endregion

					int bgx = (int)(-((-ks.camera.X * (areainfo.ScrollSpeed / 10.0)) % scrSize.Width));

					#region 描画
					//背景を描画
					DX.DrawExtendGraph(bgx, 0, bgx + scrSize.Width, scrSize.Height, hndl_bg, 0);
					DX.DrawExtendGraph(bgx + scrSize.Width, 0, bgx + scrSize.Width * 2, scrSize.Height, hndl_bg, 0);    //スクロールするから2枚使ってループできるようにしている

					//----背面の mpt を描画している
					for (int y = 0; y < map.Height * 16; y += 16)
						for (int x = 0; x < map.Width * 16; x += 16)
							DX.DrawGraph(x + camera.X, y + camera.Y, hndl_mpt[chips[x / 16, y / 16, 1]], 1);

					//----エンティティ達を動かす
					entitylist.Draw(ref ks, ref chips);
					if (((EntityLiving)entitylist.MainEntity).IsDying && IsGoal)
						((EntityLiving)entitylist.MainEntity).IsDying = false;

					//----前面の mpt を描画している
					for (int y = 0; y < map.Height * 16; y += 16)
						for (int x = 0; x < map.Width * 16; x += 16)
							DX.DrawGraph(x + camera.X, y + camera.Y, hndl_mpt[chips[x / 16, y / 16, 0]], 1);
					#endregion


					//コイン50枚毎に1UPの処理する
					if (bcoin != Coin && Coin % 50 == 0)
					{
						Life++;
						SoundUtility.PlaySound(Sounds.Player1Up);
					}

					#region デバッグモード処理
					//テキストを表示
					//通常表示とデバッグモード表示
					string buf = string.Format("①{0} ♥{1} レベル{2} ⌚{3}", GameEngine.Coin, GameEngine.Life, level, time);
					if (isDebugMode)
					{
						entitylist.DebugDraw(ref ks, ref chips);
						buf = string.Format("MS{0}/{1} P({2},{3}) {4}\nS{5} MS({6},{7}) CP({8},{9})\nV({10},{11}) AS{12} SS({13},{14})\nEntityPlayer: {15}\nLift: {16}\nIRun: {17}\nINPUT: {18}",
							//Music Sheet データと主人公の座標
							seq.nTickCount, seq.eot, (int)entitylist.MainEntity.Location.X, (int)entitylist.MainEntity.Location.Y, fps,
							//マップとカメラとエンティティ数
							entitylist.Count, map.Width, map.Height, camera.X, camera.Y,
							//現在アクティブなエンティティ数を演算
							(int)entitylist.MainEntity.Velocity.X, (int)entitylist.MainEntity.Velocity.Y, entitylist.Count(new Func<Entity, bool>((sp) =>
							{
								if (Math.Abs(entitylist.MainEntity.Location.X - sp.Location.X) > scrSize.Width)
									return false;
								return true;
							})), scrSize.Width, scrSize.Height,
							//主人公の安否
							((EntityLiving)entitylist.MainEntity).IsDying ? "DEAD" : "ALIVE",
							//甲羅が持たれているかどうかの取得
							new Func<IEnumerable<EntityTurcosShell>, string>((sps) =>
							{
								string retval = "";
								foreach (EntityTurcosShell tsb in sps)
									retval += (tsb.Owner != null ? "T" : "F");
								return retval;
							}).Invoke(entitylist.OfType<EntityTurcosShell>()),
							//甲羅が投げられているかどうかの取得
							new Func<IEnumerable<EntityTurcosShell>, string>((sps) =>
							{
								string retval = "";
								foreach (EntityTurcosShell tsb in sps)
									retval += (tsb.isRunning ? "T" : "F");
								return retval;
							}).Invoke(entitylist.OfType<EntityTurcosShell>()),
							//入力の取得
							new Func<string>(() =>
							{
								var lastdata = "";
								var joydata = DX.GetJoypadInputState(DX.DX_INPUT_KEY_PAD1);
								//forで回せるけどこうしたほうがわかりやすいと思う
								if ((joydata & DX.PAD_INPUT_1) == DX.PAD_INPUT_1)
									lastdata += "1 ";
								if ((joydata & DX.PAD_INPUT_2) == DX.PAD_INPUT_2)
									lastdata += "2 ";
								if ((joydata & DX.PAD_INPUT_3) == DX.PAD_INPUT_3)
									lastdata += "3 ";
								if ((joydata & DX.PAD_INPUT_4) == DX.PAD_INPUT_4)
									lastdata += "4 ";
								if ((joydata & DX.PAD_INPUT_5) == DX.PAD_INPUT_5)
									lastdata += "5 ";
								if ((joydata & DX.PAD_INPUT_6) == DX.PAD_INPUT_6)
									lastdata += "6 ";
								if ((joydata & DX.PAD_INPUT_7) == DX.PAD_INPUT_7)
									lastdata += "7 ";
								if ((joydata & DX.PAD_INPUT_8) == DX.PAD_INPUT_8)
									lastdata += "8 ";
								if ((joydata & DX.PAD_INPUT_9) == DX.PAD_INPUT_9)
									lastdata += "9 ";
								if ((joydata & DX.PAD_INPUT_10) == DX.PAD_INPUT_10)
									lastdata += "10 ";
								if ((joydata & DX.PAD_INPUT_11) == DX.PAD_INPUT_11)
									lastdata += "11 ";
								if ((joydata & DX.PAD_INPUT_12) == DX.PAD_INPUT_12)
									lastdata += "12 ";
								if ((joydata & DX.PAD_INPUT_13) == DX.PAD_INPUT_13)
									lastdata += "13 ";
								if ((joydata & DX.PAD_INPUT_14) == DX.PAD_INPUT_14)
									lastdata += "14 ";
								if ((joydata & DX.PAD_INPUT_15) == DX.PAD_INPUT_15)
									lastdata += "15 ";
								if ((joydata & DX.PAD_INPUT_16) == DX.PAD_INPUT_16)
									lastdata += "16 ";
								if ((joydata & DX.PAD_INPUT_17) == DX.PAD_INPUT_17)
									lastdata += "17 ";
								if ((joydata & DX.PAD_INPUT_18) == DX.PAD_INPUT_18)
									lastdata += "18 ";
								if ((joydata & DX.PAD_INPUT_19) == DX.PAD_INPUT_19)
									lastdata += "19 ";
								if ((joydata & DX.PAD_INPUT_20) == DX.PAD_INPUT_20)
									lastdata += "20 ";
								if ((joydata & DX.PAD_INPUT_21) == DX.PAD_INPUT_21)
									lastdata += "21 ";
								if ((joydata & DX.PAD_INPUT_22) == DX.PAD_INPUT_22)
									lastdata += "22 ";
								if ((joydata & DX.PAD_INPUT_23) == DX.PAD_INPUT_23)
									lastdata += "23 ";
								if ((joydata & DX.PAD_INPUT_24) == DX.PAD_INPUT_24)
									lastdata += "24 ";
								if ((joydata & DX.PAD_INPUT_25) == DX.PAD_INPUT_25)
									lastdata += "25 ";
								if ((joydata & DX.PAD_INPUT_26) == DX.PAD_INPUT_26)
									lastdata += "26 ";
								if ((joydata & DX.PAD_INPUT_27) == DX.PAD_INPUT_27)
									lastdata += "27 ";
								if ((joydata & DX.PAD_INPUT_28) == DX.PAD_INPUT_28)
									lastdata += "28 ";
								return lastdata;
							}).Invoke());
					}
					FontUtility.DrawString(0, 0, buf, 0xffffff);
					#endregion


					//タイムをデクリメント、なくなったら殺す
					if (!IsGoal && tick % 60 == 0 && !((EntityLiving)entitylist.MainEntity).IsDying)
					{
						if (time == 0)
						{
							((EntityLiving)entitylist.MainEntity).Kill();
						}
						else
							time--;
					}
					//ポーズの切り替え


					bool inenter = DX.CheckHitKey(DX.KEY_INPUT_RETURN) == 1;
					/*if (inenter)
					{
						gamemode = GameMode.Breaktime;
					}*/

					#region ゲームオーバーダイアログ
					if (IsGameOver)
					{
						DX.DrawBox(64, 64, 256, 176, 0x7f000000, 1);
						FontUtility.DrawString(124, 96, "GAME OVER", 0xffffff);
						FontUtility.DrawString(124, 106, "  Continue", 0xffffff);
						FontUtility.DrawString(124, 116, "  End", 0xffffff);
						FontUtility.DrawString(124, 106 + 10 * guiCursor, ">", 0xffffff);

						if (ks.inup && !binup)
						{
							guiCursor ^= 1;
						}
						if (ks.indown && !bindown)
						{
							guiCursor ^= 1;
						}
						if (ks.inz)
						{
							switch (guiCursor)
							{
								case 0:
									IsGameOver = false;
									Life = 5;
									Middle = Point.Empty;
									Load(level);
									break;
								case 1:
									ks.inz1 = false;
									titlemode = TitleMode.Opening;
									gamemode = GameMode.Title;
									break;
							}
						}
					}
					#endregion

					bcoin = Coin;
					break;
				#endregion
				case GameMode.Story:
					#region StoryMode
					//ESC キーでブレイクタイム
					if (inesc == 1 && binesc != 1)
					{
						SetBreakTime(new Action(() =>
						{
							storyflag = false;
							if (graphhandle != 0)
								DX.DeleteGraph(graphhandle);
							graphhandle = 0;
							if (graphhandle2 != 0)
								DX.DeleteGraph(graphhandle2);
							graphhandle2 = 0;
						}), gamemode);
						
						break;
					}
					if (!storyflag) //初期化
					{
						storyflag = true;
						storyline = 0;
						serif = "";
						storylines = storytext.Split('\n');
						facealias.Clear();
					}



					if (regpreprc.IsMatch(storylines[storyline]))	//プリプロセッサ
					{
						m = regpreprc.Matches(storylines[storyline]);
						
						switch (m[0].Groups[1].Value)
						{
							case "img":
								if (graphhandle != 0)
								{
									DX.DeleteGraph(graphhandle);
									graphhandle = 0;
								}
								if (m[0].Groups[2].Value == "white")
								{
									bgpath = "white";
									graphhandle = DX.MakeScreen(scrSize.Width, scrSize.Height);
									DX.FillGraph(graphhandle, 255, 255, 255);
									break;
								}
								graphhandle = DX.LoadGraph(bgpath = "Resources\\Graphics\\" + m[0].Groups[2].Value);
								break;
							case "bgm":
								if (seq.IsPlaying)
									BGMStop();
								BGMPlay(m[0].Groups[2].Value);
								break;
							case "face":
								string[] val = m[0].Groups[2].Value.Split(',');
								facealias.Add(val[0], val[1]);
								break;

						}
					}
					if (!storywaiting && !nowserif && !regpreprc.IsMatch(storylines[storyline]) && storylines[storyline].IndexOf(',') > -1)
					{
						nowserif = true;
						serifs = storylines[storyline].Split(',');
						serifptr = 0;
						serif = "";
						if (graphhandle2 != 0)
						{
							DX.DeleteGraph(graphhandle2);
							graphhandle2 = 0;
						}
						if (facealias.ContainsKey(serifs[0]))
						{
							graphhandle2 = DX.LoadGraph(bgpath2 = "Resources\\Graphics\\" + facealias[serifs[0]] + "\\" + serifs[2] + ".png");
						}
						else
						{
							graphhandle2 = DX.LoadGraph(bgpath2 = "Resources\\Graphics\\" + serifs[0] + "\\" + serifs[2] + ".png");
						}
						if (serifs.Length != 3)
						{
							throw new Exception("セリフの記述が不適切です。");
						}
					}

					if (!storywaiting && nowserif && tick % (ks.inz ? 1 : 4) == 0)
					{
						serif += serifs[1][serifptr];
						SoundUtility.PlaySound(Sounds.Saying);
						if (serifs[1][serifptr] == '。')
						{
							storywaiting = true;
						}
						serifptr++;
						if (serifs[1].Length <= serifptr)
						{
							storywaiting = true;
							nowserif = false;
						}
					}


					if (graphhandle != 0)
					{
						DX.DrawGraph(0, 0, graphhandle, 0);
					}

					if (serifs != null)
					{
						DX.DrawBox(0, scrSize.Height - 64, 8 + FontUtility.GetDrawingSize(serifs[0]).Width, scrSize.Height - 64 - 16, DX.GetColor(64, 64, 64), 1);
						FontUtility.DrawString(4, scrSize.Height - 64 - 13, serifs[0], Color.White);
					}

					DX.DrawBox(0, scrSize.Height - 64, scrSize.Width, scrSize.Height, DX.GetColor(64, 64, 64), 1);
					FontUtility.DrawString(8, scrSize.Height - 56, scrSize.Width - 8, scrSize.Height - 8, serif, Color.White);
					if (storywaiting)
					{
						FontUtility.DrawMiniString(scrSize.Width - 12, scrSize.Height - 12, (tick % 16 < 8 ? "▼" : "▽"), Color.White);
					}

					DX.DrawExtendGraph(90, 16, 230, 156, graphhandle2, 0);

					if (storywaiting)
					{
						if (ks.inz1)
						{
							ks.inz1 = false;
							storywaiting = false;
						}
					}

					if (!nowserif && !storywaiting)
						storyline++;
					if (storylines.Length <= storyline)
					{
						storyflag = false;
						DX.DeleteGraph(graphhandle);
						graphhandle = 0;
						DX.DeleteGraph(graphhandle2);
						graphhandle2 = 0;
						gamemode = GameMode.Action;
						if (seq.IsPlaying)
							BGMStop(500);
					}
					#endregion
					break;
				case GameMode.Title:
					#region TitleMode
					if (inesc == 1 && binesc != 1　&& titlemode == TitleMode.MainTitle)
					{
						Environment.Exit(0);
						
						break;
					}
					switch (titlemode)
					{
						case TitleMode.Opening:
							// TODO: オープニングムービーが完成したらここを実装する。
							titlemode = TitleMode.GameLogoRising;
							break;
						case TitleMode.GameLogoRising:
							#region
							if (gametick > 360 || ks.inz1)
							{
								titlemode = TitleMode.GameLogoLightning;
								gametick = 0;
								SoundUtility.PlaySound(Sounds.Flash);
								ks.inz1 = false;
								break;
							}
							
							DX.DrawGraph(scrSize.Width / 2 - 180 / 2, (int)(scrSize.Height - Math.Min(220, (float)gametick / 300 * 220)), ResourceUtility.Logo[0], 1);
				
							gametick++;
							break;
						#endregion
						case TitleMode.GameLogoLightning:
							#region
							if (gametick < 60)
							{
								DX.DrawGraph(scrSize.Width / 2 - 180 / 2, (int)(scrSize.Height - 220), ResourceUtility.Logo[Convert.ToInt32(gametick % 8 > 3)], 1);
							}
							else
								DX.DrawGraph(scrSize.Width / 2 - 180 / 2, (int)(scrSize.Height - 220), ResourceUtility.Logo[0], 1);
							if (gametick > 120 || ks.inz1)
							{
								gametick = 0;
								titlemode = TitleMode.MainTitle;
								graphhandle = DX.LoadGraph("Resources\\Graphics\\story_1.bmp");
								BGMPlay("c011.mid");
								guiCursor = 0;
								ks.inz1 = false;
							}
							gametick++;
							break;
						#endregion
						case TitleMode.MainTitle:
							#region 

							#region 描画
							DX.DrawGraph(0, 0, graphhandle, 0);
							DX.DrawGraph(scrSize.Width / 2 - 180 / 2, (int)(scrSize.Height - 220), ResourceUtility.Logo[0], 1);
							var uistr = $"{(guiCursor == 0 ? ">" : " ")}はじめから\n\n{(guiCursor == 1 ? ">" : " ")}設定\n\n{(guiCursor == 2 ? ">" : " ")}ヘルプ\n\n{(guiCursor == 3 ? ">" : " ")}おわる";
							FontUtility.DrawString(scrSize.Width / 2 - 40, scrSize.Height / 2 + 16, uistr, Color.Black);
							FontUtility.DrawMiniString(0, scrSize.Height - 8, Copyright, Color.Black);
							FontUtility.DrawMiniString(scrSize.Width - GameVersion.Length * 6, scrSize.Height - 8, GameVersion, Color.Black);

							#endregion

							#region 判定
							if (binup != ks.inup && ks.inup)
							{
								guiCursor--;
								if (guiCursor < 0)
									guiCursor = 3;
								SoundUtility.PlaySound(Sounds.Selected);
							}
							
							if (bindown != ks.indown && ks.indown)
							{
								guiCursor++;
								if (guiCursor > 3)
									guiCursor = 0;
								SoundUtility.PlaySound(Sounds.Selected);
							}
							if (ks.inz1)
							{
								switch (guiCursor)
								{
									case 0:
										gamemode = GameMode.Story;
										BGMStop();
										BGMPlay("nothing.mid");
										ks.inz1 = false;
										DX.DeleteGraph(graphhandle);
										graphhandle = 0;
										break;
									case 1:
										titlemode = TitleMode.Setting;
										break;
									case 2:
										titlemode = TitleMode.Help;
										break;
									case 3:
										Environment.Exit(0);
										break;
									default:

										break;
								}
								SoundUtility.PlaySound(Sounds.Pressed);
							}
							break;
						#endregion
						#endregion
						case TitleMode.Setting:
							FontUtility.DrawString(32, "設定できることは何もありません。\nEsc キーまたは [START] ボタンを押してください", Color.White);
							if (inesc == 1 && binesc != 1)
								titlemode = TitleMode.MainTitle;
							break;
						case TitleMode.Help:
							if (graphhandle2 == 0)
							{
								graphhandle2 = DX.LoadGraph("Resources\\Graphics\\joypadactgame.png");
							}
							if (helppage == 1 && ks.inlshift)
							{
								DX.DeleteGraph(graphhandle2);
								graphhandle2 = DX.LoadGraph("Resources\\Graphics\\joypadactgame.png");
								helppage--;
							}
							if (ks.inz1)
							{
								ks.inz1 = false;
								helppage++;
								DX.DeleteGraph(graphhandle2);
								graphhandle2 = DX.LoadGraph("Resources\\Graphics\\joypadstory.png");
								if (helppage == 2)
								{
									helppage = 0;
									DX.DeleteGraph(graphhandle2);
									graphhandle2 = 0;
									titlemode = TitleMode.MainTitle;
								}
							}
							DX.DrawGraph(0, 0, graphhandle2, 0);
							if (inesc == 1 && binesc != 1)
							{
								helppage = 0;
								DX.DeleteGraph(graphhandle2);
								graphhandle2 = 0;
								titlemode = TitleMode.MainTitle;
							}
							break;
					}
					#endregion
					break;
				case GameMode.Ending:
					#region EndingMode
					//ESC キーでブレイクタイム
					if (inesc == 1 && binesc != 1)
					{
						gamemode = GameMode.Breaktime;
						bgamemode = GameMode.Ending;
						guiCursor = 0;
						
						break;
					}
					switch (endmode)
					{
						case EndingMode.Message:
							FontUtility.DrawString(48, @"今回はここまでです!
アップデートをおまちください!", 0xffffff);
							if (ks.inz || !seq.IsPlaying)
							{
								endmode = EndingMode.Credit;
								camera.Y = 0;
								credit = File.ReadAllText("Resources\\Document\\staffrole.txt");
								BGMPlay("c011_piano.mid");
								height = FontUtility.GetDrawingSize(credit).Height + 10;
								graphhandle = DX.MakeScreen(scrSize.Width, height);
								DX.SetDrawScreen(graphhandle);
								FontUtility.DrawString(camera.Y, credit, 0xffffff);
								DX.SetDrawScreen(hMainScreen);
								gametick = DX.GetNowCount();
							}
							break;
						case EndingMode.Credit:
							camera.Y = scrSize.Height - (int)(((float)(DX.GetNowCount() - gametick) / seq.MusicTime) * (height + scrSize.Height - 10));
							DX.DrawGraph(0, camera.Y, graphhandle, 0);
							//FontUtility.DrawMiniString(0, scrSize.Height - 8, $"{camera.Y} / {-height}", 0xffffff);
							if (camera.Y < -height)
							{
								endmode = EndingMode.TheEnd;
								DX.DeleteGraph(graphhandle);
							}
							break;
						case EndingMode.TheEnd:
							DX.DrawGraph(scrSize.Width / 2 - 128 / 2, scrSize.Height / 2 - 32 / 2 - 32, ResourceUtility.TheEnd, 0);
							//DX.DrawGraph(0, 0, ResourceUtility.TheEnd, 0);
							//DX.DrawBox(scrSize.Width / 2 - 128 / 2, scrSize.Height / 2 - 32 / 2 - 32, scrSize.Width / 2 - 128 / 2 + 128, scrSize.Height / 2 - 32 / 2 - 32 + 32, DX.GetColor(255, 0, 0), 0);
							FontUtility.DrawMiniString(180, "PUSH ANY BUTTON", 0xffffff);
							if (ks.AnyKeyPressed)
							{
								gamemode = GameMode.Title;
								titlemode = TitleMode.Opening;
								ks.inz1 = false;
							}
							break;
						default:
							break;
					}
					
					#endregion
					break;
				case GameMode.Debug:
					#region Debug
					//ESC キーでブレイクタイム
					if (inesc == 1 && binesc != 1)
					{
						gamemode = GameMode.Breaktime;
						bgamemode = GameMode.Debug;
						guiCursor = 0;
						
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
					if (inesc == 1 && binesc != 1)
					{
						gamemode = GameMode.Breaktime;
						bgamemode = GameMode.Prolog;
						guiCursor = 0;
						
						break;
					}
					if (graphhandle == 0)
					{
						var story = File.ReadAllText("Resources\\Document\\prolog.txt");
						graphhandle = DX.MakeScreen(scrSize.Width, height = FontUtility.GetDrawingSize(story).Height + 32);
						DX.SetDrawScreen(graphhandle);
						FontUtility.DrawString(0, story, Color.White);
						DX.SetDrawScreen(hMainScreen);
						ks.camera.Y = scrSize.Height;
						
					}
					if (tick % 3 == 0)
						ks.camera.Y--;
					DX.DrawGraph(0, ks.camera.Y, graphhandle, 0);
					if (ks.camera.Y < scrSize.Height - height)
					{
						FontUtility.DrawMiniString(scrSize.Width - 100, "PUSH (X) BUTTON", Color.White);
						if (ks.inz1)
						{
							ks.inz1 = false;
							gamemode = GameMode.Story;
							BGMStop();
							DX.DeleteGraph(graphhandle);
							graphhandle = 0;
						}
					}
					#endregion
					break;
				case GameMode.Breaktime:
					#region BreakTime
					FontUtility.DrawString(48, "P A U S E", Color.White);
					FontUtility.DrawString(80, $"{(guiCursor == 0 ? ">" : " ")}つづける", (guiCursor == 0 ? Color.Yellow : Color.White));
					FontUtility.DrawString(90, $"{(guiCursor == 1 ? ">" : " ")}おわる　", (guiCursor == 1 ? Color.Yellow : Color.White));
					if ((ks.inup && !binup) || (ks.indown && !bindown))
						guiCursor ^= 1;
					
					if ((ks.inz1))
                    {
						if (guiCursor == 0)
						{
							gamemode = bgamemode;
						}
						else
						{
							guiCursor = 0;
							if (finallyprocess != null)
								finallyprocess();
							
							titlemode = TitleMode.Opening;
							gamemode = GameMode.Title;
						}
						ks.inz1 = false;
					}
					#endregion
					break;
			}
			binz = ks.inz;
			tick = (tick + 1) % 3600;
			camera = ks.camera;
			map = ks.map;
			binup = ks.inup;
			bindown = ks.indown;
			binesc = inesc;
			DX.SetDrawScreen(h);
			return hMainScreen;
		}

		private static void SetBreakTime(Action a, GameMode g)
		{
			bgamemode = g;
			finallyprocess = a;
			gamemode = GameMode.Breaktime;
			guiCursor = 0;
		}
	
		public static void Reload()
		{
			var h = DX.GetDrawScreen();
			hMainScreen = DX.MakeScreen(scrSize.Width, scrSize.Height);
			DX.SetDrawScreen(hMainScreen);
			ResourceUtility.Init();
			FontUtility.Init();
			switch (gamemode)
			{
				case GameMode.Title:
					switch (titlemode)
					{
						case TitleMode.MainTitle:
							graphhandle = DX.LoadGraph("Resources\\Graphics\\story_1.bmp");
							break;
						case TitleMode.Help:
							if (helppage == 0)
								graphhandle2 = DX.LoadGraph("Resources\\Graphics\\joypadactgame.png");
							if (helppage == 1)
								graphhandle2 = DX.LoadGraph("Resources\\Graphics\\joypadstory.png");
							break;
					}
					break;
				case GameMode.Story:
					if (bgpath == "white")
					{
						graphhandle = DX.MakeScreen(scrSize.Width, scrSize.Height);
						DX.FillGraph(graphhandle, 255, 255, 255);
					}
					else
						graphhandle = DX.LoadGraph(bgpath);
					graphhandle2 = DX.LoadGraph(bgpath2);
					break;
				case GameMode.Ending:
					switch (endmode)
					{
						case EndingMode.Credit:

							graphhandle = DX.MakeScreen(scrSize.Width, height);
							DX.SetDrawScreen(graphhandle);
							FontUtility.DrawString(camera.Y, credit, 0xffffff);
							DX.SetDrawScreen(hMainScreen);
							break;
					}
					break;
				case GameMode.Prolog:
					var story = File.ReadAllText("Resources\\Document\\prolog.txt");
					graphhandle = DX.MakeScreen(scrSize.Width, height = FontUtility.GetDrawingSize(story).Height + 32);
					DX.SetDrawScreen(graphhandle);
					FontUtility.DrawString(0, story, Color.White);
					DX.SetDrawScreen(hMainScreen);
					ks.camera.Y = scrSize.Height;

					break;
			}

			hndl_bg = DX.LoadGraph("Resources\\Graphics\\" + areainfo.BG);
			hndl_mpt = ResourceUtility.GetMpt(mptname);
			int hndl_mptsoft = DX.LoadSoftImage("Resources\\Graphics\\" + mptname + "_hj.png");



			mptobjects = new Data.Object[64];

			int r, g, b, a;
			Color[] hits = new Color[5];
			for (int i = 0; i < 5; i++)
			{
				DX.GetPixelSoftImage(hndl_mptsoft, i, 64, out r, out g, out b, out a);
				hits[i] = Color.FromArgb(r, g, b, a);
			}

			List<Color> hitlist = hits.ToList();
			byte[,] mask = new byte[16, 16];
			for (int iy = 0; iy < 4; iy++)
			{
				for (int ix = 0; ix < 16; ix++)
				{
					for (int y = 0; y < 16; y++)
						for (int x = 0; x < 16; x++)
						{
							DX.GetPixelSoftImage(hndl_mptsoft, x + ix * 16, y + iy * 16, out r, out g, out b, out a);
							mask[x, y] = (byte)hitlist.IndexOf(Color.FromArgb(r, g, b, a));
						}
					mptobjects[iy * 16 + ix] = new Data.Object(hndl_mpt[iy * 16 + ix], (byte[,])mask.Clone());
				}
			}

			DX.DeleteSoftImage(hndl_mptsoft);
			DX.SetDrawScreen(h);
		}

	}

	/// <summary>
	/// ゲームエンジンが使用する様々なステータス変数の集合体を表します。
	/// </summary>
	public struct Status
	{
		public bool inup, indown, inleft, inright, inlshift, inz, inz1;
		public Point camera;
		public Size map;
		public Status(bool binz, Point c, Size m)
		{
			inup = DX.CheckHitKey(DX.KEY_INPUT_UP) == 1;
			indown = DX.CheckHitKey(DX.KEY_INPUT_DOWN) == 1;
			inleft = DX.CheckHitKey(DX.KEY_INPUT_LEFT) == 1;
			inright = DX.CheckHitKey(DX.KEY_INPUT_RIGHT) == 1;
			inlshift = DX.CheckHitKey(DX.KEY_INPUT_LSHIFT) == 1;
			inz = DX.CheckHitKey(DX.KEY_INPUT_Z) == 1;

			int joy = DX.GetJoypadInputState(DX.DX_INPUT_PAD1);
			if (!inup) inup = (joy & DX.PAD_INPUT_UP) == DX.PAD_INPUT_UP;
			if (!indown) indown = (joy & DX.PAD_INPUT_DOWN) == DX.PAD_INPUT_DOWN;
			if (!inleft) inleft = (joy & DX.PAD_INPUT_LEFT) == DX.PAD_INPUT_LEFT;
			if (!inright) inright = (joy & DX.PAD_INPUT_RIGHT) == DX.PAD_INPUT_RIGHT;
			if (!inlshift) inlshift = (joy & DX.PAD_INPUT_1) == DX.PAD_INPUT_1;
			if (!inz) inz = (joy & DX.PAD_INPUT_2) == DX.PAD_INPUT_2;


			inz1 = false;
			if (inz)
				inz1 = inz;

			if (inz && binz)
				inz1 = false;
			camera = c;
			map = m;
		}

		public void Update(bool binz, Point c, Size m)
		{
			if (DX.GetWindowActiveFlag() == 0)
			{
				inup = false;
				indown = false;
				inleft = false;
				inright = false;
				inz = false;
				camera = c;
				inz1 = inz;
				map = m;
				return;
			}
			inup = DX.CheckHitKey(DX.KEY_INPUT_UP) == 1;
			indown = DX.CheckHitKey(DX.KEY_INPUT_DOWN) == 1;
			inleft = DX.CheckHitKey(DX.KEY_INPUT_LEFT) == 1;
			inright = DX.CheckHitKey(DX.KEY_INPUT_RIGHT) == 1;
			inlshift = DX.CheckHitKey(DX.KEY_INPUT_LSHIFT) == 1;
			inz = DX.CheckHitKey(DX.KEY_INPUT_Z) == 1;
			int joy = DX.GetJoypadInputState(DX.DX_INPUT_PAD1);
			if (!inup) inup = (joy & DX.PAD_INPUT_UP) == DX.PAD_INPUT_UP;
			if (!indown) indown = (joy & DX.PAD_INPUT_DOWN) == DX.PAD_INPUT_DOWN;
			if (!inleft) inleft = (joy & DX.PAD_INPUT_LEFT) == DX.PAD_INPUT_LEFT;
			if (!inright) inright = (joy & DX.PAD_INPUT_RIGHT) == DX.PAD_INPUT_RIGHT;
			if (!inlshift) inlshift = (joy & DX.PAD_INPUT_1) == DX.PAD_INPUT_1;
			if (!inz) inz = (joy & DX.PAD_INPUT_2) == DX.PAD_INPUT_2;

			if (inz)
				inz1 = inz;

			if (inz && binz)
				inz1 = false;
			camera = c;
			map = m;
		}

		public bool AnyKeyPressed => inup || indown || inleft || inright || inz || inlshift;

	}



}
