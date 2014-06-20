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
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
			this.maskedTextBox2 = new System.Windows.Forms.MaskedTextBox();
			this.maskedTextBox4 = new System.Windows.Forms.MaskedTextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
			this.SuspendLayout();
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(84, 30);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(47, 19);
			this.numericUpDown1.TabIndex = 2;
			this.numericUpDown1.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
			// 
			// maskedTextBox1
			// 
			this.maskedTextBox1.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.maskedTextBox1.Location = new System.Drawing.Point(8, 30);
			this.maskedTextBox1.Mask = "990";
			this.maskedTextBox1.Name = "maskedTextBox1";
			this.maskedTextBox1.Size = new System.Drawing.Size(32, 19);
			this.maskedTextBox1.TabIndex = 0;
			this.maskedTextBox1.Text = "0";
			this.maskedTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// maskedTextBox2
			// 
			this.maskedTextBox2.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.maskedTextBox2.Location = new System.Drawing.Point(46, 30);
			this.maskedTextBox2.Mask = "990";
			this.maskedTextBox2.Name = "maskedTextBox2";
			this.maskedTextBox2.Size = new System.Drawing.Size(32, 19);
			this.maskedTextBox2.TabIndex = 1;
			this.maskedTextBox2.Text = "0";
			this.maskedTextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// maskedTextBox4
			// 
			this.maskedTextBox4.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.maskedTextBox4.Location = new System.Drawing.Point(137, 30);
			this.maskedTextBox4.Mask = "990";
			this.maskedTextBox4.Name = "maskedTextBox4";
			this.maskedTextBox4.Size = new System.Drawing.Size(32, 19);
			this.maskedTextBox4.TabIndex = 1;
			this.maskedTextBox4.Text = "0";
			this.maskedTextBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(13, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "A";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(44, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(13, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "D";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(82, 15);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(13, 12);
			this.label3.TabIndex = 3;
			this.label3.Text = "A";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(135, 15);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(13, 12);
			this.label4.TabIndex = 3;
			this.label4.Text = "R";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(82, 15);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(12, 12);
			this.label5.TabIndex = 3;
			this.label5.Text = "S";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.maskedTextBox1);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.maskedTextBox2);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.maskedTextBox4);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.numericUpDown1);
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
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            "Sine",
            "Square",
            "12Pulse",
            "25Pulse",
            "Triangle",
            "LongNoise",
            "ShortNoise"});
			this.comboBox1.Location = new System.Drawing.Point(11, 86);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(132, 20);
			this.comboBox1.TabIndex = 7;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(321, 261);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.numericUpDown2);
			this.Controls.Add(this.groupBox1);
			this.Name = "Form1";
			this.Text = "MusicSheet SE Editor";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.MaskedTextBox maskedTextBox1;
		private System.Windows.Forms.MaskedTextBox maskedTextBox2;
		private System.Windows.Forms.MaskedTextBox maskedTextBox4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDown numericUpDown2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox comboBox1;
	}
}

