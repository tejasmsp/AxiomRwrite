//-----------------------------------------------------------------------
// <copyright file="ProjectSession.cs" company="Premiere Digital Services">
//     Copyright Premiere Digital Services. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Axiom.Entity;
using System;
using System.Collections.Generic;
using System.Web;


public class LoggedInUserDetail
{
    public int? UserAccessId { get; set; }

    public string Email { get; set; }

    public string UserId { get; set; }

    public string EmpId { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public int? InvalidLogin { get; set; }

    public int? FailedPasswordAttemptCount { get; set; }
    /// <summary>
    /// Gets or sets the image.
    /// </summary>
    /// <value>
    /// The image.
    /// </value>
    public byte[] Image { get; set; }

    /// <summary>
    /// Gets or sets the roles.
    /// </summary>
    /// <value>
    /// The roles.
    /// </value>
    public IEnumerable<string> Roles { get; set; }

    public string LoginUserUniqueKey { get; set; }

    public int? AuthenticationFlag { get; set; }

    public List<RoleRightsEntity> Permissions { get; set; }

    public IList<string> RoleName { get; set; }

    public bool? IsAdmin { get; set; }
}


/// <summary>
/// Class ProjectSession.
/// </summary>
public class ProjectSession
{
    #region "Properties"
    /// <summary>
    /// Gets or sets the logged in user detail.
    /// </summary>
    /// <value>
    /// The logged in user detail.
    /// </value>
    public static LoggedInUserDetail LoggedInUserDetail
    {
        get
        {
            if (HttpContext.Current.Session["LoggedInUserDetail"] == null)
            {
                return null;
            }

            return HttpContext.Current.Session["LoggedInUserDetail"] as LoggedInUserDetail;
        }

        set
        {
            HttpContext.Current.Session["LoggedInUserDetail"] = value;
        }
    }

    

    /// <summary>
    /// Gets or sets the exception.
    /// </summary>
    /// <value>
    /// The exception.
    /// </value>
    public static Exception Exception
    {
        get
        {
            if (HttpContext.Current.Session["Exception"] == null)
            {
                return null;
            }

            return HttpContext.Current.Session["Exception"] as Exception;
        }

        set
        {
            HttpContext.Current.Session["Exception"] = value;
        }
    }
    /// <summary>
    /// Gets or sets the user network user id.
    /// </summary>
    /// <value>
    /// The logged in Network UserId.
    /// </value>
    public static string NetworkUserId
    {
        get
        {
            if (HttpContext.Current.Session["NetworkUserId"] == null)
            {
                return null;
            }

            return Convert.ToString(HttpContext.Current.Session["NetworkUserId"]);
        }

        set
        {
            HttpContext.Current.Session["NetworkUserId"] = value;
        }
    }
    #endregion
}
