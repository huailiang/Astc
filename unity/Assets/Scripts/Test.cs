using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    private RawImage raw;
    private NativeAstc _astc;
    private IntPtr _ptr;
    private GUILayoutOption[] _options;

    private void Start()
    {
        _options = new GUILayoutOption[2];
        _options[0] = GUILayout.MinWidth(120);
        _options[1] = GUILayout.MinHeight(60);
        raw = GetComponent<RawImage>();
    }

    void OnGUI()
    {
#if UNITY_EDITOR
        if (GUILayout.Button("ldr", _options))
        {
            var tex = new Texture2D(479, 320, TextureFormat.ASTC_8x8, false);
            tex.LoadRawTextureData(ReadBuf("Assets/Resources/ldr.astc"));
            tex.Apply();
            raw.texture = tex;
        }
        if (GUILayout.Button("hdr", _options))
        {
            var tex = new Texture2D(479, 320, TextureFormat.ASTC_HDR_8x8, false);
            tex.LoadRawTextureData(ReadBuf("Assets/Resources/hdr.astc"));
            tex.Apply();
            raw.texture = tex;
        }
#endif
        GUILayout.Space(8);
        if (GUILayout.Button("ldr-2", _options))
        {
            raw.texture = Resources.Load<Texture2D>("ldr");
        }
        if (GUILayout.Button("hdr-2", _options))
        {
            raw.texture = Resources.Load<Texture2D>("hdr");
        }
        GUILayout.Space(8);
        if (GUILayout.Button("native load", _options))
        {
            var p = Path.Combine(Application.dataPath, "Resources/ldr.astc");
            LoadNative(p);
        }
        if (GUILayout.Button("native destroy", _options))
        {
            if (_ptr != IntPtr.Zero)
            {
                MediaApi.astc_dispose(_ptr);
                _ptr = IntPtr.Zero;
                Destroy(raw.texture);
                raw.texture = null;
            }
        }
        GUILayout.Space(8);
        if (GUILayout.Button("copy2persist", _options))
        {
            var buf = Resources.Load<TextAsset>("ldr").bytes;
            var t = Path.Combine(Application.persistentDataPath, "ldr.astc");
            File.WriteAllBytes(t, buf);
            var p = Path.Combine(Application.persistentDataPath, "ldr.astc");
            LoadNative(p);
        }
    }

    private byte[] ReadBuf(string path)
    {
        var bs = File.ReadAllBytes(path);
        int len = bs.Length - 16;
        var b2 = new byte[len];
        Array.Copy(bs, 16, b2, 0, len);
        return b2;
    }

    private void LoadNative(string path)
    {
        _astc = new NativeAstc();
        _ptr = Marshal.AllocHGlobal(Marshal.SizeOf(_astc));
        Marshal.StructureToPtr(_astc, _ptr, false);
        if (MediaApi.astc_read(path, _ptr))
        {
            _astc = Marshal.PtrToStructure<NativeAstc>(_ptr);
            var fomat = TextureFormat.ASTC_8x8;
            if (_astc.block_x == 4 && _astc.block_y == 4)
                fomat = TextureFormat.ASTC_4x4;
            else if (_astc.block_x == 5 && _astc.block_y == 5)
                fomat = TextureFormat.ASTC_5x5;
            else if (_astc.block_x == 6 && _astc.block_y == 6)
                fomat = TextureFormat.ASTC_6x6;
            else if (_astc.block_x == 10 && _astc.block_y == 10)
                fomat = TextureFormat.ASTC_10x10;
            else if (_astc.block_x == 12 && _astc.block_y == 12)
                fomat = TextureFormat.ASTC_12x12;
            else if (_astc.block_x != 8)
                Debug.LogErrorFormat("unknown astc format: {0}x{1}", _astc.block_x, _astc.block_y);

            Debug.Log("dim x: " + _astc.dim_x + "  " + _astc.block_x + " format:" + fomat);
            var tex = new Texture2D(_astc.dim_x, _astc.dim_y, fomat, false);
            tex.LoadRawTextureData(_astc.ptr, _astc.size);
            tex.Apply();
            raw.texture = tex;
        }
    }

}
