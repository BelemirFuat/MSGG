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

        TextBox searchBox = new TextBox();

        Panel messagePanel = new Panel();

        FlowLayoutPanel flowPanelContacts = new FlowLayoutPanel();
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


            panelChat.Width = this.Width - panelContacts.Width-35;
            panelChat.Height = this.Height-25;
            panelChat.Location = new Point(panelContacts.Width + 10, 10);
            panelChat.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);
            this.Controls.Add(panelChat);



            flowPanelContacts.Dock = DockStyle.Fill;
            flowPanelContacts.AutoScroll = true;  // Kaydırma çubuğu eklemek için
            flowPanelContacts.Padding = new Padding(10);
            panelContacts.Controls.Add(flowPanelContacts);

            searchBox.Width = panelContacts.Width - 35;
            searchBox.Location = new Point(10, 10);
            searchBox.ForeColor = ColorTranslator.FromHtml(LavenderTextColor); 
            searchBox.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);
            searchBox.BorderStyle = BorderStyle.FixedSingle;
            searchBox.PlaceholderText = "Ara...";
            flowPanelContacts.Controls.Add(searchBox);

            messagePanel.Dock = DockStyle.Fill;
            messagePanel.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);
            panelChat.Controls.Add(messagePanel);


            //örnek kişi listesi. kişi listesi çekme özelliği gelince burası değişecek
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
                        LoadMessages(contactId, messagePanel);
                    }
                };
            }
        }

        private void LoadMessages(int contactId, Panel messagePanel)
        {
            // Mesaj panelini temizleyelim
            messagePanel.Controls.Clear();

            // Başlık çubuğu ekleyelim (Kişi ismiyle)
            Label titleLabel = new Label();
            titleLabel.Text = "Kişi " + contactId;
            titleLabel.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
            titleLabel.Font = new Font("Arial", 26, FontStyle.Bold);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(10, 10);
            titleLabel.Width = messagePanel.Width - 40; // Panel genişliğine göre ayar
            titleLabel.Padding = new Padding(15);
            titleLabel.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
            messagePanel.Controls.Add(titleLabel);

            // Mesajları ekleyelim (örnek veriler)
            FlowLayoutPanel messagesFlowPanel = new FlowLayoutPanel();

            messagesFlowPanel.Location = new Point(10 , 35 + titleLabel.Height);
            messagesFlowPanel.Width = messagePanel.Width - 40; // Panel genişliğine göre ayar
            messagesFlowPanel.Height = messagePanel.Height - 100; // Başlık ve alt boşluk için ayar

            messagesFlowPanel.AutoScroll = true;
            messagePanel.Controls.Add(messagesFlowPanel);

            // Örnek mesajlar (her bir mesaj bir Label olarak eklenebilir)
            for (int i = 0; i < 100; i++) // 10 mesaj örneği
            {
                Label messageLabel = new Label();
                messageLabel.Text = "Kişi " + contactId + " ile mesaj " + (i + 1);
                messageLabel.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
                messageLabel.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
                messageLabel.Width = messagePanel.Width - 40;
                messageLabel.Margin = new Padding(10);
                messagesFlowPanel.Controls.Add(messageLabel);
            }
        }
    }
}
