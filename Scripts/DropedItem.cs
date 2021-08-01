using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace FAFB
{
    public class DropedItem : NetworkBehaviour
    {
        [SerializeField] private int _networkItemId;
        public Item droppedItem;

        private void Awake()
        {
            //droppedItem = Item.itemTypes[_networkItemId];
            if (droppedItem == null) Destroy(gameObject);
        }
    }
}
