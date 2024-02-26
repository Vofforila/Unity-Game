using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class PathMarker
{
    public MapLocation location;
    public float g;
    public float h;
    public float f;
    public GameObject marker;
    public PathMarker parent;

    public PathMarker(MapLocation _l, float _g, float _h, float _f, GameObject _marker, PathMarker _p)
    {
        location = _l;
        g = _g;
        h = _h;
        f = _f;
        marker = _marker;
        parent = _p;
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            return location.Equals(((PathMarker)obj).location);
        }
    }
}

public class MapLocation
{
    public Vector3 position;

    public MapLocation(Vector3 _position)
    {
        position = _position;
    }

    public static MapLocation operator +(MapLocation a, MapLocation b)
    {
        return new MapLocation(a.position + b.position);
    }
}

public class AStarPath : MonoBehaviour
{
    public Material closeMaterial;
    public Material openMaterial;

    public List<PathMarker> open = new();
    public List<PathMarker> close = new();

    public Transform start;
    public Transform end;
    public GameObject pathP;

    private PathMarker goalNode;
    private PathMarker startNode;
    private PathMarker lastPosition;
    private bool done = false;

    [Header("Maze Data")]
    public float size = 1f;
    public float scale = 25f;
    public float depth = 5f;
    public float width = 5f;

    public byte[,] mazeMap = {
    { 0, 0 ,0, 0, 1},
    { 0, 1, 1, 0, 0},
    { 0, 0, 0, 1, 0},
    { 1, 1, 0, 1, 0},
    { 1, 1, 0, 0, 0},
    };

    public List<MapLocation> directions =
        new()
        {
            new MapLocation(new Vector3(1, 0, 0)),
            new MapLocation(new Vector3(-1, 0, 0)),
            new MapLocation(new Vector3(0, 0, 1)),
            new MapLocation(new Vector3(0, 0, -1)),
        };

    private void RemoveAllMarkers()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("Marker");
        foreach (GameObject o in markers)
        {
            Destroy(o);
        }
    }

    private void BeginSearch()
    {
        done = false;
        RemoveAllMarkers();

        List<MapLocation> locations = new();

        for (int row = 0; row < depth; row++)
        {
            for (int col = 0; col < depth; col++)
            {
                // IF wall dont add
                if (mazeMap[row, col] == 0)
                {
                    locations.Add(new MapLocation(new Vector3(row, 0, col)));
                }
            }
        }
        ShuffleList(locations);

        Vector3 startlocation = locations[0].position;
        startNode = new PathMarker(new MapLocation(locations[0].position), 0, 0, 0, Instantiate(pathP, start.position, Quaternion.identity), null);

        Vector3 goallocation = locations[1].position;
        goalNode = new PathMarker(new MapLocation(locations[1].position), 0, 0, 0, Instantiate(pathP, end.position, Quaternion.identity), null);

        // Clearing
        open.Clear();
        close.Clear();

        open.Add(startNode);
        lastPosition = startNode;
    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void Search(PathMarker thisNode)
    {
        // Return
        if (thisNode == null) return;

        if (thisNode.Equals(goalNode))
        {
            done = true;
            return;
        }

        foreach (MapLocation dir in directions)
        {
            MapLocation neightbour = dir + thisNode.location;

            if (mazeMap[(int)neightbour.position.x, (int)neightbour.position.z] == 1)
                continue;

            if (neightbour.position.x < 1 || neightbour.position.x >= width || neightbour.position.z < 1 || neightbour.position.z >= depth)
                continue;

            if (isClosed(neightbour))
                continue;

            float G = Vector3.Distance(thisNode.location.position, neightbour.position) + thisNode.g;
            float H = Vector3.Distance(neightbour.position, goalNode.location.position);
            float F = G + H;

            GameObject pathBlock = Instantiate(pathP,
                new Vector3(neightbour.position.x * scale, neightbour.position.y, neightbour.position.z * scale), Quaternion.identity);

            List<TMP_Text> values = new(pathBlock.GetComponentsInChildren<TMP_Text>(true));

            values[0].text = "G: " + G.ToString();
            values[1].text = "H: " + H.ToString();
            values[2].text = "F: " + F.ToString();

            if (!UpdateMarker(neightbour, G, H, F, thisNode))
            {
                open.Add(new PathMarker(neightbour, G, H, F, pathBlock, thisNode));
            }
        }

        open = open.OrderBy(p => p.f).ThenBy(n => n.h).ToList<PathMarker>();

        PathMarker pm = (PathMarker)open.ElementAt(0);

        close.Add(pm);

        open.RemoveAt(0);
        pm.marker.GetComponent<Renderer>().material = closeMaterial;

        lastPosition = pm;
    }

    private bool UpdateMarker(MapLocation _pos, float _g, float _h, float _f, PathMarker _prt)
    {
        foreach (PathMarker p in open)
        {
            if (p.location.Equals(_pos))
            {
                p.g = _g;
                p.h = _h;
                p.f = _f;
                p.parent = _prt;
                return true;
            }
        }
        return false;
    }

    private bool isClosed(MapLocation marker)
    {
        foreach (PathMarker p in close)
        {
            if (p.location.Equals(marker))
                return true;
        }
        return false;
    }

    private void GetPath()
    {
        RemoveAllMarkers();
        PathMarker begin = lastPosition;

        // Make do while loop
        do
        {
            Instantiate(pathP, begin.location.position * scale, Quaternion.identity);
            begin = begin.parent;
        } while (!startNode.Equals(begin) && begin != null);

        Instantiate(pathP, startNode.location.position * scale, Quaternion.identity);
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            BeginSearch();
        }
        if (Input.GetKeyDown(KeyCode.S) && !done)
        {
            Search(lastPosition);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            GetPath();
        }
    }
}