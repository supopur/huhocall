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

    [CalloutInfo("Stolen jet", CalloutProbability.Medium)]

    public class StolenJet : Callout
    {
        private Ped Suspect;
        private Vehicle SuspectVehicle;
        private Blip SuspectBlip;
        private LHandle Pursuit;
        private Vector3 Spawnpoint;
        private bool PursuitCreated;



        public override bool OnBeforeCalloutDisplayed()
        {
            Spawnpoint = new Vector3(-1710f, -2924f, 14f);
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "A security guard reports a stolen jet";
            CalloutPosition = Spawnpoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_01 CITIZENS_REPORT_01 CRIME_GRAND_THEFT_AUTO_01 SUSPECT_IS_01 IN_A_02 JET_01 IN_OR_ON_POSITION", Spawnpoint);

            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            SuspectVehicle = new Vehicle("JET", Spawnpoint);
            SuspectVehicle.IsPersistent = true;

            Suspect = new Ped(SuspectVehicle.GetOffsetPositionFront(5f));
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.WarpIntoVehicle(SuspectVehicle, -1);
            Suspect.Inventory.GiveNewWeapon(new WeaponAsset("weapon_smg"), 100, true);
            Suspect.Inventory.GiveParachute();

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
                PursuitCreated = true;
            }
            if (SuspectVehicle.Health <= 500)
            { 
                Suspect.Tasks.Parachute();
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
