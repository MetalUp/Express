using Microsoft.Extensions.Logging;
using Model.Functions.Services;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Interface;

namespace Batch;

internal class Handler {
    public Handler(IFrameworkFacade frameworkFacade, ILogger<Handler> logger) {
        Logger = logger;
        FrameworkFacade = frameworkFacade;
    }

    private ILogger Logger { get; }
    private IFrameworkFacade FrameworkFacade { get; }

    public void Run() {
        try {
            Logger.LogInformation("Starting Batch");
            FrameworkFacade.Start();
            var args = new ArgumentsContextFacade { ExpectedActionType = MethodType.NonIdempotent, Values = new Dictionary<string, object>() };
            FrameworkFacade.ExecuteMenuAction(typeof(BatchProcessing).FullName, nameof(BatchProcessing.UpdateFiles), args);
            FrameworkFacade.End(true);
            Logger.LogInformation("Batch complete with no errors");
        }
        catch (Exception e) {
            Logger.LogError(e, $"Batch failed with error: {e.Message}");
            throw;
        }
    }
}