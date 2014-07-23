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
	public partial class MainWindow : Form
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		
		public bool playRequest = false;
		public string file = "";

		private void label1_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void label1_DragDrop(object sender, DragEventArgs e)
		{
			string[] fileName = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			if (fileName.Length > 1)
				MessageBox.Show("複数のアイテムがドロップされました。一つを再生します。");
			playRequest = true;
			file = fileName[0];
			
		}
	}
}
