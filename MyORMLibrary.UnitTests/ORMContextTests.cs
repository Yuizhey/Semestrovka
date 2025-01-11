﻿using NUnit.Framework;
using System;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;
using System.Data;
using ORMLibrary.Tests.Models;
using System.Data;
using Moq;
using MyORMLibrary;


namespace ORMLibrary.Tests;

public class ORMContextTests
{
    [SetUp]
    public void Setup()
    {
        _dbConnection = new Mock<IDbConnection>();
        _dbCommand = new Mock<IDbCommand>();
        _dbDataReader = new Mock<IDataReader>();
        _dataParameterCollection = new Mock<IDataParameterCollection>();

        // Настройка параметров для команды
        _dbCommand.Setup(c => c.Parameters).Returns(_dataParameterCollection.Object);

        _dbConnection.Setup(c => c.CreateCommand()).Returns(_dbCommand.Object);

        _dbContext = new OrmContext<UserInfo>(_dbConnection.Object);
    }

    [Test]
    public void GetById_When_()
    {
        // var _dbConnection = new Mock<IDbConnection>();
        // var _dbCommand = new Mock<IDbCommand>();
        // var _dbDataReader = new Mock<IDataReader>();
        var _dataParametr = new Mock<IDbDataParameter>();
        // var _dataParametrCollection = new Mock<IDataParameterCollection>();

        _dbContext = new OrmContext<UserInfo>(_dbConnection.Object);
        
        var userId = 1;
        var userInfo = new UserInfo()
        {
            Id = 1,
            Age = 20,
            Email = "exaple@test.com",
            Name = "Иванов Иван Иванович",
            Gender = 1
        };
        
        var data = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>
            {
                {"Id", userInfo.Id},
                {"Age", userInfo.Age},
                {"Email", userInfo.Email},
                {"Name", userInfo.Name},
                {"Gender", userInfo.Gender}
        
            }
        };
        
        _dbDataReader.SetupSequence(r => r.Read())
            .Returns(true)
            .Returns(false);
        _dbDataReader.Setup(r => r["Id"]).Returns(userInfo.Id);
        _dbDataReader.Setup(r => r["Age"]).Returns(userInfo.Age);
        _dbDataReader.Setup(r => r["Email"]).Returns(userInfo.Email);
        _dbDataReader.Setup(r => r["Name"]).Returns(userInfo.Name);
        _dbDataReader.Setup(r => r["Gender"]).Returns(userInfo.Gender);
        
        _dbCommand.Setup(c => c.ExecuteReader()).Returns(_dbDataReader.Object);
        _dbCommand.Setup(c => c.CreateParameter()).Returns(_dataParametr.Object);
        _dbCommand.Setup(c => c.Parameters).Returns(_dataParameterCollection.Object);
        _dataParameterCollection.Setup(pc => pc.Add(It.IsAny<Object>())).Returns(userId);
        _dbConnection.Setup(c => c.CreateCommand()).Returns(_dbCommand.Object);
        
        var result = _dbContext.ReadById(userId, tableName:"");
        
        Assert.IsNotNull(result);
        Assert.AreEqual(userInfo.Id, result.Id);
        Assert.AreEqual(userInfo.Age, result.Age);
        Assert.AreEqual(userInfo.Email, result.Email);
        Assert.AreEqual(userInfo.Name, result.Name);
        Assert.AreEqual(userInfo.Gender, result.Gender);
    }
    
    private OrmContext<UserInfo> _dbContext;
    private Mock<IDbConnection> _dbConnection;
    private Mock<IDbCommand> _dbCommand;
    private Mock<IDataReader> _dbDataReader;
    private Mock<IDataParameterCollection> _dataParameterCollection;

    [Test]
    public void Create_ShouldInsertRecordAndSetId()
    {
        var userInfo = new UserInfo
        {
            Age = 25,
            Email = "test@example.com",
            Name = "John Doe",
            Gender = 1
        };

        // Настройка мока для ExecuteScalar
        _dbCommand.Setup(c => c.ExecuteScalar()).Returns(1); // Возвращаем Id новой записи (например, 1)

        // Настройка мока для параметров
        var mockParameter = new Mock<IDbDataParameter>();
        _dbCommand.Setup(c => c.CreateParameter()).Returns(mockParameter.Object);

        // Вызов метода Create, который должен вставить запись и вернуть объект с Id
        var result = _dbContext.Create(userInfo, "Users");

        // Проверка, что результат не null и что Id был установлен
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id); // Проверка, что Id был установлен

        // Проверка, что ExecuteScalar был вызван
        _dbCommand.Verify(c => c.ExecuteScalar(), Times.Once); // Проверка, что ExecuteScalar был вызван

        // Проверка, что параметры были добавлены в команду
        _dataParameterCollection.Verify(pc => pc.Add(It.IsAny<object>()), Times.AtLeastOnce);
    }



    [Test]
    public void GetAll_ShouldReturnListOfRecords()
    {
        _dbDataReader.SetupSequence(r => r.Read())
            .Returns(true)
            .Returns(true)
            .Returns(false); // Две записи в таблице
        _dbDataReader.Setup(r => r["Id"]).Returns(1);
        _dbDataReader.Setup(r => r["Age"]).Returns(30);
        _dbDataReader.Setup(r => r["Email"]).Returns("test1@example.com");
        _dbDataReader.Setup(r => r["Name"]).Returns("Alice");
        _dbDataReader.Setup(r => r["Gender"]).Returns(2);

        _dbCommand.Setup(c => c.ExecuteReader()).Returns(_dbDataReader.Object);

        var result = _dbContext.GetAll("Users");

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count); // Проверяем, что две записи возвращены
        Assert.AreEqual(1, result[0].Id);
        Assert.AreEqual(30, result[0].Age);
    }

    [Test]
    public void Update_ShouldUpdateRecord()
    {
        var userInfo = new UserInfo
        {
            Id = 1,
            Age = 28,
            Email = "updated@example.com",
            Name = "Updated Name",
            Gender = 1
        };

        var mockParameter = new Mock<IDbDataParameter>();
        _dbCommand.Setup(c => c.CreateParameter()).Returns(mockParameter.Object); // Создание параметра
        _dbCommand.Setup(c => c.ExecuteNonQuery()).Returns(1); // Обновление проходит успешно
        
        _dataParameterCollection.Setup(pc => pc.Add(It.IsAny<object>())).Callback<object>((param) =>
        {
            var dbParameter = param as IDbDataParameter; // Приведение к типу IDbDataParameter
        });
        
        _dbContext.Update(userInfo.Id, userInfo, "Users");
        
        _dbCommand.Verify(c => c.ExecuteNonQuery(), Times.Once);
        
        _dataParameterCollection.Verify(pc => pc.Add(It.IsAny<object>()), Times.AtLeastOnce);
        
        _dbCommand.VerifySet(c => c.CommandText = It.IsAny<string>(), Times.Once);
    }



    [Test]
    public void Delete_ShouldDeleteRecord()
    {
        // Настройка для мока параметра и команды
        var mockParameter = new Mock<IDbDataParameter>();
        _dbCommand.Setup(c => c.CreateParameter()).Returns(mockParameter.Object); // Создание параметра
        _dbCommand.Setup(c => c.ExecuteNonQuery()).Returns(1); // Удаление проходит успешно

        // Настройка мока для добавления параметра
        _dataParameterCollection.Setup(pc => pc.Add(It.IsAny<object>())).Callback<object>((param) =>
        {
            // Если нужно, здесь можно работать с параметрами
        });

        // Вызов метода, который должен выполнить удаление
        _dbContext.Delete(1, "Users");

        // Проверка, что ExecuteNonQuery был вызван
        _dbCommand.Verify(c => c.ExecuteNonQuery(), Times.Once); // Проверка, что ExecuteNonQuery был вызван

        // Проверка, что параметры были добавлены
        _dataParameterCollection.Verify(pc => pc.Add(It.IsAny<object>()), Times.AtLeastOnce);

        // Проверка, что CommandText был установлен правильно
        _dbCommand.VerifySet(c => c.CommandText = It.IsAny<string>(), Times.Once);
    }

}