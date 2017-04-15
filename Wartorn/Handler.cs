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
                CONTENT_MANAGER.handler += MessageShow;
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
