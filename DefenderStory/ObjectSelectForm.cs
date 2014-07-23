using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

		public ObjectSelectForm(string filename)
		{
			InitializeComponent();
			if (filename != null)
				this.pictureBox1.Image = new Bitmap(filename);
		}

		public int chipno = 0;

		public Random rnd = new Random(100);


		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			
			chipno = e.Y / 16 * 16 + e.X / 16;
			this.Refresh();
		}

		private void pictureBox1_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.DrawRectangle(new Pen(Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)), 1), chipno % 16 * 16,chipno / 16 * 16 , 16, 16);
		}

		


	}


}
