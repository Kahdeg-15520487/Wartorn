namespace StatsBlancer
{
    partial class UnitEditor
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_cost = new System.Windows.Forms.TextBox();
            this.textBox_move = new System.Windows.Forms.TextBox();
            this.textBox_vision = new System.Windows.Forms.TextBox();
            this.textBox_rangemax = new System.Windows.Forms.TextBox();
            this.textBox_gas = new System.Windows.Forms.TextBox();
            this.textBox_fuelperturn = new System.Windows.Forms.TextBox();
            this.listBox_unitSelect = new System.Windows.Forms.ListBox();
            this.button_save = new System.Windows.Forms.Button();
            this.textBox_rangemin = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label_selectedUnit = new System.Windows.Forms.Label();
            this.button_done = new System.Windows.Forms.Button();
            this.dataGridView_unitdmg = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox_ammo = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_actionpoint = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_unitdmg)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select Unit";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Cost";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Movement";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Vision";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Range";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Gas";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 151);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Fuel per Turn";
            // 
            // textBox_cost
            // 
            this.textBox_cost.Location = new System.Drawing.Point(88, 18);
            this.textBox_cost.Name = "textBox_cost";
            this.textBox_cost.Size = new System.Drawing.Size(100, 20);
            this.textBox_cost.TabIndex = 1;
            this.textBox_cost.TextChanged += new System.EventHandler(this.ValidateInput);
            // 
            // textBox_move
            // 
            this.textBox_move.Location = new System.Drawing.Point(88, 44);
            this.textBox_move.Name = "textBox_move";
            this.textBox_move.Size = new System.Drawing.Size(100, 20);
            this.textBox_move.TabIndex = 2;
            this.textBox_move.TextChanged += new System.EventHandler(this.ValidateInput);
            // 
            // textBox_vision
            // 
            this.textBox_vision.Location = new System.Drawing.Point(88, 70);
            this.textBox_vision.Name = "textBox_vision";
            this.textBox_vision.Size = new System.Drawing.Size(100, 20);
            this.textBox_vision.TabIndex = 3;
            this.textBox_vision.TextChanged += new System.EventHandler(this.ValidateInput);
            // 
            // textBox_rangemax
            // 
            this.textBox_rangemax.Location = new System.Drawing.Point(88, 96);
            this.textBox_rangemax.Name = "textBox_rangemax";
            this.textBox_rangemax.Size = new System.Drawing.Size(32, 20);
            this.textBox_rangemax.TabIndex = 4;
            this.textBox_rangemax.TextChanged += new System.EventHandler(this.ValidateInput);
            // 
            // textBox_gas
            // 
            this.textBox_gas.Location = new System.Drawing.Point(88, 122);
            this.textBox_gas.Name = "textBox_gas";
            this.textBox_gas.Size = new System.Drawing.Size(100, 20);
            this.textBox_gas.TabIndex = 6;
            this.textBox_gas.TextChanged += new System.EventHandler(this.ValidateInput);
            // 
            // textBox_fuelperturn
            // 
            this.textBox_fuelperturn.Location = new System.Drawing.Point(88, 148);
            this.textBox_fuelperturn.Name = "textBox_fuelperturn";
            this.textBox_fuelperturn.ReadOnly = true;
            this.textBox_fuelperturn.Size = new System.Drawing.Size(100, 20);
            this.textBox_fuelperturn.TabIndex = 7;
            this.textBox_fuelperturn.TextChanged += new System.EventHandler(this.ValidateInput);
            // 
            // listBox_unitSelect
            // 
            this.listBox_unitSelect.FormattingEnabled = true;
            this.listBox_unitSelect.Location = new System.Drawing.Point(15, 40);
            this.listBox_unitSelect.Name = "listBox_unitSelect";
            this.listBox_unitSelect.Size = new System.Drawing.Size(120, 368);
            this.listBox_unitSelect.TabIndex = 0;
            this.listBox_unitSelect.SelectedIndexChanged += new System.EventHandler(this.listBox_unitSelect_SelectedIndexChanged);
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(141, 9);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(92, 44);
            this.button_save.TabIndex = 13;
            this.button_save.Text = "Save Unit";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // textBox_rangemin
            // 
            this.textBox_rangemin.Location = new System.Drawing.Point(153, 96);
            this.textBox_rangemin.Name = "textBox_rangemin";
            this.textBox_rangemin.Size = new System.Drawing.Size(35, 20);
            this.textBox_rangemin.TabIndex = 5;
            this.textBox_rangemin.TextChanged += new System.EventHandler(this.ValidateInput);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(130, 99);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(16, 13);
            this.label13.TabIndex = 20;
            this.label13.Text = "->";
            // 
            // label_selectedUnit
            // 
            this.label_selectedUnit.AutoSize = true;
            this.label_selectedUnit.Location = new System.Drawing.Point(12, 25);
            this.label_selectedUnit.Name = "label_selectedUnit";
            this.label_selectedUnit.Size = new System.Drawing.Size(0, 13);
            this.label_selectedUnit.TabIndex = 21;
            // 
            // button_done
            // 
            this.button_done.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_done.Location = new System.Drawing.Point(239, 9);
            this.button_done.Name = "button_done";
            this.button_done.Size = new System.Drawing.Size(100, 44);
            this.button_done.TabIndex = 14;
            this.button_done.Text = "Done";
            this.button_done.UseVisualStyleBackColor = true;
            this.button_done.Click += new System.EventHandler(this.button_done_Click);
            // 
            // dataGridView_unitdmg
            // 
            this.dataGridView_unitdmg.AllowUserToAddRows = false;
            this.dataGridView_unitdmg.AllowUserToDeleteRows = false;
            this.dataGridView_unitdmg.AllowUserToResizeColumns = false;
            this.dataGridView_unitdmg.AllowUserToResizeRows = false;
            this.dataGridView_unitdmg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_unitdmg.GridColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGridView_unitdmg.Location = new System.Drawing.Point(353, 9);
            this.dataGridView_unitdmg.Name = "dataGridView_unitdmg";
            this.dataGridView_unitdmg.Size = new System.Drawing.Size(563, 399);
            this.dataGridView_unitdmg.TabIndex = 22;
            this.dataGridView_unitdmg.TabStop = false;
            this.dataGridView_unitdmg.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_unitdmg_CellValueChanged);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.textBox_actionpoint);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.textBox_ammo);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.textBox_gas);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.textBox_rangemin);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.textBox_cost);
            this.panel2.Controls.Add(this.textBox_move);
            this.panel2.Controls.Add(this.textBox_fuelperturn);
            this.panel2.Controls.Add(this.textBox_vision);
            this.panel2.Controls.Add(this.textBox_rangemax);
            this.panel2.Location = new System.Drawing.Point(141, 59);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(206, 348);
            this.panel2.TabIndex = 23;
            // 
            // textBox_ammo
            // 
            this.textBox_ammo.Location = new System.Drawing.Point(88, 174);
            this.textBox_ammo.Name = "textBox_ammo";
            this.textBox_ammo.Size = new System.Drawing.Size(100, 20);
            this.textBox_ammo.TabIndex = 8;
            this.textBox_ammo.TextChanged += new System.EventHandler(this.ValidateInput);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(14, 177);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(36, 13);
            this.label14.TabIndex = 21;
            this.label14.Text = "Ammo";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 203);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(64, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Action Point";
            // 
            // textBox_actionpoint
            // 
            this.textBox_actionpoint.Location = new System.Drawing.Point(88, 200);
            this.textBox_actionpoint.Name = "textBox_actionpoint";
            this.textBox_actionpoint.Size = new System.Drawing.Size(100, 20);
            this.textBox_actionpoint.TabIndex = 23;
            this.textBox_actionpoint.TextChanged += new System.EventHandler(this.ValidateInput);
            // 
            // UnitEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 419);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.dataGridView_unitdmg);
            this.Controls.Add(this.button_done);
            this.Controls.Add(this.label_selectedUnit);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.listBox_unitSelect);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UnitEditor";
            this.Text = "UnitEditor";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_unitdmg)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox_cost;
        private System.Windows.Forms.TextBox textBox_move;
        private System.Windows.Forms.TextBox textBox_vision;
        private System.Windows.Forms.TextBox textBox_rangemax;
        private System.Windows.Forms.TextBox textBox_gas;
        private System.Windows.Forms.TextBox textBox_fuelperturn;
        private System.Windows.Forms.ListBox listBox_unitSelect;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.TextBox textBox_rangemin;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label_selectedUnit;
        private System.Windows.Forms.Button button_done;
        private System.Windows.Forms.DataGridView dataGridView_unitdmg;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox_ammo;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox_actionpoint;
        private System.Windows.Forms.Label label8;
    }
}