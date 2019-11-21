using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Core
{

    public interface ICardEffect { }

    public class Card
    {
        public string name = "";
        public int timeCost = 0;
        public List<ICardEffect> effects = new List<ICardEffect>();
    }

    public class Deck
    {
        public string name;
        public List<Card> cards = new List<Card>();
    }
}