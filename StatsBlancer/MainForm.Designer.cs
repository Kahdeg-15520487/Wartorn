namespace StatsBlancer
{
    partial class MainForm
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
            this.button_unit = new System.Windows.Forms.Button();
            this.button_terrain = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_unit
            // 
            this.button_unit.Location = new System.Drawing.Point(12, 33);
            this.button_unit.Name = "button_unit";
            this.button_unit.Size = new System.Drawing.Size(75, 23);
            this.button_unit.TabIndex = 0;
            this.button_unit.Text = "Unit";
            this.button_unit.UseVisualStyleBackColor = true;
            this.button_unit.Click += new System.EventHandler(this.button_unit_Click);
            // 
            // button_terrain
            // 
            this.button_terrain.Location = new System.Drawing.Point(93, 33);
            this.button_terrain.Name = "button_terrain";
            this.button_terrain.Size = new System.Drawing.Size(75, 23);
            this.button_terrain.TabIndex = 1;
            this.button_terrain.Text = "Terrain";
            this.button_terrain.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(185, 79);
            this.Controls.Add(this.button_terrain);
            this.Controls.Add(this.button_unit);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_unit;
        private System.Windows.Forms.Button button_terrain;
    }
}

