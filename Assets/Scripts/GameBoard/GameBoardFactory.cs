using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardFactory : MonoBehaviour {

    [SerializeField]
    List<GameBoard> GameBoards;


    public GameBoard GetGameBoard(GameBoardThem theme)
    {
        return GameBoards[(int)theme];
    }

    
}
