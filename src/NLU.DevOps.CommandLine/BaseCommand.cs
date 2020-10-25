﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace NLU.DevOps.CommandLine
{
    using System;
    using System.IO;
    using Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Models;

    internal abstract class BaseCommand<TOptions> : ICommand
        where TOptions : BaseOptions
    {
        public BaseCommand(TOptions options)
        {
            this.Options = options;
            this.LazyConfiguration = new Lazy<IConfiguration>(this.CreateConfiguration);
            this.LazyNLUTrainClient = new Lazy<INLUTrainClient>(this.CreateNLUTrainClient);
            this.LazyNLUTestClient = new Lazy<INLUTestClient>(this.CreateNLUTestClient);
            this.LazyNLUEvaluationClient = new Lazy<INLUEvaluationClient>(this.CreateNLUEvaluationClient);
            this.LazyLogger = new Lazy<ILogger>(this.CreateLogger);
        }

        protected TOptions Options { get; }

        protected IConfiguration Configuration => this.LazyConfiguration.Value;

        protected INLUTrainClient NLUTrainClient => this.LazyNLUTrainClient.Value;

        protected INLUTestClient NLUTestClient => this.LazyNLUTestClient.Value;

        protected INLUEvaluationClient NLUEvaluationClient => this.LazyNLUEvaluationClient.Value;

        protected ILogger Logger => this.LazyLogger.Value;

        private Lazy<IConfiguration> LazyConfiguration { get; }

        private Lazy<INLUTrainClient> LazyNLUTrainClient { get; }

        private Lazy<INLUTestClient> LazyNLUTestClient { get; }

        private Lazy<INLUEvaluationClient> LazyNLUEvaluationClient { get; }

        private Lazy<ILogger> LazyLogger { get; }

        public abstract int Main();

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual INLUTrainClient CreateNLUTrainClient()
        {
            return NLUClientFactory.CreateTrainInstance(this.Options, this.Configuration);
        }

        protected virtual INLUTestClient CreateNLUTestClient()
        {
            return NLUClientFactory.CreateTestInstance(this.Options, this.Configuration);
        }

        protected virtual INLUEvaluationClient CreateNLUEvaluationClient()
        {
            return NLUClientFactory.CreateEvaluationInstance(this.Options, this.Configuration);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                var trainClientException = default(Exception);
                if (this.LazyNLUTrainClient.IsValueCreated)
                {
                    try
                    {
                        this.NLUTrainClient.Dispose();
                    }
                    catch (Exception ex)
                    {
                        trainClientException = ex;
                    }
                }

                if (this.LazyNLUTestClient.IsValueCreated)
                {
                    try
                    {
                        this.NLUTestClient.Dispose();
                    }
                    catch (Exception ex)
                    when (trainClientException != null)
                    {
                        /* NLUTestClient exception will not be caught if no exception occurred when disposing NLUTrainClient */
                        throw new AggregateException(trainClientException, ex);
                    }
                }

                if (trainClientException != null)
                {
                    throw trainClientException;
                }
            }
        }

        protected void Log(string message)
        {
            this.LazyLogger.Value.LogInformation(message);
        }

        private IConfiguration CreateConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{this.Options.Service}.json", true)
                .AddJsonFile("appsettings.local.json", true)
                .AddEnvironmentVariables()
                .Build();
        }

        private ILogger CreateLogger()
        {
            var logLevel = this.Options.Verbose ? LogLevel.Trace : LogLevel.Information;

            if (this.Options.Quiet)
            {
                logLevel = LogLevel.Warning;
            }

            return ApplicationLogger.LoggerFactory.AddConsole(logLevel).CreateLogger(this.GetType());
        }
    }
}
