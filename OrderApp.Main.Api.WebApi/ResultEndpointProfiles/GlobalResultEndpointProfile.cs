using FluentResults;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OrderApp.Main.Api.Application.Errors;

namespace OrderApp.Main.Api.WebApi.ResultEndpointProfiles
{
    public class GlobalResultEndpointProfile : DefaultAspNetCoreResultEndpointProfile
    {
        public override ActionResult TransformFailedResultToActionResult(
            FailedResultToActionResultTransformationContext context
        )
        {
            var result = context.Result;

            if (result.Errors.Count == 1 && result.HasError<NotFoundError>())
            {
                var error = context.Result.Errors.First();
                return new NotFoundObjectResult(error.Message);
            }

            return base.TransformFailedResultToActionResult(context);
        }

        public override ActionResult TransformOkNoValueResultToActionResult(
            OkResultToActionResultTransformationContext<Result> context
        )
        {
            return new NoContentResult();
        }
    }
}
