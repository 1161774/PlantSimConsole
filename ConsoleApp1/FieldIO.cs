﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class FieldIO
    {
        public enum _direction { Input, Output };
        public enum _ioType { Digital, Analog };

        public string name = "";
        public string address = "";
        public _direction direction;
        public _ioType ioType;

        public FieldIO(string name, string address)
        {
            this.name = name.ToUpper();
            this.address = address;
        }
    }

    public class DigitalInput : FieldIO
    {
        private bool _value = false;
        public bool Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public DigitalInput(string name, string address) : base(name, address)
        {
            this.name = name.ToUpper();
            this.address = address;
            this.direction = _direction.Input;
            this.ioType = _ioType.Digital;
        }
    }

    public class DigitalOutput : FieldIO
    {
        private bool _isUpdated = false;
        public bool isUpdated
        {
            get { return _isUpdated; }
            set { _isUpdated = value; }
        }

        private bool _value;
        public bool Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    isUpdated = true;
                }
            }
        }

        public DigitalOutput(string name, string address) : base(name, address)
        {
            this.name = name.ToUpper();
            this.address = address;
            this.direction = _direction.Output;
            this.ioType = _ioType.Digital;
        }
    }
    public class AnalogInput : FieldIO
    {
        public double scaleLow;
        public double scaleHigh;

        private double _value = 0.0;
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public double FieldValue
        {
            set { _value = value / scaleHigh * scaleLow; }
        }

        public AnalogInput(string name, string address, string ScaleLow, string ScaleHigh) : base(name, address)
        {
            this.name = name.ToUpper();
            this.address = address;
            this.direction = _direction.Input;
            this.ioType = _ioType.Analog;
            this.scaleLow = double.Parse(ScaleLow);
            this.scaleHigh = double.Parse(ScaleHigh);
        }
    }
    public class AnalogOutput : FieldIO
    {
        private bool _isUpdated = false;
        public bool isUpdated
        {
            get { return _isUpdated; }
            set { _isUpdated = value; }
        }

        public double scaleLow;
        public double scaleHigh;


        private double _value;
        public double Value
        {
            get { return _value; }
            set {
                if (_value != value)
                {
                    _value = value;
                    isUpdated = true;
                }
            }
        }

        public double FieldValue
        {
            get { return _value * (scaleHigh - scaleLow) / scaleLow + scaleLow; }
        }

        public AnalogOutput(string name, string address, string ScaleLow, string ScaleHigh) : base(name, address)
        {
            this.name = name.ToUpper();
            this.address = address;
            this.direction = _direction.Output;
            this.ioType = _ioType.Analog;
            this.scaleLow = double.Parse(ScaleLow);
            this.scaleHigh = double.Parse(ScaleHigh);
        }
    }
}
