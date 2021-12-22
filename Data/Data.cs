using System;
using System.Collections.Generic;
using System.Text;

namespace DataPemain
{
    public enum Packet
    {
        SpawnPemain = 1,
        MovePemain,
        AttackPemain
    }
    class Data
    {
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;
    }
}
