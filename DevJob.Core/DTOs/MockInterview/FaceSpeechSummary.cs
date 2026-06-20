using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevJob.Application.DTOs.MockInterview
{
    public class FaceSpeechSummary
    {
        public float AvgEyeContact { get; set; }
        public float AvgConfidence { get; set; }
        public string DominantEmotion { get; set; }
        public float AvgSpeechConfidence { get; set; }
        public string SpeechPace { get; set; }
    }
}
