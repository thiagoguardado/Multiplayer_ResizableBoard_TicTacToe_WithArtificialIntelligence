using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMenu : MonoBehaviour {

    public NetworkGameSelectionMatch matchPanelPrefab;
    public Transform matchListPanelParent;

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            AddMatchToList();
        }
    }




    private void AddMatchToList()
    {
        NetworkGameSelectionMatch m = Instantiate(matchPanelPrefab, matchListPanelParent);
        m.SetName("MATCH XXX");
    }

}
