// <copyright file="TargetSelector.cs" company="EnsoulSharp">
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
    
    using EnsoulSharp.SDK.Core.UI.IMenu;
    using EnsoulSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     The TargetSelector system.
    /// </summary>
    public sealed class TargetSelector
    {
        #region Fields

        /// <summary>
        ///     The menu.
        /// </summary>
        private readonly Menu menu = new Menu("targetselector", "TargetSelector");

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TargetSelector" /> class.
        /// </summary>
        /// <param name="menu">
        ///     The menu.
        /// </param>
        public TargetSelector(Menu menu)
        {
            Events.OnLoad += (sender, args) =>
                {
                    menu.Add(this.menu);

                    this.Selected = new TargetSelectorSelected(this.menu);
                    this.Humanizer = new TargetSelectorHumanizer(this.menu);
                    this.Mode = new TargetSelectorMode(this.menu);
                    this.Drawing = new TargetSelectorDrawing(this.menu, this.Selected, this.Mode);

                    // Keep submenus at top
                    this.menu.Components =
                        this.menu.Components.OrderByDescending(c => c.Value is Menu && c.Key.Equals("drawing"))
                            .ThenByDescending(c => c.Value is Menu)
                            .ToDictionary(p => p.Key, p => p.Value);
                };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the humanizer instance.
        /// </summary>
        public TargetSelectorHumanizer Humanizer { get; private set; }

        /// <summary>
        ///     Gets the mode instance.
        /// </summary>
        public TargetSelectorMode Mode { get; private set; }

        /// <summary>
        ///     Gets the selected instance.
        /// </summary>
        public TargetSelectorSelected Selected { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the drawing instance.
        /// </summary>
        internal TargetSelectorDrawing Drawing { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the Selected target.
        /// </summary>
        /// <returns>
        ///     The <see cref="AIHeroClient" />.
        /// </returns>
        public AIHeroClient GetSelectedTarget()
        {
            return this.Selected.Target;
        }

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <param name="spell">
        ///     The spell.
        /// </param>
        /// <param name="ignoreShields">
        ///     Indicates whether to ignore shields.
        /// </param>
        /// <param name="ignoreChampions">
        ///     Indicates whether to ignore champions.
        /// </param>
        /// <returns>
        ///     The <see cref="AIHeroClient" />.
        /// </returns>
        public AIHeroClient GetTarget(
            Spell spell,
            bool ignoreShields = true,
            IEnumerable<AIHeroClient> ignoreChampions = null)
        {
            return
                this.GetTarget(
                    spell.Range + spell.Width
                    + GameObjects.EnemyHeroes.Select(e => e.BoundingRadius).DefaultIfEmpty(50).Max(),
                    spell.DamageType,
                    ignoreShields,
                    spell.From,
                    ignoreChampions);
        }

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <param name="damageType">
        ///     Type of the damage.
        /// </param>
        /// <param name="ignoreShields">
        ///     Indicates whether to ignore shields.
        /// </param>
        /// <param name="from">
        ///     The from location.
        /// </param>
        /// <param name="ignoreChampions">
        ///     Indicates whether to ignore champions.
        /// </param>
        /// <returns>
        ///     The <see cref="AIHeroClient" />.
        /// </returns>
        public AIHeroClient GetTarget(
            float range,
            DamageType damageType = DamageType.True,
            bool ignoreShields = true,
            Vector3 from = default(Vector3),
            IEnumerable<AIHeroClient> ignoreChampions = null)
        {
            var targets = this.GetTargets(range, damageType, ignoreShields, from, ignoreChampions);
            return targets?.FirstOrDefault();
        }

        /// <summary>
        ///     Gets the target without collision checking.
        /// </summary>
        /// <param name="spell">
        ///     The spell.
        /// </param>
        /// <param name="ignoreShields">
        ///     Indicates whether to ignore shields.
        /// </param>
        /// <param name="ignoreChampions">
        ///     Indicates whether to ignore champions.
        /// </param>
        /// <returns>
        ///     The <see cref="AIHeroClient" />.
        /// </returns>
        public AIHeroClient GetTargetNoCollision(
            Spell spell,
            bool ignoreShields = true,
            IEnumerable<AIHeroClient> ignoreChampions = null)
        {
            return
                this.GetTargets(spell.Range, spell.DamageType, ignoreShields, spell.From, ignoreChampions)
                    .FirstOrDefault(t => spell.GetPrediction(t).Hitchance != HitChance.Collision);
        }

        /// <summary>
        ///     Gets the targets.
        /// </summary>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <param name="damageType">
        ///     Type of the damage.
        /// </param>
        /// <param name="ignoreShields">
        ///     Indicates whether to ignore shields.
        /// </param>
        /// <param name="from">
        ///     The from location.
        /// </param>
        /// <param name="ignoreChampions">
        ///     Indicates whether to ignore champions.
        /// </param>
        /// <returns>
        ///     The <see cref="AIHeroClient" />.
        /// </returns>
        public List<AIHeroClient> GetTargets(
            float range,
            DamageType damageType = DamageType.True,
            bool ignoreShields = true,
            Vector3 from = default(Vector3),
            IEnumerable<AIHeroClient> ignoreChampions = null)
        {
            if (this.Selected.Focus && this.Selected.Force)
            {
                if (IsValidTarget(this.Selected.Target, float.MaxValue, damageType, ignoreShields, from))
                {
                    return new List<AIHeroClient> { this.Selected.Target };
                }
            }

            var targets =
                this.Humanizer.FilterTargets(GameObjects.EnemyHeroes.ToList())
                    .Where(h => ignoreChampions == null || ignoreChampions.All(i => !i.Compare(h)))
                    .Where(h => IsValidTarget(h, range, damageType, ignoreShields, from))
                    .ToList();

            targets = this.Mode.OrderChampions(targets);

            if (this.Selected.Focus && this.Selected.Target != null)
            {
                targets = targets.OrderByDescending(h => h.Compare(this.Selected.Target)).ToList();
            }

            return targets;
        }

        /// <summary>
        ///     Sets the target.
        /// </summary>
        /// <param name="target">
        ///     The target to set.
        /// </param>
        public void SetTarget(AIHeroClient target)
        {
            this.Selected.Target = target;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Determines whether [is valid target] [the specified hero].
        /// </summary>
        /// <param name="hero">
        ///     The hero.
        /// </param>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <param name="damageType">
        ///     Type of the damage.
        /// </param>
        /// <param name="ignoreShields">
        ///     Indicates whether to ignore shields.
        /// </param>
        /// <param name="from">The from location.</param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private static bool IsValidTarget(
            AIHeroClient hero,
            float range,
            DamageType damageType,
            bool ignoreShields = true,
            Vector3 from = default(Vector3))
        {
            return hero.IsValidTarget()
                   && hero.DistanceSquared(@from.Equals(default(Vector3)) ? GameObjects.Player.Position : @from)
                   < Math.Pow(range <= 0 ? hero.GetRealAutoAttackRange() : range, 2)
                   && !Invulnerable.Check(hero, damageType, ignoreShields);
        }

        #endregion
    }
}