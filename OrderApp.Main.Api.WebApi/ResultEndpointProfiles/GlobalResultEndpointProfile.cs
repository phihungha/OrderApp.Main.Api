using FluentResults;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OrderApp.Main.Api.Domain.Errors;

namespace OrderApp.Main.Api.WebApi.ResultEndpointProfiles
{
    public class GlobalResultEndpointProfile : DefaultAspNetCoreResultEndpointProfile
    {
        public override ActionResult TransformFailedResultToActionResult(
            FailedResultToActionResultTransformationContext context
        )
        {
            var result = context.Result;

            List<ErrorDto> errorDto = [];
            if (result.Errors.Count == 1 && result.HasError<NotFoundError>())
            {
                var error = context.Result.Errors.First();
                errorDto.Add(new ErrorDto { Message = error.Message });
                return new NotFoundObjectResult(errorDto);
            }

            errorDto.AddRange(result.Errors.Select(e => new ErrorDto { Message = e.Message }));
            return new BadRequestObjectResult(errorDto);
        }

        public override ActionResult TransformOkNoValueResultToActionResult(
            OkResultToActionResultTransformationContext<Result> context
        )
        {
            return new NoContentResult();
        }
    }
}
