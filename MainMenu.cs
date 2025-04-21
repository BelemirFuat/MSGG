using System;
using System.Drawing;
using System.Windows.Forms;

namespace MSGG
{
    public partial class MainMenu : Form
    {
        // color definitions
        string PrimaryBackgroundColor = "#1E1E2E";
        string SecondaryBackgroundColor = "#313244";
        string LavenderTextColor = "#CDD6F4";

        // element definitions
        Panel panelContacts = new Panel();
        Panel panelChat = new Panel();
        Panel messagePanel = new Panel();
        Panel messagesPanel = new Panel(); // Mesajların eklendiği scrollable panel
        FlowLayoutPanel flowPanelContacts = new FlowLayoutPanel();

        TextBox searchBox = new TextBox();
        TextBox messageInput = new TextBox();

        Button sendButton = new Button();
        int currentContactId = -1;

        public MainMenu()
        {
            InitializeComponent();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            this.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1000, 600);

            panelContacts.Size = new Size(250, this.Height);
            panelContacts.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
            this.Controls.Add(panelContacts);

            panelChat.Width = this.Width - panelContacts.Width - 35;
            panelChat.Height = this.Height - 25;
            panelChat.Location = new Point(panelContacts.Width + 10, 10);
            panelChat.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);
            this.Controls.Add(panelChat);

            messageInput.Name = "messageInput";
            messageInput.Width = panelChat.Width - 130;
            messageInput.Height = 30;
            messageInput.Location = new Point(10, panelChat.Height - 60);
            messageInput.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
            messageInput.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
            messageInput.Font = new Font("Arial", 14);
            messageInput.BorderStyle = BorderStyle.FixedSingle;
            messageInput.PlaceholderText = "Mesajınızı yazın...";
            panelChat.Controls.Add(messageInput);

            sendButton.Text = "Gönder";
            sendButton.Width = 100;
            sendButton.Height = 30;
            sendButton.Font = new Font("Arial", 12, FontStyle.Bold);
            sendButton.Location = new Point(messageInput.Right + 10, messageInput.Top);
            sendButton.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
            sendButton.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
            sendButton.FlatStyle = FlatStyle.Flat;
            panelChat.Controls.Add(sendButton);

            sendButton.Click += (s, e) =>
            {
                if (currentContactId != -1)
                {
                    string newMessage = messageInput.Text.Trim();
                    if (!string.IsNullOrEmpty(newMessage))
                    {
                        Panel messageBubble = CreateMessageBubble("Sen: " + newMessage, true);
                        messagesPanel.Controls.Add(messageBubble);
                        messageBubble.BringToFront();
                        messageInput.Text = "";
                    }
                }
            };

            flowPanelContacts.Location = new Point(10, 10);
            flowPanelContacts.Width = panelContacts.Width - 20;
            flowPanelContacts.Height = panelContacts.Height - 60;
            flowPanelContacts.AutoScroll = true;
            flowPanelContacts.Padding = new Padding(10);
            panelContacts.Controls.Add(flowPanelContacts);

            searchBox.Width = panelContacts.Width - 35;
            searchBox.Location = new Point(10, 10);
            searchBox.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
            searchBox.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);
            searchBox.BorderStyle = BorderStyle.FixedSingle;
            searchBox.PlaceholderText = "Ara...";
            flowPanelContacts.Controls.Add(searchBox);

            messagePanel.Location = new Point(10, 10);
            messagePanel.Width = panelChat.Width - 20;
            messagePanel.Height = panelChat.Height - sendButton.Height - 70;
            messagePanel.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);
            panelChat.Controls.Add(messagePanel);

            for (int i = 0; i < 100; i++)
            {
                Button contactButton = new Button();
                contactButton.Text = "Kişi " + (i + 1);
                contactButton.Tag = i + 1;
                contactButton.Width = flowPanelContacts.Width - 35;
                contactButton.Margin = new Padding(0, 5, 0, 5);
                contactButton.FlatStyle = FlatStyle.Flat;
                contactButton.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
                contactButton.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
                flowPanelContacts.Controls.Add(contactButton);

                contactButton.Click += (s, ev) =>
                {
                    Button clickedButton = s as Button;
                    if (clickedButton != null && clickedButton.Tag is int contactId)
                    {
                        currentContactId = contactId;
                        LoadMessages(contactId, messagePanel);
                    }
                };
            }
        }

        private void LoadMessages(int contactId, Panel messagePanel)
        {
            messagePanel.Controls.Clear();

            Label titleLabel = new Label();
            titleLabel.Text = "Kişi " + contactId;
            titleLabel.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
            titleLabel.Font = new Font("Arial", 26, FontStyle.Bold);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(10, 10);
            titleLabel.Padding = new Padding(15);
            titleLabel.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
            messagePanel.Controls.Add(titleLabel);

            messagesPanel = new FlowLayoutPanel();
           //messagesPanel.FlowDirection = FlowDirection.TopDown;
            //messagesPanel.WrapContents = false;
            messagesPanel.AutoScroll = true;
            messagesPanel.Location = new Point(0, titleLabel.Bottom + 10);
            messagesPanel.Width = messagePanel.Width;
            messagesPanel.Height = messagePanel.Height - titleLabel.Height - 20;
            messagesPanel.BackColor = Color.Transparent;
            messagePanel.Controls.Add(messagesPanel);

            for (int i = 0; i < 10; i++)
            {
                bool isSentByMe = i % 2 == 0;
                string text = isSentByMe ? $"Sen: Merhaba {i}" : $"Kişi {contactId}: Selam {i}";
                Panel bubble = CreateMessageBubble(text, isSentByMe);
                messagesPanel.Controls.Add(bubble);
            }
        }


        private Panel CreateMessageBubble(string text, bool isSentByMe)
        {
            Panel bubblePanel = new Panel();
            bubblePanel.AutoSize = true;
            bubblePanel.MaximumSize = new Size(500, 0);
            bubblePanel.Padding = new Padding(5);
            bubblePanel.Margin = new Padding(10);

            Label messageLabel = new Label();
            messageLabel.Text = text;
            messageLabel.AutoSize = true;
            messageLabel.MaximumSize = new Size(400, 0);
            messageLabel.Font = new Font("Arial", 12);
            messageLabel.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
            messageLabel.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
            messageLabel.Padding = new Padding(10);
            messageLabel.TextAlign = ContentAlignment.MiddleLeft;

            bubblePanel.Controls.Add(messageLabel);

            if (isSentByMe)
            {
                bubblePanel.Dock = DockStyle.Right;
                bubblePanel.Anchor = AnchorStyles.Right;
                bubblePanel.Padding = new Padding(150, 5, 10, 5); // sağa yaslı
            }
            else
            {
                bubblePanel.Dock = DockStyle.Left;
                bubblePanel.Anchor = AnchorStyles.Left;
                bubblePanel.Padding = new Padding(10, 5, 150, 5); // sola yaslı
            }

            return bubblePanel;
        }

    }
}
