using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausa : MonoBehaviour
{
    void Start ()
    {
        Time.timeScale = 0.0f;
    }
    
    public void Jugar () 
    {
        Time.timeScale=1.0f;
    }

    public void pausa ()
    {
        Time.timeScale=0.0f;
    }
}
