using HarmonyLib;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using System.Collections.Generic;

namespace ItemFrame;

[HarmonyPatch(typeof(BlockEntityCrate), "InitInventory")]
public static class InitInventoryPatch
{
    public static bool Prefix(BlockEntityCrate __instance, Block block)
    {
        if (block?.Attributes?["isItemframe"].AsBool() == false)
        {
            return true;
        }

        string type = __instance.type;

        __instance.quantitySlots = 1;
        __instance.retrieveOnly = false;

        InventoryGeneric inventory = new(1, null, null, onNewSlot: (slotId, self) => new ItemFrameSlot(self));
        inventory.BaseWeight = 1f;
        inventory.OnGetSuitability = (ItemSlot sourceSlot, ItemSlot targetSlot, bool isMerge) => (isMerge ? (inventory.BaseWeight + 3f) : (inventory.BaseWeight + 1f)) + (float)((sourceSlot.Inventory is InventoryBasePlayer) ? 1 : 0);
        inventory.OnGetAutoPullFromSlot = blockFacing => __instance.CallMethod<ItemSlot>("GetAutoPullFromSlot", blockFacing);
        inventory.OnGetAutoPushIntoSlot = (blockFacing, itemSlot) => __instance.CallMethod<ItemSlot>("GetAutoPushIntoSlot", blockFacing, itemSlot);
        if (block?.Attributes != null)
        {
            if (block.Attributes["spoilSpeedMulByFoodCat"][type].Exists)
            {
                inventory.PerishableFactorByFoodCategory = block.Attributes["spoilSpeedMulByFoodCat"][type].AsObject<Dictionary<EnumFoodCategory, float>>();
            }
            if (block.Attributes["transitionSpeedMul"][type].Exists)
            {
                inventory.TransitionableSpeedMulByType = block.Attributes["transitionSpeedMul"][type].AsObject<Dictionary<EnumTransitionType, float>>();
            }
        }
        inventory.PutLocked = false;
        inventory.OnInventoryClosed += (player) => __instance.CallMethod<OnInventoryClosedDelegate>("OnInvClosed", player);
        inventory.OnInventoryOpened += (player) => __instance.CallMethod<OnInventoryOpenedDelegate>("OnInvOpened", player);

        __instance.SetField("inventory", inventory);

        return false;
    }
}