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
        public class Utils {
            static public bool isOre(IMyInventoryItem item) {
                return item.Content.TypeId.ToString().Substring(16) == "Ore";
            }

            static public bool isIngot(IMyInventoryItem item) {
                return item.Content.TypeId.ToString().Substring(16) == "Ingot";
            }

            static public bool isComponent(IMyInventoryItem item) {
                return item.Content.TypeId.ToString().Substring(16) == "Component";
            }

            static public bool isMetal(IMyInventoryItem item) {
                return isOre(item) || isIngot(item);
            }

            static public string itemCategory(IMyInventoryItem item) {
                return item.Content.TypeId.ToString().Substring(16);
            }

            static public string itemSubName(IMyInventoryItem item) {
                return item.Content.SubtypeId.ToString();
            }

            static public bool sameItem(IMyInventoryItem a, IMyInventoryItem b) {
                return a.Content.TypeId == b.Content.TypeId && a.Content.SubtypeId == b.Content.SubtypeId;
            }
        }
    }
}
