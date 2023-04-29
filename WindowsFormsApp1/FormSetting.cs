using System;
using System.IO;
using System.Windows.Forms;

namespace WorkAndRest
{
    public partial class FormSetting : Form
    {
        public FormSetting()
        {
            InitializeComponent();
        }

        private void FormSetting_Load(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(".\\resources");

            foreach (string path in files)
            {
                string name = Path.GetFileName(path), suffix = Path.GetExtension(path);

                if (suffix != ".wav" && suffix != ".mp3") continue;

                comboBox1.Items.Add(name);
                if (name == Properties.Settings.Default.工作) comboBox1.SelectedIndex = comboBox1.Items.Count - 1;

                comboBox2.Items.Add(name);
                if (name == Properties.Settings.Default.休息) comboBox2.SelectedIndex = comboBox1.Items.Count - 1;
            }

            // 事件在这里注册的原因是，上方初始化会导致索引变更，会重复触发
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += ComboBox2_SelectedIndexChanged;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            axWindowsMediaPlayer.URL = ".\\resources\\" + comboBox1.Text;
            axWindowsMediaPlayer.Ctlcontrols.play();
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            axWindowsMediaPlayer.URL = ".\\resources\\" + comboBox2.Text;
            axWindowsMediaPlayer.Ctlcontrols.play();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.工作 = comboBox1.Text;
            Properties.Settings.Default.休息 = comboBox2.Text;
            Properties.Settings.Default.Save();
            Close();
        }
    }
}
