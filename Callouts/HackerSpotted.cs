using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace huhocall.Callouts
{
    [CalloutInfo("Hacker spotted", CalloutProbability.Medium)]
    class HackerSpotted : Callout
    {
        private Ped Suspect;
        private Blip Blip;
        private LHandle Pursuit;
        private Vector3 Spawnpoint;
        private bool PursuitCreated;
        private Vehicle fbi;
        private Ped fib;
        private Ped fib2;

        public override bool OnBeforeCalloutDisplayed()
        {
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(250f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_SWAT_UNITS_01 CITIZENS_REPORT_01 CRIME_SUSPECT_ON_THE_RUN_01 IN_OR_ON_POSITION", Spawnpoint);



            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            fbi = new Vehicle("FBI", Spawnpoint);
            fbi.IsPersistent = true;

            fib = new Ped("mp_m_fibsec_01", Spawnpoint, 0f);
            fib.IsPersistent = true;
            fib.Armor = 700;
            fib.Inventory.GiveNewWeapon("weapon_appistol", 2000, true);
            fib.WarpIntoVehicle(fbi, -1);

            fib2 = new Ped("mp_m_fibsec_01", Spawnpoint, 0f);
            fib2.IsPersistent = true;
            fib2.Armor = 700;
            fib2.Inventory.GiveNewWeapon("weapon_smg", 2000, true);
            fib2.WarpIntoVehicle(fbi, 0);

            Suspect = new Ped(fbi.GetOffsetPositionFront(10f));
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.Inventory.GiveNewWeapon("weapon_combatpdw", 3000, false);
            Suspect.Armor = 500;


            Blip = Suspect.AttachBlip();
            Blip.Color = System.Drawing.Color.Yellow;
            Blip.IsRouteEnabled = true;
            Blip.IsFriendly = false;

            PursuitCreated = false;

            Game.DisplayNotification("Try to arrest the suspect without killing him");
            Game.DisplayNotification("FIB agents are allready on the scene as a backup unit");

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
                Suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                PursuitCreated = true;
            }
            if (PursuitCreated && !Functions.IsPursuitStillRunning(Pursuit))
            {
                End();

                if (Suspect.Exists()) Suspect.Dismiss();
                if (Blip.Exists()) Blip.Delete();
            }


        }
        public override void End()
        {
            base.End();
        }

    }
}
