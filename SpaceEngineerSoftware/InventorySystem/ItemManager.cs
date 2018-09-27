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
        public class ItemManager {
            public ItemManager(IMyGridTerminalSystem terminal) {
                this.terminal = terminal;
            }


            public enum FillRefineryInfo {
                Success = 0,
                NoRefineries,
                NoContainers,
                NotEnoughOre
            }

            /// <summary>
            /// Try to move items from src to dst.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <typeparam name="U"></typeparam>
            /// <param name="srcGroupName">Name of group containing sources</param>
            /// <param name="itemFilter">What items to consider</param>
            /// <param name="dstGroupName">Name of group containing destinations</param>
            /// <returns>true if all items successfuly moved</returns>
            public bool Move<T, U>(string srcGroupName, Func<IMyInventoryItem, bool> itemFilter, string dstGroupName, bool preventMoveToSelf, int srcSubInventory = 0, int dstSubInventory = 0)
                where T : class, IMyTerminalBlock
                where U : class, IMyTerminalBlock {

                var inputs = new List<T>();
                var srcGroup = terminal.GetBlockGroupWithName(srcGroupName);
                srcGroup.GetBlocksOfType(inputs);

                var outputs = new List<U>();
                var dstGroup = terminal.GetBlockGroupWithName(dstGroupName);
                dstGroup.GetBlocksOfType(outputs);

                return MoveHelper(inputs, itemFilter, outputs, preventMoveToSelf, srcSubInventory, dstSubInventory);
            }

            /// <summary>
            /// Try to move items from src to dst.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <typeparam name="U"></typeparam>
            /// <param name="srcFilter">What sources to consider</param>
            /// <param name="itemFilter">What items to consider</param>
            /// <param name="dstGroupName">Name of group containing destinations</param>
            /// <returns>true if all items successfuly moved</returns>
            public bool Move<T, U>(Func<T, bool> srcFilter, Func<IMyInventoryItem, bool> itemFilter, string dstGroupName, bool preventMoveToSelf, int srcSubInventory = 0, int dstSubInventory = 0)
                where T : class, IMyTerminalBlock
                where U : class, IMyTerminalBlock {

                var inputs = new List<T>();
                terminal.GetBlocksOfType(inputs, block => block.HasInventory && srcFilter(block));

                var outputs = new List<U>();
                var dstGroup = terminal.GetBlockGroupWithName(dstGroupName);
                dstGroup.GetBlocksOfType(outputs);

                return MoveHelper(inputs, itemFilter, outputs, preventMoveToSelf, srcSubInventory, dstSubInventory);
            }

            /// <summary>
            /// Try to move items from src to dst.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <typeparam name="U"></typeparam>
            /// <param name="srcGroupName">Name of group containing sources</param>
            /// <param name="itemFilter">What items to consider</param>
            /// <param name="dstFilter">What destinations to consider</param>
            /// <returns>true if all items successfuly moved</returns>
            public bool Move<T, U>(string srcGroupName, Func<IMyInventoryItem, bool> itemFilter, Func<U, bool> dstFilter, bool preventMoveToSelf, int srcSubInventory = 0, int dstSubInventory = 0)
                where T : class, IMyTerminalBlock
                where U : class, IMyTerminalBlock {

                var inputs = new List<T>();
                var srcGroup = terminal.GetBlockGroupWithName(srcGroupName);
                srcGroup.GetBlocksOfType(inputs);

                var outputs = new List<U>();
                terminal.GetBlocksOfType(outputs, block => block.HasInventory && dstFilter(block));

                return MoveHelper(inputs, itemFilter, outputs, preventMoveToSelf, srcSubInventory, dstSubInventory);
            }

            /// <summary>
            /// Try to move items from src to dst.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <typeparam name="U"></typeparam>
            /// <param name="srcFilter">What sources to consider</param>
            /// <param name="itemFilter">What items to consider</param>
            /// <param name="dstFilter">What destinations to consider</param>
            /// <returns>true if all items successfuly moved</returns>
            public bool Move<T, U>(Func<T, bool> srcFilter, Func<IMyInventoryItem, bool> itemFilter, Func<U, bool> dstFilter, bool preventMoveToSelf, int srcSubInventory = 0, int dstSubInventory = 0) 
                where T : class, IMyTerminalBlock
                where U : class, IMyTerminalBlock {

                var inputs = new List<T>();
                terminal.GetBlocksOfType(inputs, block => block.HasInventory && srcFilter(block));

                var outputs = new List<U>();
                terminal.GetBlocksOfType(outputs, block => block.HasInventory && dstFilter(block));

                return MoveHelper(inputs, itemFilter, outputs, preventMoveToSelf, srcSubInventory, dstSubInventory);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <typeparam name="U"></typeparam>
            /// <param name="inputs"></param>
            /// <param name="itemFilter"></param>
            /// <param name="outputs"></param>
            /// <param name="srcSubInventory"></param>
            /// <param name="dstSubInventory"></param>
            /// <param name="preventMoveToSelf"></param>
            /// <returns></returns>
            private bool MoveHelper<T, U>(List<T> inputs, Func<IMyInventoryItem, bool> itemFilter, List<U> outputs, bool preventMoveToSelf, int srcSubInventory, int dstSubInventory)
                where T : class, IMyTerminalBlock
                where U : class, IMyTerminalBlock {

                HashSet<IMyTerminalBlock> outputSet = preventMoveToSelf ? new HashSet<IMyTerminalBlock>(outputs) : null;

                int dstIndex = -1;
                if (!nextNonFull(outputs, ref dstIndex)) {
                    return false;//No dst with space left found
                }
                var dst = outputs[dstIndex].GetInventory(dstSubInventory);

                var filteredInputs = preventMoveToSelf ? inputs.SkipWhile(input => outputSet.Contains(input)) : inputs;

                foreach (var input in filteredInputs) {
                    int srcItemIndex = 0;
                    var src = input.GetInventory(srcSubInventory);
                    var amountAtSlot = search(itemFilter, src, ref srcItemIndex);
                    while (amountAtSlot != 0) {
                        var srcItems = src.GetItems();

                        var itemCountBefore = srcItems.Count;
                        var itemAmountBefore = srcItems[srcItemIndex].Amount;
                        src.TransferItemTo(dst, srcItemIndex, 0, true);
                        var itemCountAfter = srcItems.Count;

                        //Nothing was moved, dst must be full
                        if (itemCountBefore == itemCountAfter && itemAmountBefore == srcItems[srcItemIndex].Amount) {
                            if (!nextNonFull(outputs, ref dstIndex)) {
                                return false;//Failed to find dst with space left
                            }
                            dst = outputs[dstIndex].GetInventory(dstSubInventory);
                        }
                    }
                }
                return true;
            }

            //O(refCount + (srcCount * srcItemSlotCount))
            public FillRefineryInfo fillRefineries(Func<IMyCargoContainer, bool> srcFilter, string itemName, int oreAvailable) {
                var refineries = new List<IMyRefinery>();
                terminal.GetBlocksOfType(refineries);

                var containers = new List<IMyCargoContainer>();
                terminal.GetBlocksOfType(containers, srcFilter);

                if (refineries.Count == 0) {
                    return FillRefineryInfo.NoRefineries;
                }
                if (containers.Count == 0) {
                    return FillRefineryInfo.NoContainers;
                }

                var refineryCapacity = (int)refineries.First().GetInventory().MaxVolume;
                var orePerRefinery = Math.Min(oreAvailable / refineries.Count, refineryCapacity);


                int srcIndex = 0;
                int srcItemIndex = 0;
                var src = containers[srcIndex].GetInventory();
                var oreAtSlot = search(itemName, src, ref srcItemIndex);

                foreach (var refinery in refineries) {
                    var dst = refinery.GetInventory();
                    var oreToTransfer = orePerRefinery - dst.CurrentVolume;

                    while (oreAtSlot < oreToTransfer) {
                        while (oreAtSlot == 0) {//No more ore in src
                            srcIndex++;

                            //Go to next src(if possible)

                            if (srcIndex >= containers.Count) {//This should not happen
                                return FillRefineryInfo.NotEnoughOre;
                            }
                            src = containers[srcIndex].GetInventory();
                            srcItemIndex = 0;
                            oreAtSlot = search(itemName, src, ref srcItemIndex);
                        }

                        src.TransferItemTo(dst, srcItemIndex, 0, true);
                        oreToTransfer -= oreAtSlot;
                        oreAtSlot = search(itemName, src, ref srcItemIndex);
                    }

                    src.TransferItemTo(dst, srcItemIndex, 0, true, oreToTransfer);
                    oreAtSlot = search(itemName, src, ref srcItemIndex);
                }
                return FillRefineryInfo.Success;
            }

            public void Dedup(IMyInventory inventory) {
                var items = inventory.GetItems();
                for (int i = 0; i < items.Count;) {
                    int j = i + 1;
                    while (search(item => item == items[i], inventory, ref j) > 0) {//TODO: Check if this equality check actually works
                        inventory.TransferItemTo(inventory, j, i, true);
                    }
                }
            }

            /// <summary>
            /// Return amount of item at first position at or after `index` matcing `p()`
            /// `index` will be set to this position if item was found otherwise `itemCount`
            /// </summary>
            /// <param name="p">Item to search for</param>
            /// <param name="inventory"></param>
            /// <param name="index">Where to start the search, this will contain the index of the found item or itemCount</param>
            /// <returns>Amount of item</returns>
            private VRage.MyFixedPoint search(Func<IMyInventoryItem, bool> p, IMyInventory inventory, ref int index) {
                var items = inventory.GetItems();
                for (; index < items.Count; index++) {
                    if (p(items[index])) {
                        return items[index].Amount;
                    }
                }
                return 0;
            }

            /// <summary>
            /// Return amount of item at first position at or after `index` with name `itemName`
            /// `index` will be set to this position if item was found otherwise `itemCount`
            /// </summary>
            /// <param name="itemName">Item to search for</param>
            /// <param name="inventory"></param>
            /// <param name="index">Where to start the search, this will contain the index of the found item or itemCount</param>
            /// <returns>Amount of item</returns>
            private VRage.MyFixedPoint search(string itemName, IMyInventory inventory, ref int index) {
                return search(item => getName(item) == itemName, inventory, ref index);
            }

            bool nextNonFull<T>(List<T> blocks, ref int index) where T : IMyTerminalBlock {
                do {
                    index++;
                    if (index >= blocks.Count) {
                        return false;
                    }
                } while (blocks[index].GetInventory().IsFull);
                return true;
            }

            public string getName(IMyInventoryItem item) {
                return item.Content.SubtypeId.ToString() + ' ' + item.Content.TypeId.ToString().Substring(16);
            }

            IMyGridTerminalSystem terminal;
        }
    }
}
