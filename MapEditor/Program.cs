using DxLibDLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DefenderStory.GUI;
using DefenderStory.Map;

namespace MapEditor
{
	class Program
	{


		[STAThread]
		static void Main(string[] args)
		{
			//---------------------------------------------------------

			Application.EnableVisualStyles();	//Visual Style を適用
			DX.SetUseGraphAlphaChannel(1);
			DX.ChangeWindowMode(1);
			string mptname = "mpt1";
			DX.SetFontSize(14);
			DX.SetFontThickness(1);
			DX.SetWindowText("Defender's Editor");
			byte[, ,] chips = null;
			DX.SetAlwaysRunFlag(1);
			Size scrSize = new Size(320, 240);
			DX.SetWindowVisibleFlag(0);
			DX.SetWaitVSyncFlag(1);
			//--------------------------------------------------------

			if (DX.DxLib_Init() == -1)			//初期化、失敗したら落ちる
			{
				ShowError("DirectX の初期化に失敗しました。");
				return;
			}

			//--------------------------------------------------------

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
			//--------------------------------------------------------
			//DX.SetMouseDispFlag(0);

			int bsec = DateTime.Now.Second;
			int bmsec = DateTime.Now.Millisecond;

			ObjectSelectForm osf = new ObjectSelectForm("Resources\\Graphics\\" + mptname + ".png");
			osf.Show();

			//int hndl_bigplayer_walkleft = DX.CreateGraphFromRectSoftImage(hndl_playerchip, 0, 0, 16, 32);

			int[] hndl_bigplayer_datas = new int[72];

		init:

			

			osf.request = RequestFlag.None;
			int[] hndl_mpt = new int[64];
			DX.LoadDivGraph("Resources\\Graphics\\" + mptname + ".png", 64, 16, 4, 16, 16, out hndl_mpt[0]);

			osf.StatusMessage = "マップチップを読み込みました。";

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

			osf.StatusMessage = "当たり判定データを設定しました。";

			DX.DeleteSoftImage(hndl_mptsoft);

			Size map = osf.MapSize;

			if (chips != null)
			{
				byte[, ,] tmp = (byte[, ,])(chips.Clone());
				chips = new byte[osf.MapSize.Width, osf.MapSize.Height, 2];
				for (int z = 0; z < tmp.GetLength(2); z++)
				{
					for (int y = 0; y < tmp.GetLength(1); y++)
					{
						if (y >= osf.MapSize.Height)
							break;
						for (int x = 0; x < tmp.GetLength(0); x++)
						{
							if (x >= osf.MapSize.Width)
								break;
							chips[x, y, z] = tmp[x, y, z];
						}
					}
				}
				osf.StatusMessage = "マップデータを読み込みました。";
			}
			else
			{
				chips = new byte[osf.MapSize.Width, osf.MapSize.Height, 2];
				osf.StatusMessage = "マップデータを初期化しました。";
			}


			int f = 0, fps = 0;
			int binz = 0;
			Point camera = new Point(0, 0);
			int btime = DX.GetNowCount();

			int thx = 0, thy = 0, bmf = 0;

			int bmousex = 0, bmousey = 0;

			bool editing = false;
			int spselidx = 0;
			Point relativepoint = Point.Empty;
			List<Entity> splist = new List<Entity>();

			osf.StatusMessage = "準備が出来ました。";
			DX.SetWindowVisibleFlag(1);
			
			while (true)
			{
				//DX.ProcessMessage();
				Color c = osf.RequestedColor;
				DX.DrawBox(0, 0, 320, 240, DX.GetColor(c.R, c.G, c.B), 1);
				DX.DrawString(0, 0, fps.ToString(), DX.GetColor(255, 255, 255));
				ChipPack pack = osf.chippack;
				int zoom = osf.Zoom;

				int wari = (int)(16 * (zoom / 100.0));

				if (osf.request != RequestFlag.None)
				{
					if (osf.request != RequestFlag.SaveCitMap && osf.request != RequestFlag.SaveSpdata)
					{
						if (editing)
							switch (MessageBox.Show("ファイルが変更されています。保存しますか？", "確認", MessageBoxButtons.YesNoCancel))
							{
								case DialogResult.Yes:
									SaveFileDialog sfd = new SaveFileDialog();
									sfd.Filter = "Defender Story マップファイル (*.citmap) | *.citmap";
									sfd.DefaultExt = "citmap";
									if (sfd.ShowDialog() == DialogResult.Cancel)
										break;
									MapUtility.SaveMap(chips, sfd.FileName);
									break;
								case DialogResult.Cancel:
									osf.request = RequestFlag.None;
									osf.StatusMessage = "キャンセルしました。";
									goto nuke;
							}
					}
					if (osf.request == RequestFlag.ChangeMpt || osf.request == RequestFlag.Resize)
					{
						if (!System.IO.File.Exists("Resources\\Graphics\\" + osf.Path + ".png"))
						{
							if (osf.request == RequestFlag.ChangeMpt)
								MessageBox.Show("指定したマップチップが見つかりません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
						}
						else
						{
							mptname = osf.Path;


							goto init;
						}
					}
					else if (osf.request == RequestFlag.CreateNew)
					{
						chips = null;
						goto init;
					}
					else if (osf.request == RequestFlag.ChangeColor)
					{
						goto nuke;
					}
					else if (osf.request == RequestFlag.OpenFile)
					{
						OpenFileDialog ofd = new OpenFileDialog();
						ofd.Filter = "Defender Story マップファイル (*.citmap) | *.citmap";
						ofd.DefaultExt = "citmap";
						if (ofd.ShowDialog() == DialogResult.Cancel)
						{
							osf.StatusMessage = "キャンセルしました。";
							osf.request = RequestFlag.None;
							goto nuke;
						}
						try
						{
							MapUtility.LoadMap(out chips, ofd.FileName);
						}
						catch (Exception ex)
						{
							MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
						}
						osf.MapSize = new Size(chips.GetLength(0), chips.GetLength(1));
						map = osf.MapSize;
						osf.StatusMessage = "マップデータを読み込みました。";

					}
					else if (osf.request == RequestFlag.SaveCitMap)
					{
						SaveFileDialog sfd = new SaveFileDialog();
						sfd.Filter = "Defender Story マップファイル (*.citmap) | *.citmap";
						sfd.DefaultExt = "citmap";
						if (sfd.ShowDialog() == DialogResult.Cancel)
						{
							osf.StatusMessage = "キャンセルしました。";
							osf.request = RequestFlag.None;
							goto nuke;
						}
						MapUtility.SaveMap(chips, sfd.FileName);
						osf.StatusMessage = "マップデータを保存しました。";
					}
					else if (osf.request == RequestFlag.SaveSpdata)
					{
						SaveFileDialog sfd = new SaveFileDialog();
						sfd.Filter = "json ドキュメント (*.json) | *.json";
						sfd.DefaultExt = "json";
						if (sfd.ShowDialog() == DialogResult.Cancel)
						{
							osf.StatusMessage = "キャンセルしました。";
							osf.request = RequestFlag.None;
							goto nuke;
						}
						SpdataUtility.Save(splist, sfd.FileName);
						osf.StatusMessage = "スプライトデータを保存しました。";
					}
					else if (osf.request == RequestFlag.SwapT)
					{
						MapSwap(chips, map, 0, -1);
					}
					else if (osf.request == RequestFlag.SwapB)
					{
						MapSwap(chips, map, 0, 1);
					}
					else if (osf.request == RequestFlag.SwapL)
					{
						MapSwap(chips, map, -1, 0);
					}
					else if (osf.request == RequestFlag.SwapR)
					{
						MapSwap(chips, map, 1, 0);
					}

					editing = false;

				}

			nuke:
				osf.request = RequestFlag.None;
				States ks = new States(binz, camera, map);
				int mousex = -1, mousey = -1;
				int mouseflag = DX.GetMouseInput();
				DX.GetMousePoint(out mousex, out mousey);


				if (ks.inz == 1 && binz == 1)
					ks.inz1 = 0;

				int inesc = DX.CheckHitKey(DX.KEY_INPUT_ESCAPE);

				if (DX.CheckHitKey(DX.KEY_INPUT_T) == 1)
					osf.Activate();

				try
				{
					if (mouseflag != 0 && DX.GetWindowActiveFlag() == 1)
					{
						if (mouseflag == DX.MOUSE_INPUT_1)
						{
							switch (osf.Tool)
							{
								case ToolFlag.Pen:
									pack.PutChipPackToArray(ref chips, (mousex - camera.X) / wari, (mousey - camera.Y) / wari, osf.sf);
									break;
								case ToolFlag.Line:
									if (bmf == 0)
									{
										thx = (mousex - camera.X) / wari;
										thy = (mousey - camera.Y) / wari;
									}
									DX.DrawLine(thx * wari + camera.X + wari / 2, thy * wari + wari / 2 + camera.Y, (mousex - camera.X) / wari * wari + camera.X + wari / 2, (mousey - camera.Y) / wari * wari + camera.Y + wari / 2, DX.GetColor(DX.GetRand(256), DX.GetRand(256), DX.GetRand(256)));

									break;
								case ToolFlag.Select:
									if (bmf == 0)
									{
										thx = (mousex - camera.X) / wari;
										thy = (mousey - camera.Y) / wari;
									}
									DX.DrawBox(thx * wari + camera.X + wari / 2, thy * wari + wari / 2 + camera.Y, (mousex - camera.X) / wari * wari + camera.X + wari / 2, (mousey - camera.Y) / wari * wari + camera.Y + wari / 2, DX.GetColor(DX.GetRand(256), DX.GetRand(256), DX.GetRand(256)), 0);

									break;
								case ToolFlag.Fill:
									if (bmf == 0)
									{
										thx = (mousex - camera.X) / wari;
										thy = (mousey - camera.Y) / wari;
									}
									DX.DrawBox(thx * wari + camera.X + wari / 2, thy * wari + wari / 2 + camera.Y, (mousex - camera.X) / wari * wari + camera.X + wari / 2, (mousey - camera.Y) / wari * wari + camera.Y + wari / 2, DX.GetColor(DX.GetRand(256), DX.GetRand(256), DX.GetRand(256)), 0);

									break;
								
								case ToolFlag.SpSel:
									if (bmf == 0)
									{
										spselidx = splist.FindIndex(new Predicate<Entity>((sp) => new Rectangle((int)(sp.PosX * (1 + (100 - zoom) / 100.0)), (int)(sp.PosY * (1 + (100 - zoom) / 100.0)), wari, wari).Contains((int)((mousex - camera.X) * (1 + (100 - zoom) / 100.0)), (int)((mousey - camera.Y) * (1 + (100 - zoom) / 100.0)))));
										if (spselidx == -1)
											break;
										relativepoint = new Point((int)((mousex - camera.X - splist[spselidx].PosX) * (1 + (100 - zoom) / 100.0)), (int)((mousey - camera.Y - splist[spselidx].PosY) * (1 + (100 - zoom) / 100.0)));
									}
									if (spselidx == -1)
										break;
									splist[spselidx].PosX = (int)((mousex - camera.X) * (1 + (100 - zoom) / 100.0) - relativepoint.X);
									splist[spselidx].PosY = (int)((mousey - camera.Y) * (1 + (100 - zoom) / 100.0) - relativepoint.Y);
									if (ks.inlshift == 1)
									{
										splist[spselidx].PosX = (int)Math.Round(splist[spselidx].PosX / 16.0) * 16;
										splist[spselidx].PosY = (int)Math.Round(splist[spselidx].PosY / 16.0) * 16;
									}
									break;
							}
							editing = true;
						}
						else
							osf.chipno = chips[(mousex - camera.X) / wari, (mousey - camera.Y) / wari, (int)osf.sf];

					}
					if (mouseflag == 0 && bmf != 0 && bmousex != -1 && bmousey != -1 && DX.GetWindowActiveFlag() == 1)
						switch (osf.Tool)
						{
							case ToolFlag.Line:
								DrawLineToArray(thx, thy, (bmousex - camera.X) / wari, (bmousey - camera.Y) / wari, ref chips, osf.sf, (byte)osf.chipno);
								break;
							case ToolFlag.Select:
								if (bmf == 0)
								{
									thx = (mousex - camera.X) / wari;
									thy = (mousey - camera.Y) / wari;
								}
								DX.DrawBox(thx * wari + wari / 2, thy * wari + wari / 2, (mousex - camera.X) / wari * wari + wari / 2, (mousey - camera.Y) / wari * wari + wari / 2, DX.GetColor(DX.GetRand(256), DX.GetRand(256), DX.GetRand(256)), 0);

								break;
							case ToolFlag.Fill:
								FillBoxToArray(thx, thy, (bmousex - camera.X) / wari, (bmousey - camera.Y) / wari, ref chips, osf.sf, pack);
								break;
							case ToolFlag.SpPut:
								Entity sp = new Entity();
								sp.PosX = (int)((mousex - camera.X) * (1 + (100 - zoom) / 100.0));
								sp.PosY = (int)((mousey - camera.Y) * (1 + (100 - zoom) / 100.0));
								if (ks.inlshift == 1)
								{
									sp.PosX = (int)Math.Round(sp.PosX / 16.0) * 16;
									sp.PosY = (int)Math.Round(sp.PosY / 16.0) * 16;
								}
								sp.EntityID = osf.EntityID;
								sp.Visible = true;
								splist.Add(sp);
								break;
							case ToolFlag.SpSel:
								osf.StatusMessage = (string)(osf.listBox1.Items[splist[spselidx].EntityID]);
								break;
							case ToolFlag.SpVisible:
								spselidx = splist.FindIndex(new Predicate<Entity>((spr) => new Rectangle((int)(spr.PosX * (1 + (100 - zoom) / 100.0)), (int)(spr.PosY * (1 + (100 - zoom) / 100.0)), wari, wari).Contains((int)((mousex - camera.X) * (1 + (100 - zoom) / 100.0)), (int)((mousey - camera.Y) * (1 + (100 - zoom) / 100.0)))));
								if (spselidx == -1)
									break;
								splist[spselidx].Visible = !splist[spselidx].Visible;
								break;
							case ToolFlag.SpDel:
									spselidx = splist.FindIndex(
										new Predicate<Entity>((spr) => 
											new Rectangle(
												(int)(spr.PosX * (1 + (100 - zoom) / 100.0)), 
												(int)(spr.PosY * (1 + (100 - zoom) / 100.0)), 
												wari, 
												wari).Contains(
												(int)((mousex - camera.X) * (1 + (100 - zoom) / 100.0)), 
												(int)((mousey - camera.Y) * (1 + (100 - zoom) / 100.0)))));
									if (spselidx == -1)
										break;
	
		
								splist.RemoveAt(spselidx);
								spselidx = 0;
								break;
						}
				}
				catch (Exception ex)
				{
					osf.StatusMessage = string.Format("例外 {0} : {1} : {2}", ex.GetType().Name, ex.Message, ex.TargetSite);
				}

				if (inesc == 1)
				{
					DX.DxLib_End();
					return;
				}

				if (ks.camera.X > 0)
					ks.camera = new Point(0, ks.camera.Y);

				if (ks.camera.Y > 0)
					ks.camera = new Point(ks.camera.X, 0);

				if (ks.camera.X < -ks.map.Width * wari + scrSize.Width)
					ks.camera = new Point(-ks.map.Width * wari + scrSize.Width, ks.camera.Y);

				if (ks.camera.Y < -ks.map.Height * wari + scrSize.Height)
					ks.camera = new Point(ks.camera.X, -ks.map.Height * wari + scrSize.Height);

				ks.camera.Offset(DX.GetMouseHWheelRotVol() * 4, DX.GetMouseWheelRotVol() * 4);

				if (ks.inup == 1)
					ks.camera.Offset(0, 4);

				if (ks.indown == 1)
					ks.camera.Offset(0, -4);

				if (ks.inleft == 1)
					ks.camera.Offset(4, 0);

				if (ks.inright == 1)
					ks.camera.Offset(-4, 0);

				if (osf.GridVisible)
				{
					for (int y = 0; y < map.Height * wari; y += wari)
						if (y > -camera.Y - 16 && y < -camera.Y + scrSize.Height)
							for (int x = 0; x < map.Width * wari; x += wari)
								if (x > -camera.X - 16 && x < -camera.X + scrSize.Width)
									DX.DrawBox(x + camera.X, y + camera.Y, x + camera.X + wari + 1, y + camera.Y + wari + 1, DX.GetColor(127, 127, 127), 0);
				}

				DX.DrawString(mousex + 16, mousey, string.Format("{0}, {1}", ((ks.inlshift == 1) ? ((mousex - camera.X) / wari * wari) : mousex - camera.X), ((ks.inlshift == 1) ? ((mousey - camera.Y) / wari * wari) : mousey - camera.Y)), DX.GetColor(255, 255, 255));

				if (osf.BackVisible)
					for (int y = 0; y < map.Height * wari; y += wari)
						if (y > -camera.Y - 16 && y < -camera.Y + scrSize.Height)
							for (int x = 0; x < map.Width * wari; x += wari)
								if (x > -camera.X - 16 && x < -camera.X + scrSize.Width)
									DX.DrawGraph(x + camera.X, y + camera.Y, hndl_mpt[chips[x / wari, y / wari, 1]], 1);

				if (osf.ForeVisible)
					for (int y = 0; y < map.Height * wari; y += wari)
						if (y > -camera.Y - 16 && y < -camera.Y + scrSize.Height)
							for (int x = 0; x < map.Width * wari; x += wari)
								if (x > -camera.X - 16 && x < -camera.X + scrSize.Width)
									DX.DrawGraph(x + camera.X, y + camera.Y, hndl_mpt[chips[x / wari, y / wari, 0]], 1);

				foreach (Entity sp in splist)
				{
					DX.DrawBox((int)(sp.PosX * (zoom / 100.0) + camera.X), (int)(sp.PosY * (zoom / 100.0) + camera.Y), (int)(sp.PosX * (zoom / 100.0) + camera.X) + wari, (int)(sp.PosY * (zoom / 100.0) + camera.Y) + wari, (spselidx != -1 && sp == splist[spselidx]) ? DX.GetColor(255, 0, 0) : DX.GetColor(255, 255, 255), 0);
					if (!sp.Visible)
						DX.DrawBox((int)(sp.PosX * (zoom / 100.0) + camera.X), (int)(sp.PosY * (zoom / 100.0) + camera.Y), (int)(sp.PosX * (zoom / 100.0) + camera.X) + wari, (int)(sp.PosY * (zoom / 100.0) + camera.Y) + wari, DX.GetColor(79, 79, 79), 0);
					 NumFont.DrawNumFontString((int)(sp.PosX * (zoom / 100.0) + camera.X) + 2, (int)(sp.PosY * (zoom / 100.0) + camera.Y) + 2, sp.Visible ? DX.GetColor(255, 255, 255) : DX.GetColor(79, 79, 79), sp.EntityID.ToString());
				}

				f++;
				if (bsec != DateTime.Now.Second)
				{
					fps = f;
					f = 0;
					bsec = DateTime.Now.Second;
				}


				if (DX.ScreenFlip() == -1)
				{
					DX.DxLib_End();
					return;
				}

				bmf = mouseflag;
				binz = ks.inz;
				bmousex = mousex; bmousey = mousey;
				camera = ks.camera;
				map = ks.map;

			}


		}

		private static void MapSwap(byte[,,] chips, Size size, int v1, int v2)
		{
			for (int y = 0; y < size.Height; y++)
			{
				for (int x = 0; x < size.Width; x++)
				{
					chips[x, y, 0] = (byte)((Math.Min(3, Math.Max(0, chips[x, y, 0] / 16 + v2)) * 16) + (Math.Max(0, Math.Min(chips[x, y, 0] % 16 + v1, 15)) % 16));
					chips[x, y, 1] = (byte)((Math.Min(3, Math.Max(0, chips[x, y, 1] / 16 + v2)) * 16) + (Math.Max(0, Math.Min(chips[x, y, 1] % 16 + v1, 15)) % 16));
				}
			}
		}


		static void dxform_MouseHover(object sender, EventArgs e)
		{
			((Form)sender).Activate();
		}

		public static DialogResult ShowError(string message)
		{
			DX.DxLib_End();
			return MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1);
		}

		public static void DrawLineToArray(int x1, int y1, int x2, int y2, ref byte[, ,] array, ScreenFlag dimension, byte value)
		{
			bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
			if (steep)
			{
				Swap(ref x1, ref y1);
				Swap(ref x2, ref y2);
			}
			if (x1 > x2)
			{
				Swap(ref x1, ref x2);
				Swap(ref y1, ref y2);
			}
			int deltax = x2 - x1;
			int deltay = Math.Abs(y2 - y1);
			int error = deltax / 2;
			int ystep;
			int y = y1;
			if (y1 < y2)
				ystep = 1;
			else
				ystep = -1;
			for (int x = x1; x <= x2; x++)
			{
				if (steep)
					array[y, x, (int)dimension] = value;
				else
					array[x, y, (int)dimension] = value;
				error = error - deltay;
				if (error < 0)
				{
					y = y + ystep;
					error = error + deltax;
				}

			}
		}

		public static void FillBoxToArray(int x1, int y1, int x2, int y2, ref byte[, ,] array, ScreenFlag dimension, ChipPack value)
		{


			if (y1 > y2)
				Swap(ref y1, ref y2);

			if (x1 > x2)
				Swap(ref x1, ref x2);


			for (int y = y1; y <= y2; y += value.height)
				for (int x = x1; x <= x2; x += value.width)
				{
					for (int px = 0; px < value.width; px++)
					{
						int nx = x + px;
						if (nx > x2)
							continue;
                        for (int py = 0; py < value.height; py++)
						{
							int ny = y + py;
							if (ny > y2)
								continue;
							array[nx, ny, (int)dimension] = value.chips[py * value.width + px];
						}
					}
				}
		}

		static void Swap<T>(ref T var1, ref T var2)
		{
			T tmp = var2;
			var2 = var1;
			var1 = tmp;
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


		public Object(int handle, byte[,] mask)
		{
			ImageHandle = handle;
			HitMask = mask;
		}


	}

	
	public static class SpdataUtility
	{
		public static void Save(List<Entity> array, string path)
		{
			StringBuilder sb = new StringBuilder();	//Json を書き込む StringBuilder
			sb.Append("[");
			foreach(Entity sp in array)
			{
				sb.AppendFormat("{{\"PosX\": {0}, \"PosY\": {1}, \"EntityID\": {2}, \"EntityData\": {3}}}, ", sp.PosX, sp.PosY, sp.EntityID, GetPropertyJson(sp.EntityID));
			}
			sb.Remove(sb.Length - 2, 2);	//最後の, を消去
			sb.Append("]");
			File.WriteAllText(path, sb.ToString());
			
		}


		static string GetPropertyJson(int spriteid)
		{
			switch (spriteid)
			{
				case 0:
					return "{\"SpawnFromDoor\": false, \"TargetDoorIndex\": 0}";
				case 5:
					return "{\"TargetEntity\": null}";
				case 7:	
					return "{\"StartX\": 0}";
				case 12:
					return "{\"TargetEntityIndex\": 0}";
				case 18:
					return "{\"NextStage\": 0}";
				case 22:
					return "{\"EntityType\": 0}";
				case 28:
					return "{\"WorkingType\": 0}";
				case 44:
					return "{\"StartX\": 0}";
				case 60:
					return "{\"NextArea\": 0}";
				case 61:
					return "{\"StartX\": 0}";
				case 62:
					return "{\"StartX\": 0}";
				case 63:
					return "{\"StartX\": 0}";
				case 83:
					return "{\"MessageText\": \"\"}";
				default:
					return "null";
			}
		}

	}

	public struct ChipPack
	{
		public int width, height;
		public byte[] chips;

		public ChipPack(int w, int h, params byte[] c)
		{
			width = w;
			height = h;
			chips = c;
		}

		public void PutChipPackToArray(ref byte[, ,] array, int x, int y, ScreenFlag dimension)
		{
			for (int ix = x; ix < x + width; ix++)
			{
				if (ix >= array.GetLength(0))
					break;
				for (int iy = y; iy < y + height; iy++)
				{
					if (iy >= array.GetLength(1))
						break					;
					array[ix, iy, (int)dimension] = chips[(iy - y) * width + (ix - x)];
				}
			}
		}
	}
}
