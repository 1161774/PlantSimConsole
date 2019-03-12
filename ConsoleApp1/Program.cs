using System;
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

        //static Dictionary<string, string> FieldInputs = new Dictionary<string, string>();
        public static readonly object FieldInputsLock = new object();

        static Hashtable FieldInputs = new Hashtable(StringComparer.InvariantCultureIgnoreCase);

        static List<Pump> Pumps = new List<Pump>();

        static void Main(string[] args)
        {

            XDocument doc = XDocument.Load(".\\Plant.xml");


            var pumps = doc.Root
                    .Elements("pump")
                    .Select(x => x)
                    .ToList();

            var pumpInputs = doc.Root
                                .Elements("pump")
                                .Elements("input")
                                .Attributes("address")
                                .Select(x => x.Value)
                                .ToList();

            foreach(var pump in pumps)
            {

                var pumpName = pump.Name.LocalName;

                Hashtable inputs = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                var ins = pump.Elements("input").Select(x => x).ToList();
                foreach(var i in ins)
                {
                    inputs.Add(i.Attribute("name").Value, i.Attribute("address").Value);
                }

                Hashtable outputs = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                var outs = pump.Elements("output").Select(x => x).ToList();
                foreach (var i in outs)
                {
                    outputs.Add(i.Attribute("name").Value, i.Attribute("address").Value);
                }

                Pump p = new Pump(inputs, outputs);

                double maxFlowRate;
                //double.TryParse(pump.Attribute("maxFlowRate").Value, out maxFlowRate);
                //p.MaxFlowRate = maxFlowRate;



                Pumps.Add(p);

            }


            lock (FieldInputsLock)
            {
                foreach (var i in pumpInputs)
                {
                    FieldInputs.Add(i, null);
                }
            }

/*            try
            {
                using (DdeClient c = new DdeClient("RSLinx", "PlantSim"))
                {
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
                        foreach (var item in FieldInputs.Values)
                        {
                            try
                            {
                                c.StartAdvise(item, 1, true, 1000);
                            }
                            catch
                            {
                                try
                                {
                                    c.StartAdvise(item, 1, true, 1000);
                                }
                                catch
                                {
                                    //If this doesn't work, fall through to exception handler
                                    c.StartAdvise(item, 1, true, 1000);
                                }
                            }
                        }
                    }

                    Console.WriteLine(" OK");

                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadKey();
            }
            */

            Timer t = new Timer();
            t.Elapsed += T_Elapsed;
            t.Interval = 1000;
            t.AutoReset = true;
            t.Start();


            Console.ReadKey();
        }

        private static void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            Hashtable fieldInputs;

            //Get a local copy of the field inputs
            lock (FieldInputsLock)
            {
                fieldInputs = FieldInputs;
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

            //Update outputs
            //foreach (var pump in Pumps)
            //{
            //    pump.UpdateOutputs();
            //}


        }

        private static void C_Advise(object sender, DdeAdviseEventArgs e)
        {
            lock (FieldInputsLock)
            {
                FieldInputs[e.Item] = e.Text;
            }
        }

        private static void C_Disconnected(object sender, DdeDisconnectedEventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}
