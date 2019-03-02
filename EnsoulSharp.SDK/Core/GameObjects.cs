// <copyright file="GameObjects.cs" company="EnsoulSharp">
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

    /// <summary>
    ///     A static (stack) class which contains a sort-of cached versions of the important game objects.
    /// </summary>
    public static class GameObjects
    {
        #region Static Fields

        /// <summary>
        ///     The ally heroes list.
        /// </summary>
        private static readonly List<AIHeroClient> AllyHeroesList = new List<AIHeroClient>();

        /// <summary>
        ///     The ally inhibitors list.
        /// </summary>
        private static readonly List<BarracksDampenerClient> AllyInhibitorsList = new List<BarracksDampenerClient>();

        /// <summary>
        ///     The ally list.
        /// </summary>
        private static readonly List<AIBaseClient> AllyList = new List<AIBaseClient>();

        /// <summary>
        ///     The ally minions list.
        /// </summary>
        private static readonly List<AIMinionClient> AllyMinionsList = new List<AIMinionClient>();

        /// <summary>
        ///     The ally shops list.
        /// </summary>
        private static readonly List<ShopClient> AllyShopsList = new List<ShopClient>();

        /// <summary>
        ///     The ally spawn points list.
        /// </summary>
        private static readonly List<Obj_SpawnPoint> AllySpawnPointsList = new List<Obj_SpawnPoint>();

        /// <summary>
        ///     The ally turrets list.
        /// </summary>
        private static readonly List<AITurretClient> AllyTurretsList = new List<AITurretClient>();

        /// <summary>
        ///     The ally wards list.
        /// </summary>
        private static readonly List<AIMinionClient> AllyWardsList = new List<AIMinionClient>();

        /// <summary>
        ///     The attackable unit list.
        /// </summary>
        private static readonly List<AttackableUnit> AttackableUnitsList = new List<AttackableUnit>();

        /// <summary>
        ///     The enemy heroes list.
        /// </summary>
        private static readonly List<AIHeroClient> EnemyHeroesList = new List<AIHeroClient>();

        /// <summary>
        ///     The enemy inhibitors list.
        /// </summary>
        private static readonly List<BarracksDampenerClient> EnemyInhibitorsList = new List<BarracksDampenerClient>();

        /// <summary>
        ///     The enemy list.
        /// </summary>
        private static readonly List<AIBaseClient> EnemyList = new List<AIBaseClient>();

        /// <summary>
        ///     The enemy minions list.
        /// </summary>
        private static readonly List<AIMinionClient> EnemyMinionsList = new List<AIMinionClient>();

        /// <summary>
        ///     The enemy shops list.
        /// </summary>
        private static readonly List<ShopClient> EnemyShopsList = new List<ShopClient>();

        /// <summary>
        ///     The enemy spawn points list.
        /// </summary>
        private static readonly List<Obj_SpawnPoint> EnemySpawnPointsList = new List<Obj_SpawnPoint>();

        /// <summary>
        ///     The enemy turrets list.
        /// </summary>
        private static readonly List<AITurretClient> EnemyTurretsList = new List<AITurretClient>();

        /// <summary>
        ///     The enemy wards list.
        /// </summary>
        private static readonly List<AIMinionClient> EnemyWardsList = new List<AIMinionClient>();

        /// <summary>
        ///     The game objects list.
        /// </summary>
        private static readonly List<GameObject> GameObjectsList = new List<GameObject>();

        /// <summary>
        ///     The heroes list.
        /// </summary>
        private static readonly List<AIHeroClient> HeroesList = new List<AIHeroClient>();

        /// <summary>
        ///     The inhibitors list.
        /// </summary>
        private static readonly List<BarracksDampenerClient> InhibitorsList = new List<BarracksDampenerClient>();

        /// <summary>
        ///     The jungle large list.
        /// </summary>
        private static readonly List<AIMinionClient> JungleLargeList = new List<AIMinionClient>();

        /// <summary>
        ///     The jungle legendary list.
        /// </summary>
        private static readonly List<AIMinionClient> JungleLegendaryList = new List<AIMinionClient>();

        /// <summary>
        ///     The jungle list.
        /// </summary>
        private static readonly List<AIMinionClient> JungleList = new List<AIMinionClient>();

        /// <summary>
        ///     The jungle small list.
        /// </summary>
        private static readonly List<AIMinionClient> JungleSmallList = new List<AIMinionClient>();

        /// <summary>
        ///     The minions list.
        /// </summary>
        private static readonly List<AIMinionClient> MinionsList = new List<AIMinionClient>();

        /// <summary>
        ///     The nexus list.
        /// </summary>
        private static readonly List<HQClient> NexusList = new List<HQClient>();

        /// <summary>
        ///     The general particle emitters list.
        /// </summary>
        private static readonly List<EffectEmitter> ParticleEmittersList =
            new List<EffectEmitter>();

        /// <summary>
        ///     The shops list.
        /// </summary>
        private static readonly List<ShopClient> ShopsList = new List<ShopClient>();

        /// <summary>
        ///     The spawn points list.
        /// </summary>
        private static readonly List<Obj_SpawnPoint> SpawnPointsList = new List<Obj_SpawnPoint>();

        /// <summary>
        ///     The turrets list.
        /// </summary>
        private static readonly List<AITurretClient> TurretsList = new List<AITurretClient>();

        /// <summary>
        ///     The wards list.
        /// </summary>
        private static readonly List<AIMinionClient> WardsList = new List<AIMinionClient>();

        /// <summary>
        ///     Indicates whether the <see cref="GameObjects" /> stack was initialized and saved required instances.
        /// </summary>
        private static bool initialized;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="GameObjects" /> class.
        /// </summary>
        static GameObjects()
        {
            Initialize();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the game objects.
        /// </summary>
        public static IEnumerable<GameObject> AllGameObjects => GameObjectsList;

        /// <summary>
        ///     Gets the ally.
        /// </summary>
        public static IEnumerable<AIBaseClient> Ally => AllyList;

        /// <summary>
        ///     Gets the ally heroes.
        /// </summary>
        public static IEnumerable<AIHeroClient> AllyHeroes => AllyHeroesList;

        /// <summary>
        ///     Gets the ally inhibitors.
        /// </summary>
        public static IEnumerable<BarracksDampenerClient> AllyInhibitors => AllyInhibitorsList;

        /// <summary>
        ///     Gets the ally minions.
        /// </summary>
        public static IEnumerable<AIMinionClient> AllyMinions => AllyMinionsList;

        /// <summary>
        ///     Gets or sets the ally nexus.
        /// </summary>
        public static HQClient AllyNexus { get; set; }

        /// <summary>
        ///     Gets the ally shops.
        /// </summary>
        public static IEnumerable<ShopClient> AllyShops => AllyShopsList;

        /// <summary>
        ///     Gets the ally spawn points.
        /// </summary>
        public static IEnumerable<Obj_SpawnPoint> AllySpawnPoints => AllySpawnPointsList;

        /// <summary>
        ///     Gets the ally turrets.
        /// </summary>
        public static IEnumerable<AITurretClient> AllyTurrets => AllyTurretsList;

        /// <summary>
        ///     Gets the ally wards.
        /// </summary>
        public static IEnumerable<AIMinionClient> AllyWards => AllyWardsList;

        /// <summary>
        ///     Gets the attackable units.
        /// </summary>
        public static IEnumerable<AttackableUnit> AttackableUnits => AttackableUnitsList;

        /// <summary>
        ///     Gets the enemy.
        /// </summary>
        public static IEnumerable<AIBaseClient> Enemy => EnemyList;

        /// <summary>
        ///     Gets the enemy heroes.
        /// </summary>
        public static IEnumerable<AIHeroClient> EnemyHeroes => EnemyHeroesList;

        /// <summary>
        ///     Gets the enemy inhibitors.
        /// </summary>
        public static IEnumerable<BarracksDampenerClient> EnemyInhibitors => EnemyInhibitorsList;

        /// <summary>
        ///     Gets the enemy minions.
        /// </summary>
        public static IEnumerable<AIMinionClient> EnemyMinions => EnemyMinionsList;

        /// <summary>
        ///     Gets or sets the enemy nexus.
        /// </summary>
        public static HQClient EnemyNexus { get; set; }

        /// <summary>
        ///     Gets the enemy shops.
        /// </summary>
        public static IEnumerable<ShopClient> EnemyShops => EnemyShopsList;

        /// <summary>
        ///     Gets the enemy spawn points.
        /// </summary>
        public static IEnumerable<Obj_SpawnPoint> EnemySpawnPoints => EnemySpawnPointsList;

        /// <summary>
        ///     Gets the enemy turrets.
        /// </summary>
        public static IEnumerable<AITurretClient> EnemyTurrets => EnemyTurretsList;

        /// <summary>
        ///     Gets the enemy wards.
        /// </summary>
        public static IEnumerable<AIMinionClient> EnemyWards => EnemyWardsList;

        /// <summary>
        ///     Gets the heroes.
        /// </summary>
        public static IEnumerable<AIHeroClient> Heroes => HeroesList;

        /// <summary>
        ///     Gets the inhibitors.
        /// </summary>
        public static IEnumerable<BarracksDampenerClient> Inhibitors => InhibitorsList;

        /// <summary>
        ///     Gets the jungle.
        /// </summary>
        public static IEnumerable<AIMinionClient> Jungle => JungleList;

        /// <summary>
        ///     Gets the jungle large.
        /// </summary>
        public static IEnumerable<AIMinionClient> JungleLarge => JungleLargeList;

        /// <summary>
        ///     Gets the jungle legendary.
        /// </summary>
        public static IEnumerable<AIMinionClient> JungleLegendary => JungleLegendaryList;

        /// <summary>
        ///     Gets the jungle small.
        /// </summary>
        public static IEnumerable<AIMinionClient> JungleSmall => JungleSmallList;

        /// <summary>
        ///     Gets the minions.
        /// </summary>
        public static IEnumerable<AIMinionClient> Minions => MinionsList;

        /// <summary>
        ///     Gets the nexuses.
        /// </summary>
        public static IEnumerable<HQClient> Nexuses => NexusList;

        /// <summary>
        ///     Gets the general particle emitters.
        /// </summary>
        public static IEnumerable<EffectEmitter> ParticleEmitters => ParticleEmittersList;

        /// <summary>
        ///     Gets or sets the player.
        /// </summary>
        public static AIHeroClient Player { get; set; }

        /// <summary>
        ///     Gets the shops.
        /// </summary>
        public static IEnumerable<ShopClient> Shops => ShopsList;

        /// <summary>
        ///     Gets the spawn points.
        /// </summary>
        public static IEnumerable<Obj_SpawnPoint> SpawnPoints => SpawnPointsList;

        /// <summary>
        ///     Gets the turrets.
        /// </summary>
        public static IEnumerable<AITurretClient> Turrets => TurretsList;

        /// <summary>
        ///     Gets the wards.
        /// </summary>
        public static IEnumerable<AIMinionClient> Wards => WardsList;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Compares two <see cref="GameObject" /> and returns if they are identical.
        /// </summary>
        /// <param name="gameObject">The GameObject</param>
        /// <param name="object">The Compare GameObject</param>
        /// <returns>Whether the <see cref="GameObject" />s are identical.</returns>
        public static bool Compare(this GameObject gameObject, GameObject @object)
        {
            return gameObject != null && gameObject.IsValid && @object != null && @object.IsValid
                   && gameObject.NetworkId == @object.NetworkId;
        }

        /// <summary>
        ///     The get operation from the GameObjects stack.
        /// </summary>
        /// <typeparam name="T">
        ///     The requested <see cref="GameObject" /> type.
        /// </typeparam>
        /// <returns>
        ///     The List containing the requested type.
        /// </returns>
        public static IEnumerable<T> Get<T>() where T : GameObject, new()
        {
            return AllGameObjects.OfType<T>();
        }

        /// <summary>
        ///     Get get operation from the native GameObjects stack.
        /// </summary>
        /// <typeparam name="T">
        ///     The requested <see cref="GameObject" /> type.
        /// </typeparam>
        /// <returns>
        ///     The List containing the requested type.
        /// </returns>
        public static IEnumerable<T> GetNative<T>() where T : GameObject, new()
        {
            return ObjectManager.Get<T>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The initialize method.
        /// </summary>
        internal static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            Events.OnLoad += (sender, args) =>
                {
                    Player = ObjectManager.Player;

                    HeroesList.AddRange(ObjectManager.Get<AIHeroClient>());
                    MinionsList.AddRange(
                        ObjectManager.Get<AIMinionClient>()
                            .Where(
                                o => o.Team != GameObjectTeam.Neutral && !o.GetMinionType().HasFlag(MinionTypes.Ward)));
                    TurretsList.AddRange(ObjectManager.Get<AITurretClient>());
                    InhibitorsList.AddRange(ObjectManager.Get<BarracksDampenerClient>());
                    JungleList.AddRange(
                        ObjectManager.Get<AIMinionClient>()
                            .Where(o => o.Team == GameObjectTeam.Neutral && o.Name != "WardCorpse"));
                    WardsList.AddRange(
                        ObjectManager.Get<AIMinionClient>().Where(o => o.GetMinionType().HasFlag(MinionTypes.Ward)));
                    ShopsList.AddRange(ObjectManager.Get<ShopClient>());
                    SpawnPointsList.AddRange(ObjectManager.Get<Obj_SpawnPoint>());
                    GameObjectsList.AddRange(ObjectManager.Get<GameObject>());
                    NexusList.AddRange(ObjectManager.Get<HQClient>());
                    AttackableUnitsList.AddRange(ObjectManager.Get<AttackableUnit>());
                    ParticleEmittersList.AddRange(ObjectManager.Get<EffectEmitter>());

                    EnemyHeroesList.AddRange(HeroesList.Where(o => o.IsEnemy));
                    EnemyMinionsList.AddRange(MinionsList.Where(o => o.IsEnemy));
                    EnemyTurretsList.AddRange(TurretsList.Where(o => o.IsEnemy));
                    EnemyInhibitorsList.AddRange(InhibitorsList.Where(o => o.IsEnemy));
                    EnemyList.AddRange(
                        EnemyHeroesList.Cast<AIBaseClient>().Concat(EnemyMinionsList).Concat(EnemyTurretsList));
                    EnemyNexus = NexusList.FirstOrDefault(n => n.IsEnemy);

                    AllyHeroesList.AddRange(HeroesList.Where(o => o.IsAlly));
                    AllyMinionsList.AddRange(MinionsList.Where(o => o.IsAlly));
                    AllyTurretsList.AddRange(TurretsList.Where(o => o.IsAlly));
                    AllyInhibitorsList.AddRange(InhibitorsList.Where(o => o.IsAlly));
                    AllyList.AddRange(
                        AllyHeroesList.Cast<AIBaseClient>().Concat(AllyMinionsList).Concat(AllyTurretsList));
                    AllyNexus = NexusList.FirstOrDefault(n => n.IsAlly);

                    JungleSmallList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Small));
                    JungleLargeList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Large));
                    JungleLegendaryList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Legendary));

                    AllyWardsList.AddRange(WardsList.Where(o => o.IsAlly));
                    EnemyWardsList.AddRange(WardsList.Where(o => o.IsEnemy));

                    AllyShopsList.AddRange(ShopsList.Where(o => o.IsAlly));
                    EnemyShopsList.AddRange(ShopsList.Where(o => o.IsEnemy));

                    AllySpawnPointsList.AddRange(SpawnPointsList.Where(o => o.IsAlly));
                    EnemySpawnPointsList.AddRange(SpawnPointsList.Where(o => o.IsEnemy));

                    GameObject.OnAssign += OnAssign;
                    GameObject.OnDelete += OnDelete;
                };
        }

        /// <summary>
        ///     OnAssign event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnAssign(GameObject sender, EventArgs args)
        {
            GameObjectsList.Add(sender);

            var attackableUnit = sender as AttackableUnit;
            if (attackableUnit != null)
            {
                AttackableUnitsList.Add(attackableUnit);
            }

            var hero = sender as AIHeroClient;
            if (hero != null)
            {
                HeroesList.Add(hero);
                if (hero.IsEnemy)
                {
                    EnemyHeroesList.Add(hero);
                    EnemyList.Add(hero);
                }
                else
                {
                    AllyHeroesList.Add(hero);
                    AllyList.Add(hero);
                }

                return;
            }

            var minion = sender as AIMinionClient;
            if (minion != null)
            {
                if (minion.Team != GameObjectTeam.Neutral)
                {
                    if (minion.GetMinionType().HasFlag(MinionTypes.Ward))
                    {
                        WardsList.Add(minion);
                        if (minion.IsEnemy)
                        {
                            EnemyWardsList.Add(minion);
                        }
                        else
                        {
                            AllyWardsList.Add(minion);
                        }
                    }
                    else
                    {
                        MinionsList.Add(minion);
                        if (minion.IsEnemy)
                        {
                            EnemyMinionsList.Add(minion);
                            EnemyList.Add(minion);
                        }
                        else
                        {
                            AllyMinionsList.Add(minion);
                            AllyList.Add(minion);
                        }
                    }
                }
                else if (minion.Name != "WardCorpse")
                {
                    switch (minion.GetJungleType())
                    {
                        case JungleType.Small:
                            JungleSmallList.Add(minion);
                            break;
                        case JungleType.Large:
                            JungleLargeList.Add(minion);
                            break;
                        case JungleType.Legendary:
                            JungleLegendaryList.Add(minion);
                            break;
                    }

                    JungleList.Add(minion);
                }

                return;
            }

            var particle = sender as EffectEmitter;
            if (particle != null)
            {
                ParticleEmittersList.Add(particle);
                return;
            }

            var turret = sender as AITurretClient;
            if (turret != null)
            {
                TurretsList.Add(turret);
                if (turret.IsEnemy)
                {
                    EnemyTurretsList.Add(turret);
                    EnemyList.Add(turret);
                }
                else
                {
                    AllyTurretsList.Add(turret);
                    AllyList.Add(turret);
                }

                return;
            }

            var shop = sender as ShopClient;
            if (shop != null)
            {
                ShopsList.Add(shop);
                if (shop.IsAlly)
                {
                    AllyShopsList.Add(shop);
                }
                else
                {
                    EnemyShopsList.Add(shop);
                }

                return;
            }

            var spawnPoint = sender as Obj_SpawnPoint;
            if (spawnPoint != null)
            {
                SpawnPointsList.Add(spawnPoint);
                if (spawnPoint.IsAlly)
                {
                    AllySpawnPointsList.Add(spawnPoint);
                }
                else
                {
                    EnemySpawnPointsList.Add(spawnPoint);
                }

                return;
            }

            var inhibitor = sender as BarracksDampenerClient;
            if (inhibitor != null)
            {
                InhibitorsList.Add(inhibitor);
                if (inhibitor.IsAlly)
                {
                    AllyInhibitorsList.Add(inhibitor);
                }
                else
                {
                    EnemyInhibitorsList.Add(inhibitor);
                }

                return;
            }

            var nexus = sender as HQClient;
            if (nexus != null)
            {
                NexusList.Add(nexus);
                if (nexus.IsAlly)
                {
                    AllyNexus = nexus;
                }
                else
                {
                    EnemyNexus = nexus;
                }

                return;
            }
        }

        /// <summary>
        ///     OnDelete event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void OnDelete(GameObject sender, EventArgs args)
        {
            foreach (var gameObject in GameObjectsList.Where(o => o.Compare(sender)).ToList())
            {
                GameObjectsList.Remove(gameObject);
            }

            foreach (var attackableUnitObject in AttackableUnitsList.Where(a => a.Compare(sender)).ToList())
            {
                AttackableUnitsList.Remove(attackableUnitObject);
            }

            var hero = sender as AIHeroClient;
            if (hero != null)
            {
                foreach (var heroObject in HeroesList.Where(h => h.Compare(hero)).ToList())
                {
                    HeroesList.Remove(heroObject);
                    if (hero.IsEnemy)
                    {
                        EnemyHeroesList.Remove(heroObject);
                        EnemyList.Remove(heroObject);
                    }
                    else
                    {
                        AllyHeroesList.Remove(heroObject);
                        AllyList.Remove(heroObject);
                    }
                }

                return;
            }

            var minion = sender as AIMinionClient;
            if (minion != null)
            {
                if (minion.Team != GameObjectTeam.Neutral)
                {
                    if (minion.GetMinionType().HasFlag(MinionTypes.Ward))
                    {
                        foreach (var ward in WardsList.Where(w => w.Compare(minion)).ToList())
                        {
                            WardsList.Remove(ward);
                            if (minion.IsEnemy)
                            {
                                EnemyWardsList.Remove(ward);
                            }
                            else
                            {
                                AllyWardsList.Remove(ward);
                            }
                        }
                    }
                    else
                    {
                        foreach (var minionObject in MinionsList.Where(m => m.Compare(minion)).ToList())
                        {
                            MinionsList.Remove(minionObject);
                            if (minion.IsEnemy)
                            {
                                EnemyMinionsList.Remove(minionObject);
                                EnemyList.Remove(minionObject);
                            }
                            else
                            {
                                AllyMinionsList.Remove(minionObject);
                                AllyList.Remove(minionObject);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var jungleObject in JungleList.Where(j => j.Compare(minion)).ToList())
                    {
                        switch (jungleObject.GetJungleType())
                        {
                            case JungleType.Small:
                                JungleSmallList.Remove(jungleObject);
                                break;
                            case JungleType.Large:
                                JungleLargeList.Remove(jungleObject);
                                break;
                            case JungleType.Legendary:
                                JungleLegendaryList.Remove(jungleObject);
                                break;
                        }

                        JungleList.Remove(jungleObject);
                    }
                }

                return;
            }

            var particle = sender as EffectEmitter;
            if (particle != null)
            {
                ParticleEmittersList.Remove(particle);
                return;
            }

            var turret = sender as AITurretClient;
            if (turret != null)
            {
                foreach (var turretObject in TurretsList.Where(t => t.Compare(turret)).ToList())
                {
                    TurretsList.Remove(turretObject);
                    if (turret.IsEnemy)
                    {
                        EnemyTurretsList.Remove(turretObject);
                        EnemyList.Remove(turretObject);
                    }
                    else
                    {
                        AllyTurretsList.Remove(turretObject);
                        AllyList.Remove(turretObject);
                    }
                }

                return;
            }

            var shop = sender as ShopClient;
            if (shop != null)
            {
                foreach (var shopObject in ShopsList.Where(s => s.Compare(shop)).ToList())
                {
                    ShopsList.Remove(shopObject);
                    if (shop.IsAlly)
                    {
                        AllyShopsList.Remove(shopObject);
                    }
                    else
                    {
                        EnemyShopsList.Remove(shopObject);
                    }
                }

                return;
            }

            var spawnPoint = sender as Obj_SpawnPoint;
            if (spawnPoint != null)
            {
                foreach (var spawnPointObject in SpawnPointsList.Where(s => s.Compare(spawnPoint)).ToList())
                {
                    SpawnPointsList.Remove(spawnPointObject);
                    if (spawnPoint.IsAlly)
                    {
                        AllySpawnPointsList.Remove(spawnPointObject);
                    }
                    else
                    {
                        EnemySpawnPointsList.Remove(spawnPointObject);
                    }
                }

                return;
            }

            var inhibitor = sender as BarracksDampenerClient;
            if (inhibitor != null)
            {
                foreach (var inhibitorObject in InhibitorsList.Where(i => i.Compare(inhibitor)).ToList())
                {
                    InhibitorsList.Remove(inhibitorObject);
                    if (inhibitor.IsAlly)
                    {
                        AllyInhibitorsList.Remove(inhibitorObject);
                    }
                    else
                    {
                        EnemyInhibitorsList.Remove(inhibitorObject);
                    }
                }

                return;
            }

            var nexus = sender as HQClient;
            if (nexus != null)
            {
                foreach (var nexusObject in NexusList.Where(n => n.Compare(nexus)).ToList())
                {
                    NexusList.Remove(nexusObject);
                    if (nexusObject.IsAlly)
                    {
                        AllyNexus = null;
                    }
                    else
                    {
                        EnemyNexus = null;
                    }
                }

                return;
            }
        }

        #endregion
    }
}