// <copyright file="DamageLibrary.cs" company="EnsoulSharp">
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

namespace EnsoulSharp.SDK.Core.Wrappers.Damages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Permissions;
    using System.Text;

    using Utils;
    using Properties;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///     Damage wrapper class, contains functions to calculate estimated damage to a unit and also provides damage details.
    /// </summary>
    public static partial class Damage
    {
        #region Static Fields

        /// <summary>
        ///     The damage version files.
        /// </summary>
        private static readonly IDictionary<string, byte[]> DamageFiles = new Dictionary<string, byte[]>
                                                                              { { "9.7", Resources._9_7_269_2391 } };

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the Damage Collection.
        /// </summary>
        internal static IDictionary<string, ChampionDamage> DamageCollection { get; } =
            new Dictionary<string, ChampionDamage>();

        #endregion

        #region Methods

        /// <summary>
        ///     Initializes a new instance of the <see cref="Damage" /> class.
        /// </summary>
        /// <param name="gameVersion">
        ///     The client version.
        /// </param>
        internal static void Initialize(Version gameVersion)
        {
            Events.OnLoad += (sender, args) =>
                {
                    OnLoad(gameVersion);
                    CreatePassives();
                };
        }

        /// <summary>
        ///     Creates the damage collection.
        /// </summary>
        /// <param name="damages">
        ///     The converted <see cref="byte" />s of damages into a dictionary collection.
        /// </param>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void CreateDamages(IDictionary<string, JToken> damages)
        {
            foreach (var champion in GameObjects.Heroes.Select(h => h.CharacterName).Distinct())
            {
                JToken value;
                if (damages.TryGetValue(champion, out value))
                {
                    DamageCollection.Add(champion, JsonConvert.DeserializeObject<ChampionDamage>(value.ToString()));
                }
            }
        }

        private static void OnLoad(Version version)
        {
            var versionString = $"{version.Major}.{version.Minor}";

            var fileBytes = DamageFiles.ContainsKey(versionString)
                                ? DamageFiles[versionString]
                                : DamageFiles.OrderByDescending(o => o.Key).FirstOrDefault().Value;
            if (fileBytes != null)
            {
                CreateDamages(JObject.Parse(Encoding.Default.GetString(fileBytes)));
                return;
            }

            Logging.Write()(LogLevel.Fatal, "No suitable damage library is available.");
        }

        /// <summary>
        ///     Resolve the monster damage.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="monsterDamage">
        ///     The damage on monster collection
        /// </param>
        /// <param name="index">
        ///     The index (spell level - 1)
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        private static double ResolveMonsterDamage(
            this AIBaseClient source,
            AIBaseClient target,
            ChampionDamageSpellOnMonster monsterDamage,
            int index)
        {
            var sourceScale = monsterDamage.ScalingTarget == DamageScalingTarget.Source ? source : target;
            var percent = monsterDamage.DamagePercentages?.Count > 0
                              ? monsterDamage.DamagePercentages[Math.Min(index, monsterDamage.DamagePercentages.Count - 1)]
                              : 0d;
            var origin = 0f;

            switch (monsterDamage.ScalingType)
            {
                case DamageScalingType.AttackPoints:
                    origin = sourceScale.TotalAttackDamage;
                    break;
                case DamageScalingType.BonusAttackPoints:
                    origin = sourceScale.FlatPhysicalDamageMod;
                    break;
                case DamageScalingType.AbilityPoints:
                    origin = sourceScale.TotalMagicalDamage;
                    break;
                case DamageScalingType.BonusHealth:
                    origin = sourceScale.BonusHealth;
                    break;
                case DamageScalingType.CurrentHealth:
                    origin = sourceScale.Health;
                    break;
                case DamageScalingType.MaxHealth:
                    origin = sourceScale.MaxHealth;
                    break;
                case DamageScalingType.MissingHealth:
                    origin = sourceScale.MaxHealth - sourceScale.Health;
                    break;
                case DamageScalingType.BonusMana:
                    origin = sourceScale.BonusMana;
                    break;
                case DamageScalingType.MaxMana:
                    origin = sourceScale.MaxMana;
                    break;
                case DamageScalingType.Armor:
                    origin = sourceScale.Armor;
                    break;
                case DamageScalingType.BonusArmor:
                    origin = sourceScale.BonusArmor;
                    break;
                case DamageScalingType.SpellBlock:
                    origin = sourceScale.SpellBlock;
                    break;
                case DamageScalingType.BonusSpellBlock:
                    origin = sourceScale.BonusSpellBlock;
                    break;
                case DamageScalingType.PhysicalLethality:
                    origin = sourceScale.PhysicalLethality;
                    break;
            }

            return origin * percent;
        }

        /// <summary>
        ///     Resolve the limit damage.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="damageLimit">
        ///     The damage limit collection
        /// </param>
        /// <param name="index">
        ///     The index (spell level - 1)
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        private static double ResolveLimitDamage(
            this AIBaseClient source,
            AIBaseClient target,
            ChampionDamageSpellDamageLimit damageLimit,
            int index)
        {
            var sourceScale = damageLimit.ScalingTarget == DamageScalingTarget.Source ? source : target;
            var percent = damageLimit.DamagePercentages?.Count > 0
                              ? damageLimit.DamagePercentages[Math.Min(index, damageLimit.DamagePercentages.Count - 1)]
                              : 0d;
            var origin = 0f;

            switch (damageLimit.ScalingType)
            {
                case DamageScalingType.AttackPoints:
                    origin = sourceScale.TotalAttackDamage;
                    break;
                case DamageScalingType.BonusAttackPoints:
                    origin = sourceScale.FlatPhysicalDamageMod;
                    break;
                case DamageScalingType.AbilityPoints:
                    origin = sourceScale.TotalMagicalDamage;
                    break;
                case DamageScalingType.BonusHealth:
                    origin = sourceScale.BonusHealth;
                    break;
                case DamageScalingType.CurrentHealth:
                    origin = sourceScale.Health;
                    break;
                case DamageScalingType.MaxHealth:
                    origin = sourceScale.MaxHealth;
                    break;
                case DamageScalingType.MissingHealth:
                    origin = sourceScale.MaxHealth - sourceScale.Health;
                    break;
                case DamageScalingType.BonusMana:
                    origin = sourceScale.BonusMana;
                    break;
                case DamageScalingType.MaxMana:
                    origin = sourceScale.MaxMana;
                    break;
                case DamageScalingType.Armor:
                    origin = sourceScale.Armor;
                    break;
                case DamageScalingType.BonusArmor:
                    origin = sourceScale.BonusArmor;
                    break;
                case DamageScalingType.SpellBlock:
                    origin = sourceScale.SpellBlock;
                    break;
                case DamageScalingType.BonusSpellBlock:
                    origin = sourceScale.BonusSpellBlock;
                    break;
                case DamageScalingType.PhysicalLethality:
                    origin = sourceScale.PhysicalLethality;
                    break;
            }

            return origin * percent + (damageLimit.Damages?.Count > 0
                                              ? damageLimit.Damages[Math.Min(index, damageLimit.Damages.Count - 1)]
                                              : 0d);
        }

        /// <summary>
        ///     Resolves the spell bonus damage.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="spellBonus">
        ///     The spell bonus collection
        /// </param>
        /// <param name="index">
        ///     The index (spell level - 1)
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        private static double ResolveBonusSpellDamage(
            this AIBaseClient source,
            AIBaseClient target,
            ChampionDamageSpellBonus spellBonus,
            int index)
        {
            var sourceScale = spellBonus.ScalingTarget == DamageScalingTarget.Source ? source : target;
            var percent = spellBonus.DamagePercentages?.Count > 0
                              ? spellBonus.DamagePercentages[Math.Min(index, spellBonus.DamagePercentages.Count - 1)]
                              : 0d;
            var origin = 0f;

            switch (spellBonus.ScalingType)
            {
                case DamageScalingType.AttackPoints:
                    origin = sourceScale.TotalAttackDamage;
                    break;
                case DamageScalingType.BonusAttackPoints:
                    origin = sourceScale.FlatPhysicalDamageMod;
                    break;
                case DamageScalingType.AbilityPoints:
                    origin = sourceScale.TotalMagicalDamage;
                    break;
                case DamageScalingType.BonusHealth:
                    origin = sourceScale.BonusHealth;
                    break;
                case DamageScalingType.CurrentHealth:
                    origin = sourceScale.Health;
                    break;
                case DamageScalingType.MaxHealth:
                    origin = sourceScale.MaxHealth;
                    break;
                case DamageScalingType.MissingHealth:
                    origin = sourceScale.MaxHealth - sourceScale.Health;
                    break;
                case DamageScalingType.BonusMana:
                    origin = sourceScale.BonusMana;
                    break;
                case DamageScalingType.MaxMana:
                    origin = sourceScale.MaxMana;
                    break;
                case DamageScalingType.Armor:
                    origin = sourceScale.Armor;
                    break;
                case DamageScalingType.BonusArmor:
                    origin = sourceScale.BonusArmor;
                    break;
                case DamageScalingType.SpellBlock:
                    origin = sourceScale.SpellBlock;
                    break;
                case DamageScalingType.BonusSpellBlock:
                    origin = sourceScale.BonusSpellBlock;
                    break;
                case DamageScalingType.PhysicalLethality:
                    origin = sourceScale.PhysicalLethality;
                    break;
            }

            var dmg = origin
                      * ((percent > 0 ? percent : 0)
                             + (spellBonus.ScalePer100Ap > 0
                                    ? Math.Abs(source.TotalMagicalDamage / 100) * spellBonus.ScalePer100Ap
                                    : 0)
                             + (spellBonus.ScalePer100Ad > 0
                                    ? Math.Abs(source.TotalAttackDamage / 100) * spellBonus.ScalePer100Ad
                                    : 0)
                             + (spellBonus.ScalePer100BonusAd > 0
                                    ? Math.Abs(source.FlatPhysicalDamageMod / 100) * spellBonus.ScalePer100BonusAd
                                    : 0)
                             + (!string.IsNullOrEmpty(spellBonus.PercentageStackBuff)
                                    ? (string.IsNullOrEmpty(spellBonus.PercentageCheckBuff) || source.HasBuff(spellBonus.PercentageCheckBuff)
                                        ? (spellBonus.PercentagePerBuffStack * source.GetBuffCount(spellBonus.PercentageStackBuff))
                                        : 0)
                                    : 0));

            if (target is AIMinionClient && spellBonus.BonusDamageOnMinion?.Count > 0)
            {
                dmg += spellBonus.BonusDamageOnMinion[Math.Min(index, spellBonus.BonusDamageOnMinion.Count - 1)];
            }

            if (target is AIMinionClient && target.Team == GameObjectTeam.Neutral && spellBonus.BonusDamageOnMonster?.Count > 0)
            {
                dmg += spellBonus.BonusDamageOnMonster[Math.Min(index, spellBonus.BonusDamageOnMonster.Count - 1)];
            }

            if (!string.IsNullOrEmpty(spellBonus.BonusBuff))
            {
                dmg += source.GetBuffCount(spellBonus.BonusBuff) + spellBonus.BonusBuffOffset;
            }

            if (!string.IsNullOrEmpty(spellBonus.ScalingBuff))
            {
                var buffCount =
                    (spellBonus.ScalingBuffTarget == DamageScalingTarget.Source ? source : target).GetBuffCount(
                        spellBonus.ScalingBuff);
                dmg = buffCount > 0 ? dmg * (buffCount + spellBonus.ScalingBuffOffset) : 0d;
            }

            if (dmg > 0)
            {
                if (spellBonus.MinDamage?.Damages?.Count > 0)
                {
                    dmg = Math.Max(dmg, source.ResolveLimitDamage(target, spellBonus.MinDamage, index));
                }

                if (target is AIMinionClient && spellBonus.MaxDamageOnMinion?.Count > 0)
                {
                    dmg = Math.Min(
                        dmg,
                        spellBonus.MaxDamageOnMinion[Math.Min(index, spellBonus.MaxDamageOnMinion.Count - 1)]);
                }

                if (target is AIMinionClient && target.Team == GameObjectTeam.Neutral && spellBonus.MaxDamageOnMonster?.Count > 0)
                {
                    dmg = Math.Min(
                        dmg,
                        spellBonus.MaxDamageOnMonster[Math.Min(index, spellBonus.MaxDamageOnMonster.Count - 1)]);
                }
            }

            return dmg;
        }

        #endregion
    }
}