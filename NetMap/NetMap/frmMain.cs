using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetMap
{
    public partial class frmMain : Form
    {
        const string SELECT = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const char NO_LETTER = '_';
        const char AUTO_LETTER = '*';

        public frmMain(char DriveLetter, string Path, string Username, string Password)
        {
            var All = Array.ConvertAll(SELECT.ToCharArray(), delegate (char c) { return (object)c; });
            InitializeComponent();
            cbLetter.Items.Add("(auto)");
            cbLetter.Items.AddRange(All);
            cbLetter.SelectedIndex = 0;

            if (SELECT.IndexOf(DriveLetter) >= 0)
            {
                cbLetter.SelectedItem = DriveLetter;
            }


            tbPath.Text = Path;

            if (!string.IsNullOrEmpty(Username))
            {
                tbUsername.Text = Username;
            }

            if (!string.IsNullOrEmpty(Password))
            {
                tbPassword.Text = Password;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Program.ErrorCode = ErrorCodes.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            char Letter;
            if (cbLetter.SelectedIndex == 0)
            {
                Letter = AUTO_LETTER;//GetFirstFreeDrive();
            }
            else
            {
                Letter = (char)cbLetter.SelectedItem;
            }
            if (Letter == NO_LETTER)
            {
                MessageBox.Show("There are no drive letters available on your system.\r\nDisconnect some drives and try again", "No free drive letter", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (string.IsNullOrEmpty(tbUsername.Text) && !string.IsNullOrEmpty(tbPassword.Text))
                {
                    if (MessageBox.Show("The username is missing but is required, if a password is given.\r\nUse your current User name?", "No Username given", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        return;
                    }
                    tbUsername.Text = Environment.UserName;
                }
                MapDrive(Letter, tbPath.Text, tbUsername.Text, tbPassword.Text);
                Close();
            }
        }

        private void MapDrive(char letter, string path, string name, string pass)
        {
            Process P = new Process();
            P.StartInfo.UseShellExecute = false;
            P.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            P.StartInfo.CreateNoWindow = true;

            P.StartInfo.FileName = "net.exe";

            StringBuilder SB = new StringBuilder();

            //Drive Letter
            if (letter == AUTO_LETTER)
            {
                SB.Append("* ");
            }
            else
            {
                SB.AppendFormat("{0}: ", letter);
            }

            //Path
            SB.AppendFormat("\"{0}\" ", path);

            //Username
            if (!string.IsNullOrEmpty(name))
            {
                //Password (only if name given, must be first)
                if (!string.IsNullOrEmpty(pass))
                {
                    SB.AppendFormat("\"{0}\" ", pass);
                }

                SB.AppendFormat("\"/USER:{0}\" ", name);
            }

            //Other arguments
            SB.Append("/PERSISTENT:NO ");

            P.StartInfo.Arguments = "USE " + SB.ToString();

            P.Start();
            P.WaitForExit();
            Close();
        }

        /*
        //There is no need to do this manually, as the net command supports auto-assigned drive letters,
        //but if you want, you can uncomment this and detect the next free letter automatically yourself.
        private IEnumerable<char> GetDriveLetters()
        {
            return Array.ConvertAll(DriveInfo.GetDrives().Select(m => m.RootDirectory).ToArray(), delegate (DirectoryInfo D) { return D.FullName.ToUpper()[0]; }).Distinct();
        }

        private char GetFirstFreeDrive()
        {
            var DI = GetDriveLetters();
            for (int i = SELECT.Length; i > 0; i--)
            {
                if (!DI.Contains(SELECT[i - 1]))
                {
                    return SELECT[i - 1];
                }
            }
            return NO_LETTER;
        }
        //*/
    }
}
