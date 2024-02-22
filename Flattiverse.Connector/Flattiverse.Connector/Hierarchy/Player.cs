﻿using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using System;

namespace Flattiverse.Connector.Hierarchy
{
    public class Player
    {
        public readonly byte ID;
        public readonly string Name;
        public readonly PlayerKind Kind;
        public readonly Team Team;

        private bool active;

        private ControllableInfo?[] controllableInfos;

        public readonly UniversalHolder<ControllableInfo> ControllableInfos;

        public bool Active => active;

        internal Player(byte id, PlayerKind kind, Team team, PacketReader reader)
        {
            active = true;
            ID = id;
            Kind = kind;
            Team = team;
            
            Name = reader.ReadString();

            controllableInfos = new ControllableInfo[256];
            ControllableInfos = new UniversalHolder<ControllableInfo>(controllableInfos);
        }

        internal void Deactivate()
        {
            active = false;
        }

        internal void AddControllableInfo(ControllableInfo info)
        {
            controllableInfos[info.Id] = info;
        }

        internal void RemoveControllableInfo(int id)
        {
            if (controllableInfos[id] is ControllableInfo info)
            {
                info.Deactivate();
                controllableInfos[id] = null;
            }
        }

        public override string ToString()
        {
            return $"Player [{ID}] {Name}({Kind})";
        }
    }
}
