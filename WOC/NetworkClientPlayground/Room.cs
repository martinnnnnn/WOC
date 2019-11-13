using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOC_Core;

namespace Playground
{
    public class Room
    {
        public string name;
        public Battle battle;
        
       
        public Room(string name, int randomSeed)
        {
            this.name = name;

            LOG.Print("[ROOM] Battle construction...");
            battle = new Battle(randomSeed);

            Initiative.Max = 50;
            Hand.Max = 3;

            //PNJS
            List<Actor> actors = new List<Actor>()
            {
                // battle | character | name | first init
                new PNJActor(new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 20), "monstre1", 5),
                new PNJActor(new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 20), "monstre2", 5),
                new PNJActor(new Character(Character.Race.OGRE, Character.Category.CHAMAN, 15), "monstre3", 5)
            };
            LOG.Print("[ROOM] Adding PNJs");
            actors.ForEach(a => battle.Add(a));
        }

        public void InitBattle()
        {
            LOG.Print("[ROOM] Battle initialization...");

            //ForEach(s =>
            //{
            //    s.account.actor = new PlayerActor(s.account.name, 5, 20);
            //    var actor = s.account.actor;
            //    if (battle.Add(actor))
            //    {
            //        actor.AddCards(s.account.defaultDeck.cardNames);
            //    }
            //});
        }
    }

    //public class Room
    //{
    //    public PlayerActor actor;
    //    public Battle battle;

    //    public void HandleRoomMessage(IPacketData data)
    //    {
    //        switch (data)
    //        {
    //            case PD_CardPlayed cardPlayed:
    //                HandleCardPlayer(cardPlayed);
    //                break;

    //            case PD_TurnEnd turnEnd:
    //                HandleTurnEnd(turnEnd);
    //                break;

    //            case PD_BattleStart battleStart:
    //                HandleBattleStart(battleStart);
    //                break;

    //            case PD_RoomJoin roomJoin:
    //                HandleRoomJoin(roomJoin);
    //                break;

    //            case PD_PNJAdd pnjAdd:
    //                HandlePNJAdd(pnjAdd);
    //                break;
    //        }
    //    }

    //    void HandleCardPlayer(PD_CardPlayed cardPlayed)
    //    {
    //        PlayerActor owner = battle.Actors.Find(a => cardPlayed.ownerName == a.Name) as PlayerActor;
    //        Card card = owner.hand.Get(cardPlayed.cardIndex);
    //        Character target = battle.Actors.Find(a => a.character.Name == cardPlayed.targetName).character;
    //        owner.PlayCard(card, target);
    //    }

    //    void HandleTurnEnd(PD_TurnEnd turnEnd)
    //    {
    //        battle.GetCurrentActor().EndTurn();
    //        battle.NextActor().StartTurn();
    //        if (battle.GetCurrentActor() == actor)
    //        {
    //            LOG.Print("[PLAYGROUND] It's my turn !");
    //        }
    //    }

    //    void HandleBattleStart(PD_BattleStart battleStart)
    //    {
    //        battle.Start();
    //        if (battle.GetCurrentActor() == actor)
    //        {
    //            LOG.Print("[PLAYGROUND] It's my turn !");
    //        }
    //    }

    //    void HandleRoomJoin(PD_RoomJoin roomJoin)
    //    {
    //        if (roomJoin.playerName == actor.Name)
    //        {
    //            LOG.Print("[CLIENT] Welcome to {0}.", roomJoin.roomName);
    //            InitBattle(roomJoin.randomSeed);
    //            if (actor != null)
    //            {
    //                battle.Add(actor);
    //            }
    //        }
    //        else
    //        {
    //            LOG.Print("[CLIENT] {0} just joined.", roomJoin.playerName);
    //            if (roomJoin.playerInfo != null)
    //            {
    //                HandlePlayerAdd(roomJoin.playerInfo as PD_PlayerAdd);
    //            }
    //        }
    //    }

    //    public void HandlePNJAdd(PD_PNJAdd pnjAdd)
    //    {
    //        var pnj = new PNJActor(new Character(pnjAdd.race, pnjAdd.category, pnjAdd.life), pnjAdd.name, pnjAdd.initiative);
    //        battle.Add(pnj);
    //        LOG.Print("[CLIENT] {0} is here to fight !", pnj.Name);
    //    }

    //    public void InitBattle(int randomSeem)
    //    {
    //        LOG.Print("[CLIENT] Battle initialization...");
    //        battle = new Battle(randomSeem);
    //        battle.OnBattleEnd += () =>
    //        {
    //            if (actor.character.Life > 0)
    //            {
    //                LOG.Print("You won !!! ");
    //            }
    //        };

    //        Initiative.Max = 50;
    //        Hand.Max = 3;

    //        Card.Clear();
    //        List<Card> cardsMap = new List<Card>()
    //        {
    //            // name | mana cost | exhaust | effects list
    //            new Card("smol_dmg", 1, false, new List<CardEffect>
    //            {
    //                new CardEffectDamage(5)
    //            }),
    //            new Card("hek", 2, false, new List<CardEffect>
    //            {
    //                new CardEffectHeal(2)
    //            }),
    //            new Card("big_dmg", 3, false, new List<CardEffect>
    //            {
    //                new CardEffectDamage(10)
    //            })
    //        };
    //        LOG.Print("[ROOM] Adding cards");
    //        cardsMap.ForEach(c => Card.Add(c));

    //        //PNJS
    //        List<Actor> actors = new List<Actor>()
    //        {
    //            // battle | character | name | first init
    //            new PNJActor(new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 20), "monstre1", 5),
    //            new PNJActor(new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 20), "monstre2", 5),
    //            new PNJActor(new Character(Character.Race.OGRE, Character.Category.CHAMAN, 15), "monstre3", 5)
    //        };
    //        LOG.Print("[CLIENT] Adding PNJs");
    //        actors.ForEach(a => battle.Add(a));
    //    }
    //}
}
