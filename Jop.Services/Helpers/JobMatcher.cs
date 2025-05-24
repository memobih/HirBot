using HirBot.Comman.Idenitity;
using HirBot.Data.Entities;
using Project.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace Jop.Services.Helpers
{
    class Job
    {
        public string Title { get; set; }
        public List<string> RequiredSkills { get; set; }
        public int MinExperience { get; set; }
        public int MaxExperience { get; set; }
        public string GetJobText() => $"{Title} {string.Join(" ", RequiredSkills)}  "; 

        public Job(HirBot.Data.Entities.Job currentJob)
        {
            this.Title= currentJob.Title;
            this.RequiredSkills = new List<string>();
            if(currentJob.JobRequirments!=null)
            foreach(var skill in currentJob.JobRequirments )
            {
                    this.RequiredSkills.Add(skill.Skill.Name);
            }

            if (currentJob.Experience != null)
            {
                string[] parts = currentJob.Experience.Split('-'); // تقسيم السلسلة على حسب "-"

                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int minExp) &&
                    int.TryParse(parts[1], out int maxExp))
                {
                    this.MinExperience = minExp;
                    this.MaxExperience = maxExp;

                }
            }
        }
    }

    class Candidate
    {

        public string Name { get; set; }
        public string? Bio { get; set; }

        public List<string> Skills { get; set; }
        public int Experience { get; set; }
        public string GetProfileText() => $"{Bio??""} {string.Join(" ", Skills)} ";
       
        public void createUser(ApplicationUser user, List<string> skills)

        {
            Name = user.FullName;
            Bio = user.Portfolio?.Title;
        
            Skills = skills;
            
            if (user.experiences != null)
                foreach (var experience in user.experiences)
                {

                    if (experience.Start_Date != null && experience.End_Date != null)
                    {
                        DateTime start = experience.Start_Date.Value;
                        if (experience.End_Date == null)
                            experience.End_Date = DateTime.Now;

                        DateTime end = experience.End_Date.Value;
                        int years = end.Year - start.Year;

                        if (end < start.AddYears(years))
                        {
                            years--;
                        }
                    }


                }
        }
    }


    static class JobMatcherWithAI
    {
        public static double ComputeTFIDFCosineSimilarity(string jobText, string candidateText)
        {
            List<string> corpus = new List<string> { jobText, candidateText };

            Dictionary<string, double[]> tfidfVectors = ComputeTFIDF(corpus);

            return CosineSimilarity(tfidfVectors[jobText], tfidfVectors[candidateText]) * 100; // Convert to percentage
        }

        private static Dictionary<string, double[]> ComputeTFIDF(List<string> corpus)
        {
            Dictionary<string, int>[] termFrequencies = corpus
    .Select(text => text
        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Select(word => new string(word
            .Where(char.IsLetterOrDigit)
            .ToArray()).ToLower())
        .Where(word => !string.IsNullOrWhiteSpace(word))
        .GroupBy(word => word)
        .ToDictionary(g => g.Key, g => g.Count())
    ).ToArray();
            Dictionary<string, int> documentFrequencies = new Dictionary<string, int>();

            foreach (var termFreq in termFrequencies)
                foreach (var term in termFreq.Keys)
                    documentFrequencies[term] = documentFrequencies.GetValueOrDefault(term, 0) + 1;

            Dictionary<string, double[]> tfidfVectors = new Dictionary<string, double[]>();

            for (int i = 0; i < corpus.Count; i++)
            {
                string text = corpus[i];
                Dictionary<string, int> termFreq = termFrequencies[i];

                double[] tfidfVector = termFreq.Select(pair =>
                {
                    string term = pair.Key;
                    double tf = pair.Value / (double)termFreq.Count;
                    double idf = Math.Log(corpus.Count / (1.0 + documentFrequencies[term]));
                    return tf * idf;
                }).ToArray();

                tfidfVectors[text] = tfidfVector;
            }

            return tfidfVectors;
        }

        private static double CosineSimilarity(double[] vectorA, double[] vectorB)
        {
            double dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
            double magnitudeA = Math.Sqrt(vectorA.Sum(a => a * a));
            double magnitudeB = Math.Sqrt(vectorB.Sum(b => b * b));

            return magnitudeA == 0 || magnitudeB == 0 ? 0 : dotProduct / (magnitudeA * magnitudeB);
        }
    }
    public class JobMatcher

    {
        private readonly UnitOfWork _unitOfWork;
      
        public JobMatcher(UnitOfWork _unitOfWork) {
        this._unitOfWork= _unitOfWork;
        }
        private  static double CalculateMatchScore(Job job, Candidate candidate)
        {

            double skillMatch = 1;
            if(job.RequiredSkills.Count!=0)
            skillMatch=job.RequiredSkills.Intersect(candidate.Skills).Count() / (double)job.RequiredSkills.Count * 100;
            double experienceMatch = (candidate.Experience >= job.MinExperience && candidate.Experience <= job.MaxExperience) ? 100 : 0;

            double totalScore = (skillMatch * 0.26) + (experienceMatch * 0.15)
                + (JobMatcherWithAI.ComputeTFIDFCosineSimilarity(job.GetJobText(), candidate.GetProfileText()) * .50);
           
            return totalScore;
        }
        public static int GetScore(ApplicationUser user, HirBot.Data.Entities.Job job , List<string > skills)
        {
            Candidate candidate =new  Candidate();
            candidate.createUser(user , skills);
            var Job = new Jop.Services.Helpers.Job(job); 
            int  score=(int)(CalculateMatchScore(Job, candidate));
            return score;
        }

     
    }
}
