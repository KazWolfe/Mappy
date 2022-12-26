using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Mappy.Util;

namespace Mappy.System;

public unsafe class AreaMapAddon : IDisposable
{
    private const string AddonName = "AreaMap";

    public bool AddonActive => true;
    
    public Vector2 GetWindowPosition()
    {
        var baseNode = new BaseNode(AddonName);
        var rootNode = baseNode.GetRootNode();
        
        return rootNode is null ? Vector2.Zero : (*rootNode).GetPosition();
    }
    
    public Vector2 GetWindowSize()
    {
        var baseNode = new BaseNode(AddonName);
        var rootNode = baseNode.GetRootNode();
        
        return rootNode is null ? Vector2.Zero : (*rootNode).GetSize();
    }

    public Vector2 GetMapPosition()
    {
        var baseNode = new BaseNode(AddonName);
        var mapNode = baseNode.GetComponentNode(44);
        var mapPosition = mapNode.GetPosition();

        return GetWindowPosition() + mapPosition;
    }

    public Vector2 GetMapSize()
    {
        var baseNode = new BaseNode(AddonName);
        var mapNode = baseNode.GetComponentNode(44);
        var mapSize = mapNode.GetSize();

        return mapSize;
    }

    public Vector2 GetMapTopLeft()
    {
        var baseNode = new BaseNode(AddonName);
        var mapNode = baseNode.GetComponentNode(44);
        var imageNode = mapNode.GetNode<AtkImageNode>(8);
        if(imageNode is null) return Vector2.Zero;

        var partsList = imageNode->PartsList;
        if(partsList is null) return Vector2.Zero;

        var parts = partsList->Parts;
        if(parts is null) return Vector2.Zero;

        var part = parts[0];

        return new Vector2(part.Width - 2048, part.Height - 2048);
    }

    public void Dispose()
    {
        
    }
}

public static unsafe class NodeExtensions
{
    public static Vector2 GetPosition(this ComponentNode node) =>
        node.GetPointer() is null
            ? Vector2.Zero
            : node.GetPointer()->AtkResNode.GetPosition();

    public static Vector2 GetSize(this ComponentNode node) =>
        node.GetPointer() is null
            ? Vector2.Zero
            : node.GetPointer()->AtkResNode.GetSize();

    public static Vector2 GetPosition(this AtkResNode node) => new(node.X, node.Y);
    public static Vector2 GetSize(this AtkResNode node) => new(node.Width, node.Height);
}
