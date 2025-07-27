using DiscountCodeServer.Models;
using DiscountCodeServer.Protos;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace DiscountCodeServer.Services
{
    public class DiscountService : DiscountCodeService.DiscountCodeServiceBase
    {
        private readonly ILogger<DiscountService> _logger;
        private readonly DatabaseContext _dbContext;

        public DiscountService(
            ILogger<DiscountService> logger,
            DatabaseContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public override async Task<GenerateResponse> GenerateCodes(GenerateRequest request, ServerCallContext context)
        {
            try
            {
                if (request.Count > 2000)
                {
                    _logger.LogWarning($"Requested count {request.Count} exceeds maximum of 2000");
                    return new GenerateResponse { Result = false };
                }

                if (request.Length is < 7 or > 8)
                {
                    _logger.LogWarning($"Invalid code length {request.Length}");
                    return new GenerateResponse { Result = false };
                }

                var codes = new List<DiscountCode>();
                var existingCodes = await _dbContext.DiscountCodes.Select(d => d.Code).ToListAsync();

                for (int i = 0; i < request.Count; i++)
                {
                    string code;
                    do
                    {
                        code = GenerateRandomCode((int)request.Length);
                    } while (existingCodes.Contains(code));

                    codes.Add(new DiscountCode
                    {
                        Code = code,
                        IsUsed = false,
                        GeneratedAt = DateTime.UtcNow
                    });
                }

                await _dbContext.DiscountCodes.AddRangeAsync(codes);
                await _dbContext.SaveChangesAsync();

                return new GenerateResponse { Result = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating codes");
                return new GenerateResponse { Result = false };
            }
        }

        public override async Task<UseCodeResponse> UseCode(
            UseCodeRequest request,
            ServerCallContext context)
        {
            _logger.LogInformation("UseCode request received for code: {Code}",
        string.IsNullOrEmpty(request.Code) ? "NULL" : request.Code);

            try
            {
                if (string.IsNullOrWhiteSpace(request.Code) || request.Code.Length is < 7 or > 8)
                {
                    _logger.LogWarning("Empty code received");
                    return new UseCodeResponse { Result = (UseCodeResponse.Types.ResultCode)2 }; // Invalid code format
                }

                _logger.LogDebug("Looking up code in database");
                var discountCode = await _dbContext.DiscountCodes
                    .FirstOrDefaultAsync(d => d.Code == request.Code);

                if (discountCode == null)
                {
                    _logger.LogWarning("Code not found: {Code}", request.Code);
                    return new UseCodeResponse { Result = (UseCodeResponse.Types.ResultCode)1 }; // Code not found
                }

                if (discountCode.IsUsed)
                {
                    _logger.LogWarning("Attempt to reuse code: {Code} (Originally used at {UsedAt})",
                request.Code, (UseCodeResponse.Types.ResultCode)3);
                    return new UseCodeResponse { Result = (UseCodeResponse.Types.ResultCode)3 }; // Code already used
                }

                discountCode.IsUsed = true;
                discountCode.UsedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return new UseCodeResponse { Result = 0 }; // Success
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error using code");
                return new UseCodeResponse { Result = (UseCodeResponse.Types.ResultCode)4 }; // Server error
            }
        }

        private static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}