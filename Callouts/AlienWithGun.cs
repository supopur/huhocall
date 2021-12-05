using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Drawing;
using System;

namespace huhocall.Callouts
{

    [CalloutInfo("Alien shooting", CalloutProbability.Low)]

    public class AlienShooting : Callout
    {
        private Ped Suspect;
        private Ped Suspect2;
        private Blip SuspectBlip;
        private LHandle Pursuit;
        private Vector3 Spawnpoint;
        private bool PursuitCreated;


        public override bool OnBeforeCalloutDisplayed()
        {
            Spawnpoint = World.GetRandomPositionOnStreet();
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            AddMinimumDistanceCheck(30f, Spawnpoint);
            CalloutMessage = "Alien escaped from human labs";
            CalloutPosition = Spawnpoint;
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_SWAT_UNITS_01 CITIZENS_REPORT_01 CRIME_GRAND_THEFT_AUTO_01 SUSPECT_IS_01 IN_OR_ON_POSITION", Spawnpoint);

            return base.OnBeforeCalloutDisplayed();
        }
        public override bool OnCalloutAccepted()
        {

            Suspect = new Ped(Spawnpoint);
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.Inventory.GiveNewWeapon(new WeaponAsset("weapon_raypistol"), 9999, true);
            Suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);

            Suspect2 = new Ped(Spawnpoint);
            Suspect2.IsPersistent = true;
            Suspect2.BlockPermanentEvents = true;
            Suspect2.Inventory.GiveNewWeapon(new WeaponAsset("weapon_raycarbine"), 9999, true);
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

            if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(Suspect) <= 20f)
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
                if (Suspect2.Exists())
                {
                    Suspect2.Dismiss();
                }
                if (SuspectBlip.Exists())
                {
                    SuspectBlip.Delete();
                }

                Game.LogTrivial("Alien pursuit Code 4");
            }
            if (!Suspect.IsAlive)
            {
                SuspectBlip.Delete();
            }
        }
        public override void End()
        {
            base.End();
        }
    }
}
