using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmeisenBotX.Core.Managers.Character.Talents
{
    public class TalentManager
    {
        /// <summary>
        /// Creates a new instance of the TalentManager class.
        /// </summary>
        /// <param name="wowInterface">The interface to the World of Warcraft game.</param>
        public TalentManager(IWowInterface wowInterface)
        {
            Wow = wowInterface;
        }

        /// <summary>
        /// Gets or sets the TalentTree property.
        /// </summary>
        public TalentTree TalentTree { get; set; }

        /// <summary>
        /// Gets or sets the WoW interface.
        /// </summary>
        private IWowInterface Wow { get; }

        /// <summary>
        /// Selects talents from a talent tree based on specified criteria.
        /// </summary>
        /// <param name="wantedTalents">The TalentTree object representing the desired talents.</param>
        /// <param name="talentPoints">The number of available talent points.</param>
        public void SelectTalents(TalentTree wantedTalents, int talentPoints)
        {
            Dictionary<int, Dictionary<int, Talent>> talentTrees = TalentTree.AsDict();
            Dictionary<int, Dictionary<int, Talent>> wantedTalentTrees = wantedTalents.AsDict();

            List<(int, int, int)> talentsToSpend = new();

            // order the trees to skill the main tree first
            foreach (KeyValuePair<int, Dictionary<int, Talent>> kv in wantedTalentTrees.OrderByDescending(e => e.Value.Count))
            {
                if (CheckTalentTree(ref talentPoints, kv.Key, talentTrees[kv.Key], kv.Value, out List<(int, int, int)> newTalents))
                {
                    talentsToSpend.AddRange(newTalents);
                }
            }

            if (talentsToSpend.Any())
            {
                SpendTalents(talentsToSpend);
            }
        }

        /// <summary>
        /// Updates the TalentTree object by getting the talents from Wow.
        /// </summary>
        public void Update()
        {
            TalentTree = new(Wow.GetTalents());
        }

        /// <summary>
        /// Checks if the talent tree can be updated with the available talent points.
        /// </summary>
        /// <param name="talentPoints">The total number of talent points available</param>
        /// <param name="treeId">The ID of the talent tree</param>
        /// <param name="tree">A dictionary representing the current talent tree</param>
        /// <param name="wantedTree">A dictionary representing the desired talent tree</param>
        /// <param name="talentsToSpend">An output list of tuples representing the talents to spend points on (treeId, talentNum, amount)</param>
        /// <returns>True if the talent tree can be updated, false otherwise</returns>
        private static bool CheckTalentTree(ref int talentPoints, int treeId, Dictionary<int, Talent> tree, Dictionary<int, Talent> wantedTree, out List<(int, int, int)> talentsToSpend)
        {
            talentsToSpend = new();

            if (talentPoints == 0)
            {
                return false;
            }

            bool result = false;

            Talent[] wantedTreeValues = wantedTree.Values.ToArray();

            foreach (Talent talent in wantedTreeValues)
            {
                if (talentPoints == 0) { break; }

                Talent wantedTalent = talent;

                if (tree.ContainsKey(wantedTalent.Num))
                {
                    int wantedRank = Math.Min(wantedTalent.Rank, tree[wantedTalent.Num].MaxRank);

                    if (tree[wantedTalent.Num].Rank < wantedRank)
                    {
                        int amount = Math.Min(talentPoints, wantedRank - tree[wantedTalent.Num].Rank);

                        talentsToSpend.Add((treeId, wantedTalent.Num, amount));

                        talentPoints -= amount;
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Spends the specified talents by adding preview talent points and learning preview talents.
        /// </summary>
        /// <param name="talentsToSpend">A list of tuples representing the talents to spend, where each tuple contains three integers.</param>
        private void SpendTalents(List<(int, int, int)> talentsToSpend)
        {
            StringBuilder sb = new();

            for (int i = 0; i < talentsToSpend.Count; ++i)
            {
                sb.Append($"AddPreviewTalentPoints({talentsToSpend[i].Item1},{talentsToSpend[i].Item2},{talentsToSpend[i].Item3});");
            }

            sb.Append("LearnPreviewTalents();");

            Wow.LuaDoString(sb.ToString());
        }
    }
}