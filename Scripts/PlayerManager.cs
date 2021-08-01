using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace FAFB
{
    public class PlayerManager : NetworkBehaviour, IDamagable
    {
        public static PlayerManager localPlayer { private get; set; }

        [Header("Movement system")]
        private Rigidbody2D _rb;
        [SerializeField] private float _speed;
        [SyncVar] 
        [SerializeField] private Vector2 _networkPosition;
        [SerializeField] private Vector2 _dampVelocity;
        [SerializeField] private float _positionSmoothDamp;

        [Header("Health system")]
        [SyncVar] 
        [SerializeField] private Health _health;

        [Header("Inventory system")]
        [SerializeField] private Inventory _inventory;

        [System.Serializable]
        public class Inventory
        {
            private PlayerManager player;

            private float _maxVolume, _maxWeight;
            public float MaxVolume => _maxVolume;
            
            public float CurrentVolume
            {
                get
                {
                    float _totalVolume = 0;
                    foreach(Item item in _items)
                    {
                        _totalVolume += item.Volume;
                    }
                    return _totalVolume;
                }
            }

            public float MaxWeight => _maxWeight;
            public float CurrentWeight
            {
                get
                {
                    float _totalWeight = 0;
                    foreach (Item item in _items)
                    {
                        _totalWeight += item.Weight;
                    }
                    return _totalWeight;
                }
            }

            [SerializeField] private List<Item> _items = new List<Item>()
            {
                { new Item("mesa", 10, 10) }
            };

            public Inventory(PlayerManager owner, float maxVolume, float maxWeight)
            {
                _maxVolume = maxVolume;
                _maxWeight = maxWeight;

                player = owner;
            }

            public bool AddItem(Item item)
            {
                if (CurrentWeight + item.Weight <= MaxWeight)
                    if (CurrentVolume + item.Volume <= MaxVolume)
                        _items.Add(item);
                return false;
            }

            [Command]
            public void CmdDropItem(int index, Vector2 position)
            {
                GameNetworkManager.SpawnItem(index, position);
                RpcDropItem(index);
                _items.RemoveAt(index);
            }
            [TargetRpc]
            public void RpcDropItem(int index)
            {
                _items.RemoveAt(index);
            }
        }

        #region Unity
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            if (isLocalPlayer)
            {
                
                if (localPlayer == null)
                {
                    localPlayer = this;

                    Debug.Log("Local player set up!");
                }
                else if (localPlayer != this)
                {
                    Debug.LogError("There is already a local player set up.");
                    Destroy(gameObject);
                }
            }
        }

        private void FixedUpdate()
        {
            if(isLocalPlayer)
            {
                Move();

                if(Input.GetKeyDown(KeyCode.Space)) _inventory.CmdDropItem(0, _rb.position);
            }

            if(!isServer) _rb.MovePosition(Vector2.SmoothDamp(_rb.position, _networkPosition, ref _dampVelocity, _positionSmoothDamp));
        }
        #endregion

        #region MoveMechanics

        public void Move()
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            CmdSendMoveInputs(input);
        }

        [Command]
        private void CmdSendMoveInputs(Vector2 input)
        {
            _rb.velocity = input.normalized * _speed;
            _networkPosition = _rb.position;
        }
        #endregion

        #region NetworkBehaviour
        public override void OnStopClient()
        {
            base.OnStopClient();
            if (isLocalPlayer) localPlayer = null;
        }
        #endregion

        #region Inventory

        #endregion

        #region IDamagable
        public void Damage(Damage damage)
        {
            if(isServer)
            {
                _health.Damage(damage.damageAmount);
            }
        }
        #endregion
    }

    [System.Serializable]
    public class Health
    {
        public float currentHealth, maxhealth;

        public UnityEvent OnDeath;

        public void Damage(float damage)
        {
            currentHealth -= damage;
            if(currentHealth < 0)
            {
                OnDeath.Invoke();
            }
        }
    }

    public class Damage
    {
        public float damageAmount;

        public static Dictionary<int, Damage> damageTypes = new Dictionary<int, Damage>()
        {
            { 1, new Damage(5) },
        };

        public Damage(int damageAmount)
        {
            this.damageAmount = damageAmount;
        }
    }

    public interface IDamagable
    {
        public void Damage(Damage damage);
    }
}