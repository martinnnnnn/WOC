﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.IO;
using Newtonsoft.Json.Linq;



namespace WOC_Core
{
    public class Battle
    {
        private static int randomSeed = 0;
        public static int RandomSeed
        {
            get
            {
                return ++randomSeed;
            }
        }

        Dictionary<string, Card> cardsMap = new Dictionary<string, Card>();
        List<Actor> Cemetery = new List<Actor>();
        public List<Actor> Actors = new List<Actor>();
        Actor current;

        public Action OnBattleEnd;

        public void Init()
        {
            foreach (Actor actor in Actors)
            {
                actor.BattleInit();
            }

            UpdateInitiatives();
        }

        public void UpdateInitiatives()
        {
            Actors.ForEach(a => a.UpdateInitiative());
            Actors.Sort((a1, a2) => a1.initiative.Value.CompareTo(a2.initiative.Value));
        }

        public Actor NextActor()
        {
            if (current == null)
            {
                current = Actors.First();
            }
            else
            {
                int currentIndex = Actors.FindIndex(a => a.Name == current.Name);
                current = Actors[(currentIndex + 1) % Actors.Count];
            }

            return current;
        }

        public void OnCharacterDeath(Character character)
        {
            int index = Actors.FindIndex(a => a.Name == character.Owner.Name);
            Actor value = Actors[index];
            Actors.RemoveAt(index);
            Cemetery.Add(value);
        }

        public Card GetCard(string cardName)
        {
            return new Card(cardsMap[cardName]);
        }

        public bool Add(Card card)
        {
            if (cardsMap.ContainsKey(card.name))
            {
                return false;
            }
            cardsMap.Add(card.name, card);
            return true;
        }

        public bool Add(Actor actor)
        {
            if (Actors.Find(a => a.Name == actor.Name) != null)
            {
                Actors.Add(actor);
                LOG.Print("[BATTLE] Actor {0} added.", actor.Name);
                return true;
            }
            return false;
        }

        public void Update()
        {

        }

        public void Shutdown()
        {

        }
    }
}