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
        //the thread that the game run on
        private Thread GameThread = null;

        //this is a hack-ish solution to allow the gamethread to call winform function;
        //reference to this form, the background handler
        private static Handler frm;
        //delegate represent the function that open a file dialog
        public delegate void MainThreadOperation(MessageEventArgs e);

        public Handler()
        {
            frm = this;
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
                CONTENT_MANAGER.togglebackgroundform += ToggleForm;
                game.Run();
            }
        }

        private static void ToggleForm(object sender, MessageEventArgs e)
        {
            MainThreadOperation temp = frm.ToggleFormMainThread;
            frm.Invoke(temp, e);
        }

        private void ToggleFormMainThread(MessageEventArgs e)
        {
            this.allowshowdisplay = true;
            this.Visible = !this.Visible;
        }


        //protected override void SetVisibleCore(bool value)
        //{
        //    //base.SetVisibleCore(allowshowdisplay ? value : allowshowdisplay);
            
        //    //MessageBox.Show("lala");
        //}       

        private static void MessageShow(object sender, MessageEventArgs e)
        {
            MessageBox.Show(e.message);
        }

        private static void OpenFile(object sender, MessageEventArgs e)
        {
            MainThreadOperation temp = OpenFileMainThread;
            frm.Invoke(temp, e);
        }

        private static void OpenFileMainThread(MessageEventArgs e)
        {
            var lala = new OpenFileDialog();
            lala.InitialDirectory = e.message;
            try
            {
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
            catch (Exception err)
            {
                e.message = string.Empty;
                Utility.HelperFunction.Log(err);
                throw;
            }
        }

        private void Handler_Shown(object sender, EventArgs e)
        {
            //MessageBox.Show("lala");
            this.Hide();
            if (GameThread != null)
            {

            }
            else
            {
                GameThread = new Thread(StartGame);
                GameThread.Start();
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
