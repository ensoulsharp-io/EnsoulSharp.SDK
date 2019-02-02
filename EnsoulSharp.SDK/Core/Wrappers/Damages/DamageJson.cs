// <copyright file="DamageJson.cs" company="EnsoulSharp">
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
    using System.Collections.Generic;

    /// <summary>
    ///     Damage wrapper class, contains functions to calculate estimated damage to a unit and also provides damage details.
    /// </summary>
    public static partial class Damage
    {
        #region Enums

        /// <summary>
        ///     The Damage Scaling Target Type enumeration.
        /// </summary>
        public enum DamageScalingTarget
        {
            /// <summary>
            ///     The Source target type.
            /// </summary>
            Source,

            /// <summary>
            ///     The Target target type.
            /// </summary>
            Target
        }

        /// <summary>
        ///     The Damage Scaling Type enumeration.
        /// </summary>
        public enum DamageScalingType
        {
            /// <summary>
            ///     The Attack Points scaling type.
            /// </summary>
            AttackPoints,

            /// <summary>
            ///     The Bonus Attack Points scaling type.
            /// </summary>
            BonusAttackPoints,

            /// <summary>
            ///     The Ability Points scaling type.
            /// </summary>
            AbilityPoints,

            /// <summary>
            ///     The Bonus Health scaling type.
            /// </summary>
            BonusHealth,

            /// <summary>
            ///     The Current Health scaling type.
            /// </summary>
            CurrentHealth,

            /// <summary>
            ///     The Max Health scaling type.
            /// </summary>
            MaxHealth,

            /// <summary>
            ///     The Missing Health scaling type.
            /// </summary>
            MissingHealth,

            /// <summary>
            ///     The Bonus Mana scaling type.
            /// </summary>
            BonusMana,

            /// <summary>
            ///     The Max Mana scaling type.
            /// </summary>
            MaxMana,

            /// <summary>
            ///     The Armor scaling type.
            /// </summary>
            Armor,

            /// <summary>
            ///     The Bonus Armor scaling type.
            /// </summary>
            BonusArmor,

            /// <summary>
            ///     The Spell Block scaling type.
            /// </summary>
            SpellBlock,

            /// <summary>
            ///     The Bonus Spell Block scaling type.
            /// </summary>
            BonusSpellBlock,

            /// <summary>
            ///     The Physical Lethality scaling type.
            /// </summary>
            PhysicalLethality
        }

        /// <summary>
        ///     The Damage Stage enumeration.
        /// </summary>
        public enum DamageStage
        {
            /// <summary>
            ///     The Default stage.
            /// </summary>
            Default,

            /// <summary>
            ///     The WayBack stage.
            /// </summary>
            WayBack,

            /// <summary>
            ///     The Detonation stage.
            /// </summary>
            Detonation,

            /// <summary>
            ///     The Damage Per Tick stage.
            /// </summary>
            DamagePerTick,

            /// <summary>
            ///     The Damage Per Time stage.
            /// </summary>
            DamagePerTime,

            /// <summary>
            ///     The Damage Per Half Second stage.
            /// </summary>
            DamagePerHalfSecond,

            /// <summary>
            ///     The Damage Per Quarter Second stage.
            /// </summary>
            DamagePerQuarterSecond,

            /// <summary>
            ///     The Damage Per Second stage.
            /// </summary>
            DamagePerSecond,

            /// <summary>
            ///     The Single Total stage.
            /// </summary>
            SingleTotal,

            /// <summary>
            ///     The Second Form stage.
            /// </summary>
            SecondForm,

            /// <summary>
            ///     The Third Form stage.
            /// </summary>
            ThirdForm,

            /// <summary>
            ///     The Second Cast stage.
            /// </summary>
            SecondCast,

            /// <summary>
            ///     The Third Cast stage.
            /// </summary>
            ThirdCast,

            /// <summary>
            ///     The Buff stage.
            /// </summary>
            Buff,

            /// <summary>
            ///     The Empowered stage.
            /// </summary>
            Empowered,

            /// <summary>
            ///     The Empowered Damage Per Second stage.
            /// </summary>
            EmpoweredDamagePerSecond,

            /// <summary>
            ///     The Empowered Damage Per Half Second stage.
            /// </summary>
            EmpoweredDamagePerHalfSecond,

            /// <summary>
            ///     The Empowered Damage Per Quarter Second stage.
            /// </summary>
            EmpoweredDamagePerQuarterSecond
        }

        /// <summary>
        /// </summary>
        public enum SpellEffectType
        {
            /// <summary>
            /// </summary>
            None,

            /// <summary>
            /// </summary>
            AoE,

            /// <summary>
            /// </summary>
            Single,

            /// <summary>
            /// </summary>
            OverTime,

            /// <summary>
            /// </summary>
            Attack
        }

        #endregion

        /// <summary>
        ///     The Champion Damage class container.
        /// </summary>
        internal class ChampionDamage
        {
            #region Public Properties

            /// <summary>
            ///     Gets the 'E' spell damage classes.
            /// </summary>
            public List<ChampionDamageSpell> E { get; set; }

            /// <summary>
            ///     Gets the 'Q' spell damage classes.
            /// </summary>
            public List<ChampionDamageSpell> Q { get; set; }

            /// <summary>
            ///     Gets the 'R' spell damage classes.
            /// </summary>
            public List<ChampionDamageSpell> R { get; set; }

            /// <summary>
            ///     Gets the 'W' spell damage classes.
            /// </summary>
            public List<ChampionDamageSpell> W { get; set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     Resolves the spell damage classes entry through the SpellSlot component.
            /// </summary>
            /// <param name="slot">
            ///     The SpellSlot.
            /// </param>
            /// <returns>
            ///     The spell damage classes of the requested Spell Slot.
            /// </returns>
            public IEnumerable<ChampionDamageSpell> GetSlot(SpellSlot slot)
            {
                switch (slot)
                {
                    case SpellSlot.Q:
                        return this.Q;
                    case SpellSlot.W:
                        return this.W;
                    case SpellSlot.E:
                        return this.E;
                    case SpellSlot.R:
                        return this.R;
                }

                return null;
            }

            #endregion
        }

        /// <summary>
        ///     The Champion Damage Spell class container.
        /// </summary>
        internal class ChampionDamageSpell
        {
            #region Public Properties

            /// <summary>
            ///     Gets the Spell Data.
            /// </summary>
            public ChampionDamageSpellData SpellData { get; set; }

            /// <summary>
            ///     Gets the Spell Stage.
            /// </summary>
            public DamageStage Stage { get; set; }

            #endregion
        }

        /// <summary>
        ///     The Champion Damage Spell Bonus On Monster class container.
        /// </summary>
        internal class ChampionDamageSpellOnMonster
        {
            #region Public Properties

            /// <summary>
            ///     Gets the Damage Percentages.
            /// </summary>
            public List<double> DamagePercentages { get; set; }

            /// <summary>
            ///     Gets the Damage Type.
            /// </summary>
            public DamageType DamageType { get; set; }

            /// <summary>
            ///     Gets the Scaling Target Type.
            /// </summary>
            public DamageScalingTarget ScalingTarget { get; set; }

            /// <summary>
            ///     Gets the Scaling Type.
            /// </summary>
            public DamageScalingType ScalingType { get; set; }

            #endregion
        }

        /// <summary>
        ///     The Champion Damage Spell Damage Limit class container.
        /// </summary>
        internal class ChampionDamageSpellDamageLimit
        {
            #region Public Properties

            public List<double> Damages { get; set; }

            public List<double> DamagePercentages { get; set; }

            public DamageScalingTarget ScalingTarget { get; set; }

            public DamageScalingType ScalingType { get; set; }

            #endregion
        }

        /// <summary>
        ///     The Champion Damage Spell Bonus class container.
        /// </summary>
        internal class ChampionDamageSpellBonus
        {
            #region Public Properties

            public List<double> BonusDamageOnMinion { get; set; }

            public List<double> BonusDamageOnMonster { get; set; }

            /// <summary>
            ///     Gets the Damage Percentages.
            /// </summary>
            public List<double> DamagePercentages { get; set; }

            /// <summary>
            ///     Gets the Damage Type.
            /// </summary>
            public DamageType DamageType { get; set; }

            public List<int> MaxDamageOnMinion { get; set; }

            public List<int> MaxDamageOnMonster { get; set; }

            public ChampionDamageSpellDamageLimit MinDamage { get; set; }

            public double ScalePer100Ad { get; set; }

            public double ScalePer100Ap { get; set; }

            public double ScalePer100BonusAd { get; set; }

            public double PercentagePerBuffStack { get; set; }

            public string PercentageStackBuff { get; set; }

            public string PercentageCheckBuff { get; set; }

            public string BonusBuff { get; set; }

            public int BonusBuffOffset { get; set; }

            /// <summary>
            ///     Gets the Scaling Buff.
            /// </summary>
            public string ScalingBuff { get; set; }

            /// <summary>
            ///     Gets the Scaling Buff Offset.
            /// </summary>
            public int ScalingBuffOffset { get; set; }

            /// <summary>
            ///     Gets the Scaling Buff Target.
            /// </summary>
            public DamageScalingTarget ScalingBuffTarget { get; set; }

            /// <summary>
            ///     Gets the Scaling Target Type.
            /// </summary>
            public DamageScalingTarget ScalingTarget { get; set; }

            /// <summary>
            ///     Gets the Scaling Type.
            /// </summary>
            public DamageScalingType ScalingType { get; set; }

            #endregion
        }

        /// <summary>
        ///     The Champion Damage Spell Data class container.
        /// </summary>
        internal class ChampionDamageSpellData
        {
            #region Public Properties

            public List<double> BonusDamageOnMinion { get; set; }

            public List<double> BonusDamageOnMonster { get; set; }

            public List<double> BonusDamageOnSoldier { get; set; }

            /// <summary>
            ///     Gets the Bonus Damages.
            /// </summary>
            public List<ChampionDamageSpellBonus> BonusDamages { get; set; }

            /// <summary>
            ///     Gets the Damages On Monster.
            /// </summary>
            public List<ChampionDamageSpellOnMonster> DamagesOnMonster { get; set; }

            /// <summary>
            ///     Gets the Main Damages.
            /// </summary>
            public List<double> Damages { get; set; }

            public List<double> DamagesPerLvl { get; set; }

            public List<double> DamagesReductionOnSoldier { get; set; }

            public List<double> DamagesReductionPerLvlOnSoldier { get; set; }

            /// <summary>
            ///     Gets the Damage Type.
            /// </summary>
            public DamageType DamageType { get; set; }

            public bool IsApplyOnHit { get; set; }

            public bool IsModifiedDamage { get; set; }

            public List<int> MaxDamageOnMinion { get; set; }

            public List<int> MaxDamageOnMonster { get; set; }

            public List<int> MinDamageOnSoldier { get; set; }

            public double MaxLevelScalingValueOnMinion { get; set; }

            public double MaxScaleTargetMissHealth { get; set; }

            public List<double> ScalePerTargetMissHealth { get; set; }

            public double ScalePerCritChance { get; set; }

            public double ScalingValueOnSoldier { get; set; }

            /// <summary>
            ///     Gets the Scaling Slot.
            /// </summary>
            public SpellSlot ScaleSlot { get; set; } = SpellSlot.Unknown;

            /// <summary>
            ///     Gets the Scaling Buff.
            /// </summary>
            public string ScalingBuff { get; set; }

            /// <summary>
            ///     Gets the Scaling Buff Offset.
            /// </summary>
            public int ScalingBuffOffset { get; set; }

            /// <summary>
            ///     Gets the Scaling Buff Target.
            /// </summary>
            public DamageScalingTarget ScalingBuffTarget { get; set; }

            public SpellEffectType SpellEffectType { get; set; }

            #endregion
        }
    }
}