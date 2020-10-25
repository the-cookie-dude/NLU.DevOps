// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace NLU.DevOps.Models
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// NLU testing interface.
    /// </summary>
    public interface INLUEvaluationClient : IDisposable
    {
        /// <summary>
        /// Evaluates (batch testing) the NLU model.
        /// </summary>
        /// <returns>Task to await the resulting fscores.</returns>
        /// <param name="testData">Query to test.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<BatchTestingResult> BatchTestAsync(
            IEnumerable<LabeledUtterance> testData,
            CancellationToken cancellationToken);
    }
}
