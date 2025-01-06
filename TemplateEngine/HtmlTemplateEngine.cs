using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TemplateEngine;

public class HtmlTemplateEngine : IHtmlTemplateEngine
{
    /// <summary>
    /// Рендерит шаблон, заменяя указанный плейсхолдер на строку data.
    /// </summary>
    /// <param name="template">Шаблон строки.</param>
    /// <param name="placeholder">Плейсхолдер, который нужно заменить (например, {name}).</param>
    /// <param name="data">Значение, на которое заменяется плейсхолдер.</param>
    /// <returns>Результат рендера строки.</returns>
    public string Render(string template, string placeholder, string data)
    {
        if (template == null) throw new ArgumentNullException(nameof(template));
        if (placeholder == null) throw new ArgumentNullException(nameof(placeholder));
        if (data == null) throw new ArgumentNullException(nameof(data));

        return template.Replace(placeholder, data);
    }

    public string Render(string template, object obj)
    {
        if (template == null) throw new ArgumentNullException(nameof(template));
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        return ReplacePlaceholders(template, obj);
    }

    public string Render<T>(string template, T obj)
    {
        if (template == null) throw new ArgumentNullException(nameof(template));
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        return ReplacePlaceholders(template, obj);
    }

    private string ReplacePlaceholders(string template, object obj)
    {
        var result = template;
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var placeholder = $"{{{property.Name.ToLower()}}}";
            var value = property.GetValue(obj);

            if (value is IEnumerable<object> collection)
            {
                // Рендер коллекции
                var renderedItems = RenderCollection(collection);
                result = result.Replace(placeholder, renderedItems);
            }
            else
            {
                // Простые значения
                result = result.Replace(placeholder, value?.ToString() ?? string.Empty);
            }
        }

        return result;
    }

    private string RenderCollection(IEnumerable<object> collection)
    {
        return string.Join("", collection.Select(item =>
        {
            // Рендер каждого элемента коллекции
            var properties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var itemTemplate = string.Join(" ", properties.Select(p => $"{p.Name.ToLower()}: {p.GetValue(item)}"));
            return $"<li>{itemTemplate}</li>";
        }));
    }
    public string Render(string template, IEnumerable<object> collection, string itemTemplate)
    {
        if (template == null) throw new ArgumentNullException(nameof(template));
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (itemTemplate == null) throw new ArgumentNullException(nameof(itemTemplate));

        var renderedCollection = RenderCollection(collection, itemTemplate);
        return template.Replace("{items}", renderedCollection);
    }
    private string RenderCollection(IEnumerable<object> collection, string itemTemplate)
    {
        return string.Join("", collection.Select(item => ReplacePlaceholders(itemTemplate, item)));
    }

}

