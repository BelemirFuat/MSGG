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
        string contactsDbPath = "Data Source="+ Path.Combine(Application.StartupPath, "cntcs.db");
        string messagesDbPath = "Data Source="+ Path.Combine(Application.StartupPath, "msg.db");
        private int myId = 0;



        public MainMenu()
        {
            InitializeComponent();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            pullFromFireBase();
            checkMyData();
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


            flowPanelContacts.Location = new Point(10, 10 + 30);
            flowPanelContacts.Width = panelContacts.Width - 20; 
            flowPanelContacts.Height = panelContacts.Height - 60 -25 ;
            flowPanelContacts.AutoScroll = true;  
            flowPanelContacts.Padding = new Padding(10);
            panelContacts.Controls.Add(flowPanelContacts);

            searchBox.Width = flowPanelContacts.Width - 35 - 30 ;
            searchBox.Location = new Point(20, 10);
            searchBox.Margin = new Padding(0, 5, 0, 5); 
            searchBox.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
            searchBox.BackColor = ColorTranslator.FromHtml(PrimaryBackgroundColor);
            searchBox.BorderStyle = BorderStyle.FixedSingle;
            searchBox.Font = new Font("Segoi UI", 12);
            searchBox.PlaceholderText = "Ara...";
            panelContacts.Controls.Add(searchBox);

            btnAddContact = new Button
            {
                Text = "+",
                Width = 25,
                Height = 25,
                Location = new Point(searchBox.Right + 10, searchBox.Top),
                BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor),
                ForeColor = ColorTranslator.FromHtml(LavenderTextColor),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5, 5, 0, 5)

            };
            btnAddContact.Click += btnAddContact_Click;

            panelContacts.Controls.Add(btnAddContact);

            messagePanel.Location = new Point(10, 10);
            messagePanel.Width = panelChat.Width - 20; 
            messagePanel.Height = panelChat.Height - sendButton.Height - 60; 
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

            InitializeContactsDatabase();
            LoadContacts();
        }
        private void checkMyData()
        {
            try
            {
                // Open a connection to the database
                using (var conn = new SQLiteConnection(cntcsDbPath))
                {
                    conn.Open();
                    // Query to retrieve your ID from the 'myData' table
                    string query = "SELECT id FROM myData LIMIT 1"; // Adjust column and table name as necessary

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        // Execute the query and retrieve the ID
                        var result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            myId = Convert.ToInt32(result);
                            Console.WriteLine($"My ID is: {myId}");
                        }
                        else
                        {
                            giveMeAnId();
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"cntcs databasesinde sorun var! : {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void giveMeAnId()
        {

        }

        private void btnAddContact_Click(object sender, EventArgs e)
        {
            addContact addContactForm = new addContact();
            addContactForm.ContactAdded += AddContactForm_ContactAdded;
            addContactForm.Show();
        }

        private void AddContactForm_ContactAdded(object sender, ContactEventArgs e)
        {
            // Get new contact data from AddContactForm
            string newName = e.Name;
            int newId = e.Id;

            // Save to database
            try
            {
                using (var conn = new SQLiteConnection(contactsDbPath))
                {
                    conn.Open();
                    string insertContactQuery = "INSERT INTO contacts (name, id) VALUES (@name, @id)";
                    using (var cmd = new SQLiteCommand(insertContactQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", newName);
                        cmd.Parameters.AddWithValue("@id", newId);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Reload contacts to show the new contact
                LoadContacts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding contact: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            filter = filter.Trim();
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

            string contactName = "Bilinmeyen";

            // Kişi ismini cntcs veritabanından çek
            try
            {
                using (SQLiteConnection cntcsConn = new SQLiteConnection("Data Source=cntcs.db"))
                {
                    cntcsConn.Open();
                    string nameQuery = "SELECT name FROM contacts WHERE id = @id";
                    using (SQLiteCommand cmd = new SQLiteCommand(nameQuery, cntcsConn))
                    {
                        cmd.Parameters.AddWithValue("@id", contactId);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                            contactName = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kişi ismi yüklenirken bir hata oluştu:\n" + ex.Message);
            }

            Label titleLabel = new Label();
            titleLabel.Text = contactName;
            titleLabel.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
            titleLabel.Font = new Font("Arial", 26, FontStyle.Bold);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(10, 10);
            titleLabel.Width = messagePanel.Width - 40;
            titleLabel.Padding = new Padding(15);
            titleLabel.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
            messagePanel.Controls.Add(titleLabel);

            messagesFlowPanel.Location = new Point(0, titleLabel.Bottom);
            messagesFlowPanel.Width = messagePanel.Width;
            messagesFlowPanel.Height = messagePanel.Height - titleLabel.Height - 20;
            messagesFlowPanel.AutoScroll = true;
            messagePanel.Controls.Add(messagesFlowPanel);

            string tableName = $"messages_ID_{contactId}";

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(messagesDbPath))
                {
                    conn.Open();
                    string query = $"CREATE TABLE IF NOT EXISTS {tableName} (" +
                                   "senderId INT, " +
                                   "messageDate DATETIME, " +
                                   "messageContent VARCHAR(256))";
                    new SQLiteCommand(query, conn).ExecuteNonQuery();

                    string selectQuery = $"SELECT * FROM {tableName} ORDER BY messageDate";
                    using (SQLiteCommand cmd = new SQLiteCommand(selectQuery, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int senderId = reader.GetInt32(0);
                            string msg = reader.GetString(2);
                            bool isSentByMe = senderId == myId;
                            Panel bubble = CreateMessageBubble(msg, isSentByMe);
                            messagesFlowPanel.Controls.Add(bubble);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mesajlar yüklenirken bir hata oluştu:\n" + ex.Message);
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

                        // Dinamik tablo ismi
                        string tableName = $"messages_ID_{activeContactId}";

                        // Tabloyu yoksa oluştur
                        string createTableQuery = $@"
                    CREATE TABLE IF NOT EXISTS {tableName} (
                        senderId INT,
                        messageDate DATETIME,
                        messageContent VARCHAR(256)
                    )";
                        using (var createCmd = new SQLiteCommand(createTableQuery, conn))
                        {
                            createCmd.ExecuteNonQuery();
                        }

                        // Mesajı ekle
                        string insertMessage = $@"
                    INSERT INTO {tableName} (senderId, messageDate, messageContent) 
                    VALUES (@senderId, @date, @content)";
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