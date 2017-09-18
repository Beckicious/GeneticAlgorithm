using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class GeneticAlgorithm<DNA>
    {

        public double MutationRate { get; set; }
        public int PopulationSize { get; set; }
        public (DNA, double)[] CurrentGeneration { get; set; }
        public int GenerationNumber { get; set; }

        private Random rng;

        public Func<DNA> InitialCreationFunction { get; set; }
        public Func<DNA, double> CalculateFitnessFunction { get; set; }
        public Func<(DNA, double), (DNA, double), double, DNA> BreedingFunction { get; set; }

        public GeneticAlgorithm(Random random, double mutationRate = 0.01, int populationSize = 10)
        {
            this.MutationRate = mutationRate;
            this.PopulationSize = populationSize;
            this.CurrentGeneration = new (DNA, double)[this.PopulationSize];
            this.GenerationNumber = 0;

            rng = (random != null ? random : new Random());
        }

        public void PopulateFirstGeneration()
        {
            for (int index = 0; index < PopulationSize; index++)
            {
                CurrentGeneration[index] = (InitialCreationFunction(), -1);
            }
        }

        public void CalculateFitnessForCurrentGeneration()
        {
            for (int index = 0; index < PopulationSize; index++)
            {
                CurrentGeneration[index] = (CurrentGeneration[index].Item1, CalculateFitnessFunction(CurrentGeneration[index].Item1));
            }
        }

        public void GenerateNextGeneration()
        {
            (DNA, double)[] nextGeneration = new(DNA, double)[PopulationSize];

            for (int index = 0; index < PopulationSize; index++)
            {
                nextGeneration[index] = (BreedingFunction(GetRandomDNA(), GetRandomDNA(), MutationRate), -1);
            }

            CurrentGeneration = nextGeneration;
            GenerationNumber++;
        }

        private (DNA, double) GetRandomDNA()
        {
            double universalProbability = CurrentGeneration.Sum(DNATuple => DNATuple.Item2);

            double randomNumber = rng.NextDouble() * universalProbability;

            double sum = 0;
            foreach ((DNA, double) DNATuple in CurrentGeneration)
            {
                if (randomNumber <= (sum += DNATuple.Item2))
                {
                    return DNATuple;
                }
            }

            //should never be reached
            return (InitialCreationFunction(), 0);
        }
    }
}
