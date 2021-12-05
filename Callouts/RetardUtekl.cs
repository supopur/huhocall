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
    [CalloutInfo("Mentally ill person escaped", CalloutProbability.Medium)]
    class RetardUtekl : Callout
    {
        private Ped Suspect;
        private Blip Blip;
        private LHandle Pursuit;
        private Vector3 Spawnpoint;
        private bool PursuitCreated;

        public override bool OnBeforeCalloutDisplayed()
        {
            Spawnpoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(250f));
            ShowCalloutAreaBlipBeforeAccepting(Spawnpoint, 30f);
            Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_01 CITIZENS_REPORT_01 CRIME_BRANDISHING_WEAPON_01 CRIME_RESIST_ARREST_02 IN_OR_ON_POSITION", Spawnpoint);



            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Suspect = new Ped(Spawnpoint);
            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.Inventory.GiveNewWeapon("weapon_nightstick", 1, true);
            Suspect.Armor = 500;

            Blip = Suspect.AttachBlip();
            Blip.Color = System.Drawing.Color.Yellow;
            Blip.IsRouteEnabled = true;

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
