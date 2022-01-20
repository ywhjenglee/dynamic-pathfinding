using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupGame : MonoBehaviour
{

    public bool pathFindingAlg = false;
    public GameObject square;
    public GameObject pentagon;
    public GameObject hexagon;
    public GameObject npc;
    public int npcCount;
    private GameObject[] shapes = new GameObject[3];
    private GameObject[] obstacles;
    private int[,] frontFloor = new int[29,12];
    private int[,] backFloor = new int[29,15];
    private int[,] upFloor = new int[29,12];
    private int[,] fFloor = new int[29,12];
    private int[,] bFloor = new int[29,15];
    private int[,] uFloor = new int[29,12];

    void Start()
    {
        shapes[0] = square;
        shapes[1] = pentagon;
        shapes[2] = hexagon;
        CreateObstacles();
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        FillGrid();
        fFloor = frontFloor.Clone() as int[,];
        bFloor = backFloor.Clone() as int[,];
        uFloor = upFloor.Clone() as int[,];
        SpawnNPCs();
    }

    // Randomly place obstacles in a given region
    private void CreateObstacles()
    {
        // Front Floor
        Instantiate(shapes[Random.Range(0,3)], new Vector3(Random.Range(-12,-9), 0.75f, Random.Range(-16,-7)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        Instantiate(shapes[Random.Range(0,3)], new Vector3(Random.Range(-8,-6), 0.75f, Random.Range(-16,-8)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        Instantiate(shapes[Random.Range(0,3)], new Vector3(Random.Range(-5,0), 0.75f, Random.Range(-16,-7)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        Instantiate(shapes[Random.Range(0,3)], new Vector3(Random.Range(2,7), 0.75f, Random.Range(-16,-7)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        Instantiate(shapes[Random.Range(0,3)], new Vector3(Random.Range(8,10), 0.75f, Random.Range(-16,-8)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        Instantiate(shapes[Random.Range(0,3)], new Vector3(Random.Range(11,14), 0.75f, Random.Range(-16,-7)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));

        // Back Floor
        // Left Side
        if (Random.Range(0f,1f) < 0.5f) {
            Instantiate(shapes[Random.Range(0,3)], new Vector3(Random.Range(-12,0), 0.75f, Random.Range(5,10)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        }
        else {
            Instantiate(shapes[Random.Range(0,3)], new Vector3(Random.Range(-4,0), 0.75f, Random.Range(11,17)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        }
        // Right Side
        if (Random.Range(0f,1f) < 0.5f) {
            Instantiate(shapes[Random.Range(0,3)], new Vector3(Random.Range(2,14), 0.75f, Random.Range(5,10)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        }
        else {
            Instantiate(shapes[Random.Range(0,3)], new Vector3(Random.Range(2,6), 0.75f, Random.Range(11,17)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        }

        // Up Floor
        Instantiate(shapes[Random.Range(0,3)], new Vector3(Random.Range(-4,6), 9.75f, Random.Range(8,12)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        Instantiate(shapes[Random.Range(0,3)], new Vector3(Random.Range(-4,6), 9.75f, Random.Range(13,17)), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
    }

    // Mark grid with relevant information
    /*
    1: Moves to frontFloor
    2: Moves to backFloor
    3: Moves to upFloor
    */
    private void FillGrid()
    {
        // Fill Bridge and Teleporter zones
        // upFloor
        upFloor[6,0] = 1;
        upFloor[22,0] = 1;
        upFloor[5,5] = 2;
        upFloor[5,6] = 2;
        upFloor[6,5] = 2;
        upFloor[6,6] = 2;
        upFloor[7,5] = 2;
        upFloor[7,6] = 2;
        upFloor[21,5] = 2;
        upFloor[21,6] = 2;
        upFloor[22,5] = 2;
        upFloor[22,6] = 2;
        upFloor[23,5] = 2;
        upFloor[23,6] = 2;
        // backFloor
        backFloor[14,0] = 1;
        backFloor[5,8] = 3;
        backFloor[5,9] = 3;
        backFloor[6,8] = 3;
        backFloor[6,9] = 3;
        backFloor[7,8] = 3;
        backFloor[7,9] = 3;
        backFloor[21,8] = 3;
        backFloor[21,9] = 3;
        backFloor[22,8] = 3;
        backFloor[22,9] = 3;
        backFloor[23,8] = 3;
        backFloor[23,9] = 3;
        // frontFloor
        frontFloor[6,11] = 3;
        frontFloor[14,11] = 2;
        frontFloor[22,11] = 3;

        // Fill obstacles (each take a 2x2 grid)
        foreach (GameObject obstacle in obstacles) {
            // upFloor
            if (obstacle.transform.position.y > 9) {
                int x = (int) obstacle.transform.position.x + 13;
                int z = (int) obstacle.transform.position.z - 7;
                upFloor[x,z] = 9;
                upFloor[x+1,z] = 9;
                upFloor[x,z+1] = 9;
                upFloor[x+1,z+1] = 9;
            }
            else {
                // backFloor
                if (obstacle.transform.position.z > 0) {
                    int x = (int) obstacle.transform.position.x + 13;
                    int z = (int) obstacle.transform.position.z - 4;
                    backFloor[x,z] = 9;
                    backFloor[x+1,z] = 9;
                    backFloor[x,z+1] = 9;
                    backFloor[x+1,z+1] = 9;
                }
                // frontFloor
                else {
                    int x = (int) obstacle.transform.position.x + 13;
                    int z = (int) obstacle.transform.position.z + 17;
                    frontFloor[x,z] = 9;
                    frontFloor[x+1,z] = 9;
                    frontFloor[x,z+1] = 9;
                    frontFloor[x+1,z+1] = 9;
                }
            }
        }
    }

    // Randomly spawn NPCs according to npcCount
    private void SpawnNPCs()
    {
        int i = 0;
        while (i < npcCount) {
            int floor = Random.Range(0,3);
            // Spawn at Front Floor
            if (floor == 1) {
                int x = Random.Range(0,29);
                int z = Random.Range(0,12);
                if (fFloor[x,z] == 0) {
                    fFloor[x,z] = 5;
                    GameObject newNPC = Instantiate(npc, new Vector3(x-13.5f, 1.25f, z-17.5f), Quaternion.identity);
                    newNPC.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f));
                    i++;
                }
            }
            // Spawn at Back Floor
            else if (floor == 2) {
                int x = Random.Range(0,29);
                int z = Random.Range(0,15);
                if (bFloor[x,z] == 0) {
                    bFloor[x,z] = 5;
                    GameObject newNPC = Instantiate(npc, new Vector3(x-13.5f, 1.25f, z+3.5f), Quaternion.identity);
                    newNPC.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f));
                    i++;
                }
            }
            // Spawn at Up Floor
            else {
                int x = Random.Range(0,29);
                int z = Random.Range(0,12);
                if (uFloor[x,z] == 0) {
                    uFloor[x,z] = 5;
                    GameObject newNPC = Instantiate(npc, new Vector3(x-13.5f, 10.25f, z+6.5f), Quaternion.identity);
                    newNPC.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f));
                    i++;
                }
            }
        }
    }

    // Getter for the grid without NPCs
    public (int[,], int[,], int[,]) getBaseGrid() {
        return (frontFloor, backFloor, upFloor);
    }
}
