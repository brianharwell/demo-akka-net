using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using ConsoleApplication5.Shared;

namespace ConsoleApplication5.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var actorSystem = ActorSystem.Create("RemoteDemo"))
            {
                actorSystem.ActorOf(Props.Create<ConsoleActor>(), "ConsoleActor");

                actorSystem.WhenTerminated.Wait();
            }
        }
    }

    public class ConsoleActor : TypedActor, IHandle<Message>
    {
        public ConsoleActor()
        {
            
        }

        public void Handle(Message message)
        {
            Console.WriteLine(message.Value);
        }
    }
    
}
