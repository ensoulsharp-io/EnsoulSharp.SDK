// <copyright file="Jungle.cs" company="EnsoulSharp">
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

namespace EnsoulSharp.SDK.Core.Utils
{
    using System.Linq;
    using System.Text.RegularExpressions;

    using EnsoulSharp.SDK;

    /// <summary>
    ///     The jungle utility class, provides utils for jungle related items.
    /// </summary>
    public static class Jungle
    {
        #region Static Fields

        /// <summary>
        ///     The large name regex list.
        /// </summary>
        private static readonly string[] LargeNameRegex =
            {
                "SRU_Razorbeak", "SRU_Red", "SRU_Krug",
                "SRU_Murkwolf", "SRU_Blue", "SRU_Gromp",
                "Sru_Crab",
                "TT_NGolem", "TT_NWraith", "TT_NWolf"
            };

        /// <summary>
        ///     The legendary name regex list.
        /// </summary>
        private static readonly string[] LegendaryNameRegex =
            {
                "SRU_Dragon_Air", "SRU_Dragon_Earth", "SRU_Dragon_Fire", "SRU_Dragon_Water", "SRU_Dragon_Elder",
                "SRU_RiftHerald", "SRU_Baron",
                "TT_Spiderboss"
            };

        /// <summary>
        ///     The small name regex list.
        /// </summary>
        private static readonly string[] SmallNameRegex =
            {
                "SRU_RazorbeakMini", "SRU_MurkwolfMini", "SRU_KrugMini", "TestCubeRender",
                "TT_NGolem2", "TT_NWraith2", "TT_NWolf2"
            };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get the minion jungle type.
        /// </summary>
        /// <param name="minion">
        ///     The minion
        /// </param>
        /// <returns>
        ///     The <see cref="JungleType" />
        /// </returns>
        public static JungleType GetJungleType(this AIMinionClient minion)
        {
            if (SmallNameRegex.Any(regex => Regex.IsMatch(minion.CharacterName, regex)))
            {
                return JungleType.Small;
            }

            if (LargeNameRegex.Any(regex => Regex.IsMatch(minion.CharacterName, regex)))
            {
                return JungleType.Large;
            }

            if (LegendaryNameRegex.Any(regex => Regex.IsMatch(minion.CharacterName, regex)))
            {
                return JungleType.Legendary;
            }

            return JungleType.Unknown;
        }

        /// <summary>
        ///     Indicates whether the object is a jungle buff carrier.
        /// </summary>
        /// <param name="minion">
        ///     The minion.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsJungleBuff(this AIMinionClient minion)
        {
            var @base = minion.CharacterName;
            return @base.Equals("SRU_Blue") || @base.Equals("SRU_Red");
        }

        #endregion
    }
}