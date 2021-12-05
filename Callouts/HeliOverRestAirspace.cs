using System;
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

    [CalloutInfo("Heli over rest airpspace", CalloutProbability.Medium)]

    public class HeliOverRestAirspace : Callout
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
        Vector3 rappelTarget = new Vector3(-2000f, 2977f, 152f);
        Vector3 rappel2Target = new Vector3(-2061f, 3043f, 32f);



        public override bool OnBeforeCalloutDisplayed()
        {
            Spawnpoint = new Vector3(-2000f, 3050f, 200f);
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "A pedastrian saw a helicopter";
            CalloutPosition = Spawnpoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_SWAT_UNITS_01 CITIZENS_REPORT_01 CRIME_GRAND_THEFT_AUTO_01 SUSPECT_IS_01 IN_A_02 VEHICLE_CATEGORY_MILITARY_VEHICLE_01 IN_OR_ON_POSITION", Spawnpoint);

            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            SuspectVehicle = new Vehicle("POLMAV", Spawnpoint);
            SuspectVehicle.IsPersistent = true;

            Suspect = new Ped(SuspectVehicle.GetOffsetPositionFront(5f));
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.WarpIntoVehicle(SuspectVehicle, -1);
            Suspect.Inventory.GiveNewWeapon(new WeaponAsset("weapon_pistol"), 100, true);
            Suspect.Tasks.DriveToPosition(Spawnpoint, 0f, VehicleDrivingFlags.Normal);


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
            


            Rappel2 = new Ped(SuspectVehicle.GetOffsetPositionFront(5f));
            Rappel2.IsPersistent = true;
            Rappel2.BlockPermanentEvents = true;
            Rappel2.WarpIntoVehicle(SuspectVehicle, 2);
            Rappel2.Inventory.GiveNewWeapon(new WeaponAsset("weapon_microsmg"), 1000, true);
            Rappel2.Inventory.GiveParachute();
            


            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.Color = System.Drawing.Color.Red;
            SuspectBlip.IsRouteEnabled = true;

            PursuitCreated = false;


            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            base.Process();


            if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(SuspectVehicle) <= 100f)
            {
                Pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(Pursuit, Suspect);
                Functions.AddPedToPursuit(Pursuit, Suspect2);
                Functions.AddPedToPursuit(Pursuit, Rappel);
                Functions.AddPedToPursuit(Pursuit, Rappel2);
                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                Rappel.Tasks.ParachuteToTarget(rappel2Target);
                Rappel2.Tasks.ParachuteToTarget(rappelTarget);
                Rappel2.Tasks.FightAgainst(Game.LocalPlayer.Character);
                Rappel.Tasks.FightAgainst(Game.LocalPlayer.Character);
                PursuitCreated = true;

            }

            if (!SuspectVehicle.HasDriver)
            {
                Suspect.WarpIntoVehicle(SuspectVehicle, -1);
            }
            
            if (!PursuitCreated && !Suspect.IsAlive)
            {
                if (Suspect.Exists())
                {
                    Suspect.Dismiss();
                }
                if (Suspect2.Exists())
                {
                    Suspect2.Dismiss();
                }
                if (Rappel.Exists())
                {
                    Rappel.Dismiss();
                }
                if (Rappel2.Exists())
                {
                    Rappel2.Dismiss();
                }
                if (SuspectBlip.Exists())
                {
                    SuspectBlip.Delete();
                }
                if (SuspectVehicle.Exists())
                {
                    SuspectVehicle.Dismiss();
                }
            }


        }
        public override void End()
        {
            base.End();
        }
    }
}
