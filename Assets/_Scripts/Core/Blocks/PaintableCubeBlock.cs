using System;

public class PaintableCubeBlock : Block, IStandardCubeBlock, IPaintableBlock
{
	public int? GetColor(int data)
    {
        if ((data & 1) != 0)
        {
            return new int?(data >> 1 & 15);
        }
        return null;
    }

	public void GenerateTerrain(int x, int y, int z, int value, int face, BlockTerrain.Chunk chunk, ref CellFace data)
	{
		int? color = GetColor(BlockTerrain.GetData(value));
		data.TextureSlot = color.HasValue ? BlocksData.paintedTextures[TextureSlot] : TextureSlot;
		data.Color = BlocksData.ColorFromInt(color);
	}
}