using LightningStrikes.Models;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LightningStrikes
{
    public class Configuration : IRocketPluginConfiguration
    {
        public bool Enabled { get; set; }
        public EventsConf Events { get; set; }


        public void LoadDefaults()
        {
            Enabled = true;
            Events = new EventsConf()
            {
                OnPlayerDied = new EventConf()
                {
                    Enabled = true,
                    StrikeType = EStrikeType.SingleStrike,
                    Amount = -1,
                    Radius = -1,
                    MinDelay = -1,
                    MaxDelay = -1
                },
                OnPlayerConnected = new EventConf()
                {
                    Enabled = true,
                    StrikeType = EStrikeType.SingleStrike,
                    Amount = -1,
                    Radius = -1,
                    MinDelay = -1,
                    MaxDelay = -1
                },
                OnPlayerDisconnected = new EventConf()
                {
                    Enabled = true,
                    StrikeType = EStrikeType.SingleStrike,
                    Amount = -1,
                    Radius = -1,
                    MinDelay = -1,
                    MaxDelay = -1
                },
                OnPlayerSpawned = new EventConf()
                {
                    Enabled = true,
                    StrikeType = EStrikeType.SingleStrike,
                    Amount = -1,
                    Radius = -1,
                    MinDelay = -1,
                    MaxDelay = -1
                },
                OnPlayerTeleported = new EventConf()
                {
                    Enabled = true,
                    StrikeType = EStrikeType.SingleStrike,
                    Amount = -1,
                    Radius = -1,
                    MinDelay = -1,
                    MaxDelay = -1
                },
                OnPlayerBanned = new EventConf()
                {
                    Enabled = true,
                    StrikeType = EStrikeType.SingleStrike,
                    Amount = -1,
                    Radius = -1,
                    MinDelay = -1,
                    MaxDelay = -1
                },
                OnZombieDied = new EventConf()
                {
                    Enabled = true,
                    StrikeType = EStrikeType.SingleStrike,
                    Amount = -1,
                    Radius = -1,
                    MinDelay = -1,
                    MaxDelay = -1
                },
                OnMegaZombieDied = new EventConf()
                {
                    Enabled = true,
                    StrikeType = EStrikeType.SingleStrike,
                    Amount = -1,
                    Radius = -1,
                    MinDelay = -1,
                    MaxDelay = -1
                },
                OnAnimalDied = new EventConf()
                {
                    Enabled = true,
                    StrikeType = EStrikeType.SingleStrike,
                    Amount = -1,
                    Radius = -1,
                    MinDelay = -1,
                    MaxDelay = -1
                },
            };
        }

        public class EventsConf
        {
            public EventConf OnPlayerDied { get; set; }
            public EventConf OnPlayerConnected { get; set; }
            public EventConf OnPlayerDisconnected { get; set; }
            public EventConf OnPlayerSpawned { get; set; }
            public EventConf OnPlayerTeleported { get; set; }
            public EventConf OnPlayerBanned { get; set; }

            public EventConf OnZombieDied { get; set; }
            public EventConf OnMegaZombieDied { get; set; }

            public EventConf OnAnimalDied { get; set; }
        }

        public class EventConf
        {
            [XmlAttribute]
            public bool Enabled { get; set; }

            [XmlAttribute]
            public EStrikeType StrikeType { get; set; }

            [XmlAttribute]
            public bool Damage { get; set; }

            [XmlAttribute]
            public int Amount { get; set; }

            [XmlAttribute]
            public float Radius { get; set; }


            [XmlAttribute]
            public int MinDelay { get; set; }


            [XmlAttribute]
            public int MaxDelay { get; set; }
        }


    }
}
