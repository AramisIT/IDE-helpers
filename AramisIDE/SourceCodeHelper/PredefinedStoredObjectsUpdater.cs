using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aramis.DatabaseConnector;
using Aramis.DatabaseConnector.MSSQL2008;
using DBInteraction;
using DBInteraction.StoredObjectsUpdating;

namespace AramisIDE.SourceCodeHelper
    {
    class PredefinedStoredObjectsUpdater
        {
        private static string predefinedStoredObjectsSourceCodeFilePath;
        private static string connectionString;
        private bool useIntegratedSecurityConnectionString;

        public PredefinedStoredObjectsUpdater(string predefinedStoredObjectsSourceCodeFilePath, string connectionString)
            {
            PredefinedStoredObjectsUpdater.predefinedStoredObjectsSourceCodeFilePath = predefinedStoredObjectsSourceCodeFilePath;

            useIntegratedSecurityConnectionString = string.IsNullOrEmpty(connectionString);
            const string INTEGRATED_SECURITY_DEFAULT_CONNECTION_STRING = "Data Source=localhost; Initial Catalog=master;Integrated Security=True";
            PredefinedStoredObjectsUpdater.connectionString = useIntegratedSecurityConnectionString ? INTEGRATED_SECURITY_DEFAULT_CONNECTION_STRING : connectionString;
            }

        public PredefinedStoredObjectsUpdater()
            { }

        internal bool Update()
            {
            DB.InitWithConnectionString(connectionString);

            string sourceCodeData = null;
            try
                {
                sourceCodeData = File.ReadAllText(predefinedStoredObjectsSourceCodeFilePath);
                }
            catch (Exception exp)
                {
                showMessage(string.Format("Can't read source code of predefined objects:\r\n{0}", exp.Message), true);
                return false;
                }

            StoredObjectsFileData storedObjectsFileData = null;
            try
                {
                storedObjectsFileData = new StoredObjectsFileData().Fill(sourceCodeData);
                }
            catch (Exception exp)
                {
                showMessage(string.Format("Can't parce source code of predefined objects:\r\n{0}", exp.Message), true);
                return false;
                }

            string newFileData = null;
            string info;
            try
                {
                var storedObjectsSqlServerData = new StoredObjectsSqlServerData(connectionString, buildConnectionString);
                newFileData = storedObjectsFileData.BuildUpdatedDocument(storedObjectsSqlServerData, out info).ToString();
                }
            catch (Exception exp)
                {
                showMessage(string.Format("Can't build update file of predefined objects:\r\n{0}", exp.Message), true);
                return false;
                }

            if (string.IsNullOrEmpty(info))
                {
                showMessage("No newer stored objects found!");
                return true;
                }

            if (!ask(string.Format("Update next stored objects?\r\n\r\n{0}", info))) return true;

            try
                {
                File.WriteAllText(predefinedStoredObjectsSourceCodeFilePath, newFileData);
                }
            catch (Exception exp)
                {
                showMessage(string.Format("Can't rewrite file of predefined objects:\r\n{0}", exp.Message), true);
                return false;
                }

            return true;
            }

        private string buildConnectionString(string specifiedDatabaseName)
            {
            return connectionString;
            //$@"Server=""{serverName}""; Initial Catalog={specifiedDatabaseName}; "
            //   + (string.IsNullOrEmpty(userId) ? "Integrated Security=True" : $@"user id=""{TestConnect.USER_NAME}""; password=""{TestConnect.USER_PASSWORD}""");
            }

        private bool ask(string message)
            {
            using (new DialogShower())
                {
                return MessageBox.Show(message, messageBoxHeader, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) ==
                 DialogResult.OK;
                }
            }

        private const string messageBoxHeader = "Predefined stored objects updater";

        void showMessage(string message, bool error = false)
            {
            using (new DialogShower())
                {
                MessageBox.Show(message, messageBoxHeader, MessageBoxButtons.OK, error ? MessageBoxIcon.Error : MessageBoxIcon.Information);
                }
            }
        }
    }
