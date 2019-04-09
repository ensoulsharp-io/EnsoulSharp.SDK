// <copyright file="DamagePassives.cs" company="EnsoulSharp">
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
    using Utils;

    /// <summary>
    ///     Damage wrapper class, contains functions to calculate estimated damage to a unit and also provides damage details.
    /// </summary>
    public static partial class Damage
    {
        #region Static Fields

        private static readonly IDictionary<string, List<PassiveDamage>> PassiveDamages =
            new Dictionary<string, List<PassiveDamage>>();

        #endregion

        #region Methods

        private static void AddPassiveAttack(
            string championName,
            Func<AIHeroClient, AIBaseClient, bool> condition,
            DamageType damageType,
            Func<AIHeroClient, AIBaseClient, double> func,
            bool ignoreCalculation = false,
            bool @override = false)
        {
            var passive = new PassiveDamage
                              {
                                  Condition = condition, Func = func, DamageType = damageType, Override = @override,
                                  IgnoreCalculation = ignoreCalculation
                              };

            if (PassiveDamages.ContainsKey(championName))
            {
                PassiveDamages[championName].Add(passive);
                return;
            }

            PassiveDamages.Add(championName, new List<PassiveDamage> { passive });
        }

        private static void CreatePassives()
        {
            AddPassiveAttack(
                string.Empty,
                (hero, @base) =>
                !new[] { "Ashe", "Corki", "Fiora", "Galio", "Graves", "Jayce", "Jhin", "Kayle", "Kled", "Pantheon", "Shaco", "Urgot", "Yasuo", "Zac" }.Contains(
                    hero.CharacterName) && Math.Abs(hero.Crit - 1) < float.Epsilon,
                DamageType.Physical,
                (hero, @base) =>
                (hero.TotalAttackDamage * (hero.CharacterName == "Kalista" ? 0.9 : 1)) * hero.GetCritMultiplier());
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Hunters_Machete, hero) && @base is AIMinionClient && @base.Team == GameObjectTeam.Neutral,
                DamageType.Physical,
                (hero, @base) => 35);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Dorans_Ring, hero) && @base is AIMinionClient && @base.Team != GameObjectTeam.Neutral && @base.Team != hero.Team,
                DamageType.Physical,
                (hero, @base) => 5);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.GetBuffCount("kagesluckypickdisplay") > 0,
                DamageType.Magical,
                (hero, @base) => Items.HasItem((int)ItemId.Spellthiefs_Edge, hero) ? 13 : (Items.HasItem((int)ItemId.Frostfang, hero) || Items.HasItem((int)ItemId.Remnant_of_the_Watchers, hero)) ? 18 : 0);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Dorans_Shield, hero) && @base is AIMinionClient && @base.Team != GameObjectTeam.Neutral && @base.Team != hero.Team,
                DamageType.Physical,
                (hero, @base) => 5);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.GetBuffCount("itemstatikshankcharge") == 100,
                DamageType.Magical,
                (hero, @base) =>
                {
                    var stormrazor = Items.HasItem((int)ItemId.Stormrazor);
                    var d0 = stormrazor ? 65 : 0;
                    var d1 = Items.HasItem((int)ItemId.Kircheis_Shard, hero) ? 50 : 0;
                    var d2 = Items.HasItem((int)ItemId.Statikk_Shiv, hero)
                                 ? new[] {
                                             60, 60, 60, 60, 60, 67, 73, 79, 85, 91, 97, 104, 110, 116, 122, 128, 134, 140
                                         }[Math.Min(17, Math.Max(0, hero.Level - 1))]
                                   * hero.GetCritMultiplier(true) * (stormrazor ? 1.3 : 1)
                                 : 0;
                    var d3 = Items.HasItem((int)ItemId.Rapid_Firecannon, hero)
                                 ? new[]
                                         {
                                             60, 60, 60, 60, 60, 67, 73, 79, 85, 91, 97, 104, 110, 116, 122, 128, 134, 140
                                         }[Math.Min(17, Math.Max(0, hero.Level - 1))]
                                   * (stormrazor ? 1.3 : 1)
                                 : 0;
                    return Math.Max(d0, Math.Max(d1, Math.Max(d2, d3)));
                });
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Recurve_Bow, hero),
                DamageType.Physical,
                (hero, @base) => 15);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Trackers_Knife, hero) && @base is AIMinionClient && @base.Team == GameObjectTeam.Neutral,
                DamageType.Physical,
                (hero, @base) => 40);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Stalkers_Blade, hero) && @base is AIMinionClient && @base.Team == GameObjectTeam.Neutral,
                DamageType.Physical,
                (hero, @base) => 40);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.HasBuff("sheen"),
                DamageType.Physical,
                (hero, @base) =>
                {
                    var d1 = Items.HasItem((int)ItemId.Sheen, hero) ? hero.BaseAttackDamage : 0;
                    var d2 = Items.HasItem((int)ItemId.Trinity_Fusion, hero) ? 2 * hero.BaseAttackDamage : 0;
                    return Math.Max(d1, d2);
                });
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.HasBuff("itemangelhandbuff"),
                DamageType.Magical,
                (hero, @base) => 5 + 15d / 17 * (hero.Level - 1));
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.HasBuff("Muramana") && hero.ManaPercent > 20,
                DamageType.Physical,
                (hero, @base) => 0.06 * hero.Mana);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Wits_End, hero),
                DamageType.Magical,
                (hero, @base) => 15 + 65d / 17 * (hero.Level - 1));
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Trackers_Knife_Enchantment_Bloodrazor) || Items.HasItem((int)ItemId.Stalkers_Blade_Enchantment_Bloodrazor),
                DamageType.Physical,
                (hero, @base) => Math.Min(@base is AIMinionClient ? 75 : float.MaxValue, 0.04 * @base.MaxHealth));
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.HasBuff("itemfrozenfist"),
                DamageType.Physical,
                (hero, @base) => hero.BaseAttackDamage);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.HasBuff("itemdusknightstalkerdamageproc"),
                DamageType.Physical,
                (hero, @base) => 30 + 120d / 17 * (hero.Level - 1));
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.HasBuff("dreadnoughtmomentumbuff"),
                DamageType.Magical,
                (hero, @base) => hero.GetBuffCount("dreadnoughtmomentumbuff"));
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Nashors_Tooth, hero),
                DamageType.Magical,
                (hero, @base) => 15 + 0.15 * hero.TotalMagicalDamage);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.HasBuff("lichbane"),
                DamageType.Magical,
                (hero, @base) => 0.75 * hero.BaseAttackDamage + 0.5 * hero.TotalMagicalDamage);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Blade_of_the_Ruined_King, hero) || Items.HasItem((int)ItemId.Might_of_the_Ruined_King, hero),
                DamageType.Physical,
                (hero, @base) => Math.Max(15, Math.Min(@base is AIMinionClient ? 60 : float.MaxValue, 0.08 * @base.Health)));
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Guinsoos_Rageblade, hero),
                DamageType.Magical,
                (hero, @base) => 15);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => Items.HasItem((int)ItemId.Titanic_Hydra, hero),
                DamageType.Physical,
                (hero, @base) => hero.HasBuff("itemtitanichydracleavebuff") ? 40 + 0.1 * hero.MaxHealth : 5 + 0.01 * hero.MaxHealth);
            AddPassiveAttack(
                string.Empty,
                (hero, @base) => hero.HasBuff("TrinityForce"),
                DamageType.Physical,
                (hero, @base) => 2 * hero.BaseAttackDamage);

            var excluded = new List<string>();
            foreach (var name in GameObjects.Heroes.Select(h => h.CharacterName).Where(name => !excluded.Contains(name)))
            {
                excluded.Add(name);

                // This is O(scary), but seems quick enough in practice.
                switch (name)
                {
                    case "Aatrox":
                        AddPassiveAttack(
                            "Aatrox",
                            (hero, @base) => hero.HasBuff("aatroxpassiveready"),
                            DamageType.Physical,
                            (hero, @base) => Math.Min((@base is AIMinionClient && @base.Team == GameObjectTeam.Neutral) ? 400 : float.MaxValue, @base.MaxHealth * (0.08 + 0.0047 * Math.Max(0, (hero.Level - 1)))));
                        break;
                    case "Akali":
                        AddPassiveAttack(
                            "Akali",
                            (hero, @base) => hero.HasBuff("akalipweapon"),
                            DamageType.Magical,
                            (hero, @base) => new[] { 39, 42, 45, 48, 51, 54, 57, 60, 69, 78, 87, 96, 105, 120, 135, 150, 165, 180 }[Math.Min(17, Math.Max(0, hero.Level - 1))] + 0.9 * hero.FlatPhysicalDamageMod + 0.7 * hero.TotalMagicalDamage);
                        break;
                    case "Alistar":
                        AddPassiveAttack(
                            "Alistar",
                            (hero, @base) => hero.HasBuff("alistareattack"),
                            DamageType.Magical,
                            (hero, @base) => 35 + 15 * Math.Max(0, hero.Level - 1));
                        break;
                    case "Ashe":
                        AddPassiveAttack(
                            "Ashe",
                            (hero, @base) => @base.HasBuff("ashepassiveslow"),
                            DamageType.Physical,
                            (hero, @base) => (0.1 + hero.Crit) * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Ashe",
                            (hero, @base) => hero.HasBuff("AsheQAttack"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true,
                            true);
                        break;
                    case "Bard":
                        AddPassiveAttack(
                            "Bard",
                            (hero, @base) => hero.GetBuffCount("bardpspiritammocount") > 0,
                            DamageType.Magical,
                            (hero, @base) => 40 + Math.Floor((double)hero.Spellbook.GetSpell((SpellSlot)63).Ammo / 5) * 15 + 0.3 * hero.TotalMagicalDamage);
                        break;
                    case "Blitzcrank":
                        AddPassiveAttack(
                            "Blitzcrank",
                            (hero, @base) => hero.HasBuff("PowerFist"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            true);
                        break;
                    case "Braum":
                        AddPassiveAttack(
                            string.Empty,
                            (hero, @base) => @base.GetBuffCount("BraumMark") == 3,
                            DamageType.Magical,
                            (hero, @base) => 26 + 10 * Math.Max(0, ((AIHeroClient)@base.GetBuff("BraumMark").Caster).Level - 1));
                        AddPassiveAttack(
                            "Braum",
                            (hero, @base) => @base.HasBuff("braummarkstunreduction"),
                            DamageType.Magical,
                            (hero, @base) => 0.2 * (26 + 10 * Math.Max(0, ((AIHeroClient)@base.GetBuff("braummarkstunreduction").Caster).Level - 1)));
                        break;
                    case "Caitlyn":
                        AddPassiveAttack(
                            "Caitlyn",
                            (hero, @base) =>
                            {
                                var WEBuff = @base.GetBuff("caitlynyordletrapinternal");
                                if (WEBuff != null && WEBuff.Caster.Compare(hero))
                                    return true;
                                return hero.HasBuff("caitlynheadshot");
                            },
                            DamageType.Physical,
                            (hero, @base) =>
                            {
                                var dmg = 0d;
                                if (@base is AIHeroClient)
                                {
                                    if (hero.Level >= 13)
                                        dmg = (1 + 1.25 * hero.Crit) * hero.TotalAttackDamage;
                                    else if (hero.Level >= 7)
                                        dmg = (0.75 + 1.25 * hero.Crit) * hero.TotalAttackDamage;
                                    else
                                        dmg = (0.5 + 1.25 * hero.Crit) * hero.TotalAttackDamage;
                                    if (@base.HasBuff("caitlynyordletrapsight"))
                                    {
                                        var idx = Math.Max(0, hero.Spellbook.GetSpell(SpellSlot.W).Level - 1);
                                        dmg += new[] { 40, 90, 140, 190, 240 }[idx] + new[] { 0.4, 0.55, 0.7, 0.85, 1 }[idx] * hero.FlatPhysicalDamageMod;
                                    }
                                }
                                else
                                {
                                    dmg = (1 + 1.25 * hero.Crit) * hero.TotalAttackDamage;
                                }
                                return dmg;
                            });
                        break;
                    case "Camille":
                        AddPassiveAttack(
                            "Camille",
                            (hero, @base) => hero.HasBuff("CamilleQ"),
                            DamageType.Physical,
                            (hero, @base) => new[] { 0.2, 0.25, 0.3, 0.35, 0.4 }[Math.Max(0, hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1)] * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Camille",
                            (hero, @base) => hero.HasBuff("CamilleQ2"),
                            DamageType.Physical,
                            (hero, @base) => (1 - Math.Min(1, 0.4 + 0.04 * Math.Max(0, hero.Level - 1))) * 2 * new[] { 0.2, 0.25, 0.3, 0.35, 0.4 }[Math.Max(0, hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1)] * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Camille",
                            (hero, @base) => hero.HasBuff("CamilleQ2"),
                            DamageType.True,
                            (hero, @base) => Math.Min(1, 0.4 + 0.04 * Math.Max(0, hero.Level - 1)) * 2 * new[] { 0.2, 0.25, 0.3, 0.35, 0.4 }[Math.Max(0, hero.Spellbook.GetSpell(SpellSlot.Q).Level - 1)] * hero.TotalAttackDamage);
                        break;
                    case "Chogath":
                        AddPassiveAttack(
                            "Chogath",
                            (hero, @base) => hero.HasBuff("VorpalSpikes"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            true);
                        break;
                    case "Corki":
                        AddPassiveAttack(
                            "Corki",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon,
                            DamageType.Magical,
                            (hero, @base) => 0.8 * hero.GetCritMultiplier() * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Corki",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon,
                            DamageType.Physical,
                            (hero, @base) => 0.2 * hero.GetCritMultiplier() * hero.TotalAttackDamage);
                        break;
                    case "Darius":
                        AddPassiveAttack(
                            "Darius",
                            (hero, @base) => hero.HasBuff("DariusNoxianTacticsONH"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Diana":
                        AddPassiveAttack(
                            "Diana",
                            (hero, @base) => hero.HasBuff("dianaarcready"),
                            DamageType.Magical,
                            (hero, @base) => new[] { 20, 25, 30, 35, 40, 50, 60, 70, 80, 90, 105, 120, 135, 155, 175, 200, 225, 250 }[Math.Min(17, Math.Max(0, hero.Level - 1))] + 0.8 * hero.TotalMagicalDamage);
                        break;
                    case "Draven":
                        AddPassiveAttack(
                            "Draven",
                            (hero, @base) => hero.HasBuff("DravenSpinning"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        break;
                    case "DrMundo":
                        AddPassiveAttack(
                            "DrMundo",
                            (hero, @base) => hero.HasBuff("Masochism") && hero.AttackRange >= 150,
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            true);
                        break;
                    case "Ekko":
                        AddPassiveAttack(
                            "Ekko",
                            (hero, @base) => @base.GetBuffCount("ekkostacks") == 2,
                            DamageType.Magical,
                            (hero, @base) =>
                            {
                                var dmg = new[] { 30, 40, 50, 60, 70, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140 }[Math.Min(17, Math.Max(0, hero.Level - 1))] + 0.8 * hero.TotalMagicalDamage;
                                if (@base is AIMinionClient && @base.Team == GameObjectTeam.Neutral)
                                    dmg = Math.Min(600, dmg * 2);
                                return dmg;
                            });
                        AddPassiveAttack(
                            "Ekko",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.W).Level > 0 && @base.HealthPercent < 30,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        AddPassiveAttack(
                            "Ekko",
                            (hero, @base) => hero.HasBuff("ekkoeattackbuff"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            true);
                        break;
                    case "Elise":
                        AddPassiveAttack(
                            "Elise",
                            (hero, @base) => hero.HasBuff("EliseR"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.R, DamageStage.SecondForm),
                            true);
                        break;
                    case "Ezreal":
                        AddPassiveAttack(
                            "Ezreal",
                            (hero, @base) => @base.HasBuff("ezrealwattach"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Fiora":
                        AddPassiveAttack(
                            "Fiora",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon && !hero.HasBuff("FioraE") && !hero.HasBuff("fiorae2"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetCritMultiplier() * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Fiora",
                            (hero, @base) => hero.HasBuff("fiorae2"),
                            DamageType.Physical,
                            (hero, @base) => (hero.GetCritMultiplier() + new[] { -0.4, -0.3, -0.2, -0.1, 0 }[Math.Max(0, hero.Spellbook.GetSpell(SpellSlot.E).Level - 1)]) * hero.TotalAttackDamage);
                        break;
                    case "Fizz":
                        AddPassiveAttack(
                            "Fizz",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.W).Level > 0,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        AddPassiveAttack(
                            "Fizz",
                            (hero, @base) => hero.HasBuff("FizzW"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W, DamageStage.Empowered),
                            true);
                        AddPassiveAttack(
                            "Fizz",
                            (hero, @base) => hero.HasBuff("fizzonhitbuff"),
                            DamageType.Magical,
                            (hero, @base) => new[] { 10, 15, 20, 25, 30 }[Math.Max(0, hero.Spellbook.GetSpell(SpellSlot.W).Level - 1)] + 0.35 * hero.TotalMagicalDamage);
                        break;
                    case "Galio":
                        AddPassiveAttack(
                            "Galio",
                            (hero, @base) => hero.HasBuff("galiopassivebuff"),
                            DamageType.Magical,
                            (hero, @base) => 12 + 4 * Math.Max(0, hero.Level - 1) + hero.TotalAttackDamage + 0.5 * hero.TotalMagicalDamage + 0.4 * hero.BonusSpellBlock);
                        AddPassiveAttack(
                            "Galio",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon && hero.HasBuff("galiopassivebuff"),
                            DamageType.Magical,
                            (hero, @base) => hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Galio",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon && !hero.HasBuff("galiopassivebuff"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetCritMultiplier() * hero.TotalAttackDamage);
                        break;
                    case "Gangplank":
                        AddPassiveAttack(
                            "Gangplank",
                            (hero, @base) => hero.HasBuff("gangplankpassiveattack"),
                            DamageType.True,
                            (hero, @base) => (55 + 10 * Math.Max(0, hero.Level - 1) + hero.FlatPhysicalDamageMod) * (@base is AITurretClient ? 0.5 : 1));
                        break;
                    case "Garen":
                        AddPassiveAttack(
                            "Garen",
                            (hero, @base) => hero.HasBuff("GarenQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        AddPassiveAttack(
                            "Garen",
                            (hero, @base) => @base.HasBuff("garenpassiveenemytarget"),
                            DamageType.True,
                            (hero, @base) => 0.01 * @base.MaxHealth);
                        break;
                    case "Gnar":
                        AddPassiveAttack(
                            "Gnar",
                            (hero, @base) => @base.GetBuffCount("gnarwproc") == 2,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Gragas":
                        AddPassiveAttack(
                            "Gragas",
                            (hero, @base) => hero.HasBuff("gragaswattackbuff"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Hecarim":
                        AddPassiveAttack(
                            "Hecarim",
                            (hero, @base) => hero.HasBuff("hecarimrampspeed"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            true);
                        break;
                    case "Illaoi":
                        AddPassiveAttack(
                            "Illaoi",
                            (hero, @base) => hero.HasBuff("IllaoiW"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Irelia":
                        AddPassiveAttack(
                            "Irelia",
                            (hero, @base) => hero.GetBuffCount("ireliapassivestacks") == 5,
                            DamageType.Magical,
                            (hero, @base) => 15 + (double)(66 - 15) / 17 * Math.Max(0, hero.Level - 1) + 0.25 * hero.FlatPhysicalDamageMod);
                        break;
                    case "Ivern":
                        AddPassiveAttack(
                            "Ivern",
                            (hero, @base) => hero.HasBuff("ivernwpassive"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Janna":
                        AddPassiveAttack(
                            string.Empty,
                            (hero, @base) => hero.Hero == Champion.Janna || hero.HasBuff(""),
                            DamageType.Magical,
                            (hero, @base) =>
                            {
                                var source = hero;
                                if (hero.Hero != Champion.Janna)
                                    source = (AIHeroClient)hero.GetBuff("").Caster;
                                return (source.Level >= 10 ? 0.35 : 0.25) * (source.MoveSpeed - source.CharacterData.BaseMoveSpeed);
                            });
                        break;
                    case "JarvanIV":
                        AddPassiveAttack(
                            "JarvenIV",
                            (hero, @base) => !@base.HasBuff("jarvanivmartialcadencecheck"),
                            DamageType.Physical,
                            (hero, @base) => Math.Min(400, Math.Max(20, 0.08 * @base.Health)));
                        break;
                    case "Jax":
                        AddPassiveAttack(
                            "Jax",
                            (hero, @base) => hero.HasBuff("JaxEmpowerTwo"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Jayce":
                        AddPassiveAttack(
                            "Jayce",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon && !hero.HasBuff("JayceHyperCharge"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetCritMultiplier() * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Jayce",
                            (hero, @base) => hero.HasBuff("JayceHyperCharge"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W, DamageStage.SecondForm),
                            true,
                            true);
                        AddPassiveAttack(
                            "Jayce",
                            (hero, @base) => hero.HasBuff("JaycePassiveMeleeAttack"),
                            DamageType.Magical,
                            (hero, @base) =>
                            {
                                var dmg = 0.25 * hero.FlatPhysicalDamageMod;
                                if (hero.Level >= 16)
                                    dmg += 145;
                                else if (hero.Level >= 11)
                                    dmg += 105;
                                else if (hero.Level >= 6)
                                    dmg += 65;
                                else
                                    dmg += 25;
                                return dmg;
                            });
                        break;
                    case "Jhin":
                        AddPassiveAttack(
                            "Jhin",
                            (hero, @base) => hero.HasBuff("jhinpassiveattackbuff"),
                            DamageType.Physical,
                            (hero, @base) => (hero.Level >= 11 ? 0.25 : (hero.Level >= 6 ? 0.2 : 0.15)) * (@base.MaxHealth - @base.Health));
                        AddPassiveAttack(
                            "Jhin",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon || hero.HasBuff("jhinpassiveattackbuff"),
                            DamageType.Physical,
                            (hero, @base) => 0.75 * (hero.HasBuff("jhinpassiveattackbuff") ? 1 : hero.GetCritMultiplier()) * hero.TotalAttackDamage);
                        break;
                    case "Jinx":
                        AddPassiveAttack(
                            "Jinx",
                            (hero, @base) => hero.HasBuff("JinxQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q) * hero.GetCritMultiplier(true),
                            true);
                        break;
                    case "Kaisa":
                        AddPassiveAttack(
                            "Kaisa",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) =>
                            {
                                var dmg = hero.Level >= 17 ? 10d
                                : (hero.Level >= 14 ? 9d
                                : (hero.Level >= 11 ? 8d
                                : (hero.Level >= 9 ? 7d
                                : (hero.Level >= 6 ? 6d
                                : (hero.Level >= 3 ? 5d
                                : 4d)))));
                                var passiveCnt = @base.GetBuffCount("kaisapassivemarker");
                                if (passiveCnt > 0)
                                {
                                    dmg += (hero.Level >= 16 ? 5d
                                    : (hero.Level >= 12 ? 4d
                                    : (hero.Level >= 8 ? 3d
                                    : (hero.Level >= 4 ? 2d
                                    : 1d)))) * passiveCnt;
                                }
                                dmg += new[] { 0.1, 0.125, 0.15, 0.175, 0.2 }[Math.Min(4, Math.Max(0, passiveCnt))] * hero.TotalMagicalDamage;
                                return dmg;
                            });
                        AddPassiveAttack(
                            "Kaisa",
                            (hero, @base) => @base.GetBuffCount("kaisapassivemarker") == 3,
                            DamageType.Magical,
                            (hero, @base) => Math.Min(@base is AIMinionClient && @base.Team == GameObjectTeam.Neutral ? 400 : float.MaxValue, (0.15 + hero.TotalMagicalDamage / 100 * 0.025) * (@base.MaxHealth - @base.Health)));
                        break;
                    case "Kalista":
                        AddPassiveAttack(
                            "Kalista",
                            (hero, @base) => @base.HasBuff("kalistacoopstrikemarkally"),
                            DamageType.Magical,
                            (hero, @base) => (@base is AIMinionClient && @base.Team != GameObjectTeam.Neutral && @base.Team != hero.Team && @base.Health < 125) ? 125 : hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        AddPassiveAttack(
                            string.Empty,
                            (hero, @base) => @base.HasBuff("kalistacoopstrikemarkself") && hero.HasBuff("kalistacoopstrikeally"),
                            DamageType.Magical,
                            (hero, @base) => (@base is AIMinionClient && @base.Team != GameObjectTeam.Neutral && @base.Team != hero.Team && @base.Health < 125) ? 125 : ((AIHeroClient)@base.GetBuff("").Caster).GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Kassadin":
                        AddPassiveAttack(
                            "Kassadin",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.W).Level > 0,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W, hero.HasBuff("NetherBlade") ? DamageStage.Empowered : DamageStage.Default),
                            true);
                        break;
                    case "Kayle":
                        AddPassiveAttack(
                            "Kayle",
                            (hero, @base) => !hero.HasBuff("KayleE") && Math.Abs(hero.Crit - 1) < float.Epsilon,
                            DamageType.Physical,
                            (hero, @base) => hero.TotalAttackDamage * hero.GetCritMultiplier());
                        AddPassiveAttack(
                            "Kayle",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.E).Level > 0,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.Default),
                            true);
                        AddPassiveAttack(
                            "Kayle",
                            (hero, @base) => hero.HasBuff("KayleE"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.Empowered),
                            true);
                        break;
                    case "Kennen":
                        AddPassiveAttack(
                            "Kennen",
                            (hero, @base) => hero.HasBuff("kennendoublestrikelive"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Khazix":
                        AddPassiveAttack(
                            "Khazix",
                            (hero, @base) => hero.HasBuff("KhazixPDamage") && @base is AIHeroClient,
                            DamageType.Magical,
                            (hero, @base) => 14 + 8 * Math.Max(0, hero.Level - 1) + 0.4 * hero.FlatPhysicalDamageMod);
                        break;
                    case "Kled":
                        AddPassiveAttack(
                            "Kled",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon,
                            DamageType.Physical,
                            (hero, @base) => (Math.Abs(hero.AbilityResource) < float.Epsilon ? 0.8 : 1) * hero.GetCritMultiplier() * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "Kled",
                            (hero, @base) => hero.HasBuff("kledwactive") && hero.Spellbook.GetSpell(SpellSlot.W).Ammo == 1,
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "KogMaw":
                        AddPassiveAttack(
                            "KogMaw",
                            (hero, @base) => hero.HasBuff("KogMawBioArcaneBarrage"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Leona":
                        AddPassiveAttack(
                            string.Empty,
                            (hero, @base) => @base.HasBuff("LeonaSunlight") && @base.GetBuff("LeonaSunlight").Caster.NetworkId != hero.NetworkId,
                            DamageType.Magical,
                            (hero, @base) => 25 + 7 * Math.Max(0, hero.Level - 1));
                        AddPassiveAttack(
                            "Leona",
                            (hero, @base) => hero.HasBuff("LeonaShieldOfDaybreak"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        AddPassiveAttack(
                            "Leona",
                            (hero, @base) => hero.HasBuff("leonarattackbuff"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.R, DamageStage.Buff),
                            true);
                        break;
                    case "Lucian":
                        AddPassiveAttack(
                            "Lucian",
                            (hero, @base) => hero.HasBuff("LucianPassiveBuff"),
                            DamageType.Physical,
                            (hero, @base) =>
                            {
                                var isMinion = @base is AIMinionClient && @base.Team != GameObjectTeam.Neutral;
                                var dmg = hero.TotalAttackDamage * (isMinion ? 1 : (hero.Level >= 13 ? 0.6 : (hero.Level >= 7 ? 0.55 : 0.5)));
                                var crit = hero.GetCritMultiplier(true);
                                if (crit > 1)
                                    dmg += isMinion ? dmg * (crit - 1) : dmg * (crit - 1) * 0.75;
                                return dmg;
                            });
                        break;
                    case "Lux":
                        AddPassiveAttack(
                            "Lux",
                            (hero, @base) => @base.HasBuff("LuxIlluminatingFraulein"),
                            DamageType.Magical,
                            (hero, @base) => 20 + 10 * Math.Max(0, hero.Level - 1) + 0.2 * hero.TotalMagicalDamage);
                        break;
                    case "Malphite":
                        AddPassiveAttack(
                            "Malphite",
                            (hero, @base) => hero.HasBuff("MalphiteCleave"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "MasterYi":
                        AddPassiveAttack(
                            "MasterYi",
                            (hero, @base) => hero.HasBuff("doublestrike"),
                            DamageType.Physical,
                            (hero, @base) => (0.5 * hero.TotalAttackDamage) * hero.GetCritMultiplier(true));
                        AddPassiveAttack(
                            "MasterYi",
                            (hero, @base) => hero.HasBuff("wujustylesuperchargedvisual"),
                            DamageType.True,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            true);
                        break;
                    case "MissFortune":
                        AddPassiveAttack(
                            "MissFortune",
                            (hero, @base) => hero.EnemyID != @base.Index,
                            DamageType.Physical,
                            (hero, @base) =>
                            {
                                double dmg = hero.TotalAttackDamage;
                                if (hero.Level >= 13)
                                    dmg *= 1;
                                else if (hero.Level >= 11)
                                    dmg *= 0.9;
                                else if (hero.Level >= 9)
                                    dmg *= 0.8;
                                else if (hero.Level >= 7)
                                    dmg *= 0.7;
                                else if (hero.Level >= 4)
                                    dmg *= 0.6;
                                else
                                    dmg *= 0.5;
                                if (@base is AIMinionClient && @base.Team != GameObjectTeam.Neutral)
                                    dmg *= 0.5;
                                return dmg;
                            });
                        break;
                    case "MonkeyKing":
                        AddPassiveAttack(
                            "MonkeyKing",
                            (hero, @base) => hero.HasBuff("MonkeyKingDoubleAttack"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        break;
                    case "Mordekaiser":
                        AddPassiveAttack(
                            "Mordekaiser",
                            (hero, @base) => hero.Buffs.Any(b => b.Name.Contains("mordekaisermaceofspades")),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q, hero.HasBuff("mordekaisermaceofspades2") ? DamageStage.Empowered : DamageStage.Default),
                            true);
                        break;
                    case "Nami":
                        AddPassiveAttack(
                            string.Empty,
                            (hero, @base) => hero.HasBuff("NamiE"),
                            DamageType.Magical,
                            (hero, @base) => ((AIHeroClient)hero.GetBuff("").Caster).GetSpellDamage(@base, SpellSlot.E),
                            true);
                        break;
                    case "Nasus":
                        AddPassiveAttack(
                            "Nasus",
                            (hero, @base) => hero.HasBuff("NasusQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        break;
                    case "Nautilus":
                        AddPassiveAttack(
                            "Nautilus",
                            (hero, @base) => !@base.HasBuff("nautiluspassivecheck"),
                            DamageType.Physical,
                            (hero, @base) => 8 + 6 * Math.Max(0, hero.Level - 1));
                        AddPassiveAttack(
                            "Nautilus",
                            (hero, @base) => hero.HasBuff("nautiluspiercinggazeshield"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W) / (@base is AIHeroClient ? 1 : 2),
                            true);
                        break;
                    case "Neeko":
                        AddPassiveAttack(
                            "Neeko",
                            (hero, @base) => hero.HasBuff("neekowpassiveready"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Nidalee":
                        AddPassiveAttack(
                            "Nidalee",
                            (hero, @base) => hero.HasBuff("Takedown"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q, @base.HasBuff("NidaleePassiveHunted") ? DamageStage.Empowered : DamageStage.SecondForm),
                            true,
                            true);
                        break;
                    case "Nocturne":
                        AddPassiveAttack(
                            "Nocturne",
                            (hero, @base) => hero.HasBuff("nocturneumbrablades"),
                            DamageType.Physical,
                            (hero, @base) => 0.2 * hero.TotalAttackDamage);
                        break;
                    case "Orianna":
                        AddPassiveAttack(
                            "Orianna",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) =>
                            {
                                var cnt = hero.GetBuffCount("orianapowerdaggerdisplay");
                                var dmg = hero.Level >= 16 ? 50d : (hero.Level >= 13 ? 42d : (hero.Level >= 10 ? 34d : (hero.Level >= 7 ? 26d : (hero.Level >= 4 ? 18d : 10d))));
                                dmg += 0.15 * hero.TotalMagicalDamage;
                                dmg *= cnt > 0 ? (1 + 0.2 * Math.Min(2, cnt)) : 1;
                                return dmg;
                            });
                        break;
                    case "Ornn":
                        AddPassiveAttack(
                            "Ornn",
                            (hero, @base) => @base.HasBuff("OrnnVulnerableDebuff"),
                            DamageType.Magical,
                            (hero, @base) => (0.12 + (0.205 - 0.12) / 17 * Math.Max(0, hero.Level - 1)) * @base.MaxHealth);
                        break;
                    case "Pantheon":
                        AddPassiveAttack(
                            "Pantheon",
                            (hero, @base) => (@base.HealthPercent < 15 && hero.Spellbook.GetSpell(SpellSlot.E).Level > 0) || Math.Abs(hero.Crit - 1) < float.Epsilon,
                            DamageType.Physical,
                            (hero, @base) => hero.GetCritMultiplier() * hero.TotalAttackDamage);
                        break;
                    case "Poppy":
                        AddPassiveAttack(
                            "Poppy",
                            (hero, @base) => hero.HasBuff("poppypassivebuff"),
                            DamageType.Magical,
                            (hero, @base) => 20 + (double)(180 - 20) / 17 * Math.Max(0, hero.Level - 1));
                        break;
                    case "Quinn":
                        AddPassiveAttack(
                            "Quinn",
                            (hero, @base) => @base.HasBuff("QuinnW"),
                            DamageType.Physical,
                            (hero, @base) => 10 + 5 * Math.Max(0, hero.Level - 1) + (0.16 + 0.02 * Math.Max(0, hero.Level - 1)) * hero.TotalAttackDamage);
                        break;
                    case "Rammus":
                        AddPassiveAttack(
                            "Rammus",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) => (Math.Min(20, 8 + Math.Max(0, hero.Level - 1)) + 0.1 * hero.Armor) * (hero.HasBuff("DefensiveBallCurl") ? 1.5 : 1));
                        break;
                    case "RekSai":
                        AddPassiveAttack(
                            "RekSai",
                            (hero, @base) => hero.HasBuff("RekSaiQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        break;
                    case "Renekton":
                        AddPassiveAttack(
                            "Renekton",
                            (hero, @base) => hero.HasBuff("RenektonPreExecute"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W, hero.Mana > 50 ? DamageStage.Empowered : DamageStage.Default),
                            true,
                            true);
                        break;
                    case "Rengar":
                        AddPassiveAttack(
                            "Rengar",
                            (hero, @base) => hero.HasBuff("RengarQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        AddPassiveAttack(
                            "Rengar",
                            (hero, @base) => hero.HasBuff("RengarQEmp"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.Empowered),
                            true);
                        break;
                    case "Riven":
                        AddPassiveAttack(
                            "Riven",
                            (hero, @base) => hero.HasBuff("RivenPassiveAABoost"),
                            DamageType.Physical,
                            (hero, @base) =>
                            {
                                double dmg = hero.TotalAttackDamage;
                                if (hero.Level >= 18)
                                    dmg *= 0.5;
                                else if (hero.Level >= 15)
                                    dmg *= 0.45;
                                else if (hero.Level >= 12)
                                    dmg *= 0.4;
                                else if (hero.Level >= 9)
                                    dmg *= 0.35;
                                else if (hero.Level >= 6)
                                    dmg *= 0.3;
                                else
                                    dmg *= 0.25;
                                return dmg;
                            });
                        break;
                    case "Rumble":
                        AddPassiveAttack(
                            "Rumble",
                            (hero, @base) => hero.HasBuff("rumbleoverheat"),
                            DamageType.Magical,
                            (hero, @base) => 25 + 5 * Math.Max(0, hero.Level - 1) + 0.3 * hero.TotalMagicalDamage);
                        break;
                    case "Sejuani":
                        AddPassiveAttack(
                            "Sejuani",
                            (hero, @base) => @base.HasBuff("sejuanistun"),
                            DamageType.Magical,
                            (hero, @base) => Math.Min(
                                (@base is AIMinionClient
                                    && @base.Team == GameObjectTeam.Neutral
                                    && (@base as AIMinionClient).GetJungleType() == JungleType.Legendary)
                                ? 300 : float.MaxValue,
                                (hero.Level >= 14 ? 0.2 : (hero.Level >= 7 ? 0.15 : 0.1)) * @base.MaxHealth));
                        break;
                    case "Shaco":
                        AddPassiveAttack(
                            "Shaco",
                            (hero, @base) => hero.HasBuff("Deceive"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        AddPassiveAttack(
                            "Shaco",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon || (hero.IsFacing(@base) && !@base.IsFacing(hero)),
                            DamageType.Physical,
                            (hero, @base) =>
                            {
                                var dmg = 0d;
                                if (hero.IsFacing(@base) && !@base.IsFacing(hero))
                                {
                                    if (@base is AIMinionClient && @base.Team == GameObjectTeam.Neutral)
                                        dmg = hero.TotalAttackDamage + 0.4 * hero.TotalMagicalDamage;
                                    else
                                        dmg = Math.Max(0.3, hero.GetCritMultiplier(true) - 1) * hero.TotalAttackDamage + 0.4 * hero.TotalMagicalDamage;
                                }
                                else
                                    dmg = hero.GetCritMultiplier() * hero.TotalAttackDamage;
                                return dmg;
                            });
                        break;
                    case "Shen":
                        AddPassiveAttack(
                            "Shen",
                            (hero, @base) => hero.HasBuff("shenqbuffweak"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q, (@base is AIMinionClient && @base.Team == GameObjectTeam.Neutral) ? DamageStage.SecondForm : DamageStage.Default),
                            true);
                        AddPassiveAttack(
                            "Shen",
                            (hero, @base) => hero.HasBuff("shenqbuffstrong"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q, (@base is AIMinionClient && @base.Team == GameObjectTeam.Neutral) ? DamageStage.ThirdForm : DamageStage.Empowered),
                            true);
                        break;
                    case "Shyvana":
                        AddPassiveAttack(
                            "Shyvana",
                            (hero, @base) => hero.HasBuff("ShyvanaDoubleAttack") || hero.HasBuff("ShyvanaDoubleAttackDragon"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        AddPassiveAttack(
                            "Shyvana",
                            (hero, @base) => hero.HasBuff("ShyvanaImmolationAura") || hero.HasBuff("ShyvanaImmolateDragon"),
                            DamageType.Magical,
                            (hero, @base) => 0.25 * hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        AddPassiveAttack(
                            "Shyvana",
                            (hero, @base) => @base.HasBuff("ShyvanaFireballMissile"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.Detonation),
                            true);
                        break;
                    case "Sion":
                        AddPassiveAttack(
                            "Sion",
                            (hero, @base) => hero.HasBuff("sionpassivezombie"),
                            DamageType.Physical,
                            (hero, @base) => Math.Min(@base is AIMinionClient ? 75 : float.MaxValue, 0.1 * @base.MaxHealth));
                        break;
                    case "Skarner":
                        AddPassiveAttack(
                            "Skarner",
                            (hero, @base) => @base.HasBuff("skarnerpassivebuff"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.Detonation),
                            true);
                        break;
                    case "Sona":
                        AddPassiveAttack(
                            "Sona",
                            (hero, @base) => hero.HasBuff("sonapassiveattack"),
                            DamageType.Magical,
                            (hero, @base) => /*(hero.HasBuff("") ? 1.4 : 1) * */(new[] { 20, 30, 40, 50, 60, 70, 80, 90, 105, 120, 135, 150, 165, 180, 195, 210, 225, 240 }[Math.Min(17, Math.Max(0, hero.Level - 1))] + 0.2 * hero.TotalMagicalDamage));
                        AddPassiveAttack(
                            string.Empty,
                            (hero, @base) => hero.HasBuff("sonaqprocattacker"),
                            DamageType.Magical,
                            (hero, @base) => ((AIHeroClient)hero.GetBuff("").Caster).GetSpellDamage(@base, SpellSlot.Q, DamageStage.Detonation),
                            true);
                        break;
                    case "Sylas":
                        AddPassiveAttack(
                            "Sylas",
                            (hero, @base) => hero.HasBuff("SylasPassiveAttack"),
                            DamageType.Magical,
                            (hero, @base) => 5 + (double)(48 - 5) / 17 * Math.Max(0, hero.Level - 1) + 1 * hero.TotalAttackDamage + 0.2 * hero.TotalMagicalDamage,
                            false,
                            true);
                        break;
                    case "TahmKench":
                        AddPassiveAttack(
                            "TahmKench",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) => Math.Max(1, @base.GetBuffCount("tahmkenchpdebuffcounter")) * (hero.Level >= 13 ? 0.0175 : (hero.Level >= 7 ? 0.015 : 0.0125)) * hero.MaxHealth);
                        break;
                    case "Talon":
                        AddPassiveAttack(
                            "Talon",
                            (hero, @base) => @base.GetBuffCount("TalonPassiveStack") == 3,
                            DamageType.Physical,
                            (hero, @base) => 75 + 10 * Math.Max(0, hero.Level - 1) + 2 * hero.FlatPhysicalDamageMod);
                        break;
                    case "Taric":
                        AddPassiveAttack(
                            "Taric",
                            (hero, @base) => hero.HasBuff("TaricPassiveAttack"),
                            DamageType.Magical,
                            (hero, @base) => 25 + 4 * Math.Max(0, hero.Level - 1) + 0.15 * hero.BonusArmor);
                        break;
                    case "Teemo":
                        AddPassiveAttack(
                            "Teemo",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.E).Level > 0,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E)
                            + (hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.DamagePerSecond)
                               * (@base is AIHeroClient ? 4 : 1)),
                            true);
                        break;
                    case "Thresh":
                        break;
                    case "Tristana":
                        AddPassiveAttack(
                            "Tristana",
                            (hero, @base) => @base.GetBuffCount("tristanaecharge") == 3,
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E) + (hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.Buff) / 3 * 4),
                            true);
                        break;
                    case "Trundle":
                        AddPassiveAttack(
                            "Trundle",
                            (hero, @base) => hero.HasBuff("TrundleTrollSmash"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        break;
                    case "TwistedFate":
                        AddPassiveAttack(
                            "TwistedFate",
                            (hero, @base) => hero.HasBuff("BlueCardPreAttack"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true,
                            true);
                        AddPassiveAttack(
                            "TwistedFate",
                            (hero, @base) => hero.HasBuff("RedCardPreAttack"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W, DamageStage.Detonation),
                            true,
                            true);
                        AddPassiveAttack(
                            "TwistedFate",
                            (hero, @base) => hero.HasBuff("GoldCardPreAttack"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W, DamageStage.Empowered),
                            true,
                            true);
                        AddPassiveAttack(
                            "TwistedFate",
                            (hero, @base) => hero.HasBuff("cardmasterstackparticle"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.E),
                            true);
                        break;
                    case "Twitch":
                        AddPassiveAttack(
                            "Twitch",
                            (hero, @base) => true,
                            DamageType.True,
                            (hero, @base) => (hero.Level >= 17 ? 5 : (hero.Level >= 13 ? 4 : (hero.Level >= 9 ? 3 : (hero.Level >= 5 ? 2 : 1))))
                            * Math.Min(Math.Max(@base.GetBuffCount("TwitchDeadlyVenom"), 0) + 1, 6)
                            * (@base is AIHeroClient ? 6 : 1));
                        break;
                    case "Udyr":
                        AddPassiveAttack(
                            "Udyr",
                            (hero, @base) => hero.GetBuffCount("UdyrTigerStance") == 3,
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        break;
                    case "Urgot":
                        AddPassiveAttack(
                            "Urgot",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon && !hero.HasBuff("urgotwshield"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetCritMultiplier() * hero.TotalAttackDamage);
                        break;
                    case "Varus":
                        AddPassiveAttack(
                            "Varus",
                            (hero, @base) => hero.Spellbook.GetSpell(SpellSlot.W).Level > 0,
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Vayne":
                        AddPassiveAttack(
                            "Vayne",
                            (hero, @base) => hero.HasBuff("vaynetumblebonus"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        AddPassiveAttack(
                            "Vayne",
                            (hero, @base) => @base.GetBuffCount("VayneSilveredDebuff") == 2,
                            DamageType.True,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        break;
                    case "Vi":
                        AddPassiveAttack(
                            "Vi",
                            (hero, @base) => @base.GetBuffCount("viwproc") == 2,
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.W),
                            true);
                        AddPassiveAttack(
                            "Vi",
                            (hero, @base) => hero.HasBuff("ViE"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetCritMultiplier(true) * hero.GetSpellDamage(@base, SpellSlot.E),
                            true);
                        break;
                    case "Viktor":
                        AddPassiveAttack(
                            "Viktor",
                            (hero, @base) => hero.HasBuff("ViktorPowerTransferReturn"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q, DamageStage.Empowered),
                            true,
                            true);
                        break;
                    case "Volibear":
                        AddPassiveAttack(
                            "Volibear",
                            (hero, @base) => hero.HasBuff("VolibearQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        AddPassiveAttack(
                            "Volibear",
                            (hero, @base) => hero.HasBuff("volibearrapplicator"),
                            DamageType.Magical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.R),
                            true);
                        break;
                    case "Warwick":
                        AddPassiveAttack(
                            "Warwick",
                            (hero, @base) => true,
                            DamageType.Magical,
                            (hero, @base) => 10 + 2 * Math.Max(0, hero.Level - 1));
                        break;
                    case "Xayah":
                        AddPassiveAttack(
                            string.Empty,
                            (hero, @base) => hero.HasBuff("XayahW") && (hero.Hero == Champion.Xayah || hero.Hero == Champion.Rakan),
                            DamageType.Physical,
                            (hero, @base) => 0.2 * hero.TotalAttackDamage);
                        break;
                    case "XinZhao":
                        AddPassiveAttack(
                            "XinZhao",
                            (hero, @base) => hero.GetBuffCount("XinZhaoPTracker") == 3,
                            DamageType.Physical,
                            (hero, @base) => (hero.Level >= 16 ? 0.45 : (hero.Level >= 11 ? 0.35 : (hero.Level >= 6 ? 0.25 : 0.15))) * hero.TotalAttackDamage);
                        AddPassiveAttack(
                            "XinZhao",
                            (hero, @base) => hero.HasBuff("XinZhaoQ"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        break;
                    case "Yasuo":
                        AddPassiveAttack(
                            "Yasuo",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon,
                            DamageType.Physical,
                            (hero, @base) => 0.9 * hero.GetCritMultiplier() * hero.TotalAttackDamage);
                        break;
                    case "Yorick":
                        AddPassiveAttack(
                            "Yorick",
                            (hero, @base) => hero.HasBuff("yorickqbuff"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetSpellDamage(@base, SpellSlot.Q),
                            true);
                        break;
                    case "Zac":
                        AddPassiveAttack(
                            "Zac",
                            (hero, @base) => Math.Abs(hero.Crit - 1) < float.Epsilon && !hero.HasBuff("zacqempowered"),
                            DamageType.Physical,
                            (hero, @base) => hero.GetCritMultiplier() * hero.TotalAttackDamage);
                        break;
                    case "Zed":
                        AddPassiveAttack(
                            "Zed",
                            (hero, @base) => @base.HealthPercent < 50 && !@base.HasBuff("zedpassivecd"),
                            DamageType.Magical,
                            (hero, @base) => hero.Level >= 17 ? 0.1 : (hero.Level >= 7 ? 0.08 : 0.06) * @base.MaxHealth);
                        break;
                    case "Ziggs":
                        AddPassiveAttack(
                            "Ziggs",
                            (hero, @base) => hero.HasBuff("ZiggsShortFuse"),
                            DamageType.Magical,
                            (hero, @base) =>
                                ((hero.Level >= 13 ? 0.5 : (hero.Level >= 7 ? 0.4 : 0.3)) * hero.TotalMagicalDamage
                                + new[] { 20, 24, 28, 32, 36, 40, 48, 56, 64, 72, 80, 88, 100, 112, 124, 136, 148, 160 }[Math.Min(17, Math.Max(0, hero.Level - 1))])
                                * (@base is AITurretClient ? 2 : 1));
                        break;
                    case "Zoe":
                        AddPassiveAttack(
                            "Zoe",
                            (hero, @base) => hero.HasBuff("zoepassivesheenbuff"),
                            DamageType.Magical,
                            (hero, @base) =>
                            new[] { 10, 12, 16, 20, 24, 28, 34, 40, 46, 52, 60, 68, 76, 84, 94, 104, 114, 124 }[Math.Min(17, Math.Max(0, hero.Level - 1))]
                            + 0.2 * hero.TotalMagicalDamage);
                        AddPassiveAttack(
                            "Zoe",
                            (hero, @base) => @base.HasBuff("zoeesleepstun"),
                            DamageType.True,
                            (hero, @base) => Math.Min(hero.GetSpellDamage(@base, SpellSlot.E, DamageStage.Empowered), hero.CalculatePhysicalDamage(@base, hero.TotalAttackDamage)),
                            true);
                        break;
                }
            }
        }

        private static float GetCritMultiplier(this AIHeroClient hero, bool checkCrit = false)
        {
            var crit = Items.HasItem((int)ItemId.Infinity_Edge, hero) || Items.HasItem((int)ItemId.Molten_Edge, hero) ? 1.25f : 1;
            return !checkCrit ? crit : (Math.Abs(hero.Crit - 1) < float.Epsilon ? 1 + crit : 1);
        }

        /// <summary>
        ///     Gets the passive raw damage summary.
        /// </summary>
        /// <param name="source">
        ///     The source
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="getOverride">
        ///     Get override damage.
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        private static PassiveDamageInfo GetPassiveDamageInfo(
            this AIHeroClient source,
            AIBaseClient target,
            bool getOverride = true)
        {
            var @double = 0d;
            var @override = false;
            List<PassiveDamage> value;

            // If you don't understand the code below, you should be flipping burgers instead.
            if (PassiveDamages.TryGetValue(string.Empty, out value))
            {
                // This should fix something that should never happen.
                value =
                    value.Where(
                        i =>
                        (i.Condition?.Invoke(source, target)).GetValueOrDefault() && i.Func != null
                        && (getOverride || !i.Override)).ToList();
                @double =
                    value.Sum(
                        item =>
                        item.IgnoreCalculation
                            ? item.Func(source, target)
                            : source.CalculateDamage(target, item.DamageType, item.Func(source, target)));
                if (getOverride)
                {
                    @override = value.Any(i => i.Override);
                }
            }

            if (PassiveDamages.TryGetValue(source.CharacterName, out value))
            {
                value =
                    value.Where(
                        i =>
                        (i.Condition?.Invoke(source, target)).GetValueOrDefault() && i.Func != null
                        && (getOverride || !i.Override)).ToList();
                @double +=
                    value.Sum(
                        item =>
                        item.IgnoreCalculation
                            ? item.Func(source, target)
                            : source.CalculateDamage(target, item.DamageType, item.Func(source, target)));
                if (getOverride && !@override)
                {
                    @override = value.Any(i => i.Override);
                }
            }

            return new PassiveDamageInfo { Value = @double, Override = @override };
        }

        #endregion

        private struct PassiveDamage
        {
            #region Public Properties

            public Func<AIHeroClient, AIBaseClient, bool> Condition { get; set; }

            public DamageType DamageType { get; set; }

            public Func<AIHeroClient, AIBaseClient, double> Func { get; set; }

            public bool IgnoreCalculation { get; set; }

            public bool Override { get; set; }

            #endregion
        }

        private struct PassiveDamageInfo
        {
            #region Public Properties

            public bool Override { get; set; }

            public double Value { get; set; }

            #endregion
        }
    }
}