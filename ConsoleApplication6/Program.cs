using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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

            actorRef.Tell(new Loop(1, 100));

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
                var collectionActor = Context.ActorOf(Props.Create<CollectionActor>(message));
                
                var props = Props.Create<IterationActor>(collectionActor).WithRouter(new RoundRobinPool(10));
                var looperActor = Context.ActorOf(props, "looper");

                for (var iter = message.Low; iter <= message.High; iter++)
                {
                    looperActor.Tell(new Iteration(iter), Self);
                }
            }
        }

        public class IterationActor : TypedActor, IHandle<Iteration>
        {
            private readonly IActorRef _collectionActor;

            public IterationActor(IActorRef collectionActor)
            {
                _collectionActor = collectionActor;
            }

            protected override void PreRestart(Exception reason, object message)
            {
                base.PreRestart(reason, message);

                if (message == null)
                {
                    return;
                }

                Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(1), Self, message, Self);
            }

            public void Handle(Iteration message)
            {
                if (new Random().Next(1, 10) == 3) //message.Id > 45 && message.Id < 50)
                {
                    Console.WriteLine($"{Context.Self.Path}: Failing iteration: {message.Id}");

                    //Thread.Sleep(1000);

                    throw new ApplicationException(message.Id.ToString());
                }

                Console.WriteLine($"{Context.Self.Path}: Processing iteration: {message.Id}");

                //if (new Random().Next(1, 5) == 3)
                //{
                //    Console.WriteLine($"{Context.Self.Path}: Failing iteration: {message.Id}");

                //    throw new ApplicationException(message.Id.ToString());
                //}
                //else
                //{
                //    Console.WriteLine($"{Context.Self.Path}: Processing iteration: {message.Id}");
                //}


                Thread.Sleep(10);

                _collectionActor.Tell(message);
            }
        }

        public class CollectionActor : TypedActor, IHandle<Iteration>
        {
            private readonly ConcurrentBag<int> _list = new ConcurrentBag<int>();
            private readonly Loop _loop;
            private Stopwatch _stopwatch;

            public CollectionActor(Loop loop)
            {
                _loop = loop;
            }

            protected override void PreStart()
            {
                _stopwatch = Stopwatch.StartNew();

                base.PreStart();
            }

            public void Handle(Iteration message)
            {
                _list.Add(message.Id);

                Console.WriteLine($"Added iteration: {message.Id}");

                if (_list.Count == _loop.High)
                {
                    Console.WriteLine($"Total duration: {_stopwatch.ElapsedMilliseconds}");
                }
            }
        }
    }
}
