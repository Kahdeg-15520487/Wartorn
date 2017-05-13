using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Wartorn;
using Wartorn.GameData;
using Wartorn.Utility;

namespace StatsBlancer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button_unit_Click(object sender, EventArgs e)
        {
            this.Hide();
            UnitEditor uniteditor = new UnitEditor();
            var dialogResult = uniteditor.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                //save
                MessageBox.Show("saved");
            }
            this.Show();
        }
    }
}
