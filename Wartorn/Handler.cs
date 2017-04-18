using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wartorn
{
    public partial class Handler : Form
    {
        private bool allowshowdisplay = false;

        public Handler()
        {
            InitializeComponent();
        }

        static void StartGame(object handler)
        {
            using (var game = new GameManager())
            {
                Constants.Width = 720;
                Constants.Height = 480;
                CONTENT_MANAGER.messagebox += MessageShow;
                CONTENT_MANAGER.fileopendialog += OpenFile;
                game.Run();
            }
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(allowshowdisplay ? value : allowshowdisplay);
            Thread theThread = new Thread(StartGame);
            theThread.Start();
        }

        private static void MessageShow(object sender, MessageEventArgs e)
        {
            MessageBox.Show(e.message);
        }

        private static void OpenFile(object sender,MessageEventArgs e)
        {
            var lala = new OpenFileDialog();
            lala.InitialDirectory = e.message;
            if (lala.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    e.message = lala.FileName;
                }
                catch (Exception er)
                {
                    e.message = string.Empty;
                    Utility.HelperFunction.Log(er);
                }
            }
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public string message = string.Empty;
        public MessageEventArgs(string e)
        {
            message = e;
        }
    }
}
