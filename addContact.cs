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
        public addContact()
        {
            InitializeComponent();
        }
        private TextBox contactNameTextBox;
        private TextBox contactIdTextBox;
        private Button saveButton;
        private Button cancelButton;

        private void addContact_Load(object sender, EventArgs e)
        {
            this.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);
            this.contactNameTextBox = new TextBox 
            { 
                PlaceholderText = "Enter Name", 
                Width = 200, Top = 10, Left = 10, 
                BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor), 
                ForeColor = ColorTranslator.FromHtml(LavenderTextColor) 
            };
            this.contactIdTextBox = new TextBox 
            { 
                PlaceholderText = "Enter ID", 
                Width = 200, Top = 40, Left = 10,
                BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor),
                ForeColor = ColorTranslator.FromHtml(LavenderTextColor)
            };
            this.saveButton = new Button 
            { 
                Text = "Save", 
                Top = 70, Left = 10,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor),
                ForeColor = ColorTranslator.FromHtml(LavenderTextColor),
                FlatAppearance = { BorderSize = 0 }
            };
            this.cancelButton = new Button 
            { 
                Text = "Cancel", 
                Top = 70, Left = 120,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor),
                ForeColor = ColorTranslator.FromHtml(LavenderTextColor),
                FlatAppearance = { BorderSize = 0 }
            };

            this.saveButton.Click += SaveButton_Click;
            this.cancelButton.Click += CancelButton_Click;

            this.Controls.Add(contactNameTextBox);
            this.Controls.Add(contactIdTextBox);
            this.Controls.Add(saveButton);
            this.Controls.Add(cancelButton);

            this.Text = "Add Contact";
            this.Size = new Size(250, 150);
        }

        private void SaveButton_Click(object sender, EventArgs e)
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
                // Pass the data back to main form
                ContactAdded?.Invoke(this, new ContactEventArgs(name, id));
                this.Close();
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