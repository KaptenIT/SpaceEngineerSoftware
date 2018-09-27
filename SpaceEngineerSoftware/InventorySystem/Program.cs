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
    partial class Program : MyGridProgram {

        public Program() {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            //Runtime.CurrentInstructionCount;
            //Runtime.MaxInstructionCount;

            itemManager = new ItemManager(GridTerminalSystem);

            oreSorter = new ItemSorter<IMyTerminalBlock, IMyCargoContainer>(
                itemManager,
                src => !(src is IMyRefinery) && !(src is IMyReactor),
                Utils.isMetal,
                "Metal Containers",
                true
            );
        }

        public void Save() {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
        }

        public void Main(string argument, UpdateType updateSource) {

            oreSorter.Sort();

        }

        ItemManager itemManager;
        ItemSorter<IMyTerminalBlock, IMyCargoContainer> oreSorter;
    }
}