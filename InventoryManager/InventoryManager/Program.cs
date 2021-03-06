﻿#region init
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
        public List<clsBlock> allBlocks = new List<clsBlock>();
        public List<clsParam> allParam = new List<clsParam>();
        public List<clsItem> allItems = new List<clsItem>();
        clsItemName ItemsConf = new clsItemName();

        Program() { Runtime.UpdateFrequency = UpdateFrequency.Update100; }
        public void Save() { }
        public void Main(string argument, UpdateType updateSource)
        {
            Echo(DateTime.Now.ToString("s"));

            switch (argument.ToLower())
            {
                case "start": Runtime.UpdateFrequency = UpdateFrequency.Update100; Echo("Start"); break;
                case "stop": Runtime.UpdateFrequency = UpdateFrequency.None; Echo("Stop"); break;
                case "scan": findAllBlocks(); Echo($"Scan all -- {STORAGETAG} --"); allBlocks.ForEach(x => Echo(x.thisBlock.CustomName)); break;
            }
            if (Runtime.UpdateFrequency != UpdateFrequency.None)
            {
                findAllBlocks();
                foreach (clsBlock e1 in allBlocks)
                {
                    Echo("--" + e1.thisBlock.CustomName);
                    if (e1.Contains("scan1")) e1.Scan1(ItemsConf, this);
                    if (e1.Contains("scan2")) e1.Scan2(ItemsConf, this);
                    if (e1.Contains("scan3")) e1.Scan3(ItemsConf, this);
                }
                findAllParm();
                Echo($"allParam={allParam.Count.ToString()}");
                foreach (clsParam e9 in allParam) Echo("#1 " + e9.thisBlock.CustomName);
                return;

                findAllItem(ItemsConf);
                Echo($"allItems = {allItems.Count}");
                do
                {
                    List<clsParam> targetParm = netxParm();
                    if (targetParm == null) break;

                    clsParam firstParm = targetParm.First();
                    Echo($"target={firstParm.name}");

                    List<clsItem> source = findItem(firstParm.name);
                    Echo($"source={source.Count.ToString()}");

                    int amount = getAnzahl(firstParm.name) / targetParm.Count;
                    Echo($"amount={amount.ToString()}");

                    foreach (clsParam e2 in targetParm)
                    {
                        foreach (clsItem e3 in source)
                        {
                            // quelle und ziel, wenn gleich nur menge kontrolieren
                            Echo($"e2={e2.thisBlock.CustomName}");
                            Echo($"e3={e3.myTerminalBlock.CustomName}");
                            if (e2.thisBlock.CustomName == e3.myTerminalBlock.CustomName) continue;

                            e3.myInventory.TransferItemTo(e2.thisBlock.GetInventory(0), e3.pos1, 0, true, amount);
                        }
                    }
                } while (allParam.Count > 0);
            }
            // Echo(Runtime.UpdateFrequency.ToString());
        }

        #region
        /// <summary>
        /// Gibt die Anzal des Items vom gesammten System
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public int getAnzahl(List<clsItem> item, string name)
        {
            int amount = 0;
            foreach (clsItem i1 in item)
            {
                List<MyInventoryItem> i2 = new List<MyInventoryItem>();
                i1.myInventory.GetItems(i2);
                foreach (MyInventoryItem i3 in i2)
                {
                    clsItem i4 = new clsItem(i3);
                    if (i4.rd == name) amount += (int)i3.Amount;
                }
            }
            return amount;
        }
        /// <summary>
        /// Gibt die gesamt Anzahl des Items aus der globalen Liste
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int getAnzahl(string name)
        {
            return allItems.Where(y => y.rd == name).Select(x => x.amount).Sum();
        }

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
                if (e1.CustomData.Contains(STORAGETAG) && e1.HasInventory) { allBlocks.Add(new clsBlock(e1)); }
            }
        }

        /// <summary>
        /// findet alle Items von allen Blocks
        /// </summary>
        public void findAllItem(clsItemName ItemsConf)
        {
            allItems.Clear();
            foreach (clsBlock b1 in allBlocks)
            {
                for (int i1 = 0; i1 < b1.thisBlock.InventoryCount; i1++)
                {
                    int pos = 0;
                    IMyInventory i2 = b1.thisBlock.GetInventory(i1);
                    List<MyInventoryItem> i3 = new List<MyInventoryItem>();
                    i2.GetItems(i3);
                    foreach (MyInventoryItem i4 in i3)
                    {
                        clsItem i5 = new clsItem(i4, i2);
                        clsItem name = ItemsConf.ContainsKey(i5.id);
                        i5.rd = name.rd;
                        i5.pos1 = pos++;
                        allItems.Add(i5);
                    }
                }
            }
        }

        /// <summary>
        /// Findet alle Items aus der globalen Liste.
        /// </summary>
        /// <param name="name">string: Name des Items</param>
        /// <returns></returns>
        public List<clsItem> findItem(string name)
        {
            return allItems.Where(x => x.rd == name).ToList();
        }

        /// <summary>
        /// findet alle Parameter von allen Blocks
        /// </summary>
        public void findAllParm()
        {
            allParam.Clear();
            foreach (clsBlock e1 in allBlocks)
            {
                allParam.AddList(clsParam.parseParams(e1.thisBlock));
            }
        }

        /// <summary>
        /// holt das erste Param, und alle mit dem gleichen Namen, aus der Liste
        /// </summary>
        public List<clsParam> netxParm()
        {
            int len = allParam.Count - 1;
            if (len < 0) return null;
            List<clsParam> a1 = new List<clsParam>();
            a1.Add(allParam[0]); allParam.RemoveAt(0);
            for (--len; len >= 0; len--) if (a1[0].name == allParam[len].name) { a1.Add(allParam[len]); allParam.RemoveAt(len); }
            return a1;
        }
        #endregion

        #region class

        /// <summary>
        /// Parameter im Custom Data
        /// </summary>
        public class clsParam
        {
            public IMyTerminalBlock thisBlock;
            public decimal num = 0;
            public string name = "";
            public clsParam() { }
            public clsParam(string value, IMyTerminalBlock b1) : this(value) { thisBlock = b1; }
            public clsParam(string value)
            {
                value = value.Trim();
                System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(value, @"^([+-]?([0-9]+)?(\.[0-9]+)?([eE][0-9]+)?)");
                if (m.Success) decimal.TryParse(m.Value, out num);
                name = m.Success ? value.Substring(m.Value.Length).Trim() : value;
            }
            public static List<clsParam> parseParams(IMyTerminalBlock b1)
            {
                string[] row = b1.CustomData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                List<clsParam> p1 = new List<clsParam>();
                foreach (string e1 in row) if (e1 != STORAGETAG) p1.Add(new clsParam(e1, b1));

                return p1;
            }
            public override string ToString() { return (num >= 0 ? "+" : "") + num.ToString() + " " + name; }
        }

        /// <summary>
        /// Block mit zusatzfuktionen
        /// </summary>
        public class clsBlock
        {
            public IMyTerminalBlock thisBlock;
            public List<clsParam> Param = new List<clsParam>();

            /// <summary>
            /// merkt sich den Block und ließt die Parameter aus den CustomData, immer ein Parameter pro Zeile
            /// </summary>
            /// <param name="block">Block</param>
            public clsBlock(IMyTerminalBlock block)
            {
                thisBlock = block;
                string[] row = block.CustomData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string x in row) if (x != STORAGETAG) Param.Add(new clsParam(x));
            }

            /// <summary>
            /// löscht alle Parameter
            /// </summary>
            public void Clear() { Param.Clear(); }

            /// <summary>
            /// Prüft ob der Parameter im Block steht
            /// </summary>
            /// <param name="name">Parameter = Itemname</param>
            public bool Contains(string name)
            {
                foreach (clsParam e1 in Param) if (e1.name.ToLower() == name.ToLower()) return true;
                return false;
            }

            public void Scan1(clsItemName liste, Program pp)
            {
                List<MyInventoryItem> c3 = new List<MyInventoryItem>();
                thisBlock.GetInventory().GetItems(c3);
                thisBlock.CustomData = STORAGETAG + "\n";
                foreach (MyInventoryItem e1 in c3)
                {
                    thisBlock.CustomData += "+ " + liste.ContainsKey(e1, pp).rd + "\n";
                }
            }

            /// <summary>
            /// zeigt alle im Inventar enthaltenen Items an
            /// </summary>
            public void Scan2(clsItemName liste, Program p1)
            {
                List<MyInventoryItem> c3 = new List<MyInventoryItem>();
                thisBlock.GetInventory(0).GetItems(c3);
                thisBlock.CustomData = STORAGETAG + "\n";
                foreach (MyInventoryItem e1 in c3)
                {
                    p1.Echo(new clsItem(e1).ToString());
                    clsItem b1 = liste.ContainsKey(e1, p1);

                    thisBlock.CustomData += $"+{(int)e1.Amount} {b1.rd}\n";
                }
            }
            public void Scan3(clsItemName liste, Program pp)
            {
                for (int k1 = 0; k1 < thisBlock.InventoryCount; k1++)
                {
                    List<MyInventoryItem> c3 = new List<MyInventoryItem>();
                    thisBlock.GetInventory(k1).GetItems(c3);
                    thisBlock.CustomData = STORAGETAG + "\n";
                    foreach (MyInventoryItem e1 in c3)
                    {
                        clsItem itm = new clsItem(e1);
                        thisBlock.CustomData += $"Add(\"{itm.rs}\",\"{itm.rt}\");\n";
                    }
                }
            }

            public override string ToString() { return thisBlock.CustomName + "\n" + string.Join("\n", Param); }
        }

        /// <summary>
        /// Item mit Name und Position
        /// </summary>
        public class clsItem
        {
            public IMyTerminalBlock myTerminalBlock;
            /// <summary>Inventory vom Block</summary>
            public IMyInventory myInventory;
            /// <summary>Anzahl der Item</summary>
            public int amount = 0;
            /// <summary>Position im inventory</summary>
            public int pos1 = 0;
            /// <summary>subType</summary>
            public string rs;

            private string _rd;
            /// <summary>Display Name</summary>
            public string rd { get { return _rd; } set { _rd = string.IsNullOrEmpty(value) ? ((rt == "Ore" || rt == "Ingot") ? $"{rs} {rt}" : rs) : value; } }

            private string _rt;
            /// <summary>mainType</summary>
            public string rt { get { return _rt; } set { _rt = value.Replace("MyObjectBuilder_", ""); } }

            /// <summary>subType + mainType</summary>
            public string id { get { return rs + rt; } }

            public clsItem(string rS, string rT, string rD = null) { rs = rS; rt = rT; rd = rD; }
            public clsItem(MyInventoryItem v) : this(v.Type.SubtypeId.ToString(), v.Type.TypeId.ToString()) { }
            public clsItem(MyInventoryItem v, IMyInventory i) : this(v) { myInventory = i; amount = (int)v.Amount; }

            public override string ToString() { return $"rs={rs},rt={rt},rd={rd}"; }
        }

        /// <summary>
        /// Name des Items mit den Anzeige-Name
        /// rs=subType, rt=mainType, rd=Display Name
        /// ea=Liste aller Items
        /// </summary>
        public class clsItemName
        {
            private Dictionary<string, clsItem> ea = new Dictionary<string, clsItem>();
            public void ADD(string rs, string rt, string rd = "") { ea.Add(rs + rt, new clsItem(rs, rt, rd)); }

            private clsItem this[string v] { get { return ea[v]; } }
            public clsItem ContainsKey(string v) { return this[v]; }
            public clsItem ContainsKey(MyInventoryItem v, Program p1)
            {
                clsItem v1 = new clsItem(v);
                return ea.ContainsKey(v1.id) ? ea[v1.id] : v1;
            }
            public clsItem ContainsDisplay(string v)
            {
                foreach (KeyValuePair<string, clsItem> e1 in ea) if (e1.Value.rd == v) return e1.Value;
                return null;
            }

            /// <summary>
            /// subType, mainType, display name
            /// </summary>
            public clsItemName()
            {
                // Ore
                ADD("Gold", "Ore");
                ADD("Cobalt", "Ore");
                ADD("Ice", "Ore", "Ice");
                ADD("Iron", "Ore");
                ADD("Stone", "Ore", "Stone");
                ADD("Magnesium", "Ore");
                ADD("Nickel", "Ore");
                ADD("Organic", "Ore", "Organic");
                ADD("Platinum", "Ore");
                ADD("Silicon", "Ore");
                ADD("Silver", "Ore");
                ADD("Scrap", "Ore", "Scrap Metal");
                ADD("Trinium", "Ore");
                ADD("Uranium", "Ore");
                ADD("Naquadah", "Ore");
                ADD("Neutronium", "Ore");
                ADD("Deuterium", "Ore");
                // Ingot
                ADD("Cobalt", "Ingot");
                ADD("Gold", "Ingot");
                ADD("Stone", "Ingot", "Gravel");
                ADD("Iron", "Ingot");
                ADD("Magnesium", "Ingot", "Magnesium Powder");
                ADD("Uranium", "Ingot");
                ADD("Nickel", "Ingot");
                ADD("Platinum", "Ingot");
                ADD("Silicon", "Ingot", "Silicon Wafer");
                ADD("Silver", "Ingot");
                ADD("Naquadah", "Ingot");
                ADD("Neutronium", "Ingot");
                ADD("Trinium", "Ingot");
                ADD("DeuteriumContainer", "Ingot", "Deuterium Container");
                ADD("ShieldPoint", "Ingot");
                ADD("Scrap", "Ingot", "Old Scrap Metal");
                // Component
                ADD("Construction", "Component");
                ADD("MetalGrid", "Component", "Metal Grid");
                ADD("InteriorPlate", "Component", "Interior Plate");
                ADD("SteelPlate", "Component", "Steel Plate");
                ADD("Girder", "Component");
                ADD("SmallTube", "Component", "Small Tube");
                ADD("LargeTube", "Component", "Large Tube");
                ADD("Motor", "Component");
                ADD("Display", "Component");
                ADD("BulletproofGlass", "Component", "Bulletp. Glass");
                ADD("Computer", "Component");
                ADD("Reactor", "Component");
                ADD("Thrust", "Component", "Thruster");
                ADD("GravityGenerator", "Component", "Gravity Comp.");
                ADD("Medical", "Component");
                ADD("RadioCommunication", "Component", "Radio-comm Comp.");
                ADD("Detector", "Component");
                ADD("Explosives", "Component");
                ADD("SolarCell", "Component", "Solar Cell");
                ADD("PowerCell", "Component", "Power Cell");
                ADD("Superconductor", "Component");
                ADD("Canvas", "Component");
                ADD("ArcFuel", "Component", "Arc Fuel");
                ADD("ArcReactorcomponent", "Component", "Arc Reactor component");
                ADD("DenseSteelPlate", "Component", "Dense Steel Plate");
                ADD("Drone", "Component", "Drone Wwapon (no function)");
                ADD("MagnetronComponent", "Component", "Magnetron Component");
                ADD("Naquadah", "Component", "Naquadah Component");
                ADD("Neutronium", "Component", "Neutronium Create");
                ADD("Shield", "Component", "Shield Component");
                ADD("SolarCellBlack", "Component", "Solar Cell Black");
                ADD("SolarCellGold", "Component", "Solar Cell Gold");
                ADD("ZPM", "Component", "Zero Point Module");
                ADD("AdvancedPowerCell", "Component", "Advanced Power Cell");
                // PhysicalGunObject
                ADD("Staff", "PhysicalGunObject", "Jaffa Staff Weapon");
                ADD("P90", "PhysicalGunObject", "Stargate P90");
                ADD("Zat", "PhysicalGunObject", "Zat'nik'tel");
                ADD("AutomaticRifleItem", "PhysicalGunObject", "Automatic Rifle");
                ADD("PreciseAutomaticRifleItem", "PhysicalGunObject", "Precise Automatic Rifle");
                ADD("UltimateAutomaticRifleItem", "PhysicalGunObject", "Elite Automatic Rifle");
                ADD("WelderItem", "PhysicalGunObject", "Welder");
                ADD("Welder2Item", "PhysicalGunObject", "Enhanced Welder");
                ADD("Welder3Item", "PhysicalGunObject", "Proficient Welder");
                ADD("Welder4Item", "PhysicalGunObject", "Elite Welder");
                ADD("AngleGrinderItem", "PhysicalGunObject", "Grinder");
                ADD("AngleGrinder2Item", "PhysicalGunObject", "Enhanced Grinder");
                ADD("AngleGrinder3Item", "PhysicalGunObject", "Proficient Grinder");
                ADD("AngleGrinder4Item", "PhysicalGunObject", "Elite Grinder");
                ADD("HandDrillItem", "PhysicalGunObject", "Hand Drill");
                ADD("HandDrill2Item", "PhysicalGunObject", "Enhanced Hand Drill");
                ADD("HandDrill3Item", "PhysicalGunObject", "Proficient Hand Drill");
                ADD("HandDrill4Item", "PhysicalGunObject", "Elite Hand Drill");
                // AmmoMagazine
                ADD("LargeAttractorEnergyCell", "AmmoMagazine", "Large Attractor Cell");
                ADD("LaserAmmo", "AmmoMagazine", "Condensed laser");
                ADD("Liquid Naquadah", "AmmoMagazine");
                ADD("NATO_25x184mm", "AmmoMagazine", "25x184mm NATO ammo container");
                ADD("Missile200mm", "AmmoMagazine", "200mm Missile container");
                ADD("NATO_5p56x45mm", "AmmoMagazine", "5.56x45mm NATO magazine");
                // other
                ADD("HydrogenBottle", "GasContainerObject", "Hydrogen Bottle");
                ADD("OxygenBottle", "OxygenContainerObject", "Oxygen Bottle");
            }
        }









        public class clsItemName1
        {
            /// <summary>mainType</summary>
            public string rt;
            /// <summary>subType</summary>
            public string rs;

            public string _rd;
            /// <summary>Display Name</summary>
            public string rd { get { return _rd; } set { _rd = string.IsNullOrEmpty(value) ? ((rt == "Ore" || rt == "Ingot") ? $"{rs} {rt}" : rs) : value; } }

            public clsItemName1() { }
            public clsItemName1(string rt, string rs, string rd = "")
            {
                this.rt = rt; this.rs = rs; this.rd = rd;
            }
            public clsItemName1(MyInventoryItem myItem)
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








        /// <summary>
        /// Sucht eine Refinery in der das Item enthalten ist
        /// </summary>
        /// <param name="item">String: Name des Items</param>
        public IMyRefinery findRefinery(string item)
        {
            List<IMyRefinery> refinery = new List<IMyRefinery>();
            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refinery);
            foreach (IMyRefinery cargo in refinery)
            {
                if (cargo.CustomName.Contains("Deuterium")) continue;
                List<MyInventoryItem> c3 = new List<MyInventoryItem>();
                cargo.GetInventory(0).GetItems(c3);
                foreach (MyInventoryItem e2 in c3)
                {
                    // Echo(getName(e2));
                    if (getName(e2) == item) return cargo;
                }
            }
            return null;
        }

        /// <summary>
        /// Sucht ein CargoContainer in dem das angegeben Item enthalten ist
        /// </summary>
        /// <param name="item">String: Name des Items</param>
        public IMyCargoContainer findItemCargo(string item)
        {
            List<IMyCargoContainer> cargos = new List<IMyCargoContainer>();
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargos);
            foreach (IMyCargoContainer cargo in cargos)
            {
                List<MyInventoryItem> c3 = new List<MyInventoryItem>();
                cargo.GetInventory().GetItems(c3);
                foreach (MyInventoryItem e2 in c3)
                {
                    // Echo(getName(e2));
                    if (getName(e2) == item) return cargo;
                }
            }
            return null;
        }

        /// <summary>
        /// Sucht ein CargoContainer in dem das angegeben Item mit ein STORAGETAG markiert ist
        /// </summary>
        /// <param name="item">String: Name des Items</param>
        public IMyCargoContainer findTagCargo(string item)
        {
            List<IMyCargoContainer> cargos = new List<IMyCargoContainer>();
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargos);
            foreach (IMyCargoContainer cargo in cargos)
            {
                if (!cargo.CustomData.Contains(STORAGETAG)) continue;
                Dictionary<string, string> info = splitInfos(cargo.CustomData);
                if (info[STORAGETAG] == item) return cargo;
            }
            return null;
        }

        /// <summary>
        /// Sucht den CargoContainer der am leersten ist.
        /// </summary>
        public IMyCargoContainer findEmptyCargo()
        {
            int nr = 0, anz1 = int.MaxValue;
            List<IMyCargoContainer> cargos = new List<IMyCargoContainer>();
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargos);
            for (int i1 = 0; i1 < cargos.Count; i1++)
            {
                int anz2 = cargos[i1].GetInventory().ItemCount;
                if (anz1 < anz2)
                {
                    nr = i1;
                    anz1 = anz2;
                }
            }
            return cargos[nr];
        }

        /// <summary>
        /// Gibt den Anzeigenamen des Item zurück
        /// </summary>
        /// <param name="value">Item aus dem Cargo</param>
        /// <returns></returns>
        public string getName(MyInventoryItem value)
        {
            string nn = value.Type.SubtypeId.ToString();
            if (nn == "Ice") return nn;
            if (value.Type.TypeId.ToString().EndsWith("_Ore")) nn += " Ore";
            else if (value.Type.TypeId.ToString().EndsWith("_Ingot")) nn += " Ingot";
            else if (value.Type.TypeId.ToString().EndsWith("_Component")) nn += " Component";
            return nn;
        }

        /// <summary>
        /// Fasst alle gleiche Items zu ein Stack zusammen
        /// </summary>
        /// <param name="cargo">IMyCargoContainer: Objekt des CargoContainer</param>
        public void ConcentrateCargo(IMyCargoContainer cargo)
        {
            IMyInventory c2 = cargo.GetInventory();
            List<MyInventoryItem> cc = new List<MyInventoryItem>();
            c2.GetItems(cc);
            for (int x = 0; x < cc.Count; x++)
            {
                for (int y = 0; y < cc.Count; y++)
                {
                    if (getName(cc[x]) == getName(cc[y])) c2.TransferItemTo(c2, x, y);
                }
            }
        }

        public void TransferItemTo(IMyTerminalBlock from, IMyTerminalBlock to, string name)
        {
            if (from == null || to == null) return;
            IMyInventory a1 = from.GetInventory(0);
            List<MyInventoryItem> a2 = new List<MyInventoryItem>();
            a1.GetItems(a2);

            Echo("a2=" + a2.Count.ToString());
            IMyInventory b1 = to.GetInventory();
            List<MyInventoryItem> b2 = new List<MyInventoryItem>();
            b1.GetItems(b2);

            Echo("b2=" + b2.Count.ToString());

            int x = 0;
            for (; x < a2.Count; x++) if (getName(a2[x]) == name) break;

            int y = 0;
            for (; y < b2.Count; y++) if (getName(b2[y]) == name) break;
            Echo(string.Format("x={0},y={1}", x, y));

            if (x >= a2.Count) return;
            if (y >= b2.Count) y = 0;
            bool z = a1.TransferItemTo(b1, x);
            Echo(z.ToString());
        }


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


        /*
            List<IMyTerminalBlock> lights = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName("hangar", lights, light => light is IMyInteriorLight);
            switch (argument.ToLower())
            {
                case "on": foreach (IMyInteriorLight e1 in lights) { e1.Enabled = true; } break;
                case "off": foreach (IMyInteriorLight e1 in lights) { e1.Enabled = false; } break;
                case "switch": foreach (IMyInteriorLight e1 in lights) { e1.Enabled = !e1.Enabled; } break;
                case "slow off": zahl = 0; break;
                case "slow on": zahl = 1; break;
            }
            if (zahl != 0)
            {
                Echo(zahl.ToString());
                IMyInteriorLight e1 = (IMyInteriorLight)GridTerminalSystem.GetBlockWithName($"Light h7-{Math.Abs(zahl)}-hangar");
                if (zahl > 0) { e1.Enabled = true; zahl++; }
                if (zahl< 0) { e1.Enabled = false; zahl--; }
                if (zahl< -59 || zahl> 59) zahl = 0;
            }
        }
        public int zahl
        {
            get { int e = 0; int.TryParse(Me.CustomData, out e); return e; }
            set { Me.CustomData = value.ToString(); }
        }
        */



    }
}
