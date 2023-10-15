using Vintagestory.API.Common;

namespace ItemFrame;

public class ItemFrameSlot : ItemSlot
{
    public ItemFrameSlot(InventoryBase inventory) : base(inventory) { }

    public override int MaxSlotStackSize => 1;
}