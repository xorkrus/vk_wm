using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Galssoft.VKontakteWM.Components;
using Galssoft.VKontakteWM.Components.SystemHelpers;
using Galssoft.VKontakteWM.Components.Configuration;
using Galssoft.VKontakteWM.Components.Server;
using System.Text;
using Galssoft.VKontakteWM.Components.UI;
using Galssoft.VKontakteWM.CustomControlls;
using Galssoft.VKontakteWM.Common;
using System.Diagnostics;
using Galssoft.VKontakteWM.Properties;
using Galssoft.VKontakteWM.Components.UI.Wrappers;

namespace Galssoft.VKontakteWM.ApplicationLogic
{
    public class UpdateHelper : IDisposable
    {
        #region Fields

        public static readonly Version VersionEmpty = new Version(0, 0, 0);

        private UIViewBase _syncControl;

        #endregion

        #region Constructors

        public UpdateHelper(UIViewBase syncControl)
        {
            _syncControl = syncControl;

            NewVersion = new VersionInfo();
        }

        #endregion

        #region Nested type: ProcessResultDelegate

        private delegate void ProcessResultDelegate(VersionInfo info);

        #endregion

        #region Properties

        public bool IsNewVersionAvailable { get; private set; }
        public bool IsUnableToDefineLatestVersion { get; private set; }
        public VersionInfo NewVersion { get; private set; }
        public bool IsProcessed { get; private set; }

        #endregion

        #region Methods

        public void CheckNewVersion()
        {
            var t = new Thread(delegate { CheckInternal(false); })
            {
                IsBackground = true
            };

            t.Start();
        }

        public void CheckInternal(bool silentCheck)
        {
            VersionInfo newVersionInfo = GetVersion();            

            if (newVersionInfo.result)
            {
                IsUnableToDefineLatestVersion = false;

                if (SystemConfiguration.CurrentProductVersion.CompareTo(newVersionInfo.newVersion) == -1)
                {
                    IsNewVersionAvailable = true;

                    UpdateVersionInfo(newVersionInfo.newVersion, newVersionInfo.description, newVersionInfo.downloadURL, newVersionInfo.newVersionIsMandatory);

                    if (!silentCheck)
                    {
                        NewVersion = newVersionInfo;

                        if (_syncControl != null)
                        {
                            ProcessResultIfVisible(newVersionInfo);
                        }
                    }
                }
                else
                {
                    IsNewVersionAvailable = false;
                }
            }
            else
            {
                IsUnableToDefineLatestVersion = true;
            }
        }

        private static void UpdateVersionInfo(Version version, string features, string updateURL, bool version_is_mandatory)
        {
            Globals.BaseLogic.IDataLogic.SetLastCheckedVersion(version);
            Globals.BaseLogic.IDataLogic.SetLastCheckedVersionInfo(features);
            Globals.BaseLogic.IDataLogic.SetLastCheckedVersionUpdateURL(updateURL);
            Globals.BaseLogic.IDataLogic.SetLastCheckedVersionIsMandatory(version_is_mandatory);
        }

        private static Version GetLastCheckedVersion()
        {
            return Globals.BaseLogic.IDataLogic.GetLastCheckedVersion();
        }

        public void ProcessResult()
        {
            if (IsNewVersionAvailable && !IsProcessed)
            {
                IsProcessed = true;

                ProcessResult(NewVersion);
            }
        }

        private void ProcessResultIfVisible(VersionInfo newVersionInfo)
        {
            if (_syncControl.InvokeRequired)
            {
                _syncControl.Invoke(new ProcessResultDelegate(ProcessResultIfVisible), newVersionInfo);
            }
            else
            {
                if (_syncControl.Visible)
                {
                    ProcessResult();
                }
            }
        }

        private void ProcessResult(VersionInfo newVersionInfo)
        {
            if (_syncControl.InvokeRequired)
            {
                _syncControl.Invoke(new ProcessResultDelegate(ProcessResult), newVersionInfo);
            }
            else
            {
                if (!IsUnableToDefineLatestVersion)
                {
                    if (IsNewVersionAvailable)
                    {
                        if (!newVersionInfo.newVersionIsMandatory)
                        {
                            if (newVersionInfo.newVersion.Equals(newVersionInfo.skippedVersion))
                            {
                                return;
                            }
                        }

                        StringBuilder newStringBuilder = new StringBuilder();

                        newStringBuilder.Append(Resources.UpdateInfo_Message_VersionAvaliable);
                        newStringBuilder.Append(" ");
                        newStringBuilder.Append(newVersionInfo.newVersion.ToString());
                        newStringBuilder.Append("\n");

                        newStringBuilder.Append(newVersionInfo.description);

                        DialogResult dlgRes = DialogResult.None;

                        if (newVersionInfo.newVersionIsMandatory)
                        {
                            dlgRes = UpdateInfoDialogControl.ShowQuery(newStringBuilder.ToString(), true);
                        }
                        else
                        {
                            dlgRes = UpdateInfoDialogControl.ShowQuery(newStringBuilder.ToString(), false);
                        }

                        switch (dlgRes)
                        {
                            case DialogResult.Yes: //Update
                                {
                                    string updateURL = Globals.BaseLogic.IDataLogic.GetLastCheckedVersionUpdateURL();

                                    if (!string.IsNullOrEmpty(updateURL))
                                    {
                                        Process.Start(updateURL, null);
                                    }

                                    break;
                                }
                            case DialogResult.No: //Skip
                                Globals.BaseLogic.IDataLogic.SetLastCheckedVersionIsSkipped(Globals.BaseLogic.IDataLogic.GetLastCheckedVersion());
                                break;
                            case DialogResult.Cancel:
                                break;
                        }

                        if (newVersionInfo.newVersionIsMandatory)
                        {
                            Application.Exit();
                        }
                    }
                }
            }
        }

        public static VersionInfo GetVersion()
        {
            var versionInfo = new VersionInfo();

            string url = SystemConfiguration.UpdateVersionURL;

            var webRequest = HttpUtility.PrepareHttpWebRequest(url);
            webRequest.AllowAutoRedirect = false;

            try
            {
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string newVersion = streamReader.ReadLine();
                        string baseVersion = streamReader.ReadLine();
                        string downloadURL = streamReader.ReadLine();

                        StringBuilder stringBuilder = new StringBuilder();

                        while (!streamReader.EndOfStream)
                        {
                            stringBuilder.Append(streamReader.ReadLine());
                            stringBuilder.Append("\n");
                        }

                        string description = stringBuilder.ToString();
                        description.Remove(description.Length - 1, 1);

                        if (newVersion[0] == '!')
                        {
                            versionInfo.newVersionIsMandatory = true;

                            newVersion = newVersion.TrimStart(new char[] { '!' });
                        }
                        else
                        {
                            versionInfo.newVersionIsMandatory = false;
                        }

                        baseVersion = baseVersion.TrimStart(new char[] { '!' });

                        versionInfo.newVersion = new Version(newVersion);
                        if (versionInfo.newVersion.Build < 0)
                        {
                            versionInfo.newVersion = new Version(versionInfo.newVersion.Major, versionInfo.newVersion.Minor, 0);
                        }

                        versionInfo.baseVersion = new Version(baseVersion);
                        if (versionInfo.baseVersion.Build < 0)
                        {
                            versionInfo.baseVersion = new Version(versionInfo.baseVersion.Major, versionInfo.baseVersion.Minor, 0);
                        }

                        versionInfo.downloadURL = downloadURL;
                        versionInfo.description = description;

                        if (versionInfo.baseVersion.CompareTo(SystemConfiguration.CurrentProductVersion) > 0)
                        {
                            versionInfo.newVersionIsMandatory = true;
                        }
                    }
                }

                versionInfo.result = true;
            }
            catch
            {
                versionInfo.result = false;
            }
            finally
            {
                webRequest.Abort();
            }

            return versionInfo;
        }

        #region IDisposable Members

        public void Dispose()
        {
            //_syncControl = null;
        }

        #endregion

        #endregion

        #region Nested type: VersionInfo

        public class VersionInfo
        {
            public Version newVersion { get; set; }

            public bool newVersionIsMandatory { get; set; }

            public Version baseVersion { get; set; }

            public Version skippedVersion { get; set; }

            public string downloadURL { get; set; }

            public string description { get; set; }

            public bool result { get; set; }

            public VersionInfo()
            {
                newVersion = new Version(0, 0, 0);

                newVersionIsMandatory = false;

                baseVersion = new Version(0, 0, 0);

                downloadURL = string.Empty;

                description = string.Empty;

                result = false;

                skippedVersion = Globals.BaseLogic.IDataLogic.GetLastCheckedVersionIsSkipped();
            }
        }

        #endregion
    }
}