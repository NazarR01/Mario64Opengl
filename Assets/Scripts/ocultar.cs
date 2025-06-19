using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ocultar : MonoBehaviour
{
    public GameObject panel;
   public void hide()
    {
        panel.gameObject.SetActive(false);
    }
}
