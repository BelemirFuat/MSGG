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
        FlowLayoutPanel flowPanelContacts = new FlowLayoutPanel();
        FlowLayoutPanel messagesFlowPanel = new FlowLayoutPanel();


        TextBox searchBox = new TextBox();
        TextBox messageInput = new TextBox();

        Button sendButton = new Button();

        // variable definitions
        private int? activeContactId = null;


        public MainMenu()
        {
            InitializeComponent();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            this.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);
            //this.FormBorderStyle = FormBorderStyle.None; 
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
            messageInput.PlaceholderText = "Mesajınızı yazın..."; // Placeholder text
            panelChat.Controls.Add(messageInput);
            messageInput.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    sendButton.PerformClick();
                    e.SuppressKeyPress = true; // "ding" sesini engeller
                }
            };

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
                sendMessage();
            };

            //flowPanelContacts.Dock = DockStyle.Fill;
            flowPanelContacts.Location = new Point(10, 10);
            flowPanelContacts.Width = panelContacts.Width - 20; // Panel genişliğine göre ayar
            flowPanelContacts.Height = panelContacts.Height - 60; // Panel yüksekliğine göre ayar
            flowPanelContacts.AutoScroll = true;  // Kaydırma çubuğu eklemek için
            flowPanelContacts.Padding = new Padding(10);
            panelContacts.Controls.Add(flowPanelContacts);

            searchBox.Width = panelContacts.Width - 35;
            searchBox.Location = new Point(10, 10);
            searchBox.Margin = new Padding(0, 5, 0, 5);  // Her buton arasında boşluk

            searchBox.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
            searchBox.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);
            searchBox.BorderStyle = BorderStyle.FixedSingle;
            searchBox.PlaceholderText = "Ara...";
            flowPanelContacts.Controls.Add(searchBox);

            // messagePanel.Dock = DockStyle.Fill;
            messagePanel.Location = new Point(10, 10);
            messagePanel.Width = panelChat.Width - 20; // Panel genişliğine göre ayar
            messagePanel.Height = panelChat.Height - sendButton.Height - 60; // Panel yüksekliğine göre ayar
            messagePanel.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);
            panelChat.Controls.Add(messagePanel);


            //örnek kişi listesi.
            //TODO : kişi listesi çekme özelliği gelince burası değişecek
            for (int i = 0; i < 100; i++) // 10 kişi örneği
            {
                Button contactButton = new Button();
                contactButton.Text = "Kişi " + (i + 1);
                contactButton.Tag = i + 1;
                contactButton.Width = flowPanelContacts.Width - 35;  // Panel genişliğine göre ayar
                contactButton.Margin = new Padding(0, 5, 0, 5);  // Her buton arasında boşluk
                contactButton.FlatStyle = FlatStyle.Flat;
                contactButton.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
                contactButton.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
                flowPanelContacts.Controls.Add(contactButton);

                contactButton.Click += (s, ev) =>
                {
                    Button clickedButton = s as Button; // veya (Button)s

                    if (clickedButton != null && clickedButton.Tag is int contactId)
                    {
                        activeContactId = contactId;
                        LoadMessages(contactId, messagePanel);
                    }
                };
            }
        }

        private void LoadMessages(int contactId, Panel messagePanel)
        {
            // Mesaj panelini temizleyelim
            messagePanel.Controls.Clear();
            messagesFlowPanel.Controls.Clear();

            // Başlık çubuğu ekleyelim (Kişi ismiyle)
            Label titleLabel = new Label();
            titleLabel.Text = "Kişi " + contactId; //TODO : kişi ismi çekilecek
            titleLabel.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
            titleLabel.Font = new Font("Arial", 26, FontStyle.Bold);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(10, 10);
            titleLabel.Width = messagePanel.Width - 40; // Panel genişliğine göre ayar
            titleLabel.Padding = new Padding(15);
            titleLabel.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
            messagePanel.Controls.Add(titleLabel);

            // Mesajları ekleyelim (örnek veriler)

            messagesFlowPanel.Location = new Point(0, titleLabel.Height);
            messagesFlowPanel.Width = messagePanel.Width; // Panel genişliğine göre ayar
            messagesFlowPanel.Height = messagePanel.Height - 60; // Başlık ve alt boşluk için ayar

            messagesFlowPanel.AutoScroll = true;
            messagePanel.Controls.Add(messagesFlowPanel);

            // Örnek mesajlar (her bir mesaj bir Label olarak eklenebilir)
            // TODO : alan kim gönderen kim belirtilmeli
            for (int i = 0; i < 10; i++)
            {
                bool isSentByMe = i % 2 == 0; // Örnek olarak yarısı bizden
                string text = isSentByMe ? $"Sen: Merhaba {i}" : $"Kişi {contactId}: Selam {i}";
                Panel bubble = CreateMessageBubble(text, isSentByMe);
                messagesFlowPanel.Controls.Add(bubble);
            }
        }

        public void sendMessage()
        {
            // TODO : mesaj gönderme işlemi
            if (activeContactId == null)
            {
                MessageBox.Show("Lütfen önce bir kişi seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string newMessage = messageInput.Text.Trim();
            if (!string.IsNullOrEmpty(newMessage))
            {
                // Yeni mesajı oluştur ve panelin en sonuna ekle
                Label sentMessage = new Label();
                Panel messageBubble = CreateMessageBubble("Sen: " + newMessage, true);
                messagesFlowPanel.Controls.Add(messageBubble);
                messageInput.Text = ""; // Kutuyu temizle
            }
        }

        private Panel CreateMessageBubble(string text, bool isSentByMe)
        {
            Panel bubble = new Panel();
            Label messageLabel = new Label();

            messageLabel.Text = text;
            messageLabel.AutoSize = true;
            messageLabel.MaximumSize = new Size(400, 0); // Uzun mesajlar için satır kaydırma
            messageLabel.Font = new Font("Arial", 14);
            messageLabel.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
            messageLabel.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
            messageLabel.Padding = new Padding(10);
            messageLabel.Margin = new Padding(0);

            bubble.AutoSize = true;
            bubble.Padding = new Padding(0);
            bubble.Margin = new Padding(10);
            bubble.BackColor = Color.Transparent;

            // Yön belirleme
            if (isSentByMe)
            {
                bubble.Dock = DockStyle.Right;
                messageLabel.TextAlign = ContentAlignment.MiddleRight;
            }
            else
            {
                bubble.Dock = DockStyle.Left;
                messageLabel.TextAlign = ContentAlignment.MiddleLeft;
            }

            bubble.Controls.Add(messageLabel);
            return bubble;
        }

    }
}