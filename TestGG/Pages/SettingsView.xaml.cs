using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Linq;

namespace TestGG.Pages
{
    public partial class SettingsView : Page
    {
        private readonly string configFilePath = "db_settings.xml";
        public SettingsView()
        {
            InitializeComponent();
            LoadSettings();
        }
        public string Password
        {
            get { return txtPassword.Password; }
            set { txtPassword.Password = value; }
        }
        private async void ConnectToDatabase_Click(object sender, RoutedEventArgs e)
        {
            // Validate fields based on the selected database type
            if (rbLocal.IsChecked == true)
            {
                txtServer.Text = "localhost";

                if (string.IsNullOrWhiteSpace(txtServer.Text))
                {
                    txtStatus.Text = "Please provide a valid server name (e.g., localhost).";
                    return;
                }
            }
            else if (rbRemote.IsChecked == true)
            {
                // Remote database: Validate all fields
                if (string.IsNullOrWhiteSpace(txtServer.Text) ||
                    string.IsNullOrWhiteSpace(txtUsername.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    txtStatus.Text = "Please fill in all fields for remote database connection.";
                    return;
                }
            }

            try
            {
                btnConnectLocal.IsEnabled = false;
                txtStatus.Text = "Connecting...";

                string connectionString;

                if (rbLocal.IsChecked == true)
                {
                    // Use Windows Authentication for local database
                    connectionString = $"Server={txtServer.Text};Integrated Security=True;Connection Timeout=30;";
                }
                else
                {
                    // Use SQL Server Authentication for remote database
                    connectionString = $"Server={txtServer.Text};User Id={txtUsername.Text};Password={txtPassword.Password};Connection Timeout=30;";
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    txtStatus.Text = "Connection successful!";
                    txtStatus.Foreground = System.Windows.Media.Brushes.Green;

                    // Save settings only if the connection is successful
                    SaveSettings(txtServer.Text, txtUsername.Text, txtPassword.Password);
                }
            }
            catch (SqlException ex)
            {
                txtStatus.Text = $"Connection failed: {ex.Message}";
                txtStatus.Foreground = System.Windows.Media.Brushes.Red;
            }
            finally
            {
                btnConnectLocal.IsEnabled = true;
            }
        }

        private void DatabaseType_Checked(object sender, RoutedEventArgs e)
        {
            if (rbLocal.IsChecked == true)
            {
                // Default server name for local database
                txtServer.Text = "localhost";
                remoteDatabasePanel.Visibility = Visibility.Collapsed;

                // Clear username and password fields
                txtUsername.Clear();
                txtPassword.Clear();
            }
            else if (rbRemote.IsChecked == true)
            {
                remoteDatabasePanel.Visibility = Visibility.Visible;

                // Clear username and password fields
                txtUsername.Clear();
                txtPassword.Clear();
            }
        }

        private void SaveSettings(string server, string username, string password)
        {
            try
            {
                // Encrypt the password before saving
                string encryptedPassword = SecurityHelper.Encrypt(password);

                XDocument xmlDoc = new XDocument(
                    new XElement("DatabaseSettings",
                        new XElement("Server", server),
                        new XElement("Username", username),
                        new XElement("Password", encryptedPassword)
                    )
                );

                xmlDoc.Save(configFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSettings()
        {
            if (!File.Exists(configFilePath)) return;

            try
            {
                XDocument xmlDoc = XDocument.Load(configFilePath);
                var dbSettings = xmlDoc.Element("DatabaseSettings");
                if (dbSettings != null)
                {
                    txtServer.Text = dbSettings.Element("Server")?.Value ?? "localhost";
                    txtUsername.Text = dbSettings.Element("Username")?.Value;

                    string encryptedPassword = dbSettings.Element("Password")?.Value;
                    txtPassword.Password = encryptedPassword != null ? SecurityHelper.Decrypt(encryptedPassword) : string.Empty;

                    // Automatically select the database type based on the saved settings
                    if (string.IsNullOrEmpty(txtUsername.Text) && string.IsNullOrEmpty(txtPassword.Password))
                    {
                        rbLocal.IsChecked = true;
                        remoteDatabasePanel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        rbRemote.IsChecked = true;
                        remoteDatabasePanel.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtServer_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void SaveAPICreds(object sender, RoutedEventArgs e)
        {

        }
    }
}
