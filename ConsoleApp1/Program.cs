using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDde.Client;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            DdeClient dde = new DdeClient("RSLinx", "PlantSim");

            Console.Write("Connecting...");
            dde.TryConnect();

            if (dde.IsConnected)
            {
                Console.WriteLine("Done");
                Console.WriteLine("Start loop:");


                dde.Advise += Dde_Advise;

                while (true)
                {
                    try
                    {

                        var res = dde.Request("TestRead", 1000);
                        Console.WriteLine(res.ToString());
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Read Failed:");
                        Console.WriteLine(ex.ToString());
                    }

                }
            }
            else
            {
                Console.WriteLine("Fail");
            }

            Console.ReadKey();

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
        }

        private static void Dde_Advise(object sender, DdeAdviseEventArgs e)
        {

            throw new NotImplementedException();
        }
    }
}
