﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemizerLibrary
{
    public class DungeonObjectDataPointerCollection
    {
        public const int TotalRooms = 320;

        public Dictionary<int, DungeonObjectDataPointer> RoomDungeonObjectDataPointers { get; private set; } = new Dictionary<int, DungeonObjectDataPointer>();

        RomData romData;

        public DungeonObjectDataPointerCollection(RomData romData)
        {
            this.romData = romData;
            for (int i=0; i<TotalRooms; i++)
            {
                var pointerAddress = AddressConstants.ObjectDataPointerTableAddress + (i * 3);
                RoomDungeonObjectDataPointers.Add(i, new DungeonObjectDataPointer(romData, i, pointerAddress, romData.GetDataChunk(pointerAddress, 3)));
            }
        }

        public void AddShellAndMoveObjectData(int roomId, int x, int y, bool clearLayer2, int shellId)
        {
            DungeonObjectDataPointer roomData;
            if(RoomDungeonObjectDataPointers.TryGetValue(roomId, out roomData))
            {
                roomData.AddShell(x, y, clearLayer2, shellId);
            }
            else
            {
                throw new Exception($"Invalid room {roomId}");
            }
        }

        public void RemoveShellAndMoveObjectData(int roomId, int shellId)
        {
            DungeonObjectDataPointer roomData;
            if (RoomDungeonObjectDataPointers.TryGetValue(roomId, out roomData))
            {
                roomData.RemoveShell(shellId);
            }
            else
            {
                throw new Exception($"Invalid room {roomId}");
            }
        }

        public void WriteChangesToRom(int startingAddress)
        {
            foreach(var d in this.RoomDungeonObjectDataPointers.Values.Where(x => x.IsModified))
            {
                startingAddress = d.WriteRom(romData, startingAddress);
            }
        }
    }

    public class DungeonObjectDataPointer
    {
        int snesAddress;
        int pointerAddress; // the pointer table slot
        public bool IsModified { get; private set; }

        public int ROMAddress
        {
            get { return Utilities.SnesToPCAddress(snesAddress); }
            set { snesAddress = Utilities.PCToSnesAddress(value); }
        }

        public int SnesAddress
        {
            get { return snesAddress; }
            set { snesAddress = value; }
        }

        public byte[] SnesAddressBytes
        {
            get { return new byte[] { (byte)(snesAddress & 0xFF), (byte)((snesAddress >> 8) & 0xFF), (byte)((snesAddress >> 16) & 0xFF) }; }
        }

        public int RoomId { get; private set; }

        public DungeonObjectDataTable Data { get; private set; }

        public DungeonObjectDataPointer(RomData romData, int roomId, int pointerAddress, byte[] address)
        {

            this.pointerAddress = pointerAddress;
            this.RoomId = roomId;
            this.snesAddress = Utilities.SnesByteArrayTo24bitSnesAddress(address);
            this.Data = new DungeonObjectDataTable(romData, this.ROMAddress);
        }

        public void AddShell(int x, int y, bool clearLayer2, int shellId)
        {
            IsModified = true;
            Data.HeaderByte0 = 0xF0; // fix something
            if(clearLayer2)
            {
                Data.Layer2Objects.Clear(); // need for blind's room, and probably mothula if we ever make kholdstare work there
            }
            Data.Layer2Objects.Add(new SubType3Object(x, y, shellId));
        }

        public void RemoveShell(int shellId)
        {
            IsModified = true;

            foreach(var l2 in Data.Layer2Objects.ToList())
            {
                if(l2.OID == shellId)
                {
                    Data.Layer2Objects.Remove(l2);
                }
            }
        }

        // used for debug
        public void MakeShutterDoorsNormal()
        {
            foreach(var door in Data.Layer1DoorObjects.ToList())
            {
                if (trapDoors.Contains(door.Bytes[1]))
                {
                    IsModified = true;
                    if (door.Bytes[1] == 0x44)
                    {
                        door.Bytes[1] = 0x40;
                    }
                    else
                    {
                        door.Bytes[1] = 0x00;
                    }
                }
            }
            foreach (var door in Data.Layer2DoorObjects.ToList())
            {
                if (trapDoors.Contains(door.Bytes[1]))
                {
                    IsModified = true;
                    if (door.Bytes[1] == 0x44)
                    {
                        door.Bytes[1] = 0x40;
                    }
                    else
                    {
                        door.Bytes[1] = 0x00;
                    }
                }
            }
            foreach (var door in Data.Layer3DoorObjects.ToList())
            {
                if (trapDoors.Contains(door.Bytes[1]))
                {
                    IsModified = true;
                    if (door.Bytes[1] == 0x44)
                    {
                        door.Bytes[1] = 0x40;
                    }
                    else
                    {
                        door.Bytes[1] = 0x00;
                    }
                }
            }
        }
        byte[] trapDoors = { 0x16, 0x18, 0x36, 0x38, 0x44 }; //, 0x48, 0x4A };

        public int WriteRom(RomData romData, int newAddess)
        {
            //this.pointerAddress = newAddess;
            ROMAddress = newAddess;
            var snesPointer = Utilities.PCAddressToSnesByteArray(newAddess);
            // update the pointer
            romData[this.pointerAddress] = snesPointer[2];
            romData[this.pointerAddress + 1] = snesPointer[1];
            romData[this.pointerAddress + 2] = snesPointer[0];

            Data.WriteRom(romData, newAddess);
            
            // update the address
            return newAddess + Data.Length; // return the new address
        }
    }
}
