// <copyright file="Stealth.cs" company="EnsoulSharp">
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
    using System.Reflection;

    /// <summary>
    ///     Provides events for OnStealth
    /// </summary>
    public static partial class Events
    {
        #region Public Events

        /// <summary>
        ///     Gets fired when any hero is invisible.
        /// </summary>
        public static event EventHandler<OnStealthEventArgs> OnStealth;

        #endregion

        #region Methods

        /// <summary>
        ///     Function is called when a <see cref="GameObject" /> gets an integer property change and is called by an event.
        /// </summary>
        /// <param name="sender">
        ///     GameObject sender
        /// </param>
        /// <param name="args">
        ///     Integer Property Change Data
        /// </param>
        private static void EventStealth(GameObject sender, GameObjectIntegerPropertyChangeEventArgs args)
        {
            var hero = sender as AIHeroClient;
            if (hero == null || !args.Property.Equals("ActionState"))
            {
                return;
            }

            var oldState = (GameObjectCharacterState)args.OldValue;
            var newState = (GameObjectCharacterState)args.NewValue;

            if (!oldState.HasFlag(GameObjectCharacterState.IsStealthed)
                && newState.HasFlag(GameObjectCharacterState.IsStealthed))
            {
                FireOnStealth(new OnStealthEventArgs { Sender = hero, Time = Game.Time, IsStealthed = true });
            }
            else if (oldState.HasFlag(GameObjectCharacterState.IsStealthed)
                     && !newState.HasFlag(GameObjectCharacterState.IsStealthed))
            {
                FireOnStealth(new OnStealthEventArgs { Sender = hero, IsStealthed = false });
            }
        }

        /// <summary>
        ///     Attempts to fire the <see cref="OnStealth" /> event.
        /// </summary>
        /// <param name="args">
        ///     OnStealthEventArgs <see cref="OnStealthEventArgs" />
        /// </param>
        private static void FireOnStealth(OnStealthEventArgs args)
        {
            OnStealth?.Invoke(MethodBase.GetCurrentMethod().DeclaringType, args);
        }

        #endregion

        /// <summary>
        ///     On Stealth Event Data, contains useful information that is passed with OnStealth
        ///     <seealso cref="OnStealth" />
        /// </summary>
        public class OnStealthEventArgs : EventArgs
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets a value indicating whether is in stealth.
            /// </summary>
            public bool IsStealthed { get; set; }

            /// <summary>
            ///     Gets or sets the sender.
            /// </summary>
            public AIHeroClient Sender { get; set; }

            /// <summary>
            ///     Gets or sets the time.
            /// </summary>
            public float Time { get; set; }

            #endregion
        }
    }
}