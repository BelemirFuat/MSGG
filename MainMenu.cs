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
            panelContacts.Dock = DockStyle.Left;
            panelContacts.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor); 
            this.Controls.Add(panelContacts);

            panelChat.Dock = DockStyle.Fill;
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



            //örnek kişi listesi. kişi listesi çekme özelliği gelince burası değişecek
            for (int i = 0; i < 100; i++) // 10 kişi örneği
            {
                Button contactButton = new Button();
                contactButton.Text = "Kişi " + (i + 1);
                contactButton.Width = flowPanelContacts.Width - 35;  // Panel genişliğine göre ayar
                contactButton.Margin = new Padding(0, 5, 0, 5);  // Her buton arasında boşluk
                contactButton.FlatStyle = FlatStyle.Flat;
                contactButton.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
                contactButton.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
                flowPanelContacts.Controls.Add(contactButton);
            }
        }
    }
}
