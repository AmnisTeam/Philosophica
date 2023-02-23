using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class DataBase
{
    private const string fileName = "db.bytes";
    private static string DBPath;
    private static SqliteConnection connection;
    private static SqliteCommand command;
    private static List<DataBaseModel> models = new List<DataBaseModel>();

    public static void Init()
    {
        DBPath = GetDatabasePath();
    }

    /// <summary> ���������� ���� � ��. ���� � ��� � ������ ����� �� ��������, �� �������� � � ��������� apk �����. </summary>
    private static string GetDatabasePath()
    {
#if UNITY_EDITOR
        return Path.Combine(Application.streamingAssetsPath, fileName);
#endif
#if UNITY_STANDALONE
        string filePath = Path.Combine(Application.dataPath, fileName);
        if (!File.Exists(filePath)) UnpackDatabase(filePath);
        return filePath;
#elif UNITY_ANDROID
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if(!File.Exists(filePath)) UnpackDatabase(filePath);
        return filePath;
#endif
    }

    /// <summary> ������������� ���� ������ � ��������� ����. </summary>
    /// <param name="toPath"> ���� � ������� ����� ����������� ���� ������. </param>
    private static void UnpackDatabase(string toPath)
    {
        string fromPath = Path.Combine(Application.streamingAssetsPath, fileName);

        WWW reader = new WWW(fromPath);
        while (!reader.isDone) { }

        File.WriteAllBytes(toPath, reader.bytes);
    }

    /// <summary> ���� ����� ��������� ����������� � ��. </summary>
    public static void OpenConnection()
    {
        connection = new SqliteConnection("Data Source=" + DBPath);
        command = new SqliteCommand(connection);
        connection.Open();
    }

    /// <summary> ���� ����� ��������� ����������� � ��. </summary>
    public static void CloseConnection()
    {
        connection.Close();
        command.Dispose();
    }

    /// <summary> ���� ����� ��������� ������ query. </summary>
    /// <param name="query"> ���������� ������. </param>
    public static void ExecuteQueryWithoutAnswer(string query)
    {
        OpenConnection();
        command.CommandText = query;
        command.ExecuteNonQuery();
        CloseConnection();
    }

    // <summary> ���� ����� ��������� ������ query � ���������� ����� �������. </summary>
    /// <param name="query"> ���������� ������. </param>
    /// <returns> ���������� �������� 1 ������ 1 �������, ���� ��� �������. </returns>
    public static SqliteDataReader ExecuteQueryWithAnswer(string query)
    {
        OpenConnection();
        command.CommandText = query;
        var answer = command.ExecuteReader();

        if (answer != null) return answer;
        else return null;
    }

    /// <summary> ���� ����� ���������� �������, ������� �������� ����������� ������� ������� query. </summary>
    /// <param name="query"> ���������� ������. </param>
    public static DataTable GetTable(string query)
    {
        OpenConnection();

        SqliteDataAdapter adapter = new SqliteDataAdapter(query, connection);

        DataSet DS = new DataSet();
        adapter.Fill(DS);
        adapter.Dispose();

        CloseConnection();

        return DS.Tables[0];
    }

    /// <summary> ������ �������. </summary>
    public static void CreateTable(DataBaseModel dataBaseModel)
    {
        string query = "CREATE TABLE IF NOT EXISTS '" + dataBaseModel.tableName + "' (";
        for (int x = 0; x < dataBaseModel.fields.Count; x++)
        {
            if (!dataBaseModel.fields[x].isNotAddToTable)
            {
                query += "'" + dataBaseModel.fields[x].name + "' " + dataBaseModel.fields[x].typeFiled;
                if (dataBaseModel.fields[x].isPrimaryKey)
                    query += " PRIMARY KEY";
                else
                    query += " NOT NULL";
                if (x < dataBaseModel.fields.Count - 1)
                    query += ",";
            }
        }
        query += ");";
        ExecuteQueryWithoutAnswer(query);
    }

    /// <summary> ������ SELECT. </summary>
    public static List<DataBaseModel> Select(DataBaseModel model, params SelectArg[] args)
    {
        string query = "SELECT * FROM " + model.tableName;

        if (args.Length > 0)
            query += " WHERE ";

        for (int x = 0; x < args.Length; x++)
        {
            query += args[x].name + " = '" + args[x].value + "'";
            if (x < args.Length - 1)
                query += " AND ";
        }
        query += ";";

        List<DataBaseModel> selectedModels = new List<DataBaseModel>();

        SqliteDataReader reader = ExecuteQueryWithAnswer(query);
        while (reader.Read())
        {
            DataBaseModel createdModel = model.Clone();
            for (int x = 0; x < createdModel.fields.Count; x++)
                createdModel.fields[x].Set(reader.GetValue(x));
            selectedModels.Add(createdModel);
        }

        CloseConnection();
        return selectedModels;
    }

    /// <summary> ������ INSERT. </summary>
    public static void Insert(DataBaseModel dataBaseModel)
    {
        string query = "INSERT INTO " + dataBaseModel.tableName + " (";

        for (int x = 0; x < dataBaseModel.fields.Count; x++)
        {
            if (!dataBaseModel.fields[x].isPrimaryKey)
            {
                query += dataBaseModel.fields[x].name;
                if (x < dataBaseModel.fields.Count - 1)
                    query += ",";
            }

        }

        query += ") VALUES (";

        for (int x = 0; x < dataBaseModel.fields.Count; x++)
        {
            if (!dataBaseModel.fields[x].isPrimaryKey)
            {
                query += dataBaseModel.fields[x].GetAsString();
                if (x < dataBaseModel.fields.Count - 1)
                    query += ",";
            }
        }
        query += ");";

        ExecuteQueryWithoutAnswer(query);
    }

    /// <summary> ������ UPDATE. </summary>
    public static void Update(DataBaseModel dataBaseModel, params SelectArg[] args)
    {
        string query = "UPDATE " + dataBaseModel.tableName + " SET ";

        for (int x = 0; x < dataBaseModel.fields.Count; x++)
        {
            if (!dataBaseModel.fields[x].isPrimaryKey)
            {
                query += dataBaseModel.fields[x].name + " = " + dataBaseModel.fields[x].Get();
                if (x < dataBaseModel.fields.Count - 1)
                    query += ",";
            }

        }

        query += " WHERE ";

        for (int x = 0; x < args.Length; x++)
        {
            query += args[x].name + " = " + args[x].value;
            if (x < dataBaseModel.fields.Count - 1)
                query += ",";
        }
        query += ";";

        ExecuteQueryWithoutAnswer(query);
    }

    /// <summary> ������������ ������ � ���� ������. </summary>
    public static void RegisterModel(DataBaseModel dataBaseModel)
    {
        models.Add(dataBaseModel);
        CreateTable(dataBaseModel);
    }
}
