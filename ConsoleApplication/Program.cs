using System;
using System.Linq;
using Akka;
using Akka.Actor;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("demo");

            var actorRef = actorSystem.ActorOf<HelloWordActor>("hello-world");

            actorRef.Tell(new HelloWord("hello from akka!"));

            Console.ReadLine();
        }

        public class HelloWord
        {
            public HelloWord(string message)
            {
                Message = message;
            }

            public string Message { get; }
        }

        public class HelloWordActor : ReceiveActor
        {
            public HelloWordActor()
            {
                Receive<HelloWord>(helloWord => Console.WriteLine(helloWord.Message));
            }
        }
    }
}
