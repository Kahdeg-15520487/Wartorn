using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wartorn
{
    public partial class DropdownDialog : Form
    {
        public string Prompt
        {
            get
            {
                return this.label_prompt.Text;
            }
            set
            {
                this.label_prompt.Text = value;
            }
        }

        private string input = string.Empty;
        public string Selected
        {
            get
            {
                return input;
            }
        }

        public IEnumerable<string> Options
        {
            set
            {
                foreach (string item in value)
                {
                    this.comboBox1.Items.Add(item);
                }
				comboBox1.SelectedIndex = 0;
            }
        }

        public DropdownDialog()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            input = comboBox1.SelectedItem.ToString();
        }
    }
}
