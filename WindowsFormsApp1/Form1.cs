using System;
using System.Threading;
using System.Windows.Forms;

namespace WorkAndRest
{
    public partial class FormTips : Form
    {
        private readonly int _time;
        private Thread threadHide, threadShow;
        private readonly AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer;

        public FormTips(int time, string msg, AxWMPLib.AxWindowsMediaPlayer ax)
        {
            InitializeComponent();

            // 不在任务栏显示窗口
            ShowInTaskbar = false;
            // 使窗口始终保持在最前
            TopMost = true;

            _time = time;
            axWindowsMediaPlayer = ax;

            label1.Text = msg;

            Width = label1.Width + 15;
            Height = label1.Height + 20;

            Left = Screen.PrimaryScreen.WorkingArea.Width;
            Top = 100;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // WS_EX_TOOLWINDOW: 防止窗口显示在任务栏
                // WS_EX_NOACTIVATE: 防止窗口激活和获取焦点
                cp.ExStyle |= 0x80 | 0x08000000;
                return cp;
            }
        }

        private void FormTips_Load(object sender, EventArgs e)
        {
            threadHide = new Thread(() =>
            {
                Thread.Sleep(_time);

                for (double i = 1; i >= 0; i -= 0.1)
                {
                    Thread.Sleep(20);
                    Invoke(new Action(() =>
                    {
                        Opacity = i;
                    }));
                }

                Close();
            });

            threadShow = new Thread(() =>
            {
                int screenWidth = Left;

                for (int left = screenWidth; left >= screenWidth - (Width + 30); left -= 20)
                {
                    Left = left;
                    Thread.Sleep(10);
                }

                threadHide.Start();
                threadHide.Join();
            });

            threadShow.Start();
        }

        private void HideWindow()
        {
            threadHide.Abort();

            for (double i = 1; i >= 0; i -= 0.1)
            {
                Thread.Sleep(20);
                Invoke(new Action(() =>
                {
                    Opacity = i;
                }));
            }

            axWindowsMediaPlayer.Ctlcontrols.stop();
            Close();
        }

        private void Label1_Click(object sender, EventArgs e)
        {
            HideWindow();
        }

        private void FormTips_Click(object sender, EventArgs e)
        {
            HideWindow();
        }
    }
}
