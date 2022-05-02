# Astc 图片兼容性测试


>Adaptive Scalable Texture Compression ([ASTC][i2])是Arm和AMD共同研发的一种纹理压缩格式，不同于ETC和ETC2的固定块尺寸（4x4），ASTC支持可变块大小的压缩，从而获得灵活的更大压缩率的纹理数据，降低GPU的带宽和能耗。 ASTC虽然尚未成为OpenGL的标准格式，只是以扩展的形式存在，但目前已经广泛地被主流GPU支持，可谓不是标准的的标准扩展。但在Vulkan中，ASTC已经是标准的特性了。


<br>

unity 虽然也支持了 .astc， 但其引擎实现和 [astcenc][i1] 转换出来的文件还是有一些差异的， 因此需要一定的转换。 最近在服务器集群docker环境里批量处理用户上传的图片转为原生astc， 此工程就是测试unity和astcenc二者之间的兼容性和性能优化。更多详细的介绍参考[我的博客][i3]。

<br>

##  c#加载astc

原始的astc图片是包含16位头文件的， 直接使用 LoadRawTextureData 的方式前需要对 bytes进行裁剪， 这里有一个较大的内存开销。

```cs
var tex = new Texture2D(479, 320, TextureFormat.ASTC_HDR_8x8, false);
tex.LoadRawTextureData(bytes)
```

由于c# 没有提供类似的 带偏移和长度的方式加载rawtexturedata, 类似下面的接口：

```cs
tex.LoadRawTextureData(byte[] bytes, int offset, int size);
```

因此 需要事先实现好对头的处理， 比如使用 Array.Copy 这样， 很容易带来额外的GC开销。


##  c++ 里加载astc


c++加载好之后， 直接将对应的指针传递给c#

```cs
var tex = new Texture2D(_astc.dim_x, _astc.dim_y, fomat, false);
tex.LoadRawTextureData(ptr, size);
```


c++ 直接通过地址偏移， 读取信息解析出来, 直接将地址传递给c#侧, 如下代码， 先解析出头， 然后将astc->ptr传递到上面c#接口， 就可以避免gc开销了。


```cpp
bool ReadAstc(const char *path, Astc *astc) {
    ifstream ifs;
    ifs.exceptions(ifstream::failbit | ifstream::badbit);
    try {
        ifs.open(path, ifstream::binary | std::ios::in);
        ifs.clear();
        ifs.seekg(0, ios::beg);
        uint8_t magic[4];
        ifs.read((char *) &magic[0], sizeof(uint8_t));
        ifs.read((char *) &magic[1], sizeof(uint8_t));
        ifs.read((char *) &magic[2], sizeof(uint8_t));
        ifs.read((char *) &magic[3], sizeof(uint8_t));
        if (magic[0] == 0x13 && magic[1] == 0xab && magic[2] == 0xa1 && magic[3] == 0x5c) {
            ifs.read((char *) (&astc->block_x), sizeof(uint8_t));
            ifs.read((char *) (&astc->block_y), sizeof(uint8_t));
            ifs.read((char *) (&astc->block_z), sizeof(uint8_t));
            astc->dim_x = ReadDim(ifs);
            astc->dim_y = ReadDim(ifs);
            astc->dim_z = ReadDim(ifs);
#ifdef _DEBUG
            cout << "dim x: " << astc->dim_x << " y: " << astc->dim_y << " z: " << astc->dim_z << endl;
#endif
            streampos pos = ifs.tellg(); // save current position
            ifs.seekg(0, ios::end);
            astc->size = (int) (ifs.tellg() - pos);
#ifdef _DEBUG
            cout << "file length = " << astc->size << endl;
#endif
            ifs.seekg(pos);
            astc->ptr = new char[astc->size];
            ifs.read((char *) (astc->ptr), astc->size * sizeof(char));
            return true;
        }
        ifs.close();
    }
    catch (ifstream::failure e) {
#ifdef _DEBUG
        cout << "read table error " << path << std::endl;
#endif
        ifs.close();
    }
    return false;
}
```

由于是c++是非托管的内存管理， 因此还需要提供收到析构相关的接口：

```cpp
void Dispose(Astc *astc) {
    if (astc) {
        delete[] astc->ptr;
        delete astc;
        astc = nullptr;
    }
}
```


[i1]: https://github.com/ARM-software/astc-encoder/
[i2]: https://github.com/ARM-software/astc-encoder/blob/main/Docs/FormatOverview.md
[i3]: https://huailiang.github.io/blog/2022/astc