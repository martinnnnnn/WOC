using System.Collections.Generic;



namespace WOC_Network
{
    public class Card
    {
        string name;
    }

    public class PacketDataDeckUpdate : IPacketData
    {
        string name;
        List<Card> cards;
    }
}
