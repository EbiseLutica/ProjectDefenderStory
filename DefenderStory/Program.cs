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
			
			//DX.SetWaitVSyncFlag(1);
			DX.SetFontSize(14);
			DX.SetFontThickness(1);
			DX.SetWindowText("Defender Story");
			
			//DX.SetAlwaysRunFlag(1);

			Size scrSize = new Size(640, 480);
			
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

			int[] hndl_bigplayer_datas = new int[11];

			if (DX.LoadDivGraph("Resources\\Graphics\\player_chip.png", 11, 11, 1, 16, 32, out hndl_bigplayer_datas[0]) == -1)
			{
				throw new Exception("プレイヤーキャラの読み込みに失敗しました。");
			}


			int[] hndl_bigplayer_walkleft = {
												hndl_bigplayer_datas[0],
												hndl_bigplayer_datas[1],
												hndl_bigplayer_datas[2],
												hndl_bigplayer_datas[3]
											};

			int[] hndl_bigplayer_jumpleft = {
												hndl_bigplayer_datas[4]
											};

			int[] hndl_bigplayer_dead = {
											hndl_bigplayer_datas[5]
										};

			int[] hndl_bigplayer_walkright = {
												hndl_bigplayer_datas[6],
												hndl_bigplayer_datas[7],
												hndl_bigplayer_datas[8],
												hndl_bigplayer_datas[9]
											 };

			int[] hndl_bigplayer_jumpright = {
												 hndl_bigplayer_datas[10]
											 };

			


			int[] hndl_mpt = new int[64];
			DX.LoadDivGraph("Resources\\Graphics\\mpttest.png", 64, 16, 4, 16, 16, out hndl_mpt[0]);

			int hndl_mptsoft = DX.LoadSoftImage("Resources\\Graphics\\mpttest_hj.png");
			


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

			ObjectSelectForm osf = new ObjectSelectForm("Resources\\Graphics\\mpttest.png");
			osf.Show();

			Size map = new Size(80, 40);
			byte[,] chips = new byte[map.Width, map.Height];

			for (int y = map.Height - 3; y < map.Height; y++)
				for (int x = 0; x < map.Width; x++)
					chips[x, y] = 20;


			top:
			int f = 0, fps = 0;
			int binz = 0;
			Point camera = new Point(0,0);

			SpriteCollection spcolle = new SpriteCollection();

			
			Player me = new Player(new Point(32, 32), new Size(16, 32), hndl_bigplayer_walkright, true, 8, 0, mptobjects, hndl_bigplayer_walkleft, hndl_bigplayer_walkright, hndl_bigplayer_jumpleft, hndl_bigplayer_jumpright, hndl_bigplayer_dead, spcolle);
			
			Player me2 = new Player(new Point(64, 32), new Size(16, 32), hndl_bigplayer_walkright, true, 8, 0, mptobjects, hndl_bigplayer_walkleft, hndl_bigplayer_walkright, hndl_bigplayer_jumpleft, hndl_bigplayer_jumpright, hndl_bigplayer_dead, spcolle);

			Player me3 = new Player(new Point(96, 32), new Size(16, 32), hndl_bigplayer_walkright, true, 8, 0, mptobjects, hndl_bigplayer_walkleft, hndl_bigplayer_walkright, hndl_bigplayer_jumpleft, hndl_bigplayer_jumpright, hndl_bigplayer_dead, spcolle);

			Player me4 = new Player(new Point(128, 32), new Size(16, 32), hndl_bigplayer_walkright, true, 8, 0, mptobjects, hndl_bigplayer_walkleft, hndl_bigplayer_walkright, hndl_bigplayer_jumpleft, hndl_bigplayer_jumpright, hndl_bigplayer_dead, spcolle);

			

			spcolle.Add(me,true);
			spcolle.Add(me2);
			spcolle.Add(me3);
			spcolle.Add(me4);

			for (int i = 0; i < 240; i++)
			{
				DX.ClearDrawScreen();

				DX.DrawString(136, 96, "テスト", DX.GetColor(255, 255, 255));

				if (DX.ScreenFlip() == -1)
				{
					DX.DxLib_End();
					return;
				}
				break;		//if you want to skip splash screen, comment out this line.
			}



			while (true)
			{
				//DX.ClearDrawScreen();	//消し去る

				DX.DrawBox(0, 0, 640,480, Color.SkyBlue.ToArgb(), 1);

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
						chips[(mousex - camera.X) / 16, (mousey - camera.Y) / 16] = (byte)osf.chipno;
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


				spcolle.Draw(ref ks, chips);

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
						DX.DrawGraph(x + camera.X, y + camera.Y, hndl_mpt[chips[x / 16, y / 16]], 1);

				f++;
				if (bsec != DateTime.Now.Second)
				{
					fps = f;
					f = 0;
					bsec = DateTime.Now.Second;
				}

				

				DX.DrawString(0, 0, string.Format("camX:{0}/{1} camY:{2}/{3} fps: {4}", camera.X/16, map.Width, camera.Y/16, map.Height, fps), DX.GetColor(255, 255, 255));

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

		public void Draw(ref States ks, byte[,] chips)
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
		public virtual void Draw(int x, int y, ref States ks, byte[,] chips)
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

		public void Draw(ref States ks, byte[,] chips)
		{
			Draw(0, 0, ref ks, chips);
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

		int[] bwalkleft = null, bwalkright = null, bjumpleft = null, bjumpright = null, bdead = null;

		public Player(Point p, Size s, int[] handle, bool useanime, int speed, int loop, Object[] objs, int[] wl, int[] wr, int[] jl, int[] jr, int[] d, SpriteCollection par)
			: base(p, s, handle, useanime, speed, loop, objs, par)
		{
			bwalkleft = wl;
			bwalkright = wr;
			bjumpleft = jl;
			bjumpright = jr;
			bdead = d;
			this.KilledImageHandle = bdead;
			Sin = new double[360];
			for (int i = 0; i < 360; i++)
				Sin[i] = Math.Sin(i / 180.0 * Math.PI)*128;
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

		public override void Draw(int x, int y, ref States ks, byte[,] chips)
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

				if (spdx != 0 && !isRight)
				{
					if (jumping)
					{
						this.ImageHandle = bjumpleft;
						this.nowImageNumber = 0;
						this.UseAnime = false;
					}
					else
					{
						this.ImageHandle = bwalkleft;
						this.UseAnime = true;
					}
				}

				if (spdx != 0 && isRight)
				{
					if (jumping)
					{
						this.ImageHandle = bjumpright;
						this.nowImageNumber = 0;
						this.UseAnime = false;
					}
					else
					{
						this.ImageHandle = bwalkright;
						this.UseAnime = true;
					}
				}

				if (spdx == 0 && !isRight)
				{
					if (jumping)
					{
						this.ImageHandle = bjumpleft;
						this.nowImageNumber = 0;
						this.UseAnime = false;
					}
					else
					{
						this.nowImageNumber = 0;
						this.ImageHandle = bwalkleft;
						this.UseAnime = false;
					}
				}
				else if (spdx == 0 && isRight)
				{
					if (jumping)
					{
						this.ImageHandle = bjumpright;
						this.nowImageNumber = 0;
						this.UseAnime = false;
					}
					else
					{
						this.nowImageNumber = 0;
						this.ImageHandle = bwalkright;
						this.UseAnime = false;
					}
				}
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
				ohf1 = mptobjects[chips[judge1.X / 16, judge1.Y / 16]].CheckHit(judge1.X % 16, judge1.Y % 16);

				ohf2 = mptobjects[chips[judge2.X / 16, judge2.Y / 16]].CheckHit(judge2.X % 16, judge2.Y % 16);

				ohf3 = mptobjects[chips[judge3.X / 16, judge3.Y / 16]].CheckHit(judge3.X % 16, judge3.Y % 16);

				ohf4 = mptobjects[chips[judge4.X / 16, judge4.Y / 16]].CheckHit(judge4.X % 16, judge4.Y % 16);

				ohf5 = mptobjects[chips[judge5.X / 16, judge5.Y / 16]].CheckHit(judge5.X % 16, judge5.Y % 16);

				ohf6 = mptobjects[chips[judge6.X / 16, judge6.Y / 16]].CheckHit(judge6.X % 16, judge6.Y % 16);

				ohf7 = mptobjects[chips[judge7.X / 16, judge7.Y / 16]].CheckHit(judge7.X % 16, judge7.Y % 16);

				ohf8 = mptobjects[chips[judge8.X / 16, judge8.Y / 16]].CheckHit(judge8.X % 16, judge8.Y % 16);


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
				if (ks.inz == 0 && jumping && jumpcnt < 90)
				{
					spdy += 0.1f;
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

			base.Draw(x, y, ref ks, chips);
		}

	}

}