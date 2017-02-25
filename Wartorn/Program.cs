using System;

namespace Wartorn
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new GameManager())
            {
                Utility.Constants.Width = 800;
                Utility.Constants.Height = 600;
                game.Run();
            }
        }
    }
}
