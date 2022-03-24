/*
 * Copyright 2022 Systemic Pty Ltd
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Sif.Framework.AspNetCore.Extensions;
using Sif.Framework.Model.Authentication;
using Sif.Framework.Model.Exceptions;
using Sif.Framework.Service.Authentication;
using System.Net.Http.Headers;
using Environment = Sif.Framework.Model.Infrastructure.Environment;

namespace Sif.Framework.AspNetCore.Services.Authentication;

/// <inheritdoc cref="IAuthenticationService{THeaders}"/>
public abstract class AuthenticationService : IAuthenticationService<IHeaderDictionary>
{
    /// <inheritdoc cref="IAuthenticationService{THeaders}.GetEnvironmentBySessionToken(string)" />
    public abstract Environment GetEnvironmentBySessionToken(string sessionToken);

    /// <summary>
    /// Retrieve the shared secret value associated with the passed in application key.
    /// </summary>
    /// <param name="applicationKey">Application key associated with the shared secret.</param>
    /// <returns>Shared secret value.</returns>
    protected abstract string? InitialSharedSecret(string applicationKey);

    /// <summary>
    /// Retrieve the shared secret value associated with the passed in session token.
    /// </summary>
    /// <param name="sessionToken">Session token associated with the shared secret.</param>
    /// <returns>Shared secret value.</returns>
    /// <exception cref="InvalidSessionException">sessionToken is invalid.</exception>
    protected abstract string? SharedSecret(string sessionToken);

    /// <summary>
    /// Verify the authentication header.
    /// </summary>
    /// <param name="headers">HTTP request headers.</param>
    /// <param name="initial">Flag to indicate whether this is the initial verification call.</param>
    /// <param name="sessionToken">Session token associated with the authentication header.</param>
    /// <returns>True if the initial authentication header is valid; false otherwise.</returns>
    protected bool VerifyAuthenticationHeader(IHeaderDictionary headers, bool initial, out string? sessionToken)
    {
        var verified = false;
        string? sessionTokenChecked = null;

        StringValues authorization = headers[HeaderNames.Authorization];

        if (AuthenticationHeaderValue.TryParse(authorization, out AuthenticationHeaderValue? headerValue))
        {
            GetSharedSecret sharedSecret;

            if (initial)
            {
                sharedSecret = InitialSharedSecret;
            }
            else
            {
                sharedSecret = SharedSecret;
            }

            try
            {
                if (AuthenticationMethod.Basic.ToString().Equals(
                        headerValue.Scheme,
                        StringComparison.OrdinalIgnoreCase))
                {
                    var authorisationToken = new AuthorisationToken { Token = headerValue.Parameter };
                    IAuthorisationTokenService authorisationTokenService = new BasicAuthorisationTokenService();
                    verified = authorisationTokenService.Verify(
                        authorisationToken,
                        sharedSecret,
                        out sessionTokenChecked);
                }
                else if (AuthenticationMethod.SIF_HMACSHA256.ToString().Equals(
                             headerValue.Scheme,
                             StringComparison.OrdinalIgnoreCase))
                {
                    string? timestamp = headers.GetTimestamp();

                    var authorisationToken = new AuthorisationToken
                    {
                        Token = headerValue.Parameter,
                        Timestamp = timestamp
                    };

                    IAuthorisationTokenService authorisationTokenService = new HmacShaAuthorisationTokenService();
                    verified = authorisationTokenService.Verify(
                        authorisationToken,
                        sharedSecret,
                        out sessionTokenChecked);
                }
            }
            catch (InvalidSessionException)
            {
                verified = false;
            }
        }

        sessionToken = sessionTokenChecked;

        return verified;
    }

    /// <inheritdoc cref="IAuthenticationService{THeaders}.VerifyAuthenticationHeader(THeaders)" />
    public virtual bool VerifyAuthenticationHeader(IHeaderDictionary headers)
    {
        return VerifyAuthenticationHeader(headers, false, out string? _);
    }

    /// <inheritdoc cref="IAuthenticationService{THeaders}.VerifyAuthenticationHeader(THeaders, out string)" />
    public virtual bool VerifyAuthenticationHeader(IHeaderDictionary headers, out string? sessionToken)
    {
        return VerifyAuthenticationHeader(headers, false, out sessionToken);
    }

    /// <inheritdoc cref="IAuthenticationService{THeaders}.VerifyInitialAuthenticationHeader(THeaders, out string)" />
    public virtual bool VerifyInitialAuthenticationHeader(IHeaderDictionary header, out string? sessionToken)
    {
        return VerifyAuthenticationHeader(header, true, out sessionToken);
    }
}