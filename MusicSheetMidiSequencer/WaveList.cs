using System;
using System.Collections.Generic;
using System.ComponentModel;
using MusicSheet.Mssf;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicSheetMidiSequencer
{
	public partial class WaveList : Form
	{
		Bitmap bitmap = new Bitmap(512, 512);
		public WaveList()
		{
			InitializeComponent();
			Graphics g = Graphics.FromImage(bitmap);
			for (int i = 0; i < 128; i++)
			{
				short[] wave = null;
				int a, d, r, pan;
				byte s;
				int x = i % 12 * 40, y = i / 12 * 44;
				
				g.DrawString(i.ToString(), new Font("MS UI Gothic", 10), Brushes.Black, x, y);
				y += 20;
				if (System.IO.File.Exists("Insts\\" + i + ".mssf"))
				{
					MssfUtility.LoadFileDynamic("Insts\\" + i + ".mssf", out wave, out a, out d, out s, out r, out pan);
					DrawWave(x, y, g, wave);
				}
				else
				{
					g.DrawLine(Pens.Black, x, y - 8, x + 31, y + 8);
					g.DrawLine(Pens.Black, x, y + 8, x + 31, y - 8);
					g.DrawRectangle(Pens.Black, x, y - 8, 32, 16);
				}
			}

			g.Dispose();
			pictureBox1.Image = bitmap;

		}

		

		private void pictureBox1_Click(object sender, EventArgs e)
		{

		}

		public static void DrawWave(int x, int y, Graphics g, short[] wave)
		{

			for (int j = 0; j < 32; j++)
			{
				g.DrawLine(Pens.Black, x + j, y, x + j, y + (wave[j] / 4096));
			}
		}
	
	}
}
