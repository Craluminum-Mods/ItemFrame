using System.Collections.Generic;
using System.Linq;

namespace ItemFrame;

public class TypeDefinition
{
    public string Size { get; set; }
    public string Type { get; set; } = "nothing";
    public string Material { get; set; } = "nothing";

    public TypeDefinition(string type)
    {
        List<string> typeArray = type.Split("-").ToList();

        switch (typeArray.Count)
        {
            case 2:
                Size = typeArray[0];
                Type = typeArray[1];
                break;
            case 3:
                Size = typeArray[0];
                Type = typeArray[1];
                Material = typeArray[2];
                break;
        }
    }
}
