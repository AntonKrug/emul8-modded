using Emul8.Core;
using Emul8.Peripherals.Bus;

namespace Emul8.Peripherals.Microsemi {
    public sealed class BitBand : IDoubleWordPeripheral, IKnownSize {

        public BitBand(Machine machine, long bitBandAlias ) 
        {
            this.machine            = machine;
            this.bitBandAlias       = bitBandAlias;
            this.registrationOffset = machine.SystemBus.GetRegistrationPoints(this).GetEnumerator().Current.Offset;
        }

        public long Size { get { return 0x2000000; } }  // 32MB region

        public void WriteDoubleWord(long offset, uint value)
        {
            long bits       = (offset - registrationOffset) / 4;
            long byteOffset = (bits / 8) + bitBandAlias;
            uint bitMask    = (uint)(1 << (int)(bits % 8L));

            uint newValue = (value > 0) ? this.machine.SystemBus.ReadByte(byteOffset) | bitMask : this.machine.SystemBus.ReadByte(byteOffset) & ~bitMask;
            this.machine.SystemBus.WriteDoubleWord(byteOffset, newValue);
        }

        public uint ReadDoubleWord(long offset)
        {
            long bits       = (offset - registrationOffset) / 4;
            long byteOffset = (bits / 8) + bitBandAlias;
            uint bitMask    = (uint)(1 << (int)(bits % 8L));

            return ((this.machine.SystemBus.ReadByte(byteOffset) & bitMask) > 0 ) ? 1u : 0u;
        }

        public void Reset() { }

        private readonly Machine machine;
        private readonly long    registrationOffset;
        private readonly long    bitBandAlias;
    }
}
