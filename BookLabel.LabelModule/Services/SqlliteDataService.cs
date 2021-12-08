using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using BLToolkit.Data.Linq;
using BLToolkit.DataAccess;
using BLToolkit.Reflection;
using BLToolkit.Validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BookLabel.LabelModule.Services
{
    /// <summary>
    /// 数据库实体类的基础类，主要是封装了CUD操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ModelBase<T>
        where T : class
    {
        /// <summary>
        /// 当前实体类的表中所有的对象，用于访问表中所有的实体
        /// </summary>
        public static Table<T> Records
        {
            get
            {
                //初始化DBHelper
                RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);

                System.Diagnostics.Debug.Assert(DBConnection.DefaultConnection.ThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId);

                return DBConnection.DefaultConnection.GetTable<T>();
            }
        }

        #region CreateInstance

        public static T Create()
        {
            return TypeAccessor.CreateInstanceEx<T>();
        }

        public static T1 Create<T1>()
        {
            return TypeAccessor.CreateInstanceEx<T1>();
        }

        #endregion

        /// <summary>
        /// 插入当前数据实体对象到数据库中
        /// </summary>
        /// <returns>受影响记录的条数（1）</returns>
        public virtual int Insert()
        {
            System.Diagnostics.Debug.Assert(DBConnection.DefaultConnection.ThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId);

            return DBConnection.DefaultConnection.Insert(this as T);
        }

        /// <summary>
        /// 插入当前数据实体对象到数据库中
        /// </summary>
        /// <returns>受影响记录的条数（1）</returns>
        public virtual int Insert(DBConnection conn)
        {
            return conn.Insert(this as T);
        }

        /// <summary>
        /// 插入当前数据实体对象到数据库中
        /// </summary>
        /// <returns>新插入记录的自动增长字段的值（Identity）</returns>
        public virtual int InsertWithIdentity()
        {
            System.Diagnostics.Debug.Assert(DBConnection.DefaultConnection.ThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId);

            return Convert.ToInt32(DBConnection.DefaultConnection.InsertWithIdentity(this as T));
        }

        /// <summary>
        /// 按照关键字更新数据库中的当前数据实体
        /// 注意：此方法不能更新关键字
        /// </summary>
        /// <returns></returns>
        public virtual int Update()
        {
            System.Diagnostics.Debug.Assert(DBConnection.DefaultConnection.ThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId);

            return DBConnection.DefaultConnection.Update(this as T);
        }

        /// <summary>
        /// 按照关键字更新数据库中的当前数据实体
        /// 注意：此方法不能更新关键字
        /// </summary>
        /// <returns></returns>
        public virtual int Update(DBConnection conn)
        {
            return conn.Update(this as T);
        }

        /// <summary>
        /// 删除数据库中的当前数据实体
        /// </summary>
        /// <returns></returns>
        public virtual int Delete()
        {
            System.Diagnostics.Debug.Assert(DBConnection.DefaultConnection.ThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId);

            return DBConnection.DefaultConnection.Delete(this as T);
        }

        /// <summary>
        /// 删除数据库中的当前数据实体
        /// </summary>
        /// <returns></returns>
        public virtual int Delete(DBConnection conn)
        {
            return conn.Delete(this as T);
        }
    }

    public partial class DBConnection : DbManager
    {
        /// <summary>
        /// 当前线程默认操作的数据库连接对象
        /// </summary>
        [ThreadStatic]
        public static DBConnection DefaultConnection;

        public DBConnection(string providerName, string connectionString, DBConnectionPool dbpool)
            : base(new SQLiteDataProvider(), connectionString)
        {
            ConnectionString = connectionString;
            DBConnectionPool = dbpool;
            ThreadId = -1;
        }

        public DBConnection(string providerName, string connectionString)
            : base(new SQLiteDataProvider(), connectionString)
        {
            ConnectionString = connectionString;
            ThreadId = -1;
        }

        ~DBConnection()
        {
            if (DBConnection.DefaultConnection == this)
                Dispose();
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; internal set; }

        /// <summary>
        /// 线程ID
        /// 用于判断当前DBConnection对象是被哪个线程使用
        /// </summary>
        public int ThreadId { get; internal set; }

        private int refCount;
        /// <summary>
        /// 引用次数
        /// </summary>
        protected int RefCount { get { return refCount; } }

        /// <summary>
        /// 是否空闲
        /// </summary>
        private bool _isFree = true;
        public bool IsFree
        {
            get
            {
                return _isFree;
            }
            internal set
            {
                _isFree = value;

                //如果空闲，则通知
                if (_isFree && DBConnectionPool != null)
                {
                    DBConnectionPool.FreeEvent.Set();
                }
            }
        }

        /// <summary>
        /// DbManager对象
        /// </summary>
        public DbManager DbManager { get { return this; } }

        /// <summary>
        /// 连接池对象
        /// </summary>
        public DBConnectionPool DBConnectionPool { get; internal set; }

        /// <summary>
        /// 重新连接
        /// </summary>
        public bool ReConnect()
        {
            try
            {
                Close();
                Connection.Open();

                return Connection.State == ConnectionState.Open;
            }
            catch (Exception err)
            {
                //忽略错误，但是需要记录日志
                //LogHelper.LogErrMsg(err);
            }
            return false;
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public new void Close()
        {
            base.Close();
        }

        /// <summary>
        /// 添加引用
        /// </summary>
        public void AddRef()
        {
            Interlocked.Increment(ref refCount);

            System.Threading.Monitor.Enter(this); //使用中避免被关闭
        }

        /// <summary>
        /// 释放数据库连接对象
        /// </summary>
        public void Release()
        {
            Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                int count = Interlocked.Decrement(ref refCount);

                System.Threading.Monitor.Exit(this); //使用中避免被关闭

                if (count == 0)
                {
                    try
                    {
                        //回退事务，放弃所有修改
                        this.RollbackTransaction();
                    }
                    finally
                    {
                        //释放数据库连接对象到连接池中，供其他线程使用
                        ThreadId = -1;
                        IsFree = true;

                        //释放当前线程使用的数据库连接对象
                        if (DBConnection.DefaultConnection == this)
                            DBConnection.DefaultConnection = null;
                    }
                }
                else if (count < 0)
                {
                    refCount = 0;

                    System.Diagnostics.Trace.WriteLine("警告：数据库连接的RefCount < 0，将被释放！");

                    Close();
                }
            }
        }

        /// <summary>
        /// 操作异常处理
        /// </summary>
        /// <param name="op"></param>
        /// <param name="ex"></param>
        protected override void OnOperationException(OperationType op, BLToolkit.Data.DataException ex)
        {
            //记录错误日志
            switch (op)
            {
                case OperationType.PrepareCommand:
                    //LogHelper.LogErrMsg(string.Format("预编译SQL语句出错，错误：{0}，SQL语句：{1}", ex.Message, SelectCommand.CommandText));
                    break;

                case OperationType.Fill:
                case OperationType.Read:
                case OperationType.ExecuteReader:
                case OperationType.ExecuteNonQuery:
                    //LogHelper.LogErrMsg(string.Format("数据操作[{0}]出错，错误：{1}，SQL语句：{2}", op.ToString(), ex.Message, SelectCommand.CommandText));
                    break;

                case OperationType.Update:
                    //LogHelper.LogErrMsg(string.Format("数据操作[{0}]出错，错误：{1}，SQL语句：{2}", op.ToString(), ex.Message, UpdateCommand.CommandText));
                    break;

                default:
                    //LogHelper.LogErrMsg(string.Format("数据操作[{0}]出错，错误：{1}", op.ToString(), ex.Message));
                    break;
            }

            //记录日志
            base.OnOperationException(op, ex);
        }

        /// <summary>
        /// 修复表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="repairDatas">是否尽量修复数据</param>
        /// <returns></returns>
        public bool RepairTable(string tableName, bool repairDatas = true)
        {
            try
            {
                //如果之前有未提交事务，则此处取消
                if (this.Transaction != null)
                    this.RollbackTransaction();

                string sqlDefine = "";

                using (var results = this.SetCommand(string.Format("SELECT sql FROM sqlite_master WHERE name LIKE '{0}'", tableName)).ExecuteReader())
                {
                    if (results.Read())
                        sqlDefine = results[0] as string;
                }

                if (string.IsNullOrWhiteSpace(sqlDefine))
                    return false;

                string tmpTableName = string.Format("BACKUP_{0}_{1}_{2}", tableName, DateTime.Now.ToString("yyyyMMddHHmmss"), Guid.NewGuid().ToString().Replace("-", ""));

                //修复表结构
                this.BeginTransaction();
                this.SetCommand(string.Format("ALTER TABLE {0} RENAME TO {1};", tableName, tmpTableName)).ExecuteNonQuery();
                this.SetCommand(sqlDefine).ExecuteNonQuery();
                this.CommitTransaction();

                if (!repairDatas)
                    return true;

                //尝试修复数据
                int offset = 0;
                foreach (var step in new List<int>() { 10000, 1000, 100, 10, 1 })
                {
                    try
                    {
                        while (true)
                        {
                            //将老表中的数据Copy至新的表
                            this.SetCommand(string.Format("INSERT INTO {0} SELECT * FROM {1} LIMIT {2} OFFSET {3};", tableName, tmpTableName, step, offset)).ExecuteNonQuery();
                            offset += step;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                return true;
            }
            catch (Exception err)
            {
                //忽略错误，但是需要记录日志
                //LogHelper.LogErrMsg(err, "修复表{0}失败", tableName);
            }

            return false;
        }
    }

    /// <summary>
    /// 数据库操作辅助文件
    /// </summary>
    public class DBHelper
    {

        /// <summary>
        /// ORM是否初始化
        /// 静态变量，整个进程只需要初始化一次
        /// </summary>
        internal static bool ORMInitialized = false;

        static DBHelper()
        {
            //初始化ORM
            //InitializeORM();
        }

        /// <summary>
        /// 初始化ORM
        /// 因为第一次使用ORM时比较耗时，所以需要提前初始化
        /// </summary>
        private static void InitializeORM()
        {
            //只需要初始化一次
            if (!ORMInitialized)
            {
                ORMInitialized = true;

                ThreadPool.QueueUserWorkItem((WaitCallback)delegate
                {
                    try
                    {

                        //创建一个临时数据库，用于初始化ORM的操作
                        string dbpath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                        while (System.IO.File.Exists(dbpath))
                            dbpath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                        using (DBConnectionPool pool = new DBConnectionPool("SQLite", string.Format("Data Source={0};Version=3;Journal Mode=Off;Synchronous=Off;", dbpath), dbpath, 1))
                        {
                            //初始化/升级数据库
                            using (DBConnection conn = pool.GetConnection())
                            {
                                //获取数据库版本信息
                                int curVersion = GetDBVersion(conn);

                                DBVERSION version = DBVERSION.Records.Where(x => x.NAME == "DB").FirstOrDefault();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //LogHelper.LogErrMsg("初始化ORM异常 {0}", ex.ToString());
                    }
                });
            }
        }

        /// <summary>
        /// 创建数据库连接池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbPath"></param>
        /// <param name="maxConnNum">因为Sqlite多线程并发存在问题，所以默认只允许一个连接对象</param>
        /// <param name="password"></param>
        /// <param name="retry">报错时，尝试删除DB文件重新连接，重试次数，0表示不删除</param>
        /// <returns></returns>
        public static DBConnectionPool CreateDBConnectionPool<T>(string dbPath, int maxConnNum = 1, string password = null, int retry = 0)
            where T : DBUpdater
        {
            if (string.IsNullOrEmpty(password))
                password = "06zSah3QyW8e";

            //获取绝对路径
            dbPath = Path.GetFullPath(dbPath);

            //如果路径(目录)不存在，则创建目录
            if (!Directory.Exists(Path.GetDirectoryName(dbPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

            //打开/创建数据库连接
            DBConnectionPool pool = new DBConnectionPool("SQLite", string.Format("Data Source={0};Version=3;Journal Mode=Off;Synchronous=Off;Password={1};", dbPath, password), dbPath, maxConnNum);

            //初始化/升级数据库
            try
            {
                using (DBConnection conn = pool.GetConnection())
                {
                    if (!typeof(T).IsAbstract)
                    {
                        DBUpdater dbu = Activator.CreateInstance(typeof(T)) as DBUpdater;

                        #region 检查DB中是否有坏掉的Table，并尝试修复

                        //检查DB中是否有坏掉的Table，并尝试修复
                        List<string> repairTables = new List<string>();
                        {
                            //列举所有的表名称
                            using (var reader = conn.SetCommand("select name from sqlite_master where type='table'").ExecuteReader())
                            {
                                while (reader.Read())
                                    repairTables.Add(reader[0] as string);
                            }

                            //排除那些备份表（上一次修复时的备份表）
                            repairTables.RemoveAll(x => x.StartsWith("BACKUP_"));

                            //检查所有表的状态是否正常
                            foreach (var table in repairTables.ToList())
                            {
                                try
                                {
                                    int count = conn.SetCommand(string.Format("select count(*) from {0}", table)).ExecuteScalar<int>();
                                    //正常
                                    repairTables.Remove(table);
                                }
                                catch (Exception)
                                {
                                }
                            }

                            //开始修复
                            foreach (var table in repairTables)
                            {
                                if (string.Compare(table, "DBVersion", true) == 0)
                                    conn.RepairTable(table, true);
                                else
                                    conn.RepairTable(table, dbu.NeedPreserveData(table));
                            }
                        }

                        #endregion

                        //获取数据库版本信息
                        int curVersion = GetDBVersion(conn);

                        //升级
                        dbu.UpdateDB(curVersion, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                //LogHelper.LogErrMsg("创建数据库连接池异常:{0}", ex.ToString());

                if (retry > 0)
                {
                    //这里抛异常说明文件可能损坏了，尝试删除整个DB文件，然后重新创建
                    pool.Dispose();
                    File.Delete(dbPath);
                    return CreateDBConnectionPool<T>(dbPath, maxConnNum, password, retry - 1);
                }
                else
                    throw;
            }

            return pool;
        }

        /// <summary>
        /// 升级数据库中的表结构
        /// </summary>
        public static void UpdateTableSchema<T>(DBConnectionPool pool)
            where T : IDBUpdater
        {
            //初始化/升级数据库
            using (DBConnection conn = pool.GetConnection())
            {
                if (!typeof(T).IsAbstract)
                {
                    IDBUpdater dbu = Activator.CreateInstance(typeof(T)) as IDBUpdater;

                    #region 检查DB中是否有坏掉的Table，并尝试修复

                    //检查DB中是否有坏掉的Table，并尝试修复
                    List<string> repairTables = new List<string>();
                    {
                        //列举所有的表名称
                        using (var reader = conn.SetCommand("select name from sqlite_master where type='table'").ExecuteReader())
                        {
                            while (reader.Read())
                                repairTables.Add(reader[0] as string);
                        }

                        //排除那些备份表（上一次修复时的备份表）
                        repairTables.RemoveAll(x => x.StartsWith("BACKUP_"));

                        //检查所有表的状态是否正常
                        foreach (var table in repairTables.ToList())
                        {
                            try
                            {
                                int count = conn.SetCommand(string.Format("select count(*) from {0}", table)).ExecuteScalar<int>();
                                //正常
                                repairTables.Remove(table);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        //开始修复
                        foreach (var table in repairTables)
                        {
                            if (string.Compare(table, "DBVersion", true) == 0)
                                conn.RepairTable(table, true);
                            else
                                conn.RepairTable(table, dbu.NeedPreserveData(table));
                        }
                    }

                    #endregion

                    //获取数据库版本信息
                    int curVersion = GetDBVersion(conn, typeof(T).Name);

                    //升级
                    dbu.UpdateDB(curVersion, conn);
                }
            }
        }

        /// <summary>
        /// 初始化数据库版本信息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="name"></param>
        /// <returns>数据库版本信息</returns>
        static int GetDBVersion(DBConnection conn, string name = "Database")
        {
            if (conn == null)
                return -1;

            if (conn.Transaction != null)
            {
                Exception ex = new Exception("初始化数据表时不允许使用事务");
                ex.Data["tablename"] = name;
                throw ex;
            }

            //获取当前的数据库版本
            int curVersion = -1;

            //确保DBVersion表存在
            conn.SetCommand(@"CREATE TABLE IF NOT EXISTS DBVersion(Name varchar(20) PRIMARY KEY, Version int);").ExecuteNonQuery();

            System.Data.IDataReader reader = conn.SetCommand("select count(*) count, max(VERSION) version from DBVERSION where Name == @Name",
                conn.Parameter("@Name", name)).ExecuteReader();

            int count = 0;

            if (reader != null && reader.Read())
            {
                count = reader.GetInt32(0);
                if (count > 0)
                    curVersion = reader.GetInt32(1);

                reader.Close();
            }

            if (count <= 0)
            {
                conn.SetCommand(string.Format("insert into DBVersion(Name,Version) values('{0}', -1)", name)).ExecuteNonQuery();
            }

            return curVersion;
        }
    }

    /// <summary>
    /// 数据库版本控制表
    /// </summary>
    [TableName(Name = "DBVersion")]
    internal class DBVERSION : ModelBase<DBVERSION>
    {
        [PrimaryKey(0), Required]
        public string NAME { get; set; }
        [Required]
        public int VERSION { get; set; }
    }

    /// <summary>
    /// 数据库连接池
    /// </summary>
    public class DBConnectionPool : IDisposable
    {
        [DllImport("kernel32.dll")]
        public static extern int SetProcessWorkingSetSize(IntPtr proc, int min, int max);

        /// <summary>
        /// 连接池有新的空闲对象时间
        /// </summary>
        public AutoResetEvent FreeEvent { get; set; }

        /// <summary>
        /// DBAL日志对象
        /// </summary>
        //private static Log4cb.Log4cbHelper DBALLog = null;
        private static int logRefCount = 0;

        static DBConnectionPool()
        {
            //对所有的SQL进行记录
            DBConnection.TraceSwitch = new TraceSwitch("DB", "DB执行语句追踪") { Level = System.Diagnostics.TraceLevel.Verbose };
            //DBConnection.WriteTraceLine = (message, displayName) =>
            //{
            //    if (DBALLog == null) return;

            //    message = message.Replace("\r\n", "\t");
            //    DBALLog.LogDebugMsg(message);
            //};
        }

        public DBConnectionPool(string providerName, string connectionString, string dbPath, int maxConnNum)
        {
            DataProviderName = providerName;
            ConnectionString = connectionString;
            MaxConnNum = Math.Max(1, maxConnNum); //Sqlite只支持单线程,最多一个连接
            Connections = new List<DBConnection>(MaxConnNum);

            FreeEvent = new AutoResetEvent(false);

            //创建日志对象
            logRefCount++;
            //if (DBALLog == null)
            //    DBALLog = new Log4cb.Log4cbHelper("DBAL", Log4cb.BuiltInModules.DBAL);

            #region 如果是sqlite数据库，需要测试一下db文件格式是否完整正确

            //如果是sqlite数据库，需要测试一下db文件格式是否完整正确
            if (providerName == ProviderName.SQLite)
            {
                try
                {
                    //测试db文件是否能够正常打开
                    using (var conn = GetConnection(5000))
                    {
                    }
                }
                catch (Exception err)
                {
                    if (err.InnerException != null && err.InnerException is BLToolkit.Data.DataException
                        && !string.IsNullOrEmpty(err.InnerException.Message)
                        && err.InnerException.Message.Contains("is not a database")
                        )
                    {
                        try
                        {
                            //清除之前的连接池连接对象（失效了）
                            lock (Connections)
                                Connections.Clear();

                            //备份db文件
                            string fileName = Path.GetFileName(dbPath);
                            File.Move(dbPath, Path.Combine(Path.GetDirectoryName(dbPath), string.Format("BACKUP_{0}_{1}{2}", Path.GetFileNameWithoutExtension(fileName), DateTime.Now.ToString("yyyyMMddHHmmss"), Path.GetExtension(fileName))));
                        }
                        catch (Exception) { }
                    }
                }
            }

            #endregion
        }

        ~DBConnectionPool()
        {
            Dispose();
        }

        public void Dispose()
        {
            ClearAllConnection();

            if (logRefCount > 0)
            {
                logRefCount--;

                //释放日志对象
                //if (logRefCount == 0 && DBALLog != null)
                //{
                //    DBALLog.Dispose();
                //    DBALLog = null;
                //}
            }
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; internal set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DataProviderName { get; internal set; }

        /// <summary>
        /// 连接池允许的最大连接数
        /// </summary>
        public int MaxConnNum { get; internal set; }

        /// <summary>
        /// 连接池当前所有的连接
        /// </summary>
        internal List<DBConnection> Connections { get; set; }

        /// <summary>
        /// 获取一个空闲的数据库连接对象
        /// </summary>
        /// <returns></returns>
        public DBConnection GetConnection()
        {
            //默认超时时间是20s
            return GetConnection(
#if DEBUG
                long.MaxValue
#else
				20000
#endif
                , true);
        }

        /// <summary>
        /// 获取一个空闲的数据库连接对象
        /// </summary>
        /// <param name="usedByCurrentThread">是否获取当前线程已经获取的连接对象</param>
        /// <returns></returns>
        public DBConnection GetConnection(bool usedByCurrentThread)
        {
            //默认超时时间是20s
            return GetConnection(
#if DEBUG
                long.MaxValue
#else
				20000
#endif
                , usedByCurrentThread);
        }

        /// <summary>
        /// 获取一个空闲的数据库连接对象
        /// </summary>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public DBConnection GetConnection(long timeout)
        {
            return GetConnection(timeout, true);
        }

        /// <summary>
        /// 获取一个空闲的数据库连接对象
        /// </summary>
        /// <param name="timeout">超时时间，单位毫秒ms</param>
        /// <param name="usedByCurrentThread">是否获取当前线程已经获取的连接对象</param>
        /// <returns></returns>
        public DBConnection GetConnection(long timeout, bool usedByCurrentThread)
        {
            DateTime dtStart = DateTime.Now;
            DBConnection conn = null;

            //首先查看当前线程是否已经获取了一个数据库连接对象，如果有，则返回
            if (usedByCurrentThread)
            {
                try
                {
                    //避免多个线程同时获取连接
                    lock (Connections)
                    {
                        conn = Connections.FirstOrDefault(x => !x.IsFree && x.ThreadId == Thread.CurrentThread.ManagedThreadId);
                        if (conn != null)
                        {
                            lock (conn)
                            {
                                conn.AddRef();
                                conn.IsFree = false;
                                conn.ThreadId = Thread.CurrentThread.ManagedThreadId;
                            }

                            //检查线程默认的数据库连接对象
                            Debug.Assert(DBConnection.DefaultConnection == conn);
                        }
                    }
                }
                catch (Exception err)
                {
                    throw new Exception(string.Format("获取同线程正在使用的DBConnection出错，信息：{0}，堆栈: {1}", err.ToString(), err.StackTrace.ToString()), err);
                }
            }

            //当前连接是否一切都OK的
            bool bOk = conn != null && conn.Connection.State == ConnectionState.Open;

            //循环判断是否有空闲数据库连接，并能够连接到数据库，直至超时
            while (!bOk && (DateTime.Now - dtStart).TotalMilliseconds < timeout)
            {
                try
                {
                    if (conn == null)
                    {
                        lock (Connections)
                        {
                            //查询是否有空闲的连接
                            conn = Connections.FirstOrDefault(x => x.IsFree);

                            //如果没有空闲连接，但连接池还未满，则新建连接
                            if (conn == null && Connections.Count < MaxConnNum)
                            {
                                //新建连接
                                conn = new DBConnection(DataProviderName, ConnectionString, this);

                                //添加到连接池
                                Connections.Add(conn);
                            }

                            //修改连接的空闲状态
                            if (conn != null)
                            {
                                lock (conn)
                                {
                                    conn.AddRef();
                                    conn.IsFree = false;
                                    conn.ThreadId = Thread.CurrentThread.ManagedThreadId;
                                }
                            }
                        }
                    }

                    //如果没有可用的连接，则等待
                    if (conn == null)
                    {
                        Thread.Sleep(5);
                        //FreeEvent.WaitOne(100);
                        continue;
                    }

                    //确保连接到数据库
                    try
                    {
                        if (conn != null && (conn.Connection.State == ConnectionState.Open || conn.ReConnect()))
                        {
                            bOk = true;
                            break;
                        }
                        else
                        {
                            //每次循环间都等待100ms
                            Thread.Sleep(10);
                        }
                    }
                    catch (Exception err)
                    {
                        conn = null;
                        //忽略错误，但是需要记录日志
                        //LogHelper.LogErrMsg(err, "连接数据库失败，原因：{0}", err.Message);
                    }
                }
                catch (Exception err)
                {
                    conn = null;
                    //LogHelper.LogErrMsg(err, "获取空闲DBConnection或创建新DBConnection出错, 原因：{0}", err.Message);
                    throw new Exception(string.Format("获取空闲DBConnection或创建新DBConnection出错，信息：{0}，堆栈: {1}", err.ToString(), err.StackTrace.ToString()), err);
                }
            }

            //更新当前连接对象的线程ID标识
            if (conn == null)
            {
                try
                {
                    lock (Connections)
                    {
                        //将当前所有数据库连接的状态枚举出来
                        string sErr = "没有获取到数据库连接！ 连接池详细信息：连接个数（" + Connections.Count.ToString() + "）";
                        foreach (var cn in Connections)
                        {
                            sErr += "\r\n" + ((cn == null || cn.Connection == null) ? "NULL" :
                                ("IsFree:" + cn.IsFree.ToString()
                                + ", ThreadID:" + cn.ThreadId.ToString()
                                + ", State:" + cn.Connection.State.ToString()));
                        }

                        Debug.Assert(false, sErr);
                    }
                }
                catch (Exception err)
                {
                    //LogHelper.LogErrMsg(err, "未获取有效的DBConnection，打印ConnectionPool信息出错， 原因：{0}", err.Message);
                    throw new Exception(string.Format("未获取有效的DBConnection，打印ConnectionPool信息出错，信息：{0}，堆栈: {1}", err.ToString(), err.StackTrace.ToString()), err);
                }
            }

            //设置默认连接对象
            if (usedByCurrentThread && conn != null)
                DBConnection.DefaultConnection = conn;

            return conn;
        }

        /// <summary>
        /// 清空连接池内所有的数据库连接
        /// </summary>
        internal void ClearAllConnection()
        {
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);

            //释放数据库连接对象
            foreach (DBConnection conn in Connections)
            {
                lock (conn)
                {
                    conn.Close();
                }
            }

            Connections.Clear();
        }
    }

    /// <summary>
	/// 数据库升级类接口
	/// </summary>
    public interface IDBUpdater
    {
        /// <summary>
        /// 升级DB
        /// </summary>
        /// <param name="version"></param>
        /// <param name="conn"></param>
        void UpdateDB(int version, DBConnection conn);

        /// <summary>
        /// 修复数据时是否保留数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        bool NeedPreserveData(string tableName);
    }

    /// <summary>
    /// 数据库升级操作
    /// </summary>
    public class UpdateAction
    {
        /// <summary>
        /// 版本信息
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 升级操作对应的SQL语句
        /// </summary>
        public string UpdateSQL { get; set; }
    }

    public abstract class DBPartUpdater<T> : IDBUpdater
    {
        protected static Dictionary<Type, List<UpdateAction>> Actions = new Dictionary<Type, List<UpdateAction>>();
        protected static Dictionary<Type, int> Versions = new Dictionary<Type, int>();

        /// <summary>
        /// 添加升级操作SQL语句
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sql"></param>
        protected static void AddUpdateAction(Type type, string sql)
        {
            if (!Versions.ContainsKey(type)) Versions[type] = 0;
            if (!Actions.ContainsKey(type)) Actions[type] = new List<UpdateAction>();

            UpdateAction ua = new UpdateAction { Version = Versions[type]++, UpdateSQL = sql };
            Actions[type].Add(ua);
        }

        /// <summary>
        /// 当前数据库需要进行的升级操作
        /// </summary>
        List<UpdateAction> NeedUpdateActions;

        public DBPartUpdater()
        {
        }

        /// <summary>
        /// 升级操作
        /// </summary>
        /// <param name="version"></param>
        /// <param name="conn"></param>
        public void UpdateDB(int version, DBConnection conn)
        {
            NeedUpdateActions = new List<UpdateAction>(Actions[this.GetType()].Where(x => x.Version > version));

            if (conn == null || NeedUpdateActions.Count == 0) return;

            if (conn.Transaction != null)
            {
                Exception ex = new Exception("初始化数据表时不允许使用事务");
                throw ex;
            }

            NeedUpdateActions.ForEach((x) =>
            {
                if (!string.IsNullOrEmpty(x.UpdateSQL))
                {
                    //执行升级SQL
                    conn.SetCommand(x.UpdateSQL).ExecuteNonQuery();

                    //更新版本
                    version = x.Version;
                }
            });

            //更新版本
            conn.SetCommand("update DBVersion set Version=@Version where Name=@Name", conn.Parameter("@Version", version), conn.Parameter("@Name", typeof(T).Name)).ExecuteNonQuery();
        }

        /// <summary>
        /// 修复数据时是否保留数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual bool NeedPreserveData(string tableName)
        {
            return true;
        }
    }

    /// <summary>
    /// 虚拟的整个数据库表
    /// </summary>
    public abstract class Database { }

    public abstract class DBUpdater : DBPartUpdater<Database>
    {
    }

}
