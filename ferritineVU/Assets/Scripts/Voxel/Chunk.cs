public struct Voxel
{
    public byte type;
}

public class Chunk
{
    public const int Size = 16;

    private Voxel[,,] voxels;
    
    public Chunk()
    {
        voxels = new Voxel[Size, Size, Size];
    }
}