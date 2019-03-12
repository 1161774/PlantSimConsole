using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Pump
    {
        List<DigitalInput> DigitalInputs = new List<DigitalInput>() {
            new DigitalInput("Run", null),
            new DigitalInput("Stop", null),
            new DigitalInput("Reset", null)
        };

        List<DigitalOutput> DigitalOutputs = new List<DigitalOutput>()
        {
            new DigitalOutput("Available", null),
            new DigitalOutput("Running", null),
            new DigitalOutput("Fault", null),
            new DigitalOutput("Reset", null),
            new DigitalOutput("Start", null),
            new DigitalOutput("Stop", null),
            new DigitalOutput("Auto", null),
            new DigitalOutput("Manual", null)
        };

        List<AnalogInput> AnalogInputs = new List<AnalogInput>()
        {
            new AnalogInput("RunSpeed", null)
        };

        List<AnalogOutput> AnalogOutputs = new List<AnalogOutput>()
        {
            new AnalogOutput("Speed", null)
        };


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

        private double maxFlowRate;
        public double MaxFlowRate
        {
            get { return maxFlowRate; }
            set { maxFlowRate = MaxFlowRate; }
        }


        private double currentSpeed;
        public double CurrentSpeed
        {
            get { return currentSpeed; }
        }


        public void UpdateInputs(Hashtable FieldInputs)
        {
            foreach(var di in DigitalInputs)
            {
                bool.TryParse(FieldInputs[di.address].ToString(), out bool res);
                di.Value = res;
            }

            foreach (var di in AnalogInputs)
            {
                double.TryParse(FieldInputs[di.address].ToString(), out double res);
                di.Value = res;
            }

        }

        public void Run()
        {
        }

        public void UpdateOutputs(out Dictionary<string, string> FieldOutputs)
        {
            Dictionary<string, string> fieldOutputs = new Dictionary<string, string>();




            FieldOutputs = fieldOutputs;
        }

        public Pump(Hashtable inputs, Hashtable outputs)
        {
            foreach (var di in DigitalInputs)
            {
                di.address = inputs[di.name].ToString();
            }
        }
    }
}
