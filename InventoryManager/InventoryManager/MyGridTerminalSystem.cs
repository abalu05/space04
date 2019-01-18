#line hidden
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace SpaceEngineers00
{

    /// <summary>
    /// https://github.com/KeenSoftwareHouse/SpaceEngineers/blob/master/Sources/Sandbox.Game/ModAPI/MyGridTerminalSystem_ModAPI.cs
    /// </summary>
    public class MyGridTerminalSystem : IMyGridTerminalSystem
    {
        public Dictionary<long, IMyTerminalBlock> Blocks = new Dictionary<long, IMyTerminalBlock>();
        public List<IMyBlockGroup> BlockGroups = new List<IMyBlockGroup>();

        /// <summary>
        /// sucht ein Block nach dem CustomName
        /// </summary>
        /// <param name="name">string: CustomName des Blocks</param>
        /// <returns>IMyTerminalBlock: gefundener Block</returns>
        public IMyTerminalBlock GetBlockWithName(string name)
        {
            foreach (IMyTerminalBlock block in Blocks.Values)
            {
                if (block.CustomName.CompareTo(name) == 0) // && block.IsAccessibleForProgrammableBlock)
                {
                    return block;
                }
            }
            return null;
        }

        /// <summary>
        /// sucht ein Block nach ID
        /// </summary>
        /// <param name="id">Long: Id vom Block</param>
        /// <returns>IMyTerminalBlock: gefundener Block</returns>
        public IMyTerminalBlock GetBlockWithId(long id)
        {
            IMyTerminalBlock block;
            if (Blocks.TryGetValue(id, out block)) // && block.IsAccessibleForProgrammableBlock)
            {
                return block;
            }
            return null;
        }

        /// <summary>
        /// copy aller Blocks
        /// </summary>
        /// <param name="blocks">List: alle Bocks</param>
        public void GetBlocks(List<IMyTerminalBlock> blocks)
        {
            blocks.Clear();
            foreach (var block in Blocks.Values)
            {
                blocks.Add(block);
            }
        }

        public void GetBlockGroups(List<IMyBlockGroup> blockGroups, Func<IMyBlockGroup, bool> collect = null)
        {
            // Allow a pure collect search by allowing a null block list
            if (blockGroups != null) { blockGroups.Clear(); }
            for (var index = 0; index < BlockGroups.Count; index++)
            {
                var blockGroup = BlockGroups[index];
                if (collect != null && !collect(blockGroup)) continue;
                if (blockGroups != null) { blockGroups.Add(blockGroup); }
            }
        }

        public IMyBlockGroup GetBlockGroupWithName(string name)
        {
            for (var i = 0; i < BlockGroups.Count; i++)
            {
                var group = BlockGroups[i];
                if (group.Name.CompareTo(name) != 0) { continue; }
                return group;
            }
            return null;
        }

        public void GetBlocksOfType<T>(List<T> blocks, Func<T, bool> collect = null) where T : class
        {
            // Allow a pure collect search by allowing a null block list
            if (blocks != null)
            {
                blocks.Clear();
            }
            foreach (var block in Blocks.Values)
            {
                var typedBlock = block as T;
                if (typedBlock == null || (collect != null && !collect(typedBlock))) // || !block.IsAccessibleForProgrammableBlock)
                {
                    continue;
                }
                if (blocks != null)
                {
                    blocks.Add(typedBlock);
                }
            }
        }

        public void GetBlocksOfType<T>(List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null) where T : class
        {
            // Allow a pure collect search by allowing a null block list
            if (blocks != null)
            {
                blocks.Clear();
            }
            foreach (var block in Blocks.Values)
            {
                var typedBlock = block as T;
                if (typedBlock == null || (collect != null && !collect(block))) // || !block.IsAccessibleForProgrammableBlock
                {
                    continue;
                }
                if (blocks != null)
                {
                    blocks.Add(block);
                }
            }
        }

        public void SearchBlocksOfName(string name, List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null)
        {
            // Allow a pure collect search by allowing a null block list
            if (blocks != null)
            {
                blocks.Clear();
            }
            foreach (var block in Blocks.Values)
            {
                if (!block.CustomName.ToString().Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                if ((collect != null && !collect(block))) // || !block.IsAccessibleForProgrammableBlock
                {
                    continue;
                }
                if (blocks != null)
                {
                    blocks.Add(block);
                }
            }
        }

    }

}
#line default
