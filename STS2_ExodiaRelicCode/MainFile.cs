using System.Reflection;
using BaseLib.Abstracts;
using BaseLib.Patches.Localization;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Modding;

namespace STS2_ExodiaRelic.STS2_ExodiaRelicCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "STS2_ExodiaRelic"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        // 1. Initialize Harmony
        Harmony harmony = new(ModId);

        // 2. Patch only YOUR assembly to avoid the Steamworks/Reflection crashes.
        // This picks up GlobalEventPatch and any future patches you write.
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        GD.Print("[Exodia Mod] Harmony.PatchAll executed.");
        
        // 3. Enable BaseLib's SimpleLoc for your mod.
        // This allows you to use # and *tags* in your JSON files for easy formatting.
        SimpleLoc.EnableSimpleLoc(ModId);

        Logger.Info($"{ModId} initialized successfully with JSON localization.");
        
    }
    
}