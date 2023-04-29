using System;
using System.Threading;
using System.Windows.Forms;

namespace WorkAndRest
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            var mutex = new Mutex(true, "OnlyRun");

            if (!mutex.WaitOne(0, false))
            {
                MessageBox.Show("程序已打开，请勿重复点击!", "提示");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
