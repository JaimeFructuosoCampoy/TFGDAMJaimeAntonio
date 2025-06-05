using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GlobalFunctions
{
    private int test;
    public static List<Tuple<bool, bool>> DetectDirectionAndWay(Collision2D collision)
    {
        List<Tuple<bool, bool>> directionsAndWays = new();
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;
            bool way = false;
            bool direction = false;

            if (normal.x != 0) //Hay colisión horizontal
            {
                direction = true;
                if (normal.x > 0) //Recibe la colisión por el lado izquierdo
                    way = true;

                if (normal.x < 0) //Recibe la colisión por el lado derecho
                    way = false;
            }

            if (normal.y != 0) //Hay colisión vertical
            {
                direction = false;
                if (normal.y > 0) //Recibe la colisión por abajo
                    way = true;

                if (normal.y < 0) //Recibe la colisión por arriba
                    way = false;
            }
            Tuple<bool, bool> tuple = new(direction, way);
            directionsAndWays.Add(tuple);
        }

        return directionsAndWays;
    }

    public static IEnumerator DestroyObjectAfterSeconds(float seconds, GameObject target)
    {
        yield return new WaitForSeconds(seconds);
        MonoBehaviour.Destroy(target);
    }
}
