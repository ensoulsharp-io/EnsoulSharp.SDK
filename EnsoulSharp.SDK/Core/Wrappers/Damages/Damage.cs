// <copyright file="Damage.cs" company="EnsoulSharp">
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
    using System.Linq;

    using Utils;

    /// <summary>
    ///     Damage wrapper class, contains functions to calculate estimated damage to a unit and also provides damage details.
    /// </summary>
    public static partial class Damage
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the calculated damage based on the given damage type onto the target from source.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="damageType">
        ///     The damage type
        /// </param>
        /// <param name="amount">
        ///     The amount
        /// </param>
        /// <returns>
        ///     The estimated damage from calculations.
        /// </returns>
        public static double CalculateDamage(
            this AIBaseClient source,
            AIBaseClient target,
            DamageType damageType,
            double amount)
        {
            var damage = 0d;
            switch (damageType)
            {
                case DamageType.Magical:
                    damage = source.CalculateMagicDamage(target, amount);
                    break;
                case DamageType.Physical:
                    damage = source.CalculatePhysicalDamage(target, amount);
                    break;
                case DamageType.Mixed:
                    damage = source.CalculateMixedDamage(target, damage / 2, damage / 2);
                    break;
                case DamageType.True:
                    damage = Math.Floor(source.PassivePercentMod(target, Math.Max(amount, 0), DamageType.True));
                    break;
            }

            return damage;
        }

        /// <summary>
        ///     Gets the calculated mixed damage onto the target from source.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="physicalAmount">
        ///     The physical amount
        /// </param>
        /// <param name="magicalAmount">
        ///     The magical amount
        /// </param>
        /// <returns>
        ///     The estimated damage from calculations.
        /// </returns>
        public static double CalculateMixedDamage(
            this AIBaseClient source,
            AIBaseClient target,
            double physicalAmount,
            double magicalAmount)
        {
            return source.CalculatePhysicalDamage(target, physicalAmount)
                   + source.CalculateMagicDamage(target, magicalAmount);
        }

        /// <summary>
        ///     Gets the source auto attack damage on the target.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <returns>
        ///     The estimated auto attack damage.
        /// </returns>
        public static double GetAutoAttackDamage(this AIBaseClient source, AIBaseClient target)
        {
            double dmgPhysical = source.TotalAttackDamage, dmgMagical = 0, dmgPassive = 0;
            double dmgPhysicalAddition = 0, dmgMagicalAddition = 0, dmgPhysicalFlat = 0, dmgMagicalFlat = 0;
            var dmgReduce = 1d;

            var hero = source as AIHeroClient;
            var targetHero = target as AIHeroClient;

            // Turrets vs Minions
            var @override = source.AutoAttackDamageOverrideMod(target, dmgPhysical);
            if (@override.Item1)
            {
                return @override.Item2;
            }

            if (hero != null)
            {
                // Spoils Of War
                if (hero.IsMelee() && target is AIMinionClient && target.Team != GameObjectTeam.Neutral
                    && hero.GetBuffCount("talentreaperdisplay") > 0)
                {
                    if (GameObjects.Heroes.Any(
                            h => h.Team == hero.Team && !h.Compare(hero) && h.Distance(hero) < 1000 && h.Distance(target) < 1000))
                    {
                        var spoilwarDmg = 0;
                        if (Items.HasItem((int)ItemId.Relic_Shield, hero))
                        {
                            spoilwarDmg = 195 + 5 * hero.Level;
                        }
                        else if (Items.HasItem((int)ItemId.Targons_Brace, hero))
                        {
                            spoilwarDmg = 200 + 15 * hero.Level;
                        }
                        else if (Items.HasItem((int)ItemId.Remnant_of_the_Aspect, hero))
                        {
                            spoilwarDmg = 320 + 30 * hero.Level;
                        }
                        if (target.Health < spoilwarDmg)
                        {
                            return float.MaxValue;
                        }
                    }
                }

                // Bonus Damage (Passive)
                var passiveInfo = hero.GetPassiveDamageInfo(target);
                dmgPassive += passiveInfo.Value;
                if (passiveInfo.Override)
                {
                    return dmgPassive;
                }

                var crit = Math.Abs(hero.Crit - 1) < float.Epsilon;
                var critMultiplier = 1d;
                switch (hero.CharacterName)
                {
                    case "Kalista":
                        dmgPhysical *= 0.9;
                        break;
                    case "Corki":
                        dmgMagical = dmgPhysical * 0.8;
                        dmgPhysical = dmgPhysical - dmgMagical;
                        break;
                    case "Galio":
                        dmgPhysical = hero.HasBuff("galiopassivebuff") ? 0 : dmgPhysical;
                        break;
                    case "Jhin":
                        if (crit || hero.HasBuff("jhinpassiveattackbuff"))
                            dmgPhysical *= 0.75 * critMultiplier;
                        break;
                    case "Kled":
                        dmgPhysical *= Math.Abs(hero.AbilityResource) < float.Epsilon ? 0.8 : 1;
                        break;
                    case "Urgot":
                        if (hero.HasBuff("urgotwshield"))
                        {
                            dmgPhysicalAddition = hero.GetSpellDamage(target, SpellSlot.W);
                            dmgPhysical = 0;
                            dmgPassive /= 2;
                        }
                        break;
                    case "Yasuo":
                        if (crit)
                            dmgPhysical *= 0.9 * critMultiplier;
                        break;
                    case "Zac":
                        if (hero.HasBuff("zacqempowered"))
                        {
                            dmgMagicalFlat = hero.GetSpellDamage(target, SpellSlot.Q, DamageStage.Detonation);
                            dmgPhysical = 0;
                            dmgPassive = 0;
                        }
                        break;
                }
            }

            // Ninja Tabi
            if (targetHero != null
                && new[] { ItemId.Ninja_Tabi }.Any(i => Items.HasItem((int)i, targetHero)))
            {
                dmgReduce *= 0.88;
            }

            // Guardian's Horn
            if (targetHero != null
                && Items.HasItem((int)ItemId.Guardians_Horn, targetHero))
            {
                var dmgBlock = 12d;
                if (dmgPhysical > dmgBlock)
                    dmgPhysical -= dmgBlock;
                else if (dmgMagical > dmgBlock)
                    dmgMagical -= dmgBlock;
            }

            dmgPhysical = source.CalculatePhysicalDamage(target, dmgPhysical);
            dmgMagical = source.CalculateMagicDamage(target, dmgMagical);

            dmgPhysical += dmgPhysicalAddition;
            dmgMagical += dmgMagicalAddition;

            // Fizz P
            if (targetHero != null && targetHero.CharacterName == "Fizz")
            {
                dmgPhysical -= 4 + (2 * Math.Floor((targetHero.Level - 1) / 3d));
            }

            // This formula is right, work out the math yourself if you don't believe me
            return
                Math.Max(
                    Math.Floor((dmgPhysical + dmgMagical) * dmgReduce + source.PassiveFlatMod(target) + dmgPassive + dmgPhysicalFlat + dmgMagicalFlat),
                    0);
        }

        /// <summary>
        ///     Get the spell damage value.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="spellSlot">
        ///     The spell slot
        /// </param>
        /// <param name="stage">
        ///     The stage
        /// </param>
        /// <returns>
        ///     The <see cref="double" /> value of damage.
        /// </returns>
        public static double GetSpellDamage(
            this AIHeroClient source,
            AIBaseClient target,
            SpellSlot spellSlot,
            DamageStage stage = DamageStage.Default)
        {
            if (source == null || !source.IsValid || target == null || !target.IsValid)
            {
                return 0;
            }

            ChampionDamage value;
            if (!DamageCollection.TryGetValue(source.CharacterName, out value))
            {
                return 0;
            }

            var spellData = value.GetSlot(spellSlot)?.FirstOrDefault(e => e.Stage == stage)?.SpellData;
            if (spellData == null)
            {
                return 0;
            }

            var spellLevel =
                source.Spellbook.GetSpell(spellData.ScaleSlot != SpellSlot.Unknown ? spellData.ScaleSlot : spellSlot)
                    .Level;
            if (spellLevel == 0)
            {
                return 0;
            }

            bool alreadyAdd1 = false, alreadyAdd2 = false, alreadyAdd3 = false, isBuff = false;
            var targetHero = target as AIHeroClient;
            var targetMinion = target as AIMinionClient;

            double dmgBase = 0, dmgBonus = 0, dmgPassive = 0, dmgExtra = 0;
            var dmgReduce = 1d;

            if (spellData.DamagesPerLvl?.Count > 0)
            {
                dmgBase += spellData.DamagesPerLvl[Math.Min(source.Level - 1, spellData.DamagesPerLvl.Count - 1)];
            }
            if (spellData.Damages?.Count > 0)
            {
                dmgBase += spellData.Damages[Math.Min(spellLevel - 1, spellData.Damages.Count - 1)];
                if (!string.IsNullOrEmpty(spellData.ScalingBuff))
                {
                    var buffCount =
                        (spellData.ScalingBuffTarget == DamageScalingTarget.Source ? source : target).GetBuffCount(
                            spellData.ScalingBuff);
                    dmgBase = buffCount > 0 ? dmgBase * (buffCount + spellData.ScalingBuffOffset) : 0;
                    isBuff = buffCount <= 0;
                }
            }

            if (dmgBase > 0 || !isBuff)
            {
                if (targetMinion != null && spellData.BonusDamageOnMinion?.Count > 0)
                {
                    dmgBase +=
                        spellData.BonusDamageOnMinion[Math.Min(spellLevel - 1, spellData.BonusDamageOnMinion.Count - 1)];
                }
                if (targetMinion != null && target.Team == GameObjectTeam.Neutral && spellData.BonusDamageOnMonster?.Count > 0)
                {
                    dmgBase +=
                        spellData.BonusDamageOnMonster[Math.Min(spellLevel - 1, spellData.BonusDamageOnMonster.Count - 1)];
                }
                if (targetMinion != null && target.Team != GameObjectTeam.Neutral && target.Team != source.Team && spellData.BonusDamageOnSoldier?.Count > 0)
                {
                    dmgBase +=
                        spellData.BonusDamageOnSoldier[Math.Min(spellLevel - 1, spellData.BonusDamageOnSoldier.Count - 1)];
                }
            }

            if (dmgBase > 0)
            {
                if (spellData.IsApplyOnHit || spellData.IsModifiedDamage
                    || (spellData.SpellEffectType == SpellEffectType.Single || spellData.SpellEffectType == SpellEffectType.AoE || spellData.SpellEffectType == SpellEffectType.Attack))
                {
                    // Spoils Of War
                    if (spellData.IsApplyOnHit || spellData.IsModifiedDamage)
                    {
                        if (source.IsMelee() && target is AIMinionClient && target.Team != GameObjectTeam.Neutral
                            && source.GetBuffCount("talentreaperdisplay") > 0)
                        {
                            if (GameObjects.Heroes.Any(
                                    h => h.Team == source.Team && !h.Compare(source) && h.Distance(source) < 1000 && h.Distance(target) < 1000))
                            {
                                var spoilwarDmg = 0;
                                if (Items.HasItem((int)ItemId.Relic_Shield, source))
                                {
                                    spoilwarDmg = 195 + 5 * source.Level;
                                }
                                else if (Items.HasItem((int)ItemId.Targons_Brace, source))
                                {
                                    spoilwarDmg = 200 + 15 * source.Level;
                                }
                                else if (Items.HasItem((int)ItemId.Remnant_of_the_Aspect, source))
                                {
                                    spoilwarDmg = 320 + 30 * source.Level;
                                }
                                if (target.Health < spoilwarDmg)
                                {
                                    return float.MaxValue;
                                }
                            }
                        }
                    }

                    // Serrated Dirk
                    if (!spellData.IsModifiedDamage && source.HasBuff("itemserrateddirkprocbuff"))
                    {
                        dmgExtra += source.CalculateDamage(target, DamageType.Physical, 40);
                    }

                    alreadyAdd1 = true;
                }

                dmgBase = source.CalculateDamage(target, spellData.DamageType, dmgBase);
                dmgBase += dmgExtra;

                if (spellData.IsModifiedDamage && spellData.DamageType == DamageType.Physical
                    && targetHero != null && targetHero.CharacterName == "Fizz")
                {
                    dmgBase -= 4 + (2 * Math.Floor((targetHero.Level - 1) / 3d));
                    alreadyAdd2 = true;
                }

                if (targetHero != null && Items.HasItem((int)ItemId.Guardians_Horn, targetHero))
                {
                    dmgBase -= spellData.SpellEffectType == SpellEffectType.OverTime ? 3 : 12;
                    alreadyAdd3 = true;
                }
            }
            if (spellData.BonusDamages?.Count > 0)
            {
                foreach (var bonusDmg in spellData.BonusDamages)
                {
                    dmgExtra = 0;
                    var dmg = source.ResolveBonusSpellDamage(target, bonusDmg, spellLevel - 1);
                    if (dmg <= 0)
                    {
                        continue;
                    }

                    if (!alreadyAdd1
                        && (spellData.IsApplyOnHit || spellData.IsModifiedDamage || (spellData.SpellEffectType == SpellEffectType.Single || spellData.SpellEffectType == SpellEffectType.AoE || spellData.SpellEffectType == SpellEffectType.Attack)))
                    {
                        // Spoils Of War
                        if (spellData.IsApplyOnHit || spellData.IsModifiedDamage)
                        {
                            if (source.IsMelee() && target is AIMinionClient && target.Team != GameObjectTeam.Neutral
                                && source.GetBuffCount("talentreaperdisplay") > 0)
                            {
                                if (GameObjects.Heroes.Any(
                                        h => h.Team == source.Team && !h.Compare(source) && h.Distance(source) < 1000 && h.Distance(target) < 1000))
                                {
                                    var spoilwarDmg = 0;
                                    if (Items.HasItem((int)ItemId.Relic_Shield, source))
                                    {
                                        spoilwarDmg = 195 + 5 * source.Level;
                                    }
                                    else if (Items.HasItem((int)ItemId.Targons_Brace, source))
                                    {
                                        spoilwarDmg = 200 + 15 * source.Level;
                                    }
                                    else if (Items.HasItem((int)ItemId.Remnant_of_the_Aspect, source))
                                    {
                                        spoilwarDmg = 320 + 30 * source.Level;
                                    }
                                    if (target.Health < spoilwarDmg)
                                    {
                                        return float.MaxValue;
                                    }
                                }
                            }
                        }

                        // Serrated Dirk
                        if (!spellData.IsModifiedDamage && source.HasBuff("itemserrateddirkprocbuff"))
                        {
                            dmgExtra += source.CalculateDamage(target, DamageType.Physical, 40);
                        }

                        alreadyAdd1 = true;
                    }

                    dmgBonus += source.CalculateDamage(target, bonusDmg.DamageType, dmg);
                    dmgBonus += dmgExtra;

                    if (!alreadyAdd2 && spellData.IsModifiedDamage && bonusDmg.DamageType == DamageType.Physical
                        && targetHero != null && targetHero.CharacterName == "Fizz")
                    {
                        dmgBonus -= 4 + (2 * Math.Floor((targetHero.Level - 1) / 3d));
                        alreadyAdd2 = true;
                    }

                    if (!alreadyAdd3 && targetHero != null && Items.HasItem((int)ItemId.Guardians_Horn, targetHero))
                    {
                        dmgBonus -= spellData.SpellEffectType == SpellEffectType.OverTime ? 3 : 12;
                        alreadyAdd3 = true;
                    }
                }
            }

            var totalDamage = dmgBase + dmgBonus;

            if (spellData.DamagesOnMonster?.Count > 0 && targetMinion != null && targetMinion.Team == GameObjectTeam.Neutral)
            {
                foreach (var dmgOnMonster in spellData.DamagesOnMonster)
                {
                    totalDamage += source.CalculateDamage(
                                       target,
                                       dmgOnMonster.DamageType,
                                       source.ResolveMonsterDamage(target,
                                           dmgOnMonster,
                                           spellLevel - 1));
                }
            }

            if (totalDamage > 0)
            {
                if (spellData.ScalingValueOnSoldier > 0 && targetMinion != null && target.Team != GameObjectTeam.Neutral && target.Team != source.Team)
                {
                    totalDamage *= spellData.ScalingValueOnSoldier;
                }
                if (spellData.MaxLevelScalingValueOnMinion > 0 && spellLevel == 5 && targetMinion != null)
                {
                    totalDamage *= spellData.MaxLevelScalingValueOnMinion;
                }
                if (spellData.ScalePerCritChance > 0)
                {
                    totalDamage *= source.Crit * spellData.ScalePerCritChance + 1;
                }
                if (spellData.ScalePerTargetMissHealth?.Count > 0)
                {
                    totalDamage *= Math.Min((target.MaxHealth - target.Health) / target.MaxHealth, spellData.MaxScaleTargetMissHealth)
                                   * spellData.ScalePerTargetMissHealth[Math.Min(spellLevel - 1, spellData.ScalePerTargetMissHealth.Count - 1)] + 1;
                }
                if (spellData.DamagesReductionOnSoldier?.Count > 0 && targetMinion != null && target.Team != GameObjectTeam.Neutral && target.Team != source.Team)
                {
                    totalDamage *= 1 - spellData.DamagesReductionOnSoldier[Math.Min(spellLevel - 1, spellData.DamagesReductionOnSoldier.Count - 1)];
                }
                if (spellData.DamagesReductionPerLvlOnSoldier?.Count > 0 && targetMinion != null && target.Team != GameObjectTeam.Neutral && target.Team != source.Team)
                {
                    totalDamage *= 1 - spellData.DamagesReductionPerLvlOnSoldier[Math.Min(source.Level - 1, spellData.DamagesReductionPerLvlOnSoldier.Count - 1)];
                }
                // Jax E
                if (spellData.SpellEffectType == SpellEffectType.AoE && target.HasBuff("JaxCounterStrike"))
                {
                    totalDamage *= 0.75;
                }
                // Hand of Baron
                if (target.HasBuff("exaltedwithbaronnashorminion"))
                {
                    var minion = target as AIMinionClient;
                    if (minion != null && minion.GetMinionType() == MinionTypes.Super
                        && (spellData.SpellEffectType == SpellEffectType.AoE || spellData.SpellEffectType == SpellEffectType.OverTime))
                    {
                        totalDamage *= 0.25;
                    }
                }
                if (targetMinion != null && spellData.MaxDamageOnMinion?.Count > 0)
                {
                    totalDamage = Math.Min(
                        totalDamage,
                        spellData.MaxDamageOnMinion[Math.Min(spellLevel - 1, spellData.MaxDamageOnMinion.Count - 1)]);
                }
                if (targetMinion != null && target.Team == GameObjectTeam.Neutral && spellData.MaxDamageOnMonster?.Count > 0)
                {
                    totalDamage = Math.Min(
                        totalDamage,
                        spellData.MaxDamageOnMonster[Math.Min(spellLevel - 1, spellData.MaxDamageOnMonster.Count - 1)]);
                }
                if (targetMinion != null && target.Team != GameObjectTeam.Neutral && target.Team != source.Team && spellData.MinDamageOnSoldier?.Count > 0)
                {
                    totalDamage = Math.Max(
                        totalDamage,
                        spellData.MinDamageOnSoldier[Math.Min(spellLevel - 1, spellData.MinDamageOnSoldier.Count - 1)]);
                }
                if (spellData.IsApplyOnHit || spellData.IsModifiedDamage)
                {
                    dmgPassive += source.GetPassiveDamageInfo(target, false).Value;

                    // Ninja Tabi
                    if (targetHero != null && spellData.IsModifiedDamage
                        && new[] { ItemId.Ninja_Tabi }.Any(i => Items.HasItem((int)i, targetHero)))
                    {
                        dmgReduce *= 0.88;
                    }
                }
            }

            return
                Math.Max(
                    Math.Floor(
                        totalDamage * dmgReduce
                        + (spellData.IsApplyOnHit || spellData.IsModifiedDamage ? source.PassiveFlatMod(target) : 0)
                        + dmgPassive),
                    0);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Calculates the physical damage the source would deal towards the target on a specific given amount, taking in
        ///     consideration all of the damage modifiers.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="amount">
        ///     The amount of damage
        /// </param>
        /// <param name="ignoreArmorPercent">
        ///     The amount of armor to ignore.
        /// </param>
        /// <returns>
        ///     The amount of estimated damage dealt to target from source.
        /// </returns>
        internal static double CalculatePhysicalDamage(
            this AIBaseClient source,
            AIBaseClient target,
            double amount,
            double ignoreArmorPercent)
        {
            return source.CalculatePhysicalDamage(target, amount) * ignoreArmorPercent;
        }

        /// <summary>
        ///     Calculates the magic damage the source would deal towards the target on a specific given amount, taking in
        ///     consideration all of the damage modifiers.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="amount">
        ///     The amount
        /// </param>
        /// <returns>
        ///     The amount of estimated damage dealt to target from source.
        /// </returns>
        private static double CalculateMagicDamage(this AIBaseClient source, AIBaseClient target, double amount)
        {
            if (amount <= 0)
            {
                return 0;
            }

            // Penetration can't reduce magic resist below 0.
            var magicResist = target.SpellBlock;
            var bonusMagicResist = target.BonusSpellBlock;

            double value, flatValue = 0d;

            if (magicResist < 0)
            {
                value = 2 - (100 / (100 - magicResist));
            }
            else if ((magicResist * source.PercentMagicPenetrationMod)
                - bonusMagicResist * (1 - source.PercentBonusMagicPenetrationMod)
                - source.FlatMagicPenetrationMod - source.MagicLethality < 0)
            {
                value = 1;
            }
            else
            {
                value = 100 / (100 + (magicResist * source.PercentMagicPenetrationMod)
                    - bonusMagicResist * (1 - source.PercentBonusMagicPenetrationMod)
                    - source.FlatMagicPenetrationMod - source.MagicLethality);
            }

            // Amumu P
            if (target.HasBuff("cursedtouch"))
            {
                flatValue = 0.1 * amount;
            }

            return
                Math.Max(
                    Math.Floor(
                        source.DamageReductionMod(
                            target,
                            source.PassivePercentMod(target, value, DamageType.Magical) * amount,
                            DamageType.Magical)) + flatValue,
                    0);
        }

        /// <summary>
        ///     Calculates the physical damage the source would deal towards the target on a specific given amount, taking in
        ///     consideration all of the damage modifiers.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="amount">
        ///     The amount of damage
        /// </param>
        /// <returns>
        ///     The amount of estimated damage dealt to target from source.
        /// </returns>
        private static double CalculatePhysicalDamage(this AIBaseClient source, AIBaseClient target, double amount)
        {
            if (amount <= 0)
            {
                return 0;
            }

            double armorPenetrationPercent = source.PercentArmorPenetrationMod;
            double armorPenetrationFlat = source.FlatArmorPenetrationMod + source.PhysicalLethality;
            double bonusArmorPenetrationMod = source.PercentBonusArmorPenetrationMod;

            // Minions return wrong percent values.
            if (source is AIMinionClient)
            {
                armorPenetrationFlat = 0;
                armorPenetrationPercent = 1;
                bonusArmorPenetrationMod = 1;
            }

            // Turrets passive.
            var turret = source as AITurretClient;
            if (turret != null)
            {
                armorPenetrationFlat = 0;
                armorPenetrationPercent = 0.7;
                bonusArmorPenetrationMod = 1;
            }

            // Penetration can't reduce armor below 0.
            var armor = target.Armor;
            var bonusArmor = target.BonusArmor;

            double value;
            if (armor < 0)
            {
                value = 2 - (100 / (100 - armor));
            }
            else if ((armor * armorPenetrationPercent) - (bonusArmor * (1 - bonusArmorPenetrationMod))
                     - armorPenetrationFlat < 0)
            {
                value = 1;
            }
            else
            {
                value = 100
                        / (100 + (armor * armorPenetrationPercent) - (bonusArmor * (1 - bonusArmorPenetrationMod))
                           - armorPenetrationFlat);
            }

            // Take into account the percent passives, flat passives and damage reduction.
            return
                Math.Max(
                    Math.Floor(
                        source.DamageReductionMod(
                            target,
                            source.PassivePercentMod(target, value, DamageType.Physical) * amount,
                            DamageType.Physical)),
                    0);
        }

        /// <summary>
        ///     Applies damage reduction mod calculations towards the given amount of damage, a modifier onto the amount based on
        ///     damage reduction passives.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="amount">
        ///     The amount.
        /// </param>
        /// <param name="damageType">
        ///     The damage Type.
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        private static double DamageReductionMod(
            this AIBaseClient source,
            AIBaseClient target,
            double amount,
            DamageType damageType)
        {
            var targetHero = target as AIHeroClient;
            var targetMinion = target as AIMinionClient;
            var sourceMinion = source as AIMinionClient;

            if (targetHero != null && source is AIMinionClient && source.Team == GameObjectTeam.Neutral)
            {
                // Ancient Grudge
                if (new[] { "SRU_Dragon_Air", "SRU_Dragon_Earth", "SRU_Dragon_Fire", "SRU_Dragon_Water" }.Any(i => i.Equals(source.CharacterName)))
                {
                    var dragonBuff = targetHero.GetBuff("dragonbuff_tooltipmanager");
                    if (dragonBuff != null)
                    {
                        amount *= 1 + 0.2 * dragonBuff.ToolTipVars.Take(4).Sum();
                    }
                }
            }

            if (source is AIHeroClient)
            {
                if (target is AIMinionClient && target.Team == GameObjectTeam.Neutral)
                {
                    // Baron's Gaze
                    if (source.HasBuff("barontarget") && target.CharacterName.Equals("SRU_Baron"))
                    {
                        amount *= 0.5;
                    }

                    // Ancient Grudge
                    if (new[] { "SRU_Dragon_Air", "SRU_Dragon_Earth", "SRU_Dragon_Fire", "SRU_Dragon_Water" }.Any(i => i.Equals(target.CharacterName)))
                    {
                        var dragonBuff = source.GetBuff("dragonbuff_tooltipmanager");
                        if (dragonBuff != null)
                        {
                            amount *= 1 - 0.07 * dragonBuff.ToolTipVars.Take(4).Sum();
                        }
                    }

                    // Shyvana P
                    if (source.HasBuff("shyvanapassive")
                        && new[] { "SRU_Dragon_Air", "SRU_Dragon_Earth", "SRU_Dragon_Fire", "SRU_Dragon_Water", "SRU_Dragon_Elder", "TT_Spiderboss" }.Any(i => i.Equals(target.CharacterName)))
                    {
                        amount *= 1.2;
                    }
                }

                // Exhaust
                if (source.HasBuff("SummonerExhaust"))
                {
                    amount *= 0.6;
                }
            }

            // Vladimir R.P
            if (target.HasBuff("vladimirhemoplaguedamageamp"))
            {
                amount *= 1.1;
            }

            // Sona W
            var sonaW = source.GetBuff("sonapassivedebuff");
            if (sonaW != null)
            {
                var sona = (AIHeroClient)sonaW.Caster;
                amount *= 1 - 0.25
                            - 0.04 * sona.TotalMagicalDamage / 100;
            }

            // Hand of Baron
            if (targetMinion != null && targetMinion.HasBuff("exaltedwithbaronnashorminion"))
            {
                switch (targetMinion.GetMinionType())
                {
                    case MinionTypes.Ranged:
                        if (source is AIHeroClient)
                            amount *= 0.3;
                        break;
                    case MinionTypes.Melee:
                        if (source is AIHeroClient)
                            amount *= 0.3;
                        if (source is AIMinionClient && source.Team != GameObjectTeam.Neutral)
                            amount *= 0.25;
                        break;
                }
            }
            if (sourceMinion != null
                && target is AITurretClient
                && sourceMinion.GetMinionType() == MinionTypes.Siege
                && sourceMinion.HasBuff("exaltedwithbaronnashorminion"))
            {
                amount *= 2;
            }

            if (targetHero != null)
            {
                // Alistar R
                if (targetHero.HasBuff("FerociousHowl"))
                {
                    amount *= 1 - new[] { 0.55, 0.65, 0.75 }[targetHero.Spellbook.GetSpell(SpellSlot.R).Level - 1];
                }
                // Amumu E
                if (targetHero.HasBuff("Tantrum") && damageType == DamageType.Physical)
                {
                    amount -= new[] { 2, 4, 6, 8, 10 }[targetHero.Spellbook.GetSpell(SpellSlot.E).Level - 1]
                        + 0.03 * targetHero.BonusArmor
                        + 0.03 * targetHero.BonusSpellBlock;
                }
                // Annie E
                if (targetHero.HasBuff("AnnieE"))
                {
                    amount *= 1 - new[] { 0.16, 0.22, 0.28, 0.34, 0.4 }[targetHero.Spellbook.GetSpell(SpellSlot.E).Level - 1];
                }
                // Braum E
                if (targetHero.HasBuff("braumeshieldbuff"))
                {
                    amount *= 1 - new[] { 0.3, 0.325, 0.35, 0.375, 0.4 }[targetHero.Spellbook.GetSpell(SpellSlot.E).Level - 1];
                }
                // Galio W
                if (targetHero.HasBuff("galiowbuff"))
                {
                    var percent = new[] { 0.2, 0.25, 0.3, 0.35, 0.4 }[targetHero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                        + 0.05 * targetHero.TotalMagicalDamage / 100
                        + 0.08 * targetHero.BonusSpellBlock / 100;
                    amount *= 1 - (damageType == DamageType.Magical ? percent : (damageType == DamageType.Physical ? percent / 2 : 0));
                }
                // Garen W
                var garenW = targetHero.GetBuff("GarenW");
                if (garenW != null)
                {
                    amount *= 1 - ((Game.Time - garenW.StartTime) < 0.75 ? 0.6 : 0.3);
                }
                // Gragas W
                if (targetHero.HasBuff("gragaswself"))
                {
                    amount *= 1 - new[] { 0.1, 0.12, 0.14, 0.16, 0.18 }[targetHero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                                - 0.04 * targetHero.TotalMagicalDamage / 100;
                }
                // Irelia W
                if (targetHero.HasBuff("ireliawdefense") && damageType == DamageType.Physical)
                {
                    amount *= 1 - 0.5
                                - 0.07 * targetHero.TotalMagicalDamage / 100;
                }
                // Kassadin P
                if (targetHero.HasBuff("voidstone") && damageType == DamageType.Magical)
                {
                    amount *= 1 - 0.15;
                }
                // MasterYi W
                if (targetHero.HasBuff("Meditate"))
                {
                    amount *= 1 - new[] { 0.6, 0.625, 0.65, 0.675, 0.7 }[targetHero.Spellbook.GetSpell(SpellSlot.W).Level - 1]
                                  * (source is AITurretClient ? 0.5 : 1);
                }
                // Warwick E
                if (targetHero.HasBuff("WarwickE"))
                {
                    amount *= 1 - new[] { 0.35, 0.4, 0.45, 0.5, 0.55 }[targetHero.Spellbook.GetSpell(SpellSlot.E).Level - 1];
                }
            }

            return amount;
        }

        /// <summary>
        ///     Apples passive percent mod calculations towards the given amount of damage, a modifier onto the amount based on
        ///     passive flat effects.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <returns>
        ///     The damage after passive flat modifier calculations.
        /// </returns>
        private static double PassiveFlatMod(this GameObject source, AIBaseClient target)
        {
            return 0d;
        }

        /// <summary>
        ///     Apples passive percent mod calculations towards the given amount of damage, a modifier onto the amount based on
        ///     passive percent effects.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target
        /// </param>
        /// <param name="amount">
        ///     The amount
        /// </param>
        /// <param name="damageType">
        ///     The damage Type.
        /// </param>
        /// <returns>
        ///     The damage after passive percent modifier calculations.
        /// </returns>
        private static double PassivePercentMod(
            this AIBaseClient source,
            AIBaseClient target,
            double amount,
            DamageType damageType)
        {
            return amount;
        }

        private static Tuple<bool, double> AutoAttackDamageOverrideMod(
            this AIBaseClient source,
            AIBaseClient target,
            double amount)
        {
            var origin = amount;

            if (source is AITurretClient)
            {
                var minion = target as AIMinionClient;
                if (minion != null)
                {
                    var minionType = minion.GetMinionType();
                    if (minionType.HasFlag(MinionTypes.Melee))
                    {
                        amount = 0.45 * minion.MaxHealth;
                    }
                    else if (minionType.HasFlag(MinionTypes.Ranged))
                    {
                        amount = 0.7 * minion.MaxHealth;
                    }
                    else if (minionType.HasFlag(MinionTypes.Siege))
                    {
                        switch ((source as AITurretClient).GetTurretType())
                        {
                            case TurretType.TierOne:
                                amount = 0.14 * minion.MaxHealth;
                                break;
                            case TurretType.TierTwo:
                                amount = 0.11 * minion.MaxHealth;
                                break;
                            case TurretType.TierThree:
                            case TurretType.TierFour:
                                amount = 0.08 * minion.MaxHealth;
                                break;
                        }
                    }
                    else if (minionType.HasFlag(MinionTypes.Super))
                    {
                        amount = 0.05 * minion.MaxHealth;
                    }
                }
            }

            return new Tuple<bool, double>(amount != origin, amount);
        }

        #endregion
    }
}