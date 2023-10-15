using HarmonyLib;
using Vintagestory.GameContent;
using Vintagestory.API.Client;

namespace ItemFrame;

[HarmonyPatch(typeof(BlockCrate), "genContentMesh")]
public static class ContentMeshPatch
{
    public static void Postfix(BlockCrate __instance, ref MeshData __result)
    {
        if (__instance?.Attributes?["isItemframe"].AsBool() == true)
        {
            __result = null;
        }
    }
}