using DxLibDLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TakeUpJewel.GUI;
using TakeUpJewel.Map;
using TakeUpJewel.Util;
using TakeUpJewel;
using TakeUpJewel.Entities;
using Codeplex.Data;

namespace MapEditor
{
	class Program
	{


		[STAThread]
		static void Main()
		{
			//---------------------------------------------------------

			Application.EnableVisualStyles();	//Visual Style を適用
			DX.SetUseGraphAlphaChannel(1);
			DX.ChangeWindowMode(1);
			var mptname = "mpt1";
			DX.SetFontSize(14);
			DX.SetFontThickness(1);
			DX.SetWindowText("Defender's Editor");
			byte[, ,] chips = null;
			DX.SetAlwaysRunFlag(1);
			var scrSize = new Size(320, 240);
			DX.SetWindowVisibleFlag(0);
			DX.SetWaitVSyncFlag(1);

			var osf = new ObjectSelectForm("Resources\\Graphics\\" + mptname + ".png");
			osf.Show();

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

			FontUtility.Init();
			SoundUtility.Init();
			ResourceUtility.Init();

			var splist = new List<Entity>();
			splist.Add(new Entity
			{
				PosX = 0,
				PosY = 0,
				EntityId = 0,
				EntityData = SpdataUtility.GetPropertyObject(0)
			});
			//--------------------------------------------------------
			//DX.SetMouseDispFlag(0);

			var bsec = DateTime.Now.Second;
			var bmsec = DateTime.Now.Millisecond;

			

			//int hndl_bigplayer_walkleft = DX.CreateGraphFromRectSoftImage(hndl_playerchip, 0, 0, 16, 32);

			var hndlBigplayerDatas = new int[72];

		init:

			

			osf.Request = RequestFlag.None;
			var hndlMpt = new int[64];
			DX.LoadDivGraph("Resources\\Graphics\\" + mptname + ".png", 64, 16, 4, 16, 16, out hndlMpt[0]);

			osf.StatusMessage = "マップチップを読み込みました。";

			var hndlMptsoft = DX.LoadSoftImage("Resources\\Graphics\\" + mptname + "_hj.png");



			var mptobjects = new Object[64];

			int r, g, b, a;
			var hits = new Color[5];
			for (var i = 0; i < 5; i++)
			{
				DX.GetPixelSoftImage(hndlMptsoft, i, 64, out r, out g, out b, out a);
				hits[i] = Color.FromArgb(r, g, b, a);
			}

			var hitlist = hits.ToList();

			for (var iy = 0; iy < 4; iy++)
			{
				for (var ix = 0; ix < 16; ix++)
				{
					var mask = new byte[16, 16];
					for (var y = 0; y < 16; y++)
						for (var x = 0; x < 16; x++)
						{
							DX.GetPixelSoftImage(hndlMptsoft, x + ix * 16, y + iy * 16, out r, out g, out b, out a);
							mask[x, y] = (byte)hitlist.IndexOf(Color.FromArgb(r, g, b, a));
						}
					mptobjects[iy * 16 + ix] = new Object(hndlMpt[iy * 16 + ix], mask);
				}
			}

			osf.StatusMessage = "当たり判定データを設定しました。";

			DX.DeleteSoftImage(hndlMptsoft);

			var map = osf.MapSize;

			if (chips != null)
			{
				var tmp = (byte[, ,])(chips.Clone());
				chips = new byte[osf.MapSize.Width, osf.MapSize.Height, 2];
				for (var z = 0; z < tmp.GetLength(2); z++)
				{
					for (var y = 0; y < tmp.GetLength(1); y++)
					{
						if (y >= osf.MapSize.Height)
							break;
						for (var x = 0; x < tmp.GetLength(0); x++)
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
			var binz = 0;
			var camera = new Point(0, 0);
			var btime = DX.GetNowCount();

			int thx = 0, thy = 0, bmf = 0;

			int bmousex = 0, bmousey = 0;

			var editing = false;
			var spselidx = 0;
			var relativepoint = Point.Empty;

			osf.StatusMessage = "準備が出来ました。";
			DX.SetWindowVisibleFlag(1);
			
			while (true)
			{
				DX.ProcessMessage();
				Application.DoEvents();
				var c = osf.RequestedColor;
				DX.DrawBox(0, 0, 320, 240, DX.GetColor(c.R, c.G, c.B), 1);
				DX.DrawString(0, 0, fps.ToString(), DX.GetColor(255, 255, 255));
				var pack = osf.Chippack;
				var zoom = osf.Zoom;

				var wari = (int)(16 * (zoom / 100.0));

				if (osf.Request != RequestFlag.None)
				{
					if (osf.Request == RequestFlag.CreateNew ||
						osf.Request == RequestFlag.OpenCitMap
						)
					{
						if (editing)
							switch (MessageBox.Show("ファイルが変更されています。保存しますか？", "確認", MessageBoxButtons.YesNoCancel))
							{
								case DialogResult.Yes:
									var sfd = new SaveFileDialog();
									sfd.Filter = "Defender Story マップファイル (*.citmap) | *.citmap";
									sfd.DefaultExt = "citmap";
									if (sfd.ShowDialog() == DialogResult.Cancel)
										break;
									MapUtility.SaveMap(chips, sfd.FileName);
									break;
								case DialogResult.Cancel:
									osf.Request = RequestFlag.None;
									osf.StatusMessage = "キャンセルしました。";
									goto nuke;
							}
					}

					

					if (osf.Request == RequestFlag.ChangeMpt || osf.Request == RequestFlag.Resize)
					{
						if (!File.Exists("Resources\\Graphics\\" + osf.Path + ".png"))
						{
							if (osf.Request == RequestFlag.ChangeMpt)
								MessageBox.Show("指定したマップチップが見つかりません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
						}
						else
						{
							mptname = osf.Path;


							goto init;
						}
					}
					else if (osf.Request == RequestFlag.CreateNew)
					{
						chips = null;
						splist.Clear();
						splist.Add(new Entity
						{
							PosX = 0,
							PosY = 0,
							EntityId = 0,
							EntityData = SpdataUtility.GetPropertyObject(0)
						});

						goto init;
					}
					else if (osf.Request == RequestFlag.ChangeColor)
					{
						goto nuke;
					}
					else if (osf.Request == RequestFlag.OpenCitMap)
					{
						var ofd = new OpenFileDialog();
						ofd.Filter = "Defender Story マップファイル (*.citmap) | *.citmap";
						ofd.DefaultExt = "citmap";
						if (ofd.ShowDialog() == DialogResult.Cancel)
						{
							osf.StatusMessage = "キャンセルしました。";
							osf.Request = RequestFlag.None;
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
					else if (osf.Request == RequestFlag.OpenSpdata)
					{
						var ofd = new OpenFileDialog();
						ofd.Filter = "json ドキュメント (*.json) | *.json";
						ofd.DefaultExt = "json";
						if (ofd.ShowDialog() == DialogResult.Cancel)
						{
							osf.StatusMessage = "キャンセルしました。";
							osf.Request = RequestFlag.None;
							goto nuke;
						}
						//try
						//{
							SpdataUtility.Load(out splist, ofd.FileName);
						//}
						//catch (Exception ex)
						//{
						//	MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
						//}
						osf.MapSize = new Size(chips.GetLength(0), chips.GetLength(1));
						map = osf.MapSize;
						osf.StatusMessage = "エンティティデータを読み込みました。";

					}
					else if (osf.Request == RequestFlag.SaveCitMap)
					{
						var sfd = new SaveFileDialog();
						sfd.Filter = "Defender Story マップファイル (*.citmap) | *.citmap";
						sfd.DefaultExt = "citmap";
						if (sfd.ShowDialog() == DialogResult.Cancel)
						{
							osf.StatusMessage = "キャンセルしました。";
							osf.Request = RequestFlag.None;
							goto nuke;
						}
						MapUtility.SaveMap(chips, sfd.FileName);
						osf.StatusMessage = "マップデータを保存しました。";
					}
					else if (osf.Request == RequestFlag.SaveSpdata)
					{
						var sfd = new SaveFileDialog();
						sfd.Filter = "json ドキュメント (*.json) | *.json";
						sfd.DefaultExt = "json";
						if (sfd.ShowDialog() == DialogResult.Cancel)
						{
							osf.StatusMessage = "キャンセルしました。";
							osf.Request = RequestFlag.None;
							goto nuke;
						}
						SpdataUtility.Save(splist, sfd.FileName);
						osf.StatusMessage = "エンティティデータを保存しました。";
					}
					else if (osf.Request == RequestFlag.SwapT)
					{
						MapSwap(chips, map, 0, -1);
					}
					else if (osf.Request == RequestFlag.SwapB)
					{
						MapSwap(chips, map, 0, 1);
					}
					else if (osf.Request == RequestFlag.SwapL)
					{
						MapSwap(chips, map, -1, 0);
					}
					else if (osf.Request == RequestFlag.SwapR)
					{
						MapSwap(chips, map, 1, 0);
					}
					else if (osf.Request == RequestFlag.TestPlay || osf.Request == RequestFlag.CheatPlay)
					{
						if (!Directory.Exists("temp"))
							Directory.CreateDirectory("temp");
						MapUtility.SaveMap(chips, "temp\\map.citmap");
						SpdataUtility.Save(splist, "temp\\spdata.json");
						foreach (var handle in GameEngine.TestPlay("temp", osf.Request != RequestFlag.TestPlay, mptname, (PlayerForm)Enum.Parse(typeof(PlayerForm), (string)osf.PlayerFormSelector.SelectedItem), scrSize, DX.GetColor(c.R, c.G, c.B)))
						{
							DX.DrawGraph(0, 0, handle, 0);
							DX.ProcessMessage();
							if (DX.ScreenFlip() == -1)
							{
								DX.DxLib_End();
								return;
							}

						}
					}

					editing = false;

				}

			nuke:
				osf.Request = RequestFlag.None;
				var ks = new States(binz, camera, map);
				int mousex = -1, mousey = -1;
				var mouseflag = DX.GetMouseInput();
				DX.GetMousePoint(out mousex, out mousey);

				

				if (ks.Inz == 1 && binz == 1)
					ks.Inz1 = 0;

				var inesc = DX.CheckHitKey(DX.KEY_INPUT_ESCAPE);

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
									pack.PutChipPackToArray(ref chips, (mousex - camera.X) / wari, (mousey - camera.Y) / wari, osf.Sf);
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
										spselidx = splist.FindIndex(sp => new Rectangle((int)(sp.PosX * (1 + (100 - zoom) / 100.0)), (int)(sp.PosY * (1 + (100 - zoom) / 100.0)), wari, wari).Contains((int)((mousex - camera.X) * (1 + (100 - zoom) / 100.0)), (int)((mousey - camera.Y) * (1 + (100 - zoom) / 100.0))));
										if (spselidx == -1)
											break;
										relativepoint = new Point((int)((mousex - camera.X - splist[spselidx].PosX) * (1 + (100 - zoom) / 100.0)), (int)((mousey - camera.Y - splist[spselidx].PosY) * (1 + (100 - zoom) / 100.0)));
									}
									if (spselidx == -1)
										break;
									splist[spselidx].PosX = (int)((mousex - camera.X) * (1 + (100 - zoom) / 100.0) - relativepoint.X);
									splist[spselidx].PosY = (int)((mousey - camera.Y) * (1 + (100 - zoom) / 100.0) - relativepoint.Y);
									if (ks.Inlshift == 1)
									{
										splist[spselidx].PosX = (int)Math.Round(splist[spselidx].PosX / 16.0) * 16;
										splist[spselidx].PosY = (int)Math.Round(splist[spselidx].PosY / 16.0) * 16;
									}
									
									break;
							}
							editing = true;
						}
						else
							osf.Chipno = chips[(mousex - camera.X) / wari, (mousey - camera.Y) / wari, (int)osf.Sf];

					}
					if (mouseflag == 0 && bmf != 0 && bmousex != -1 && bmousey != -1 && DX.GetWindowActiveFlag() == 1)
						switch (osf.Tool)
						{
							case ToolFlag.Line:
								DrawLineToArray(thx, thy, (bmousex - camera.X) / wari, (bmousey - camera.Y) / wari, ref chips, osf.Sf, (byte)osf.Chipno);
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
								FillBoxToArray(thx, thy, (bmousex - camera.X) / wari, (bmousey - camera.Y) / wari, ref chips, osf.Sf, pack);
								break;
							case ToolFlag.SpPut:
								var sp = new Entity();
								sp.PosX = (int)((mousex - camera.X) * (1 + (100 - zoom) / 100.0));
								sp.PosY = (int)((mousey - camera.Y) * (1 + (100 - zoom) / 100.0));
								if (ks.Inlshift == 1)
								{
									sp.PosX = (int)Math.Round(sp.PosX / 16.0) * 16;
									sp.PosY = (int)Math.Round(sp.PosY / 16.0) * 16;
								}
								sp.EntityId = osf.EntityId;
								sp.EntityData = SpdataUtility.GetPropertyObject(sp.EntityId);
								splist.Add(sp);
								break;
							case ToolFlag.SpSel:
								osf.StatusMessage = (string)(osf.Items[splist[spselidx].EntityId]);
								osf.propertyGrid1.SelectedObject = splist[spselidx];
								break;
							case ToolFlag.SpDel:
								spselidx = splist.FindIndex(
									spr =>
										new Rectangle(
											(int)(spr.PosX * (1 + (100 - zoom) / 100.0)),
											(int)(spr.PosY * (1 + (100 - zoom) / 100.0)),
											wari,
											wari).Contains(
											(int)((mousex - camera.X) * (1 + (100 - zoom) / 100.0)),
											(int)((mousey - camera.Y) * (1 + (100 - zoom) / 100.0))));
								if (spselidx == -1)
									break;

								//自らがPlayerかつリスト上にPlayerが一人しかいない(自分が唯一のPlayer)
								if (splist[spselidx].EntityId == 0 && splist.Count(s => s.EntityId == 0) == 1)
								{
									MessageBox.Show("Player は最低１人必要です．", "Defender's Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
									break;
								}
								if (splist[spselidx] == osf.propertyGrid1.SelectedObject)
									osf.propertyGrid1.SelectedObject = null;
								splist.RemoveAt(spselidx);
								spselidx = 0;
								break;
						}
				}
				catch (Exception ex)
				{
					osf.StatusMessage = string.Format("例外 {0} : {1} : {2}", ex.GetType().Name, ex.Message, ex.TargetSite);
				}

				//if (inesc == 1)
				//{
				//	DX.DxLib_End();
				//	return;
				//}

				if (ks.Camera.X > 0)
					ks.Camera = new Point(0, ks.Camera.Y);

				if (ks.Camera.Y > 0)
					ks.Camera = new Point(ks.Camera.X, 0);

				if (ks.Camera.X < -ks.Map.Width * wari + scrSize.Width)
					ks.Camera = new Point(-ks.Map.Width * wari + scrSize.Width, ks.Camera.Y);

				if (ks.Camera.Y < -ks.Map.Height * wari + scrSize.Height)
					ks.Camera = new Point(ks.Camera.X, -ks.Map.Height * wari + scrSize.Height);

				ks.Camera.Offset(DX.GetMouseHWheelRotVol() * 4, DX.GetMouseWheelRotVol() * 4);

				if (ks.Inup == 1)
					ks.Camera.Offset(0, 4);

				if (ks.Indown == 1)
					ks.Camera.Offset(0, -4);

				if (ks.Inleft == 1)
					ks.Camera.Offset(4, 0);

				if (ks.Inright == 1)
					ks.Camera.Offset(-4, 0);

				if (osf.GridVisible)
				{
					for (var y = 0; y < map.Height * wari; y += wari)
						if (y > -camera.Y - 16 && y < -camera.Y + scrSize.Height)
							for (var x = 0; x < map.Width * wari; x += wari)
								if (x > -camera.X - 16 && x < -camera.X + scrSize.Width)
									DX.DrawBox(x + camera.X, y + camera.Y, x + camera.X + wari + 1, y + camera.Y + wari + 1, DX.GetColor(127, 127, 127), 0);
				}

				DX.DrawString(mousex + 16, mousey, string.Format("{0}, {1}", ((ks.Inlshift == 1) ? ((mousex - camera.X) / wari * wari) : mousex - camera.X), ((ks.Inlshift == 1) ? ((mousey - camera.Y) / wari * wari) : mousey - camera.Y)), DX.GetColor(255, 255, 255));

				if (osf.BackVisible)
					for (var y = 0; y < map.Height * wari; y += wari)
						if (y > -camera.Y - 16 && y < -camera.Y + scrSize.Height)
							for (var x = 0; x < map.Width * wari; x += wari)
								if (x > -camera.X - 16 && x < -camera.X + scrSize.Width)
									DX.DrawGraph(x + camera.X, y + camera.Y, hndlMpt[chips[x / wari, y / wari, 1]], 1);

				if (osf.ForeVisible)
					for (var y = 0; y < map.Height * wari; y += wari)
						if (y > -camera.Y - 16 && y < -camera.Y + scrSize.Height)
							for (var x = 0; x < map.Width * wari; x += wari)
								if (x > -camera.X - 16 && x < -camera.X + scrSize.Width)
									DX.DrawGraph(x + camera.X, y + camera.Y, hndlMpt[chips[x / wari, y / wari, 0]], 1);

				foreach (var sp in splist)
				{
					DX.DrawBox((int)(sp.PosX * (zoom / 100.0) + camera.X), (int)(sp.PosY * (zoom / 100.0) + camera.Y), (int)(sp.PosX * (zoom / 100.0) + camera.X) + wari, (int)(sp.PosY * (zoom / 100.0) + camera.Y) + wari, (spselidx != -1 && sp == splist[spselidx]) ? DX.GetColor(255, 0, 0) : DX.GetColor(255, 255, 255), 0);
					NumFont.DrawNumFontString((int)(sp.PosX * (zoom / 100.0) + camera.X) + 2, (int)(sp.PosY * (zoom / 100.0) + camera.Y) + 2, DX.GetColor(255, 255, 255), sp.EntityId.ToString());
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
				binz = ks.Inz;
				bmousex = mousex; bmousey = mousey;
				camera = ks.Camera;
				map = ks.Map;

			}


		}

		private static void MapSwap(byte[,,] chips, Size size, int v1, int v2)
		{
			for (var y = 0; y < size.Height; y++)
			{
				for (var x = 0; x < size.Width; x++)
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
			var steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
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
			var deltax = x2 - x1;
			var deltay = Math.Abs(y2 - y1);
			var error = deltax / 2;
			int ystep;
			var y = y1;
			if (y1 < y2)
				ystep = 1;
			else
				ystep = -1;
			for (var x = x1; x <= x2; x++)
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


			for (var y = y1; y <= y2; y += value.Height)
				for (var x = x1; x <= x2; x += value.Width)
				{
					for (var px = 0; px < value.Width; px++)
					{
						var nx = x + px;
						if (nx > x2)
							continue;
                        for (var py = 0; py < value.Height; py++)
						{
							var ny = y + py;
							if (ny > y2)
								continue;
							array[nx, ny, (int)dimension] = value.Chips[py * value.Width + px];
						}
					}
				}
		}

		static void Swap<T>(ref T var1, ref T var2)
		{
			var tmp = var2;
			var2 = var1;
			var1 = tmp;
		}


	}




	//エンティティのスペル間違えてたので修正

	public struct States
	{
		public int Inup, Indown, Inleft, Inright, Inlshift, Inw, Ina, Ins, Ind, Inz, Inz1;
		public Point Camera;
		public Size Map;
		public States(int binz, Point c, Size m)
		{
			Inup = DX.CheckHitKey(DX.KEY_INPUT_UP);
			Indown = DX.CheckHitKey(DX.KEY_INPUT_DOWN);
			Inleft = DX.CheckHitKey(DX.KEY_INPUT_LEFT);
			Inright = DX.CheckHitKey(DX.KEY_INPUT_RIGHT);
			Inlshift = DX.CheckHitKey(DX.KEY_INPUT_LSHIFT);
			Inw = DX.CheckHitKey(DX.KEY_INPUT_W);
			Ina = DX.CheckHitKey(DX.KEY_INPUT_A);
			Ins = DX.CheckHitKey(DX.KEY_INPUT_S);
			Ind = DX.CheckHitKey(DX.KEY_INPUT_D);

			Inz = DX.CheckHitKey(DX.KEY_INPUT_Z);
			Inz1 = Inz;

			if (Inz == 1 && binz == 1)
				Inz1 = 0;
			Camera = c;
			Map = m;
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
			/*StringBuilder sb = new StringBuilder();	//Json を書き込む StringBuilder
			sb.Append("[");
			foreach(Entity sp in array)
			{
				sb.AppendFormat("{{\"PosX\": {0}, \"PosY\": {1}, \"EntityID\": {2}, \"EntityData\": {3}}}, ", sp.PosX, sp.PosY, sp.EntityID, GetPropertyJson(sp.EntityID));
			}
			sb.Remove(sb.Length - 2, 2);	//最後の, を消去
			sb.Append("]");
			File.WriteAllText(path, sb.ToString());*/

			var savedata = new object[array.Count];

			for (var i = 0; i < savedata.Length; i++)
			{
				var e = array[i];
				savedata[i] = new
				{
					e.PosX,
					e.PosY,
					EntityID = e.EntityId,
					EntityData = e.EntityData ?? GetPropertyObject(e.EntityId)
                };
			}
			File.WriteAllText(path, DynamicJson.Serialize(savedata));
		}

		public static void Load(out List<Entity> array, string path)
		{
			array = new List<Entity>();
			dynamic loaddata = DynamicJson.Parse(File.ReadAllText(path));

			foreach (var data in loaddata)
			{
				var e = new Entity
				{
					PosX = (int)data.PosX,
					PosY = (int)data.PosY,
					EntityId = (int)data.EntityID,
					EntityData = LoadPropertyObject((int)data.EntityID, data.EntityData)
				};
				array.Add(e);
			}
		}

		public static IEntityData LoadPropertyObject(int spriteid, dynamic dydat)
		{
			switch (spriteid)
			{
				case 0:
					return dydat.Deserialize<Player>();
				case 5:
					return dydat.Deserialize<InfinitySpawner>();
				case 7:
					return dydat.Deserialize<Woody>();
				case 12:
					return dydat.Deserialize<SpiderString>();
				case 18:
					return dydat.Deserialize<Goal>();
				case 22:
					return dydat.Deserialize<ItemSpawner>();
				case 28:
					return dydat.Deserialize<Coin>();
				case 44:
					return dydat.Deserialize<GoblinGirl>();
				case 60:
					return dydat.Deserialize<HalfGoal>();
				case 61:
					return dydat.Deserialize<DevilTsubasa>();
				case 62:
					return dydat.Deserialize<NormalTsubasa>();
				case 63:
					return dydat.Deserialize<Yuan>();
				case 84:
					return dydat.Deserialize<Sign>();
				default:
					return null;
			}
		}

		public static dynamic GetPropertyJson(int spriteid) => DynamicJson.Parse(DynamicJson.Serialize(GetPropertyObject(spriteid)));

		public static IEntityData GetPropertyObject(int spriteid)
		{
			switch (spriteid)
			{
				case 0:
					return new Player{ SpawnFromDoor = false, TargetDoorTag = "" };
				case 5:
					return new InfinitySpawner{ TargetEntity = (Entity)null };
				case 7:
					return new Woody{ StartX = 0 };
				case 12:
					return new SpiderString{ TargetEntityTag = "" };
				case 18:
					return new Goal{ NextStage = 0 };
				case 22:
					return new ItemSpawner{ EntityType = 0 };
				case 28:
					return new Coin{ WorkingType = 0 };
				case 44:
					return new GoblinGirl{ StartX = 0 };
				case 60:
					return new HalfGoal{ NextArea = 0 };
				case 61:
					return new DevilTsubasa{ StartX = 0 };
				case 62:
					return new NormalTsubasa{ StartX = 0 };
				case 63:
					return new Yuan{ StartX = 0 };
				case 84:
					return new Sign{ MessageText = "" };
				default:
					return null;
			}
		}

	}

	public struct ChipPack
	{
		public int Width, Height;
		public byte[] Chips;

		public ChipPack(int w, int h, params byte[] c)
		{
			Width = w;
			Height = h;
			Chips = c;
		}

		public void PutChipPackToArray(ref byte[, ,] array, int x, int y, ScreenFlag dimension)
		{
			for (var ix = x; ix < x + Width; ix++)
			{
				if (ix >= array.GetLength(0))
					break;
				for (var iy = y; iy < y + Height; iy++)
				{
					if (iy >= array.GetLength(1))
						break;
					array[ix, iy, (int)dimension] = Chips[(iy - y) * Width + (ix - x)];
				}
			}
		}
	}
}