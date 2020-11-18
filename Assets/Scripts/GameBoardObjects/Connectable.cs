using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connectable : InteractableProperty
{
    public bool isButton;
    bool isConditioned;

    public Connectable connectedObject;
    public List<Connectable> connecteds = new List<Connectable>();

    bool pressed;
    [SerializeField]
    OnTileObject keyObject;

    bool turnOn;

    public enum connectableType { gate, power, nothing }
    public connectableType theCType;

    [SerializeField]
    float gateCloseTime = 0.5f;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public override void GameUpdate()
    {
        PressedCheck();
    }

    public override void Initialize(GameBoardObject gameBoardObject)
    {
        base.Initialize(gameBoardObject);

        //ConnectableObject connectable = interactable as ConnectableObject;
        //isButton = connectable.isButton;
        //keyObject = connectable.keyObject;
        //isConditioned = connectable.isConditioned;

        if (isButton)
            theCType = connectableType.nothing;

        LEditor_ConnectableObject onEditConnectable = gameBoardObject.GetComponent<LEditor_ConnectableObject>();
        if (isButton)
        {
            if (onEditConnectable.connectedObject != null)
            {
                connectedObject = onEditConnectable.connectedObject.GetComponent<Connectable>();
            }
        }
        else
        {
            if (onEditConnectable.connecteds.Count > 0)
            {
                for (int i = 0; i < onEditConnectable.connecteds.Count; i++)
                {
                    connecteds.Add(onEditConnectable.connecteds[i].GetComponent<Connectable>());
                }
            }
        }
    }

    public void PressedCheck()
    {
        if (isButton && !IsOnTile)
        {
            OnTileObject objectOnThis = GetComponent<TileObject>().objectOnThis;
            if (objectOnThis != null)
            {
                if (!pressed)
                {
                    if (isConditioned)
                    {
                        if (objectOnThis.name == keyObject.name)
                        {
                            pressed = true;
                            if (connectedObject != null)
                            {
                                connectedObject.TurnOnAndOff();
                            }
                            Debug.Log(name + " is preseed.");
                        }
                    }
                    else
                    {
                        pressed = true;
                        if (connectedObject != null)
                        {
                            connectedObject.TurnOnAndOff();
                        }
                        Debug.Log(name + " is preseed.");
                    }

                }
            }
            else
            {
                if (pressed)
                {
                    pressed = false;
                    if (connectedObject != null)
                    {
                        connectedObject.TurnOnAndOff();
                    }
                }
            }
        }
        else
        {
            TurnOnAndOff();
        }
    }

    public void TurnOnAndOff()
    {
        for (int i = 0; i < connecteds.Count; i++)
        {
            if (connecteds[i] != null && connecteds[i].isButton)
            {
                if (!connecteds[i].pressed)
                {
                    turnOn = false;

                    Invoke("OnTurnOnEvent", gateCloseTime);
                    return;
                }
            }
        }
        turnOn = true;
        OnTurnOnEvent();
        Debug.Log(name + " is turn On.");
    }

    public void OnTurnOnEvent()
    {
        switch (theCType)
        {
            case connectableType.gate:
                if (turnOn == true)
                {
                    collide.isTrigger = true;

                    if (OverlapedByOnTile())
                    {
                        collide.enabled = false;
                    }

                    GetComponent<OnTileObject>().isHindrance = false;
                    isHindrance = false;
                }
                else
                {
                    collide.isTrigger = false;
                    if (OverlapedByOnTile())
                    {
                        collide.enabled = false;
                    }
                    else
                    {
                        collide.enabled = true;
                    }
                    GetComponent<OnTileObject>().isHindrance = true;
                    isHindrance = true;
                }
                break;
        }
    }

    public bool OverlapedByOnTile()
    {
        collide.enabled = false;
        Collider2D hit = Physics2D.OverlapPoint(transform.position, LevelManager.Instance.gameBoardObjectLayer);
        collide.enabled = true;

        if (hit != null)
        {
            if (hit.tag == "onTileObject" || hit.tag == "player")
            {
                Debug.Log(gameObject.name + " is overlaped by " + hit.name + ".");
                return true;
            }
        }

        return false;
    }

}
