﻿using Pure.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pure
{
    internal class Settings
    {
        public uint Magic { get; private set; }
        public byte AddressVersion { get; private set; }
        public byte StealthAddressVersion { get; private set; }
        public byte StealthAddressRingSize { get; private set; }
        public byte AnonymousAddressVersion { get; private set; }
        public string[] StandbyValidators { get; private set; }
        public string[] SeedList { get; private set; }
        public IReadOnlyDictionary<TransactionType, Fixed8> SystemFee { get; private set; }

        public static Settings Default { get; private set; }

        static Settings()
        {
            IConfigurationSection section = new ConfigurationBuilder().AddJsonFile("protocol.json").Build().GetSection("ProtocolConfiguration");
            Default = new Settings
            {
                Magic = uint.Parse(section.GetSection("Magic").Value),
                AddressVersion = byte.Parse(section.GetSection("AddressVersion").Value),
                AnonymousAddressVersion = byte.Parse(section.GetSection("AnonymousAddressVersion").Value),
                StealthAddressVersion = byte.Parse(section.GetSection("StealthAddressVersion").Value),
                StealthAddressRingSize = byte.Parse(section.GetSection("RingSize").Value),
                StandbyValidators = section.GetSection("StandbyValidators").GetChildren().Select(p => p.Value).ToArray(),
                SeedList = section.GetSection("SeedList").GetChildren().Select(p => p.Value).ToArray(),
                SystemFee = section.GetSection("SystemFee").GetChildren().ToDictionary(p => (TransactionType)Enum.Parse(typeof(TransactionType), p.Key, true), p => Fixed8.Parse(p.Value)),
            };
        }
    }
}
