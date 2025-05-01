using System.Data.SQLite;
using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Protobuf.WellKnownTypes;



namespace MSGG
{
    public partial class MainMenu : Form
    {
        // color definitions
        string PrimaryBackgroundColor = "#1E1E2E";
        string SecondaryBackgroundColor = "#313244";
        string LavenderTextColor = "#CDD6F4";
        string secondaryTextColor = "#f5e0dc";


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
        string contactsDbPath = "Data Source=" + Path.Combine(Application.StartupPath, "cntcs.db");
        string messagesDbPath = "Data Source=" + Path.Combine(Application.StartupPath, "msg.db");
        private int myId = 0;

        // class definitions
        private FirebaseHelper firebaseHelper;
        private IDisposable messageSubscription;

        public MainMenu()
        {
            InitializeComponent();
            firebaseHelper = new FirebaseHelper();
        }

        private async void MainMenu_Load(object sender, EventArgs e)
        {
            createApperanceSettings();

            pullFromFireBase();
            checkMyData();
            InitializeContactsDatabase();
            InitializeMessagesDatabase();

            await firebaseHelper.UpdateUserLastActiveAsync(myId);

            // Set up a timer to check for online status
            System.Windows.Forms.Timer onlineStatusTimer = new System.Windows.Forms.Timer();
            onlineStatusTimer.Interval = 30000; // Check every 30 seconds
            onlineStatusTimer.Tick += async (s, ev) =>
            {
                await CheckContactsOnlineStatus();
            };
            onlineStatusTimer.Start();


            LoadContacts();
        }
        public void createApperanceSettings()
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


            flowPanelContacts.Location = new Point(10, 10 + 30);
            flowPanelContacts.Width = panelContacts.Width - 20;
            flowPanelContacts.Height = panelContacts.Height - 60 - 25;
            flowPanelContacts.AutoScroll = true;
            flowPanelContacts.Padding = new Padding(10);
            panelContacts.Controls.Add(flowPanelContacts);

            searchBox.Width = flowPanelContacts.Width - 35 - 30;
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
            messagePanel.Controls.Add(messagesFlowPanel);

            label1.BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor);
            label1.ForeColor = ColorTranslator.FromHtml(LavenderTextColor);
            label1.Font = new Font("Arial", 12);
        }

        private async Task CheckContactsOnlineStatus()
        {
            try
            {
                var allUsers = await firebaseHelper.GetAllUsersAsync();

                foreach (Control control in flowPanelContacts.Controls)
                {
                    if (control is Button contactButton)
                    {
                        int contactId = (int)contactButton.Tag;
                        var user = allUsers.FirstOrDefault(u => u.Id == contactId);

                        if (user != null)
                        {
                            // If user was active in the last 2 minutes, consider them online
                            bool isOnline = (DateTime.UtcNow - user.LastActive).TotalMinutes < 2;

                            // Update UI to show online status
                            // For simplicity, just change button color
                            contactButton.BackColor = isOnline
                                ? ColorTranslator.FromHtml("#2E7D32")  // Green for online
                                : ColorTranslator.FromHtml(SecondaryBackgroundColor);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking online status Line 206: {ex.Message}");
            }
        }

        void InitializeMessagesDatabase()
        {
            string msgPath = messagesDbPath;
            using (SQLiteConnection conn = new SQLiteConnection(msgPath))
            {
                conn.Open();
                // No need to create specific message tables here,
                // they'll be created when needed in the sendMessage method
            }
        }

        private async void pullFromFireBase()
        {
            try
            {
                // If we have a local ID, make sure our user exists in Firebase
                if (myId > 0)
                {
                    var userName = await firebaseHelper.GetUserNameAsync(myId);
                    if (string.IsNullOrEmpty(userName))
                    {
                        // Our user isn't in Firebase yet, ask for name and save
                        string name = AskUserName();
                        await firebaseHelper.SaveUserNameAsync(myId, name);
                    }

                    // Sync local contacts to cloud
                    var contacts = GetLocalContacts();
                    await firebaseHelper.SyncLocalContactsAsync(myId, contacts);

                    // Check for any cloud contacts not in local DB
                    await SyncCloudContactsToLocal();

                    label1.Text = myId.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Firebase sync error Line 246: {ex.Message}");
                // Continue without Firebase
            }
        }

        private List<Contact> GetLocalContacts()
        {
            var contacts = new List<Contact>();

            try
            {
                using (var conn = new SQLiteConnection(contactsDbPath))
                {
                    conn.Open();
                    string query = "SELECT name, id FROM contacts";

                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contacts.Add(new Contact
                            {
                                Name = reader["name"].ToString(),
                                Id = Convert.ToInt32(reader["id"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving local contacts Line 278: {ex.Message}");
            }

            return contacts;
        }

        // Method to sync cloud contacts to local DB
        private async Task SyncCloudContactsToLocal()
        {
            try
            {
                var cloudContacts = await firebaseHelper.GetContactsAsync(myId);
                if (cloudContacts == null || cloudContacts.Count == 0)
                    return;

                using (var conn = new SQLiteConnection(contactsDbPath))
                {
                    conn.Open();

                    // Get existing contact IDs
                    var existingIds = new HashSet<int>();
                    using (var cmd = new SQLiteCommand("SELECT id FROM contacts", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            existingIds.Add(Convert.ToInt32(reader["id"]));
                        }
                    }

                    // Add any missing contacts
                    foreach (var contact in cloudContacts)
                    {
                        if (!existingIds.Contains(contact.Id))
                        {
                            string insertQuery = "INSERT INTO contacts (name, id) VALUES (@name, @id)";
                            using (var cmd = new SQLiteCommand(insertQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@name", contact.Name);
                                cmd.Parameters.AddWithValue("@id", contact.Id);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // Refresh contacts display
                LoadContacts();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing cloud contacts Line 329: {ex.Message}");
            }
        }

        private void checkMyData()
        {
            try
            {
                // Open a connection to the database
                using (var conn = new SQLiteConnection(contactsDbPath))
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
                            label1.Text = myId.ToString();

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
                MessageBox.Show($"cntcs databasesinde sorun var! Line 363: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void giveMeAnId()
        {
            try
            {
                int newUserId = await GetNewUserIdAsync();

                // Save to SQLite
                using (var conn = new SQLiteConnection(contactsDbPath))
                {
                    conn.Open();
                    string insertQuery = "INSERT INTO myData (id) VALUES (@id)";
                    using (var cmd = new SQLiteCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", newUserId);
                        cmd.ExecuteNonQuery();
                    }
                }

                myId = newUserId; // Assign to your variable
                await RegisterUserAsync(AskUserName(), newUserId); // Or use a real name if you have one
                MessageBox.Show($"Yeni ID alındı: {newUserId}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                label1.Text = myId.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Yeni ID alınırken hata oluştu Line 391:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string AskUserName()
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Lütfen bir kullanıcı adı girin:",
                "Kullanıcı Adı",
                "Yeni Kullanıcı"
            );
            return input.Trim();
        }

        public async Task<int> GetNewUserIdAsync()
        {
            FirestoreDb db = FirestoreDb.Create("msgg-9424f");
            CollectionReference usersRef = db.Collection("users");

            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();
            HashSet<int> usedIds = new HashSet<int>();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (int.TryParse(doc.Id, out int userId))
                    usedIds.Add(userId);
            }

            int newId = 1;
            while (usedIds.Contains(newId))
            {
                newId++;
            }

            return newId;
        }
        public async Task RegisterUserAsync(string userName, int userId)
        {
            DocumentReference docRef = FirestoreDb.Create("msgg-9424fd").Collection("users").Document(userId.ToString());

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "name", userName },
                { "createdAt", Google.Cloud.Firestore.Timestamp.GetCurrentTimestamp() }
            };

            await docRef.SetAsync(data);
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
                MessageBox.Show($"Error adding contact: Line 473 {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    if (filter == "")
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
                MessageBox.Show($"Kişiler yüklenirken bir hata oluştu Line 544:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadMessages(int contactId, Panel messagePanel)
        {
            InitializeUIComponents();

            string contactName = GetContactName(contactId);
            SetupMessagePanelHeader(messagePanel, contactName);

            LoadMessagesFromDatabase(contactId);
            ScrollToLastMessage();

            SubscribeToFirebaseMessages(contactId);
        }

        // Initialize UI components and clear existing elements
        private void InitializeUIComponents()
        {
            messagePanel.Controls.Clear();
            messagesFlowPanel.Controls.Clear();
            messageSubscription?.Dispose();
        }

        private string GetContactName(int contactId)
        {
            try
            {
                using (var conn = new SQLiteConnection(contactsDbPath))
                {
                    conn.Open();
                    var cmd = new SQLiteCommand("SELECT name FROM contacts WHERE id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", contactId);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "Bilinmeyen";
                }
            }
            catch
            {
                return "Bilinmeyen";
            }
        }

        private void SetupMessagePanelHeader(Panel messagePanel, string contactName)
        {
            var titleLabel = new Label
            {
                Text = contactName,
                ForeColor = ColorTranslator.FromHtml(LavenderTextColor),
                Font = new Font("Arial", 26, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10),
                Padding = new Padding(15),
                BackColor = ColorTranslator.FromHtml(SecondaryBackgroundColor)
            };
            messagePanel.Controls.Add(titleLabel);

            messagesFlowPanel.Location = new Point(0, titleLabel.Bottom);
            messagesFlowPanel.Width = messagePanel.Width;
            messagesFlowPanel.Height = messagePanel.Height - titleLabel.Height - 20;
            messagesFlowPanel.AutoScroll = true;
            messagePanel.Controls.Add(messagesFlowPanel);
        }

        private void LoadMessagesFromDatabase(int contactId)
        {
            string tableName = $"messages_ID_{contactId}";
            DateTime? lastMessageDate = null;

            try
            {
                using (var conn = new SQLiteConnection(messagesDbPath))
                {
                    conn.Open();
                    new SQLiteCommand($@"
                CREATE TABLE IF NOT EXISTS {tableName} (
                     messageId TEXT PRIMARY KEY,

                    senderId INT,
                    messageDate DATETIME,
                    messageContent VARCHAR(256))", conn).ExecuteNonQuery();

                    var cmd = new SQLiteCommand($"SELECT * FROM {tableName} ORDER BY messageDate", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int senderId = reader.GetInt32(0);
                            DateTime msgTime = DateTime.Parse(reader.GetString(1));
                            string content = reader.GetString(2);

                            AddMessageToUI(content, senderId == myId, msgTime, ref lastMessageDate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mesajlar yüklenirken hata oluştu:\n" + ex.Message);
            }
        }

        private void SubscribeToFirebaseMessages(int contactId)
        {
            HashSet<string> seen = new HashSet<string>();
            messageSubscription = firebaseHelper.SubscribeToMessages(myId, contactId, newMessage =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    string hash = $"{newMessage.SenderId}_{newMessage.Timestamp.Ticks}_{newMessage.Content}";
                    if (seen.Contains(hash)) return;

                    seen.Add(hash);
                    SaveMessageToLocal(contactId, newMessage);

                    DateTime? lastDate = messagesFlowPanel.Controls
                        .OfType<Label>()
                        .Where(l => l.Font.Bold)
                        .Select(l => DateTime.TryParse(l.Text, out var dt) ? dt : (DateTime?)null)
                        .LastOrDefault();

                    AddMessageToUI(newMessage.Content, newMessage.SenderId == myId, newMessage.Timestamp, ref lastDate);
                    ScrollToLastMessage();
                });
            });
        }

        private void AddMessageToUI(string content, bool isSentByMe, DateTime timestamp, ref DateTime? lastMessageDate)
        {
            if (lastMessageDate == null || lastMessageDate.Value.Date != timestamp.Date)
            {
                messagesFlowPanel.Controls.Add(new Label
                {
                    Text = GetFriendlyDate(timestamp),
                    AutoSize = true,
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    ForeColor = ColorTranslator.FromHtml(secondaryTextColor),
                    Padding = new Padding(10),
                    TextAlign = ContentAlignment.MiddleCenter
                });
                lastMessageDate = timestamp;
            }

            var bubble = CreateMessageBubble(content, isSentByMe, timestamp);
            messagesFlowPanel.Controls.Add(bubble);
        }

        private void ScrollToLastMessage()
        {
            if (messagesFlowPanel.Controls.Count > 0)
            {
                messagesFlowPanel.ScrollControlIntoView(messagesFlowPanel.Controls[^1]);
            }
        }


        private void SaveMessageToLocal(int contactId, Message message)
        {
            try
            {
                using (var conn = new SQLiteConnection(messagesDbPath))
                {
                    conn.Open();
                    string tableName = $"messages_ID_{contactId}";

                    // Create table with messageId as primary key
                    string createTableQuery = $@"
            CREATE TABLE IF NOT EXISTS {tableName} (
                messageId TEXT PRIMARY KEY,
                senderId INT,
                messageDate DATETIME,
                messageContent VARCHAR(256)
            )";
                    using (var createCmd = new SQLiteCommand(createTableQuery, conn))
                    {
                        createCmd.ExecuteNonQuery();
                    }

                    // Insert message (avoid duplicates with messageId)
                    string insertMessage = $@"
            INSERT OR IGNORE INTO {tableName} (senderId, messageDate, messageContent, messageId) 
            VALUES (@senderId, @date, @content, @id)";
                    using (var cmd = new SQLiteCommand(insertMessage, conn))
                    {
                        cmd.Parameters.AddWithValue("@senderId", message.SenderId);
                        cmd.Parameters.AddWithValue("@date", message.Timestamp);
                        cmd.Parameters.AddWithValue("@content", message.Content);
                        cmd.Parameters.AddWithValue("@id", message.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving message to local DB Line 687: {ex.Message}");
            }
        }


        public async void sendMessage()
        {
            if (activeContactId == null) return;

            try
            {
                string newMessage = messageInput.Text.Trim();
                if (!string.IsNullOrEmpty(newMessage))
                {
                    DateTime timestamp = DateTime.UtcNow;
                    string messageId = GenerateMessageId(myId, timestamp, newMessage);

                    using (var conn = new SQLiteConnection(messagesDbPath))
                    {
                        conn.Open();

                        string tableName = $"messages_ID_{activeContactId}";

                        string createTableQuery = $@"
                    CREATE TABLE IF NOT EXISTS {tableName} (
                        messageId TEXT PRIMARY KEY,
                        senderId INT,
                        messageDate DATETIME,
                        messageContent VARCHAR(256)
                    )";
                        using (var createCmd = new SQLiteCommand(createTableQuery, conn))
                            createCmd.ExecuteNonQuery();

                        string insertMessage = $@"
                    INSERT OR IGNORE INTO {tableName} (messageId, senderId, messageDate, messageContent)
                    VALUES (@id, @senderId, @date, @content)";
                        using (var cmd = new SQLiteCommand(insertMessage, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", messageId);
                            cmd.Parameters.AddWithValue("@senderId", myId);
                            cmd.Parameters.AddWithValue("@date", timestamp);
                            cmd.Parameters.AddWithValue("@content", newMessage);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    try
                    {
                        Message firebaseMessage = new Message
                        (
                           messageId,
                           myId,
                           timestamp,
                           newMessage
                        );

                        await firebaseHelper.SendMessageAsync(myId, (int)activeContactId, firebaseMessage);
                    }
                    catch (Exception fbEx)
                    {
                        Console.WriteLine($"Firebase message sync failed Line 739: {fbEx.Message}");
                    }

                    messageInput.Text = "";
                    LoadMessages((int)activeContactId, messagePanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mesaj gönderilirken bir hata oluştu: Line 748\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private string GenerateMessageId(int senderId, DateTime timestamp, string content)
        {
            string input = $"{senderId}_{timestamp.Ticks}_{content}";
            using (System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hash).Replace("-", "").ToLower(); // compact string
            }
        }



        private Panel CreateMessageBubble(string text, bool isSentByMe, DateTime timestamp)
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

            Label timestampLabel = new Label
            {
                Text = timestamp.ToString("HH:mm"), // Only show time, like "14:32"
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Italic),
                ForeColor = ColorTranslator.FromHtml(secondaryTextColor),
                Margin = new Padding(5, 0, 0, 0),
            };

            FlowLayoutPanel messageContent = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
            };

            Panel bubble = new Panel
            {
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(10),
                Padding = new Padding(0),
            };

            messageContent.Controls.Add(messageLabel);
            messageContent.Controls.Add(timestampLabel);
            bubble.Controls.Add(messageContent);

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
        private string GetFriendlyDate(DateTime date)
        {
            if (date.Date == DateTime.Today)
                return "Today";
            else if (date.Date == DateTime.Today.AddDays(-1))
                return "Yesterday";
            else
                return date.ToString("MMMM dd, yyyy"); // Example: April 26, 2025
        }


        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            messageSubscription?.Dispose();
        }
    }
}