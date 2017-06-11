namespace StatsBlancer
{
    partial class TerrainEditor
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
            this.listBox_terraintypes = new System.Windows.Forms.ListBox();
            this.button_save_terrain = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button_done = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_defstar = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // listBox_terraintypes
            // 
            this.listBox_terraintypes.FormattingEnabled = true;
            this.listBox_terraintypes.Location = new System.Drawing.Point(12, 25);
            this.listBox_terraintypes.Name = "listBox_terraintypes";
            this.listBox_terraintypes.Size = new System.Drawing.Size(120, 225);
            this.listBox_terraintypes.TabIndex = 0;
            this.listBox_terraintypes.SelectedIndexChanged += new System.EventHandler(this.listBox_terraintypes_SelectedIndexChanged);
            // 
            // button_save_terrain
            // 
            this.button_save_terrain.Location = new System.Drawing.Point(138, 12);
            this.button_save_terrain.Name = "button_save_terrain";
            this.button_save_terrain.Size = new System.Drawing.Size(62, 41);
            this.button_save_terrain.TabIndex = 1;
            this.button_save_terrain.Text = "Save";
            this.button_save_terrain.UseVisualStyleBackColor = true;
            this.button_save_terrain.Click += new System.EventHandler(this.button_save_terrain_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Terrain list";
            // 
            // button_done
            // 
            this.button_done.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_done.Location = new System.Drawing.Point(233, 12);
            this.button_done.Name = "button_done";
            this.button_done.Size = new System.Drawing.Size(62, 41);
            this.button_done.TabIndex = 4;
            this.button_done.Text = "Done";
            this.button_done.UseVisualStyleBackColor = true;
            this.button_done.Click += new System.EventHandler(this.button_done_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(138, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Def star";
            // 
            // textBox_defstar
            // 
            this.textBox_defstar.Location = new System.Drawing.Point(195, 72);
            this.textBox_defstar.Name = "textBox_defstar";
            this.textBox_defstar.Size = new System.Drawing.Size(100, 20);
            this.textBox_defstar.TabIndex = 7;
            this.textBox_defstar.TextChanged += new System.EventHandler(this.ValidateInput);
            // 
            // TerrainEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 261);
            this.Controls.Add(this.textBox_defstar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_done);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_save_terrain);
            this.Controls.Add(this.listBox_terraintypes);
            this.Name = "TerrainEditor";
            this.Text = "TerrainEditor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_terraintypes;
        private System.Windows.Forms.Button button_save_terrain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_done;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_defstar;
    }
}