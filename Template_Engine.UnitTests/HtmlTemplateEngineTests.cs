using NUnit.Framework;
using System;
using System.Collections.Generic;
using TemplateEngine.Tests.Models;

namespace TemplateEngine.Tests
{
    public class HtmlTemplateEngineTests
    {
        private HtmlTemplateEngine _templateEngine;

        [SetUp]
        public void SetUp()
        {
            _templateEngine = new HtmlTemplateEngine();
        }

        [Test]
        public void Render_WithPlaceholderAndData_ShouldReplacePlaceholderWithData()
        {
            var template = "Hello, {name}!";
            var placeholder = "{name}";
            var data = "John";

            var result = _templateEngine.Render(template, placeholder, data);

            Assert.AreEqual("Hello, John!", result);
        }

        [Test]
        public void Render_WithObject_ShouldReplacePlaceholdersWithObjectProperties()
        {
            var template = "Name: {name}, Age: {age}";
            var person = new Person { Name = "John", Age = 30 };

            var result = _templateEngine.Render(template, person);

            Assert.AreEqual("Name: John, Age: 30", result);
        }
        

        [Test]
        public void Render_WithCollection_ShouldRenderCollectionCorrectly()
        {
            var template = "Names: {items}";
            var people = new List<Person>
            {
                new Person { Name = "John", Age = 30 },
                new Person { Name = "Alice", Age = 25 }
            };

            var itemTemplate = "<li>{name}</li>";
            var result = _templateEngine.Render(template, people, itemTemplate);

            Assert.AreEqual("Names: <li>John</li><li>Alice</li>", result);
        }

        [Test]
        public void Render_WithNullTemplate_ShouldThrowArgumentNullException()
        {
            string template = null;
            string placeholder = "placeholder";
            string data = "data";
            
            try
            {
                _templateEngine.Render(template, placeholder, data);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("template", ex.ParamName);
            }
        }


        [Test]
        public void Render_WithNullPlaceholder_ShouldThrowArgumentNullException()
        {
            // Arrange
            string template = "Hello, {name}!";
            string placeholder = null;
            string data = "data";

            // Act & Assert
            try
            {
                _templateEngine.Render(template, placeholder, data);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("placeholder", ex.ParamName);
            }
        }

        [Test]
        public void Render_WithNullData_ShouldThrowArgumentNullException()
        {
            // Arrange
            string template = "Hello, {name}!";
            string placeholder = "{name}";
            string data = null;

            // Act & Assert
            try
            {
                _templateEngine.Render(template, placeholder, data);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("data", ex.ParamName);
            }
        }

        [Test]
        public void Render_WithNullObject_ShouldThrowArgumentNullException()
        {
            // Arrange
            string template = "Name: {name}, Age: {age}";
            Person person = null;

            // Act & Assert
            try
            {
                _templateEngine.Render(template, person);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("obj", ex.ParamName);
            }
        }

        [Test]
        public void Render_WithNullCollection_ShouldThrowArgumentNullException()
        {
            // Arrange
            string template = "Names: {names}";
            IEnumerable<object> collection = null;
            string itemTemplate = "{name}";

            // Act & Assert
            try
            {
                _templateEngine.Render(template, collection, itemTemplate);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("collection", ex.ParamName);
            }
        }
        
        [Test]
        public void Render_WithoutPlaceholder_ShouldReturnOriginalTemplate()
        {
            // Arrange
            string template = "Hello, world!";
            string placeholder = "{name}";
            string data = "John Doe";

            // Act
            string result = _templateEngine.Render(template, placeholder, data);

            // Assert
            Assert.AreEqual("Hello, world!", result);
        }

        [Test]
        public void Render_WithMultipleSamePlaceholders_ShouldReplaceAllPlaceholders()
        {
            // Arrange
            string template = "Hello, {name}. Your email is {name}.";
            string placeholder = "{name}";
            string data = "John Doe";

            // Act
            string result = _templateEngine.Render(template, placeholder, data);

            // Assert
            Assert.AreEqual("Hello, John Doe. Your email is John Doe.", result);
        }



    }
}
