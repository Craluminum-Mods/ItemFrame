using System.Text;
using Vintagestory.API.Config;

namespace ItemFrame;

public class DescriptionBuilder
{
    public TypeDefinition Definition { get; set; }
    public string LabelSize { get; set; }

    public DescriptionBuilder(TypeDefinition definition, string labelSize = null)
    {
        Definition = definition;
        LabelSize = labelSize;
    }

    public StringBuilder GetDescription()
    {
        StringBuilder stringBuilder = new();

        if (!string.IsNullOrEmpty(LabelSize))
        {
            stringBuilder.AppendLine(Lang.Get("itemframe:size", Lang.Get($"itemframe:{LabelSize}")));
        }

        if (Definition.Material != "nothing")
        {
            stringBuilder.AppendLine(Lang.Get("Material: {0}", Lang.Get($"material-{Definition.Material}")));
        }

        stringBuilder.AppendLine();

        return stringBuilder;
    }
}