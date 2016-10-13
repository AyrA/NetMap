using System;
using System.Windows.Forms;

namespace NetMap
{
    public struct ErrorCodes
    {
        public const int OK = 0;
        public const int Cancel = 1;
        public const int Help = 2;
    }

    static class Program
    {
        public static int ErrorCode = ErrorCodes.OK;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
#if DEBUG
            args = @"*|\\localhost\C$".Split('|');
            AyrA.IO.Terminal.CreateConsole();
#else
            AyrA.IO.Terminal.AttachToConsole(AyrA.IO.Terminal.PARENT);
#endif
            if (args.Length < 2 || args[0] == "/?")
            {
                Console.WriteLine(@"NetMap <Letter>[:] <Path> [Username] [Password]

Maps Network Drives with given user credentials.
Prompts the user to input name and password.

Letter    - Drive letter (A-Z). The Colon is optional.
            Using an invalid letter will attempt to mount from Z backwards.
Path      - UNC Path to mount
username  - Username prefilled into the form
Password  - Password prefilled into the form

Note: The user is always asked for credentials even if ones are given here.
Hello Max!");
                ErrorCode = ErrorCodes.Help;
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //To be honest, not only the colon, but everything in the drive letter argument is optional.
                //"Seriously" will map the drive to "S" (if available)
                Application.Run(new frmMain(args[0].ToUpper()[0], args[1], args.Length > 2 ? args[2] : null, args.Length > 3 ? args[3] : null));
            }
            return ErrorCode;
        }
    }
}
