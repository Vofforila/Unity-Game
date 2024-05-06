using Data;
using Firebase.Firestore;
using SpecialFunction;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

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
                // Link Data
                User = specialFunctions.UpperCase(_username),
                Email = _email,
                Id = currentAccount_doc.Id, // Generated by the API

                // Player Profile
                Winrate = 100,
                GamesPlayed = 0,
                GamesLost = 0,
                GamesWon = 0,
                PlayerIcon = 1,
                RankPoints = 100,
                TimePlayed = 0,
                Rank = "Bronze",

                // FriendList
                Status = false,
                FriendList = new(),
                FriendRequestsList = new(),
                InviteToGameList = new(),
                SentFriendRequests = new(),
                OnlineFriends = new(),
                OfflineFriends = new(),
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

                Debug.Log("Added Friend");

                // Add Current Id to Other User
                otherAccountFirebase.FriendList.Add(accountFirebase.Id);
                otherAccount_doc.UpdateAsync("FriendList", otherAccountFirebase.FriendList).ContinueWith(task =>
                {
                    // Remove SentFriendRequest from Other User
                    otherAccountFirebase.SentFriendRequests.Remove(accountFirebase.Id);
                    otherAccount_doc.UpdateAsync("SentFriendRequests", otherAccountFirebase.SentFriendRequests);

                    // Remove FriendRequest from Current User
                    DocumentReference currentAccount_doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());
                    accountFirebase.FriendRequestsList.Remove(otherAccountFirebase.Id);
                    currentAccount_doc.UpdateAsync("FriendRequestsList", accountFirebase.FriendRequestsList).ContinueWith(task =>
                     {
                         // Add Other Id to Current User
                         accountFirebase.FriendList.Add(otherAccountFirebase.Id);
                         currentAccount_doc.UpdateAsync("FriendList", accountFirebase.FriendList);
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

        #region Lobby

        public void InviteToLobby(string _invitedFriendId)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference otherAccount_doc = db.Collection("AccountInfo").Document(_invitedFriendId);

            otherAccount_doc.GetSnapshotAsync().ContinueWith((task) =>
            {
                DocumentSnapshot snapshot = task.Result;

                AccountFirebase otherAccountFirebase = snapshot.ConvertTo<AccountFirebase>();

                otherAccountFirebase.InviteToGameList.Add(accountFirebase.Id);

                otherAccount_doc.UpdateAsync("InviteToGameList", otherAccountFirebase.InviteToGameList);

                Debug.Log("Lobby Invite Sent");
            });
        }

        public void RemoveInvite(string _inviteId)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference currentAccount_doc = db.Collection("AccountInfo").Document(accountFirebase.Id.ToString());

            accountFirebase.InviteToGameList.Remove(_inviteId);

            currentAccount_doc.UpdateAsync("InviteToGameList", accountFirebase.InviteToGameList);
            Debug.Log("Removed Invite");
        }

        #endregion Lobby

        #region Profile

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
                        playerIcon = (int)(long)data["PlayerIcon"];
                        break;
                    }
                }
            });
        }

        public int UpdatePlayerProfile(bool _win, float _timeplayer)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference accountsIdIndex_doc = db.Collection("AccountInfo").Document(accountFirebase.Id);

            accountFirebase.GamesPlayed++;

            int RankPoints;
            if (_win == true)
            {
                accountFirebase.GamesWon++;
                if (accountFirebase.Winrate >= 60)
                {
                    RankPoints = 20;
                    accountFirebase.RankPoints += RankPoints;
                }
                else
                {
                    RankPoints = 15;
                    accountFirebase.RankPoints += RankPoints;
                }
            }
            else
            {
                accountFirebase.GamesLost++;
                if (accountFirebase.Winrate >= 60)
                {
                    RankPoints = -10;
                    accountFirebase.RankPoints += RankPoints;
                }
                else
                {
                    RankPoints = -15;
                    accountFirebase.RankPoints += RankPoints;
                }
            }
            if (accountFirebase.GamesLost == 0)
            {
                accountFirebase.Winrate = 100;
            }
            else
            {
                accountFirebase.Winrate = (accountFirebase.GamesWon / accountFirebase.GamesLost) * 100;
            }
            accountFirebase.TimePlayed += _timeplayer;
            if (accountFirebase.RankPoints >= 100)
            {
                accountFirebase.Rank = "Iron";
            }
            else if (accountFirebase.RankPoints >= 200)
            {
                accountFirebase.Rank = "Bronze";
            }
            else if (accountFirebase.RankPoints >= 300)
            {
                accountFirebase.Rank = "Silver";
            }
            else if (accountFirebase.RankPoints >= 400)
            {
                accountFirebase.Rank = "Gold";
            }
            else if (accountFirebase.RankPoints >= 500)
            {
                accountFirebase.Rank = "Platinum";
            }
            else if (accountFirebase.RankPoints >= 600)
            {
                accountFirebase.Rank = "Diamond";
            }
            else if (accountFirebase.RankPoints >= 700)
            {
                accountFirebase.Rank = "Master";
            }
            else if (accountFirebase.RankPoints >= 800)
            {
                accountFirebase.Rank = "GrandMaster";
            }
            else if (accountFirebase.RankPoints >= 900)
            {
                accountFirebase.Rank = "Challanger";
            }

            accountsIdIndex_doc.UpdateAsync("GamesWon", accountFirebase.GamesWon);
            accountsIdIndex_doc.UpdateAsync("GamesLost", accountFirebase.GamesLost);
            accountsIdIndex_doc.UpdateAsync("GamesPlayed", accountFirebase.GamesPlayed);
            accountsIdIndex_doc.UpdateAsync("RankPoints", accountFirebase.RankPoints);
            accountsIdIndex_doc.UpdateAsync("TimePlayed", accountFirebase.TimePlayed);
            accountsIdIndex_doc.UpdateAsync("Rank", accountFirebase.Rank);

            return RankPoints;
        }

        #endregion Profile

        #region State

        public void StateChange(bool _currentState)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

            DocumentReference currentAccount_doc = db.Collection("AccountInfo").Document(accountFirebase.Id);
            accountFirebase.Status = _currentState;

            currentAccount_doc.UpdateAsync("Status", accountFirebase.Status);

            foreach (string friendId in accountFirebase.FriendList)
            {
                DocumentReference otherAccount_doc = db.Collection("AccountInfo").Document(friendId.ToString());
                otherAccount_doc.GetSnapshotAsync().ContinueWith((task) =>
                {
                    DocumentSnapshot snapshot = task.Result;
                    AccountFirebase otherAccount = snapshot.ConvertTo<AccountFirebase>();

                    // If Player is no online dont notify
                    if (otherAccount.Status == false) return;
                    if (_currentState == true)
                    {
                        otherAccount.OnlineFriends = new();
                        otherAccount.OnlineFriends.Add(accountFirebase.Id);
                        otherAccount_doc.UpdateAsync("OnlineFriends", otherAccount.OnlineFriends);
                    }
                    else
                    {
                        otherAccount.OfflineFriends = new();
                        otherAccount.OfflineFriends.Add(accountFirebase.Id);
                        otherAccount_doc.UpdateAsync("OfflineFriends", otherAccount.OfflineFriends);
                    }

                    Debug.Log("State Changed:" + _currentState);
                });
            }
        }

        #endregion State

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
            public bool Status { get; set; }

            [FirestoreProperty]
            public string User { get; set; }

            [FirestoreProperty]
            public string Email { get; set; }

            [FirestoreProperty]
            public string Id { get; set; }

            [FirestoreProperty]
            public int GamesPlayed { get; set; }

            [FirestoreProperty]
            public int GamesWon { get; set; }

            [FirestoreProperty]
            public int GamesLost { get; set; }

            [FirestoreProperty]
            public float TimePlayed { get; set; }

            [FirestoreProperty]
            public string Rank { get; set; }

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
            public List<string> OnlineFriends { get; set; }

            [FirestoreProperty]
            public List<string> OfflineFriends { get; set; }
        }
    }
}