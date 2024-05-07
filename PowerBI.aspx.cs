using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using etlMon.Common.Managers;
using etlMon.Common.Managers.Enterprise;
using etlMon.Common.Types;
using etlMon.Common.Types.PowerBI;
using etlMon.Common.Utilities;
using NLog;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Linq;
using System.IO;
using File = etlMon.Common.Types.PowerBI.File;
using Newtonsoft.Json;

namespace etlMon
{
    public partial class PowerBI : System.Web.UI.Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static string _sortDirection = "ASC";
        DataTable dt = new DataTable();

        Color cAltRowColor = ColorTranslator.FromHtml("#FCCFFCC");
        Color cHeaderColor = ColorTranslator.FromHtml("#FFFCCCC");

        private static Workspaces workspaces = new Workspaces();
        private static Reports reports = new Reports();
        private static Folders folders = new Folders();
        private static Files files = new Files();

        protected void Page_Load(object sender, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"PageLoad BEGIN: {DateTime.Now.ToLocalTime()}");
            if (!IsPostBack) /* This is the first load of this page. */
            {
                try
                {
                    string sOperator = Request.LogonUserIdentity.Name;
                    if (Message1.Text.Length == 0) // This is the first load of this page so .
                    {
                        Message1.Text = sOperator + "&nbsp;&nbsp;&nbsp;&nbsp;" + Convert.ToString(DateTime.Now); //System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                    }
                    var operatorName = Request.LogonUserIdentity.Name.Replace("CGI\\", "");
                    Plant plant = GetOperatorPlant(operatorName);
                    workspaces = GetWorkspaces();
                    reports = GetReports(selWorkspaceNameSource.SelectedValue);
                    folders = GetFolders();
                    files = GetFiles(selFolderName.SelectedValue);
                    fnSelDivs(RadioButtonList1.SelectedValue);
                    if (divSelPublishFile.Visible) txtReportNameTarget.Text = selFileName.SelectedValue.Replace(".pbix", "");
                    if (divSelReportNameSource.Visible) txtReportNameTarget.Text = selReportNameSource.SelectedValue;
                }
                catch (Exception err)
                {
                    Logger.Error($"ERROR: {err.Message}: {err.StackTrace}");
                }
            }
            Logger.Info("PageLoad END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected Folders GetFolders()
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetFolders BEGIN: {DateTime.Now.ToLocalTime()}");

            Folders folders = new Folders();

            ClearGrid();

            string oneDrivePath = ConfigurationManager.AppSettings["PowerBIOneDrivePath"];
            string[] directories = System.IO.Directory.GetDirectories(oneDrivePath);
            foreach (string d in directories)
            {
                var folder = new Folder();
                folder.FolderName = System.IO.Path.GetFileNameWithoutExtension(d);
                folder.FolderValue = System.IO.Path.GetFileNameWithoutExtension(d);
                folders.Add(folder);
            }
            selFolderName.Items.Clear();
            selFolderName.DataTextField = "FolderName";
            selFolderName.DataValueField = "FolderValue";
            selFolderName.DataSource = folders;
            selFolderName.DataBind();

            Logger.Info($"Rows: {selFolderName.Items.Count}");
            Logger.Info("GetFolders END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");

            return folders;
        }


        protected Files GetFiles(string folderName)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetFiles BEGIN: {DateTime.Now.ToLocalTime()}");

            Files files = new Files();

            ClearGrid();

            string oneDrivePath = ConfigurationManager.AppSettings["PowerBIOneDrivePath"];
            string folderPath = $@"{oneDrivePath}\{folderName}";

            DirectoryInfo d = new DirectoryInfo(folderPath);
            FileInfo[] fileInfos = d.GetFiles("*.pbix");

            foreach (FileInfo fileInfo in fileInfos)
            {
                var file = new File();
                file.FileName = fileInfo.Name;
                file.FileValue = fileInfo.Name;
                files.Add(file);
            }

            selFileName.Items.Clear();
            selFileName.DataTextField = "FileName";
            selFileName.DataValueField = "FileValue";
            selFileName.DataSource = files;
            selFileName.DataBind();

            if (divSelPublishFile.Visible) txtReportNameTarget.Text = selFileName.SelectedValue.Replace(".pbix", "");

            Logger.Info($"Rows: {selFileName.Items.Count}");
            Logger.Info("GetFiles END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");

            return files;
        }

        protected Workspaces GetWorkspaces()
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetWorkspaces BEGIN: {DateTime.Now.ToLocalTime()}");

            Workspaces workspaces = new Workspaces();

            ClearGrid();

            RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
            Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();

            PowerShell ps = PowerShell.Create();
            ps.Runspace = runspace;

            var ps1FilePath = ConfigurationManager.AppSettings["PowerShellScriptPath"];
            var ps1File = $@"{ps1FilePath}\GetWorkspaces.ps1";
            ps.AddCommand(ps1File);
            Collection<PSObject> psObjects = ps.Invoke();
            runspace.Close();

            var outFile = $@"{ps1FilePath}\Workspaces.txt";
            StreamReader sr = new StreamReader(outFile);
            string line = sr.ReadLine().Trim();
            while (line != null)
            {
                line = line.Trim();
                if (line != "")
                {
                    var workspace = new Workspace();
                    workspace.WorkspaceName = line.Trim();
                    workspace.WorkspaceValue = line.Trim();
                    workspaces.Add(workspace);
                }
                line = sr.ReadLine();
            }
            sr.Close();

            selWorkspaceNameSource.Items.Clear();
            selWorkspaceNameSource.DataTextField = "WorkspaceName";
            selWorkspaceNameSource.DataValueField = "WorkspaceValue";
            selWorkspaceNameSource.DataSource = workspaces;
            selWorkspaceNameSource.DataBind();

            Workspaces workspacesTarget = workspaces;

            cblWorkspaces.Items.Clear();
            cblWorkspaces.DataTextField = "WorkspaceName";
            cblWorkspaces.DataValueField = "WorkspaceValue";
            cblWorkspaces.DataSource = workspacesTarget;
            cblWorkspaces.DataBind();

            Logger.Info($"Rows: {selWorkspaceNameSource.Items.Count}");
            Logger.Info("GetWorkspaces END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");

            return workspaces;
        }

        protected Reports GetReports(string workspaceName)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetReports BEGIN: {DateTime.Now.ToLocalTime()}");

            Reports reports = new Reports();

            ClearGrid();

            RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
            Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();

            PowerShell ps = PowerShell.Create();
            ps.Runspace = runspace;

            var ps1FilePath = ConfigurationManager.AppSettings["PowerShellScriptPath"];
            var ps1File = $@"{ps1FilePath}\GetReports.ps1";
            ps.AddCommand(ps1File);
            ps.AddArgument(workspaceName);
            Collection<PSObject> psObjects = ps.Invoke();
            runspace.Close();

            var outFile = $@"{ps1FilePath}\Reports.txt";
            StreamReader sr = new StreamReader(outFile);
            selReportNameSource.Items.Clear();
            if (sr.ReadLine() != null)
            {
                string line = sr.ReadLine().Trim();
                while (line != null)
                {
                    line = line.Trim();
                    if (line != "")
                    {
                        var report = new etlMon.Common.Types.PowerBI.Report();
                        report.ReportName = line.Trim();
                        report.ReportValue = line.Trim();
                        reports.Add(report);
                    }
                    line = sr.ReadLine();
                }
                sr.Close();

                selReportNameSource.DataTextField = "ReportName";
                selReportNameSource.DataValueField = "ReportValue";
                selReportNameSource.DataSource = reports;
                selReportNameSource.DataBind();
            }

            if (divSelReportNameSource.Visible) txtReportNameTarget.Text = selReportNameSource.SelectedValue;

            Logger.Info($"Rows: {selReportNameSource.Items.Count}");
            Logger.Info("GetReports END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");

            return reports;
        }

        protected string GetReportParameters(string workspaceName, string reportName)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetReportParameters BEGIN: {DateTime.Now.ToLocalTime()}");

            string retVal = "";
            ClearGrid();

            RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
            Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();

            PowerShell ps = PowerShell.Create();
            ps.Runspace = runspace;

            var ps1FilePath = ConfigurationManager.AppSettings["PowerShellScriptPath"];
            var ps1File = $@"{ps1FilePath}\GetReportParameters.ps1";
            ps.AddCommand(ps1File);
            ps.AddArgument(workspaceName);
            ps.AddArgument(reportName);
            Collection<PSObject> psObjects = ps.Invoke();
            runspace.Close();

            var outFile = $@"{ps1FilePath}\ReportParameters.txt";
            StreamReader sr = new StreamReader(outFile);
            string reportParams = sr.ReadToEnd().Trim();
            sr.Close();

            var jsonDeserializedParams = JsonConvert.DeserializeObject(reportParams);



            Logger.Info($"Rows: {selReportNameSource.Items.Count}");
            Logger.Info("GetReportParameters END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");

            return retVal;
        }

        protected void ClearGrid()
        {
            divGridHistory.Visible = false;
            //gvHistory.DataSource = null;
            //gvHistory.DataBind();
        }

        protected void fnEMailChanged(Object sender, EventArgs e)
        {
            //_email = txtEmail.Text;
        }

        protected void fnSelActionChanged(Object sender, EventArgs e)
        {
            fnSelDivs(RadioButtonList1.SelectedValue);
            if (divSelPublishFile.Visible) txtReportNameTarget.Text = selFileName.SelectedValue.Replace(".pbix", "");
            if (divSelReportNameSource.Visible) txtReportNameTarget.Text = selReportNameSource.SelectedValue;
        }

        protected void fnSelDivs(string selValue)
        {
            if (selValue == "PublishReport")
            {
                divSelPublishReport.Visible = true;
                divSelPublishFile.Visible = false;
                divSelUpdateParameter.Visible = false;
                divPublish.Visible = true; 
            }
            else if (selValue == "PublishFile")
            {
                divSelPublishReport.Visible = false;
                divSelPublishFile.Visible = true;
                divSelUpdateParameter.Visible = false;
                divPublish.Visible = true; 
            }
            else if (selValue == "UpdateParameter")
            {
                divSelPublishReport.Visible = false;
                divSelPublishFile.Visible = false;
                divSelUpdateParameter.Visible = true;
                divPublish.Visible = true; 
            }
        }

        protected void fnReportNameSourceChanged(Object sender, EventArgs e)
        {
            ClearGrid();
            //if (txtReportNameTarget.Text == "")
            //{
                txtReportNameTarget.Text = selReportNameSource.SelectedValue;
            //var reportParams = GetReportParameters(selWorkspaceNameSource.SelectedValue, selReportNameSource.SelectedValue);
            //}
        }

        protected void fnWorkspaceNameSourceChanged(Object sender, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"fnPlantChanged BEGIN: {DateTime.Now.ToLocalTime()}");
            ClearGrid();
            reports = GetReports(selWorkspaceNameSource.SelectedValue);
            //var reportParams = GetReportParameters(selWorkspaceNameSource.SelectedValue, selReportNameSource.SelectedValue);
            divPublish.Visible = true;
            Logger.Info("fnPlantChanged END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected void fnWorkspaceNameTargetChanged(Object sender, EventArgs e)
        {
            ClearGrid();
            txtNewValue.Enabled = true;
            lblConnString.Visible = false;
            var itemsSelected = 0;
            var plantsSelected = 0;
            var plantSelected = "";
            var nonPlantsSelected = 0;
            var workspacesSelected = 0;
            lblConnString.ForeColor = Color.Green;
            foreach (ListItem item in cblWorkspaces.Items)
            { 
                if (item.Selected)
                {
                    itemsSelected++;
                    workspacesSelected++;
                    if (item.Value.Length == 4)
                    {
                        if (item.Value.EndsWith("CG") || item.Value.EndsWith("CT") || item.Value.EndsWith("IG") || item.Value.EndsWith("LG"))
                        {
                            plantsSelected++;
                            plantSelected = $@"{item.Value}SQLSRV03";
                        }
                        else
                        {
                            nonPlantsSelected++;
                        }
                    }
                    else
                    {
                        nonPlantsSelected++; 
                    }
                }
            }
            if (plantsSelected == 1 && nonPlantsSelected == 0)
            {
                txtNewValue.Text = plantSelected;
                txtNewValue.Enabled = true;
                lblConnString.Visible = false;
            }
            else if (plantsSelected > 1 && nonPlantsSelected == 0)
            {
                txtNewValue.Text = "";
                txtNewValue.Enabled = false;
                lblConnString.Visible = true;
                lblConnString.Text = "Connection String Value will be derived from PlantCode plus 'SQLSRV03'";
                lblConnString.ForeColor = Color.Green;
            }
            if (nonPlantsSelected > 0)
            {
                //txtNewValue.Text = "";
                txtNewValue.Enabled = true;
                lblConnString.Visible = false;
            }
            if (plantsSelected > 0 && nonPlantsSelected > 0 || nonPlantsSelected > 1)
            {
                lblConnString.Visible = true;
                lblConnString.Text = "All targeted workspaces will use the same Parameter Name and New Value.";
                lblConnString.ForeColor = Color.Red;
            }
            if (itemsSelected == 0)
            {
                //txtNewValue.Text = "";
                txtNewValue.Enabled = true;
                lblConnString.Visible = false;
            }
        }

        protected void fnWorkspaceGroupTargetChanged(Object sender, EventArgs e)
        {
            ClearGrid();
            var cgChecked = false;
            var ctChecked = false;
            var igChecked = false;
            var lgChecked = false;

            foreach (ListItem item in cblWorkspaceGroup.Items)
            {
                if (item.Selected)
                {
                    switch (item.Value)
                    {
                        case "All CG":
                            fnCheckGroup("CG");
                            cgChecked = true;
                            txtNewValue.Enabled = false;
                            lblConnString.Visible = true;
                            break;
                        case "All CT":
                            fnCheckGroup("CT");
                            ctChecked = true;
                            txtNewValue.Enabled = false;
                            lblConnString.Visible = true;
                            break;
                        case "All IG":
                            fnCheckGroup("IG");
                            igChecked = true;
                            txtNewValue.Enabled = false;
                            lblConnString.Visible = true;
                            break;
                        case "All LG":
                            fnCheckGroup("LG");
                            lgChecked = true;
                            txtNewValue.Enabled = false;
                            lblConnString.Visible = true;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (item.Value)
                    {
                        case "All CG":
                            fnUnCheckGroup("CG");
                            cgChecked = false;
                            break;
                        case "All CT":
                            fnUnCheckGroup("CT");
                            ctChecked = false;
                            break;
                        case "All IG":
                            fnUnCheckGroup("IG");
                            igChecked = false;
                            break;
                        case "All LG":
                            fnUnCheckGroup("LG");
                            lgChecked = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            if (!cgChecked && !ctChecked && !igChecked && !lgChecked)
            {
                txtNewValue.Enabled = true;
                lblConnString.Visible = false;
            }
            fnWorkspaceNameTargetChanged(sender, e);
        }

        protected void fnCheckGroup(string groupCode)
        {
            foreach (ListItem item in cblWorkspaces.Items)
            {
                if (item.Value.EndsWith(groupCode)) item.Selected = true; 
            }
        }

        protected void fnUnCheckGroup(string groupCode)
        {
            foreach (ListItem item in cblWorkspaces.Items)
            {
                if (item.Value.EndsWith(groupCode)) item.Selected = false;
            }
        }

        protected void fnFileNameChanged(Object sender, EventArgs e)
        {
            txtReportNameTarget.Text = selFileName.Text.Replace(".pbix", "");
        }

        protected void fnFolderNameChanged(Object sender, EventArgs e)
        {
            GetFiles(selFolderName.SelectedValue);
        }

        protected void btnPublish_Click(Object obj, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"btnPublish_Click BEGIN: {DateTime.Now.ToLocalTime()}");
            ClearGrid();

            if (fnValidate())
            {
                fnPublish();
                //fnInclude_Changed();
                //divGridHistory.Visible = true;
            }
            Logger.Info("btnPublish_Click END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected void btnUpdateParam_Click(Object obj, EventArgs e)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"btnGetHistory_Click BEGIN: {DateTime.Now.ToLocalTime()}");
            ClearGrid();
            Logger.Info("btnGetHistory_Click END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected Boolean fnValidate()
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"fnValidate BEGIN: {DateTime.Now.ToLocalTime()}");
            lblError.Text = "";
            divError.Visible = false;

            if (!txtEmail.Text.Contains("@cardinalcorp.com") && !txtEmail.Text.Contains("@cgi.int"))
            {
                lblError.Text = "Please enter a valid Cardinal email address.";
                lblError.Visible = true;
                divError.Visible = true;
                return false;
            }

            // Get Workspace Target scenario
            var itemsSelected = 0;
            var plantsSelected = 0;
            var nonPlantsSelected = 0;
            var workspacesSelected = 0;
            lblConnString.ForeColor = Color.Green;
            foreach (ListItem item in cblWorkspaces.Items)
            {
                if (item.Selected)
                {
                    itemsSelected++;
                    workspacesSelected++;
                    if (item.Value.Length == 4)
                    {
                        if (item.Value.EndsWith("CG") || item.Value.EndsWith("CT") || item.Value.EndsWith("IG") || item.Value.EndsWith("LG"))
                        {
                            plantsSelected++;
                        }
                        else
                        {
                            nonPlantsSelected++;
                        }
                    }
                    else
                    {
                        nonPlantsSelected++;
                    }
                }
            }

            if (workspacesSelected == 0)
            {
                lblError.Text = "Please select a workspace target.";
                lblError.Visible = true;
                divError.Visible = true;
                return false;
            }
            else if (txtParameterName.Text.Length == 0)
            {
                lblError.Text = "Please enter a report parameter name.";
                lblError.Visible = true;
                divError.Visible = true;
                return false;
            }
            else if (txtNewValue.Text.Length == 0 && !(plantsSelected > 1 && nonPlantsSelected == 0))
            {
                lblError.Text = "Please enter a report parameter new value.";
                lblError.Visible = true;
                divError.Visible = true;
                return false;
            }

            var searchType = RadioButtonList1.SelectedValue;
            if (searchType == "PublishReport")
            {
            }
            else if (searchType == "PublishFile")
            {
            }
            else if (searchType == "UpdateParameter")
            {
            }

            Logger.Info("fnValidate END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return true;
        }

        protected void fnPublish()
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"fnPublish BEGIN: {DateTime.Now.ToLocalTime()}");
            string email = txtEmail.Text;
            string workSpaceNameSource = selWorkspaceNameSource.SelectedValue;
            string rptNameSource = selReportNameSource.Text;
            string folderName = selFolderName.Text;
            string fileName = selFileName.Text;
            var reportNameTarget = txtReportNameTarget.Text;
            string parameterName = txtParameterName.Text;
            string newValue = txtNewValue.Text;
            var manager = new ReportManager();

            // Get Workspace Target scenario
            var itemsSelected = 0;
            var plantsSelected = 0;
            var nonPlantsSelected = 0;
            var workspacesSelected = 0;
            lblConnString.ForeColor = Color.Green;
            foreach (ListItem item in cblWorkspaces.Items)
            {
                if (item.Selected)
                {
                    itemsSelected++;
                    workspacesSelected++;
                    if (item.Value.Length == 4)
                    {
                        if (item.Value.EndsWith("CG") || item.Value.EndsWith("CT") || item.Value.EndsWith("IG") || item.Value.EndsWith("LG"))
                        {
                            plantsSelected++;
                        }
                        else
                        {
                            nonPlantsSelected++;
                        }
                    }
                    else
                    {
                        nonPlantsSelected++;
                    }
                }
            }

            if (RadioButtonList1.SelectedValue == "PublishReport")
            {
                foreach (ListItem item in cblWorkspaces.Items)
                {
                    if (item.Selected)
                    {
                        if (plantsSelected > 1 && nonPlantsSelected == 0) newValue = $@"{item.Text}SQLSRV03";
                        manager.PublishReport(workSpaceNameSource, rptNameSource, item.Value, reportNameTarget, parameterName, newValue, email);
                    }
                }
            }
            else if (RadioButtonList1.SelectedValue == "PublishFile")
            {
                foreach (ListItem item in cblWorkspaces.Items)
                {
                    if (item.Selected)
                    {
                        if (plantsSelected > 1 && nonPlantsSelected == 0) newValue = $@"{item.Text}SQLSRV03";
                        manager.PublishFile(folderName, fileName, item.Value, reportNameTarget, parameterName, newValue, email);
                    }
                }
            }

            else if (RadioButtonList1.SelectedValue == "UpdateParameter")
            {
                foreach (ListItem item in cblWorkspaces.Items)
                {
                    if (item.Selected)
                    {
                        if (plantsSelected > 1 && nonPlantsSelected == 0) newValue = $@"{item.Text}SQLSRV03";
                        manager.UpdateReportParameter(item.Value, reportNameTarget, parameterName, newValue, email);
                    }
                }
            }

            Logger.Info($"Rows: {dt.Rows.Count}");
            Logger.Info("fnPublish END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
        }

        protected Plant GetOperatorPlant(string operatorName)
        {
            var startTime = DateTime.Now.ToLocalTime();
            Logger.Info($"GetOperatorPlant BEGIN: {DateTime.Now.ToLocalTime()}");
            Plant plant = new Plant();
            String csDW = Properties.Settings.Default.csDW;
            SqlConnection cnDW = new SqlConnection(csDW);
            cnDW.Open();
            try
            {
                var displayname = "";
                var description = "";
                var mail = "";
                var userprincipalname = "";
                var distinguishedname = "";
                var memberof = "";

                using (IDataReader reader = GetOperatorPlant(cnDW, operatorName))
                {
                    while (reader.Read())
                    {
                        displayname = CastDbNull.To(reader["displayname"], string.Empty);
                        description = CastDbNull.To(reader["description"], string.Empty);
                        mail = CastDbNull.To(reader["mail"], string.Empty);
                        userprincipalname = CastDbNull.To(reader["userprincipalname"], string.Empty);
                        distinguishedname = CastDbNull.To(reader["distinguishedname"], string.Empty);
                        memberof = CastDbNull.To(reader["memberof"], string.Empty);
                    }
                    reader.Close();
                }

                Logger.Info($"userprincipalname: {userprincipalname}");
                if (txtEmail.Text.Length == 0) txtEmail.Text = userprincipalname;
                var plantCode = displayname.Substring(displayname.Length - 4, 4);
                plant = PlantManager.GetPlantNull(plantCode);
                Logger.Info($"plant: {plant}");
                if (plant == null)
                {
                    Logger.Info($"distinguishedname OU=[A-Z][A-Z][A-Z][A-Z]: {distinguishedname}");
                    string pattern = @"OU=[A-Z][A-Z][A-Z][A-Z],";
                    Regex rg = new Regex(pattern);
                    var plantCodes = rg.Matches(distinguishedname);
                    for (int count = 0; count < plantCodes.Count; count++)
                    {
                        plantCode = plantCodes[count].Value.Replace("OU=", "").Replace(",", "");
                        plant = PlantManager.GetPlantNull(plantCode);
                        if (plant != null) count = plantCodes.Count;
                        Logger.Info($"plant: {plant}");
                    }
                    if (plant == null)
                    {
                        Logger.Info($"distinguishedname OU=Corporate: {distinguishedname}");
                        pattern = @"OU=Corporate,";
                        rg = new Regex(pattern);
                        plantCodes = rg.Matches(distinguishedname);
                        for (int count = 0; count < plantCodes.Count; count++)
                        {
                            plantCode = plantCodes[count].Value.Replace("OU=", "").Replace(",", "");
                            plant = PlantManager.GetPlantNull(plantCode);
                            if (plant != null) count = plantCodes.Count;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
            cnDW.Close();
            Logger.Info("GetOperatorPlant END: " + Convert.ToDecimal((DateTime.Now.ToLocalTime() - startTime).TotalSeconds) + " SECONDS");
            return plant;
        }

        public static IDataReader GetOperatorPlant(SqlConnection cnDW, string operatorName)
        {
            var startTime = DateTime.Now.ToLocalTime();
            try
            {
                var cmd = new SqlCommand()
                {
                    Connection = cnDW,
                    CommandTimeout = 0,
                    CommandType = CommandType.Text,
                    CommandText = $@"
                        SELECT [samaccountname]
                              ,[displayname]
                              ,[description]
                              ,[userprincipalname]
                              ,[mail]
                              ,[distinguishedname]
                              ,[memberof]
                          FROM [PSBI_WH_PROD].[PROD].[ACTIVE_DIRECTORY]
                          WHERE [samaccountname] = @OperatorName"
                };
                // Plant Num
                cmd.Parameters.Add(new SqlParameter("@OperatorName", SqlDbType.NVarChar, 100));
                cmd.Parameters["@OperatorName"].Value = operatorName;
                // Reader
                Logger.Info(Logging.DbCmdString(cmd));
                return cmd.ExecuteReader(CommandBehavior.SingleRow);
            }
            catch (Exception e)
            {
                Logger.Error($"ERROR: {e.Message}: {e.StackTrace}");
                throw new Exception(e.Message);
            }
        }

    }
}