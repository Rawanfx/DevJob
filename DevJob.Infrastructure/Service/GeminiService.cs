using DevJob.Application.DTOs.MockInterview;
using DevJob.Application.ServiceContract;
using Microsoft.Extensions.Configuration;
using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Types;
using OpenAI;
using OpenAI.Chat;
using Serilog;
using System.ClientModel;
using System.Text.Json;

namespace DevJob.Infrastructure.Service
{
    public class GeminiService:IGeminiService
    {
        private readonly IConfiguration configuration;

        private readonly ChatClient chat;
        public GeminiService (IConfiguration configuration)
        {
            this.configuration = configuration;
            string apiKey = configuration["Groq:ApiKey"];
            var groqOptions = new OpenAIClientOptions
            {
                Endpoint = new Uri("https://api.groq.com/openai/v1")
            };
            var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey), groqOptions);
            chat = openAiClient.GetChatClient( model: "llama-3.3-70b-versatile"
           );
        }

        public async Task<AnswerEvaluation> EvaluateAnswer(string question, string answer, string jobTitle, List<string> skills)
        {
            var skillsList = string.Join(", ", skills);

            if (string.IsNullOrWhiteSpace(answer) || answer.Length < 3)
            {
                return new AnswerEvaluation
                {
                    Score = 0,
                    Feedback = "No clear answer was detected. Please make sure to speak clearly into the microphone.",
                    Correct_Points = "",
                    Missing_Points = "A complete verbal answer to the question.",
                    Suggested_Answer = ""
                };
            }

            var prompt = $@"
You are evaluating a technical interview answer.

Job Title: {jobTitle}
Required Skills: {skillsList}
Question: {question}
Candidate Answer (transcribed from speech, may contain minor 
transcription errors or mixed Arabic/English technical terms): {answer}

Evaluate the answer considering it was spoken, not written
(so be lenient with grammar/transcription issues, focus on content).

Return ONLY a valid JSON object, nothing else:
{{
  ""score"": <number between 0 and 10>,
  ""feedback"": ""<overall feedback in 2-3 sentences>"",
  ""correct_points"": ""<what the candidate got right>"",
  ""missing_points"": ""<what was missing or could be improved>"",
  ""suggested_answer"": ""<brief ideal answer outline>""
}}
";
            try
            {
                var response = await chat.CompleteChatAsync(prompt);
                var json = CleanJsonResponse(response.Value.Content[0].Text);
                var evaluation = JsonSerializer.Deserialize<AnswerEvaluation>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return evaluation ?? new AnswerEvaluation { Score = 0 };
            }
            catch (Exception ex)
            {
               
                // ✅ Fallback — متوقفيش الـ Interview لو Gemini فشل
                return new AnswerEvaluation
                {
                    Score = 5,
                    Feedback = "Unable to evaluate this answer automatically.",
                    Correct_Points = "",
                    Missing_Points = "",
                    Suggested_Answer = ""
                };
                throw new Exception(ex.Message);
            }
        }

        public async Task<InterviewReport> GenerateFinalReport(string jobTitle, List<QuestionAnswerPair> questionsAndAnswers, float overallScore, FaceSpeechSummary bodyLanguageSummary = null)
        {
            var qaText = string.Join("\n\n", questionsAndAnswers.Select((qa, i) =>
               $"Q{i + 1}: {qa.Question}\nA: {qa.Answer}\nScore: {qa.Score}/10"));

            var bodyLanguagePart = "";
            if (bodyLanguageSummary != null)
            {
                bodyLanguagePart = $@"
Body Language & Speech Analysis:
- Average Eye Contact: {bodyLanguageSummary.AvgEyeContact}/100
- Average Confidence: {bodyLanguageSummary.AvgConfidence}/100
- Dominant Emotion: {bodyLanguageSummary.DominantEmotion}
- Average Speech Confidence: {bodyLanguageSummary.AvgSpeechConfidence}/100
- Speech Pace: {bodyLanguageSummary.SpeechPace}
";
            }

            var prompt = $@"
You are generating a final mock interview report.

Job Title: {jobTitle}
Overall Technical Score: {overallScore}/10

Interview Summary:
{qaText}
{bodyLanguagePart}

Return ONLY a valid JSON object:
{{
  ""overall_feedback"": ""<3-4 sentences overall assessment combining 
     both technical answers AND body language/speech if provided>"",
  ""strengths"": ""<main strong points, technical and non-technical>"",
  ""areas_to_improve"": ""<main areas needing improvement>"",
  ""recommendation"": ""<Hire / Consider / Reject>"",
  ""study_topics"": [""<topic1>"", ""<topic2>"", ""<topic3>""]
}}
";
            try
            {
                var response = await chat.CompleteChatAsync(prompt);
                var json = CleanJsonResponse(response.Value.Content[0].Text);
                var report = JsonSerializer.Deserialize<InterviewReport>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = false });
                return report ?? new InterviewReport();
            }
            catch (Exception ex)
            {
                return new InterviewReport
                {
                    Overall_Feedback = "Report generation failed, but your answers have been saved.",
                    Strengths = "",
                    Areas_To_Improve = "",
                    Recommendation = "Consider",
                    Study_Topics = new List<string>()
                };
                throw new Exception(ex.Message);
            }

        }

        public async Task<List<string>> GenerateGeneralInterviewQuestions(
       string jobTitle,
       List<string> skills,
       string level)
        {
            var skillsList = string.Join(", ", skills);

            var levelGuidance = level.ToLower() switch
            {
                "junior" => "Focus on fundamentals, basic concepts, and simple practical scenarios. Avoid overly advanced architecture or system design questions.",
                "senior" => "Focus on system design, architecture decisions, trade-offs, scalability, leadership, and mentoring scenarios in addition to deep technical knowledge.",
                _ => "Mix fundamental concepts with some intermediate practical scenarios and problem-solving questions." // Mid-level default
            };

            var prompt = $@"
        You are a professional technical interviewer conducting a general 
        mock interview (not tied to a specific job posting).

        Candidate Profile:
        - Target Role: {jobTitle}
        - Experience Level: {level}
        - Skills: {skillsList}

        Level Guidance: {levelGuidance}

STRICT OUTPUT FORMAT RULES:
1. You MUST generate EXACTLY 7 questions in total.
2. The breakdown MUST be: EXACTLY 4 technical questions followed by EXACTLY 3 behavioral questions.
3. Questions should be based on the candidate's skills and 
  general expectations for a {level} {jobTitle}.
4. Make questions progressively harder.
5. Return ONLY a valid JSON array of strings, nothing else

Example format:
[""Question 1 here"", ""Question 2 here""]
";
            try
            {
                var response = await chat.CompleteChatAsync(prompt);
                var json = CleanJsonResponse(response.Value.Content[0].Text);
                var questions = JsonSerializer.Deserialize<List<string>>(json);
                return questions ?? new List<string>();
            }
            catch (Exception ex)
            {
             
                throw new Exception($"Failed to generate questions: {ex.Message}");
            }
        }

        public async Task<FollowUpResult> ShouldAskFollowUp(
            string question,
            string answer,
            float score,
            string jobTitle,
            List<string> skills)
        {
            if (score >= 7)
                return new FollowUpResult { NeedFollowUp = false };

            if (string.IsNullOrWhiteSpace(answer) || answer.Length < 3)
                return new FollowUpResult { NeedFollowUp = false };

            var skillsList = string.Join(", ", skills);
            var prompt = $@"
You are a technical interviewer.
The candidate gave an incomplete or unclear answer.

Job Title: {jobTitle}
Required Skills: {skillsList}
Original Question: {question}
Candidate Answer: {answer}
Score: {score}/10

Generate ONE short follow-up question to help them elaborate or clarify.
The follow-up should be answerable in under 1 minute.

Return ONLY a valid JSON object:
{{
  ""need_follow_up"": true,
  ""follow_up_question"": ""<your follow-up question here>"",
  ""reason"": ""<why you're asking this follow-up, 1 sentence>""
}}

If you think a follow-up won't help (the candidate clearly doesn't 
know this topic), return:
{{
  ""need_follow_up"": false,
  ""follow_up_question"": null,
  ""reason"": null
}}
";
            try
            {
                var response = await chat.CompleteChatAsync(prompt);
                var json = CleanJsonResponse(response.Value.Content[0].Text);
                var result = JsonSerializer.Deserialize<FollowUpResult>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result ?? new FollowUpResult { NeedFollowUp = false };
            }
            catch (Exception ex)
            {
                return new FollowUpResult { NeedFollowUp = false };
                throw new Exception(ex.Message);
            }
        }


        private string CleanJsonResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
                throw new Exception("Empty response from Gemini");

            response = response.Trim();
            if (response.StartsWith("```json"))
                response = response.Substring(7);
            if (response.StartsWith("```"))
                response = response.Substring(3);
            if (response.EndsWith("```"))
                response = response.Substring(0, response.Length - 3);

            return response.Trim();
        }
    }
}
