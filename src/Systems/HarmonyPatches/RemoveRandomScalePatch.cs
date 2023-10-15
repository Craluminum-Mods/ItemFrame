using HarmonyLib;
using Vintagestory.GameContent;

namespace ItemFrame;

[HarmonyPatch(typeof(BlockEntityCrate), "rndScale", MethodType.Getter)]
public static class RemoveRandomScalePatch
{
    public static void Postfix(BlockEntityCrate __instance, ref float __result)
    {
        if (__instance?.Block?.Attributes?["isItemframe"].AsBool() == true)
        {
            __result = 1f;
        }
    }
}
