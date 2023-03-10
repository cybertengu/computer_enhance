#include <stdio.h>

void ReadBinFile(void)
{
    static const int BufferSize = 17;
    int i;
    FILE *ptr;
    unsigned char buffer2[BufferSize];

    ptr = fopen("listing_0037_single_register_mov","rb");
    const int fileSize = fread(buffer2, sizeof(unsigned char), BufferSize, ptr);

    printf("File size = %d bytes\n", fileSize);
    printf("Size of each item in bytes = %d\n", (int)sizeof(unsigned char));

    for(i = 0; i < (fileSize / sizeof(unsigned char)); i++)
    {
        printf("0x%x ", (int)buffer2[i]);
    }
    fclose (ptr);
}

int main (void)
{
    ReadBinFile();
    printf("\nPress enter to exit\n");
    return fgetc(stdin);
}
