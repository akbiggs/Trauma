namespace Trauma.Engine
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameEngine game = new GameEngine(800, 640, true))
            {
                game.Run();
            }
        }
    }
#endif
}

