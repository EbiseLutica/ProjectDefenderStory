using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DxLibDLL;
using System.Drawing;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using MapEditor;
using MusicSheetMidiSequencer;

namespace DefenderStory
{
	class Program
	{
		static void Main(string[] args)
		{

			DX.SetUseGraphAlphaChannel(1);
			DX.ChangeWindowMode(1);
			string mptname = "mpt1";
			//DX.SetWaitVSyncFlag(1);
			DX.SetFontSize(14);
			DX.SetFontThickness(1);
			DX.SetWindowText("Defender Story");
			
			//DX.SetAlwaysRunFlag(1);

			Size scrSize = new Size(320,240);
			
			if (DX.DxLib_Init() == -1)
			{
				ShowError("DirectX の初期化に失敗しました。");
				return;
			}
			

			
			if (DX.SetGraphMode(scrSize.Width, scrSize.Height, 32, 60) == -1)
			{
				ShowError("サイズの変更に失敗しました。");
				return;
			}
			

			if (DX.SetDrawScreen(DX.DX_SCREEN_BACK) == -1)
			{
				ShowError("裏画面の指定に失敗しました。");
				return;
			}

			//int f = 0;
			//int fps = 1;

			//int nextFps = 60;
			int bsec = DateTime.Now.Second;
			int bmsec = DateTime.Now.Millisecond;

			int level = 1;
			int area;

			var data = GetJsonData<LevelData>("Resources\\Levels\\Level " + level + "\\lvldat.json");
			area = data.FirstArea;

			var areainfo = GetJsonData<AreaInfo>("Resources\\Levels\\Level " + level + "\\Area " + area + "\\area.json");

			mptname = areainfo.Mpt;

			//int hndl_bigplayer_walkleft = DX.CreateGraphFromRectSoftImage(hndl_playerchip, 0, 0, 16, 32);

			int[] hndl_bigplayer_datas = new int[72];

			if (DX.LoadDivGraph("Resources\\Graphics\\player_chip.png", 72, 18, 4, 16, 32, out hndl_bigplayer_datas[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			int[] hndl_commonMob_datas = new int[144];
			
			if (DX.LoadDivGraph("Resources\\Graphics\\commonMob.png", 144, 16, 4, 16, 16, out hndl_commonMob_datas[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			int[] hndl_modokee_datas = new int[8];

			if (DX.LoadDivGraph("Resources\\Graphics\\spModokee.png", 8, 8, 1, 32, 16, out hndl_modokee_datas[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			int[] hndl_modokee_cave_datas = new int[8];

			if (DX.LoadDivGraph("Resources\\Graphics\\spCaveModokee.png", 8, 8, 1, 32, 16, out hndl_modokee_cave_datas[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			int[] hndl_daemon_datas = new int[14];

			if (DX.LoadDivGraph("Resources\\Graphics\\spdaemon.png", 14, 14, 1, 16, 16, out hndl_daemon_datas[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			int[] hndl_archer_datas = new int[14];

			if (DX.LoadDivGraph("Resources\\Graphics\\sparcher.png", 8, 8, 1, 32, 32, out hndl_archer_datas[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			int[] hndl_weapon_datas = new int[14];

			if (DX.LoadDivGraph("Resources\\Graphics\\spweapon.png", 14, 14, 1, 16, 16, out hndl_weapon_datas[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			int[] hndl_dwarf_datas = new int[5];

			if (DX.LoadDivGraph("Resources\\Graphics\\spdwarf.png", 5, 5, 1, 16, 32, out hndl_dwarf_datas[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			int[] hndl_woody_datas = new int[4];

			if (DX.LoadDivGraph("Resources\\Graphics\\spwoody.png", 4, 4, 1, 64, 64, out hndl_woody_datas[0]) == -1)
			{
				throw new Exception("キャラの読み込みに失敗しました。");
			}

			init:

			int[] hndl_mpt = new int[64];
			DX.LoadDivGraph("Resources\\Graphics\\" + mptname + ".png", 64, 16, 4, 16, 16, out hndl_mpt[0]);

			int hndl_mptsoft = DX.LoadSoftImage("Resources\\Graphics\\" + mptname + "_hj.png");
			


			Object[] mptobjects = new Object[64];

			int r, g, b, a;
			Color[] hits = new Color[5];
			for (int i = 0; i < 5; i++)
			{
				DX.GetPixelSoftImage(hndl_mptsoft, i, 64, out r, out g, out b, out a);
				hits[i] = Color.FromArgb(r, g, b, a);
			}



			List<Color> hitlist = hits.ToList();

			for (int iy = 0; iy < 4; iy++)
			{
				for (int ix = 0; ix < 16; ix++)
				{
					byte[,] mask = new byte[16, 16];
					for (int y = 0; y < 16; y++)
						for (int x = 0; x < 16; x++)
						{
							DX.GetPixelSoftImage(hndl_mptsoft, x + ix * 16, y + iy * 16, out r, out g, out b, out a);
							mask[x, y] = (byte)hitlist.IndexOf(Color.FromArgb(r, g, b, a));
						}
					mptobjects[iy * 16 + ix] = new Object(hndl_mpt[iy * 16 + ix], mask);
				}
			}
			

			DX.DeleteSoftImage(hndl_mptsoft);

			ObjectSelectForm osf = new ObjectSelectForm("Resources\\Graphics\\" + mptname + ".png");
			osf.Show();

			Size map = new Size(20, 15);
			byte[, ,] chips;
			
			
			top:
			
			int f = 0, fps = 0;
			int binz = 0;
			Point camera = new Point(0,0);
			int btime = DX.GetNowCount();
			SpriteCollection spcolle = new SpriteCollection();

			
			Player me = new Player(new Point(32, 32), new Size(16, 32), true, 8, 0, mptobjects, spcolle, hndl_bigplayer_datas);


			//Player me2 = new Player(new Point(64, 32), new Size(16, 32), true, 8, 0, mptobjects, spcolle, hndl_bigplayer_datas);

			//Player me3 = new Player(new Point(96, 32), new Size(16, 32), true, 8, 0, mptobjects, spcolle, hndl_bigplayer_datas);

			//Player me4 = new Player(new Point(128, 32), new Size(16, 32), true, 8, 0, mptobjects, spcolle, hndl_bigplayer_datas);

			//Bunyo hoge = new Bunyo(new Point(256,2), new Size(16,16), true, 8, 0, mptobjects, spcolle, hndl_commonMob_datas);

			//Modokee_Ground modokee = new Modokee_Ground(new Point(100, 192), new Size(32, 16), true, 8, 0, mptobjects, spcolle, hndl_modokee_datas);

			//Modokee_Ground modocave = new Modokee_Cave(new Point(200, 0), new Size(32, 16), true, 8, 0, mptobjects, spcolle, hndl_modokee_cave_datas);

			//Daemon daemon = new Daemon(new Point(200, 192), new Size(16, 16), true, 8, 0, mptobjects, spcolle, hndl_daemon_datas);


			//bool xflg = false;

			spcolle.Add(me, true);
			//spcolle.Add(new Archer(new Point(200, 60), new Size(32, 32), mptobjects, spcolle, hndl_archer_datas, hndl_weapon_datas));
			//spcolle.Add(new Solider(new Point(200, 60), new Size(16,32), true, 8, 0, mptobjects, spcolle, hndl_dwarf_datas));
			//spcolle.Add(new Solider(new Point(232, 60), new Size(16, 32), true, 8, 0, mptobjects, spcolle, hndl_dwarf_datas));
			//spcolle.Add(new Solider(new Point(264, 60), new Size(16, 32), true, 8, 0, mptobjects, spcolle, hndl_dwarf_datas));
			//spcolle.Add(new Woody(new Point(256, 128), new Size(64,64), mptobjects, spcolle, hndl_woody_datas, hndl_weapon_datas));

			//spcolle.Add(new Archer(new Point(264, 60), new Size(32, 16), mptobjects, spcolle, hndl_archer_datas, hndl_weapon_datas));
			//spcolle.Add(hoge);
			//spcolle.Add(modokee);
			//spcolle.Add(modocave);
			//spcolle.Add(daemon);

			//spcolle.Add(me2);
			//spcolle.Add(me3);
			//spcolle.Add(me4);

			//spcolle.Add(hoge);

			//for (int i = 0; i < 240; i++)
			//{
				DX.ClearDrawScreen();

				DX.DrawString(136, 96, "テスト", 0xffffff);

				if (DX.ScreenFlip() == -1)
				{
					DX.DxLib_End();
					return;
				}
				//break;		//if you want to skip splash screen, comment out this line.
			//}

			MapUtility.LoadMap(out chips, "Resources\\Levels\\Level " + level + "\\Area " + area + "\\map.citmap");
			map = new Size(chips.GetLength(0), chips.GetLength(1));

			int hndl_bg = DX.LoadGraph("Resources\\Graphics\\" + areainfo.BG);

			Sequencer seq = new Sequencer();
			seq.Load("Resources\\Music\\" + areainfo.Music);
			seq.Play();


			while (true)
			{
				DX.ClearDrawScreen();	//消し去る

				//DX.DrawBox(0, 0, 640,480, 0X3F9FFF, 1);

				//DX.DrawBox(0, jimeny, 320, 240, Color.SaddleBrown.ToArgb(), 1);

				if (osf.reloadRequest)
				{
					mptname = osf.Path;
					osf.reloadRequest = false;
					goto init;
				}

				//
				//2014/12/19 あとで死んだ時に音を止めるようにする。
				//

				States ks = new States(binz, camera, map);
				int mousex = -1, mousey = -1;
				if (DX.GetMouseInput() == DX.MOUSE_INPUT_1)
					DX.GetMousePoint(out mousex, out mousey);
				

				if (ks.inz == 1 && binz == 1)
					ks.inz1 = 0;

				int inesc = DX.CheckHitKey(DX.KEY_INPUT_ESCAPE);

				if (mousex != -1 && mousey != -1)
				{
					try
					{
						chips[(mousex - camera.X) / 16, (mousey - camera.Y) / 16, (int)osf.sf] = (byte)osf.chipno;
					}
					catch (Exception)
					{

					}
				}



				if (inesc == 1)
				{
					DX.DxLib_End();
					return;
				}

				if (spcolle.MainSprite.isDead)
					goto top;

				if (spcolle.MainSprite.killed && seq.IsPlaying)
				{
					seq.Stop();
				}

				if (!spcolle.MainSprite.killed)
				{
					if (spcolle.MainSprite.nowX + ks.camera.X > scrSize.Width / 2 && spcolle.MainSprite.spdx > 0 && ks.camera.X > -ks.map.Width * 16 + scrSize.Width)
					{
						//ks.camera.Offset(-(int)spcolle.MainSprite.spdx, 0);
						ks.camera = new Point(-(int)spcolle.MainSprite.nowX + scrSize.Width / 2, ks.camera.Y);
					}

					if (ks.map.Width * 16 - spcolle.MainSprite.nowX > scrSize.Width / 2 && spcolle.MainSprite.spdx < 0 && ks.camera.X < 0)
					{
						//ks.camera.Offset(-(int)spcolle.MainSprite.spdx, 0);
						ks.camera = new Point(-(int)spcolle.MainSprite.nowX + scrSize.Width / 2, ks.camera.Y);
					}

					if (spcolle.MainSprite.nowY + ks.camera.Y > scrSize.Height / 2 && spcolle.MainSprite.spdy > 0 && ks.camera.Y > -ks.map.Height * 16 + scrSize.Height)
					{
						ks.camera.Offset(0, -(int)spcolle.MainSprite.spdy);
						//ks.camera = new Point(ks.camera.X , -(int)spcolle.MainSprite.nowY + scrSize.Height / 2);
					}

					if (ks.map.Height * 16 - spcolle.MainSprite.nowY > scrSize.Height / 2 && spcolle.MainSprite.spdy < 0 && ks.camera.Y < 0)
					{
						ks.camera.Offset(0, -(int)spcolle.MainSprite.spdy);
						//ks.camera.Offset(ks.camera.X, -(int)spcolle.MainSprite.nowY + scrSize.Height / 2);
						
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

				int bgx = (int)(-((-ks.camera.X  * (areainfo.ScrollSpeed / 10.0)) % 320));

				DX.DrawGraph(bgx, 0, hndl_bg, 0);
				DX.DrawGraph(bgx + 320, 0, hndl_bg, 0);

				for (int y = 0; y < map.Height * 16; y += 16)
					for (int x = 0; x < map.Width * 16; x += 16)
						DX.DrawGraph(x + camera.X, y + camera.Y, hndl_mpt[chips[x / 16, y / 16, 1]], 1);

				spcolle.Draw(ref ks, chips);

				for (int y = 0; y < map.Height * 16; y += 16)
					for (int x = 0; x < map.Width * 16; x += 16)
						DX.DrawGraph(x + camera.X, y + camera.Y, hndl_mpt[chips[x / 16, y / 16, 0]], 1);

				f++;
				if (bsec != DateTime.Now.Second)
				{
					fps = f;
					f = 0;
					bsec = DateTime.Now.Second;
				}


				string aho = "";
				foreach (Sprite sp in spcolle)
				{
					aho += (sp.killed ? "死" : "生");
				}
				DX.DrawString(0, 0, string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}", aho, bgx , camera.X / 16, map.Width, camera.Y / 16, map.Height, fps), 0xffffff);

				seq.PlayLoop();

				if (DX.ScreenFlip() == -1)
				{
					//DX.StopSoundMem(nowHandle);
					//DX.DeleteSoundMem(nowHandle);
					DX.DxLib_End();
					return;
				}

				//Console.Write(output + "\t");



				me.bspdx = me.spdx;
				binz = ks.inz;
				camera = ks.camera;
				map = ks.map;

			}



		}

		public static DialogResult ShowError(string message)
		{
			DX.DxLib_End();
			return MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1);

		}

		public static T GetJsonData<T>(string json)
		{
			string jsonstring = File.ReadAllText(json);
			var s = new DataContractJsonSerializer(typeof(T));
			var jsonBytes = Encoding.Unicode.GetBytes(jsonstring);
			var sr = new MemoryStream(jsonBytes);
			var obj = (T)s.ReadObject(sr);

			return obj;
		}

	}

	[DataContract]
	public class LevelData
	{
		[DataMember]
		public int FirstArea { get; set; }
	}

	[DataContract]
	public class AreaInfo
	{
		[DataMember]
		public string Mpt { get; set; }
		[DataMember]
		public string Music { get; set; }
		[DataMember]
		public string BG { get; set; }
		[DataMember]
		public int ScrollSpeed { get; set; }
	}

	//スプライトのスペル間違えてたので修正

	public struct States
	{
		public int inup, indown, inleft, inright, inlshift, inw, ina, ins, ind, inz, inz1;
		public Point camera;
		public Size map;
		public States(int binz, Point c, Size m)
		{
			inup = DX.CheckHitKey(DX.KEY_INPUT_UP);
			indown = DX.CheckHitKey(DX.KEY_INPUT_DOWN);
			inleft = DX.CheckHitKey(DX.KEY_INPUT_LEFT);
			inright = DX.CheckHitKey(DX.KEY_INPUT_RIGHT);
			inlshift = DX.CheckHitKey(DX.KEY_INPUT_LSHIFT);
			inw = DX.CheckHitKey(DX.KEY_INPUT_W);
			ina = DX.CheckHitKey(DX.KEY_INPUT_A);
			ins = DX.CheckHitKey(DX.KEY_INPUT_S);
			ind = DX.CheckHitKey(DX.KEY_INPUT_D);

			inz = DX.CheckHitKey(DX.KEY_INPUT_Z);
			inz1 = inz;

			if (inz == 1 && binz == 1)
				inz1 = 0;
			camera = c;
			map = m;
		}


	}

	public class Object
	{
		public int ImageHandle { get; set; }
		/// <summary>
		/// オブジェクトの当たり判定をビットマップで指定します。
		/// ～記法～
		/// 0...当たらない
		/// 1...当たる
		/// 2...当たるとダメージ
		/// 3...当たると即死
		/// 4...当たると水中
		/// </summary>
		public byte[,] HitMask { get; set; }

		public ObjectHitFlag CheckHit(int x, int y)
		{
			return (ObjectHitFlag)HitMask[x, y];
		}

		public Object(int handle, byte[,] mask)
		{
			ImageHandle = handle;
			HitMask = mask;
		}


	}



	public enum ObjectHitFlag
	{
		NotHit, Hit, Damage, Death, InWater
	}

	public class SpriteCollection : ICollection<Sprite>
	{

		public List<Sprite> Items { get; private set; }


		public SpriteCollection()
		{
			Items = new List<Sprite>();
		}

		public void Add(Sprite item)
		{
			Add(item, false);
		}

		public void Add(Sprite item, bool isMain)
		{
			if (Contains(item) == false)
			{
				Items.Add(item);
				if (isMain)
					MainSprite = item;
			}

		}

		public Sprite this[int i]
		{
			get
			{
				return Items[i];
			}
			set
			{
				Items[i] = value;
			}
		}


		public Sprite MainSprite { get; private set; }

		public void Draw(ref States ks, byte[, ,] chips)
		{
			//foreach (Sprite item in this.Items)
			for (int i = 0; i < Count; i++)
			{
				Sprite item = this[i];
				if (item.isDead)
				{
					this.Remove(item);
					//continue;
				}
				if (i >= Count)
					break;
				item = this[i];
				item.Draw(ks.camera.X, ks.camera.Y, ref ks, chips);
			}
		}



		public bool IsReadOnly { get { return false; } }
		public void Clear() { Items.Clear(); }
		public int Count { get { return Items.Count; } }
		public bool Contains(Sprite item) { return Items.Contains(item); }
		public void CopyTo(Sprite[] array, int arrayIndex) { Items.CopyTo(array, arrayIndex); }
		public bool Remove(Sprite item) { return Items.Remove(item); }
		public IEnumerator<Sprite> GetEnumerator()
		{
			return Items.GetEnumerator();

		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Items.GetEnumerator();
		}



	}


	/// <summary>
	/// スプライトの基本機能を提供します。
	/// </summary>
	public abstract class Sprite
	{
		/// <summary>
		/// スプライトがいるマップ座標。
		/// </summary>
		public Point Point { get; set; }
		/// <summary>
		/// スプライトのサイズ。
		/// </summary>
		public Size Size { get; set; }
		/// <summary>
		/// 画像のハンドル。
		/// </summary>
		public int[] ImageHandle { get; set; }
		/// <summary>
		/// アニメーションを利用するかどうかを取得または設定します。
		/// </summary>
		public bool UseAnime { get; set; }
		/// <summary>
		/// ループ回数を取得または設定します。
		/// </summary>
		public int LoopTimes { get; set; }
		/// <summary>
		/// アニメーションのスピードを取得または設定します。
		/// </summary>
		public int AnimeSpeed { get; set; }
		public bool killed = false;
		public float spdAddition = 0.2f, spddivition = 0.8f, spdx = 0, spdy = 0, spdlimit = 2, nowX = 32, nowY = 32, bspdx = 0;
		public bool isRight = false;
		public bool isFall = false;

		public bool isDead = false;

		public abstract SpriteGroup MyGroup { get; }

		/// <summary>
		/// この Sprite が属する SpriteCollection を取得します。
		/// </summary>
		public SpriteCollection Parent { get; private set; }

		public Object[] mptobjects = null;

		/// <summary>
		/// スプライトのイベント ID を取得または設定します。この ID は、他のスプライトが一意のイベントに属するすべてのスプライトを特定するために使用します。
		/// </summary>
		public int EventID { get; set; }

		/// <summary>
		/// 殺された時の画像ハンドルを取得または設定します。
		/// </summary>
		public int[] KilledImageHandle { get; set; }

		/// <summary>
		/// 殺されたあとの処理を行います。このメソッドは、スプライトが殺された時に、継承したクラスでオーバーライドされる Draw() メソッドで、内部的に実行することを推奨します。
		/// </summary>
		/// <param name="tick">殺されてからの Tick 。</param>
		public abstract void Killing(int tick);

		public int nowImageNumber = 0;

		/// <summary>
		/// 殺されたあと処理をする最大のTickを取得します。
		/// </summary>
		public abstract int killedMaxTick { get; }

		public int looptimes = 0;

		int bms = DX.GetNowCount();

		/// <summary>
		/// 必要な値を指定して、 Sprite クラスの新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="p">位置。</param>
		/// <param name="s">サイズ。</param>
		/// <param name="handle">使用する画像ハンドルの配列。</param>
		/// <param name="useanime">アニメーションするかどうか。</param>
		/// <param name="speed">アニメーションの速度。</param>
		/// <param name="loop">ループ回数。 0 を指定すると無限ループします。</param>
		/// <param name="objs">マップチップの配列。</param>
		/// <param name="par">属するスプライト一覧。</param>
		public Sprite(Point p, Size s, int[] handle, bool useanime, int speed, int loop, Object[] objs, SpriteCollection par)
		{
			Point = p;
			Size = s;
			ImageHandle = handle;
			UseAnime = useanime;
			AnimeSpeed = speed;
			LoopTimes = loop;
			mptobjects = objs;
			nowX = p.X;
			nowY = p.Y;
			this.Parent = par;
		}

		/// <summary>
		/// 指定されたカメラ座標を考慮して描画します。
		/// </summary>
		/// <param name="x">X 座標。</param>
		/// <param name="y">Y 座標。</param>
		/// <param name="ks">現在の状況。</param>
		/// <param name="chips">マップデータ。</param>
		public virtual void Draw(int x, int y, ref States ks, byte[, ,] chips)
		{
			if (UseAnime && DX.GetNowCount() - bms >= AnimeSpeed * 15)
			{
				nowImageNumber++;
				if (nowImageNumber >= ImageHandle.Length)
				{

					looptimes++;
					if (looptimes >= LoopTimes && LoopTimes != 0)
						UseAnime = false;
					else
						nowImageNumber = 0;
				}
				bms = DX.GetNowCount();
			}
			DX.DrawGraph(Point.X + x, Point.Y + y, ImageHandle[nowImageNumber], 1);
		}

		/// <summary>
		/// スプライトを描画します。
		/// </summary>
		/// <param name="ks">現在の状況。</param>
		/// <param name="chips">マップデータ。</param>
		public void Draw(ref States ks, byte[, ,] chips)
		{
			Draw(0, 0, ref ks, chips);
		}

		/// <summary>
		/// 矩形同士の当たり判定を計算します。
		/// </summary>
		/// <param name="rect1">矩形。</param>
		/// <param name="rect2">矩形。</param>
		/// <returns>当たっているかどうか。</returns>
		public static bool CheckHitJudge(Rectangle rect1, Rectangle rect2)
		{
			return (rect1.X < rect2.X + rect2.Width) && (rect2.X < rect1.X + rect1.Width) &&
				   (rect1.Y < rect2.Y + rect2.Height) && (rect2.Y < rect1.Y + rect1.Height);
		}


	}

	/// <summary>
	/// 主人公を表します。
	/// </summary>
	public class Player : Sprite
	{

		public bool jumping = false;
		public bool jumped = false;
		public int jumpcnt = 0;
		public double[] Sin = null;
		ObjectHitFlag bohf7 = ObjectHitFlag.NotHit, bohf8 = ObjectHitFlag.NotHit;
		//bool isRight = false;
		//bool killed = false;
		int killedtick = 0;
		int bubbleNumber = 16;

		static int[] biglwalk = { 0, 1, 2, 3 };
		static int[] bigljump = { 4 };
		static int[] bigrwalk = { 6, 7, 8, 9 };
		static int[] bigrjump = { 10 };
		static int[] bigdead = { 5 };
		static int[] biglcrch = { 11 };
		static int[] bigrcrch = { 12 };
		static int[] biglprat = { 13 };
		static int[] bigrprat = { 14 };
		static int[] biglpratw = { 15 };
		static int[] bigrpratw = { 16 };
		static int[] biglswimw = { 18, 19, 20, 21 };
		static int[] bigrswimw = { 22, 23, 24, 25 };
		static int[] biglwalkw = { 26, 27, 28, 29 };
		static int[] bigrwalkw = { 30, 31, 32, 33 };
		static int[] biglcrchw = { 34 };
		static int[] bigrcrchw = { 35 };

		static int[] biglwalkt = { 36, 37, 38, 39 };
		static int[] bigljumpt = { 40 };
		static int[] bigrwalkt = { 41, 42, 43, 44 };
		static int[] bigrjumpt = { 45 };
		static int[] bigldeadt = { 46 };
		static int[] biglcrcht = { 47 };
		static int[] bigrcrcht = { 48 };
		static int[] biglpratt = { 49 };
		static int[] bigrpratt = { 50 };
		static int[] biglpratwt = { 51 };
		static int[] bigrpratwt = { 52 };
		static int[] biglswimwt = { 54, 55, 56, 57 };
		static int[] bigrswimwt = { 58, 59, 60, 61 };
		static int[] biglwalkwt = { 62, 63, 64, 65 };
		static int[] bigrwalkwt = { 66, 67, 68, 69 };
		static int[] biglcrchwt = { 70 };
		static int[] bigrcrchwt = { 71 };

		int[] datas;

		public override SpriteGroup MyGroup
		{
			get
			{
				return SpriteGroup.Defender;
			}
		}

		/// <summary>
		/// 必要な値を指定して、 Player クラスの新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="p">位置。</param>
		/// <param name="s">サイズ。</param>
		/// <param name="handle">使用する画像ハンドルの配列。</param>
		/// <param name="useanime">アニメーションするかどうか。</param>
		/// <param name="speed">アニメーションの速度。</param>
		/// <param name="loop">ループ回数。 0 を指定すると無限ループします。</param>
		/// <param name="objs">マップチップの配列。</param>
		/// <param name="par">属するスプライト一覧。</param>
		public Player(Point p, Size s, bool useanime, int speed, int loop, Object[] objs, SpriteCollection par, int[] datas)
			: base(p, s, new[] { 0 }, useanime, speed, loop, objs, par)
		{

			this.datas = datas;
			this.KilledImageHandle = GetImage(bigdead);
			Sin = new double[360];
			for (int i = 0; i < 360; i++)
				Sin[i] = Math.Sin(i / 180.0 * Math.PI) * 64;
		}

		/// <summary>
		/// 殺されたあとの処理時間を取得します。
		/// </summary>
		public override int killedMaxTick
		{
			get { return 240; }
		}

		int killedy = -16;

		/// <summary>
		/// 殺されたあとの処理をします。
		/// </summary>
		/// <param name="tick">現在の Tick 。</param>
		public override void Killing(int tick)
		{
			if (this.ImageHandle != this.KilledImageHandle)
				this.ImageHandle = this.KilledImageHandle;
			this.nowImageNumber = 0;
			this.UseAnime = false;
			if (killedy == -16)
				killedy = Point.Y;
			if (tick < 180)
				this.Point = new Point(Point.X, (int)(killedy - Sin[tick % 360]));
			else
				this.Point = new Point(Point.X, Point.Y + 1);
		}

		/// <summary>
		/// スプライトの制御を行います。
		/// </summary>
		/// <param name="x">X 座標。</param>
		/// <param name="y">Y 座標。</param>
		/// <param name="ks">現在の状況。</param>
		/// <param name="chips">マップデータ。</param>
		public override void Draw(int x, int y, ref States ks, byte[, ,] chips)
		{


			if (ks.inleft == 1)
			{
				isRight = false;
				if (spdx > 0)
				{
					spdx *= spddivition;
					if (spdx < 0.4f && spdx > -0.4f)
						spdx = 0;
				}
				else
				{

					spdx -= spdAddition;
					if (spdx < -spdlimit)
						spdx = -spdlimit;
				}
			}
			else if (ks.inright == 1)
			{
				isRight = true;
				if (spdx < 0)
				{
					spdx *= spddivition;
					if (spdx < 0.4f && spdx > -0.4f)
						spdx = 0;
				}
				else
				{
					spdx += spdAddition;
					if (spdx > spdlimit)
						spdx = spdlimit;
				}
			}
			else
			{
				spdx *= spddivition;
				if (spdx < 0.4f && spdx > -0.4f)
					spdx = 0;
			}

			nowX += spdx; nowY += spdy;





			if (nowX < 0)
			{
				spdx = 0;
				nowX = 0;
			}

			if (nowX > ks.map.Width * 16 - 16)
			{
				spdx = 0;
				nowX = ks.map.Width * 16 - 16;
			}

			if (nowY < 0)
			{
				spdy = 0;
				nowY = 0;
			}
			if (nowY > ks.map.Height * 16)
			{
				killed = true;
				isFall = true;
			}



			if (!killed)
			{
				if (ks.inz == 1 && !jumped)
				{
					if (bohf7 == ObjectHitFlag.InWater && bohf8 == ObjectHitFlag.InWater)
					{
						spdy = -2.5f;

					}
					else
					{
						jumped = true;

						spdy = -3.5f - Math.Abs(spdx) / 10;
					}
					jumping = true;
				}

				if (ks.indown == 1)
				{
					spdy += 0.2f;
				}



				if (ks.inlshift == 1)
				{
					spdAddition = 0.4f;
					spddivition = 0.85f;
					spdlimit = 4;
				}
				else
				{
					spdAddition = 0.2f;
					spddivition = 0.7f;
					spdlimit = 2;
				}

				spdx = (float)Math.Round(spdx, 3);
				//nowX = (int)nowX;

				Point judge1 = new Point((int)nowX + 2, (int)nowY + 32);	//足判定1

				Point judge2 = new Point((int)nowX + 12, (int)nowY + 32);	//足判定2

				Point judge3 = new Point((int)nowX + 1, (int)nowY + 24);	//左障害物判定

				Point judge4 = new Point((int)nowX + 13, (int)nowY + 24);	//右障害物判定

				Point judge5 = new Point((int)nowX + 1, (int)nowY + 12);	//左障害物判定2

				Point judge6 = new Point((int)nowX + 13, (int)nowY + 12);	//右障害物判定2

				Point judge7 = new Point((int)nowX + 2, (int)nowY - 1);		//頭判定1

				Point judge8 = new Point((int)nowX + 12, (int)nowY - 1);	//頭判定2

				Point judge9 = new Point((int)nowX + 2, (int)nowY + 36);	//足判定3

				Point judge10 = new Point((int)nowX + 12, (int)nowY + 36);	//足判定4



				ObjectHitFlag ohf1 = ObjectHitFlag.NotHit,
							  ohf2 = ObjectHitFlag.NotHit,
							  ohf3 = ObjectHitFlag.NotHit,
							  ohf4 = ObjectHitFlag.NotHit,
							  ohf5 = ObjectHitFlag.NotHit,
							  ohf6 = ObjectHitFlag.NotHit,
							  ohf7 = ObjectHitFlag.NotHit,
							  ohf8 = ObjectHitFlag.NotHit,
							  ohf9 = ObjectHitFlag.NotHit,
							 ohf10 = ObjectHitFlag.NotHit;
						

				//もし判定点が床部分なら、上を目指す。
				try
				{
					ohf1 = mptobjects[chips[judge1.X / 16, judge1.Y / 16, 0]].CheckHit(judge1.X % 16, judge1.Y % 16);

					ohf2 = mptobjects[chips[judge2.X / 16, judge2.Y / 16, 0]].CheckHit(judge2.X % 16, judge2.Y % 16);

					ohf3 = mptobjects[chips[judge3.X / 16, judge3.Y / 16, 0]].CheckHit(judge3.X % 16, judge3.Y % 16);

					ohf4 = mptobjects[chips[judge4.X / 16, judge4.Y / 16, 0]].CheckHit(judge4.X % 16, judge4.Y % 16);

					ohf5 = mptobjects[chips[judge5.X / 16, judge5.Y / 16, 0]].CheckHit(judge5.X % 16, judge5.Y % 16);

					ohf6 = mptobjects[chips[judge6.X / 16, judge6.Y / 16, 0]].CheckHit(judge6.X % 16, judge6.Y % 16);

					ohf7 = mptobjects[chips[judge7.X / 16, judge7.Y / 16, 0]].CheckHit(judge7.X % 16, judge7.Y % 16);

					ohf8 = mptobjects[chips[judge8.X / 16, judge8.Y / 16, 0]].CheckHit(judge8.X % 16, judge8.Y % 16);

					ohf9 = mptobjects[chips[judge9.X / 16, judge9.Y / 16, 0]].CheckHit(judge9.X % 16, judge9.Y % 16);
					
					ohf10 = mptobjects[chips[judge10.X / 16, judge10.Y / 16, 0]].CheckHit(judge10.X % 16, judge10.Y % 16);

				}
				catch (Exception)
				{
				}

				if (ohf7 == ObjectHitFlag.InWater && ohf8 == ObjectHitFlag.InWater)
				{
					for (int bi = 0; bi < bubbleNumber / 2; bi++)
						DX.DrawCircle(this.Point.X + DX.GetRand(this.Size.Width), this.Point.Y + this.Size.Height - DX.GetRand(4), DX.GetRand(3), DX.GetColor(255, 255, 255), 0);
					if (bubbleNumber > -1 && DX.GetNowCount() % 2 == 0)
						bubbleNumber--;
					jumping = false;
					jumped = false;
				}
				else
				{
					if (bohf7 != ObjectHitFlag.InWater && bohf8 != ObjectHitFlag.InWater)
						bubbleNumber = 16;

				}


				if (!killed)
				{
					if ((ohf1 == ObjectHitFlag.NotHit || ohf1 == ObjectHitFlag.InWater) && (ohf2 == ObjectHitFlag.NotHit || ohf2 == ObjectHitFlag.InWater))
					{
						spdy += 0.1f;
					}

					if (ohf9 == ObjectHitFlag.NotHit && ohf10 == ObjectHitFlag.NotHit)
					{
						jumped = true;
					}

					if (ohf1 == ObjectHitFlag.Hit || ohf2 == ObjectHitFlag.Hit)
					{
						nowY = nowY - 1;
						spdy = 0;
						jumped = false;
						jumping = false;
					}

					if (ohf3 == ObjectHitFlag.Hit || ohf5 == ObjectHitFlag.Hit)
					{
						//spdx = 0;
						nowX = (int)(nowX + 2);
						//ks.camera.Offset(-(int)spdx, 0);
						spdx = 0;
						this.nowImageNumber = 0;
					}

					if (ohf4 == ObjectHitFlag.Hit || ohf6 == ObjectHitFlag.Hit)
					{
						//spdx = 0;
						nowX = (int)(nowX - 2);
						//ks.camera.Offset(-(int)spdx, 0);
						spdx = 0;
						this.nowImageNumber = 0;
					}

					if (ohf7 == ObjectHitFlag.Hit || ohf8 == ObjectHitFlag.Hit)
					{
						nowY = nowY + 1;
						spdy = 0;
					}

					Rectangle jibun = new Rectangle(this.Point, this.Size);

					if (ks.inz == 0 && jumping && jumpcnt < 90)
					{
						spdy += 0.1f;
					}

					if (spdx != 0 && !isRight)
					{
						if (jumping)
						{

							if (ohf7 == ObjectHitFlag.InWater && ohf8 == ObjectHitFlag.InWater)
							{
								this.ImageHandle = GetImage(biglswimw);
								this.UseAnime = true;
							}
							else
							{
								this.ImageHandle = GetImage(bigljump);

								this.nowImageNumber = 0;
								this.UseAnime = true;
							}
						}
						else
						{
							if (ohf7 == ObjectHitFlag.InWater && ohf8 == ObjectHitFlag.InWater)
							{
								this.ImageHandle = GetImage(biglwalkw);
								this.UseAnime = true;
							}
							else
							{
								this.ImageHandle = GetImage(biglwalk);
								this.UseAnime = true;
							}
						}
					}

					if (spdx != 0 && isRight)
					{
						if (jumping)
						{
							if (ohf7 == ObjectHitFlag.InWater && ohf8 == ObjectHitFlag.InWater)
							{
								this.ImageHandle = GetImage(bigrswimw);
								this.UseAnime = true;
							}
							else
							{
								this.ImageHandle = GetImage(bigrjump);

								this.nowImageNumber = 0;
								this.UseAnime = false;
							}
						}
						else
						{
							if (ohf7 == ObjectHitFlag.InWater && ohf8 == ObjectHitFlag.InWater)
							{
								this.ImageHandle = GetImage(bigrwalkw);
								this.UseAnime = true;
							}
							else
							{
								this.ImageHandle = GetImage(bigrwalk);

								this.UseAnime = true;
							}
						}
					}

					if (spdx == 0 && !isRight)
					{
						if (jumping)
						{
							if (ohf7 == ObjectHitFlag.InWater && ohf8 == ObjectHitFlag.InWater)
							{
								this.ImageHandle = GetImage(biglswimw);
								this.UseAnime = true;
							}
							else
							{
								this.ImageHandle = GetImage(bigljump);

								this.nowImageNumber = 0;
								this.UseAnime = false;
							}
						}
						else
						{
							if (ohf7 == ObjectHitFlag.InWater && ohf8 == ObjectHitFlag.InWater)
							{
								this.ImageHandle = GetImage(biglwalkw);
								this.UseAnime = false;
							}
							else
							{
								this.ImageHandle = GetImage(biglwalk);

								this.nowImageNumber = 0;
								this.UseAnime = false;
							}
						}
					}
					else if (spdx == 0 && isRight)
					{
						if (jumping)
						{
							if (ohf7 == ObjectHitFlag.InWater && ohf8 == ObjectHitFlag.InWater)
							{
								this.ImageHandle = GetImage(bigrswimw);
								this.UseAnime = true;
							}
							else
							{
								this.ImageHandle = GetImage(bigrjump);

								this.nowImageNumber = 0;
								this.UseAnime = false;
							}
						}
						else
						{
							if (ohf7 == ObjectHitFlag.InWater && ohf8 == ObjectHitFlag.InWater)
							{
								this.ImageHandle = GetImage(bigrwalkw);
								this.UseAnime = false;
							}
							else
							{
								this.ImageHandle = GetImage(bigrwalk);

								this.nowImageNumber = 0;
								this.UseAnime = false;
							}
						}
					}
				}

				bohf7 = ohf7;
				bohf8 = ohf8;




				/*
				if (nowY + me.Size.Height > jimeny)
				{
					nowY = jimeny - me.Size.Height;
					spdy = 0;
					jumped = false;
					jumping = false;
				}
				
				*/
			}

			if (killed)
			{
				Killing(killedtick);
				if (killedMaxTick < killedtick)
				{
					isDead = true;
				}
				killedtick++;
			}


			if (!killed)
				this.Point = new Point((int)nowX, (int)nowY);

			base.Draw(x, y, ref ks, chips);

		}

		int[] GetImage(int[] datas)
		{
			int[] lastdata = new int[datas.Length];
			int i = 0;
			foreach (int data in datas)
			{
				lastdata[i] = this.datas[data];
				i++;
			}
			return lastdata;

		}
	}

	/// <summary>
	/// スプライトが属するグループを指定します。この値は、スプライトの制御で、他のスプライトに敵対するか、味方するか、無視するかなどを判定するために利用するのが目的です。
	/// </summary>
	public enum SpriteGroup
	{
		/// <summary>
		/// 主人公に味方する生き物であることを表します。
		/// </summary>
		Defender,
		/// <summary>
		/// 主人公に敵対する生き物であることを表します。
		/// </summary>
		Monster,
		/// <summary>
		/// 制御のためのスプライト (スクリプト実行など...) であることを表します。
		/// </summary>
		System,
		/// <summary>
		/// ステージの仕掛け (背景や絵画や仕掛けなど...) であることを表します。
		/// </summary>
		Stage,
		/// <summary>
		/// ディフェンダー側の武器(アイスやリーフなど)であることを表します。
		/// </summary>
		DefenderWeapon,
		/// <summary>
		/// 敵側の武器(矢など)であることを表します。
		/// </summary>
		MonsterWeapon

	}

	public class Bunyo : Sprite
	{
		public bool jumping = false;
		public bool jumped = false;
		public int jumpcnt = 0;
		public double[] Sin = null;
		ObjectHitFlag bohf1 = ObjectHitFlag.NotHit, bohf2 = ObjectHitFlag.NotHit;
		//bool isRight = false;
		//bool killed = false;
		int killedtick = 0;
		int bubbleNumber = 16;

		static int[] walkleft = { 0, 1 };
		static int[] walkright = { 4, 5 };
		static int[] dead = { 2 };
		static int[] stepped = { 3 };

		bool crushed = false;

		int[] datas;

		public override SpriteGroup MyGroup
		{
			get
			{
				return SpriteGroup.Monster;
			}
		}

		public Bunyo(Point p, Size s, bool useanime, int speed, int loop, Object[] objs, SpriteCollection par, int[] datas)
			: base(p, s, new[] { 0 }, useanime, speed, loop, objs, par)
		{

			this.datas = datas;
			this.KilledImageHandle = GetImage(dead);
			Sin = new double[360];
			for (int i = 0; i < 360; i++)
				Sin[i] = Math.Sin(i / 180.0 * Math.PI) * 64;
			spdx = -1;
		}



		public override int killedMaxTick
		{
			get { return 240; }
		}

		int killedy = -16;

		public override void Killing(int tick)
		{

			if (this.ImageHandle != this.KilledImageHandle)
				this.ImageHandle = this.KilledImageHandle;
			this.nowImageNumber = 0;
			this.UseAnime = false;
			if (killedy == -16)
				killedy = Point.Y;
			if (tick < 180)
				this.Point = new Point(Point.X, (int)(killedy - Sin[tick % 360]));
			else
				this.Point = new Point(Point.X, Point.Y - (int)(Sin[179] - Sin[178]) * 5);
		}

		public override void Draw(int x, int y, ref States ks, byte[, ,] chips)
		{
			int w, h;
			DX.GetWindowSize(out w, out h);

			if (Math.Abs(Parent.MainSprite.Point.X - this.nowX) > w)
				return;

			if (nowX < 0)
			{
				spdx = 1;
			}

			if (nowX > ks.map.Width * 16 - 16)
			{
				spdx = -1;
				nowX = ks.map.Width * 16 - 16;
			}

			if (nowY < 0)
			{
				spdy = 0;
				nowY = 0;
			}
			if (nowY > ks.map.Height * 16)
			{
				killed = true;
				isFall = true;
			}


			if (!killed)
			{


				Point judge1 = new Point((int)nowX + 2, (int)nowY + 16);	//足判定1	

				Point judge2 = new Point((int)nowX + 12, (int)nowY + 16);	//足判定2

				Point judge3 = new Point((int)nowX + 1, (int)nowY + 12);	//左障害物判定

				Point judge4 = new Point((int)nowX + 13, (int)nowY + 12);	//右障害物判定

				Point judge5 = new Point((int)nowX + 1, (int)nowY + 6);		//左障害物判定2

				Point judge6 = new Point((int)nowX + 13, (int)nowY + 6);	//右障害物判定2

				Point judge7 = new Point((int)nowX + 2, (int)nowY - 1);		//頭判定1

				Point judge8 = new Point((int)nowX + 12, (int)nowY - 1);	//頭判定2



				ObjectHitFlag ohf1 = ObjectHitFlag.NotHit,
							  ohf2 = ObjectHitFlag.NotHit,
							  ohf3 = ObjectHitFlag.NotHit,
							  ohf4 = ObjectHitFlag.NotHit,
							  ohf5 = ObjectHitFlag.NotHit,
							  ohf6 = ObjectHitFlag.NotHit,
							  ohf7 = ObjectHitFlag.NotHit,
							  ohf8 = ObjectHitFlag.NotHit;

				//もし判定点が床部分なら、上を目指す。
				try
				{
					ohf1 = mptobjects[chips[judge1.X / 16, judge1.Y / 16, 0]].CheckHit(judge1.X % 16, judge1.Y % 16);

					ohf2 = mptobjects[chips[judge2.X / 16, judge2.Y / 16, 0]].CheckHit(judge2.X % 16, judge2.Y % 16);

					ohf3 = mptobjects[chips[judge3.X / 16, judge3.Y / 16, 0]].CheckHit(judge3.X % 16, judge3.Y % 16);

					ohf4 = mptobjects[chips[judge4.X / 16, judge4.Y / 16, 0]].CheckHit(judge4.X % 16, judge4.Y % 16);

					ohf5 = mptobjects[chips[judge5.X / 16, judge5.Y / 16, 0]].CheckHit(judge5.X % 16, judge5.Y % 16);

					ohf6 = mptobjects[chips[judge6.X / 16, judge6.Y / 16, 0]].CheckHit(judge6.X % 16, judge6.Y % 16);

					ohf7 = mptobjects[chips[judge7.X / 16, judge7.Y / 16, 0]].CheckHit(judge7.X % 16, judge7.Y % 16);

					ohf8 = mptobjects[chips[judge8.X / 16, judge8.Y / 16, 0]].CheckHit(judge8.X % 16, judge8.Y % 16);


				}
				catch (Exception)
				{
				}

				if (ohf1 == ObjectHitFlag.InWater && ohf2 == ObjectHitFlag.InWater)
				{
					for (int bi = 0; bi < bubbleNumber / 2; bi++)
						DX.DrawCircle(this.Point.X + DX.GetRand(this.Size.Width), this.Point.Y + this.Size.Height - DX.GetRand(4), DX.GetRand(3), DX.GetColor(255, 255, 255), 0);
					if (bubbleNumber > -1 && DX.GetNowCount() % 2 == 0)
						bubbleNumber--;
					jumping = false;
					jumped = false;
				}
				else
				{
					if (bohf1 != ObjectHitFlag.InWater && bohf2 != ObjectHitFlag.InWater)
						bubbleNumber = 16;

				}



				if (!killed)
				{
					if ((ohf1 == ObjectHitFlag.NotHit || ohf1 == ObjectHitFlag.InWater) && (ohf2 == ObjectHitFlag.NotHit || ohf2 == ObjectHitFlag.InWater))
						spdy += 0.1f;



					if (ohf1 == ObjectHitFlag.Hit || ohf2 == ObjectHitFlag.Hit)
					{
						nowY = nowY - 1;
						spdy = 0;
						jumped = false;
						jumping = false;
					}





					if (ohf3 == ObjectHitFlag.Hit || ohf5 == ObjectHitFlag.Hit)
					{
						nowX = (int)(nowX + 1);
						spdx = 0;
						this.nowImageNumber = 0;
						spdx = 1;
					}

					if (ohf4 == ObjectHitFlag.Hit || ohf6 == ObjectHitFlag.Hit)
					{
						nowX = (int)(nowX - 1);
						spdx = 0;
						this.nowImageNumber = 0;
						spdx = -1;
					}

					if (ohf7 == ObjectHitFlag.Hit || ohf8 == ObjectHitFlag.Hit)
					{
						nowY = nowY + 1;
						spdy = 0;
					}

					Rectangle jibun = new Rectangle(this.Point, this.Size);


					//プレイヤーを殺す判定
					foreach (Sprite sp in Parent)
					{
						//プレイヤーの弾
						if (sp.MyGroup == SpriteGroup.DefenderWeapon && CheckHitJudge(jibun, new Rectangle(sp.Point, sp.Size)))
						{
							this.killed = true;
						}
						if (!(sp.MyGroup == SpriteGroup.Defender))
							continue;
						if (sp.killed)
							continue;
						if (CheckHitJudge(new Rectangle(sp.Point.X, sp.Point.Y + sp.Size.Height / 2, sp.Size.Width, sp.Size.Height / 2), new Rectangle(this.Point.X, this.Point.Y + 8, this.Size.Width, this.Size.Height / 2)))
						{
							sp.killed = true;
						}
						else if (CheckHitJudge(new Rectangle(sp.Point.X, sp.Point.Y + sp.Size.Height / 2, sp.Size.Width, sp.Size.Height / 2), new Rectangle(this.Point.X, this.Point.Y, this.Size.Width, this.Size.Height / 2)))
						{
							if (ks.inz == 1)
								sp.spdy -= 5;
							else
								sp.spdy -= 3;
							this.killed = true;
							this.crushed = true;
						}

					}


					if (spdx < 0)		//ひだり
					{
						ImageHandle = GetImage(walkleft);
						UseAnime = true;
					}
					else if (spdx > 0)	//みぎ
					{
						ImageHandle = GetImage(walkright);
						UseAnime = true;
					}


					nowX += spdx;
					nowY += spdy;

					bohf1 = ohf1;
					bohf2 = ohf2;

				}


				/*
				if (nowY + me.Size.Height > jimeny)
				{
					nowY = jimeny - me.Size.Height;
					spdy = 0;
					jumped = false;
					jumping = false;
				}
				
				*/



				if (!killed)
					this.Point = new Point((int)nowX, (int)nowY);


			}

			if (killed)
			{
				Killing(killedtick);
				if (killedMaxTick < killedtick)
				{
					isDead = true;
				}
				killedtick += 5;
			}
			base.Draw(x, y, ref ks, chips);
		}

		int[] GetImage(int[] datas)
		{
			int[] lastdata = new int[datas.Length];
			int i = 0;
			foreach (int data in datas)
			{
				lastdata[i] = this.datas[data];
				i++;
			}
			return lastdata;
		}
	}

	

	public class Daemon : Sprite
	{
		public bool jumping = false;
		public bool jumped = false;
		public int jumpcnt = 0;
		public double[] Sin = null;
		ObjectHitFlag bohf1 = ObjectHitFlag.NotHit, bohf2 = ObjectHitFlag.NotHit;
		//bool isRight = false;
		//bool killed = false;
		int killedtick = 0;
		int bubbleNumber = 16;

		static int[] walkleft = { 0, 1, 2, 3 };
		static int[] walkright = { 7, 8, 9, 10 };
		static int[] dead = { 5, 6 };
		static int[] stepped = { 4 };

		bool crushed = false;

		int[] datas;

		public override SpriteGroup MyGroup
		{
			get
			{
				return SpriteGroup.Monster;
			}
		}

		public Daemon(Point p, Size s, bool useanime, int speed, int loop, Object[] objs, SpriteCollection par, int[] datas)
			: base(p, s, new[] { 0 }, useanime, speed, loop, objs, par)
		{

			this.datas = datas;
			this.KilledImageHandle = GetImage(dead);
			Sin = new double[360];
			for (int i = 0; i < 360; i++)
				Sin[i] = Math.Sin(i / 180.0 * Math.PI) * 64;
			spdx = -1;
		}



		public override int killedMaxTick
		{
			get { return 240; }
		}

		int killedy = -16;

		public override void Killing(int tick)
		{

			if (this.ImageHandle != this.KilledImageHandle)
				this.ImageHandle = this.KilledImageHandle;
			this.nowImageNumber = 0;
			this.UseAnime = false;
			if (killedy == -16)
				killedy = Point.Y;
			if (tick < 180)
				this.Point = new Point(Point.X, (int)(killedy - Sin[tick % 360]));
			else
				this.Point = new Point(Point.X, Point.Y - (int)(Sin[179] - Sin[178]) * 5);
		}

		public override void Draw(int x, int y, ref States ks, byte[, ,] chips)
		{
			int w, h;
			DX.GetWindowSize(out w, out h);

			if (Math.Abs(Parent.MainSprite.Point.X - this.nowX) > w)
				return;

			if (nowX < 0)
			{
				spdx = 1;
			}

			if (nowX > ks.map.Width * 16 - 16)
			{
				spdx = -1;
				nowX = ks.map.Width * 16 - 16;
			}

			if (nowY < 0)
			{
				spdy = 0;
				nowY = 0;
			}
			if (nowY > ks.map.Height * 16)
			{
				killed = true;
				isFall = true;
			}


			if (!killed)
			{


				Point judge1 = new Point((int)nowX + 2, (int)nowY + 16);	//足判定1	

				Point judge2 = new Point((int)nowX + 12, (int)nowY + 16);	//足判定2

				Point judge3 = new Point((int)nowX + 1, (int)nowY + 12);	//左障害物判定

				Point judge4 = new Point((int)nowX + 13, (int)nowY + 12);	//右障害物判定

				Point judge5 = new Point((int)nowX + 1, (int)nowY + 6);		//左障害物判定2

				Point judge6 = new Point((int)nowX + 13, (int)nowY + 6);	//右障害物判定2

				Point judge7 = new Point((int)nowX + 2, (int)nowY - 1);		//頭判定1

				Point judge8 = new Point((int)nowX + 12, (int)nowY - 1);	//頭判定2



				ObjectHitFlag ohf1 = ObjectHitFlag.NotHit,
							  ohf2 = ObjectHitFlag.NotHit,
							  ohf3 = ObjectHitFlag.NotHit,
							  ohf4 = ObjectHitFlag.NotHit,
							  ohf5 = ObjectHitFlag.NotHit,
							  ohf6 = ObjectHitFlag.NotHit,
							  ohf7 = ObjectHitFlag.NotHit,
							  ohf8 = ObjectHitFlag.NotHit;

				//もし判定点が床部分なら、上を目指す。
				try
				{
					ohf1 = mptobjects[chips[judge1.X / 16, judge1.Y / 16, 0]].CheckHit(judge1.X % 16, judge1.Y % 16);

					ohf2 = mptobjects[chips[judge2.X / 16, judge2.Y / 16, 0]].CheckHit(judge2.X % 16, judge2.Y % 16);

					ohf3 = mptobjects[chips[judge3.X / 16, judge3.Y / 16, 0]].CheckHit(judge3.X % 16, judge3.Y % 16);

					ohf4 = mptobjects[chips[judge4.X / 16, judge4.Y / 16, 0]].CheckHit(judge4.X % 16, judge4.Y % 16);

					ohf5 = mptobjects[chips[judge5.X / 16, judge5.Y / 16, 0]].CheckHit(judge5.X % 16, judge5.Y % 16);

					ohf6 = mptobjects[chips[judge6.X / 16, judge6.Y / 16, 0]].CheckHit(judge6.X % 16, judge6.Y % 16);

					ohf7 = mptobjects[chips[judge7.X / 16, judge7.Y / 16, 0]].CheckHit(judge7.X % 16, judge7.Y % 16);

					ohf8 = mptobjects[chips[judge8.X / 16, judge8.Y / 16, 0]].CheckHit(judge8.X % 16, judge8.Y % 16);


				}
				catch (Exception)
				{
				}

				if (ohf1 == ObjectHitFlag.InWater && ohf2 == ObjectHitFlag.InWater)
				{
					for (int bi = 0; bi < bubbleNumber / 2; bi++)
						DX.DrawCircle(this.Point.X + DX.GetRand(this.Size.Width), this.Point.Y + this.Size.Height - DX.GetRand(4), DX.GetRand(3), DX.GetColor(255, 255, 255), 0);
					if (bubbleNumber > -1 && DX.GetNowCount() % 2 == 0)
						bubbleNumber--;
					jumping = false;
					jumped = false;
				}
				else
				{
					if (bohf1 != ObjectHitFlag.InWater && bohf2 != ObjectHitFlag.InWater)
						bubbleNumber = 16;

				}



				if (!killed)
				{
					if ((ohf1 == ObjectHitFlag.NotHit || ohf1 == ObjectHitFlag.InWater) && (ohf2 == ObjectHitFlag.NotHit || ohf2 == ObjectHitFlag.InWater))
						spdy += 0.1f;



					if (ohf1 == ObjectHitFlag.Hit || ohf2 == ObjectHitFlag.Hit)
					{
						nowY = nowY - 1;
						spdy = 0;
						jumped = false;
						jumping = false;
					}





					if (ohf3 == ObjectHitFlag.Hit || ohf5 == ObjectHitFlag.Hit)
					{
						nowX = (int)(nowX + 1);
						spdx = 0;
						this.nowImageNumber = 0;
						spdx = 1;
					}

					if (ohf4 == ObjectHitFlag.Hit || ohf6 == ObjectHitFlag.Hit)
					{
						nowX = (int)(nowX - 1);
						spdx = 0;
						this.nowImageNumber = 0;
						spdx = -1;
					}

					if (ohf7 == ObjectHitFlag.Hit || ohf8 == ObjectHitFlag.Hit)
					{
						nowY = nowY + 1;
						spdy = 0;
					}

					Rectangle jibun = new Rectangle(this.Point, this.Size);

					//ディフェンダーを殺す判定
					foreach (Sprite sp in Parent)
					{
						//プレイヤーの弾
						if (sp.MyGroup == SpriteGroup.DefenderWeapon && CheckHitJudge(jibun, new Rectangle(sp.Point, sp.Size)))
						{
							this.killed = true;
						}
						if (!(sp.MyGroup == SpriteGroup.Defender))
							continue;
						if (sp.killed)
							continue;
						if (CheckHitJudge(new Rectangle(this.Point, this.Size), new Rectangle(sp.Point, sp.Size)))
							sp.killed = true;
					}
					if (spdx < 0)		//ひだり
					{
						ImageHandle = GetImage(walkleft);
						UseAnime = true;
					}
					else if (spdx > 0)	//みぎ
					{
						ImageHandle = GetImage(walkright);
						UseAnime = true;
					}


					nowX += spdx;
					nowY += spdy;

					bohf1 = ohf1;
					bohf2 = ohf2;

				}


				/*
				if (nowY + me.Size.Height > jimeny)
				{
					nowY = jimeny - me.Size.Height;
					spdy = 0;
					jumped = false;
					jumping = false;
				}
				
				*/



				if (!killed)
					this.Point = new Point((int)nowX, (int)nowY);


			}

			if (killed)
			{
				Killing(killedtick);
				if (killedMaxTick < killedtick)
				{
					isDead = true;
				}
				killedtick += 5;
			}
			base.Draw(x, y, ref ks, chips);
		}

		int[] GetImage(int[] datas)
		{
			int[] lastdata = new int[datas.Length];
			int i = 0;
			foreach (int data in datas)
			{
				lastdata[i] = this.datas[data];
				i++;
			}
			return lastdata;
		}
	}

	public enum ModokeeMoveFlag
	{
		SineCurve, TailingMainSprite
	}

	public class Modokee_Ground : Sprite
	{
		public bool jumping = false;
		public bool jumped = false;
		public int jumpcnt = 0;
		public double[] Sin = null;
		ObjectHitFlag bohf1 = ObjectHitFlag.NotHit, bohf2 = ObjectHitFlag.NotHit;
		//bool isRight = false;
		//bool killed = false;
		int killedtick = 0;
		int bubbleNumber = 16;

		static int[] walkleft = { 0, 1 };
		static int[] walkright = { 4, 5 };
		static int[] dead = { 2 };
		static int[] stepped = { 3 };

		bool crushed = false;

		public ModokeeMoveFlag flag;

		int[] datas;

		public override SpriteGroup MyGroup
		{
			get
			{
				return SpriteGroup.Monster;
			}
		}

		public Modokee_Ground(Point p, Size s, bool useanime, int speed, int loop, Object[] objs, SpriteCollection par, int[] datas)
			: base(p, s, new[] { 0 }, useanime, speed, loop, objs, par)
		{

			this.datas = datas;
			this.KilledImageHandle = GetImage(dead);
			Sin = new double[360];
			for (int i = 0; i < 360; i++)
				Sin[i] = Math.Sin(i / 180.0 * Math.PI) * 64;
			spdx = -1;
			this.flag = ModokeeMoveFlag.SineCurve;
		}



		public override int killedMaxTick
		{
			get { return 240; }
		}

		int killedy = -16;


		public override void Killing(int tick)
		{

			if (this.ImageHandle != this.KilledImageHandle)
				this.ImageHandle = this.KilledImageHandle;
			this.nowImageNumber = 0;
			this.UseAnime = false;
			if (killedy == -16)
				killedy = Point.Y;
			if (tick < 180)
				this.Point = new Point(Point.X, (int)(killedy - Sin[tick % 360]));
			else
				this.Point = new Point(Point.X, Point.Y - (int)(Sin[179] - Sin[178]) * 5);
		}

		int deg = 0;

		public override void Draw(int x, int y, ref States ks, byte[, ,] chips)
		{
			int w, h;
			DX.GetWindowSize(out w, out h);

			if (Math.Abs(Parent.MainSprite.Point.X - this.nowX) > w)
				return;

			if (nowX < 0)
			{
				spdx = 1;
			}

			if (nowX > ks.map.Width * 16 - 16)
			{
				spdx = -1;
				nowX = ks.map.Width * 16 - 16;
			}

			if (nowY < 0)
			{
				spdy = 0;
				nowY = 0;
			}
			if (nowY > ks.map.Height * 16)
			{
				killed = true;
				isFall = true;
			}


			if (!killed)
			{


				Point judge1 = new Point((int)nowX, (int)nowY + 12);	//足判定1	

				Point judge2 = new Point((int)nowX + 32, (int)nowY + 12);	//足判定2

				Point judge3 = new Point((int)nowX, (int)nowY + 12);	//左障害物判定

				Point judge4 = new Point((int)nowX + 32, (int)nowY + 12);	//右障害物判定

				Point judge5 = new Point((int)nowX, (int)nowY + 6);		//左障害物判定2

				Point judge6 = new Point((int)nowX + 32, (int)nowY + 6);	//右障害物判定2

				Point judge7 = new Point((int)nowX, (int)nowY - 1);		//頭判定1

				Point judge8 = new Point((int)nowX + 32, (int)nowY - 1);	//頭判定2



				ObjectHitFlag ohf1 = ObjectHitFlag.NotHit,
							  ohf2 = ObjectHitFlag.NotHit,
							  ohf3 = ObjectHitFlag.NotHit,
							  ohf4 = ObjectHitFlag.NotHit,
							  ohf5 = ObjectHitFlag.NotHit,
							  ohf6 = ObjectHitFlag.NotHit,
							  ohf7 = ObjectHitFlag.NotHit,
							  ohf8 = ObjectHitFlag.NotHit;

				//もし判定点が床部分なら、上を目指す。
				try
				{
					ohf1 = mptobjects[chips[judge1.X / 16, judge1.Y / 16, 0]].CheckHit(judge1.X % 16, judge1.Y % 16);

					ohf2 = mptobjects[chips[judge2.X / 16, judge2.Y / 16, 0]].CheckHit(judge2.X % 16, judge2.Y % 16);

					ohf3 = mptobjects[chips[judge3.X / 16, judge3.Y / 16, 0]].CheckHit(judge3.X % 16, judge3.Y % 16);

					ohf4 = mptobjects[chips[judge4.X / 16, judge4.Y / 16, 0]].CheckHit(judge4.X % 16, judge4.Y % 16);

					ohf5 = mptobjects[chips[judge5.X / 16, judge5.Y / 16, 0]].CheckHit(judge5.X % 16, judge5.Y % 16);

					ohf6 = mptobjects[chips[judge6.X / 16, judge6.Y / 16, 0]].CheckHit(judge6.X % 16, judge6.Y % 16);

					ohf7 = mptobjects[chips[judge7.X / 16, judge7.Y / 16, 0]].CheckHit(judge7.X % 16, judge7.Y % 16);

					ohf8 = mptobjects[chips[judge8.X / 16, judge8.Y / 16, 0]].CheckHit(judge8.X % 16, judge8.Y % 16);


				}
				catch (Exception)
				{
				}

				if (ohf1 == ObjectHitFlag.InWater && ohf2 == ObjectHitFlag.InWater)
				{
					for (int bi = 0; bi < bubbleNumber / 2; bi++)
						DX.DrawCircle(this.Point.X + DX.GetRand(this.Size.Width), this.Point.Y + this.Size.Height - DX.GetRand(4), DX.GetRand(3), DX.GetColor(255, 255, 255), 0);
					if (bubbleNumber > -1 && DX.GetNowCount() % 2 == 0)
						bubbleNumber--;
					jumping = false;
					jumped = false;
				}
				else
				{
					if (bohf1 != ObjectHitFlag.InWater && bohf2 != ObjectHitFlag.InWater)
						bubbleNumber = 16;

				}

				if (ohf3 != ObjectHitFlag.Hit && ohf4 != ObjectHitFlag.Hit && ohf5 != ObjectHitFlag.Hit && ohf6 != ObjectHitFlag.Hit)
				{


					switch (flag)
					{
						case ModokeeMoveFlag.SineCurve:
							spdy = (float)Math.Sin(deg / 180.0 * Math.PI);
							deg = (deg + 5) % 360;
							break;
						case ModokeeMoveFlag.TailingMainSprite:
							float atan = (float)Math.Atan2(this.Parent.MainSprite.Point.Y - nowY, this.Parent.MainSprite.Point.X - nowX);
							spdx = (float)Math.Cos(atan);
							spdy = (float)Math.Sin(atan);
							break;
					}
				}

				if (!killed)
				{
					//if ((ohf1 == ObjectHitFlag.NotHit || ohf1 == ObjectHitFlag.InWater) && (ohf2 == ObjectHitFlag.NotHit || ohf2 == ObjectHitFlag.InWater))
					//spdy += 0.1f;



					if (ohf1 == ObjectHitFlag.Hit || ohf2 == ObjectHitFlag.Hit)
					{
						//nowY = nowY - spdy;
						spdy = -1;
						jumped = false;
						jumping = false;
						//spdx = DX.GetRand(10) - 5;
						//spdy = DX.GetRand(10) - 5;
					}





					if (ohf3 == ObjectHitFlag.Hit || ohf5 == ObjectHitFlag.Hit)
					{
						//nowX = (int)(nowX - spdx);
						spdx = 1;
						this.nowImageNumber = 0;
						//spdx = DX.GetRand(10) - 5;
						//spdy = DX.GetRand(10) - 5;
						//spdy = -1;
					}

					if (ohf4 == ObjectHitFlag.Hit || ohf6 == ObjectHitFlag.Hit)
					{
						//nowX = (int)(nowX - spdx);
						spdx = -1;
						this.nowImageNumber = 0;
						//spdx = -1;
						//spdx = -1;
					}

					if (ohf7 == ObjectHitFlag.Hit || ohf8 == ObjectHitFlag.Hit)
					{
						//nowY = nowY - spdy;
						spdy = 1;
					}


					Rectangle jibun = new Rectangle(this.Point, this.Size);


					//プレイヤーを殺す判定
					foreach (Sprite sp in Parent)
					{
						//プレイヤーの弾
						if (sp.MyGroup == SpriteGroup.DefenderWeapon && CheckHitJudge(jibun, new Rectangle(sp.Point, sp.Size)))
						{
							this.killed = true;
						}
						if (!(sp.MyGroup == SpriteGroup.Defender))
							continue;
						if (sp.killed)
							continue;
						if (CheckHitJudge(new Rectangle(sp.Point.X, sp.Point.Y + sp.Size.Height / 2, sp.Size.Width, sp.Size.Height / 2), new Rectangle(this.Point.X, this.Point.Y + 8, this.Size.Width, this.Size.Height / 2)))
						{
							sp.killed = true;
						}
						else if (CheckHitJudge(new Rectangle(sp.Point.X, sp.Point.Y + sp.Size.Height / 2, sp.Size.Width, sp.Size.Height / 2), new Rectangle(this.Point.X, this.Point.Y, this.Size.Width, this.Size.Height / 2)))
						{
							if (ks.inz == 1)
								sp.spdy -= 5;
							else
								sp.spdy -= 3;
							this.killed = true;
							this.crushed = true;
						}
					}


					if (spdx < 0)		//ひだり
					{
						ImageHandle = GetImage(walkleft);
						UseAnime = true;
					}
					else if (spdx > 0)	//みぎ
					{
						ImageHandle = GetImage(walkright);
						UseAnime = true;
					}


					nowX += spdx;
					nowY += spdy;

					bohf1 = ohf1;
					bohf2 = ohf2;

				}


				/*
				if (nowY + me.Size.Height > jimeny)
				{
					nowY = jimeny - me.Size.Height;
					spdy = 0;
					jumped = false;
					jumping = false;
				}
				
				*/



				if (!killed)
					this.Point = new Point((int)nowX, (int)nowY);


			}

			if (killed)
			{
				Killing(killedtick);
				if (killedMaxTick < killedtick)
				{
					isDead = true;
				}
				killedtick += 5;
			}
			base.Draw(x, y, ref ks, chips);
		}

		int[] GetImage(int[] datas)
		{
			int[] lastdata = new int[datas.Length];
			int i = 0;
			foreach (int data in datas)
			{
				lastdata[i] = this.datas[data];
				i++;
			}
			return lastdata;
		}


	}

	public class Modokee_Cave : Modokee_Ground
	{
		public Modokee_Cave(Point p, Size s, bool useanime, int speed, int loop, Object[] objs, SpriteCollection par, int[] datas)
			: base(p, s, useanime, speed, loop, objs, par, datas)
		{
			this.flag = ModokeeMoveFlag.TailingMainSprite;
		}
	}

	public class Arrow : Sprite
	{
		public bool jumping = false;
		public bool jumped = false;
		public int jumpcnt = 0;
		public double[] Sin = null;
		double rad = 0;
		ObjectHitFlag bohf1 = ObjectHitFlag.NotHit;
		//bool isRight = false;
		//bool killed = false;
		int killedtick = 0;
		int bubbleNumber = 16;

		bool crushed = false;

		int[] datas;

		public override SpriteGroup MyGroup
		{
			get
			{
				return SpriteGroup.MonsterWeapon;
			}
			
		}

		public Arrow(Point p, Size s, Object[] objs, SpriteCollection par, int[] datas, float spdx)
			: base(p, s, new[] { 0 }, false, 0, 0, objs, par)
		{

			this.datas = datas;
			this.KilledImageHandle = null;
			//Sin = new double[360];
			//for (int i = 0; i < 360; i++)
			//	Sin[i] = Math.Sin(i / 180.0 * Math.PI) * 64;
			this.spdx = spdx;
			this.spdy = 0;
			ImageHandle = GetImage(new[] { 0 });
		}



		public override int killedMaxTick
		{
			get { return 240; }
		}

		int killedy = -16;

		public override void Killing(int tick)
		{
			if (tick > 135)
				isDead = true;
		}

		public override void Draw(int x, int y, ref States ks, byte[, ,] chips)
		{
			int w, h;
			DX.GetWindowSize(out w, out h);

			if (Math.Abs(Parent.MainSprite.Point.X - this.nowX) > w)
				return;

			if (nowX < 0)
			{
				spdx = 1;
			}

			if (nowX > ks.map.Width * 16 - 16)
			{
				spdx = -1;
				nowX = ks.map.Width * 16 - 16;
			}

			if (nowY < 0)
			{
				spdy = 0;
				nowY = 0;
			}
			if (nowY > ks.map.Height * 16)
			{
				killed = true;
				isFall = true;
			}


			if (!killed)
			{

				/*
				Point judge1 = new Point((int)nowX + 0, (int)nowY + 10);	//足判定1	

				Point judge2 = new Point((int)nowX + 16, (int)nowY + 10);	//足判定2

				Point judge3 = new Point((int)nowX + 5, (int)nowY + 7);	//左障害物判定

				Point judge4 = new Point((int)nowX + 11, (int)nowY + 7);	//右障害物判定

				Point judge5 = new Point((int)nowX + 0, (int)nowY + 4);		//頭判定1

				Point judge6 = new Point((int)nowX + 16, (int)nowY + 4);	//頭判定2
				*/

				Point judgearrow = new Point((int)(nowX + Math.Cos(rad) * 4), (int)(nowY + Math.Sin(rad) * 4));

				ObjectHitFlag ohf1 = ObjectHitFlag.NotHit;
				//もし判定点が床部分なら、上を目指す。
				try
				{

					ohf1 = mptobjects[chips[judgearrow.X / 16, judgearrow.Y / 16, 0]].CheckHit(judgearrow.X % 16, judgearrow.Y % 16);
					

				}
				catch (Exception)
				{
				}



				
				if (ohf1 == ObjectHitFlag.InWater)
				{
					for (int bi = 0; bi < bubbleNumber / 2; bi++)
						DX.DrawCircle(this.Point.X + DX.GetRand(this.Size.Width), this.Point.Y + this.Size.Height - DX.GetRand(4), DX.GetRand(3), DX.GetColor(255, 255, 255), 0);
					if (bubbleNumber > -1 && DX.GetNowCount() % 2 == 0)
						bubbleNumber--;
					jumping = false;
					jumped = false;
					spdx = spdx * 0.9f;
					spdy = spdy * 0.9f;
				}
				else
				{
					if (bohf1 != ObjectHitFlag.InWater)
					{
						bubbleNumber = 16;
						
					}
				}

				

				if (!killed)
				{
					
					if (ohf1 == ObjectHitFlag.NotHit || ohf1 == ObjectHitFlag.InWater)
						spdy += 0.1f;



					if (ohf1 == ObjectHitFlag.Hit)
					{
						spdx = 0;
						spdy = 0;
						killed = true;
					}

				
					//ディフェンダーを殺す判定
					foreach (Sprite sp in Parent)
					{
						if (!(sp.MyGroup == SpriteGroup.Defender))
							continue;
						if (sp == this)
							continue;
						if (sp.killed)
							continue;
						if (CheckHitJudge(new Rectangle(this.Point, this.Size), new Rectangle(sp.Point, sp.Size)))
						{
							isDead= true;
							sp.killed = true;
						}
					}

					


					nowX += spdx;
					nowY += spdy;

					bohf1 = ohf1;

				}


				/*
				if (nowY + me.Size.Height > jimeny)
				{
					nowY = jimeny - me.Size.Height;
					spdy = 0;
					jumped = false;
					jumping = false;
				}
				
				*/



				if (!killed)
					this.Point = new Point((int)nowX, (int)nowY);


			}

			if (killed)
				Killing(killedtick++);
			//base.Draw(x, y, ref ks, chips);

			//DX.DrawGraph(Point.X + x, Point.Y + y, ImageHandle[nowImageNumber], 1);

			DX.DrawRotaGraph2(Point.X + x, Point.Y + y, 8, 8, 1, ((spdx != 0 && spdy != 0) ? (rad = Math.Atan2(spdy, spdx)) + Math.PI : rad + Math.PI), ImageHandle[0], 1);
			
		}

		int[] GetImage(int[] datas)
		{
			int[] lastdata = new int[datas.Length];
			int i = 0;
			foreach (int data in datas)
			{
				lastdata[i] = this.datas[data];
				i++;
			}
			return lastdata;
		}

		bool CheckHitJudgeEx(int x, int y, int w, int h, int r)
		{
			var s = Math.Sin(-r);
			var c = Math.Cos(-r);
			var xx = Math.Abs(x * c - y * s);
			var yy = Math.Abs(y * s - y * c);

			if (xx < w / 2.0 && yy < h / 2.0)
				return true;

			return false;
		}

	}

	public class Archer : Sprite
	{
		public bool jumping = false;
		public bool jumped = false;
		public int jumpcnt = 0;
		public double[] Sin = null;
		ObjectHitFlag bohf1 = ObjectHitFlag.NotHit, bohf2 = ObjectHitFlag.NotHit;
		//bool isRight = false;
		//bool killed = false;
		int killedtick = 0;
		int bubbleNumber = 16;
		int tick = 0;

		bool crushed = false;

		int[] datas;

		public override SpriteGroup MyGroup
		{
			get
			{
				return SpriteGroup.Monster;
			}
		}

		int[] arrows;

		public Archer(Point p, Size s, Object[] objs, SpriteCollection par, int[] datas, int[] arrowdatas)
			: base(p, s, new[] { 0 }, false, 0, 0, objs, par)
		{

			this.datas = datas;
			this.KilledImageHandle = null;
			Sin = new double[360];
			for (int i = 0; i < 360; i++)
				Sin[i] = Math.Sin(i / 180.0 * Math.PI) * 64;
			ImageHandle = new[] { datas[0] };
			arrows = arrowdatas;
		}



		public override int killedMaxTick
		{
			get { return 240; }
		}

		int killedy = -16;

		public override void Killing(int tick)
		{

			//if (this.ImageHandle != this.KilledImageHandle)
			//	this.ImageHandle = this.KilledImageHandle;
			this.nowImageNumber = 0;
			this.UseAnime = false;
			if (killedy == -16)
				killedy = Point.Y;
			if (tick < 180)
				this.Point = new Point(Point.X, (int)(killedy - Sin[tick % 360]));
			else
				this.Point = new Point(Point.X, Point.Y - (int)(Sin[179] - Sin[178]) * 5);
		}

		int nowstatus = 0;

		public override void Draw(int x, int y, ref States ks, byte[, ,] chips)
		{
			int w, h;
			DX.GetWindowSize(out w, out h);

			if (Math.Abs(Parent.MainSprite.Point.X - this.nowX) > w)
				return;

			if (nowX < 0)
			{
				spdx = 1;
			}

			if (nowX > ks.map.Width * 16 - 16)
			{
				spdx = -1;
				nowX = ks.map.Width * 16 - 16;
			}

			if (nowY < 0)
			{
				spdy = 0;
				nowY = 0;
			}
			if (nowY > ks.map.Height * 16)
			{
				killed = true;
				isFall = true;
			}


			if (!killed)
			{
				nowX += spdx; nowY += spdy;

				Point judge1 = new Point((int)nowX + 0, (int)nowY + 31);	//足判定1	

				Point judge2 = new Point((int)nowX + 32, (int)nowY + 31);	//足判定2

				Point judge3 = new Point((int)nowX - 1, (int)nowY + 24);	//左障害物判定

				Point judge4 = new Point((int)nowX + 32, (int)nowY + 24);	//右障害物判定

				Point judge5 = new Point((int)nowX - 1, (int)nowY + 12);	//左障害物判定2

				Point judge6 = new Point((int)nowX + 32, (int)nowY + 12);	//右障害物判定2

				Point judge7 = new Point((int)nowX + 0, (int)nowY - 1);		//頭判定1

				Point judge8 = new Point((int)nowX + 32, (int)nowY - 1);	//頭判定2



				ObjectHitFlag ohf1 = ObjectHitFlag.NotHit,
							  ohf2 = ObjectHitFlag.NotHit,
							  ohf3 = ObjectHitFlag.NotHit,
							  ohf4 = ObjectHitFlag.NotHit,
							  ohf5 = ObjectHitFlag.NotHit,
							  ohf6 = ObjectHitFlag.NotHit,
							  ohf7 = ObjectHitFlag.NotHit,
							  ohf8 = ObjectHitFlag.NotHit;

				//もし判定点が床部分なら、上を目指す。
				try
				{
					ohf1 = mptobjects[chips[judge1.X / 16, judge1.Y / 16, 0]].CheckHit(judge1.X % 16, judge1.Y % 16);

					ohf2 = mptobjects[chips[judge2.X / 16, judge2.Y / 16, 0]].CheckHit(judge2.X % 16, judge2.Y % 16);

					ohf3 = mptobjects[chips[judge3.X / 16, judge3.Y / 16, 0]].CheckHit(judge3.X % 16, judge3.Y % 16);

					ohf4 = mptobjects[chips[judge4.X / 16, judge4.Y / 16, 0]].CheckHit(judge4.X % 16, judge4.Y % 16);

					ohf5 = mptobjects[chips[judge5.X / 16, judge5.Y / 16, 0]].CheckHit(judge5.X % 16, judge5.Y % 16);

					ohf6 = mptobjects[chips[judge6.X / 16, judge6.Y / 16, 0]].CheckHit(judge6.X % 16, judge6.Y % 16);

					ohf7 = mptobjects[chips[judge7.X / 16, judge7.Y / 16, 0]].CheckHit(judge7.X % 16, judge7.Y % 16);

					ohf8 = mptobjects[chips[judge8.X / 16, judge8.Y / 16, 0]].CheckHit(judge8.X % 16, judge8.Y % 16);


				}
				catch (Exception)
				{
				}

				if (ohf1 == ObjectHitFlag.InWater && ohf2 == ObjectHitFlag.InWater)
				{
					for (int bi = 0; bi < bubbleNumber / 2; bi++)
						DX.DrawCircle(this.Point.X + DX.GetRand(this.Size.Width), this.Point.Y + this.Size.Height - DX.GetRand(4), DX.GetRand(3), DX.GetColor(255, 255, 255), 0);
					if (bubbleNumber > -1 && DX.GetNowCount() % 2 == 0)
						bubbleNumber--;
				}
				else
				{
					if (bohf1 != ObjectHitFlag.InWater && bohf2 != ObjectHitFlag.InWater)
						bubbleNumber = 16;

				}

				if (!killed)
				{
					if ((ohf1 == ObjectHitFlag.NotHit || ohf1 == ObjectHitFlag.InWater) && (ohf2 == ObjectHitFlag.NotHit || ohf2 == ObjectHitFlag.InWater))
						spdy += 0.1f;

					if (ohf1 == ObjectHitFlag.Hit || ohf2 == ObjectHitFlag.Hit)
					{
						nowY = (int)(nowY - 1);
						spdy = 0;
					}

					if (ohf3 == ObjectHitFlag.Hit || ohf5 == ObjectHitFlag.Hit)
					{
						nowX = (int)(nowX - spdx);
						spdx = 0;
						this.nowImageNumber = 0;
					}

					if (ohf4 == ObjectHitFlag.Hit || ohf6 == ObjectHitFlag.Hit)
					{
						nowX = (int)(nowX - spdx);
						spdx = 0;
						this.nowImageNumber = 0;
					}

					if (ohf7 == ObjectHitFlag.Hit || ohf8 == ObjectHitFlag.Hit)
					{
						nowY = nowY - spdy;
						spdy = 0;
					}

					Rectangle jibun = new Rectangle(this.Point, this.Size);

					//プレイヤーを殺す判定
					foreach (Sprite sp in Parent)
					{
						//プレイヤーの弾
						if (sp.MyGroup == SpriteGroup.DefenderWeapon && CheckHitJudge(jibun, new Rectangle(sp.Point, sp.Size)))
						{
							this.killed = true;
						}
						if (!(sp.MyGroup == SpriteGroup.Defender))
							continue;
						if (Parent.MainSprite.MyGroup == SpriteGroup.Defender)
							isRight = Parent.MainSprite.Point.X > this.Point.X;
							
						if (sp.killed)
							continue;
						/*
						if (CheckHitJudge(new Rectangle(sp.Point.X, sp.Point.Y + sp.Size.Height / 2, sp.Size.Width, sp.Size.Height / 2), new Rectangle(this.Point.X, this.Point.Y + 8, this.Size.Width, this.Size.Height / 2)))
						{
							sp.killed = true;
						}
						 */
						else if (CheckHitJudge(new Rectangle(sp.Point.X, sp.Point.Y + sp.Size.Height / 2, sp.Size.Width, sp.Size.Height / 2), new Rectangle(this.Point.X, this.Point.Y, this.Size.Width, this.Size.Height / 2)))
						{
							if (ks.inz == 1)
								sp.spdy -= 5;
							else
								sp.spdy -= 3;
							this.killed = true;
							this.crushed = true;
						}

					}




					bohf1 = ohf1;
					bohf2 = ohf2;

				}


				/*
				if (nowY + me.Size.Height > jimeny)
				{
					nowY = jimeny - me.Size.Height;
					spdy = 0;
					jumped = false;
					jumping = false;
				}
				
				*/

				if (nowstatus != 2 && nowstatus != 0 && tick == 15 || tick == 30)
				{
					if (nowstatus == 2)
						Parent.Add(new Arrow(new Point(this.Point.X + (isRight ? 33 : -17), this.Point.Y + 8), new Size(16, 16), mptobjects, Parent, arrows, (isRight ? (DX.GetRand(4) + 1) : (-DX.GetRand(4) - 1))));
					tick = -1;
					nowstatus = (nowstatus + 1) % 4;
				}

				ImageHandle = GetImage(nowstatus + (isRight ? 4 : 0));

				if (!killed)
					this.Point = new Point((int)nowX, (int)nowY);

				tick++;
			}

			if (killed)
			{
				Killing(killedtick);
				if (killedMaxTick < killedtick)
				{
					isDead = true;
				}
				killedtick += 5;
			}
			if (!killed)
				base.Draw(x, y, ref ks, chips);
			else
				DX.DrawRotaGraph2(Point.X + x, Point.Y + y, 0, 0, 1, (killedtick*2 % 360) / 180.0 * Math.PI, ImageHandle[0], 1, 0);
				//DX.DrawGraph(Point.X + x, Point.Y + y, ImageHandle[nowImageNumber], 1);
		}


		int[] GetImage(params int[] datas)
		{
			int[] lastdata = new int[datas.Length];
			int i = 0;
			foreach (int data in datas)
			{
				lastdata[i] = this.datas[data];
				i++;
			}
			return lastdata;
		}

	}

	public class Solider : Sprite
	{
		public bool jumping = false;
		public bool jumped = false;
		public int jumpcnt = 0;
		public double[] Sin = null;
		ObjectHitFlag bohf1 = ObjectHitFlag.NotHit, bohf2 = ObjectHitFlag.NotHit;
		//bool isRight = false;
		//bool killed = false;
		int killedtick = 0;
		int bubbleNumber = 16;

		static int[] main = { 0, 1, 2, 3 };
		static int[] dead = { 4 };

		bool crushed = false;

		int[] datas;

		public override SpriteGroup MyGroup
		{
			get
			{
				return SpriteGroup.Monster;
			}
		}

		public Solider(Point p, Size s, bool useanime, int speed, int loop, Object[] objs, SpriteCollection par, int[] datas)
			: base(p, s, new[] { 0 }, useanime, speed, loop, objs, par)
		{

			this.datas = datas;
			this.ImageHandle = GetImage(main);
			this.KilledImageHandle = GetImage(dead);
			Sin = new double[360];
			for (int i = 0; i < 360; i++)
				Sin[i] = Math.Sin(i / 180.0 * Math.PI) * 64;
		}



		public override int killedMaxTick
		{
			get { return 240; }
		}

		int killedy = -16;

		public override void Killing(int tick)
		{

			if (this.ImageHandle != this.KilledImageHandle)
				this.ImageHandle = this.KilledImageHandle;
			this.nowImageNumber = 0;
			this.UseAnime = false;
			if (killedy == -16)
				killedy = Point.Y;
			if (tick < 180)
				this.Point = new Point(Point.X, (int)(killedy - Sin[tick % 360]));
			else
				this.Point = new Point(Point.X, Point.Y - (int)(Sin[179] - Sin[178]) * 5);
		}

		public override void Draw(int x, int y, ref States ks, byte[, ,] chips)
		{
			int w, h;
			DX.GetWindowSize(out w, out h);

			if (Math.Abs(Parent.MainSprite.Point.X - this.nowX) > w)
				return;

			if (nowX < 0)
			{
				spdx = 0;
			}

			if (nowX > ks.map.Width * 16 - 16)
			{
				spdx = 0;
				nowX = ks.map.Width * 16 - 16;
			}

			if (nowY < 0)
			{
				spdy = 0;
				nowY = 0;
			}
			if (nowY > ks.map.Height * 16)
			{
				killed = true;
				isFall = true;
			}


			if (!killed)
			{
				nowX += spdx;
				nowY += spdy;

				Point judge1 = new Point((int)nowX + 2, (int)nowY + 32);	//足判定1	

				Point judge2 = new Point((int)nowX + 12, (int)nowY + 32);	//足判定2

				Point judge3 = new Point((int)nowX + 1, (int)nowY + 12);	//左障害物判定

				Point judge4 = new Point((int)nowX + 13, (int)nowY + 12);	//右障害物判定

				Point judge5 = new Point((int)nowX + 1, (int)nowY + 6);		//左障害物判定2

				Point judge6 = new Point((int)nowX + 13, (int)nowY + 6);	//右障害物判定2

				Point judge7 = new Point((int)nowX + 2, (int)nowY - 1);		//頭判定1

				Point judge8 = new Point((int)nowX + 12, (int)nowY - 1);	//頭判定2



				ObjectHitFlag ohf1 = ObjectHitFlag.NotHit,
							  ohf2 = ObjectHitFlag.NotHit,
							  ohf3 = ObjectHitFlag.NotHit,
							  ohf4 = ObjectHitFlag.NotHit,
							  ohf5 = ObjectHitFlag.NotHit,
							  ohf6 = ObjectHitFlag.NotHit,
							  ohf7 = ObjectHitFlag.NotHit,
							  ohf8 = ObjectHitFlag.NotHit;

				//もし判定点が床部分なら、上を目指す。
				try
				{
					ohf1 = mptobjects[chips[judge1.X / 16, judge1.Y / 16, 0]].CheckHit(judge1.X % 16, judge1.Y % 16);

					ohf2 = mptobjects[chips[judge2.X / 16, judge2.Y / 16, 0]].CheckHit(judge2.X % 16, judge2.Y % 16);

					ohf3 = mptobjects[chips[judge3.X / 16, judge3.Y / 16, 0]].CheckHit(judge3.X % 16, judge3.Y % 16);

					ohf4 = mptobjects[chips[judge4.X / 16, judge4.Y / 16, 0]].CheckHit(judge4.X % 16, judge4.Y % 16);

					ohf5 = mptobjects[chips[judge5.X / 16, judge5.Y / 16, 0]].CheckHit(judge5.X % 16, judge5.Y % 16);

					ohf6 = mptobjects[chips[judge6.X / 16, judge6.Y / 16, 0]].CheckHit(judge6.X % 16, judge6.Y % 16);

					ohf7 = mptobjects[chips[judge7.X / 16, judge7.Y / 16, 0]].CheckHit(judge7.X % 16, judge7.Y % 16);

					ohf8 = mptobjects[chips[judge8.X / 16, judge8.Y / 16, 0]].CheckHit(judge8.X % 16, judge8.Y % 16);


				}
				catch (Exception)
				{
				}

				if (ohf1 == ObjectHitFlag.InWater && ohf2 == ObjectHitFlag.InWater)
				{
					for (int bi = 0; bi < bubbleNumber / 2; bi++)
						DX.DrawCircle(this.Point.X + DX.GetRand(this.Size.Width), this.Point.Y + this.Size.Height - DX.GetRand(4), DX.GetRand(3), DX.GetColor(255, 255, 255), 0);
					if (bubbleNumber > -1 && DX.GetNowCount() % 2 == 0)
						bubbleNumber--;
					jumping = false;
					jumped = false;
				}
				else
				{
					if (bohf1 != ObjectHitFlag.InWater && bohf2 != ObjectHitFlag.InWater)
						bubbleNumber = 16;

				}



				if (!killed)
				{
					if ((ohf1 == ObjectHitFlag.NotHit || ohf1 == ObjectHitFlag.InWater) && (ohf2 == ObjectHitFlag.NotHit || ohf2 == ObjectHitFlag.InWater))
						spdy += 0.1f;



					if (ohf1 == ObjectHitFlag.Hit || ohf2 == ObjectHitFlag.Hit)
					{
						nowY = nowY - 1;
						spdy = 0;
						jumped = false;
						jumping = false;
					}





					if (ohf3 == ObjectHitFlag.Hit || ohf5 == ObjectHitFlag.Hit)
					{
						nowX = (int)(nowX + 1);
						spdx = 0;
						this.nowImageNumber = 0;
						spdx = 1;
					}

					if (ohf4 == ObjectHitFlag.Hit || ohf6 == ObjectHitFlag.Hit)
					{
						nowX = (int)(nowX - 1);
						spdx = 0;
						this.nowImageNumber = 0;
						spdx = -1;
					}

					if (ohf7 == ObjectHitFlag.Hit || ohf8 == ObjectHitFlag.Hit)
					{
						nowY = nowY + 1;
						spdy = 0;
					}

					Rectangle jibun = new Rectangle(this.Point, this.Size);


					//プレイヤーを殺す判定
					foreach (Sprite sp in Parent)
					{
						//プレイヤーの弾
						if (sp.MyGroup == SpriteGroup.DefenderWeapon && CheckHitJudge(jibun, new Rectangle(sp.Point, sp.Size)))
						{
							this.killed = true;
						}
						if (!(sp.MyGroup == SpriteGroup.Defender))
							continue;
						if (sp.killed)
							continue;
						if (CheckHitJudge(new Rectangle(sp.Point.X, sp.Point.Y + sp.Size.Height / 2, sp.Size.Width, sp.Size.Height / 2), new Rectangle(this.Point.X, this.Point.Y + 8, this.Size.Width, this.Size.Height / 2)))
						{
							sp.killed = true;
						}
						else if (CheckHitJudge(new Rectangle(sp.Point.X, sp.Point.Y + sp.Size.Height / 2, sp.Size.Width, sp.Size.Height / 2), new Rectangle(this.Point.X, this.Point.Y, this.Size.Width, this.Size.Height / 2)))
						{
							if (ks.inz == 1)
								sp.spdy -= 5;
							else
								sp.spdy -= 3;
							this.killed = true;
							this.crushed = true;
						}

					}


					

					bohf1 = ohf1;
					bohf2 = ohf2;

				}


				/*
				if (nowY + me.Size.Height > jimeny)
				{
					nowY = jimeny - me.Size.Height;
					spdy = 0;
					jumped = false;
					jumping = false;
				}
				
				*/



				if (!killed)
					this.Point = new Point((int)nowX, (int)nowY);


			}

			if (killed)
			{
				Killing(killedtick);
				if (killedMaxTick < killedtick)
				{
					isDead = true;
				}
				killedtick += 5;
			}
			base.Draw(x, y, ref ks, chips);
		}

		int[] GetImage(int[] datas)
		{
			int[] lastdata = new int[datas.Length];
			int i = 0;
			foreach (int data in datas)
			{
				lastdata[i] = this.datas[data];
				i++;
			}
			return lastdata;
		}
	}

	public class Woody : Sprite
	{
		public bool jumping = false;
		public bool jumped = false;
		public int jumpcnt = 0;
		public double[] Sin = null;
		ObjectHitFlag bohf1 = ObjectHitFlag.NotHit, bohf2 = ObjectHitFlag.NotHit;
		//bool isRight = false;
		//bool killed = false;
		int killedtick = 0;
		int bubbleNumber = 16;

		static int normalleft = 0;
		static int normalright = 1;
		static int hurtleft = 2;
		static int hurtright = 3;

		int[] leafhndl;

		int life = 3;

		bool gunflag = false;

		// 0...待機
		// 1...葉っぱを投げる
		// 2...移動
		int nowstatus = 0;

		bool crushed = false;

		int[] datas;

		public override SpriteGroup MyGroup
		{
			get
			{
				return SpriteGroup.Monster;
			}
		}

		public Woody(Point p, Size s, Object[] objs, SpriteCollection par, int[] datas, int[] leafdatas)
			: base(p, s, new[] { 0 }, false, 0, 0, objs, par)
		{

			this.datas = datas;
			this.ImageHandle = GetImage(normalleft);
			leafhndl = leafdatas;
			//this.KilledImageHandle = GetImage(dead);
			Sin = new double[360];
			for (int i = 0; i < 360; i++)
				Sin[i] = Math.Sin(i / 180.0 * Math.PI) * 64;
		}



		public override int killedMaxTick
		{
			get { return 240; }
		}

		int killedy = -16;

		public override void Killing(int tick)
		{

			//if (this.ImageHandle != this.KilledImageHandle)
			//	this.ImageHandle = this.KilledImageHandle;
			this.nowImageNumber = 0;
			this.UseAnime = false;
			if (killedy == -16)
				killedy = Point.Y;
			if (tick < 180)
				this.Point = new Point(Point.X, (int)(killedy - Sin[tick % 360]));
			else
				this.Point = new Point(Point.X, Point.Y - (int)(Sin[179] - Sin[178]) * 5);
		}

		int tick = 0;
		int targetX = 0;
		public override void Draw(int x, int y, ref States ks, byte[, ,] chips)
		{
			int w, h;
			DX.GetWindowSize(out w, out h);

			if (Math.Abs(Parent.MainSprite.Point.X - this.nowX) > w)
				return;

			if (nowX < 0)
			{
				spdx = 0;
				nowX = 0;
				targetX = (int)nowX;
			}

			if (nowX > ks.map.Width * 16 - 16)
			{
				spdx = 0;
				targetX = (int)nowX;
				nowX = ks.map.Width * 16 - 16;
			}

			if (nowY < 0)
			{
				spdy = 0;
				nowY = 0;
			}
			if (nowY > ks.map.Height * 16)
			{
				killed = true;
				isFall = true;
			}

			

			if (!killed)
			{
				//nowX += spdx;
				nowY += spdy;

				Point judge1 = new Point((int)nowX, (int)nowY + 64);	//足判定1	

				Point judge2 = new Point((int)nowX + 63, (int)nowY + 64);	//足判定2

				Point judge3 = new Point((int)nowX + 1, (int)nowY + 52);	//左障害物判定

				Point judge4 = new Point((int)nowX + 63, (int)nowY + 52);	//右障害物判定

				Point judge5 = new Point((int)nowX, (int)nowY + 32);		//左障害物判定2

				Point judge6 = new Point((int)nowX + 64, (int)nowY + 32);	//右障害物判定2

				Point judge7 = new Point((int)nowX + 2, (int)nowY - 1);		//頭判定1

				Point judge8 = new Point((int)nowX + 64, (int)nowY - 1);	//頭判定2



				ObjectHitFlag ohf1 = ObjectHitFlag.NotHit,
							  ohf2 = ObjectHitFlag.NotHit,
							  ohf3 = ObjectHitFlag.NotHit,
							  ohf4 = ObjectHitFlag.NotHit,
							  ohf5 = ObjectHitFlag.NotHit,
							  ohf6 = ObjectHitFlag.NotHit,
							  ohf7 = ObjectHitFlag.NotHit,
							  ohf8 = ObjectHitFlag.NotHit;

				//もし判定点が床部分なら、上を目指す。
				try
				{
					ohf1 = mptobjects[chips[judge1.X / 16, judge1.Y / 16, 0]].CheckHit(judge1.X % 16, judge1.Y % 16);

					ohf2 = mptobjects[chips[judge2.X / 16, judge2.Y / 16, 0]].CheckHit(judge2.X % 16, judge2.Y % 16);

					ohf3 = mptobjects[chips[judge3.X / 16, judge3.Y / 16, 0]].CheckHit(judge3.X % 16, judge3.Y % 16);

					ohf4 = mptobjects[chips[judge4.X / 16, judge4.Y / 16, 0]].CheckHit(judge4.X % 16, judge4.Y % 16);

					ohf5 = mptobjects[chips[judge5.X / 16, judge5.Y / 16, 0]].CheckHit(judge5.X % 16, judge5.Y % 16);

					ohf6 = mptobjects[chips[judge6.X / 16, judge6.Y / 16, 0]].CheckHit(judge6.X % 16, judge6.Y % 16);

					ohf7 = mptobjects[chips[judge7.X / 16, judge7.Y / 16, 0]].CheckHit(judge7.X % 16, judge7.Y % 16);

					ohf8 = mptobjects[chips[judge8.X / 16, judge8.Y / 16, 0]].CheckHit(judge8.X % 16, judge8.Y % 16);


				}
				catch (Exception)
				{
				}

				if (ohf1 == ObjectHitFlag.InWater && ohf2 == ObjectHitFlag.InWater)
				{
					for (int bi = 0; bi < bubbleNumber / 2; bi++)
						DX.DrawCircle(this.Point.X + DX.GetRand(this.Size.Width), this.Point.Y + this.Size.Height - DX.GetRand(4), DX.GetRand(3), DX.GetColor(255, 255, 255), 0);
					if (bubbleNumber > -1 && DX.GetNowCount() % 2 == 0)
						bubbleNumber--;
					jumping = false;
					jumped = false;
				}
				else
				{
					if (bohf1 != ObjectHitFlag.InWater && bohf2 != ObjectHitFlag.InWater)
						bubbleNumber = 16;

				}



				if (!killed)
				{
					if ((ohf1 == ObjectHitFlag.NotHit || ohf1 == ObjectHitFlag.InWater) && (ohf2 == ObjectHitFlag.NotHit || ohf2 == ObjectHitFlag.InWater))
						spdy += 0.1f;



					if (ohf1 == ObjectHitFlag.Hit || ohf2 == ObjectHitFlag.Hit)
					{
						nowY = nowY - 1;
						spdy = 0;
						jumped = false;
						jumping = false;
					}





					if (ohf3 == ObjectHitFlag.Hit || ohf5 == ObjectHitFlag.Hit)
					{
						nowX = (int)(nowX + 1);
						spdx = 0;
						this.nowImageNumber = 0;
					}

					if (ohf4 == ObjectHitFlag.Hit || ohf6 == ObjectHitFlag.Hit)
					{
						nowX = (int)(nowX - 1);
						spdx = 0;
						this.nowImageNumber = 0;
					}

					if (ohf7 == ObjectHitFlag.Hit || ohf8 == ObjectHitFlag.Hit)
					{
						nowY = nowY + 1;
						spdy = 0;
					}

					Rectangle jibun = new Rectangle(this.Point, this.Size);


					//プレイヤーを殺す判定
					if (nowstatus != 3)
						foreach (Sprite sp in Parent)
						{
							//プレイヤーの弾
							if (sp.MyGroup == SpriteGroup.DefenderWeapon && CheckHitJudge(jibun, new Rectangle(sp.Point, sp.Size)))
							{
								this.life -= 1;
								tick = 0;
								if (life < 1)
									this.killed = true;
								nowstatus = 3;
							}
							if (!(sp.MyGroup == SpriteGroup.Defender))
								continue;
							if (sp.killed)
								continue;
							if (CheckHitJudge(new Rectangle(sp.Point.X, sp.Point.Y + sp.Size.Height / 2, sp.Size.Width, sp.Size.Height / 2), new Rectangle(this.Point.X, this.Point.Y + 32, this.Size.Width, this.Size.Height / 2)))
							{
								sp.killed = true;
							}
							else if (CheckHitJudge(new Rectangle(sp.Point.X, sp.Point.Y + sp.Size.Height / 2, sp.Size.Width, sp.Size.Height / 2), new Rectangle(this.Point.X, this.Point.Y + 16, this.Size.Width, this.Size.Height / 4)))
							{
								if (ks.inz == 1)
									sp.spdy -= 5;
								else
									sp.spdy -= 3;
								this.life -= 1;
								nowstatus = 3;
								tick = 0;
								if (life < 1)
									this.killed = true;
							}
						}


					switch (nowstatus)
					{
						case 0:
							ImageHandle = GetImage(isRight ? normalright : normalleft);
							if (tick > 160)
							{
								nowstatus = 1;
								tick = 0;
								for (int i = 0; i < 360; i += 30)
									Parent.Add(new LeafGun(new Point(Point.X + 32, Point.Y + 32), new Size(16, 16), mptobjects, Parent, leafhndl, i / 180.0 * Math.PI));
								//Console.WriteLine(nowstatus);
							}
							break;
						case 1:
							ImageHandle = GetImage(isRight ? normalright : normalleft);
							if (tick > 120)
							{
								nowstatus = 2;
								tick = 0;
								if (!isRight)
									targetX = (int)nowX - 256;
								else
									targetX = (int)nowX + 256;
								spdx = 1;
								//Console.WriteLine(nowstatus);
								break;
							}
							if (gunflag && tick % (100 / (4 - life)) == 0)
								for (int i = 15; i < 367; i += 30)
									Parent.Add(new LeafGun(new Point(Point.X + 32, Point.Y + 32), new Size(16, 16), mptobjects, Parent, leafhndl, i / 180.0 * Math.PI));
							break;
						case 2:
							ImageHandle = GetImage(isRight ? normalright : normalleft);
							if (!isRight)
							{
								if (targetX >= nowX)
								{
									isRight = true;
									nowstatus = 0;
									tick = 0;
									spdx = 0;
									nowX = targetX;
								//Console.WriteLine(nowstatus);
									break;
								}
								spdx = spdx + 0.05f;
								nowX -= spdx;
							}
							else
							{
								if (targetX <=  nowX)
								{
									isRight = false;
									nowstatus = 0;
									tick = 0;
									spdx = 0;
									nowX = targetX;
								//	Console.WriteLine(nowstatus);
									gunflag = !gunflag;
									break;
								}
								spdx = spdx + 0.05f;
								nowX += spdx;
							}
							break;
						case 3:
							ImageHandle = GetImage(isRight ? hurtright : hurtleft);
							if (tick > 120)
							{
								tick = 118;
								nowstatus = 1;
								break;
							}
							break;
					}

					tick++;
					

					bohf1 = ohf1;
					bohf2 = ohf2;

				}


				/*
				if (nowY + me.Size.Height > jimeny)
				{
					nowY = jimeny - me.Size.Height;
					spdy = 0;
					jumped = false;
					jumping = false;
				}
				
				*/



				if (!killed)
					this.Point = new Point((int)nowX, (int)nowY);


			}

			if (killed)
			{
				Killing(killedtick);
				if (killedMaxTick < killedtick)
				{
					isDead = true;
				}
				killedtick += 5;
			}
			base.Draw(x, y, ref ks, chips);
		}

		int[] GetImage(params int[] datas)
		{
			int[] lastdata = new int[datas.Length];
			int i = 0;
			foreach (int data in datas)
			{
				lastdata[i] = this.datas[data];
				i++;
			}
			return lastdata;
		}
	}

	public class LeafGun : Sprite
	{
		public bool jumping = false;
		public bool jumped = false;
		public int jumpcnt = 0;
		public double[] Sin = null;
		double rad = 0;
		ObjectHitFlag bohf1 = ObjectHitFlag.NotHit;
		//bool isRight = false;
		//bool killed = false;
		int killedtick = 0;
		int bubbleNumber = 16;

		bool crushed = false;

		int[] datas;

		public override SpriteGroup MyGroup
		{
			get
			{
				return SpriteGroup.MonsterWeapon;
			}

		}

		public LeafGun(Point p, Size s, Object[] objs, SpriteCollection par, int[] datas, double rad)
			: base(p, s, new[] { 0 }, false, 0, 0, objs, par)
		{

			this.datas = datas;
			this.KilledImageHandle = null;
			//Sin = new double[360];
			//for (int i = 0; i < 360; i++)
			//	Sin[i] = Math.Sin(i / 180.0 * Math.PI) * 64;
			this.spdx = (float)Math.Cos(rad) * 2;
			this.spdy = (float)Math.Sin(rad) * 2;
			ImageHandle = GetImage(new[] { 2 });
		}



		public override int killedMaxTick
		{
			get { return 240; }
		}

		int killedy = -16;

		public override void Killing(int tick)
		{
			if (tick > 135)
				isDead = true;
		}
		int tick = 0;
		public override void Draw(int x, int y, ref States ks, byte[, ,] chips)
		{
			int w, h;
			DX.GetWindowSize(out w, out h);

			if (Math.Abs(Parent.MainSprite.Point.X - this.nowX) > w)
				return;

			if (nowX < 0)
			{
				//spdx = 1;
			}

			if (nowX > ks.map.Width * 16 - 16)
			{
				//spdx = -1;
				//nowX = ks.map.Width * 16 - 16;
			}

			if (nowY < 0)
			{
				//spdy = 0;
				//nowY = 0;
			}
			if (nowY > ks.map.Height * 16)
			{
				killed = true;
				isFall = true;
			}


			if (!killed)
			{

				if (!killed)
				{
					//ディフェンダーを殺す判定
					foreach (Sprite sp in Parent)
					{
						if (!(sp.MyGroup == SpriteGroup.Defender))
							continue;
						if (sp == this)
							continue;
						if (sp.killed)
							continue;
						if (CheckHitJudge(new Rectangle(sp.Point, sp.Size), new Rectangle(new Point(this.Point.X + 4, this.Point.Y + 4), new Size(4,4))))
						{
							isDead = true;
							sp.killed = true;
						}
					}

					if (tick > 240)
						isDead = true;


					nowX += spdx;
					nowY += spdy;
					tick++;

				}


				/*
				if (nowY + me.Size.Height > jimeny)
				{
					nowY = jimeny - me.Size.Height;
					spdy = 0;
					jumped = false;
					jumping = false;
				}
				
				*/



				if (!killed)
					this.Point = new Point((int)nowX, (int)nowY);


			}

			if (killed)
				Killing(killedtick++);
			//base.Draw(x, y, ref ks, chips);

			//DX.DrawGraph(Point.X + x, Point.Y + y, ImageHandle[nowImageNumber], 1);

			DX.DrawRotaGraph2(Point.X + x, Point.Y + y, 8, 8, 1, ((spdx != 0 && spdy != 0) ? (rad = Math.Atan2(spdy, spdx)) + Math.PI : rad + Math.PI), ImageHandle[0], 1);

		}

		int[] GetImage(int[] datas)
		{
			int[] lastdata = new int[datas.Length];
			int i = 0;
			foreach (int data in datas)
			{
				lastdata[i] = this.datas[data];
				i++;
			}
			return lastdata;
		}

		bool CheckHitJudgeEx(int x, int y, int w, int h, int r)
		{
			var s = Math.Sin(-r);
			var c = Math.Cos(-r);
			var xx = Math.Abs(x * c - y * s);
			var yy = Math.Abs(y * s - y * c);

			if (xx < w / 2.0 && yy < h / 2.0)
				return true;

			return false;
		}

	}

}