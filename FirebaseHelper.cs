using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using System.Diagnostics;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace MSGG
{
    internal class FirebaseHelper
    {
        private FirebaseClient firebase;
        private const string DatabaseUrl = "https://msgg-9424f-default-rtdb.firebaseio.com/";

        public FirebaseHelper()
        {
            string credentialPath = Path.Combine(Application.StartupPath, "msgg-9424f-firebase-adminsdk-fbsvc-d94785ffb2.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

            try
            {
                firebase = new FirebaseClient(DatabaseUrl,
                    new FirebaseOptions());
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(credentialPath)
                });

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Firebase initialization error: {ex.Message}");
                // Consider showing a message to the user that cloud features might not work
            }
        }

        // User methods
        public async Task SaveUserNameAsync(int id, string name)
        {
            try
            {
                await firebase
                    .Child("users")
                    .Child(id.ToString())
                    .PutAsync(new User { Name = name, LastActive = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving user name: {ex.Message}");
                throw; // Or handle as appropriate for your app
            }
        }

        public async Task<User> GetUserAsync(int id)
        {
            try
            {
                return await firebase
                    .Child("users")
                    .Child(id.ToString())
                    .OnceSingleAsync<User>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting user: {ex.Message}");
                return null;
            }
        }

        public async Task<string> GetUserNameAsync(int id)
        {
            var user = await GetUserAsync(id);
            return user?.Name;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                var users = await firebase
                    .Child("users")
                    .OnceAsync<User>();

                return users.Select(u => new User
                {
                    Id = int.Parse(u.Key),
                    Name = u.Object.Name,
                    LastActive = u.Object.LastActive
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting all users: {ex.Message}");
                return new List<User>();
            }
        }

        // Message methods
        public async Task SendMessageAsync(int senderId, int receiverId, string content)
        {
            try
            {
                string chatId = GetChatId(senderId, receiverId);
                var message = new Message
                {
                    SenderId = senderId,
                    Timestamp = DateTime.UtcNow,
                    Content = content
                };

                await firebase
                    .Child("chats")
                    .Child(chatId)
                    .Child("messages")
                    .PostAsync(message);

                // Update last activity for both users
                await UpdateUserLastActiveAsync(senderId);
                await UpdateUserLastActiveAsync(receiverId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending message: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Message>> GetMessagesAsync(int user1Id, int user2Id)
        {
            try
            {
                string chatId = GetChatId(user1Id, user2Id);
                var messages = await firebase
                    .Child("chats")
                    .Child(chatId)
                    .Child("messages")
                    .OnceAsync<Message>();

                return messages.Select(m => new Message
                {
                    Id = m.Key,
                    SenderId = m.Object.SenderId,
                    Timestamp = m.Object.Timestamp,
                    Content = m.Object.Content
                })
                .OrderBy(m => m.Timestamp)
                .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting messages: {ex.Message}");
                return new List<Message>();
            }
        }

        public IDisposable SubscribeToMessages(int user1Id, int user2Id, Action<Message> onMessageReceived)
        {
            string chatId = GetChatId(user1Id, user2Id);
            return firebase
                .Child("chats")
                .Child(chatId)
                .Child("messages")
                .AsObservable<Message>()
                .Subscribe(message =>
                {
                    if (message.Object != null)
                    {
                        message.Object.Id = message.Key;
                        onMessageReceived(message.Object);
                    }
                });
        }

        // Helper methods
        private string GetChatId(int user1Id, int user2Id)
        {
            // Ensures consistent chat IDs regardless of who initiates
            return user1Id < user2Id
                ? $"{user1Id}_{user2Id}"
                : $"{user2Id}_{user1Id}";
        }

        public async Task UpdateUserLastActiveAsync(int userId)
        {
            await firebase
                .Child("users")
                .Child(userId.ToString())
                .Child("LastActive")
                .PutAsync(DateTime.UtcNow);
        }

        // Sync methods
        public async Task SyncLocalContactsAsync(int myId, List<Contact> localContacts)
        {
            try
            {
                // Upload my contacts to Firebase
                await firebase
                    .Child("contacts")
                    .Child(myId.ToString())
                    .PutAsync(localContacts);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error syncing contacts: {ex.Message}");
            }
        }

        public async Task<List<Contact>> GetContactsAsync(int userId)
        {
            try
            {
                return await firebase
                    .Child("contacts")
                    .Child(userId.ToString())
                    .OnceSingleAsync<List<Contact>>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting contacts: {ex.Message}");
                return new List<Contact>();
            }
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastActive { get; set; }
    }

    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Message
    {
        public string Id { get; set; }
        public int SenderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Content { get; set; }
    }
}