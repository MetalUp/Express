﻿using Model.Functions.Services;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Interface;

namespace Batch;

internal class Handler {
    public Handler(IFrameworkFacade frameworkFacade) => FrameworkFacade = frameworkFacade;

    private IFrameworkFacade FrameworkFacade { get; }

    public void Run() {
        try {
            FrameworkFacade.Start();
            var args = new ArgumentsContextFacade { ExpectedActionType = MethodType.NonIdempotent, Values = new Dictionary<string, object>() };
            FrameworkFacade.ExecuteMenuAction(typeof(BatchProcessing).FullName, nameof(BatchProcessing.UpdateFiles), args);
            FrameworkFacade.End(true);
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
}