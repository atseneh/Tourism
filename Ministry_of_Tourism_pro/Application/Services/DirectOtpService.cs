// File: DirectOtpService.cs
// .NET 6 Compatible
// In-Memory OTP Generation & Verification Service
// Stores OTP in memory until expiration and auto removes expired OTPs
// This is a SECOND OPTION alongside the existing CNET OTP system

using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ministry_of_Tourism_pro.Application.Services
{
    /// <summary>
    /// Direct OTP Service - Generates, stores, and verifies OTPs in-memory.
    /// Sends SMS via the existing SMS/Send_SMS API endpoint.
    /// Register as Singleton in DI container.
    /// </summary>
    public sealed class DirectOtpService : IDisposable
    {
        // In-Memory OTP Store (Thread-Safe)
        private readonly ConcurrentDictionary<string, OtpEntry> _otpStore = new();

        // Cleanup Timer for expired OTPs
        private readonly Timer _cleanupTimer;

        private readonly ILogger<DirectOtpService> _logger;

        public DirectOtpService(ILogger<DirectOtpService> logger)
        {
            _logger = logger;

            // Cleanup expired OTPs every minute
            _cleanupTimer = new Timer(
                RemoveExpiredOtps,
                null,
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1)
            );

            _logger.LogInformation("DirectOtpService initialized. Cleanup timer started.");
        }

        /// <summary>
        /// Generates a cryptographically secure OTP of the specified length.
        /// </summary>
        public string GenerateOtp(int length = 6)
        {
            if (length <= 0)
                throw new ArgumentException("OTP length must be greater than zero.");

            const string digits = "0123456789";

            char[] otp = new char[length];

            using var rng = RandomNumberGenerator.Create();
            byte[] buffer = new byte[length];
            rng.GetBytes(buffer);

            for (int i = 0; i < length; i++)
            {
                otp[i] = digits[buffer[i] % digits.Length];
            }

            return new string(otp);
        }

        /// <summary>
        /// Generate + Store OTP in memory for the given phone number.
        /// Returns the generated OTP code and a unique verification ID.
        /// The caller is responsible for sending the SMS.
        /// </summary>
        public DirectOtpResult CreateOtp(
            string phoneNumber,
            int otpLength = 6,
            int expiryMinutes = 5)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.");

            string otp = GenerateOtp(otpLength);
            string verificationId = Guid.NewGuid().ToString();

            string message =
                $"Your verification code is {otp}. " +
                $"It expires in {expiryMinutes} minutes.";

            // Store OTP
            _otpStore[phoneNumber] = new OtpEntry
            {
                Code = otp,
                VerificationId = verificationId,
                ExpiryTime = DateTime.UtcNow.AddMinutes(expiryMinutes),
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation(
                "Direct OTP created for {Phone}. VerificationId: {VerificationId}. Expires in {Minutes} minutes.",
                phoneNumber, verificationId, expiryMinutes);

            return new DirectOtpResult
            {
                Success = true,
                Code = otp,
                VerificationId = verificationId,
                Message = message,
                PhoneNumber = phoneNumber
            };
        }

        /// <summary>
        /// Verify the OTP code for a given phone number.
        /// OTP is removed after successful verification (one-time use).
        /// </summary>
        public DirectOtpVerifyResult VerifyOtp(string phoneNumber, string code)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(code))
            {
                return new DirectOtpVerifyResult
                {
                    IsValid = false,
                    Message = "Phone number and verification code are required."
                };
            }

            if (!_otpStore.TryGetValue(phoneNumber, out var entry))
            {
                _logger.LogWarning("Direct OTP verification failed for {Phone}: No OTP found.", phoneNumber);
                return new DirectOtpVerifyResult
                {
                    IsValid = false,
                    Message = "No verification code found for this phone number. Please request a new code."
                };
            }

            // Check expiry
            if (DateTime.UtcNow > entry.ExpiryTime)
            {
                _otpStore.TryRemove(phoneNumber, out _);
                _logger.LogWarning("Direct OTP verification failed for {Phone}: OTP expired.", phoneNumber);
                return new DirectOtpVerifyResult
                {
                    IsValid = false,
                    Message = "Verification code has expired. Please request a new code."
                };
            }

            // Check code match
            if (entry.Code != code)
            {
                _logger.LogWarning("Direct OTP verification failed for {Phone}: Code mismatch.", phoneNumber);
                return new DirectOtpVerifyResult
                {
                    IsValid = false,
                    Message = "Invalid verification code. Please try again."
                };
            }

            // Success — Remove after successful verification (one-time use)
            _otpStore.TryRemove(phoneNumber, out _);
            _logger.LogInformation("Direct OTP verified successfully for {Phone}.", phoneNumber);

            return new DirectOtpVerifyResult
            {
                IsValid = true,
                Message = "Phone number verified successfully."
            };
        }

        /// <summary>
        /// Check if an OTP exists and is still valid for a phone number (without consuming it).
        /// </summary>
        public bool HasValidOtp(string phoneNumber)
        {
            if (_otpStore.TryGetValue(phoneNumber, out var entry))
            {
                return DateTime.UtcNow <= entry.ExpiryTime;
            }
            return false;
        }

        /// <summary>
        /// Remove Expired OTPs Automatically (called by timer).
        /// </summary>
        private void RemoveExpiredOtps(object? state)
        {
            var now = DateTime.UtcNow;
            int removedCount = 0;

            foreach (var item in _otpStore)
            {
                if (item.Value.ExpiryTime <= now)
                {
                    if (_otpStore.TryRemove(item.Key, out _))
                        removedCount++;
                }
            }

            if (removedCount > 0)
            {
                _logger.LogInformation("DirectOtpService cleanup: Removed {Count} expired OTP(s).", removedCount);
            }
        }

        /// <summary>
        /// Cleanup Timer Dispose
        /// </summary>
        public void Dispose()
        {
            _cleanupTimer?.Dispose();
        }

        // ============================================================
        // Internal Models
        // ============================================================

        /// <summary>
        /// Internal OTP storage entry
        /// </summary>
        private sealed class OtpEntry
        {
            public string Code { get; set; } = string.Empty;
            public string VerificationId { get; set; } = string.Empty;
            public DateTime ExpiryTime { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }

    // ============================================================
    // Public Result Models
    // ============================================================

    /// <summary>
    /// Result returned when creating/sending a direct OTP
    /// </summary>
    public class DirectOtpResult
    {
        public bool Success { get; set; }
        public string Code { get; set; } = string.Empty;
        public string VerificationId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    /// <summary>
    /// Result returned when verifying a direct OTP
    /// </summary>
    public class DirectOtpVerifyResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
