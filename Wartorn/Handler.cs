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
                CONTENT_MANAGER.promptbox += InputBox;
                CONTENT_MANAGER.dropdownbox += DropdownBox;
                game.Run();
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            frm.ShowInTaskbar = false;
            frm.Opacity = 0;
        }

        private static void MessageShow(object sender, MessageEventArgs e)
        {
            DialogResult result = MessageBox.Show(e.message);
            e.message = result.ToString();
        }

        #region Open file dialog
        private static void OpenFile(object sender, MessageEventArgs e)
        {
            MainThreadOperation temp = OpenFileMainThread;
            frm.Invoke(temp, e);
        }

        private static void OpenFileMainThread(MessageEventArgs e)
        {
            var filedialog = new OpenFileDialog();
            filedialog.InitialDirectory = e.message;
            try
            {
                if (filedialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        e.message = filedialog.FileName;
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
        #endregion

        #region InputDialog

        private static void InputBox(object sender, MessageEventArgs e)
        {
            MainThreadOperation temp = ShowInputBoxMainThread;
            frm.Invoke(temp, e);
        }

        private static void ShowInputBoxMainThread(MessageEventArgs e)
        {
            InputDialog inputDialog = new InputDialog();
            inputDialog.Prompt = e.message;
            var result = inputDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                e.message = inputDialog.Input;
            }
            else
            {
                e.message = result.ToString();
            }
        }
        #endregion

        #region DropdownDialog

        private static void DropdownBox(object sender, MessageEventArgs e)
        {
            MainThreadOperation temp = ShowDropdownBoxMainThread;
            frm.Invoke(temp, e);
        }

        private static void ShowDropdownBoxMainThread(MessageEventArgs e)
        {
            DropdownDialog dropdownDialog = new DropdownDialog();
            var options = e.message.Split('|').ToList();
            dropdownDialog.Prompt = options[0];
            options.RemoveAt(0);
            dropdownDialog.Options = options;
            var result = dropdownDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                e.message = dropdownDialog.Selected;
            }
            else
            {
                e.message = result.ToString();
            }
        }

        #endregion

        private void Handler_Shown(object sender, EventArgs e)
        {
            //MessageBox.Show("lala");
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
