﻿using AspNetCore.MongoDB;
using System.Collections.Generic;

namespace Backend.Database.Widgets.Quiz
{
    public class QuizQuestion : IMongoEntity
    {
        public string Question { get; set; }

        public IEnumerable<string> IncorrectAnswers { get; set; }

        public IEnumerable<string> CorrectAnswers { get; set; }

        public int Points { get; set; }

        public string DetailedAnswer { get; set; }
    }
}
