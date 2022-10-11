using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raidable_Bases_Spreadsheet_Parser
{
    public static class Program
    {
        public enum DifficultyType
        {
            Easy = 0,
            Medium = 1,
            Hard = 2,
            Expert = 3,
            Nightmare = 4
        }

        public enum SpreadSheetColumnType
        {
            Shortname = 0,
            QuantityEasy = 1,
            QuantityMedium = 2,
            QuantityHard = 3,
            QuantityExpert = 4,
            QuantityNightmare = 5,
            ProbabilityEasy = 6,
            ProbabilityMedium = 7,
            ProbabilityHard = 8,
            ProbabilityExpert = 9,
            ProbabilityNightmare = 10,
            StackSize = 11

        }

        private static double Normalize(double value)
        {
            return value / 100.0D;
        }

        public static void Main(string[] args)
        {
            /**
             * Create and Initialize the Database
             */
            List<GameItem> database = new List<GameItem>();
            /**
             * Begin parsing the spreadsheet.
             */
            Console.WriteLine("Example File Path: \"C:/Users/Krythic/Desktop/LootTableData.txt\"");
            Console.Write("Input File Path:");
            string path = Console.ReadLine();
            if (File.Exists(path))
            {
                string[] spreadsheet = File.ReadAllLines(path);
                foreach (string rowData in spreadsheet)
                {
                    // Clear the GameItem Cache
                    GameItem gameItem = new GameItem();
                    string[] columnData = rowData.Split('\t');
                    for (int i = 0; i < columnData.Length; i++)
                    {
                        switch (i)
                        {
                            /**
                             * Parse Short Name Column
                             */
                            case (int)SpreadSheetColumnType.Shortname:
                                string shortname = columnData[i];
                                if (shortname != null && shortname.Length > 0)
                                {
                                    gameItem.Shortname = shortname;
                                }
                                break;
                            /**
                             * Parse Difficulty Columns
                             */
                            case (int)SpreadSheetColumnType.QuantityEasy:
                            case (int)SpreadSheetColumnType.QuantityMedium:
                            case (int)SpreadSheetColumnType.QuantityHard:
                            case (int)SpreadSheetColumnType.QuantityExpert:
                            case (int)SpreadSheetColumnType.QuantityNightmare:
                                string quantityColumnData = columnData[i];

                                if (quantityColumnData != null && quantityColumnData.Length > 0)
                                {
                                    if (quantityColumnData.Equals("--"))
                                    {
                                        quantityColumnData = "0-0";
                                    }
                                    string[] minMax = quantityColumnData.Split('-');
                                    if (minMax.Length == 2)
                                    {
                                        int minimum = Int32.Parse(minMax[0]);
                                        int maximum = Int32.Parse(minMax[1]);
                                        gameItem.AmountMin[ConvertToDifficultyType((SpreadSheetColumnType)i)] = minimum;
                                        gameItem.Amount[ConvertToDifficultyType((SpreadSheetColumnType)i)] = maximum;
                                    }
                                }
                                break;

                            /**
                             * Parse Probability Columns
                             */
                            case (int)SpreadSheetColumnType.ProbabilityEasy:
                            case (int)SpreadSheetColumnType.ProbabilityMedium:
                            case (int)SpreadSheetColumnType.ProbabilityHard:
                            case (int)SpreadSheetColumnType.ProbabilityExpert:
                            case (int)SpreadSheetColumnType.ProbabilityNightmare:
                                string probabilityColumnData = columnData[i];
                                if (probabilityColumnData != null && probabilityColumnData.Length > 0)
                                {
                                    if (probabilityColumnData.Equals("--"))
                                    {
                                        probabilityColumnData = "0.00%";
                                    }
                                    double probability = Double.Parse(probabilityColumnData.Replace("%", ""));
                                    gameItem.Probability[ConvertToDifficultyType((SpreadSheetColumnType)i)] = Normalize(probability);

                                }
                                break;
                            /**
                             * Parse Stack Size
                             */
                            case (int)SpreadSheetColumnType.StackSize:
                                gameItem.StackSize = Int32.Parse(columnData[i]);
                                break;

                            default:
                                break;
                        }

                    }
                    /**
                     * Add the parsed item to the database
                     */
                    database.Add(gameItem);
                }
                Console.WriteLine("Successfully parsed " + database.Count + " GameItems");
                Console.WriteLine("Generating JSON Files...");
                /**
                 * Generate the JSON files
                 */
                DifficultyType[] difficulties = (DifficultyType[])Enum.GetValues(typeof(DifficultyType)); // Cached for performance
                Dictionary<DifficultyType, StringBuilder> builders = new Dictionary<DifficultyType, StringBuilder>();
                foreach (DifficultyType difficultyType in difficulties)
                {
                    builders.Add(difficultyType, new StringBuilder());
                }
                foreach (DifficultyType difficultyType in difficulties)
                {
                    StringBuilder difficultyBuilder = builders[difficultyType];
                    difficultyBuilder.AppendLine("[");
                    foreach (GameItem item in database)
                    {
                        difficultyBuilder.AppendLine("{");
                        difficultyBuilder.AppendLine("\"shortname\": \"" + item.Shortname + "\",");
                        difficultyBuilder.AppendLine("\"name\": null,");
                        difficultyBuilder.AppendLine("\"amount\":" + item.Amount[difficultyType] + ",");
                        difficultyBuilder.AppendLine("\"skin\": " + item.Skin + ",");
                        difficultyBuilder.AppendLine("\"amountmin\": " + item.AmountMin[difficultyType] + ",");
                        difficultyBuilder.AppendLine("\"probability\": " + item.Probability[difficultyType] + ",");
                        difficultyBuilder.AppendLine("\"stacksize\": " + item.StackSize);
                        difficultyBuilder.AppendLine("},");
                    }
                    string finalOutput = difficultyBuilder.ToString().Trim().TrimEnd(',') + "\n]";
                    string outputDirectory = Path.GetDirectoryName(path) + "/LootTables/";
                    Directory.CreateDirectory(outputDirectory);
                    File.WriteAllText(outputDirectory + GetDifficultyFileName(difficultyType), finalOutput);
                    Console.WriteLine("Successfully Generated " + GetDifficultyFileName(difficultyType));
                }
                Console.WriteLine("Finished!");
                Console.Read();
            }
            else
            {
                Console.WriteLine("File at path: '" + path + "' does not exist!");
            }
        }

        public static string GetDifficultyFileName(DifficultyType difficulty)
        {
            switch (difficulty)
            {
                case DifficultyType.Easy:
                    return "Easy.json";
                case DifficultyType.Medium:
                    return "Medium.json";
                case DifficultyType.Hard:
                    return "Hard.json";
                case DifficultyType.Expert:
                    return "Expert.json";
                case DifficultyType.Nightmare:
                    return "Nightmare.json";
                default:
                    throw new Exception("Could not get file name for difficulty: " + difficulty);
            }
        }

        public static DifficultyType ConvertToDifficultyType(SpreadSheetColumnType column)
        {
            switch (column)
            {
                case SpreadSheetColumnType.QuantityEasy:
                case SpreadSheetColumnType.ProbabilityEasy:
                    return DifficultyType.Easy;
                case SpreadSheetColumnType.QuantityMedium:
                case SpreadSheetColumnType.ProbabilityMedium:
                    return DifficultyType.Medium;
                case SpreadSheetColumnType.QuantityHard:
                case SpreadSheetColumnType.ProbabilityHard:
                    return DifficultyType.Hard;
                case SpreadSheetColumnType.QuantityExpert:
                case SpreadSheetColumnType.ProbabilityExpert:
                    return DifficultyType.Expert;
                case SpreadSheetColumnType.QuantityNightmare:
                case SpreadSheetColumnType.ProbabilityNightmare:
                    return DifficultyType.Nightmare;
                default:
                    return DifficultyType.Easy; // Dirty hack.
            }
        }
    }
}
