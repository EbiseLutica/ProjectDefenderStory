using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicSheetMidiSequencer
{
	public partial class MidiDeviceDialog : Form
	{
		public int SelectedDeviceIndex
		{
			get
			{
				return listBox1.SelectedIndex;
			}
		}
		public MidiDeviceDialog(string[] devices)
		{
			InitializeComponent();
			listBox1.Items.AddRange(devices);
			listBox1.SelectedIndex = 0;
		}
	}
}
