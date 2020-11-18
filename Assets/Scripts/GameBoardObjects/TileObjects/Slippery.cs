using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slippery : InteractableProperty {


    [SerializeField]
    float slidingSpeed = 1;

    [SerializeField]
    float basicDistance = 1f;

    [SerializeField]
    float meetHinderanceReduction = 0.85f;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void GameUpdate()
    {
        
    }


    public override void Interact(Player interacter)
    {
        base.Interact(interacter);

        Debug.Log("interacting with a Slippery.");

        Vector3 direction = new Vector3(interacter.Horizontal, interacter.Vertical, interacter.transform.position.z);        

        if (GetComponent<TileObject>().objectOnThis == interacter.GetComponent<OnTileObject>() && !interacter.isSliding)
        {
            Vector3 targetPositon = transform.position;
            if (direction.x != 0 && direction.y == 0)
            {
                targetPositon = interacter.transform.position + SlidingDistance(direction, "x") * new Vector3(direction.x / Mathf.Abs(direction.x), 0, 0);
            }
            else if (direction.x == 0 && direction.y != 0)
            {
                targetPositon = interacter.transform.position + SlidingDistance(direction, "y") * new Vector3(0, direction.y / Mathf.Abs(direction.y), 0);
            }

            Debug.Log("Start sliding.");
            interacter.StartSliding(targetPositon, slidingSpeed);
        }
    }

    public void SlideStoppedPlayer(Player interacter, int x, int y)
    {
        Vector3 targetPosition = new Vector3(int.MaxValue, int.MinValue, interacter.transform.position.z);
        Vector3 Direction = new Vector3(x, y, interacter.transform.position.z);
        if (x != 0)
        {
            targetPosition = interacter.transform.position + SlidingDistance(Direction, "x") * Direction;
        }
        else if (y != 0)
        {
            targetPosition = interacter.transform.position + SlidingDistance(Direction, "y") * Direction;
        }

        if (targetPosition != new Vector3(int.MaxValue, int.MinValue, interacter.transform.position.z))
        {
            interacter.StartSliding(targetPosition, slidingSpeed);
        }
    }


    float SlidingDistance(Vector3 direction, string axis)
    {
        float slidingDistance = basicDistance;
        int tileId = GetComponent<TileObject>().TileId;

        int row = LevelManager.Instance.currentGameBoard.Row;
        int column = LevelManager.Instance.currentGameBoard.Column;

        if (axis == "x")
        {
            if (direction.x >= 0)
            {
                int NumToRighEdge = (row - (tileId % row) - 1);

                for (int i = 1; i < NumToRighEdge; i++)
                {
                    if (LevelManager.Instance.currentGameBoard.GetTile(tileId + i).theType == ObjectType.slippery)
                    {
                        slidingDistance += 1;
                    }
                    
                }
            }
            else
            {
                int NumToLeftEdge = (tileId % row);

                for (int i = 1; i < NumToLeftEdge; i++)
                {
                    if (LevelManager.Instance.currentGameBoard.GetTile(tileId - i).theType == ObjectType.slippery)
                    {
                        slidingDistance += 1;
                    }
                    
                }
            }
        }
        else if (axis == "y")
        {
            if (direction.y >= 0)
            {
                int NumToTopEdge = (int)Mathf.Floor(tileId / row);

                for (int i = 1; i < NumToTopEdge; i++)
                {
                    if (LevelManager.Instance.currentGameBoard.GetTile(tileId - row * i).theType == ObjectType.slippery)
                    {
                        slidingDistance += 1;
                    }
                    else
                    {
                     
                    }
                }
            }
            else
            {
                int NumToBottomEdge = column - 1 - (int)Mathf.Floor(tileId / row);

                for (int i = 1; i < NumToBottomEdge; i++)
                {
                    if (LevelManager.Instance.currentGameBoard.GetTile(tileId + row * i).theType == ObjectType.slippery)
                    {
                        slidingDistance += 1;
                    }
                    else
                    {
                     
                    }
                }
            }
        }

        return slidingDistance;
    }

}
