using UnityEngine;
using System.IO;

public class TextureModifier : MonoBehaviour
{
    public Texture2D sourceTexture; // ԭʼͼƬ
    public BlockColor[] blocksToModify; // �洢���ɫ�鼰���Ӧ��ɫ

    private const int blockSize = 64; // ÿ��ɫ��Ĵ�С��64x64��

    [System.Serializable]
    public class BlockColor
    {
        public int blockX;    // ɫ��� X λ��
        public int blockY;    // ɫ��� Y λ��
        public Color color;   // ��ɫ���Ŀ����ɫ
    }

    void Start()
    {
        if (sourceTexture == null)
        {
            Debug.LogError("��ָ��Ҫ�޸ĵ�����");
            return;
        }

        // ����һ�������Ա����޸�ԭʼͼƬ
        Texture2D editableTexture = Instantiate(sourceTexture);

        // ȷ�������ǿɶ�д��
        if (!editableTexture.isReadable)
        {
            Debug.LogError("����δ���ö�д������Inspector�����ã�");
            return;
        }

        // �޸Ķ��ɫ�����ɫ
        foreach (BlockColor block in blocksToModify)
        {
            ModifyBlockColor(editableTexture, block.blockX, block.blockY, block.color);
        }

        // Ӧ���޸�
        editableTexture.Apply();

        // ���޸ĺ������Ӧ�õ�������
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = editableTexture;
        }

        // ���浽�ļ�
        SaveTextureToFile(editableTexture, "ModifiedTexture.png");
    }

    private void ModifyBlockColor(Texture2D texture, int blockX, int blockY, Color color)
    {
        // ����ɫ�����½����ص���ʼλ��
        int startX = blockX * blockSize;
        int startY = blockY * blockSize;

        // ����Ƿ񳬳�����Χ
        if (startX + blockSize > texture.width || startY + blockSize > texture.height)
        {
            Debug.LogError($"ָ����ɫ��({blockX}, {blockY})��������Χ��");
            return;
        }

        // �޸�����ɫ�����ɫ
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
        // ���������Ϊ PNG ��ʽ
        byte[] pngData = texture.EncodeToPNG();
        if (pngData != null)
        {
            // ��ȡ��Ŀ�ļ��е�·��
            string path = Path.Combine(Application.dataPath, fileName);

            // ������д���ļ�
            File.WriteAllBytes(path, pngData);
            Debug.Log($"�����ѱ��浽: {path}");
        }
        else
        {
            Debug.LogError("�������ʧ�ܣ�");
        }
    }
}
