using System;
using System.Collections.Generic;
using UnityEngine;

public struct Hex {
    public int q { get; set; }
    public int r { get; set; }
    public int s { get; set; }

    public Hex(int q, int r) {
        this.q = q;
        this.r = r;
        this.s = -q - r;
        if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
    }

    public Hex(int q, int r, int s) {
        this.q = q;
        this.r = r;
        this.s = s;
        if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
    }
}

public struct FractionalHex {
    public float q { get; set; }
    public float r { get; set; }
    public float s { get; set; }

    public FractionalHex(float q, float r) {
        this.q = q;
        this.r = r;
        this.s = -q - r;
        if (Math.Round(q + r + s) != 0) throw new ArgumentException("q + r + s must be 0");
    }

    public FractionalHex(float q, float r, float s) {
        this.q = q;
        this.r = r;
        this.s = s;
        if (Math.Round(q + r + s) != 0) throw new ArgumentException("q + r + s must be 0");
    }
}

public static class HexUtils {

    public static Vector2 hexToOffset(Hex hex) {
        //Debug.Log("hex: q " + hex.q + " r " + hex.r + " s " + hex.s);
        int col = hex.q + (hex.r - (hex.r & 1)) / 2;
        int row = hex.r;
        return new Vector2(col, row);
    }

    //function evenq_to_cube(hex):
    //    var x = hex.col
    //    var z = hex.row - (hex.col + (hex.col & 1)) / 2
    //    var y = -x - z
    //    return Cube(x, y, z)


    public static Vector2 PositionToOffsetCoords(Vector3 position) {
        float q = (((float)Math.Sqrt(3f) / 3f * position.x) - (1f / 3f * position.y)) / 0.55f; //Default hex width is 1 unit
        float r = ((2f / 3f) * position.y) / 0.5f; //Default height is sqrt(3/2)

        //float q = ((2f / 3f) * position.x) / 0.5f; //Default hex width is 1 unit
        //float r = ((-1f / 3f) * position.x + ((float)Math.Sqrt(3f) / 3f) * position.y) / ((float)Math.Sqrt(3f) / 2f); //Default height is sqrt(3/2)

        return hexToOffset(HexRound(new FractionalHex(q, r)));
    }

    public static Vector3 OffsetCoordsToPosition(Vector2 offsetCoords) {
        float x = 0.55f * (float)Math.Sqrt(3f) * (offsetCoords.x + 0.5f * ((int)offsetCoords.y & 1)); 
        float y = 0.5f * (2f / 3f) * offsetCoords.y;
        return new Vector3(x, y, -9f);
    }

    public static Hex HexRound(FractionalHex fHex) {
       // Debug.Log("fHex: q " + fHex.q + " r " + fHex.r + " s " + fHex.s);
        int rq = (int)Math.Round(fHex.q);
        int rr = (int)Math.Round(fHex.r);
        int rs = (int)Math.Round(fHex.s);

        float q_diff = Math.Abs(rq - fHex.q);
        float r_diff = Math.Abs(rr - fHex.r);
        float s_diff = Math.Abs(rs - fHex.s);

        if (q_diff > r_diff && q_diff > s_diff) {
            rq = -rr - rs;
        } else if (r_diff > s_diff) {
            rr = -rq - rs;
        } else {
            rs = -rq - rr;
        }

        return new Hex(rq, rr, rs);
    }

    private static List<Vector2Int> NeighborDirections = new List<Vector2Int> {
        new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, -1),
        new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1)
    };

    private static List<Vector2Int> OffsetNeighborDirections = new List<Vector2Int> {
        new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1),
        new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1)
    };

    public static bool AreNeighbors(Vector3Int tile1, Vector3Int tile2) {
        int parity = tile1.y & 1;
        List<Vector2Int> directions = parity == 0 ? NeighborDirections : OffsetNeighborDirections;

        for (int i = 0; i < directions.Count; i++) {
            Vector2Int direction = directions[i];
            if (tile2.Equals(new Vector3Int(tile1.x + direction.x, tile1.y + direction.y, tile1.z))) {
                return true;
            }
        }

        return false;
    }

    public static List<Vector2Int> GetNeighborDirections(Vector3Int tile) {
        int parity = tile.y & 1;
        return parity == 0 ? NeighborDirections : OffsetNeighborDirections;
    }

    public static Dictionary<Vector2Int, List<Vector2Int>> PushDirections = new Dictionary<Vector2Int, List<Vector2Int>>() {
        {
            new Vector2Int(1, 0),
            new List<Vector2Int> {
                new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(-1, -1), new Vector2Int(-1, 1)
            }
        },
        {
            new Vector2Int(0, -1),
            new List<Vector2Int> {
                new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(1, 1)
            }
        },
        {
            new Vector2Int(-1, -1),
            new List<Vector2Int> {
                new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(1, -1), new Vector2Int(0, 1), new Vector2Int(1, 0)
            }
        },
        {
            new Vector2Int(-1, 0),
            new List<Vector2Int> {
                new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(-1, -1), new Vector2Int(0, 1), new Vector2Int(0, -1)
            }
        },
        {
            new Vector2Int(-1, 1),
            new List<Vector2Int> {
                new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, 1)
            }
        },
        {
            new Vector2Int(0, 1),
            new List<Vector2Int> {
                new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(-1, 0)
            }
        },
    };

    public static Dictionary<Vector2Int, List<Vector2Int>> OffsetPushDirections = new Dictionary<Vector2Int, List<Vector2Int>>() {
        {
            new Vector2Int(1, 0),
            new List<Vector2Int> {
                new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(1, 1), new Vector2Int(0, -1), new Vector2Int(0, 1)
            }
        },
        {
            new Vector2Int(1, -1),
            new List<Vector2Int> {
                new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, -1), new Vector2Int(0, 1), new Vector2Int(-1, 0)
            }
        },
        {
            new Vector2Int(0, -1),
            new List<Vector2Int> {
                new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(-1, 1)
            }
        },
        {
            new Vector2Int(-1, 0),
            new List<Vector2Int> {
                new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, -1)
            }
        },
        {
            new Vector2Int(0, 1),
            new List<Vector2Int> {
                new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(-1, -1), new Vector2Int(1, 0)
            }
        },
        {
            new Vector2Int(1, 1),
            new List<Vector2Int> {
                new Vector2Int(0, 1), new Vector2Int(-1, 1), new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1)
            }
        },
    };

    public static List<Vector2Int> GetPushDirections(Vector2Int from, Vector2Int to) {
        int parity = from.y & 1;
        Dictionary<Vector2Int, List<Vector2Int>> directions = parity == 0 ? PushDirections : OffsetPushDirections;

        Vector2Int pushedDirection = to - from;
        return directions[pushedDirection];
    }
}
