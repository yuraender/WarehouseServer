using System;
using System.Drawing;
using System.Windows.Forms;
using WarehouseServer.Properties;

namespace WarehouseServer.Server {
    public partial class ChangeDatabase : Form {
        public ChangeDatabase() {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        private void ChangeDatabase_Load(object sender, EventArgs e) {
            ipBox.Text = Settings.Default.DbServer;
            userBox.Text = Settings.Default.DbUser;
            passwordBox.Text = Settings.Default.DbPassword;
            databaseBox.Text = Settings.Default.DbDatabase;
        }

        private void updateButton_Click(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(ipBox.Text) || string.IsNullOrEmpty(userBox.Text)
                || string.IsNullOrEmpty(passwordBox.Text) || string.IsNullOrEmpty(databaseBox.Text)) {
                MessageBox.Show("Введены неверные данные.");
                return;
            }
            Settings.Default.DbServer = ipBox.Text;
            Settings.Default.DbUser = userBox.Text;
            Settings.Default.DbPassword = passwordBox.Text;
            Settings.Default.DbDatabase = databaseBox.Text;
            Settings.Default.Save();
            Close();
        }

        private void ChangeDatabase_FormClosed(object sender, FormClosedEventArgs e) {
            MessageBox.Show("Вы изменили соединение с базой данных. Программа будет завершена.");
            Environment.Exit(0);
        }
    }
}
