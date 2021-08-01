using System.Collections.Generic;
using UnityEngine;

namespace FAFB
{
    [CreateAssetMenu(fileName = "New Item", menuName = "FAFB/Item/Basic", order = 0)]
    public class Item : ScriptableObject
    {
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private float _volume;
        public float Volume => _volume;

        [SerializeField] private float _weight;
        public float Weight => _weight;

        public static Dictionary<int, Item> itemTypes = new Dictionary<int, Item>()
        {
            { 1, new Item("la switch", 1, 1) },
            { 2, new Item("mesa", 3, 10) },
        };

        public Item(string name, float volume, float weight)
        {
            _name = name;
            _volume = volume;
            _weight = weight;
        }
    }
}
