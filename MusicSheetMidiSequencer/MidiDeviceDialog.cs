using System.Windows.Forms;

namespace MusicSheetMidiSequencer
{
	public partial class MidiDeviceDialog : Form
	{
		public MidiDeviceDialog(string[] devices)
		{
			InitializeComponent();
			listBox1.Items.AddRange(devices);
			listBox1.SelectedIndex = 0;
		}

		public int SelectedDeviceIndex => listBox1.SelectedIndex;
	}
}