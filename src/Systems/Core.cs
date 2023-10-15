using Vintagestory.API.Common;

[assembly: ModInfo(name: "Item Frame", modID: "itemframe")]

namespace ItemFrame;

public class Core : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        base.Start(api);

        api.RegisterBlockClass("ItemFrame:BlockItemFrame", typeof(BlockItemFrame));
        api.RegisterBlockEntityClass("ItemFrame:BlockEntityItemFrame", typeof(BlockEntityItemFrame));

        api.World.Logger.Event("started '{0}' mod", Mod.Info.Name);
    }
}