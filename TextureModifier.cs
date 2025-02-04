using UnityEngine;
using System.IO;

public class TextureModifier : MonoBehaviour
{
    public Texture2D sourceTexture; // 原始图片
    public BlockColor[] blocksToModify; // 存储多个色块及其对应颜色

    private const int blockSize = 64; // 每个色块的大小（64x64）

    [System.Serializable]
    public class BlockColor
    {
        public int blockX;    // 色块的 X 位置
        public int blockY;    // 色块的 Y 位置
        public Color color;   // 该色块的目标颜色
    }

    void Start()
    {
        if (sourceTexture == null)
        {
            Debug.LogError("请指定要修改的纹理！");
            return;
        }

        // 创建一个副本以避免修改原始图片
        Texture2D editableTexture = Instantiate(sourceTexture);

        // 确保纹理是可读写的
        if (!editableTexture.isReadable)
        {
            Debug.LogError("纹理未启用读写，请在Inspector中启用！");
            return;
        }

        // 修改多个色块的颜色
        foreach (BlockColor block in blocksToModify)
        {
            ModifyBlockColor(editableTexture, block.blockX, block.blockY, block.color);
        }

        // 应用修改
        editableTexture.Apply();

        // 将修改后的纹理应用到材质上
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = editableTexture;
        }

        // 保存到文件
        SaveTextureToFile(editableTexture, "ModifiedTexture.png");
    }

    private void ModifyBlockColor(Texture2D texture, int blockX, int blockY, Color color)
    {
        // 计算色块左下角像素的起始位置
        int startX = blockX * blockSize;
        int startY = blockY * blockSize;

        // 检查是否超出纹理范围
        if (startX + blockSize > texture.width || startY + blockSize > texture.height)
        {
            Debug.LogError($"指定的色块({blockX}, {blockY})超出纹理范围！");
            return;
        }

        // 修改整个色块的颜色
        for (int x = startX; x < startX + blockSize; x++)
        {
            for (int y = startY; y < startY + blockSize; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }
    }

    private void SaveTextureToFile(Texture2D texture, string fileName)
    {
        // 将纹理编码为 PNG 格式
        byte[] pngData = texture.EncodeToPNG();
        if (pngData != null)
        {
            // 获取项目文件夹的路径
            string path = Path.Combine(Application.dataPath, fileName);

            // 将数据写入文件
            File.WriteAllBytes(path, pngData);
            Debug.Log($"纹理已保存到: {path}");
        }
        else
        {
            Debug.LogError("纹理编码失败！");
        }
    }
}
