//
// Copyright (c) Antmicro
// Copyright (c) Realtime Embedded
//
// This file is part of the Emul8 project.
// Full license details are defined in the 'LICENSE' file.
//
using System;
using Emul8.Utilities;

namespace Emul8.Core.Structure.Registers
{
    public interface IRegisterField<T>
    {
        /// <summary>
        /// Gets or sets the field's value. Access to this property does not invoke verification procedures in terms of FieldMode checking.
        /// Also, it does not invoke callbacks.
        /// </summary>
        T Value { get; set; }
    }

    public partial class PeripheralRegister
    {
        private sealed class ValueRegisterField : RegisterField<uint>, IValueRegisterField
        {
            public ValueRegisterField(PeripheralRegister parent, int position, int width, FieldMode fieldMode, Action<uint, uint> readCallback,
                Action<uint, uint> writeCallback, Action<uint, uint> changeCallback, Func<uint, uint> valueProviderCallback, string name)
                : base(parent, position, width, fieldMode, readCallback, writeCallback, changeCallback, valueProviderCallback, name)
            {
            }

            protected override uint FromBinary(uint value)
            {
                return value;
            }

            protected override uint ToBinary(uint value)
            {
                return value;
            }
        }

        private sealed class EnumRegisterField<TEnum> : RegisterField<TEnum>, IEnumRegisterField<TEnum> where TEnum : struct, IConvertible
        {
            public EnumRegisterField(PeripheralRegister parent, int position, int width, FieldMode fieldMode, Action<TEnum, TEnum> readCallback,
                                     Action<TEnum, TEnum> writeCallback, Action<TEnum, TEnum> changeCallback, Func<TEnum, TEnum> valueProviderCallback, string name, Type enumType)
                : base(parent, position, width, fieldMode, readCallback, writeCallback, changeCallback, valueProviderCallback, name, enumType)
            {
            }

            protected override TEnum FromBinary(uint value)
            {
                return EnumConverter<TEnum>.ToEnum(value);
            }

            protected override uint ToBinary(TEnum value)
            {
                return EnumConverter<TEnum>.ToUInt(value);
            }
        }

        private sealed class FlagRegisterField : RegisterField<bool>, IFlagRegisterField
        {
            public FlagRegisterField(PeripheralRegister parent, int position, FieldMode fieldMode, Action<bool, bool> readCallback,
                Action<bool, bool> writeCallback, Action<bool, bool> changeCallback, Func<bool, bool> valueProviderCallback, string name)
                : base(parent, position, 1, fieldMode, readCallback, writeCallback, changeCallback, valueProviderCallback, name)
            {
            }

            protected override bool FromBinary(uint value)
            {
                return value != 0;
            }

            protected override uint ToBinary(bool value)
            {
                return value ? 1u : 0;
            }
        }

        private abstract class RegisterField<T> : RegisterField, IRegisterField<T>
        {
            public T Value
            {
                get
                {
                    return FromBinary(FilterValue(parent.UnderlyingValue));
                }
                set
                {
                    var binary = ToBinary(value);
                    // '1' must be of type long as width can have value up to 32 (shifting int of 32 bit would lead to overflow)
                    if(binary >= (1L << width))
                    {
                        throw new ArgumentException("Value exceeds the size of the field.");
                    }
                    WriteFiltered(binary);
                }
            }

            public override void CallReadHandler(uint oldValue, uint newValue)
            {
                if(readCallback != null)
                {
                    var oldValueFiltered = FilterValue(oldValue);
                    var newValueFiltered = FilterValue(newValue);
                    readCallback(FromBinary(oldValueFiltered), FromBinary(newValueFiltered));
                }
            }

            public override void CallWriteHandler(uint oldValue, uint newValue)
            {
                if(writeCallback != null)
                {
                    var oldValueFiltered = FilterValue(oldValue);
                    var newValueFiltered = FilterValue(newValue);
                    writeCallback(FromBinary(oldValueFiltered), FromBinary(newValueFiltered));
                }
            }

            public override void CallChangeHandler(uint oldValue, uint newValue)
            {
                if(changeCallback != null)
                {
                    var oldValueFiltered = FilterValue(oldValue);
                    var newValueFiltered = FilterValue(newValue);
                    changeCallback(FromBinary(oldValueFiltered), FromBinary(newValueFiltered));
                }
            }

            public override uint CallValueProviderHandler(uint currentValue)
            {
                if(valueProviderCallback != null)
                {
                    var currentValueFiltered = FilterValue(currentValue);
                    return UnfilterValue(currentValue, ToBinary(valueProviderCallback(FromBinary(currentValueFiltered))));
                }
                return currentValue;
            }

            protected RegisterField(PeripheralRegister parent, int position, int width, FieldMode fieldMode, Action<T, T> readCallback,
                                    Action<T, T> writeCallback, Action<T, T> changeCallback, Func<T, T> valueProviderCallback, string name, Type enumType = null) : base(parent, position, width, fieldMode, name, enumType)
            {
                this.readCallback = readCallback;
                this.writeCallback = writeCallback;
                this.changeCallback = changeCallback;
                this.valueProviderCallback = valueProviderCallback;
            }

            protected abstract T FromBinary(uint value);

            protected abstract uint ToBinary(T value);

            private readonly Action<T, T> readCallback;
            private readonly Action<T, T> writeCallback;
            private readonly Action<T, T> changeCallback;
            private readonly Func<T, T> valueProviderCallback;
        }

        private abstract class RegisterField
        {
            public abstract void CallReadHandler(uint oldValue, uint newValue);

            public abstract void CallWriteHandler(uint oldValue, uint newValue);

            public abstract void CallChangeHandler(uint oldValue, uint newValue);

            public abstract uint CallValueProviderHandler(uint currentValue);

            public readonly int position;
            public readonly int width;
            public readonly FieldMode fieldMode;
            public readonly string name;
            public readonly Type enumType;

            public override string ToString() {
                string str = "Ofsset:" + position.ToString() + " width:" + width.ToString() + " fieldMode:" + fieldMode.GetType().Name + " access:" + fieldMode.ToString() + " name/description:" + name + "\n\r";
                if (null != enumType) {
                    str += " Enum's " + enumType.ToString() + " possible values: \r\n";
                    foreach (int value in Enum.GetValues(enumType))
                    {
                        string name = Enum.GetName(enumType, value);
                        str += " (" + value + ") " + name + "\r\n";
                    }

                }
                return str;
            }

            protected RegisterField(PeripheralRegister parent, int position, int width, FieldMode fieldMode, string name = null, Type enumType = null)
            {
                if(!fieldMode.IsValid())
                {
                    throw new ArgumentException("Invalid {0} flags for register field: {1}.".FormatWith(fieldMode.GetType().Name, fieldMode.ToString()));
                }
                this.parent = parent;
                this.position = position;
                this.fieldMode = fieldMode;
                this.width = width;
                this.name = name;
                this.enumType = enumType;
            }

            protected uint FilterValue(uint value)
            {
                return BitHelper.GetValue(value, position, width);
            }

            protected uint UnfilterValue(uint baseValue, uint fieldValue)
            {
                BitHelper.UpdateWithShifted(ref baseValue, fieldValue, position, width);
                return baseValue;
            }

            protected void WriteFiltered(uint value)
            {
                BitHelper.UpdateWithShifted(ref parent.UnderlyingValue, value, position, width);
            }

            protected readonly PeripheralRegister parent;
        }
    }
}
