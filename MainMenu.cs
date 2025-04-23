using System.Data.SQLite;

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
        Panel addContactPanel = new Panel();

        FlowLayoutPanel flowPanelContacts = new FlowLayoutPanel();
        FlowLayoutPanel messagesFlowPanel = new FlowLayoutPanel();


        TextBox searchBox = new TextBox();
        TextBox messageInput = new TextBox();

        Button sendButton = new Button();
        Button btnAddContact = new Button();
        // variable definitions
        private int? activeContactId = null;
        string contactsDbPath = "Data Source=cntcs.db;";
        string messagesDbPath = "Data Source=msg.db;";



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

            searchBox.Width = panelContacts.Width - 35 - 30 ;
            searchBox.Location = new Point(10, 10);
            searchBox.Margin = new Padding(0, 5, 0, 5);  // Her buton arasında boşluk

            searchBox.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
            searchBox.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);
            searchBox.BorderStyle = BorderStyle.FixedSingle;
            searchBox.PlaceholderText = "Ara...";
            flowPanelContacts.Controls.Add(searchBox);

            btnAddContact = new Button
            {
                Text = "+",
                Width = 30,
                Height = 30,
                Location = new Point(searchBox.Right + 10, searchBox.Top)
            };
            btnAddContact.Click += (s, e) =>
            {
                addContactPanel.Visible = true;
                addContactPanel.Location = new Point(this.Width/2-addContactPanel.Width / 2, this.Height / 2 - addContactPanel.Height / 2);
            };
            flowPanelContacts.Controls.Add(btnAddContact);

            // messagePanel.Dock = DockStyle.Fill;
            messagePanel.Location = new Point(10, 10);
            messagePanel.Width = panelChat.Width - 20; // Panel genişliğine göre ayar
            messagePanel.Height = panelChat.Height - sendButton.Height - 60; // Panel yüksekliğine göre ayar
            messagePanel.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);
            panelChat.Controls.Add(messagePanel);

            messagesFlowPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Location = new Point(0, 70),
                Width = messagePanel.Width,
                Height = messagePanel.Height - 80,
                BackColor = Color.Transparent,
            };

            addContactPanel = new Panel
            {
                Size = new Size(250, 150),
                BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor),
                Location = new Point(this.Width,this.Height),
                Visible = false
            };

            Label lblName = new Label { Text = "İsim:", Location = new Point(10, 10) };
            TextBox txtName = new TextBox { Location = new Point(70, 10), Width = 150 };

            Label lblId = new Label { Text = "ID:", Location = new Point(10, 40) };
            TextBox txtId = new TextBox { Location = new Point(70, 40), Width = 150 };

            Button btnSave = new Button { Text = "Kaydet", Location = new Point(30, 80), Width = 80 };
            Button btnCancel = new Button { Text = "İptal", Location = new Point(130, 80), Width = 80 };

            // Kaydet
            btnSave.Click += (s, e) =>
            {
                if (int.TryParse(txtId.Text, out int id))
                {
                    AddContact(txtName.Text.Trim(), id);
                    txtName.Clear();
                    txtId.Clear();
                    addContactPanel.Visible = false;
                    addContactPanel.Location = new Point(this.Width, this.Height);
                }
                else
                {
                    MessageBox.Show("ID geçerli bir sayı olmalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            // İptal
            btnCancel.Click += (s, e) =>
            {
                txtName.Clear();
                txtId.Clear();
                addContactPanel.Visible = false;
                addContactPanel.Location = new Point(this.Width, this.Height);

            };

            // Panel'e elemanları ekle
            addContactPanel.Controls.Add(lblName);
            addContactPanel.Controls.Add(txtName);
            addContactPanel.Controls.Add(lblId);
            addContactPanel.Controls.Add(txtId);
            addContactPanel.Controls.Add(btnSave);
            addContactPanel.Controls.Add(btnCancel);
            this.Controls.Add(addContactPanel);
            InitializeContactsDatabase();
            LoadContacts();
        }

        private void AddContact(string name, int id)
        {
            try
            {
                using (var conn = new SQLiteConnection(contactsDbPath))
                {
                    conn.Open();
                    string insertQuery = "INSERT INTO contacts (name, id) VALUES (@name, @id)";
                    using (var cmd = new SQLiteCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Kişi başarıyla eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadContacts(searchBox.Text); // Yeniden yükle
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kişi eklenirken bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void InitializeContactsDatabase()
        {
            string cntcsPath = Path.Combine(Application.StartupPath, "cntcs.db");
            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={cntcsPath};Version=3;"))
            {
                conn.Open();
                string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS contacts (
                name VARCHAR(50),
                id INT
            );
        ";

                using (SQLiteCommand cmd = new SQLiteCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadContacts(string filter = "")
        {
            flowPanelContacts.Controls.Clear();

            try
            {
                using (var conn = new SQLiteConnection(contactsDbPath))
                {
                    conn.Open();
                    string query = "";
                    if(filter == "")
                    {
                        query = "SELECT name, id FROM contacts";
                    }
                    else
                    {
                        query = "SELECT name, id FROM contacts WHERE name LIKE @filter";
                    }
                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader["name"].ToString();
                            int id = Convert.ToInt32(reader["id"]);

                            Button contactButton = new Button();
                            contactButton.Text = name;
                            contactButton.Tag = id;
                            contactButton.Width = flowPanelContacts.Width - 35;
                            contactButton.Margin = new Padding(0, 5, 0, 5);
                            contactButton.FlatStyle = FlatStyle.Flat;
                            contactButton.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
                            contactButton.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
                            flowPanelContacts.Controls.Add(contactButton);

                            contactButton.Click += (s, ev) =>
                            {
                                activeContactId = id;
                                LoadMessages(id, messagePanel);
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kişiler yüklenirken bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadMessages(int contactId, Panel messagePanel)
        {
            messagePanel.Controls.Clear();
            messagesFlowPanel.Controls.Clear();

            try
            {
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

                using (var conn = new SQLiteConnection(messagesDbPath))
                {
                    conn.Open();
                    string query = "SELECT senderId, messageDate, messageContent FROM message_schema";
                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int senderId = Convert.ToInt32(reader["senderId"]);
                            string content = reader["messageContent"].ToString();
                            bool isMe = senderId == 0;

                            Panel bubble = CreateMessageBubble(content, isMe);
                            messagesFlowPanel.Controls.Add(bubble);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mesajlar yüklenirken bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void sendMessage()
        {

            if (activeContactId == null) return;

            try
            { 

                string newMessage = messageInput.Text.Trim();
                if (!string.IsNullOrEmpty(newMessage))
                {
                    using (var conn = new SQLiteConnection(messagesDbPath))
                    {
                        conn.Open();
                        string insertMessage = "INSERT INTO message_schema (senderId, messageDate, messageContent) VALUES (@senderId, @date, @content)";
                        using (var cmd = new SQLiteCommand(insertMessage, conn))
                        {
                            cmd.Parameters.AddWithValue("@senderId", 0); // 0 = biz
                            cmd.Parameters.AddWithValue("@date", DateTime.Now);
                            cmd.Parameters.AddWithValue("@content", newMessage);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    messageInput.Text = "";
                    LoadMessages((int)activeContactId, messagePanel); // Güncelle
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mesaj gönderilirken bir hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CreateMessageBubble(string text, bool isSentByMe)
        {
            Label messageLabel = new Label
            {
                Text = text,
                AutoSize = true,
                MaximumSize = new Size(400, 0),
                Font = new Font("Arial", 14),
                ForeColor = ColorTranslator.FromHtml(LavenderTextColor),
                BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor),
                Padding = new Padding(10),
                TextAlign = ContentAlignment.MiddleLeft,
            };

            Panel bubble = new Panel
            {
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(10),
                Padding = new Padding(0),
            };
            bubble.Controls.Add(messageLabel);

            // Mesajı sağa ya da sola yasla
            if (isSentByMe)
            {
                bubble.Padding = new Padding(messagePanel.Width - 40 - messageLabel.Width, 0, 0, 0); // Sola boşluk = sağa yaslanır
            }
            else
            {
                bubble.Padding = new Padding(0, 0, 100, 0); // Sağa boşluk = sola yaslanır
            }

            return bubble;
        }


    }
}