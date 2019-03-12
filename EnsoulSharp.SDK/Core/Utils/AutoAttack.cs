// <copyright file="AutoAttack.cs" company="EnsoulSharp">
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

namespace EnsoulSharp.SDK.Core.Utils
{
    using System;
    using System.Linq;

    /// <summary>
    ///     AutoAttack utility class.
    /// </summary>
    public static class AutoAttack
    {
        #region Static Fields

        /// <summary>
        ///     Spells which reset the attack timer.
        /// </summary>
        private static readonly string[] AttackResets =
            {
                "powerfist","camilleq","camilleq2","vorpalspikes",
                "dariusnoxiantacticsonh","masochism","ekkoe","fiorae",
                "fizzw","garenq","gravesmove","hecarimramp",
                "illaoiw","jaxempowertwo","jaycehypercharge","netherblade",
                "kaylee","kindredq","leonashieldofdaybreak","luciane",
                "meditate","monkeykingdoubleattack","mordekaisermaceofspades","nasusq",
                "nautiluspiercinggaze","takedown","reksaiq","renektonpreexecute",
                "rengarq","riventricleave","shyvanadoubleattack","shyvanadoubleattackdragon",
                "sivirw","talonqattack","trundletrollsmash","vaynetumble",
                "vie","volibearq","xinzhaoq","yorickq",
                "itemtitanichydracleave"
            };

        /// <summary>
        ///     Spells that are attacks even if they don't have the "attack" word in their name.
        /// </summary>
        private static readonly string[] Attacks =
            {
                "caitlynheadshotmissile","kennenmegaproc","masteryidoublestrike",
                "quinnwenhanced","renektonexecute","renektonsuperexecute",
                "trundleq","viktorqbuff",
                "xinzhaoqthrust1","xinzhaoqthrust2","xinzhaoqthrust3"
            };

        /// <summary>
        ///     Spells that are not attacks even if they have the "attack" word in their name.
        /// </summary>
        private static readonly string[] NoAttacks =
            {
                "annietibbersbasicattack","annietibbersbasicattack2",
                "asheqattacknoonhit","volleyattackwithsound","volleyattack",
                "azirbasicattacksoldier",
                "dravenattackp_r","dravenattackp_l","dravenattackp_rc","dravenattackp_rq","dravenattackp_lc","dravenattackp_lq",
                "elisespiderlingbasicattack",
                "gravesbasicattackspread","gravesautoattackrecoil",
                "heimertyellowbasicattack","heimertyellowbasicattack2","heimertbluebasicattack","heimerdingerwattack2","heimerdingerwattack2ult",
                "ivernminionbasicattack","ivernminionbasicattack2",
                "kindredwolfbasicattack",
                "malzaharvoidlingbasicattack","malzaharvoidlingbasicattack2","malzaharvoidlingbasicattack3",
                "monkeykingdoubleattack",
                "shyvanadoubleattack","shyvanadoubleattackdragon",
                "talonqattack","talonqdashattack",
                "redcardattack","bluecardattack","goldcardattack",
                "yorickghoulmeleebasicattack","yorickghoulmeleebasicattack2","yorickghoulmeleebasicattack3","yorickbigghoulbasicattack",
                "zoebasicattackspecial1","zoebasicattackspecial2","zoebasicattackspecial3","zoebasicattackspecial4",
                "zyraeplantattack"
            };

        /// <summary>
        ///     Champions which can't cancel AA.
        /// </summary>
        private static readonly string[] NoCancelChamps = { "Kalista" };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns if the hero can't cancel an AA
        /// </summary>
        /// <param name="hero">The Hero (<see cref="AIHeroClient" />)</param>
        /// <returns>Returns if the hero can't cancel his AA</returns>
        public static bool CanCancelAutoAttack(this AIHeroClient hero)
        {
            return !NoCancelChamps.Contains(hero.CharacterName);
        }

        /// <summary>
        ///     Returns player auto-attack missile speed.
        /// </summary>
        /// <param name="hero">
        ///     The hero.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float GetProjectileSpeed(this AIHeroClient hero)
        {
            var name = hero.CharacterName;
            return hero.IsMelee() || name == "Azir" || name == "Velkoz" || name == "Thresh"
                   || (name == "Viktor" && hero.HasBuff("ViktorPowerTransferReturn"))
                       ? float.MaxValue
                       : hero.BasicAttack.MissileSpeed;
        }

        /// <summary>
        ///     Returns the auto-attack range.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float GetRealAutoAttackRange(this AttackableUnit target)
        {
            return GetRealAutoAttackRange(GameObjects.Player, target.Compare(GameObjects.Player) ? null : target);
        }

        /// <summary>
        ///     Returns the auto-attack range.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float GetRealAutoAttackRange(this AIBaseClient sender, AttackableUnit target)
        {
            if (sender == null)
            {
                return 0;
            }
            var result = sender.AttackRange + sender.BoundingRadius + (target?.BoundingRadius ?? 0);
            var heroSource = sender as AIHeroClient;
            if (heroSource != null && heroSource.CharacterName == "Caitlyn")
            {
                var aiBaseTarget = target as AIBaseClient;
                if (aiBaseTarget != null && aiBaseTarget.HasBuff("caitlynyordletrapinternal"))
                {
                    result += 650;
                }
            }
            return result;
        }

        /// <summary>
        ///     Returns the time it takes to hit a target with an auto attack
        /// </summary>
        /// <param name="target"><see cref="AttackableUnit" /> target</param>
        /// <returns>The <see cref="float" /></returns>
        public static float GetTimeToHit(this AttackableUnit target)
        {
            var time = (GameObjects.Player.AttackCastDelay * 1000) - 100 + (Game.Ping / 2f);
            if (Math.Abs(GameObjects.Player.GetProjectileSpeed() - float.MaxValue) > float.Epsilon)
            {
                var aiBaseTarget = target as AIBaseClient;
                time += 1000
                        * Math.Max(
                            0,
                            GameObjects.Player.Distance(aiBaseTarget?.Position ?? target.Position)
                            - GameObjects.Player.BoundingRadius) / GameObjects.Player.BasicAttack.MissileSpeed;
            }
            return time;
        }

        /// <summary>
        ///     Returns true if the target is in auto-attack range.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool InAutoAttackRange(this AttackableUnit target)
        {
            return target.IsValidTarget(target.GetRealAutoAttackRange());
        }

        /// <summary>
        ///     Returns if the name is an auto attack
        /// </summary>
        /// <param name="name">Name of spell</param>
        /// <returns>The <see cref="bool" /></returns>
        public static bool IsAutoAttack(string name)
        {
            name = name.ToLower();
            return (name.Contains("attack") && !NoAttacks.Contains(name)) || Attacks.Contains(name);
        }

        /// <summary>
        ///     Returns true if the SpellName resets the attack timer.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        #endregion
    }
}