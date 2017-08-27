﻿using System;
using System.Linq;
using Xunit;
using EnemizerLibrary;
using System.IO;
using Xunit.Abstractions;

namespace EnemizerTests
{
    public class NewBossRandomizerTests
    {
        readonly ITestOutputHelper output;
        public const int seed = 0;
        public NewBossRandomizerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void default_boss_pool_contains_two_armos()
        {
            BossRandomizer br = new BossRandomizer(new Random(seed));
            Assert.Equal(2, br.PossibleBossesPool.Where(x => x.BossType == BossType.Armos).Count());
        }

        [Fact]
        public void default_boss_pool_contains_two_lanmola()
        {
            BossRandomizer br = new BossRandomizer(new Random(seed));
            Assert.Equal(2, br.PossibleBossesPool.Where(x => x.BossType == BossType.Lanmola).Count());
        }

        [Fact]
        public void default_boss_pool_contains_two_moldorm()
        {
            BossRandomizer br = new BossRandomizer(new Random(seed));
            Assert.Equal(2, br.PossibleBossesPool.Where(x => x.BossType == BossType.Moldorm).Count());
        }

        [Fact]
        public void default_boss_pool_contains_one_helmasaur()
        {
            BossRandomizer br = new BossRandomizer(new Random(seed));
            Assert.Equal(1, br.PossibleBossesPool.Where(x => x.BossType == BossType.Helmasaur).Count());
        }

        [Fact]
        public void default_boss_pool_contains_one_arrghus()
        {
            BossRandomizer br = new BossRandomizer(new Random(seed));
            Assert.Equal(1, br.PossibleBossesPool.Where(x => x.BossType == BossType.Arrghus).Count());
        }

        [Fact]
        public void default_boss_pool_contains_one_mothula()
        {
            BossRandomizer br = new BossRandomizer(new Random(seed));
            Assert.Equal(1, br.PossibleBossesPool.Where(x => x.BossType == BossType.Mothula).Count());
        }

        [Fact]
        public void default_boss_pool_contains_one_blind()
        {
            BossRandomizer br = new BossRandomizer(new Random(seed));
            Assert.Equal(1, br.PossibleBossesPool.Where(x => x.BossType == BossType.Blind).Count());
        }

        [Fact]
        public void default_boss_pool_contains_one_kholdstare()
        {
            BossRandomizer br = new BossRandomizer(new Random(seed));
            Assert.Equal(1, br.PossibleBossesPool.Where(x => x.BossType == BossType.Kholdstare).Count());
        }

        [Fact]
        public void default_boss_pool_contains_one_vitreous()
        {
            BossRandomizer br = new BossRandomizer(new Random(seed));
            Assert.Equal(1, br.PossibleBossesPool.Where(x => x.BossType == BossType.Vitreous).Count());
        }

        [Fact]
        public void default_boss_pool_contains_one_trinexx()
        {
            BossRandomizer br = new BossRandomizer(new Random(seed));
            Assert.Equal(1, br.PossibleBossesPool.Where(x => x.BossType == BossType.Trixnexx).Count());
        }

        [Fact]
        public void default_boss_pool_generates_boss_in_all_dungeons()
        {
            byte[] ROM_DATA = LoadRom("rando.sfc");
            RomData romData = new RomData(ROM_DATA);

            //int i = 1;
            for (int i = 0; i < 100; i++)
            {
                BossRandomizer br = new BossRandomizer(new Random(i));

                try
                {
                    br.GenerateRandomizedBosses(romData);
                }
                catch
                {
                    output.WriteLine($"seed failed: i = {i}");
                }
                Assert.Equal(13, br.DungeonPool.Where(x => x.SelectedBoss != null).Count());
            }
        }

        public byte[] LoadRom(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            byte[] rom_data = new byte[fs.Length];
            fs.Read(rom_data, 0, (int)fs.Length);
            fs.Close();

            return rom_data;
        }
    }
}
