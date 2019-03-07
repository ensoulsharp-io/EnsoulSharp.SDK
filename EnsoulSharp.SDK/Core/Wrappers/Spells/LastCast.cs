// <copyright file="LastCast.cs" company="EnsoulSharp">
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

namespace EnsoulSharp.SDK
{
    using System.Collections.Generic;

    /// <summary>
    ///     Extension for getting the last casted spell of an <see cref="AIHeroClient" />
    /// </summary>
    public static class LastCast
    {
        #region Static Fields

        /// <summary>
        ///     Casted Spells of the champions
        /// </summary>
        internal static readonly Dictionary<uint, LastCastedSpellEntry> CastedSpells =
            new Dictionary<uint, LastCastedSpellEntry>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="LastCast" /> class.
        ///     Static constructor
        /// </summary>
        static LastCast()
        {
            AIBaseClient.OnDoCast += AIHeroClient_OnDoCast;
            Spellbook.OnCastSpell += OnCastSpell;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the last cast packet sent.
        /// </summary>
        public static LastCastPacketSentEntry LastCastPacketSent { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the <see cref="LastCastedSpellEntry" /> of the unit.
        /// </summary>
        /// <param name="target">The Target</param>
        /// <returns>
        ///     <see cref="LastCastedSpellEntry" />
        /// </returns>
        public static LastCastedSpellEntry GetLastCastedSpell(this AIHeroClient target)
        {
            LastCastedSpellEntry entry;
            var contains = CastedSpells.TryGetValue(target.NetworkId, out entry);

            return contains ? entry : new LastCastedSpellEntry();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Function that is called by the OnDoCast event.
        /// </summary>
        /// <param name="sender">
        ///     The Sender
        /// </param>
        /// <param name="args">
        ///     Processed Spell Cast Data
        /// </param>
        private static void AIHeroClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            var hero = sender as AIHeroClient;
            if (hero != null)
            {
                var entry = new LastCastedSpellEntry(args);
                if (!CastedSpells.ContainsKey(sender.NetworkId))
                {
                    CastedSpells.Add(sender.NetworkId, entry);
                    return;
                }

                CastedSpells[sender.NetworkId] = entry;
            }
        }

        /// <summary>
        ///     OnCastSpell event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe)
            {
                LastCastPacketSent = new LastCastPacketSentEntry(
                    args.Slot,
                    Variables.TickCount,
                    (args.Target is AIBaseClient) ? args.Target.NetworkId : 0);
            }
        }

        #endregion
    }
}