using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PuzzleManager : MonoBehaviour
{
    // if not a scriptable object class. 
    public static PuzzleManager Instance { get; private set; }

    public List<PuzzleData> horizontalPieceDatas;
    public List<PuzzleData> verticalPieceDatas;
    public List<PuzzleData> rotationalPieceDatas;
    public List<PuzzleData> obstaclePieceDatas;

    [System.Serializable]
    public struct PuzzleRoom
    {
        public GameObject puzzleRoomPrefabContainer;
        [Tooltip("These are the Puzzle Pieces that makes up the Room you've sent as Container")]
        public roomPiecePartitions roomPartitions;

        public int totalTriggerToHit;

        [Tooltip("Could be in the form of experience or otherwise")]
        public int reward;
    }

    [System.Serializable]
    public struct roomPiecePartitions
    {
        [Tooltip("Horizontal Piece")]
        public bool hasH_Piece;
        [Tooltip("Vertical Piece")]
        public bool hasV_Piece;
        [Tooltip("Rotating Piece")]
        public bool hasR_Piece;
        [Tooltip("Obstacle Piece")]
        public bool hasO_Piece; 

        [HideInInspector]
        public List<GameObject> horizontalPieces;
        [HideInInspector]
        public List<GameObject> verticalPieces;
        [HideInInspector]
        public List<GameObject> rotationalPieces;
        [HideInInspector]
        public List<GameObject> obstaclePieces; 
    }
    public List<PuzzleRoom> puzzleRooms;

    internal PuzzleRoom selectedRoom;

    // This keeps the reference to our solver for solver operations. 
    [SerializeField]
    private PuzzleSolveTrigger solveTriggerObject;

    private void Awake()
    {
        Instance = this;
    }

    [Button]
    public void SetUpRoom()
    {
        selectedRoom = puzzleRooms[Random.Range(0, puzzleRooms.Count)];
        ClearPartitions(selectedRoom);

        ConfigureRoom();

        // instantiate the Room.
        Instantiate(selectedRoom.puzzleRoomPrefabContainer);
    }

    private void ClearPartitions(PuzzleRoom selectedRoom)
    {
        selectedRoom.roomPartitions.rotationalPieces.Clear();
        selectedRoom.roomPartitions.horizontalPieces.Clear();
        selectedRoom.roomPartitions.verticalPieces.Clear();
    }

    /// <summary>
    /// Randomly set all the randomizer functions of the puzzle room. 
    /// </summary>
    public void ConfigureRoom()
    {
        int randomizer;

        #region SETTING UP PARTITIONS RANDOM BEHAVIORS 
        // getting the Pieces objects. 
        // 0 for vertical, 1 horizontal, 2 rotational

        if (selectedRoom.roomPartitions.hasV_Piece)
        {
            int verticalCount = selectedRoom.puzzleRoomPrefabContainer.transform.GetChild(0).childCount;
            for (int i = 0; i < verticalCount; i++)
            {
                selectedRoom.roomPartitions.verticalPieces.
                    Add(selectedRoom.puzzleRoomPrefabContainer.transform.GetChild(0).GetChild(i).gameObject);
            }
            for (int i = 0; i < selectedRoom.roomPartitions.verticalPieces.Count; i++)
            {
                randomizer = Random.Range(0, verticalPieceDatas.Count);
                selectedRoom.roomPartitions.verticalPieces[i]
                    .GetComponent<PuzzlePiece>().pieceData = verticalPieceDatas[randomizer];
            }
        }

        if (selectedRoom.roomPartitions.hasH_Piece)
        {
            int horizontalCount = selectedRoom.puzzleRoomPrefabContainer.transform.GetChild(1).childCount;
            for (int i = 0; i < horizontalCount; i++)
            {
                selectedRoom.roomPartitions.horizontalPieces.
                    Add(selectedRoom.puzzleRoomPrefabContainer.transform.GetChild(1).GetChild(i).gameObject);
            }

            for (int i = 0; i < selectedRoom.roomPartitions.horizontalPieces.Count; i++)
            {
                randomizer = Random.Range(0, horizontalPieceDatas.Count);
                selectedRoom.roomPartitions.horizontalPieces[i]
                    .GetComponent<PuzzlePiece>().pieceData = horizontalPieceDatas[randomizer];
            }
        }

        if (selectedRoom.roomPartitions.hasR_Piece)
        {
            int rotationalCount = selectedRoom.puzzleRoomPrefabContainer.transform.GetChild(2).childCount;
            for (int i = 0; i < rotationalCount; i++)
            {
                selectedRoom.roomPartitions.rotationalPieces.
                    Add(selectedRoom.puzzleRoomPrefabContainer.transform.GetChild(2).GetChild(i).gameObject);
            }

            for (int i = 0; i < selectedRoom.roomPartitions.rotationalPieces.Count; i++)
            {
                randomizer = Random.Range(0, rotationalPieceDatas.Count);
                selectedRoom.roomPartitions.rotationalPieces[i]
                    .GetComponent<PuzzlePiece>().pieceData = rotationalPieceDatas[randomizer];
            }
        }
        #endregion

        #region SETTING UP ROOM TRIGGERS 

        // after all configs. 

        // the puzzle solver should always be the last child
        solveTriggerObject = selectedRoom.puzzleRoomPrefabContainer.transform.GetChild(
            selectedRoom.puzzleRoomPrefabContainer.transform.childCount - 1).GetComponent<PuzzleSolveTrigger>();

        solveTriggerObject.EstablishSolveTriggers();
        #endregion
    }

    public void CheckTrigger()
    {
        solveTriggerObject.TriggerActivated();
    }
    public void RemoveTrigger()
    {
        solveTriggerObject.TriggerDeactivated();
    }
}
