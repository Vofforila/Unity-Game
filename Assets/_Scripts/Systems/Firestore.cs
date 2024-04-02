using Data;
using Firebase.Firestore;
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
        public Friend currentUser = new();

        public int playerIcon;

        [Header("Events")]
        public UnityEvent updateUI;
        public UnityEvent updateLobbyDataEvent;

        #region Test

        public void TestFunction()
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference accountsIdIndex_doc = db.Collection("AccountsIdIndex").Document("2");

            accountsIdIndex_doc.GetSnapshotAsync().ContinueWith((task) =>
            {
                /*  DocumentSnapshot snapshot = task.Result;
                  Dictionary<string, object> data = snapshot.ToDictionary();
                  List<object> Harta = (List<object>)data["FriendList"];

                  // All data needs to be List<obj> when reading we need to use CreateObjectFromDictionary

                  foreach (Dictionary<string, object> friend in Harta)
                  {
                      Friend newFriend = CreateObjectFromDictionary<Friend>(friend);

                      Debug.Log(newFriend.User);
                  }*/
                /*
                                Test newTest = new()
                                {
                                    FriendList = new(),
                                };

                                Friend newFriend = new()
                                {
                                    User = "Paul",
                                    Id = 4,
                                    Coins = 40,
                                };

                                newTest.FriendList.Add(newFriend);
                                newTest.FriendList.Add(newFriend);

                                accountsIdIndex_doc.SetAsync(newTest).ContinueWith(task =>
                                {
                                    Debug.Log("Up");
                                });*/
            });
        }

        #endregion Test

        #region Register

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
                    FriendList = new(),
                    FriendRequestsList = new(),
                    InviteToGameList = new(),
                    SentFriendRequests = new(),
                };

                // Add the new Account to Firebase
                accountInfo_Doc.SetAsync(newAccountFirebase).ContinueWith(task =>
                {
                    Debug.Log("Added Account Info");
                });
            });
        }

        #endregion Register

        #region GetAccount

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

                        currentUser.User = accountFirebase.User;
                        currentUser.Id = accountFirebase.Id;

                        Debug.Log("Got Account from Email");
                        break;
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

                currentUser.User = accountFirebase.User;
                currentUser.Id = accountFirebase.Id;

                Debug.Log("User Data Updated: " + accountFirebase.User);

                /* UpdateState(true);*/
                updateUI.Invoke();
            });
        }

        #endregion GetAccount

        #region FriendRequest

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
                    Debug.Log(_newFriend);
                    if (_newFriend == data["User"].ToString())
                    {
                        // Send FriendRequest to Other
                        List<object> newFriendRequestList = (List<object>)data["FriendRequestsList"];

                        Friend otherUser = new()
                        {
                            User = (string)data["User"],
                            Id = (int)(long)data["Id"],
                        };

                        newFriendRequestList.Add(currentUser);
                        DocumentReference accountInfo_doc = db.Collection("AccountInfo").Document(data["Id"].ToString());
                        accountInfo_doc.UpdateAsync("FriendRequestsList", newFriendRequestList);

                        // Add PendingReqest to Current User
                        accountInfo_doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());
                        accountFirebase.SentFriendRequests.Add(otherUser);
                        accountInfo_doc.UpdateAsync("SentFriendRequests", accountFirebase.SentFriendRequests);

                        Debug.Log("Sent Friend Request");
                    }
                }
            });
        }

        public void AcceptFriendRequest(string newFriend)
        {
            // Look for the Id of the Friend
            Friend otherUser = new();
            foreach (Friend friendRequest in accountFirebase.FriendRequestsList)
            {
                if (friendRequest.User == newFriend)
                {
                    otherUser = friendRequest;
                    break;
                }
            }

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference friendRequests_Doc = db.Collection("AccountInfo").Document(otherUser.Id.ToString());

            // Add Current User to Friend
            friendRequests_Doc.GetSnapshotAsync().ContinueWith(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                AccountFirebase otherAccountFirebase = snapshot.ConvertTo<AccountFirebase>();

                otherAccountFirebase.FriendList.Add(currentUser);
                friendRequests_Doc.UpdateAsync("FriendList", otherAccountFirebase.FriendList).ContinueWith(task => { });

                // Remove FriendRequest from Other User
                otherAccountFirebase.FriendRequestsList.Remove(currentUser);
                friendRequests_Doc.UpdateAsync("FriendRequestsList", otherAccountFirebase.FriendRequestsList).ContinueWith(task => { });
            });

            // Add the Friend to the Current User
            DocumentReference accountInfo_Doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());

            accountFirebase.FriendList.Add(otherUser);
            accountInfo_Doc.UpdateAsync("FriendList", accountFirebase.FriendList).ContinueWith(task =>
            {
                Debug.Log("FriendList Update");
            });

            // Remove FriendRequest from Current User
            accountFirebase.FriendRequestsList.Remove(otherUser);
            accountInfo_Doc.UpdateAsync("FriendRequestsList", accountFirebase.FriendRequestsList).ContinueWith(task =>
            {
                Debug.Log("FriendRequestList Update");
            });

            // Remove SentFriendRequest from Current User
            accountFirebase.SentFriendRequests.Remove(otherUser);
            accountInfo_Doc.UpdateAsync("SentFriendRequests", accountFirebase.SentFriendRequests).ContinueWith(task =>
            {
                Debug.Log("SentFriendRequest Update");
            });
        }

        public void DeclineFriendRequest(string newFriend)
        {
            // Look for the Id of the Friend
            Friend otherUser = new();
            foreach (Friend friendRequest in accountFirebase.FriendRequestsList)
            {
                if (friendRequest.User == newFriend)
                {
                    otherUser = friendRequest;
                    break;
                }
            }

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference friendAccountInfo_doc = db.Collection("AccountInfo").Document(otherUser.Id.ToString());

            // Remove FriendRequest from Other User
            friendAccountInfo_doc.GetSnapshotAsync().ContinueWith(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                AccountFirebase otherAccountFirebase = snapshot.ConvertTo<AccountFirebase>();
                otherAccountFirebase.FriendRequestsList.Remove(currentUser);
                friendAccountInfo_doc.UpdateAsync("FriendRequestsList", otherAccountFirebase.FriendRequestsList).ContinueWith(task => { });
            });

            // Remove FriendRequest from Current User
            DocumentReference accountInfo_Doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());

            accountFirebase.FriendRequestsList.Remove(otherUser);
            accountInfo_Doc.UpdateAsync("FriendRequestsList", accountFirebase.FriendRequestsList).ContinueWith(task =>
            {
                Debug.Log("FriendRequestList Update");
            });

            // Remove SentFriendRequest from Current User
            accountFirebase.SentFriendRequests.Remove(otherUser);
            accountInfo_Doc.UpdateAsync("SentFriendRequests", accountFirebase.SentFriendRequests).ContinueWith(task =>
            {
                Debug.Log("SentFriendRequest Update");
            });
        }

        #endregion FriendRequest

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
        public class Friend
        {
            [FirestoreProperty]
            public string User { get; set; }

            [FirestoreProperty]
            public int Id { get; set; }
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
            public List<Friend> FriendList { get; set; }

            [FirestoreProperty]
            public List<Friend> FriendRequestsList { get; set; }

            [FirestoreProperty]
            public List<object> InviteToGameList { get; set; }

            [FirestoreProperty]
            public List<Friend> SentFriendRequests { get; set; }

            [FirestoreProperty]
            public bool Status { get; set; }
        }
    }
}