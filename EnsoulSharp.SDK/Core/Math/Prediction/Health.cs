// <copyright file="Health.cs" company="EnsoulSharp">
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

    using EnsoulSharp.SDK.Core.Utils;
    using EnsoulSharp.SDK.Core.Wrappers.Damages;

    /// <summary>
    ///     Health Prediction class for prediction of health of units.
    /// </summary>
    public class Health
    {
        #region Static Fields

        /// <summary>
        ///     List of Active Attacks.
        /// </summary>
        private static readonly Dictionary<uint, PredictedDamage> ActiveAttacks = new Dictionary<uint, PredictedDamage>();

        /// <summary>
        ///     Last Tick Update
        /// </summary>
        private static int lastTick;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Health" /> class.
        /// </summary>
        static Health()
        {
            Game.OnUpdate += OnGameUpdate;
            AIBaseClient.OnDoCast += OnAIBaseClientDoCast;
            Spellbook.OnStopCast += OnSpellbookStopCast;
            GameObject.OnDelete += OnGameObjectDelete;
            AIBaseClient.OnProcessSpellCast += OnAIBaseClientProcessSpellCast;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Return the Attacking turret.
        /// </summary>
        /// <param name="minion">
        ///     The minion.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />
        /// </returns>
        public static AIBaseClient GetAggroTurret(AIMinionClient minion)
        {
            var activeTurret =
                ActiveAttacks.Values.FirstOrDefault(m => m.Source is AITurretClient && m.Target.Compare(minion));
            return activeTurret?.Source;
        }

        /// <summary>
        ///     Returns the health prediction, either default or simulated
        /// </summary>
        /// <param name="unit"><see cref="AIBaseClient" /> unit</param>
        /// <param name="time">The time in milliseconds</param>
        /// <param name="delay">An optional delay</param>
        /// <param name="type"><see cref="HealthPredictionType" /> type</param>
        /// <returns>
        ///     The <see cref="float" />
        /// </returns>
        public static float GetPrediction(
            AIBaseClient unit,
            int time,
            int delay = 70,
            HealthPredictionType type = HealthPredictionType.Default)
        {
            return type == HealthPredictionType.Simulated
                       ? GetPredictionSimulated(unit, time)
                       : GetPredictionDefault(unit, time, delay);
        }

        /// <summary>
        ///     Determines whether the specified minion has minion aggro.
        /// </summary>
        /// <param name="minion">The minion.</param>
        /// <returns>
        ///     The <see cref="bool" />
        /// </returns>
        public static bool HasMinionAggro(AIMinionClient minion)
        {
            return ActiveAttacks.Values.Any(m => m.Source is AIMinionClient && m.Target.Compare(minion));
        }

        /// <summary>
        ///     Determines whether the specified minion has turret aggro.
        /// </summary>
        /// <param name="minion">The minion</param>
        /// <returns>
        ///     The <see cref="bool" />
        /// </returns>
        public static bool HasTurretAggro(AIMinionClient minion)
        {
            return ActiveAttacks.Values.Any(m => m.Source is AITurretClient && m.Target.Compare(minion));
        }

        /// <summary>
        ///     Return the starttick of the attacking turret.
        /// </summary>
        /// <param name="minion">
        ///     The minion.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />
        /// </returns>
        public static int TurretAggroStartTick(AIMinionClient minion)
        {
            var activeTurret =
                ActiveAttacks.Values.FirstOrDefault(m => m.Source is AITurretClient && m.Target.Compare(minion));
            return activeTurret?.StartTick ?? 0;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Calculates the default prediction of the unit.
        /// </summary>
        /// <param name="unit">
        ///     The unit
        /// </param>
        /// <param name="time">
        ///     The time
        /// </param>
        /// <param name="delay">
        ///     The delay
        /// </param>
        /// <returns>
        ///     The <see cref="float" />
        /// </returns>
        private static float GetPredictionDefault(AIBaseClient unit, int time, int delay = 70)
        {
            var predictedDamage = 0f;
            foreach (var attack in ActiveAttacks.Values.Where(i => i.Target.Compare(unit) && !i.Processed))
            {
                var attackDamage = 0f;
                if (attack.Source.IsValidTarget(float.MaxValue, false) && attack.Target.IsValidTarget())
                {
                    var landTime = attack.StartTick + attack.Delay
                                   + 1000
                                   * (attack.Source.IsMelee
                                          ? 0
                                          : Math.Max(0, unit.Distance(attack.Source) - attack.Source.BoundingRadius)
                                            / attack.ProjectileSpeed) + delay;
                    if (landTime < Variables.TickCount + time)
                    {
                        attackDamage = attack.Damage;
                    }
                }

                predictedDamage += attackDamage;
            }

            return unit.Health - predictedDamage;
        }

        /// <summary>
        ///     Calculates the simulated prediction of the unit.
        /// </summary>
        /// <param name="unit">
        ///     The unit
        /// </param>
        /// <param name="time">
        ///     The time
        /// </param>
        /// <returns>
        ///     The <see cref="float" />
        /// </returns>
        private static float GetPredictionSimulated(AIBaseClient unit, int time)
        {
            var predictedDamage = 0f;
            foreach (var attack in ActiveAttacks.Values.Where(i => i.Target.Compare(unit)))
            {
                var n = 0;
                if (Variables.TickCount - 100 <= attack.StartTick + attack.AnimationTime
                    && attack.Source.IsValidTarget(float.MaxValue, false) && attack.Target.IsValidTarget())
                {
                    var fromT = attack.StartTick;
                    var toT = Variables.TickCount + time;
                    while (fromT < toT)
                    {
                        if (fromT >= Variables.TickCount
                            && fromT + attack.Delay / 1000
                            + (attack.Source.IsMelee
                                   ? 0
                                   : Math.Max(0, unit.Distance(attack.Source) - attack.Source.BoundingRadius)
                                     / attack.ProjectileSpeed) < toT)
                        {
                            n++;
                        }

                        fromT += (int)attack.AnimationTime;
                    }
                }

                predictedDamage += n * attack.Damage;
            }

            return unit.Health - predictedDamage;
        }

        /// <summary>
        ///     GameObject on delete subscribed event function.
        /// </summary>
        /// <param name="sender">
        ///     <see cref="GameObject" /> sender
        /// </param>
        /// <param name="args">
        ///     <see cref="System.EventArgs" /> event data
        /// </param>
        private static void OnGameObjectDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid)
            {
                return;
            }

            var aiBase = sender as AIBaseClient;
            if (aiBase != null)
            {
                var objNetworkId = aiBase.NetworkId;
                if (ActiveAttacks.ContainsKey(objNetworkId))
                {
                    ActiveAttacks.Remove(objNetworkId);
                    return;
                }
                ActiveAttacks.ToList()
                    .Where(pair => pair.Value.Target.Compare(aiBase))
                    .ToList()
                    .ForEach(pair => ActiveAttacks.Remove(pair.Key));
                return;
            }

            var missile = sender as MissileClient;
            if (missile?.SpellCaster != null)
            {
                var casterNetworkId = missile.SpellCaster.NetworkId;
                if (ActiveAttacks.ContainsKey(casterNetworkId))
                {
                    ActiveAttacks[casterNetworkId].Processed = true;
                }
            }
        }

        /// <summary>
        ///     Game Tick which is called by the game update event.
        /// </summary>
        /// <param name="args">
        ///     <see cref="System.EventArgs" /> event data
        /// </param>
        private static void OnGameUpdate(EventArgs args)
        {
            if (Variables.TickCount - lastTick <= 1000)
            {
                return;
            }

            ActiveAttacks.ToList()
                .Where(pair => pair.Value.StartTick < Variables.TickCount - 3000)
                .ToList()
                .ForEach(pair => ActiveAttacks.Remove(pair.Key));

            lastTick = Variables.TickCount;
        }

        /// <summary>
        ///     AIBaseClient on ProcessSpellCast subscribed event function.
        /// </summary>
        /// <param name="sender">
        ///     <see cref="AIBaseClient" /> sender
        /// </param>
        /// <param name="args">
        ///     <see cref="GameObjectProcessSpellCastEventArgs" /> event data
        /// </param>
        private static void OnAIBaseClientProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsValid && sender.IsMelee)
            {
                var casterNetworkId = sender.NetworkId;
                if (ActiveAttacks.ContainsKey(casterNetworkId))
                {
                    ActiveAttacks[casterNetworkId].Processed = true;
                }
            }
        }

        /// <summary>
        ///     AIBaseClient on DoCast subscribed event function.
        /// </summary>
        /// <param name="sender"><see cref="AIBaseClient" /> sender</param>
        /// <param name="args">Processed Spell Cast Data</param>
        private static void OnAIBaseClientDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (!sender.IsValidTarget(2000, false) || !AutoAttack.IsAutoAttack(args.SData.Name) || !sender.IsAlly)
            {
                return;
            }

            if (!(sender is AIMinionClient) && !(sender is AITurretClient))
            {
                return;
            }

            var target = args.Target as AIMinionClient;

            if (target == null)
            {
                return;
            }

            ActiveAttacks.Remove(sender.NetworkId);
            ActiveAttacks.Add(
                sender.NetworkId,
                new PredictedDamage(
                    sender,
                    target,
                    Variables.TickCount - (Game.Ping / 2),
                    sender.AttackCastDelay * 1000,
                    (sender.AttackDelay * 1000) - (sender is AITurretClient ? 70 : 0),
                    sender.IsMelee ? int.MaxValue : (int)args.SData.MissileSpeed,
                    (float)sender.GetAutoAttackDamage(target)));
        }

        /// <summary>
        ///     Spell-book on casting stop subscribed event function.
        /// </summary>
        /// <param name="sender">
        ///     <see cref="Spellbook" /> sender
        /// </param>
        /// <param name="args">Spell-book Stop Cast Data</param>
        private static void OnSpellbookStopCast(AIBaseClient sender, SpellbookStopCastEventArgs args)
        {
            if (sender.IsValid && args.KeepAnimationPlaying && args.DestroyMissile)
            {
                var casterNetworkId = sender.NetworkId;
                if (ActiveAttacks.ContainsKey(casterNetworkId))
                {
                    ActiveAttacks.Remove(casterNetworkId);
                }
            }
        }

        #endregion

        /// <summary>
        ///     Predicted Damage Container
        /// </summary>
        private class PredictedDamage
        {
            #region Fields

            /// <summary>
            ///     Animation Time
            /// </summary>
            public readonly float AnimationTime;

            /// <summary>
            ///     The Damage
            /// </summary>
            public readonly float Damage;

            /// <summary>
            ///     Delay before damage impact
            /// </summary>
            public readonly float Delay;

            /// <summary>
            ///     Projectile Speed
            /// </summary>
            public readonly int ProjectileSpeed;

            /// <summary>
            ///     The Source
            /// </summary>
            public readonly AIBaseClient Source;

            /// <summary>
            ///     Start Tick
            /// </summary>
            public readonly int StartTick;

            /// <summary>
            ///     The Target
            /// </summary>
            public readonly AIBaseClient Target;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="PredictedDamage" /> class.
            /// </summary>
            /// <param name="source">
            ///     Damage Source
            /// </param>
            /// <param name="target">
            ///     Damage Target
            /// </param>
            /// <param name="startTick">
            ///     Starting Game Tick
            /// </param>
            /// <param name="delay">
            ///     Delay of damage impact
            /// </param>
            /// <param name="animationTime">
            ///     Animation time
            /// </param>
            /// <param name="projectileSpeed">
            ///     Projectile Speed
            /// </param>
            /// <param name="damage">
            ///     The Damage
            /// </param>
            public PredictedDamage(
                AIBaseClient source,
                AIBaseClient target,
                int startTick,
                float delay,
                float animationTime,
                int projectileSpeed,
                float damage)
            {
                this.Source = source;
                this.Target = target;
                this.StartTick = startTick;
                this.Delay = delay;
                this.ProjectileSpeed = projectileSpeed;
                this.Damage = damage;
                this.AnimationTime = animationTime;
            }

            #endregion

            #region Public Properties

            public bool Processed { get; set; }

            #endregion
        }
    }
}