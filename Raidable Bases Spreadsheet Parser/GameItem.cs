using System;
using System.Collections.Generic;
using static Raidable_Bases_Spreadsheet_Parser.Program;

namespace Raidable_Bases_Spreadsheet_Parser
{
    public class GameItem
    {
        public string Shortname { get; set; }
        public string Name { get; set; }
        public Dictionary<DifficultyType, int> Amount { get; set; }
        public int Skin { get; set; }
        public Dictionary<DifficultyType, int> AmountMin { get; set; }
        public Dictionary<DifficultyType, double> Probability { get; set; }
        public int StackSize { get; set; }
        public static readonly DifficultyType[] Difficulties;

        static GameItem()
        {
            Difficulties = (DifficultyType[])Enum.GetValues(typeof(DifficultyType));
        }

        public GameItem()
        {
            Amount = new Dictionary<DifficultyType, int>();
            foreach (DifficultyType difficulty in Difficulties)
            {
                Amount.Add(difficulty, 0);
            }
            AmountMin = new Dictionary<DifficultyType, int>();
            foreach (DifficultyType difficulty in Difficulties)
            {
                AmountMin.Add(difficulty, 0);
            }
            Probability = new Dictionary<DifficultyType, double>();
            foreach (DifficultyType difficulty in Difficulties)
            {
                Probability.Add(difficulty, 0);
            }
        }
    }
}
