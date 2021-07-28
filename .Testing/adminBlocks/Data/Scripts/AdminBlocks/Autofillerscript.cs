using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SpaceEngineers.Game.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.Game.ModAPI.Ingame;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Library;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.ModAPI.Interfaces.Terminal;
using IMyInventory = VRage.Game.ModAPI.Ingame.IMyInventory;

namespace Sherbert.CreativeAutoFill
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_CargoContainer), false, "CreativeAutofiller")]
    public class AutoFiller : MyGameLogicComponent
    {
        private IMyCargoContainer cargo;

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            cargo = Entity as IMyCargoContainer;

            NeedsUpdate = MyEntityUpdateEnum.EACH_100TH_FRAME;

        }



        public override void UpdateBeforeSimulation100()
        {
            if (MyAPIGateway.Session.IsServer)
            {
                if (cargo.IsFunctional)
                {

                    if (cargo.HasInventory)
                    {

                        MyInventory temp = cargo.GetInventory() as MyInventory;

                        List<VRage.Game.Entity.MyPhysicalInventoryItem> invent = temp.GetItems();

                        //  temp.GetItems(invent);

                        for (int idx = 0; idx < invent.Count; idx++)
                        {
                            var i = invent[idx];

                            string itype = i.Content.TypeId.ToString() + "/" + i.Content.SubtypeId.ToString();
                                                       

                            if (itype.ToString().Contains("MyObjectBuilder_OxygenContainerObject"))
                            {
                                MyObjectBuilder_OxygenContainerObject bottle = i.Content as MyObjectBuilder_OxygenContainerObject;

                                if (bottle.GasLevel != 1)
                                    bottle.GasLevel = 1;    
                            }
                            if (itype.ToString().Contains("MyObjectBuilder_GasContainerObject"))
                            {
                                MyObjectBuilder_GasContainerObject bottle = i.Content as MyObjectBuilder_GasContainerObject;

                                if (bottle.GasLevel != 1)
                                    bottle.GasLevel = 1;
                            }




                        }
                    }

                }
            }            

        }

   


        public override void Close()
        {
            cargo = null;
        }
    }
}