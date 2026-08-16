using AuthService.Api.Contracts.Auth;
using AuthService.Application.Users.Register;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public sealed class AuthController(
        RegisterHandler registerHandler) : ControllerBase
    {
        [HttpPost("register")]
        [ProducesResponseType<RegisterResponse>(
            StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(
            StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RegisterResponse>> Register(
            RegisterRequest request,
            CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(
                request.Email,
                request.Password);

            var result = await registerHandler.HandleAsync(
                command,
                cancellationToken);

            return result.Status switch
            {
                RegisterStatus.Success =>
                    StatusCode(
                        StatusCodes.Status201Created,
                        new RegisterResponse(
                            result.UserId!.Value,
                            result.Email!)),

                RegisterStatus.InvalidEmail =>
                    CreateProblem(
                        StatusCodes.Status400BadRequest,
                        "Invalid email",
                        "The provided email address is invalid.",
                        "auth.invalid_email"),

                RegisterStatus.WeakPassword =>
                    CreateProblem(
                        StatusCodes.Status400BadRequest,
                        "Invalid password",
                        $"Password length must be between " +
                        $"{PasswordPolicy.MinimumLength} and " +
                        $"{PasswordPolicy.MaximumLength} characters.",
                        "auth.invalid_password"),

                RegisterStatus.EmailAlreadyExists =>
                    CreateProblem(
                        StatusCodes.Status409Conflict,
                        "Email already registered",
                        "An account with this email already exists.",
                        "auth.email_already_exists"),

                _ => throw new InvalidOperationException(
                    $"Unsupported register status: {result.Status}.")
            };
        }

        private ObjectResult CreateProblem(
            int status,
            string title,
            string detail,
            string code)
        {
            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Type = $"https://httpstatuses.com/{status}"
            };

            problem.Extensions["code"] = code;

            return StatusCode(status, problem);
        }
    }
}
