﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;

namespace huhocall.Callouts
{

    [CalloutInfo("Suspects are in a stolen heli", CalloutProbability.Low)]

    public class StolenHeli : Callout
    {
        private Ped Suspect;
        private Ped Suspect2;
        private Ped Rappel;
        private Ped Rappel2;
        private Vehicle SuspectVehicle;
        private Blip SuspectBlip;
        private LHandle Pursuit;
        private Vector3 Spawnpoint;
        private bool PursuitCreated;
        private Vector3 offset;



        public override bool OnBeforeCalloutDisplayed()
        {
            offset = new Vector3(100f, 100f, 100f);
            Spawnpoint = World.GetRandomPositionOnStreet();
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "A pedastrian saw a helicopter";
            CalloutPosition = Spawnpoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_SWAT_UNITS_01 CITIZENS_REPORT_01 CRIME_GRAND_THEFT_AUTO_01 SUSPECT_IS_01 IN_A_02 VEHICLE_CATEGORY_MILITARY_VEHICLE_01 IN_OR_ON_POSITION", Spawnpoint);

            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            SuspectVehicle = new Vehicle("AKULA", Spawnpoint);
            SuspectVehicle.IsPersistent = true;

            Suspect = new Ped(SuspectVehicle.GetOffsetPositionFront(5f));
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.WarpIntoVehicle(SuspectVehicle, -1);
            Suspect.Inventory.GiveNewWeapon(new WeaponAsset("weapon_pistol"), 100, true);
            Suspect.Tasks.Wander();


            Suspect2 = new Ped(SuspectVehicle.GetOffsetPositionFront(5f));
            Suspect2.IsPersistent = true;
            Suspect2.BlockPermanentEvents = true;
            Suspect2.WarpIntoVehicle(SuspectVehicle, 0);
            Suspect2.Inventory.GiveNewWeapon(new WeaponAsset("weapon_microsmg"), 500, true);
            Suspect2.Tasks.FightAgainst(Game.LocalPlayer.Character);


            Rappel = new Ped(SuspectVehicle.GetOffsetPositionFront(5f));
            Rappel.IsPersistent = true;
            Rappel.BlockPermanentEvents = true;
            Rappel.WarpIntoVehicle(SuspectVehicle, 1);
            Rappel.Inventory.GiveNewWeapon(new WeaponAsset("weapon_microsmg"), 1000, true);
            Rappel.Inventory.GiveParachute();
            Rappel.Tasks.FightAgainst(Game.LocalPlayer.Character);


            Rappel2 = new Ped(SuspectVehicle.GetOffsetPositionFront(5f));
            Rappel2.IsPersistent = true;
            Rappel2.BlockPermanentEvents = true;
            Rappel2.WarpIntoVehicle(SuspectVehicle, 2);
            Rappel2.Inventory.GiveNewWeapon(new WeaponAsset("weapon_microsmg"), 1000, true);
            Rappel2.Inventory.GiveParachute();
            Rappel2.Tasks.FightAgainst(Game.LocalPlayer.Character);


            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.Color = System.Drawing.Color.Red;
            SuspectBlip.IsRouteEnabled = true;

            PursuitCreated = false;


            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            base.Process();

            if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(SuspectVehicle) <= 50f)
            {
                Pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(Pursuit, Suspect);
                Functions.AddPedToPursuit(Pursuit, Suspect2);
                Functions.AddPedToPursuit(Pursuit, Rappel);
                Functions.AddPedToPursuit(Pursuit, Rappel2);
                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                PursuitCreated = true;

            }
            if (SuspectVehicle.Health <= 500)
            {
                Rappel.Tasks.Parachute();
                Rappel2.Tasks.Parachute();

            }
            if (PursuitCreated && !Functions.IsPursuitStillRunning(Pursuit))
            {
                End();

                if (Suspect.Exists())
                {
                    Suspect.Dismiss();
                }
                if (SuspectBlip.Exists())
                {
                    SuspectBlip.Delete();
                }
                if (SuspectVehicle.Exists())
                {
                    SuspectVehicle.Dismiss();
                }

                Game.LogTrivial("Suspect in a tank over");
            }


        }
        public override void End()
        {
            base.End();
        }
    }
}
