using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicSheetSEEditor
{
	public partial class ParamBox : UserControl
	{

		public override string Text
		{
			get
			{
				return groupBox2.Text;
			}
			set
			{
				groupBox2.Text = value;
			}
		}

		
		public int WaveID
		{
			get
			{
				return this.combobox1.SelectedIndex;
			}
			set
			{
				this.combobox1.SelectedIndex = value;
			}
		}
		public int Frequency
		{
			get
			{
				return (int)numericUpDown1.Value;
			}
			set
			{
				this.numericUpDown1.Value = value;
			}
		}
		public float Volume
		{
			get
			{
				return (float)basicVol1.Value;
			}
			set
			{
				this.basicVol1.Value = (decimal)value;
			}
		}
		public float Offset
		{
			get
			{
				return (float)numericUpDown2.Value;
			}
			set
			{
				this.numericUpDown2.Value = (decimal)value;
			}
		}

		public ParamBox()
		{
			InitializeComponent();
			this.combobox1.SelectedIndex = 0;
		}
	}
}
