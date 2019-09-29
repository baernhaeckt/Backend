﻿namespace Backend.Database.Entities
{
    public class Location : Entity
    {
        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string Zip { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
    }
}