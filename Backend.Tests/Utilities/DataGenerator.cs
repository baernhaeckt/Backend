﻿using System;
using Bogus;

namespace Backend.Tests.Utilities
{
    public static class DataGenerator
    {
        public static string RandomEmail()
        {
            Randomizer.Seed = new Random();
            var faker = new Faker();
            return faker.Internet.Email();
        }
    }
}