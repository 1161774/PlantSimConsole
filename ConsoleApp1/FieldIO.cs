using System;
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


        public string name;
        public string address;
        public _direction direction;
        public _ioType ioType;

        private double value;

        protected FieldIO(string name, string address)
        {
            this.name = name;
            this.address = address;
        }
    }

    public class DigitalInput : FieldIO
    {
        public DigitalInput(string name, string address) : base(name, address)
        {
            this.name = name;
            this.address = address;
            this.direction = _direction.Input;
            this.ioType = _ioType.Digital;
        }
    }

    public class DigitalOutput : FieldIO
    {
        public DigitalOutput(string name, string address) : base(name, address)
        {
            this.name = name;
            this.address = address;
            this.direction = _direction.Output;
            this.ioType = _ioType.Digital;
        }
    }
    public class AnalogInput : FieldIO
    {
        public AnalogInput(string name, string address) : base(name, address)
        {
            this.name = name;
            this.address = address;
            this.direction = _direction.Input;
            this.ioType = _ioType.Analog;
        }
    }
    public class AnalogOutput : FieldIO
    {
        public AnalogOutput(string name, string address) : base(name, address)
        {
            this.name = name;
            this.address = address;
            this.direction = _direction.Output;
            this.ioType = _ioType.Analog;
        }
    }
}
