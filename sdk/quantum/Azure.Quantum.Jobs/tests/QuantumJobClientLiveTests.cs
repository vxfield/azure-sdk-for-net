﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.TestFramework;
using Azure.Quantum.Jobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Azure.Quantum.Jobs.Tests
{
    public class QuantumJobClientLiveTests: RecordedTestBase<QuantumJobClientTestEnvironment>
    {
        public QuantumJobClientLiveTests(bool isAsync) : base(isAsync)
        {
            Sanitizer = new QuantumJobClientRecordedTestSanitizer();

            //TODO: https://github.com/Azure/autorest.csharp/issues/689
            TestDiagnostics = false;
        }

        private QuantumJobClient CreateClient()
        {
            TestEnvironment.Initialize();

            var rawClient = new QuantumJobClient(TestEnvironment.SubscriptionId, TestEnvironment.ResourceGroup, TestEnvironment.WorkspaceName, TestEnvironment.Location, TestEnvironment.Credential, InstrumentClientOptions(new QuantumJobClientOptions()));

            return InstrumentClient(rawClient);
        }

        [RecordedTest]
        public async Task JobLifecycleTest()
        {
            var client = CreateClient();

            // Get container Uri with SAS key
            var containerName = "testcontainer";
            var containerUri = (await client.GetStorageSasUriAsync(
                new BlobDetails(containerName))).Value.SasUri;

            // Create container if not exists
            var containerClient = new BlobContainerClient(new Uri(containerUri));
            await containerClient.CreateIfNotExistsAsync();

            // Get input data blob Uri with SAS key
            var inputDataUri = (await client.GetStorageSasUriAsync(
                new BlobDetails("testcontainer")
                {
                    BlobName = $"input-{Guid.NewGuid():N}.json",
                })).Value.SasUri;

            // Upload input data to blob
            var blobClient = new BlobClient(new Uri(inputDataUri));
            await blobClient.UploadAsync("problem.json");

            // Submit job
            var jobId = $"job-{Guid.NewGuid():N}";
            var jobName = $"jobName-{Guid.NewGuid():N}";
            var inputDataFormat = "microsoft.qio.v2";
            var outputDataFormat = "microsoft.qio-results.v2";
            var providerId = "microsoft";
            var target = "microsoft.paralleltempering-parameterfree.cpu";
            var createJobDetails = new JobDetails(containerUri, inputDataFormat, providerId, target)
            {
                Id = jobId,
                InputDataUri = inputDataUri,
                Name = jobName,
                OutputDataFormat = outputDataFormat
            };
            var jobDetails = (await client.CreateJobAsync(jobId, createJobDetails)).Value;

            // Check if job was created correctly
            Assert.AreEqual(inputDataFormat, jobDetails.InputDataFormat);
            Assert.AreEqual(outputDataFormat, jobDetails.OutputDataFormat);
            Assert.AreEqual(providerId, jobDetails.ProviderId);
            Assert.AreEqual(target, jobDetails.Target);
            Assert.IsNotEmpty(jobDetails.Id);
            Assert.IsNotEmpty(jobDetails.Name);
            Assert.IsNotEmpty(jobDetails.InputDataUri);
            if (Mode == RecordedTestMode.Playback)
            {
                Assert.IsTrue(jobDetails.Id.StartsWith("job-"));
                Assert.IsTrue(jobDetails.Name.StartsWith("jobName-"));
            }
            else
            {
                Assert.AreEqual(jobId, jobDetails.Id);
                Assert.AreEqual(jobName, jobDetails.Name);
                Assert.AreEqual(inputDataUri, jobDetails.InputDataUri);
            }

            var gotJob = (await client.GetJobAsync(jobId)).Value;
            Assert.AreEqual(jobDetails.InputDataFormat, gotJob.InputDataFormat);
            Assert.AreEqual(jobDetails.OutputDataFormat, gotJob.OutputDataFormat);
            Assert.AreEqual(jobDetails.ProviderId, gotJob.ProviderId);
            Assert.AreEqual(jobDetails.Target, gotJob.Target);
            Assert.AreEqual(jobDetails.Id, gotJob.Id);
            Assert.AreEqual(jobDetails.Name, gotJob.Name);

            await client.CancelJobAsync(jobId);
        }

        [RecordedTest]
        public async Task GetJobsTest()
        {
            var client = CreateClient();

            int index = 0;
            await foreach (JobDetails job in client.GetJobsAsync(CancellationToken.None))
            {
                if (Mode == RecordedTestMode.Playback)
                {
                    Assert.AreEqual("Sanitized", job.ContainerUri);
                    Assert.AreEqual("Sanitized", job.InputDataUri);
                    Assert.AreEqual("Sanitized", job.OutputDataUri);
                }
                else
                {
                    Assert.IsNotEmpty(job.ContainerUri);
                    Assert.IsNotEmpty(job.InputDataUri);
                    Assert.IsNotEmpty(job.OutputDataUri);
                }

                Assert.AreEqual(null, job.CancellationTime);
                Assert.AreEqual(null, job.ErrorData);
                Assert.IsNotEmpty(job.Id);
                Assert.IsNotEmpty(job.InputDataFormat);
                Assert.IsNotEmpty(job.Name);
                Assert.IsNotEmpty(job.OutputDataFormat);
                Assert.IsNotEmpty(job.ProviderId);
                Assert.IsNotNull(job.Status);
                Assert.IsNotEmpty(job.Target);

                JobDetails singleJob = await client.GetJobAsync(job.Id);
                Assert.AreEqual(job.Id, singleJob.Id);

                ++index;
            }

            // Should have at least a couple jobs in the list.
            Assert.GreaterOrEqual(index, 2);
        }

        [Ignore("Only verifying that the sample builds")]
        public void GetJobsSample()
        {
            #region Snippet:Azure_Quantum_Jobs_GetJobs
            var client = new QuantumJobClient("subscriptionId", "resourceGroupName", "workspaceName", "location");
            var jobs = client.GetJobs();
            #endregion
        }
    }
}
