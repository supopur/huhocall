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

    [CalloutInfo("Suspects are in a armored kuruma", CalloutProbability.Low)]

    public class ArmoredCarPursuit : Callout
    {
        private Ped Suspect;
        private Ped Suspect2;
        private Vehicle SuspectVehicle;
        private Blip SuspectBlip;
        private LHandle Pursuit;
        private Vector3 Spawnpoint;
        private bool PursuitCreated;


        public override bool OnBeforeCalloutDisplayed()
        {
            Spawnpoint = World.GetRandomPositionOnStreet();
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "A pedastrian reported an armored kuruma";
            CalloutPosition = Spawnpoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_SWAT_UNITS_01 CITIZENS_REPORT_01 CRIME_GRAND_THEFT_AUTO_01 SUSPECT_IS_01 IN_A_02 VEHICLE_CATEGORY_MILITARY_VEHICLE_01 IN_OR_ON_POSITION", Spawnpoint);

            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {
            SuspectVehicle = new Vehicle("KURUMA2", Spawnpoint);
            SuspectVehicle.IsPersistent = true;

            Suspect = new Ped(SuspectVehicle.GetOffsetPositionFront(5f));
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.WarpIntoVehicle(SuspectVehicle, -1);
            Suspect.Inventory.GiveNewWeapon(new WeaponAsset("weapon_pistol"), 500, true);
            Suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);

            Suspect2 = new Ped(SuspectVehicle.GetOffsetPositionFront(5f));
            Suspect2.IsPersistent = true;
            Suspect2.BlockPermanentEvents = true;
            Suspect2.WarpIntoVehicle(SuspectVehicle, 0);
            Suspect2.Inventory.GiveNewWeapon(new WeaponAsset("weapon_microsmg"), 500, true);
            Suspect2.Tasks.FightAgainst(Game.LocalPlayer.Character);

            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.Color = System.Drawing.Color.Red;
            SuspectBlip.IsRouteEnabled = true;

            PursuitCreated = false;


            return base.OnCalloutAccepted();
        }
        public override void Process()
        {
            base.Process();

            if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(SuspectVehicle) <= 20f)
            {
                Pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(Pursuit, Suspect);
                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                PursuitCreated = true;

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

                Game.LogTrivial("Kuruma pursuit Code 4");
            }


        }
        public override void End()
        {
            base.End();
        }
    }
}
