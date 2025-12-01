using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using BuildingsAndGrid;
using Messages;

public class GridColliderBuilder : IStartable
{
    private readonly TilesController tilesController;
    private readonly GridSettings gridSettings;
    private readonly Transform parentTransform;

    private GameObject colliderRoot;
    private MeshCollider meshCollider;

    private const float ColliderYOffset = 0.01f; // чтобы не конфликтовать визуально

    public GridColliderBuilder(
        TilesController tilesController,
        GridSettings gridSettings,
        [Key("GridRoot")] Transform parentTransform,
        ISubscriber<GridExtendMessage> gridExtendSub)
    {
        this.tilesController = tilesController;
        this.gridSettings = gridSettings;
        this.parentTransform = parentTransform;

        gridExtendSub.Subscribe(_ => RebuildCollider());
    }

    public void Start()
    {
        CreateRuntimeObject();
        RebuildCollider();
    }

    private void CreateRuntimeObject()
    {
        colliderRoot = new GameObject("GridCollider");
        colliderRoot.transform.SetParent(parentTransform, false);

        meshCollider = colliderRoot.AddComponent<MeshCollider>();
    }

    private void RebuildCollider()
    {
        var tiles = tilesController.Tiles;
        if (tiles == null)
            return;

        var verts = new List<Vector3>();
        var tris = new List<int>();

        float tileX = gridSettings.TileSize.x;
        float tileZ = gridSettings.TileSize.z;
        float y = gridSettings.yOffset + ColliderYOffset;

        int vertOffset = 0;

        // Проходим по ВСЕМ существующим тайлам
        for (int x = tiles.MinX; x < tiles.MaxX; x++)
        {
            for (int yTile = tiles.MinY; yTile < tiles.MaxY; yTile++)
            {
                var tile = SafeGetTile(tiles, x, yTile);
                if (tile == null)
                    continue;

                // bottom-left
                float wx0 = x * tileX;
                float wz0 = yTile * tileZ;

                // top-right
                float wx1 = (x + 1) * tileX;
                float wz1 = (yTile + 1) * tileZ;

                // Вершины квадрата
                verts.Add(new Vector3(wx0, y, wz0));
                verts.Add(new Vector3(wx1, y, wz0));
                verts.Add(new Vector3(wx1, y, wz1));
                verts.Add(new Vector3(wx0, y, wz1));

                // Треугольники
                tris.Add(vertOffset + 0);
                tris.Add(vertOffset + 2);
                tris.Add(vertOffset + 1);

                tris.Add(vertOffset + 0);
                tris.Add(vertOffset + 3);
                tris.Add(vertOffset + 2);

                vertOffset += 4;
            }
        }

        // Строим меш
        Mesh mesh = new Mesh();
        mesh.name = "GridColliderMesh";
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;
    }

    private Tile SafeGetTile(Tiles tiles, int x, int y)
    {
        try { return tiles.GetTile(x, y); }
        catch { return null; }
    }
}
