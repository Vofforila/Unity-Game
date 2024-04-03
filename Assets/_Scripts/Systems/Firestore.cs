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
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference currentAccount_doc = db.Collection("AccountInfo").Document();

            // Create the new Account
            AccountFirebase newAccountFirebase = new()
            {
                User = specialFunctions.UpperCase(_username),
                Status = false,
                Email = _email,
                Id = currentAccount_doc.Id, // Generated by the API
                RankPoints = 0,
                Winrate = 0,
                PlayerIcon = 1,
                FriendList = new(),
                FriendRequestsList = new(),
                InviteToGameList = new(),
                SentFriendRequests = new(),
            };

            // Add the new Account to Firebase
            currentAccount_doc.SetAsync(newAccountFirebase).ContinueWith(task =>
            {
                Debug.Log("Added Account Info");
            });
        }

        #endregion Register

        #region GetInfo

        public async Task GetUserFromEmail(string _email)
        {
            // Get User Data from Email for first time
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            CollectionReference currentAccount_col = db.Collection("AccountInfo");

            await currentAccount_col.GetSnapshotAsync().ContinueWith((task) =>
            {
                QuerySnapshot snapshots = task.Result;
                foreach (DocumentSnapshot document in snapshots)
                {
                    Dictionary<string, object> data = document.ToDictionary(); ;
                    if (_email == (data["Email"]).ToString())
                    {
                        accountFirebase = document.ConvertTo<AccountFirebase>();

                        Debug.Log("Got Account from Email");

                        break;
                    }
                }
            });
        }

        public void UpdateLocalAccountListener()
        {
            // Updates the Current User Local Data
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference currentAccount_doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());

            currentAccount_doc.Listen(snapshot =>
            {
                accountFirebase = snapshot.ConvertTo<AccountFirebase>();

                Debug.Log("User Data Updated: " + accountFirebase.User);

                /* UpdateState(true);*/
                updateUI.Invoke();
            });
        }

        public async Task<AccountFirebase> GetAccountFromId(string _accountId)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference currentAccount_doc = db.Collection("AccountInfo").Document(_accountId.ToString());
            AccountFirebase account = new();

            await currentAccount_doc.GetSnapshotAsync().ContinueWith((task) =>
            {
                DocumentSnapshot snapshot = task.Result;

                account = snapshot.ConvertTo<AccountFirebase>();
            });

            return account;
        }

        #endregion GetInfo

        #region FriendRequest

        public void SendFriendRequest(string _newFriend)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            CollectionReference currentAccount_col = db.Collection("AccountInfo");

            currentAccount_col.GetSnapshotAsync().ContinueWith((task) =>
            {
                QuerySnapshot snapshots = task.Result;
                foreach (DocumentSnapshot document in snapshots.Documents)
                {
                    Dictionary<string, object> data = document.ToDictionary();
                    if (_newFriend == data["User"].ToString())
                    {
                        // Send FriendRequest to Other
                        AccountFirebase otherAccountFirebase = document.ConvertTo<AccountFirebase>();

                        otherAccountFirebase.FriendRequestsList.Add(accountFirebase.Id);
                        DocumentReference otherAccount_doc = db.Collection("AccountInfo").Document(otherAccountFirebase.Id.ToString());
                        otherAccount_doc.UpdateAsync("FriendRequestsList", otherAccountFirebase.FriendRequestsList);

                        // Add PendingReqest to Current User
                        otherAccount_doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());
                        accountFirebase.SentFriendRequests.Add(otherAccountFirebase.Id);
                        otherAccount_doc.UpdateAsync("SentFriendRequests", accountFirebase.SentFriendRequests);

                        Debug.Log("Sent Friend Request");
                    }
                }
            });
        }

        public void AcceptFriendRequest(string _newFriendId)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference otherAccount_doc = db.Collection("AccountInfo").Document(_newFriendId.ToString());

            otherAccount_doc.GetSnapshotAsync().ContinueWith(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                AccountFirebase otherAccountFirebase = snapshot.ConvertTo<AccountFirebase>();

                // Add Current Id to Other User
                otherAccountFirebase.FriendList.Add(accountFirebase.Id);
                otherAccount_doc.UpdateAsync("FriendList", otherAccountFirebase.FriendList).ContinueWith(task =>
                {
                    // Remove FriendRequest from Other User
                    otherAccountFirebase.FriendRequestsList.Remove(accountFirebase.Id);
                    otherAccount_doc.UpdateAsync("FriendRequestsList", otherAccountFirebase.FriendRequestsList).ContinueWith(task =>
                    {
                        // Add the Other Id to the Current User
                        DocumentReference currentAccount_doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());

                        accountFirebase.FriendList.Add(otherAccountFirebase.Id);

                        Debug.Log("FriendList Update");

                        currentAccount_doc.UpdateAsync("FriendList", accountFirebase.FriendList).ContinueWith(task =>
                        {
                            // Remove FriendRequest from Current User
                            accountFirebase.FriendRequestsList.Remove(otherAccountFirebase.Id);
                            Debug.Log("FriendRequestList Update");
                            currentAccount_doc.UpdateAsync("FriendRequestsList", accountFirebase.FriendRequestsList).ContinueWith(task =>
                            {
                                // Remove SentFriendRequest from Current User
                                accountFirebase.SentFriendRequests.Remove(otherAccountFirebase.Id);
                                currentAccount_doc.UpdateAsync("SentFriendRequests", accountFirebase.SentFriendRequests);

                                Debug.Log("SentFriendRequest Update");
                            });
                        });
                    });
                });
            });
        }

        public void DeclineFriendRequest(string _newFriendId)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

            // Remove FriendRequest from Current User
            DocumentReference currentAccount_doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());

            accountFirebase.FriendRequestsList.Remove(_newFriendId);
            Debug.Log("FriendRequestList Update");

            currentAccount_doc.UpdateAsync("FriendRequestsList", accountFirebase.FriendRequestsList).ContinueWith(task =>
            {
                // Remove SentFriendRequest from Current User
                accountFirebase.SentFriendRequests.Remove(_newFriendId);
                currentAccount_doc.UpdateAsync("SentFriendRequests", accountFirebase.SentFriendRequests).ContinueWith(task =>
                {
                    Debug.Log("SentFriendRequest Update");
                });
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
        public class AccountFirebase
        {
            [FirestoreProperty]
            public string User { get; set; }

            [FirestoreProperty]
            public string Email { get; set; }

            [FirestoreProperty]
            public string Id { get; set; }

            [FirestoreProperty]
            public int RankPoints { get; set; }

            [FirestoreProperty]
            public int Winrate { get; set; }

            [FirestoreProperty]
            public int PlayerIcon { get; set; }

            [FirestoreProperty]
            public List<string> FriendList { get; set; }

            [FirestoreProperty]
            public List<string> FriendRequestsList { get; set; }

            [FirestoreProperty]
            public List<string> InviteToGameList { get; set; }

            [FirestoreProperty]
            public List<string> SentFriendRequests { get; set; }

            [FirestoreProperty]
            public bool Status { get; set; }
        }
    }
}