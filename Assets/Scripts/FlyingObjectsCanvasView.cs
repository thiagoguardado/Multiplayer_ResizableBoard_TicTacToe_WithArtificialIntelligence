using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyingObjectsCanvasView : MonoBehaviour {

    public Transform diagonalPanelsParent;

    private void Start()
    {
        ChangeAllImagesColors(diagonalPanelsParent);
    }


    private void ChangeAllImagesColors(Transform diagonalPanelsParent)
    {
        for (int i = 0; i < diagonalPanelsParent.childCount; i++)
        {

            for (int j = 0; j < diagonalPanelsParent.GetChild(i).childCount; j++)
            {
                Image im = diagonalPanelsParent.GetChild(i).GetChild(j).GetComponent<Image>();
                if (im != null)
                {
                    im.color = new Color(Random.value, Random.value, Random.value);

                }


            }


        }
    }

}
