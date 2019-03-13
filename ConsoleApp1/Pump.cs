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

            foreach (var di in DigitalInputs)
            {
                if (di.address != null)
                {
                    if (FieldInputs[di.address] != null)
                    {
                        di.Value = FieldInputs[di.address].ToString().Contains("1");
                    }
                }
            }

            foreach (var ai in AnalogInputs)
            {
                if (ai.address != null)
                {
                    if (FieldInputs[ai.address] != null)
                    {
                        double.TryParse(FieldInputs[ai.address].ToString(), out double res);
                        ai.Value = res;
                    }
                }
            }
        }

        public void Run()
        {
            var runCmd = DigitalInputs.First(s => s.name == "RUN").Value;

            bool runOut = runCmd;

            DigitalOutputs.First(s => s.name == "RUNNING").Value = runOut;
        }

        public void UpdateOutputs(out Hashtable FieldOutputs)
        {
            Hashtable fieldOutputs = new Hashtable();

            foreach (var o in DigitalOutputs)
            {
                if (o.isUpdated && o.address != null)
                {
                    fieldOutputs.Add(o.address, o.Value);
                    o.isUpdated = false;
                }
            }

            foreach (var o in AnalogOutputs)
            {
                if (o.isUpdated && o.address != null)
                {
                    fieldOutputs.Add(o.address, o.Value);
                    o.isUpdated = false;
                }
            }

            FieldOutputs = fieldOutputs;
        }

        public Pump(Hashtable inputs, Hashtable outputs)
        {
            foreach (var di in DigitalInputs)
            {
                if (inputs.ContainsKey(di.name))
                {
                    di.address = inputs[di.name].ToString();
                }
            }

            foreach (var ai in AnalogInputs)
            {
                if (inputs.ContainsKey(ai.name))
                {
                    ai.address = inputs[ai.name].ToString();
                }
            }

            foreach (var _do in DigitalOutputs)
            {
                if (outputs.ContainsKey(_do.name))
                {
                    _do.address = outputs[_do.name].ToString();
                }
            }

            foreach (var ao in AnalogOutputs)
            {
                if (outputs.ContainsKey(ao.name))
                {
                    ao.address = outputs[ao.name].ToString();
                }
            }

        }
    }
}
