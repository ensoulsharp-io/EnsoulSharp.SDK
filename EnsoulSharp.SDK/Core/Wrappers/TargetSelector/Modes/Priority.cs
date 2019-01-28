// <copyright file="Priority.cs" company="EnsoulSharp">
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

namespace EnsoulSharp.SDK.Modes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EnsoulSharp.SDK.Core.UI.IMenu;
    using EnsoulSharp.SDK.Core.UI.IMenu.Values;
    using EnsoulSharp.SDK.Core.Utils;

    /// <summary>
    ///     The priority Mode.
    /// </summary>
    [ResourceImport]
    public class Priority : ITargetSelectorMode
    {
        #region Constants

        /// <summary>
        ///     The maximum priority
        /// </summary>
        private const int MaxPriority = 5;

        /// <summary>
        ///     The minimum priority
        /// </summary>
        private const int MinPriority = 1;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The priority categories
        /// </summary>
        public static IReadOnlyList<PriorityCategory> PriorityCategories => PriorityCategoriesList;

        [ResourceImport("Data.Priority.json")]
        private static List<PriorityCategory> PriorityCategoriesList = new List<PriorityCategory>();

        #endregion

        #region Fields

        /// <summary>
        ///     The menu.
        /// </summary>
        private Menu menu;

        #endregion

        #region Public Properties

        /// <inheritdoc />
        public string DisplayName => "Priorities";

        /// <inheritdoc />
        public string Name => "priorities";

        #endregion

        #region Public Methods and Operators

        /// <inheritdoc />
        public void AddToMenu(Menu tsMenu)
        {
            this.menu = tsMenu;

            var priorityMenu = new Menu("priority", "Priority");

            foreach (var enemy in GameObjects.EnemyHeroes)
            {
                priorityMenu.Add(
                    new MenuSlider(enemy.CharacterName, enemy.CharacterName, MinPriority, MinPriority, MaxPriority));
            }

            priorityMenu.Add(new MenuBool("autoPriority", "Auto Priority"));

            priorityMenu.MenuValueChanged += (sender, args) =>
                {
                    var boolean = sender as MenuBool;
                    if (boolean != null)
                    {
                        if (boolean.Name.Equals("autoPriority") && boolean.Value)
                        {
                            foreach (var enemy in GameObjects.EnemyHeroes)
                            {
                                this.SetPriority(enemy, this.GetDefaultPriority(enemy));
                            }
                        }
                    }
                };

            this.menu.Add(priorityMenu);

            if (priorityMenu["autoPriority"].GetValue<MenuBool>().Value)
            {
                foreach (var enemy in GameObjects.EnemyHeroes)
                {
                    this.SetPriority(enemy, this.GetDefaultPriority(enemy));
                }
            }
        }

        /// <summary>
        ///     Gets the default priority.
        /// </summary>
        /// <param name="hero">
        ///     The hero.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int GetDefaultPriority(AIHeroClient hero)
        {
            return PriorityCategories.FirstOrDefault(i => i.Champions.Contains(hero.CharacterName))?.Value ?? MinPriority;
        }

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        /// <param name="hero">
        ///     The hero.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int GetPriority(AIHeroClient hero)
        {
            return this.menu?["priority"][hero.CharacterName]?.GetValue<MenuSlider>().Value ?? MinPriority;
        }

        /// <inheritdoc />
        public List<AIHeroClient> OrderChampions(List<AIHeroClient> heroes)
        {
            return heroes.OrderByDescending(this.GetPriority).ToList();
        }

        /// <summary>
        ///     Sets the priority.
        /// </summary>
        /// <param name="hero">
        ///     The hero.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public void SetPriority(AIHeroClient hero, int value)
        {
            var item = this.menu?["priority"][hero.CharacterName];
            if (item != null)
            {
                item.GetValue<MenuSlider>().Value = Math.Max(MinPriority, Math.Min(MaxPriority, value));
            }
        }

        #endregion
    }
}