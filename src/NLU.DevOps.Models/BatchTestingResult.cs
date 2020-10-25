// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace NLU.DevOps.Models
{
    /// <summary>
    /// Result returned from batch testing.
    /// </summary>
    public class BatchTestingResult
    {
        /// <summary>
        /// Gets or sets gets the model name.
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// Gets or sets gets the model type.
        /// </summary>
        public string ModelType { get; set; }

        /// <summary>
        /// Gets or sets gets the precision.
        /// </summary>
        public long Precision { get; set; }
    }
}
