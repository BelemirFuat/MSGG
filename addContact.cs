using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSGG
{
    public partial class addContact : Form
    {
        string PrimaryBackgroundColor = "#1E1E2E";
        string SecondaryBackgroundColor = "#313244";
        string LavenderTextColor = "#CDD6F4";

        private TextBox contactNameTextBox;
        private TextBox contactIdTextBox;
        private Button saveButton;
        private Button cancelButton;
        private Label statusLabel;
        private FirebaseHelper firebaseHelper;

        public addContact()
        {
            InitializeComponent();
            firebaseHelper = new FirebaseHelper();
        }

        private void addContact_Load(object sender, EventArgs e)
        {
            this.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);

            this.contactNameTextBox = new TextBox
            {
                PlaceholderText = "Enter Name",
                Width = 200,
                Top = 10,
                Left = 10,
                BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor),
                ForeColor = ColorTranslator.FromHtml(LavenderTextColor)
            };

            this.contactIdTextBox = new TextBox
            {
                PlaceholderText = "Enter ID",
                Width = 200,
                Top = 40,
                Left = 10,
                BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor),
                ForeColor = ColorTranslator.FromHtml(LavenderTextColor)
            };

            this.saveButton = new Button
            {
                Text = "Save",
                Top = 70,
                Left = 10,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor),
                ForeColor = ColorTranslator.FromHtml(LavenderTextColor),
                FlatAppearance = { BorderSize = 0 }
            };

            this.cancelButton = new Button
            {
                Text = "Cancel",
                Top = 70,
                Left = 120,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor),
                ForeColor = ColorTranslator.FromHtml(LavenderTextColor),
                FlatAppearance = { BorderSize = 0 }
            };

            this.statusLabel = new Label
            {
                Text = "",
                AutoSize = true,
                Top = 105,
                Left = 10,
                ForeColor = ColorTranslator.FromHtml(LavenderTextColor)
            };

            this.saveButton.Click += SaveButton_Click;
            this.cancelButton.Click += CancelButton_Click;
            this.contactIdTextBox.Leave += ContactIdTextBox_Leave;

            this.Controls.Add(contactNameTextBox);
            this.Controls.Add(contactIdTextBox);
            this.Controls.Add(saveButton);
            this.Controls.Add(cancelButton);
            this.Controls.Add(statusLabel);

            this.Text = "Add Contact";
            this.Size = new Size(250, 180);
        }

        private async void ContactIdTextBox_Leave(object sender, EventArgs e)
        {
            // Verify if ID exists in Firebase when user moves away from the ID field
            if (int.TryParse(contactIdTextBox.Text.Trim(), out int id))
            {
                statusLabel.Text = "Checking user...";
                statusLabel.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);

                try
                {
                    string userName = await firebaseHelper.GetUserNameAsync(id);

                    if (!string.IsNullOrEmpty(userName))
                    {
                        statusLabel.Text = $"Found: {userName}";
                        statusLabel.ForeColor = Color.LightGreen;

                        // Pre-fill the name field if it's empty
                        if (string.IsNullOrEmpty(contactNameTextBox.Text))
                        {
                            contactNameTextBox.Text = userName;
                        }
                    }
                    else
                    {
                        statusLabel.Text = "User not found!";
                        statusLabel.ForeColor = Color.Salmon;
                    }
                }
                catch (Exception ex)
                {
                    statusLabel.Text = "Error checking user";
                    statusLabel.ForeColor = Color.Salmon;
                    Console.WriteLine($"Firebase error: {ex.Message}");
                }
            }
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            string name = contactNameTextBox.Text.Trim();
            string idText = contactIdTextBox.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(idText))
            {
                MessageBox.Show("Please fill out all fields.");
                return;
            }

            if (int.TryParse(idText, out int id))
            {
                saveButton.Enabled = false;
                statusLabel.Text = "Verifying user...";

                try
                {
                    // Verify the user exists in Firebase
                    string userName = await firebaseHelper.GetUserNameAsync(id);

                    if (string.IsNullOrEmpty(userName))
                    {
                        DialogResult result = MessageBox.Show(
                            "This user ID doesn't exist in the system. Add anyway?",
                            "User Not Found",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (result == DialogResult.No)
                        {
                            saveButton.Enabled = true;
                            statusLabel.Text = "Cancelled";
                            return;
                        }
                    }

                    // Pass the data back to main form
                    ContactAdded?.Invoke(this, new ContactEventArgs(name, id));
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error verifying user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    saveButton.Enabled = true;
                    statusLabel.Text = "Error occurred";
                }
            }
            else
            {
                MessageBox.Show("Invalid ID.");
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Event for notifying that contact has been added
        public event EventHandler<ContactEventArgs> ContactAdded;
    }

    public class ContactEventArgs : EventArgs
    {
        public string Name { get; }
        public int Id { get; }

        public ContactEventArgs(string name, int id)
        {
            Name = name;
            Id = id;
        }
    }
}