using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfind : MonoBehaviour
{

    private bool pathFindingAlg = false;
    private Tracker tracker;
    private Rigidbody npc;
    /*
    0: No Transfer
    1: Left Bridge
    2: Middle Bridge
    3: Right Bridge
    4: Left Teleporter
    5: Right Teleporter
    */
    private int transfer = 0;
    private bool transferring = false;
    private bool reached = false;
    private float timer = 0;
    private int currentFloor;
    private Vector2 startCoords;
    private int destFloor;
    private Vector2 destCoords;
    private Vector3 destPos;
    private Vector2 transCoords;
    private Vector3 transPos;
    private int[,] fFloor = new int[29,12];
    private int[,] bFloor = new int[29,15];
    private int[,] uFloor = new int[29,12];
    private Cell[,] aPath;
    private Cell[,] fPath = new Cell[29,12];
    private Cell[,] bPath = new Cell[29,15];
    private Cell[,] uPath = new Cell[29,12];
    private Queue<Vector3> path = new Queue<Vector3>();
    private int[,] steps = {{-1,-1},{-1,0},{-1,1},{0,1},{0,-1},{1,-1},{1,0},{1,1}};

    void Start()
    {
        pathFindingAlg = GameObject.Find("GameScript").GetComponent<SetupGame>().pathFindingAlg;
        tracker = GameObject.Find("GameScript").GetComponent<Tracker>();
        npc = gameObject.GetComponent<Rigidbody>();
        (fFloor, bFloor, uFloor) = GameObject.Find("GameScript").GetComponent<SetupGame>().getBaseGrid();
        SetDestination();
    }

    void Update() {
        // Move NPC if destination not reached
        if (!reached) {
            // Using A Star
            if (pathFindingAlg) {
                timer += Time.deltaTime;
                if (transferring) {
                    return;
                }
                if (path.Count != 0) {
                    // If stuck abandon path
                    if (timer > 15f) {
                        tracker.Abandoned();
                        reached = true;
                        timer = 0;
                        Invoke("SetDestination", 1f);
                        return;
                    }
                    // If reached destination get a new destination to path to
                    if (transform.position == destPos) {
                        tracker.Completed();
                        reached = true;
                        timer = 0;
                        Invoke("SetDestination", 1f);
                        return;
                    }
                    // Move NPC along its path
                    if (transform.position != path.Peek()) {
                        transform.position = Vector3.MoveTowards(transform.position, path.Peek(), 4f * Time.deltaTime);
                        npc.MovePosition(transform.position);
                    }
                    // If last node keep pathing to it (ensures transfer)
                    else {
                        if (path.Count > 1) {
                            path.Dequeue();
                        }
                    }
                }
            }
            // Using "Potential Fields"
            else {
                timer += Time.deltaTime;
                if (transferring) {
                    return;
                }
                // If stuck abandon path
                if (timer > 15f) {
                    tracker.Abandoned();
                    reached = true;
                    timer = 0;
                    Invoke("SetDestination", 1f);
                    return;
                }
                // Move NPC along its path (to destination)
                if (currentFloor == destFloor) {
                    // If reached destination get a new destination to path to
                    if (transform.position == destPos) {
                        tracker.Completed();
                        reached = true;
                        timer = 0;
                        Invoke("SetDestination", 1f);
                        return;
                    }
                    transform.position = Vector3.MoveTowards(transform.position, destPos, 4f * Time.deltaTime);
                    npc.MovePosition(transform.position);
                }
                // Move NPC along its path (to transfer
                else {
                    transform.position = Vector3.MoveTowards(transform.position, transPos, 4f * Time.deltaTime);
                    npc.MovePosition(transform.position);
                }
            }
        }
    }

    // Set a random destination for the NPC
    private void SetDestination() {
        bool destSet = false;
        while (!destSet) {
            int floor = Random.Range(0,3);
            // On Front Floor
            if (floor == 1) {
                int x = Random.Range(0,29);
                int z = Random.Range(0,12);
                if (fFloor[x,z] == 0) {
                    destCoords = new Vector2(x,z);
                    destPos = new Vector3(x-13.5f, 1.25f, z-17.5f);
                    destFloor = 1;
                    destSet = true;
                }
            }
            // On Back Floor
            else if (floor == 2) {
                int x = Random.Range(0,29);
                int z = Random.Range(0,15);
                if (bFloor[x,z] == 0) {
                    destCoords = new Vector2(x,z);
                    destPos = new Vector3(x-13.5f, 1.25f, z+3.5f);
                    destFloor = 2;
                    destSet = true;
                }
            }
            // On Up Floor
            else {
                int x = Random.Range(0,29);
                int z = Random.Range(0,12);
                if (uFloor[x,z] == 0) {
                    destCoords = new Vector2(x,z);
                    destPos = new Vector3(x-13.5f, 10.25f, z+6.5f);
                    destFloor = 3;
                    destSet = true;
                }
            }
        }
        GetPathPlan();
    }

    // Set starting, transfer and destination positions
    private void GetPathPlan() {
        if (transform.position.y > 9) {
            startCoords = new Vector2(transform.position.x + 13.5f, transform.position.z - 6.5f);
            currentFloor = 3;
        }
        else {
            if (transform.position.z > 0) {
                startCoords = new Vector2(transform.position.x + 13.5f, transform.position.z - 3.5f);
                currentFloor = 2;
            }
            else {
                startCoords = new Vector2(transform.position.x + 13.5f, transform.position.z + 17.5f);
                currentFloor = 1;
            }
        }

        if (currentFloor != destFloor) {
            if (currentFloor == 1) {
                if (destFloor == 2) {
                    transfer = 2;
                    transCoords = new Vector2(14, 11);
                    transPos = new Vector3(0.5f, 1.25f, -6.5f);
                }
                else {
                    if (transform.position.x < 0.5) {
                        transfer = 1;
                        transCoords = new Vector2(6, 11);
                        transPos = new Vector3(-7.5f, 1.25f, -6.5f);
                    }
                    else {
                        transfer = 3;
                        transCoords = new Vector2(22, 11);
                        transPos = new Vector3(8.5f, 1.25f, -6.5f);
                    }
                }
            }
            else if (currentFloor == 2) {
                if (destFloor == 1) {
                    transfer = 2;
                    transCoords = new Vector2(14, 0);
                    transPos = new Vector3(0.5f, 1.25f, 3.5f);
                }
                else {
                    if (transform.position.x < 0.5) {
                        transfer = 4;
                        transCoords = new Vector2(6, 8);
                        transPos = new Vector3(-7.5f, 1.25f, 12f);
                    }
                    else {
                        transfer = 5;
                        transCoords = new Vector2(22, 8);
                        transPos = new Vector3(8.5f, 1.25f, 12f);
                    }
                }
            }
            else {
                if (destFloor == 1) {
                    if (transform.position.x < 0.5) {
                        transfer = 1;
                        transCoords = new Vector2(6, 0);
                        transPos = new Vector3(-7.5f, 10.25f, 6.5f);
                    }
                    else {
                        transfer = 3;
                        transCoords = new Vector2(22, 0);
                        transPos = new Vector3(8.5f, 10.25f, 6.5f);
                    }
                }
                else {
                    if (transform.position.x < 0.5) {
                        transfer = 4;
                        transCoords = new Vector2(6, 6);
                        transPos = new Vector3(-7.5f, 10.25f, 12f);
                    }
                    else {
                        transfer = 5;
                        transCoords = new Vector2(22, 6);
                        transPos = new Vector3(8.5f, 10.25f, 12f);
                    }
                }
            }
        }
        reached = false;
        // If using A Star get its path
        if (pathFindingAlg) {
            GetAStarPath();
        }
    }

    // Get the path that A Star computed
    private void GetAStarPath() {
        path.Clear();
        List<Cell> p = AStar();
        // Offset coords to reflect actual (x,y,z) position
        for (int i = 0; i < p.Count; i++) {
            if (currentFloor == 1) {
                path.Enqueue(new Vector3(p[i].x-13.5f, 1.25f, p[i].y-17.5f));
            }
            else if (currentFloor == 2) {
                path.Enqueue(new Vector3(p[i].x-13.5f, 1.25f, p[i].y+3.5f));
            }
            else {
                path.Enqueue(new Vector3(p[i].x-13.5f, 10.25f, p[i].y+6.5f));
            }
        }
    }

    // A Star algorithm
    private List<Cell> AStar() {
        Vector2 endCoords = new Vector2();
        // Determine start and end coords
        if (currentFloor == destFloor) {
            endCoords = destCoords;
        }
        else {
            endCoords = transCoords;
        }
        // Determine which floors grid to use
        if (currentFloor == 1) {
            aPath = fPath;
        }
        else if (currentFloor == 2) {
            aPath = bPath;
        }
        else {
            aPath = uPath;
        }
        // Initialize Cell array
        for (int i = 0; i < aPath.GetLength(0); i++) {
            for (int j = 0; j < aPath.GetLength(1); j++) {
                aPath[i,j] = new Cell(i,j);
            }
        }

        // Set g cost of start to 0, calculate h cost and f cost
        aPath[(int)startCoords.x,(int)startCoords.y].g = 0f;
        aPath[(int)startCoords.x,(int)startCoords.y].h = (endCoords-startCoords).magnitude;
        aPath[(int)startCoords.x,(int)startCoords.y].CalculateF();
        // Create open and closed list
        List<Cell> openList = new List<Cell>{aPath[(int)startCoords.x,(int)startCoords.y]};
        List<Cell> closedList = new List<Cell>();

        // Keep checking open list until empty
        while (openList.Count > 0) {
            // Get lowest f cost of open list
            Cell currentCell = GetLowestFCell(openList);
            // If its our destination we are done (return the path)
            if (currentCell == aPath[(int)endCoords.x,(int)endCoords.y]) {
                return GetPath(currentCell);
            }
            // If not our destination transfer cell to closed list
            openList.Remove(currentCell);
            closedList.Add(currentCell);
            //  Check all neighbors and calculate each g,h,f costs
            foreach (Cell neighborCell in GetNeighbors(currentCell)) {
                if (!closedList.Contains(neighborCell)) {
                    float newG = currentCell.g + (new Vector2(currentCell.x,currentCell.y)-new Vector2(neighborCell.x,neighborCell.y)).magnitude;
                    if (newG < neighborCell.g) {
                        neighborCell.previousCell = currentCell;
                        neighborCell.g = newG;
                        neighborCell.h = (destCoords-new Vector2(neighborCell.x,neighborCell.y)).magnitude;
                        neighborCell.CalculateF();
                        // Add to open list if not already in it
                        if (!openList.Contains(neighborCell)) {
                            openList.Add(neighborCell);
                        }
                    }
                }
            }
        }
        // No path found
        return null;
    }

    // Get neighbors of current cell
    private List<Cell> GetNeighbors(Cell currentCell) {
        List<Cell> neighbors = new List<Cell>();
        int[,] aFloor;
        // Determine which floor grid to use
        if (currentFloor == 1) {
            aFloor = fFloor;
        }
        else if (currentFloor == 2) {
            aFloor = bFloor;
        }
        else {
            aFloor = uFloor;
        }

        // For all possible steps check if valid neighbor
        for (int i = 0; i < steps.GetLength(0); i++) {
            var x = currentCell.x + steps[i,0];
            var z = currentCell.y + steps[i,1];
            if (0 <= x && x < aFloor.GetLength(0) && 0 <= z && z < aFloor.GetLength(1) && aFloor[x,z] != 9)  {
                neighbors.Add(aPath[x,z]);
                if ((aFloor[x,z] == 2 && currentFloor == 3) && (transfer != 4 && transfer != 5)) {
                    neighbors.Remove(aPath[x,z]);
                }
                if ((aFloor[x,z] == 3 && currentFloor == 2) && (transfer != 4 && transfer != 5)) {
                    neighbors.Remove(aPath[x,z]);
                }
            }
        }
        return neighbors;

    }

    // Returns the Cell path computed by A Star
    private List<Cell> GetPath(Cell destCell) {
        List<Cell> path = new List<Cell>();
        path.Add(destCell);
        Cell currentCell = destCell;
        // Backtrack until theres no more previous cell
        while (currentCell.previousCell != null) {
            path.Add(currentCell.previousCell);
            currentCell = currentCell.previousCell;
        }
        // Reverse the backtracked order and return
        path.Reverse();
        return path;
    }

    // find lowest f cost from given cell list
    private Cell GetLowestFCell(List<Cell> cells) {
        Cell lowestFCell = cells[0];
        for (int i = 1; i < cells.Count; i++) {
            if (cells[i].f < lowestFCell.f) {
                lowestFCell = cells[i];
            }
        }
        return lowestFCell;
    }

    // Getter for transfer
    public int getTransfer() {
        return transfer;
    }

    // Setter for transferring
    public void setTransferring(bool b) {
        transferring = b;
        transfer = 0;
        // If using A Star clear current path
        if (pathFindingAlg) {
            path.Clear();
        }
    }

    // Setter for currentFloor
    public void setCurrentFloor(int n) {
        currentFloor = n;
        // If using A Star get a new path plan / compute it
        if (pathFindingAlg) {
            GetPathPlan();
            GetAStarPath();
        }
    }
}