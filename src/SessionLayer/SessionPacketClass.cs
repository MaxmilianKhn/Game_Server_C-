using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using src.Security;

namespace src.SessionLayer
{
    //
    // Zusammenfassung:
    //     Stellt den größtmöglichen Wert von System.UInt16 dar. Dieses Feld ist konstant.
    [ComVisible(true)]
    public class SessionPacket
    {
        private List<byte>sessionPacket=new List<byte> { 0, 0, 0, 0, 0, 0, 0 };//Do not touch from outside
        private byte version
        {
            get=> (byte)((this.sessionPacket[0] & 0b11000000) >> 6);
            set
            {
                this.sessionPacket[0]=(byte)(this.sessionPacket[0] & (~0b11000000));
                this.sessionPacket[0] = (byte)(this.sessionPacket[0] | ((value & 0b11) << 6));
            }
        }
        private byte invalidate
        {
            get => (byte)((this.sessionPacket[0] & 0b100000) >> 5);
            set
            {
                this.sessionPacket[0] = (byte)(this.sessionPacket[0] & (~0b100000));
                this.sessionPacket[0] = (byte)(this.sessionPacket[0] | ((value & 0b1) << 5));
            }
        }       
        private byte result
        {
            get => (byte)((this.sessionPacket[0] & 0b10000) >> 4);
            set
            {
                this.sessionPacket[0] = (byte)(this.sessionPacket[0] & (~0b10000));
                this.sessionPacket[0] = (byte)(this.sessionPacket[0] | ((value & 0b1) << 4));
            }
        }
        private byte challengeResponse
        {
            get => (byte)((this.sessionPacket[0] & 0b1000) >> 3);
            set
            {
                this.sessionPacket[0] = (byte)(this.sessionPacket[0] & (~0b1000));
                this.sessionPacket[0] = (byte)(this.sessionPacket[0] | ((value & 0b1) << 3));
            }
        }
        private byte challenge
        {
            get => (byte)((this.sessionPacket[0] & 0b100) >> 2);
            set
            {
                this.sessionPacket[0] = (byte)(this.sessionPacket[0] & (~0b100));
                this.sessionPacket[0] = (byte)(this.sessionPacket[0] | ((value & 0b1) << 2));
            }
        }
        private byte request
        {
            get => (byte)((this.sessionPacket[0] & 0b10) >> 1);
            set
            {
                this.sessionPacket[0] = (byte)(this.sessionPacket[0] & (~0b10));
                this.sessionPacket[0] = (byte)(this.sessionPacket[0] | ((value & 0b1) << 1));
            }
        }
        private byte heartbeat
        {
            get => (byte)(this.sessionPacket[0] & 0b1);
            set
            {
                this.sessionPacket[0] = (byte)(this.sessionPacket[0] & (~0b1));
                this.sessionPacket[0] = (byte)(this.sessionPacket[0] | (value & 0b1));
            }
        }
        private ushort lengthPayload
        {
            get =>(ushort)(this.sessionPacket[2]+(this.sessionPacket[1]<<8));
            set
            {
                this.sessionPacket[1] = (byte)((value & 0xFF00) >> 8);
                this.sessionPacket[2] = (byte)(value & 0xFF);
            }
        }
        private ushort ID
        {
            get => (ushort)(this.sessionPacket[4] + (this.sessionPacket[3] << 8));
            set
            {
                this.sessionPacket[3] = (byte)((value & 0xFF00) >> 8);
                this.sessionPacket[4] = (byte)(value & 0xFF);
            }
        }
        private ushort sequenceNumber
        {
            get => (ushort)(this.sessionPacket[6] + (this.sessionPacket[5] << 8));
            set
            {
                this.sessionPacket[5] = (byte)((value & 0xFF00) >> 8);
                this.sessionPacket[6] = (byte)(value & 0xFF);
            }
        }
        private byte[] payload
        {
            get
            {
                byte[] temp = new byte[this.sessionPacket.Count-7];
                Array.ConstrainedCopy(this.sessionPacket.ToArray(), 7, temp, 0, temp.Length);
                return temp;
            }
            set
            {
                this.sessionPacket.AddRange(value);
            }
        }
        private ushort HMAC { get; set; }//Do not touch from outside

        //Heartbeat Constructor
        private SessionPacket(ushort SessionID)
        {
            this.version = 0x00;
            this.heartbeat = 0x1;
            this.ID = SessionID;
        }
        //Standard Transmisson Constructor
        private SessionPacket(byte[] txPayloadPacket, ushort seqNum)
        {
            this.version = 0x00;
            this.payload = txPayloadPacket;
            this.lengthPayload = (ushort)txPayloadPacket.Length;
            this.sequenceNumber = seqNum;
        }
        //Standard Receive Constructor
        private SessionPacket(byte[] rxPacket)
        {
            this.sessionPacket.AddRange(rxPacket);
            this.HMAC = (ushort)(this.sessionPacket[this.sessionPacket.Count - 1]);
            this.HMAC = (ushort)(this.sessionPacket[this.sessionPacket.Count - 2]<<8);
            this.sessionPacket.RemoveRange(this.sessionPacket.Count - 2, 2);
        }
        //Returns a byte Array which can be transmitted
        private byte[] ToTxpacket()
        {
            byte[] txpacket = new byte[2 + this.sessionPacket.Count];//2 Bytes for HMAC
            Array.ConstrainedCopy(this.sessionPacket.ToArray(), 0, txpacket, 0,this.sessionPacket.Count);
            this.HMAC = this.CalculateHMAC();
            txpacket[this.sessionPacket.Count] = (byte)((this.HMAC & 0xFF00) >> 8);
            txpacket[this.sessionPacket.Count + 1] = (byte)(this.HMAC & 0xFF);
            return txpacket;
        }
        private ushort CalculateHMAC()
        {
            return SecurityFunc.Hash(this.sessionPacket.ToArray());
        }
        private bool VerifyPacket()
        {
            return this.HMAC == this.CalculateHMAC();
        }
    }
}