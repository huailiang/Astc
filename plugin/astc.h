//
// Created by huailiang on 2022/5/2.
//

#ifndef ASTCPLUGIN_ASTC_H
#define ASTCPLUGIN_ASTC_H

#include <iostream>
#include <fstream>
#include <string>
#include "common.h"

using namespace std;

struct Astc {
    uint8_t block_x, block_y, block_z;
    int32_t dim_x, dim_y, dim_z;
    int32_t size;
    char *ptr;
};


int ReadDim(ifstream &ifs);

bool ReadAstc(const char *path, Astc *astc);

void Dispose(Astc *astc);

extern "C"
{
    ASTC_API bool astc_read(const char *path, Astc *ptr);

    ASTC_API void astc_dispose(Astc *ptr);
}


#endif //ASTCPLUGIN_ASTC_H
