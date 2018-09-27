using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public class MetalSorter {
            public MetalSorter(ItemManager itemManager, string[] metalNames) {
                this.itemManager = itemManager;
                this.metalNames = metalNames;
            }

            //TODO: Consider Splitting stuff between ticks
            public bool Sort() {
                return metalNames.All(metalName =>
                    itemManager.Move<IMyTerminalBlock, IMyCargoContainer>(
                        src => !(src is IMyRefinery) && !(src is IMyReactor),
                        item => Utils.isMetal(item) && Utils.itemSubName(item) == metalName,
                        $"{metalName} Containers",
                        true
                    )
                );
            }

            ItemManager itemManager;
            string[] metalNames;
        }
    }
}
