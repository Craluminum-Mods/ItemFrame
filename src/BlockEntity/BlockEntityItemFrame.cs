using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ItemFrame;

public class BlockEntityItemFrame : BlockEntityCrate
{
    public TypeDefinition Definition => new(type);
    public DescriptionBuilder DescriptionBuilder => new(Definition, label);

    public Cuboidf[] SelectionBoxesFromSize => Block?.Attributes?["selectionBoxesFromSize"][Definition.Size].AsObject<Cuboidf[]>();

    private Cuboidf selBox;

    public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
    {
        dsc.Append(DescriptionBuilder.GetDescription());

        switch (Inventory[0].StackSize)
        {
            case > 0:
                dsc.AppendLine(Lang.Get("Contents: {0}", Inventory.FirstNonEmptySlot.GetStackName()));
                break;
            default:
                dsc.AppendLine(Lang.Get("Empty"));
                break;
        }
    }

    public new Cuboidf[] GetSelectionBoxes()
    {
        float rotAngleY = this.GetField<float>("rotAngleY");

        if (selBox == null)
        {
            selBox = SelectionBoxesFromSize[0].RotatedCopy(0f, rotAngleY * (180f / MathF.PI), 0f, new Vec3d(0.5, 0.0, 0.5));
        }
        return new Cuboidf[1] { selBox };
    }

    public BlockSounds GetSounds()
    {
        return Block.Attributes["blockSoundsFromType"]?[Definition.Type].AsObject<BlockSounds>()
            ?? Block.Attributes["blockSoundsFromType"]["*"].AsObject<BlockSounds>();
    }

    public InventoryGeneric InventoryField
    {
        get => this.GetField<InventoryGeneric>("inventory");
        set => this.SetField("inventory", value);
    }

    public int LabelColorField
    {
        get => this.GetField<int>("labelColor");
        set => this.SetField("labelColor", value);
    }

    public MeshData LabelMeshField
    {
        get => this.GetField<MeshData>("labelMesh");
        set => this.SetField("labelMesh", value);
    }

    public ItemStack LabelStackField
    {
        get => this.GetField<ItemStack>("labelStack");
        set => this.SetField("labelStack", value);
    }

    public new bool OnBlockInteractStart(IPlayer byPlayer, BlockSelection blockSel)
    {
        bool shiftKey = byPlayer.Entity.Controls.ShiftKey;
        ItemSlot firstNonEmptySlot = InventoryField.FirstNonEmptySlot;
        ItemSlot activeHotbarSlot = byPlayer.InventoryManager.ActiveHotbarSlot;

        if (shiftKey && !activeHotbarSlot.Empty && activeHotbarSlot.Itemstack?.ItemAttributes?["pigment"]?["color"].Exists == true)
        {
            if (!InventoryField.Empty)
            {
                JsonObject jsonObject = activeHotbarSlot.Itemstack.ItemAttributes["pigment"]["color"];
                int num = jsonObject["red"].AsInt();
                int num2 = jsonObject["green"].AsInt();
                int num3 = jsonObject["blue"].AsInt();
                LabelColorField = ColorUtil.ToRgba(255, (int)GameMath.Clamp((float)num * 1.2f, 0f, 255f), (int)GameMath.Clamp((float)num2 * 1.2f, 0f, 255f), (int)GameMath.Clamp((float)num3 * 1.2f, 0f, 255f));
                LabelStackField = InventoryField.FirstNonEmptySlot.Itemstack.Clone();
                LabelMeshField = null;
                byPlayer.Entity.World.PlaySoundAt(new AssetLocation("sounds/player/chalkdraw"), (double)blockSel.Position.X + blockSel.HitPosition.X, (double)blockSel.Position.Y + blockSel.HitPosition.Y, (double)blockSel.Position.Z + blockSel.HitPosition.Z, byPlayer, randomizePitch: true, 8f);
                MarkDirty(redrawOnClient: true);
                activeHotbarSlot.MarkDirty();
            }
            else
            {
                (Api as ICoreClientAPI)?.TriggerIngameError(this, "empty", Lang.Get("Can't draw item symbol on an empty crate. Put something inside the crate first"));
            }
            return true;
        }
        if (firstNonEmptySlot != null)
        {
            ItemStack itemStack = firstNonEmptySlot.TakeOut(1);
            if (!byPlayer.InventoryManager.TryGiveItemstack(itemStack, slotNotifyEffect: true))
            {
                Api.World.SpawnItemEntity(itemStack, Pos.ToVec3d().Add(0.5f + blockSel.Face.Normalf.X, 0.5f + blockSel.Face.Normalf.Y, 0.5f + blockSel.Face.Normalf.Z));
            }
            else
            {
                didMoveItems(itemStack, byPlayer);
            }
            if (InventoryField.Empty)
            {
                LabelMeshField = null;
            }
            firstNonEmptySlot.MarkDirty();
            MarkDirty();
            return true;
        }
        if (!activeHotbarSlot.Empty)
        {
            if (firstNonEmptySlot == null && activeHotbarSlot.TryPutInto(Api.World, InventoryField[0], 1) > 0)
            {
                didMoveItems(InventoryField[0].Itemstack, byPlayer);
            }
            activeHotbarSlot.MarkDirty();
            MarkDirty();
        }
        return true;
    }
}