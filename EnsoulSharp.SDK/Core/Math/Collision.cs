// <copyright file="Collision.cs" company="EnsoulSharp">
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
    using System.Text.RegularExpressions;

    using SharpDX;

    /// <summary>
    ///     Collision class, calculates collision for moving objects.
    /// </summary>
    public static class Collision
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Returns the list of the units that the skill-shot will hit before reaching the set positions.
        /// </summary>
        /// <param name="positions">
        ///     The positions.
        /// </param>
        /// <param name="input">
        ///     The input.
        /// </param>
        /// <returns>
        ///     A list of <c>AIBaseClient</c>s which the input collides with.
        /// </returns>
        public static List<AIBaseClient> GetCollision(List<Vector3> positions, PredictionInput input)
        {
            var result = new List<AIBaseClient>();

            foreach (var position in positions)
            {
                if (input.CollisionObjects.HasFlag(CollisionableObjects.Minions))
                {
                    foreach (var minion in
                        GameObjects.EnemyMinions.Where(
                            minion =>
                            minion.IsValidTarget(
                                Math.Min(input.Range + input.Radius + 100, 2000),
                                true,
                                input.RangeCheckFrom)))
                    {
                        input.Unit = minion;
                        var minionPrediction = Movement.GetPrediction(input, false, false);
                        if (minionPrediction.UnitPosition.ToVector2()
                                .DistanceSquared(input.From.ToVector2(), position.ToVector2(), true)
                            <= Math.Pow(input.Radius + 15 + minion.BoundingRadius, 2))
                        {
                            result.Add(minion);
                        }
                    }
                }

                if (input.CollisionObjects.HasFlag(CollisionableObjects.Heroes))
                {
                    foreach (var hero in
                        GameObjects.EnemyHeroes.Where(
                            hero =>
                            hero.IsValidTarget(
                                Math.Min(input.Range + input.Radius + 100, 2000),
                                true,
                                input.RangeCheckFrom)))
                    {
                        input.Unit = hero;
                        var prediction = Movement.GetPrediction(input, false, false);
                        if (prediction.UnitPosition.ToVector2()
                                .DistanceSquared(input.From.ToVector2(), position.ToVector2(), true)
                            <= Math.Pow(input.Radius + 50 + hero.BoundingRadius, 2))
                        {
                            result.Add(hero);
                        }
                    }
                }

                if (input.CollisionObjects.HasFlag(CollisionableObjects.Walls))
                {
                    var step = position.Distance(input.From) / 20;
                    for (var i = 0; i < 20; i++)
                    {
                        var p = input.From.ToVector2().Extend(position.ToVector2(), step * i);
                        if (NavMesh.GetCollisionFlags(p.X, p.Y).HasFlag(CollisionFlags.Wall))
                        {
                            result.Add(GameObjects.Player);
                        }
                    }
                }

                if (input.CollisionObjects.HasFlag(CollisionableObjects.YasuoWall))
                {
                    if (!GameObjects.EnemyHeroes
                        .Any(
                            hero => hero.IsValidTarget(float.MaxValue, false) && hero.CharacterName == "Yasuo"))
                    {
                        break;
                    }

                    foreach (var effectEmitter in GameObjects.ParticleEmitters)
                    {
                        if (effectEmitter.IsValid &&
                            Regex.IsMatch(effectEmitter.Name, @"Yasuo_.+_w_windwall_enemy_\d", RegexOptions.IgnoreCase))
                        {
                            var wall = effectEmitter;
                            var level = wall.Name.Substring(wall.Name.Length - 2, 2);
                            var wallWidth = 250 + 50 * Convert.ToInt32(level);
                            var wallDirection = wall.Perpendicular.ToVector2();
                            var wallStart = wall.Position.ToVector2() + wallWidth / 2 * wallDirection;
                            var wallEnd = wallStart - wallWidth * wallDirection;

                            if (wallStart.Intersection(wallEnd, position.ToVector2(), input.From.ToVector2()).Intersects)
                            {
                                var t = Variables.TickCount
                                        + (((wallStart.Intersection(wallEnd, position.ToVector2(), input.From.ToVector2())
                                                 .Point.Distance(input.From) / input.Speed) + input.Delay) * 1000);
                                if (t < wall.RestartTime + 4000)
                                {
                                    result.Add(GameObjects.Player);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return result.Distinct().ToList();
        }

        #endregion
    }
}