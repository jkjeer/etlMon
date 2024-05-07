using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using etlMon.Common.Types.Enterprise;
using etlMon.Common.Types.Response;
//using Microsoft.PowerBI.Api.Models;
using NLog;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Linq;
using etlMon.Common.Utilities;
using System.Text;
using System.Security.Policy;
using System.Web;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace etlMon.Common.Managers
{
    public class ReportManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ProcessResponse PublishFile(string folderName, string fileName, string workspaceNameTarget, string reportNameTarget, string parameterName, string newValue, string email)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Publish File");
            string decodedUrl = "";
            try
            {
                newValue = newValue.ToUpper();
                var oneDrivePath = ConfigurationManager.AppSettings["PublishFileOneDrivePath"];
                var ps1FilePath = ConfigurationManager.AppSettings["PublishFilePowerShellScriptPath"];
                var ps1FileName = ConfigurationManager.AppSettings["PublishFilePowerShellScriptFileName"];
                var ps1File = $@"{ps1FilePath}\{ps1FileName}";

                decodedUrl = $@"{oneDrivePath}\{folderName}\{fileName}";

                RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
                Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
                runspace.Open();

                PowerShell ps = PowerShell.Create();
                ps.Runspace = runspace;
                ps.AddCommand(ps1File);

                ps.AddArgument(decodedUrl);
                ps.AddArgument(workspaceNameTarget);
                ps.AddArgument(reportNameTarget);
                ps.AddArgument(parameterName);
                ps.AddArgument(newValue);

                Collection<PSObject> psObjects;
                psObjects = ps.Invoke();

                runspace.Close();

                foreach (var obj in psObjects)
                {
                    var bo = obj.BaseObject;
                    switch (bo.ToString())
                    {
                        case "Microsoft.PowerBI.Common.Abstractions.PowerBIProfile":
                            break;
                        case "Microsoft.PowerBI.Common.Api.Workspaces.Workspace":
                            break;
                        case "Microsoft.PowerBI.Common.Api.Reports.Report":
                            break;
                        case "Microsoft.PowerBI.Common.Api.Shared.Datasource":
                            break;
                        case "Microsoft.PowerBI.Common.Api.Shared.DatasourceConnectionDetails":
                            break;
                        case "FilePath:":
                            break;
                        case "DatasourceSource:":
                            break;
                        case "WorkspaceTarget:":
                            break;
                        case "ReportTarget:":
                            break;
                        case "Before:":
                            break;
                        case "":
                            break;
                        case "After:":
                            break;
                        default:
                            var step = new ProcessStep(bo.ToString());
                            step.Done();
                            response.ProcessSteps.Add(step);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            var endTime = DateTime.Now.ToLocalTime();
            var headerSummary = new
            {
                StartDate = startTime,
                StartDateString = startTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                EndDate = endTime,
                EndDateString = endTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                ElapsedSeconds = (decimal)(endTime - startTime).TotalSeconds,
                FilePath = decodedUrl,
                FolderName = folderName,
                FileName = fileName,
                WorkspaceNameTarget = workspaceNameTarget,
                ReportNameTarget = reportNameTarget,
                ParameterName = parameterName,
                NewValue = newValue,
                URL = $@"http://cgd2intsvcqa/BIServices/BIServices.ReportRestService.svc/PublishFile?FolderName={folderName}&FileName={fileName}&WorkspaceNameTarget={workspaceNameTarget}&ReportNameTarget={reportNameTarget}&ParameterName={parameterName}&NewValue={newValue}SQLSRV03".Replace(" ", "%20"),
                Script = $@"powershell -Command ''C:\inetpub\wwwroot\BIServices\PowerShell\PublishReport.ps1 -WorkspaceNameSource '{folderName}' -ReportNameSource '{fileName}' -WorkspaceNameTarget '{workspaceNameTarget}' -ReportNameTarget '{reportNameTarget}' -ParameterName '{parameterName}' -NewValue '{newValue}SQLSRV03'''"
            };
            response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));
            NotifyPublishFile(response, email);
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");

            return response;
        }

        public static void NotifyPublishFile(ProcessResponse response, string email)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["PublishFileMailConfirmToAddress"];
                if (!string.IsNullOrEmpty(email)) toConfigAddresses += $@",{email}";
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "etlMon.Common.Xslt.PublishFileResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");
                var subject = @"BI Publish File";
                if (response.HasErrors()) subject = $@"Error - {subject}";
                Mail.SendHtml(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toConfigAddresses,
                    null,
                    subject,
                    xslOutput);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }

        public ProcessResponse PublishReport(string workspaceNameSource, string reportNameSource, string workspaceNameTarget, string reportNameTarget, string parameterName, string newValue, string email)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Publish Report");
            try
            {
                newValue = newValue.ToUpper();
                var ps1FilePath = ConfigurationManager.AppSettings["PublishReportPowerShellScriptPath"];
                var ps1FileName = ConfigurationManager.AppSettings["PublishReportPowerShellScriptFileName"];
                var ps1File = $@"{ps1FilePath}\{ps1FileName}";

                RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
                Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
                runspace.Open();

                PowerShell ps = PowerShell.Create();
                ps.Runspace = runspace;
                ps.AddCommand(ps1File);

                ps.AddArgument(workspaceNameSource);
                ps.AddArgument(reportNameSource);
                ps.AddArgument(workspaceNameTarget);
                ps.AddArgument(reportNameTarget);
                ps.AddArgument(parameterName);
                ps.AddArgument(newValue);

                Collection<PSObject> psObjects;
                psObjects = ps.Invoke();
                runspace.Close();

                foreach (var obj in psObjects)
                {
                    var bo = obj.BaseObject;
                    switch (bo.ToString())
                    {
                        case "Microsoft.PowerBI.Common.Abstractions.PowerBIProfile":
                            break;
                        case "Microsoft.PowerBI.Common.Api.Workspaces.Workspace":
                            break;
                        case "Microsoft.PowerBI.Common.Api.Reports.Report":
                            break;
                        case "Microsoft.PowerBI.Common.Api.Shared.Datasource":
                            break;
                        case "Microsoft.PowerBI.Common.Api.Shared.DatasourceConnectionDetails":
                            break;
                        case "WorkspaceSource:":
                            break;
                        case "ReportSource:":
                            break;
                        case "DatasourceSource:":
                            break;
                        case "WorkspaceTarget:":
                            break;
                        case "ReportTarget:":
                            break;
                        case "Before:":
                            break;
                        case "":
                            break;
                        case "After:":
                            break;
                        default:
                            var step = new ProcessStep(bo.ToString());
                            step.Done();
                            response.ProcessSteps.Add(step);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            var endTime = DateTime.Now.ToLocalTime();
            var headerSummary = new
            {
                StartDate = startTime,
                StartDateString = startTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                EndDate = endTime,
                EndDateString = endTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                ElapsedSeconds = (decimal)(endTime - startTime).TotalSeconds,
                WorkspaceNameSource = workspaceNameSource,
                ReportNameSource = reportNameSource,
                WorkspaceNameTarget = workspaceNameTarget,
                ReportNameTarget = reportNameTarget,
                ParameterName = parameterName,
                NewValue = newValue,
                URL = $@"http://cgd2intsvcqa/BIServices/BIServices.ReportRestService.svc/PublishReport?WorkspaceNameSource={workspaceNameSource}&ReportNameSource={reportNameSource}&WorkspaceNameTarget={workspaceNameTarget}&ReportNameTarget={reportNameTarget}&ParameterName={parameterName}&NewValue={newValue}".Replace(" ", "%20"),
                Script = $@"powershell -Command ""C:\inetpub\wwwroot\BIServices\PowerShell\PublishReport.ps1 -WorkspaceNameSource '{workspaceNameSource}' -ReportNameSource '{reportNameSource}' -WorkspaceNameTarget '{workspaceNameTarget}' -ReportNameTarget '{reportNameTarget}' -ParameterName '{parameterName}' -NewValue '{newValue}'"""
            };
            response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));
            NotifyPublishReport(response, email);
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");

            return response;
        }

        public static void NotifyPublishReport(ProcessResponse response, string email)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["PublishReportMailConfirmToAddress"];
                if (!string.IsNullOrEmpty(email)) toConfigAddresses += $@",{email}";
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "etlMon.Common.Xslt.PublishReportResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");
                var subject = @"BI Publish Report";
                if (response.HasErrors()) subject = $@"Error - {subject}";
                Mail.SendHtml(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toConfigAddresses,
                    null,
                    subject,
                    xslOutput);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }

        public ProcessResponse UpdateReportParameter(string workspaceName, string reportName, string parameterName, string newValue, string email)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"BEGIN: {DateTime.Now.ToLocalTime()}");
            var response = new ProcessResponse($"Update Report Parameter");
            try
            {
                newValue = newValue.ToUpper();
                var ps1FilePath = ConfigurationManager.AppSettings["ReportUpdatePowerShellScriptPath"];
                var ps1FileName = ConfigurationManager.AppSettings["ReportUpdatePowerShellScriptFileName"];
                var ps1File = $@"{ps1FilePath}\{ps1FileName}";

                RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
                Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
                runspace.Open();

                PowerShell ps = PowerShell.Create();
                ps.Runspace = runspace;
                ps.AddCommand(ps1File);

                ps.AddArgument(workspaceName);
                ps.AddArgument(reportName);
                ps.AddArgument(parameterName);
                ps.AddArgument(newValue);

                Collection<PSObject> psObjects;
                psObjects = ps.Invoke();
                runspace.Close();

                foreach (var obj in psObjects)
                {
                    var bo = obj.BaseObject;
                    switch (bo.ToString())
                    {
                        case "Microsoft.PowerBI.Common.Abstractions.PowerBIProfile":
                            break;
                        case "Microsoft.PowerBI.Common.Api.Workspaces.Workspace":
                            break;
                        case "Microsoft.PowerBI.Common.Api.Reports.Report":
                            break;
                        case "Microsoft.PowerBI.Common.Api.Shared.Datasource":
                            break;
                        case "Microsoft.PowerBI.Common.Api.Shared.DatasourceConnectionDetails":
                            break;
                        case "Workspace:":
                            break;
                        case "Report:":
                            break;
                        case "Datasource:":
                            break;
                        case "Before:":
                            break;
                        case "":
                            break;
                        case "After:":
                            break;
                        default:
                            var step = new ProcessStep(bo.ToString());
                            step.Done();
                            response.ProcessSteps.Add(step);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(new Error(e));
            }
            response.Close();
            var endTime = DateTime.Now.ToLocalTime();
            var headerSummary = new
            {
                StartDate = startTime,
                StartDateString = startTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                EndDate = endTime,
                EndDateString = endTime.ToString("M/dd/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US")),
                ElapsedSeconds = (decimal)(endTime - startTime).TotalSeconds,
                WorkspaceName = workspaceName,
                ReportName = reportName,
                ParameterName = parameterName,
                NewValue = newValue,
                URL = $@"http://cgd2intsvcqa/BIServices/BIServices.ReportRestService.svc/UpdateReportParameter?WorkspaceNameSource={workspaceName}&ReportNameSource={reportName}&ParameterName={parameterName}&NewValue={newValue}SQLSRV03".Replace(" ", "%20"),
                Script = $@"powershell -Command ''C:\inetpub\wwwroot\BIServices\PowerShell\UpdateReportParameter.ps1 -WorkspaceNameSource '{workspaceName}' -ReportName '{reportName}' -ParameterName '{parameterName}' -NewValue '{newValue}SQLSRV03'''"
            };
            response.SerializedObjects.Add(new SerializedObject("SummaryNode", XmlSerializationUtility.SerializeAnonymousType(headerSummary, "Header")));
            NotifyUpdateReportParameter(response, email);
            Logger.Info("END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");

            return response;
        }

        public static void NotifyUpdateReportParameter(ProcessResponse response, string email)
        {
            var stepSendMail = new ProcessStep("Send Mail");
            try
            {
                var toConfigAddresses = ConfigurationManager.AppSettings["ReportUpdateMailConfirmToAddress"];
                if (!string.IsNullOrEmpty(email)) toConfigAddresses += $@",{email}";
                var serializedXml = response.Serialize();
                Logger.Info($@"Serialized Process: {serializedXml}");
                var xslOutput = XsltManager.Transform(serializedXml, "etlMon.Common.Xslt.UpdateReportParameterResponseBody.xslt", Encoding.UTF8);
                Logger.Info($@"Report XML: {xslOutput}");
                var subject = @"BI Update Report Parameter";
                if (response.HasErrors()) subject = $@"Error - {subject}";
                Mail.SendHtml(ConfigurationManager.AppSettings["SmtpHost"],
                    ConfigurationManager.AppSettings["MailFromAddress"],
                    toConfigAddresses,
                    null,
                    subject,
                    xslOutput);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                stepSendMail.Errors.Add(new Error(e));
            }
            finally
            {
                stepSendMail.Done();
                response.ProcessSteps.Add(stepSendMail);
            }
        }
    }
}