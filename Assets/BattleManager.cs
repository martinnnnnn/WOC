using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using WOC_Core;

namespace WOC_Client
{
    //public class BattleManager : MonoBehaviour
    //{
    //    NetworkInterface network;

    //    WOC_Core.RTTS.Battle battle;

    //    public Transform[] avatarsPositions;
    //    public GameObject avatarPrefab;
    //    public List<AvatarController> avatarControllers = new List<AvatarController>();


    //    public Transform[] monstersPositions;
    //    public GameObject monsterPrefab;
    //    public List<MonsterController> monsterControllers = new List<MonsterController>();

    //    public WOC_Core.RTTS.BattlePlayer mainPlayer;

    //    private void Start()
    //    {
    //        network = FindObjectOfType<NetworkInterface>();
    //        network.Callback_BattleStart += HandleAPICall;
    //        network.Callback_BattlePlayCard += HandleAPICall;
    //        network.Callback_BattlePlayerTurnEnd += HandleAPICall;
    //    }

    //    private void Update()
    //    {
    //        battle.Update(Time.deltaTime);
    //    }

    //    public void HandleAPICall(PD_BattlePlayerTurnEnd data)
    //    {
    //        battle.PlayerTurnEnd(data.name);
    //    }

    //    public void HandleAPICall(PD_BattleStart data)
    //    {

    //        List<WOC_Core.RTTS.BattlePlayer> players =
    //            new List<WOC_Core.RTTS.BattlePlayer>(network.accountNames.Select(n =>
    //                new WOC_Core.RTTS.BattlePlayer(battle, n, new WOC_Core.RTTS.Deck()
    //                {
    //                    name = "defaultDeck",
    //                    cards = new List<WOC_Core.RTTS.Card>()
    //                    {
    //                        new WOC_Core.RTTS.Card() { name = "card1", timeCost = 1 },
    //                        new WOC_Core.RTTS.Card() { name = "card1", timeCost = 1 },
    //                        new WOC_Core.RTTS.Card() { name = "card1", timeCost = 1 },
    //                        new WOC_Core.RTTS.Card() { name = "card1", timeCost = 1 },
    //                        new WOC_Core.RTTS.Card() { name = "card2", timeCost = 2 },
    //                        new WOC_Core.RTTS.Card() { name = "card2", timeCost = 2 },
    //                        new WOC_Core.RTTS.Card() { name = "card2", timeCost = 2 },
    //                        new WOC_Core.RTTS.Card() { name = "card2", timeCost = 2 },
    //                        new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
    //                        new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
    //                        new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
    //                        new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
    //                        new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
    //                        new WOC_Core.RTTS.Card() { name = "card4", timeCost = 4 },
    //                        new WOC_Core.RTTS.Card() { name = "card4", timeCost = 4 },
    //                        new WOC_Core.RTTS.Card() { name = "card4", timeCost = 4 },
    //                        new WOC_Core.RTTS.Card() { name = "card4", timeCost = 4 },
    //                        new WOC_Core.RTTS.Card() { name = "card5", timeCost = 5 },
    //                        new WOC_Core.RTTS.Card() { name = "card5", timeCost = 5 },
    //                        new WOC_Core.RTTS.Card() { name = "card5", timeCost = 5 },
    //                        new WOC_Core.RTTS.Card() { name = "card6", timeCost = 6 },
    //                    }
    //                }
    //        )));

    //        mainPlayer = new WOC_Core.RTTS.BattlePlayer(battle, network.session.account.name, new WOC_Core.RTTS.Deck()
    //        {
    //            name = "defaultDeck",
    //            cards = new List<WOC_Core.RTTS.Card>()
    //            {
    //                new WOC_Core.RTTS.Card() { name = "card1", timeCost = 1 },
    //                new WOC_Core.RTTS.Card() { name = "card1", timeCost = 1 },
    //                new WOC_Core.RTTS.Card() { name = "card1", timeCost = 1 },
    //                new WOC_Core.RTTS.Card() { name = "card1", timeCost = 1 },
    //                new WOC_Core.RTTS.Card() { name = "card2", timeCost = 2 },
    //                new WOC_Core.RTTS.Card() { name = "card2", timeCost = 2 },
    //                new WOC_Core.RTTS.Card() { name = "card2", timeCost = 2 },
    //                new WOC_Core.RTTS.Card() { name = "card2", timeCost = 2 },
    //                new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
    //                new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
    //                new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
    //                new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
    //                new WOC_Core.RTTS.Card() { name = "card3", timeCost = 3 },
    //                new WOC_Core.RTTS.Card() { name = "card4", timeCost = 4 },
    //                new WOC_Core.RTTS.Card() { name = "card4", timeCost = 4 },
    //                new WOC_Core.RTTS.Card() { name = "card4", timeCost = 4 },
    //                new WOC_Core.RTTS.Card() { name = "card4", timeCost = 4 },
    //                new WOC_Core.RTTS.Card() { name = "card5", timeCost = 5 },
    //                new WOC_Core.RTTS.Card() { name = "card5", timeCost = 5 },
    //                new WOC_Core.RTTS.Card() { name = "card5", timeCost = 5 },
    //                new WOC_Core.RTTS.Card() { name = "card6", timeCost = 6 }
    //            }
    //        });

    //        players.Add(mainPlayer);

    //        avatarControllers.Clear();
    //        for (int i = 0; i < players.Count + 1; ++i)
    //        {
    //            GameObject newAvatar = Instantiate(avatarPrefab, this.transform);
    //            AvatarController newController = newAvatar.GetComponent<AvatarController>();
    //            newController.transform.position = avatarsPositions[i].position;
    //            newController.SetPlayer(players[i]);
    //            avatarControllers.Add(newController);
    //        }

    //        List<WOC_Core.RTTS.Monster> monsters = new List<WOC_Core.RTTS.Monster>()
    //        {
    //            new WOC_Core.RTTS.Monster("monster", 15)
    //        };
    //        for (int i = 0; i < monsters.Count; ++i)
    //        {
    //            GameObject newMonster = Instantiate(monsterPrefab, this.transform);
    //            MonsterController newController = newMonster.GetComponent<MonsterController>();
    //            newController.transform.position = monstersPositions[i].position;
    //            monsterControllers.Add(newController);
    //        }

    //        battle = new WOC_Core.RTTS.Battle(players, monsters, data.randomSeed);

            
    //    }

    //    public void HandleAPICall(PD_BattlePlayCard data)
    //    {
    //        battle.PlayCard(data.ownerName, data.cardIndex, data.targetName, true);
    //    }
    //}
}

