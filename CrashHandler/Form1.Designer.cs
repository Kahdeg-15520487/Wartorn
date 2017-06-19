namespace CrashHandler
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_crashinfo = new System.Windows.Forms.TextBox();
            this.label_crashreportfilename = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Crash report";
            // 
            // textBox_crashinfo
            // 
            this.textBox_crashinfo.Location = new System.Drawing.Point(12, 49);
            this.textBox_crashinfo.Multiline = true;
            this.textBox_crashinfo.Name = "textBox_crashinfo";
            this.textBox_crashinfo.ReadOnly = true;
            this.textBox_crashinfo.Size = new System.Drawing.Size(403, 266);
            this.textBox_crashinfo.TabIndex = 2;
            // 
            // label_crashreportfilename
            // 
            this.label_crashreportfilename.AutoSize = true;
            this.label_crashreportfilename.Location = new System.Drawing.Point(12, 33);
            this.label_crashreportfilename.Name = "label_crashreportfilename";
            this.label_crashreportfilename.Size = new System.Drawing.Size(117, 13);
            this.label_crashreportfilename.TabIndex = 3;
            this.label_crashreportfilename.Text = "crash report file name:  ";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(407, -1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(21, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "X";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 327);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label_crashreportfilename);
            this.Controls.Add(this.textBox_crashinfo);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Crash report";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_crashinfo;
        private System.Windows.Forms.Label label_crashreportfilename;
        private System.Windows.Forms.Button button1;
    }
}

