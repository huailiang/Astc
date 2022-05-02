//
// Created by huailiang on 2022/5/2.
//

#include "astc.h"

//#define _DEBUG

int ReadDim(ifstream &ifs) {
    uint8_t dim[3];
    ifs.read((char *) (&dim[0]), sizeof(uint8_t));
    ifs.read((char *) (&dim[1]), sizeof(uint8_t));
    ifs.read((char *) (&dim[2]), sizeof(uint8_t));
    int decoded_dim = dim[0] + (dim[1] << 8) + (dim[2] << 16);
    return decoded_dim;
}

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

void Dispose(Astc *astc) {
    if (astc) {
        delete[] astc->ptr;
        delete astc;
        astc = nullptr;
    }
}

extern "C"
{
bool astc_read(const char *path, Astc *ptr) {
    return ReadAstc(path, ptr);
}

void astc_dispose(Astc *ptr) {
    Dispose(ptr);
}
}