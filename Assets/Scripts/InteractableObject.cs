using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : ScriptableObject
{
    //public enum type {connectable, portable, pickalbe, movable}
    public enum placementType {normal, onTileOnly, tileOnly}
    public virtual condition theType { get; set; }
    public Sprite sprite;
}

[CreateAssetMenu]
public class MovableObject : InteractableObject
{
    public override condition theType { get { return condition.movable; }}
    public placementType placement = placementType.onTileOnly;
    public int squarePerMove;
    public float moveSpeed;
    public bool isConditioned;
}

[CreateAssetMenu]
public class PickableObject : InteractableObject
{
    public override condition theType { get { return condition.pickable; } }
    public placementType placement = placementType.onTileOnly;
    public bool isConditioned;
}

[CreateAssetMenu]
public class ConnectableObject : InteractableObject
{
    public override condition theType { get { return condition.connectable; } }
    public placementType placement;
    public bool isButton;
    public OnTileObject keyObject;
    public bool isConditioned;
}
[CreateAssetMenu]
public class PortableObject : InteractableObject
{
    public override condition theType { get { return condition.portable; } }
    public placementType placement;
    public bool isExit;
    public bool isConditioned;
}

