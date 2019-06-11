#region init
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRageMath;
using VRage.Game.ModAPI.Ingame;
using Sandbox.Game.GameSystems;
using SpaceEngineers00;

namespace space18
{
    public class Program
    {
#line hidden
        public SpaceEngineers00.MyGridTerminalSystem GridTerminalSystem = new SpaceEngineers00.MyGridTerminalSystem();
        public Sandbox.Game.Entities.Blocks.MyProgrammableBlock Me;
        public IMyGridProgramRuntimeInfo Runtime;
        public string Storage = "";
        public delegate void jwEcho(string txt);
        public jwEcho Echo;
        static void Main(string[] args)
        {
            jwEcho Echo = delegate (string txt) { System.Diagnostics.Debug.Print(txt); };
            Program aa = new Program();
            aa.Main(string.Empty, UpdateType.Terminal);
            aa.Save();
        }
#line default
        #endregion init



        // IMyTextSurfaceProvider b1 = GridTerminalSystem.GetBlockWithName("Programmable block inf2") as IMyTextSurfaceProvider;
        // IMyTextSurface b3 = b1.GetSurface(0);
        // b3.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        // b3.WriteText("bla");




        /// <summary>
        /// TAG zum Markiren im CusotomData
        /// </summary>
        public const string STORAGETAG = "StorageItem";
        public List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();

        Program() { Runtime.UpdateFrequency = UpdateFrequency.Update100; }
        public void Save() { }
        public void Main(string argument, UpdateType updateSource)
        {
            Echo(DateTime.Now.ToString("s"));

            switch (argument.ToLower())
            {
                case "start": Runtime.UpdateFrequency = UpdateFrequency.Update100; Echo("Start"); break;
                case "stop": Runtime.UpdateFrequency = UpdateFrequency.None; Echo("Stop"); break;
                case "scan": findAllBlocks(); Echo($"Scan all -- {STORAGETAG} --"); allBlocks.ForEach(x => Echo(x.CustomName)); break;
            }
            if (Runtime.UpdateFrequency != UpdateFrequency.None)
            {
                findAllBlocks();

                IMyTerminalBlock a1 = allBlocks.FirstOrDefault();
                Echo(a1.CustomName);
                IMyInventory a2 = a1.GetInventory(0);

                List<MyInventoryItem> a3 = new List<MyInventoryItem>();
                a2.GetItems(a3);
                foreach (MyInventoryItem a4 in a3)
                {
                    clsItemName a5 = new clsItemName(a4);
                    Echo(a5.ToString());

                }
            }
            // Echo(Runtime.UpdateFrequency.ToString());
        }

        #region
        /// <summary>
        /// findet alle Blöcke die das STORAGETAG in den CustomData enthalten und ein Invntory haben
        /// </summary>
        public void findAllBlocks()
        {
            allBlocks.Clear();
            List<IMyTerminalBlock> Blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocks(Blocks);
            foreach (IMyTerminalBlock e1 in Blocks)
            {
                if (e1.CustomData.Contains(STORAGETAG) && e1.HasInventory) { allBlocks.Add(e1); }
            }
        }

        #endregion

        #region class


        public class clsItemName
        {
            /// <summary>mainType</summary>
            public string rt;
            /// <summary>subType</summary>
            public string rs;

            public string _rd;
            /// <summary>Display Name</summary>
            public string rd { get { return _rd; } set { _rd = string.IsNullOrEmpty(value) ? ((rt == "Ore" || rt == "Ingot") ? $"{rs} {rt}" : rs) : value; } }

            public clsItemName() { }
            public clsItemName(string rt, string rs, string rd = "")
            {
                this.rt = rt; this.rs = rs; this.rd = rd;
            }
            public clsItemName(MyInventoryItem myItem)
            {
                rt = myItem.Type.TypeId.ToString(); rs = myItem.Type.SubtypeId.ToString(); rd = null;
            }
            public override string ToString() { return rd; }
        }

        /// <summary>
        /// Position eines Item im Grid (Block.Inventory.Cell)
        /// </summary>
        public class clsPosition
        {
            /// <summary>Block ist das Container</summary>
            public IMyTerminalBlock myTerminalBlock;
            /// <summary>Inventory vom myTerminalBlock</summary>
            public IMyInventory myInventory;
            private int _myInventoryPos;
            /// <summary>Position im inventory</summary>
            public int myCell = 0;

            public int myInventoryPos
            {
                get { return _myInventoryPos; }
                set { if (myTerminalBlock.InventoryCount > value) myTerminalBlock.GetInventory(myInventoryPos = value); else _myInventoryPos = 0; }
            }

            public static bool operator ==(clsPosition a, clsPosition b) { return (a.myTerminalBlock.CustomName == b.myTerminalBlock.CustomName) && (a.myInventoryPos == b.myInventoryPos) && (a.myCell == b.myCell); }
            public static bool operator !=(clsPosition a, clsPosition b) { return !(a == b); }
            public override string ToString()
            {
                return $"\"{myTerminalBlock.CustomName}\" {_myInventoryPos}.{myCell}";
            }

            public override bool Equals(object obj)
            {
                return obj is clsPosition p && p.myTerminalBlock.CustomName == p.myTerminalBlock.CustomName && _myInventoryPos == p._myInventoryPos && myCell == p.myCell;
            }
        }
        #endregion class


        #region helper
        public Dictionary<string, string> splitInfos(string value)
        {
            Dictionary<string, string> d2 = new Dictionary<string, string>();
            if (value != null)
            {
                string[] d1 = value.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string d3 in d1)
                {
                    int pos = d3.IndexOf(':');
                    if (pos < 0) pos = d3.Length;
                    d2.Add(d3.Substring(0, pos).Trim(), d3.Substring(pos + 1).Trim());
                }
            }
            return d2;
        }
        public string joinInfos(Dictionary<string, string> infos)
        {
            return string.Join("\n", infos.Select(x => x.Key + ":" + x.Value).ToArray());
        }

        public DateTime ParseTime(string CustomData)
        {
            try
            {
                return DateTime.ParseExact(CustomData, "s", null);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        public int ParseInt(string value)
        {
            int a1 = 0;
            if (int.TryParse(value, out a1)) return a1;
            return 0;
        }
        #endregion helper




    }
}
