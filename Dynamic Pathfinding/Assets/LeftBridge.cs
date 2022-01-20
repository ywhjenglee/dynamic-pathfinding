using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftBridge : MonoBehaviour
{

    private bool inUse = false;

    void Update() {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("NPC")) {
            var gs = go.GetComponent<Pathfind>();
            // Make sure the NPC wants to transfer. If so put bridge inUse
            if (gs.getTransfer() == 1 && !inUse && (go.transform.position - new Vector3(-7.5f, 5.75f, 0f)).magnitude < 7.95f) {
                inUse = true;
                // Determine where NPC transfers to
                if (go.transform.position.z > 0) {
                    gs.setTransferring(true);
                    StartCoroutine(Move(go, 1));
                }
                else {
                    gs.setTransferring(true);
                    StartCoroutine(Move(go, 3));
                }
            }
        }
    }

    // Make NPC cross bridge
    IEnumerator Move(GameObject go, int floor) {
        var currentPos = go.transform.position;
        var gs = go.GetComponent<Pathfind>();
        Vector3 dir;
        if (floor == 1) {
            dir = new Vector3(0, -9, -13);
        }
        else {
            dir = new Vector3(0, 9, 13);
        }
        // Move NPC
        var finalPos = go.transform.position + dir;
        var t = 0f;
        while (t < 1) {
            t += Time.deltaTime / 3;
            go.transform.position = Vector3.Lerp(currentPos, finalPos, t);
            go.GetComponent<Rigidbody>().MovePosition(go.transform.position);
            yield return null;
        }
        // Complete transfer and change NPC's current floor
        inUse = false;
        gs.setTransferring(false);
        gs.setCurrentFloor(floor);
    }
}
