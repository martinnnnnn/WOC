//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace WOC_Core
//{
//    public class PNJActor : Actor
//    {

//        public PNJActor(
//            Character character,
//            string name,
//            int initialInitiative) : base(character, name)
//        {
//            initiative.Set(initialInitiative);
//        }

//        public override void BattleInit()
//        {
//            base.BattleInit();
//        }

//        public override void BattleEnd()
//        {
//            base.BattleEnd();
//        }

//        public override bool StartTurn()
//        {
//            if (base.StartTurn())
//            {
//                Console.WriteLine("[PNJ] {0} plays his turn", Name);
//                return EndTurn();
//            }
//            return false;
//        }

//        public override bool EndTurn()
//        {
//            if (base.EndTurn())
//            {
//                Battle.NextActor().StartTurn();
//                return true;
//            }
//            return false;
//        }
//        public override void UpdateInitiative()
//        {
//            //initiative.FromCardCount(initialInitiative);
//        }
//    }
//}
