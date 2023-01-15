# Tracer

__Затрагиваемые темы__
- Reflection.
- Многопоточное программирование.
- Сериализация.
- Объектно-ориентированный дизайн.
- Плагины.

Необходимо реализовать измеритель времени выполнения методов.

__Трассировка методов__

Класс должен реализовывать следующий интерфейс:
```
public interface ITracer 
{
    // вызывается в начале замеряемого метода
    void StartTrace();

    // вызывается в конце замеряемого метода
    void StopTrace();

    // получить результаты измерений
    TraceResult GetTraceResult();
}
```

Конкретная структура TraceResult на усмотрение автора, однако публичный интерфейс должен предоставлять доступ только для чтения: свойства должны быть неизменяемыми и использовать неизменяемые типы данных (IReadOnlyList<T>, IReadOnlyDictionary<TKey, TValue> и подобные), также не должно быть публичных методов, изменяющих внутреннее состояние TraceResult.
Tracer должен собирать следующую информацию об измеряемом методе:
- имя метода;
- имя класса с измеряемым методом;
- время выполнения метода.

Также должно подсчитываться общее время выполнения анализируемых методов в одном потоке. Для этого достаточно подсчитать сумму времен "корневых" методов, вызванных из потока.
Результаты трассировки вложенных методов должны быть представлены в соответствующем месте в дереве результатов.
Для замеров времени следует использовать класс Stopwatch.

__Представление результата__
Результат измерений должен быть представлен в трёх форматах: JSON, XML и YAML. При реализации плагинов следует использовать готовые библиотеки для работы с данными форматами. 
При этом класс TraceResult не должен содержать никакого дополнительного кода для сериализации: атрибутов, ненужных конструкторов/полей/свойств, реализаций интерфейсов или наследований. Подобный код, если он нужен, должен содержаться только в проекте для конкретного сериализатора.
Классы для сериализации результата должны иметь общий интерфейс.

```
public interface ITraceResultSerializer
{
    // Опционально: возвращает формат, используемый сериализатором (xml/json/yaml).
    // Может быть удобно для выбора имени файлов (см. ниже).
    string Format { get; }
    void Serialize(TraceResult traceResult, Stream to);
}
```

- Tracer: содержит основной код, тесты и интерфейс для создания плагинов.
  - Tracer.Core: основная часть библиотеки, реализующая измерение и формирование результатов, инфраструктурный код для загрузки и использования плагинов.
  - Tracer.Core.Tests: модульные тесты для основной части библиотеки.
  - Tracer.Serialization.Abstractions: содержит интерфейс ITraceResultSerializerдля использования в плагинах.
  - Tracer.Example: консольное приложение, демонстрирующее общий случай работы библиотеки (в многопоточном режиме при трассировке вложенных методов) и записывающее результат в файлы в соответствии с загруженными плагинами.
- TracerSerialization: содержит проекты с реализацией плагинов для требуемых форматов сериализации и ссылку на Tracer.Serialization.Abstractions из основного решения.
  - TracerSerializerJSON: сериализатор в формат данных Json
  - TracerSerializerYAML: сериализатор в формат данных Yaml
  - TracerSerializerXML: сериализатор в формат данных Xml
  - Tracer.Serialization.Abstractions: данный проект из основного решения нужен для использования интерфейса ITraceResultSerializer из проектов .Json, .Yaml и .Xml.

