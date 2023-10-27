using System;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Managers.Character.Talents.Objects
{
    /// <summary>
    /// Represents a talent tree that is initialized with a talentString.
    /// </summary>
    public class TalentTree
    {
        /// <summary>
        /// Initializes a new instance of the TalentTree class with a given talentString.
        /// The talentString is split into individual talents based on the "|" delimiter.
        /// Each talent is further split into individual items based on the ";" delimiter.
        /// Only talents with a length of at least 4 and items with a length of at least 5 are considered.
        /// For each valid talent, a new Talent object is created using the specified item values.
        /// The Talent object is then added to the corresponding Tree (Tree1, Tree2, or Tree3) based on the second item value.
        /// </summary>
        /// <param name="talentString">The string representation of talents, where each talent is separated by "|" and each item within a talent is separated by ";"</param>
        public TalentTree(string talentString)
        {
            Tree1 = new();
            Tree2 = new();
            Tree3 = new();

            string[] talentSplits = talentString.Split('|');

            foreach (string talent in talentSplits)
            {
                if (talent.Length < 4) { continue; }

                string[] items = talent.Split(';');

                if (items.Length < 5) { continue; }

                Talent t = new(items[0], int.Parse(items[1]), int.Parse(items[2]), int.Parse(items[3]), int.Parse(items[4]));

                if (items[1].Equals("1", StringComparison.OrdinalIgnoreCase))
                {
                    Tree1.Add(int.Parse(items[2]), t);
                }
                else if (items[1].Equals("2", StringComparison.OrdinalIgnoreCase))
                {
                    Tree2.Add(int.Parse(items[2]), t);
                }
                else if (items[1].Equals("3", StringComparison.OrdinalIgnoreCase))
                {
                    Tree3.Add(int.Parse(items[2]), t);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the TalentTree class.
        /// </summary>
        public TalentTree()
        {
        }

        /// <summary>
        /// Gets or sets the first talent tree, represented as a dictionary of integers keyed by talents.
        /// </summary>
        public Dictionary<int, Talent> Tree1 { get; set; }

        /// <summary>
        /// Gets or sets the second tree of talents, represented as a dictionary with integer keys and Talent values.
        /// </summary>
        public Dictionary<int, Talent> Tree2 { get; set; }

        /// <summary>
        /// Gets or sets the third talent tree in the form of a dictionary. The dictionary consists of integer keys and Talent values.
        /// </summary>
        public Dictionary<int, Talent> Tree3 { get; set; }

        /// <summary>
        /// Converts the current instance to a nested dictionary, with the first level keys being integers, 
        /// the second level keys being integers, and the values being instances of the Talent class.
        /// </summary>
        /// <returns>A nested dictionary representation of the current instance.</returns>
        public Dictionary<int, Dictionary<int, Talent>> AsDict()
        {
            return new()
            {
                { 1, Tree1 },
                { 2, Tree2 },
                { 3, Tree3 },
            };
        }
    }
}