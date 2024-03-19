using Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FriendRequestPrefab : MonoBehaviour
    {
        public TMP_Text newFriend;
        public Button acceptFriendRequest;
        public Button declineFriendRequest;
        public Firestore firestore;

        public void AcceptFriendRequest()
        {
            firestore.AcceptFriendRequest(newFriend.text);
        }

        public void DeclineFriendRequest()
        {
            firestore.DeclineFriendRequest(newFriend.text);
        }
    }
}