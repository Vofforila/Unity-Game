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
        public string newFriendId;
        public Button acceptFriendRequest;
        public Button declineFriendRequest;
        public Firestore firestore;

        private void Awake()
        {
            newFriendId = gameObject.name;
        }

        public void AcceptFriendRequest()
        {
            firestore.AcceptFriendRequest(newFriendId);
        }

        public void DeclineFriendRequest()
        {
            firestore.DeclineFriendRequest(newFriendId);
        }
    }
}