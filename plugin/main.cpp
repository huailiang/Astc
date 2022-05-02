#include <iostream>
#include <string>
#include "astc.h"

using namespace std;

#define _DEBUG


int main() {
    cout << "Hello, World!" <<sizeof(char)<< endl;
    Astc *astc = new Astc();
    ReadAstc("../ldr.astc", astc);
    cout << "end dim_x: " << astc->dim_x <<" len: "<<astc->size<< endl;
    delete[] astc->ptr;
    delete astc;
    return 0;
}
