﻿using System;
using System.Threading;
using System.Windows.Forms;

namespace WorkAndRest
{
    public partial class FormTips : Form
    {
        private int _time;

        private Thread threadHide, threadShow;

        public FormTips(int time, string msg)
        {
            InitializeComponent();

            _time = time;

            label1.Text = msg;

            Width = label1.Width + 15;
            Height = label1.Height + 20;

            Left = Screen.PrimaryScreen.WorkingArea.Width;
            Top = 100;
        }

        private void FormTips_Load(object sender, EventArgs e)
        {
            ShowInTaskbar = false;

            threadHide = new Thread(() =>
            {
                Thread.Sleep(_time);
                
                for(double i = 1; i >= 0; i -= 0.1)
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

                for (int left = screenWidth; left >= screenWidth - (Width + 20); left -= 20)
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
