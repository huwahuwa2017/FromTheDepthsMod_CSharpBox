using CSharpBox;
using UnityEngine;

public class Program
{
    public static LuaBinding Lua;

    private static void Start(CSharpBoxClass cSharpBox)
    {
        Lua = new LuaBinding(cSharpBox.MainConstruct as MainConstruct);
    }

    private static void Update(CSharpBoxClass cSharpBox)
    {
        Transform myTransform = cSharpBox.MainConstruct.GameObject.myTransform;

        myTransform.position = new Vector3(0, 10, 0);
        myTransform.rotation = Quaternion.Euler(0, 0, 0);

        //Vector3 position = Lua.GetConstructPosition();
        //Lua.LogToHud("Position : " + position.ToString());
    }
}
