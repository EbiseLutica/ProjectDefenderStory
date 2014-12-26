using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DxLibDLL;
using System.Drawing;


namespace DefenderStory
{
	class Program
	{
		static void Main(string[] args)
		{

			DX.SetUseGraphAlphaChannel(1);
			DX.ChangeWindowMode(1);
			string mptname = "mpt2";
			//DX.SetWaitVSyncFlag(1);
			DX.SetFontSize(14);
			DX.SetFontThickness(1);
			DX.SetWindowText("Defender's Story");
			
			//DX.SetAlwaysRunFlag(1);

			Size scrSize = new Size(320,240);
			
			if (DX.DxLib_Init() == -1)
			{
				ShowError("DirectX の初期化に失敗しました。");
				return;
			}
			

			
			if (DX.SetGraphMode(scrSize.Width, scrSize.Height, 32, 60) == -1)
			{
				ShowError("画面サイズの変更に失敗しました。");
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



			//int hndl_bigplayer_walkleft = DX.CreateGraphFromRectSoftImage(hndl_playerchip, 0, 0, 16, 32);

			int[] hndl_bigplayer_datas = new int[72];

			if (DX.LoadDivGraph("Resources\\Graphics\\player_chip.png", 72, 18, 4, 16, 32, out hndl_bigplayer_datas[0]) == -1)
			{
				throw new Exception("プレイヤーキャラの読み込みに失敗しました。");
			}

			int[] hndl_commonMob_datas = new int[144];
			
			if (DX.LoadDivGraph("Resources\\Graphics\\commonMob.png", 144, 16, 4, 16, 16, out hndl_commonMob_datas[0]) == -1)
			{
				throw new Exception("プレイヤーキャラの読み込みに失敗しました。");
			}

			int[] hndl_spmodokee_datas = new int[8];

			if (DX.LoadDivGraph("Resources\\Graphics\\spModokee.png", 8, 8, 1, 32, 16, out hndl_spmodokee_datas[0]) == -1)
			{
				throw new Exception("リソースの読み込みに失敗しました。");
			}

			//init:

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
			byte[,,] chips = new byte[map.Width, map.Height,2];
			

			for (int y = map.Height - 3; y < map.Height; y++)
				for (int x = 0; x < map.Width; x++)
					chips[x, y, 0] = 18;
			
			
			top:
			int f = 0, fps = 0;
			int binz = 0;
			Point camera = new Point(0,0);

			SpriteCollection spcolle = new SpriteCollection();

			
			Player me = new Player(new Point(32, 32), new Size(16, 32), true, 8, 0, mptobjects, spcolle, hndl_bigplayer_datas);


			//Player me2 = new Player(new Point(64, 32), new Size(16, 32), true, 8, 0, mptobjects, spcolle, hndl_bigplayer_datas);

			//Player me3 = new Player(new Point(96, 32), new Size(16, 32), true, 8, 0, mptobjects, spcolle, hndl_bigplayer_datas);

			//Player me4 = new Player(new Point(128, 32), new Size(16, 32), true, 8, 0, mptobjects, spcolle, hndl_bigplayer_datas);

			//Bunyo hoge = new Bunyo(new Point(256,2), new Size(16,16), true, 8, 0, mptobjects, spcolle, hndl_commonMob_datas);

			Modokee_Ground modokee = new Modokee_Ground(new Point(256, 2), new Size(32, 16), true, 8, 0, mptobjects, spcolle, hndl_spmodokee_datas);

			spcolle.Add(me, true);
			spcolle.Add(modokee);
			//spcolle.Add(me2);
			//spcolle.Add(me3);
			//spcolle.Add(me4);

			//spcolle.Add(hoge);

			//for (int i = 0; i < 240; i++)
			//{
				DX.ClearDrawScreen();

				DX.DrawString(136, 96, "テスト", DX.GetColor(255, 255, 255));

				if (DX.ScreenFlip() == -1)
				{
					DX.DxLib_End();
					return;
				}
				//break;		//if you want to skip splash screen, comment out this line.
			//}



			while (true)
			{
				//DX.ClearDrawScreen();	//消し去る

				DX.DrawBox(0, 0, 640,480, 0x000020, 1);

				//DX.DrawBox(0, jimeny, 320, 240, Color.SaddleBrown.ToArgb(), 1);

				States ks = new States(binz, camera, map);
				int mousex = -1, mousey = -1;
				if (DX.GetMouseInput() == DX.MOUSE_INPUT_1)
					DX.GetMousePoint(out mousex, out mousey);
				

				if (ks.inz == 1 && binz == 1)
					ks.inz1 = 0;

				int inesc = DX.CheckHitKey(DX.KEY_INPUT_ESCAPE);

				/*
				if (inup == 1)
				{
					spdy -= spdAddition;
					if (spdy < -spdlimit)
						spdy = -spdlimit;

				}
				else if (indown == 1)
				{
					spdy += spdAddition;
					if (spdy > spdlimit)
						spdy = spdlimit;
				}
				else
				{
					spdy *= spddivition;
					if (spdy < 0.1f && spdy > -0.1f)
						spdy = 0;
				}
				*/

				

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

				if (spcolle.MainSprite.nowX + ks.camera.X > scrSize.Width / 2 && spcolle.MainSprite.spdx > 0 && ks.camera.X > -ks.map.Width * 16 + scrSize.Width)
				{
					ks.camera.Offset(-(int)spcolle.MainSprite.spdx, 0);
				}

				if (ks.map.Width * 16 - spcolle.MainSprite.nowX > scrSize.Width / 2 && spcolle.MainSprite.spdx < 0 && ks.camera.X < 0)
				{
					ks.camera.Offset(-(int)spcolle.MainSprite.spdx, 0);
				}

				if (spcolle.MainSprite.nowY + ks.camera.Y > scrSize.Height / 2 && spcolle.MainSprite.spdy > 0 && ks.camera.Y > -ks.map.Height * 16 + scrSize.Height)
				{
					ks.camera.Offset(0, -(int)spcolle.MainSprite.spdy);
				}

				if (ks.map.Height * 16 - spcolle.MainSprite.nowY > scrSize.Height / 2 && spcolle.MainSprite.spdy < 0 && ks.camera.Y < 0)
				{
					ks.camera.Offset(0, -(int)spcolle.MainSprite.spdy);
				}


				

				if (spcolle.MainSprite.isDead)
					goto top;

				if (ks.camera.X > 0)
					ks.camera = new Point(0, ks.camera.Y);

				if (ks.camera.Y > 0)
					ks.camera = new Point(ks.camera.X, 0);

				if (ks.camera.X < -ks.map.Width * 16 + scrSize.Width)
					ks.camera = new Point(-ks.map.Width * 16 + scrSize.Width, ks.camera.Y);

				if (ks.camera.Y < -ks.map.Height * 16 + scrSize.Height)
					ks.camera = new Point(ks.camera.X, -ks.map.Height * 16 + scrSize.Height);

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
				DX.DrawString(0, 0, string.Format(aho, camera.X/16, map.Width, camera.Y/16, map.Height, fps), DX.GetColor(255, 255, 255));

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

		public void Draw(ref States ks, byte[,,] chips)
		{
			foreach (Sprite item in this.Items)
			{
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
		public int[] ImageHandle { get; set; }
		public bool UseAnime { get; set; }
		public int LoopTimes { get; set; }
		public int AnimeSpeed { get; set; }
		public bool killed = false;
		public float spdAddition = 0.2f, spddivition = 0.8f, spdx = 0, spdy = 0, spdlimit = 2, nowX = 32, nowY = 32, bspdx = 0;
		public bool isRight = false;
		public bool isFall = false;

		public bool isDead = false;

		public SpriteCollection Parent { get; private set; }

		public Object[] mptobjects = null;

		public int EventID { get; set; }

		public int[] KilledImageHandle { get; set; }

		public abstract void Killing(int tick);

		public int nowImageNumber = 0;

		public abstract int killedMaxTick { get;}

		public int looptimes = 0;

		int bms = DX.GetNowCount();

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
		/// <param name="x"></param>
		/// <param name="y"></param>
		public virtual void Draw(int x, int y, ref States ks, byte[,,] chips)
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
			DX.DrawGraph(Point.X+x, Point.Y + y, ImageHandle[nowImageNumber], 1);

		}

		public void Draw(ref States ks, byte[,,] chips)
		{
			Draw(0, 0, ref ks, chips);
		}

		public static bool CheckHitJudge(Rectangle rect1, Rectangle rect2)
		{
			return (rect1.X < rect2.X + rect2.Width) && (rect2.X < rect1.X + rect1.Width) &&
				   (rect1.Y < rect2.Y + rect2.Height) && (rect2.Y < rect1.Y + rect1.Height);
		}
		

	}

	
	public class Player : Sprite
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

		public Player(Point p, Size s, bool useanime, int speed, int loop, Object[] objs, SpriteCollection par, int[] datas)
			: base(p, s, new[]{0}, useanime, speed, loop, objs, par)
		{

			this.datas = datas;
			this.KilledImageHandle = GetImage(bigdead);
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
				this.Point = new Point(Point.X, Point.Y + 1);
		}

		public override void Draw(int x, int y, ref States ks, byte[,,] chips)
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
				if (ks.inz1 == 1 && !jumped)
				{
					if (bohf1 == ObjectHitFlag.InWater && bohf2 == ObjectHitFlag.InWater)
					{
						spdy = -2.5f;
					}
					else
					{
						jumping = true;
						jumped = true;

						spdy = -3.5f - Math.Abs(spdx) / 10;
					}
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

				Point judge1 = new Point((int)nowX - 1, (int)nowY + this.Size.Height - 1);	//足判定1

				Point judge2 = new Point((int)nowX + this.Size.Width + 1, (int)nowY + this.Size.Height - 1);	//足判定2

				Point judge3 = new Point((int)nowX - 1, (int)nowY + this.Size.Height - 4);	//左障害物判定

				Point judge4 = new Point((int)nowX + this.Size.Width + 1, (int)nowY + this.Size.Height - 4);	//右障害物判定

				Point judge5 = new Point((int)nowX - 1, (int)nowY + this.Size.Height / 2 - 6);		//左障害物判定2

				Point judge6 = new Point((int)nowX + this.Size.Width + 1, (int)nowY + this.Size.Height / 2 - 6);	//右障害物判定2

				Point judge7 = new Point((int)nowX - 1, (int)nowY - 1);		//頭判定1

				Point judge8 = new Point((int)nowX + this.Size.Width + 1, (int)nowY - 1);	//頭判定2



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
						nowX = nowX - spdx;
						spdx = 0;
						this.nowImageNumber = 0;
					}

					if (ohf7 == ObjectHitFlag.Hit || ohf8 == ObjectHitFlag.Hit)
					{
						nowY = nowY - spdy;
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

							if (ohf1 == ObjectHitFlag.InWater && ohf2 == ObjectHitFlag.InWater)
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
							if (ohf1 == ObjectHitFlag.InWater && ohf2 == ObjectHitFlag.InWater)
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
							if (ohf1 == ObjectHitFlag.InWater && ohf2 == ObjectHitFlag.InWater)
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
							if (ohf1 == ObjectHitFlag.InWater && ohf2 == ObjectHitFlag.InWater)
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
							if (ohf1 == ObjectHitFlag.InWater && ohf2 == ObjectHitFlag.InWater)
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
							if (ohf1 == ObjectHitFlag.InWater && ohf2 == ObjectHitFlag.InWater)
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
							if (ohf1 == ObjectHitFlag.InWater && ohf2 == ObjectHitFlag.InWater)
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
							if (ohf1 == ObjectHitFlag.InWater && ohf2 == ObjectHitFlag.InWater)
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

				bohf1 = ohf1;
				bohf2 = ohf2;




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

		static int[] walkleft = {0, 1};
		static int[] walkright = { 4, 5 };
		static int[] dead = { 2 };
		static int[] stepped = { 3 };

		bool crushed = false;

		int[] datas;

		public Bunyo(Point p, Size s, bool useanime, int speed, int loop, Object[] objs, SpriteCollection par, int[] datas)
			: base(p, s, new[]{0}, useanime, speed, loop, objs, par)
		{

			this.datas = datas;
			this.KilledImageHandle = GetImage(dead);
			Sin = new double[360];
			for (int i = 0; i < 360; i++)
				Sin[i] = Math.Sin(i / 180.0 * Math.PI) * 128;
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
				this.Point = new Point(Point.X, Point.Y + 1);
		}

		public override void Draw(int x, int y, ref States ks, byte[,,] chips)
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

				Point judge1 = new Point((int)nowX - 1, (int)nowY + this.Size.Height - 1);	//足判定1

				Point judge2 = new Point((int)nowX + this.Size.Width + 1, (int)nowY + this.Size.Height - 1);	//足判定2

				Point judge3 = new Point((int)nowX - 1, (int)nowY + this.Size.Height - 4);	//左障害物判定

				Point judge4 = new Point((int)nowX + this.Size.Width + 1, (int)nowY + this.Size.Height - 4);	//右障害物判定

				Point judge5 = new Point((int)nowX - 1, (int)nowY + this.Size.Height / 2 - 6);		//左障害物判定2

				Point judge6 = new Point((int)nowX + this.Size.Width + 1, (int)nowY + this.Size.Height / 2 - 6);	//右障害物判定2

				Point judge7 = new Point((int)nowX - 1, (int)nowY - 1);		//頭判定1

				Point judge8 = new Point((int)nowX + this.Size.Width + 1, (int)nowY - 1);	//頭判定2


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
						if (!(sp is Player))
							continue;
						if (CheckHitJudge(new Rectangle(sp.Point.X, sp.Point.Y + sp.Size.Height / 2, sp.Size.Width, sp.Size.Height / 2), new Rectangle(this.Point.X, this.Point.Y + 8, this.Size.Width, this.Size.Height / 2)))
						{
							sp.killed = true;
						}
						if (CheckHitJudge(new Rectangle(sp.Point.X, sp.Point.Y + sp.Size.Height / 2, sp.Size.Width, sp.Size.Height / 2), new Rectangle(this.Point.X, this.Point.Y, this.Size.Width, this.Size.Height / 2)))
						{
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

				base.Draw(x, y, ref ks, chips);
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
	/// モドキーの移動フラグを表します。
	/// </summary>
	public enum ModokeeMoveFlag
	{
		/// <summary>
		/// サインカーブを描いて移動。
		/// </summary>
		SineCurve, 
		/// <summary>
		/// メインスプライトを追尾。
		/// </summary>
		SearchMainSprite, 
		/// <summary>
		/// 壁から道を探るため、上下移動
		/// </summary>
		UpDown
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
		static int[] dead =  { 2 };
		static int[] stepped = { 3 };

		int deg = 0;

		public ModokeeMoveFlag defaultFlag = ModokeeMoveFlag.SineCurve;

		bool crushed = false;

		public ModokeeMoveFlag flag;

		int[] datas;

		SpriteCollection par;

		public Modokee_Ground(Point p, Size s, bool useanime, int speed, int loop, Object[] objs, SpriteCollection par, int[] datas)
			: base(p, s, new[] { 0 }, useanime, speed, loop, objs, par)
		{
			this.par = par;
			this.datas = datas;
			this.KilledImageHandle = GetImage(dead);
			Sin = new double[360];
			for (int i = 0; i < 360; i++)
				Sin[i] = Math.Sin(i / 180.0 * Math.PI) * 128;
			spdx = -1;
			this.flag = defaultFlag;
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
				this.Point = new Point(Point.X, Point.Y + 1);
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
				isRight = true;
			}

			if (nowX > ks.map.Width * 16 - 16)
			{
				spdx = -1;
				isRight = false;
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


				Point judge1 = new Point((int)nowX - 1, (int)nowY + this.Size.Height - 1);	//足判定1

				Point judge2 = new Point((int)nowX + this.Size.Width + 1, (int)nowY + this.Size.Height - 1);	//足判定2

				Point judge3 = new Point((int)nowX - 1, (int)nowY + this.Size.Height - 4);	//左障害物判定

				Point judge4 = new Point((int)nowX + this.Size.Width + 1, (int)nowY + this.Size.Height - 4);	//右障害物判定

				Point judge5 = new Point((int)nowX - 1, (int)nowY + this.Size.Height / 2 - 6);		//左障害物判定2

				Point judge6 = new Point((int)nowX + this.Size.Width + 1, (int)nowY + this.Size.Height / 2 - 6);	//右障害物判定2

				Point judge7 = new Point((int)nowX - 1, (int)nowY - 1);		//頭判定1

				Point judge8 = new Point((int)nowX + this.Size.Width + 1, (int)nowY - 1);	//頭判定2



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

					switch (flag)
					{
						case ModokeeMoveFlag.SineCurve:
							spdy = (float)Math.Sin(deg / 180.0 * Math.PI) * 2;
							deg = (deg + 5) % 360;
							break;
						case ModokeeMoveFlag.SearchMainSprite:
							float hohe = (float)Math.Atan2(this.Point.Y - par.MainSprite.Point.Y, this.Point.X - par.MainSprite.Point.X);
							spdx = (float)Math.Cos(hohe);
							spdy = (float)Math.Sin(hohe);
							break;
					}

					//if ((ohf1 == ObjectHitFlag.NotHit || ohf1 == ObjectHitFlag.InWater) && (ohf2 == ObjectHitFlag.NotHit || ohf2 == ObjectHitFlag.InWater))
						//spdy += 0.1f;

					if (ohf1 == ObjectHitFlag.Hit || ohf2 == ObjectHitFlag.Hit)
					{
						nowY = nowY - spdy;
						if (this.flag == ModokeeMoveFlag.UpDown)
							spdy = -1;
						jumped = false;
						jumping = false;
					}




					//左にオブジェクトあるか
					if (ohf3 == ObjectHitFlag.Hit || ohf5 == ObjectHitFlag.Hit)
					{
						nowX = nowX - spdx;
						//spdx = 0;
						this.nowImageNumber = 0;
						if (flag == defaultFlag)
							spdy = -1;
						this.flag = ModokeeMoveFlag.UpDown;
					}
					else
					{
						if (!isRight)
							spdx = -1;
						else
							spdx = 1;
					}

					//右にオブジェクトあるか
					if (ohf4 == ObjectHitFlag.Hit || ohf6 == ObjectHitFlag.Hit)
					{
						nowX = nowX - spdx;
						//spdx = 0;
						this.nowImageNumber = 0;
						if (flag == defaultFlag)
							spdy = -1;
						this.flag = ModokeeMoveFlag.UpDown;
					}
					else
					{
						if (!isRight)
							spdx = -1;
						else
							spdx = 1;
						this.flag = ModokeeMoveFlag.SineCurve;
					}

					if (ohf7 == ObjectHitFlag.Hit || ohf8 == ObjectHitFlag.Hit)
					{
						nowY = nowY - spdy;
						if (this.flag == ModokeeMoveFlag.UpDown)
							spdy = 1;
					}

					Rectangle jibun = new Rectangle(this.Point, this.Size);


					//プレイヤーを殺す判定
					foreach (Sprite sp in Parent)
					{
						if (!(sp is Player))
							continue;
						if (CheckHitJudge(new Rectangle(sp.Point.X, sp.Point.Y + sp.Size.Height / 2, sp.Size.Width, sp.Size.Height / 2), new Rectangle(this.Point.X, this.Point.Y + 8, this.Size.Width, this.Size.Height / 2)))
						{
							sp.killed = true;
						}
						if (CheckHitJudge(new Rectangle(sp.Point.X, sp.Point.Y + sp.Size.Height / 2, sp.Size.Width, sp.Size.Height / 2), new Rectangle(this.Point.X, this.Point.Y, this.Size.Width, this.Size.Height / 2)))
						{
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

				base.Draw(x, y, ref ks, chips);
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
			this.defaultFlag = ModokeeMoveFlag.SearchMainSprite;
			this.flag = ModokeeMoveFlag.SearchMainSprite;
		}



	}

}