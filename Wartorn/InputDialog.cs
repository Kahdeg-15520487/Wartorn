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
    public partial class InputDialog : Form
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

        public string Input
        {
            get
            {
                return this.textBox_Input.Text;
            }
            set
            {
                this.textBox_Input.Text = value;
            }
        }

        public InputDialog()
        {
            InitializeComponent();
        }
    }
}
