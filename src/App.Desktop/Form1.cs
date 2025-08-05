using App.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App.Desktop
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            var input = txtbxInput.Text;
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Please enter input data");
            }

            var output = EncryptionUtility.Encrypt(input, ConfigurationUtility.GetAppSettingValue("EncryptionKey"));
            txtbxOutput.Text = output;
            txtbxOutput.ReadOnly = true;
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            var input = txtbxInput.Text;
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Please enter input data");
            }

            var output = EncryptionUtility.Decrypt(input, ConfigurationUtility.GetAppSettingValue("EncryptionKey"));
            txtbxOutput.Text = output;
            txtbxOutput.ReadOnly = true;
        }
    }
}
