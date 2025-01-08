using System.Data;
using System.Linq.Expressions;
using MyORMLibrary;

namespace MyORMLibrary;

public class OrmContext<T> where T : class, new()
{
    private readonly IDbConnection _dbConnection;

    public OrmContext(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public T ReadById(int Id, string tableName)
    {
        string query = $"SELECT * FROM {tableName} WHERE Id = @Id";

        try
        {
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = query;
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Id";
                parameter.Value = Id;
                command.Parameters.Add(parameter);

                _dbConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Map(reader);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            _dbConnection.Close();
        }

        return null;
    }

    public T Create(T entity, string tableName)
    {
        var properties = typeof(T).GetProperties();
        var columnNames = string.Join(", ", properties.Where(p => p.Name != "Id").Select(p => p.Name));  // Не включаем Id
        var parameterNames = string.Join(", ", properties.Where(p => p.Name != "Id").Select(p => "@" + p.Name));  // Не включаем Id

        string sql = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames}); SELECT SCOPE_IDENTITY();";

        Console.WriteLine($"Executing SQL: {sql}");

        try
        {
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                foreach (var property in properties.Where(p => p.Name != "Id"))  // Не передаем Id
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = $"@{property.Name}";
                    parameter.Value = property.GetValue(entity) ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }

                _dbConnection.Open();
                var result = command.ExecuteScalar();
                Console.WriteLine($"Insert result: {result}");

                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    var idProperty =
                        properties.FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
                    idProperty?.SetValue(entity, newId); // Присваиваем Id, полученное от базы данных
                }
            }
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            _dbConnection.Close();
        }

        return entity;
    }

    public List<T> GetAll(string tableName)
    {
        List<T> results = new List<T>();
        string sql = $"SELECT * FROM {tableName}";

        try
        {
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                _dbConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(Map(reader));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            _dbConnection.Close();
        }

        return results;
    }

    public void Update(int id, T entity, string tableName)
    {
        var properties = typeof(T).GetProperties();
        var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
        string sql = $"UPDATE {tableName} SET {setClause} WHERE Id = @Id";
        try
        {
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                foreach (var property in properties)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = $"@{property.Name}";
                    parameter.Value = property.GetValue(entity) ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }

                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "@Id";
                idParameter.Value = id;
                command.Parameters.Add(idParameter);

                _dbConnection.Open();
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            _dbConnection.Close();
        }
    }

    public void Delete(int id, string tableName)
    {
        string sql = $"DELETE FROM {tableName} WHERE Id = @Id";

        try
        {
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Id";
                parameter.Value = id;
                command.Parameters.Add(parameter);

                _dbConnection.Open();
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            _dbConnection.Close();
        }
    }

    private T Map(IDataReader reader)
    {
        var obj = new T();
        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            if (reader[property.Name] != DBNull.Value)
            {
                property.SetValue(obj, Convert.ChangeType(reader[property.Name], property.PropertyType));
            }
        }

        return obj;
    }

    public List<T> Where(string condition, string tableName)
    {
        List<T> results = new List<T>();
        string sql = $"SELECT * FROM {tableName} WHERE {condition}";

        try
        {
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                _dbConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(Map(reader));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            _dbConnection.Close();
        }

        return results;
    }
    public T FirstOrDefault(Expression<Func<T, bool>> predicate)
    {
        var sqlQuery = ExpressionParser<T>.BuildSqlQuery(predicate, singleResult: true);
        return ExecuteQuerySingle(sqlQuery);
    }

    public IEnumerable<T> Where(Expression<Func<T, bool>> predicate)
    {
        var sqlQuery = ExpressionParser<T>.BuildSqlQuery(predicate, singleResult: false);
        return ExecuteQueryMultiple(sqlQuery).ToList();
    }

    private T ExecuteQuerySingle(string query)
    {
        using (var command = _dbConnection.CreateCommand())
        {
            command.CommandText = query;
            _dbConnection.Open();
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return Map(reader);
                }
            }
            _dbConnection.Close();
        }

        return null;
    }
    public T FirstOrDefaultByLogin(string login)
    {
        string sqlQuery = $"SELECT * FROM Users WHERE Login = @Login";
    
        try
        {
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = sqlQuery;
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@Login";
                parameter.Value = login;
                command.Parameters.Add(parameter);
    
                _dbConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Map(reader);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            _dbConnection.Close();
        }
    
        return null;
    }
    private IEnumerable<T> ExecuteQueryMultiple(string query)
    {
        var results = new List<T>();
        using (var command = _dbConnection.CreateCommand())
        {
            command.CommandText = query;
            _dbConnection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    results.Add(Map(reader));
                }
            }
            _dbConnection.Close();
        }
        return results;
    }


}