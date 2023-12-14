using System;
using System.Threading;
using System.Windows.Forms;

namespace WorkAndRest
{
    public partial class FormMain : Form
    {
        // 程序中频繁需要用到的变量，分别为“当前状态结束的时间（毫秒级时间戳）”、“工作的时间（分钟）”、“休息的时间（分钟）”、“代表结束时间到开始时间的时间间隔，即timestamp-当前时间戳”
        private long timestamp, gi = 50, xi = 10, spacing = 50 * 60 * 1000;

        // 是否允许关闭程序
        private bool isClose = false;
        // 当前的计时线程
        private Thread threadTimekeeping = null;
        // 当前计时器线程的状态，为false是线程结束
        private bool threadState = false;
        // 计时器
        public static long timekeeping;
        // 当前状态(工作/休息) 默认为休息
        public static bool state = false;
        

        public FormMain()
        {
            InitializeComponent();

            buttonOver.Enabled = buttonPause.Enabled = false;
            checkBox1.Checked = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isClose) e.Cancel = true;
            Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxGTime.SelectedIndex = 3;
            comboBoxXTime.SelectedIndex = 1;
        }

        private void 退出程序ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isClose = true;

            if (threadTimekeeping != null)
            {
                threadTimekeeping.Abort();
            }

            Close();
        }

        private void 显示主界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
        }


        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (buttonPause.Text == "暂停") PauseTimekeeping();
            else ContinueTimekeeping();
        }

        private void PauseTimekeeping()
        {
            buttonPause.Text = "继续";

            // 防止线程结束失败， 这里终止线程继续执行的条件
            threadState = false;

            threadTimekeeping = null;
        }

        private void ContinueTimekeeping()
        {
            buttonPause.Text = "暂停";

            threadState = true;
            timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            timekeeping = timestamp + spacing;

            BeginTimekeeping();

        }

        private void ButtonOver_Click(object sender, EventArgs e)
        {
            // 结束计时器线程
            timestamp = timekeeping + 10000;
            threadTimekeeping = null;

            label4.Text = "倒计时: 00:00";

            // 更改按钮状态
            buttonOver.Enabled = buttonPause.Enabled = false;
            buttonBegin.Enabled = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            state = !checkBox1.Checked;
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormSetting().Show();
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) Show();
        }

        private void BeginTimekeeping()
        {

            threadTimekeeping = new Thread(() =>
            {
                while (threadState && timekeeping > timestamp)
                {
                    timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                    spacing = timekeeping - timestamp;

                    label4.Invoke(new Action(() =>
                    {
                        label4.Text = $"倒计时：{ spacing / 1000 / 60:D2}:{ (spacing / 1000) % 60:D2}";
                    }));

                    Thread.Sleep(500);
                }

                if(threadState) TimerEnd(checkBoxAuto.Checked);

                threadTimekeeping = null;
            });

            threadTimekeeping.Start();
        }

        // 当前状态的计时器结束，参数为true时表示自动开始下一个状态的计时
        private void TimerEnd(bool isAuto)
        {
            if (buttonBegin.Enabled) return;

            axWindowsMediaPlayer.URL = ".\\resources\\" + ( state ? Properties.Settings.Default.工作 : Properties.Settings.Default.休息 );

            var tips = state ? "工作时间结束了，快休息一会儿吧!" : "休息时间结束了，快去工作吧!";

            axWindowsMediaPlayer.Ctlcontrols.play();
            int durationInSeconds = Math.Max((int)axWindowsMediaPlayer.Ctlcontrols.currentItem.duration, 2);

            Thread.Sleep(1000);

            Invoke(new Action(() =>
            {
                new FormTips((int)Math.Floor(axWindowsMediaPlayer.currentMedia.duration) * durationInSeconds * 1000, tips).Show();
            }));

            if (isAuto) Button1_Click(null, null);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            buttonOver.Enabled = buttonPause.Enabled = true;
            buttonBegin.Enabled = false;

            // 根据当前选择项来计算工作时间和休息时间
            timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            switch (comboBoxGTime.Text)
            {
                case " 25 分钟": gi = 25; break;
                case " 30 分钟": gi = 30; break;
                case " 40 分钟": gi = 40; break;
                case " 50 分钟": gi = 50; break;
            }

            switch (comboBoxXTime.Text)
            {
                case " 5 分钟": xi = 5; break;
                case " 10 分钟": xi = 10; break;
                case " 15 分钟": xi = 15; break;
                case " 20 分钟": xi = 20; break;
                case " 30 分钟": xi = 30; break;
            }

            if (state)
                timekeeping = timestamp + gi * 60 * 1000;
            else
                timekeeping = timestamp + xi * 60 * 1000; 

            label3.Text = state ? "工作中" : "休息中";

            threadState = true;

            // 开始计时，初始时该值为false
            state = !state;

            if (state) checkBox1.Checked = false;

            BeginTimekeeping();
        }
    }
}
