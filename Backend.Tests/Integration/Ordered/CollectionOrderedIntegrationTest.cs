﻿using Backend.Tests.Integration.Utilities;
using Xunit;

namespace Backend.Tests.Integration
{
    [CollectionDefinition("IntegrationTests")]
    public class CollectionOrderedIntegrationTest : ICollectionFixture<OrderedTestContext>
    {
    }
}
