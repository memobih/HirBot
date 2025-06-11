using HirBot.Comman.Idenitity;
using HirBot.Data.Entities;
using Project.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using UglyToad.PdfPig;

namespace Jop.Services.Helpers
{
    class Job
    {
        public string Title { get; set; }
        public List<string> RequiredSkills { get; set; }
        public int MinExperience { get; set; }
        public int MaxExperience { get; set; }
        public string Description { get; set; }

        public string GetJobText() => $"{Title} {string.Join(" ", RequiredSkills)} {Description}";

        public Job(HirBot.Data.Entities.Job currentJob)
        {
            Title = currentJob.Title;
            Description = currentJob.Description;
            RequiredSkills = currentJob.JobRequirments?
                .Select(skill => skill.Skill.Name)
                .ToList() ?? new List<string>();

            if (!string.IsNullOrWhiteSpace(currentJob.Experience))
            {
                var parts = currentJob.Experience.Split('-');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int minExp) &&
                    int.TryParse(parts[1], out int maxExp))
                {
                    MinExperience = minExp;
                    MaxExperience = maxExp;
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

        public string GetProfileText() => $"{Bio ?? ""} {string.Join(" ", Skills)}";

        public void createUser(ApplicationUser user, List<string> skills)
        {
            Name = user.FullName;
            Bio = user.Portfolio?.Title ?? "";
            Skills = skills ?? new List<string>();
            Experience = 0;

            if (user.experiences != null)
            {
                foreach (var exp in user.experiences)
                {
                    if (exp.Start_Date != null && exp.End_Date != null)
                    {
                        DateTime start = exp.Start_Date.Value;
                        DateTime end = exp.End_Date ?? DateTime.Now;
                        int years = end.Year - start.Year;
                        if (end < start.AddYears(years))
                            years--;

                        Experience += Math.Max(0, years);
                    }
                }
            }
        }
    }

    static class JobMatcherWithAI
    {
        private static readonly HashSet<string> StopWords = new HashSet<string>
        {
            "a","an","the","and","or","but","is","are","was","were","be","to","of","in","that",
            "it","on","for","with","as","by","at","from","this","which","you","your","I","we",
            "they","he","she","him","her","his","their","them","my","me","our","us","not","can",
            "will","would","should","could","has","have","had","do","does","did","so","such"
            // يمكنك إضافة المزيد حسب الحاجة
        };

        public static double ComputeTFIDFCosineSimilarity(string jobText, string candidateText)
        {
            List<string> corpus = new List<string> { jobText, candidateText };
            Dictionary<string, double[]> tfidfVectors = ComputeTFIDF(corpus);
            return CosineSimilarity(tfidfVectors[jobText], tfidfVectors[candidateText]) * 100;
        }

        private static Dictionary<string, double[]> ComputeTFIDF(List<string> corpus)
        {
            // استخراج الكلمات من كل نص مع إزالة stop words
            var docsWords = corpus
                .Select(text => text
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => new string(word.Where(char.IsLetterOrDigit).ToArray()).ToLower())
                    .Where(word => !string.IsNullOrWhiteSpace(word) && !StopWords.Contains(word))
                    .ToList())
                .ToList();

            // بناء قائمة الكلمات الموحدة (vocabulary)
            var vocabulary = docsWords.SelectMany(words => words).Distinct().ToList();

            int docCount = docsWords.Count;

            // حساب Document Frequency لكل كلمة
            var documentFrequencies = new Dictionary<string, int>();
            foreach (var word in vocabulary)
            {
                int df = docsWords.Count(doc => doc.Contains(word));
                documentFrequencies[word] = df;
            }

            var tfidfVectors = new Dictionary<string, double[]>();

            for (int i = 0; i < docCount; i++)
            {
                var wordCounts = docsWords[i].GroupBy(w => w).ToDictionary(g => g.Key, g => g.Count());
                int totalWords = docsWords[i].Count;

                double[] vector = new double[vocabulary.Count];

                for (int j = 0; j < vocabulary.Count; j++)
                {
                    string term = vocabulary[j];
                    int tf = wordCounts.ContainsKey(term) ? wordCounts[term] : 0;

                    double tfValue = totalWords > 0 ? (double)tf / totalWords : 0;
                    double idfValue = Math.Log((double)docCount / (1 + documentFrequencies[term]));

                    vector[j] = tfValue * idfValue;
                }

                tfidfVectors[corpus[i]] = vector;
            }

            return tfidfVectors;
        }

        private static double CosineSimilarity(double[] vectorA, double[] vectorB)
        {
            double dot = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
            double magA = Math.Sqrt(vectorA.Sum(a => a * a));
            double magB = Math.Sqrt(vectorB.Sum(b => b * b));
            return (magA == 0 || magB == 0) ? 0 : dot / (magA * magB);
        }
    }

    public class JobMatcher
    {
        private readonly UnitOfWork _unitOfWork;

        public JobMatcher(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private static double CalculateMatchScore(Job job, Candidate candidate, bool hasCV)
        {
            if (string.IsNullOrWhiteSpace(candidate.Bio) && candidate.Skills.Count == 0)
                return 0;

            double skillMatch = job.RequiredSkills.Count != 0
                ? job.RequiredSkills.Intersect(candidate.Skills).Count() / (double)job.RequiredSkills.Count * 100
                : 1;

            double experienceMatch = (candidate.Experience >= job.MinExperience && candidate.Experience <= job.MaxExperience) ? 100 : 0;

            double aiSimilarity = hasCV
                ? JobMatcherWithAI.ComputeTFIDFCosineSimilarity(job.GetJobText(), candidate.GetProfileText())
                : 0;

            double aiWeight = hasCV ? 0.7 : 0.3;

            // زيادة وزن المهارات لرفع الدقة
            double totalScore = (skillMatch * 0.5) + (experienceMatch * 0.1) + (aiSimilarity * aiWeight);
            return totalScore;
        }

        public static int GetScore(ApplicationUser user, HirBot.Data.Entities.Job job, List<string> skills)
        {
            Candidate candidate = new Candidate();
            candidate.createUser(user, skills);

            bool hasCV = !string.IsNullOrEmpty(user.Portfolio?.CVUrl);
            if (hasCV)
            {
                string cvText = ExtractTextFromPdfUrlAsync(user.Portfolio.CVUrl).Result;
                candidate.Bio += " " + cvText;
            }

            Job jobModel = new Job(job);
            int score = (int)(CalculateMatchScore(jobModel, candidate, hasCV));
            return score;
        }

        public static async Task<string> ExtractTextFromPdfUrlAsync(string pdfUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(pdfUrl))
                {
                    response.EnsureSuccessStatusCode();

                    using (Stream pdfStream = await response.Content.ReadAsStreamAsync())
                    using (PdfDocument document = PdfDocument.Open(pdfStream))
                    {
                        StringBuilder text = new StringBuilder();
                        foreach (var page in document.GetPages())
                        {
                            text.AppendLine(page.Text);
                        }
                        return text.ToString();
                    }
                }
            }
            catch
            {
                return "";
            }
        }
    }
}
