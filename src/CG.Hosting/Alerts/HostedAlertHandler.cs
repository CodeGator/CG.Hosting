using CG.Alerts;
using CG.Email;
using CG.Hosting.Options;
using CG.Validations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;

namespace CG.Hosting.Alerts
{
    /// <summary>
    /// This class is a hosted implementation of the <see cref="IAlertHandler"/>
    /// interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The idea, with this class, is to create a set of handlers that are able
    /// to use the services of the associated <see cref="IHost"/> object to do
    /// more useful things when processing alerts. In this case, these handlers
    /// process alerts like this:
    /// <list type = "table" >
    /// <listheader>
    /// <term>Alert Type</term>
    /// <description>Processing Action</description>
    /// </listheader>
    /// <item>
    /// <term>Audit</term>
    /// <description>Writes to the console window.</description>
    /// </item>
    /// <item>
    /// <term>Information</term>
    /// <description>Logs, writes to the console window.</description>
    /// </item>
    /// <item>
    /// <term>Warning</term>
    /// <description>Logs and writes to the console window.</description>
    /// </item>
    /// <item>
    /// <term>Error</term>
    /// <description>Logs, sends email, writes to the console window.</description>
    /// </item>
    /// <item>
    /// <term>Critical</term>
    /// <description>Logs, sends email, sends a text, writes to the console window.</description>
    /// </item>
    /// <item>
    /// <term>Debug</term>
    /// <description>Writes to the debug console.</description>
    /// </item>
    /// <item>
    /// <term>Trace</term>
    /// <description>Logs, writes to the console window.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    public class HostedAlertHandler<TOptions> : DefaultAlertHandler, IAlertHandler
        where TOptions : StandardOptions, new()
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a reference to the current host.
        /// </summary>
        protected IHost Host { get; set; }

        /// <summary>
        /// This property contains a collection of named replacement tokens,
        /// for rendering email templates at runtime.
        /// </summary>
        protected IDictionary<string, string> Tokens { get; set; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="HostedAlertHandler{TOptions}"/>
        /// class.
        /// </summary>
        /// <param name="host">The host to use with this handler.</param>
        public HostedAlertHandler(
            IHost host
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(host, nameof(host));

            // Save the references.
            Host = host;

            // Create the fixed replacement tokens whose values will never change.
            Tokens = new Dictionary<string, string>()
            {
                { "%APP%", AppDomain.CurrentDomain.FriendlyNameEx(true) },
                { "%MN%", Environment.MachineName },
                { "%USER%",  Environment.UserName }
            };
        }

        #endregion

        // *******************************************************************
        // Protected methods.
        // *******************************************************************

        #region Protected methods

        /// <inheritdoc />
        protected override void HandleInformation(
            IDictionary<string, object> args,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(args, nameof(args));

            // This handler processes information alerts by: 
            //   (1) logging the alert.

            var handled = false;

            // Look for the standard host options.
            var options = Host.Services.GetService<IOptions<TOptions>>();

            // Did we find them?
            if (null != options)
            {
                // Are alerts configured?
                if (null != options.Value.Alerts)
                {
                    // Are information alerts configured?
                    if (null != options.Value.Alerts.InformationAlerts)
                    {
                        // Look for a logger on the host.
                        var logger = Host.Services.GetService<ILogger<Alert>>();

                        // Did we find one?
                        if (null != logger)
                        {
                            try
                            {
                                // Log the alert.
                                if (args.ContainsKey("ex"))
                                {
                                    logger.LogInformation(
                                        args["message"] as string,
                                        args["ex"] as Exception
                                        );
                                }
                                else
                                {
                                    logger.LogInformation(
                                        args["message"] as string
                                        );
                                }

                                // We handled the event.
                                handled = true;
                            }
                            catch (Exception ex)
                            {
                                // If we get here then we encountered an error while
                                //   we were in the process of handling the original
                                //   information. 

                                // Create new args for this error.
                                var newArgs = new Dictionary<string, object>()
                                {
                                    { "message", $"Failed to log an information event in the hosted handler! Error: {ex.Message}" }
                                };

                                // Report that we failed to handle the original alert.
                                base.HandleError(
                                    newArgs
                                    );
                            }
                        }
                    }
                }
            }

            // Did we fail to handle the event?
            if (false == handled)
            {
                // If we get here then we failed to handle the alert, so,
                //   we'll give the base class a chance at it.

                // Give the base class a chance.
                base.HandleError(
                    args,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber
                    );
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        protected override void HandleWarning(
            IDictionary<string, object> args,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(args, nameof(args));

            var handled = false;

            // This handler processes warning alerts by: 
            //   (1) logging the alert.

            // Look for the standard host options.
            var options = Host.Services.GetService<IOptions<TOptions>>();

            // Did we find them?
            if (null != options)
            {
                // Are alerts configured?
                if (null != options.Value.Alerts)
                {
                    // Are warning alerts configured?
                    if (null != options.Value.Alerts.WarningAlerts)
                    {
                        // Look for a logger on the host.
                        var logger = Host.Services.GetService<ILogger<Alert>>();

                        // Did we find one?
                        if (null != logger)
                        {
                            try
                            {
                                // Log the alert.
                                if (args.ContainsKey("ex"))
                                {
                                    logger.LogWarning(
                                        args["message"] as string,
                                        args["ex"] as Exception
                                        );
                                }
                                else
                                {
                                    logger.LogWarning(
                                        args["message"] as string
                                        );
                                }

                                // We handled the event.
                                handled = true;
                            }
                            catch (Exception ex)
                            {
                                // If we get here then we encountered an error while
                                //   we were in the process of handling the original
                                //   warning. 

                                // Create new args for this error.
                                var newArgs = new Dictionary<string, object>()
                                {
                                    { "message", $"Failed to log a warning event in the hosted handler! Error: {ex.Message}" }
                                };

                                // Report that we failed to handle the original alert.
                                base.HandleError(
                                    newArgs
                                    );
                            }
                        }
                    }                    
                }
            }

            // Did we fail to handle the event?
            if (false == handled)
            {
                // If we get here then we failed to handle the alert, so,
                //   we'll give the base class a chance at it.

                // Give the base class a chance.
                base.HandleError(
                    args,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber
                    );
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        protected override void HandleError(
            IDictionary<string, object> args,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(args, nameof(args));

            var handled = false;

            // This handler processes error alerts by: 
            //   (1) logging the alert.
            //   (2) Emailing the alert.

            // Look for the standard host options.
            var options = Host.Services.GetService<IOptions<TOptions>>();

            // Did we find them?
            if (null != options)
            {
                // Are alerts configured?
                if (null != options.Value.Alerts)
                {
                    // Are error alerts configured?
                    if (null != options.Value.Alerts.ErrorAlerts)
                    {
                        // Look for a logger on the host.
                        var logger = Host.Services.GetService<ILogger<Alert>>();

                        // Did we find one?
                        if (null != logger)
                        {
                            try
                            {
                                // Log the alert.
                                if (args.ContainsKey("ex"))
                                {
                                    logger.LogError(
                                        args["message"] as string,
                                        args["ex"] as Exception
                                        );
                                }
                                else
                                {
                                    logger.LogError(
                                        args["message"] as string
                                        );
                                }
                            }
                            catch (Exception ex)
                            {
                                // If we get here then we encountered an error while
                                //   we were in the process of handling the original
                                //   warning. 

                                // Create new args for this error.
                                var newArgs = new Dictionary<string, object>()
                                {
                                    { "message", $"Failed to log an error event in the hosted handler! Error: {ex.Message}" }
                                };

                                // Report that we failed to handle the original alert.
                                base.HandleError(
                                    newArgs
                                    );
                            }
                        }
                    }
                    
                    // Are error emails configured?
                    if (null != options.Value.Alerts.ErrorAlerts.Email)
                    {
                        // Are error emails currently enabled?
                        if (options.Value.Alerts.ErrorAlerts.Email.Enabled)
                        {
                            // Look for an email service on the host.
                            var email = Host.Services.GetService<IEmailService>();

                            // Did we find one?
                            if (null != email)
                            {
                                try
                                {
                                    // Create replacement tokens.
                                    var tokens = new Dictionary<string, string>(Tokens)
                                    {
                                        { "%MSG%", args["message"] as string }
                                    };

                                    // Should we add the original exception?
                                    if (args.ContainsKey("ex"))
                                    {
                                        tokens["%EX%"] = (args["ex"] as Exception).Message;
                                    }

                                    // Render the body.
                                    var body = Template.Render(
                                        options.Value.Alerts.ErrorAlerts.Email.Body,
                                        tokens
                                        );

                                    // Render the subject.
                                    var subject = Template.Render(
                                        options.Value.Alerts.ErrorAlerts.Email.Subject,
                                        tokens
                                        );

                                    // Send the email.
                                    email.Send(
                                        options.Value.Alerts.ErrorAlerts.Email.From,
                                        options.Value.Alerts.ErrorAlerts.Email.To,
                                        subject,
                                        body,
                                        options.Value.Alerts.ErrorAlerts.Email.IsHtml
                                        );

                                    // We handled the event.
                                    handled = true;
                                }
                                catch (Exception ex)
                                {
                                    // If we get here then we encountered an error while
                                    //   we were in the process of handling the original
                                    //   warning. 

                                    // Create new args for this error.
                                    var newArgs = new Dictionary<string, object>()
                                    {
                                        { "message", $"Failed to email an error event in the hosted handler! Error: {ex.Message}" }
                                    };

                                    // Report that we failed to handle the original alert.
                                    base.HandleError(
                                        newArgs
                                        );
                                }
                            }
                        }
                    }
                } 
            }

            // Did we fail to handle the event?
            if (false == handled)
            {
                // If we get here then we failed to handle the alert, so,
                //   we'll give the base class a chance at it.

                // Give the base class a chance.
                base.HandleError(
                    args,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber
                    );
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        protected override void HandleCritical(
            IDictionary<string, object> args,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(args, nameof(args));

            var handled = false;

            // This handler processes critical alerts by: 
            //   (1) logging the alert.
            //   (2) Emailing the alert.

            // Look for the standard host options.
            var options = Host.Services.GetService<IOptions<TOptions>>();

            // Did we find them?
            if (null != options)
            {
                // Are alerts configured?
                if (null != options.Value.Alerts)
                {
                    // Are critical alerts configured?
                    if (null != options.Value.Alerts.CriticalAlerts)
                    {
                        // Look for a logger on the host.
                        var logger = Host.Services.GetService<ILogger<Alert>>();

                        // Did we find one?
                        if (null != logger)
                        {
                            try
                            {
                                // Log the alert.
                                if (args.ContainsKey("ex"))
                                {
                                    logger.LogCritical(
                                        args["message"] as string,
                                        args["ex"] as Exception
                                        );
                                }
                                else
                                {
                                    logger.LogCritical(
                                        args["message"] as string
                                        );
                                }
                            }
                            catch (Exception ex)
                            {
                                // If we get here then we encountered an error while
                                //   we were in the process of handling the original
                                //   warning. 

                                // Create new args for this error.
                                var newArgs = new Dictionary<string, object>()
                                {
                                    { "message", $"Failed to log a critical event in the hosted handler! Error: {ex.Message}" }
                                };

                                // Report that we failed to handle the original alert.
                                base.HandleError(
                                    newArgs
                                    );
                            }
                        }
                    }

                    // Are critical emails configured?
                    if (null != options.Value.Alerts.CriticalAlerts.Email)
                    {
                        // Are critical emails currently enabled?
                        if (options.Value.Alerts.CriticalAlerts.Email.Enabled)
                        {
                            // Look for an email service on the host.
                            var email = Host.Services.GetService<IEmailService>();

                            // Did we find one?
                            if (null != email)
                            {
                                try
                                {
                                    // Create replacement tokens.
                                    var tokens = new Dictionary<string, string>(Tokens)
                                    {
                                        { "%MSG%", args["message"] as string }
                                    };

                                    // Should we add the original exception?
                                    if (args.ContainsKey("ex"))
                                    {
                                        tokens["%EX%"] = (args["ex"] as Exception).Message;
                                    }

                                    // Render the body.
                                    var body = Template.Render(
                                        options.Value.Alerts.ErrorAlerts.Email.Body,
                                        tokens
                                        );

                                    // Render the subject.
                                    var subject = Template.Render(
                                        options.Value.Alerts.ErrorAlerts.Email.Subject,
                                        tokens
                                        );

                                    // Send the email.
                                    email.Send(
                                        options.Value.Alerts.ErrorAlerts.Email.From,
                                        options.Value.Alerts.ErrorAlerts.Email.To,
                                        subject,
                                        body,
                                        options.Value.Alerts.ErrorAlerts.Email.IsHtml
                                        );

                                    // We handled the event.
                                    handled = true;
                                }
                                catch (Exception ex)
                                {
                                    // If we get here then we encountered an error while
                                    //   we were in the process of handling the original
                                    //   warning. 

                                    // Create new args for this error.
                                    var newArgs = new Dictionary<string, object>()
                                    {
                                        { "message", $"Failed to email a critical event in the hosted handler! Error: {ex.Message}" }
                                    };

                                    // Report that we failed to handle the original alert.
                                    base.HandleError(
                                        newArgs
                                        );
                                }
                            }
                        }
                    }
                }
            }

            // Did we fail to handle the event?
            if (false == handled)
            {
                // If we get here then we failed to handle the alert, so,
                //   we'll give the base class a chance at it.

                // Give the base class a chance.
                base.HandleError(
                    args,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber
                    );
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        protected override void HandleDebug(
            IDictionary<string, object> args,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(args, nameof(args));

            var handled = false;

            // This handler processes debug alerts by: 
            //   (1) logging the alert.

            // Look for the standard host options.
            var options = Host.Services.GetService<IOptions<TOptions>>();

            // Did we find them?
            if (null != options)
            {
                // Are alerts configured?
                if (null != options.Value.Alerts)
                {
                    // Are debug alerts configured?
                    if (null != options.Value.Alerts.DebugAlerts)
                    {
                        // Look for a logger on the host.
                        var logger = Host.Services.GetService<ILogger<Alert>>();

                        // Did we find one?
                        if (null != logger)
                        {
                            try
                            {
                                // Log the alert.
                                logger.LogDebug(
                                    args["message"] as string
                                    );

                                // We handled the event.
                                handled = true;
                            }
                            catch (Exception ex)
                            {
                                // If we get here then we encountered an error while
                                //   we were in the process of handling the original
                                //   warning. 

                                // Create new args for this error.
                                var newArgs = new Dictionary<string, object>()
                                {
                                    { "message", $"Failed to log a debug event in the hosted handler! Error: {ex.Message}" }
                                };

                                // Report that we failed to handle the original alert.
                                base.HandleError(
                                    newArgs
                                    );
                            }
                        }
                    }
                }
            }

            // Did we fail to handle the event?
            if (false == handled)
            {
                // If we get here then we failed to handle the alert, so,
                //   we'll give the base class a chance at it.

                // Give the base class a chance.
                base.HandleError(
                    args,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber
                    );
            }
        }

        // *******************************************************************

        /// <inheritdoc />
        protected override void HandleAudit(
            IDictionary<string, object> args,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(args, nameof(args));

            // This handler processes audit alerts by: 
            //   (1) Writing the alert to the console.

            // Give the base class a chance.
            base.HandleAudit(
                args,
                memberName,
                sourceFilePath,
                sourceLineNumber
                );
        }

        // *******************************************************************

        /// <inheritdoc />
        protected override void HandleTrace(
            IDictionary<string, object> args,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(args, nameof(args));

            var handled = false;

            // This handler processes trace alerts by: 
            //   (1) logging the alert.

            // Look for the standard host options.
            var options = Host.Services.GetService<IOptions<TOptions>>();

            // Did we find them?
            if (null != options)
            {
                // Are alerts configured?
                if (null != options.Value.Alerts)
                {
                    // Are trace alerts configured?
                    if (null != options.Value.Alerts.TraceAlerts)
                    {
                        // Look for a logger on the host.
                        var logger = Host.Services.GetService<ILogger<Alert>>();

                        // Did we find one?
                        if (null != logger)
                        {
                            try
                            {
                                // Log the alert.
                                logger.LogTrace(
                                    args["message"] as string
                                    );

                                // We handled the event.
                                handled = true;
                            }
                            catch (Exception ex)
                            {
                                // If we get here then we encountered an error while
                                //   we were in the process of handling the original
                                //   warning. 

                                // Create new args for this error.
                                var newArgs = new Dictionary<string, object>()
                                {
                                    { "message", $"Failed to log a trace event in the hosted handler! Error: {ex.Message}" }
                                };

                                // Report that we failed to handle the original alert.
                                base.HandleError(
                                    newArgs
                                    );
                            }
                        }
                    }
                }
            }

            // Did we fail to handle the event?
            if (false == handled)
            {
                // If we get here then we failed to handle the alert, so,
                //   we'll give the base class a chance at it.

                // Give the base class a chance.
                base.HandleError(
                    args,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber
                    );
            }
        }

        #endregion
    }
}
