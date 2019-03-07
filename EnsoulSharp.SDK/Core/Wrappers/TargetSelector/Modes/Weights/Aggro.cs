// <copyright file="Aggro.cs" company="EnsoulSharp">
//    Copyright (c) 2019 EnsoulSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace EnsoulSharp.SDK.Modes.Weights
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    ///     Aggro tracking
    /// </summary>
    public static class Aggro
    {
        #region Static Fields

        private static readonly Dictionary<uint, AggroEntry> PEntries = new Dictionary<uint, AggroEntry>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Aggro" /> class.
        /// </summary>
        static Aggro()
        {
            AIBaseClient.OnAggro += OnAIBaseClientAggro;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the entries
        /// </summary>
        public static ReadOnlyDictionary<uint, AggroEntry> Entries => new ReadOnlyDictionary<uint, AggroEntry>(PEntries);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the sender items.
        /// </summary>
        /// <param name="sender">The sender.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable{T}" /> of <see cref="AggroEntry" />.
        /// </returns>
        public static IEnumerable<AggroEntry> GetSenderItems(AIBaseClient sender)
        {
            return PEntries.Where(i => i.Key.Equals(sender.NetworkId)).Select(i => i.Value);
        }

        /// <summary>
        ///     Gets the sender target item.
        /// </summary>
        /// <param name="sender">The sender.
        /// </param>
        /// <param name="target">The target.
        /// </param>
        /// <returns>
        ///     The <see cref="AggroEntry" />.
        /// </returns>
        public static AggroEntry GetSenderTargetItem(AIBaseClient sender, AIBaseClient target)
        {
            return GetSenderItems(sender).FirstOrDefault(entry => entry.Target.Compare(target));
        }

        /// <summary>
        ///     Gets the target items.
        /// </summary>
        /// <param name="target">The target.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable{T}" /> of <see cref="AggroEntry" />.
        /// </returns>
        public static IEnumerable<AggroEntry> GetTargetItems(AIBaseClient target)
        {
            return PEntries.Where(i => i.Value.Target.Compare(target)).Select(i => i.Value);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Called when aggro is changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The <see cref="EnsoulSharp.GameObjectAggroEventArgs" /> instance containing the event data.
        /// </param>
        private static void OnAIBaseClientAggro(AIBaseClient sender, AIBaseClientAggroEventArgs args)
        {
            if (!sender.IsEnemy)
            {
                return;
            }

            var hero = sender as AIHeroClient;
            var target = GameObjects.EnemyHeroes.FirstOrDefault(h => h.NetworkId == args.TargetId);
            if (hero != null && target != null)
            {
                AggroEntry aggro;
                if (PEntries.TryGetValue(hero.NetworkId, out aggro))
                {
                    aggro.Target = target;
                }
                else
                {
                    PEntries[hero.NetworkId] = new AggroEntry(hero, target);
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     Entry for the class Aggro
    /// </summary>
    public class AggroEntry
    {
        #region Fields

        /// <summary>
        ///     The sender.
        /// </summary>
        private AIHeroClient sender;

        /// <summary>
        ///     The target.
        /// </summary>
        private AIHeroClient target;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AggroEntry" /> class.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="hero">
        ///     The hero.
        /// </param>
        public AggroEntry(AIHeroClient sender, AIHeroClient hero)
        {
            this.Sender = sender;
            this.Target = hero;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the sender.
        /// </summary>
        public AIHeroClient Sender
        {
            get
            {
                return this.sender;
            }

            set
            {
                this.sender = value;
                this.TickCount = Variables.TickCount;
            }
        }

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        public AIHeroClient Target
        {
            get
            {
                return this.target;
            }

            set
            {
                this.target = value;
                this.TickCount = Variables.TickCount;
            }
        }

        /// <summary>
        ///     Gets the tick count.
        /// </summary>
        public int TickCount { get; private set; }

        #endregion
    }
}