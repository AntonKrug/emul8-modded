//
// Copyright (c) Antmicro
// Copyright (c) Realtime Embedded
//
// This file is part of the Emul8 project.
// Full license details are defined in the 'LICENSE' file.
//
using System;
using Emul8.Peripherals;
using System.Collections.Generic;
using Emul8.Utilities;
using System.Linq;
using Emul8.Logging;

namespace Emul8.Core.Structure.Registers
{
    /// <summary>
    /// 32 bit <see cref="PeripheralRegister"/>.
    /// </summary>
    public sealed class DoubleWordRegister : PeripheralRegister
    {
        /// <summary>
        /// Creates a register with one field, serving a purpose of read and write register.
        /// </summary>
        /// <returns>A new register.</returns>
        /// <param name="resetValue">Reset value.</param>
        /// <param name="name">Ignored parameter, for convenience. Treat it as a comment.</param>
        public static DoubleWordRegister CreateRWRegister(uint resetValue = 0, string name = null)
        {
            //null because parent is used for logging purposes only - this will never happen in this case.
            var register = new DoubleWordRegister(null, resetValue);
            register.DefineValueField(0, register.MaxRegisterLength);
            return register;
        }
                   
        public DoubleWordRegister(IPeripheral parent, uint resetValue = 0) : base(parent, resetValue, 32)
        {
        }

        /// <summary>
        /// Retrieves the current value of readable fields. All FieldMode values are interpreted and callbacks are executed where applicable.
        /// </summary>
        public uint Read()
        {
            return ReadInner();
        }

        /// <summary>
        /// Writes the given value to writeable fields. All FieldMode values are interpreted and callbacks are executed where applicable.
        /// </summary>
        public void Write(long offset, uint value)
        {
            WriteInner(offset, value);
        }


        /// <summary>
        /// Defines the read callback that is called once on each read, regardles of the number of defined register fields.
        /// Note that it will also be called for unreadable registers.
        /// </summary>
        /// <param name="readCallback">Method to be called whenever this register is read. The first parameter is the value of this register before read,
        /// the second parameter is the value after read.</param>
        public void DefineReadCallback(Action<uint, uint> readCallback)
        {
            readCallbacks.Add(readCallback);
        }

        /// <summary>
        /// Defines the write callback that is called once on each write, regardles of the number of defined register fields.
        /// Note that it will also be called for unwrittable registers.
        /// </summary>
        /// <param name="writeCallback">Method to be called whenever this register is written to. The first parameter is the value of this register before write,
        /// the second parameter is the value written (without any modification).</param>
        public void DefineWriteCallback(Action<uint, uint> writeCallback)
        {
            writeCallbacks.Add(writeCallback);
        }

        /// <summary>
        /// Defines the change callback that is called once on each change, regardles of the number of defined register fields.
        /// Note that it will also be called for unwrittable registers.
        /// </summary>
        /// <param name="changeCallback">Method to be called whenever this register's value is changed, either due to read or write. The first parameter is the value of this register before change,
        /// the second parameter is the value after change.</param>
        public void DefineChangeCallback(Action<uint, uint> changeCallback)
        {
            changeCallbacks.Add(changeCallback);
        }

        /// <summary>
        /// Gets the underlying value without any modification or reaction.
        /// </summary>
        public uint Value
        {
            get
            {
                return UnderlyingValue;
            }
        }

        protected override void CallChangeHandlers(uint oldValue, uint newValue)
        {
            CallHandlers(changeCallbacks, oldValue, newValue);
        }

        protected override void CallReadHandlers(uint oldValue, uint newValue)
        {
            CallHandlers(readCallbacks, oldValue, newValue);
        }

        protected override void CallWriteHandlers(uint oldValue, uint newValue)
        {
            CallHandlers(writeCallbacks, oldValue, newValue);
        }

        private List<Action<uint, uint>> readCallbacks = new List<Action<uint, uint>>();
        private List<Action<uint, uint>> writeCallbacks = new List<Action<uint, uint>>();
        private List<Action<uint, uint>> changeCallbacks = new List<Action<uint, uint>>();
    }

    /// <summary>
    /// 16 bit <see cref="PeripheralRegister"/>.
    /// </summary>
    public sealed class WordRegister : PeripheralRegister
    {
        /// <summary>
        /// Creates a register with one field, serving a purpose of read and write register.
        /// </summary>
        /// <returns>A new register.</returns>
        /// <param name="resetValue">Reset value.</param>
        /// <param name="name">Ignored parameter, for convenience. Treat it as a comment.</param>
        public static WordRegister CreateRWRegister(uint resetValue = 0, string name = null)
        {
            //null because parent is used for logging purposes only - this will never happen in this case.
            var register = new WordRegister(null, resetValue);
            register.DefineValueField(0, register.MaxRegisterLength);
            return register;
        }

        public WordRegister(IPeripheral parent, uint resetValue = 0) : base(parent, resetValue, 16)
        {
        }

        /// <summary>
        /// Retrieves the current value of readable fields. All FieldMode values are interpreted and callbacks are executed where applicable.
        /// </summary>
        public ushort Read()
        {
            return (ushort)ReadInner();
        }

        /// <summary>
        /// Writes the given value to writeable fields. All FieldMode values are interpreted and callbacks are executed where applicable.
        /// </summary>
        public void Write(long offset, ushort value)
        {
            WriteInner(offset, value);
        }



        /// <summary>
        /// Defines the read callback that is called once on each read, regardles of the number of defined register fields.
        /// Note that it will also be called for unreadable registers.
        /// </summary>
        /// <param name="readCallback">Method to be called whenever this register is read. The first parameter is the value of this register before read,
        /// the second parameter is the value after read.</param>
        public void DefineReadCallback(Action<ushort, ushort> readCallback)
        {
            readCallbacks.Add(readCallback);
        }

        /// <summary>
        /// Defines the write callback that is called once on each write, regardles of the number of defined register fields.
        /// Note that it will also be called for unwrittable registers.
        /// </summary>
        /// <param name="writeCallback">Method to be called whenever this register is written to. The first parameter is the value of this register before write,
        /// the second parameter is the value written (without any modification).</param>
        public void DefineWriteCallback(Action<ushort, ushort> writeCallback)
        {
            writeCallbacks.Add(writeCallback);
        }

        /// <summary>
        /// Defines the change callback that is called once on each change, regardles of the number of defined register fields.
        /// Note that it will also be called for unwrittable registers.
        /// </summary>
        /// <param name="changeCallback">Method to be called whenever this register's value is changed, either due to read or write. The first parameter is the value of this register before change,
        /// the second parameter is the value after change.</param>
        public void DefineChangeCallback(Action<ushort, ushort> changeCallback)
        {
            changeCallbacks.Add(changeCallback);
        }

        /// <summary>
        /// Gets the underlying value without any modification or reaction.
        /// </summary>
        public ushort Value
        {
            get
            {
                return (ushort)UnderlyingValue;
            }
        }

        protected override void CallChangeHandlers(uint oldValue, uint newValue)
        {
            CallHandlers(changeCallbacks, (ushort)oldValue, (ushort)newValue);
        }

        protected override void CallReadHandlers(uint oldValue, uint newValue)
        {
            CallHandlers(readCallbacks, (ushort)oldValue, (ushort)newValue);
        }

        protected override void CallWriteHandlers(uint oldValue, uint newValue)
        {
            CallHandlers(writeCallbacks, (ushort)oldValue, (ushort)newValue);
        }

        private List<Action<ushort, ushort>> readCallbacks = new List<Action<ushort, ushort>>();
        private List<Action<ushort, ushort>> writeCallbacks = new List<Action<ushort, ushort>>();
        private List<Action<ushort, ushort>> changeCallbacks = new List<Action<ushort, ushort>>();
    }

    /// <summary>
    /// 8 bit <see cref="PeripheralRegister"/>.
    /// </summary>
    public sealed class ByteRegister : PeripheralRegister
    {
        /// <summary>
        /// Creates a register with one field, serving a purpose of read and write register.
        /// </summary>
        /// <returns>A new register.</returns>
        /// <param name="resetValue">Reset value.</param>
        /// <param name="name">Ignored parameter, for convenience. Treat it as a comment.</param>
        public static ByteRegister CreateRWRegister(uint resetValue = 0, string name = null)
        {
            //null because parent is used for logging purposes only - this will never happen in this case.
            var register = new ByteRegister(null, resetValue);
            register.DefineValueField(0, register.MaxRegisterLength);
            return register;
        }

        public ByteRegister(IPeripheral parent, uint resetValue = 0) : base(parent, resetValue, 8)
        {
        }

        /// <summary>
        /// Retrieves the current value of readable fields. All FieldMode values are interpreted and callbacks are executed where applicable.
        /// </summary>
        public byte Read()
        {
            return (byte)ReadInner();
        }

        /// <summary>
        /// Writes the given value to writeable fields. All FieldMode values are interpreted and callbacks are executed where applicable.
        /// </summary>
        public void Write(long offset, byte value)
        {
            WriteInner(offset, value);
        }

        /// <summary>
        /// Defines the read callback that is called once on each read, regardles of the number of defined register fields.
        /// Note that it will also be called for unreadable registers.
        /// </summary>
        /// <param name="readCallback">Method to be called whenever this register is read. The first parameter is the value of this register before read,
        /// the second parameter is the value after read.</param>
        public void DefineReadCallback(Action<byte, byte> readCallback)
        {
            readCallbacks.Add(readCallback);
        }

        /// <summary>
        /// Defines the write callback that is called once on each write, regardles of the number of defined register fields.
        /// Note that it will also be called for unwrittable registers.
        /// </summary>
        /// <param name="writeCallback">Method to be called whenever this register is written to. The first parameter is the value of this register before write,
        /// the second parameter is the value written (without any modification).</param>
        public void DefineWriteCallback(Action<byte, byte> writeCallback)
        {
            writeCallbacks.Add(writeCallback);
        }

        /// <summary>
        /// Defines the change callback that is called once on each change, regardles of the number of defined register fields.
        /// Note that it will also be called for unwrittable registers.
        /// </summary>
        /// <param name="changeCallback">Method to be called whenever this register's value is changed, either due to read or write. The first parameter is the value of this register before change,
        /// the second parameter is the value after change.</param>
        public void DefineChangeCallback(Action<byte, byte> changeCallback)
        {
            changeCallbacks.Add(changeCallback);
        }

        /// <summary>
        /// Gets the underlying value without any modification or reaction.
        /// </summary>
        public byte Value
        {
            get
            {
                return (byte)UnderlyingValue;
            }
        }

        protected override void CallChangeHandlers(uint oldValue, uint newValue)
        {
            CallHandlers(changeCallbacks, (byte)oldValue, (byte)newValue);
        }

        protected override void CallReadHandlers(uint oldValue, uint newValue)
        {
            CallHandlers(readCallbacks, (byte)oldValue, (byte)newValue);
        }

        protected override void CallWriteHandlers(uint oldValue, uint newValue)
        {
            CallHandlers(writeCallbacks, (byte)oldValue, (byte)newValue);
        }

        private List<Action<byte, byte>> readCallbacks = new List<Action<byte, byte>>();
        private List<Action<byte, byte>> writeCallbacks = new List<Action<byte, byte>>();
        private List<Action<byte, byte>> changeCallbacks = new List<Action<byte, byte>>();

    }

    /// <summary>
    /// Represents a register of a given width, containing defined fields.
    /// Fields may not exceed this register's width, nor may they overlap each other.
    /// Fields that are not handled (e.g. left for future implementation or unimportant) have to be tagged.
    /// Otherwise, they will not be logged.
    /// </summary>
    public abstract partial class PeripheralRegister
    {

        public string GetFields() {
            string ret = "";
            foreach (RegisterField field in registerFields) {
                ret += field;
            }
            ret += "unimplemented/reserved fields:\r\n";
            foreach (Tag tag in tags) {
                ret += tag;
            }

            return ret;
        }

        /// <summary>
        /// Restores this register's value to its reset value, defined on per-field basis.
        /// </summary>
        public void Reset()
        {
            UnderlyingValue = resetValue;
        }

        private Tuple<int, int> GetPositionAndWidth(string range, string name) 
        {
            string[] list = range.Split(':');

            if (2 > list.Length && Int32.TryParse(range, out int singleBitBegin)) {
                return Tuple.Create(singleBitBegin, 1);
            }
            else if (2 == list.Length && Int32.TryParse(list[1], out int begin) && Int32.TryParse(list[0], out int end)) {
                return Tuple.Create(begin, (end-begin)+1);
            }

            throw new ArgumentException("Range has wrong syntax for register {0}. Should be in \"31:2\" syntax, or \"22\" for single bits, but not the \"{1}\"".FormatWith(name, range));
        }

        /// <summary>
        /// Mark an unhandled field, so it is logged with its name.
        /// </summary>
        /// <param name="name">Name of the unhandled field.</param>
        /// <param name="position">Offset in the register.</param>
        /// <param name="width">Width of field.</param>
        public void Tag(string name, int position, int width)
        {
            ThrowIfRangeIllegal(position, width, name);
            tags.Add(new Tag
            {
                Name = name,
                Position = position,
                Width = width
            });
        }

        public void Tag(string name, string range)
        {
            Tuple<int, int> positionAndWidth = GetPositionAndWidth(range, name);
            this.Tag(name, positionAndWidth.Item1, positionAndWidth.Item2);
        }

        /// <summary>
        /// Defines the flag field. Its width is always 1 and is interpreted as boolean value.
        /// </summary>
        /// <param name="position">Offset in the register.</param>
        /// <param name="mode">Access modifiers of this field.</param>
        /// <param name="readCallback">Method to be called whenever the containing register is read. The first parameter is the value of this field before read,
        /// the second parameter is the value after read. Note that it will also be called for unreadable fields.</param>
        /// <param name="writeCallback">Method to be called whenever the containing register is written to. The first parameter is the value of this field before write,
        /// the second parameter is the value written (without any modification). Note that it will also be called for unwrittable fields.</param>
        /// <param name="changeCallback">Method to be called whenever this field's value is changed, either due to read or write. The first parameter is the value of this field before change,
        /// the second parameter is the value after change. Note that it will also be called for unwrittable fields.</param>
        /// <param name="valueProviderCallback">Method to be called whenever this field is read. The value passed is the current field's value, that will be overwritten by
        /// the value returned from it. This returned value is eventually passed as the first parameter of <paramref name="readCallback"/>.</param>
        /// <param name="name">Ignored parameter, for convenience. Treat it as a comment.</param>
        public IFlagRegisterField DefineFlagField(int position, FieldMode mode = FieldMode.Read | FieldMode.Write, Action<bool, bool> readCallback = null,
            Action<bool, bool> writeCallback = null, Action<bool, bool> changeCallback = null, Func<bool, bool> valueProviderCallback = null, string name = null)
        {
            ThrowIfRangeIllegal(position, 1, name);
            var field = new FlagRegisterField(this, position, mode, readCallback, writeCallback, changeCallback, valueProviderCallback, name);
            registerFields.Add(field);
            RecalculateFieldMask();
            return field;
        }




        /// <summary>
        /// Defines the value field. Its value is interpreted as a regular number.
        /// </summary>
        /// <param name="position">Offset in the register.</param>
        /// <param name="width">Maximum width of the value, in terms of binary representation.</param> 
        /// <param name="mode">Access modifiers of this field.</param>
        /// <param name="readCallback">Method to be called whenever the containing register is read. The first parameter is the value of this field before read,
        /// the second parameter is the value after read. Note that it will also be called for unreadable fields.</param>
        /// <param name="writeCallback">Method to be called whenever the containing register is written to. The first parameter is the value of this field before write,
        /// the second parameter is the value written (without any modification). Note that it will also be called for unwrittable fields.</param>
        /// <param name="changeCallback">Method to be called whenever this field's value is changed, either due to read or write. The first parameter is the value of this field before change,
        /// the second parameter is the value after change. Note that it will also be called for unwrittable fields.</param>
        /// <param name="valueProviderCallback">Method to be called whenever this field is read. The value passed is the current field's value, that will be overwritten by
        /// the value returned from it. This returned value is eventually passed as the first parameter of <paramref name="readCallback"/>.</param>
        /// <param name="name">Ignored parameter, for convenience. Treat it as a comment.</param>
        public IValueRegisterField DefineValueField(int position, int width, FieldMode mode = FieldMode.Read | FieldMode.Write, Action<uint, uint> readCallback = null,
            Action<uint, uint> writeCallback = null, Action<uint, uint> changeCallback = null, Func<uint, uint> valueProviderCallback = null, string name = null)
        {
            ThrowIfRangeIllegal(position, width, name);
            var field = new ValueRegisterField(this, position, width, mode, readCallback, writeCallback, changeCallback, valueProviderCallback, name);
            registerFields.Add(field);
            RecalculateFieldMask();
            return field;
        }

        public IValueRegisterField DefineValueField(string range, FieldMode mode = FieldMode.Read | FieldMode.Write, Action<uint, uint> readCallback = null,
            Action<uint, uint> writeCallback = null, Action<uint, uint> changeCallback = null, Func<uint, uint> valueProviderCallback = null, string name = null)
        {
            Tuple<int, int> positionAndWidth = GetPositionAndWidth(range, name);
            return DefineValueField(positionAndWidth.Item1, positionAndWidth.Item2, mode, readCallback, writeCallback, changeCallback, valueProviderCallback, name);
        }

        /// <summary>
        /// Defines the enum field. Its value is interpreted as an enumeration
        /// </summary>
        /// <param name="position">Offset in the register.</param>
        /// <param name="width">Maximum width of the value, in terms of binary representation.</param> 
        /// <param name="mode">Access modifiers of this field.</param>
        /// <param name="readCallback">Method to be called whenever the containing register is read. The first parameter is the value of this field before read,
        /// the second parameter is the value after read. Note that it will also be called for unreadable fields.</param>
        /// <param name="writeCallback">Method to be called whenever the containing register is written to. The first parameter is the value of this field before write,
        /// the second parameter is the value written (without any modification). Note that it will also be called for unwrittable fields.</param>
        /// <param name="changeCallback">Method to be called whenever this field's value is changed, either due to read or write. The first parameter is the value of this field before change,
        /// the second parameter is the value after change. Note that it will also be called for unwrittable fields.</param>
        /// <param name="valueProviderCallback">Method to be called whenever this field is read. The value passed is the current field's value, that will be overwritten by
        /// the value returned from it. This returned value is eventually passed as the first parameter of <paramref name="readCallback"/>.</param> 
        /// <param name="name">Ignored parameter, for convenience. Treat it as a comment.</param>
        public IEnumRegisterField<TEnum> DefineEnumField<TEnum>(int position, int width, FieldMode mode = FieldMode.Read | FieldMode.Write, Action<TEnum, TEnum> readCallback = null,
            Action<TEnum, TEnum> writeCallback = null, Action<TEnum, TEnum> changeCallback = null, Func<TEnum, TEnum> valueProviderCallback = null, string name = null, Type enumType = null)
            where TEnum : struct, IConvertible
        {
            ThrowIfRangeIllegal(position, width, name);
            var field = new EnumRegisterField<TEnum>(this, position, width, mode, readCallback, writeCallback, changeCallback, valueProviderCallback, name, enumType);
            registerFields.Add(field);
            RecalculateFieldMask();
            return field;
        }

        public IEnumRegisterField<TEnum> DefineEnumField<TEnum>(string range, FieldMode mode = FieldMode.Read | FieldMode.Write, Action<TEnum, TEnum> readCallback = null,
                                                                Action<TEnum, TEnum> writeCallback = null, Action<TEnum, TEnum> changeCallback = null, Func<TEnum, TEnum> valueProviderCallback = null, string name = null, Type enumType = null)
            where TEnum : struct, IConvertible
        {
            Tuple<int, int> positionAndWidth = GetPositionAndWidth(range, name);
            return DefineEnumField(positionAndWidth.Item1, positionAndWidth.Item2, mode, readCallback, writeCallback, changeCallback, valueProviderCallback, name, enumType);
        }

        protected PeripheralRegister(IPeripheral parent, uint resetValue, int maxLength)
        {
            this.parent = parent;
            this.MaxRegisterLength = maxLength;
            this.resetValue = resetValue;
            Reset();
        }

        protected uint ReadInner()
        {
            foreach(var registerField in registerFields)
            {
                UnderlyingValue = registerField.CallValueProviderHandler(UnderlyingValue);
            }
            var baseValue = UnderlyingValue;
            var valueToRead = UnderlyingValue;
            var changedFields = new List<RegisterField>();
            foreach(var registerField in registerFields)
            {
                if(!registerField.fieldMode.IsReadable())
                {
                    BitHelper.ClearBits(ref valueToRead, registerField.position, registerField.width);
                }
                if(registerField.fieldMode.IsFlagSet(FieldMode.ReadToClear)
                   && BitHelper.AreAnyBitsSet(UnderlyingValue, registerField.position, registerField.width))
                {
                    BitHelper.ClearBits(ref UnderlyingValue, registerField.position, registerField.width);
                    changedFields.Add(registerField);
                }
            }
            foreach(var registerField in registerFields)
            {
                registerField.CallReadHandler(baseValue, UnderlyingValue);
            }
            foreach(var changedRegister in changedFields.Distinct())
            {
                changedRegister.CallChangeHandler(baseValue, UnderlyingValue);
            }

            CallReadHandlers(baseValue, UnderlyingValue);
            if(changedFields.Any())
            {
                CallChangeHandlers(baseValue, UnderlyingValue);
            }

            return valueToRead;
        }

        protected void WriteInner(long offset, uint value)
        {
            var baseValue = UnderlyingValue;
            var difference = UnderlyingValue ^ value;
            var setRegisters = value & (~UnderlyingValue);
            var changedRegisters = new List<RegisterField>();
            foreach(var registerField in registerFields)
            {
                //switch is OK, because write modes are exclusive.
                switch(registerField.fieldMode.WriteBits())
                {
                case FieldMode.Write:
                    if(BitHelper.AreAnyBitsSet(difference, registerField.position, registerField.width))
                    {
                        BitHelper.UpdateWith(ref UnderlyingValue, value, registerField.position, registerField.width);
                        changedRegisters.Add(registerField);
                    }
                    break;
                case FieldMode.Set:
                    if(BitHelper.AreAnyBitsSet(setRegisters, registerField.position, registerField.width))
                    {
                        BitHelper.OrWith(ref UnderlyingValue, setRegisters, registerField.position, registerField.width);
                        changedRegisters.Add(registerField);
                    }
                    break;
                case FieldMode.Toggle:
                    if(BitHelper.AreAnyBitsSet(value, registerField.position, registerField.width))
                    {
                        BitHelper.XorWith(ref UnderlyingValue, value, registerField.position, registerField.width);
                        changedRegisters.Add(registerField);
                    }
                    break;
                case FieldMode.WriteOneToClear:
                    if(BitHelper.AreAnyBitsSet(value, registerField.position, registerField.width))
                    {
                        BitHelper.AndWithNot(ref UnderlyingValue, value, registerField.position, registerField.width);
                        changedRegisters.Add(registerField);
                    }
                    break;
                case FieldMode.WriteZeroToClear:
                    if(BitHelper.AreAnyBitsSet(~value, registerField.position, registerField.width))
                    {
                        BitHelper.AndWithNot(ref UnderlyingValue, ~value, registerField.position, registerField.width);
                        changedRegisters.Add(registerField);
                    }
                    break;
                }
            }
            foreach(var registerField in registerFields)
            {
                registerField.CallWriteHandler(baseValue, value);
            }
            foreach(var changedRegister in changedRegisters.Distinct())
            {
                changedRegister.CallChangeHandler(baseValue, UnderlyingValue);
            }

            CallWriteHandlers(baseValue, value);
            if(changedRegisters.Any())
            {
                CallChangeHandlers(baseValue, UnderlyingValue);
            }

            var unhandledWrites = difference & ~definedFieldsMask;
            if(unhandledWrites != 0)
            {
                parent.Log(LogLevel.Warning, TagLogger(offset, unhandledWrites, value));
            }
        }

        protected void CallHandlers<T>(List<Action<T, T>> handlers, T oldValue, T newValue)
        {
            foreach(var handler in handlers)
            {
                handler(oldValue, newValue);
            }
        }

        protected abstract void CallWriteHandlers(uint oldValue, uint newValue);
        protected abstract void CallReadHandlers(uint oldValue, uint newValue);
        protected abstract void CallChangeHandlers(uint oldValue, uint newValue);


        protected uint UnderlyingValue;

        protected readonly int MaxRegisterLength;

        /// <summary>
        /// Returns information about tag writes. Extracted as a method to allow future lazy evaluation.
        /// </summary>
        /// <param name="offset">The offset of the affected register.</param>
        /// <param name="value">Unhandled value.</param>
        /// <param name="originalValue">The whole value written to the register.</param>
        private string TagLogger(long offset, uint value, uint originalValue)
        {
            var tagsAffected = tags.Select(x => new { x.Name, Value = BitHelper.GetValue(value, x.Position, x.Width) })
                .Where(x => x.Value != 0);
            return "Unhandled write to offset 0x{2:X}. Unhandled bits: [{1}] when writing value 0x{3:X}.{0}"
                .FormatWith(tagsAffected.Any() ? " Tags: {0}.".FormatWith(
                    tagsAffected.Select(x => "{0} (0x{1:X})".FormatWith(x.Name, x.Value)).Stringify(", ")) : String.Empty,
                    BitHelper.GetSetBitsPretty(value),
                    offset,
                    originalValue);
        }

        private void ThrowIfRangeIllegal(int position, int width, string name)
        {
            if(width <= 0)
            {
                throw new ArgumentException("Field {0} has to have a size > 0.".FormatWith(name ?? "at {0} of {1} bits".FormatWith(position, width)));
            }
            if(position + width > MaxRegisterLength)
            {
                throw new ArgumentException("Field {0} does not fit in the register size.".FormatWith(name ?? "at {0} of {1} bits".FormatWith(position, width)));
            }
            foreach(var field in registerFields.Select(x => new { x.position, x.width }).Concat(tags.Select(x => new { position = x.Position, width = x.Width })))
            {
                var minEnd = Math.Min(position + width, field.position + field.width);
                var maxStart = Math.Max(position, field.position);
                if(minEnd > maxStart)
                {
                    throw new ArgumentException("Field {0} intersects with another range.".FormatWith(name ?? "at {0} of {1} bits".FormatWith(position, width)));
                }
            }
        }

        private void RecalculateFieldMask()
        {
            var mask = 0u;
            foreach(var field in registerFields)
            {
                if(field.width == 32)
                {
                    mask = uint.MaxValue;
                    break;
                }
                mask |= ((1u << field.width) - 1) << field.position;
            }
            definedFieldsMask = mask;
        }

        private List<RegisterField> registerFields = new List<RegisterField>();

        private List<Tag> tags = new List<Tag>();

        private IPeripheral parent;

        private uint definedFieldsMask;

        private readonly uint resetValue;
    }
}
