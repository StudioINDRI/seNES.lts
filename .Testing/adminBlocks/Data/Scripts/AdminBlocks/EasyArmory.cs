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

namespace Sherbert.AdminEasyArmory
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_CargoContainer), false, "AdminEasyArmory")]
    public class Armory : MyGameLogicComponent
    {
        private IMyCargoContainer cargo;

        private IMyPlayer player;

        private List<IMyPlayer> allPlayers = new List<IMyPlayer>();

        private int cooldown = 0;


        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            cargo = Entity as IMyCargoContainer;   

            NeedsUpdate = MyEntityUpdateEnum.EACH_10TH_FRAME;

        }
  
  
        public override void UpdateBeforeSimulation10()
        {
            if (MyAPIGateway.Session.IsServer)
            {

                if (cooldown > 0)
                    cooldown--;

                if (cargo.IsFunctional && cooldown <= 0)
                {
                    player = null;
                    allPlayers.Clear();
                    MyAPIGateway.Multiplayer.Players.GetPlayers(allPlayers);

                    foreach (var i in allPlayers)
                    {
                        double dist = (i.GetPosition() - cargo.GetPosition()).Length();
                        if (dist <= 1.3)
                        {
                            player = i;
                            break;
                        }                 
                    }


                    if (player != null && player.Character != null)
                    {

                        // VRage.Game.ModAPI.IMyInventory temp = cargo.GetInventory();

                        MyInventory temp = cargo.GetInventory() as MyInventory;

                        List<VRage.Game.Entity.MyPhysicalInventoryItem> invent = temp.GetItems();

                        //  temp.GetItems(invent);

                        for (int idx = invent.Count - 1; idx >= 0; idx--)
                        {
                            var i = invent[idx];

                            string itype = i.Content.TypeId.ToString() + "/" + i.Content.SubtypeId.ToString();
                            if (player.Character.CurrentMovementState != MyCharacterMovementEnum.Crouching)
                            {
                                if (i.Amount >= 1)
                                {
                                    MyDefinitionId itemType = new MyDefinitionId(i.Content.TypeId, i.Content.SubtypeId);

                                    int currentInv = MyVisualScriptLogicProvider.GetPlayersInventoryItemAmount(player.IdentityId, itemType);

                                    if (currentInv < i.Amount)
                                    {
                                        int transferAmount = (int)i.Amount - currentInv ;                             

                                        MyVisualScriptLogicProvider.AddToPlayersInventory(player.IdentityId, itemType, transferAmount);

                                        if (itype.ToString().Contains("MyObjectBuilder_OxygenContainerObject"))
                                        {
                                            MyObjectBuilder_OxygenContainerObject bottle = i.Content as MyObjectBuilder_OxygenContainerObject;
                                            MyInventory playerinv = player.Character.GetInventory() as MyInventory;
                                            List<VRage.Game.Entity.MyPhysicalInventoryItem> playerItems = playerinv.GetItems();
                                            foreach (var j in playerItems)
                                            {
                                                if ((j.Content.TypeId.ToString() + "/" + j.Content.SubtypeId.ToString()).Contains("MyObjectBuilder_OxygenContainerObject"))
                                                {
                                                    MyObjectBuilder_OxygenContainerObject pBottle = j.Content as MyObjectBuilder_OxygenContainerObject;
                                                    if (pBottle.GasLevel == 0)
                                                    {
                                                        pBottle.GasLevel = bottle.GasLevel;
                                                        break;
                                                    }
                                                }
                                            }


                                        }
                                        if (itype.ToString().Contains("MyObjectBuilder_GasContainerObject"))
                                        {
                                            MyObjectBuilder_GasContainerObject bottle = i.Content as MyObjectBuilder_GasContainerObject;
                                            MyInventory playerinv = player.Character.GetInventory() as MyInventory;
                                            List<VRage.Game.Entity.MyPhysicalInventoryItem> playerItems = playerinv.GetItems();
                                            foreach (var j in playerItems)
                                            {
                                                if ((j.Content.TypeId.ToString() + "/" + j.Content.SubtypeId.ToString()).Contains("MyObjectBuilder_GasContainerObject"))
                                                {
                                                    MyObjectBuilder_GasContainerObject pBottle = j.Content as MyObjectBuilder_GasContainerObject;
                                                    if (pBottle.GasLevel == 0)
                                                    {
                                                        pBottle.GasLevel = bottle.GasLevel;
                                                        break;
                                                    }
                                                }
                                            }

                                        }

                                    }

                                }
                            }
                            else
                            {
                                
                                cooldown = 30;

                            }
                        }

                    }
                }
            }

        }     

        private bool HasLogic(IMyTerminalBlock block)
        {
            return block ==cargo as IMyTerminalBlock;
        }

        private bool IsFriendly(IMyPlayer player)
        {
           return cargo.HasPlayerAccess(player.IdentityId);          
        }

        public override void Close()
        {
            cargo = null;
            allPlayers = null;
        }
    }
}