using System;
using System.Runtime.InteropServices;


[StructLayout(LayoutKind.Sequential)]
public struct NativeAstc
{
    public byte block_x, block_y, block_z;
    public int dim_x, dim_y, dim_z;
    public int size;
    public IntPtr ptr;
}

public class MediaApi 
{
    
#if (UNITY_IPHONE || UNITY_TVOS || UNITY_WEBGL || UNITY_SWITCH) && !UNITY_EDITOR
        const string AstcDLL = "__Internal";
#else
        const string AstcDLL = "xmedia";
#endif
    
    
    [DllImport(AstcDLL)]
    public static extern bool astc_read(string path, IntPtr ptr);
    

    [DllImport(AstcDLL)]
    public static extern bool astc_dispose(IntPtr ptr);
    
}
