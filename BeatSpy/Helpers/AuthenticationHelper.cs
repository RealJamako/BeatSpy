﻿using System;
using System.IO;
using BeatSpy.Models;
using SpotifyAPI.Web;
using System.Text.Json;
using System.Threading.Tasks;
using BeatSpy.DataTypes.Constants;

namespace BeatSpy.Helpers;

public static class AuthenticationHelper
{
    /// <summary>
    /// Try to retrieve content from the authentication file.
    /// </summary>
    /// <param name="authContent"></param>
    /// <returns></returns>
    public static bool TryGetAuthFile(out string authContent)
    {
        string? authPath = DataFolderHelper.GetFullDataPath(FileConstants.AUTH_FILE);

        if (File.Exists(authPath))
        {
            authContent = File.ReadAllText(authPath);
            return !string.IsNullOrEmpty(authContent);
        }

        authContent = string.Empty;
        return false;
    }

    /// <summary>
    /// Deserializes the provided authentication content
    /// </summary>
    /// <param name="authContent">Authentication content</param>
    /// <returns></returns>
    public static SpotifyToken? DeserializeTokenContent(string authContent)
    {
        return JsonSerializer.Deserialize<SpotifyToken>(authContent);
    }

    /// <summary>
    /// Serializes the provided authentication token and writes to data path
    /// </summary>
    /// <param name="authToken">Authentication object</param>
    public static void SerializeTokenContent(SpotifyToken authToken)
    {
        string serializedToken = JsonSerializer.Serialize(authToken)!;
        File.WriteAllText(DataFolderHelper.GetFullDataPath("AppAuth.json"), serializedToken);
    }

    /// <summary>
    /// Initiates an asynchronous PKCE token request using provided client ID, callback URL, code, and verifier.
    /// </summary>
    /// <param name="clientID">Spotify client ID</param>
    /// <param name="callBack">Callback Url</param>
    /// <param name="code">The code query from the listener context</param>
    /// <param name="verifier">Verifier code</param>
    /// <returns></returns>
    public static async Task<PKCETokenResponse> PKCETokenRequestResponse(string clientID, string callBack, string code, string verifier)
    {
        PKCETokenResponse reponse = await new OAuthClient().RequestToken(new PKCETokenRequest(clientID, code, new Uri(callBack), verifier));
        return reponse;
    }

    /// <summary>
    /// Initiates an asynchronous PKCE token refresh request using provided client ID and the current refresh token
    /// </summary>
    /// <param name="clientID">Spotify client ID</param>
    /// <param name="refreshToken">Current refresh token</param>
    /// <returns></returns>
    public static async Task<PKCETokenResponse> PKCETokenRefreshResponse(string clientID, string refreshToken)
    {
        PKCETokenResponse response = await new OAuthClient().RequestToken(new PKCETokenRefreshRequest(clientID, refreshToken));
        return response;
    }
}