mkdir -p build_osx && cd build_osx
cmake -GXcode ../
cd ..
cmake --build build_osx --config Release
mkdir -p Plugins/xmedia.bundle/Contents/MacOS/
cp build_osx/Release/xmedia.bundle/Contents/MacOS/xmedia Plugins/xmedia.bundle/Contents/MacOS/xmedia

