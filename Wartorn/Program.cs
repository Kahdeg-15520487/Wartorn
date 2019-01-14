using System;

namespace Wartorn {
	/// <summary>
	/// The main class.
	/// </summary>
	public static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			using (var game = new GameManager()) {
				Constants.Width = 720;
				Constants.Height = 480;
				CONTENT_MANAGER.ParseArguments(args);
				game.Run();
			}
		}
	}
}
