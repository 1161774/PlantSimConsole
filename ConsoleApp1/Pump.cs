using System;
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


        public void UpdateInputs(Dictionary<string, string> FieldInputs)
        {
            foreach(var i in DigitalInputs)
            {
                bool res;

                res = false;
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

        public Pump(List<KeyValuePair<string, string>> inputs, List<KeyValuePair<string, string>> outputs)
        {
            foreach (var di in DigitalInputs)
            {
                bool r = false;

                inputs

            }



            /*
            if(i.Key.ToUpper() == "RUN")
            {
                inputRun.address = i.Value;
            }
            else if (i.Key.ToUpper() == "STOP")
            {
                inputStop.address = i.Value;
            }
            else if(i.Key.ToUpper() == "RESET")
            {
                inputReset.address = i.Value;
            }
        }

        foreach (var i in outputs)
        {
            if (i.Key.ToUpper() == "AVAILABLE")
            {
                outputAvailable.address = i.Value;
            }
            else if (i.Key.ToUpper() == "RUNNING")
            {
                outputRunning.address = i.Value;
            }
            else if (i.Key.ToUpper() == "FAULT")
            {
                outputFaulted.address = i.Value;
            }
            else if (i.Key.ToUpper() == "RESET")
            {
                outputReset.address = i.Value;
            }
            else if (i.Key.ToUpper() == "START")
            {
                outputStart.address = i.Value;
            }
            else if (i.Key.ToUpper() == "STOP")
            {
                outputStop.address = i.Value;
            }
            else if (i.Key.ToUpper() == "AUTO")
            {
                outputAuto.address = i.Value;
            }
            else if (i.Key.ToUpper() == "MANUAL")
            {
                outputManual.address = i.Value;
            }*/

        }
    }
}
