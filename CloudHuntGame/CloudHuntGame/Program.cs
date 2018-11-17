using System;

namespace CloudHuntGame
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static CloudHunt game = new CloudHunt();

        [STAThread]
        static void Main()
        {
            using (game)
                game.Run();
        }
    }
#endif
}
