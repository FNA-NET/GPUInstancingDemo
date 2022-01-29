using System;

namespace GPUInstancing.Desktop
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var g = new Game1())
            {
                g.Run();
            }
        }
    }
}
