# What is Identity Server?

Identity Server is a framework that allows you to secure your web applications and APIs using OpenID Connect and OAuth 2.0. It is built on top of the .NET Core framework and is designed to be easy to use and configure.

Unlike other methods of Single Sign-On, like Facebook, Google, Azure Active Directory etc. Identity Server is designed to be a customizable solution so that you have full controle over all aspects of the authentication process.

- No longer open source, license required for prod
- Identity Server is a Single Sign-On solution that allows you to authenticate users across multiple applications and services, typically you would not use Identity Server to authenticate users for a single application.

## Single Sign-On (SSO)

Single Sign-On is a method of authentication that allows users to log in to multiple applications and services using a single set of credentials. This is useful because it allows users to access all of their applications and services without having to log in to each one individually. Users usually get tired of having to remember multiple passwords for different applications and services, so SSO is a great way to simplify the authentication process.

## Oauth 2.0

OAuth 2.0 is an open standard for access delegation, commonly used as a way for Internet users to grant websites or applications access to their information on other websites but without giving them the passwords. This is done by authorizing the application to access the user's information on the other website, without revealing the user's password.

This is a security standard where we give one app permission to access data in another application. So instead of giving username and passowrd to the app, we give the app a `token`\ `key` that gives permission to access the data or do things on our behalf in another application. And steps taking to grant permission is called `authorization`. We authorize one app to access our data or use features in another app on our behalf without giving them our password. And we can take back the permission at any time. Whenever we try to login with google or facebook, thats what we are doing. We are authorizing the app to access our data in google or facebook on our behalf. So this is the standard of `OAuth 2.0.`

The flow would in the case of our app would be like this:

- User clicks the login button in our app, because they want to do some action that requires them to be authenticated.
- User is redirected to the Identity Server login page, where they can enter their credentials.
- They login and Identity server is going to check if they're logged in already, if yes they may not even see the login page and be redirected back to our app.
- After they logged in, IS is going to give them a token, which is a key that they can use to access our app, so our ap on behalf of the user can then proceed and use the functionality that required authentication.

Some Terminology to understand what's going on under the hood:

- `Resource Owner`: The user who owns the data that the client wants to access, you're the owner of your data, and any actions that can be performed on your behalf.
- `Client`: The application that wants to access the user's data. This could be a web app, mobile app, or desktop app, wants to acess the user's data to do actions on behalf of the user (resource owner). So after you've logged in to Identity Server in our example, our client will then be given a key so it can go and authenticate to our other services.
  So we have an authorization server.
- `Authorization Server`: (Facebook login, google login, apple, github, Identity Server etc.) The server that issues access tokens to the client after successfully authenticating the resource owner and obtaining authorization. This is the Identity Server in our case. This could be a third party service that the `Resource Server` trusts to authenticate users.
- `Resource Server`: The server that hosts the user's data. This could be a web API, a database, or any other service that the client wants to access. This is our API that we want to access after we've logged in.
- We also have a `Redirect URI` which is the URL that the client will be redirected to after successfully authenticating the user. This is the URL that the client will use to access the user's data. The auth server (identity) will redirect the `resource owner ` back to the client with the token after granting permission.
- ALso the `Response type` which is the type of response, information that the client expects from the authorization server. This could be a token, a code, or some other type of response. In our case, it's going to be a token.
- We also have the `Scope` which is the permissions that the client is requesting from the authorization server. This could be `read`, `create`, `delete`, `write`, or some other type of permission.
- `Consent Form`: This is a form that the user must fill out before the client can access their data. This is to ensure that the user is aware of what data the client is requesting and what permissions they are granting. This is the form that you see when you log in to an app using Facebook or Google, where you have to grant permission for the app to access your data. This is optional and we wont implement it in our app because we're not sharing the users data.
- `Client Id`: This is a unique identifier that the client uses to identify itself to the authorization server. This is like a username for the client, so the auth server knows who is making the request.
- `Client Secret`: This is a secret key that the client uses to authenticate itself to the authorization server. This is like a password for the client, so the auth server knows that the client is who it says it is.
- `Authorization Code`: This is a temporary code that the client uses to exchange for an access token. This is used to prevent replay attacks and to ensure that the client is who it says it is. This is the code that the client will use to get the token from the auth server. `The client will send this code to the auth server and the auth server will send back the access token.`
- `Access Token`: This is a token that the client uses to access the user's data. This is like a key that the client uses to access the user's data. This is the token that the client will use to access the API after it has been authenticated. Access token gives permission to access data, perform actions to the client. This is the token that the client will use to access the API after it has been authenticated.
- `Flow`: The flow is the sequence of steps that the client must take to authenticate the user and obtain an access token. This could be a code flow, a token flow, or some other type of flow. In our case, it's going to be a code flow.

In our case the flow will be OAuth 2.0 Authorization Code Flow.

- So first of all, we want the client to access some information about you that is contained in the `Authorization Server` (Identity Server). We're going to use identity server to store our user accounts.
- So the client then redirects our browser to the `Authorization Server` (Identity Server) and that includes with the request the `client id`, `redirect uri`, `response type`,one or more `scope` it needs etc.
- The `Authorization Server` then verifies who you are, and if necessary prompts for a login. Maybe you have a session on identity server,and if you do have a session it may not even prompt you for a login and just redirect you back to the client with the `token`.
- possibly you could have a `consent form` based on the `scopes` that the client is requesting, but we're not going to implement that.
  -Next the temporary `authorization code` is returned to the client browser, which then uses the call back url provided by Authorization server `redirect uri` to the `client app server`

1. The `Client` then contacts the `Authorization Server` directly it does not use resource owner browser, it directly contacts the `authorization server` and securely sends the `authorization code`,`client id`, `client secret` to the `Authorization Server`.
2. And then the `Authorization Server` then validates the `authorization code`, `client id`, `client secret` and if everything is correct, it then sends back the `access token` to the `client`, it's not going to send it to the browser, it's going to send it to the `client app server` and retained there. The client app server not the users browser!. This is a private communication between the `client app server` and the `authorization server` and the client browser is not involved in this communication.
3. It's just gibberish string that is sent to the resource server when requesting data. So our client app then can send this token to the auction service for example, and ask it to give it some stuff, such as a list of auctions, for example. And behind the scenes the auction service will verify this is a valid token and if the token is valid, then it returns the data.

Now, before any of this stuff takes place, the authorization Server Identity server establishes a

relationship with the client, and the authorization server generates a client ID and client secret

and gives them to the client to use for all future auth users.

This secret must be kept secret and only the client app and the authorization server know what this

is and this is how the authorization server can then verify the client's.

Now we also have another standard on top of OAuth two called OpenID Connect.

OAuth 2.0 is designed only for authorization and granting access to data.

And OAuth is like giving an application a key.

It's useful, but it doesn't tell the client who you are or anything about you.

And OpenID Connect or Oidc sits on top of OAuth two that adds additional functionality around login

and profile information about the person who is logged in.

It doesn't just give the client permissions, but it also includes some basic information about who

you are.

And when an authorization server supports Oidc, it's often referred to as an identity provider since

it provides information about the resource owner that's you, the user, back to the client, which

is the server running our client application.

And most applications that you see out there will use.

So that uses this flow such as Facebook, such as Apple, such as Google and other social networking

services so that users can use something they already have rather than signing up every time when they

encounter a new application and use its functionality.

So that's a very, very brief overview of OpenID Connect and OAuth 2.0.

## Configuration

### Api Scopes

- `Api Scopes` are a way to define the permissions that a client has to access an API. This is a way to control what a client can do with the API. For example, you might have a scope that allows a client to read data from the API, or a scope that allows a client to write data to the API. This is a way to control what a client can do with the API and to ensure that the client only has access to the data that it needs.

API Scopes define the permissions that clients can request when accessing APIs. They represent the level of access that a client application can have. In this configuration, there is one API scope defined:

in my app we have this scope:

`auctionApp`: This scope represents full access to the Auction application. When a client requests this scope, it is asking for permission to perform any action within the Auction application.

### Clients

Clients are applications that request tokens from the IdentityServer. Each client has a unique configuration that specifies how it can authenticate and what resources it can access. In this configuration, there are two clients defined:

a. Client Credentials Client
This client uses the Client Credentials flow, which is typically used for machine-to-machine communication where no user is involved. Here are the details:

- ClientId: "m2m.client"

This is the unique identifier for the client.
ClientName: "Client Credentials Client"
A descriptive name for the client.
AllowedGrantTypes: GrantTypes.ClientCredentials
Specifies that this client uses the Client Credentials flow.
ClientSecrets: { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) }
The secret used to authenticate the client. It is hashed using SHA-256 for security.
AllowedScopes: { "scope1" }
Specifies the scopes that this client is allowed to request. In this case, it can request the "scope1" scope.
b. Interactive Client
This client uses the Authorization Code flow with PKCE (Proof Key for Code Exchange), which is typically used for user authentication. Here are the details:

- ClientId: "interactive"

This is the unique identifier for the client.
ClientSecrets: { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) }
The secret used to authenticate the client. It is hashed using SHA-256 for security.
AllowedGrantTypes: GrantTypes.Code
Specifies that this client uses the Authorization Code flow with PKCE.
RedirectUris: { "https://localhost:44300/signin-oidc" }
The URI to which the authorization server will redirect the user after they have authenticated.
FrontChannelLogoutUri: "https://localhost:44300/signout-oidc"
The URI to which the authorization server will redirect the user after they have logged out.
PostLogoutRedirectUris: { "https://localhost:44300/signout-callback-oidc" }
The URI to which the authorization server will redirect the user after the logout process is complete.
AllowOfflineAccess: true
Allows the client to request refresh tokens, which can be used to obtain new access tokens without requiring the user to log in again.

## AUTHENTICATION FLOW

1. Client Authentication
   Step 1: Client Requests Token

Client: Postman
Action: Postman sends a request to the IdentityServer's token endpoint (/connect/token) to obtain an access token.
Details:
URL: https://localhost:5001/connect/token
HTTP Method: POST
Headers: Content-Type: application/x-www-form-urlencoded
Body:
grant_type: password
client_id: postmanClient
client_secret: postmanSecret
username: <user's username>
password: <user's password>
scope: auctionApp openid profile
Step 2: IdentityServer Validates Client Credentials

IdentityServer: Receives the token request and validates the client's credentials (client_id and client_secret).
Details:
ClientId: postmanClient
ClientSecret: postmanSecret (hashed using SHA-256)
Step 3: IdentityServer Validates User Credentials

IdentityServer: Validates the user's credentials (username and password).
Details:
Username: <user's username>
Password: <user's password> 2. Token Issuance
Step 4: IdentityServer Issues Tokens

IdentityServer: If both the client and user credentials are valid, the IdentityServer issues an access token and an ID token (if requested).
Details:
Access Token: Contains information about the client, user, and the scopes granted.
ID Token: Contains identity information about the user (if openid scope is requested).
Step 5: IdentityServer Responds with Tokens

IdentityServer: Sends a response back to the client (Postman) with the issued tokens.
Details:
Response Body:
access_token: <JWT access token>
id_token: <JWT ID token> (if openid scope is requested)
token_type: Bearer
expires_in: <expiration time in seconds>

3. Accessing the API
   Step 6: Client Uses Access Token

Client: Postman
Action: Postman uses the access token to authenticate requests to the Auction application API.
Details:
URL: https://localhost:5001/api/auctions
HTTP Method: GET (or any other method)
Headers:
Authorization: Bearer <access_token>
Step 7: API Validates Access Token

API: The Auction application API receives the request and validates the access token.
Details:
Access Token: The API verifies the token's signature, expiration, and scopes.
Step 8: API Processes Request

API: If the access token is valid, the API processes the request and returns the appropriate response.
Details:
Response: The API returns the requested data or performs the requested action.
Detailed Flow Diagram
Client Requests Token:

Postman sends a POST request to /connect/token with client credentials, user credentials, and requested scopes.
IdentityServer Validates Credentials:

IdentityServer validates the client_id and client_secret.
IdentityServer validates the username and password.
IdentityServer Issues Tokens:

If credentials are valid, IdentityServer issues an access token and an ID token (if requested).
Client Receives Tokens:

Postman receives the tokens in the response.
Client Uses Access Token:

Postman sends a request to the Auction application API with the access token in the Authorization header.
API Validates Access Token:

The API validates the access token's signature, expiration, and scopes.
API Processes Request:

If the token is valid, the API processes the request and returns the response.
Security Considerations
Client Secrets: The client secret is hashed using SHA-256 for security. In a production environment, secrets should be stored securely and not hard-coded in the configuration.
Grant Types: The Resource Owner Password flow is used here for simplicity in development. In production, more secure flows like Authorization Code with PKCE should be used for public clients.
Scopes: The allowed scopes should be limited to the minimum required for the client. In this case, the client has full access to the Auction application and user identity information.
By following this flow, we ensure that only authenticated clients can access the API and that they have the appropriate permissions based on the requested scopes. This setup provides a robust foundation for secure communication between clients and APIs in a microservices architecture.

https://docs.duendesoftware.com/identityserver/v7/tokens/authentication/jwt/
