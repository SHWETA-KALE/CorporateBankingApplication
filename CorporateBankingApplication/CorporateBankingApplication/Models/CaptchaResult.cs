using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApplication.Models
{
    public class CaptchaResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        //[JsonProperty("score")]
        //public float Score { get; set; }
        //[JsonProperty("action")]
        //public string Action { get; set; }
        [JsonProperty("challenge_ts")]
        public DateTime ChallengeTs { get; set; }
        [JsonProperty("hostname")]
        public string Hostname { get; set; }
    }
}