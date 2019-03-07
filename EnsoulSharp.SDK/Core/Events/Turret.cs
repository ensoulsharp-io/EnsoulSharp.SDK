// <copyright file="Turret.cs" company="EnsoulSharp">
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

    /// <summary>
    ///     Turret tracker and event handler.
    /// </summary>
    public static partial class Events
    {
        #region Static Fields

        /// <summary>
        ///     The Turrets list.
        /// </summary>
        private static readonly IDictionary<uint, TurretArgs> Turrets = new Dictionary<uint, TurretArgs>();

        #endregion

        #region Public Events

        /// <summary>
        ///     On turret attack event.
        /// </summary>
        public static event EventHandler<TurretArgs> OnTurretAttack;

        #endregion

        #region Methods

        /// <summary>
        ///     On Create event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        private static void EventTurret(GameObject sender)
        {
            if (sender is EffectEmitter && sender.Name.Contains("Turret"))
            {
                var turret =
                    Turrets.Values.Where(t => t.Turret.IsValid())
                        .OrderBy(t => t.Turret.Distance(sender))
                        .FirstOrDefault();
                if (turret != null)
                {
                    turret.TurretBoltObject = sender;
                }
            }
        }

        /// <summary>
        ///     On do cast event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        private static void EventTurret(AIBaseClient sender)
        {
            var turret = sender as AITurretClient;
            if (turret == null)
            {
                return;
            }
            var turNetworkId = turret.NetworkId;
            if (!Turrets.ContainsKey(turNetworkId))
            {
                Turrets.Add(turNetworkId, new TurretArgs { Turret = turret });
            }

            Turrets[turNetworkId].AttackStart = Variables.TickCount;
            if (Turrets[turNetworkId].Target != null && Turrets[turNetworkId].Target.IsValid)
            {
                Turrets[turNetworkId].AttackDelay = (turret.AttackCastDelay * 1000)
                                                    + (turret.Distance(Turrets[turNetworkId].Target)
                                                       / turret.BasicAttack.MissileSpeed * 1000);
                Turrets[turNetworkId].AttackEnd = (int)(Variables.TickCount + Turrets[turNetworkId].AttackDelay);
            }
            OnTurretAttack?.Invoke(MethodBase.GetCurrentMethod().DeclaringType, Turrets[turNetworkId]);
        }

        private static void EventTurretConstruct()
        {
            OnLoad += (sender, args) =>
                {
                    foreach (var turret in GameObjects.Turrets)
                    {
                        Turrets.Add(turret.NetworkId, new TurretArgs { Turret = turret });
                    }
                };
        }

        #endregion
    }

    /// <summary>
    ///     Turret event data which are passed with <see cref="Events.OnTurretAttack" />
    /// </summary>
    public class TurretArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the attack delay.
        /// </summary>
        public float AttackDelay { get; set; }

        /// <summary>
        ///     Gets or sets the attack end.
        /// </summary>
        public int AttackEnd { get; set; }

        /// <summary>
        ///     Gets or sets the attack start.
        /// </summary>
        public int AttackStart { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the turret is winding up.
        /// </summary>
        public bool IsWindingUp => this.Turret.IsWindingUp;

        /// <summary>
        ///     Gets the target.
        /// </summary>
        public AttackableUnit Target => this.Turret?.Target;

        /// <summary>
        ///     Gets or sets the turret.
        /// </summary>
        public AITurretClient Turret { get; set; }

        /// <summary>
        ///     Gets or sets the turret bolt object.
        /// </summary>
        public GameObject TurretBoltObject { get; set; }

        #endregion
    }
}