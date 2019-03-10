using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Pump
    {

        FieldIO inputRun;
        FieldIO inputFaultReset;

        FieldIO outputRunning;
        FieldIO outputFaulted;


        private string state;
        public string State
        {
            get { return state; }
        }

        private double flowRate;
        public double FlowRate
        {
            get { return flowRate;}
            set { flowRate = FlowRate;}
        }

        private double currentSpeed;
        public double CurrentSpeed
        {
            get { return currentSpeed; }
        }


        public Pump(List<FieldIO> inputs, List<FieldIO> outputs)
        {
            
        }


    }
}
