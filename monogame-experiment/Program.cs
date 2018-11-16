using System;
using System.Globalization;
using System.Threading;


namespace monogame_experiment.Desktop
{
    // Main class, do not edit
    public static class Program
    {
        // Main
        [STAThread]
        static void Main()
        {
            // Set culture
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            // Start game
            using (var game = new Runner())
                game.Run();
        }
    }
}
