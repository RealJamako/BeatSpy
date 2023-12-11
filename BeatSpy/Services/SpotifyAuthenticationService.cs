﻿using System;
using System.IO;
using System.Net;
using BeatSpy.Models;
using SpotifyAPI.Web;
using BeatSpy.Helpers;
using System.Threading.Tasks;
using BeatSpy.DataTypes.Constants;

namespace BeatSpy.Services;

internal class SpotifyAuthenticationService : ISpotifyAuthenticationService
{
    private const string CLIENT_ID = "4b675b489cf74ff2a4a2d4483cf1dbe1";
    private const string CALL_BACK = "http://localhost:5543/callback";

    public async Task LogIn()
    {
        var (verifier, challenge) = PKCEUtil.GenerateCodes(120);

        LoginRequest loginRequest = new(new Uri(CALL_BACK), CLIENT_ID, LoginRequest.ResponseType.Code)
        {
            CodeChallengeMethod = "S256",
            CodeChallenge = challenge,
        };

        BrowserHelper.OpenURLInBrowser(loginRequest.ToUri().AbsoluteUri);

        await StartServiceCallbackListener(verifier);
    }

    public async Task<SpotifyClient> Connect()
    {
        if (AuthenticationHelper.TryGetAuthFile(out string tokenContents))
        {
            SpotifyToken? spotifyToken = AuthenticationHelper.DeserializeTokenContent(tokenContents);
            if (spotifyToken != null)
            {
                PKCETokenResponse tokenReponse = await AuthenticationHelper.PKCETokenRefreshResponse(CLIENT_ID, spotifyToken.RefreshToken!);

                //Update the refresh token on the spotify token,
                //Serialize the token content and save to the auth file
                spotifyToken.RefreshToken = tokenReponse.RefreshToken;
                AuthenticationHelper.SerializeTokenContent(spotifyToken);

                //Return a spotify client
                return new SpotifyClient(tokenReponse.AccessToken);
            }
            else
            {
                throw new FileFormatException(LogConstants.AUTH_FILE_CONTENTS_ERROR);
            }
        }
        else
        {
            throw new FileNotFoundException(LogConstants.AUTH_FILE_NOTFOUND);
        }
    }

    public void LogOut()
    {
        File.Delete(DataFolderHelper.GetFullDataPath(FileConstants.AUTH_FILE));
    }

    private async Task StartServiceCallbackListener(string verifier)
    {
        using HttpListener serviceListener = new();
        serviceListener.Prefixes.Add(string.Concat(CALL_BACK, "/"));

        serviceListener.Start();

        while (true)
        {
            HttpListenerContext ctx = serviceListener.GetContext();
            if (ctx != null)
            {
                string requestQueryCode = ctx.Request.QueryString.Get("code") ?? throw new Exception(LogConstants.SERVER_REQUEST_QUERY_ERROR);
                await GetCallBack(requestQueryCode, verifier);
                serviceListener.Stop();
                break;
            }
        }
    }

    private async Task GetCallBack(string code, string verifier)
    {
        PKCETokenResponse response = await AuthenticationHelper.PKCETokenRequestResponse(CLIENT_ID, CALL_BACK, code, verifier);

        if (response != null)
        {
            AuthenticationHelper.SerializeTokenContent(new SpotifyToken()
            {
                RefreshToken = response.RefreshToken,
            });
        }
        else
        {
            //TODO: throw some sort of error here
            throw new NotImplementedException();
        }
    }
}