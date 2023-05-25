using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SARP.Data
{
    [CreateAssetMenu(menuName = "SARP/Data/Faction", fileName = "Faction")]
    public class Faction : ScriptableObject
    {
        [SerializeField]
        string factionName;

        [SerializeField]
        List<Faction> enemys;

        public string FactionName => factionName;
        public IReadOnlyList<Faction> Enemys => enemys;
    }
}
