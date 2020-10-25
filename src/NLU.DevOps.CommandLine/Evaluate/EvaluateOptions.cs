// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace NLU.DevOps.CommandLine.Evaluate
{
    using global::CommandLine;

    [Verb("evaluate", HelpText = "Runs batch testing against the NLU model.")]
    internal class EvaluateOptions : BaseOptions
    {
        [Option('u', "utterances", HelpText = "Path to utterances.", Required = true)]
        public string UtterancesPath { get; set; }

        [Option('o', "output", HelpText = "Path to labeled results output.", Required = false)]
        public string OutputPath { get; set; }
    }
}
