﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Azure.Core.TestFramework;
using Azure.Identity;
using Azure.Quantum.Jobs.Models;
using NUnit.Framework;

namespace Azure.Quantum.Jobs.Tests.Samples
{
    public class QuantumJobsClientSamples: SamplesBase<QuantumJobsClientTestEnvironment>
    {
        [Test]
        public void GettingASecret()
        {
//             var endpoint = TestEnvironment.KeyVaultUri;
//
//             #region Snippet:GetSecret
//             var client = new QuantumJobsClient(new Uri(endpoint), new DefaultAzureCredential());
//
//             SecretBundle secret = client.GetSecret("TestSecret");
//
//             Console.WriteLine(secret.Value);
//             #endregion
        }
    }
}
