using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeowDSIO;
using MeowDSIO.DataFiles;
using MeowDSIO.DataTypes;
using System.Reflection;

namespace MetalGearSouls
{
    class Program
    {
        static void Main(string[] args)
        {
            //Load list of params into memory
            List<PARAMDEF> ParamDefs = new List<PARAMDEF>();
            List<PARAM> AllParams = new List<PARAM>();
            var gameFolder = @"C:\Users\mcouture\Desktop\DS-Modding\Dark Souls Prepare to Die Edition\DATA\";

            var gameparamBnds = Directory.GetFiles(gameFolder + "param\\GameParam\\", "*.parambnd")
                .Select(p => DataFile.LoadFromFile<BND>(p, new Progress<(int, int) > ((pr) =>
                {

                })));

            var drawparamBnds = Directory.GetFiles(gameFolder + "param\\DrawParam\\", "*.parambnd")
                .Select(p => DataFile.LoadFromFile<BND>(p, new Progress<(int, int) > ((pr) =>
                {

                })));

            List<BND> PARAMBNDs = gameparamBnds.Concat(drawparamBnds).ToList();

            var paramdefBnds = Directory.GetFiles(gameFolder + "paramdef\\", "*.paramdefbnd")
                .Select(p => DataFile.LoadFromFile<BND>(p, new Progress<(int, int) > ((pr) =>
                {

                }))).ToList();

            for (int i = 0; i < paramdefBnds.Count(); i++)
            {
                foreach (var paramdef in paramdefBnds[i])
                {
                    PARAMDEF newParamDef = paramdef.ReadDataAs<PARAMDEF>(new Progress<(int, int) > ((r) =>
                    {

                    }));
                    ParamDefs.Add(newParamDef);
                }
            }

            for (int i = 0; i < PARAMBNDs.Count(); i++)
            {
                foreach (var param in PARAMBNDs[i])
                {
                    PARAM newParam = param.ReadDataAs<PARAM>(new Progress<(int, int) > ((p) =>
                    {

                    }));

                    newParam.ApplyPARAMDEFTemplate(ParamDefs.Where(x => x.ID == newParam.ID).First());

                    AllParams.Add(newParam);
                }
            }
            //loading complete

            //change all the things
            foreach (PARAM paramFile in AllParams)
            {
                if (paramFile.VirtualUri.EndsWith("AtkParam_Npc.param"))
                {
                    foreach (MeowDSIO.DataTypes.PARAM.ParamRow paramRow in paramFile.Entries)
                    {
                        foreach (MeowDSIO.DataTypes.PARAM.ParamCellValueRef cell in paramRow.Cells)
                        {
                            //max guard break, knockback
                            //do everything to make enemies as difficult to fight as possible
                            if (cell.Def.Name == "guardAtkRate")
                            {
                                Type type = cell.GetType();
                                PropertyInfo prop = type.GetProperty("Value");
                                prop.SetValue(cell, cell.Def.Max, null);
                            }
                            //every hit knocks you down btw
                            else if (cell.Def.Name == "dmgLevel")
                            {
                                Type type = cell.GetType();
                                PropertyInfo prop = type.GetProperty("Value");
                                prop.SetValue(cell, 7, null);
                            }
                        }
                    }
                }
                else if (paramFile.ID == "NPC_THINK_PARAM_ST")
                {
                    foreach (MeowDSIO.DataTypes.PARAM.ParamRow paramRow in paramFile.Entries)
                    {
                        foreach (MeowDSIO.DataTypes.PARAM.ParamCellValueRef cell in paramRow.Cells)
                        {
                            //max these
                            //once enemies see you, they'll chase you like no tomorrow
                            string[] attrsToMax = { "farDist", "outDist", "maxBackhomeDist", "backhomeDist", "backhomeBattleDist", "BackHome_LookTargetTime", "BackHome_LookTargetDist", "BattleStartDist", "ear_angX", "ear_angY", "ear_dist" };
                            if (attrsToMax.Contains(cell.Def.Name))
                            {
                                Type type = cell.GetType();
                                PropertyInfo prop = type.GetProperty("Value");
                                prop.SetValue(cell, cell.Def.Max, null);
                            }
                           
                            string[] attrsToMin = { "nearDist", "midDist", "nose_dist" };
                            if (attrsToMin.Contains(cell.Def.Name))
                            {
                                Type type = cell.GetType();
                                PropertyInfo prop = type.GetProperty("Value");
                                prop.SetValue(cell, cell.Def.Min, null);
                            }

                            //specific values
                            if(cell.Def.Name == "eye_angX")
                            {
                                Type type = cell.GetType();
                                PropertyInfo prop = type.GetProperty("Value");
                                prop.SetValue(cell, 60, null);
                            }

                            if (cell.Def.Name == "eye_angY")
                            {
                                Type type = cell.GetType();
                                PropertyInfo prop = type.GetProperty("Value");
                                prop.SetValue(cell, 60, null);
                            }

                            if (cell.Def.Name == "eye_dist")
                            {
                                Type type = cell.GetType();
                                PropertyInfo prop = type.GetProperty("Value");
                                prop.SetValue(cell, 6, null);
                            }
                        }
                    }
                }
                else if (paramFile.ID == "NPC_PARAM_ST")
                {
                    foreach (MeowDSIO.DataTypes.PARAM.ParamRow paramRow in paramFile.Entries)
                    {
                        foreach (MeowDSIO.DataTypes.PARAM.ParamCellValueRef cell in paramRow.Cells)
                        {
                            if (cell.Def.Name == "turnVellocity")
                            {
                                //something relatively slow, but not so slow you can backstep fish super easily
                                Type type = cell.GetType();
                                PropertyInfo prop = type.GetProperty("Value");
                                prop.SetValue(cell, 90, null);
                            }
                        }
                    }
                }
                else if (paramFile.ID == "CHARACTER_INIT_PARAM")
                {
                    foreach (MeowDSIO.DataTypes.PARAM.ParamRow paramRow in paramFile.Entries)
                    {
                        //change one id entry only (theif)
                        if (paramRow.ID == 3003)
                        {
                            foreach (MeowDSIO.DataTypes.PARAM.ParamCellValueRef cell in paramRow.Cells)
                            {
                                //comp bow
                                if (cell.Def.Name == "equip_Subwep_Right")
                                {
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 1204000, null);
                                }
                                //sorc catalyst
                                else if (cell.Def.Name == "equip_Subwep_Left")
                                {
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 1300000, null);
                                }
                                //feather arrows
                                else if (cell.Def.Name == "equip_Arrow")
                                {
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 2002000, null);
                                }
                                //feather arrow quantity
                                else if (cell.Def.Name == "arrowNum")
                                {
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 999, null);
                                }
                                //start with black firebombs
                                else if (cell.Def.Name == "item_01")
                                {
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 297, null);
                                }
                                //99 black firebombs
                                else if (cell.Def.Name == "itemNum_01")
                                {
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 99, null);
                                }
                                //alluring skulls
                                else if (cell.Def.Name == "item_02")
                                {
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 294, null);
                                }
                                //alluring skulls quantity                         
                                else if (cell.Def.Name == "itemNum_02")
                                {
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 99, null);
                                }
                                //slumbering ring
                                else if (cell.Def.Name == "equip_Accessory01")
                                {
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 123, null);
                                }
                                //chameleon
                                else if (cell.Def.Name == "equip_Spell_01")
                                {
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 3550, null);
                                }

                            }
                        }
                    }
                }
                else if (paramFile.ID == "BULLET_PARAM_ST")
                {
                    foreach (MeowDSIO.DataTypes.PARAM.ParamRow paramRow in paramFile.Entries)
                    {
                        if (paramRow.ID == 502)
                        {
                            foreach (MeowDSIO.DataTypes.PARAM.ParamCellValueRef cell in paramRow.Cells)
                            {
                                if (cell.Def.Name == "GravityOutRange")
                                {
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 0, null);
                                }
                                else if (cell.Def.Name == "initVellocity")
                                {
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 100, null);
                                }
                                else if (cell.Def.Name == "maxVellocity")
                                {
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 100, null);
                                }
                            }
                        }
                    }
                }
                else if (paramFile.ID == "EQUIP_PARAM_WEAPON_ST")
                {
                    foreach (MeowDSIO.DataTypes.PARAM.ParamRow paramRow in paramFile.Entries)
                    {
                        if (paramRow.ID == 1204000)
                        {
                            foreach (MeowDSIO.DataTypes.PARAM.ParamCellValueRef cell in paramRow.Cells)
                            {
                                if (cell.Def.Name == "bowDistRate")
                                {
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 10, null);
                                }
                            }
                        }
                        else if (paramRow.ID == 100000)
                        {
                            foreach (MeowDSIO.DataTypes.PARAM.ParamCellValueRef cell in paramRow.Cells)
                            {
                                if (cell.Def.Name == "throwAtkRate")
                                {
                                    //backstabbu
                                    Type type = cell.GetType();
                                    PropertyInfo prop = type.GetProperty("Value");
                                    prop.SetValue(cell, 50, null);
                                }
                            }
                        }
                    }
                }
                else if (paramFile.ID == "THROW_INFO_BANK")
                {
                    foreach (MeowDSIO.DataTypes.PARAM.ParamRow paramRow in paramFile.Entries)
                    {
                        foreach (MeowDSIO.DataTypes.PARAM.ParamCellValueRef cell in paramRow.Cells)
                        {
                            if (cell.Def.Name == "Dist")
                            {
                                Type type = cell.GetType();
                                PropertyInfo prop = type.GetProperty("Value");
                                prop.SetValue(cell, 3, null);
                            }
                            else if (cell.Def.Name == "diffAngMax")
                            {
                                Type type = cell.GetType();
                                PropertyInfo prop = type.GetProperty("Value");
                                prop.SetValue(cell, 75, null);
                            }
                            else if (cell.Def.Name == "upperYRange")
                            {
                                Type type = cell.GetType();
                                PropertyInfo prop = type.GetProperty("Value");
                                prop.SetValue(cell, 0.5, null);
                            }
                            else if (cell.Def.Name == "lowerYRange")
                            {
                                Type type = cell.GetType();
                                PropertyInfo prop = type.GetProperty("Value");
                                prop.SetValue(cell, 0.65, null);
                            }
                        }
                    }
                }
            }


            //Resave params as BNDs
            foreach (var paramBnd in PARAMBNDs)
            {
                foreach (var param in paramBnd)
                {
                    var filteredParamName = param.Name.Substring(param.Name.LastIndexOf("\\") + 1).Replace(".param", "");

                    var matchingParam = AllParams.Where(x => x.VirtualUri == param.Name).First();

                    param.ReplaceData(matchingParam,
                        new Progress<(int, int) > ((p) =>
                        {

                        }));
                }

                DataFile.Resave(paramBnd, new Progress<(int, int) > ((p) =>
                {

                }));
            }
        }
    }
}
