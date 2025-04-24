# MSGG - Messaging and Video Chat Desktop Client

MSGG is a Windows-based desktop client that allows users to exchange text messages and engage in video calls in real time. It is developed as a personal project for educational purposes, but with a focus on practical usability and a clean user interface.

## 🚀 Features

- 🔒 User authentication via Firebase
- 🗣️ Real-time text messaging
- 🎥 Peer-to-peer video calling
- 🧩 Local SQLite storage for contacts and message history
- 📁 Simple, clean UI built with Windows Forms

---

## 📦 Technologies Used

- **Language**: C#
- **UI Framework**: Windows Forms
- **Database**: SQLite
- **Backend Services**: Firebase Realtime Database & Firebase Authentication

---

## 🛠️ Setup Instructions

Follow the steps below to run the project on your local machine:

### 1. Clone the repository

```bash
git clone https://github.com/BelemirFuat/MSGG.git
cd MSGG
```

### 2. Configure Firebase
Go to Firebase Console.

Create a new project.

Navigate to Firestore Database and create a database instance.

Go to Project Settings > Service Accounts, and generate a new private key (.json file).

Download this file and place it in the project folder.

Ensure your Firebase authentication settings allow email/password sign-in.

### 3. Build the project
Open the solution file (MSGG.sln) using Visual Studio and build the solution. Make sure NuGet packages are restored if needed.

## 🖥️ Usage
Launch the application.

Register or log in using your Firebase credentials.

Add contacts and start messaging.

Initiate video calls from the contact list.

