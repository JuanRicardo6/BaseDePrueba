using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu(fileName = "Events")]
public class REvents : ScriptableObject
{
    public Action GEvent; //aca se declara el evento

    public void FireEvent() //esta funcion dispara el evento
    {
        GEvent?.Invoke();
    }
}
