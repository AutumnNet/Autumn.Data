using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Autumn.Net.Annotation;
using Autumn.Net.Interfaces;


namespace Autumn.Net.Data
{
	
	public delegate object DExecute (IDataReader reader);
	
	[Service]
    public class DataService : IDataTemplate
    {	    
	    [Autowired]
	    [Option("logger.name", "Data")]
	    private ILog logger;

	    [Autowired]
	    private IDataConnectionPoolService connectionService;

		public IDbCommand CreateCommand(DataTransaction t, IDbConnection dbConnection) {
			var dbCommand = dbConnection.CreateCommand();
			dbCommand.CommandText = string.Format (t.Query, t.Args);
			if (t.DataParams != null)
				foreach(var p in t.DataParams)
					dbCommand.Parameters.Add(p);
			dbCommand.ExecuteNonQuery ();
			return dbCommand;
		}

		public IDbDataParameter CreateParameter(string paramName, DbType dbType, object value)
		{
			return connectionService.CreateParameter(paramName, dbType, value);
		}

		public void ExecuteParams(IEnumerable<DataTransaction> transactions) {
			var dataTransactions = transactions as DataTransaction[] ?? transactions.ToArray();
			var count = 5;
			while (count>0) {
				count--;
				var dbConnection = connectionService.Get();
				try
				{						
					using (var tr = dbConnection.BeginTransaction())
					{
						var dbCommand = tr.Connection.CreateCommand();
						foreach (var t in dataTransactions)
						{
							dbCommand.Transaction = tr;
							dbCommand.CommandText = string.Format(t.Query, t.Args);
							if (t.DataParams != null)
								foreach (var p in t.DataParams)
									dbCommand.Parameters.Add(p);
							dbCommand.ExecuteNonQuery();
						}

						tr.Commit();
						dbCommand?.Dispose();
					}
				}
				catch (Exception e)
				{
					logger.Error ($"SQLiteException(Transactions):{e.Message}\n{e.StackTrace}\n", e);
				}
				finally
				{
					connectionService.Release(dbConnection);	
				}				
			}
		}

		public void ExecuteParams(string query, IDbDataParameter[] dataParams, params object[] args) {
			var count = 5;
			while (count>0) {
				count--;
				var dbConnection = connectionService.Get();
				IDbCommand dbCommand = null;
				try {
					dbCommand = dbConnection.CreateCommand ();
					logger.Debug(string.Format("("+count+") " + query, args));
					dbCommand.CommandText = string.Format(query, args);
					if (dataParams != null)
						foreach(var p in dataParams)
							dbCommand.Parameters.Add(p);
					dbCommand.ExecuteNonQuery();
				} catch (DbException e) {
					logger.Error ($"SQLiteException:{e.Message}\n{e.StackTrace}\n{query}", e);
					if (count == 0)
						throw;
				}finally{
					dbCommand?.Dispose ();
					count = 0;
					connectionService.Release(dbConnection);
				}
			}

		}

		public void Execute(string query, params object[] args) => ExecuteParams(query, null, args);

		public object Query(DExecute exec, string query, params object[] args)
		{
			var count = 5;
			while (count > 0) {
				count--;
				object res = null;
				var dbConnection = connectionService.Get();
				IDbCommand dbCommand = null;
				IDataReader reader = null;
				try {
					dbConnection.Open();
					dbCommand = dbConnection.CreateCommand ();
					logger.Debug (string.Format("("+count+") " + query, args));
					dbCommand.CommandText = string.Format (query, args);
					reader = dbCommand.ExecuteReader ();
					res = exec.Invoke (reader);
				} catch (DbException e) {
					logger.Error($"SQLiteException:{e.Message}\n{e.StackTrace}\n{query}", e);
					if (count == 0) break;
				}finally {
					reader?.Close ();
					dbCommand?.Dispose ();
					connectionService.Release(dbConnection);
				}
				return res;
			}
			logger.Error("Max Retry Exception");
			throw new DbMaxRetryException();
		}

		public T One<T>(IObjectMapper<T> mapper, string query, params object[] args)
		{
			var count = 5;
			var res = default(T);
			while (count > 0) {
				count--;
				var dbConnection = connectionService.Get();
				IDbCommand dbCommand = null;
				IDataReader reader = null;
				try {
					dbConnection.Open();
					dbCommand = dbConnection.CreateCommand ();
					logger.Debug (string.Format("("+count+") " + query, args));
					dbCommand.CommandText = string.Format (query, args);
					reader = dbCommand.ExecuteReader ();
					res = mapper.Get(reader);
				} catch (DbException e) {
					logger.Error($"SQLiteException:{e.Message}\n{e.StackTrace}\n{query}", e);
					if (count == 0) break;
				}finally {
					reader?.Close ();
					dbCommand?.Dispose ();
					connectionService.Release(dbConnection);
				}
				return res;
			}
			logger.Error("Max Retry Exception");
			throw new DbMaxRetryException();			
		}

		public IEnumerable<T> Query<T>(IObjectMapper<T> mapper, string query, params object[] args)
		{
			var count = 5;
			var res = new List<T>();
			while (count > 0) {
				count--;
				var dbConnection = connectionService.Get();
				IDbCommand dbCommand = null;
				IDataReader reader = null;
				try {
					dbConnection.Open();
					dbCommand = dbConnection.CreateCommand ();
					logger.Debug (string.Format("("+count+") " + query, args));
					dbCommand.CommandText = string.Format (query, args);
					reader = dbCommand.ExecuteReader ();
					while (reader.Read())
						res.Add(mapper.Get(reader));
				} catch (DbException e) {
					logger.Error($"SQLiteException:{e.Message}\n{e.StackTrace}\n{query}", e);
					if (count == 0) break;
				}finally {
					reader?.Close ();
					dbCommand?.Dispose ();
					connectionService.Release(dbConnection);
				}
				return res;
			}
			logger.Error("Max Retry Exception");
			throw new DbMaxRetryException();			
		}

    }

}
