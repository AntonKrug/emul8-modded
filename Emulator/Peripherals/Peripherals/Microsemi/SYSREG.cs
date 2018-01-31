using System;
using System.Collections.Generic;
using Emul8.Core;
using Emul8.Core.Structure.Registers;
using Emul8.Peripherals.Bus;
using Emul8.Logging;

namespace Emul8.Peripherals.Microsemi
{
    public sealed class SYSREG : IDoubleWordPeripheral, IPeripheral, IKnownSize
    {

        public SYSREG(Machine machine)
        {
            CreateRegisters();
            Reset();
            this.Log(LogLevel.Info, "As defualt remapping is set to eSRAM");
        }

        public void Reset()
        {
            registers.Reset();
        }

        public uint ReadDoubleWord(long offset)
        {
            return registers.Read(offset);
        }

        public void WriteDoubleWord(long offset, uint value)
        {
            registers.Write(offset, value);
        }

        public long Size
        {
            get { return 0x290; }
        }

        public static Tuple<long, long> RemapOffset { get; set; } = Tuple.Create(536870912L, 536903680L);  // 0x2000 0000, 0x2000 8000

        private void CalculateOffset()
        {
            this.Log(LogLevel.Info, "Remapping called:");
            RemapOffset = Tuple.Create(-1L, -1L);  // unhandled state

            if(eSramRemap.Value == RemapMode.Enabled && ddrRemap.Value == RemapMode.Disabled && eNvmRemap.Value == RemapMode.Disabled)
            {
                this.Log(LogLevel.Info, "Remapping to eSRAM");
                if(eSramSwap.Value == ESramSwap.NoSwap)
                {
                    RemapOffset = Tuple.Create(536870912L, 536903680L);  // 0x2000 0000, 0x2000 8000
                }
                else if(eSramSwap.Value == ESramSwap.SwapESram0WithESram1)
                {
                    RemapOffset = Tuple.Create(536903680L, 536870912L);  // 0x2000 8000, 0x2000 0000
                }
            }
            else if(eSramRemap.Value == RemapMode.Disabled && ddrRemap.Value == RemapMode.Enabled && eNvmRemap.Value == RemapMode.Disabled)
            {
                this.Log(LogLevel.Info, "Remapping to DDR");
                RemapOffset = Tuple.Create(2684354560L, 2684387328L);  //0xA000 0000, 0xA000 8000
            }
            else if(eSramRemap.Value == RemapMode.Disabled && ddrRemap.Value == RemapMode.Disabled && eNvmRemap.Value == RemapMode.Enabled)
            {
                this.Log(LogLevel.Info, "Remapping to eNVM");
                RemapOffset = Tuple.Create(1610612736L + this.eNvmOffset.Value, 1610645504L + this.eNvmOffset.Value);  //0x6000 0000 + offset, 0x6000 8000 + offset
            }

            // if there is more than one remapper enabled at the same time, or no remapper selected at all, then stay in unhandled state

        }

        private void CreateRegisters()
        {
            var registersMap = new Dictionary<long, DoubleWordRegister> {
                {(long)Registers.ESRAM_CR, new DoubleWordRegister(this, 0)
                    .WithTag(      "31:2", "reserved")
                    .WithEnumField("1",    out eSramSwap,  name: "Ordering of eSRAM0 and eSRAM1 when remap is enabled", enumType: typeof(ESramSwap))
                    .WithEnumField("0",    out eSramRemap, name: "Enable eSRAM remap to 0x0", enumType: typeof(RemapMode))
                    .WithWriteCallback( (_, __) => CalculateOffset())
                },

                {(long)Registers.DDR_CR, new DoubleWordRegister(this, 0)
                    .WithTag(      "31:1", "reserved")
                    .WithEnumField("0",    out ddrRemap, name: "Enable DDR remap to 0x0", enumType: typeof(RemapMode))
                    .WithWriteCallback( (_, __) => CalculateOffset())
                },

                {(long)Registers.ENVM_REMAP_BASE_CR, new DoubleWordRegister(this, 0)
                    .WithTag(       "31:19", "reserved")
                    .WithValueField("18:1",  out eNvmOffset, name: "Offset within the eNVM region")
                    .WithEnumField( "0",     out eNvmRemap,  name: "Enable eNVM remap to 0x0", enumType: typeof(RemapMode))
                    .WithWriteCallback( (_, __) => CalculateOffset())
                },

                {(long)Registers.EDAC_CR, new DoubleWordRegister(this, 0)
                    .WithTag("31:7", "reserved")
                    .WithFlag(6, out edacCan,    name: "Allows the EDAC for CAN to be disabled")
                    .WithFlag(5, out edacUsb,    name: "Allows the EDAC for USB to be disabled")
                    .WithFlag(4, out edacMacRx,  name: "Allows the EDAC for Ethernet Rx RAM to be disabled")
                    .WithFlag(3, out edacMacTx,  name: "Allows the EDAC for Ethernet Tx RAM to be disabled")
                    .WithTag("2", "reserved")
                    .WithFlag(1, out edacEsram1, name: "Allows the EDAC for eSRAM1 to be disabled")
                    .WithFlag(0, out edacEsram0, name: "Allows the EDAC for eSRAM0 to be disabled")
                },

                {(long)Registers.SOFT_RESET_CR, new DoubleWordRegister(this, 0b111111111110111111111110000)
                    .WithTag(       "31:27", "reserved")
                    .WithValueField("26:0", out resetBits, name: "Reset bits for each feature/peripheral")
                },

                {(long)Registers.MSSDDR_PLL_STATUS_HIGH, new DoubleWordRegister(this, 0x6)
                    .WithTag("31:13", "reserved")
                    .WithTag("12:1", "not-implemented")
                    .WithFlag(0, out mssddrPllBypas, name: "Powers down the MPLL core and bypasses it such that PLLOUT tracks REFCLK")
                },

                {(long)Registers.MSSDDR_FACC1_CR, new DoubleWordRegister(this, 0x4000000)
                    .WithTag("31:28", "reserved")
                    .WithTag("27", "not-implemented")
                    .WithFlag(26, out mssddrPllInit, name: "Indicates whether the FACC is to be configured for PLL initialization mode")
                    .WithTag("25:13", "not-implemented")
                    .WithFlag(12, out mssddrFaacGlmuxSel, name: "Contains the select line for the four no-glitch multiplexers within the FACC, which are related to the aligned clocks. All four of these multiplexers are switched by one signal")
                    .WithTag("11:0", "not-implemented")
                },

                {(long)Registers.DEVICE_VERSION, new DoubleWordRegister(this, 0x1F802)
                    .WithTag(       "31:20", "reserved")
                    .WithValueField("19:16", out deviceVersionIdv, FieldMode.Read, name: "Internal device version")
                    .WithValueField("15:0",  out deviceVersionIdp, FieldMode.Read, name: "Internal device product")
                },

                {(long)Registers.MSSDDR_PLL_STATUS, new DoubleWordRegister(this, 3) // originaly is reset to 0 but then it will be set to values 1-3 by hardware
                    .WithTag("31:3", "reserved")
                    .WithTag("2",    "Input from System Controler 25/50MHz")
                    .WithFlag(1,     out mpllLock, FieldMode.Read, name: "MPLL lock")
                    .WithFlag(0,     out pllFabricLock, FieldMode.Read, name: "PLL fabric lock")
                }
            };

            registers = new DoubleWordRegisterCollection(this, typeof(Registers), registersMap);
        }

        public string ShowRegisters() {
            return registers.GetAllRegistersFields();
        }

        private DoubleWordRegisterCollection  registers;
        private IEnumRegisterField<RemapMode> eSramRemap;
        private IEnumRegisterField<ESramSwap> eSramSwap;
        private IEnumRegisterField<RemapMode> ddrRemap;
        private IEnumRegisterField<RemapMode> eNvmRemap;
        private IValueRegisterField           eNvmOffset;
        private IValueRegisterField           deviceVersionIdv;
        private IValueRegisterField           deviceVersionIdp;
        private IValueRegisterField           resetBits;
        private IFlagRegisterField            pllFabricLock;
        private IFlagRegisterField            mpllLock;
        private IFlagRegisterField            edacCan;
        private IFlagRegisterField            edacUsb;
        private IFlagRegisterField            edacMacRx;
        private IFlagRegisterField            edacMacTx;
        private IFlagRegisterField            edacEsram1;
        private IFlagRegisterField            edacEsram0;
        private IFlagRegisterField            mssddrPllBypas;
        private IFlagRegisterField            mssddrPllInit;
        private IFlagRegisterField            mssddrFaacGlmuxSel;

        private enum RemapMode
        {
            Disabled = 0x0,
            Enabled = 0x1,
        }

        private enum ESramSwap
        {
            NoSwap = 0x0,
            SwapESram0WithESram1 = 0x1,
        }

        private enum Registers
        {
            ESRAM_CR               = 0x0,
            DDR_CR                 = 0x08,
            ENVM_REMAP_BASE_CR     = 0x10,
            DDRB_NB_SIZE_CR        = 0x30,
            DDRB_CR                = 0x34,
            EDAC_CR                = 0x38,
            SOFT_RESET_CR          = 0x48,
            WDOG_CR                = 0x6C,
            MSSDDR_PLL_STATUS_HIGH = 0x94,
            MSSDDR_FACC1_CR        = 0x98,
            DEVICE_VERSION         = 0x14C,
            MSSDDR_PLL_STATUS      = 0x150
        }

    }
}
