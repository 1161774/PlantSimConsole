using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class FlowMeter
    {
        List<AnalogOutput> AnalogOutputs = new List<AnalogOutput>()
        {
            new AnalogOutput("Rate", null, "0.0", "1.0")
        };

        List<string> Sources = new List<string>();

        double FlowRate = 0;

        public FlowMeter(List<string> Sources, Hashtable analogOutputs)
        {
            this.Sources = Sources;

            foreach (var output in AnalogOutputs)
            {
                if (analogOutputs.ContainsKey(output.name))
                {
                    output.address = ((AnalogOutput)analogOutputs[output.name]).address;
                    output.scaleLow = ((AnalogOutput)analogOutputs[output.name]).scaleLow;
                    output.scaleHigh = ((AnalogOutput)analogOutputs[output.name]).scaleHigh;
                }
            }
        }


        public void UpdateOutputs(out Hashtable FieldOutputs)
        {
            Hashtable fieldOutputs = new Hashtable();

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


        public void Run(List<Pump> pumps)
        {
            List<double> sourceRates = new List<double>();

            //initialise flow rate
            this.FlowRate = 0;
            
            foreach(var s in Sources)
            {
                sourceRates.Add(pumps.First(x => x.name == s).FlowRate);
            }

            //remove pumps with no flow from list
            sourceRates.RemoveAll(x => x == 0);

            //number of pumps pumping
            var sum = sourceRates.Count();

            if (sum == 1)
            {
                //single pump running, that's the flow rate
                this.FlowRate = sourceRates[0];
            }
            else if(sum > 1)
            {
                //multiple pumps running. Each pump has less effect on the total flow rate.
                sourceRates = sourceRates.OrderByDescending(x => x).ToList();

                foreach (var r in sourceRates)
                {
                    //Take the greatest flow rate as the primary contributor, then incrementally derate any additional pumps.
                    this.FlowRate += (r * sum-- / sourceRates.Count());
                }
            }
            else
            {
                //no pumps running, no flow
                this.FlowRate = 0.0;
            }

            AnalogOutputs.First(s => s.name == "RATE").Value = FlowRate;
        }
    }
}
