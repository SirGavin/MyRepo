using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Material mat;

    void OnPostRender() {
        GL.PushMatrix();
        mat.SetPass(0);

        //DrawLine(new Vector2(0f, 0f), new Vector3(0f, 0f, 0f));
        //DrawLine(new Vector2(1f, 0f), new Vector3(0f, 1f, 0f));
        //DrawLine(new Vector2(-1f, 0f), new Vector3(0f, -1f, 0f));
        //DrawLine(new Vector2(0f, -1f), new Vector3(-0.75f, 0.55f, 0f));
        //DrawLine(new Vector2(-1f, -1f), new Vector3(-0.75f, -0.55f, 0f));
        //DrawLine(new Vector2(0f, 1f), new Vector3(0.75f, 0.55f, 0f));
        //DrawLine(new Vector2(-1f, 1f), new Vector3(0.75f, -0.55f, 0f));


       /* DrawLine(new Vector2(0f, 2f), new Vector3(0f, 0f, 0f));
        DrawLine(new Vector2(0f, -2f), new Vector3(0f, 1f, 0f));
        DrawLine(new Vector2(-1f, 2f), new Vector3(0f, 0f, 0f));
        DrawLine(new Vector2(-1f, -2f), new Vector3(0f, 1f, 0f));
        DrawLine(new Vector2(1f, 1f), new Vector3(0f, 0f, 0f));
        DrawLine(new Vector2(1f, -1f), new Vector3(0f, 1f, 0f));
        DrawLine(new Vector2(-1f, 1f), new Vector3(0f, 0f, 0f));
        DrawLine(new Vector2(-1f, -1f), new Vector3(0f, 1f, 0f));*/
        /* Vector2 offsetCoords = new Vector2(0f, 0f);
         Vector3 position = HexUtils.OffsetCoordsToPosition(offsetCoords);
         Debug.Log("position: " + position);

         GL.Begin(GL.LINES);
         GL.Color(Color.red);
         GL.Vertex(position);
         GL.Vertex(new Vector3(position.x + 0.05f, position.y, position.z));
         GL.End();*/

        GL.PopMatrix();
    }

    void DrawLine(Vector2 offsetCoords, Vector3 center) {
        Vector3 position = HexUtils.OffsetCoordsToPosition(offsetCoords);
        //Debug.Log("position: " + position);

        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        // GL.Vertex(position);
        //GL.Vertex(new Vector3(position.x, position.y + 0.1f, position.z));
        
        float minY = -100f, maxY = -100f;
        int lines = 0;
        for (float x = -5; x <= 5; x += 0.01f) {
            for (float y = -5; y <= 5; y += 0.01f) {
                Vector3 newPosition = new Vector3(x, y, 0);
                Vector2 offsetCheck = HexUtils.PositionToOffsetCoords(newPosition);
                /*if (offsetCoords.x == offsetCheck.x && offsetCoords.y == offsetCheck.y) {
                    GL.Vertex(position);
                    GL.Vertex(newPosition);
                    lines++;
                }*/

                if (minY == -100f && offsetCoords.x == offsetCheck.x && offsetCoords.y == offsetCheck.y) {
                    minY = y;
                } else if (minY != -100f && (offsetCoords.x != offsetCheck.x || offsetCoords.y != offsetCheck.y)) {
                    maxY = y - 0.01f;
                    break;
                }
            }
            if (minY != -100f) {
                GL.Vertex(new Vector3(x, minY, -9));
                GL.Vertex(new Vector3(x, maxY, -9));
                minY = -100f;
                lines++;
            }
        }
        Debug.Log("lines: " + lines);
        GL.End();
    }
}
