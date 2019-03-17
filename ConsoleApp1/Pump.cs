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
            new AnalogInput("RunSpeed", null, "0.0", "1.0")
        };

        List<AnalogOutput> AnalogOutputs = new List<AnalogOutput>()
        {
            new AnalogOutput("RunningSpeed", null, "0.0", "1.0")
        };


        public double rampUpTime = 10;
        public double rampDownTime = 10;

        private bool hasRunCmd = false;
        private bool hasRunSpeedSP = false;

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

                //The curve doesn't really go completely to 0 or 1, so jump it there when it gets close.
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


        public Pump(Hashtable digitalInputs, Hashtable digitalOutputs, Hashtable analogInputs, Hashtable analogOutputs)
        {

            //Link IO to addresses
            #region IO Resolution
            foreach (var input in DigitalInputs)
            {
                if (digitalInputs.ContainsKey(input.name))
                {
                    input.address = ((DigitalInput)digitalInputs[input.name]).address;
                }
            }

            foreach (var input in AnalogInputs)
            {
                if (analogInputs.ContainsKey(input.name))
                {
                    input.address = ((AnalogInput)analogInputs[input.name]).address;
                    input.scaleLow = ((AnalogInput)analogInputs[input.name]).scaleLow;
                    input.scaleHigh = ((AnalogInput)analogInputs[input.name]).scaleHigh;
                }
            }

            foreach (var output in DigitalOutputs)
            {
                if (digitalOutputs.ContainsKey(output.name))
                {
                    output.address = ((DigitalOutput)digitalOutputs[output.name]).address;
                }
            }

            foreach (var output in AnalogOutputs)
            {
                if (analogOutputs.ContainsKey(output.name))
                {
                    output.address = ((AnalogOutput)analogOutputs[output.name]).address;
                    output.scaleLow = ((AnalogOutput)analogOutputs[output.name]).scaleLow;
                    output.scaleHigh = ((AnalogOutput)analogOutputs[output.name]).scaleHigh;
                }
            }
            #endregion

            //Determine what the pump can actually do
            #region Function Resolution

            hasRunCmd = DigitalInputs.First(s => s.name == "RUN").address != null;
            hasRunSpeedSP = AnalogInputs.First(s => s.name == "RUNSPEED").address != null;

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
                        ai.FieldValue = res;
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
                    fieldOutputs.Add(o.address, o.FieldValue);
                    o.isUpdated = false;
                }
            }

            FieldOutputs = fieldOutputs;
        }

        public void Run()
        {
            var elapsed = cycleTimer.ElapsedMilliseconds;
			cycleTimer.Restart();

            bool runCmd;
            double runSpeedSP;


            if(hasRunCmd && hasRunSpeedSP)
            {
                runCmd = DigitalInputs.First(s => s.name == "RUN").Value;
                runSpeedSP = AnalogInputs.First(s => s.name == "RUNSPEED").Value;
            }
            else if(hasRunCmd)
            {
                runCmd = DigitalInputs.First(s => s.name == "RUN").Value;
                runSpeedSP = 1.0;
            }
            else if(hasRunCmd)
            {
                runSpeedSP = AnalogInputs.First(s => s.name == "RUNSPEED").Value;
                runCmd = runSpeedSP > 0.0;
            }
            else
            {
                runCmd = false;
                runSpeedSP = 0.0;
            }

            var inc = elapsed / rampUpTime / 1000;
            var dec = elapsed / rampDownTime / 1000;

            if (runCmd)
			{
                if (CurrentSpeed + inc < runSpeedSP) CurrentSpeed += inc;
                else if (CurrentSpeed - dec > runSpeedSP) CurrentSpeed -= dec;
                else
                {
                    currentSpeed = runSpeedSP;
                }
			}
			else
			{
				CurrentSpeed -= dec;
			}




            DigitalOutputs.First(s => s.name == "RUNNING").Value = currentSpeed > 0;
            DigitalOutputs.First(s => s.name == "AVAILABLE").Value = true;
            DigitalOutputs.First(s => s.name == "FAULT").Value = false;
            DigitalOutputs.First(s => s.name == "RESET").Value = false;
            DigitalOutputs.First(s => s.name == "AUTO").Value = true;
            DigitalOutputs.First(s => s.name == "MANUAL").Value = false;

            AnalogOutputs.First(s => s.name == "RUNNINGSPEED").Value = CurrentSpeed;


        }
    }
}
