using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{

    private int completedPaths;
    private int abandonedPaths;

    void Start() {
        Invoke("displayResults", 120);
    }

    private void displayResults() {
        Debug.Log("Completed Paths: " + completedPaths);
        Debug.Log("Abandoned Paths: " + abandonedPaths);
    }

    public void Completed() {
        completedPaths++;
    }

    public void Abandoned() {
        abandonedPaths++;
    }
}
