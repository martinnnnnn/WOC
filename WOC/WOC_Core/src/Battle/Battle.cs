using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;



namespace WOC_Core
{
    //public class Battle
    //{
    //    public int RandomSeed = 0;
    //    public Random random;

    //    List<Actor> Cemetery = new List<Actor>();
    //    public List<Actor> Actors = new List<Actor>();
    //    Actor current;

    //    public Action OnBattleEnd;
    //    public bool HasStarted = false;

    //    public Battle(int randomSeed)
    //    {
    //        RandomSeed = randomSeed;
    //        random = new Random(RandomSeed);
    //    }

    //    public bool Start()
    //    {
    //        if (!HasStarted)
    //        {
    //            Console.WriteLine("[BATTLE] Starting !");
    //            foreach (Actor actor in Actors)
    //            {
    //                actor.BattleInit();
    //            }

    //            UpdateInitiatives();
    //            NextActor().StartTurn();
    //            HasStarted = true;
    //            return true;
    //        }
    //        Console.WriteLine("[BATTLE] Already started !");
    //        return false;
    //    }

    //    public void UpdateInitiatives()
    //    {
    //        Actors.ForEach(a => a.UpdateInitiative());
    //        Actors.Sort((a1, a2) => a1.initiative.Value.CompareTo(a2.initiative.Value));
    //    }

    //    public Actor NextActor()
    //    {
    //        if (current == null)
    //        {
    //            current = Actors.First();
    //        }
    //        else
    //        {
    //            int currentIndex = Actors.FindIndex(a => a.Name == current.Name);
    //            current = Actors[(currentIndex + 1) % Actors.Count];
    //        }

    //        return current;
    //    }

    //    public void OnCharacterDeath(Character character)
    //    {
    //        Console.WriteLine("{0} died !", character.Name);
    //        int index = Actors.FindIndex(a => a.Name == character.Owner.Name);
    //        Actor value = Actors[index];
    //        Actors.RemoveAt(index);
    //        Cemetery.Add(value);
    //        if (Actors.Find(a => a is PNJActor) == null || Actors.Find(a => a is PlayerActor) == null)
    //        {
    //            OnBattleEnd();
    //        }
    //    }

    //    public bool Add(Actor actor)
    //    {
    //        if (Actors.Find(a => a.Name == actor.Name) == null)
    //        {
    //            Actors.Add(actor);
    //            actor.Battle = this;
    //            Console.WriteLine("[BATTLE] Actor {0} added.", actor.Name);
    //            return true;
    //        }
    //        Console.WriteLine("[BATTLE] Actor {0} already exists.", actor.Name);
    //        return false;
    //    }

    //    public void Update()
    //    {

    //    }

    //    public void Shutdown()
    //    {

    //    }

    //    public Actor GetCurrentActor()
    //    {
    //        return Actors.Find(a => a.turnState == Actor.TurnState.MINE);
    //    }
    //}
}
