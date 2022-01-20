using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomLeftTP : MonoBehaviour
{
    
    private List<GameObject> inTeleporter = new List<GameObject>();

    void Start()
    {
        InvokeRepeating("TransferNPCs", 0, 4f);
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
            if (gs.getTransfer() == 4 && (go.transform.position - new Vector3(-7.5f, 1.25f, 12f)).magnitude < 1.5f) {
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
            inTeleporter[i].transform.position = transform.position + new Vector3(i-1, 9, -0.5f);
            inTeleporter[i].SetActive(true);
            gs.setTransferring(false);
            gs.setCurrentFloor(3);
        }
        inTeleporter.Clear();
    }

    void OnCollisionEnter(Collision col) {
        var gs = col.gameObject.GetComponent<Pathfind>();
        // If NPC wants to transfer ignore collision
        if (gs.getTransfer() == 4) {
            Physics.IgnoreCollision(col.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}
