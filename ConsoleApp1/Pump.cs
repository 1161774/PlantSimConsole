using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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


        public double rampUpTime = 10;
        public double rampDownTime = 10;

        private double flowRate;
        public double FlowRate
        {
            get { return flowRate;}
            set { flowRate = value;}
        }

        private double maxFlowRate = 1;
        public double MaxFlowRate
        {
            get { return maxFlowRate; }
            set { maxFlowRate = value; }
        }

		Stopwatch cycleTimer = new Stopwatch();


        private double currentSpeed;

        public double CurrentSpeed
        {
            get
            {
                    return currentSpeed;
            }
            set
            {
				if(value < 0.0)
				{
                    currentSpeed = 0.0;
				}
				else if(value > 1.0)
				{
					currentSpeed = 1.0;
				}
				else
				{
					currentSpeed = value;
				}

                //=1/(1+(EXP((-8*(E3-0.5)))))
                //Use a logistics 'S' curve to map the pump speed to a flow rate. constant chosen arbitrarily.
                double scaleFactor = 1 / (1 + Math.Pow(Math.E , (-8.0 * (currentSpeed - 0.5))));

                if (scaleFactor < 0.05)
                {
                    scaleFactor = 0.0;
                }
                else if(scaleFactor > 0.95)
                {
                    scaleFactor = 1.0;
                }

                FlowRate = scaleFactor * MaxFlowRate;
            }
        }


        public Pump(Hashtable inputs, Hashtable outputs)
        {

            //Link IO to addresses
            #region IO Resolution
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
            #endregion

            

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

        public void Run()
        {
            var elapsed = cycleTimer.ElapsedMilliseconds;
			cycleTimer.Restart();

            bool runCmd = DigitalInputs.First(s => s.name == "RUN").Value;

            if(runCmd)
			{
				CurrentSpeed += elapsed / rampUpTime / 1000;
			}
			else
			{
				CurrentSpeed -= elapsed / rampDownTime / 1000;
			}

            Console.WriteLine(FlowRate);

            DigitalOutputs.First(s => s.name == "RUNNING").Value = currentSpeed > 0.9;
        }
    }
}
