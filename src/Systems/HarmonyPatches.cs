using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace ItemFrame;

public partial class HarmonyPatches : ModSystem
{
    public const string HarmonyID = "craluminum2413.itemframe";
    public static Harmony HarmonyInstance { get; set; } = new Harmony(HarmonyID);

    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        HarmonyInstance.Patch(original: typeof(BlockCrate).GetMethod("genContentMesh", AccessTools.all), postfix: typeof(ContentMeshPatch).GetMethod(nameof(ContentMeshPatch.Postfix)));
        HarmonyInstance.Patch(original: typeof(BlockEntityCrate).GetMethod("InitInventory", AccessTools.all), prefix: typeof(InitInventoryPatch).GetMethod(nameof(InitInventoryPatch.Prefix)));
        HarmonyInstance.Patch(original: AccessTools.PropertyGetter(typeof(BlockEntityCrate), "rndScale"), postfix: typeof(RemoveRandomScalePatch).GetMethod(nameof(RemoveRandomScalePatch.Postfix)));
    }

    public override void Dispose()
    {
        HarmonyInstance.Unpatch(original: typeof(BlockCrate).GetMethod("genContentMesh", AccessTools.all), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: typeof(BlockEntityCrate).GetMethod("InitInventory", AccessTools.all), type: HarmonyPatchType.All, HarmonyID);
        HarmonyInstance.Unpatch(original: AccessTools.PropertyGetter(typeof(BlockEntityCrate), "rndScale"), type: HarmonyPatchType.All, HarmonyID);
        base.Dispose();
    }
}