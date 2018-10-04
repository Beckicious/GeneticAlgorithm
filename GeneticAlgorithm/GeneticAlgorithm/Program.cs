using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class Program
    {
        private static GeneticAlgorithm<string> ga;
        private static char[] chars = "-_$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:.,ABCDEFGHIJKLMNOPQRSTUVWXYZ^& ".ToCharArray();
        private const string Goal = "Some random Text with a bit of OMEGALUL and Kappa123_#Partybus";
        private static int NumOfGenes = Goal.Length;

        static void Main(string[] args)
        {
            ga = new GeneticAlgorithm<string>(0.001, 10000);

            ga.InitialCreationFunction = CreateNew;
            ga.CalculateFitnessFunction = CalculateFitness;
            ga.BreedingFunction = Breed;

            ga.PopulateFirstGeneration();
            ga.CalculateFitnessForCurrentGeneration();

            //PrintCurrentGeneration();
            PrintBestAndAverageFitness();
            Console.ReadLine();

            while(!IsGoalReached())
            {
                ga.GenerateNextGeneration();
                ga.CalculateFitnessForCurrentGeneration();

                PrintBestAndAverageFitness();
                //Console.ReadLine();
            }

            Console.WriteLine($"\nGOAL REACHED IN GENERATION {ga.GenerationNumber} !!!\n");
            PrintBestAndAverageFitness();
            Console.ReadLine();
        }

        private static string CreateNew()
        {
            string s = "";

            for (int i = 0; i < NumOfGenes; i++)
            {
                s += chars[new ThreadSafeRandom().Next(chars.Length)];
            }

            return s;
        }

        private static double CalculateFitness(string DNA)
        {
            int fitness = 0;

            for (int i = 0; i < NumOfGenes; i++)
            {
                if (DNA[i] == Goal[i]) fitness++;
            }

            return Math.Pow((double)fitness / DNA.Length,2);
        }

        private static string Breed((string, double) DNA1, (string, double) DNA2, double mutationRate)
        {
            string s = "";
            double fitnessSum = DNA1.Item2 + DNA2.Item2;
            double percentageOfSum = DNA1.Item2 / fitnessSum;
            double cutPoint = (1 - mutationRate) * percentageOfSum + mutationRate;

            for (int i = 0; i < NumOfGenes; i++)
            {
                double random = new ThreadSafeRandom().NextDouble();

                if (random < mutationRate) s += chars[new ThreadSafeRandom().Next(chars.Length)];
                else if (random < cutPoint) s += DNA1.Item1[i];
                else s += DNA2.Item1[i];
            }

            return s;
        }

        private static bool IsGoalReached()
        {
            return ga.CurrentGeneration.Any(DNATupel => DNATupel.Item1 == Goal);
        }

        private static void PrintCurrentGeneration()
        {
            Console.WriteLine($"Generation : {ga.GenerationNumber}\n");

            for (int i = 0; i < ga.PopulationSize; i++)
            {
                Console.WriteLine($"{ga.CurrentGeneration[i].Item1} : {ga.CurrentGeneration[i].Item2}");
            }

            Console.WriteLine("------------------------------------------");
        }

        private static void PrintBestAndAverageFitness()
        {
            string best = ga.CurrentGeneration.OrderByDescending(t => t.Item2).First().Item1;
            double average = ga.CurrentGeneration.Average(t => t.Item2);

            Console.WriteLine($"Generation : {ga.GenerationNumber}");
            Console.WriteLine($"Best Solution = {best}");
            Console.WriteLine($"Average fitness = {Math.Round(average, 2) * 100}%");
            Console.WriteLine("------------------------------------------");
        }
    }
}
