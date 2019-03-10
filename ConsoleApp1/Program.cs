using System;
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

        static Dictionary<string, string> FieldInputs = new Dictionary<string, string>();

        public static readonly object FieldInputsLock = new object();


        static void Main(string[] args)
        {

            XDocument doc = XDocument.Load(".\\Plant.xml");

            var pumpInputs = doc.Root
                                .Elements("pump")
                                .Elements("input")
                                .Attributes("address")
                                .Select(x => x.Value)
                                .ToList();

            lock (FieldInputsLock)
            {
                foreach(var i in pumpInputs)
                {
                    FieldInputs.Add(i, null);
                }
                //FieldInputs.Add("TestRead4", "");
                //FieldInputs.Add("TestRead5", "");
            }

                        try
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
                                    foreach (var item in FieldInputs.Keys)
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

                                Timer t = new Timer();
                                t.Elapsed += T_Elapsed;
                                t.Interval = 1000;
                                t.AutoReset = true;
                                t.Start();

                                Console.ReadLine();
                            }
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.ReadKey();
                        }
                        

            /*            Pump PMP1001 = new Pump(
                            new List<FieldIO>
                            {
                                new FieldIO() 
                            },
                            new List<FieldIO>
                            {
                                new FieldIO(){  name = "Running",
                                                address = "E01R1S2CH0",
                                                direction = FieldIO._direction.Output,
                                                ioType = FieldIO._ioType.Digital
                                },
                                new FieldIO(){  name = "Faulted",
                                                address = "E01R1S2CH1",
                                                direction = FieldIO._direction.Output,
                                                ioType = FieldIO._ioType.Digital
                                },
                                new FieldIO(){  name = "Speed",
                                                address = "E01R1S3CH0",
                                                direction = FieldIO._direction.Output,
                                                ioType = FieldIO._ioType.Analog
                                }
                            }
                        );*/
            Console.ReadKey();
        }

        private static void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (FieldInputsLock)
            {

                foreach (var i in FieldInputs)
                {
                    Console.WriteLine(i.Key + ": " + i.Value);
                }
            }
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
