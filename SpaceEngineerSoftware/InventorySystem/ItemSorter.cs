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
        public class ItemSorter<T, U>
            where T: class, IMyTerminalBlock
            where U: class, IMyTerminalBlock {

            public ItemSorter(ItemManager itemManager, string srcGroup, Func<IMyInventoryItem, bool> itemFilter, string dstGroup, bool preventMoveToSelf) {
                this.itemManager = itemManager;

                this.srcGroup = srcGroup;
                this.srcFilter = null;

                this.itemFilter = itemFilter;

                this.dstGroup = dstGroup;
                this.dstFilter = null;

                this.preventMoveToSelf = preventMoveToSelf;
            }

            public ItemSorter(ItemManager itemManager, Func<T, bool> srcFilter, Func<IMyInventoryItem, bool> itemFilter, string dstGroup, bool preventMoveToSelf) {
                this.itemManager = itemManager;

                this.srcGroup = null;
                this.srcFilter = srcFilter;

                this.itemFilter = itemFilter;

                this.dstGroup = dstGroup;
                this.dstFilter = null;

                this.preventMoveToSelf = preventMoveToSelf;
            }

            public ItemSorter(ItemManager itemManager, string srcGroup, Func<IMyInventoryItem, bool> itemFilter, Func<U, bool> dstFilter, bool preventMoveToSelf) {
                this.itemManager = itemManager;

                this.srcGroup = srcGroup;
                this.srcFilter = null;

                this.itemFilter = itemFilter;

                this.dstGroup = null;
                this.dstFilter = dstFilter;

                this.preventMoveToSelf = preventMoveToSelf;
            }

            public ItemSorter(ItemManager itemManager, Func<T, bool> srcFilter, Func<IMyInventoryItem, bool> itemFilter, Func<U, bool> dstFilter, bool preventMoveToSelf) {
                this.itemManager = itemManager;

                this.srcGroup = null;
                this.srcFilter = srcFilter;

                this.itemFilter = itemFilter;

                this.dstGroup = null;
                this.dstFilter = dstFilter;

                this.preventMoveToSelf = preventMoveToSelf;
            }

            public bool Sort() {
                if (srcFilter == null && dstFilter == null) {
                    return itemManager.Move<T, U>(srcGroup, itemFilter, dstGroup, preventMoveToSelf);
                }
                else if (srcFilter == null) {
                    return itemManager.Move<T, U>(srcGroup, itemFilter, dstFilter, preventMoveToSelf);
                }
                else if (dstFilter == null) {
                    return itemManager.Move<T, U>(srcFilter, itemFilter, dstGroup, preventMoveToSelf);
                }
                else {
                    return itemManager.Move(srcFilter, itemFilter, dstFilter, preventMoveToSelf);
                }
            }


            string srcGroup;
            Func<T, bool> srcFilter;

            Func<IMyInventoryItem, bool> itemFilter;

            string dstGroup;
            Func<U, bool> dstFilter;

            bool preventMoveToSelf;

            ItemManager itemManager;
        }
    }
}
