﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.MongoDB;
using Backend.Core.Startup;
using Backend.Database;
using MongoDB.Driver;

namespace Backend.Core
{
    public class GenerateSufficientTypesStartupTask : IStartupTask
    {
        private readonly IMongoOperation<SufficientType> _sufficientTypeRepository;

        public GenerateSufficientTypesStartupTask(IMongoOperation<SufficientType> sufficientTypeRepository)
        {
            _sufficientTypeRepository = sufficientTypeRepository;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (_sufficientTypeRepository.Count(FilterDefinition<SufficientType>.Empty) != 0)
            {
                return;
            }

            IList<SufficientType> sufficientTypes = new List<SufficientType>()
            {
                new SufficientType
                {
                    Title = "Energie",
                    Description = "Du hast Energie gespart.",
                    BaselinePoint = 100,
                    BaselineCo2Saving = 5.89
                },
                new SufficientType
                {
                    Title = "Verpackung",
                    Description = "Du hast Verpackungslos eingekauft.",
                    BaselinePoint = 70,
                    BaselineCo2Saving = 7.67
                },
                new SufficientType
                {
                    Title = "Food Waste",
                    Description = "Du hast Food Waste vermieden.",
                    BaselinePoint = 90,
                    BaselineCo2Saving = 4.89
                },
                new SufficientType
                {
                    Title = "Wissen",
                    Description = "Du hast dein Suffizienz mit anderen geteilt.",
                    BaselinePoint = 50,
                    BaselineCo2Saving = 1.88
                },
                new SufficientType
                {
                    Title = "Teilen",
                    Description = "Du hast deinen Besitz mit anderen geteilt.",
                    BaselinePoint = 65,
                    BaselineCo2Saving = 3.99
                },
                new SufficientType
                {
                    Title = "Unterstützen",
                    Description = "Die hast gemeinnützige Dienstleistung geleistet.",
                    BaselinePoint = 35,
                    BaselineCo2Saving = 2.66
                }
            };

            await _sufficientTypeRepository.InsertManyAsync(sufficientTypes);
        }
    }
}