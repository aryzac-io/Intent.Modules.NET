using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.TestApplication.Application.Customers.CreateCustomer;
using AzureFunctions.TestApplication.Domain.Common.Exceptions;
using AzureFunctions.TestApplication.Domain.Common.Interfaces;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureFunctionClass", Version = "1.0")]

namespace AzureFunctions.TestApplication.Api
{
    public class CreateCustomer
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomer(IMediator mediator, IUnitOfWork unitOfWork)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [FunctionName("CreateCustomer")]
        [OpenApiOperation("CreateCustomerCommand", tags: new[] { "Customers" }, Description = "Create customer command")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CreateCustomerCommand))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Guid))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(object))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "customers")] HttpRequest req,
            CancellationToken cancellationToken)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var command = JsonConvert.DeserializeObject<CreateCustomerCommand>(requestBody)!;
                var result = await _mediator.Send(command, cancellationToken);
                return new CreatedResult(string.Empty, result);
            }
            catch (ValidationException exception)
            {
                return new BadRequestObjectResult(exception.Errors);
            }
            catch (NotFoundException exception)
            {
                return new NotFoundObjectResult(new { Message = exception.Message });
            }
            catch (FormatException exception)
            {
                return new BadRequestObjectResult(new { Message = exception.Message });
            }
        }
    }
}