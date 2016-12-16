using System;
using System.Linq;
using Akka;
using Akka.Actor;
using Akka.Event;

namespace ConsoleApplication2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("demo");

            var actorRef = actorSystem.ActorOf<CalculatorActor>("calculator");

            var result = actorRef.Ask(new Calculator(4, 5)).GetAwaiter().GetResult();

            Console.WriteLine($"Sum is: {result}");

            Console.ReadLine();
        }

        public class Calculator
        {
            public Calculator(int number1, int number2)
            {
                Number1 = number1;
                Number2 = number2;
            }

            public int Number1 { get; }
            public int Number2 { get; }

            public int Sum()
            {
                return Number1 + Number2;
            }
        }

        public class CalculatorActor : ReceiveActor
        {
            public CalculatorActor()
            {
                Receive<Calculator>(calculator =>
                    {
                        Sender.Tell(calculator.Sum());
                    });
            }
        }
    }
}
