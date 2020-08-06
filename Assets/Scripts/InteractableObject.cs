using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : ScriptableObject
{
    public int IdInFactory = 0;
    //public enum type {connectable, portable, pickalbe, movable}
    public enum placementType {normal, onTileOnly, tileOnly}
    public virtual ObjectType theType { get; set; }
    public Sprite sprite;
}

[CreateAssetMenu]
public class MovableObject : InteractableObject
{
    public override ObjectType theType { get { return ObjectType.movable; }}
    public placementType placement = placementType.onTileOnly;
    public int squarePerMove;
    public float moveSpeed;
    public bool isConditioned;
}

[CreateAssetMenu]
public class PickableObject : InteractableObject
{
    public override ObjectType theType { get { return ObjectType.pickable; } }
    public placementType placement = placementType.onTileOnly;
    public bool isConditioned;
}

[CreateAssetMenu]
public class ConnectableObject : InteractableObject
{
    public override ObjectType theType { get { return ObjectType.connectable; } }
    public placementType placement;
    public bool isButton;
    public OnTileObject keyObject;
    public bool isConditioned;
}
[CreateAssetMenu]
public class PortableObject : InteractableObject
{
    public override ObjectType theType { get { return ObjectType.portable; } }
    public placementType placement;
    public bool isExit;
    public bool isConditioned;
}

