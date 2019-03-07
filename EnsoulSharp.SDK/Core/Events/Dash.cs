// <copyright file="Dash.cs" company="EnsoulSharp">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using SharpDX;

    /// <summary>
    ///     Dash class, contains the OnDash event for tracking for Dash events of a champion.
    /// </summary>
    public static partial class Events
    {
        #region Static Fields

        /// <summary>
        ///     DetectedDashes list.
        /// </summary>
        private static readonly Dictionary<uint, DashArgs> DetectedDashes = new Dictionary<uint, DashArgs>();

        #endregion

        #region Public Events

        /// <summary>
        ///     OnDash Event.
        /// </summary>
        public static event EventHandler<DashArgs> OnDash;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the speed of the dashing unit if it is dashing.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        /// <returns>
        ///     The <see cref="DashArgs" />.
        /// </returns>
        public static DashArgs GetDashInfo(this AIBaseClient unit)
        {
            DashArgs value;
            return DetectedDashes.TryGetValue(unit.NetworkId, out value) ? value : new DashArgs();
        }

        /// <summary>
        ///     Returns true if the unit is dashing.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsDashing(this AIBaseClient unit)
        {
            DashArgs value;
            if (DetectedDashes.TryGetValue(unit.NetworkId, out value) && unit.Path.Length != 0)
            {
                return value.EndTick != 0;
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     New Path subscribed event function.
        /// </summary>
        /// <param name="sender"><see cref="AIBaseClient" /> sender</param>
        /// <param name="args">New Path event data</param>
        private static void EventDash(AIBaseClient sender, AIBaseClientNewPathEventArgs args)
        {
            var hero = sender as AIHeroClient;
            if (hero != null && hero.IsValid)
            {
                if (!DetectedDashes.ContainsKey(hero.NetworkId))
                {
                    DetectedDashes.Add(hero.NetworkId, new DashArgs());
                }

                if (args.IsDash)
                {
                    var path = new List<Vector2> { hero.Position.ToVector2() };
                    path.AddRange(args.Path.ToList().ToVector2());

                    DetectedDashes[hero.NetworkId].Unit = sender;
                    DetectedDashes[hero.NetworkId].Path = path;
                    DetectedDashes[hero.NetworkId].Speed = args.Speed;
                    DetectedDashes[hero.NetworkId].StartPos = hero.Position.ToVector2();
                    DetectedDashes[hero.NetworkId].StartTick = Variables.TickCount - (Game.Ping / 2);
                    DetectedDashes[hero.NetworkId].EndPos = path.Last();
                    DetectedDashes[hero.NetworkId].EndTick = DetectedDashes[hero.NetworkId].StartTick
                                                             + (int)
                                                               (1000
                                                                * (DetectedDashes[hero.NetworkId].EndPos.Distance(
                                                                    DetectedDashes[hero.NetworkId].StartPos)
                                                                   / DetectedDashes[hero.NetworkId].Speed));
                    DetectedDashes[hero.NetworkId].Duration = DetectedDashes[hero.NetworkId].EndTick
                                                              - DetectedDashes[hero.NetworkId].StartTick;

                    OnDash?.Invoke(MethodBase.GetCurrentMethod().DeclaringType, DetectedDashes[hero.NetworkId]);
                }
                else
                {
                    DetectedDashes[hero.NetworkId].EndTick = 0;
                }
            }
        }

        #endregion

        /// <summary>
        ///     Dash event data.
        /// </summary>
        public class DashArgs : EventArgs
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the dash duration.
            /// </summary>
            public int Duration { get; set; }

            /// <summary>
            ///     Gets or sets the end position.
            /// </summary>
            public Vector2 EndPos { get; set; }

            /// <summary>
            ///     Gets or sets the end tick.
            /// </summary>
            public int EndTick { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether is blink.
            /// </summary>
            public bool IsBlink { get; set; }

            /// <summary>
            ///     Gets or sets the path.
            /// </summary>
            public List<Vector2> Path { get; set; }

            /// <summary>
            ///     Gets or sets the speed.
            /// </summary>
            public float Speed { get; set; }

            /// <summary>
            ///     Gets or sets the start position.
            /// </summary>
            public Vector2 StartPos { get; set; }

            /// <summary>
            ///     Gets or sets the start tick.
            /// </summary>
            public int StartTick { get; set; }

            /// <summary>
            ///     Gets or sets the unit.
            /// </summary>
            public AIBaseClient Unit { get; set; }

            #endregion
        }
    }
}