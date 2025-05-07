//using HirBot.Comman.Idenitity;
//using HirBot.Data.Entities;
//using Project.Repository.Repository;
//using System;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Jop.Services.Helpers
//{
//    class Job
//    {
//        public string Title { get; set; }
//        public List<string> RequiredSkills { get; set; }
//        public int MinExperience { get; set; }
//        public int MaxExperience { get; set; }
//        public string Location { get; set; }
//        public string GetJobText() => $"{Title} {string.Join(" ", RequiredSkills)} {MinExperience}-{MaxExperience} years {Location}";

//    }

//    class Candidate
//    {
//        private readonly UnitOfWork _unitOfWork;
        
//        public string Name { get; set; }
//        public string ?Bio { get; set; }

//        public List<string> Skills { get; set; }
//        public int Experience { get; set; }
//        public string Location { get; set; }
//        public string GetProfileText() => $"Candidate {Bio} {string.Join(" ", Skills)} {Experience} years {Location}";
//        Candidate(ApplicationUser user  , UnitOfWork unitOfWork)

//        {
//            _unitOfWork = unitOfWork;
//            Name = user.FullName;
//            Bio = user.Portfolio?.Title;
//            //Skills=user.Skills. 
//             var ids=user.Skills.Select(s=>s.SkillID).ToList();
//           this. Skills=_unitOfWork._context.Skills.Where(s => ids.Contains(s.ID)).Select(s=>s.Name).ToList();
//            foreach(var experience in user.experiences)
//            {
                      
//                Experience+=()
//            }
//        }
//    } 


//    class JobMatcherWithAI
//    {
//        public static double ComputeTFIDFCosineSimilarity(string jobText, string candidateText)
//        {
//            List<string> corpus = new List<string> { jobText, candidateText };

//            Dictionary<string, double[]> tfidfVectors = ComputeTFIDF(corpus);

//            return CosineSimilarity(tfidfVectors[jobText], tfidfVectors[candidateText]) * 100; // Convert to percentage
//        }

//        private static Dictionary<string, double[]> ComputeTFIDF(List<string> corpus)
//        {
//            Dictionary<string, int>[] termFrequencies = corpus
//                .Select(text => text.Split(' ')
//                    .GroupBy(word => word.ToLower())
//                    .ToDictionary(g => g.Key, g => g.Count()))
//                .ToArray();

//            Dictionary<string, int> documentFrequencies = new Dictionary<string, int>();

//            foreach (var termFreq in termFrequencies)
//                foreach (var term in termFreq.Keys)
//                    documentFrequencies[term] = documentFrequencies.GetValueOrDefault(term, 0) + 1;

//            Dictionary<string, double[]> tfidfVectors = new Dictionary<string, double[]>();

//            for (int i = 0; i < corpus.Count; i++)
//            {
//                string text = corpus[i];
//                Dictionary<string, int> termFreq = termFrequencies[i];

//                double[] tfidfVector = termFreq.Select(pair =>
//                {
//                    string term = pair.Key;
//                    double tf = pair.Value / (double)termFreq.Count;
//                    double idf = Math.Log(corpus.Count / (1.0 + documentFrequencies[term]));
//                    return tf * idf;
//                }).ToArray();

//                tfidfVectors[text] = tfidfVector;
//            }

//            return tfidfVectors;
//        }

//        private static double CosineSimilarity(double[] vectorA, double[] vectorB)
//        {
//            double dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
//            double magnitudeA = Math.Sqrt(vectorA.Sum(a => a * a));
//            double magnitudeB = Math.Sqrt(vectorB.Sum(b => b * b));

//            return magnitudeA == 0 || magnitudeB == 0 ? 0 : dotProduct / (magnitudeA * magnitudeB);
//        }
//    }
//    class JobMatcher
//    {
//        private static double CalculateMatchScore(Job job, Candidate candidate)
//        {
//            double skillMatch = job.RequiredSkills.Intersect(candidate.Skills).Count() / (double)job.RequiredSkills.Count * 100;
//            double experienceMatch = (candidate.Experience >= job.MinExperience && candidate.Experience <= job.MaxExperience) ? 100 : 0;
//            double locationMatch = job.Location.Equals(candidate.Location, StringComparison.OrdinalIgnoreCase) ? 100 : 0;


//            double totalScore = (skillMatch * 0.25) + (experienceMatch * 0.15) + (locationMatch * 0.1)
//                + (JobMatcherWithAI.ComputeTFIDFCosineSimilarity(job.GetJobText(), candidate.GetProfileText()) * .50);
//            Console.WriteLine(JobMatcherWithAI.ComputeTFIDFCosineSimilarity(job.GetJobText(), candidate.GetProfileText()));
//            return totalScore;
//        }
//        public static double GetScore(ApplicationUser user, HirBot.Data.Entities.Job job) {
//            var candidate =;

        
//        }

//        public static void RecommendJobs(List<Job> jobs, Candidate candidate)
//        {
//            foreach (var job in jobs)
//            {
//                double matchScore = CalculateMatchScore(job, candidate);
//                if (matchScore >= 75)
//                {
//                    Console.WriteLine($"Recommended Job: {job.Title} - Match Score: {matchScore}%");
//                }
//            }
//        }
//    }

//    class Program
//    {
//        static void Main()
//        {
//            List<Job> jobList = new List<Job>
//        {
//            new Job { Title = "Full Stack Developer", RequiredSkills = new List<string> { "C#", "SQL", "JavaScript" }, MinExperience = 3, MaxExperience = 6, Location = "Cairo" },
//            new Job { Title = "Backend Developer", RequiredSkills = new List<string> { "C#", "ASP.NET", "Microservices" }, MinExperience = 5, MaxExperience = 8, Location = "Giza" }
//        };

//            Candidate candidate = new Candidate
//            {
//                Name = "Mohamed Adel",
//                Bio = "Developer",
//                Skills = new List<string> { "C#", "SQL", "JavaScript", "ASP.NET" },
//                Experience = 4,
//                Location = "Cairo"
//            };

//            JobMatcher.RecommendJobs(jobList, candidate);
//        }
//    }

//}
