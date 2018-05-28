using UnityEngine;
using System.Collections;

public class ElectricGateBlock : Block, INormalBlock
{
	MeshData[] meshes = new MeshData[24];

    public override void Initialize()
    {      
        Mesh blockMesh;
        switch (Index)
        {
            case 134:
                blockMesh = BlockMeshes.FindMesh("NandGate");
                break;
            case 135:
                blockMesh = BlockMeshes.FindMesh("NorGate");
                break;
            case 137:
                blockMesh = BlockMeshes.FindMesh("AndGate");
                break;
            case 140:
                blockMesh = BlockMeshes.FindMesh("NotGate");
                break;
            case 143:
                blockMesh = BlockMeshes.FindMesh("OrGate");
                break;
            case 145:
                blockMesh = BlockMeshes.FindMesh("DelayGate");
                break;
            case 146:
                blockMesh = BlockMeshes.FindMesh("SRLatch");
                break;
            case 156:
                blockMesh = BlockMeshes.FindMesh("XorGate");
                break;
            case 157:
                blockMesh = BlockMeshes.FindMesh("RandomGenerator");
                break;
            case 179:
                blockMesh = BlockMeshes.FindMesh("MotionDetector");
                break;
            case 180:
                blockMesh = BlockMeshes.FindMesh("DigitalToAnalogConverter");
                break;
            case 181:
                blockMesh = BlockMeshes.FindMesh("AnalogToDigitalConverter");
                break;
            case 183:
                blockMesh = BlockMeshes.FindMesh("SoundGenerator");
                break;
            case 184:
                blockMesh = BlockMeshes.FindMesh("Counter");
                break;
            case 186:
                blockMesh = BlockMeshes.FindMesh("MemoryBank");
                break;
            case 187:
                blockMesh = BlockMeshes.FindMesh("RealTimeClock");
                break;
            case 188:
                blockMesh = BlockMeshes.FindMesh("TruthTableCircuit");
                break;
            default:
                throw new System.Exception("unsupported electric gate: " + Index);
        }

        Matrix4x4 m;
        Matrix4x4 m2;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                switch (j)
                {
                    case 0:
                        m = Matrix4x4.Rotate(Quaternion.Euler(0, 0, i * 90));
                        m2 = Matrix4x4.TRS(new Vector3(0.5f, -0.5f, 0f), Quaternion.Euler(90, 0, 0), Vector3.one);
                        break;
                    case 1:
                        m = Matrix4x4.Rotate(Quaternion.Euler(i * -90, 0, 0));
                        m2 = Matrix4x4.TRS(new Vector3(0f, -0.5f, 0.5f), Quaternion.Euler(90, 0, -270), Vector3.one);
                        break;
                    case 2:
                        m = Matrix4x4.Rotate(Quaternion.Euler(0, 0, i * -90));
                        m2 = Matrix4x4.TRS(new Vector3(-0.5f, -0.5f, 0f), Quaternion.Euler(90, 0, -180), Vector3.one);
                        break;
                    case 3:
                        m = Matrix4x4.Rotate(Quaternion.Euler(i * 90, 0, 0));
                        m2 = Matrix4x4.TRS(new Vector3(0f, -0.5f, -0.5f), Quaternion.Euler(90, 0, -90), Vector3.one);
                        break;
                    case 4:
                        m = Matrix4x4.Rotate(Quaternion.Euler(0, i * 90, 0));
                        m2 = Matrix4x4.Translate(new Vector3(0.5f, 0f, 0.5f));
                        break;
                    case 5:
                        m = Matrix4x4.Rotate(Quaternion.Euler(0, i * -90, 0));
                        m2 = Matrix4x4.TRS(new Vector3(0.5f, 0f, -0.5f), Quaternion.Euler(180, 0, 0), Vector3.one);
                        break;
                    default:
                        throw new System.Exception();
                }

                meshes[(j << 2) + i] = new MeshData(BlockMeshes.TranslateMeshRaw(blockMesh, m * m2));
            }
        }
    }

    public static int GetFace(int value)
    {
        return BlockTerrain.GetData(value) >> 2 & 7;
    }

    public void GenerateTerrain(int x, int y, int z, int value, BlockTerrain.Chunk chunk, MeshGenerator g)
	{
        g.Terrain.Mesh(x, y, z, meshes[BlockTerrain.GetData(value) & 31], Color.white);
	}
}
