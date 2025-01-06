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

    /// <summary>
    /// Рендерит шаблон, заменяя плейсхолдеры по свойствам объекта.
    /// </summary>
    public string Render(string template, object obj)
    {
        if (template == null) throw new ArgumentNullException(nameof(template));
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        return ReplacePlaceholders(template, obj);
    }

    /// <summary>
    /// Рендерит шаблон для объектов любого типа.
    /// </summary>
    public string Render<T>(string template, T obj)
    {
        if (template == null) throw new ArgumentNullException(nameof(template));
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        return ReplacePlaceholders(template, obj);
    }

    /// <summary>
    /// Замена плейсхолдеров в шаблоне значениями из объекта.
    /// </summary>
    private string ReplacePlaceholders(string template, object obj)
    {
        var result = template;
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var placeholder = $"{{{property.Name.ToLower()}}}";
            var value = property.GetValue(obj);

            if (value is IEnumerable collection && !(value is string))
            {
                // Рендеринг коллекции объектов
                var itemTemplate = ExtractItemTemplate(template, placeholder);
                if (!string.IsNullOrEmpty(itemTemplate))
                {
                    var renderedItems = RenderCollection(collection, itemTemplate);
                    result = result.Replace(placeholder, renderedItems);
                }
            }
            else
            {
                // Простая замена значения
                result = result.Replace(placeholder, value?.ToString() ?? string.Empty);
            }
        }

        return result;
    }

    /// <summary>
    /// Извлекает подшаблон для коллекции из общего шаблона.
    /// </summary>
    private string ExtractItemTemplate(string template, string placeholder)
    {
        var startTag = $"<!-- {placeholder}:start -->";
        var endTag = $"<!-- {placeholder}:end -->";

        var startIndex = template.IndexOf(startTag, StringComparison.Ordinal);
        var endIndex = template.IndexOf(endTag, StringComparison.Ordinal);

        if (startIndex >= 0 && endIndex > startIndex)
        {
            var start = startIndex + startTag.Length;
            return template[start..endIndex].Trim();
        }

        return null;
    }

    /// <summary>
    /// Рендерит коллекцию объектов, используя подшаблон.
    /// </summary>
    private string RenderCollection(IEnumerable collection, string itemTemplate)
    {
        var renderedItems = collection.Cast<object>()
                                      .Select(item => ReplacePlaceholders(itemTemplate, item));
        return string.Join("", renderedItems);
    }

}

