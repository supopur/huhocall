using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using LSPD_First_Response.Mod.API;
using System.Reflection;

namespace huhocall
{
    public class Main: Plugin
    {

        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
            Game.LogTrivial("Human labs callouts loading");
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LSPDFRResolveEventHandler);
        }

        public override void Finally()
        {
            Game.LogTrivial("Human labs callouts has been cleaned up");
        }

        private static void OnOnDutyStateChangedHandler(bool OnDuty)
        {
            if (OnDuty)
            {
                RegisterCallouts();
                Game.DisplayNotification("If you can see this rage plugin hook is fine");
            }
        }

        private static void RegisterCallouts()
        {
            Functions.RegisterCallout(typeof(Callouts.SuspectInATank));
            Functions.RegisterCallout(typeof(Callouts.ArmoredCarPursuit));
            Functions.RegisterCallout(typeof(Callouts.SerialKillerInArmor));
            Functions.RegisterCallout(typeof(Callouts.StolenHeli));
            Functions.RegisterCallout(typeof(Callouts.HeliOverRestAirspace));
            Functions.RegisterCallout(typeof(Callouts.RetardUtekl));
            Functions.RegisterCallout(typeof(Callouts.HackerSpotted));
            Functions.RegisterCallout(typeof(Callouts.StolenJet));
            Functions.RegisterCallout(typeof(Callouts.StolenLazer));
            Functions.RegisterCallout(typeof(Callouts.AlienPursuit));
            Functions.RegisterCallout(typeof(Callouts.AlienShooting));

        }

        public static Assembly LSPDFRResolveEventHandler(object sender, ResolveEventArgs args)
        {
            foreach (Assembly assembly in Functions.GetAllUserPlugins())
            {
                if (args.Name.ToLower().Contains(assembly.GetName().Name.ToLower()))
                {
                    return assembly;
                }
            }
            return null;
        }

        public static bool IsLSPDFRPluginRunning(string Plugin, Version minversion = null)
        {
            foreach (Assembly assembly in Functions.GetAllUserPlugins())
            {
                AssemblyName an = assembly.GetName();
                if (an.Name.ToLower() == Plugin.ToLower())
                {
                    if (minversion == null || an.Version.CompareTo(minversion) >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
