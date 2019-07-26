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




        /// <summary>TAG zum Markiren im CusotomData</summary>
        const string STORAGETAG = "StorageItem";
        List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
        delegate string jPrint(MyInventoryItem item);

        Program() { Runtime.UpdateFrequency = UpdateFrequency.Update100; }
        void Save() { }
        void Main(string argument, UpdateType updateSource)
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
                foreach (IMyTerminalBlock e1 in allBlocks)
                {
                    // Echo("--" + e1.CustomName);
                    if (e1.CustomData.Contains("scan1")) Scan((item) => { clsItemName b1 = new clsItemName(item); return $"+ {b1.rd}"; });
                    if (e1.CustomData.Contains("scan2")) Scan((item) => { clsItemName b1 = new clsItemName(item); return $"+ {(int)item.Amount} {b1.rd}"; });
                    if (e1.CustomData.Contains("scan3")) Scan((item) => { clsItemName b1 = new clsItemName(item); return $"new clsItemName(\"{b1.rs}\",\"{b1.rt}\");"; });
                }

                foreach (IMyTerminalBlock a1 in allBlocks)
                {
                    Echo($"-{a1.CustomName}\n");
                    for (int nr = 0; nr < a1.InventoryCount; nr++)
                    {
                        IMyInventory a2 = a1.GetInventory(nr);

                        List<MyInventoryItem> a3 = new List<MyInventoryItem>(); a2.GetItems(a3);
                        foreach (MyInventoryItem a4 in a3)
                        {
                            clsItem a5 = new clsItem(a1, a2, a4);
                            Echo($"--{a5}\n");
                        }
                    }
                }
            }
            // Echo(Runtime.UpdateFrequency.ToString());
        }

        #region
        /// <summary>
        /// findet alle Blöcke die das STORAGETAG in den CustomData enthalten und ein Invntory haben
        /// </summary>
        void findAllBlocks()
        {
            allBlocks.Clear();
            List<IMyTerminalBlock> Blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocks(Blocks);
            foreach (IMyTerminalBlock e1 in Blocks)
            {
                if (e1.CustomData.Contains(STORAGETAG) && e1.HasInventory) { allBlocks.Add(e1); }
            }
        }
        void Scan(jPrint print)
        {
            foreach (IMyTerminalBlock b1 in allBlocks)
            {
                List<MyInventoryItem> b2 = new List<MyInventoryItem>();
                b1.GetInventory().GetItems(b2);
                b1.CustomData = STORAGETAG + "\n";
                foreach (MyInventoryItem b3 in b2) b1.CustomData += print(b3) + "\n";
            }
        }

        #endregion

        #region class
        /// <summary>
        /// Position eines Item im Grid (Block.Inventory.Cell)
        /// </summary>
        class clsPosition
        {
            /// <summary>Block ist das Container</summary>
            IMyTerminalBlock block;
            /// <summary>Inventory vom myTerminalBlock</summary>
            private IMyInventory _myInventory;
            /// <summary>Nr des Inventory</summary>
            private int _myInventoryPos;
            /// <summary>Position im inventory</summary>
            public int myCell = 0;

            public clsPosition(IMyTerminalBlock b, IMyInventory i = null, int pos = 0) { block = b; myInventory = i; myCell = pos; }

            public IMyInventory myInventory
            {
                get { return _myInventory; }
                set { _myInventory = value; for (int i1 = 0; i1 < block.InventoryCount; i1++) if (block.GetInventory(i1).Equals(value)) _myInventoryPos = i1; }
            }

            public int myInventoryPos
            {
                get { return _myInventoryPos; }
                set { if (block.InventoryCount > value) myInventory = block.GetInventory(myInventoryPos = value); else _myInventoryPos = 0; }
            }
            public override string ToString()
            {
                return $"\"{block.CustomName}\" {_myInventoryPos}.{myCell}";
            }
        }

        /// <summary>
        /// Item mit Name und Position
        /// </summary>
        class clsItem
        {
            public clsPosition pos;
            /// <summary>subType</summary>
            public clsItemName itemName;
            /// <summary>subType</summary>
            public MyInventoryItem item;
            /// <summary>Anzahl der Item</summary>
            public int amount { get { return (int)item.Amount; } }

            public clsItem(string rS, string rT, string rD = null) { itemName = new clsItemName(rS, rT, rD); }
            public clsItem(MyInventoryItem v) { itemName = new clsItemName(item = v); }
            public clsItem(IMyInventory i, MyInventoryItem v) : this(null, i, v) { }
            public clsItem(IMyTerminalBlock b, IMyInventory i, MyInventoryItem v) : this(v) { pos = new clsPosition(b, i); }

            public override string ToString() { return $"{itemName}"; }
        }

        class clsItemName1
        {
            private string _rd;

            public string rs { get; set; }
            public string rt { get; set; }
            public string rd { get { return _rd; } set { _rd = string.IsNullOrEmpty(value) ? ((rt == "Ore" || rt == "Ingot") ? $"{rs} {rt}" : rs) : value; } }
            public clsItemName1(string rS, string rT, string rD = "") { rs = rS; rt = rT; rd = rD; }
        }

        /// <summary>
        /// Name des Items mit den Anzeige-Name
        /// rs=subType, rt=mainType, rd=Display Name
        /// ea=Liste aller Items
        /// </summary>
        class clsItemName
        {
            private string _rt;
            private string _rd;

            /// <summary>subType</summary>
            public string rs;
            /// <summary>mainType</summary>
            public string rt { get { return _rt; } set { _rt = value.Replace("MyObjectBuilder_", ""); } }
            /// <summary>Display Name</summary>
            public string rd
            {
                get { return _rd; }
                set {
                    _rd = (string.IsNullOrEmpty(value)) ? (from e1 in ea where e1.rs == this.rs && e1.rt == this.rt select e1.rd).FirstOrDefault() : value;
                    if (string.IsNullOrEmpty(_rd)) _rd = (rt == "Ore" || rt == "Ingot") ? $"{rs} {rt}" : rs;
                }
            }

            public clsItemName(string rS, string rT, string rD = null) { rs = rS; rt = rT; rd = rD; }
            public clsItemName(MyInventoryItem v) : this(v.Type.SubtypeId.ToString(), v.Type.TypeId.ToString()) { }

            public override string ToString() { return $"rs={rt},rt={rs},rd={rd}"; }

            public List<clsItemName1> ea = new List<clsItemName1>()
            {
            // Ore
            new clsItemName1("Gold", "Ore"),
            new clsItemName1("Cobalt", "Ore"),
            new clsItemName1("Ice", "Ore", "Ice"),
            new clsItemName1("Iron", "Ore"),
            new clsItemName1("Stone", "Ore", "Stone"),
            new clsItemName1("Magnesium", "Ore"),
            new clsItemName1("Nickel", "Ore"),
            new clsItemName1("Organic", "Ore", "Organic"),
            new clsItemName1("Platinum", "Ore"),
            new clsItemName1("Silicon", "Ore"),
            new clsItemName1("Silver", "Ore"),
            new clsItemName1("Scrap", "Ore", "Scrap Metal"),
            new clsItemName1("Trinium", "Ore"),
            new clsItemName1("Uranium", "Ore"),
            new clsItemName1("Naquadah", "Ore"),
            new clsItemName1("Neutronium", "Ore"),
            new clsItemName1("Deuterium", "Ore"),
            // Ingot
            new clsItemName1("Cobalt", "Ingot"),
            new clsItemName1("Gold", "Ingot"),
            new clsItemName1("Stone", "Ingot", "Gravel"),
            new clsItemName1("Iron", "Ingot"),
            new clsItemName1("Magnesium", "Ingot", "Magnesium Powder"),
            new clsItemName1("Uranium", "Ingot"),
            new clsItemName1("Nickel", "Ingot"),
            new clsItemName1("Platinum", "Ingot"),
            new clsItemName1("Silicon", "Ingot", "Silicon Wafer"),
            new clsItemName1("Silver", "Ingot"),
            new clsItemName1("Naquadah", "Ingot"),
            new clsItemName1("Neutronium", "Ingot"),
            new clsItemName1("Trinium", "Ingot"),
            new clsItemName1("DeuteriumContainer", "Ingot", "Deuterium Container"),
            new clsItemName1("ShieldPoint", "Ingot"),
            new clsItemName1("Scrap", "Ingot", "Old Scrap Metal"),
            // Component
            new clsItemName1("Construction", "Component"),
            new clsItemName1("MetalGrid", "Component", "Metal Grid"),
            new clsItemName1("InteriorPlate", "Component", "Interior Plate"),
            new clsItemName1("SteelPlate", "Component", "Steel Plate"),
            new clsItemName1("Girder", "Component"),
            new clsItemName1("SmallTube", "Component", "Small Tube"),
            new clsItemName1("LargeTube", "Component", "Large Tube"),
            new clsItemName1("Motor", "Component"),
            new clsItemName1("Display", "Component"),
            new clsItemName1("BulletproofGlass", "Component", "Bulletp. Glass"),
            new clsItemName1("Computer", "Component"),
            new clsItemName1("Reactor", "Component"),
            new clsItemName1("Thrust", "Component", "Thruster"),
            new clsItemName1("GravityGenerator", "Component", "Gravity Comp."),
            new clsItemName1("Medical", "Component"),
            new clsItemName1("RadioCommunication", "Component", "Radio-comm Comp."),
            new clsItemName1("Detector", "Component"),
            new clsItemName1("Explosives", "Component"),
            new clsItemName1("SolarCell", "Component", "Solar Cell"),
            new clsItemName1("PowerCell", "Component", "Power Cell"),
            new clsItemName1("Superconductor", "Component"),
            new clsItemName1("Canvas", "Component"),
            new clsItemName1("ArcFuel", "Component", "Arc Fuel"),
            new clsItemName1("ArcReactorcomponent", "Component", "Arc Reactor component"),
            new clsItemName1("DenseSteelPlate", "Component", "Dense Steel Plate"),
            new clsItemName1("Drone", "Component", "Drone Wwapon (no function)"),
            new clsItemName1("MagnetronComponent", "Component", "Magnetron Component"),
            new clsItemName1("Naquadah", "Component", "Naquadah Component"),
            new clsItemName1("Neutronium", "Component", "Neutronium Create"),
            new clsItemName1("Shield", "Component", "Shield Component"),
            new clsItemName1("SolarCellBlack", "Component", "Solar Cell Black"),
            new clsItemName1("SolarCellGold", "Component", "Solar Cell Gold"),
            new clsItemName1("ZPM", "Component", "Zero Point Module"),
            new clsItemName1("AdvancedPowerCell", "Component", "Advanced Power Cell"),
            // PhysicalGunObject
            new clsItemName1("Staff", "PhysicalGunObject", "Jaffa Staff Weapon"),
            new clsItemName1("P90", "PhysicalGunObject", "Stargate P90"),
            new clsItemName1("Zat", "PhysicalGunObject", "Zat'nik'tel"),
            new clsItemName1("AutomaticRifleItem", "PhysicalGunObject", "Automatic Rifle"),
            new clsItemName1("PreciseAutomaticRifleItem", "PhysicalGunObject", "Precise Automatic Rifle"),
            new clsItemName1("UltimateAutomaticRifleItem", "PhysicalGunObject", "Elite Automatic Rifle"),
            new clsItemName1("WelderItem", "PhysicalGunObject", "Welder"),
            new clsItemName1("Welder2Item", "PhysicalGunObject", "Enhanced Welder"),
            new clsItemName1("Welder3Item", "PhysicalGunObject", "Proficient Welder"),
            new clsItemName1("Welder4Item", "PhysicalGunObject", "Elite Welder"),
            new clsItemName1("AngleGrinderItem", "PhysicalGunObject", "Grinder"),
            new clsItemName1("AngleGrinder2Item", "PhysicalGunObject", "Enhanced Grinder"),
            new clsItemName1("AngleGrinder3Item", "PhysicalGunObject", "Proficient Grinder"),
            new clsItemName1("AngleGrinder4Item", "PhysicalGunObject", "Elite Grinder"),
            new clsItemName1("HandDrillItem", "PhysicalGunObject", "Hand Drill"),
            new clsItemName1("HandDrill2Item", "PhysicalGunObject", "Enhanced Hand Drill"),
            new clsItemName1("HandDrill3Item", "PhysicalGunObject", "Proficient Hand Drill"),
            new clsItemName1("HandDrill4Item", "PhysicalGunObject", "Elite Hand Drill"),
            // AmmoMagazine
            new clsItemName1("LargeAttractorEnergyCell", "AmmoMagazine", "Large Attractor Cell"),
            new clsItemName1("LaserAmmo", "AmmoMagazine", "Condensed laser"),
            new clsItemName1("Liquid Naquadah", "AmmoMagazine"),
            new clsItemName1("NATO_25x184mm", "AmmoMagazine", "25x184mm NATO ammo container"),
            new clsItemName1("Missile200mm", "AmmoMagazine", "200mm Missile container"),
            new clsItemName1("NATO_5p56x45mm", "AmmoMagazine", "5.56x45mm NATO magazine"),
            // other
            new clsItemName1("HydrogenBottle", "GasContainerObject", "Hydrogen Bottle"),
            new clsItemName1("OxygenBottle", "OxygenContainerObject", "Oxygen Bottle")
            };
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
