﻿using System.Collections.Generic;
using System.Linq;

namespace Backend.Database.Entities.Widgets.Quiz
{
    public class QuizQuestion : Entity
    {
        public string Question { get; set; } = string.Empty;

        public IEnumerable<string> IncorrectAnswers { get; set; } = Enumerable.Empty<string>();

        public IEnumerable<string> CorrectAnswers { get; set; } = Enumerable.Empty<string>();

        public int Points { get; set; }

        public string DetailedAnswer { get; set; } = string.Empty;
    }
}