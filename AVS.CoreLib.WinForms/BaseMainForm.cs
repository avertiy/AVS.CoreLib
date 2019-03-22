using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AVS.CoreLib.WinForms
{
    public partial class BaseMainForm : Form
    {
        [Category("Appearance")]
        [Localizable(true)]
        [DefaultValue(null)]
        public string NotifyIconText
        {
            get => notifyIcon.Text;
            set => notifyIcon.Text = value;
        }

        [Category("Appearance")]
        [Localizable(true)]
        [DefaultValue(null)]
        public Icon NotifyIcon
        {
            get => notifyIcon.Icon;
            set => notifyIcon.Icon = value;
        }

        public BaseMainForm()
        {
            InitializeComponent();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BaseMainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }
    }
}
