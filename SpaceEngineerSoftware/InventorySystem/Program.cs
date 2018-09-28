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
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            //Runtime.CurrentInstructionCount;
            //Runtime.MaxInstructionCount;

            itemManager = new ItemManager(GridTerminalSystem);

            oreSorter = new ItemSorter<IMyTerminalBlock, IMyCargoContainer>(
                itemManager,
                src => !(src is IMyAssembler) && !(src is IMyGasGenerator) && !(src is IMyRefinery) && !(src is IMyReactor),
                Utils.isMetal,
                "Metal Containers",
                true
            );
            componentSorter = new ItemSorter<IMyTerminalBlock, IMyCargoContainer>(
                itemManager,
                src => src.CubeGrid == Me.CubeGrid && !(src is IMyAssembler),
                Utils.isComponent,
                "Component Containers",
                true
            );
        }

        public void Save() {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
        }

        public void Main(string argument, UpdateType updateSource) {
            var debugScreen = GridTerminalSystem.GetBlockWithName("DebugScreen 1") as IMyTextPanel;
            debugScreen.WritePublicText($"Before: {Runtime.CurrentInstructionCount}/{Runtime.MaxInstructionCount}\n", false);
            oreSorter.Sort();
            componentSorter.Sort();

            List<IMyCargoContainer> blocks = new List<IMyCargoContainer>();
            GridTerminalSystem.GetBlocksOfType(blocks, block => block.HasInventory);
            int dedupCount = 0;
            foreach (var block in blocks) {
                if ((double)Runtime.CurrentInstructionCount / Runtime.MaxInstructionCount > 0.5)
                    break;
                itemManager.DedupOnce(block.GetInventory());
                dedupCount++;
            }
            debugScreen.WritePublicText($"Dedup count: {dedupCount}/{blocks.Count}\n", true);
            debugScreen.WritePublicText($"After: {Runtime.CurrentInstructionCount}/{Runtime.MaxInstructionCount}", true);
        }

        ItemManager itemManager;
        ItemSorter<IMyTerminalBlock, IMyCargoContainer> oreSorter;
        ItemSorter<IMyTerminalBlock, IMyCargoContainer> componentSorter;
    }
}