namespace MusicSheetSEEditor
{
	partial class ParamBox
	{
		/// <summary> 
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region コンポーネント デザイナーで生成されたコード

		/// <summary> 
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.basicVol1 = new System.Windows.Forms.NumericUpDown();
			this.combobox1 = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.basicVol1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.numericUpDown2);
			this.groupBox2.Controls.Add(this.numericUpDown1);
			this.groupBox2.Controls.Add(this.basicVol1);
			this.groupBox2.Controls.Add(this.combobox1);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(0, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(221, 156);
			this.groupBox2.TabIndex = 14;
			this.groupBox2.TabStop = false;
			// 
			// basicVol1
			// 
			this.basicVol1.DecimalPlaces = 1;
			this.basicVol1.Location = new System.Drawing.Point(80, 83);
			this.basicVol1.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.basicVol1.Name = "basicVol1";
			this.basicVol1.Size = new System.Drawing.Size(84, 19);
			this.basicVol1.TabIndex = 17;
			this.basicVol1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.basicVol1.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
			// 
			// combobox1
			// 
			this.combobox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.combobox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.combobox1.FormattingEnabled = true;
			this.combobox1.Items.AddRange(new object[] {
            "0 Sine",
            "1 Square",
            "2 12Pulse",
            "3 25Pulse",
            "4 Triangle",
            "5 Saw",
            "6 LongNoise",
            "7 ShortNoise",
            "8 SoftLongNoise",
            "9 SoftShortNoise"});
			this.combobox1.Location = new System.Drawing.Point(32, 18);
			this.combobox1.Name = "combobox1";
			this.combobox1.Size = new System.Drawing.Size(132, 20);
			this.combobox1.TabIndex = 7;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(12, 56);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(61, 19);
			this.label7.TabIndex = 9;
			this.label7.Text = "周波数:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(169, 116);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(11, 12);
			this.label12.TabIndex = 11;
			this.label12.Text = "%";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(169, 85);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(11, 12);
			this.label11.TabIndex = 11;
			this.label11.Text = "%";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(8, 85);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(65, 19);
			this.label8.TabIndex = 9;
			this.label8.Text = "音量:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(170, 59);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(18, 12);
			this.label10.TabIndex = 11;
			this.label10.Text = "Hz";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(10, 113);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(63, 19);
			this.label9.TabIndex = 9;
			this.label9.Text = "オフセット:";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(80, 56);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            44100,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(84, 19);
			this.numericUpDown1.TabIndex = 17;
			this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDown1.Value = new decimal(new int[] {
            440,
            0,
            0,
            0});
			// 
			// numericUpDown2
			// 
			this.numericUpDown2.DecimalPlaces = 1;
			this.numericUpDown2.Location = new System.Drawing.Point(80, 114);
			this.numericUpDown2.Name = "numericUpDown2";
			this.numericUpDown2.Size = new System.Drawing.Size(84, 19);
			this.numericUpDown2.TabIndex = 17;
			this.numericUpDown2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numericUpDown2.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// ParamBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox2);
			this.Name = "ParamBox";
			this.Size = new System.Drawing.Size(221, 156);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.basicVol1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.NumericUpDown basicVol1;
		private System.Windows.Forms.ComboBox combobox1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.NumericUpDown numericUpDown2;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
	}
}
