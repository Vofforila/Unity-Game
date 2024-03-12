using Data;
using Firebase.Extensions;
using Firebase.Firestore;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using SpecialFunction;

namespace Database
{
    [CreateAssetMenu(fileName = "Firestore", menuName = "Firebase/Firestore")]
    public class Firestore : ScriptableObject
    {
        [Header("Scriptable")]
        public SpecialFunctions specialFunctions;
        public LocalData localData;

        public AccountFirebase accountFirebase = new();
        public LobbyData lobbydata = new();
        public int playerIcon;

        [Header("Events")]
        public UnityEvent updateUI;
        public UnityEvent updateLobbyDataEvent;

        public void RegisterUser(string _username, string _email)
        {
            // Add new user to Database

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference accountsIdIndex_doc = db.Collection("AccountsIdIndex").Document("ids");

            accountsIdIndex_doc.GetSnapshotAsync().ContinueWith((task) =>
            {
                DocumentSnapshot snapshot = task.Result;

                Dictionary<string, object> data = snapshot.ToDictionary();

                // Initialize a newID
                int newID;

                // Get the lastID
                newID = (int)(long)data["LastId"];

                // Create a new Id
                newID++;

                // Update the LastId available to FireBase
                accountsIdIndex_doc.UpdateAsync("LastId", newID).ContinueWith(task =>
                    {
                        if (Debug.isDebugBuild)
                            Debug.Log("Added Account ID");
                    });

                // Create a Account Document refrence
                DocumentReference accountInfo_Doc = db.Collection("AccountInfo").Document(newID.ToString());

                // Create the new Account
                AccountFirebase newAccountFirebase = new()
                {
                    User = specialFunctions.UpperCase(_username),
                    Email = _email,
                    Id = newID,
                    RankPoints = 0,
                    Winrate = 0,
                    PlayerIcon = 1,
                    FriendList = new(),
                    FriendRequestsList = new(),
                    InviteToGameList = new(),
                    SentFriendRequests = new(),
                };

                // Add the new Account to Firebase
                accountInfo_Doc.SetAsync(newAccountFirebase).ContinueWith(task =>
                    {
                        if (Debug.isDebugBuild)
                            Debug.Log("Added Account Info");
                    });
            });
        }

        public async Task GetUserFromEmail(string _email)
        {
            // Get User Data from Email for first time

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            CollectionReference accountsIdIndex_col = db.Collection("AccountInfo");

            await accountsIdIndex_col.GetSnapshotAsync().ContinueWith((task) =>
            {
                QuerySnapshot snapshots = task.Result;

                foreach (DocumentSnapshot document in snapshots)
                {
                    Dictionary<string, object> data = document.ToDictionary();

                    if (data.ContainsKey("Email") && (_email == (data["Email"]).ToString()))
                    {
                        accountFirebase = document.ConvertTo<AccountFirebase>();

                        Debug.Log("Got Account from Email");
                        break;
                    }
                }
            });
        }

        public void SendFriendRequest(string _newFriend)
        {
            //  Get a reference to the firestore service, using the default Firebase App
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            // Create FriendList Collection refrence
            CollectionReference accountInfo_col = db.Collection("AccountInfo");

            accountInfo_col.GetSnapshotAsync().ContinueWith((task) =>
            {
                QuerySnapshot snapshots = task.Result;
                foreach (DocumentSnapshot document in snapshots.Documents)
                {
                    Dictionary<string, object> data = document.ToDictionary();
                    if (_newFriend == (data["User"]).ToString())
                    {
                        // Send FriendRequest to Friend
                        List<object> newfriendRequestList = (List<object>)data["FriendRequestsList"];
                        newfriendRequestList.Add(accountFirebase.User);
                        DocumentReference accountInfo_doc = db.Collection("AccountInfo").Document(data["Id"].ToString());
                        accountInfo_doc.UpdateAsync("FriendRequestsList", newfriendRequestList);

                        // Add Friend to Current User SentFriendRequests
                        accountInfo_doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());
                        List<object> newSentfriendRequestList = (List<object>)data["SentFriendRequests"];
                        newSentfriendRequestList.Add(_newFriend);
                        accountInfo_doc.UpdateAsync("SentFriendRequests", newSentfriendRequestList);

                        Debug.Log("Sent Friend Request");
                    }
                }
            });
        }

        public void UpdateAccountFirebase()
        {
            // Updates the Current User Local Data

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference accountInfo_col = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());

            accountInfo_col.Listen(snapshot =>
            {
                accountFirebase = snapshot.ConvertTo<AccountFirebase>();

                Debug.Log("User Data Updated");

                updateUI.Invoke();
            });
        }

        public void AcceptFriendRequest(string newFriend)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            CollectionReference accountInfo_Col = db.Collection("AccountInfo");

            // Add Current User to Friend
            accountInfo_Col.GetSnapshotAsync().ContinueWith(task =>
            {
                QuerySnapshot snapshots = task.Result;
                foreach (DocumentSnapshot document in snapshots.Documents)
                {
                    Dictionary<string, object> data = document.ToDictionary();
                    if (newFriend == (data["User"]).ToString())
                    {
                        List<object> FriendList;
                        FriendList = (List<object>)data["FriendList"];
                        FriendList.Add(accountFirebase.User);

                        DocumentReference friendRequests_Doc = db.Collection("AccountInfo").Document(data["Id"].ToString());

                        friendRequests_Doc.UpdateAsync("FriendList", FriendList).ContinueWith(task =>
                        {
                            if (Debug.isDebugBuild)
                                Debug.Log("FriendList Update");
                        });
                        break;
                    }
                }
            });

            DocumentReference accountInfo_Doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());

            // Add the Friend to the Current User
            accountFirebase.FriendList.Add(newFriend);
            accountInfo_Doc.UpdateAsync("FriendList", accountFirebase.FriendList).ContinueWith(task =>
            {
                if (Debug.isDebugBuild)
                    Debug.Log("FriendList Update");
            });

            // Remove FriendRequest from Current User
            accountFirebase.FriendRequestsList.Remove(newFriend);
            accountInfo_Doc.UpdateAsync("FriendRequestsList", accountFirebase.FriendRequestsList).ContinueWith(task =>
            {
                if (Debug.isDebugBuild)
                    Debug.Log("FriendRequestList Update");
            });

            // Remove SentFriendRequest from Current User
            accountInfo_Doc.UpdateAsync("SentFriendRequests", accountFirebase.SentFriendRequests).ContinueWith(task =>
            {
                if (Debug.isDebugBuild)
                    Debug.Log("SentFriendRequest Update");
            });
        }

        // Add Functionality
        public void DeclineFriendRequest(string newFriend)
        {
        }

        public void InviteToLobby(string invitedFriend)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            CollectionReference accountInfo_Col = db.Collection("AccountInfo");

            accountInfo_Col.GetSnapshotAsync().ContinueWith((task) =>
            {
                QuerySnapshot snapshots = task.Result;

                foreach (DocumentSnapshot document in snapshots)
                {
                    Dictionary<string, object> data = document.ToDictionary();
                    if (invitedFriend == (data["User"]).ToString())
                    {
                        List<object> InviteToGameList = (List<object>)data["InviteToGameList"];
                        InviteToGameList.Add(accountFirebase.User);

                        DocumentReference accountInfo_doc = db.Collection("AccountInfo").Document(data["Id"].ToString());

                        Debug.Log("Lobby Invite Sent");
                        accountInfo_doc.UpdateAsync("InviteToGameList", InviteToGameList);
                    }
                }
            });
        }

        public void RemoveInvite()
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference accountInfo_doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());

            accountFirebase.InviteToGameList.Remove(localData.inviteName);

            Debug.Log("Removed Invite");
            accountInfo_doc.UpdateAsync("InviteToGameList", accountFirebase.InviteToGameList);
        }

        public void RaiseLobbyDataListener(string _sessionName)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference lobbies_doc = db.Collection("Lobbies").Document(_sessionName);

            lobbies_doc.Listen(snapshot =>
            {
                lobbydata = snapshot.ConvertTo<LobbyData>();
                Debug.Log("Update PlayerBanner - Event");
                updateLobbyDataEvent.Invoke();
            });
        }

        public void CreateLobbyData()
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference lobbies_doc = db.Collection("Lobbies").Document(accountFirebase.User);

            lobbydata.HostPlayerList = new();
            lobbydata.ScoreList = new();
            lobbydata.RoomChat = "";

            lobbydata.HostPlayerList.Add(accountFirebase.User);
            lobbydata.ScoreList[accountFirebase.User] = 0;

            lobbies_doc.SetAsync(lobbydata);
        }

        public void UpdateLobbyData(string _sessionName)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference lobbies_doc = db.Collection("Lobbies").Document(_sessionName);

            lobbies_doc.GetSnapshotAsync().ContinueWith((task) =>
            {
                DocumentSnapshot snapshot = task.Result;

                LobbyData targetLobbyData = snapshot.ConvertTo<LobbyData>();

                targetLobbyData.HostPlayerList.Add(accountFirebase.User);
                targetLobbyData.ScoreList[accountFirebase.User] = 0;

                lobbies_doc.UpdateAsync("HostPlayerList", targetLobbyData.HostPlayerList);
                lobbies_doc.UpdateAsync("ScoreList", targetLobbyData.ScoreList);
            });
        }

        public void UpdateScore(string _sessionName, int _score)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference lobbies_doc = db.Collection("Lobbies").Document(_sessionName);

            lobbies_doc.GetSnapshotAsync().ContinueWith((task) =>
            {
                DocumentSnapshot snapshot = task.Result;

                LobbyData targetLobbyData = snapshot.ConvertTo<LobbyData>();

                targetLobbyData.ScoreList[accountFirebase.User] = _score;

                lobbies_doc.UpdateAsync("ScoreList", targetLobbyData.ScoreList);
            });
        }

        public void UpdateChat(string _message, string _sessionName)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference lobbies_doc = db.Collection("Lobbies").Document(_sessionName);

            lobbies_doc.GetSnapshotAsync().ContinueWith((task) =>
            {
                DocumentSnapshot snapshot = task.Result;

                LobbyData targetLobbyData = snapshot.ConvertTo<LobbyData>();

                targetLobbyData.RoomChat += _message;

                lobbies_doc.UpdateAsync("RoomChat", targetLobbyData.RoomChat);
            });
        }

        public void UpdateUserIcon(int _var)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference accountInfo_doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());

            accountInfo_doc.GetSnapshotAsync().ContinueWith((task) =>
            {
                accountFirebase.PlayerIcon = _var;

                accountInfo_doc.UpdateAsync("PlayerIcon", accountFirebase.PlayerIcon);
            });
        }

        public async Task GetPlayerIcon(string _user)
        {
            // Get User Data from Email for first time

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            CollectionReference accountsIdIndex_col = db.Collection("AccountInfo");

            await accountsIdIndex_col.GetSnapshotAsync().ContinueWith((task) =>
            {
                QuerySnapshot snapshots = task.Result;

                foreach (DocumentSnapshot document in snapshots)
                {
                    Dictionary<string, object> data = document.ToDictionary();

                    if (data.ContainsKey("User") && (_user == (data["User"]).ToString()))
                    {
                        /*  AccountFirebase otheraccountFirebase = document.ConvertTo<AccountFirebase>();*/

                        playerIcon = (int)(long)data["PlayerIcon"];

                        Debug.Log("Got Player Icon");
                        break;
                    }
                }
            });
        }

        [FirestoreData]
        public class LobbyData

        {
            [FirestoreProperty]
            public List<object> HostPlayerList { get; set; }

            [FirestoreProperty]
            public Dictionary<string, int> ScoreList { get; set; }

            [FirestoreProperty]
            public string RoomChat { get; set; }
        }

        [FirestoreData]
        public class AccountFirebase
        {
            [FirestoreProperty]
            public string User { get; set; }

            [FirestoreProperty]
            public string Email { get; set; }

            [FirestoreProperty]
            public int Id { get; set; }

            [FirestoreProperty]
            public int RankPoints { get; set; }

            [FirestoreProperty]
            public int Winrate { get; set; }

            [FirestoreProperty]
            public int PlayerIcon { get; set; }

            [FirestoreProperty]
            public List<object> FriendList { get; set; }

            [FirestoreProperty]
            public List<object> FriendRequestsList { get; set; }

            [FirestoreProperty]
            public List<object> InviteToGameList { get; set; }

            [FirestoreProperty]
            public List<object> SentFriendRequests { get; set; }
        }
    }
}