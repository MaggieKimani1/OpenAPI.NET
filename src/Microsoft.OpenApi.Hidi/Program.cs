﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.CommandLine;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.OpenApi.Hidi
{
    static class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand() {
            };

            // command option parameters and aliases
            var descriptionOption = new Option<string>("--openapi", "Input OpenAPI description file path or URL");
            descriptionOption.AddAlias("-d");

            var csdlOption = new Option<string>("--csdl", "Input CSDL file path or URL");
            csdlOption.AddAlias("-cs");

            var outputOption = new Option<FileInfo>("--output", () => new FileInfo("./output"), "The output directory path for the generated file.") { Arity = ArgumentArity.ZeroOrOne };
            outputOption.AddAlias("-o");

            var versionOption = new Option<OpenApiSpecVersion?>("--version", "OpenAPI specification version");
            versionOption.AddAlias("-v");

            var formatOption = new Option<OpenApiFormat?>("--format", "File format");
            formatOption.AddAlias("-f");

            var logLevelOption = new Option<LogLevel>("--loglevel", () => LogLevel.Warning, "The log level to use when logging messages to the main output.");
            logLevelOption.AddAlias("-ll");

            var filterByOperationIdsOption = new Option<string>("--filter-by-operationids", "Filters OpenApiDocument by OperationId(s) provided");
            filterByOperationIdsOption.AddAlias("-op");

            var filterByTagsOption = new Option<string>("--filter-by-tags", "Filters OpenApiDocument by Tag(s) provided");
            filterByTagsOption.AddAlias("-t");

            var filterByCollectionOption = new Option<string>("--filter-by-collection", "Filters OpenApiDocument by Postman collection provided");
            filterByCollectionOption.AddAlias("-c");

            var inlineOption = new Option<bool>("--inline", "Inline $ref instances");
            inlineOption.AddAlias("-i");

            var resolveExternalOption = new Option<bool>("--resolve-external", "Resolve external $refs");
            resolveExternalOption.AddAlias("-ex");

            var validateCommand = new Command("validate")
            {
                descriptionOption,
                logLevelOption
            };

            validateCommand.SetHandler<string, LogLevel>(OpenApiService.ValidateOpenApiDocument, descriptionOption, logLevelOption);

            var transformCommand = new Command("transform")
            {
                descriptionOption,
                csdlOption,
                outputOption,
                versionOption,
                formatOption,
                logLevelOption,               
                filterByOperationIdsOption,
                filterByTagsOption,
                filterByCollectionOption,
                inlineOption,
                resolveExternalOption,
            };

            transformCommand.SetHandler<string, string, FileInfo, OpenApiSpecVersion?, OpenApiFormat?, LogLevel, bool, bool, string, string, string> (
                OpenApiService.ProcessOpenApiDocument, descriptionOption, csdlOption, outputOption, versionOption, formatOption, logLevelOption, inlineOption, resolveExternalOption, filterByOperationIdsOption, filterByTagsOption, filterByCollectionOption);

            rootCommand.Add(transformCommand);
            rootCommand.Add(validateCommand);

            // Parse the incoming args and invoke the handler
            return await rootCommand.InvokeAsync(args);
        }
    }
}
