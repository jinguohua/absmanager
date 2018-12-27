using System;
using System.Linq;
using System.Windows.Forms;

namespace ReleaseTool
{
    public partial class ModifyVersionForm : Form
    {
        public ModifyVersionForm(string oldVersion)
        {
            InitializeComponent();
            this.textBoxVersion.Text = oldVersion;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        public string GetNewVersion()
        {
            var newVersion = this.textBoxVersion.Text.Trim();
            var args = newVersion.Split('.').ToList();
            var intArgs = 0;
            args.ForEach(x => Common.Assert(int.TryParse(x, out intArgs) && intArgs >= 0 && intArgs <= 65535, "版本号参数必须大于0小于65535"));
            return newVersion;
        }
    }
}
