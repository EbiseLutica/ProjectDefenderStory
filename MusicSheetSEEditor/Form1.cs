using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicSheetSEEditor
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			comboBox1.SelectedIndex = 0;
		}

		int hztemp = 440;



		private void numericUpDown2_ValueChanged(object sender, EventArgs e)
		{

		}

		private void textBox1_Leave(object sender, EventArgs e)
		{
			if (int.TryParse(textBox1.Text, out ))
		}



	}

	public struct SoundData
	{
		public int A;
		public int D;
		public byte S;
		public int R;

		public int waveidx;
		public int freq;
		public int startidx;
		public int length;
		public bool enabled;
		public int pan;
	}

}
