using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.OperationsClasses.SyncWithReturn
{
    public class SyncWithReturn : IRequest<object>, ICommand
    {
        public SyncWithReturn(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}