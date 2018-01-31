
using System.Collections.Generic;
using Emul8.Core;
using Emul8.Core.Structure;
using Emul8.Core.Structure.Registers;
using Emul8.Logging;
using Emul8.Peripherals.Bus;

namespace Emul8.Peripherals.SPI {
    public sealed class Experiment_SPI : NullRegistrationPointPeripheralContainer<ISPIPeripheral>, IDoubleWordPeripheral, IKnownSize {
        
        public Experiment_SPI(Machine machine) : base(machine) {
            CreateRegisters();
            Reset();
        }

        public override void Reset() {
            registers.Reset();
        }

        public uint ReadDoubleWord(long offset) {
            return registers.Read(offset);
        }

        public void WriteDoubleWord(long offset, uint value) {
            registers.Write(offset, value);
        }

        public long Size {
            get { return 0x0f; }
        }

        public uint RogueMethod() {
            return 0xdeadbeef;
        }

        private void CreateRegisters() {
            var registersMap = new Dictionary<long, DoubleWordRegister> {
                {(long)Registers.Control0, new DoubleWordRegister(this, 0x8)
                    .WithEnumField(1, 2, out mode, FieldMode.Read | FieldMode.Write, name:"Mode of the peripheral") 
                    .WithTag("Disable peripheral", 3, 1)
                    .WithTag("Enable interupt", 4, 1) 
                                
                },
                {(long)Registers.Control1, new DoubleWordRegister(this, 0xff)
                    .WithValueField(0, 8, out bufferSize, name: "Size of buffer", 
                                    writeCallback: (oldValue, newValue) => this.Log(LogLevel.Info, "Write to buffer {0}", newValue))
                    .WithTag("Add CRC after buffer", 9, 1)
                },
            };
            var valueRegister = new DoubleWordRegister(this, 0x55).WithValueField(0, 8, out valueContent, name: "Value registers", 
                                    writeCallback: (oldValue, newValue) => this.Log(LogLevel.Info, "Value was written {0}", newValue),
                                    readCallback:  (oldValue, newValue) => this.Log(LogLevel.Info, "Value was read {0}",    newValue));

            // store all the registers to the same IValueRegisterField => will lose the content from previous offsets
            for(var i = 0; i < this.Size - registersMap.Count; i++) {
                registersMap.Add((long)Registers.Value + i, valueRegister);
            }

            registers = new DoubleWordRegisterCollection(this, registersMap);
        }

        private DoubleWordRegisterCollection registers;
        private IEnumRegisterField<Mode>     mode;
        private IValueRegisterField          bufferSize;
        private IValueRegisterField          valueContent;

        private enum Mode {
            Active  = 0x0,
            Passive = 0x1,
            Sleep   = 0x2
        }

        private enum Registers {
            Control0 = 0x0,
            Control1 = 0x4,
            Value    = 0x8,
        }
    }
}
