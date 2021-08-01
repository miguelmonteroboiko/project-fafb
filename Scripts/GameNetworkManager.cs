using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace FAFB
{
    public class GameNetworkManager : NetworkManager
    {
        public static GameNetworkManager instance { get; private set; }

        private void Awake()
        {
            base.Awake();
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.LogError("Two NetworkManagers, deleting new one.");
                Destroy(gameObject);
            }
        }

        [Server]
        public static void SpawnItem(int itemId, Vector2 at)
        {
            GameObject dropedItemGameObject = GameObject.Instantiate(instance.spawnPrefabs[0], at, Quaternion.identity);
            NetworkServer.Spawn(dropedItemGameObject);
        }
    }
}
