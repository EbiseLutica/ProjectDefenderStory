namespace MusicSheetSEEditor
{
	partial class Form1
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

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			this.textBox7 = new System.Windows.Forms.TextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.paramBox1 = new MusicSheetSEEditor.ParamBox();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(178, 56);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "エンベロープ";
			// 
			// numericUpDown2
			// 
			this.numericUpDown2.Location = new System.Drawing.Point(253, 25);
			this.numericUpDown2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown2.Minimum = new decimal(new int[] {
            255,
            0,
            0,
            -2147483648});
			this.numericUpDown2.Name = "numericUpDown2";
			this.numericUpDown2.Size = new System.Drawing.Size(52, 19);
			this.numericUpDown2.TabIndex = 5;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(217, 29);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(30, 12);
			this.label6.TabIndex = 6;
			this.label6.Text = "パン: ";
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(82, 224);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(44, 16);
			this.checkBox1.TabIndex = 10;
			this.checkBox1.Text = "Use";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(365, 27);
			this.button1.Name = "button1";
			this.button1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.button1.Size = new System.Drawing.Size(97, 32);
			this.button1.TabIndex = 15;
			this.button1.Text = "ビルド";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// textBox7
			// 
			this.textBox7.Location = new System.Drawing.Point(468, 98);
			this.textBox7.Name = "textBox7";
			this.textBox7.Size = new System.Drawing.Size(82, 19);
			this.textBox7.TabIndex = 16;
			this.textBox7.Text = "44100";
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(397, 98);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(65, 19);
			this.label20.TabIndex = 12;
			this.label20.Text = "byte数:";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// paramBox1
			// 
			this.paramBox1.Frequency = 440;
			this.paramBox1.Location = new System.Drawing.Point(12, 74);
			this.paramBox1.Name = "paramBox1";
			this.paramBox1.Offset = 100F;
			this.paramBox1.Size = new System.Drawing.Size(197, 140);
			this.paramBox1.TabIndex = 17;
			this.paramBox1.Volume = 50F;
			this.paramBox1.WaveID = 0;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(553, 247);
			this.Controls.Add(this.paramBox1);
			this.Controls.Add(this.textBox7);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.numericUpDown2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label20);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "Form1";
			this.Text = "MusicSheet SE Editor";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDown numericUpDown2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBox7;
		private System.Windows.Forms.Label label20;
		private ParamBox paramBox1;
	}
}

