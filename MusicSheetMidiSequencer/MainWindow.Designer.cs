namespace MusicSheetMidiSequencer
{
	partial class MainWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.公式サイトToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.バージョン情報ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mssf波形一覧ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.midi入力ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.process1 = new System.Diagnostics.Process();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.netcitringomusicSheetةڼةڼةڼةڼةڼToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.button3 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			this.contextMenuStrip2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.BackColor = System.Drawing.Color.DarkGray;
			this.tableLayoutPanel1.ColumnCount = 5;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 144F));
			this.tableLayoutPanel1.Controls.Add(this.button3, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.button2, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.button1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 4, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(624, 72);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.Color.Transparent;
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.trackBar1);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(147, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(330, 66);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.ForeColor = System.Drawing.Color.Black;
			this.label4.Location = new System.Drawing.Point(6, 11);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(11, 12);
			this.label4.TabIndex = 4;
			this.label4.Text = "<";
			this.label4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label4_MouseDown);
			this.label4.MouseEnter += new System.EventHandler(this.label4_MouseEnter);
			this.label4.MouseLeave += new System.EventHandler(this.label4_MouseLeave);
			this.label4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label4_MouseUp);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.Color.Black;
			this.label3.Location = new System.Drawing.Point(313, 11);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(11, 12);
			this.label3.TabIndex = 3;
			this.label3.Text = ">";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label4_MouseDown);
			this.label3.MouseEnter += new System.EventHandler(this.label4_MouseEnter);
			this.label3.MouseLeave += new System.EventHandler(this.label4_MouseLeave);
			this.label3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label4_MouseUp);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label2.ForeColor = System.Drawing.Color.Gainsboro;
			this.label2.Location = new System.Drawing.Point(0, 25);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(330, 19);
			this.label2.TabIndex = 2;
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoEllipsis = true;
			this.label1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(23, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(284, 19);
			this.label1.TabIndex = 0;
			this.label1.Text = "No Standard Midi File";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// trackBar1
			// 
			this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.trackBar1.AutoSize = false;
			this.trackBar1.BackColor = System.Drawing.Color.DarkGray;
			this.trackBar1.Location = new System.Drawing.Point(6, 41);
			this.trackBar1.Maximum = 10000;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(318, 19);
			this.trackBar1.TabIndex = 1;
			this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.公式サイトToolStripMenuItem,
            this.バージョン情報ToolStripMenuItem,
            this.mssf波形一覧ToolStripMenuItem,
            this.toolStripSeparator1,
            this.midi入力ToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(177, 120);
			// 
			// 公式サイトToolStripMenuItem
			// 
			this.公式サイトToolStripMenuItem.Name = "公式サイトToolStripMenuItem";
			this.公式サイトToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
			this.公式サイトToolStripMenuItem.Text = "Citringo.net へ...";
			this.公式サイトToolStripMenuItem.ToolTipText = "Citringo の公式サイトを、既定のブラウザーで開きます。";
			this.公式サイトToolStripMenuItem.Click += new System.EventHandler(this.公式サイトToolStripMenuItem_Click);
			// 
			// バージョン情報ToolStripMenuItem
			// 
			this.バージョン情報ToolStripMenuItem.Name = "バージョン情報ToolStripMenuItem";
			this.バージョン情報ToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
			this.バージョン情報ToolStripMenuItem.Text = "バージョン情報...";
			this.バージョン情報ToolStripMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
			this.バージョン情報ToolStripMenuItem.ToolTipText = "このアプリのバージョン情報を表示します。";
			this.バージョン情報ToolStripMenuItem.Click += new System.EventHandler(this.バージョン情報ToolStripMenuItem_Click);
			// 
			// mssf波形一覧ToolStripMenuItem
			// 
			this.mssf波形一覧ToolStripMenuItem.Name = "mssf波形一覧ToolStripMenuItem";
			this.mssf波形一覧ToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
			this.mssf波形一覧ToolStripMenuItem.Text = "Mssf 波形一覧...";
			this.mssf波形一覧ToolStripMenuItem.ToolTipText = "Music Sheet で使用されている波形の一覧を表示します。";
			this.mssf波形一覧ToolStripMenuItem.Click += new System.EventHandler(this.mssf波形一覧ToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(173, 6);
			// 
			// timer1
			// 
			this.timer1.Interval = 1;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// process1
			// 
			this.process1.StartInfo.Domain = "";
			this.process1.StartInfo.LoadUserProfile = false;
			this.process1.StartInfo.Password = null;
			this.process1.StartInfo.StandardErrorEncoding = null;
			this.process1.StartInfo.StandardOutputEncoding = null;
			this.process1.StartInfo.UserName = "";
			this.process1.SynchronizingObject = this;
			// 
			// backgroundWorker1
			// 
			this.backgroundWorker1.WorkerSupportsCancellation = true;
			this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
			// 
			// contextMenuStrip2
			// 
			this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.netcitringomusicSheetةڼةڼةڼةڼةڼToolStripMenuItem});
			this.contextMenuStrip2.Name = "contextMenuStrip1";
			this.contextMenuStrip2.Size = new System.Drawing.Size(542, 92);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(541, 22);
			this.toolStripMenuItem1.Text = "net.citringo.musicSheet.GoToWebSite";
			this.toolStripMenuItem1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(541, 22);
			this.toolStripMenuItem2.Text = "net.citringo.musicSheet.OpenAbout";
			this.toolStripMenuItem2.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(541, 22);
			this.toolStripMenuItem3.Text = "net.citringo.musicSheet.OpenMssfWaveList";
			// 
			// netcitringomusicSheetةڼةڼةڼةڼةڼToolStripMenuItem
			// 
			this.netcitringomusicSheetةڼةڼةڼةڼةڼToolStripMenuItem.Name = "netcitringomusicSheetةڼةڼةڼةڼةڼToolStripMenuItem";
			this.netcitringomusicSheetةڼةڼةڼةڼةڼToolStripMenuItem.Size = new System.Drawing.Size(541, 22);
			this.netcitringomusicSheetةڼةڼةڼةڼةڼToolStripMenuItem.Text = "net.citringo.musicSheet.└(՞ةڼ◔)」└(՞ةڼ◔)」└(՞ةڼ◔)」└(՞ةڼ◔)」└(՞ةڼ◔)」";
			this.netcitringomusicSheetةڼةڼةڼةڼةڼToolStripMenuItem.Click += new System.EventHandler(this.netcitringomusicSheetةڼةڼةڼةڼةڼToolStripMenuItem_Click);
			// 
			// pictureBox2
			// 
			this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox2.Location = new System.Drawing.Point(0, 72);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(624, 333);
			this.pictureBox2.TabIndex = 2;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox2_Paint);
			// 
			// button3
			// 
			this.button3.BackColor = System.Drawing.Color.Transparent;
			this.button3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.button3.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.button3.Image = global::MusicSheetMidiSequencer.Properties.Resources.Symbols_Stop_32xLG;
			this.button3.Location = new System.Drawing.Point(99, 3);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(42, 66);
			this.button3.TabIndex = 3;
			this.button3.UseVisualStyleBackColor = false;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button2
			// 
			this.button2.BackColor = System.Drawing.Color.Transparent;
			this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.button2.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.button2.Image = global::MusicSheetMidiSequencer.Properties.Resources.Symbols_Pause_32xLG;
			this.button2.Location = new System.Drawing.Point(51, 3);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(42, 66);
			this.button2.TabIndex = 2;
			this.button2.UseVisualStyleBackColor = false;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button1
			// 
			this.button1.BackColor = System.Drawing.Color.Transparent;
			this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.button1.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.button1.Image = global::MusicSheetMidiSequencer.Properties.Resources.Symbols_Play_32xLG;
			this.button1.Location = new System.Drawing.Point(3, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(42, 66);
			this.button1.TabIndex = 1;
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.button1_Click_1);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(483, 3);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(138, 66);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 4;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
			// 
			// MainWindow
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(624, 405);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(640, 444);
			this.Name = "MainWindow";
			this.Text = "Music Sheet Player";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainWindow_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainWindow_DragEnter);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			this.contextMenuStrip2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem 公式サイトToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem バージョン情報ToolStripMenuItem;
		public System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.ToolStripMenuItem mssf波形一覧ToolStripMenuItem;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Timer timer1;
		private System.Diagnostics.Process process1;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem netcitringomusicSheetةڼةڼةڼةڼةڼToolStripMenuItem;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem midi入力ToolStripMenuItem;




	}
}