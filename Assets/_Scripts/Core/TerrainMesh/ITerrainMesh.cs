using UnityEngine;
using System.Collections;

public interface ITerrainMesh
{
    void TwoSidedQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int texSlot, Color color);
    void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int texSlot, Color color);
    void FurnitureQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int u, int v, int du, int dv, float blockSize, int texSlot, Color color);
    void Mesh(int x, int y, int z, MeshData mesh);
    void Mesh(int x, int y, int z, MeshData mesh, Color color);
    void PushToMesh(out MeshData mesh);
}
