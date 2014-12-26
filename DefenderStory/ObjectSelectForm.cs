using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DefenderStory
{
	public partial class ObjectSelectForm : Form
	{
		public ObjectSelectForm()
		{
			InitializeComponent();
		}

		public bool reloadRequest = false;

		public string Path
		{
			get
			{
				return this.toolStripButton2.Text;
			}
			private set
			{
				this.toolStripButton2.Text = value;
			}
		}

		public ObjectSelectForm(string filename)
		{
			InitializeComponent();
			if (filename != null)
				this.pictureBox1.Image = new Bitmap(filename);
			Task.Factory.StartNew(new Action(()=>
				{
					while (true)
					{
						if (!this.IsHandleCreated)
							continue;
						this.Invoke(new MethodInvoker(() =>
							{
								pictureBox1.Refresh();
							}));
						gradient = (gradient + 10) % 360;
						Thread.Sleep(16);
						
					}
				}));
		}

		public int chipno = 0;

		public Random rnd = new Random(100);


		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			
			chipno = e.Y / 16 * 16 + e.X / 16;
			this.Refresh();
		}


		int gradient = 0;

		private void pictureBox1_Paint(object sender, PaintEventArgs e)
		{
			int dat = (int)(Math.Sin(gradient / 180.0 * Math.PI) * 127 + 127);
			e.Graphics.DrawRectangle(new Pen(Color.FromArgb(dat, dat, dat), 1), chipno % 16 * 16,chipno / 16 * 16 , 16, 16);
			//Console.Write(dat + " ");
		}

		public ScreenFlag sf = ScreenFlag.Fore;

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			if (sf == ScreenFlag.Fore)
			{
				sf = ScreenFlag.Back;
				toolStripButton1.Text = "ウラ";
			}
			else
			{
				sf = ScreenFlag.Fore;
				toolStripButton1.Text = "オモテ";
			}
		}

		private void toolStripContainer1_ContentPanel_Load(object sender, EventArgs e)
		{

		}

		private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

		}

		

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			reloadRequest = true;
		}

	}

	public enum ScreenFlag
	{
		Fore, Back
	}


}
