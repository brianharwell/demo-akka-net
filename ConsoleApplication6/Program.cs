using System;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Routing;

namespace ConsoleApplication6
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("pooling");
            var actorRef = actorSystem.ActorOf(Props.Create<SupervisorActor>());

            actorRef.Tell(new Loop(1, 25));

            actorSystem.WhenTerminated.Wait();
        }

        public class Loop
        {
            public Loop(int low, int high)
            {
                Low = low;
                High = high;
            }

            public int Low { get; }
            public int High { get; }
        }

        public class Iteration
        {
            public Iteration(int id)
            {
                Id = id;
            }

            public int Id { get; }
        }

        public class SupervisorActor : TypedActor, IHandle<Loop>
        {
            public void Handle(Loop message)
            {
                var props = Props.Create<IterationActor>().WithRouter(new RoundRobinPool(10));
                var actorRef = Context.ActorOf(props, "looper");

                for (var iter = message.Low; iter < message.High; iter++)
                {
                    actorRef.Tell(new Iteration(iter));
                }
            }
        }

        public class IterationActor : TypedActor, IHandle<Iteration>
        {
            public void Handle(Iteration message)
            {
                Console.WriteLine($"{Context.Self.Path}: Processing iteration: {message.Id}");

                Thread.Sleep(10);
            }
        }
    }
}
