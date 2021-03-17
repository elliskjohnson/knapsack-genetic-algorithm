using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnapsackGA
{
    public static class GenericToStringExt
    {
        public static string ToStringExt<T>(this List<T> list) => "[" + string.Join(", ", list) + "]";
    }

    public class Knapsack
    {
        #region Private Properties
        private static List<int> _items;
        private static List<int> _weights;
        private static List<int> _values;
        private static Random random;

        private List<string> _chromosomes;
        private List<string> _children;
        private List<int> _chromosomeFitness;
        private List<int> _chromosomeWeights;
        private int _generation;
        private int _populationSize;
        private int _maxWeight;
        private int _maxItems;
        private double _mutationProbability;
        private int _fitnessSum;
        private int _maxFitness;
        private int _maxFitnessUpdateCount;
        private int _generationWorseCount;
        private int _numMutations;
        private int _genRun;
        #endregion

        #region Constructors
        static Knapsack()
        {
            int[] weightvals = { 6, 14, 13, 9, 11, 16, 20, 17, 3, 5 };
            Weights = new List<int>(weightvals);
            int[] valvals = { 18, 60, 47, 55, 53, 72, 90, 83, 21, 16 };
            Values = new List<int>(valvals);
            random = new Random();
        }
        public Knapsack(int populationSize = 4, int maxWeight = 35, int maxItems = 10, double mutationProbability = 0.05, int genRun = 0)
        {
            Chromosomes = new List<string>();
            ChromosomeFitness = new List<int>();
            Children = new List<string>();
            _chromosomeWeights = new List<int>();
            PopulationSize = populationSize;
            MaxWeight = maxWeight;
            MaxItems = maxItems;
            MutationProbability = mutationProbability;
            _generation = 0;
            FitnessSum = 0;
            MaxFitness = 0;
            MaxFitnessUpdateCount = 0;
            GenerationNoChangeCount = 0;
            GenRun = genRun;
        }
        #endregion

        #region Public Accessors
        public int Generation { get => _generation; set => _generation = value; }
        public int PopulationSize { get => _populationSize; set => _populationSize = value; }
        public int MaxWeight { get => _maxWeight; set => _maxWeight = value; }
        public int MaxItems { get => _maxItems; set => _maxItems = value; }
        public double MutationProbability { get => _mutationProbability; set => _mutationProbability = value; }
        public static List<int> Weights { get => _weights; set => _weights = value; }
        public static List<int> Values { get => _values; set => _values = value; }
        public List<string> Chromosomes { get => _chromosomes; set => _chromosomes = value; }
        public List<int> ChromosomeFitness { get => _chromosomeFitness; set => _chromosomeFitness = value; }
        public int FitnessSum { get => _fitnessSum; set => _fitnessSum = value; }
        public List<string> Children { get => _children; set => _children = value; }
        public int MaxFitness { get => _maxFitness; set => _maxFitness = value; }
        public int MaxFitnessUpdateCount { get => _maxFitnessUpdateCount; set => _maxFitnessUpdateCount = value; }
        public int GenerationWorseCount { get => _generationWorseCount; set => _generationWorseCount = value; }
        public int NumMutations { get => _numMutations; set => _numMutations = value; }
        public int GenerationNoChangeCount { get; private set; }
        public static List<int> Items { get => _items; set => _items = value; }
        public int GenRun { get => _genRun; set => _genRun = value; }
        #endregion

        #region General Use Optimization Methods
        public void Optimize(int numGenerations, bool printEachGeneration = false)
        {
            InitializePopulation();
            for (int i = 0; i < numGenerations; i++)
            {
                RunSingleGeneration();
                if (printEachGeneration)
                {
                    DebugGenerationPrint();
                }
            }
            if (!printEachGeneration)
            {
                DebugGenerationPrint();
            }
        }

        public void Optimize()
        {
            InitializePopulation();
            while (GenerationNoChangeCount < (4 * PopulationSize))
            {
                RunSingleGeneration();
                DebugGenerationPrint();
            }
        }

        public void RunSingleGeneration()
        {
            Crossover();
            MutateChildren();
            UpdatePopulationWithChildren();
        }

        public void RunSingleGenerationOutput()
        {
            Crossover();
            MutateChildren();
            UpdatePopulationWithChildren();
            DebugGenerationPrint();
        }
        #endregion

        #region Debugging Print Function
        public void DebugGenerationPrint()
        {
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Generation: " + _generation.ToString());
            Console.WriteLine(Chromosomes.ToStringExt());
            Console.WriteLine(ChromosomeFitness.ToStringExt());
        }
        #endregion

        #region Initialize Population Functions
        public void InitializePopulation()
        {
            string addedMember = "";
            for (int i = 0; i < PopulationSize; i++)
            {
                int curFitness = 0;
                while (curFitness == 0)
                {
                    // Randomly Generate a Weights.Count length string of 0s and 1s
                    addedMember = GenerateRouletteString(Weights.Count);
                    // Check Fitness, if > 0 add to chromosomes
                    curFitness = GetFitness(addedMember);
                }

                // Now that we know fitness > 0, add string and fitness value to tracking arrays
                AddChromosomeMemberWithFitness(addedMember, curFitness);
            }
        }

        #region Initialize Population - Private Helper Functions
        private string GenerateRouletteString(int size)
        {
            var builder = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                // Generate random double between 0, 1
                double p = random.NextDouble();
                // if random double > 0.5, append '1' else append '0'
                char x = p > 0.5 ? '1' : '0';
                builder.Append(x);
            }
            return builder.ToString();
        }

        private int GetFitness(string s)
        {
            int weight = 0;
            int fitness = 0;
            int i = 0;
            foreach (char x in s)
            {
                if (x == '1')
                {
                    weight += Weights[i];
                    fitness += Values[i];
                }
                if (weight > MaxWeight) { return 0; }
                i++;
            }
            return fitness;
        }


        private void AddChromosomeMemberWithFitness(string addedMember, int curFitness)
        {
            Chromosomes.Add(addedMember);
            ChromosomeFitness.Add(curFitness);
            FitnessSum += curFitness;
        }
        #endregion

        #endregion

        #region Fitness Evaluation Methods
        public void EvaluateFitness()
        {
            int i = 0;
            int fitSum = 0;
            foreach (string s in Chromosomes)
            {
                int curFitness = GetFitness(s);
                UpdateFitnessTracking(curFitness);
                ChromosomeFitness[i] = curFitness;
                fitSum += curFitness;
                i++;
            }
            FitnessSum = fitSum;
        }

        private void UpdateFitnessTracking(int curFitness)
        {
            if (curFitness > MaxFitness)
            {
                GenerationNoChangeCount = 0;
                MaxFitness = curFitness;
                MaxFitnessUpdateCount++;
            }
            else if (curFitness == MaxFitness) { GenerationNoChangeCount++; }
            else { GenerationWorseCount++; }
        }
        #endregion

        #region Selection Methods
        public int[] Selection()
        {
            // Select Each Max in Order
            int[] selected_indices = GetNFittestParents(PopulationSize);
            return selected_indices;
        }

        private int[] GetNFittestParents(int n)
        {
            int[] fitnessCopy = ChromosomeFitness.ToArray();
            int[] selectedIndex = new int[n];
            for (int i = 0; i < n; i++)
            {
                int maxIndex = Array.IndexOf(fitnessCopy, fitnessCopy.Max());
                selectedIndex[i] = maxIndex;
                fitnessCopy[maxIndex] = -1;
            }
            return selectedIndex;
        }
        #endregion

        #region Crossover Methods
        public void Crossover()
        {
            Children = CrossoverAllParents(Weights.Count, Selection());
        }

        // TODO: Fix to work with different population size
        // note pop size must be even!
        public List<string> CrossoverAllParents(int size, int[] parentIndexArray)
        {
            int randomPos;
            List<string> children = new List<string>(PopulationSize);

            randomPos = random.Next(0, Weights.Count - 1);
            string p1 = Chromosomes[parentIndexArray[0]];
            string p2 = Chromosomes[parentIndexArray[1]];
            children.AddRange(CrossoverTwoParents(size, p1, p2, randomPos));

            randomPos = random.Next(0, Weights.Count - 1);
            p1 = Chromosomes[parentIndexArray[0]];
            p2 = Chromosomes[parentIndexArray[2]];
            children.AddRange(CrossoverTwoParents(size, p1, p2, randomPos));

            return children;
        }

        private string[] CrossoverTwoParents(int size, string p1, string p2, int crossover_point)
        {
            string leftP1, rightP1, leftP2, rightP2;
            StringBuilder child1 = new StringBuilder(size);
            StringBuilder child2 = new StringBuilder(size);
            int cpoint = crossover_point + 1;

            // Parent 1 Substrings:
            leftP1 = p1.Substring(0, cpoint);
            rightP1 = p1.Substring(cpoint);

            // Parent 2 Substrings:
            leftP2 = p2.Substring(0, cpoint);
            rightP2 = p2.Substring(cpoint);

            // Create Children:
            // Create child 1 = leftP1, rightP2
            child1.Append(leftP1+ rightP2);

            // Create child 2 = leftP2, rightP1
            child2.Append(leftP2 + rightP1);

            return new string[] { child1.ToString(), child2.ToString() };
        }
        #endregion

        #region Update Chromosome Methods
        public void UpdatePopulationWithChildren()
        {
            string[] newChromosomes = new string[PopulationSize];
            Chromosomes.Clear();
            Children.CopyTo(newChromosomes);
            Chromosomes = new List<string>(newChromosomes);
            EvaluateFitness();
        }
        #endregion

        #region Mutation Methods
        public void MutateChildren()
        {
            Children = MutateAllChildren(PopulationSize, Children);
            Generation += 1;
        }

        private List<string> MutateAllChildren(int size, List<string> children)
        {
            List<string> mutatedChildren = new List<string>(size);
            foreach (string s in children)
            {
                mutatedChildren.Add(MutateGiven(s));
            }
            return mutatedChildren;

        }

        private string MutateGiven(string s)
        {
            StringBuilder build = new StringBuilder(s.Length);
            // Given string s:
            // For each character x in s:
            //  Generate random double 0->1 if > mutation prob, flip bit
            double mutationRand;
            char toAppend;
            foreach (char x in s)
            {
                mutationRand = random.NextDouble();
                toAppend = x;
                if (mutationRand <= MutationProbability)
                {
                    NumMutations++;
                    bool condition = x == '1';
                    toAppend = condition ? '0' : '1';
                }
                build.Append(toAppend);
            }
            return build.ToString();
        }
        #endregion
    }
}
