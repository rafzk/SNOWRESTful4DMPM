using Newtonsoft.Json;
using SNOWRESTful4DMPM.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SNOWRESTful4DMPM
{
    class Program
    {
        static NameValueCollection AppSettings = ConfigurationManager.AppSettings;

        static void Main(string[] args)
        {
            var settings = new Endpoint();
            List<Request> itemsList = new List<Request>();
            itemsList = Utils.ReadSchema(settings.SchemaFile);
            Utils.log.Info($"Found a total of {itemsList.Count} nodes");
            ProcessData(itemsList, settings);

        }

        private static void ProcessData(List<Request> itemsList, Endpoint settings)
        {
            List<string> resultCollection = new List<string>();

            for (int j = 0; j < itemsList.Count; j++)
            {
                Utils.log.Info($"Begin transaction #{j}");
                string apitable = itemsList[j].ApiTable;
                string fields = itemsList[j].ApiFieldsList;
                string query = itemsList[j].Query;
                string limit = itemsList[j].Limit;

                Utils.log.Info($"API Table: {apitable}");
                Utils.log.Debug($"API Fields: {fields}");
                Utils.log.Debug($"Query: {query}");
                Utils.log.Debug($"Limit: {limit}");

                string endpoint = settings.EndPoint;
                string parameters = settings.RestParameters;
                endpoint += apitable + "?sysparm_query=" + query + "&sysparm_limit=" + limit + parameters;
                Utils.log.Debug($"Address: {endpoint}");
                Console.WriteLine(endpoint);


                // Call REST
                Utils.log.Debug($"Making the REST call");

                var rest = new API(endpoint, settings.RestContentType, settings.RestUser, settings.RestPwd, settings.RestTimeout);
                var result = rest.MakeWebRequest();
                Utils.log.Debug($"Result: {result}");

                result = Utils.RemoveSpecialChars(result);
                dynamic results = JsonConvert.DeserializeObject<dynamic>(result);


                resultCollection.Clear();
                //Array tt = results.result;

                for (int k = 0; k < results.result.Count; k++)
                {
                    string entryResult = string.Empty;
                    string[] splitFields = itemsList[j].ApiFieldsList.Split(',');
                    Utils.log.Debug($"API (#) table affected: {apitable}");
                    Utils.log.Debug($"API (#) fields to collect: {splitFields.Length}");
                    for (int l = 0; l < splitFields.Length; l++)
                    {
                        //Console.WriteLine(results.result[k][splitFields[l]]);


                        if (entryResult != string.Empty) entryResult += ",";

                        // vai buscar os campos
                        var _key = splitFields[l];
                        var _fields = _key.Split('.');
                        Utils.log.Debug($"Value #{l} Parent {_fields[0]}");
                        Utils.log.Debug($"Value #{l} Child: {_fields[1]}");

                        try
                        {
                            var _keyParent = _fields[0];
                            var _keyChild = _fields[1];
                            entryResult += "'" + results.result[k][_keyParent][_keyChild] + "'";
                        }
                        catch (System.IndexOutOfRangeException e)  // CS0168
                        {
                            System.Console.WriteLine(e.Message);
                            // Set IndexOutOfRangeException to the new exception's InnerException.
                            throw new System.ArgumentOutOfRangeException($ "index parameter is out of range. {e}");
                        }

                    }

                    // Console.WriteLine(entryResult);
                    resultCollection.Add(entryResult);

                }


                var t = resultCollection;

                if (settings.WriteType == "SQL")
                {
                    Utils.log.Debug($"Method selected: {settings.WriteType}");
                    Utils.log.Debug($"Schema DB Table: {itemsList[j].DbTable}");
                    using (SqlConnection myConnection = new SqlConnection(settings.ConnectionStringBD))
                    {
                        try
                        {
                            Utils.log.Debug("Trying to loggin into SQL Server");
                            myConnection.Open();

                            Utils.log.Debug("You are successfully logged in");

                            using (SqlTransaction tran = myConnection.BeginTransaction())
                            {
                                try
                                {
                                    Utils.log.Debug($"DB (#) fields to insert: {resultCollection.Count}");
                                    for (int m = 0; m < resultCollection.Count; m++)
                                    {
                                        // percorrer cada linha a lista dos campos
                                        string buildQuery = "INSERT INTO " + itemsList[j].DbTable + "(" + itemsList[j].DbFieldsList + ",import_on)";

                                        buildQuery += "Values (" + resultCollection[m].ToString() + ",@ImportDate)";
                                        Utils.log.Debug($"SQL query: {buildQuery}");
                                        SqlCommand myCommand = new SqlCommand(buildQuery, myConnection, tran);

                                        // Para utilizasr o insert sem a transaction
                                        //SqlCommand myCommand = new SqlCommand(buildQuery, myConnection);
                                        // Para utilizar com transaction
                                        myCommand.Parameters.Add(new SqlParameter("@ImportDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                                        myCommand.ExecuteNonQuery();
                                    }

                                    Utils.log.Info($"SQL executed! Total rows affected are {resultCollection.Count}");
                                    tran.Commit();
                                }
                                catch
                                {
                                    tran.Rollback();
                                    throw;
                                }
                                finally
                                {
                                    myConnection.Close();
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            Utils.log.Error($"SQL Transaction Rollback Exception Output: {e.ToString()}");
                            //Console.WriteLine("SQL Transaction Rollback Exception Type: {0}", e.GetType());
                            Utils.log.Error($"SQL Transaction Rollback Exception Type: {e.GetType()}");
                            //Console.WriteLine("Message: {0}", e.Message);
                            Utils.log.Error($"Message: {e.Message}");
                        }
                    }
                }
                else if (settings.WriteType == "File")
                {
                    Utils.log.Info($"Method selected: {settings.WriteType}");
                    Utils.log.Info("Writting data to file");
                    string filePath = AppDomain.CurrentDomain.BaseDirectory;
                    Utils.log.Info($"Path: {filePath}");
                    Utils.WriteToFile(filePath, apitable, result);
                }
                else
                {
                    Utils.log.Error("Error on selecting method type");
                }
                Utils.log.Info("End transaction");
            }


        }
    }

}