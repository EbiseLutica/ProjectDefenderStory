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

		private void textBox1_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				//ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
				e.Effect = DragDropEffects.Copy;
			else
				//ファイル以外は受け付けない
				e.Effect = DragDropEffects.None;
		}

		private void textBox1_DragDrop(object sender, DragEventArgs e)
		{
			//コントロール内にドロップされたとき実行される
			//ドロップされたすべてのファイル名を取得する
			string[] fileName =
				(string[])e.Data.GetData(DataFormats.FileDrop, false);
			//ListBoxに追加する
			if (fileName.Length > 1)
				MessageBox.Show("複数のファイルがドロップされました。\r\n先頭のファイルのみ再生を開始します");
			file = fileName[0];
			playRequest = true;
		}

		/// <summary>
		/// 再生リクエストが送られたかどうかを判定し、ある場合は指定した文字列変数にファイル名を代入します。
		/// </summary>
		/// <param name="data">代入する String 変数。</param>
		/// <returns>リクエストが送られたかどうか。</returns>
		public bool GetPlayRequest(ref string data)
		{
			if (playRequest)
				data = file;
			bool p = playRequest;
			playRequest = false;
			return p;
		}


		
	}
}
