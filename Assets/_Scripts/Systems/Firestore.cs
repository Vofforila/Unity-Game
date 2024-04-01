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
                        Debug.Log("Added Account ID");
                    });

                // Create a Account Document refrence
                DocumentReference accountInfo_Doc = db.Collection("AccountInfo").Document(newID.ToString());

                // Create the new Account
                AccountFirebase newAccountFirebase = new()
                {
                    User = specialFunctions.UpperCase(_username),
                    Status = false,
                    Email = _email,
                    Id = newID,
                    RankPoints = 0,
                    Winrate = 0,
                    PlayerIcon = 1,
                    FriendList = null,
                    FriendRequestsList = null,
                    InviteToGameList = null,
                    SentFriendRequests = null,
                };

                // Add the new Account to Firebase
                accountInfo_Doc.SetAsync(newAccountFirebase).ContinueWith(task =>
                    {
                        Debug.Log("Added Account Info");
                    });
            });
        }

        public async Task GetUserFromEmail(string _email)
        {
            // Get User Data from Email for first time
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            CollectionReference accountsIdIndex_col = db.Collection("AccountInfo");
            Debug.Log("Got Account from Email");
            await accountsIdIndex_col.GetSnapshotAsync().ContinueWith((task) =>
            {
                Debug.Log("Got Account from Email");
                QuerySnapshot snapshots = task.Result;
                foreach (DocumentSnapshot document in snapshots)
                {
                    Debug.Log("Got Account from Email");
                    Dictionary<string, object> data = document.ToDictionary();
                    Debug.Log(data);
                    if (_email == (data["Email"]).ToString())
                    {
                        Debug.Log("Got Account from Email");
                        accountFirebase = document.ConvertTo<AccountFirebase>();

                        Debug.Log("Got Account from Email");
                        break;
                    }
                }
            });
        }

        public void SendFriendRequest(string _newFriend)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            CollectionReference accountInfo_col = db.Collection("AccountInfo");

            accountInfo_col.GetSnapshotAsync().ContinueWith((task) =>
            {
                QuerySnapshot snapshots = task.Result;
                foreach (DocumentSnapshot document in snapshots.Documents)
                {
                    Dictionary<string, object> data = document.ToDictionary();
                    if (_newFriend == (data["User"]).ToString())
                    {
                        // Send FriendRequest to Other
                        Dictionary<int, string> newFriendRequestList = (Dictionary<int, string>)data["FriendRequestsList"];
                        newFriendRequestList.Add(accountFirebase.Id, accountFirebase.User);
                        DocumentReference accountInfo_doc = db.Collection("AccountInfo").Document(data["Id"].ToString());
                        accountInfo_doc.UpdateAsync("FriendRequestsList", newFriendRequestList);

                        // Add PendingReqest to Current User
                        accountInfo_doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());
                        accountFirebase.FriendRequestsList.Add((int)data["Id"], data["User"].ToString());
                        accountInfo_doc.UpdateAsync("SentFriendRequests", accountFirebase.FriendRequestsList);

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

                Debug.Log("User Data Updated: " + accountFirebase.User);

                /* UpdateState(true);*/
                updateUI.Invoke();
            });
        }

        /*       public void UpdateState(bool _var)
               {
                   FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
                   DocumentReference accountInfo_doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());

                   CollectionReference accountInfo_Col = db.Collection("AccountInfo");
                   accountInfo_Col.GetSnapshotAsync().ContinueWith(task =>
                   {
                       QuerySnapshot snapshots = task.Result;
                       foreach (KeyValuePair<object, bool> friend in accountFirebase.FriendList)
                       {
                           foreach (DocumentSnapshot document in snapshots.Documents)
                           {
                               Dictionary<string, object> data = document.ToDictionary();
                               if (friend.Key.ToString() == (data["User"]).ToString())
                               {
                                   Dictionary<object, bool> FriendList;
                                   FriendList = (Dictionary<object, bool>)data["FriendList"];
                                   FriendList[friend.Key] = true;
                               }
                           }
                       }
                   });
               }*/

        public void AcceptFriendRequest(string newFriend)
        {
            // Look for the Id of the Friend
            int friendId = 0;
            foreach (KeyValuePair<int, string> friendRequest in accountFirebase.FriendRequestsList)
            {
                if (friendRequest.Value == newFriend)
                {
                    friendId = friendRequest.Key;
                    break;
                }
            }

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference friendRequests_Doc = db.Collection("AccountInfo").Document(friendId.ToString());

            // Add Current User to Friend
            friendRequests_Doc.GetSnapshotAsync().ContinueWith(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                AccountFirebase otherAccountFirebase = snapshot.ConvertTo<AccountFirebase>();

                otherAccountFirebase.FriendList.Add(accountFirebase.Id, accountFirebase.User);
                friendRequests_Doc.UpdateAsync("FriendList", otherAccountFirebase.FriendList).ContinueWith(task => { });

                // Remove FriendRequest from Other User
                otherAccountFirebase.FriendRequestsList.Remove(friendId);
                friendRequests_Doc.UpdateAsync("FriendRequestsList", otherAccountFirebase.FriendRequestsList).ContinueWith(task => { });
            });

            // Add the Friend to the Current User
            DocumentReference accountInfo_Doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());

            accountFirebase.FriendList.Add(friendId, newFriend);
            accountInfo_Doc.UpdateAsync("FriendList", accountFirebase.FriendList).ContinueWith(task =>
            {
                Debug.Log("FriendList Update");
            });

            // Remove FriendRequest from Current User
            accountFirebase.FriendRequestsList.Remove(friendId);
            accountInfo_Doc.UpdateAsync("FriendRequestsList", accountFirebase.FriendRequestsList).ContinueWith(task =>
            {
                Debug.Log("FriendRequestList Update");
            });

            // Remove SentFriendRequest from Current User
            accountFirebase.SentFriendRequests.Remove(friendId);
            accountInfo_Doc.UpdateAsync("SentFriendRequests", accountFirebase.SentFriendRequests).ContinueWith(task =>
            {
                Debug.Log("SentFriendRequest Update");
            });
        }

        public void DeclineFriendRequest(string newFriend)
        {
            // Look for the Id of the Friend
            int friendId = 0;
            foreach (KeyValuePair<int, string> friendRequest in accountFirebase.FriendRequestsList)
            {
                if (friendRequest.Value == newFriend)
                {
                    friendId = friendRequest.Key;
                    break;
                }
            }

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference friendAccountInfo_doc = db.Collection("AccountInfo").Document(friendId.ToString());

            // Remove FriendRequest from Other User
            friendAccountInfo_doc.GetSnapshotAsync().ContinueWith(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                AccountFirebase otherAccountFirebase = snapshot.ConvertTo<AccountFirebase>();
                otherAccountFirebase.FriendRequestsList.Remove(friendId);
                friendAccountInfo_doc.UpdateAsync("FriendRequestsList", otherAccountFirebase.FriendRequestsList).ContinueWith(task => { });
            });

            DocumentReference accountInfo_Doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());

            // Remove FriendRequest from Current User
            accountFirebase.FriendRequestsList.Remove(friendId);
            accountInfo_Doc.UpdateAsync("FriendRequestsList", accountFirebase.FriendRequestsList).ContinueWith(task =>
            {
                Debug.Log("FriendRequestList Update");
            });

            // Remove SentFriendRequest from Current User
            accountFirebase.SentFriendRequests.Remove(friendId);
            accountInfo_Doc.UpdateAsync("SentFriendRequests", accountFirebase.SentFriendRequests).ContinueWith(task =>
            {
                Debug.Log("SentFriendRequest Update");
            });
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
            public Dictionary<int, string> FriendList { get; set; }

            [FirestoreProperty]
            public Dictionary<int, string> FriendRequestsList { get; set; }

            [FirestoreProperty]
            public List<object> InviteToGameList { get; set; }

            [FirestoreProperty]
            public List<object> SentFriendRequests { get; set; }

            [FirestoreProperty]
            public bool Status { get; set; }
        }
    }
}