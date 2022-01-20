using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopRightTP : MonoBehaviour
{

    private List<GameObject> inTeleporter = new List<GameObject>();

    void Start()
    {
        InvokeRepeating("TransferNPCs", 2, 4f);
    }

    private void TransferNPCs() {
        Invoke("TakeNPCs", 1f);
        Invoke("PutNPCs", 2f);
    }

    // In repeating intervals take at most 3 NPCs within teleporter area and put in teleporter
    private void TakeNPCs() {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("NPC")) {
            var gs = go.GetComponent<Pathfind>();
            // Make sure the NPC wants to transfer
            if (gs.getTransfer() == 5 && (go.transform.position - new Vector3(8.5f, 10.25f, 12f)).magnitude < 1.5f) {
                // Add NPC in teleporter and start transfer
                if (inTeleporter.Count < 3) {
                    inTeleporter.Add(go);
                    gs.setTransferring(true);
                    go.SetActive(false);
                }
            }
        }
    }

    // In repeating intervals place all NPCs in teleporter to other floor
    private void PutNPCs() {
        // Complete transfer and change NPC's current floor
        for (int i = 0; i < inTeleporter.Count; i++) {
            var gs = inTeleporter[i].GetComponent<Pathfind>();
            inTeleporter[i].transform.position = transform.position + new Vector3(i-1, -9, -0.5f);
            inTeleporter[i].SetActive(true);
            gs.setTransferring(false);
            gs.setCurrentFloor(2);
        }
        inTeleporter.Clear();
    }

    void OnCollisionEnter(Collision col) {
        var gs = col.gameObject.GetComponent<Pathfind>();
        // If NPC wants to transfer ignore collision
        if (gs.getTransfer() == 5) {
            Physics.IgnoreCollision(col.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}
