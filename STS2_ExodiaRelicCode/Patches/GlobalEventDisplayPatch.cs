using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Commands;
using STS2_ExodiaRelic.STS2_ExodiaRelicCode.Cards; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace STS2_ExodiaRelic.STS2_ExodiaRelicCode.Patches;

[HarmonyPatch(typeof(EventModel), "SetEventState")]
public static class GlobalEventDisplayPatch
{
    private const string Key = "EXODIA.pages.INITIAL.options.UNLEASH";
    private const string Res = "EXODIA.pages.INITIAL.description";

    // This is vibecoded, high risk of throwing up when reading this
    [HarmonyPrefix]
    public static void Prefix(EventModel __instance, LocString description, ref IEnumerable<EventOption> eventOptions)
    {
        // 1. ANCIENT FILTER (Ignore Neow and friends)
        if (__instance == null || __instance is AncientEventModel || eventOptions == null) return;

        // 2. STICKY RNG REPORTING
        int seed = __instance.GetHashCode();
        var rng = new Random(seed);
        double roll = rng.NextDouble();
        
        // Keeping the log exactly as requested
        GD.Print($"[Exodia Trace] RNG Roll: {roll:F2} (Need <= 0.30) | Seed: {seed}");

        if (roll > 0.30) return;

        // 3. STATE GUARDS
        var isFin = AccessTools.Property(typeof(EventModel), "IsFinished")?.GetValue(__instance) as bool? ?? false;
        if (isFin || eventOptions.Any(o => o?.TextKey == Key)) return;

        try 
        {
            var baseOption = eventOptions.FirstOrDefault(o => o != null);
            if (baseOption == null) return;

            // 4. CLONE THE BUTTON
            var cloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
            var clone = (EventOption)cloneMethod.Invoke(baseOption, null);

            // 5. DEFINE NEW BEHAVIOR
            Func<Task> exodiaAction = async () => {
                GD.Print("[Exodia] !!! ACTION TRIGGERED: Giving Job Application !!!");
                
                // Add the card
                await CardPileCmd.AddCurseToDeck<JobApplication>(__instance.Owner);
                
                // Finish the event
                var l10nMethod = typeof(EventModel).GetMethod("L10NLookup", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var finishText = (LocString)l10nMethod.Invoke(__instance, new[] { Res });
                
                AccessTools.Method(typeof(EventModel), "SetEventFinished").Invoke(__instance, new[] { finishText });
            };

            // 6. SCORCHED EARTH: Replace all behavior fields
            // This kills the "Regression" where it copied the first option's effect
            var allFields = typeof(EventOption).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var f in allFields) {
                if (f.FieldType == typeof(Func<Task>) || f.FieldType == typeof(Action)) {
                    f.SetValue(clone, exodiaAction);
                    GD.Print($"[Exodia] Overwrote behavior field: {f.Name}");
                }
            }

            // 7. INJECT TEXT
            AccessTools.Property(typeof(EventOption), "TextKey").SetValue(clone, Key);
            
            var l10n = (Func<string, LocString>)((s) => (LocString)AccessTools.Method(typeof(EventModel), "L10NLookup").Invoke(__instance, new[] { s }));
            var locFields = allFields.Where(f => f.FieldType == typeof(LocString)).ToList();

            if (locFields.Count > 0) locFields[0].SetValue(clone, l10n(Key)); // Title
            if (locFields.Count > 1) locFields[1].SetValue(clone, l10n(Res)); // Description

            // 8. PUSH TO UI
            eventOptions = eventOptions.Concat(new[] { clone });
            GD.Print($"[Exodia] SUCCESS: Custom button added to {__instance.GetType().Name}");
        }
        catch (Exception e) 
        { 
            GD.PrintErr($"[Exodia] CRASH: {e.Message}\n{e.StackTrace}"); 
        }
    }
}