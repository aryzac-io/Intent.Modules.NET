﻿using System;

namespace Intent.Modules.Constants
{
    public static class TemplateRoles
    {
        public static class Distribution
        {

            public static class Custom
            {
                public const string Dispatcher = "Distribution.Custom.Dispatcher";
            }

            public static class WebApi
            {
                public const string Controller = "Distribution.WebApi.Controller";
                public const string Startup = "App.Startup";
                public const string MultiTenancyConfiguration = "Configuration.MultiTenancy";
            }
        }
        public static class Domain
        {
            public const string Enum = "Domain.Enum";
            public const string UnitOfWork = "Domain.UnitOfWork";
            public const string MongoDbUnitOfWork = "Domain.UnitOfWork.MongoDb";
            public const string ValueObject = "Domain.ValueObject";
            public const string DataContract = "Domain.DataContract";
            public const string Events = "Domain.Events";

            public static class Entity
            {
                public const string Primary = "Domain.Entity";
                public const string Interface = "Domain.Entity.Interface";
                public const string EntityImplementation = "Domain.Entity.Primary";
                public const string State = "Domain.Entity.State";
            }

            public static class DomainServices
            {
                public const string Interface = "Domain.DomainServices.Interface";
                public const string Implementation = "Domain.DomainServices.Implementation";
            }
        }

        public static class Repository
        {
            [Obsolete("This template has been moved Infrastructure.Data.PagedList")]
            public const string PagedList = "Repository.Implementation.PagedList";

            public static class Interface
            {
                public const string Entity = "Repository.Interface.Entity";
                [Obsolete("This template has been moved Repository.Interface.PagedList")]
                public const string PagedResult = "Repository.Interface.PagedResult";
                public const string PagedList = "Repository.Interface.PagedList";
            }
        }

        public static class Application
        {
            public const string DependencyInjection = "Application.DependencyInjection";
            public const string Query = "Application.Query";
            public const string Command = "Application.Command";

            public static class Common
            {
                public const string DbContextInterface = "Application.Common.DbContextInterface";
                public const string ValidationServiceInterface = "Application.Common.ValidatonServiceInterface";
                public const string PagedList = "Application.Common.PagedList";

            }

            public static class Contracts
            {
                public const string Dto = "Application.Contract.Dto";
                public const string Enum = "Application.Contract.Enum";
            }
            
            public static class Services
            {
                [Obsolete("Use Distribution.WebApi.Controller")]
                public const string Controllers = "Intent.AspNetCore.Controllers.Controller";
                public const string Interface = "Application.Contracts";
                public const string  Implementation = "Application.Implementation";
            }
            
            public static class Eventing
            {
                public const string EventBusInterface = "Application.Eventing.EventBusInterface";
                public const string EventHandler = "Application.Eventing.EventHandler";
            }

            public static class Validation
            {
                public const string Dto = "Application.Validation.Dto";
                public const string Command = "Application.Validation.Command";
                public const string Query = "Application.Validation.Query";
            }

            public const string Mappings = "Application.Mappings";

            public static class DomainEventHandler
            {
                [Obsolete("Remove this in favor of Default")]
                public const string OldDefault = "Application.DomainEventHandler";
                public const string Default = "Application.DomainEventHandler.Default";
                public const string Explicit = "Application.DomainEventHandler.Explicit";
            }
        }

        public static class Infrastructure
        {
            public const string DependencyInjection = "Infrastructure.DependencyInjection";
            public static class Data
            {
                public const string DbContext = "Infrastructure.Data.DbContext";
                [Obsolete("This template has been moved Application.Common.PagedList")]
                public const string PagedList = "Infrastructure.Data.PagedList";
            }
        }
    }
}
