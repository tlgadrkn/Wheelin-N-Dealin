// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
// IdentityUser class is coming from ASP.NET Identity not the IdentityService. 
// IdentityServer is a library that provides authentication and authorization services, IdentityProvider is handling issuing tokens and validating them. etc.
// ASP.NET Identity is a user store and user management system.
public class ApplicationUser : IdentityUser
{
}
