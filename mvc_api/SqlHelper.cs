using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DBUtility
{
    /// <summary> 
    /// 针对SQLServer数据库操作的通用类 
    /// </summary> 
    public class SqlDbHelper
    {
        #region 变量与构造函数

        /// <summary> 
        /// 设置数据库连接字符串 
        /// </summary> 
        private string connectionString;

        public string ConnectionString
        {
            set { connectionString = value; }
        }

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        public SqlDbHelper()
            : this(ConfigurationManager.ConnectionStrings["BankConnectionString"].ConnectionString)
        {
        }

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="connectionString">数据库连接字符串</param> 
        public SqlDbHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #endregion

        #region Transaction 采用事务处理

        /// <summary> 
        /// 取得一个SqlConnection对象 
        /// </summary> 
        /// <returns></returns> 
        public SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            return connection;
        }

        /// <summary> 
        /// 采用事务控制的对数据库执行增删改操作 
        /// </summary> 
        /// <param name="commandText">要执行的查询SQL文本命令</param> 
        /// <param name="connection">数据库连接对象,同一个事务要同一个连接对象</param> 
        /// <param name="transaction">事务对象,同一个事务对象</param> 
        /// <returns></returns> 
        public int ExecuteNonQuery(string commandText, SqlConnection connection, SqlTransaction transaction)
        {
            return ExecuteNonQuery(commandText, CommandType.Text, null, connection, transaction);
        }

        /// <summary> 
        /// 采用事务控制的对数据库执行增删改操作 
        /// </summary> 
        /// <param name="commandText">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param> 
        /// <param name="connection">数据库连接对象,同一个事务要同一个连接对象</param> 
        /// <param name="transaction">事务对象,同一个事务对象</param> 
        /// <returns>返回执行操作受影响的行数</returns> 
        public int ExecuteNonQuery(string commandText, CommandType commandType, SqlParameter[] parameters, SqlConnection connection, SqlTransaction transaction)
        {
            int rowsAffected = 0;
            using (SqlCommand cmd = new SqlCommand(commandText, connection, transaction))
            {
                cmd.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (SqlParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }
                rowsAffected = cmd.ExecuteNonQuery();
            }
            return rowsAffected;//返回执行增删改操作之后，数据库中受影响的行数 
        }

        #endregion

        #region ExecuteDataTable 返回DataTable对象

        /// <summary> 
        /// 执行一个查询，并返回结果集 
        /// </summary> 
        /// <param name="commandText">要执行的查询SQL文本命令</param> 
        /// <returns>返回查询结果集</returns> 
        public DataTable ExecuteDataTable(string commandText)
        {
            return ExecuteDataTable(commandText, CommandType.Text, null);
        }

        /// <summary> 
        /// 执行一个查询,并返回查询结果 
        /// </summary> 
        /// <param name="commandText">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <returns>返回查询结果集</returns> 
        public DataTable ExecuteDataTable(string commandText, CommandType commandType)
        {
            return ExecuteDataTable(commandText, commandType, null);
        }

        /// <summary> 
        /// 执行一个查询,并返回查询结果 
        /// </summary> 
        /// <param name="commandText">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param> 
        /// <returns></returns> 
        public DataTable ExecuteDataTable(string commandText, CommandType commandType, SqlParameter[] parameters)
        {
            DataTable data = new DataTable();//实例化DataTable，用于装载查询结果集 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType 
                    //如果同时传入了参数，则添加这些参数 
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    //通过包含查询SQL的SqlCommand实例来实例化SqlDataAdapter 
                    SqlDataAdapter adapter = new SqlDataAdapter(command);

                    adapter.Fill(data);//填充DataTable 
                }
            }
            return data;
        }

        #endregion

        #region ExecuteNonQuery 返回受影响行数

        /// <summary> 
        /// 对数据库执行增删改操作 
        /// </summary> 
        /// <param name="commandText">要执行的查询SQL文本命令</param> 
        /// <returns></returns> 
        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(commandText, CommandType.Text, null);
        }

        /// <summary> 
        /// 对数据库执行增删改操作 
        /// </summary> 
        /// <param name="commandText">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <returns></returns> 
        public int ExecuteNonQuery(string commandText, CommandType commandType)
        {
            return ExecuteNonQuery(commandText, commandType, null);
        }

        /// <summary> 
        /// 对数据库执行增删改操作 
        /// </summary> 
        /// <param name="commandText">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param> 
        /// <returns>返回执行操作受影响的行数</returns> 
        public int ExecuteNonQuery(string commandText, CommandType commandType, SqlParameter[] parameters)
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType 
                    //如果同时传入了参数，则添加这些参数 
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();//打开数据库连接 
                    count = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return count;//返回执行增删改操作之后，数据库中受影响的行数 
        }

        #endregion

        #region ExecuteReader 返回SqlDataReader对象

        /// <summary> 
        /// 将 CommandText 发送到 Connection 并生成一个 SqlDataReader。 
        /// </summary> 
        /// <param name="commandText">要执行的查询SQL文本命令</param> 
        /// <returns></returns> 
        public SqlDataReader ExecuteReader(string commandText)
        {
            return ExecuteReader(commandText, CommandType.Text, null);
        }

        /// <summary> 
        /// 将 CommandText 发送到 Connection 并生成一个 SqlDataReader。 
        /// </summary> 
        /// <param name="commandText">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <returns></returns> 
        public SqlDataReader ExecuteReader(string commandText, CommandType commandType)
        {
            return ExecuteReader(commandText, commandType, null);
        }

        /// <summary> 
        /// 将 CommandText 发送到 Connection 并生成一个 SqlDataReader。 
        /// </summary> 
        /// <param name="commandText">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param> 
        /// <returns></returns> 
        public SqlDataReader ExecuteReader(string commandText, CommandType commandType, SqlParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(commandText, connection);
            //如果同时传入了参数，则添加这些参数 
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            connection.Open();
            //CommandBehavior.CloseConnection参数指示关闭Reader对象时关闭与其关联的Connection对象 
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        #endregion

        #region ExecuteScalar 检索单个值

        /// <summary> 
        /// 从数据库中检索单个值（例如一个聚合值）。 
        /// </summary> 
        /// <param name="commandText">要执行的查询SQL文本命令</param> 
        /// <returns></returns> 
        public object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(commandText, CommandType.Text, null);
        }

        /// <summary> 
        /// 从数据库中检索单个值（例如一个聚合值）。 
        /// </summary> 
        /// <param name="commandText">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <returns></returns> 
        public object ExecuteScalar(string commandText, CommandType commandType)
        {
            return ExecuteScalar(commandText, commandType, null);
        }

        /// <summary> 
        /// 从数据库中检索单个值（例如一个聚合值）。 
        /// </summary> 
        /// <param name="commandText">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param> 
        /// <returns></returns> 
        public object ExecuteScalar(string commandText, CommandType commandType, SqlParameter[] parameters)
        {
            //object result = null; 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType 
                    //如果同时传入了参数，则添加这些参数 
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();//打开数据库连接 
                    //result = command.ExecuteScalar(); 
                    return command.ExecuteScalar();
                }
            }
            //return result;//返回查询结果的第一行第一列，忽略其它行和列 
        }

        #endregion

        #region ExecuteNonQueryReturnOutParameterValue

        /// <summary> 
        /// 对数据库执行增删改操作,此方法仅适合向数据增加一条记录并返回该记录的主键编号（该主键必须是自动递增的） 
        /// </summary> 
        /// <param name="commandText">要执行的SQL语句</param> 
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param> 
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param> 
        /// <param name="outParameterName">要输出的参数值的参数名</param> 
        /// <returns>返回整数类型的参数值</returns> 
        public int ExecuteNonQueryReturnOutParameterValue(string commandText, CommandType commandType, SqlParameter[] parameters, string outParameterName)
        {
            int value = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType 
                    //如果同时传入了参数，则添加这些参数 
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();//打开数据库连接 
                    if (command.ExecuteNonQuery() > 0)
                    {
                        value = int.Parse(command.Parameters[outParameterName].Value.ToString());
                    }
                    connection.Close();
                }
            }
            return value;//返回执行增删改操作之后，数据库中受影响的行数 
        }

        #endregion

        #region 其他类型

        /// <summary> 
        /// 返回当前连接的数据库中所有由用户创建的数据库 
        /// </summary> 
        /// <returns></returns> 
        public DataTable GetTables()
        {
            DataTable data = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();//打开数据库连接 
                data = connection.GetSchema("Tables");
            }
            return data;
        }

        /// <summary> 
        /// 将List&lt;int&gt;这样的整数集合转换成(1,3,5)这样的形式,以便在SQL语句中使用in 
        /// </summary> 
        /// <param name="intList">整数集合</param> 
        /// <returns></returns> 
        internal string ListToString(List<int> intList)
        {
            string result = " (";
            if (intList == null || intList.Count == 0)
            {
                //因为数据库中id字段都是自增的，所以不可能存在-1这样的id 
                //下面的做法只是为了让执行SQL语句时不报错 
                result = result + "-1";
            }
            else
            {
                int count = intList.Count;
                for (int i = 0; i < count - 1; i++)
                {
                    result = result + intList[i].ToString() + ",";
                }
                result = result + intList[count - 1].ToString();
            }
            result = result + ")";
            //最终将返回类似于" (1,3,4)"或者" (-1)"这样的结果 
            return result;
        }

        /// <summary> 
        /// 分页查询方法，适用于任何表或者视图 
        /// </summary> 
        /// <param name="tableName">要查询的表名或者视图名</param> 
        /// <param name="where">查询的where条件</param> 
        /// <param name="selectColumnName">要在in语句中查询的字段</param> 
        /// <param name="orderColumnName">要排序的字段名</param> 
        /// <param name="orderBy">排序方式</param> 
        /// <param name="startIndex">返回记录的起始位置</param> 
        /// <param name="size">返回的最大记录条数</param> 
        /// <param name="parameters">查询中用到的参数集合</param> 
        /// <returns>返回分页查询结果</returns> 
        //internal DataTable GetPagedDataTable(string tableName, string where, string selectColumnName, string orderColumnName, OrderBy orderBy, int startIndex, int size,SqlParameter[] parameters) 
        //{ 
        //    string orderByString = orderBy == OrderBy.ASC ? " ASC " : " DESC "; 
        //    StringBuilder buffer = new StringBuilder(1024); 
        //    buffer.AppendFormat("select top {0} * from {1} where {2} not in(", size, tableName, selectColumnName); 
        //    buffer.AppendFormat("select top {0} {1} from {2} where {3} order by {4} {5}", startIndex, selectColumnName, tableName,where,orderColumnName, orderByString); 
        //    buffer.AppendFormat(") and {0} order by {1} {2}", where, orderColumnName, orderByString); 
        //    string commandText = buffer.ToString(); 
        //    return ExecuteDataTable(commandText,CommandType.Text,parameters); 
        //} 
        #endregion

    }
}