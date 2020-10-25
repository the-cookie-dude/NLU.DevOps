// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace NLU.DevOps.CommandLine.Evaluate
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Models;
    using static Serializer;

    internal class EvaluateCommand : BaseCommand<EvaluateOptions>
    {
        public EvaluateCommand(EvaluateOptions options)
            : base(options)
        {
        }

        public override int Main()
        {
            this.RunAsync().Wait();
            return 0;
        }

        protected override INLUEvaluationClient CreateNLUEvaluationClient()
        {
            return NLUClientFactory.CreateEvaluationInstance(this.Options, this.Configuration);
        }

        private async Task RunAsync()
        {
            this.Log("Running batch testing against NLU model...");

            // 01. load test data from file
            var testUtterances = this.LoadUtterances();

            // 02. call evluation method on NLU Client
            var evaluationResult = await this.NLUEvaluationClient.BatchTestAsync(testUtterances).ConfigureAwait(false);

            // 04. store result

            // 03. compare with baseline file
            Stream getFileStream(string filePath)
            {
                EnsureDirectory(filePath);
                return File.Open(filePath, FileMode.Create);
            }

            var stream = this.Options.OutputPath != null
                ? getFileStream(this.Options.OutputPath)
                : Console.OpenStandardOutput();

            using (stream)
            {
                Write(stream, evaluationResult);
            }
        }

        private IEnumerable<LabeledUtterance> LoadUtterances()
        {
            return Read<IEnumerable<LabeledUtterance>>(this.Options.UtterancesPath);
        }

        private static void EnsureDirectory(string filePath)
        {
            var baseDirectory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(baseDirectory) && !Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }
        }
    }
}
