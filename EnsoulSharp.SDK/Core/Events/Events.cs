// <copyright file="Events.cs" company="EnsoulSharp">
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

    using EnsoulSharp.SDK.Core.Utils;

    /// <summary>
    ///     The provided events by the kit.
    /// </summary>
    [ResourceImport]
    public static partial class Events
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Events" /> class.
        /// </summary>
        static Events()
        {
            Game.OnUpdate += OnUpdate;
            AIBaseClient.OnDoCast += OnDoCast;
            AIBaseClient.OnNewPath += OnNewPath;
            Spellbook.OnStopCast += OnStopCast;
            GameObject.OnAssign += OnAssign;
            GameObject.OnIntegerPropertyChange += OnIntegerPropertyChange;
            AIBaseClient.OnTeleport += OnTeleportEvent;

            EventTurretConstruct();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     On assign event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnAssign(GameObject sender, EventArgs args)
        {
            EventTurret(sender);
        }

        /// <summary>
        ///     On integer property change event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnIntegerPropertyChange(GameObject sender, GameObjectIntegerPropertyChangeEventArgs args)
        {
            EventStealth(sender, args);
        }

        /// <summary>
        ///     On new path event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnNewPath(AIBaseClient sender, AIBaseClientNewPathEventArgs args)
        {
            EventDash(sender, args);
        }

        /// <summary>
        ///     On do cast event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            EventGapcloser(sender, args);
            EventInterruptableSpell(sender, args);
            EventTurret(sender);
        }

        /// <summary>
        ///     On stop cast event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnStopCast(AIBaseClient sender, SpellbookStopCastEventArgs args)
        {
            EventInterruptableSpell(sender.Spellbook);
        }

        /// <summary>
        ///     On teleport event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnTeleportEvent(AIBaseClient sender, AIBaseClientTeleportEventArgs args)
        {
            EventTeleport(sender, args);
        }

        /// <summary>
        ///     OnUpdate event.
        /// </summary>
        /// <param name="args">
        ///     The event args.
        /// </param>
        private static void OnUpdate(EventArgs args)
        {
            EventLoad();
            EventGapcloser();
            EventInterruptableSpell();
        }

        #endregion
    }
}