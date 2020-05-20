using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace src.SessionLayer
{
    //
    // Zusammenfassung:
    //     Stellt den größtmöglichen Wert von System.UInt16 dar. Dieses Feld ist konstant.
    [ComVisible(true)]
    public class SessionPacket
    {
        private BitArray header = new BitArray(8);
        private byte version
        {
            get => 
            set
            {
                this.header.Set(6, value[0]);
                this.header.Set(7, value[1]);
            }
        }
        private bool invalidate
        {
            get => this.header.Get(5);
            set => this.header.Set(5, value);
        }
        private bool result
        {
            get => this.header.Get(4);
            set => this.header.Set(4, value);
        }
        private bool challengeResponse
        {
            get => this.header.Get(3);
            set => this.header.Set(3, value);
        }
        private bool challenge
        {
            get => this.header.Get(2);
            set => this.header.Set(2, value);
        }
        private bool request
        {
            get => this.header.Get(1);
            set => this.header.Set(1, value);
        }
        private bool heartbeat
        {
            get => this.header.Get(0);
            set => this.header.Set(0, value);
        }
        private UInt16 lengthPayload { get; set; }
        private UInt16 ID { get; set; }
        private UInt16 sequenceNumber { get; set; }
        private byte[] payload { get; set; }
        private UInt16 HMAC { get; set; }

        //Heartbeat Constructor
        private SessionPacket(UInt16 SessionID)
        {
            this.version = { false,false};
            this.heartbeat = 0b1;
        }
        private SessionPacket(byte[] rxPacket)
        {

        }
    }
}
