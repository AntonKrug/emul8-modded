using System;
using Emul8.Core;
using Emul8.Logging;
using Emul8.Peripherals.Bus;

namespace Emul8.Peripherals.Microsemi {
    public sealed class CodeRamapper : IBytePeripheral, IWordPeripheral, IDoubleWordPeripheral, IKnownSize {

        public CodeRamapper(Machine machine) 
        {
            this.machine = machine;
        }

        public long Size { get { return 0x80000; } }

        public void WriteByte(long position, byte value) 
        {
            this.machine.SystemBus.WriteByte(GetTranslatedPosition(position), value);
        }

        public byte ReadByte(long position) 
        {
            return this.machine.SystemBus.ReadByte(GetTranslatedPosition(position));
        }

        public void WriteWord(long offset, ushort value)
        {
            this.machine.SystemBus.WriteWord(GetTranslatedPosition(offset), value);
        }

        public ushort ReadWord(long offset)
        {
            return this.machine.SystemBus.ReadWord(GetTranslatedPosition(offset));
        }

        public void WriteDoubleWord(long offset, uint value)
        {
            this.machine.SystemBus.WriteDoubleWord(GetTranslatedPosition(offset), value);
        }

        public uint ReadDoubleWord(long offset)
        {
            return this.machine.SystemBus.ReadDoubleWord(GetTranslatedPosition(offset));
        }

        public void Reset() { }

        private long GetTranslatedPosition(long position) {
            if (null == SYSREG.RemapOffset || -1 == SYSREG.RemapOffset.Item1) {
                this.Log(LogLevel.Error, "0x0 memory accessed when remaping was undefined, default to eNVM");
                SYSREG.RemapOffset = Tuple.Create(1610612736L, 1610645504L);  //0x6000 0000 + offset, 0x6000 8000 + offset
            }

            if (position < 0x8000) {
                return position + SYSREG.RemapOffset.Item1;
            }
            else {
                return position + SYSREG.RemapOffset.Item2;
            }
        }



        private readonly Machine machine;
    }
}
