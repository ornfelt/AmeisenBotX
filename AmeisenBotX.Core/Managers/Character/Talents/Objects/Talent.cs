namespace AmeisenBotX.Core.Managers.Character.Talents.Objects
{
    /// <summary>
    /// Initializes a new instance of the Talent class with the specified tab, number, and rank.
    /// </summary>
    public class Talent
    {
        /// <summary>
        /// Initializes a new instance of the Talent class with the specified tab, number, and rank.
        /// </summary>
        public Talent(int tab, int num, int rank)
        {
            Tab = tab;
            Num = num;
            Rank = rank;
        }

        /// <summary>
        /// Initializes a new instance of the Talent class with the specified parameters.
        /// </summary>
        /// <param name="name">The name of the talent.</param>
        /// <param name="tab">The tab of the talent.</param>
        /// <param name="num">The number of the talent.</param>
        /// <param name="rank">The current rank of the talent.</param>
        /// <param name="maxRank">The maximum rank of the talent.</param>
        public Talent(string name, int tab, int num, int rank, int maxRank)
        {
            Name = name;
            Tab = tab;
            Num = num;
            Rank = rank;
            MaxRank = maxRank;
        }

        /// <summary>
        /// Gets or sets the maximum rank.
        /// </summary>
        public int MaxRank { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the Num property.
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// Gets or sets the rank.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets the value of the Tab property.
        /// </summary>
        public int Tab { get; set; }

        /// <summary>
        /// Returns a string representation of the object, including the tab, num, name, rank, and maximum rank information.
        /// </summary>
        public override string ToString()
        {
            return $"[{Tab}][{Num}] {Name}: {Rank}/{MaxRank}";
        }
    }
}