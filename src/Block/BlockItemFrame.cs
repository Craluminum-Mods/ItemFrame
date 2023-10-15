using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using System.Linq;

namespace ItemFrame;

public class BlockItemFrame : BlockCrate
{
    private WorldInteraction[] interactions;

    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);
        interactions = ObjectCacheUtil.GetOrCreate(api, "itemframeInteractions-", delegate
        {
            List<ItemStack> list = new();
            list.AddRange(api.World.Items.Where(item => item.Attributes?["pigment"]?["color"].Exists == true).Select(item => new ItemStack(item)));
            return new WorldInteraction[3]
              {
                new() { ActionLangCode = "blockhelp-groundstorage-add", MouseButton = EnumMouseButton.Right, HotKeyCode = null },
                new() { ActionLangCode = "blockhelp-groundstorage-remove", MouseButton = EnumMouseButton.Right, HotKeyCode = null },
                new() { ActionLangCode = "heldhelp-draw", MouseButton = EnumMouseButton.Right, HotKeyCode = "shift", Itemstacks = list.ToArray() }
              };
        });
    }

    public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel)
    {
        return world.BlockAccessor.GetBlockEntity(blockSel.Position) is BlockEntityItemFrame beItemFrame
            ? beItemFrame.OnBlockInteractStart(byPlayer, blockSel)
            : OnBlockInteractStart(world, byPlayer, blockSel);
    }

    public override string GetHeldItemName(ItemStack itemStack)
    {
        return Lang.GetMatching($"{Code?.Domain}:block-{Code?.Path}");
    }

    public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
        string type = inSlot.Itemstack.Attributes.GetString("type");
        string label = inSlot.Itemstack.Attributes.GetString("label");
        dsc.Append(new DescriptionBuilder(new TypeDefinition(type), label).GetDescription());
    }

    public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos)
    {
        ItemStack original = base.OnPickBlock(world, pos);
        original.Attributes.RemoveAttribute("lidState");
        return original;
    }

    public override Cuboidf[] GetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos)
    {
        return CollisionBoxes;
    }

    public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos)
    {
        return blockAccessor.GetBlockEntity(pos) is BlockEntityItemFrame beItemFrame
            ? beItemFrame.GetSelectionBoxes()
            : base.GetSelectionBoxes(blockAccessor, pos);
    }

    public override BlockSounds GetSounds(IBlockAccessor blockAccessor, BlockPos pos, ItemStack stack = null)
    {
        return blockAccessor.GetBlockEntity(pos) is BlockEntityItemFrame beItemFrame
            ? beItemFrame.GetSounds()
            : base.GetSounds(blockAccessor, pos, stack);
    }

    public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer)
    {
        return interactions;
    }
}