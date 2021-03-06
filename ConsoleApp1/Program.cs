﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDde.Client;
using System.Timers;
using System.Xml.Linq;

namespace ConsoleApp1
{
    class Program
    {

        static DdeClient c;

        public static readonly object FieldInputsLock = new object();

        static Hashtable _FieldInputs = new Hashtable(StringComparer.InvariantCultureIgnoreCase);

        static List<Pump> Pumps = new List<Pump>();
        static List<FlowMeter> FlowMeters = new List<FlowMeter>();

        static void Main(string[] args)
        {

            XDocument doc = XDocument.Load(".\\Plant.xml");


            var pumps = doc.Root
                    .Elements("drives")
                    .Elements("pump")
                    .Select(x => x)
                    .ToList();

            var pumpInputs = doc.Root
                                .Elements("drives")
                                .Elements("pump")
                                .Elements("input")
                                .Attributes("address")
                                .Select(x => x.Value)
                                .ToList();

            var flowMeters = doc.Root
                                .Elements("sensors")
                                .Elements("flowMeter")
                                .Select(x => x)
                                .ToList();

            foreach(var pump in pumps)
            {

                Hashtable DigitalInputs = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                var digitalInputs = pump.Elements("digitalInput").Select(x => x).ToList();
                foreach(var i in digitalInputs)
                {
                    var input = new DigitalInput(i.Attribute("name").Value.ToString(), 
                                                 i.Attribute("address").Value.ToString());
                    DigitalInputs.Add(i.Attribute("name").Value, (object)input);
                }

                Hashtable DigitalOutputs = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                var digitalOutputs = pump.Elements("digitalOutput").Select(x => x).ToList();
                foreach (var i in digitalOutputs)
                {
                    var output = new DigitalOutput(i.Attribute("name").Value.ToString(), 
                                                   i.Attribute("address").Value.ToString());
                    DigitalOutputs.Add(i.Attribute("name").Value, (object)output);
                }

                Hashtable AnalogInputs = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                var analogInputs = pump.Elements("analogInput").Select(x => x).ToList();
                foreach (var i in analogInputs)
                {
                    var input = new AnalogInput(i.Attribute("name").Value.ToString(), 
                                                i.Attribute("address").Value.ToString(), 
                                                i.Attribute("scaleLow").Value.ToString(), 
                                                i.Attribute("scaleHigh").Value.ToString());
                    AnalogInputs.Add(i.Attribute("name").Value, (object)input);
                }

                Hashtable AnalogOutputs = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                var analogOutputs = pump.Elements("analogOutput").Select(x => x).ToList();
                foreach (var i in analogOutputs)
                {
                    var output = new AnalogOutput(i.Attribute("name").Value.ToString(), 
                                                  i.Attribute("address").Value.ToString(), 
                                                  i.Attribute("scaleLow").Value.ToString(), 
                                                  i.Attribute("scaleHigh").Value.ToString());
                    AnalogOutputs.Add(i.Attribute("name").Value, (object)output);
                }


                Pump p = new Pump(pump.Attribute("name").Value, DigitalInputs, DigitalOutputs, AnalogInputs, AnalogOutputs);


                double maxFlowRate;
                double.TryParse(pump.Attribute("maxFlowRate").Value, out maxFlowRate);
                p.MaxFlowRate = maxFlowRate;

                double rampUpTime;
                double.TryParse(pump.Attribute("rampUpTime").Value, out rampUpTime);
                p.rampUpTime = rampUpTime;

                double rampDownTime;
                double.TryParse(pump.Attribute("rampDownTime").Value, out rampDownTime);
                p.rampDownTime = rampDownTime;

                Pumps.Add(p);

            }

            foreach(var flowMeter in flowMeters)
            {
                var sources = flowMeter.Elements("source").Attributes("name").Select(x => x.Value).ToList();

                FlowMeter f = new FlowMeter(sources, new Hashtable());

                FlowMeters.Add(f);
            }


            lock (FieldInputsLock)
            {
                foreach (var i in pumpInputs)
                {
                    _FieldInputs.Add(i, null);
                }
            }
            


/*            try
            {
                c = new DdeClient("RSLinx", "PlantSim");

                c.Disconnected += C_Disconnected;

                Console.Write("Connecting...");
                c.Connect();
                Console.WriteLine(" OK");

                Console.Write("Starting advisor...");
                c.Advise += C_Advise;
                Console.WriteLine(" OK");

                Console.Write("Adding vars...");

                // RSLinx seems to reject the first register request. Try it 3 times, if it still doesn't work then an issue.
                lock (FieldInputsLock)
                {
                    foreach (var item in _FieldInputs.Keys)
                    {
                        if (item != null)
                        {
                            try
                            {
                                c.StartAdvise(item.ToString(), 1, true, 1000);
                            }
                            catch
                            {
                                try
                                {
                                    c.StartAdvise(item.ToString(), 1, true, 1000);
                                }
                                catch
                                {
                                    //If this doesn't work, fall through to exception handler
                                    c.StartAdvise(item.ToString(), 1, true, 1000);
                                }
                            }
                        }
                    }
                }

                Console.WriteLine(" OK");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadKey();
            }
            */

            Timer t = new Timer();
            t.Elapsed += T_Elapsed;
            t.Interval = 100;
            t.AutoReset = true;
            t.Start();

            Console.WriteLine("Press any key to close application");
            Console.ReadKey();

            Console.Write("Stopping Timer...");
            t.Stop();
            t.Elapsed -= T_Elapsed;
            Console.WriteLine(" OK");

            Console.Write("Closing DDE Link...");
            c.Dispose();

//            c.Disconnect();
            Console.WriteLine(" OK");
        }

        private static void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            Hashtable fieldInputs;

            //Get a local copy of the field inputs
            lock (FieldInputsLock)
            {
                fieldInputs = _FieldInputs;
            }

            //Update inputs
            foreach (var pump in Pumps)
            {
                pump.UpdateInputs(fieldInputs);
            }

            //Run the pump logic
            foreach (var pump in Pumps)
            {
                pump.Run();
            }

            foreach (var flowMeter in FlowMeters)
            {
                flowMeter.Run(Pumps);
            }


            Hashtable fieldOutputs = new Hashtable();

            //Update outputs
            foreach (var pump in Pumps)
            {
                pump.UpdateOutputs(out Hashtable outputs);

                foreach (DictionaryEntry o in outputs)
                {
                    if (!fieldOutputs.ContainsKey(o.Key))
                    {
                        fieldOutputs.Add(o.Key, o.Value);
                    }
                }
            }

            foreach (var flowMeter in FlowMeters)
            {
                flowMeter.UpdateOutputs(out Hashtable outputs);

                foreach (DictionaryEntry o in outputs)
                {
                    if (!fieldOutputs.ContainsKey(o.Key))
                    {
                        fieldOutputs.Add(o.Key, o.Value);
                    }
                }
            }


            if (c != null && c.IsConnected)
            {
                foreach (DictionaryEntry o in fieldOutputs)
                {
                    if (o.Key != null && o.Value != null)
                    {
                        Console.WriteLine("Poke: " + o.Key.ToString() + ": " + o.Value.ToString());
                        string res = o.Value.ToString() == "True" ? "1" : "0";
                        c.Poke(o.Key.ToString(), res, 1000);
                    }
                }
            }

        }

        private static void C_Advise(object sender, DdeAdviseEventArgs e)
        {
            lock (FieldInputsLock)
            {
                Console.WriteLine("Advise: " + e.Item + ": " + e.Text);
                _FieldInputs[e.Item] = e.Text;
            }
        }

        private static void C_Disconnected(object sender, DdeDisconnectedEventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}
