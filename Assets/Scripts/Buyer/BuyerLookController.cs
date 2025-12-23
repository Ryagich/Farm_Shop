using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using NaughtyAttributes;
using VContainer;

namespace Buyer
{
    public class BuyerLookController : MonoBehaviour
    {
        [SerializeField] private BuyerLook femaleLook;
        [SerializeField] private BuyerLook maleLook;
        [SerializeField] private BuyerSettings buyerSettings;

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        [Inject]
        private void Construct(BuyerSettings buyerSettings)
        {
            this.buyerSettings = buyerSettings;
        }

        private void Start()
        {
            ShowRandomLook();
        }

        [Button]
        private void ShowRandomLook()
        {
            femaleLook.HideAll();
            maleLook.HideAll();
            var currentLook = Random.Range(.0f, 1.0f) < buyerSettings.GenderChance
                                  ? maleLook.ShowRandomLook().ToList()
                                  : femaleLook.ShowRandomLook().ToList();
        }
    }

    [Serializable]
    public class BuyerLook
    {
        public List<GameObject> bodies = new();
        public List<GameObject> hairs = new();
        public List<GameObject> glasses = new();
        public List<GameObject> earrings = new();
        public List<GameObject> breads = new();

        public void HideAll()
        {
            foreach (var body in bodies)
                if (body)
                    body.SetActive(false);
            foreach (var hair in hairs)
                if (hair)
                  hair.SetActive(false);
            foreach (var e in earrings)
                if (e)
                    e.SetActive(false);
            foreach (var g in glasses)
                if (g)
                    g.SetActive(false);
            foreach (var bread in breads)
                if (bread)
                    bread.SetActive(false);    
        }
        
        public IEnumerable<GameObject> ShowRandomLook()
        {
            var body = bodies[Random.Range(0, bodies.Count)];
            var hair = hairs[Random.Range(0, hairs.Count)];
            var e = earrings[Random.Range(0, earrings.Count)];
            var g = glasses[Random.Range(0, glasses.Count)];
            var bread = glasses[Random.Range(0, glasses.Count)];
            
            if (body)
            {
                body.SetActive(true);
                yield return body;
            }
            if (hair)
            {
                hair.SetActive(true);
                yield return hair;
            }
            if (e)
            {
                e.SetActive(true);
                yield return e;
            }
            if (g)
            {
                g.SetActive(true);
                yield return g;
            }
            if (bread)
            {
                bread.SetActive(true);
                yield return bread;
            }
        }
    }
}