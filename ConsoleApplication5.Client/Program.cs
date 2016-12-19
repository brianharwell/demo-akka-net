using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using ConsoleApplication5.Shared;

namespace ConsoleApplication5.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var actorSystem = ActorSystem.Create("Demo"))
            {
                var actorSelection = actorSystem.ActorSelection("akka.tcp://RemoteDemo@localhost:8081/user/ConsoleActor");

                actorSelection.Tell(new Message("Hello"));
                //actorSelection.Tell("Hello");

                Console.ReadLine();

                //actorSystem.Stop(actorSelection.Anchor);

                actorSystem.Terminate();
            }
        }
    }
}
