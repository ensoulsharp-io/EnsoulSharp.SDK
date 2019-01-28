// <copyright file="Unit.cs" company="EnsoulSharp">
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
    using System.Linq;

    using SharpDX;

    /// <summary>
    ///     Provides helpful extensions to Units.
    /// </summary>
    public static partial class Extensions
    {
        #region Static Fields

        /// <summary>
        ///     Turrets Tier Four
        /// </summary>
        private static readonly string[] TurretsTierFour =
            {
                "SRUAP_Turret_Order4", "SRUAP_Turret_Chaos4",
                "TT_OrderTurret3", "TT_ChaosTurret3"
            };

        /// <summary>
        ///     Turrets Tier One
        /// </summary>
        private static readonly string[] TurretsTierOne =
            {
                "SRUAP_Turret_Order1", "SRUAP_Turret_Chaos1",
                "TT_OrderTurret5", "TT_ChaosTurret5",
                "HA_AP_OrderTurret", "HA_AP_ChaosTurret"
            };

        /// <summary>
        ///     Turrets Tier Three
        /// </summary>
        private static readonly string[] TurretsTierThree =
            {
                "SRUAP_Turret_Order3", "SRUAP_Turret_Chaos3",
                "TT_OrderTurret1", "TT_ChaosTurret1",
                "HA_AP_OrderTurret3", "HA_AP_ChaosTurret3"
            };

        /// <summary>
        ///     Turrets Tier Two
        /// </summary>
        private static readonly string[] TurretsTierTwo =
            {
                "SRUAP_Turret_Order2", "SRUAP_Turret_Chaos2",
                "TT_OrderTurret2", "TT_ChaosTurret2",
                "HA_AP_OrderTurret2", "HA_AP_ChaosTurret2"
            };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Counts the ally heroes in range.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="range">The range.</param>
        /// <returns>
        ///     The count.
        /// </returns>
        public static int CountAllyHeroesInRange(this AIBaseClient unit, float range)
        {
            return unit.Position.CountAllyHeroesInRange(range, unit);
        }

        /// <summary>
        ///     Counts the enemy heroes in range.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="range">The range.</param>
        /// <returns>
        ///     The count.
        /// </returns>
        public static int CountEnemyHeroesInRange(this AIBaseClient unit, float range)
        {
            return unit.Position.CountEnemyHeroesInRange(range, unit);
        }

        /// <summary>
        ///     Gets the distance between two GameObjects
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="target">The Target</param>
        /// <returns>The distance between the two objects</returns>
        public static float Distance(this GameObject source, GameObject target)
        {
            return source.Distance(target.Position);
        }

        /// <summary>
        ///     Gets the distance between a <see cref="GameObject" /> and a <see cref="Vector3" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance between a <see cref="GameObject" /> and a <see cref="Vector3" /></returns>
        public static float Distance(this GameObject source, Vector3 position)
        {
            return source.Distance(position.ToVector2());
        }

        /// <summary>
        ///     Gets the distance between a <see cref="GameObject" /> and a <see cref="Vector2" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance between a <see cref="GameObject" /> and a <see cref="Vector2" /></returns>
        public static float Distance(this GameObject source, Vector2 position)
        {
            return source.Position.Distance(position);
        }

        /// <summary>
        ///     Gets the distance between two <c>AIBaseClient</c>s using ServerPosition
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="target">The Target</param>
        /// <returns>The Distance</returns>
        public static float Distance(this AIBaseClient source, AIBaseClient target)
        {
            return source.Distance(target.Position);
        }

        /// <summary>
        ///     Gets the distance between a <see cref="AIBaseClient" /> and a <see cref="Vector3" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance between a <see cref="AIBaseClient" /> and a <see cref="Vector3" /></returns>
        public static float Distance(this AIBaseClient source, Vector3 position)
        {
            return source.Distance(position.ToVector2());
        }

        /// <summary>
        ///     Gets the distance between a <see cref="AIBaseClient" /> and a <see cref="Vector2" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance between a <see cref="AIBaseClient" /> and a <see cref="Vector2" /></returns>
        public static float Distance(this AIBaseClient source, Vector2 position)
        {
            return source.Position.Distance(position);
        }

        /// <summary>
        ///     Gets the distance squared between two GameObjects
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="target">The Target</param>
        /// <returns>The squared distance between the two objects</returns>
        public static float DistanceSquared(this GameObject source, GameObject target)
        {
            return source.DistanceSquared(target.Position);
        }

        /// <summary>
        ///     Gets the distance squared between a <see cref="GameObject" /> and a <see cref="Vector3" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance squared between a <see cref="GameObject" /> and a <see cref="Vector3" /></returns>
        public static float DistanceSquared(this GameObject source, Vector3 position)
        {
            return source.DistanceSquared(position.ToVector2());
        }

        /// <summary>
        ///     Gets the distance squared between a <see cref="GameObject" /> and a <see cref="Vector2" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance squared between a <see cref="GameObject" /> and a <see cref="Vector2" /></returns>
        public static float DistanceSquared(this GameObject source, Vector2 position)
        {
            return source.Position.DistanceSquared(position);
        }

        /// <summary>
        ///     Gets the distance squared between two AIBaseClient
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="target">The Target</param>
        /// <returns>The squared distance between the two AIBaseClient</returns>
        public static float DistanceSquared(this AIBaseClient source, AIBaseClient target)
        {
            return source.DistanceSquared(target.Position);
        }

        /// <summary>
        ///     Gets the distance squared between a <see cref="AIBaseClient" /> and a <see cref="Vector3" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance squared between a <see cref="AIBaseClient" /> and a <see cref="Vector3" /></returns>
        public static float DistanceSquared(this AIBaseClient source, Vector3 position)
        {
            return source.DistanceSquared(position.ToVector2());
        }

        /// <summary>
        ///     Gets the distance squared between a <see cref="AIBaseClient" /> and a <see cref="Vector2" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance squared between a <see cref="AIBaseClient" /> and a <see cref="Vector2" /></returns>
        public static float DistanceSquared(this AIBaseClient source, Vector2 position)
        {
            return source.Position.DistanceSquared(position);
        }

        /// <summary>
        ///     Gets the distance between <c>AIBaseClient</c> source and Player
        /// </summary>
        /// <param name="source">The Source</param>
        /// <returns>The distance between a <see cref="AIBaseClient" /> and the Player</returns>
        public static float DistanceToPlayer(this AIBaseClient source)
        {
            return GameObjects.Player.Distance(source);
        }

        /// <summary>
        ///     Gets the distance between the point and the Player
        /// </summary>
        /// <param name="position">The Position</param>
        /// <returns>The distance between the position and the Player</returns>
        public static float DistanceToPlayer(this Vector3 position)
        {
            return position.ToVector2().DistanceToPlayer();
        }

        /// <summary>
        ///     Gets the distance between the point and the Player
        /// </summary>
        /// <param name="position">The Position</param>
        /// <returns>The distance between the position and the Player</returns>
        public static float DistanceToPlayer(this Vector2 position)
        {
            return GameObjects.Player.Distance(position);
        }

        /// <summary>
        ///     Returns the recall time duration for a specified <see cref="AIHeroClient" />
        /// </summary>
        /// <param name="hero">The Hero</param>
        /// <returns>Recall Time Duration</returns>
        public static int GetRecallTime(this AIHeroClient hero)
        {
            return GetRecallTime(hero.Spellbook.GetSpell(SpellSlot.Recall).Name);
        }

        /// <summary>
        ///     Returns the recall time duration for a specific recall type by name.
        /// </summary>
        /// <param name="recallName">Recall type name</param>
        /// <returns>Recall Time Duration</returns>
        public static int GetRecallTime(string recallName)
        {
            switch (recallName.ToLower())
            {
                case "recall":
                    return 8000;
                case "recallimproved":
                    return 7000;
                case "odinrecall":
                    return 4500;
                case "odinrecallimproved":
                case "superrecall":
                case "superrecallimproved":
                    return 4000;
                default:
                    return 0;
            }
        }

        /// <summary>
        ///     Returns the spell slot with the name.
        /// </summary>
        /// <param name="unit">
        ///     The Unit
        /// </param>
        /// <param name="name">
        ///     Spell Name
        /// </param>
        /// <returns>
        ///     The <see cref="SpellSlot" />.
        /// </returns>
        public static SpellSlot GetSpellSlot(this AIHeroClient unit, string name)
        {
            foreach (var spell in
                unit.Spellbook.Spells.Where(
                    spell => string.Equals(spell.Name, name, StringComparison.CurrentCultureIgnoreCase)))
            {
                return spell.Slot;
            }

            return SpellSlot.Unknown;
        }

        /// <summary>
        ///     Gets the turret tier type.
        /// </summary>
        /// <param name="turret">
        ///     The turret.
        /// </param>
        /// <returns>
        ///     The <see cref="TurretType" />.
        /// </returns>
        public static TurretType GetTurretType(this AITurretClient turret)
        {
            var name = turret.CharacterName;
            if (TurretsTierOne.Contains(name))
            {
                return TurretType.TierOne;
            }
            if (TurretsTierTwo.Contains(name))
            {
                return TurretType.TierOne;
            }
            if (TurretsTierThree.Contains(name))
            {
                return TurretType.TierOne;
            }
            if (TurretsTierFour.Contains(name))
            {
                return TurretType.TierOne;
            }
            return TurretType.Unknown;
        }

        /// <summary>
        ///     Returns whether the hero is in fountain range.
        /// </summary>
        /// <param name="hero">The Hero</param>
        /// <returns>Is Hero in fountain range</returns>
        public static bool InFountain(this AIHeroClient hero)
        {
            float fountainRange = 562500; // 750 * 750
            if (Game.MapId == GameMapId.SummonersRift)
            {
                fountainRange = 1102500; // 1050 * 1050
            }

            return hero.IsVisible
                   && GameObjects.AllySpawnPoints.Any(sp => hero.DistanceSquared(sp.Position) < fountainRange);
        }

        /// <summary>
        ///     Returns whether the hero is in shop range.
        /// </summary>
        /// <param name="hero">The Hero</param>
        /// <returns>Is Hero in shop range</returns>
        public static bool InShop(this AIHeroClient hero)
        {
            return hero.IsVisible && GameObjects.AllyShops.Any(s => hero.DistanceSquared(s.Position) < 1562500);
        }

        /// <summary>
        ///     Calculates if the source and the target are facing each-other.
        /// </summary>
        /// <param name="source">Extended source</param>
        /// <param name="target">The Target</param>
        /// <returns>Returns if the source and target are facing each-other (boolean)</returns>
        public static bool IsBothFacing(this AIBaseClient source, AIBaseClient target)
        {
            return source.IsFacing(target) && target.IsFacing(source);
        }

        /// <summary>
        ///     Calculates if the source is facing the target.
        /// </summary>
        /// <param name="source">Extended source</param>
        /// <param name="target">The Target</param>
        /// <returns>Returns if the source is facing the target (boolean)</returns>
        public static bool IsFacing(this AIBaseClient source, AIBaseClient target)
        {
            return (source.IsValid() && target.IsValid())
                   && source.Direction.AngleBetween(target.Position - source.Position) < 90;
        }

        /// <summary>
        ///     Return whether the specific unit is melee
        /// </summary>
        /// <param name="unit">Extended unit</param>
        /// <returns>Returns if the unit is melee</returns>
        public static bool IsMelee(this AIBaseClient unit)
        {
            return unit.IsMelee && unit.AttackRange < 500;
        }

        /// <summary>
        ///     Returns if the unit is recalling.
        /// </summary>
        /// <param name="unit">Extended unit</param>
        /// <returns>Returns if the unit is recalling (boolean)</returns>
        public static bool IsRecalling(this AIHeroClient unit)
        {
            return unit.Buffs.Any(buff => buff.Name.ToLower().Contains("recall") && buff.Type == BuffType.Aura);
        }

        /// <summary>
        ///     Returns whether the specific unit is under a ally turret.
        /// </summary>
        /// <param name="unit"><see cref="AIBaseClient" /> unit</param>
        /// <returns>Is Unit under an Enemy Turret</returns>
        public static bool IsUnderAllyTurret(this AIBaseClient unit)
        {
            return unit.Position.IsUnderAllyTurret();
        }

        /// <summary>
        ///     Returns whether the specific unit is under a enemy turret.
        /// </summary>
        /// <param name="unit"><see cref="AIBaseClient" /> unit</param>
        /// <returns>Is Unit under an Enemy Turret</returns>
        public static bool IsUnderEnemyTurret(this AIBaseClient unit)
        {
            return unit.Position.IsUnderEnemyTurret();
        }

        /// <summary>
        ///     Checks if the Unit is valid.
        /// </summary>
        /// <param name="unit">
        ///     Unit from <c>AIBaseClient</c> type
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsValid(this AIBaseClient unit)
        {
            return unit != null && unit.IsValid;
        }

        /// <summary>
        ///     Checks if the target unit is valid.
        /// </summary>
        /// <param name="unit">
        ///     The Unit
        /// </param>
        /// <param name="range">
        ///     The Range
        /// </param>
        /// <param name="checkTeam">
        ///     Checks if the target is an enemy from the Player's side
        /// </param>
        /// <param name="from">
        ///     Check From
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsValidTarget(
            this AttackableUnit unit,
            float range = float.MaxValue,
            bool checkTeam = true,
            Vector3 from = default(Vector3))
        {
            if (unit == null || !unit.IsValid || unit.IsDead || !unit.IsVisible || !unit.IsTargetable
                || unit.IsInvulnerable)
            {
                return false;
            }

            if (!unit.IsTargetableToTeam(GameObjects.Player.Team))
            {
                return false;
            }

            if (checkTeam && GameObjects.Player.Team == unit.Team)
            {
                return false;
            }

            var @base = unit as AIBaseClient;

            if (@base != null && !@base.IsHPBarRendered)
            {
                return false;
            }

            var unitPosition = @base?.Position ?? unit.Position;

            return @from.IsValid()
                       ? @from.DistanceSquared(unitPosition) < range * range
                       : GameObjects.Player.DistanceSquared(unitPosition) < range * range;
        }

        #endregion
    }
}